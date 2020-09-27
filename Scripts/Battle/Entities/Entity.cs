using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class Entity : Resource, IModifier {

    /*** Piece ***/
    public Combat.Alignment alignment;
    [IgnoreSave] [Export] public bool actor = true;
    public abstract AppearanceData GetAppearance();


    /*** Variables ***/
    [Export] public string name;

    [Export] public ElementalAffinity affinity = ElementalAffinity.RandomAffinity();


    /*** Health ***/
    [Export] public int maxHealth;
    [Export] public int health;
    [Signal] public delegate void health_modified(int new_health, int delta);
    [Signal] public delegate void fallen();

    public void ModifyHealth(int delta) {
        int newHealth = Math.Max(0, Math.Min(health + delta, maxHealth));
        delta = newHealth - health;
        health = newHealth;
        EmitSignal(nameof(health_modified), health, delta);
        if (health == 0) {
            EmitSignal(nameof(fallen));
        }
    }
    public int ModifyHealthSimulation(int delta) {
        int newHealth = Math.Max(0, Math.Min(health + delta, maxHealth));
        delta = newHealth - health;
        return delta;
    }


    /*** Skills ***/
    [Export] public List<Skill> Skills;
    public Skill GetSkillAtIndexOrNull(int index) {
        if (Skills == null || Skills.Count <= index) {
            return null;
        }
        return Skills[index];
    }
    public void SetSkillAtIndex(int index, Skill value) {
        if (Skills == null) {
            Skills = new List<Skill> { };
        }
        while (Skills.Count <= index) {
            Skills.Add(null);
        }
        Skills[index] = value;
    }
    public IEnumerable<Skill> GetSkills() {
        return Skills.Where(s => s != null);
    }
    public virtual IEnumerable<Skill> GetMoveSkills() {
        yield break;
    }
    public virtual IEnumerable<Skill> GetCoreSkills() {
        yield break;
    }

    public virtual IEnumerable<IModifier> GetModifiers() {
        yield break;
    }

    public int GetMod(Modifier mod) {
        return GetModifiers().Select(m => m.GetMod(mod)).Sum();
    }
}