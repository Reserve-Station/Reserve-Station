// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Construction.Prototypes;
using Content.Shared.Construction;
using Content.Shared.Construction.NodeEntities;
using Content.Shared.Construction.Steps;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.EntityEffects;
using Content.Shared.EntityEffects.Effects;
using Content.Shared.Kitchen;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Markdown.Value;
using System.Linq;

namespace Content.Client.Guidebook;

public enum FoodEntitySourceKind
{
    MixingReaction,
    SliceFrom,
    RollFrom,
    Hydroponics,
}

public readonly record struct FoodEntitySource(
    FoodEntitySourceKind Kind,
    ReactionPrototype? Reaction,
    EntProtoId? SourceEntity,
    string? SeedId);

public sealed class FoodGuideDataSystem : EntitySystem
{
    private const string RollingToolQuality = "Rolling";

    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;

    private readonly Dictionary<EntProtoId, List<FoodEntitySource>> _sources = new();
    private readonly Dictionary<EntProtoId, List<FoodRecipePrototype>> _microwaveByResult = new();
    private readonly HashSet<EntProtoId> _plantEntities = new();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnPrototypesReloaded);
        Rebuild();
    }

    private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
    {
        if (ev.ByType.ContainsKey(typeof(ReactionPrototype))
            || ev.ByType.ContainsKey(typeof(EntityPrototype))
            || ev.ByType.ContainsKey(typeof(FoodRecipePrototype))
            || ev.ByType.ContainsKey(typeof(ConstructionGraphPrototype)))
            Rebuild();
    }

    public IReadOnlyList<FoodEntitySource> GetSources(EntProtoId entityId)
    {
        return _sources.GetValueOrDefault(entityId) ?? [];
    }

    public IReadOnlyList<FoodEntitySource> GetNonPlantSources(EntProtoId entityId)
    {
        return GetSources(entityId).Where(s => s.Kind != FoodEntitySourceKind.Hydroponics).ToList();
    }

    public IReadOnlyList<FoodRecipePrototype> GetMicrowaveRecipes(EntProtoId result)
    {
        return _microwaveByResult.GetValueOrDefault(result) ?? [];
    }

    public bool IsPlant(EntProtoId entityId) => _plantEntities.Contains(entityId);

    public bool HasObtainSource(EntProtoId entityId)
    {
        if (IsPlant(entityId))
            return false;

        if (GetMicrowaveRecipes(entityId).Count > 0)
            return true;

        foreach (var source in GetNonPlantSources(entityId))
        {
            if (source.Kind is FoodEntitySourceKind.MixingReaction
                or FoodEntitySourceKind.SliceFrom
                or FoodEntitySourceKind.RollFrom)
                return true;
        }

        return false;
    }

    public IEnumerable<EntityPrototype> GetGuideIngredients()
    {
        var ids = new HashSet<EntProtoId>();

        foreach (var recipe in _prototypes.EnumeratePrototypes<FoodRecipePrototype>())
        {
            foreach (var solid in recipe.IngredientsSolids.Keys)
            {
                if (!IsPlant(solid))
                    ids.Add(solid);
            }
        }

        foreach (var id in ids.ToList())
        {
            foreach (var source in GetSources(id))
            {
                if (source.SourceEntity is { } parent && !IsPlant(parent))
                    ids.Add(parent);
            }
        }

        return ids
            .Where(HasObtainSource)
            .Select(id => _prototypes.Index<EntityPrototype>(id))
            .OrderBy(p => p.Name);
    }

    private void Rebuild()
    {
        _sources.Clear();
        _microwaveByResult.Clear();
        _plantEntities.Clear();

        var pendingSlices = new List<(EntProtoId Slice, EntProtoId Parent)>();

        foreach (var entity in _prototypes.EnumeratePrototypes<EntityPrototype>())
        {
            if (entity.Abstract)
                continue;

            if (entity.TryGetComponent<SliceableFoodComponent>(out var sliceable, _componentFactory)
                && sliceable.Slice is { } sliceId)
            {
                pendingSlices.Add((sliceId, entity.ID));
            }

            if (entity.Components.TryGetValue("Produce", out var produceEntry)
                && produceEntry.Mapping.TryGet<ValueDataNode>("seedId", out var seedNode)
                && !string.IsNullOrEmpty(seedNode.Value))
            {
                _plantEntities.Add(entity.ID);
                AddSource(entity.ID, new FoodEntitySource(FoodEntitySourceKind.Hydroponics, null, null, seedNode.Value));
            }
        }

        foreach (var (slice, parent) in pendingSlices)
        {
            AddSource(slice, new FoodEntitySource(FoodEntitySourceKind.SliceFrom, null, parent, null));
            if (_plantEntities.Contains(parent))
                _plantEntities.Add(slice);
        }

        IndexConstructionRollingSources();

        foreach (var reaction in _prototypes.EnumeratePrototypes<ReactionPrototype>())
        {
            foreach (var effect in reaction.Effects)
            {
                if (effect is not CreateEntityReactionEffect createEffect)
                    continue;

                AddSource(createEffect.Entity, new FoodEntitySource(FoodEntitySourceKind.MixingReaction, reaction, null, null));
            }
        }

        foreach (var recipe in _prototypes.EnumeratePrototypes<FoodRecipePrototype>())
        {
            if (!_microwaveByResult.TryGetValue(recipe.Result, out var list))
            {
                list = new List<FoodRecipePrototype>();
                _microwaveByResult[recipe.Result] = list;
            }

            list.Add(recipe);
        }

        foreach (var list in _microwaveByResult.Values)
            list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
    }

    private void IndexConstructionRollingSources()
    {
        foreach (var graph in _prototypes.EnumeratePrototypes<ConstructionGraphPrototype>())
        {
            foreach (var node in graph.Nodes.Values)
            {
                if (!TryGetGraphNodeEntityId(node.Entity, out var sourceEntity))
                    continue;

                foreach (var edge in node.Edges)
                {
                    if (!graph.Nodes.TryGetValue(edge.Target, out var targetNode))
                        continue;

                    if (!TryGetGraphNodeEntityId(targetNode.Entity, out var resultEntity))
                        continue;

                    if (!edge.Steps.Any(step => step is ToolConstructionGraphStep toolStep
                            && toolStep.Tool == RollingToolQuality))
                        continue;

                    AddSource(resultEntity, new FoodEntitySource(FoodEntitySourceKind.RollFrom, null, sourceEntity, null));
                }
            }
        }
    }

    private static bool TryGetGraphNodeEntityId(IGraphNodeEntity entity, out EntProtoId id)
    {
        if (entity is StaticNodeEntity staticEntity && !string.IsNullOrEmpty(staticEntity.Id))
        {
            id = new EntProtoId(staticEntity.Id);
            return true;
        }

        id = default;
        return false;
    }

    private void AddSource(EntProtoId entityId, FoodEntitySource source)
    {
        if (!_sources.TryGetValue(entityId, out var list))
        {
            list = new List<FoodEntitySource>();
            _sources[entityId] = list;
        }

        list.Add(source);
    }
}
