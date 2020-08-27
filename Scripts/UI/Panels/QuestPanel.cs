using System;
using Godot;

namespace UI {
    public class QuestPanel : VBoxContainer {

        private VBoxContainer questList;
        private Visual.Tables.QuestTable questTable;
        private PartyPanel party;
        public override void _Ready() {
            GetNode("TopLine/Cancel").Connect("pressed", this, nameof(OpenQuest), Global.ArrayFrom(-1));
            questList = GetNode<VBoxContainer>("QuestSelection/Selection/List");
            questTable = GetNode<Visual.Tables.QuestTable>("QuestSelection/QuestTable");
            party = GetNode<PartyPanel>("Party");
            Connect("visibility_changed", this, nameof(Refresh));
            Refresh();
        }

        private void Refresh() {
            if (!Visible) {
                return;
            }
            questList.QueueFreeChildren();
            foreach (Quest quest in Game.data.quests) {
                Button b = new Button();
                b.Text = quest?.name ?? "";
                b.Connect("pressed", this, nameof(OpenQuest), Global.ArrayFrom(quest));
                // b.Connect("focus_entered", this, nameof(OpenQuest), Global.ArrayFrom(i));
                questList.AddChild(b);
            }
            OpenQuest(Village.quest);
        }

        private void OpenQuest(Quest quest) {
            Village.quest = quest;
            if (quest != null) {
                questTable.SetQuest(quest);
                questTable.Modulate = Colors.White;
                party.Show();
                party.SetMax(quest.partySize);
            } else {
                questTable.Modulate = Colors.Transparent;
                party.SetMax(0);
                party.Hide();
            }
        }
    }
}