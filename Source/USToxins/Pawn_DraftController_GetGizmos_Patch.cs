using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x02000013 RID: 19
    [HarmonyPatch(typeof(Pawn_DraftController), "GetGizmos")]
    public class Pawn_DraftController_GetGizmos_Patch
    {
        // Token: 0x06000036 RID: 54 RVA: 0x00003598 File Offset: 0x00001798
        public static void Postfix(ref IEnumerable<Gizmo> __result, ref Pawn_DraftController __instance)
        {
            if (__result == null)
            {
                return;
            }

            var list = __result.ToList();
            var pawn = __instance.pawn;
            if (pawn != null && pawn.equipment.Primary != null)
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
}