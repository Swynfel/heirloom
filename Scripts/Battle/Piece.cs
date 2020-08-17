using System;
using Godot;

public class Piece : Node2D {
    private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Battle/Piece.tscn");
    private static Piece Instance() {
        return (Piece) template.Instance();
    }
    public AnimatedSprite sprite { get; private set; }
    [Export] public bool friendly;

    public Board board { get; private set; }
    public Tile on { get; private set; }

    public static Piece Create(Board board, Tile tile) {
        Piece piece = Instance();
        board.AddChild(piece);
        piece.board = board;
        piece.MoveOn(tile);
        return piece;
    }

    public void MoveOn(Tile tile) {
        if (on != null) {
            on.pieces.Remove(this);
        }
        on = tile;
        if (on != null) {
            on.pieces.Add(this);
            Position = tile.Position;
        }
    }

    public override void _Ready() {
        sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        if (friendly) {
            sprite.Modulate = Colors.Green;
        } else {
            sprite.Modulate = Colors.Red;
        }
    }
}
