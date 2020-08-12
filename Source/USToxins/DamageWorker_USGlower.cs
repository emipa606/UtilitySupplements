using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USToxins
{
	// Token: 0x02000009 RID: 9
	public class DamageWorker_USGlower : DamageWorker
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002718 File Offset: 0x00000918
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

		// Token: 0x06000015 RID: 21 RVA: 0x000027A8 File Offset: 0x000009A8
		public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
		{
			DamageWorker.DamageResult damageResult = new DamageWorker.DamageResult();
			if (victim.def.category == ThingCategory.Filth && victim.def.defName != "Filth_USGlowFoam" && victim.def.defName != "Filth_USGlowStick" && victim.def.useHitPoints && dinfo.Def.harmsHealth)
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
