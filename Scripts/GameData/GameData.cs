
using System;
using System.Collections.Generic;
using Godot;

public class GameData : Resource {
    public Date date { get; private set; }
    [Export] private int _date;

    [Export] public string name;

    [Export] public Family family = Family.RandomFamily(3);
    public Dictionary<Entity, VillageAction> actions = new Dictionary<Entity, VillageAction>();

    [Export]
    public List<Quest> quests = new List<Quest>() {
        new Quest(),
        new Quest(),
        new Quest(),
        new Quest(),
        new Quest(),
    };

    private const string SAVE_FILE = "user://heirloom.save.tres";
    private const ResourceSaver.SaverFlags SAVE_FORMAT =
        ResourceSaver.SaverFlags.RelativePaths &
        ResourceSaver.SaverFlags.BundleResources &
        ResourceSaver.SaverFlags.ChangePath &
        ResourceSaver.SaverFlags.OmitEditorProperties &
        ResourceSaver.SaverFlags.ReplaceSubresourcePaths
    ;
    private GameData() { }

    public static GameData New() {
        GameData data = (GameData) ResourceLoader.Load("res://Assets/empty_save.tres");
        data.date = new Date(0, 0);
        data.name = "NAME";
        return data;
    }
    public Error Save() {
        // HACK
        _date = date.SeasonsPassed();
        // Save
        Error error = ResourceSaver.Save(SAVE_FILE, this, SAVE_FORMAT);
        if (error != Error.Ok) {
            GD.PrintErr("Game could not be saved");
        }
        return error;
    }

    public static bool HasSave() {
        return new File().FileExists(SAVE_FILE);
    }

    public static GameData Load() {
        if (!HasSave()) {
            GD.PrintErr("Game could not be loaded");
            return null;
        }
        GameData data = ResourceLoader.Load<GameData>(SAVE_FILE);
        // HACK
        data.date = Date.FromSeasonsPassed(data._date);
        return data;
    }
}