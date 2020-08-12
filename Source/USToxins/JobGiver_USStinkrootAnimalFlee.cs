using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace USToxins
{
	// Token: 0x02000010 RID: 16
	public class JobGiver_USStinkrootAnimalFlee : ThinkNode_JobGiver
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00003118 File Offset: 0x00001318
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
			if (pawn.GetLord() == null && (pawn.Faction != Faction.OfPlayer || !pawn.Map.IsPlayerHome) && pawn.CurJob == null)
			{
				List<Thing> Plantlist = pawn.Map.listerThings.ThingsOfDef(ThingDef.Named("Plant_USStinkroot"));
				for (int i = 0; i < Plantlist.Count; i++)
				{
					Plant plantchk = Plantlist[i] as Plant;
					if (plantchk.def.defName == "Plant_USStinkroot" && (plantchk.Position - pawn.Position).LengthHorizontal <= 5f)
					{
						Job fleeplant = this.FleeJob(pawn, plantchk);
						if (fleeplant != null)
						{
							return fleeplant;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000031F8 File Offset: 0x000013F8
		private Job FleeJob(Pawn pawn, Thing danger)
		{
			IntVec3 intVec;
			if (pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Flee)
			{
				intVec = pawn.CurJob.targetA.Cell;
			}
			else
			{
				JobGiver_USStinkrootAnimalFlee.tmpThings.Clear();
				JobGiver_USStinkrootAnimalFlee.tmpThings.Add(danger);
				intVec = CellFinderLoose.GetFleeDest(pawn, JobGiver_USStinkrootAnimalFlee.tmpThings, 24f);
				JobGiver_USStinkrootAnimalFlee.tmpThings.Clear();
			}
			if (intVec != pawn.Position)
			{
				return new Job(JobDefOf.Flee, intVec, danger);
			}
			return null;
		}

		// Token: 0x0400002B RID: 43
		private const int FleeDistance = 24;

		// Token: 0x0400002C RID: 44
		private const int DistToDangerToFlee = 5;

		// Token: 0x0400002D RID: 45
		private static List<Thing> tmpThings = new List<Thing>();
	}
}
