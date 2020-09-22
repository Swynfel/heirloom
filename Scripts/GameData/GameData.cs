
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class GameData : Resource {
    public Date date;
    [Export] private int _date;

    [Export] public Memory memory = new Memory();
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
    [Export] public AdventureProgress progress = new AdventureProgress();

    private const string SAVE_FILE = "user://heirloom.save";
    private GameData() { }

    public static GameData New() {
        GameData data = (GameData) ResourceLoader.Load("res://Assets/empty_save.tres");
        Game.data = data; // HACK
        data.date = Date.START;
        data.name = "NAME";
        data.family = Family.StartingFamily();
        data.family.ResourceLocalToScene = true;
        data.quests = new List<Quest> {
            QuestGeneration.GenerateRandomQuest(maximumIntensity: 5)
        };
        data.inventory = new Riches(5, 20, new List<Item> { Item.ARTEFACT_SWORD
        });
        History.Clear();
        History.Append(string.Format("{0} have been passed the {1}.", CharacterEntity.MetaNames(data.family.alive), Item.ARTEFACT_SWORD.MetaName()));
        return data;
    }

    public static bool HasSave() {
        return new File().FileExists(SAVE_FILE);
    }

    public Error Save() {
        File file = new File();
        Error error = file.Open(SAVE_FILE, File.ModeFlags.Write);
        if (error != Error.Ok) {
            GD.PrintErr("Savefile could not be opened");
        }
        // Save
        GD.Print(JSON.Print(this.SaveData()));
        return error;
    }

    public static GameData Load() {
        if (!HasSave()) {
            GD.PrintErr("Savefile could not be found");
            return null;
        }
        File file = new File();
        Error error = file.Open(SAVE_FILE, File.ModeFlags.Read);
        if (error != Error.Ok) {
            GD.PrintErr("Savefile could not be opened");
        }
        // Load
        GameData data = new GameData();
        var raw_data = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary) JSON.Parse(file.GetLine()).Result);
        // Parse
        // data.name = (string) raw_data["name"];
        // data.date = Date.Load(raw_data["date"]);
        // data.memory = Memory.Load(raw_data["memory"]);
        // data.family = Family.Load(data, raw_data["family"]);
        // data.quests = (raw_data["quests"] as Godot.Collections.Array<object>).Select(q => Quest.Load(data, q));
        // data.inventory = Riches.Load(data, raw_data["inventory"]);
        // data.history = History.Load(data, raw_data["history"]);
        // data.progress = AdventureProgress.Load(data, raw_data["progress"]);
        return data;
    }
}