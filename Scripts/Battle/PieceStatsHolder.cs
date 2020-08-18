using System;
using Godot;

namespace Combat {
    class PieceStatsHolder : Node2D {
        [Export] public Combat.Alignment alignment;
        [Export] public bool actor;

        [Export] public int maxHealth;

        [Export] public int maxMovement;

        public PieceStats ConvertAndDelete() {
            PieceStats stats = new PieceStats();
            stats.alignment = alignment;
            stats.actor = actor;
            stats.maxHealth = maxHealth;
            stats.maxMovement = maxMovement;
            CallDeferred("free");
            return stats;
        }
    }
}