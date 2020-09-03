using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class ObstacleEntity : Entity {
    [Export] public Visual.CharacterAppearanceData appearance; //TODO: Change
    public override AppearanceData GetAppearance() {
        return appearance;
    }

    /*** Skills ***/
    public override IEnumerable<Skill> GetCoreSkills() {
        return GetSkills();
    }

    public ObstacleEntity() : this(Combat.Alignment.NEUTRAL, false, 10) { }

    public ObstacleEntity(Combat.Alignment alignment, bool actor, int maxHealth,
        string name = "Obstacle", Visual.CharacterAppearanceData appearance = null, ElementalAffinity affinity = null, Date? birth = null, int? health = null) {
        this.alignment = alignment;
        this.actor = actor;
        this.maxHealth = maxHealth;
        this.name = name;
        // optional parameters
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
        if (health.HasValue) {
            this.health = health.Value;
        } else {
            this.health = maxHealth;
        }
        //TODO: SkillHandler.FillSkills(this);
    }
}
