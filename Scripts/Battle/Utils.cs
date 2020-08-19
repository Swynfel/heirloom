namespace Combat {
    public enum Direction {
        NONE,
        RIGHT,
        UP,
        LEFT,
        DOWN,
    }

    public enum Alignment {
        NEUTRAL,
        FRIENDLY,
        HOSTILE,
    }

    public static class Utils {
        public static Direction[] DIRECTIONS = { Direction.RIGHT, Direction.UP, Direction.LEFT, Direction.DOWN };
        public static Direction Opposite(this Direction direction) {
            switch (direction) {
                case Direction.RIGHT:
                    return Direction.LEFT;
                case Direction.UP:
                    return Direction.DOWN;
                case Direction.LEFT:
                    return Direction.RIGHT;
                case Direction.DOWN:
                    return Direction.UP;
                default:
                    return Direction.NONE;
            }
        }
    }
}