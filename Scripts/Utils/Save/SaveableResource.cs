using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace Utils {
    public class Saver {
        List<object> objects = new List<object>();
        List<Godot.Collections.Dictionary> encoded_objects = new List<Godot.Collections.Dictionary>();

        public static string SaveSingle(Resource resource) {
            return SaveSingleObject(resource);
        }
        public static string SaveSingle(ISaveable saveable) {
            return SaveSingleObject(saveable);
        }
        private static string SaveSingleObject(object obj) {
            Saver saver = new Saver();
            saver.ToKeyPrivate(obj);
            return saver.Flush();
        }
        public string Flush() {
            return JSON.Print(encoded_objects);
        }
        public int ToKey(Resource resource) {
            return ToKeyPrivate(resource);
        }
        public int ToKey(ISaveable saveable) {
            return ToKeyPrivate(saveable);
        }
        private int ToKeyPrivate(object obj) {
            int index = objects.IndexOf(obj);
            if (index == -1) {
                index = objects.Count;
                objects.Add(obj);
                encoded_objects.Add(null);
                encoded_objects[index] = EncodeObject(obj);
            }
            return index;
        }

        private object Encode(object obj) {
            if (obj == null) {
                return null;
            }
            if (obj is ISaveable || obj is Resource) {
                return ToKeyPrivate(obj);
            }
            if (obj is string s) {
                return s;
            }
            if (obj is Enum e) {
                return Encoding.EncodeNumeric(Convert.ToInt32(e));
            }
            if (obj.GetType().IsNumeric()) {
                return Encoding.EncodeNumeric(Convert.ToInt64(obj));
            }
            if (obj is Color color) {
                return Encoding.EncodeNumeric(color.ToRgba64());
            }
            if (obj is IEnumerable<object> obj_enumerable) {
                return obj_enumerable.Select(obj => Encode(obj)).ToGodotArray();
            }
            if (obj is IEnumerable enumerable) {
                return enumerable.Cast<object>().Select(obj => Encode(obj)).ToGodotArray();
            }
            return obj;
        }

        private Godot.Collections.Dictionary EncodeObject(object obj) {
            Type type = obj.GetType();
            var data = new Godot.Collections.Dictionary();
            data["type"] = type.FullName;
            foreach (FieldInfo field in type.GetSaveableFields()) {
                data[field.Name] = Encode(field.GetValue(obj));
            }
            return data;
        }
    }

    public class Loader {
        List<Godot.Collections.Dictionary> encoded_resources = new List<Godot.Collections.Dictionary>();
        Dictionary<int, object> objects = new Dictionary<int, object>();

        public static Loader Load(string value) {
            Loader instance = new Loader();
            instance.encoded_resources = ((Godot.Collections.Array) JSON.Parse(value).Result).Cast<object>().Select(obj => (Godot.Collections.Dictionary) obj).ToList();
            return instance;
        }
        public static object LoadSingle(string data) {
            return Load(data).FromKey(0);
        }
        public object FromKey(int key) {
            if (!objects.ContainsKey(key)) {
                Godot.Collections.Dictionary data = encoded_resources[key];
                Type type = Type.GetType((string) data["type"]);
                objects[key] = Activator.CreateInstance(type);
                DecodeObject(objects[key], data);
            }
            return objects[key];
        }

        private object Decode(Type type, object data) {
            if (data == null) {
                return null;
            }
            if (typeof(Resource).IsAssignableFrom(type) || typeof(ISaveable).IsAssignableFrom(type)) {
                return FromKey(Convert.ToInt32((float) data));
            }
            if (type == typeof(string)) {
                return (string) data;
            }
            if (type.IsEnum) {
                return Enum.ToObject(type, Decode(Enum.GetUnderlyingType(type), data));
            }
            if (type.IsNumeric()) {
                return Convert.ChangeType(Encoding.DecodeNumeric((string) data), type);
            }
            if (type == typeof(Color)) {
                return new Color(Encoding.DecodeNumeric((string) data));
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

        private object DecodeObject(object obj, object _data) {
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
            Type type = obj.GetType();
            foreach (FieldInfo field in type.GetSaveableFields()) {
                var value = Decode(field.FieldType, data[field.Name]);
                if (value == null) {
                    continue;
                }
                try {
                    field.SetValue(obj, value);
                } catch (Exception e) {
                    GD.PrintErr($"{type}.{field.Name} = {value} failed");
                    throw e;
                }
            }
            return obj;
        }
    }
}