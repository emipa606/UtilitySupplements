using Verse;

namespace USToxins;

public class CompProperties_USToxUses : CompProperties
{
    public readonly int USToxUses = 100;

    public CompProperties_USToxUses()
    {
        compClass = typeof(CompUSToxUses);
    }
}