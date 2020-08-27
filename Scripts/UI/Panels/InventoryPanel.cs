using System;
using Godot;

namespace UI {
    public class InventoryPanel : ScrollContainer {
        GridContainer container;
        public override void _Ready() {
            container = GetNode<GridContainer>("Grid");
            Connect("visibility_changed", this, nameof(Refresh));
            container.Connect("resized", this, nameof(on_Resized));
            Refresh();
        }

        private void Refresh() {
            container.QueueFreeChildren();
            foreach (Item item in Game.data.inventory.items) {
                var itemTable = Visual.Tables.ItemTable.New();
                container.AddChild(itemTable);
                itemTable.SetItem(item);
            }
        }

        private const int MIN_COLUMN_SIZE = 132;

        private void on_Resized() {
            container.Columns = (int) Math.Max(1, container.RectSize.x / MIN_COLUMN_SIZE);
        }
    }
}