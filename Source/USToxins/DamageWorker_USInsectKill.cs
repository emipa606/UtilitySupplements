using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USToxins
{
	// Token: 0x0200000A RID: 10
	public class DamageWorker_USInsectKill : DamageWorker
	{
		// Token: 0x06000017 RID: 23 RVA: 0x00002888 File Offset: 0x00000A88
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

		// Token: 0x06000018 RID: 24 RVA: 0x00002918 File Offset: 0x00000B18
		public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
		{
			DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
			if (victim.def.category == ThingCategory.Pawn && (victim as Pawn).RaceProps.FleshType.defName == "Insectoid" && victim.def.useHitPoints && dinfo.Def.harmsHealth)
			{
				float num = dinfo.Amount;
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
	}
}
