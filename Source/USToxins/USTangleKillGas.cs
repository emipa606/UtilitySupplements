using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x02000023 RID: 35
    public class USTangleKillGas : Gas
    {
        // Token: 0x0400004A RID: 74
        public float toxicRatio = Settings.USToxLevels / 100f;

        // Token: 0x0600007D RID: 125 RVA: 0x00005790 File Offset: 0x00003990
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, true);
            destroyTick = Find.TickManager.TicksGame + def.gas.expireSeconds.RandomInRange.SecondsToTicks();
        }

        // Token: 0x0600007E RID: 126 RVA: 0x000057C5 File Offset: 0x000039C5
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref destroyTick, "destroyTick");
        }

        // Token: 0x0600007F RID: 127 RVA: 0x000057E0 File Offset: 0x000039E0
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
                if (thing is Pawn pawn && !pawn.RaceProps.IsMechanoid &&
                    pawn.RaceProps.FleshType.defName != "Insectoid" &&
                    pawn.Position == TargetCell)
                {
                    DoUSTangleKillGasToxic(this, pawn);
                }
            }
        }

        // Token: 0x06000080 RID: 128 RVA: 0x000058D8 File Offset: 0x00003AD8
        public void DoUSTangleKillGasToxic(Thing Gas, Thing targ)
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
                hediff = hediffSet?.GetFirstHediffOfDef(Globals.USTangleRootStrike);
            }

            var sensitivity = victim.GetStatValue(StatDefOf.ToxicSensitivity);
            var addsev = Rand.Range(0.0249999985f * sensitivity * toxicRatio, 0.125f * sensitivity * toxicRatio);
            if (Globals.USVictimImmuneTo(victim, Globals.USTangleRootStrike))
            {
                return;
            }

            if (hediff != null && addsev > 0f)
            {
                hediff.Severity += addsev;
                return;
            }

            var addhediff = HediffMaker.MakeHediff(Globals.USTangleRootStrike, victim);
            addhediff.Severity = addsev;
            victim.health.AddHediff(addhediff);
        }
    }
}