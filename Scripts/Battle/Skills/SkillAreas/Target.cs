using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat.SkillAreas {
    public class Target : SkillAreaCreator {
        public Target() : base(maxRange: 4) { }
        public Target(int minRange = 1, int maxRange = 4, Constraint constraint = Constraint.SQUARE) : base(minRange, maxRange, constraint) { }
        public Target(Target other) : base(other) { }
        public override SkillAreaCreator Clone() {
            return new Target(this);
        }

        public override void Start(Piece launcher) {
            base.Start(launcher);
            SetArea(launcher.on);
        }

        private void SetArea(Tile tile) {
            if (area.Count > 0) {
                TileFlow single = area[0];
                area.Clear();
                single.tile.ResetDisplay();
            }
            TileFlow flow = new TileFlow(tile);
            area.Add(flow);
            flow.UpdateDisplay(CanTarget(tile) ? 2 : 1);
        }

        public override bool IsValid() {
            return CanTarget(area[0].tile);
        }

        public override void Hover(Tile tile) {
            if (CanSelect(tile)) {
                SetArea(tile);
            }
        }

        public override void Key(Direction direction) {
            Tile tile = area.Last().tile.GetNeighbor(direction);
            if (CanSelect(tile)) {
                SetArea(tile);
            }
        }

        public override void Undo() {
            if (area[0].tile == launcher.on) {
                ForceCancel();
                return;
            }
            SetArea(launcher.on);
        }
    }
}
