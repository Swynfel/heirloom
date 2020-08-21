using System;
using Godot;

namespace Visual.Icons {
    public class HealthBar : NinePatchRect {
        private Label label;
        private NinePatchRect content;
        public override void _Ready() {
            label = GetNode<Label>("Label");
            content = GetNode<NinePatchRect>("Content/Bar");
            SetHealth(currentHealth, maxHealth);
        }

        [Export] private int maxHealth = 50;
        [Export] private int currentHealth = 42;

        public void SetHealth(int currentHealth, int maxHealth) {
            //Not loaded yet
            if (content == null) {
                this.maxHealth = maxHealth;
                this.currentHealth = currentHealth;
                return;
            }
            content.AnchorRight = Math.Max(0, Math.Min(1, ((float) currentHealth) / maxHealth));
            label.Text = string.Format("{0}/{1}", currentHealth, maxHealth);
        }
        public void SetCurrentHealth(int currentHealth) {
            SetHealth(currentHealth, maxHealth);
        }
        public void SetMaxHealth(int maxHealth) {
            SetHealth(currentHealth, maxHealth);
        }
    }
}
