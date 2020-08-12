using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x02000025 RID: 37
	public class USTearGas : Gas
	{
		// Token: 0x06000087 RID: 135 RVA: 0x00005C31 File Offset: 0x00003E31
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, true);
			this.destroyTick = Find.TickManager.TicksGame + this.def.gas.expireSeconds.RandomInRange.SecondsToTicks();
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00005C66 File Offset: 0x00003E66
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.destroyTick, "destroyTick", 0, false);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005C80 File Offset: 0x00003E80
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
						if (Thinglist[i] is Pawn && Thinglist[i].Position == TargetCell)
						{
							this.DoUSTearGasToxic(this, Thinglist[i]);
						}
					}
				}
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005D30 File Offset: 0x00003F30
		public void DoUSTearGasToxic(Thing Gas, Thing targ)
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
					hediff = ((hediffSet != null) ? hediffSet.GetFirstHediffOfDef(Globals.USTearGas, false) : null);
				}
				float sensitivity = victim.GetStatValue(StatDefOf.ToxicSensitivity, true);
				float addsev = 0.125f * sensitivity * this.toxicRatio;
				if (!Globals.USVictimImmuneTo(victim, Globals.USTearGas))
				{
					if (hediff != null && addsev > 0f)
					{
						hediff.Severity += addsev;
						return;
					}
					Hediff addhediff = HediffMaker.MakeHediff(Globals.USTearGas, victim, null);
					addhediff.Severity = addsev;
					victim.health.AddHediff(addhediff, null, null, null);
				}
			}
		}

		// Token: 0x0400004C RID: 76
		public float toxicRatio = Settings.USToxLevels / 100f;
	}
}
