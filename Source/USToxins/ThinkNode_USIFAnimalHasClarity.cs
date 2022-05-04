using RimWorld;
using Verse;
using Verse.AI;

namespace USToxins;

public class ThinkNode_USIFAnimalHasClarity : ThinkNode_Conditional
{
    protected override bool Satisfied(Pawn pawn)
    {
        return Settings.doAnimalFlee && pawn.AnimalOrWildMan() && !pawn.Downed && !pawn.IsBurning() &&
               !pawn.InMentalState && pawn.Awake();
    }
}