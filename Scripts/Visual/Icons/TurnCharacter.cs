using System;
using Godot;

namespace Visual.Icons {
    public class TurnCharacter : HBoxContainer {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/TurnCharacter.tscn");
        public static TurnCharacter Create(Combat.Piece piece) {
            TurnCharacter character = (TurnCharacter) template.Instance();
            character.Configure(piece);
            return character;
        }

        private bool setup = false;
        private Label name;
        private Control icon;
        private HealthBar health;
        private ElementalAffinityIcon affinity;

        private void TrySetup() {
            if (!setup) {
                setup = true;
                name = GetNode<Label>("Left/NameHolder/Name");
                icon = GetNode<Control>("Right/Icon");
                GetNode<Button>("Right").Connect("pressed", this, nameof(OpenCharacter));
                health = GetNode<HealthBar>("Left/HealthHolder/HealthBar");
                affinity = GetNode<ElementalAffinityIcon>("Left/Elements");
            }
        }

        private Entity entity;
        public void OpenCharacter() {
            MetaPopup.instance.OpenEntity(entity);
        }

        public override void _Ready() {
            TrySetup();
        }
        public void Configure(Combat.Piece piece) {
            TrySetup();
            entity = piece.entity;
            name.Text = piece.entity.name;
            piece.entity.GetAppearance().GenerateOnWithOutline(icon, piece.entity.alignment).Position = Vector2.Zero;
            health.SetHealth(piece.entity.health, piece.entity.maxHealth);
            piece.entity.Connect(nameof(CharacterEntity.health_modified), health, nameof(HealthBar.ChangeHealth));
            affinity.SetAffinity(piece.entity.affinity);
        }

        private static Vector2 BIG_ICON = new Vector2(40, 40);
        public void Grow() {
            GetNode<Control>("Right").RectMinSize = BIG_ICON;
            GetNode<VBoxContainer>("Left").AddConstantOverride("separation", 4);
            AddConstantOverride("separation", 6);
            health.MarginLeft -= 20;
        }
    }
}
