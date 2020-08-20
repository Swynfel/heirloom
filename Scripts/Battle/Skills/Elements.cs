using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
public enum Element {
    NONE = 0,
    FIRE = 1,
    LIGHT = 2,
    METAL = 3,
    WATER = 4,
    WIND = 5,
    PLANT = 6,
    DARK = 7,
}

public enum ElementAffinity {
    NEUTRAL,
    WEAK,
    RESISTANT,
    IMMUNE,
}

public class Elemental<T> {
    private T[] elements = new T[ElementUtils.TOTAL_ELEMENTS];

    public Elemental() { }

    public Elemental(Element element, T value) {
        this[element] = value;
    }

    public T this[Element element] {
        get => elements[(int) element];
        set => elements[(int) element] = value;
    }

    public List<Element> Where(Func<T, bool> predicate) {
        List<Element> elements = new List<Element>();
        foreach (Element e in ElementUtils.GetAllElements()) {
            if (predicate(this[e])) {
                elements.Add(e);
            }
        }
        return elements;
    }
}

public class ElementalAffinity : Elemental<ElementAffinity> {

    private static Element PopRandomElement(List<Element> elements) {
        int index = Global.rng.Next(0, elements.Count);
        Element element = elements[index];
        elements.RemoveAt(index);
        return element;
    }

    public static ElementalAffinity RandomAffinity(int resist = 2, int weak = 2) {
        if (resist + weak > 7) {
            GD.PrintErr("Too much resistances and weaknesses asked");
            resist = Math.Min(resist, 7);
            weak = Math.Min(weak, 7 - resist);
        }
        ElementalAffinity affinity = new ElementalAffinity();
        List<Element> elements = ElementUtils.GetAllElements();
        for (int k = 0 ; k < resist ; k++) {
            affinity[PopRandomElement(elements)] = ElementAffinity.RESISTANT;
        }
        for (int k = 0 ; k < weak ; k++) {
            affinity[PopRandomElement(elements)] = ElementAffinity.WEAK;
        }
        return affinity;
    }

    public List<Element> GetResistances() {
        return Where(a => a == ElementAffinity.RESISTANT || a == ElementAffinity.IMMUNE);
    }

    public List<Element> GetWeaknesses() {
        return Where(a => a == ElementAffinity.WEAK);
    }
}

public static class ElementUtils {
    public const int TOTAL_ELEMENTS = 8;

    private static readonly List<Element> ALL_ELEMENTS = new List<Element> { Element.FIRE, Element.LIGHT, Element.METAL, Element.WATER, Element.WIND, Element.PLANT, Element.DARK };

    public static List<Element> GetAllElements() {
        return new List<Element>(ALL_ELEMENTS);
    }
}