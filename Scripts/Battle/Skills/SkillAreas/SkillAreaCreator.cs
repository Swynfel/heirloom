using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat {
    public abstract class SkillAreaCreator : Resource {
        protected SkillArea area;

        public abstract void Start(Piece launcher);

        protected void ClearDisplay() {
            foreach (Tile tile in area.AllTiles()) {
                tile.ResetDisplay();
            }
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

        public virtual void Click(Tile tile) {
            if (IsValid()) {
                // TODO: Trigger Done
            }
        }
        public abstract void Hover(Tile tile);
        public abstract void Key(Direction direction);
        public abstract void Undo(); //Also left-click

    }
}