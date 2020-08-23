using System;
using Godot;

public class MetaPopup : CenterContainer {

    public static MetaPopup instance { get; private set; }

    public override void _Ready() {
        instance = this;
    }

    private TabContainer tabs;
    private TabContainer Tabs {
        get {
            if (tabs == null) {
                tabs = GetNode<TabContainer>("Popup/Container");
            }
            return tabs;
        }
    }

    private Popup popup;
    private Popup Popup {
        get {
            if (popup == null) {
                popup = GetNode<Popup>("Popup");
            }
            return popup;
        }
    }

    public void OpenMemory(Memory.MetaTag tag) {
        GD.Print("TAG " + tag);
        switch (tag.group) {
            case (Memory.Group.CHARACTER):
                OpenEntity(tag.RememberEntity());
                break;
            case (Memory.Group.ITEMS):
                OpenItem(tag.RememberItem());
                break;
        }
    }

    public void OpenEntity(Entity entity) {
        Tabs.CurrentTab = 0;
        Tabs.GetNode<Visual.Tables.CharacterTableBig>("CharacterTable").SetCharacter(entity);
        Open();
    }

    public void OpenItem(Item item) {
        Tabs.CurrentTab = 1;
        Tabs.GetNode<Visual.Tables.ItemTable>("ItemTable").SetItem(item);
        Open();
    }

    public void OpenSkill(Skill skill) {
        Tabs.CurrentTab = 2;
        Tabs.GetNode<Visual.Tables.SkillTable>("SkillTable").SetSkill(skill);
        Open();
    }

    private void Open() {
        GD.Print("OPEN");
        Popup.PopupCentered();
    }
}
