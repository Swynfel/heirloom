using System;
using Godot;

namespace Visual.Tables {
    using Icons;
    public class CharacterTableBig : CenterContainer {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/CharacterTableBig.tscn");
        public static CharacterTableBig Create(Entity character) {
            CharacterTableBig table = (CharacterTableBig) template.Instance();
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
            //GetNode<Label>("List/Details/List/Action").Text = character.Action().ToString();
            GetNode<SmartText>("List/Details/List/Lover").BbcodeText = "[right]" + (character.lover?.MetaName() ?? "None") + "[/right]";
            GetNode<SmartText>("List/Details/List/Item").BbcodeText = "[right]" + (character.heldItem?.MetaName() ?? "None") + "[/right]";
        }
    }
}