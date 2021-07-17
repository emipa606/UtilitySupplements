using HarmonyLib;
using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x0200000D RID: 13
    [HarmonyPatch(typeof(Filth), "CanDropAt")]
    public class Filth_CanDropAt
    {
        // Token: 0x06000020 RID: 32 RVA: 0x00002CD8 File Offset: 0x00000ED8
        [HarmonyPostfix]
        public static void Postfix(ref Filth __instance, ref bool __result, ref IntVec3 c, ref Map map)
        {
            if (__result)
            {
                return;
            }

            BuildableDef buildableDef = map.terrainGrid.TerrainAt(c);
            var chkfilth = __instance;
            if (buildableDef.fertility > 0f && Globals.IsUSFilth(chkfilth.def))
            {
                __result = true;
            }
        }
    }
}