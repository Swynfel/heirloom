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
    HUNT,
    ORPHANAGE,
    FIND_LOVE,
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

    public static VillageAction Action(this Entity entity) {
        return Village.actions.GetOrDefault(entity, VillageAction.REST);
    }
    public static bool Idle(this Entity entity) {
        return entity.Action() == VillageAction.REST && entity.AllowedActions().Count > 1;
    }
    public static List<VillageAction> AllowedActions(this Entity entity) {
        switch (entity.ageGroup) {
            case Date.AgeGroup.UNBORN:
            case Date.AgeGroup.BABY:
                return new List<VillageAction>() { VillageAction.REST };
            case Date.AgeGroup.CHILD:
                return new List<VillageAction>() { VillageAction.REST }; // TODO: School?...
            case Date.AgeGroup.TEEN:
                return new List<VillageAction>() {
                    VillageAction.REST,
                    VillageAction.QUEST,
                    VillageAction.FIND_QUEST
                };
        }
        List<VillageAction> list = new List<VillageAction>() {
            VillageAction.REST,
            VillageAction.QUEST,
            VillageAction.WORK,
            VillageAction.FIND_DUNGEON,
            VillageAction.ORPHANAGE
        };
        if (entity.lover == null) {
            list.Add(VillageAction.FIND_LOVE);
        } else {
            if (entity.ageGroup != Date.AgeGroup.SENIOR) {
                list.Add(VillageAction.LOVE);
            }
        }
        return list;
    }

    public static (string, string) ActionText(this Entity entity) {
        return ActionText(entity.Action(), entity.ageGroup);
    }
    public static (string, string) ActionText(VillageAction action, Date.AgeGroup age) {
        switch (action) {
            case VillageAction.REST:
                switch (age) {
                    case Date.AgeGroup.BABY:
                        return ("Baby", "being a baby is already a full-time job");
                    case Date.AgeGroup.CHILD:
                        return ("Play", "runs outside and explores the world");
                    default:
                        return ("Rest", "recovers some health");
                }
            case VillageAction.QUEST:
                return ("Quest", "busy adventuring");
            case VillageAction.FIND_QUEST:
                return ("Find Quest", "looks for a quest");
            case VillageAction.FIND_DUNGEON:
                return ("Find Dungeon", "looks for a dungeon containing rare items");
            case VillageAction.WORK:
                return ("Work", "gets some money");
            case VillageAction.HUNT:
                return ("Hunt", "find some food");
            case VillageAction.ORPHANAGE:
                return ("Orphanage", "for adoption");
            case VillageAction.FIND_LOVE:
                return ("Look for love", "look for a special someone");
            case VillageAction.LOVE:
                return ("Spend time with lover", "to increase the family size");
            default:
                return ("ACTION", "ERROR");
        }
    }
}