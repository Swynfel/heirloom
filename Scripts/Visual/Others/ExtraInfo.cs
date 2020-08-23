using System;
using Godot;

public class ExtraInfo : MarginContainer {
    public override void _Ready() {
        GetNode<Label>("List/Season").Text = Game.data.date.ToString();
    }

    // HACK: EXTREMELY DIRTY, no time
    public override void _Process(float delta) {
        GetNode<Label>("List/Grid/Food").Text = Game.data.inventory.food.ToString();
        GetNode<Label>("List/Grid/Gold").Text = Game.data.inventory.gold.ToString();
    }
}
