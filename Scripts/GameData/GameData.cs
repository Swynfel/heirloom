
using System;
using System.Collections.Generic;
using Godot;
using Utils;

public class GameData : Resource {
    [Save] public Date date;

    [Save] public Memory memory = new Memory();
    [Save] public string name;

    [Save] public Family family = Family.RandomFamily(5);

    [Save]
    public List<Quest> quests = new List<Quest>() {
        new Quest(),
        new Quest(),
        new Quest(),
        new Quest(),
        new Quest(),
    };
    [Save] public Riches inventory = new Riches(12, 44, new List<Item> { Item.ARTEFACT_SHIELD, Item.ARTEFACT_SWORD, Item.ARTEFACT_CROWN, Item.DAGGER, Item.VASE });

    [Save] public History history = new History();
    [Save] public AdventureProgress progress = new AdventureProgress();

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
        file.StoreLine(Saver.SaveSingle(this));
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
        GameData data = (GameData) Loader.Load(file.GetLine()).FromKey(0);
        file.Close();
        return data;
    }
}