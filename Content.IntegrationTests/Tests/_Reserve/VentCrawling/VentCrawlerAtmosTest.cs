// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Goobstation.Maths.FixedPoint;
using Content.IntegrationTests.Tests.Atmos;
using Content.Server.Atmos.Components;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Shared._Starlight.VentCrawling;
using Content.Shared._Starlight.VentCrawling.Components;
using Content.Shared.Atmos;
using Content.Shared.Damage.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.IntegrationTests.Tests._Reserve.VentCrawling;

/// <summary>
/// Checks that vent crawlers don't suffocate in thin pipes, but still feel vacuum and high pressure.
/// </summary>
[TestOf(typeof(SharedVentCrawableSystem))]
public sealed class VentCrawlerAtmosTest : AtmosTest
{
    protected override ResPath? TestMapPath => new("Maps/Test/Atmospherics/DeltaPressure/deltapressuretest.yml");

    [Test]
    [TestCase(10f, TestName = "ThinPipe")]
    [TestCase(Atmospherics.OneAtmosphere, TestName = "NormalPipe")]
    public async Task CrawlerSurvivesInAiredPipe(float pressure)
    {
        var (mouse, pipeAir) = await CrawlIntoPipe(pressure);

        await Server.WaitAssertion(() =>
        {
            var felt = SAtmos.GetContainingMixture(mouse)!;
            Assert.That(felt.Pressure, Is.EqualTo(Atmospherics.OneAtmosphere).Within(0.1f));
            Assert.That(felt.Temperature, Is.EqualTo(pipeAir.Temperature));
        });

        await RunSeconds(20);

        await Server.WaitAssertion(() =>
        {
            Assert.That(SEntMan.HasComponent<BeingVentCrawlerComponent>(mouse), "Crawler left the pipe");
            var damage = SEntMan.GetComponent<DamageableComponent>(mouse).Damage;
            Assert.That(damage.GetTotal(), Is.EqualTo(FixedPoint2.Zero), $"Crawler took damage: {string.Join(", ", damage.DamageDict)}");
            Assert.That(SEntMan.GetComponent<BarotraumaComponent>(mouse).TakingDamage, Is.False);
        });
    }

    [Test]
    public async Task PressurizedPipeHurts()
    {
        var (mouse, pipeAir) = await CrawlIntoPipe(4500f);

        await RunSeconds(5);

        await Server.WaitAssertion(() =>
        {
            Assert.That(SAtmos.GetContainingMixture(mouse), Is.SameAs(pipeAir));
            Assert.That(SEntMan.GetComponent<BarotraumaComponent>(mouse).TakingDamage);
        });
    }

    [Test]
    public async Task EmptyPipeIsVacuum()
    {
        var (mouse, _) = await CrawlIntoPipe(0f);

        await RunSeconds(5);

        await Server.WaitAssertion(() =>
        {
            Assert.That(SAtmos.GetContainingMixture(mouse)!.Pressure, Is.Zero);
            Assert.That(SEntMan.GetComponent<BarotraumaComponent>(mouse).TakingDamage);
        });
    }

    /// <summary>
    /// Fills a vent + pipe net with air at the given pressure and crawls a mouse from the vent into the pipe.
    /// </summary>
    private async Task<(EntityUid Mouse, GasMixture PipeAir)> CrawlIntoPipe(float pressure)
    {
        var grid = SEntMan.GetNetEntity(MapData.Grid.Owner);
        var vent = SEntMan.GetEntity(await Spawn("GasVentPump", new NetCoordinates(grid, new Vector2(0.5f, 0.5f))));
        var pipeUid = SEntMan.GetEntity(await Spawn("GasPipeStraight", new NetCoordinates(grid, new Vector2(0.5f, -0.5f))));
        await RunTicks(5);

        EntityUid mouse = default;
        GasMixture pipeAir = default!;
        VentCrawlerHolderComponent holder = default!;
        await Server.WaitAssertion(() =>
        {
            Assert.That(SEntMan.System<NodeContainerSystem>().TryGetNode<PipeNode>(pipeUid, "pipe", out var pipe));
            pipeAir = pipe!.Air;
            pipeAir.Temperature = Atmospherics.T20C;
            var moles = pressure * pipeAir.Volume / (Atmospherics.R * pipeAir.Temperature);
            pipeAir.AdjustMoles(Gas.Oxygen, moles * Atmospherics.OxygenStandard);
            pipeAir.AdjustMoles(Gas.Nitrogen, moles * Atmospherics.NitrogenStandard);

            // The test map is a vacuum, so the mouse goes straight into the vent.
            var coords = SEntMan.GetComponent<TransformComponent>(vent).Coordinates;
            mouse = SEntMan.SpawnAtPosition("MobMouse", coords);
            var crawl = SEntMan.System<SharedVentCrawableSystem>();
            var holderUid = SEntMan.SpawnAtPosition("VentCrawlerHolder", coords);
            holder = SEntMan.GetComponent<VentCrawlerHolderComponent>(holderUid);
            Assert.That(crawl.TryInsert(holderUid, mouse));
            Assert.That(crawl.EnterTube(holderUid, vent));
            holder.CurrentDirection = Direction.South;
            holder.IsMoving = true;
        });

        // Stop as soon as we reach the pipe, otherwise we crawl out of its open end.
        for (var i = 0; i < 60 && holder.CurrentTube != pipeUid; i++)
            await RunTicks(1);

        await Server.WaitPost(() => holder.IsMoving = false);
        Assert.That(holder.CurrentTube, Is.EqualTo(pipeUid), "Crawler did not move into the pipe");

        return (mouse, pipeAir);
    }
}
