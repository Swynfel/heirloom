using Utils;

public class AdventureProgress : ISaveable {
    [Save] public bool foundCrownQuest = false;
    [Save] public bool foundShieldQuest = false;
    [Save] public bool foundFinalQuest = false;
    [Save] public int dungeonsFound = 0;
}