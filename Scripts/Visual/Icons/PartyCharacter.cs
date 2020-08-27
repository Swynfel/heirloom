using System;
using Godot;

namespace Visual.Icons {
    public class PartyCharacter : Button {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/PartyCharacter.tscn");
        public static PartyCharacter Create(Entity entity = null) {
            PartyCharacter character = (PartyCharacter) template.Instance();
            if (entity != null) {
                character.ConfigureEntity(entity);
            }
            return character;
        }

        private bool setup = false;
        private Label name;
        private CharacterAppearance character;
        private HealthBar health;
        private ElementalAffinityIcon affinity;
        private Control selected;

        private void TrySetup() {
            if (!setup) {
                setup = true;
                name = GetNode<Label>("List/Name");
                character = GetNode<CharacterAppearance>("List/Center/Block/Character");
                health = GetNode<HealthBar>("List/HealthHolder/HealthBar");
                affinity = GetNode<ElementalAffinityIcon>("List/Elements");
                selected = GetNode<Control>("Selected");
            }
        }

        public void ConfigureEntity(Entity entity) {
            TrySetup();
            name.Text = entity.name;
            character.data = entity.appearance;
            health.SetHealth(entity.health, entity.maxHealth);
            affinity.SetAffinity(entity.affinity);
        }

        public void Toggle(bool on) {
            TrySetup();
            if (on) {
                selected.Show();
            } else {
                selected.Hide();
            }
        }
    }
}
