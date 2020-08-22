using System;
using System.Collections.Generic;
using Godot;


namespace UI {
    public class CharacterSelectorPopup : PopupPanel {
        private Control list;
        private Label comment;
        private Button cancel;

        public override void _Ready() {
            list = GetNode<Control>("List/Character/Margin/List");
            comment = GetNode<Label>("List/Comment/Label");
            cancel = GetNode<Button>("List/Cancel/Button");
            cancel.Connect("pressed", this, nameof(on_Selected));
        }

        public void Setup(bool nullable = false, string comment = null, List<Entity> list = null) {
            cancel.Visible = nullable;
            if (comment == null) {
                this.comment.Hide();
            } else {
                this.comment.Show();
                this.comment.Text = comment;
            }
            Setup(list);
        }

        public void Setup(IEnumerable<Entity> list) {
            this.list.QueueFreeChildren();
            if (list != null) {
                foreach (Entity entity in list) {
                    Button button = Visual.Icons.CharacterIcon.CreateButton(entity);
                    button.Connect("pressed", this, nameof(on_Selected), Global.ArrayFrom(entity));
                    this.list.AddChild(button);
                }
            }
        }

        [Signal] public delegate void selected(Entity entity);

        private void on_Selected(Entity entity = null) {
            Hide();
            EmitSignal(nameof(selected), entity);
        }

    }
}
