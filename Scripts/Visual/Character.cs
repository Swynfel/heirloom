using System;
using System.Collections.Generic;
using Godot;

namespace Visual {
    public class Character : Node2D {

        private const int MIN_EYES = 4;
        private const int MAX_EYES = 12;
        private const int MIN_HEAD = 1;
        private const int MAX_HEAD = 16;
        private const int MIN_BODY = 1;
        private const int MAX_BODY = 16;

        enum ColorVariation {
            VERY_DARK = 0,
            DARK = 1,
            NORMAL = 2,
            LIGHT = 3,
        }
        private static Color GetColorVariation(Color color, ColorVariation variation) {
            float brightness = color.v;
            switch (variation) {
                case ColorVariation.VERY_DARK:
                    return color.Darkened(0.2f + brightness * 0.3f);
                case ColorVariation.DARK:
                    return color.Darkened(0.1f + brightness * 0.15f);
                case ColorVariation.LIGHT:
                    return color.Lightened(0.1f + (1 - brightness) * 0.15f);
                default:
                    return color;
            }
        }

        private static void Paint(ShaderMaterial material, Color color, string key, int count = 4, int offset = 0) {
            for (int k = 0 ; k < count ; k++) {
                material.SetShaderParam(string.Format("{0}_{1}", key, k), GetColorVariation(color, (ColorVariation) (k + offset)));
            }
        }

        private static Color EYEBROW_COLOR = Color.Color8(42, 39, 36);

        private Sprite eyes;
        private Sprite head;
        private Sprite body;

        public Color RandomPrimaryColor() {
            return Color.FromHsv((float) (Global.rng.NextDouble()), 0.6f, 0.5f);
        }

        private const float DARKEST_SKIN = 0.22f;
        private const float SKIN_BRIGHTNESS_RANGE = 0.78f;
        // Hard coded formula to try to generate a realistic human skin color
        private const float REDEST_HUE = 0.036f;
        private const float SKIN_HUE_RANGE = 0.028f;
        public Color RandomSkinColor() {
            float rawBrightness = (float) Math.Sqrt(Global.rng.NextDouble());
            float brightness = DARKEST_SKIN + rawBrightness * SKIN_BRIGHTNESS_RANGE;
            float rawSaturation = (float) Global.rng.NextDouble();
            float saturation = (0.6f - 0.35f * rawBrightness) + (0.2f + 0.05f * rawBrightness) * rawSaturation;
            float hue = REDEST_HUE + ((float) Global.rng.NextDouble()) * SKIN_HUE_RANGE;
            return Color.FromHsv(hue, saturation, brightness);
        }

        public Color RandomSecondaryColor(Color primary) {
            if (Global.rng.Next(0, 3) == 0) {
                return Color.FromHsv(0f, 0f, .85f);
            } else {
                return Color.FromHsv(0f, 0f, .2f);
            }
        }

        public void RandomDisplay() {
            // Parts
            eyes.Frame = Global.rng.Next(MIN_EYES, MAX_EYES);
            head.Frame = Global.rng.Next(MIN_HEAD, MAX_HEAD);
            body.Frame = Global.rng.Next(MIN_BODY, MAX_HEAD);
            // Color info
            Color skin = RandomSkinColor();
            // TODO : primary color depends on skin, and element affinity
            Color primary = RandomPrimaryColor();
            // TODO : hair color random and depends on skin, element affinity, and primary
            Color hair = primary;
            Color secondary = RandomSecondaryColor(primary);
            ShaderMaterial material = (ShaderMaterial) Material;
            // Paint
            Paint(material, hair, "hair");
            Paint(material, primary, "primary");
            Paint(material, secondary, "secondary");
            Paint(material, skin, "skin");
            material.SetShaderParam("eyebrows", EYEBROW_COLOR);
        }

        [Export] public bool loadRandomly = true;

        public override void _Ready() {
            eyes = GetNode<Sprite>("Eyes");
            head = GetNode<Sprite>("Head");
            body = GetNode<Sprite>("Body");
            if (loadRandomly) {
                RandomDisplay();
                loadRandomly = false;
            }
        }
    }
}
