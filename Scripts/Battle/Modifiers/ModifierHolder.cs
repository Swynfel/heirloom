using System;
using System.Collections.Generic;
using Godot;

public class ModifierHolder : Resource, IModifier {
    [Export] public Dictionary<Modifier, int> Modifiers;
    public int GetMod(Modifier mod) {
        return Modifiers.GetOrDefault(mod, 0);
    }
}