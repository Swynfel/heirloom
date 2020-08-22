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
            switch (e.ageGroup) {
                case Date.AgeGroup.BABY:
                case Date.AgeGroup.CHILD:
                    food++;
                    break;
                case Date.AgeGroup.TEEN:
                    food += 2;
                    break;
                case Date.AgeGroup.YOUNG_ADULT:
                case Date.AgeGroup.ADULT:
                    food += 3;
                    break;
                case Date.AgeGroup.SENIOR:
                    food += 2;
                    break;
            }
        }
        return food;
    }
}