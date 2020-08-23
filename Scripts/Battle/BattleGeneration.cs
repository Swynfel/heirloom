using System;
using System.Collections.Generic;
using System.Linq;
using Combat;
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
    [Export] public int count = 1;
    public ElementalAffinity elements {
        get => ElementalAffinity.Deserialize(_elements);
        set => _elements = value.Serialize();
    }
    [Export] private int[] _elements = ElementalAffinity.RandomAffinity().Serialize();


    public void Generate(Battle battle, IEnumerable<Entity> party) {
        Board board = battle.board;
        List<Entity> friendly = party.ToList();
        int size = (count + friendly.Count) / 3;
        int width = 5 + size / 2;
        int height = 4;
        int f = Global.rng.Next(0, size + 1);
        width += f;
        height += size - f;
        board.width = Global.rng.Next(5 + size, 11);
        board.height = Global.rng.Next(4 + size, 7);
        board.CreateTerrain((x, y) => Tile.GroundType.GRASS);

        List<Tile> tiles = new List<Tile>(board.tiles);

        foreach (Entity friend in friendly) {
            friend.actor = true;
            friend.alignment = Alignment.FRIENDLY;
            Tile tile = tiles.PopRandom();
            foreach (Tile n in tile.GetNeighbors()) {
                tiles.Remove(n);
            }
            Place(friend, tile);
        }

        foreach (Entity enemy in RandomEnemies(count)) {
            enemy.actor = true;
            enemy.alignment = Alignment.HOSTILE;
            Tile tile = tiles.PopRandom();
            foreach (Tile n in tile.GetNeighbors()) {
                tiles.Remove(n);
            }
            Place(enemy, tile);
        }
    }

    private static IEnumerable<Entity> RandomEnemies(int memberCount) {
        while (memberCount > 0) {
            yield return new Entity();
            memberCount--;
        }
    }

    public void Place(Entity entity, Tile tile) {
        Piece.Create(Battle.current, entity, tile);
    }
}