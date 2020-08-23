using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat.SkillAreas {
    public class Cone : SkillAreaCreator {
        public Direction currentDirection;
        public Cone() : base(maxRange: 4) { }
        public Cone(Cone other) : base(other) { }
        public override SkillAreaCreator Clone() {
            return new Cone(this);
        }

        public override void Start(Piece launcher) {
            base.Start(launcher);
        }

        public override bool IsValid() {
            return currentDirection != Direction.NONE;
        }

        private void SetDirection(Direction direction) {
            currentDirection = direction;
            // Clear area
            foreach (Tile oldTile in area.AllTiles()) {
                oldTile.ResetDisplay();
            }
            area.Clear();
            // Set area
            Tile hackTile = launcher.on.GetNeighbor(direction.Opposite());
            foreach (Tile newTile in BoardUtils.AllTiles(t => {
                if (hackTile.DirectionTo(t) != direction) {
                    return false;
                }
                float distance = BoardUtils.DistanceBetween(launcher.on, constraint, t);
                return minRange <= distance && distance <= maxRange;
            })) {
                var flow = new TileFlow(newTile, direction);
                area.Add(flow);
                flow.UpdateDisplay(2);
            }
        }

        public override void Hover(Tile tile) {
            Direction dir = launcher.on.DirectionTo(tile);
            if (dir != Direction.NONE) {
                SetDirection(dir);
            }
        }

        public override void Key(Direction direction) {
            if (direction != Direction.NONE) {
                SetDirection(direction);
            }
        }

        public override void Undo() {
            if (currentDirection == Direction.NONE) {
                ForceCancel();
                return;
            }
            SetDirection(Direction.NONE);
        }
    }
}
