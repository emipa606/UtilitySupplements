using UnityEngine;
using Verse;

namespace USToxins
{
    // Token: 0x0200000E RID: 14
    [StaticConstructorOnStartup]
    internal class Gizmo_SprayerStatus : Gizmo
    {
        // Token: 0x04000008 RID: 8
        private const float ArrowScale = 0.5f;

        // Token: 0x04000005 RID: 5
        private static readonly Texture2D FullBarTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.2f));

        // Token: 0x04000006 RID: 6
        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

        // Token: 0x04000007 RID: 7
        private static readonly Texture2D TargetLevelArrow =
            ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarkerRotated");

        // Token: 0x04000004 RID: 4
        public ThingWithComps Sprayer;

        // Token: 0x06000022 RID: 34 RVA: 0x00002D22 File Offset: 0x00000F22
        public Gizmo_SprayerStatus()
        {
            order = -303f;
        }

        // Token: 0x06000023 RID: 35 RVA: 0x00002D35 File Offset: 0x00000F35
        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00002D3C File Offset: 0x00000F3C
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            var overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Widgets.DrawWindowBackground(overRect);
            Rect rect2;
            var rect4 = rect2 = overRect.ContractedBy(6f);
            rect2.height = overRect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect2, "Sprayer uses left");
            var rect3 = rect4;
            rect3.yMin = rect2.yMin + (overRect.height / 2f) - 6f;
            rect3.height = (overRect.height / 2f) - 6f;
            var fillPercent = ((USToxUsesData) Sprayer).USToxUses /
                              (float) ((USToxUsesData) Sprayer).ToxUses.Props.USToxUses;
            Widgets.FillableBar(rect3, fillPercent, FullBarTex, EmptyBarTex, false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect3,
                ((USToxUsesData) Sprayer)?.USToxUses.ToString("F0") + " / " +
                (Sprayer as USToxUsesData)?.ToxUses.Props.USToxUses.ToString("F0"));
            Text.Anchor = TextAnchor.UpperLeft;
            return new GizmoResult(GizmoState.Clear);
        }
    }
}