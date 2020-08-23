using System;
using System.Threading.Tasks;
using Godot;
using OutcomeProcesses;
public static class OutcomeProcess {
    public static UI.Outcome ui => UI.Outcome.instance;
    public static async Task Process() {
        GD.Print("--1");
        if (Village.quest != null) {
            await new Battle().Process();
        }
        GD.Print("--2");
        await Town.Process();
        GD.Print("--3");
        Game.data.date = Game.data.date.Plus(1);
        GD.Print("--4");
        History.NextYear();
        GD.Print("--5");
        await End.Process();
        GD.Print("--6");
        ui.GetTree().ChangeScene("Scenes/Village.tscn");
        GD.Print("--7");
    }
}