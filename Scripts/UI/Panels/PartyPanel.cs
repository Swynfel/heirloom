using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Visual.Tables;

namespace UI {
    public class PartyPanel : VBoxContainer {

        private int partyMax = 0;
        private Dictionary<CharacterEntity, PartyCharacter> buttons = new Dictionary<CharacterEntity, PartyCharacter>();
        private Control list;
        private Label label;

        public override void _Ready() {
            list = GetNode<Control>("Characters/List");
            label = GetNode<Label>("PartyCount/Label");
            Refresh();
        }
        public void Refresh() {
            list.QueueFreeChildren();
            buttons.Clear();
            foreach (CharacterEntity member in Family.familyMembers.Where(e => e.ageGroup >= Date.AgeGroup.TEEN)) {
                PartyCharacter character = PartyCharacter.CreateParty(member);
                list.AddChild(character);
                character.Connect("pressed", this, nameof(on_ToggleMember), Global.ArrayFrom(member));
                buttons[member] = character;
            }
        }

        public void LabelRefresh() {
            label.Text = string.Format("{0} / {1}", CurrentSize(), partyMax);
        }

        public int CurrentSize() {
            return Village.actions.Count(VillageAction.QUEST);
        }
        public void SetMax(int partyMax) {
            this.partyMax = partyMax;
            if (CurrentSize() > partyMax) {
                foreach (CharacterEntity member in (Family.familyMembers as IEnumerable<CharacterEntity>).Reverse()) {
                    if (Village.actions.EqualsOrFalse(member, VillageAction.QUEST)) {
                        Village.actions[member] = VillageAction.REST;
                        if (CurrentSize() <= partyMax) {
                            break;
                        }
                    }
                }
            }
            LabelRefresh();
        }

        public void on_ToggleMember(CharacterEntity entity) {
            VillageAction a;
            bool questing = Village.actions.TryGetValue(entity, out a) && a == VillageAction.QUEST;
            if (questing) {
                Village.actions[entity] = VillageAction.REST;
            } else {
                if (CurrentSize() == partyMax) {
                    // TODO: Shake number?
                    return;
                }
                Village.actions[entity] = VillageAction.QUEST;
            }
            buttons[entity].Toggle(!questing);
            LabelRefresh();
        }
    }
}