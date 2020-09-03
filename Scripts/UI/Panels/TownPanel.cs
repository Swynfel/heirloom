using System;
using Godot;

namespace UI {
    public class TownPanel : ScrollContainer {
        GridContainer container;
        public override void _Ready() {
            container = GetNode<GridContainer>("Grid");
            Connect("visibility_changed", this, nameof(Refresh));
            container.Connect("resized", this, nameof(on_Resized));
            Refresh();
        }

        private void Refresh() {
            if (Visible) {
                container.QueueFreeChildren();
                foreach (CharacterEntity entity in Family.familyMembers) {
                    container.AddChild(Visual.Tables.JobTable.Create(entity));
                }
            }
        }

        private const int MIN_COLUMN_SIZE = 132;

        private void on_Resized() {
            container.Columns = (int) (container.RectSize.x / MIN_COLUMN_SIZE);
        }
    }
}