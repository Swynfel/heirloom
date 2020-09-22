using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Family : Resource, ISaveable {

    public static List<CharacterEntity> familyMembers { get { return Game.data.family.alive; } }
    public HashSet<CharacterEntity> members = new HashSet<CharacterEntity>();

    public List<CharacterEntity> alive = new List<CharacterEntity>();

    public Family() { }
    public Family(IEnumerable<CharacterEntity> members) {
        AddRange(members);
    }

    public void Add(CharacterEntity entity) {
        entity.alignment = Combat.Alignment.FRIENDLY;
        members.Add(entity);
        int index = alive.Select(e => -e.age).ToList().BinarySearch(-entity.age);
        if (index < 0) {
            index = -index - 1;
        }
        alive.Insert(index, entity);
    }

    public void Die(CharacterEntity entity) {
        members.Add(entity);
        alive.Remove(entity);
        entity.death = Game.data.date;
        entity.heldItem = null; // TODO: Something cleaner: Popup to ask heir ?
    }

    public void AddRange(IEnumerable<CharacterEntity> entities) {
        foreach (CharacterEntity e in entities) {
            Add(e);
        }
    }

    private static IEnumerable<CharacterEntity> RandomMembers(int memberCount) {
        while (memberCount > 0) {
            yield return new CharacterEntity();
            memberCount--;
        }
    }
    public static Family RandomFamily(int memberCount) {
        return new Family(RandomMembers(memberCount));
    }

    public int FoodConsumption() {
        int food = 0;
        foreach (CharacterEntity e in members) {
            food += FoodConsumption(e);
        }
        return food;
    }
    public static int FoodConsumption(CharacterEntity e) {
        switch (e.ageGroup) {
            case Date.AgeGroup.BABY:
                return 2;
            case Date.AgeGroup.CHILD:
                return 3;
            case Date.AgeGroup.TEEN:
                return 5;
            case Date.AgeGroup.YOUNG_ADULT:
            case Date.AgeGroup.ADULT:
                return 4;
            case Date.AgeGroup.SENIOR:
                return 3;
            default:
                return 0;
        }
    }

    public static Family StartingFamily() {
        CharacterEntity eldest = new CharacterEntity();
        eldest.SetAge(9);
        CharacterEntity middle = new CharacterEntity();
        middle.SetAge(7);
        CharacterEntity youngest = new CharacterEntity();
        youngest.SetAge(5);
        return new Family(new CharacterEntity[] { eldest, middle, youngest });
    }

    public void SaveExtraData(Godot.Collections.Dictionary<string, object> data) {
        data["members"] = members.Select(d => d.rememberId).ToList();
        data["alive"] = alive.Select(d => d.rememberId).ToList();
    }
    public void LoadExtraData(Godot.Collections.Dictionary<string, object> data) {
        members = new HashSet<CharacterEntity>((data["members"] as Godot.Collections.Array<int>).Select(d => Memory.memory.characters[d]));
        alive = new List<CharacterEntity>((data["alive"] as Godot.Collections.Array<int>).Select(d => Memory.memory.characters[d]));

    }
}