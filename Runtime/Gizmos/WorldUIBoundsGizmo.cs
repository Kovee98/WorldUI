using UnityEngine;

namespace WorldUI {
    public class WorldUIBoundsGizmo : WorldUIGizmo {
        [SerializeField] private WorldUIRendererFilter rendererFilter = WorldUIConstants.DEFAULT_RENDERER_FILTER;

        public override void DrawGizmo () {
            Bounds bounds = WorldUIUtils.CalculateLocalBounds(transform, rendererFilter);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
