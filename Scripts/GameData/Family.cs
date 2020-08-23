using System;
using System.Collections.Generic;
using Godot;

public class Family : Resource {

    public static List<Entity> familyMembers { get { return Game.data.family.alive; } }
    [Export] public HashSet<Entity> members = new HashSet<Entity>();

    [Export] public List<Entity> alive = new List<Entity>();

    public Family() { }
    public Family(IEnumerable<Entity> members) {
        AddRange(members);
    }

    public void Add(Entity entity) {
        members.Add(entity);
        alive.Add(entity);
    }

    public void Die(Entity entity) {
        members.Add(entity);
        alive.Remove(entity);
        entity.death = Game.data.date;
        entity.heldItem = null; // TODO: Something cleaner: Popup to ask heir ?
    }

    public void AddRange(IEnumerable<Entity> entities) {
        foreach (Entity e in entities) {
            Add(e);
        }
    }

    private static IEnumerable<Entity> RandomMembers(int memberCount) {
        while (memberCount > 0) {
            yield return new Entity();
            memberCount--;
        }
    }
    public static Family RandomFamily(int memberCount) {
        return new Family(RandomMembers(memberCount));
    }

    public int FoodConsumption() {
        int food = 0;
        foreach (Entity e in members) {
            food += FoodConsumption(e);
        }
        return food;
    }
    public static int FoodConsumption(Entity e) {
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
        Entity eldest = new Entity();
        Entity middle = new Entity();
        Entity youngest = new Entity();
        return new Family(new Entity[] { eldest, middle, youngest });
    }
}