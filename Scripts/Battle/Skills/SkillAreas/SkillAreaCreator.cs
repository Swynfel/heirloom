using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat {
    public enum Constraint {
        SQUARE,
        DIAMOND,
        PLUS,
        CROSS,
    }
    public abstract class SkillAreaCreator : Resource {
        protected SkillArea area;
        [Export] public int minRange;
        [Export] public int maxRange;
        [Export] public Constraint constraint;

        protected Piece launcher;

        public SkillAreaCreator(int minRange = 1, int maxRange = 3, Constraint constraint = Constraint.SQUARE) {
            this.minRange = minRange;
            this.maxRange = maxRange;
            this.constraint = constraint;
        }

        public SkillAreaCreator(SkillAreaCreator skillAreaCreator) : this(skillAreaCreator.minRange, skillAreaCreator.maxRange, skillAreaCreator.constraint) { }

        public abstract SkillAreaCreator Clone();

        public virtual void Start(Piece launcher) {
            area = new SkillArea();
            this.launcher = launcher;
        }

        protected void ClearDisplay() {
            foreach (Tile tile in area.AllTiles()) {
                tile.ResetDisplay();
            }
        }

        protected bool CanSelect(Tile tile) {
            return BoardUtils.DistanceBetween(launcher.on, constraint, tile) <= maxRange;
        }

        protected bool CanTarget(Tile tile) {
            int distance = BoardUtils.DistanceBetween(launcher.on, constraint, tile);
            return distance <= maxRange && distance >= minRange;
        }


        // Override
        public virtual SkillArea Done() {
            if (IsValid()) {
                ClearDisplay();
                return area;
            }
            return null;
        }
        public abstract bool IsValid();
        public virtual void Cancel() {
            ClearDisplay();
        }

        public virtual void Click() {
            if (IsValid()) {
                Global.battleUI.launcherPanel.Launch();
            }
        }
        public abstract void Hover(Tile tile);
        public abstract void Key(Direction direction);
        public abstract void Undo(); //Also left-click

        protected void ForceCancel() {
            Global.battleUI.launcherPanel.Clear();
        }

    }
}