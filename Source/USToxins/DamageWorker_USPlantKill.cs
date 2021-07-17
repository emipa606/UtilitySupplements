using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USToxins
{
    // Token: 0x0200002A RID: 42
    public class DamageWorker_USPlantKill : DamageWorker
    {
        // Token: 0x06000098 RID: 152 RVA: 0x00006024 File Offset: 0x00004224
        public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
        {
            if (def.explosionHeatEnergyPerCell > 1.401298E-45f)
            {
                GenTemperature.PushHeat(explosion.Position, explosion.Map,
                    def.explosionHeatEnergyPerCell * cellsToAffect.Count);
            }

            FleckMaker.Static(explosion.Position, explosion.Map, FleckDefOf.ExplosionFlash, explosion.radius * 6f);
            FleckMaker.Static(explosion.Position, explosion.Map, FleckDefOf.ExplosionFlash, explosion.radius * 6f);
            ExplosionVisualEffectCenter(explosion);
        }

        // Token: 0x06000099 RID: 153 RVA: 0x000060B4 File Offset: 0x000042B4
        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            var damageResult = new DamageResult();
            if (victim.def.category != ThingCategory.Plant || victim.def.plant.purpose == PlantPurpose.Health ||
                victim.def.plant.IsTree || !victim.def.useHitPoints || !dinfo.Def.harmsHealth)
            {
                return damageResult;
            }

            var num = dinfo.Amount;
            if (victim.def.category == ThingCategory.Plant)
            {
                num *= dinfo.Def.plantDamageFactor;
            }

            damageResult.totalDamageDealt = Mathf.Min(victim.HitPoints, GenMath.RoundRandom(num));
            victim.HitPoints -= Mathf.RoundToInt(damageResult.totalDamageDealt);
            if (victim.HitPoints > 0)
            {
                return damageResult;
            }

            victim.HitPoints = 0;
            victim.Kill(dinfo);

            return damageResult;
        }
    }
}