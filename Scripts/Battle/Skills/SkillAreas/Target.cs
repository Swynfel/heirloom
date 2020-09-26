using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat.SkillAreas {
    public class Target : SkillAreaCreator {
        [Save] [Export] public Constraint areaConstraint = Constraint.SQUARE;
        [Save] [Export] public int areaRange = 0;
        //[Export] public bool friendlyFire = 0;

        public Tile center;
        public Target() : base(maxRange: 4) { }
        public Target(int minRange = 1, int maxRange = 4, Constraint constraint = Constraint.SQUARE) : base(minRange, maxRange, constraint) { }
        public Target(Target other) : base(other) {
            areaConstraint = other.areaConstraint;
            areaRange = other.areaRange;
        }
        public override SkillAreaCreator Clone() {
            return new Target(this);
        }

        public override void Start(Piece launcher) {
            base.Start(launcher);
            SetCenter(launcher.on);
        }

        private void SetCenter(Tile tile) {
            center = tile;
            // Clear area
            foreach (Tile oldTile in area.AllTiles()) {
                oldTile.ResetDisplay();
            }
            area.Clear();
            // Set area
            bool valid = CanTarget(center);
            foreach (Tile newTile in BoardUtils.AreaOf(center, areaRange, areaConstraint)) {
                var flow = new TileFlow(newTile);
                area.Add(flow);
                flow.UpdateDisplay(1, valid ? Tile.TileColor.SECONDARY : Tile.TileColor.ERROR);
            }
            new TileFlow(center).UpdateDisplay(CanTarget(center) ? 2 : 1, valid ? Tile.TileColor.VALID : Tile.TileColor.ERROR);
        }

        public SkillArea SkillAreaIfTarget(Tile tile) {
            if (CanSelect(tile)) {
                return new SkillArea(BoardUtils.AreaOf(tile, areaRange, areaConstraint).Select(t => new TileFlow(t)));
            } else {
                return null;
            }
        }

        public override bool IsValid() {
            return CanTarget(center);
        }

        public override void Hover(Tile tile) {
            if (CanSelect(tile)) {
                SetCenter(tile);
            }
        }

        public override void Key(Direction direction) {
            Tile tile = center.GetNeighbor(direction);
            if (CanSelect(tile)) {
                SetCenter(tile);
            }
        }

        public override void Undo() {
            if (center == launcher.on) {
                ForceCancel();
                return;
            }
            SetCenter(launcher.on);
        }
    }
}
