using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USToxins
{
	// Token: 0x0200002B RID: 43
	public class DamageWorker_USTreeKill : DamageWorker
	{
		// Token: 0x0600009B RID: 155 RVA: 0x0000619C File Offset: 0x0000439C
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

		// Token: 0x0600009C RID: 156 RVA: 0x0000622C File Offset: 0x0000442C
		public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
		{
			DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
			if (victim.def.category == ThingCategory.Plant && victim.def.plant.IsTree && victim.def.useHitPoints && dinfo.Def.harmsHealth)
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
