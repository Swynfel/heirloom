
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class GameData : Resource {
    [Export] public Date date;

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

    private const File.CompressionMode COMPRESSION_MODE = File.CompressionMode.Gzip;
    private const string SAVE_FILE = "user://heirloom.save";
    public GameData() { }

    public static GameData New() {
        GameData data = new GameData();
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
        Error error = file.OpenCompressed(SAVE_FILE, File.ModeFlags.Write, COMPRESSION_MODE);
        if (error != Error.Ok) {
            GD.PrintErr("Savefile could not be opened");
            return error;
        }
        // Save
        GameDataResourceSaver.Init();
        int key = GameDataResourceSaver.instance.ToKey(this);
        file.StoreLine(GameDataResourceSaver.Flush());
        file.StoreLine(key.ToString());
        file.Close();
        return error;
    }

    public static GameData Load() {
        if (!HasSave()) {
            GD.PrintErr("Savefile could not be found");
            return null;
        }
        File file = new File();
        Error error = file.OpenCompressed(SAVE_FILE, File.ModeFlags.Read, COMPRESSION_MODE);
        if (error != Error.Ok) {
            GD.PrintErr("Savefile could not be opened");
            return null;
        }
        // Load
        GameDataResourceLoader.Load(file.GetLine());
        Resource res = GameDataResourceLoader.instance.FromKey(int.Parse(file.GetLine()));
        GameData data = (GameData) res;
        GameDataResourceLoader.Done();
        file.Close();
        return data;
    }
}