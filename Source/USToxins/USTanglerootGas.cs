using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x02000024 RID: 36
	public class USTanglerootGas : Gas
	{
		// Token: 0x06000082 RID: 130 RVA: 0x000059D9 File Offset: 0x00003BD9
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, true);
			this.destroyTick = Find.TickManager.TicksGame + this.def.gas.expireSeconds.RandomInRange.SecondsToTicks();
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00005A0E File Offset: 0x00003C0E
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.destroyTick, "destroyTick", 0, false);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00005A28 File Offset: 0x00003C28
		public override void Tick()
		{
			if (this.destroyTick <= Find.TickManager.TicksGame)
			{
				this.Destroy(DestroyMode.Vanish);
			}
			this.graphicRotation += this.graphicRotationSpeed;
			if (!base.Destroyed && Find.TickManager.TicksGame % 120 == 0)
			{
				Map TargetMap = base.Map;
				IntVec3 TargetCell = base.Position;
				List<Thing> Thinglist = TargetCell.GetThingList(TargetMap);
				if (Thinglist.Count > 0)
				{
					for (int i = 0; i < Thinglist.Count; i++)
					{
						if (Thinglist[i] is Pawn && !(Thinglist[i] as Pawn).RaceProps.IsMechanoid && !((Thinglist[i] as Pawn).RaceProps.FleshType.defName == "Insectoid") && Thinglist[i].Position == TargetCell)
						{
							this.DoUSTangleKillGasToxic(this, Thinglist[i]);
						}
					}
				}
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00005B20 File Offset: 0x00003D20
		public void DoUSTangleKillGasToxic(Thing Gas, Thing targ)
		{
			Pawn victim = targ as Pawn;
			if (victim != null && victim.health.capacities.CapableOf(PawnCapacityDefOf.Breathing))
			{
				Pawn_HealthTracker health = victim.health;
				Hediff hediff;
				if (health == null)
				{
					hediff = null;
				}
				else
				{
					HediffSet hediffSet = health.hediffSet;
					hediff = ((hediffSet != null) ? hediffSet.GetFirstHediffOfDef(Globals.USTangleRootStrike, false) : null);
				}
				float sensitivity = victim.GetStatValue(StatDefOf.ToxicSensitivity, true);
				float addsev = Rand.Range(0.01f * sensitivity * this.toxicRatio, 0.05f * sensitivity * this.toxicRatio);
				if (!Globals.USVictimImmuneTo(victim, Globals.USTangleRootStrike))
				{
					if (hediff != null && addsev > 0f)
					{
						if (hediff.Severity + addsev < 1.8f)
						{
							hediff.Severity += addsev;
							return;
						}
					}
					else
					{
						Hediff addhediff = HediffMaker.MakeHediff(Globals.USTangleRootStrike, victim, null);
						addhediff.Severity = addsev;
						victim.health.AddHediff(addhediff, null, null, null);
					}
				}
			}
		}

		// Token: 0x0400004B RID: 75
		public float toxicRatio = Settings.USToxLevels / 100f;
	}
}
