using System;
using Godot;

namespace Combat {
    using GT = Tile.GroundType;
    public enum MapType {
        PLAINS,
        DESERT,
        DIRT_ROAD,
        STONE,
        DIRT_CAVE,
        STONE_CAVE,
        HIDEOUT,
    }

    internal static class Extensions {
        public static BoardGeneration AsGen(this GT groundType) {
            return new PlainBoardGeneration(groundType);
        }
    }
    public abstract class BoardGeneration {
        public abstract Tile.GroundType Pick(int x, int y);

        public static Func<int, int, GT> Build(MapType map, int width, int height) {
            switch (map) {
                case MapType.PLAINS:
                    return new RoundCornerBoardGeneration(
                        1, 3, width, height,
                        new MixBoardGeneration(GT.GRASS.AsGen(), GT.DIRT.AsGen(), 0.1f)
                    ).Pick;
                case MapType.DESERT:
                    return new RoundCornerBoardGeneration(
                        1, 3, width, height,
                        new MixBoardGeneration(GT.SAND.AsGen(), GT.DRY.AsGen(), 0f)
                    ).Pick;
                case MapType.DIRT_ROAD:
                    return new RoundCornerBoardGeneration(
                        1, 3, width, height,
                        new RoadBoardGeneration(width, height, GT.GRASS.AsGen(), GT.DIRT.AsGen(), 1.5f, 0.5f, 2f)
                    ).Pick;
                case MapType.DIRT_CAVE:
                    return new RoundCornerBoardGeneration(
                        1, 3, width, height,
                        new LakeBoardGeneration(width, height, GT.DIRT.AsGen(), GT.NONE.AsGen(), 0f, 2f)
                    ).Pick;
                case MapType.STONE_CAVE:
                    return new RoundCornerBoardGeneration(
                        1, 3, width, height,
                        new LakeBoardGeneration(width, height, GT.STONE.AsGen(), GT.NONE.AsGen(), 0f, 2f)
                    ).Pick;
                case MapType.HIDEOUT:
                    return GT.WOOD.AsGen().Pick; //TODO: Fancier
                default:
                    throw new NotImplementedException($"Map type {map} not implemented");
            }
        }
    }

    internal class PlainBoardGeneration : BoardGeneration {
        GT groundType;

        public PlainBoardGeneration(GT groundType) {
            this.groundType = groundType;
        }

        public override GT Pick(int x, int y) {
            return groundType;
        }
    }

    internal class RoundCornerBoardGeneration : BoardGeneration {
        int topLeft;
        int topRight;
        int bottomLeft;
        int bottomRight;
        int width;
        int height;
        BoardGeneration innerGeneration;
        public RoundCornerBoardGeneration(int chippedCornerMin, int chippedCornerMax, int width, int height, BoardGeneration innerGeneration) {
            this.width = width - 1;
            this.height = height - 1;
            this.innerGeneration = innerGeneration;
            topLeft = Global.rng.Next(chippedCornerMin, chippedCornerMax + 1);
            topRight = Global.rng.Next(chippedCornerMin, chippedCornerMax + 1);
            bottomLeft = Global.rng.Next(chippedCornerMin, chippedCornerMax + 1);
            bottomRight = Global.rng.Next(chippedCornerMin, chippedCornerMax + 1);
        }

        public override GT Pick(int x, int y) {
            if ((x + y < topLeft) || ((width - x) + y < topRight)
            || (x + (height - y)) < bottomLeft || ((width - x) + (height - y)) < bottomRight) {
                return GT.NONE;
            }
            return innerGeneration.Pick(x, y);
        }
    }

    internal class RoadBoardGeneration : BoardGeneration {
        BoardGeneration outside;
        BoardGeneration inside;
        Vector2 direction;
        Vector2 center;
        float bent;
        float halfThickness;
        float sinusStretch;
        float sinusImpact;
        public RoadBoardGeneration(int width, int height, BoardGeneration outside, BoardGeneration inside, float halfThickness, float sinusStretch, float sinusImpact) {
            this.outside = outside;
            this.inside = inside;
            this.halfThickness = halfThickness;
            this.sinusStretch = sinusStretch;
            this.sinusImpact = sinusImpact;
            center = new Vector2(Global.rng.Next(width / 3, width * 2 / 3), Global.rng.Next(height / 3, height * 2 / 3));
            bent = (float) Global.rng.NextDouble();
            double angle = Global.rng.NextDouble();
            direction = new Vector2((float) Math.Sin(angle), (float) Math.Cos(angle));
        }
        public override GT Pick(int x, int y) {
            Vector2 offset = new Vector2(x, y) - center;
            float along = offset.Dot(direction);
            float off = offset.Cross(direction) + sinusImpact * (float) Math.Sin(sinusStretch * along + bent);
            return Math.Abs(off) > halfThickness ? outside.Pick(x, y) : inside.Pick(x, y);
        }
    }

    internal class MixBoardGeneration : BoardGeneration {
        BoardGeneration main;
        BoardGeneration secondary;
        float bias;
        OpenSimplexNoise noise;
        public MixBoardGeneration(BoardGeneration main, BoardGeneration secondary, float bias, float period = 2f, float persistence = 0.5f, float lacunarity = 2f) {
            this.main = main;
            this.secondary = secondary;
            this.bias = bias;
            noise = new OpenSimplexNoise {
                Period = period,
                Persistence = persistence,
                Lacunarity = lacunarity,
            };
            noise.Seed = Global.rng.Next();
        }
        public override GT Pick(int x, int y) {
            return noise.GetNoise2d(x, y) <= bias ? main.Pick(x, y) : secondary.Pick(x, y);
        }
    }

    internal class LakeBoardGeneration : BoardGeneration {
        BoardGeneration outside;
        BoardGeneration inside;
        Vector2 direction;
        Vector2 center;

        float lakeHalfWidth;
        float lakeHalfHeight;
        float lakeMinRadius;
        float lakeMaxRadius;
        float sinusImpact;
        public LakeBoardGeneration(int width, int height, BoardGeneration outside, BoardGeneration inside, float lakeMinRadius, float lakeMaxRadius) {
            this.outside = outside;
            this.inside = inside;
            this.lakeMinRadius = lakeMinRadius;
            this.lakeMaxRadius = lakeMaxRadius;
            center = new Vector2(Global.rng.Next(width / 3, width * 2 / 3), Global.rng.Next(height / 3, height * 2 / 3));
            double angle = Global.rng.NextDouble();
            direction = new Vector2((float) Math.Sin(angle), (float) Math.Cos(angle));
            lakeHalfHeight = (float) (Global.rng.NextDouble() % (lakeMaxRadius - lakeMinRadius)) + lakeMinRadius;
            lakeHalfWidth = (float) (Global.rng.NextDouble() % (lakeMaxRadius - lakeMinRadius)) + lakeMinRadius;
        }
        public override GT Pick(int x, int y) {
            Vector2 offset = new Vector2(x, y) - center;
            float i = offset.Dot(direction) / lakeHalfWidth;
            float j = offset.Cross(direction) / lakeHalfHeight;
            float r = i * i + j * j;
            return r > 1 ? outside.Pick(x, y) : inside.Pick(x, y);
        }
    }
}