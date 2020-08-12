using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USToxins
{
	// Token: 0x02000018 RID: 24
	public class USAcidGas : Gas
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00003FDB File Offset: 0x000021DB
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, true);
			this.destroyTick = Find.TickManager.TicksGame + this.def.gas.expireSeconds.RandomInRange.SecondsToTicks();
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004010 File Offset: 0x00002210
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.destroyTick, "destroyTick", 0, false);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000402C File Offset: 0x0000222C
		public override void Tick()
		{
			if (this.destroyTick <= Find.TickManager.TicksGame || this.amountAcidDam <= 0)
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
						if (!(Thinglist[i] is Gas) && Thinglist[i].Position == TargetCell)
						{
							if ((Thinglist[i].def.useHitPoints && !(Thinglist[i] is Pawn)) || (Thinglist[i].def.useHitPoints && Thinglist[i].def.IsCorpse))
							{
								int DMG;
								this.DoUSAcidGasDegrade(this, Thinglist[i], out DMG);
								this.amountAcidDam -= DMG;
							}
							else if (Thinglist[i] is Pawn && !Thinglist[i].def.IsCorpse)
							{
								this.DoUSAcidGasPawn(this, Thinglist[i]);
								if (USAcidGas.PawnIsAlive(Thinglist[i] as Pawn) && !(Thinglist[i] as Pawn).RaceProps.IsMechanoid && (Thinglist[i] as Pawn).health.capacities.CapableOf(PawnCapacityDefOf.Breathing))
								{
									this.DoUSAcidGasToxic(this, Thinglist[i]);
								}
							}
						}
					}
				}
				if (Settings.doAdWalls)
				{
					List<IntVec3> cells = GenAdjFast.AdjacentCellsCardinal(TargetCell);
					if (cells.Count > 0)
					{
						foreach (IntVec3 cell in cells)
						{
							if (cell.InBounds(TargetMap))
							{
								Building edifice = cell.GetEdifice(TargetMap);
								if (edifice != null)
								{
									int DMG2;
									this.DoUSAcidGasDegrade(this, edifice, out DMG2);
									this.amountAcidDam -= DMG2;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000426C File Offset: 0x0000246C
		public static bool PawnIsAlive(Pawn pawn)
		{
			return pawn != null && pawn.Spawned && !pawn.Dead && !pawn.def.IsCorpse;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00004294 File Offset: 0x00002494
		public void DoUSAcidGasDegrade(Thing Gas, Thing targ, out int DMG)
		{
			float baseDMG = Math.Max(1f, Math.Min(10f, this.GetBaseDMG(targ)));
			DMG = (int)baseDMG;
			targ.HitPoints -= DMG;
			if (targ.HitPoints <= 0 && !targ.Destroyed)
			{
				targ.Destroy(DestroyMode.Vanish);
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000042E8 File Offset: 0x000024E8
		public float GetBaseDMG(Thing targ)
		{
			if (targ is Building || targ.def.IsMetal || targ.def.IsShell)
			{
				return 6f;
			}
			if (targ.def.IsWeapon)
			{
				return 5f;
			}
			if (targ is Apparel || targ.def.IsLeather)
			{
				return 3f;
			}
			if (targ.def.IsDrug || targ.def.IsMedicine)
			{
				return 4f;
			}
			if (targ is Plant)
			{
				return 5f;
			}
			if (targ.def.IsIngestible || targ.def.IsMeat)
			{
				return 3f;
			}
			if (targ.def.IsFilth)
			{
				return 4f;
			}
			return 3f;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000043B1 File Offset: 0x000025B1
		public void DoUSAcidGasPawn(Thing Gas, Thing targ)
		{
			if (USAcidGas.PawnIsAlive(targ as Pawn))
			{
				USAcidGas.DoAcidDamPawn(targ as Pawn);
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000043CC File Offset: 0x000025CC
		public static void DoAcidDamPawn(Pawn pawn)
		{
			if (pawn != null && pawn != null && ((pawn != null) ? pawn.Map : null) != null)
			{
				BodyPartRecord candidate;
				DamageDef DamDef;
				float dmg;
				USAcidGas.SetUpBDVars(pawn, pawn, out candidate, out DamDef, out dmg);
				if (candidate != null)
				{
					DamageInfo dinfo = new DamageInfo(DamDef, dmg, 0f, -1f, pawn, candidate, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
					pawn.TakeDamage(dinfo);
					USAcidGas.DoAcidMiniEffectMech(pawn);
				}
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004424 File Offset: 0x00002624
		public static void DoAcidMiniEffectMech(Pawn pawn)
		{
			if (USAcidGas.PawnIsAlive(pawn) && pawn.RaceProps.IsMechanoid)
			{
				MoteMaker.ThrowSmoke(pawn.Position.ToVector3(), pawn.Map, 1f);
				MoteMaker.ThrowMicroSparks(pawn.Position.ToVector3(), pawn.Map);
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x0000447D File Offset: 0x0000267D
		public static int Rnd100()
		{
			return UnityEngine.Random.Range(1, 100);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004488 File Offset: 0x00002688
		public static void SetUpBDVars(Pawn Victim, Thing instigator, out BodyPartRecord candidate, out DamageDef DamDef, out float dmg)
		{
			DamDef = USAcidGas.USAcid;
			dmg = Mathf.Lerp(2f, 5f, (float)USAcidGas.Rnd100() / 100f);
			if (Victim != null)
			{
				float bodyV = Victim.BodySize;
				if (bodyV > 0f)
				{
					dmg = Math.Max(1f, dmg * bodyV);
				}
				if (!Victim.RaceProps.IsMechanoid)
				{
					if (Victim.RaceProps.FleshType.defName == "Insectoid")
					{
						dmg = Math.Max(1f, dmg / 2.5f);
					}
					else if (Victim.RaceProps.Animal)
					{
						dmg = Math.Max(1f, dmg / 1.5f);
					}
					else
					{
						dmg = Math.Max(1f, dmg / 1.75f);
					}
				}
				else
				{
					dmg *= 1.25f;
				}
			}
			float toxRatio = Settings.USToxLevels / 100f;
			float sensitivity = 1f;
			if (Victim != null && !Victim.RaceProps.IsMechanoid)
			{
				sensitivity = Victim.GetStatValue(StatDefOf.ToxicSensitivity, true);
			}
			dmg *= toxRatio * sensitivity;
			candidate = null;
			List<BodyPartRecord> potentials = new List<BodyPartRecord>();
			RaceProperties raceProps = Victim.RaceProps;
			bool flag;
			if (raceProps == null)
			{
				flag = (null != null);
			}
			else
			{
				BodyDef body = raceProps.body;
				flag = (((body != null) ? body.AllParts : null) != null);
			}
			if (flag)
			{
				potentials.AddRange(Victim.RaceProps.body.AllParts);
			}
			if (potentials.Count > 0)
			{
				candidate = USAcidGas.GetCandidate(potentials, Victim);
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000045F8 File Offset: 0x000027F8
		public static BodyPartRecord GetCandidate(List<BodyPartRecord> potentials, Pawn Victim)
		{
			BodyPartRecord candidate = null;
			List<BodyPartRecord> candidates = new List<BodyPartRecord>();
			foreach (BodyPartRecord BPR in potentials)
			{
				if (!BPR.IsCorePart)
				{
					if (Victim.RaceProps.IsMechanoid)
					{
						if (!BPR.def.defName.ToString().EndsWith("CentipedeBodyFirstRing"))
						{
							candidates.AddDistinct(BPR);
						}
					}
					else if (BPR.depth != BodyPartDepth.Inside)
					{
						candidates.AddDistinct(BPR);
					}
				}
			}
			if (candidates.Count > 0)
			{
				candidate = candidates.RandomElement<BodyPartRecord>();
				if (candidate.IsCorePart)
				{
					candidate = null;
				}
			}
			return candidate;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000046AC File Offset: 0x000028AC
		public void DoUSAcidGasToxic(Thing Gas, Thing targ)
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
					hediff = ((hediffSet != null) ? hediffSet.GetFirstHediffOfDef(Globals.USAcidGasEffect, false) : null);
				}
				float sensitivity = victim.GetStatValue(StatDefOf.ToxicSensitivity, true);
				float addsev = Rand.Range(0.01f * sensitivity * this.toxicRatio, 0.1f * sensitivity * this.toxicRatio);
				if (!Globals.USVictimImmuneTo(victim, Globals.USAcidGasEffect))
				{
					if (hediff != null && addsev > 0f)
					{
						hediff.Severity += addsev;
						return;
					}
					Hediff addhediff = HediffMaker.MakeHediff(Globals.USAcidGasEffect, victim, null);
					addhediff.Severity = addsev;
					victim.health.AddHediff(addhediff, null, null, null);
				}
			}
		}

		// Token: 0x0400003B RID: 59
		public int amountAcidDam = 1000;

		// Token: 0x0400003C RID: 60
		public static DamageDef USAcid = DefDatabase<DamageDef>.GetNamed("Dam_USAcidGas", true);

		// Token: 0x0400003D RID: 61
		public float toxicRatio = Settings.USToxLevels / 100f;
	}
}
