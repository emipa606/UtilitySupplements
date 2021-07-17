﻿using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace USToxins
{
    // Token: 0x02000003 RID: 3
    [HarmonyPatch(typeof(ReservationUtility), "CanReserve")]
    public class CanReserve_Patch
    {
        // Token: 0x06000004 RID: 4 RVA: 0x000021B4 File Offset: 0x000003B4
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, Pawn p, LocalTargetInfo target, int maxPawns = 1,
            int stackCount = -1, ReservationLayerDef layer = null, bool ignoreOtherReservations = false)
        {
            if (!__result)
            {
                return;
            }

            var thing = target.Thing;
            if (thing == null || thing is not Filth)
            {
                return;
            }

            var defname = thing.def.defName;
            if (defname == "Filth_USAntifreeze" || defname == "Filth_USGlowFoam" ||
                defname == "Filth_USInsectKillFoam")
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
            if (terDef is {fertility: > 0f} && (defname == "Filth_USBlightKillFoam" ||
                                                defname == "Filth_USPlantKillFoam" ||
                                                defname == "Filth_USTreeKillFoam" ||
                                                defname == "Filth_USWeedKillFoam" ||
                                                defname == "Filth_USAmmoniaFertFoam"))
            {
                __result = false;
            }
        }
    }
}