using System;
using Godot;

public class Entity : Resource {

    /*** Piece ***/
    [Export] public Combat.Alignment alignment;
    [Export] public bool actor;


    /*** Constant ***/
    [Export] public string name;
    public ElementalAffinity affinity {
        get => ElementalAffinity.Deserialize(_affinity);
        set => _affinity = value.Serialize();
    }
    [Export] private int[] _affinity = ElementalAffinity.RandomAffinity().Serialize();

    [Export] public int maxHealth;
    [Export] public Visual.CharacterAppearanceData appearance;


    /*** Mutables ***/
    [Export] public int health;


    /*** Skills ***/
    // TODO: Move in actors / character class
    [Export] public Skill skillWalk = Skill.Load("walk");
    [Export] public Skill skill1 = Skill.Load("teleport");
    [Export] public Skill skill2 = Skill.Load("point");
    [Export] public Skill skill3;
    [Export] public Skill skillSpecial = null;

    [Export] public Item heldItem = null;

    public bool hasSpecial { get => skillSpecial != null; }

    public int skillCount { get => hasSpecial ? 5 : 4; }

    public Skill[] skills {
        get => hasSpecial ? new Skill[] { skillWalk, skill1, skill2, skill3, skillSpecial } : new Skill[] { skillWalk, skill1, skill2, skill3, skillSpecial };
    }
    public Skill[] coreSkills {
        get => new Skill[] { skill1, skill2, skill3 };
    }

    [Export] private int _birth = (Game.data?.date.SeasonsPassed()).GetValueOrDefault();
    public Date birth {
        get => Date.FromSeasonsPassed(_birth);
        set => _birth = value.SeasonsPassed();
    }
    public int age { get => Date.Delta(birth); }
    public Date.AgeGroup ageGroup { get => Date.Age(age); }
    public string AgeString() {
        return Date.TextAge(age);
    }

    public Entity() : this(Combat.Alignment.NEUTRAL, false, 10) { }

    public Entity(Combat.Alignment alignment, bool actor, int maxHealth,
        string name = null, Visual.CharacterAppearanceData appearance = null, ElementalAffinity affinity = null, Date? birth = null, int? health = null) {
        this.alignment = alignment;
        this.actor = actor;
        this.maxHealth = maxHealth;
        // optional parameters
        if (name != null) {
            this.name = name;
        } else {
            this.name = RandomName();
        }
        if (appearance != null) {
            this.appearance = appearance;
        } else {
            this.appearance = Visual.CharacterAppearanceData.Random();
        }
        if (affinity != null) {
            this.affinity = affinity;
        } else {
            this.affinity = ElementalAffinity.RandomAffinity();
        }
        if (birth.HasValue) {
            this.birth = birth.Value;
        } else {
            Date d = Game.data?.date ?? new Date(0, 0);
            d = d.Plus(Global.rng.Next(-20, 0));
            this.birth = d;
            GD.Print(d, " ", d.year, "-", d.season);
        }
        if (health.HasValue) {
            this.health = health.Value;
        } else {
            this.health = maxHealth;
        }
    }

    [Signal] public delegate void health_modified(int new_health, int delta);
    public void ModifyHealth(int delta) {
        health += delta;
        EmitSignal(nameof(health_modified), health, delta);
    }

    private static string[] NAMES = { "Alice", "Bob", "Charles", "Denis", "Elise", "Felix", "Gwendoline", "Harry", "Isabelle", "John", "Karen", "Louis", "Mary" };
    public static string RandomName() {
        return NAMES[Global.rng.Next(0, NAMES.Length)];
    }
}
