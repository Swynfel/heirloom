using Combat.Generate;
using Godot;

public class Quest : Resource {
    /*** Constant ***/
    [Save] [Export] public string name = "Super Epic Quest of Coolness";
    [Save] [Export] public QuestReward reward = new QuestReward();
    [Save] [Export] public Date deadline = Game.data?.date ?? Date.NEVER;
    [Save] [Export] public BattleGeneration battle;
    [Save] [Export] public string difficulty;
    [Save] [Export] public int partySize = 2;

}
