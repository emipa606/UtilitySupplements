using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x02000024 RID: 36
    public class USTanglerootGas : Gas
    {
        // Token: 0x0400004B RID: 75
        public float toxicRatio = Settings.USToxLevels / 100f;

        // Token: 0x06000082 RID: 130 RVA: 0x000059D9 File Offset: 0x00003BD9
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, true);
            destroyTick = Find.TickManager.TicksGame + def.gas.expireSeconds.RandomInRange.SecondsToTicks();
        }

        // Token: 0x06000083 RID: 131 RVA: 0x00005A0E File Offset: 0x00003C0E
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref destroyTick, "destroyTick");
        }

        // Token: 0x06000084 RID: 132 RVA: 0x00005A28 File Offset: 0x00003C28
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

        // Token: 0x06000085 RID: 133 RVA: 0x00005B20 File Offset: 0x00003D20
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
            var addsev = Rand.Range(0.01f * sensitivity * toxicRatio, 0.05f * sensitivity * toxicRatio);
            if (Globals.USVictimImmuneTo(victim, Globals.USTangleRootStrike))
            {
                return;
            }

            if (hediff != null && addsev > 0f)
            {
                if (hediff.Severity + addsev < 1.8f)
                {
                    hediff.Severity += addsev;
                }
            }
            else
            {
                var addhediff = HediffMaker.MakeHediff(Globals.USTangleRootStrike, victim);
                addhediff.Severity = addsev;
                victim.health.AddHediff(addhediff);
            }
        }
    }
}