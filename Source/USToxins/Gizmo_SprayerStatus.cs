using UnityEngine;
using Verse;

namespace USToxins;

[StaticConstructorOnStartup]
internal class Gizmo_SprayerStatus : Gizmo
{
    private const float ArrowScale = 0.5f;

    private static readonly Texture2D FullBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.2f));

    private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

    private static readonly Texture2D TargetLevelArrow =
        ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarkerRotated");

    public ThingWithComps Sprayer;

    public Gizmo_SprayerStatus()
    {
        base.Order = -303f;
    }

    public override float GetWidth(float maxWidth)
    {
        return 140f;
    }

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
        var fillPercent = ((USToxUsesData)Sprayer).USToxUses /
                          (float)((USToxUsesData)Sprayer).ToxUses.Props.USToxUses;
        Widgets.FillableBar(rect3, fillPercent, FullBarTex, EmptyBarTex, false);
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect3,
            $"{((USToxUsesData)Sprayer)?.USToxUses:F0} / {(Sprayer as USToxUsesData)?.ToxUses.Props.USToxUses:F0}");
        Text.Anchor = TextAnchor.UpperLeft;
        return new GizmoResult(GizmoState.Clear);
    }
}