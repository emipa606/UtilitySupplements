using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace USToxins;

[HarmonyPatch(typeof(ReservationUtility), "CanReserve")]
public class CanReserve_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, Pawn p, LocalTargetInfo target, int maxPawns = 1,
        int stackCount = -1, ReservationLayerDef layer = null, bool ignoreOtherReservations = false)
    {
        if (!__result)
        {
            return;
        }

        var thing = target.Thing;
        if (thing is not Filth)
        {
            return;
        }

        var defname = thing.def.defName;
        if (defname is "Filth_USAntifreeze" or "Filth_USGlowFoam" or "Filth_USInsectKillFoam")
        {
            __result = false;
            return;
        }

        var map = thing.Map;
        if (map == null)
        {
            return;
        }

        var terDef = thing.Position.GetTerrain(map);
        if (terDef is { fertility: > 0f } && defname is "Filth_USBlightKillFoam" or "Filth_USPlantKillFoam"
                or "Filth_USTreeKillFoam" or "Filth_USWeedKillFoam" or "Filth_USAmmoniaFertFoam")
        {
            __result = false;
        }
    }
}