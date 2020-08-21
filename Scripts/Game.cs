using System;
using Godot;

public class Game : Node {
    public static Game instance { get; private set; }

    public static GameData data = GameData.New();

    public override void _Ready() {
        instance = this;
    }
    private static int busyCount = 0;

    public static bool busy { get => busyCount > 0; }

    [Signal] public delegate void busy_switch(bool busy);
    [Signal] public delegate void busy_on();
    [Signal] public delegate void busy_off();

    public static void StartBusy() {
        if (!busy) {
            instance.EmitSignal(nameof(busy_switch), true);
            instance.EmitSignal(nameof(busy_on));
        }
        busyCount++;
    }

    public static void EndBusy() {
        busyCount--;
        if (!busy) {
            instance.EmitSignal(nameof(busy_switch), false);
            instance.EmitSignal(nameof(busy_off));
        }
    }
}