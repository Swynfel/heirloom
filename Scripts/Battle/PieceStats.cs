
using System;
using Godot;

public class PieceStats : Resource {

    // Piece
    [Export] public Combat.Alignment alignment;
    [Export] public bool actor;


    // Constant
    public ElementalAffinity affinity;
    [Export] public int maxHealth;

    [Export] public int maxMovement;

    // Mutable
    [Export] public int health;

    public PieceStats() : this(Combat.Alignment.NEUTRAL, false, 10, 3) { }

    public PieceStats(Combat.Alignment alignment, bool actor, int maxHealth, int maxMovement,
        ElementalAffinity affinity = null, int? health = null) {
        this.alignment = alignment;
        this.actor = actor;
        this.maxHealth = maxHealth;
        this.maxMovement = maxMovement;
        if (health.HasValue) {
            this.health = health.Value;
        } else {
            this.health = maxHealth;
        }
        if (affinity != null) {
            this.affinity = affinity;
        } else {
            this.affinity = ElementalAffinity.RandomAffinity();
        }
    }

    [Signal] public delegate void health_modified(int new_health, int delta);
    public void ModifyHealth(int delta) {
        health += delta;
        EmitSignal(nameof(health_modified), health, delta);
    }
}
