using System;
using Godot;

namespace Visual.Tables {
    public class QuestTable : GridContainer {
        public void SetQuest(Quest quest) {
            GetNode<Label>("Name").Text = quest.name;
            GetNode<Label>("Reward").Text = quest.reward.ToString();
            GetNode<Label>("Deadline").Text = quest.deadline.ContextString();
            GetNode<Label>("Difficulty").Text = quest.difficulty;
            GetNode<Icons.ElementalAffinityIcon>("Elements/ElementalAffinity").SetAffinity(quest.battle.elements);
            GetNode<Label>("Party").Text = quest.partySize.ToString();
        }
    }
}