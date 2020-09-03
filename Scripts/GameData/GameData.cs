
using System;
using System.Collections.Generic;
using Godot;

public class GameData : Resource {
    public Date date { get; set; }
    [Export] private int _date;

    [Export] public string name;

    [Export] public Family family = Family.RandomFamily(5);

    [Export]
    public List<Quest> quests = new List<Quest>() {
        new Quest(),
        new Quest(),
        new Quest(),
        new Quest(),
        new Quest(),
    };

    [Export] public Riches inventory = new Riches(12, 44, new List<Item> { Item.ARTEFACT_SHIELD, Item.ARTEFACT_SWORD, Item.ARTEFACT_CROWN, Item.DAGGER, Item.VASE });

    [Export] public History history = new History();
    [Export] public Memory memory = new Memory();

    [Export] public AdventureProgress progress = new AdventureProgress();

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
        Game.data = data; // HACK
        data.date = Date.START;
        data.name = "NAME";
        data.family = Family.StartingFamily();
        data.quests = new List<Quest> {
            QuestGeneration.GenerateRandomQuest()
        };
        data.inventory = new Riches(5, 20, new List<Item> { Item.ARTEFACT_SWORD
        });
        History.Clear();
        History.Append(string.Format("{0} have been passed the {1}.", Entity.MetaNames(data.family.alive), Item.ARTEFACT_SWORD.MetaName()));
        return data;
    }
    public Error Save() {
        // Trick to parse date with Godot Resource
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
        // Trick to parse date with Godot Resource 
        data.date = Date.FromSeasonsPassed(data._date);
        return data;
    }
}