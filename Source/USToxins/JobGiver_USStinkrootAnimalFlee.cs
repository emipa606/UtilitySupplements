using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace USToxins;

public class JobGiver_USStinkrootAnimalFlee : ThinkNode_JobGiver
{
    private const int FleeDistance = 24;

    private const int DistToDangerToFlee = 5;

    private static readonly List<Thing> tmpThings = [];

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

        var Plantlist = pawn.Map.listerThings.ThingsOfDef(ThingDef.Named("Plant_USStinkroot"));
        foreach (var thing in Plantlist)
        {
            var plantchk = thing as Plant;
            if (plantchk?.def.defName != "Plant_USStinkroot" ||
                !((plantchk.Position - pawn.Position).LengthHorizontal <= DistToDangerToFlee))
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
            intVec = CellFinderLoose.GetFleeDest(pawn, tmpThings, FleeDistance);
            tmpThings.Clear();
        }

        return intVec != pawn.Position ? new Job(JobDefOf.Flee, intVec, danger) : null;
    }
}