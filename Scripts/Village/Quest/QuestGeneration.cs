using Combat.Generate;
using MapType = Combat.Generate.MapType;

public static class QuestGeneration {

    public static Quest GenerateRandomQuest(int minimumIntensity = 3, int maximumIntensity = 12) {
        int intensity = Global.rng.Next(minimumIntensity, maximumIntensity);
        Quest quest = GenerateQuest(intensity);
        int extension = (Global.rng.Next(3, 10) + intensity) / 4;
        quest.deadline = Game.data.date.Plus(extension);
        return quest;
    }

    public static Quest TUTORIAL_QUEST {
        get {
            return new Quest() {
                name = "Go to Children's village",
                reward = new QuestReward(0, QuestReward.Group.NONE),
                battle = new DeterministicBattleGeneration(DeterministicBattleGeneration.DeterministicType.TUTORIAL),
                partySize = 1,
                difficulty = "tutorial",
                deadline = Date.NEVER,
            };
        }
    }

    public static Quest CROWN_QUEST {
        get {
            return new Quest() {
                name = "Ruins of the Eternal Castle",
                reward = new QuestReward(100, QuestReward.Group.DUNGEON_CROWN),
                battle = new DeterministicBattleGeneration(DeterministicBattleGeneration.DeterministicType.CROWN),
                partySize = 5,
                difficulty = "difficult",
                deadline = Date.NEVER,
            };
        }
    }
    public static Quest SHIELD_QUEST {
        get {
            (string name, ClassicBattleGeneration battle, int party) = GenerateDungeon(10);
            return new Quest() {
                name = "Tomb of the Undefeated Hero",
                reward = new QuestReward(100, QuestReward.Group.DUNGEON_SHIELD),
                battle = new DeterministicBattleGeneration(DeterministicBattleGeneration.DeterministicType.SHIELD),
                partySize = 4,
                difficulty = "difficult",
                deadline = Date.NEVER,
            };
        }
    }
    public static Quest FINAL_QUEST {
        get {
            (string name, ClassicBattleGeneration battle, int party) = GenerateDungeon(15);
            return new Quest() {
                name = "The Last Quest",
                reward = new QuestReward(200, QuestReward.Group.VICTORY),
                battle = new DeterministicBattleGeneration(DeterministicBattleGeneration.DeterministicType.FINAL),
                partySize = 6,
                difficulty = "impossible",
            };
        }
    }

    public static Quest GenerateRandomBasicDungeon() {
        int intensity = Global.rng.Next(4, 13);
        (string name, ClassicBattleGeneration battle, int party) = GenerateDungeon(intensity);
        int extension = (Global.rng.Next(3, 10) + intensity) / 4;
        return new Quest() {
            name = name,
            reward = new QuestReward(intensity * intensity, QuestReward.Group.DUNGEON),
            battle = battle,
            partySize = party,
            difficulty = Difficulty(party, battle.enemyCount, intensity),
            deadline = Game.data.date.Plus(extension),
        };
    }


    // Can return null if "failIfNotRare" is true
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
            rare = false;
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
        return GenerateRandomBasicDungeon();
    }

    private static string Difficulty(int party, int enemies, int intensity) {
        int indicator = 5 * enemies - 3 * party + (intensity / 2);
        if (indicator < 5) return "very easy";
        if (indicator < 10) return "easy";
        if (indicator < 15) return "medium";
        if (indicator < 20) return "hard";
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
    private static readonly string[] SANCTUARY_NOUNS = {
        "sanctuary", "temple", "shrine"
    };

    private static readonly ElementalAffinity DIRT_CAVE_ELEMENTAL = new ElementalAffinity(p1: Element.PLANT, p2: Element.DARK, n1: Element.FIRE);
    private static readonly ElementalAffinity STONE_CAVE_ELEMENTAL = new ElementalAffinity(p1: Element.METAL, p2: Element.DARK, n1: Element.LIGHT);

    private static (string, ClassicBattleGeneration, int) GenerateDungeon(int intensity) {
        int roll = Global.rng.Next(0, 20) + (intensity / 3);
        int count = (Global.rng.Next(1, 6) + intensity) / 3;
        int party = (Global.rng.Next(7, 11) + intensity) / 5;
        if (roll <= 13) {
            return (
                string.Format("The {0} {1}", CAVE_ADJECTIVES.Random(), DIRT_CAVE_NOUNS.Random()),
                new ClassicBattleGeneration(
                    MapType.DIRT_CAVE,
                    ClassicBattleGeneration.EnemyType.BANDITS,
                    count,
                    DIRT_CAVE_ELEMENTAL
                ), party
            );
        } else {
            return (
                string.Format("The {0} {1}", CAVE_ADJECTIVES.Random(), STONE_CAVE_NOUNS.Random()),
                new ClassicBattleGeneration(
                    MapType.STONE_CAVE,
                    ClassicBattleGeneration.EnemyType.BANDITS,
                    count,
                    STONE_CAVE_ELEMENTAL
                ), party
            );
        }
    }

    private static Quest GenerateQuest(int intensity) {
        int roll = Global.rng.Next(0, 100) + intensity;
        if (roll < 25) {
            return GenerateDesertQuest(intensity);
        } else if (roll < 50) {
            return GeneratePlainsQuest(intensity);
        } else if (roll < 75) {
            return GenerateRoadQuest(intensity);
        } else {
            return GenerateHideoutQuest(intensity);
        }
    }


    private static readonly ElementalAffinity HIDEOUT_ELEMENTAL = new ElementalAffinity(p1: Element.FIRE, p2: Element.METAL, n1: Element.DARK);
    private static readonly (string, string)[] HIDEOUT_ADJECTIVES_WITH_NOUN = {
        ("outlawed", "outlaw"),
        ("robbers'", "robber"),
        ("pirates'", "pirate"),
        ("pillagers'", "pillager"),
        ("barbarians'", "barbarian")
    };
    private static readonly string[] HIDEOUT_NOUNS = {
        "tavern", "pub", "hideout", "lair",
    };
    private static Quest GenerateHideoutQuest(int intensity) {
        (string adj, string noun) = HIDEOUT_ADJECTIVES_WITH_NOUN.Random();
        string name = string.Format("The {0} {1}", adj, HIDEOUT_NOUNS.Random());
        int count = (Global.rng.Next(1, 6) + intensity) / 3;
        int party = (Global.rng.Next(7, 11) + intensity) / 5;
        ClassicBattleGeneration battle = new ClassicBattleGeneration(
            MapType.HIDEOUT,
            ClassicBattleGeneration.EnemyType.BANDITS,
            count,
            HIDEOUT_ELEMENTAL,
            enemyNoun: noun
        );
        return new Quest() {
            name = name,
            reward = new QuestReward(intensity * (intensity + 1) + 5, QuestReward.Group.FOOD_GOLD),
            battle = battle,
            partySize = party,
            difficulty = Difficulty(party, battle.enemyCount, intensity),
        };
    }


    private static readonly ElementalAffinity PLAINS_ELEMENTAL = new ElementalAffinity(p1: Element.PLANT, p2: Element.AIR, n1: Element.FIRE);

    private static readonly string[] PLAINS_ADJECTIVES = {
        "wild", "savage", "overgrown",
    };
    private static readonly string[] PLAINS_NOUNS = {
        "plains", "field", "steppe", "meadow", "prairie", "meadow", "grassland"
    };

    private static Quest GeneratePlainsQuest(int intensity) {
        string name = string.Format("The {0} {1}", PLAINS_ADJECTIVES.Random(), PLAINS_NOUNS.Random());
        int count = (Global.rng.Next(1, 6) + intensity) / 3;
        int party = (Global.rng.Next(7, 11) + intensity) / 5;
        ClassicBattleGeneration battle = new ClassicBattleGeneration(
            MapType.PLAINS,
            ClassicBattleGeneration.EnemyType.BANDITS,
            count,
            PLAINS_ELEMENTAL
        );
        return new Quest() {
            name = name,
            reward = new QuestReward(intensity * intensity + 5, QuestReward.Group.FOOD),
            battle = battle,
            partySize = party,
            difficulty = Difficulty(party, battle.enemyCount, intensity),
        };
    }

    private static readonly ElementalAffinity DESERT_ELEMENTAL = new ElementalAffinity(p1: Element.LIGHT, p2: Element.FIRE, n1: Element.WATER);

    private static readonly string[] DESERT_ADJECTIVES = {
        "desolated", "dry", "arid", "barren"
    };
    private static readonly string[] DESERT_NOUNS = {
        "desert", "wastelands", "dunes"
    };
    private static Quest GenerateDesertQuest(int intensity) {
        string name = string.Format("The {0} {1}", PLAINS_ADJECTIVES.Random(), PLAINS_NOUNS.Random());
        int count = (Global.rng.Next(1, 6) + intensity) / 3;
        int party = (Global.rng.Next(7, 11) + intensity) / 5;
        ClassicBattleGeneration battle = new ClassicBattleGeneration(
            MapType.DESERT,
            ClassicBattleGeneration.EnemyType.BANDITS,
            count,
            PLAINS_ELEMENTAL
        );
        return new Quest() {
            name = name,
            reward = new QuestReward((intensity * intensity) / 2 + 5, QuestReward.Group.GOLD),
            battle = battle,
            partySize = party,
            difficulty = Difficulty(party, battle.enemyCount, intensity),
        };
    }

    private static readonly ElementalAffinity ROAD_ELEMENTAL = new ElementalAffinity(p1: Element.PLANT, p2: Element.WATER, n1: Element.METAL);

    private static readonly string[] ROAD_ADJECTIVES = {
        "muddy", "dirt", "mud",
    };
    private static readonly string[] ROAD_NOUNS = {
        "road", "path", "tracks", "trail",
    };

    private static Quest GenerateRoadQuest(int intensity) {
        string name = string.Format("The {0} {1}", ROAD_ADJECTIVES.Random(), ROAD_NOUNS.Random());
        int count = (Global.rng.Next(1, 6) + intensity) / 3;
        int party = (Global.rng.Next(7, 11) + intensity) / 5;
        ClassicBattleGeneration battle = new ClassicBattleGeneration(
            MapType.DIRT_ROAD,
            ClassicBattleGeneration.EnemyType.BANDITS,
            count,
            ROAD_ELEMENTAL
        );
        return new Quest() {
            name = name,
            reward = new QuestReward(intensity * intensity + 5, QuestReward.Group.FOOD_GOLD),
            battle = battle,
            partySize = party,
            difficulty = Difficulty(party, battle.enemyCount, intensity),
        };
    }
}