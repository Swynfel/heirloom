using System;
using System.Collections.Generic;
using Godot;
public class SeasonHistory : Resource {
    [Export] public Date date;

    [Export] public List<string> events = new List<string>();

    public SeasonHistory() { }
    public SeasonHistory(Date date) {
        this.date = date;
    }
}