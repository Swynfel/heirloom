using System;
using System.Collections.Generic;
using Godot;

public class Village : CanvasLayer {
    public static Dictionary<CharacterEntity, VillageAction> actions = new Dictionary<CharacterEntity, VillageAction>();
    public static Quest quest = null;

    public override void _Ready() {
        // Clear previous quest
        quest = null;
    }
}
