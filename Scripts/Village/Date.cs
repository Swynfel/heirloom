public struct Date {
    public enum Season {
        SPRING = 0,
        SUMMER = 1,
        AUTUMN = 2,
        WINTER = 3,
    }

    public Season season;
    public int year;

    public Date(int year, Season season) {
        this.year = year;
        this.season = season;
    }

    public static Date FromSeasonsPassed(int seasons) {
        return new Date(seasons / 4, (Season) (seasons % 4));
    }

    public int SeasonsPassed() {
        return 4 * year + (int) season;
    }

    public Date Plus(int seasons) {
        return Date.FromSeasonsPassed(SeasonsPassed() - seasons);
    }

    public int Delta(Date date) {
        return SeasonsPassed() - date.SeasonsPassed();
    }

    public override string ToString() {
        return string.Format("{0} {1}", season.Word(), year);
    }

    public static string TextDelta(int delta) {
        if (delta <= -8) {
            return (delta / 4) + " years ago";
        } else if (delta >= 8) {
            return "in " + (delta / 4) + " years";
        }
        switch (delta) {
            case -7:
            case -6:
            case -5:
            case -4:
                return "last year";
            case -3:
                return "3 seasons ago";
            case -2:
                return "half a year ago";
            case -1:
                return "last season";
            case 0:
                return "now";
            case 1:
                return "next season";
            case 2:
                return "in half a year";
            case 3:
                return "in 3 seasons";
            case 4:
            case 5:
            case 6:
            case 7:
                return "next year";
        }
        return "ERROR";
    }

    public string ContextString() {
        return ToString() + " - " + TextDelta(Game.data.date.Delta(this));
    }
}

public static class SeasonExtensions {
    public static string Word(this Date.Season season) {
        switch (season) {
            case Date.Season.SPRING:
                return "Spring";
            case Date.Season.SUMMER:
                return "Summer";
            case Date.Season.AUTUMN:
                return "Autumn";
            case Date.Season.WINTER:
                return "Winter";
            default:
                return "SEASON";
        }
    }
}