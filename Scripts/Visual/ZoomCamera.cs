using System;
using Godot;

public class ZoomCamera : Node2D {
    public override void _Ready() {
        GetTree().Connect("screen_resized", this, nameof(on_ScreenResized));
        on_ScreenResized();
    }

    private const int MIN_WIDTH = 480;
    private const int MIN_HEIGHT = 270;

    private static Vector2 MIN_WINDOW = new Vector2(MIN_WIDTH, MIN_HEIGHT);
    public void on_ScreenResized() {
        Vector2 size = OS.WindowSize;
        float x_ratio = size.x / MIN_WIDTH;
        float y_ratio = size.y / MIN_HEIGHT;
        int ratio = (int) Math.Max(1, Math.Min(x_ratio, y_ratio));
        if (Math.Min(1.25f, x_ratio - ratio) + Math.Min(1.25f, y_ratio - ratio) >= 1.9f) {
            ratio++;
        }
        GD.Print(x_ratio, " ", y_ratio);
        GetTree().SetScreenStretch(SceneTree.StretchMode.Disabled, SceneTree.StretchAspect.Keep, MIN_WINDOW, ratio);
    }
}
