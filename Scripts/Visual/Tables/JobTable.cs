using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Visual.Tables {
    using Icons;
    public class JobTable : MarginContainer {
        private static PackedScene template = (PackedScene) ResourceLoader.Load("res://Nodes/Visual/Components/JobTable.tscn");
        public static JobTable Create(Entity entity) {
            JobTable table = (JobTable) template.Instance();
            table.Link();
            table.SetEntity(entity);
            return table;
        }
        private MenuButton jobSelector;
        private Label actionName;
        private Label actionDescription;
        private PopupMenu popup;

        private bool linked = false;

        public override void _Ready() {
            Link();
            Connect("visibility_changed", this, nameof(Refresh));
            Refresh();
        }

        private void Link() {
            if (linked) {
                return;
            }
            jobSelector = GetNode<MenuButton>("Body/Right/Button");
            actionName = GetNode<Label>("Body/Right/Job/Name");
            actionDescription = GetNode<Label>("Body/Right/Job/Description");
            popup = (PopupMenu) jobSelector.Call("get_popup");
            popup.Connect("id_pressed", this, nameof(on_ChangeAction));
            linked = true;
        }

        private Entity entity;
        private List<VillageAction> actions;
        public void SetEntity(Entity entity) {
            this.entity = entity;
            GetNode<CharacterIcon>("Body/Left/CharacterIcon").SetCharacter(entity);
            actions = entity.AllowedActions();
            if (actions.Contains(VillageAction.QUEST)) {
                actions.Remove(VillageAction.QUEST);
            }
            popup.Clear();
            foreach (VillageAction action in actions) {
                popup.AddItem(VillageActionExtensions.ActionText(action, entity.ageGroup).Item1.ToString(), id: (int) action);
            }
        }


        private void on_ChangeAction(int id) {
            SetAction((VillageAction) id);
        }

        private void SetAction(VillageAction action) {
            Village.actions[entity] = action;
            Refresh();
        }

        private void Refresh() {
            (string name, string description) = entity.ActionText();
            actionName.Text = name;
            actionDescription.Text = description;
            jobSelector.Visible = !Village.actions.EqualsOrFalse(entity, VillageAction.QUEST) && actions.Count > 1;
        }
    }
}