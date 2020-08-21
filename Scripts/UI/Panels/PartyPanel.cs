using System;
using System.Collections.Generic;
using Godot;
using Visual.Icons;

namespace UI {
    public class PartyPanel : VBoxContainer {

        private int partyMax = 0;
        private PartyCharacter[] buttons;
        private Entity[] members;
        private bool[] selected;

        private Control list;
        private Label label;

        public override void _Ready() {
            list = GetNode<Control>("Characters/List");
            label = GetNode<Label>("PartyCount/Label");
            Refresh();
        }
        public void Refresh() {
            list.QueueFreeChildren();
            int i = 0;
            int count = Game.data.family.members.Count;
            selected = new bool[count];
            buttons = new PartyCharacter[count];
            foreach (Entity member in Game.data.family.members) {
                PartyCharacter character = PartyCharacter.Create(member);
                character.Connect("pressed", this, nameof(on_ToggleMember), Global.ArrayFrom(i));
                list.AddChild(character);
                buttons[i] = character;
                members[i] = member;
                i++;
            }
        }

        public void LabelRefresh() {
            label.Text = string.Format("{0} / {1}", CurrentSize(), partyMax);
        }

        public int CurrentSize() {
            int i = 0;
            foreach (bool b in selected) {
                if (b) i++;
            }
            return i;
        }
        public void SetMax(int partyMax) {
            this.partyMax = partyMax;
            if (CurrentSize() > partyMax) {
                int left = CurrentSize() - partyMax;
                int k = selected.Length - 1;
                while (left > 0) {
                    if (selected[k]) {
                        left--;
                        on_ToggleMember(k);
                    }
                    k--;
                }
            }
            LabelRefresh();
        }

        public void on_ToggleMember(int i) {
            bool toggle = selected[i];
            if (!toggle && CurrentSize() == partyMax) {
                // TODO: Shake number?
                return;
            }
            selected[i] = !toggle;
            buttons[i].Toggle(selected[i]);
            LabelRefresh();
        }

        public IEnumerable<Entity> GetParty() {
            for (int i = 0 ; i < selected.Length ; i++) {
                if (selected[i]) {
                    yield return members[i];
                }
            }
        }
    }
}