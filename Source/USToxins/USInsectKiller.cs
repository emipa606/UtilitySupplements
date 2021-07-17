using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x0200001F RID: 31
    public class USInsectKiller : Filth
    {
        // Token: 0x04000047 RID: 71
        public float toxRatio = Settings.USToxLevels / 100f;

        // Token: 0x04000046 RID: 70
        private int USspawnTick;

        // Token: 0x06000069 RID: 105 RVA: 0x00004C8F File Offset: 0x00002E8F
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref USspawnTick, "USspawnTick");
        }

        // Token: 0x0600006A RID: 106 RVA: 0x00004CAC File Offset: 0x00002EAC
        public override void Tick()
        {
            USspawnTick++;
            if (USspawnTick >= 60000)
            {
                Destroy();
                return;
            }

            if (Find.TickManager.TicksGame % 120 != 0)
            {
                return;
            }

            var TargetMap = Map;
            var TargetCell = Position;
            var Pawnlist = TargetCell.GetThingList(TargetMap);
            if (Pawnlist.Count <= 0)
            {
                return;
            }

            foreach (var thing in Pawnlist)
            {
                if (thing is not Pawn pawn || pawn.Position != TargetCell ||
                    pawn.RaceProps.FleshType.defName != "Insectoid")
                {
                    continue;
                }

                DoUSToxicDamage(pawn);
                ApplyUSToxicHediff(pawn);
            }
        }

        // Token: 0x0600006B RID: 107 RVA: 0x00004D8C File Offset: 0x00002F8C
        private void DoUSToxicDamage(Pawn I)
        {
            if (I == null)
            {
                return;
            }

            var Dmg = 3f * toxRatio;
            if (Dmg < 1f)
            {
                Dmg = 1f;
            }

            I.TakeDamage(new DamageInfo(DamageDefOf.Flame, Dmg, 0f, -1f, this));
        }

        // Token: 0x0600006C RID: 108 RVA: 0x00004DD8 File Offset: 0x00002FD8
        private void ApplyUSToxicHediff(Pawn I)
        {
            if (I == null)
            {
                return;
            }

            var health = I.health;
            Hediff hediff;
            if (health == null)
            {
                hediff = null;
            }
            else
            {
                var hediffSet = health.hediffSet;
                hediff = hediffSet?.GetFirstHediffOfDef(HediffDefOf.Heatstroke);
            }

            var addSev = 0.02f * toxRatio;
            if (addSev < 0.01f)
            {
                addSev = 0.01f;
            }

            if (hediff != null)
            {
                hediff.Severity += addSev;
                return;
            }

            var addhediff = HediffMaker.MakeHediff(HediffDefOf.Heatstroke, I);
            addhediff.Severity = addSev;
            I.health.AddHediff(addhediff);
        }
    }
}