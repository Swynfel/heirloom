using System;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Godot;
public class BattleGeneration : Resource {
    public enum EnemyType {
        BANDITS
    }

    public BattleGeneration() { }

    public BattleGeneration(MapType map, EnemyType enemyType, int count, ElementalAffinity elements, string enemyNoun = null) {
        this.map = map;
        this.enemy = enemyType;
        this.enemyNoun = enemyNoun;
        this.count = count;
        this.elements = elements;
    }

    [Export] public MapType map;
    [Export] public EnemyType enemy;
    [Export] public string enemyNoun;
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
        width += Global.rng.Next(0, 3 + size / 2);
        height += Global.rng.Next(0, 1 + size / 2);
        while (!GenerateInternal(board, friendly, width, height)) {
            width += 1;
            height += 1;
        }

    }

    private bool GenerateInternal(Board board, List<Entity> friendly, int width, int height) {
        try {
            board.width = width;
            board.height = height;
            board.CreateTerrain(
                BoardGeneration.Build(map, width, height)
            );

            List<Tile> tiles = new List<Tile>(board.Tiles);

            // Check it tiles are all connected
            int totalTiles = tiles.Count;
            int totalConnected = BoardUtils.AllReachableTiles(tiles[0]).Count;
            if (totalTiles != totalConnected) {
                return false;
            }

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
                Tile tile = tiles.PopRandom();
                foreach (Tile n in tile.GetNeighbors()) {
                    tiles.Remove(n);
                }
                Place(enemy, tile);
            }
            return true;
        } catch {
            board.Clear();
            return false;
        }
    }

    private static IEnumerable<Entity> RandomEnemies(int memberCount) {
        while (memberCount > 0) {
            Entity e = new Entity();
            e.actor = true;
            e.alignment = Alignment.HOSTILE;
            e.SetAge(Global.rng.Next(7, 12));
            yield return e;
            memberCount--;
        }
    }

    public void Place(Entity entity, Tile tile) {
        Battle.current.AddChild(Piece.New(entity, tile));
    }
}