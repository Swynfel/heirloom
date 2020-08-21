using System;
using Godot;

public class QuestTable : GridContainer {
    public void SetQuest(Quest quest) {
        GetNode<Label>("Name").Text = quest.name;
        GetNode<Label>("Reward").Text = quest.reward;
        GetNode<Label>("Deadline").Text = quest.deadline.ContextString();
        GetNode<Label>("Difficulty").Text = quest.difficulty;
        GetNode<Visual.Icons.ElementalAffinityIcon>("Elements/ElementalAffinity").SetAffinity(quest.elements);
        GetNode<Label>("Party").Text = quest.partySize.ToString();
    }
}
