using RimWorld;
using Verse;
using Verse.AI;

namespace USToxins
{
    // Token: 0x02000017 RID: 23
    public class ThinkNode_USIFAnimalHasClarity : ThinkNode_Conditional
    {
        // Token: 0x06000044 RID: 68 RVA: 0x00003F9E File Offset: 0x0000219E
        protected override bool Satisfied(Pawn pawn)
        {
            return Settings.doAnimalFlee && pawn.AnimalOrWildMan() && !pawn.Downed && !pawn.IsBurning() &&
                   !pawn.InMentalState && pawn.Awake();
        }
    }
}