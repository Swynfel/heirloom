using System;
using Godot;

public class AdventureProgress : Resource {
    [Export] public bool foundCrownQuest = false;
    [Export] public bool foundShieldQuest = false;
    [Export] public bool foundFinalQuest = false;
    [Export] public int dungeonsFound = 0;
}