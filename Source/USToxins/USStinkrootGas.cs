using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x02000022 RID: 34
    public class USStinkrootGas : Gas
    {
        // Token: 0x04000049 RID: 73
        public float toxicRatio = Settings.USToxLevels / 100f;

        // Token: 0x06000078 RID: 120 RVA: 0x000053E8 File Offset: 0x000035E8
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
                    DoUSStinkRootGasToxic(this, thing);
                }

                if (thing is Plant plant && plant.def.plant.purpose != PlantPurpose.Health &&
                    plant.def.defName != "Plant_USStinkroot")
                {
                    DoUSToxicDamage(plant);
                }
            }
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00005504 File Offset: 0x00003704
        public void DoUSStinkRootGasToxic(Thing Gas, Thing targ)
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
            var addsev = 0.05f * sensitivity * toxicRatio;
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

        // Token: 0x0600007A RID: 122 RVA: 0x000055F0 File Offset: 0x000037F0
        private void DoUSToxicDamage(Thing targ)
        {
            if (targ is not Plant plant || plant.def.defName == "Plant_USStinkroot")
            {
                return;
            }

            var Dmg = 2f * toxicRatio;
            if (Dmg < 1f)
            {
                Dmg = 1f;
            }

            plant.TakeDamage(new DamageInfo(Globals.USPlantToxin, Dmg, 0f, -1f, this));
        }

        // Token: 0x0600007B RID: 123 RVA: 0x0000565C File Offset: 0x0000385C
        public void DoUSToxicMental(Pawn victim, Hediff hediff)
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