using System;
using System.Collections.Generic;
using Godot;
using SpriteTemplate = Visual.Icons.SkillIcon.SpriteTemplate;

public class Item : Resource, IModifier {
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

    [Export] public float bonusDamagePer = 0f;
    [Export] public int bonusDamage = 0;
    [Export] public int armor = 0;
    [Export] public int estimatedPrice = 30;
    [Export] public string effect = "";
    [Export] public string description = "";
    [Export] public string special = "";

    public int GetMod(Modifier mod) {
        switch (mod) {
            case Modifier.BONUS_DAMAGE:
                return bonusDamage;
            case Modifier.ARMOR:
                return armor;
            default:
                return 0;
        }
    }

    [Save] public CharacterEntity holder = null;
    [Save] public CharacterEntity lastHolder = null;

    [Save] public int rememberId = -1;
    public string MetaName() {
        return Memory.memory.Tag(this).Box(name);
    }

    public void SetHolder(CharacterEntity entity) {
        if (holder != null) {
            holder.heldItem = null;
        }
        holder = entity;
        if (entity != null) {
            entity.heldItem = this;
        }
    }
    public static Item ARTEFACT_SWORD = new Item {
        name = "Legendary Sword",
        icon = SpriteTemplate.SWORD,
        group = Group.ARTEFACT,
        bonusDamage = 3,
        effect = "+3 damage",
        description = "legendary sword passed down through your family since the beginning of time"
    };

    public static Item ARTEFACT_CROWN = new Item {
        name = "Mystical Crown",
        icon = SpriteTemplate.CROWN,
        group = Group.ARTEFACT,
        special = "crown",
        effect = "+1 max health per season",
        description = "magical crown said to grant eternal life"
    };

    public static Item ARTEFACT_SHIELD = new Item {
        name = "Divine Shield",
        icon = SpriteTemplate.SHIELD,
        group = Group.ARTEFACT,
        armor = 2,
        effect = "+2 armor",
        description = "protects the wielder even against the worst hazards"
    };

    public static Item DAGGER = new Item {
        name = "Dagger",
        icon = SpriteTemplate.SWORD,
        group = Group.EQUIPMENT,
        estimatedPrice = 60,
        bonusDamage = 1,
        effect = "+1 damage",
        description = "it's sharp, more or less"
    };

    public static Item FAN = new Item {
        name = "Fan",
        icon = SpriteTemplate.CAST_BALL,
        group = Group.EQUIPMENT,
        effect = "none",
        description = "no effect, but its neat"
    };

    public static Item VASE = new Item {
        name = "Fancy Vase",
        icon = SpriteTemplate.CAST_BALL,
        group = Group.TREASURE,
        description = "you could sell it!"
    };
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