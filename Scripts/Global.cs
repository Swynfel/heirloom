using System;
using Godot;

public static class Global {
    public static Random rng { get; } = new Random();

    public static Godot.Collections.Array ArrayFrom(object obj) {
        Godot.Collections.Array result = new Godot.Collections.Array();
        result.Add(obj);
        return result;
    }
}