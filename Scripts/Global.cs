using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class Global {
    public static Random rng { get; } = new Random();

    public static Combat.Battle battle { get => Combat.Battle.current; }
    public static Combat.Board board { get => Combat.Battle.current.board; }
    public static UI.BattleUI battleUI { get => UI.BattleUI.current; }
    public static Godot.Collections.Array ArrayFrom(params object[] objects) {
        Godot.Collections.Array result = new Godot.Collections.Array();
        foreach (object obj in objects) {
            result.Add(obj);
        }
        return result;
    }
    public static void QueueFreeChildren(this Node node) {
        foreach (Node child in node.GetChildren()) {
            child.QueueFree();
        }
    }

    public static V GetOrDefault<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue) {
        V value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }

    public static bool EqualsOrFalse<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue) {
        V value;
        return dictionary.TryGetValue(key, out value) && value.Equals(defaultValue);
    }

    public static T Random<T>(this T[] list) {
        return list[Global.rng.Next(0, list.Length)];
    }
    public static T Random<T>(this IEnumerable<T> list) {
        return list.ElementAt(Global.rng.Next(0, list.Count()));
    }

    public static T PopRandom<T>(this List<T> list) {
        T r = list.Random();
        list.Remove(r);
        return r;
    }

    public static IEnumerable<T> RandomOrder<T>(this IEnumerable<T> list) {
        List<T> elements = new List<T>(list);
        while (elements.Count > 0) {
            yield return elements.PopRandom();
        }
    }

    public static string FancyJoin(this IEnumerable<string> list, string none = "") {
        string previous = null;
        string last = null;
        foreach (string word in list) {
            if (last != null) {
                if (previous == null) {
                    previous = last;
                } else {
                    previous += ", " + last;
                }
            }
            last = word;
        }
        if (last == null) {
            return none;
        }
        if (previous == null) {
            return last;
        }
        return previous + " and " + last;
    }
}