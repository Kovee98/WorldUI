using UnityEngine;

namespace WorldUI {
    [ExecuteAlways]
    public abstract class WorldUILayout : MonoBehaviour {
        [SerializeField] protected bool updateContinuously = false;
        [SerializeField] protected WorldUIRendererFilter rendererFilter = WorldUIConstants.DEFAULT_RENDERER_FILTER;
        [SerializeField] protected WorldUIRendererFilter childRendererfilter = WorldUIConstants.DEFAULT_RENDERER_FILTER;
        private WorldUIUpdater updater;
        private Camera cam;

        public abstract void UpdateUI (bool continuousUpdate = false);

        protected Camera GetCamera () {
            if (cam == null) cam = Camera.main;
            return cam;
        }

        public WorldUIUpdater GetWorldUIUpdater () {
            if (updater == null) updater = FindAnyObjectByType<WorldUIUpdater>();
            return updater;
        }
    }
}
