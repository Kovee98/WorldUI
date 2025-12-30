using UnityEngine;

namespace WorldUI {
    public abstract class WorldUIGizmo : MonoBehaviour {
        [SerializeField] protected bool requireSelection = false;
        [SerializeField] protected Color gizmoColor = WorldUIConstants.DEFAULT_GIZMO_COLOR;

        void OnDrawGizmosSelected () {
            if (requireSelection) DrawGizmo();
        }

        void OnDrawGizmos () {
            if (!requireSelection) DrawGizmo();
        }

        public abstract void DrawGizmo ();
    }
}
