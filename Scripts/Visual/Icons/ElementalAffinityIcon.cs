using System;
using Godot;

namespace Visual.Icons {
    public class ElementalAffinityIcon : HBoxContainer {
        public ElementIconList resistance { get; private set; }
        public ElementIconList weakness { get; private set; }
        public override void _Ready() {
            CheckSetup();
        }

        private bool setup = false;

        private void CheckSetup() {
            if (setup) {
                return;
            }
            setup = true;
            resistance = GetNode<ElementIconList>("Resistances");
            weakness = GetNode<ElementIconList>("Weaknesses");
        }
        public void SetAffinity(ElementalAffinity affinity) {
            CheckSetup();
            resistance.SetElements(affinity.GetResistances());
            weakness.SetElements(affinity.GetWeaknesses());
        }
    }
}
