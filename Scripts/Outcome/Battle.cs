using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using P = OutcomeProcess;

namespace OutcomeProcesses {
    public class Battle {
        List<Entity> questers;
        Quest quest;
        public async Task Process() {
            quest = Village.quest;
            questers = Village.actions.Where(VillageAction.QUEST).ToList();
            P.ui.SetCharacters(questers);
            if (Combat.Battle.won) {
                QuestSuccessContent();
            } else {
                QuestFailContent();
            }
            HurtInBattle(Combat.Battle.won);
            P.ui.SetButtons("Continue");
            await P.ui.ButtonPressed();
            // Final
            foreach (Entity entity in questers) {
                if (entity.health <= 0) {
                    entity.health = 1;
                } else {
                    entity.ModifyHealth(3);
                }
            }
        }

        public const string QUEST_SUCCESS =
            "{0} succeeded in their quest \"{1}\"!";
        private void QuestSuccessContent() {
            Riches riches = quest.reward.Generate();
            P.ui.SetTitle("Quest Succeeded!");
            P.ui.ClearDescription();
            string s = string.Format(
                QUEST_SUCCESS,
                Entity.MetaNames(questers),
                quest.name
            );
            s += "\n";
            s += Found(riches);
            P.ui.AddDescription(s);
            History.Append(s);
            Game.data.quests.Remove(quest);
            Game.data.inventory.Add(riches);
        }

        public const string QUEST_FAILED =
            "{0} failed in their quest \"{1}\"...";
        private void QuestFailContent() {
            Riches riches = quest.reward.Generate();
            P.ui.SetTitle("Quest Failed...");
            P.ui.ClearDescription();
            string s = string.Format(
                QUEST_FAILED,
                Entity.MetaNames(questers),
                quest.name
            );
            P.ui.AddDescription(s);
            History.Append(s);
            Game.data.quests.Remove(quest);
        }

        private void HurtInBattle(bool won) {
            List<(Entity, int)> wounds = new List<(Entity, int)>();
            foreach (Entity e in questers) {
                if (e.health <= 0) {
                    int wound = Global.rng.Next(-1, 7);
                    if (e.ageGroup == Date.AgeGroup.YOUNG_ADULT) {
                        wound -= 1;
                    } else if (e.ageGroup == Date.AgeGroup.SENIOR) {
                        wound += 1;
                    }
                    if (won) {
                        wound -= 3;
                    }
                    wound = wound / 2;
                    if (wound > 0) {
                        wounds.Add((e, wound));
                    }
                }
            }
            if (wounds.Count == 0) {
                return;
            }
            // Wounded
            string s;
            if (wounds.Count == 1) {
                (Entity e, int w) = wounds[0];
                s = string.Format(
                    "{0} was wounded in battle and lost {1} max health.",
                    e.MetaName(), w
                );
            } else {
                s = string.Format(
                    "{0} were wounded in battle ({1}).",
                    Entity.MetaNames(wounds.Select(x => x.Item1)),
                    string.Join(", ", wounds.Select(x => string.Format("{0} lost {1} max health", x.Item1.MetaName(), x.Item2)))
                );
            }
            P.ui.AddDescription("\n\n" + s);
            History.Append(s);
            foreach ((Entity e, int w) in wounds) {
                e.maxHealth -= w;
                // TODO: Check if death
            }
        }

        private string Found(Riches r) {
            List<string> things = new List<string>();
            if (r.gold > 0) {
                things.Add(string.Format("{0} gold", r.gold));
            }
            if (r.food > 0) {
                things.Add(string.Format("{0} food", r.food));
            }
            if (r.items.Count > 0) {
                string s;
                if (r.items.Count == 1) {
                    s = "the treasure " + r.items[0].MetaName();
                } else {
                    s = "the following treasures: ";
                    s += r.items.Select(item => item.MetaName());
                }
                things.Add(s);
            }
            return string.Format("They found {0}.", things.FancyJoin("nothing"));
        }
    }
}