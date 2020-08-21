using System;
using Godot;

namespace Visual.Tables {
    using Icons;
    public class CharacterTable : CenterContainer {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/CharacterTable.tscn");
        public static CharacterTable Create(Entity character) {
            CharacterTable table = (CharacterTable) template.Instance();
            table.SetCharacter(character);
            return table;
        }
        public void SetCharacter(Entity character) {
            GetNode<Label>("List/Name").Text = character.name;
            GetNode<CharacterAppearance>("List/Center/Block/Character").data = character.appearance;
            GetNode<HealthBar>("List/HealthHolder/HealthBar").SetHealth(character.health, character.maxHealth);
            GetNode<ElementalAffinityIcon>("List/Elements").SetAffinity(character.affinity);
            GetNode<SkillIconList>("List/Skills/Skills").SetCoreSkills(character);
            GetNode<SkillIconList>("List/Skills/Talents").SetTalents(character);
            GetNode<Label>("List/Details/List/Age").Text = character.AgeString();
            GetNode<Label>("List/Details/List/Birth").Text = character.birth.ToString();
            GetNode<Label>("List/Details/List/Action").Text = Village.actions.GetOrDefault(character, VillageAction.REST).ToString();
        }
    }
}