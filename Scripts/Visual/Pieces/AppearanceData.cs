using System;
using Godot;

public abstract class AppearanceData : Resource {
    public abstract Node2D GenerateOn(CanvasItem node);
    private static Color FRIENDLY_COLOR = new Color(0f, 0.15f, 0.1f);
    private static Color NEUTRAL_COLOR = new Color(0.25f, 0.25f, 0.25f);
    private static Color HOSTILE_COLOR = new Color(0.5f, 0f, 0f);

    public Node2D GenerateOnWithOutline(CanvasItem node, Combat.Alignment alignment) {
        Node2D appearance = GenerateOn(node);
        Color outlineColor = alignment switch
        {
            Combat.Alignment.FRIENDLY => FRIENDLY_COLOR,
            Combat.Alignment.HOSTILE => HOSTILE_COLOR,
            _ => NEUTRAL_COLOR,
        };
        (appearance?.Material as ShaderMaterial)?.SetShaderParam("outline", outlineColor);
        return appearance;
    }
}