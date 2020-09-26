using System;
using System.Collections.Generic;
using Godot;

public class Riches : Resource {
    [Save] [Export] public int gold = 0;
    [Save] [Export] public int food = 0;
    [Save] [Export] public List<Item> items = new List<Item>();

    public Riches() : this(0, 0) { }

    public Riches(int gold, int food = 0, List<Item> items = null) {
        this.gold = gold;
        this.food = food;
        if (items != null) {
            this.items = items;
        }
    }

    public void Add(Riches other) {
        gold += other.gold;
        food += other.food;
        items.AddRange(other.items);
    }

    public bool IsEmpty() {
        return gold == 0 && food == 0 && items.Count == 0;
    }

    public override string ToString() {
        return string.Format("{0} gold, {1} food and {2} items", gold, food, items);
    }

    [Obsolete]
    public string ToMinString() {
        List<string> words = new List<string>();
        if (gold != 0) {
            words.Add(string.Format("{0} gold", gold));
        }
        if (food != 0) {
            words.Add(string.Format("{0} food", food));
        }
        if (items.Count != 0) {
            words.Add(string.Format("{0} items", items.Count));
        }
        if (words.Count == 0) {
            return "nothing";
        }
        if (words.Count == 3) {
            return ToString();
        }
        return string.Join("and ", words);
    }
}