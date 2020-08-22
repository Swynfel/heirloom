using System;
using System.Collections.Generic;
using Godot;
using SpriteTemplate = Visual.Icons.SkillIcon.SpriteTemplate;

public class Item : Resource {
    public enum Group {
        NONE,
        ARTEFACT,
        TREASURE,
        EQUIPMENT,
        CONSUMABLE,
    }

    [Export] public string name = "ITEM";
    [Export] public SpriteTemplate icon = SpriteTemplate.BUBBLE;
    [Export] public Group group = Group.NONE;
    public bool equipable { get => group.IsEquipable(); }
    public bool consumable { get => group == Group.CONSUMABLE; }
    [Export] public Skill skill = null;

    [Export] public float bonusDamage = 0f;
    [Export] public int armor = 0;
    [Export] public int estimatedPrice = 30;
    [Export] public string effect = "";
    [Export] public string description = "";

    [Export] public Entity holder = null;
    [Export] public Entity lastHolder = null;

    public Item() { }
    public Item(string name, SpriteTemplate icon, Group group, Skill skill = null, float bonusDamage = 0f, int armor = 0, string effect = "", string description = "") {
        this.name = name;
        this.icon = icon;
        this.group = group;
        this.skill = skill;
        this.bonusDamage = bonusDamage;
        this.armor = armor;
        this.effect = effect;
        this.description = description;
    }

    public static Item ARTEFACT_SWORD = new Item("Legendary Sword", SpriteTemplate.SWORD, Group.ARTEFACT,
        bonusDamage: 0.2f, effect: "+20% damage",
        description: "legendary sword passed down through your family since the beginning of time"
    );

    public static Item ARTEFACT_CROWN = new Item("Mystical Crown", SpriteTemplate.CROWN, Group.ARTEFACT,
        armor: 1, effect: "+1 armor",
        description: "magical crown that grants infinite wisdom"
    );

    public static Item ARTEFACT_SHIELD = new Item("Divine Shield", SpriteTemplate.CROWN, Group.ARTEFACT,
        armor: 2, effect: "+2 armor",
        description: "it is said that the wielder and their loved ones cannot die"
    );

    public static Item DAGGER = new Item("Dagger", SpriteTemplate.SWORD, Group.EQUIPMENT,
        armor: 1, effect: "+10% damage",
        description: "it's sharp, more or less"
    );

    public static Item FAN = new Item("Fan", SpriteTemplate.CAST_BALL, Group.EQUIPMENT,
        effect: "none",
        description: "no effect, but its neat"
    );

    public static Item VASE = new Item("Fancy Vase", SpriteTemplate.CAST_BALL, Group.TREASURE,
        description: "you could sell it!"
    );
}

public static class ItemExtensions {
    public static bool IsEquipable(this Item.Group group) {
        switch (group) {
            case Item.Group.ARTEFACT:
            case Item.Group.EQUIPMENT:
                return true;
            default:
                return false;
        }
    }
}