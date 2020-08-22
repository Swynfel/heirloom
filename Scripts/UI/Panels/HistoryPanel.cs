using System;
using Godot;

namespace UI {
    public class HistoryPanel : ScrollContainer {
        public override void _Ready() {
            Refresh();
        }

        private void Refresh() {
            Control container = GetNode<Control>("Seasons");
            foreach (SeasonHistory season in Game.data.history.past) {
                container.AddChild(SeasonHistoryTable.Create(season));
                container.AddChild(new HSeparator());
            }
            History.NextYear();
            container.AddChild(SeasonHistoryTable.Create(Game.data.history.now));
            ScrollVertical = (int) GetVScrollbar().MaxValue;
        }
    }
}