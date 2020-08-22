
using System;
using Godot;
public class QuestReward : Resource {

    public enum Group {
        NONE,
        FOOD,
        GOLD,
        FOOD_GOLD,
        DUNGEON,
        DUNGEON_CROWN,
        DUNGEON_SHIELD,
        VICTORY,
    }

    [Export] private int intensity = 64;

    [Export] private Group group = Group.NONE;

    public QuestReward() { }

    public QuestReward(int i, Group g) {
        intensity = i;
        group = g;
    }

    public override string ToString() {
        switch (group) {
            default:
            case Group.NONE:
                return "nothing";
            case Group.FOOD:
                return RewardQualifier(intensity) + " food";
            case Group.GOLD:
                return RewardQualifier(intensity) + " gold";
            case Group.FOOD_GOLD:
                return RewardQualifier(intensity) + " food and gold";
            case Group.DUNGEON:
                if (intensity <= 100) {
                    return "treasures";
                } else {
                    return "rare treasures";
                }
            case Group.DUNGEON_CROWN:
            case Group.DUNGEON_SHIELD:
                return "rare treasures";
            case Group.VICTORY:
                return "Victory!";
        }
    }

    private static string RewardQualifier(int sum) {
        if (sum < 30) return "a little";
        if (sum < 60) return "some";
        if (sum < 100) return "a lot of";
        if (sum < 150) return "heaps of";
        return "countless";
    }
}