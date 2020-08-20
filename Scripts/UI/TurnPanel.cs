using System;
using Combat;
using Godot;

namespace UI {
    public class TurnPanel : Control {

        private VBoxContainer turnCharacters;

        public override void _Ready() {
            turnCharacters = GetNode<VBoxContainer>("ScrollContainer/TurnCharacterContainer");
            Global.battle.Connect(nameof(Battle.next_turn), this, nameof(UpdateTurn));
        }
        public void UpdateTurn(Piece nextActor) {
            turnCharacters.QueueFreeChildren();
            bool shouldAdd = false;
            foreach (Piece actor in Global.battle.actors) {
                if (nextActor == actor) {
                    shouldAdd = true;
                }
                if (shouldAdd) {
                    turnCharacters.AddChild(TurnCharacter.Create(actor));
                }
            }
            foreach (Piece actor in Global.battle.actors) {
                if (nextActor == actor) {
                    return;
                }
                turnCharacters.AddChild(TurnCharacter.Create(actor));
            }
        }
    }
}
