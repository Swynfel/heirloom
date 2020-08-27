using System;
using System.Threading.Tasks;
using Godot;
using OutcomeProcesses;
public static class OutcomeProcess {
    public static UI.Outcome ui => UI.Outcome.instance;
    public static async Task Process() {
        if (Village.quest != null) {
            GD.Print("[OUTCOME] Battle");
            await new Battle().Process();
        }
        GD.Print("[OUTCOME] Town");
        await Town.Process();
        GD.Print("[OUTCOME] Date++");
        Game.data.date = Game.data.date.Plus(1);
        History.NextYear();

        GD.Print("[OUTCOME] End");
        await End.Process();

        ui.GetTree().ChangeScene("Scenes/Village.tscn");
    }
}