using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x02000025 RID: 37
    public class USTearGas : Gas
    {
        // Token: 0x0400004C RID: 76
        public float toxicRatio = Settings.USToxLevels / 100f;

        // Token: 0x06000087 RID: 135 RVA: 0x00005C31 File Offset: 0x00003E31
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, true);
            destroyTick = Find.TickManager.TicksGame + def.gas.expireSeconds.RandomInRange.SecondsToTicks();
        }

        // Token: 0x06000088 RID: 136 RVA: 0x00005C66 File Offset: 0x00003E66
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref destroyTick, "destroyTick");
        }

        // Token: 0x06000089 RID: 137 RVA: 0x00005C80 File Offset: 0x00003E80
        public override void Tick()
        {
            if (destroyTick <= Find.TickManager.TicksGame)
            {
                Destroy();
            }

            graphicRotation += graphicRotationSpeed;
            if (Destroyed || Find.TickManager.TicksGame % 120 != 0)
            {
                return;
            }

            var TargetMap = Map;
            var TargetCell = Position;
            var Thinglist = TargetCell.GetThingList(TargetMap);
            if (Thinglist.Count <= 0)
            {
                return;
            }

            foreach (var thing in Thinglist)
            {
                if (thing is Pawn && thing.Position == TargetCell)
                {
                    DoUSTearGasToxic(this, thing);
                }
            }
        }

        // Token: 0x0600008A RID: 138 RVA: 0x00005D30 File Offset: 0x00003F30
        public void DoUSTearGasToxic(Thing Gas, Thing targ)
        {
            if (targ is not Pawn victim || !victim.health.capacities.CapableOf(PawnCapacityDefOf.Breathing))
            {
                return;
            }

            var health = victim.health;
            Hediff hediff;
            if (health == null)
            {
                hediff = null;
            }
            else
            {
                var hediffSet = health.hediffSet;
                hediff = hediffSet?.GetFirstHediffOfDef(Globals.USTearGas);
            }

            var sensitivity = victim.GetStatValue(StatDefOf.ToxicSensitivity);
            var addsev = 0.125f * sensitivity * toxicRatio;
            if (Globals.USVictimImmuneTo(victim, Globals.USTearGas))
            {
                return;
            }

            if (hediff != null && addsev > 0f)
            {
                hediff.Severity += addsev;
                return;
            }

            var addhediff = HediffMaker.MakeHediff(Globals.USTearGas, victim);
            addhediff.Severity = addsev;
            victim.health.AddHediff(addhediff);
        }
    }
}