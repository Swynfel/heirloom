public static class QuestGeneration {

    public static Quest GenerateRandomQuest() {
        return GenerateRandomDungeon(noRare: true);
    }

    // TODO: Create special quests
    private static readonly Quest CROWN_QUEST;
    private static readonly Quest SHIELD_QUEST;
    private static readonly Quest FINAL_QUEST;

    public static Quest GenerateRandomDungeon(bool failIfNotRare = false, bool noRare = false) {
        // Should be rare?
        if (!noRare) {
            bool rare;
            if (!Game.data.progress.foundFinalQuest) {
                int luckTweak = 2;
                if (Game.data.progress.foundCrownQuest) {
                    luckTweak -= 2;
                }
                if (Game.data.progress.foundShieldQuest) {
                    luckTweak -= 2;
                }
                luckTweak += Game.data.progress.dungeonsFound;
                rare = Global.rng.Next(0, 5) <= luckTweak;
            } else {
                rare = false;
            }
            if (failIfNotRare && !rare) {
                return null;
            }
            // Load rare
            if (rare) {
                if (!Game.data.progress.foundCrownQuest && !Game.data.progress.foundShieldQuest) {
                    if (Global.rng.Next(0, 2) == 0) {
                        Game.data.progress.foundCrownQuest = true;
                        return CROWN_QUEST;
                    } else {
                        Game.data.progress.foundShieldQuest = true;
                        return SHIELD_QUEST;
                    }
                }
                if (!Game.data.progress.foundCrownQuest) {
                    Game.data.progress.foundCrownQuest = true;
                    return CROWN_QUEST;
                }
                if (!Game.data.progress.foundShieldQuest) {
                    Game.data.progress.foundShieldQuest = true;
                    return SHIELD_QUEST;
                }
                Game.data.progress.foundFinalQuest = true;
                return FINAL_QUEST;
            }
        }
        // Generate normal quest
        Quest quest = new Quest();
        int intensity = Global.rng.Next(3, 13);
        QuestReward reward = new QuestReward(intensity * intensity, QuestReward.Group.DUNGEON);
        (string name, BattleGeneration battle, int party) = GenerateDungeon(intensity);
        int extension = (Global.rng.Next(3, 10) + intensity) / 4;
        quest.name = name;
        quest.battle = battle;
        quest.partySize = party;
        quest.difficulty = Difficulty(party, battle.count, intensity);
        quest.deadline = Game.data.date.Plus(extension);
        return quest;
    }

    private static string Difficulty(int party, int enemies, int intensity) {
        int indicator = 5 * (enemies - party) + intensity - 3;
        if (indicator < 0) return "very easy";
        if (indicator < 5) return "easy";
        if (indicator < 10) return "medium";
        if (indicator < 15) return "hard";
        return "epic";
    }

    private static readonly string[] CAVE_ADJECTIVES = {
        "dark", "gloomy", "spooky", "evil", "mystical", "pitch-black"
    };
    private static readonly string[] DIRT_CAVE_NOUNS = {
        "den", "hollow", "tunnel"
    };
    private static readonly string[] STONE_CAVE_NOUNS = {
        "cave", "cavern",
    };

    private static readonly ElementalAffinity DIRT_CAVE_ELEMENTAL = new ElementalAffinity(p1: Element.PLANT, p2: Element.DARK, n1: Element.FIRE);
    private static readonly ElementalAffinity STONE_CAVE_ELEMENTAL = new ElementalAffinity(p1: Element.METAL, p2: Element.DARK, n1: Element.LIGHT);

    private static (string, BattleGeneration, int) GenerateDungeon(int intensity) {
        int roll = Global.rng.Next(0, 20) + (intensity / 3);
        int count = (Global.rng.Next(0, 5) + intensity) / 3;
        int party = (Global.rng.Next(7, 11) + intensity) / 5;
        if (roll <= 13) {
            return (
                string.Format("The {0} {1}", CAVE_ADJECTIVES.Random(), DIRT_CAVE_NOUNS.Random()),
                new BattleGeneration(
                    BattleGeneration.Map.DIRT_CAVE,
                    BattleGeneration.EnemyType.BANDITS,
                    count,
                    DIRT_CAVE_ELEMENTAL
                ), party
            );
        } else {
            return (
                string.Format("The {0} {1}", CAVE_ADJECTIVES.Random(), STONE_CAVE_NOUNS.Random()),
                new BattleGeneration(
                    BattleGeneration.Map.STONE_CAVE,
                    BattleGeneration.EnemyType.BANDITS,
                    count,
                    STONE_CAVE_ELEMENTAL
                ), party
            );
        }
    }
}