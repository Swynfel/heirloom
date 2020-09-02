using System;
using Godot;

namespace Visual.Tables {
    public class InfoTable : Control {
        public override void _Ready() {
            InfoText found = InfoText.Find("empty");
            if (found == null) {
                GD.Print("Null");
            } else {
                SetInfoText(found);
            }
        }

        public void SetInfoText(InfoText infoText) {
            GetNode<Label>("Title").Text = infoText.Title;
            GetNode<SmartText>("Scroll/Description").BbcodeText = infoText.BBDescription;
        }
    }
}
