using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat.Generate {
    public class ClassicBattleGeneration : BattleGeneration {
        public enum EnemyType {
            BANDITS
        }

        public ClassicBattleGeneration() { }

        public ClassicBattleGeneration(MapType map, EnemyType enemyType, int count, ElementalAffinity elements, string enemyNoun = null) {
            this.map = map;
            this.enemyType = enemyType;
            this.enemyNoun = enemyNoun;
            this.enemyCount = count;
            this.elements = elements;
        }

        [Export] public MapType map;
        [Export] public EnemyType enemyType;
        [Export] public string enemyNoun;
        [Export] public int enemyCount = 1;

        public override void Generate(Battle battle, IEnumerable<CharacterEntity> party) {
            Board board = battle.board;
            List<CharacterEntity> friendly = party.ToList();
            int size = (enemyCount + friendly.Count) / 3;
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

        private bool GenerateInternal(Board board, List<CharacterEntity> friendly, int width, int height) {
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

                foreach (CharacterEntity friend in friendly) {
                    Tile tile = tiles.PopRandom();
                    foreach (Tile n in tile.GetNeighbors()) {
                        tiles.Remove(n);
                    }
                    Place(friend, tile);
                }

                foreach (CharacterEntity enemy in RandomEnemies(enemyCount)) {
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

        private static IEnumerable<CharacterEntity> RandomEnemies(int memberCount) {
            while (memberCount > 0) {
                CharacterEntity e = new CharacterEntity();
                e.actor = true;
                e.alignment = Alignment.HOSTILE;
                e.SetAge(Global.rng.Next(7, 12));
                yield return e;
                memberCount--;
            }
        }
    }
}