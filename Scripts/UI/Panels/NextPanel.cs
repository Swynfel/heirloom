using System;
using System.Linq;
using Godot;

namespace UI {
    public class NextPanel : VBoxContainer {
        GridContainer container;

        private Label questName;
        private Control partyContainer;
        private Control partyList;
        private Control warningQuest;
        private Control warningParty;

        private Control idleSeparator;
        private Control idleBox;
        private Control idleList;

        private Label currentFood;
        private Label consumedFood;
        private Label finalFood;
        private Control warningFood;


        private Button nextSeasonButton;
        public override void _Ready() {
            Connect("visibility_changed", this, nameof(Refresh));
            // Quest
            questName = GetNode<Label>("QuestBox/Quest/Name");
            partyContainer = GetNode<Control>("QuestBox/Party");
            partyList = GetNode<Control>("QuestBox/Party/List");
            warningQuest = GetNode<Control>("QuestBox/Quest/WarningQuest");
            warningParty = GetNode<Control>("QuestBox/WarningParty");
            // Idle
            idleSeparator = GetNode<Control>("IdleSeparator");
            idleBox = GetNode<Control>("IdleBox");
            idleList = GetNode<Control>("IdleBox/Resting/List");
            // Food
            currentFood = GetNode<Label>("Food/Center/Grid/Current");
            consumedFood = GetNode<Label>("Food/Center/Grid/Consumed");
            finalFood = GetNode<Label>("Food/Center/Grid/Final");
            warningFood = GetNode<Control>("Food/Warning");
            // Next Season
            nextSeasonButton = GetNode<Button>("Next/Holder/NextSeasonButton");
            nextSeasonButton.Connect("pressed", this, nameof(NextSeason));
        }
        private void Refresh() {
            if (!Visible) {
                return;
            }
            // Quest
            if (Village.quest == null) {
                questName.Hide();
                partyContainer.Hide();
                warningParty.Hide();
                warningQuest.Show();
            } else {
                questName.Show();
                partyContainer.Show();
                warningQuest.Hide();
                bool party = false;
                partyList.QueueFreeChildren();
                foreach (CharacterEntity e in Village.actions.Where(VillageAction.QUEST)) {
                    party = true;
                    partyList.AddChild(Visual.Icons.CharacterIcon.Create(e));
                }
                if (party) {
                    warningParty.Hide();
                } else {
                    warningParty.Show();
                }
            }
            // Idle
            bool idle = false;
            idleList.QueueFreeChildren();
            foreach (CharacterEntity e in Family.familyMembers) {
                if (e.Idle()) {
                    idle = true;
                    idleList.AddChild(Visual.Icons.CharacterIcon.Create(e));
                }
            }
            if (idle) {
                idleSeparator.Show();
                idleBox.Show();
            } else {
                idleSeparator.Hide();
                idleBox.Hide();
            }
            // Food
            currentFood.Text = Game.data.inventory.food.ToString();
            int food = -Game.data.family.FoodConsumption();
            consumedFood.Text = food.ToString();
            food += Game.data.inventory.food;
            finalFood.Text = food.ToString();
            if (food < 0) {
                warningFood.Show();
            } else {
                warningFood.Hide();
            }
        }

        private void NextSeason() {
            if (Village.actions.Where(VillageAction.QUEST).Count() == 0) {
                Village.quest = null;
            }
            if (Village.quest != null) {
                GetTree().ChangeScene("Scenes/Battle.tscn");
            } else {
                GetTree().ChangeScene("Scenes/Outcome.tscn");
            }
        }
    }
}