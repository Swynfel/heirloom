using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using P = OutcomeProcess;

namespace OutcomeProcesses {
    public static class Town {
        public static async Task Process() {
            List<CharacterEntity> workers = new List<CharacterEntity>();
            List<CharacterEntity> hunters = new List<CharacterEntity>();
            foreach (CharacterEntity e in Family.familyMembers.ToArray()) {
                switch (e.Action()) {
                    case VillageAction.FIND_QUEST:
                        await ActionLookForDungeon(e); // ActionLookForQuest(e);
                        break;
                    // case VillageAction.FIND_DUNGEON:
                    //     await ActionLookForDungeon(e);
                    //     break;
                    case VillageAction.WORK:
                        workers.Add(e);
                        break;
                    case VillageAction.HUNT:
                        hunters.Add(e);
                        break;
                    case VillageAction.ORPHANAGE:
                        await ActionOrphanage(e);
                        break;
                    case VillageAction.FIND_LOVE:
                        await ActionFindLove(e);
                        break;
                    case VillageAction.LOVE:
                        await ActionLove(e);
                        break;
                }
            }
            if (workers.Count > 0) {
                await ActionWork(workers);
            }
            if (hunters.Count > 0) {
                await ActionHunt(hunters);
            }
        }

        // private static async Task ActionLookForQuest(Entity e) {
        //     // TODO:[TALENT]
        //     float x = (float) Global.rng.NextDouble();
        //     Quest quest;
        //     if (x < 0.1f) {
        //         quest = null;
        //         P.ui.SetTitle("No quest found...");
        //         P.ui.NoHead();
        //         P.ui.SetDescription(string.Format(
        //             "{0} didn't find any quest...", e.MetaName()
        //         ));
        //         P.ui.SetButtons("Continue");
        //     } else if (x < 0.2f) {
        //         quest = QuestGeneration.GenerateRandomDungeon(noRare: true);
        //         P.ui.SetTitle("Dungeon found!");
        //         P.ui.SetQuest(quest);
        //         P.ui.SetDescription(string.Format(
        //             "{0} found a dungeon while looking for a quest!", e.MetaName()
        //         ));
        //         P.ui.SetButtons("Accept", "Decline");
        //     } else {
        //         quest = QuestGeneration.GenerateRandomQuest();
        //         P.ui.SetTitle("Quest found!");
        //         P.ui.SetQuest(quest);
        //         P.ui.SetDescription(string.Format(
        //             "{0} found a quest!", e.MetaName()
        //         ));
        //         P.ui.SetButtons("Accept", "Decline");
        //     }
        //     var pressed = await P.ui.ButtonPressed();
        //     if (quest != null && pressed.Yes()) {
        //         AddLover(e, quest);
        //     }
        // }

        private static async Task ActionLookForDungeon(CharacterEntity e) {
            // TODO:[TALENT]
            float x = (float) Global.rng.NextDouble();
            Quest quest;
            if (x < 0.1f) { // ERROR OTHERWISE
                quest = null;
                P.ui.SetTitle("No dungeon found...");
                P.ui.NoHead();
                P.ui.SetDescription(string.Format(
                    "{0} didn't find any dungeon...", e.MetaName()
                ));
                P.ui.SetButtons("Continue");
            } else if (x < 0.2f) {
                quest = QuestGeneration.GenerateRandomQuest();
                P.ui.SetTitle("Quest found!");
                P.ui.SetQuest(quest);
                P.ui.SetDescription(string.Format(
                    "{0} found a quest!", e.MetaName()
                ));
                P.ui.SetButtons("Accept", "Decline");
            } else {
                quest = QuestGeneration.GenerateRandomDungeon();
                if (quest.reward.Special()) {
                    P.ui.SetTitle("Legendary dungeon found!");
                    P.ui.SetQuest(quest);
                    P.ui.SetDescription(string.Format(
                        "{0} found a legendary dungeon! Who knows what's in there?", e.MetaName()
                    ));
                    P.ui.SetButtons("Accept");
                } else {
                    P.ui.SetTitle("Dungeon found!");
                    P.ui.SetQuest(quest);
                    P.ui.SetDescription(string.Format(
                        "{0} found a dungeon!", e.MetaName()
                    ));
                    P.ui.SetButtons("Accept", "Decline");
                }
            }
            var pressed = await P.ui.ButtonPressed();
            if (quest != null && pressed.Yes()) {
                AddQuest(e, quest);
            }
        }

        private static void AddQuest(CharacterEntity finder, Quest quest) {
            History.Append(string.Format(
                "{0} found \"{1}\"", finder.MetaName(), quest.name
            ));
            Game.data.quests.Add(quest);
        }

        private static async Task ActionWork(List<CharacterEntity> entities) {
            int gold = 0;
            P.ui.SetTitle("Work");
            foreach (CharacterEntity e in entities) {
                // TODO:[TALENT]
                int earn = Global.rng.Next(1, 10);
                gold += earn;
            }
            string s = string.Format(
                "{0} worked and earned {1} gold.",
                CharacterEntity.MetaNames(entities), gold
            );
            P.ui.SetCharacters(entities);
            P.ui.SetButtons("Continue");
            P.ui.SetDescription(s);
            await P.ui.ButtonPressed();
            Game.data.inventory.gold += gold;
            History.Append(s);
        }

        private static async Task ActionHunt(List<CharacterEntity> entities) {
            int food = 0;
            P.ui.SetTitle("Hunt");
            foreach (CharacterEntity e in entities) {
                // TODO:[TALENT]
                int earn = Global.rng.Next(3, 10);
                food += earn;
            }
            string s = string.Format(
                "{0} hunted and collected {1} food.",
                CharacterEntity.MetaNames(entities), food
            );
            P.ui.SetCharacters(entities);
            P.ui.SetButtons("Continue");
            P.ui.SetDescription(s);
            await P.ui.ButtonPressed();
            Game.data.inventory.food += food;
            History.Append(s);
        }

        private static readonly string[] LOVE_FAIL = {
            "{0} looked for love, but didn't find anyone...",
            "No-one captured {0}'s heart...",
        };
        private static async Task ActionFindLove(CharacterEntity e) {
            // TODO:[TALENT]
            float x = (float) Global.rng.NextDouble();
            CharacterEntity potentialLover;
            if (x < 0.3f) {
                potentialLover = null;
                P.ui.SetTitle("Still single");
                P.ui.NoHead();
                P.ui.SetDescription(string.Format(
                    LOVE_FAIL.Random(), e.MetaName()
                ));
                P.ui.SetButtons("Continue");
            } else {
                potentialLover = new CharacterEntity();
                potentialLover.SetAge(e.age);
                P.ui.SetTitle("Is this love?");
                P.ui.SetCouple(e, potentialLover);
                P.ui.SetDescription(string.Format(
                    "{0} met {1}, but is it The One?",
                    e.MetaName(), potentialLover.name
                ));
                P.ui.SetButtons("Yes!", "Nope");
            }
            var pressed = await P.ui.ButtonPressed();
            if (potentialLover != null && pressed.Yes()) {
                AddLover(e, potentialLover);
            }
        }

        private static void AddLover(CharacterEntity e, CharacterEntity lover) {
            History.Append(string.Format(
                "{0} and {1} are now together!", e.MetaName(), lover.MetaName()
            ));
            e.lover = lover;
            lover.lover = e;
        }

        // Orphanage

        private static readonly string[] ORPHAN_FAILED = {
            "{0} visited the orphanage, but no child caught their eyes",
            "{0} went to the orphanage, but no child caught their eyes",
        };
        private static async Task ActionOrphanage(CharacterEntity e) {
            // TODO:[TALENT]
            float x = (float) Global.rng.NextDouble();
            if (x < 0.3f) {
                P.ui.SetTitle("Visit at the Orphanage");
                P.ui.NoHead();
                P.ui.SetDescription(string.Format(
                    ORPHAN_FAILED.Random(), e.MetaName()
                ));
                P.ui.SetButtons("Continue");
                await P.ui.ButtonPressed();
                return;
            }

            CharacterEntity potentialChild = CharacterEntity.Orphan();
            int gold = 5 * Global.rng.Next(2, 7);
            P.ui.SetTitle("Visit at the Orphanage");
            P.ui.SetAdoption(e, potentialChild);
            P.ui.SetDescription(string.Format(
                "{0} found a cute child called {1}. The owners of the orphanage ask for a donation of {2}.",
                e.MetaName(), potentialChild.name, gold
            ));
            bool enoughMoney = Game.data.inventory.gold >= gold;
            P.ui.SetButtons(string.Format("Adopt (Pay {0})", gold), "Decline", "Bargain");
            if (!enoughMoney) {
                P.ui.buttonValidate.Text = "Not enough gold";
                P.ui.buttonValidate.Disabled = true;
            }
            UI.Outcome.ButtonOutcome pressed = await P.ui.ButtonPressed();
            if (pressed == UI.Outcome.ButtonOutcome.THIRD) {
                int nD = 5 * Global.rng.Next(1, 8);
                if (nD < gold) {
                    gold = nD;
                    P.ui.AddDescription(
                        string.Format("\n You convince the orphanage that {0} is enough.", gold)
                    );
                    P.ui.SetButtons(string.Format("Adopt (Pay {0})", gold), "Decline");
                    enoughMoney = Game.data.inventory.gold >= gold;
                    if (!enoughMoney) {
                        P.ui.buttonValidate.Text = "Not enough gold";
                        P.ui.buttonValidate.Disabled = true;
                    }
                } else {
                    P.ui.AddDescription(
                        "\n The orphanage is offended that you are trying to bargain."
                    );
                    P.ui.buttonValidate.Disabled = false;
                    P.ui.SetButtons("Continue");
                    await P.ui.ButtonPressed();
                    return;
                }
                pressed = await P.ui.ButtonPressed();
            }
            P.ui.buttonValidate.Disabled = false;
            if (potentialChild != null && pressed.Yes()) {
                AddOrphan(e, potentialChild);
            }
        }

        private static void AddOrphan(CharacterEntity e, CharacterEntity orphan) {
            History.Append(string.Format(
                "{0} adopted {1}!", e.MetaName(), orphan.MetaName()
            ));
            Game.data.family.Add(orphan);
        }

        private static async Task ActionLove(CharacterEntity e) {
            // TODO:[TALENT]
            float x = (float) Global.rng.NextDouble();
            if (x < 0.3f) {
                return;
            }
            CharacterEntity child = CharacterEntity.FromBirth(e, e.lover);
            P.ui.SetTitle("New member of the family!");
            P.ui.SetBirth(e, e.lover, child);
            P.ui.SetDescription(string.Format(
                "{0} and {1} had a child {2}!",
                e.MetaName(), e.lover.MetaName(), child.MetaName()
            ));
            P.ui.SetButtons("Continue");
            await P.ui.ButtonPressed();
            AddBirth(e, e.lover, child);
        }

        private static void AddBirth(CharacterEntity p1, CharacterEntity p2, CharacterEntity child) {
            History.Append(string.Format(
                "{0} and {1} had a child {1}!", p1.MetaName(), p2.MetaName(), child.MetaName()
            ));
            Game.data.family.Add(child);
        }
    }
}