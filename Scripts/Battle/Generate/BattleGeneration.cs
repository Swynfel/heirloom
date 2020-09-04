using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat.Generate {
    public abstract class BattleGeneration : Resource {
        public ElementalAffinity elements {
            get => ElementalAffinity.Deserialize(_elements);
            set => _elements = value.Serialize();
        }
        [Export] private int[] _elements = ElementalAffinity.RandomAffinity().Serialize();
        public abstract void Generate(Battle battle, IEnumerable<CharacterEntity> party);
        protected void Place(CharacterEntity entity, Tile tile) {
            Battle.current.AddChild(Piece.New(entity, tile));
        }
    }
}