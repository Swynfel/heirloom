using System;
using Godot;

public class TurnPanel : Control {

    private VBoxContainer turnCharacters;

    public override void _Ready() {
        turnCharacters = GetNode<VBoxContainer>("ScrollContainer/TurnCharacterContainer");
        UpdateTurn();
    }

    private bool pendingUpdate = true;
    public void UpdateTurn() {
        if (Global.battle == null) {
            pendingUpdate = true;
        } else {
            turnCharacters.QueueFreeChildren();
            foreach (Combat.Piece piece in Global.battle.pieces) {
                turnCharacters.AddChild(TurnCharacter.Create(piece));
            }
            pendingUpdate = false;
        }
    }

    public override void _Process(float delta) {
        if (pendingUpdate) {
            UpdateTurn();
        }
    }
}
