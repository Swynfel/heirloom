using System;
using System.Collections.Generic;
using Godot;

namespace Visual {
    enum ColorVariation {
        VERY_DARK = 0,
        DARK = 1,
        NORMAL = 2,
        LIGHT = 3,
    }

    // DUO, TRIO, QUARTET
    // Primary, Secondary, Hair, Eye
    public struct ColorDisposition {
        private const ushort THREE = 3;
        private ushort disposition;
        public int primary { get => disposition & THREE; }
        public int secondary { get => (disposition >> 2) & THREE; }
        public int hair { get => (disposition >> 4) & THREE; }
        public int eyes { get => (disposition >> 6) & THREE; }

        public int Total() {
            return Math.Max(Math.Max(primary, secondary), Math.Max(hair, eyes)) + 1;
        }
        public ColorDisposition(ColorDisposition other) {
            disposition = other.disposition;
        }
        public ColorDisposition(int p, int s, int h, int e) {
            if (p < 0 || p >= 4 || s < 0 || s >= 4 || h < 0 || h >= 4 || e < 0 || e >= 4) {
                GD.PrintErr("Error while creating color disposition");
                disposition = 0;
                return;
            }
            disposition = (ushort) (p + (s << 2) + (h << 4) + (e << 6));
        }
    }

    public class CharacterAppearanceData : Resource {
        private const int MIN_EYES = 4;
        private const int MAX_EYES = 12;
        private const int MIN_HEAD = 1;
        private const int MAX_HEAD = 16;
        private const int MIN_BODY = 1;
        private const int MAX_BODY = 16;

        [Export] public int eyes;
        [Export] public int head;
        [Export] public int body;
        [Export] public ColorDisposition disposition;
        [Export] public Color[] colors;

        [Export] public Color skinColor;

        /*** static utils ***/

        // TODO: Slightly increase saturation
        private static Color GetColorVariation(Color color, int variation) {
            if (variation == 0) {
                return color;
            }
            float brightness = color.v;
            if (variation < 0) {
                float delta = -variation * (0.1f + brightness * 0.15f);
                return color.Darkened(delta);
            } else {
                float delta = variation * (0.1f + (1f - brightness) * 0.15f);
                return color.Lightened(delta);
            }
        }

        private const float DARKEST_SKIN = 0.22f;
        private const float SKIN_BRIGHTNESS_RANGE = 0.78f;
        // Hard coded formula to try to generate a realistic human skin color
        private const float REDEST_HUE = 0.036f;
        private const float SKIN_HUE_RANGE = 0.028f;
        public static Color RandomSkinColor() {
            float rawBrightness = (float) Math.Sqrt(Global.rng.NextDouble());
            float brightness = DARKEST_SKIN + rawBrightness * SKIN_BRIGHTNESS_RANGE;
            float rawSaturation = (float) Global.rng.NextDouble();
            float saturation = (0.6f - 0.35f * rawBrightness) + (0.2f + 0.05f * rawBrightness) * rawSaturation;
            float hue = REDEST_HUE + ((float) Global.rng.NextDouble()) * SKIN_HUE_RANGE;
            return Color.FromHsv(hue, saturation, brightness);
        }

        public static Color RandomPrimaryColor() {
            return Color.FromHsv((float) (Global.rng.NextDouble()), 0.6f, 0.5f);
        }

        public static Color RandomSecondaryColor(Color primary) {
            if (Global.rng.Next(0, 3) == 0) {
                return Color.FromHsv(0f, 0f, .85f);
            } else {
                return Color.FromHsv(0f, 0f, .2f);
            }
        }
        private static Color EYEBROW_COLOR = Color.Color8(42, 39, 36);

        private static void Paint(ShaderMaterial material, Color color, string key, IEnumerable<int> variations) {
            int k = 0;
            foreach (int variation in variations) {
                material.SetShaderParam(string.Format("{0}_{1}", key, k), GetColorVariation(color, variation));
                k++;
            }
        }

        private static ColorDisposition PRIMARY_HAIR = new ColorDisposition(0, 1, 0, 2);

        /*** generation ***/
        public static CharacterAppearanceData Random() {
            CharacterAppearanceData data = new CharacterAppearanceData();
            // Parts
            data.eyes = Global.rng.Next(MIN_EYES, MAX_EYES);
            data.head = Global.rng.Next(MIN_HEAD, MAX_HEAD);
            data.body = Global.rng.Next(MIN_BODY, MAX_HEAD);
            // Color info
            data.skinColor = RandomSkinColor();
            // TODO : Other Dispositions
            data.disposition = PRIMARY_HAIR;
            // TODO : primary color depends on skin, and element affinity
            Color primary = RandomPrimaryColor();
            // TODO : hair color random and depends on skin, element affinity, and primary
            Color secondary = RandomSecondaryColor(primary);
            Color eyes = RandomPrimaryColor();
            data.colors = new Color[] { primary, secondary, eyes };
            return data;
        }

        private static int[] PRIMARY_VARIATIONS = { -2, -1, 0, 1 };
        private static int[] SECONDARY_VARIATIONS = { -1, 0, 1 };
        private static int[] HAIR_VARIATIONS = { -2, -1, 0, 2 };
        private static int[] IRIS_VARIATIONS = { -2, 0, 1 };
        private static int[] SKIN_VARIATIONS = { -2, -1, 0, 1 };
        private static int[] EYEBALL_VARIATIONS = { -1, 0 };

        public void Paint(CharacterAppearance appearance) {
            appearance.eyes.Frame = eyes;
            appearance.head.Frame = head;
            appearance.body.Frame = body;
            ShaderMaterial material = (ShaderMaterial) appearance.Material;
            Paint(material, colors[disposition.primary], "primary", PRIMARY_VARIATIONS);
            Paint(material, colors[disposition.secondary], "secondary", SECONDARY_VARIATIONS);
            Paint(material, colors[disposition.hair], "hair", HAIR_VARIATIONS);
            Paint(material, colors[disposition.eyes], "iris", IRIS_VARIATIONS);
            Paint(material, skinColor, "skin", SKIN_VARIATIONS);
            Paint(material, Colors.White, "eyeball", EYEBALL_VARIATIONS);
            material.SetShaderParam("eyebrows", EYEBROW_COLOR);
        }
    }
}
