using System;
using System.Collections.Generic;
using Godot;


namespace UI {
    public class CharacterSelectorPopup : PopupPanel {
        private Control list;
        private Label comment;
        private Button cancel;

        private bool linked = false;
        public void Link() {
            if (linked) {
                return;
            }
            list = GetNode<Control>("List/Character/Margin/List");
            comment = GetNode<Label>("List/Comment/Label");
            cancel = GetNode<Button>("List/Cancel/Button");
            cancel.Connect("pressed", this, nameof(on_Cancel));
            linked = true;
        }

        public void Setup(bool nullable = false, string comment = null, List<Entity> list = null) {
            Link();
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
            Link();
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
        private void on_Cancel() {
            on_Selected(null);
        }
        private void on_Selected(Entity entity = null) {
            Hide();
            EmitSignal(nameof(selected), entity);
        }

    }
}
