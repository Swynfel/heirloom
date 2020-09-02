using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Godot;

public class InfoText : Resource {

    [Export] public string Key;
    [Export] public string Title;
    [Export(PropertyHint.MultilineText)] public string Description;
    public string BBDescription => BBfy(Description);

    private static Dictionary<string, InfoText> loadedInfoTexts = null;

    public static InfoText Find(string key) {
        if (loadedInfoTexts == null) {
            LoadInfoTexts();
        }
        return loadedInfoTexts.GetOrDefault(key, null);
    }

    private static string DIRECTORY = "res://Data/InfoText/";
    private static void LoadInfoTexts() {
        Directory dir = new Directory();
        loadedInfoTexts = new Dictionary<string, InfoText>();
        dir.Open(DIRECTORY);
        dir.ListDirBegin();

        while (true) {
            string file = dir.GetNext();
            if (file == "") {
                break;
            } else if (file[0] != '.') {
                InfoText info = ResourceLoader.Load<InfoText>(DIRECTORY + file);
                loadedInfoTexts[info.Key] = info;
            }

            dir.ListDirEnd();
        }
    }

    private static string BBfy(string raw) {
        return raw
            .Replace("[/?]", "[/url][/rem]")
            .Replace("[?", "[rem][url=?")
            .Replace("[img]icon://", "[img]res://Data/Icons/");
    }
}