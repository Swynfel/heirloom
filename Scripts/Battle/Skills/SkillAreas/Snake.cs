using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat.SkillAreas {
    public class Snake : SkillAreaCreator {
        [Export] public int maxLength;
        [Export] public bool allowEmpty;

        public Snake() : this(3, false) {

        }
        public Snake(int maxLength = 3, bool allowEmpty = false) {
            this.maxLength = maxLength;
            this.allowEmpty = allowEmpty;
        }

        public override void Start(Piece launcher) {
            area = new SkillArea();
            TileFlow first = new TileFlow(launcher.on);
            area.tileflows.Add(first);
            first.UpdateDisplay(2);
        }

        public override bool IsValid() {
            return allowEmpty || area.tileflows.Count > 1;
        }

        private bool CheckTileOrCut(Tile tile) {
            int count = area.tileflows.Count;
            for (int i = 0 ; i < count - 1 ; i++) {
                TileFlow flow = area.tileflows[i];
                if (flow.tile == tile) {
                    flow.direction = Direction.NONE;
                    flow.UpdateDisplay(2);
                    for (int k = i + 1 ; k < count ; k++) {
                        area.tileflows[k].tile.ResetDisplay();
                    }
                    area.tileflows.RemoveRange(i + 1, count - i);
                    return false;
                }
            }
            return true;
        }
        public override void Hover(Tile tile) {
            TileFlow prev = area.tileflows.Last();
            Direction direction = Direction.NONE; //TODO: Get actual direction
                                                  // TODO: Use shortest path algorithm
            if (CheckTileOrCut(tile)) {
                if (area.tileflows.Count <= maxLength) {
                    TileFlow flow = new TileFlow(tile);
                    area.tileflows.Add(flow);
                    flow.UpdateDisplay(2);
                    prev.direction = direction;
                    prev.UpdateDisplay(1);
                }
            }
        }

        public override void Key(Direction direction) {
            TileFlow prev = area.tileflows.Last();
            Tile tile = prev.tile.GetNeighbor(direction);
            if (tile != null && CheckTileOrCut(tile)) {
                if (area.tileflows.Count <= maxLength) {
                    TileFlow flow = new TileFlow(tile);
                    area.tileflows.Add(flow);
                    flow.UpdateDisplay(2);
                    prev.direction = direction;
                    prev.UpdateDisplay(1);
                }
            }
        }

        public override void Undo() {
            ClearDisplay();
            area.tileflows.RemoveRange(1, area.tileflows.Count - 1);
            area.tileflows.First().direction = Direction.NONE;
            area.tileflows.First().UpdateDisplay(2);
        }
    }
}
