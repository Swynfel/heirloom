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
        name.Text = piece.entity.name;
        icon.QueueFreeChildren();
        Node2D clonedDisplay = (Node2D) piece.GetNode("Display").Duplicate();
        icon.AddChild(clonedDisplay);
        clonedDisplay.Position = new Vector2(0, 8);
        health.SetHealth(piece.entity.health, piece.entity.maxHealth);
        piece.entity.Connect(nameof(Entity.health_modified), health, nameof(HealthBar.SetHealth));
        resistance.SetElements(piece.entity.affinity.GetResistances());
        weakness.SetElements(piece.entity.affinity.GetWeaknesses());
    }

    private static Vector2 BIG_ICON = new Vector2(40, 40);
    public void Grow() {
        GetNode<Control>("Right").RectMinSize = BIG_ICON;
        GetNode<VBoxContainer>("Left").AddConstantOverride("separation", 4);
        AddConstantOverride("separation", 6);
        health.MarginLeft -= 20;
    }
}
