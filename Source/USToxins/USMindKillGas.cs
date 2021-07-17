using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x02000020 RID: 32
    public class USMindKillGas : Gas
    {
        // Token: 0x04000048 RID: 72
        public float toxicRatio = Settings.USToxLevels / 100f;

        // Token: 0x0600006E RID: 110 RVA: 0x00004E88 File Offset: 0x00003088
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, true);
            destroyTick = Find.TickManager.TicksGame + def.gas.expireSeconds.RandomInRange.SecondsToTicks();
        }

        // Token: 0x0600006F RID: 111 RVA: 0x00004EBD File Offset: 0x000030BD
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref destroyTick, "destroyTick");
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00004ED8 File Offset: 0x000030D8
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
                    DoUSMindKillGasToxic(this, thing);
                }

                if (thing is Plant plant && plant.def.plant.purpose != PlantPurpose.Health &&
                    plant.def.defName != "Plant_USStinkroot")
                {
                    DoUSToxicDamage(plant);
                }
            }
        }

        // Token: 0x06000071 RID: 113 RVA: 0x00004FF4 File Offset: 0x000031F4
        public void DoUSMindKillGasToxic(Thing Gas, Thing targ)
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
                hediff = hediffSet?.GetFirstHediffOfDef(Globals.USStinkRootGas);
            }

            var sensitivity = victim.GetStatValue(StatDefOf.ToxicSensitivity);
            var addsev = 0.25f * sensitivity * toxicRatio;
            if (Globals.USVictimImmuneTo(victim, Globals.USStinkRootGas))
            {
                return;
            }

            if (hediff != null && addsev > 0f)
            {
                hediff.Severity += addsev;
                if (hediff.Severity >= 2.5f)
                {
                    DoUSToxicMental(victim, hediff);
                }
            }
            else
            {
                var addhediff = HediffMaker.MakeHediff(Globals.USStinkRootGas, victim);
                addhediff.Severity = addsev;
                victim.health.AddHediff(addhediff);
            }
        }

        // Token: 0x06000072 RID: 114 RVA: 0x000050E0 File Offset: 0x000032E0
        private void DoUSToxicDamage(Thing targ)
        {
            if (targ is not Plant plant || plant.def.defName == "Plant_USStinkroot")
            {
                return;
            }

            var Dmg = 5f * toxicRatio;
            if (Dmg < 1f)
            {
                Dmg = 1f;
            }

            plant.TakeDamage(new DamageInfo(Globals.USPlantToxin, Dmg, 0f, -1f, this));
        }

        // Token: 0x06000073 RID: 115 RVA: 0x0000514C File Offset: 0x0000334C
        private void DoUSToxicMental(Pawn victim, Hediff hediff)
        {
            if (victim.RaceProps.IsMechanoid || victim.RaceProps.FleshType.defName == "Insectoid" ||
                !(hediff.def.maxSeverity > 0f) ||
                !(Rand.Range(0f + (0.2f * (hediff.Severity / hediff.def.maxSeverity)), 1f) >= 0.95f) ||
                victim.mindState.mentalStateHandler.InMentalState)
            {
                return;
            }

            var whichBreak = Rand.Range(0f, 1f);
            if (whichBreak > 0.75f)
            {
                victim.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);
                return;
            }

            if (whichBreak is >= 0.45f and <= 0.75f)
            {
                victim.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Psychotic);
                return;
            }

            if (whichBreak < 0.45f)
            {
                victim.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.PanicFlee);
            }
        }
    }
}