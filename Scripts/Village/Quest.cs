using Combat.Generate;
using Godot;

public class Quest : Resource {
    /*** Constant ***/
    [Export] public string name = "Super Epic Quest of Coolness";
    [Export] public QuestReward reward = new QuestReward();
    [Export] public Date deadline = Game.data?.date ?? Date.NEVER;
    [Export] public BattleGeneration battle;
    [Export] public string difficulty;
    [Export] public int partySize = 2;

}
