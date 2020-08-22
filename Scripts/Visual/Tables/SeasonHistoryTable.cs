using System;
using Godot;

public class SeasonHistoryTable : VBoxContainer {
    private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/SeasonHistoryTable.tscn");
    public static SeasonHistoryTable Create(SeasonHistory data) {
        SeasonHistoryTable table = (SeasonHistoryTable) template.Instance();
        table.Setup(data);
        return table;
    }

    private void Setup(SeasonHistory data) {
        GetNode<Label>("Season").Text = data.date.ToString();
        foreach (string @event in data.events) {
            AddChild(SmartText.Create(@event));
        }
    }
}
