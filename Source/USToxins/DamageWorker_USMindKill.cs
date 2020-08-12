using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USToxins
{
	// Token: 0x0200000B RID: 11
	public class DamageWorker_USMindKill : DamageWorker
	{
		// Token: 0x0600001A RID: 26 RVA: 0x000029D8 File Offset: 0x00000BD8
		public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
		{
			if (this.def.explosionHeatEnergyPerCell > 1.401298E-45f)
			{
				GenTemperature.PushHeat(explosion.Position, explosion.Map, this.def.explosionHeatEnergyPerCell * (float)cellsToAffect.Count);
			}
			MoteMaker.MakeStaticMote(explosion.Position, explosion.Map, ThingDefOf.Mote_ExplosionFlash, explosion.radius * 6f);
			MoteMaker.MakeStaticMote(explosion.Position, explosion.Map, ThingDefOf.Mote_ExplosionFlash, explosion.radius * 6f);
			this.ExplosionVisualEffectCenter(explosion);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002A68 File Offset: 0x00000C68
		public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
		{
			DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
			if (victim.def.category == ThingCategory.Pawn && (victim as Pawn).health.capacities.CapableOf(PawnCapacityDefOf.Breathing) && victim.def.useHitPoints && dinfo.Def.harmsHealth)
			{
				float num = dinfo.Amount;
				if (num > 0f)
				{
					float sensitivity = victim.GetStatValue(StatDefOf.ToxicSensitivity, true);
					num = num * sensitivity * this.toxicRatio;
				}
				damageResult.totalDamageDealt = (float)Mathf.Min(victim.HitPoints, GenMath.RoundRandom(num));
				victim.HitPoints -= Mathf.RoundToInt(damageResult.totalDamageDealt);
				if (victim.HitPoints <= 0)
				{
					victim.HitPoints = 0;
					victim.Kill(new DamageInfo?(dinfo), null);
				}
			}
			return damageResult;
		}

		// Token: 0x04000002 RID: 2
		public float toxicRatio = Settings.USToxLevels / 100f;
	}
}
