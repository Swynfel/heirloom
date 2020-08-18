using System;
using System.Collections.Generic;
using Godot;

namespace Visual {
    public class Character : Node2D {

        public enum PaletteColor {
            WHITE,
            BLACK,
            RED,
            PINK,
            //ORANGE,
            YELLOW,
            GREEN,
            // CYAN,
            // BLUE,
            // PURPLE,
        }

        public static List<PaletteColor> PRIMARY_COLORS = new List<PaletteColor> { PaletteColor.RED, PaletteColor.PINK, PaletteColor.YELLOW, PaletteColor.GREEN };
        public static List<PaletteColor> SECONDARY_COLORS = new List<PaletteColor> { PaletteColor.WHITE, PaletteColor.BLACK };

        private static PaletteColor Random(List<PaletteColor> list) {
            return list[Global.rng.Next(0, list.Count)];
        }

        private static void Paint(ShaderMaterial material, PaletteColor color, string key, int count = 4, int offset = 0) {
            Paint(material, GetColors(color), key, count, offset);
        }

        private static void Paint(ShaderMaterial material, Color[] colors, string key, int count = 4, int offset = 0) {
            for (int k = 0 ; k < count ; k++) {
                material.SetShaderParam(string.Format("{0}_{1}", key, k), colors[k + offset]);
            }
        }

        private static Color EYEBROW_COLOR = Color.Color8(39, 37, 35);

        private static Color[][] palettes = {
            // WHITE
            new Color[] { Color.Color8(66, 66, 66), Color.Color8(138, 138, 138), Color.Color8(195, 195, 195), Color.Color8(223, 223, 223) },
            // BLACK
            new Color[] { Color.Color8(15, 15, 15), Color.Color8(32, 32, 32), Color.Color8(43, 43, 43), Color.Color8(66, 66, 66) },
            // RED
            new Color[] { Color.Color8(90, 5, 5), Color.Color8(130, 20, 20), Color.Color8(160, 40, 40), Color.Color8(170, 55, 55) },
            // PINK
            new Color[] { Color.Color8(103, 17, 48), Color.Color8(118, 25, 58), Color.Color8(133, 44, 75), Color.Color8(156, 64, 96) },
            // ORANGE
            //new Color[] { Color.Color8(80, 3, 3), Color.Color8(130, 20, 20), Color.Color8(160, 40, 40), Color.Color8(170, 55, 55) },
            // YELLOW
            new Color[] { Color.Color8(112, 81, 6), Color.Color8(148, 114, 35), Color.Color8(176, 144, 69), Color.Color8(216, 180, 97) },
            // GREEN
            new Color[] { Color.Color8(15, 58, 15), Color.Color8(31, 92, 31), Color.Color8(51, 114, 51), Color.Color8(65, 152, 65) },
            // CYAN
            // new Color[] { Color.Color8(15, 58, 15), Color.Color8(31, 92, 31), Color.Color8(51, 114, 51), Color.Color8(65, 152, 65) },
            // BLUE
            // new Color[] { Color.Color8(15, 58, 15), Color.Color8(31, 92, 31), Color.Color8(51, 114, 51), Color.Color8(65, 152, 65) },
            // PURPLE
            // new Color[] { Color.Color8(15, 58, 15), Color.Color8(31, 92, 31), Color.Color8(51, 114, 51), Color.Color8(65, 152, 65) },
        };

        public static Color[] GetColors(PaletteColor color) {
            return palettes[(int) color];
        }
        private Sprite head;
        private Sprite body;

        public void RandomDisplay() {
            head.Frame = Global.rng.Next(3, 8);
            body.Frame = Global.rng.Next(4, 8);
            PaletteColor primary = Random(PRIMARY_COLORS);
            PaletteColor secondary = Random(SECONDARY_COLORS);
            ShaderMaterial material = (ShaderMaterial) Material;
            Paint(material, primary, "hair");
            Paint(material, primary, "primary");
            Paint(material, secondary, "secondary");
            material.SetShaderParam("eyebrows", EYEBROW_COLOR);
        }

        public override void _Ready() {
            head = GetNode<Sprite>("Head");
            body = GetNode<Sprite>("Body");
            RandomDisplay();
        }
    }
}
