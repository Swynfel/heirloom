using System;
using Godot;

public class SmartText : RichTextLabel {
    private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Effects/SmartText.tscn");
    public static SmartText Instance() {
        return (SmartText) template.Instance();
    }

    public static SmartText Create(string value) {
        SmartText smartText = Instance();
        smartText.BbcodeText = value;
        return smartText;
    }
    public override void _Ready() {
        Connect("meta_clicked", this, nameof(MetaClicked));
    }

    private void MetaClicked(string info) {
        var metaTag = Memory.MetaTag.Parse(info);
        MetaPopup.instance.OpenMemory(metaTag);
    }
}
