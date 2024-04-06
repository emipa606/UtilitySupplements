using HarmonyLib;
using Verse;

namespace USToxins;

[HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
public class TryCastShot
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, ref Verb_Shoot __instance)
    {
        if (!__result)
        {
            return;
        }

        if (__instance is null or null)
        {
            return;
        }

        var ChkSprayer = __instance.EquipmentSource;
        if (ChkSprayer == null || !ChkSprayer.def.HasComp(typeof(CompUSToxUses)))
        {
            return;
        }

        var uses = ((USToxUsesData)ChkSprayer).USToxUses;
        uses--;
        if (uses <= 0)
        {
            ChkSprayer.Destroy();
            return;
        }

        (ChkSprayer as USToxUsesData).USToxUses = uses;
    }
}