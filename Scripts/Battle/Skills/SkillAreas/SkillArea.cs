using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    class TileFlow {
        public Tile tile;
        public Direction direction;
        public bool flag;

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

        public void UpdateDisplay(int strength = 1) {
            // TODO: Draw arrows
            tile.SelectDisplay(strength);
        }
    }
    struct PieceFlow {
        public Piece piece;
        public Direction direction;
        public bool flag;

        public PieceFlow(Piece piece, Direction direction = Direction.NONE, bool flag = false) {
            this.piece = piece;
            this.direction = direction;
            this.flag = flag;
        }
    }

    public class SkillArea {
        internal List<TileFlow> tileflows = new List<TileFlow>();
        internal IEnumerable<TileFlow> AllFlows() {
            return tileflows;
        }

        internal IEnumerable<Tile> AllTiles() {
            foreach (TileFlow flow in tileflows) {
                yield return flow.tile;
            }
        }

        internal IEnumerable<Piece> AllPieces() {
            foreach (TileFlow flow in tileflows) {
                foreach (Piece piece in flow.tile.pieces) {
                    yield return piece;
                }
            }
        }
        internal IEnumerable<PieceFlow> AllPieceFlows() {
            foreach (TileFlow flow in tileflows) {
                foreach (PieceFlow pieceFlow in flow.GetPieces()) {
                    yield return pieceFlow;
                }
            }
        }
    }
}