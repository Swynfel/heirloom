using System;
using Godot;

public class TitleScreen : CanvasLayer {

    private Label title;

    public override void _Ready() {
        title = GetNode<Label>("Holder/List/Title");
        GetNode("Holder/List/Button/List/New").Connect("pressed", this, nameof(on_New));
        if (GameData.HasSave()) {
            GetNode("Holder/List/Button/List/Load").Connect("pressed", this, nameof(on_Load));
        } else {
            GetNode<Button>("Holder/List/Button/List/Load").Disabled = true;
        }
    }

    public void on_New() {
        GameData.New();
        GetTree().ChangeScene("Scenes/Village.tscn");
    }

    public void on_Load() {
        var l = GameData.Load();
        GD.Print("TODO: Load Game");
    }
}
