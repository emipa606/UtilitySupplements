using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x02000023 RID: 35
	public class USTangleKillGas : Gas
	{
		// Token: 0x0600007D RID: 125 RVA: 0x00005790 File Offset: 0x00003990
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, true);
			this.destroyTick = Find.TickManager.TicksGame + this.def.gas.expireSeconds.RandomInRange.SecondsToTicks();
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000057C5 File Offset: 0x000039C5
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.destroyTick, "destroyTick", 0, false);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000057E0 File Offset: 0x000039E0
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

		// Token: 0x06000080 RID: 128 RVA: 0x000058D8 File Offset: 0x00003AD8
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
				float addsev = Rand.Range(0.0249999985f * sensitivity * this.toxicRatio, 0.125f * sensitivity * this.toxicRatio);
				if (!Globals.USVictimImmuneTo(victim, Globals.USTangleRootStrike))
				{
					if (hediff != null && addsev > 0f)
					{
						hediff.Severity += addsev;
						return;
					}
					Hediff addhediff = HediffMaker.MakeHediff(Globals.USTangleRootStrike, victim, null);
					addhediff.Severity = addsev;
					victim.health.AddHediff(addhediff, null, null, null);
				}
			}
		}

		// Token: 0x0400004A RID: 74
		public float toxicRatio = Settings.USToxLevels / 100f;
	}
}
