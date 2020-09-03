using System;
using Godot;

namespace Visual.Tables {
    using Icons;
    public class CharacterTable : Control {

        [Export] private readonly string NAME_PATH = "";
        [Export] private readonly string APPEARANCE_PATH = "";
        [Export] private readonly string HEALTHBAR_PATH = "";
        [Export] private readonly string ELEMENTS_PATH = "";
        [Export] private readonly string SKILLS_PATH = "";
        [Export] private readonly string TALENTS_PATH = "";
        [Export] private readonly string AGE_PATH = "";
        [Export] private readonly string BIRTH_PATH = "";
        [Export] private readonly string ACTION_PATH = "";
        [Export] private readonly string LOVER_PATH = "";
        [Export] private readonly string ITEM_PATH = "";

        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/CharacterTable.tscn");
        public static CharacterTable Create(CharacterEntity character) {
            CharacterTable table = (CharacterTable) template.Instance();
            table.SetCharacter(character);
            return table;
        }
        public void SetCharacter(CharacterEntity character) {
            if (NAME_PATH != "")
                GetNode<Label>(NAME_PATH).Text = character.name;
            if (APPEARANCE_PATH != "")
                GetNode<CharacterAppearance>(APPEARANCE_PATH).data = character.appearance;
            if (HEALTHBAR_PATH != "")
                GetNode<HealthBar>(HEALTHBAR_PATH).SetHealth(character.health, character.maxHealth);
            if (ELEMENTS_PATH != "")
                GetNode<ElementalAffinityIcon>(ELEMENTS_PATH).SetAffinity(character.affinity);
            if (SKILLS_PATH != "")
                GetNode<SkillIconList>(SKILLS_PATH).SetCoreSkills(character);
            if (TALENTS_PATH != "")
                GetNode<SkillIconList>(TALENTS_PATH).SetTalents(character);
            if (AGE_PATH != "")
                GetNode<Label>(AGE_PATH).Text = character.AgeString();
            if (BIRTH_PATH != "")
                GetNode<Label>(BIRTH_PATH).Text = character.birth.ToString();
            if (ACTION_PATH != "")
                GetNode<Label>(ACTION_PATH).Text = character.Action().ToString();
            if (LOVER_PATH != "")
                GetNode<SmartText>(LOVER_PATH).BbcodeText = "[right]" + (character.lover?.MetaName() ?? "None") + "[/right]";
            if (ITEM_PATH != "")
                GetNode<SmartText>(ITEM_PATH).BbcodeText = "[right]" + (character.heldItem?.MetaName() ?? "None") + "[/right]";
        }
    }
}