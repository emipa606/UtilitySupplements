using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USToxins
{
    // Token: 0x02000029 RID: 41
    public class DamageWorker_USFilthKill : DamageWorker
    {
        // Token: 0x06000095 RID: 149 RVA: 0x00005ED0 File Offset: 0x000040D0
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

        // Token: 0x06000096 RID: 150 RVA: 0x00005F60 File Offset: 0x00004160
        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            var damageResult = new DamageResult();
            if (victim.def.category != ThingCategory.Filth || victim.def.defName == "Filth_USFilthKillFoam" ||
                !victim.def.useHitPoints || !dinfo.Def.harmsHealth)
            {
                return damageResult;
            }

            var num = dinfo.Amount;
            num *= dinfo.Def.plantDamageFactor;
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