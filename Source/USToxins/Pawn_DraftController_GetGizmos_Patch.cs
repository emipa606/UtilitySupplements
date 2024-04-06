using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace USToxins;

[HarmonyPatch(typeof(Pawn_DraftController), "GetGizmos")]
public class Pawn_DraftController_GetGizmos_Patch
{
    public static void Postfix(ref IEnumerable<Gizmo> __result, ref Pawn_DraftController __instance)
    {
        if (__result == null)
        {
            return;
        }

        var list = __result.ToList();
        var pawn = __instance.pawn;
        if (pawn?.equipment.Primary != null)
        {
            var ChkSprayer = pawn.equipment.Primary;
            if (ChkSprayer.def.HasComp(typeof(CompUSToxUses)))
            {
                Gizmo item = new Gizmo_SprayerStatus
                {
                    Sprayer = ChkSprayer
                };
                list.Add(item);
            }
        }

        __result = list;
    }
}