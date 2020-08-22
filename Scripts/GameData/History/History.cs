using System;
using System.Collections.Generic;
using Godot;
public class History : Resource {
    [Export] public List<SeasonHistory> past;
    [Export] public SeasonHistory now;

    public History() {
        past = new List<SeasonHistory>();
        now = null;
    }

    public static void Append(string line) {
        Game.data.history.now.events.Add(line);
    }

    public static void NextYear() {
        History history = Game.data.history;
        if (history.now != null) {
            history.past.Add(history.now);
        }
        history.now = new SeasonHistory(Game.data.date);
    }
}