// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._White.CustomGhostSystem;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests._White
{
    [TestFixture]
    public sealed class CustomGhostDefaultTest
    {
        [Test]
        public async Task CustomGhostDefaultPrototypePresent()
        {
            await using var pair = await PoolManager.GetServerClient();
            var server = pair.Server;
            var prototypeManager = server.ResolveDependency<IPrototypeManager>();
            var defaultVal = "default";  // Reserve edit: Fix warnings
            Assert.That(prototypeManager.HasIndex<CustomGhostPrototype>(defaultVal));  // Reserve edit: Fix warnings
            await pair.CleanReturnAsync();
        }
    }
}
