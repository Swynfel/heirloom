using System;
using Godot;
public class BattleGeneration : Resource {
    public enum Map {
        PLAINS,
        DIRT_ROAD,
        STONE,
        DIRT_CAVE,
        STONE_CAVE,
        HIDEOUT,
    }

    public enum EnemyType {
        BANDITS
    }

    public BattleGeneration() { }

    public BattleGeneration(Map map, EnemyType enemyType, int count, ElementalAffinity elements) {
        this.map = map;
        this.enemy = enemyType;
        this.count = count;
        this.elements = elements;
    }

    [Export] public Map map;
    [Export] public EnemyType enemy;
    [Export] public int count;
    public ElementalAffinity elements {
        get => ElementalAffinity.Deserialize(_elements);
        set => _elements = value.Serialize();
    }
    [Export] private int[] _elements = ElementalAffinity.RandomAffinity().Serialize();

}