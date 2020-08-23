using System;
using System.Threading.Tasks;
using Godot;
using OutcomeProcesses;
public static class OutcomeProcess {
    public static UI.Outcome ui => UI.Outcome.instance;
    public static async Task Process() {
        if (Village.quest != null) {
            await new Battle().Process();
        }
        await Town.Process();
        Game.data.date = Game.data.date.Plus(1);
        History.NextYear();
        await End.Process();
        ui.GetTree().ChangeScene("Scenes/Village.tscn");
    }
}