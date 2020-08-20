using System;
using Godot;

public static class Global {
    public static Random rng { get; } = new Random();

    public static Combat.Battle battle { get => Combat.Battle.current; }
    public static Combat.Board board { get => Combat.Battle.current.board; }
    public static UI.BattleUI battleUI { get => UI.BattleUI.current; }

    public static Godot.Collections.Array ArrayFrom(object obj) {
        Godot.Collections.Array result = new Godot.Collections.Array();
        result.Add(obj);
        return result;
    }
    public static void QueueFreeChildren(this Node node) {
        foreach (Node child in node.GetChildren()) {
            child.QueueFree();
        }
    }
}