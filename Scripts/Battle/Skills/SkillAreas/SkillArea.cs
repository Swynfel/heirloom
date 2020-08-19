using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    public class SkillArea : List<TileFlow> {
        internal IEnumerable<Tile> AllTiles() {
            foreach (TileFlow flow in this) {
                yield return flow.tile;
            }
        }

        internal IEnumerable<Piece> AllPieces() {
            foreach (TileFlow flow in this) {
                foreach (Piece piece in flow.tile.pieces) {
                    yield return piece;
                }
            }
        }
        internal IEnumerable<PieceFlow> AllPieceFlows() {
            foreach (TileFlow flow in this) {
                foreach (PieceFlow pieceFlow in flow.GetPieces()) {
                    yield return pieceFlow;
                }
            }
        }
    }
}