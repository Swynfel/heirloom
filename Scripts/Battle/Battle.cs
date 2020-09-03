using System;
using System.Collections.Generic;
using Godot;

namespace Combat {
    public class Battle : Node2D {
        public static Battle current { get; private set; } = null;

        public Board board { get; private set; }
        public Visual.BattleCamera camera { get; private set; }

        public List<Piece> pieces { get; } = new List<Piece>();

        public List<Piece> actors { get; } = new List<Piece>();

        public Piece currentActor { get; private set; } = null;


        public override void _Ready() {
            current = this;
            board = new Board();
            AddChild(board);
            camera = GetNode<Visual.BattleCamera>("Camera");
        }

        public void SetupBattle() {
            // HACK to force a quest for debugging
            if (Village.quest == null) {
                Village.quest = Game.data.quests[0];
                Village.actions[Family.familyMembers[0]] = VillageAction.QUEST;
            }

            // Generate battle
            Village.quest.battle.Generate(this, Village.actions.Where(VillageAction.QUEST));

            // Ready
            foreach (Piece actor in actors) {
                foreach (Skill skill in actor.entity.GetSkills()) {
                    skill?.condition.ResetCombat();
                }
            }

            // Start battle
            TurnOf(actors[0]);
            camera.Position = board.GetCenter();
            battleStarted = true;
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
            foreach (Skill skill in actor.entity.GetSkills()) {
                skill?.condition.ResetTurn();
            }
            EmitSignal(nameof(next_turn), actor);
            currentActor = actor;
        }

        private const float NEXT_TURN_TIMER = 0.2f;

        private bool pendingNextTurn = false;
        private float pendingNextTurnTimer = 0.5f;

        public void NextTurn() {
            pendingNextTurn = true;
            pendingNextTurnTimer = NEXT_TURN_TIMER;
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
