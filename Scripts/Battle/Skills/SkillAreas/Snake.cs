using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat.SkillAreas {
    public class Snake : SkillAreaCreator {
        [Export] public int maxLength;
        [Export] public bool allowEmpty;
        [Export] public bool blockedByPieces;

        public Snake() : this(3, false, false) { }
        public Snake(int maxLength = 3, bool allowEmpty = false, bool blockedByPieces = false) {
            this.maxLength = maxLength;
            this.allowEmpty = allowEmpty;
            this.blockedByPieces = blockedByPieces;
        }

        public override SkillAreaCreator Clone() {
            return new Snake(maxLength, allowEmpty, blockedByPieces);
        }

        public override void Start(Piece launcher) {
            area = new SkillArea();
            TileFlow first = new TileFlow(launcher.on);
            area.Add(first);
            first.UpdateDisplay(2);
        }

        public override bool IsValid() {
            return allowEmpty || area.Count > 1;
        }

        private bool CheckTileOrCut(Tile tile) {
            int count = area.Count;
            for (int i = 0 ; i < count - 1 ; i++) {
                TileFlow flow = area[i];
                if (flow.tile == tile) {
                    flow = flow.WithDirection(Direction.NONE);
                    area[i] = flow;
                    flow.UpdateDisplay(2);
                    for (int k = i + 1 ; k < count ; k++) {
                        area[k].tile.ResetDisplay();
                    }
                    area.RemoveRange(i + 1, count - i - 1);
                    return false;
                }
            }
            return true;
        }

        public bool FreeTile(Tile tile) {
            if (!CanStepOn(tile)) {
                return false;
            }
            if (area.AllTiles().Contains(tile)) {
                return false;
            }
            return true;
        }

        public bool CanStepOn(Tile tile) {
            return !blockedByPieces || tile.pieces.Count == 0;
        }

        public override void Hover(Tile tile) {
            TileFlow prev = area.Last();
            if (CheckTileOrCut(tile) && CanStepOn(tile)) {
                // Check if can be appended
                List<TileFlow> extra = BoardUtils.ShortestPath(prev.tile, tile, maxLength - (area.Count - 1), FreeTile);
                if (extra != null) {
                    // Success, remove the head
                    area.Remove(prev);
                } else {
                    // Check if can be replaced
                    extra = BoardUtils.ShortestPath(area.First().tile, tile, maxLength, CanStepOn);
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
                    flow.UpdateDisplay(flow.tile == tile ? 2 : 1);
                }
            }
        }

        public override void Key(Direction direction) {
            TileFlow prev = area.Last();
            Tile tile = prev.tile.GetNeighbor(direction);
            if (tile != null && CheckTileOrCut(tile) && CanStepOn(tile)) {
                if (area.Count <= maxLength) {
                    TileFlow flow = new TileFlow(tile);
                    area.Add(flow);
                    flow.UpdateDisplay(2);
                    area[area.IndexOf(prev)] = prev.WithDirection(direction);
                    prev.UpdateDisplay(1);
                }
            }
        }

        public override void Undo() {
            ClearDisplay();
            area.RemoveRange(1, area.Count - 1);
            area[0] = new TileFlow(area[0].tile);
            area[0].UpdateDisplay(2);
        }
    }
}
