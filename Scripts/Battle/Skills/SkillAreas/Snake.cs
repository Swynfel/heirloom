using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat.SkillAreas {
    public class Snake : SkillAreaCreator {
        [Save] [Export] public bool blockedByPieces = false;

        public Snake() : base() { }
        public Snake(int minRange = 1, int maxRange = 3, Constraint constraint = Constraint.SQUARE, bool blockedByPieces = false) : base(minRange, maxRange, constraint) {
            this.blockedByPieces = blockedByPieces;
        }

        public Snake(Snake other) : base(other) {
            blockedByPieces = other.blockedByPieces;
        }

        public override SkillAreaCreator Clone() {
            return new Snake(this);
        }

        public override void Start(Piece launcher) {
            base.Start(launcher);
            TileFlow first = new TileFlow(launcher.on);
            area.Add(first);
            first.UpdateDisplay(2, IsValid() ? Tile.TileColor.VALID : Tile.TileColor.ERROR);
        }

        private int currentLength { get => area.Count - 1; }

        public override bool IsValid() {
            return currentLength >= minRange && currentLength <= maxRange;
        }

        private bool CheckTileOrCut(Tile tile) {
            int count = area.Count;
            for (int i = 0 ; i < count - 1 ; i++) {
                TileFlow flow = area[i];
                if (flow.tile == tile) {
                    flow = flow.WithDirection(Direction.NONE);
                    area[i] = flow;
                    for (int k = i + 1 ; k < count ; k++) {
                        area[k].tile.ResetDisplay();
                    }
                    area.RemoveRange(i + 1, count - i - 1);
                    flow.UpdateDisplay(2, IsValid() ? Tile.TileColor.VALID : Tile.TileColor.ERROR);
                    return false;
                }
            }
            return true;
        }

        private bool FreeTile(Tile tile) {
            if (!CanStepOn(tile)) {
                return false;
            }
            if (area.AllTiles().Contains(tile)) {
                return false;
            }
            return true;
        }

        private bool CanStepOn(Tile tile) {
            return !blockedByPieces || tile.pieces.Count == 0;
        }

        public override void Hover(Tile tile) {
            TileFlow prev = area.Last();
            if (CheckTileOrCut(tile) && CanStepOn(tile)) {
                // Check if can be appended
                List<TileFlow> extra = BoardUtils.ShortestPath(prev.tile, tile, maxRange - currentLength, FreeTile);
                if (extra != null) {
                    // Success, remove the head
                    area.Remove(prev);
                } else {
                    // Check if can be replaced
                    extra = BoardUtils.ShortestPath(area.First().tile, tile, maxRange, CanStepOn);
                    if (extra != null) {
                        // Success, remove everything
                        foreach (TileFlow flow in area) {
                            flow.tile.ResetDisplay();
                        }
                        area.Clear();
                    } else {
                        // Nothing worked
                        return;
                    }
                }
                // Add the extra
                area.AddRange(extra);
                foreach (TileFlow flow in extra) {
                    var color = flow.tile != tile ? Tile.TileColor.SECONDARY : IsValid() ? Tile.TileColor.VALID : Tile.TileColor.ERROR;
                    flow.UpdateDisplay(flow.tile == tile ? 2 : 1, color);
                }
            }
        }

        public override void Key(Direction direction) {
            TileFlow prev = area.Last();
            Tile tile = prev.tile.GetNeighbor(direction);
            if (tile != null && CheckTileOrCut(tile) && CanStepOn(tile)) {
                if (currentLength < maxRange) {
                    TileFlow flow = new TileFlow(tile);
                    area.Add(flow);
                    flow.UpdateDisplay(2, IsValid() ? Tile.TileColor.VALID : Tile.TileColor.ERROR);
                    area[area.IndexOf(prev)] = prev.WithDirection(direction);
                    prev.UpdateDisplay(1, Tile.TileColor.SECONDARY);
                }
            }
        }

        public override void Undo() {
            if (currentLength == 0) {
                ForceCancel();
                return;
            }
            ClearDisplay();
            area.RemoveRange(1, area.Count - 1);
            area[0] = new TileFlow(area[0].tile);
            area[0].UpdateDisplay(2, IsValid() ? Tile.TileColor.VALID : Tile.TileColor.ERROR);
        }
    }
}
