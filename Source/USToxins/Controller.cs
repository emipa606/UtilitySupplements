using UnityEngine;
using Verse;

namespace USToxins
{
    // Token: 0x02000004 RID: 4
    public class Controller : Mod
    {
        // Token: 0x04000001 RID: 1
        public static Settings Settings;

        // Token: 0x06000008 RID: 8 RVA: 0x000022B0 File Offset: 0x000004B0
        public Controller(ModContentPack content) : base(content)
        {
            Settings = GetSettings<Settings>();
        }

        // Token: 0x06000006 RID: 6 RVA: 0x00002292 File Offset: 0x00000492
        public override string SettingsCategory()
        {
            return "USTox.Name".Translate();
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000022A3 File Offset: 0x000004A3
        public override void DoSettingsWindowContents(Rect canvas)
        {
            Settings.DoWindowContents(canvas);
        }
    }
}