using System;
using Godot;

namespace Combat.Generate {
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
        // Pick Ground Type
        public abstract GT Pick(int x, int y);

        // If Ground Type isn't null, generate potential Obstacle
        public abstract Entity Obstacle(int x, int y);

        public static BoardGeneration Build(MapType map, int width, int height) {
            switch (map) {
                case MapType.PLAINS:
                    return new RoundCornerBoardGeneration(
                        1, 3, width, height,
                        new MixBoardGeneration(GT.GRASS.AsGen(), GT.DIRT.AsGen(), 0.1f)
                    );
                case MapType.DESERT:
                    return new RoundCornerBoardGeneration(
                        1, 3, width, height,
                        new MixBoardGeneration(GT.SAND.AsGen(), GT.DRY.AsGen(), 0f)
                    );
                case MapType.DIRT_ROAD:
                    return new RoundCornerBoardGeneration(
                        1, 3, width, height,
                        new RoadBoardGeneration(width, height, GT.GRASS.AsGen(), GT.DIRT.AsGen(), 1.5f, 0.5f, 2f)
                    );
                case MapType.DIRT_CAVE:
                    return new RoundCornerBoardGeneration(
                        1, 3, width, height,
                        new LakeBoardGeneration(width, height, GT.DIRT.AsGen(), GT.NONE.AsGen(), 0f, 2f)
                    );
                case MapType.STONE_CAVE:
                    return new RoundCornerBoardGeneration(
                        1, 3, width, height,
                        new LakeBoardGeneration(width, height, GT.STONE.AsGen(), GT.NONE.AsGen(), 0f, 2f)
                    );
                case MapType.HIDEOUT:
                    return GT.WOOD.AsGen(); //TODO: Fancier
                default:
                    throw new NotImplementedException($"Map type {map} not implemented");
            }
        }
    }

    internal abstract class MultiBoardGeneration : BoardGeneration {

        public abstract BoardGeneration Which(int x, int y);

        public override GT Pick(int x, int y) {
            return Which(x, y)?.Pick(x, y) ?? GT.NONE;
        }

        public override Entity Obstacle(int x, int y) {
            return Which(x, y)?.Obstacle(x, y);
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

        public override Entity Obstacle(int x, int y) {
            return null;
        }
    }

    internal class RoundCornerBoardGeneration : MultiBoardGeneration {
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

        public override BoardGeneration Which(int x, int y) {
            if ((x + y < topLeft) || ((width - x) + y < topRight)
            || (x + (height - y)) < bottomLeft || ((width - x) + (height - y)) < bottomRight) {
                return null;
            }
            return innerGeneration;
        }
    }

    internal class RoadBoardGeneration : MultiBoardGeneration {
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
        public override BoardGeneration Which(int x, int y) {
            Vector2 offset = new Vector2(x, y) - center;
            float along = offset.Dot(direction);
            float off = offset.Cross(direction) + sinusImpact * (float) Math.Sin(sinusStretch * along + bent);
            return Math.Abs(off) > halfThickness ? outside : inside;
        }
    }

    internal class MixBoardGeneration : MultiBoardGeneration {
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
        public override BoardGeneration Which(int x, int y) {
            return noise.GetNoise2d(x, y) <= bias ? main : secondary;
        }
    }

    internal class LakeBoardGeneration : MultiBoardGeneration {
        BoardGeneration outside;
        BoardGeneration inside;
        Vector2 direction;
        Vector2 center;

        float lakeHalfWidth;
        float lakeHalfHeight;
        float lakeMinRadius;
        float lakeMaxRadius;
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
        public override BoardGeneration Which(int x, int y) {
            Vector2 offset = new Vector2(x, y) - center;
            float i = offset.Dot(direction) / lakeHalfWidth;
            float j = offset.Cross(direction) / lakeHalfHeight;
            float r = i * i + j * j;
            return r > 1 ? outside : inside;
        }
    }
}