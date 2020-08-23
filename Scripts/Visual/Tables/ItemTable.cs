using System;
using System.Linq;
using Godot;

namespace Visual.Tables {
    using Icons;
    public class ItemTable : PanelContainer {
        [Export] bool noSelector = false;
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/ItemTable.tscn");
        public static ItemTable Create(Item item) {
            ItemTable table = (ItemTable) template.Instance();
            table.Link();
            table.SetItem(item);
            return table;
        }

        private CharacterSelectorButton characterSelector;
        private void Link() {
            characterSelector = GetNode<CharacterSelectorButton>("List/Bottom/Right/CharacterSelectorButton");
            if (noSelector) {
                characterSelector.SetupDeactivated();
            } else {
                characterSelector.Connect(nameof(CharacterSelectorButton.change_to), this, nameof(on_ChangeHolder));
                characterSelector.GetNode("CharacterSelectorPopup").Connect("about_to_show", this, nameof(on_OpenSelector));
            }
        }

        private Item item;
        public void SetItem(Item item) {
            this.item = item;
            GetNode<SkillIcon>("List/Top/SkillIcon").Set(item.icon);
            GetNode<Label>("List/Top/Name").Text = item.name;
            GetNode<Label>("List/Bottom/Left/Group").Text = item.group.ToString();
            if (item.equipable) {
                GetNode<Label>("List/Bottom/Left/Effect").Text = item.effect;
                GetNode<Control>("List/Bottom/Right").Show();
                characterSelector.Setup(item.holder);
            } else {
                GetNode<Label>("List/Bottom/Left/Effect").Text = item.description;
                GetNode<Control>("List/Bottom/Right").Hide();
            }
        }

        private void on_ChangeHolder(Entity entity) {
            item.SetHolder(entity);
        }

        private void on_OpenSelector() {
            if (item.group == Item.Group.ARTEFACT) {
                int holderBirth = (item.holder?.birth.SeasonsPassed()).GetValueOrDefault(int.MinValue);
                var choices = Family.familyMembers.Where(e => e.heldItem == null && e.birth.SeasonsPassed() > holderBirth);
                characterSelector.Setup(item.holder, false, "Artefacts can only be passed down to someone yonger", choices.ToList());
            } else {
                var choices = Family.familyMembers.Where(e => e.heldItem == null);
                characterSelector.Setup(item.holder, true, list: choices.ToList());
            }
        }
    }
}