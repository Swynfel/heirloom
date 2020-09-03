using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Entity : Resource {

    /*** Piece ***/
    [Export] public Combat.Alignment alignment;
    [Export] public bool actor = true;


    /*** Constant ***/
    [Export] public string name;

    public string MetaName() {
        return Memory.memory.Tag(this).Box(name);
    }

    public static string MetaNames(IEnumerable<Entity> entities) {
        return entities.Select(e => e.MetaName()).FancyJoin("nobody");
    }

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
    [Export] public Skill skillWalk = SkillHandler.WALK;
    [Export] public Skill skill1;
    [Export] public Skill skill2;
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

    [Export] private int _death = Date.NEVER.SeasonsPassed();
    [Export] public Entity lover = null;
    [Export] public int rememberId = -1;
    public Date birth {
        get => Date.FromSeasonsPassed(_birth);
        set => _birth = value.SeasonsPassed();
    }

    public Date death {
        get => Date.FromSeasonsPassed(_death);
        set => _death = value.SeasonsPassed();
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
            Date d = Game.data?.date ?? Date.START;
            d = d.Plus(Global.rng.Next(-16, +1));
            this.birth = d;
        }
        if (health.HasValue) {
            this.health = health.Value;
        } else {
            this.health = maxHealth;
        }
        SkillHandler.FillSkills(this);
    }

    public void SetAge(int age) {
        birth = Game.data.date.Plus(-age);
        maxHealth = HealthAtAge(Date.Age(age));
        health = maxHealth;
    }

    public static Entity FromBirth(Entity parentA, Entity parentB) {
        Entity baby = new Entity();
        baby.SetAge(0);
        baby.skill1 = parentA.coreSkills.Random().Clone();
        baby.skill2 = parentB.coreSkills.Where(s => SkillHandler.AreCompatible(s, baby.skill1)).Random().Clone();
        baby.skill3 = null;
        baby.skill3 = SkillHandler.FindRandomSkillFor(baby);
        return baby;
    }

    public static Entity Orphan() {
        Entity orphan = new Entity();
        orphan.SetAge(Global.rng.Next(1, 4));
        return orphan;
    }

    public static int HealthAtAge(Date.AgeGroup age, int shake = 3) {
        if (shake > 0) {
            return HealthAtAge(age, 0) + Global.rng.Next(-shake, shake + 1);
        }
        switch (age) {
            case Date.AgeGroup.BABY:
                return 5;
            case Date.AgeGroup.CHILD:
                return 9;
            case Date.AgeGroup.TEEN:
                return 13;
            case Date.AgeGroup.YOUNG_ADULT:
                return 15;
            case Date.AgeGroup.ADULT:
                return 14;
            case Date.AgeGroup.SENIOR:
                return 10;
            default:
                return 5;
        }
    }

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
    public static string RandomName() {
        return NameGenerator.RandomName();
    }
}
