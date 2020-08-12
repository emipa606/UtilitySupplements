using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USToxins
{
	// Token: 0x0200002C RID: 44
	public class DamageWorker_USWeedKill : DamageWorker
	{
		// Token: 0x0600009E RID: 158 RVA: 0x000062EC File Offset: 0x000044EC
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

		// Token: 0x0600009F RID: 159 RVA: 0x0000637C File Offset: 0x0000457C
		public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
		{
			DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
			if (victim.def.category == ThingCategory.Plant && victim.def.plant.purpose == PlantPurpose.Misc && !victim.def.plant.IsTree && victim.def.useHitPoints && dinfo.Def.harmsHealth)
			{
				float num = dinfo.Amount;
				num *= dinfo.Def.plantDamageFactor;
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
