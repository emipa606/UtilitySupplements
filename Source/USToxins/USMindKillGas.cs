using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x02000020 RID: 32
	public class USMindKillGas : Gas
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00004E88 File Offset: 0x00003088
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, true);
			this.destroyTick = Find.TickManager.TicksGame + this.def.gas.expireSeconds.RandomInRange.SecondsToTicks();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004EBD File Offset: 0x000030BD
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.destroyTick, "destroyTick", 0, false);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00004ED8 File Offset: 0x000030D8
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
							this.DoUSMindKillGasToxic(this, Thinglist[i]);
						}
						if (Thinglist[i] is Plant && (Thinglist[i] as Plant).def.plant.purpose != PlantPurpose.Health && (Thinglist[i] as Plant).def.defName != "Plant_USStinkroot")
						{
							this.DoUSToxicDamage(Thinglist[i]);
						}
					}
				}
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004FF4 File Offset: 0x000031F4
		public void DoUSMindKillGasToxic(Thing Gas, Thing targ)
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
					hediff = ((hediffSet != null) ? hediffSet.GetFirstHediffOfDef(Globals.USStinkRootGas, false) : null);
				}
				float sensitivity = victim.GetStatValue(StatDefOf.ToxicSensitivity, true);
				float addsev = 0.25f * sensitivity * this.toxicRatio;
				if (!Globals.USVictimImmuneTo(victim, Globals.USStinkRootGas))
				{
					if (hediff != null && addsev > 0f)
					{
						hediff.Severity += addsev;
						if (hediff.Severity >= 2.5f)
						{
							this.DoUSToxicMental(victim, hediff);
							return;
						}
					}
					else
					{
						Hediff addhediff = HediffMaker.MakeHediff(Globals.USStinkRootGas, victim, null);
						addhediff.Severity = addsev;
						victim.health.AddHediff(addhediff, null, null, null);
					}
				}
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000050E0 File Offset: 0x000032E0
		private void DoUSToxicDamage(Thing targ)
		{
			Plant plant = targ as Plant;
			if (plant != null && plant.def.defName != "Plant_USStinkroot")
			{
				float Dmg = 5f * this.toxicRatio;
				if (Dmg < 1f)
				{
					Dmg = 1f;
				}
				plant.TakeDamage(new DamageInfo(Globals.USPlantToxin, Dmg, 0f, -1f, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x0000514C File Offset: 0x0000334C
		private void DoUSToxicMental(Pawn victim, Hediff hediff)
		{
			if (!victim.RaceProps.IsMechanoid && !(victim.RaceProps.FleshType.defName == "Insectoid") && hediff.def.maxSeverity > 0f && Rand.Range(0f + 0.2f * (hediff.Severity / hediff.def.maxSeverity), 1f) >= 0.95f && !victim.mindState.mentalStateHandler.InMentalState)
			{
				float whichBreak = Rand.Range(0f, 1f);
				if (whichBreak > 0.75f)
				{
					victim.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, false, false, null, false);
					return;
				}
				if (whichBreak >= 0.45f && whichBreak <= 0.75f)
				{
					victim.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Psychotic, null, false, false, null, false);
					return;
				}
				if (whichBreak < 0.45f)
				{
					victim.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.PanicFlee, null, false, false, null, false);
				}
			}
		}

		// Token: 0x04000048 RID: 72
		public float toxicRatio = Settings.USToxLevels / 100f;
	}
}
