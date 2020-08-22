using System;
using Godot;

namespace UI {
    public class InventoryPanel : ScrollContainer {
        GridContainer container;
        public override void _Ready() {
            container = GetNode<GridContainer>("Grid");
            Connect("visibility_changed", this, nameof(Refresh));
            Refresh();
        }

        private void Refresh() {
            container.QueueFreeChildren();
            foreach (Item item in Game.data.inventory.items) {
                container.AddChild(Visual.Tables.ItemTable.Create(item));
            }
        }
    }
}