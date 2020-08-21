using System;
using Godot;

namespace UI {
    public class QuestPanel : VBoxContainer {

        private VBoxContainer questList;
        private QuestTable questTable;
        private PartyPanel party;
        public override void _Ready() {
            GetNode("TopLine/Cancel").Connect("pressed", this, nameof(OpenQuest), Global.ArrayFrom(-1));
            questList = GetNode<VBoxContainer>("QuestSelection/Selection/List");
            questTable = GetNode<QuestTable>("QuestSelection/QuestTable");
            party = GetNode<PartyPanel>("Party");
            Refresh();
        }

        private void Refresh() {
            questList.QueueFreeChildren();
            int i = 0;
            foreach (Quest quest in Game.data.quests) {
                Button b = new Button();
                b.Text = quest.name;
                b.Connect("pressed", this, nameof(OpenQuest), Global.ArrayFrom(i));
                // b.Connect("focus_entered", this, nameof(OpenQuest), Global.ArrayFrom(i));
                questList.AddChild(b);
                i++;
            }
            OpenQuest(-1);
        }

        private void OpenQuest(int i) {
            if (i >= 0) {
                questTable.SetQuest(Game.data.quests[i]);
                questTable.Modulate = Colors.White;
                party.Show();
                party.SetMax(Game.data.quests[i].partySize);
            } else {
                questTable.Modulate = Colors.Transparent;
                party.Hide();
            }
        }
    }
}