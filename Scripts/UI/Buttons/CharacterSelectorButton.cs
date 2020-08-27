using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class CharacterSelectorButton : Control {
    private Visual.Icons.CharacterIcon character;
    private UI.CharacterSelectorPopup popup;

    private bool linked = false;
    private List<Entity> entities;
    private Entity current;

    public void Link() {
        if (linked) {
            return;
        }
        character = GetNode<Visual.Icons.CharacterIcon>("CharacterButton/Icon");
        popup = GetNode<UI.CharacterSelectorPopup>("CharacterSelectorPopup");
        GetNode<Button>("CharacterButton").Connect("pressed", popup, "popup");
        popup.Connect(nameof(UI.CharacterSelectorPopup.selected), this, nameof(on_Selected));
        popup.Connect("about_to_show", this, nameof(on_Open));
        linked = true;
    }

    public void SetupDeactivated() {
        Link();
        popup.Disconnect(nameof(UI.CharacterSelectorPopup.selected), this, nameof(on_Selected));
        GetNode<Button>("CharacterButton").Disconnect("pressed", popup, "popup");
        GetNode<Button>("CharacterButton").Connect("pressed", this, nameof(OpenMetaPopup));
    }

    private void OpenMetaPopup() {
        if (current != null) MetaPopup.instance.OpenEntity(current);
    }

    public void Setup(Entity current, bool nullable = false, string comment = null, List<Entity> list = null) {
        this.current = current;
        Link();
        entities = list;
        popup.Setup(nullable, comment);
        SetCurrent(current);
    }

    private void SetCurrent(Entity current) {
        if (entities == null) {
            popup.Setup(null);
        } else {
            popup.Setup(entities.Where(e => e != current));
        }
        character.SetCharacter(current);
    }

    [Signal] public delegate void change_to(Entity entity);
    private void on_Selected(Entity entity) {
        EmitSignal(nameof(change_to), entity);
        SetCurrent(entity);
    }
    private static Vector2 OFFSET = new Vector2(58, 0);
    private void on_Open() {
        popup.SetGlobalPosition(RectGlobalPosition + OFFSET, true);
    }
}
