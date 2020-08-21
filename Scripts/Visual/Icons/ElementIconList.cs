using System;
using System.Collections.Generic;
using Godot;

namespace Visual.Icons {
    public class ElementIconList : HBoxContainer {
        public void SetElements(List<Element> elements) {
            this.QueueFreeChildren();
            foreach (Element e in elements) {
                AddChild(ElementIcon.Create(e));
            }
        }
    }
}
