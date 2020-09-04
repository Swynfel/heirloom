
using System;
using System.Collections.Generic;
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
        if (sum < 25) return "a little";
        if (sum < 50) return "some";
        if (sum < 90) return "a lot of";
        if (sum < 130) return "heaps of";
        return "countless";
    }

    public bool Special() {
        switch (group) {
            case Group.DUNGEON_CROWN:
            case Group.DUNGEON_SHIELD:
            case Group.VICTORY:
                return true;
            default:
                return false;
        }
    }

    public Riches Generate() {
        if (group == Group.NONE) {
            return new Riches();
        }
        int lootLeft = intensity + Global.rng.Next(0, intensity / 4 + 10);
        bool dungeonTreasureBoost = group == Group.DUNGEON;
        int treasureLikeliness = intensity;
        if (dungeonTreasureBoost) {
            treasureLikeliness += treasureLikeliness / 2;
        }
        // Items
        List<Item> items = new List<Item>();
        while (lootLeft > 0 && treasureLikeliness > 0) {
            if (dungeonTreasureBoost || Global.rng.Next(0, 100) < intensity) {
                dungeonTreasureBoost = false;
                int x = Global.rng.Next(0, 4);
                Item treasure = x == 0 ? Item.VASE :
                    x == 1 ? Item.FAN : Item.DAGGER; // TODO: Randomly generate
                treasure = (Item) treasure.Duplicate();
                if (treasure.estimatedPrice > lootLeft) {
                    break;
                }
                treasureLikeliness -= 50;
                lootLeft -= treasure.estimatedPrice / 2;
            } else {
                break;
            }
        }
        // Gold and Food
        int gold = 0;
        int food = 0;
        if (group == Group.FOOD_GOLD) {
            float frac = 0.3f + 0.5f * (float) Global.rng.NextDouble();
            gold = (int) (lootLeft * frac);
            food = lootLeft - gold;
        } else if (group == Group.FOOD) {
            food = lootLeft;
        } else {
            gold = lootLeft;
        }
        return new Riches(gold, food, items);
    }
}