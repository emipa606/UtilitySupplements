using Verse;

namespace USToxins;

public class CompProperties_USToxUses : CompProperties
{
    public int USToxUses = 100;

    public CompProperties_USToxUses()
    {
        compClass = typeof(CompUSToxUses);
    }
}