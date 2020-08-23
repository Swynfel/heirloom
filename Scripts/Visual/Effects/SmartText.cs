using System;
using Godot;

public class SmartText : RichTextLabel {
    private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Effects/SmartText.tscn");
    public static SmartText Instance() {
        return (SmartText) template.Instance();
    }

    public static SmartText Create(string value) {
        SmartText smartText = Instance();
        smartText.Text = value;
        return smartText;
    }
    public override void _Ready() {
        Connect("meta_clicked", this, nameof(MetaClicked));
        Clear();
        AppendBbcode("This is a test...");
        foreach (Entity m in Family.familyMembers) {
            AppendBbcode(" " + m.MetaName());
        }
    }

    private void MetaClicked(string info) {
        var i = Memory.MetaTag.Parse(info);
        GD.Print(i.Remember());
    }
}
