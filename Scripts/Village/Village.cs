using System;
using System.Collections.Generic;
using Godot;

public class Village : CanvasLayer {
    public static Dictionary<Entity, VillageAction> actions = new Dictionary<Entity, VillageAction>();

    public static Quest quest = null;
}
