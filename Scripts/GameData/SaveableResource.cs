using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

public interface ISpecialSaveable {
    bool IncludeDefaultSaveable();
    void SaveData(Godot.Collections.Dictionary data);
    void LoadData(Godot.Collections.Dictionary data);
}

public interface IExportable {
    Godot.Collections.Dictionary SaveProperties();
    void LoadProperties(Godot.Collections.Dictionary data);
}

public class GameDataResourceSaver {
    public static GameDataResourceSaver instance = null;
    List<Resource> resources = new List<Resource>();
    List<Godot.Collections.Dictionary> encoded_resources = new List<Godot.Collections.Dictionary>();

    public static void Init() {
        instance = new GameDataResourceSaver();
    }
    public static string Flush() {
        string result = JSON.Print(instance.encoded_resources);
        instance = null;
        return result;
    }
    public int ToKey(Resource resource) {
        int index = resources.IndexOf(resource);
        if (index == -1) {
            index = resources.Count;
            resources.Add(resource);
            encoded_resources.Add(null);
            encoded_resources[index] = resource.SaveData();
        }
        return index;
    }
}

public class GameDataResourceLoader {
    public static GameDataResourceLoader instance = null;
    List<Godot.Collections.Dictionary> encoded_resources = new List<Godot.Collections.Dictionary>();
    Dictionary<int, Resource> resources = new Dictionary<int, Resource>();

    public static void Load(string value) {
        instance = new GameDataResourceLoader();
        instance.encoded_resources = ((Godot.Collections.Array) JSON.Parse(value).Result).Cast<object>().Select(obj => (Godot.Collections.Dictionary) obj).ToList();
    }
    public static void Done() {
        instance = null;
    }
    public Resource FromKey(int key) {
        if (!resources.ContainsKey(key)) {
            Godot.Collections.Dictionary data = encoded_resources[key];
            Type type = Type.GetType((string) data["type"]);
            resources[key] = (Resource) Activator.CreateInstance(type);
            resources[key].LoadData(data);
        }
        return resources[key];
    }
}

public static class SaveableResource {

    private static PropertyUsageFlags PROPERTY_USAGE_EXPORT = PropertyUsageFlags.ScriptVariable;
    public static Godot.Collections.Array<T> ToGodotArray<T>(this IEnumerable<T> enumerable) {
        return new Godot.Collections.Array<T>(enumerable);
    }

    public static bool IsNumeric(this Type type) {
        switch (Type.GetTypeCode(type)) {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
                return true;
            default:
                return false;
        }
    }

    private static string EncodeNumeric(long i) {
        return Convert.ToBase64String(BitConverter.GetBytes(i)).TrimEnd('A', '=');
    }

    private static long DecodeNumeric(string s) {
        s = s + "AAAAAAAAAAA=".Substring(s.Length);
        return BitConverter.ToInt64(Convert.FromBase64String(s), 0);
    }

    public static object Encode(object obj) {
        if (obj is IExportable exportable) {
            return exportable.SaveProperties();
        }
        if (obj is string s) {
            return s;
        }
        if (obj is Enum e) {
            return EncodeNumeric(Convert.ToInt32(e));
        }
        if (obj is int i) {
            return EncodeNumeric(i);
        }
        if (obj is Color color) {
            return EncodeNumeric(color.ToRgba64());
        }
        if (obj is Resource subresource) {
            return GameDataResourceSaver.instance.ToKey(subresource);
        }
        if (obj is IEnumerable<object> obj_enumerable) {
            return obj_enumerable.Select(obj => Encode(obj)).ToGodotArray();
        }
        if (obj is IEnumerable enumerable) {
            return enumerable.Cast<object>().Select(obj => Encode(obj)).ToGodotArray();
        }
        return obj;
    }

    public static object Decode(Type type, object data) {
        if (data == null) {
            return null;
        }
        if (typeof(IExportable).IsAssignableFrom(type)) {
            IExportable obj = (IExportable) Activator.CreateInstance(type);
            obj.LoadProperties((Godot.Collections.Dictionary) data);
            return obj;
        }
        if (type == typeof(string)) {
            return (string) data;
        }
        if (type.IsEnum) {
            return Enum.ToObject(type, Decode(Enum.GetUnderlyingType(type), data));
        }
        if (type.IsNumeric()) {
            return Convert.ChangeType(DecodeNumeric((string) data), type);
        }
        if (type == typeof(Color)) {
            return new Color(DecodeNumeric((string) data));
        }
        if (typeof(Resource).IsAssignableFrom(type)) {
            return GameDataResourceLoader.instance.FromKey(Convert.ToInt32((float) data));
        }
        if (typeof(IEnumerable).IsAssignableFrom(type)) {
            Type elementType = type.GetElementType() ?? type.GenericTypeArguments[0];
            var raw_enumerable = ((IEnumerable) data).Cast<object>().Select(v => Decode(elementType, v)).ToList();
            var value = typeof(Enumerable)
                .GetMethod("Cast")
                .MakeGenericMethod(elementType)
                .Invoke(
                    null,
                    new object[] { raw_enumerable });
            if (typeof(Array).IsAssignableFrom(type)) {
                return typeof(Enumerable)
                    .GetMethod("ToArray")
                    .MakeGenericMethod(elementType)
                    .Invoke(null, new object[] { value });
            }
            return Activator.CreateInstance(type, value);
        }
        try {
            return Convert.ChangeType(data, type);
        } catch {
            try {
                return Activator.CreateInstance(type, data);
            } catch {
                return Activator.CreateInstance(type, Convert.ToInt32(data));
            }
        }

    }
    public static Godot.Collections.Dictionary SaveData(this Resource resource) {
        Type type = resource.GetType();
        var data = new Godot.Collections.Dictionary();
        data["type"] = type.FullName;
        if (resource is ISpecialSaveable saveableResource) {
            saveableResource.SaveData(data);
            if (!saveableResource.IncludeDefaultSaveable()) {
                return data;
            }
        }
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
        return data;
    }

    public static Resource LoadData(this Resource resource, object _data) {
        Godot.Collections.Dictionary data;
        if (_data == null) {
            return null;
        } else if (_data is Godot.Collections.Dictionary<string, object> strObjDict) {
            data = (Godot.Collections.Dictionary) strObjDict;
        } else if (_data is Godot.Collections.Dictionary nDict) {
            data = nDict;
        } else {
            throw new Exception("Type of data " + _data.GetType() + " could not be casted");
        }
        Type type = resource.GetType();
        if (resource is ISpecialSaveable saveableResource) {
            saveableResource.LoadData(data);
            if (!saveableResource.IncludeDefaultSaveable()) {
                return resource;
            }
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
                continue;
            }
            try {
                field.SetValue(resource, value);
            } catch (Exception e) {
                GD.PrintErr($"{type}[{key}] = {value} didn't work");
                throw e;
            }
        }
        return resource;
    }
}