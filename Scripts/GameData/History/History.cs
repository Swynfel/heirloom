using System.Collections.Generic;
using Utils;
public class History : ISaveable {
    [Save] public List<SeasonHistory> past;
    [Save] public SeasonHistory now;

    public History() {
        past = new List<SeasonHistory>();
        now = null;
        now = new SeasonHistory(Date.START);
    }


    public static void Clear() {
        Game.data.history = new History();
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