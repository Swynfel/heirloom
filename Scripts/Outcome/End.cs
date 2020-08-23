using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using P = OutcomeProcess;

namespace OutcomeProcesses {
    public static class End {
        public static async Task Process() {
            Crown();
            await Food();
            await Age();
            CleanJobs();
        }

        private static void Crown() {

            GD.Print("AAA");
            Entity crownHolder = Family.familyMembers.FirstOrDefault(e => e?.heldItem?.special == "crown");

            GD.Print("AAA");
            if (crownHolder != null) {
                crownHolder.maxHealth++;
                crownHolder.ModifyHealth(1);
            }
        }

        private static void AddTo(Dictionary<Date.AgeGroup, List<Entity>> groups, Date.AgeGroup key, Entity entity) {
            if (!groups.Keys.Contains(key)) {
                groups[key] = new List<Entity>();
            }
            groups[key].Add(entity);
        }
        private static async Task Age() {
            Dictionary<Date.AgeGroup, List<Entity>> growups = new Dictionary<Date.AgeGroup, List<Entity>>();
            List<Entity> deathOfOldAge = new List<Entity>();
            // Preprocess
            foreach (Entity e in Family.familyMembers) {
                int age = e.age;
                // AgeGroup.CHILD;
                if (age == 2) {
                    e.maxHealth += 4;
                    e.ModifyHealth(4);
                    AddTo(growups, Date.AgeGroup.CHILD, e);
                    continue;
                }

                // AgeGroup.TEEN;
                if (age == 4) {
                    e.maxHealth += 4;
                    e.ModifyHealth(4);
                    AddTo(growups, Date.AgeGroup.TEEN, e);
                    continue;
                }

                // AgeGroup.YOUNG_ADULT;
                if (age == 6) {
                    e.maxHealth += 2;
                    e.ModifyHealth(2);
                    AddTo(growups, Date.AgeGroup.YOUNG_ADULT, e);
                    continue;
                }

                // AgeGroup.ADULT
                if (age == 8) {
                    AddTo(growups, Date.AgeGroup.ADULT, e);
                    continue;
                }

                // AgeGroup.SENIOR
                if (age >= 12) {
                    e.maxHealth -= 2;
                    e.ModifyHealth(-1);
                    if (e.maxHealth <= 0) {
                        deathOfOldAge.Add(e);
                        continue;
                    }
                }
                if (age == 12) {
                    AddTo(growups, Date.AgeGroup.SENIOR, e);
                    continue;
                }
            }
            // Write
            if (growups.Count == 0 && deathOfOldAge.Count == 0) {
                return;
            }
            P.ui.SetTitle("Time passes...");
            P.ui.ClearDescription();
            string history = "";
            foreach (var group in growups) {
                Date.AgeGroup age = group.Key;
                bool plural = group.Value.Count > 1;
                string format;
                switch (age) {
                    case (Date.AgeGroup.CHILD):
                        format = plural ?
                        "{0} is now a child (+4 max health)." :
                        "{0} are now children (+4 max health).";
                        break;
                    case (Date.AgeGroup.TEEN):
                        format = plural ?
                        "{0} is now a teenager (+4 max health)." :
                        "{0} are now teenagers (+4 max health).";
                        break;
                    case (Date.AgeGroup.YOUNG_ADULT):
                        format = plural ?
                        "{0} is now a young adult (+2 max health)." :
                        "{0} are now young adults (+2 max health).";
                        break;
                    case (Date.AgeGroup.ADULT):
                        format = plural ?
                        "{0} is now a true adult." :
                        "{0} are now true adults.";
                        break;
                    case (Date.AgeGroup.SENIOR):
                        format = plural ?
                        "{0} is now a senior (will loose 2 max health per season)." :
                        "{0} are now seniors (will loose 2 max health per season).";
                        break;
                    default:
                        format = "";
                        break;
                }
                string s = string.Format(format, Entity.MetaNames(group.Value));
                history += s;
                P.ui.AddDescription(s + "\n");
            }
            if (history != "") {
                History.Append(history);
            }
            if (deathOfOldAge.Count >= 1) {
                string s = string.Format("{0} passed away...", Entity.MetaNames(deathOfOldAge));
                History.Append(s);
                P.ui.AddDescription(s);
            }
            foreach (Entity dead in deathOfOldAge) {
                Game.data.family.Die(dead);
            }
            P.ui.SetButtons("Continue");
            await P.ui.ButtonPressed();
        }

        private static async Task Food() {
            int food = Game.data.family.FoodConsumption();
            if (Game.data.inventory.food >= food) {
                Game.data.inventory.food -= food;
                P.ui.NoHead();
                if (Game.data.inventory.food >= food) {
                    P.ui.SetTitle("Plenty of food");
                    P.ui.SetDescription(string.Format("The family consumes {0} units of food. There is still plenty left.", food));
                } else {
                    P.ui.SetTitle("Low on food...");
                    P.ui.SetDescription(string.Format("The family consumes {0} units of food. There might be not enough food for next time, be sure to send to people to hunt.", food));
                }
                P.ui.SetButtons("Continue");
                await P.ui.ButtonPressed();
                return;
            }

            P.ui.SetTitle("We are starving...");
            P.ui.NoHead();
            P.ui.SetDescription(string.Format("There is no food left... {0} would have been required, but we had only {1}.\n", food, Game.data.inventory.food));

            int missing = food - Game.data.inventory.food;
            Game.data.inventory.food = 0;

            List<Entity> starved = new List<Entity>();
            List<Entity> sick = new List<Entity>();

            foreach (Entity e in Family.familyMembers.RandomOrder()) {
                e.maxHealth -= 2;
                e.ModifyHealth(-1);
                if (e.maxHealth <= 0) {
                    starved.Add(e);
                    Game.data.family.Die(e);
                } else {
                    sick.Add(e);
                }
                missing -= Family.FoodConsumption(e);
                if (missing <= 0) {
                    break;
                }
            }
            string history = "";
            if (sick.Count > 0) {
                string sickStr = string.Format("{0} {1} sick and lost 2 max health.\n", Entity.MetaNames(sick), sick.Count <= 1 ? "is" : "are");
                history += sickStr;
                P.ui.AddDescription(sickStr);
            }
            if (starved.Count > 0) {
                string starvedStr = string.Format("{0} starved.\n", Entity.MetaNames(starved));
                history += starvedStr;
                P.ui.AddDescription(starvedStr);
            }
            P.ui.SetButtons("Continue");
            await P.ui.ButtonPressed();
            return;
        }

        public static void CleanJobs() {
            foreach (Entity e in Family.familyMembers) {
                VillageAction action = e.Action();
                if (!e.AllowedActions().Contains(action)) {
                    Village.actions[e] = VillageAction.REST;
                    continue;
                }
                if (action == VillageAction.QUEST) {
                    Village.actions[e] = VillageAction.REST;
                }
            }
        }
    }
}