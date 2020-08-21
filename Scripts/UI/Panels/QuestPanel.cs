using System;
using Godot;

public class QuestPanel : VBoxContainer {
    public override void _Ready() {
        Quest quest = new Quest();
        GetNode<QuestTable>("QuestSelection/QuestTable").SetQuest(
            quest
        );
    }
}
