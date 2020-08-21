using System;
using System.Collections.Generic;
using Godot;

public class Family : Resource {
    [Export] public HashSet<Entity> members = new HashSet<Entity>();

    public Family() { }
    public Family(IEnumerable<Entity> members) {
        this.members.UnionWith(members);
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
}