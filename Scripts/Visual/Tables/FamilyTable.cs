using System;
using Godot;

namespace Visual.Tables {
    public class FamilyTable : ScrollContainer {
        HBoxContainer container;
        public override void _Ready() {
            container = GetNode<HBoxContainer>("Table");
            Connect("visibility_changed", this, nameof(Refresh));
            Refresh();
        }

        private void Refresh() {
            container.QueueFreeChildren();
            foreach (Entity entity in Game.data.family.alive) {
                container.AddChild(CharacterTable.Create(entity));
            }
        }
    }
}