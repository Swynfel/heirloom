using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class CharacterSelectorButton : Control {
    public Visual.Icons.CharacterIcon character;
    public UI.CharacterSelectorPopup popup;

    private List<Entity> entities;
    private Entity current;

    public override void _Ready() {
        character = GetNode<Visual.Icons.CharacterIcon>("CharacterButton/Icon");
        popup = GetNode<UI.CharacterSelectorPopup>("CharacterSelectorPopup");
        GetNode<Button>("CharacterButton").Connect("pressed", popup, "popup");
        popup.Connect(nameof(UI.CharacterSelectorPopup.selected), this, nameof(on_Selected));
        // TODO
        Setup(null, list: Family.familyMembers);
    }
    public void Setup(Entity current, bool nullable = false, string comment = null, List<Entity> list = null) {
        entities = list;
        popup.Setup(nullable, comment);
        SetCurrent(current);
    }

    private void SetCurrent(Entity current) {
        GD.Print(current != null ? current.name : "none");
        if (entities == null) {
            popup.Setup(null);
        } else {
            popup.Setup(entities.Where(e => e != current));
        }
        character.SetCharacter(current);
    }

    [Signal] delegate void change_to(Entity entity);
    private void on_Selected(Entity entity) {
        EmitSignal(nameof(change_to), entity);
        SetCurrent(entity);
    }
}
