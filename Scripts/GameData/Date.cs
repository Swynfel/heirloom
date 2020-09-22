public struct Date : IExportable {
    public enum Season {
        SPRING = 0,
        SUMMER = 1,
        AUTUMN = 2,
        WINTER = 3,
    }

    public enum AgeGroup {
        UNBORN,
        NEWBORN,
        BABY,
        CHILD,
        TEEN,
        YOUNG_ADULT,
        ADULT,
        SENIOR,
    }

    public Season season;
    public int year;
    public Date(int year, Season season) {
        this.year = year;
        this.season = season;
    }

    public static Date FromSeasonsPassed(int seasons) {
        int y = seasons / 4;
        int s = seasons % 4;
        if (s < 0) {
            y--;
            s += 4;
        }
        return new Date(y, (Season) s);
    }

    public int SeasonsPassed() {
        return 4 * year + (int) season;
    }

    public Date Plus(int seasons) {
        return Date.FromSeasonsPassed(SeasonsPassed() + seasons);
    }

    public int Minus(Date date) {
        return SeasonsPassed() - date.SeasonsPassed();
    }
    public static int Delta(Date date) {
        return (Game.data?.date ?? Date.START).Minus(date);
    }

    public override string ToString() {
        return string.Format("{0} {1}", season.Word(), year);
    }

    public string ToLongString() {
        return string.Format("{0} - year {1}", season.Word(), year);
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
    public static string TextAge(int delta) {
        string s = delta >= 2 ? "s" : "";
        return string.Format("{0} season{1} - {2}", delta, s, Age(delta).Word());
    }

    public static AgeGroup Age(int delta) {
        if (delta < 0) {
            return AgeGroup.UNBORN;
        } else if (delta < 1) {
            return AgeGroup.NEWBORN;
        } else if (delta < 2) {
            return AgeGroup.BABY;
        } else if (delta < 4) {
            return AgeGroup.CHILD;
        } else if (delta < 6) {
            return AgeGroup.TEEN;
        } else if (delta < 8) {
            return AgeGroup.YOUNG_ADULT;
        } else if (delta < 12) {
            return AgeGroup.ADULT;
        } else {
            return AgeGroup.SENIOR;
        }
    }
    public string ContextString() {
        if (SeasonsPassed() == NEVER.SeasonsPassed()) {
            return "never";
        }
        return ToString() + " - " + TextDelta(-Date.Delta(this));
    }

    public static readonly Date START = new Date(100, Season.SPRING);
    public static readonly Date NEVER = FromSeasonsPassed(int.MinValue);


    public Godot.Collections.Dictionary SaveProperties() {
        return new Godot.Collections.Dictionary{
            {"year", year},
            {"season", season},
        };
    }
    public void LoadProperties(Godot.Collections.Dictionary data) {
        year = (int) (float) data["year"];
        season = (Date.Season) (int) (float) data["season"];
    }

    public static Date Create() {
        return new Date(0, 0);
    }
}

public static class DateExtensions {
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

    public static string Word(this Date.AgeGroup age) {
        switch (age) {
            case Date.AgeGroup.UNBORN:
                return "unborn";
            case Date.AgeGroup.BABY:
                return "baby";
            case Date.AgeGroup.CHILD:
                return "child";
            case Date.AgeGroup.TEEN:
                return "teenager";
            case Date.AgeGroup.YOUNG_ADULT:
                return "young adult";
            case Date.AgeGroup.ADULT:
                return "adult";
            case Date.AgeGroup.SENIOR:
                return "senior";
            default:
                return "AGEGROUP";
        }
    }
}