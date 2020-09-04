
using System;
using Combat.Generate;
using Godot;

public class Quest : Resource {
    /*** Constant ***/
    [Export] public string name = "Super Epic Quest of Coolness";
    [Export] public QuestReward reward = new QuestReward();
    [Export] private int _deadline = (Game.data?.date.SeasonsPassed()).GetValueOrDefault();
    public Date deadline {
        get => Date.FromSeasonsPassed(_deadline);
        set => _deadline = value.SeasonsPassed();
    }
    [Export] public BattleGeneration battle;
    [Export] public string difficulty;
    [Export] public int partySize = 2;

}
