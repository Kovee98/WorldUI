using UnityEngine;

namespace WorldUI {
    public class WorldUICircleGizmo : WorldUIGizmo {
        [SerializeField] private float diameter = 1f;
        [SerializeField] private int segments = 100;

        public override void DrawGizmo () {
            Gizmos.color = gizmoColor;
            WorldUIUtils.DrawGizmoCircle(transform.position, Vector3.up, diameter / 2, segments);
        }
    }
}
