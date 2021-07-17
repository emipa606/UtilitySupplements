using HarmonyLib;
using Verse;

namespace USToxins
{
    // Token: 0x02000031 RID: 49
    [HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
    public class TryCastShot
    {
        // Token: 0x060000B1 RID: 177 RVA: 0x000069D8 File Offset: 0x00004BD8
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, ref Verb_Shoot __instance)
        {
            if (!__result)
            {
                return;
            }

            if (__instance == null || __instance == null)
            {
                return;
            }

            var ChkSprayer = __instance.EquipmentSource;
            if (ChkSprayer == null || !ChkSprayer.def.HasComp(typeof(CompUSToxUses)))
            {
                return;
            }

            var uses = ((USToxUsesData) ChkSprayer).USToxUses;
            uses--;
            if (uses <= 0)
            {
                ChkSprayer.Destroy();
                return;
            }

            (ChkSprayer as USToxUsesData).USToxUses = uses;
        }
    }
}