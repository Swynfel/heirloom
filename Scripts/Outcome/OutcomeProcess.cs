using System;
using System.Threading.Tasks;
using Godot;
using OutcomeProcesses;
public static class OutcomeProcess {
    public static UI.Outcome ui => UI.Outcome.instance;
    public static async Task Process() {
        Village.quest = Game.data.quests[0];
        if (Village.quest != null) {
            await new Battle().Process();
        }
        ui.SetTitle("DONE");
        GD.Print("DONE");
    }
}