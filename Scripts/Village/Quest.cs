
using System;
using Godot;

public class Quest : Resource {
    /*** Constant ***/
    [Export] public string name = "Super Epic Quest of Coolness";
    [Export] public string reward = "Nothing";
    [Export] private int _deadline = (Game.data?.date.SeasonsPassed()).GetValueOrDefault();
    public Date deadline {
        get => Date.FromSeasonsPassed(_deadline);
        set => _deadline = value.SeasonsPassed();
    }
    [Export] public string difficulty;
    public ElementalAffinity elements {
        get => ElementalAffinity.Deserialize(_elements);
        set => _elements = value.Serialize();
    }
    [Export] private int[] _elements = ElementalAffinity.RandomAffinity().Serialize();
    [Export] public int partySize = 2;

}
