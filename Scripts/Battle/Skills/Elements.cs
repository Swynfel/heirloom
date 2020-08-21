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
    protected T[] elements = new T[ElementUtils.TOTAL_ELEMENTS];

    public Elemental() { }

    public Elemental(Element element, T value) {
        this[element] = value;
    }

    protected Elemental(T[] elements) {
        this.elements = elements;
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
    public ElementalAffinity() : base() { }
    public ElementalAffinity(Element element, ElementAffinity value) : base(element, value) { }
    public ElementalAffinity(ElementAffinity[] elements) : base(elements) { }

    public int[] Serialize() {
        return elements.Select(e => (int) e).ToArray();
    }

    public static ElementalAffinity Deserialize(int[] elements) {
        return new ElementalAffinity(elements.Select(e => (ElementAffinity) e).ToArray());
    }
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

    private static Color[] ELEMENT_COLORS = {
        Colors.White,
        Color.Color8(122,21,21),
        Color.Color8(150,140,13),
        Color.Color8(100,105,105),
        Color.Color8(25,123,128),
        Color.Color8(158,184,139),
        Color.Color8(33,156,48),
        Color.Color8(71,34,135),
    };

    public static Color GetColor(Element element) {
        return ELEMENT_COLORS[(int) element];
    }
}