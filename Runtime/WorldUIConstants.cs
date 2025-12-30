using UnityEngine;

namespace WorldUI {
    public static class WorldUIConstants {
        public static readonly Color DEFAULT_GIZMO_COLOR = new Color(0f, 0.5224285f, 1f, 1f);
        public static readonly WorldUILayoutType DEFAULT_LAYOUT_TYPE = WorldUILayoutType.Simple;

        // everything except disabled, trail renderers, and particle system renderers
        public static readonly WorldUIRendererFilter DEFAULT_RENDERER_FILTER =
            WorldUIRendererFilter.BillboardRenderer |
            WorldUIRendererFilter.LineRenderer |
            WorldUIRendererFilter.MeshRenderer |
            WorldUIRendererFilter.SkinnedMeshRenderer |
            WorldUIRendererFilter.SpriteMask |
            WorldUIRendererFilter.SpriteRenderer |
            WorldUIRendererFilter.SpriteShapeRenderer
        ;

        // everything except disabled, trail renderers, and particle system renderers
        public static readonly WorldUIUpdaterMode DEFAULT_UPDATER_MODE = WorldUIUpdaterMode.All;
    }
}
