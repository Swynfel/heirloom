using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace UI {
    public class Outcome : CanvasLayer {

        public static Outcome instance { get; private set; }
        public Label title;

        private Control _content;
        private Control content {
            get {
                if (_content == null) {
                    _content = GetNode<Control>("OutcomeUI/Content");
                }
                return _content;
            }
        }

        public T N<T>(string p) where T : Node {
            return content.GetNode<T>(p);
        }

        // HEAD
        private TabContainer head;
        private Head current = Head.NONE;
        public enum Head {
            NONE,
            CHARACTER,
            COUPLE,
            BIRTH_OR_ADOPTION,
            QUESTS
        }

        public void OpenHead(Head head) {
            if (current != head) {
                this.head.CurrentTab = (int) head;
                current = head;
            }
        }

        // Character
        private Control characterList;
        public void SetCharacters(IEnumerable<Entity> characters) {
            OpenHead(Head.CHARACTER);
            characterList.QueueFreeChildren();
            foreach (Entity e in characters) {
                characterList.AddChild(Visual.Icons.CharacterIcon.Create(e));
            }
        }

        // Couple

        private Visual.Tables.CharacterTable coupleLeft;
        private Visual.Tables.CharacterTable coupleRight;
        public void SetCouple(Entity left, Entity right) {
            OpenHead(Head.COUPLE);
            coupleLeft.SetCharacter(left);
            coupleRight.SetCharacter(right);
        }

        // Birth or Adoption


        private Visual.Icons.CharacterIcon boaLeft;
        private Visual.Tables.CharacterTable boaMiddle;
        private Visual.Icons.CharacterIcon boaRight;

        public void SetAdoption(Entity parent, Entity child) {
            OpenHead(Head.BIRTH_OR_ADOPTION);
            boaLeft.SetCharacter(parent);
            boaMiddle.SetCharacter(child);
            boaRight.Hide();
        }

        public void SetBirth(Entity leftParent, Entity rightParent, Entity child) {
            OpenHead(Head.BIRTH_OR_ADOPTION);
            boaLeft.SetCharacter(leftParent);
            boaMiddle.SetCharacter(child);
            boaRight.Show();
            boaRight.SetCharacter(rightParent);
        }

        // Quests

        private Visual.Tables.QuestTable questTable;
        public void SetQuest(Quest quest) {
            OpenHead(Head.QUESTS);
            questTable.SetQuest(quest);
        }

        // Buttons

        private Button buttonValidate;
        private Button buttonNo;
        private Button buttonThird;

        public enum ButtonOutcome {
            VALIDATE,
            NO,
            THIRD,
        }

        public void SetButtons(string validate, string no = null, string third = null) {
            buttonValidate.Text = validate;
            if (no == null) {
                buttonNo.Hide();
            } else {
                buttonNo.Text = no;
                buttonNo.Show();
            }
            if (third == null) {
                buttonThird.Hide();
            } else {
                buttonThird.Text = no;
                buttonThird.Show();
            }
        }

        public async Task<ButtonOutcome> ButtonPressed() {
            await ToSignal(this, nameof(button_pressed));
            return buttonJustPressed;
        }

        private ButtonOutcome buttonJustPressed;
        [Signal] delegate void button_pressed();
        private void pressed(int b) {
            buttonJustPressed = (ButtonOutcome) b;
            EmitSignal(nameof(button_pressed));
        }

        public override void _Ready() {
            instance = this;
            head = N<TabContainer>("Head");
            characterList = N<Control>("Head/CharacterList/List");
            coupleLeft = N<Visual.Tables.CharacterTable>("Head/CharacterCouple/Left");
            coupleRight = N<Visual.Tables.CharacterTable>("Head/CharacterCouple/Right");
            boaLeft = N<Visual.Icons.CharacterIcon>("Head/BirthOrAdoption/Left");
            boaMiddle = N<Visual.Tables.CharacterTable>("Head/BirthOrAdoption/Middle");
            boaRight = N<Visual.Icons.CharacterIcon>("Head/BirthOrAdoption/Right");
            questTable = N<Visual.Tables.QuestTable>("Head/Quests/QuestTable");
            buttonValidate = N<Button>("Buttons/Validate");
            buttonNo = N<Button>("Buttons/No");
            buttonThird = N<Button>("Buttons/Third");
            buttonValidate.Connect("pressed", this, nameof(pressed), Global.ArrayFrom(ButtonOutcome.VALIDATE));
            buttonNo.Connect("pressed", this, nameof(pressed), Global.ArrayFrom(ButtonOutcome.NO));
            buttonThird.Connect("pressed", this, nameof(pressed), Global.ArrayFrom(ButtonOutcome.THIRD));
        }
    }
}
