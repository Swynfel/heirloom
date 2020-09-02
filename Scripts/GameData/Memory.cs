using System;
using System.Collections.Generic;
using Godot;

public class Memory : Resource {

    [Export] private List<Entity> characters = new List<Entity>();
    [Export] private List<Item> items = new List<Item>();

    public enum Group {
        CHARACTER = 'C',
        ITEMS = 'I',
        INFO = '?',
    }

    public const string BBCODE = "[rem][url={1}]{0}[/url][/rem]";

    public struct MetaTag {
        public Group group;
        public string key;
        public int id => int.Parse(key);
        public string Box(string s) {
            return string.Format(BBCODE, s, this);
        }

        public override string ToString() {
            return ((char) group) + id.ToString();
        }

        public static MetaTag Parse(string s) {
            return new MetaTag() {
                group = (Group) s[0],
                key = s.Substring(1),
            };
        }

        public Resource Remember() {
            return Memory.memory.Remember(this);
        }
        public Entity RememberEntity() {
            return Memory.memory.RememberEntity(this);
        }
        public Item RememberItem() {
            return Memory.memory.RememberItem(this);
        }
        public InfoText RememberInfo() {
            return InfoText.Find(key);
        }
    }

    public static Memory memory => Game.data.memory;

    public MetaTag Tag(Entity entity) {
        if (entity.rememberId == -1) {
            entity.rememberId = characters.Count;
            characters.Add(entity);
        }
        return new MetaTag {
            group = Group.CHARACTER,
            key = entity.rememberId.ToString(),
        };
    }
    public MetaTag Tag(Item item) {
        if (item.rememberId == -1) {
            item.rememberId = items.Count;
            items.Add(item);
        }
        return new MetaTag {
            group = Group.ITEMS,
            key = item.rememberId.ToString(),
        };
    }

    public Resource Remember(MetaTag meta) {
        switch (meta.group) {
            case Group.CHARACTER:
                return RememberEntity(meta);
            case Group.ITEMS:
                return RememberItem(meta);
            case Group.INFO:
                return RememberInfo(meta);
            default:
                return null;
        }
    }

    public Entity RememberEntity(MetaTag meta) {
        return characters[meta.id];
    }
    public Item RememberItem(MetaTag meta) {
        return items[meta.id];
    }
    public InfoText RememberInfo(MetaTag meta) {
        return InfoText.Find(meta.key);
    }
}