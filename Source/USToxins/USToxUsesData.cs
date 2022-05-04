using System.Collections.Generic;
using Verse;

namespace USToxins;

public class USToxUsesData : ThingWithComps
{
    public int USToxUses = 100;

    public CompUSToxUses ToxUses => GetComp<CompUSToxUses>();

    public override void PostMake()
    {
        base.PostMake();
        USToxUses = ToxUses.Props.USToxUses;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref USToxUses, "USToxUses", 100);
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        yield return new Gizmo_SprayerStatus
        {
            Sprayer = this
        };
        foreach (var item in base.GetGizmos())
        {
            yield return item;
        }
    }
}