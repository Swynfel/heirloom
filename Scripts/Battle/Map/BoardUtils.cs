using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat {
    public static class BoardUtils {
        public static int DistanceBetween(Tile a, Tile b) {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        private class ComparablePriorityElement : IComparer<(int, int, TileFlow)> {
            public int Compare((int, int, TileFlow) a, (int, int, TileFlow) b) {
                return a.Item1.CompareTo(b.Item1);
            }
            public static ComparablePriorityElement comparer = new ComparablePriorityElement();
        }
        public static List<TileFlow> ShortestPath(Tile from, Tile to, int maxLength = int.MaxValue, Func<Tile, bool> valid = null) {
            TileFlow toTileFlow = new TileFlow(to);
            if (from == to) {
                return new List<TileFlow> { toTileFlow };
            }
            if (maxLength == 0) {
                return null;
            }
            List<(int, int, TileFlow)> priorityQueue = new List<(int, int, TileFlow)>();
            Dictionary<Tile, TileFlow> encountered = new Dictionary<Tile, TileFlow>();
            priorityQueue.Add((0, 0, toTileFlow));
            encountered.Add(to, toTileFlow);
            while (priorityQueue.Count > 0) {
                var first = priorityQueue[0];
                (int heuristic, int length, TileFlow head) = first;
                if (heuristic > maxLength) {
                    return null;
                }
                priorityQueue.RemoveAt(0);
                foreach (TileFlow neighbor in head.tile.GetNeighborInFlows()) {
                    if (neighbor.tile == from) {
                        List<TileFlow> result = new List<TileFlow>();
                        TileFlow currentFlow = neighbor;
                        Tile currentTile;
                        do {
                            result.Add(currentFlow);
                            currentTile = currentFlow.tile.GetNeighbor(currentFlow.direction);
                            currentFlow = encountered[currentTile];
                        } while (currentTile != to);
                        result.Add(toTileFlow);
                        return result;
                    }
                    if (!encountered.ContainsKey(neighbor.tile) && (valid == null || valid(neighbor.tile))) {
                        encountered.Add(neighbor.tile, neighbor);
                        heuristic = length + 1 + DistanceBetween(from, neighbor.tile);
                        var item = (heuristic, length + 1, neighbor);
                        int index = priorityQueue.BinarySearch(item, ComparablePriorityElement.comparer);
                        if (index < 0) {
                            index = -index - 1;
                        }
                        priorityQueue.Insert(index, item);
                    }
                }
            }
            // Cannot be reached
            return null;
        }
    }
    public struct TileFlow {
        public Tile tile { get; private set; }
        public Direction direction { get; private set; }
        public bool flag { get; private set; }

        public TileFlow(Tile tile, Direction direction = Direction.NONE, bool flag = false) {
            this.tile = tile;
            this.direction = direction;
            this.flag = flag;
        }

        public IEnumerable<PieceFlow> GetPieces() {
            foreach (Piece piece in tile.pieces) {
                yield return new PieceFlow(piece, direction, flag);
            }
        }

        public TileFlow WithDirection(Direction newDirection) {
            return new TileFlow(tile, newDirection, flag);
        }

        public TileFlow WithFlag(bool newFlag) {
            return new TileFlow(tile, direction, newFlag);
        }

        public void UpdateDisplay(int strength = 1) {
            // TODO: Draw arrows
            tile.SelectDisplay(strength);
        }
    }
    public struct PieceFlow {
        public Piece piece;
        public Direction direction;
        public bool flag;

        public PieceFlow(Piece piece, Direction direction = Direction.NONE, bool flag = false) {
            this.piece = piece;
            this.direction = direction;
            this.flag = flag;
        }
    }
}
