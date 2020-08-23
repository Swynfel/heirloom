using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    public class Battle : Node2D {
        public static Battle current = null;

        public Board board { get; private set; }
        public Camera2D camera { get; private set; }

        public List<Piece> pieces { get; } = new List<Piece>();

        public List<Piece> actors { get; } = new List<Piece>();

        public Piece currentActor { get; private set; } = null;


        public override void _Ready() {
            current = this;
            board = new Board();
            AddChild(board);
            camera = GetNode<Camera2D>("Camera");
        }

        private bool battleStarted = false;

        public override void _Process(float delta) {
            if (pendingNextTurn && !Game.busy) {
                pendingNextTurnTimer -= delta;
                if (pendingNextTurnTimer > 0) {
                    return;
                }
                pendingNextTurn = false;
                int i = actors.IndexOf(currentActor);
                TurnOf(actors[(i + 1) % actors.Count]);
            }
        }

        [Signal] public delegate void next_turn(Piece actor);

        private void TurnOf(Piece actor) {
            EmitSignal(nameof(next_turn), actor);
            currentActor = actor;
        }

        private bool pendingNextTurn = false;
        private float pendingNextTurnTimer = 0.5f;

        public void NextTurn() {
            pendingNextTurn = true;
            pendingNextTurnTimer = 0.5f;
        }

        public void StartBattle() {
            battleStarted = true;
            // HACK
            if (Village.quest == null) {
                Village.quest = Game.data.quests[0];
                Village.actions[Family.familyMembers[0]] = VillageAction.QUEST;
            }
            Village.quest.battle.Generate(this, Village.actions.Where(VillageAction.QUEST));
            TurnOf(actors[0]);
            camera.Position = board.GetCenter();
        }

        public static bool won = false;

        public async void CheckIfFinished() {
            bool friends = false;
            bool enemies = false;
            foreach (Piece piece in actors) {
                switch (piece.entity.alignment) {
                    case Alignment.FRIENDLY:
                        if (enemies) return;
                        friends = true;
                        break;
                    case Alignment.HOSTILE:
                        if (friends) return;
                        enemies = true;
                        break;
                }
            }
            won = !enemies; // TODO: Draws?
            Game.StartBusy();
            var t = new Timer();
            t.WaitTime = 2;
            t.OneShot = true;
            AddChild(t);
            t.Start();
            await ToSignal(t, "timeout");
            GetTree().ChangeScene("Scenes/Outcome.tscn");
        }
    }
}
