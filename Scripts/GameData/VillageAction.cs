using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public enum VillageAction {
    REST,
    QUEST,
    FIND_QUEST,
    FIND_DUNGEON,

    WORK,
    ORPHANAGE,
    LOVE,
    LEARN,
}

public static class VillageActionExtensions {
    public static int Count(this IEnumerable<VillageAction> actions, VillageAction action) {
        return actions.Where(a => a == action).Count();
    }

    public static int Count<T>(this Dictionary<T, VillageAction> actions, VillageAction action) {
        return actions.Values.Where(a => a == action).Count();
    }

    public static IEnumerable<T> Where<T>(this Dictionary<T, VillageAction> actions, VillageAction action) {
        return actions.Where(a => a.Value == action).Select(a => a.Key);
    }
}