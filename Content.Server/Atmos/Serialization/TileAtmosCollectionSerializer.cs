// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Globalization;
using System.Linq;
using Content.Shared.Atmos;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Content.Server.Atmos.Serialization;

public sealed partial class TileAtmosCollectionSerializer : ITypeSerializer<Dictionary<Vector2i, TileAtmosphere>, MappingDataNode>, ITypeCopier<Dictionary<Vector2i, TileAtmosphere>>
{
    public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        return serializationManager.ValidateNode<TileAtmosData>(node, context);
    }

    public Dictionary<Vector2i, TileAtmosphere> Read(ISerializationManager serializationManager, MappingDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx, ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<Dictionary<Vector2i, TileAtmosphere>>? instanceProvider = null)
    {
        node.TryGetValue("version", out var versionNode);
        var version = ((ValueDataNode?) versionNode)?.AsInt() ?? 1;
        Dictionary<Vector2i, TileAtmosphere> tiles = new();

        // Backwards compatability
        if (version == 1)
        {
            var tile2 = node["tiles"];

            var mixies = serializationManager.Read<Dictionary<Vector2i, int>?>(tile2, hookCtx, context);
            var unique = serializationManager.Read<List<GasMixture>?>(node["uniqueMixes"], hookCtx, context);

            if (unique != null && mixies != null)
            {
                foreach (var (indices, mix) in mixies)
                {
                    try
                    {
                        tiles.Add(indices, new TileAtmosphere(EntityUid.Invalid, indices,
                            unique[mix].Clone()));
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Logger.Error(
                            $"Error during atmos serialization! Tile at {indices} points to an unique mix ({mix}) out of range!");
                    }
                }
            }
        }
        else
        {
            var dataNode = (MappingDataNode) node["data"];
            var chunkSize = serializationManager.Read<int>(dataNode["chunkSize"], hookCtx, context);

            dataNode.TryGet("uniqueMixes", out var mixNode);
            var unique = mixNode == null ? null : serializationManager.Read<List<GasMixture>?>(mixNode, hookCtx, context);

            if (unique != null)
            {
                var tileNode = (MappingDataNode) dataNode["tiles"];
                foreach (var (chunkNode, valueNode) in tileNode)
                {
                    var chunkOrigin = serializationManager.Read<Vector2i>(tileNode.GetKeyNode(chunkNode), hookCtx, context);
                    var chunk = serializationManager.Read<TileAtmosChunk>(valueNode, hookCtx, context);

                    foreach (var (mix, data) in chunk.Data)
                    {
                        for (var x = 0; x < chunkSize; x++)
                        {
                            for (var y = 0; y < chunkSize; y++)
                            {
                                var flag = data & (uint) (1 << (x + y * chunkSize));

                                if (flag == 0)
                                    continue;

                                var indices = new Vector2i(x + chunkOrigin.X * chunkSize,
                                    y + chunkOrigin.Y * chunkSize);

                                try
                                {
                                    tiles.Add(indices, new TileAtmosphere(EntityUid.Invalid, indices,
                                        unique[mix].Clone()));
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    Logger.Error(
                                        $"Error during atmos serialization! Tile at {indices} points to an unique mix ({mix}) out of range!");
                                }
                            }
                        }
                    }
                }
            }
        }

        return tiles;
    }

    // Reserve edit start
    private sealed class GasMixtureComparer : IComparer<GasMixture>, IEqualityComparer<GasMixture>
    {
        public static readonly GasMixtureComparer Instance = new();

        public int Compare(GasMixture? x, GasMixture? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            var cmp = x.Volume.CompareTo(y.Volume);
            if (cmp != 0) return cmp;

            cmp = x.Temperature.CompareTo(y.Temperature);
            if (cmp != 0) return cmp;

            cmp = x.Immutable.CompareTo(y.Immutable);
            if (cmp != 0) return cmp;

            // ReactionResults is intentionally ignored because it is not serialized.
            for (var i = 0; i < Atmospherics.TotalNumberOfGases; i++)
            {
                cmp = x.GetMoles(i).CompareTo(y.GetMoles(i));
                if (cmp != 0) return cmp;
            }

            return 0;
        }

        public bool Equals(GasMixture? x, GasMixture? y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(GasMixture? obj)
        {
            if (obj == null)
                return 0;

            var hashCode = new HashCode();
            // ReactionResults is intentionally ignored because it is not serialized.
            for (var i = 0; i < Atmospherics.TotalNumberOfGases; i++)
            {
                hashCode.Add(obj.GetMoles(i));
            }
            hashCode.Add(obj.Temperature);
            hashCode.Add(obj.Immutable);
            hashCode.Add(obj.Volume);
            return hashCode.ToHashCode();
        }
    }
    // Reserve edit end

    public DataNode Write(ISerializationManager serializationManager, Dictionary<Vector2i, TileAtmosphere> value, IDependencyCollection dependencies,
        bool alwaysWrite = false, ISerializationContext? context = null)
    {
        // Reserve edit start
        var mixIndices = new Dictionary<GasMixture, int>(GasMixtureComparer.Instance);
        var rawMixes = new List<GasMixture>();
        foreach (var (_, tile) in value)
        {
            if (tile.Air == null) continue;

            if (mixIndices.TryAdd(tile.Air, 0))
            {
                rawMixes.Add(tile.Air);
            }
        }

        rawMixes.Sort(GasMixtureComparer.Instance);

        for (var i = 0; i < rawMixes.Count; i++)
        {
            mixIndices[rawMixes[i]] = i;
        }

        var tileChunks = new Dictionary<Vector2i, TileAtmosChunk>();
        var chunkSize = 4;

        foreach (var (gridIndices, tile) in value)
        {
            if (tile.Air == null) continue;

            var mixIndex = mixIndices[tile.Air];

            var chunkOrigin = SharedMapSystem.GetChunkIndices(gridIndices, chunkSize);
            var tileChunk = tileChunks.GetOrNew(chunkOrigin);
            var indices = SharedMapSystem.GetChunkRelative(gridIndices, chunkSize);

            var mixFlag = tileChunk.Data.GetOrNew(mixIndex);
            mixFlag |= (uint) 1 << (indices.X + indices.Y * chunkSize);
            tileChunk.Data[mixIndex] = mixFlag;
        }

        var uniqueMixes = rawMixes.Count == 0 ? null : rawMixes;

        var dataNode = (MappingDataNode) serializationManager.WriteValue(new TileAtmosData
        {
            ChunkSize = chunkSize,
            UniqueMixes = uniqueMixes,
        }, alwaysWrite, context);

        if (tileChunks.Count > 0)
        {
            var tilesNode = new MappingDataNode(tileChunks.Count);
            foreach (var (chunkKey, chunk) in tileChunks.OrderBy(kv => kv.Key.Y).ThenBy(kv => kv.Key.X))
            {
                var chunkNode = new MappingDataNode(chunk.Data.Count);
                foreach (var (mixKey, mixVal) in chunk.Data.OrderBy(kv => kv.Key))
                {
                    chunkNode.Add(mixKey.ToString(CultureInfo.InvariantCulture), mixVal.ToString(CultureInfo.InvariantCulture));
                }
                tilesNode.Add($"{chunkKey.X.ToString(CultureInfo.InvariantCulture)},{chunkKey.Y.ToString(CultureInfo.InvariantCulture)}", chunkNode);
            }

            dataNode["tiles"] = tilesNode;
        }

        var map = new MappingDataNode
        {
            { "version", 2.ToString(CultureInfo.InvariantCulture) },
            { "data", dataNode }
        };

        return map;
        // Reserve edit end
    }

    [DataDefinition]
    private partial struct TileAtmosData
    {
        [DataField("chunkSize")] public int ChunkSize;

        [DataField("uniqueMixes")] public List<GasMixture>? UniqueMixes;

        [DataField("tiles")] public Dictionary<Vector2i, TileAtmosChunk>? TilesUniqueMixes;
    }

    [DataDefinition]
    private partial record struct TileAtmosChunk()
    {
        /// <summary>
        /// Key is unique mix and value is bitflag of the affected tiles.
        /// </summary>
        [IncludeDataField(customTypeSerializer: typeof(DictionarySerializer<int, uint>))]
        public Dictionary<int, uint> Data = new();
    }

    public void CopyTo(ISerializationManager serializationManager, Dictionary<Vector2i, TileAtmosphere> source, ref Dictionary<Vector2i, TileAtmosphere> target,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null)
    {
        target.Clear();
        foreach (var (key, val) in source)
        {
            target.Add(key, new TileAtmosphere(val));
        }
    }
}