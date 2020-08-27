using System;
using Godot;

namespace Visual.Icons {
    public class CharacterIcon : CenterContainer {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/CharacterIcon.tscn");
        private static PackedScene buttonTemplate = (PackedScene) ResourceLoader.Load("res://Nodes/UI/Buttons/CharacterButton.tscn");
        public static Button CreateButton(Entity character) {
            Button button = (Button) buttonTemplate.Instance();
            button.GetNode<CharacterIcon>("Icon").SetCharacter(character);
            return button;
        }
        public static CharacterIcon Create(Entity character) {
            CharacterIcon icon = (CharacterIcon) template.Instance();
            icon.SetCharacter(character);
            return icon;
        }
        public void SetCharacter(Entity character) {
            if (character != null) {
                GetNode<Label>("List/Name").Text = character.name;
                GetNode<CharacterAppearance>("List/Center/Block/Character").Show();
                GetNode<CharacterAppearance>("List/Center/Block/Character").data = character.appearance;
            } else {
                GetNode<Label>("List/Name").Text = "None";
                GetNode<CharacterAppearance>("List/Center/Block/Character").Hide();
            }
        }
    }
}