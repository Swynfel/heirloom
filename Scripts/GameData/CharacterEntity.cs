using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class CharacterEntity : Entity {

    /*** Constant ***/

    public string MetaName() {
        return Memory.memory.Tag(this).Box(name);
    }

    public static string MetaNames(IEnumerable<CharacterEntity> entities) {
        return entities.Select(e => e.MetaName()).FancyJoin("nobody");
    }

    [Save] public Visual.CharacterAppearanceData appearance;

    /*** Skills ***/

    public Skill skillWalk {
        get => GetSkillAtIndexOrNull(0);
        set => SetSkillAtIndex(0, value);
    }

    public Skill skill1 {
        get => GetSkillAtIndexOrNull(1);
        set => SetSkillAtIndex(1, value);
    }

    public Skill skill2 {
        get => GetSkillAtIndexOrNull(2);
        set => SetSkillAtIndex(2, value);
    }

    public Skill skill3 {
        get => GetSkillAtIndexOrNull(3);
        set => SetSkillAtIndex(3, value);
    }

    public Skill skillSpecial {
        get => GetSkillAtIndexOrNull(4);
        set => SetSkillAtIndex(4, value);
    }
    [Export] public Item heldItem = null;

    public bool hasSpecial { get => skillSpecial != null; }

    public override AppearanceData GetAppearance() {
        return appearance;
    }
    public override IEnumerable<Skill> GetMoveSkills() {
        yield return skillWalk;
    }

    public override IEnumerable<Skill> GetCoreSkills() {
        yield return skill1;
        yield return skill2;
        yield return skill3;
    }

    public override IEnumerable<IModifier> GetModifiers() {
        if (heldItem != null) yield return heldItem;
    }
    [Save] public Date birth = Game.data?.date ?? Date.NEVER;

    [Save] public Date death = Date.NEVER;
    [Save] public CharacterEntity lover = null;
    [Save] public int rememberId = -1;

    public int age { get => Date.Delta(birth); }
    public Date.AgeGroup ageGroup { get => Date.Age(age); }
    public string AgeString() {
        return Date.TextAge(age);
    }

    public CharacterEntity() : this(Combat.Alignment.NEUTRAL, false, 10) { }

    public CharacterEntity(Combat.Alignment alignment, bool actor, int maxHealth,
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
        skillWalk = SkillHandler.WALK.Clone();
        SkillHandler.FillSkills(this);
    }

    public void SetAge(int age) {
        birth = Game.data.date.Plus(-age);
        maxHealth = HealthAtAge(Date.Age(age));
        health = maxHealth;
    }

    public static CharacterEntity FromBirth(CharacterEntity parentA, CharacterEntity parentB) {
        CharacterEntity baby = new CharacterEntity();
        baby.SetAge(0);
        baby.skill1 = parentA.GetCoreSkills().Random().Clone();
        baby.skill2 = parentB.GetCoreSkills().Where(s => SkillHandler.AreCompatible(s, baby.skill1)).Random().Clone();
        baby.skill3 = null;
        baby.skill3 = SkillHandler.FindRandomSkillFor(baby);
        return baby;
    }

    public static CharacterEntity Orphan() {
        CharacterEntity orphan = new CharacterEntity();
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
    public static string RandomName() {
        return Utils.NameGenerator.RandomName();
    }
}
