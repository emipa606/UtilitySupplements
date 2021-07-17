using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace USToxins
{
    // Token: 0x02000011 RID: 17
    public class JobGiver_USTanglerootAnimalFlee : ThinkNode_JobGiver
    {
        // Token: 0x0400002E RID: 46
        private const int FleeDistance = 20;

        // Token: 0x0400002F RID: 47
        private const int DistToDangerToFlee = 5;

        // Token: 0x04000030 RID: 48
        private static readonly List<Thing> tmpThings = new List<Thing>();

        // Token: 0x0600002E RID: 46 RVA: 0x0000329C File Offset: 0x0000149C
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!Settings.doAnimalFlee)
            {
                return null;
            }

            if (!pawn.AnimalOrWildMan())
            {
                return null;
            }

            if (ThinkNode_ConditionalShouldFollowMaster.ShouldFollowMaster(pawn))
            {
                return null;
            }

            if (pawn.GetLord() != null || pawn.Faction == Faction.OfPlayer && pawn.Map.IsPlayerHome ||
                pawn.CurJob != null)
            {
                return null;
            }

            var Plantlist = pawn.Map.listerThings.ThingsOfDef(ThingDef.Named("Plant_USTangleroot"));
            foreach (var thing in Plantlist)
            {
                var plantchk = thing as Plant;
                if (plantchk?.def.defName != "Plant_USTangleroot" ||
                    !((plantchk.Position - pawn.Position).LengthHorizontal <= 5f))
                {
                    continue;
                }

                var fleeplant = FleeJob(pawn, plantchk);
                if (fleeplant != null)
                {
                    return fleeplant;
                }
            }

            return null;
        }

        // Token: 0x0600002F RID: 47 RVA: 0x0000337C File Offset: 0x0000157C
        private Job FleeJob(Pawn pawn, Thing danger)
        {
            IntVec3 intVec;
            if (pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Flee)
            {
                intVec = pawn.CurJob.targetA.Cell;
            }
            else
            {
                tmpThings.Clear();
                tmpThings.Add(danger);
                intVec = CellFinderLoose.GetFleeDest(pawn, tmpThings, 20f);
                tmpThings.Clear();
            }

            if (intVec != pawn.Position)
            {
                return new Job(JobDefOf.Flee, intVec, danger);
            }

            return null;
        }
    }
}