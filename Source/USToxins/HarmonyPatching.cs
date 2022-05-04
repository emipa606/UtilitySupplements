using System.Reflection;
using HarmonyLib;
using Verse;

namespace USToxins;

[StaticConstructorOnStartup]
internal static class HarmonyPatching
{
    static HarmonyPatching()
    {
        new Harmony("com.Pelador.RW.US.USToxins").PatchAll(Assembly.GetExecutingAssembly());
    }
}