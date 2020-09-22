using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public interface IExtraSaveable {
    void SaveExtraData(Godot.Collections.Dictionary data);
    void LoadExtraData(Godot.Collections.Dictionary data);
}

public interface IExportable {
    Godot.Collections.Dictionary SaveProperties();
    void LoadProperties(Godot.Collections.Dictionary data);
}

public static class SavableResourceExtension {

    private static PropertyUsageFlags PROPERTY_USAGE_EXPORT = PropertyUsageFlags.ScriptVariable;
    public static Godot.Collections.Array<T> ToGodotArray<T>(this IEnumerable<T> enumerable) {
        return new Godot.Collections.Array<T>(enumerable);
    }

    public static object Encode(object obj) {
        if (obj is IExportable exportable) {
            return exportable.SaveProperties();
        }
        if (obj is Resource subresource) {
            return subresource.SaveData();
        }
        if (obj is IEnumerable<object> array) {
            return array.Select(obj => Encode(obj)).ToGodotArray();
        }
        if (obj is Godot.Collections.Array godotArray) {
            return godotArray.Cast<object>().Select(obj => Encode(obj)).ToGodotArray();
        }
        return obj;
    }

    public static object Decode(Type type, object data) {
        if (typeof(IExportable).IsAssignableFrom(type)) {
            GD.Print("IExp", type, ":", data, " ", data.GetType());
            IExportable obj = (IExportable) Activator.CreateInstance(type);
            // IExportable obj = (IExportable) type.GetConstructor(new Type[] { })?.Invoke(new object[] { })
            //     ?? (IExportable) type.GetMethod("Create").Invoke(null, null);
            obj.LoadProperties((Godot.Collections.Dictionary) data);
            return obj;
        }
        if (typeof(IEnumerable<object>).IsAssignableFrom(type)) {
            GD.Print("IEnu ", type, ":", data.GetType());
            Type elementType = type.GenericTypeArguments[0];
            var value = typeof(Enumerable)
                .GetMethod("Cast")
                .MakeGenericMethod(elementType)
                .Invoke(
                    null,
                    new object[] { ((Godot.Collections.Array) data).Cast<object>().Select(v => Decode(elementType, v)) });
            // GD.Print("Understood ", elementType, " , ", type, " , ", value, " , ", value.GetType());
            return Activator.CreateInstance(type, value);
            // return type.GetConstructor(new Type[] { enumerableType }).Invoke(new object[] { value });
        }
        if (typeof(Resource).IsAssignableFrom(type)) {
            GD.Print("Res", type);
            Resource subresource = (Resource) type.GetConstructor(new Type[] { }).Invoke(new object[] { });
            subresource.LoadData(data);
            return subresource;
        }
        if (type == typeof(int)) {
            return Convert.ToInt32(data);
        }
        // if (data is Godot.Collections.Array godotArray) {
        //     return godotArray.Cast<object>().Select(obj => Encode(obj)).ToGodotArray();
        // }
        GD.Print("No cool type:", type.ToString());
        return data;
    }
    public static Godot.Collections.Dictionary SaveData(this Resource resource) {
        Type type = resource.GetType();
        var data = new Godot.Collections.Dictionary();
        foreach (Godot.Collections.Dictionary property in resource.GetPropertyList()) {
            string key = (string) property["name"];
            if (((PropertyUsageFlags) property["usage"] & PROPERTY_USAGE_EXPORT) != PROPERTY_USAGE_EXPORT) {
                continue;
            }
            if (type.GetField(key) == null) {
                continue;
            }
            data[key] = Encode(type.GetField(key).GetValue(resource));
        }
        if (resource is IExtraSaveable saveableResource) {
            saveableResource.SaveExtraData(data);
        }
        return data;
    }

    public static void LoadData(this Resource resource, object _data) {
        Type type = resource.GetType();
        Godot.Collections.Dictionary data;
        if (_data is Godot.Collections.Dictionary<string, object> strObjDict) {
            data = (Godot.Collections.Dictionary) strObjDict;
        } else if (_data is Godot.Collections.Dictionary nDict) {
            data = nDict;
        } else {
            throw new Exception("Type of data " + _data.ToString() + " could not be casted");
        }
        foreach (Godot.Collections.Dictionary property in resource.GetPropertyList()) {
            string key = (string) property["name"];
            if (((PropertyUsageFlags) property["usage"] & PROPERTY_USAGE_EXPORT) != PROPERTY_USAGE_EXPORT) {
                continue;
            }
            System.Reflection.FieldInfo field = type.GetField(key);
            if (field == null) {
                continue;
            }
            var value = Decode(field.FieldType, data[key]);
            if (value == null) {
                GD.PrintErr("Could not load ", key, " in ", type.ToString());
                continue;
            }
            field.SetValue(resource, value);
        }
        if (resource is IExtraSaveable saveableResource) {
            saveableResource.LoadExtraData(data);
        }
    }
}