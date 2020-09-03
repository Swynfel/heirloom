using System;
using Combat;
using Combat.SkillAreas;
using Combat.SkillEffects;
using Godot;

namespace Visual.Tables {
    using Icons;
    public class SkillTable : MarginContainer {

        [Export] private bool displayChargesLeft = true;
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/SkillTable.tscn");
        public static SkillTable Create(Skill skill) {
            SkillTable table = (SkillTable) template.Instance();
            table.SetSkill(skill);
            return table;
        }
        public void SetSkill(Skill skill) {
            // Left
            GetNode<Label>("Body/Left/Top/Name").Text = skill.name;
            GetNode<SkillButton>("Body/Left/Top/Skill").Configure(skill);
            GetNode<SmartText>("Body/Left/Description").BbcodeText = skill.BBDescription;
            // Right
            string leftString = "";
            string rightString = "[right]";
            // Cone-type skills
            if (skill.area is Cone cone) {
                leftString += "[?range]Cone[/?]\n";
                rightString += skill.area.minRange + "-" + skill.area.maxRange + ConeConstraintIcon(skill.area.constraint, cone.wide) + "\n";
            } else if (skill.area is Target target) {
                // Range
                leftString += "[?range]Range[/?]\n";
                if (skill.area.minRange == skill.area.maxRange) {
                    int single = skill.area.minRange;
                    if (single == 0) {
                        rightString += "0\n";
                    } else {
                        rightString += single.ToString() + AreaConstraintIcon(skill.area.constraint) + "\n";
                    }
                } else {
                    rightString += skill.area.minRange + "-" + skill.area.maxRange + AreaConstraintIcon(skill.area.constraint) + "\n";
                }
                // Area
                if (target.areaRange > 0) {
                    leftString += "[?range]Area[/?]\n";
                    int diameter = 2 * target.areaRange + 1;
                    rightString += $"{diameter}x{diameter}" + AreaConstraintIcon(target.areaConstraint) + "\n";
                }
            } else if (skill.area is Snake) {
                // Range
                leftString += "[?range]Distance[/?]\n";
                rightString += skill.area.minRange + "-" + skill.area.maxRange + AreaConstraintIcon(skill.area.constraint) + "\n";
            }
            // Effect
            if (skill.effect is Damage damage) {
                leftString += "[?damage]Damage[/?]\n";
                rightString += $"{damage.damage}\n";
                if (damage.noFriendlyFire) {
                    leftString += "[center]No friendly fire[/center]\n";
                    rightString += "\n";
                }
            } else if (skill.effect is Heal heal) {
                leftString += "[?heal]Heal[/?]\n";
                rightString += $"{heal.heal}\n";
            } else if (skill.effect is Move move) {
                if (move.ignoreGround) {
                    leftString += "[center]Movement (Instant)[/center]\n";
                } else {
                    leftString += "[center]Movement[/center]\n";
                }
                rightString += "\n";
            }
            // Cooldown
            if (skill.condition.cooldown > 0) {
                leftString += "Cooldown\n";
                rightString += $"{skill.condition.cooldown}\n";
            }
            // Charges
            if (skill.condition.combatCharges > 0) {
                leftString += "[?charge]Charges per Combat[/?]\n";
                if (displayChargesLeft) {
                    rightString += $"{skill.condition.remainingCombatCharges}/{skill.condition.combatCharges}\n";
                } else {
                    rightString += $"{skill.condition.combatCharges}\n";
                }
            }
            if (skill.condition.turnCharges > 0) {
                leftString += "[?charge]Charges per Turn[/?]\n";
                if (displayChargesLeft) {
                    rightString += $"{skill.condition.remainingTurnCharges}/{skill.condition.turnCharges}\n";
                } else {
                    rightString += $"{skill.condition.turnCharges}\n";
                }
            }
            // Special
            if (!skill.condition.passTurn) {
                leftString += "[center]Doesn't pass turn[/center]\n";
                rightString += "\n";
            }
            rightString += "[/right]";
            GetNode<SmartText>("Body/Right/Details/Left").BbcodeText = InfoText.BBfy(leftString);
            GetNode<SmartText>("Body/Right/Details/Right").BbcodeText = InfoText.BBfy(rightString);
        }

        public string AreaConstraintIcon(Constraint constraint) {
            string core = constraint switch
            {
                Constraint.SQUARE => "square_area",
                Constraint.DIAMOND => "diamond_area",
                Constraint.PLUS => "plus_area",
                Constraint.CROSS => "cross_area",
                Constraint.PATH => "path_area",
                _ => ""
            };
            if (core == "") {
                return "";
            } else {
                return $"[img]icon://ranges/{core}.png[/img]";
            }
        }
        public string ConeConstraintIcon(Constraint constraint, bool isWide) {
            string core =
            (constraint, isWide) switch
            {
                (Constraint.SQUARE, false) => "thin_square_cone",
                (Constraint.SQUARE, true) => "wide_square_cone",
                (Constraint.DIAMOND, false) => "thin_diamond_cone",
                (Constraint.DIAMOND, true) => "wide_diamond_cone",
                (Constraint.PLUS, _) => "line_cone",
                _ => ""
            };
            if (core == "") {
                return "";
            } else {
                return $"[img]icon://ranges/{core}.png[/img]";
            }
        }

        public override void _Ready() {
            SetSkill(SkillHandler.WALK);
        }
    }
}