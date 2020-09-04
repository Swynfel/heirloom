using System;
using Godot;

namespace Visual.Tables {
    public class PartyCharacter : CharacterTable {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/PartyCharacter.tscn");
        public static PartyCharacter CreateParty(CharacterEntity entity = null) {
            PartyCharacter character = (PartyCharacter) template.Instance();
            if (entity != null) {
                character.SetCharacter(entity);
            }
            return character;
        }

        private Control selected;

        public override void _Ready() {
            base._Ready();
            selected = GetNode<Control>("Selected");
        }
        public void Toggle(bool on) {
            if (on) {
                selected.Show();
            } else {
                selected.Hide();
            }
        }
    }
}
