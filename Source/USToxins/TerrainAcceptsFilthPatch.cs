using HarmonyLib;
using RimWorld;
using Verse;

namespace USToxins;

[HarmonyPatch(typeof(FilthMaker), "TerrainAcceptsFilth")]
public class TerrainAcceptsFilthPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, TerrainDef terrainDef, ThingDef filthDef,
        FilthSourceFlags additionalFlags = FilthSourceFlags.None)
    {
        if (!__result && terrainDef.AffectsFertility && Globals.IsUSFilth(filthDef))
        {
            __result = true;
        }
    }
}