using System;
using Godot;

public interface ISaveable {
    void SaveExtraData(Godot.Collections.Dictionary<string, object> data);
    void LoadExtraData(Godot.Collections.Dictionary<string, object> data);
}

public static class SavableResourceExtension {
    public static Godot.Collections.Dictionary<string, object> SaveData(this Resource resource) {
        var data = new Godot.Collections.Dictionary<string, object>();
        foreach (Godot.Collections.Dictionary property in resource.GetPropertyList()) {
            string key = (string) property["name"];
            GD.Print(key);
            if ((string) property["class_name"] == "Resource") {
                data[key] = ((Resource) resource.Get(key)).SaveData();
            } else {
                data[key] = resource.Get(key);
            }
        }
        if (resource is ISaveable saveableResource) {
            // saveableResource.SaveExtraData(data);
        }
        return data;
    }

    public static void LoadData(this Resource resource, object _data) {
        var data = _data as Godot.Collections.Dictionary<string, object>;
        foreach (Godot.Collections.Dictionary<string, string> property in resource.GetPropertyList()) {
            string key = property["name"];
            GD.Print(property);
            resource.Set(key, data[key]);
        }
        if (resource is ISaveable saveableResource) {
            saveableResource.LoadExtraData(data);
        }
    }
}