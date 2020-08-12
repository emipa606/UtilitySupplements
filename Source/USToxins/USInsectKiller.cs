using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x0200001F RID: 31
	public class USInsectKiller : Filth
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00004C8F File Offset: 0x00002E8F
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.USspawnTick, "USspawnTick", 0, false);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004CAC File Offset: 0x00002EAC
		public override void Tick()
		{
			this.USspawnTick++;
			if (this.USspawnTick >= 60000)
			{
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			if (Find.TickManager.TicksGame % 120 == 0)
			{
				Map TargetMap = base.Map;
				IntVec3 TargetCell = base.Position;
				List<Thing> Pawnlist = TargetCell.GetThingList(TargetMap);
				if (Pawnlist.Count > 0)
				{
					for (int i = 0; i < Pawnlist.Count; i++)
					{
						if (Pawnlist[i] is Pawn && Pawnlist[i].Position == TargetCell && (Pawnlist[i] as Pawn).RaceProps.FleshType.defName == "Insectoid")
						{
							this.DoUSToxicDamage(Pawnlist[i] as Pawn);
							this.ApplyUSToxicHediff(Pawnlist[i] as Pawn);
						}
					}
				}
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004D8C File Offset: 0x00002F8C
		private void DoUSToxicDamage(Pawn I)
		{
			if (I != null)
			{
				float Dmg = 3f * this.toxRatio;
				if (Dmg < 1f)
				{
					Dmg = 1f;
				}
				I.TakeDamage(new DamageInfo(DamageDefOf.Flame, Dmg, 0f, -1f, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004DD8 File Offset: 0x00002FD8
		private void ApplyUSToxicHediff(Pawn I)
		{
			if (I != null)
			{
				Pawn_HealthTracker health = I.health;
				Hediff hediff;
				if (health == null)
				{
					hediff = null;
				}
				else
				{
					HediffSet hediffSet = health.hediffSet;
					hediff = ((hediffSet != null) ? hediffSet.GetFirstHediffOfDef(HediffDefOf.Heatstroke, false) : null);
				}
				float addSev = 0.02f * this.toxRatio;
				if (addSev < 0.01f)
				{
					addSev = 0.01f;
				}
				if (hediff != null)
				{
					hediff.Severity += addSev;
					return;
				}
				Hediff addhediff = HediffMaker.MakeHediff(HediffDefOf.Heatstroke, I, null);
				addhediff.Severity = addSev;
				I.health.AddHediff(addhediff, null, null, null);
			}
		}

		// Token: 0x04000046 RID: 70
		private int USspawnTick;

		// Token: 0x04000047 RID: 71
		public float toxRatio = Settings.USToxLevels / 100f;
	}
}
