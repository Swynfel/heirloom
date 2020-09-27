using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

[AttributeUsage(AttributeTargets.Field)]
public class SaveAttribute : Attribute {
    public SaveAttribute() { }
}

[AttributeUsage(AttributeTargets.Field)]
public class IgnoreSaveAttribute : Attribute {
    public IgnoreSaveAttribute() { }
}

namespace Utils {
    internal static class SaveExtension {
        public static IEnumerable<FieldInfo> GetSaveableFields(this Type type) {
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                if ((field.GetCustomAttribute<SaveAttribute>() != null || field.GetCustomAttribute<ExportAttribute>() != null) && field.GetCustomAttribute<IgnoreSaveAttribute>() == null) {
                    yield return field;
                }
            }
        }
    }
}