using System;
using Godot;

public class LoadBattle : Node2D {
    public override void _Ready() {
        Combat.Battle.current.StartBattle();
    }
}
