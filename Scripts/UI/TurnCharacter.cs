using System;
using Godot;

public class TurnCharacter : HBoxContainer {
    private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/UI/TurnCharacter.tscn");
    public static TurnCharacter Create(Combat.Piece piece) {
        TurnCharacter character = (TurnCharacter) template.Instance();
        character.Set(piece);
        return character;
    }

    private bool setup = false;
    private Label name;
    private Control icon;
    private HealthBar health;
    private ElementIconList resistance;
    private ElementIconList weakness;

    private void TrySetup() {
        if (!setup) {
            setup = true;
            name = GetNode<Label>("Left/NameHolder/Name");
            icon = GetNode<Control>("Right/Icon");
            health = GetNode<HealthBar>("Left/HealthHolder/HealthBar");
            resistance = GetNode<ElementIconList>("Left/Elements/Resistances");
            weakness = GetNode<ElementIconList>("Left/Elements/Weaknesses");
        }
    }

    public override void _Ready() {
        TrySetup();
    }
    public void Set(Combat.Piece piece) {
        TrySetup();
        name.Text = piece.Name;
        icon.QueueFreeChildren();
        Node2D clonedDisplay = (Node2D) piece.GetNode("Display").Duplicate();
        icon.AddChild(clonedDisplay);
        clonedDisplay.Position = new Vector2(0, 8);
        health.SetHealth(piece.stats.health, piece.stats.maxHealth);
        piece.stats.Connect(nameof(PieceStats.health_modified), health, nameof(HealthBar.SetHealth));
        resistance.SetElements(piece.stats.affinity.GetResistances());
        weakness.SetElements(piece.stats.affinity.GetWeaknesses());
    }
}
