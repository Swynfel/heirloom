using System;
using Godot;

namespace Visual.Tables {
    using Icons;
    public class SkillTable : MarginContainer {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/SkillTable.tscn");
        public static SkillTable Create(Skill skill) {
            SkillTable table = (SkillTable) template.Instance();
            table.SetSkill(skill);
            return table;
        }
        public void SetSkill(Skill skill) {
            GetNode<Label>("Body/Top/Name").Text = skill.name;
            GetNode<SkillButton>("Body/Top/Skill").Configure(skill);
            GetNode<Label>("Body/Bottom/Grid/Range").Text = skill.area.minRange + "-" + skill.area.maxRange;
            GetNode<Label>("Body/Bottom/Grid/Type").Text = skill.area.constraint.ToString();
            GetNode<SmartText>("Body/Bottom/Description").Text = skill.description;
        }

        public override void _Ready() {
            SetSkill(SkillHandler.WALK);
        }
    }
}