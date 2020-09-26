using System.Collections.Generic;
using Utils;
public class SeasonHistory : ISaveable {
    [Save] public Date date;

    [Save] public List<string> events = new List<string>();

    public SeasonHistory() { }
    public SeasonHistory(Date date) {
        this.date = date;
    }
}