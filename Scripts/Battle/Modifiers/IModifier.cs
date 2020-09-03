using System;
using Godot;

public interface IModifier {
    int GetMod(Modifier mod);
}

public enum Modifier {
    BONUS_DAMAGE,
    ARMOR,
}