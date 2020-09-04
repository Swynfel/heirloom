using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat.Generate {
    public class DeterministicBattleGeneration : BattleGeneration {

        [Export] public readonly DeterministicType deterministicType;
        public enum DeterministicType {
            TUTORIAL,
            CROWN,
            SHIELD,
            FINAL,
        }

        private static readonly ElementalAffinity NONE_ELEMENTAL = new ElementalAffinity();
        private static readonly ElementalAffinity CROWN_ELEMENTAL = NONE_ELEMENTAL; //TODO
        private static readonly ElementalAffinity SHIELD_ELEMENTAL = NONE_ELEMENTAL; //TODO
        private static readonly ElementalAffinity FINAL_ELEMENTAL = NONE_ELEMENTAL; //TODO

        public DeterministicBattleGeneration() : this(DeterministicType.TUTORIAL) { }

        public DeterministicBattleGeneration(DeterministicType deterministicType) {
            this.deterministicType = deterministicType;
            elements = deterministicType switch
            {
                DeterministicType.CROWN => CROWN_ELEMENTAL,
                DeterministicType.SHIELD => SHIELD_ELEMENTAL,
                DeterministicType.FINAL => FINAL_ELEMENTAL,
                _ => NONE_ELEMENTAL,
            };
        }

        private Entity LoadEntity(string name) {
            return (Entity) GD.Load($"res://Nodes/Battle/Entities/{name}.tres");
        }

        public override void Generate(Battle battle, IEnumerable<CharacterEntity> party) {
            var friendly = new List<CharacterEntity>(party);
            DeterministicBoardGeneration generation = deterministicType switch
            {
                /*** Tutorial ***/
                DeterministicType.TUTORIAL => new DeterministicBoardGeneration(
                // Map
@"
xxxxx      xxxxx
xXXXx      xXXXx
xX@XXXX  XXXX$Xx
xXXXx X  X xXXXx
xxxxx X  X xxxxx
      X01X
",
                // Tiles 
                new Dictionary<Tile.GroundType, string> {
                    { Tile.GroundType.DIRT, "X01@$" },
                    { Tile.GroundType.DRY, "x" }
                },
                // Obstacles
                c => c switch {
                    '0' => LoadEntity("Boulder"),
                    '1' => LoadEntity("Bush"),
                    '$' => LoadEntity("Signpost"),
                    _ => null
                },
                // Party
                party
            ),

                /*** Crown ***/
                DeterministicType.CROWN => new DeterministicBoardGeneration(
                // Map
@"
xxxxxxx     xxxxxxx
xXXXXXxxxxxxxXXXXXx
xX@X@XXXX$XXXX$X$Xx
xXX@XXx~~~~~xXX$XXx
xX@X@XXXX$XXXX$X$Xx
xXXXXXxxxxxxxXXXXXx
xxxxxxx     xxxxxxx
",
                // Tiles 
                new Dictionary<Tile.GroundType, string> {
                    { Tile.GroundType.TEMPLE_BORDER, "X@$" },
                    { Tile.GroundType.TEMPLE_WATER, "~" },
                    { Tile.GroundType.TEMPLE_FULL, "x" }
                },
                // Obstacles
                c => c switch {
                    '$' => LoadEntity("Zombie"),
                    _ => null
                },
                // Party
                party
            ),

                /*** Crown ***/
                DeterministicType.SHIELD => new DeterministicBoardGeneration(
                // Map
@"
xxxxxxx     xxxxxxx
xXXXXXxxxxxxxXXXXXx
xX@X@XXXX$XXXX$X$Xx
xXX@XXx~~~~~xXX$XXx
xX@X@XXXX$XXXX$X$Xx
xXXXXXxxxxxxxXXXXXx
xxxxxxx     xxxxxxx
",
                // Tiles 
                new Dictionary<Tile.GroundType, string> {
                    { Tile.GroundType.TEMPLE_BORDER, "X@$" },
                    { Tile.GroundType.TEMPLE_WATER, "~" },
                    { Tile.GroundType.TEMPLE_FULL, "x" }
                },
                // Obstacles
                c => c switch {
                    '$' => LoadEntity("Zombie"),
                    _ => null
                },
                // Party
                party
            ),

                /*** Final ***/
                DeterministicType.FINAL => new DeterministicBoardGeneration(
                // Map
@"
xxxxxxx     xxxxxxx
xXXXXXxxxxxxxXXXXXx
xX@X@XXXX$XXXX$X$Xx
xXX@XXx~~~~~xXX$XXx
xX@X@XXXX$XXXX$X$Xx
xXXXXXxxxxxxxXXXXXx
xxxxxxx     xxxxxxx
",
                // Tiles 
                new Dictionary<Tile.GroundType, string> {
                    { Tile.GroundType.TEMPLE_BORDER, "X@$" },
                    { Tile.GroundType.TEMPLE_WATER, "~" },
                    { Tile.GroundType.TEMPLE_FULL, "x" }
                },
                // Obstacles
                c => c switch {
                    '$' => LoadEntity("Zombie"),
                    _ => null
                },
                // Party
                party
            ),


                /*** Default ***/
                _ => null,
            };

            battle.board.width = generation.width;
            battle.board.height = generation.height;
            battle.board.CreateTerrain(
                generation
            );
        }

        private void GenerateFromTemplate(Board board, List<CharacterEntity> friendly, string template) {

        }
        private class DeterministicBoardGeneration : BoardGeneration {
            private string[] template;
            private List<Entity> friendly;
            private Dictionary<Tile.GroundType, string> tiles;
            private Func<char, Entity> spawn;

            public int width { get; private set; }
            public int height { get; private set; }

            public DeterministicBoardGeneration(string rawTemplate, Dictionary<Tile.GroundType, string> tiles, Func<char, Entity> spawn, IEnumerable<Entity> friendly) {
                template = rawTemplate.StripEdges().Split('\n');
                this.tiles = tiles;
                this.spawn = spawn;
                this.friendly = new List<Entity>(friendly);
                height = template.Length;
                width = template.Select(line => line.Length).Max();
            }

            private char CharAt(int x, int y) {
                if (x < 0 || y < 0 || height <= y) {
                    return ' ';
                }
                string line = template[y];
                if (x >= line.Length) {
                    return ' ';
                }
                return line[x];
            }

            public override Tile.GroundType Pick(int x, int y) {
                char c = CharAt(x, y);
                foreach (var pair in tiles) {
                    if (pair.Value.Contains(c)) {
                        return pair.Key;
                    }
                }
                return Tile.GroundType.NONE;
            }

            public override Entity Obstacle(int x, int y) {
                char c = CharAt(x, y);
                if (c == '@') {
                    return friendly.Count > 0 ? friendly.PopRandom() : null;
                }
                return spawn(c);
            }
        }
    }
}