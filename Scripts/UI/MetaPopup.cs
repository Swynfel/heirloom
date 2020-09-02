using System;
using Godot;

public class MetaPopup : CenterContainer {

    public static MetaPopup instance { get; private set; }

    public override void _Ready() {
        instance = this;
        GetTree().Connect("screen_resized", this, nameof(ScreenResized));
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
        switch (tag.group) {
            case (Memory.Group.CHARACTER):
                OpenEntity(tag.RememberEntity());
                break;
            case (Memory.Group.ITEMS):
                OpenItem(tag.RememberItem());
                break;
            case (Memory.Group.INFO):
                OpenInfo(tag.RememberInfo());
                break;
        }
    }

    public void OpenEntity(Entity entity) {
        Tabs.CurrentTab = 0;
        Tabs.GetNode<Visual.Tables.CharacterTable>("CharacterTable").SetCharacter(entity);
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

    public void OpenInfo(InfoText info) {
        Tabs.CurrentTab = 3;
        Tabs.GetNode<Visual.Tables.InfoTable>("InfoTable").SetInfoText(info);
        Open();
    }

    private int delayedRefresh = 0;
    private void Open() {
        Popup.RectSize = Popup.RectMinSize;
        Popup.PopupCenteredMinsize();
        delayedRefresh = 5;
    }

    public override void _Process(float delta) {
        if (delayedRefresh > 0) {
            delayedRefresh--;
            if (Popup.Visible) {
                Popup.SetAsMinsize();
                Popup.PopupCenteredMinsize();
            }
        }
    }

    private void ScreenResized() {
        delayedRefresh = 5;
    }
}
