using HarmonyLib;
using RimWorld;
using Verse;

namespace USToxins;

[HarmonyPatch(typeof(Filth), "CanDropAt")]
public class Filth_CanDropAt
{
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