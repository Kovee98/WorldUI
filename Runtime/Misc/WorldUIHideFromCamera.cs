using UnityEngine;
using UnityEditor;

namespace WorldUI {
    enum FaceDirection { Behind, InFront, ToTheLeft, ToTheRight, Below, Above }

    [ExecuteAlways]
    public class WorldUIHideFromCamera : MonoBehaviour {
        public Renderer wallRenderer;
        private Transform camTransform;
        [SerializeField] private FaceDirection hideWhenCameraIs = FaceDirection.Behind;

        // This ensures the script runs even when just moving the camera
        void OnEnable () {
            #if UNITY_EDITOR
            EditorApplication.update += OnEditorUpdate;
            #endif
        }

        void OnDisable () {
            #if UNITY_EDITOR
            EditorApplication.update -= OnEditorUpdate;
            #endif
        }

        void OnEditorUpdate () {
            if (!Application.isPlaying) {
                HandleVisibility();
            }
        }

        void Update () {
            if (Application.isPlaying) {
                HandleVisibility();
            }
        }

        void HandleVisibility () {
            if (wallRenderer == null) wallRenderer = GetComponent<Renderer>();
            if (camTransform == null) camTransform = GetActiveCamera();
            if (camTransform == null || wallRenderer == null) return;

            Vector3 dirToCamera = camTransform.position - transform.position;

            // Determine the face normal based on the selected direction
            float dot = Vector3.Dot(transform.forward, dirToCamera);

            switch (hideWhenCameraIs) {
                case FaceDirection.InFront:
                    dot = Vector3.Dot(-transform.forward, dirToCamera);
                    break;
                case FaceDirection.ToTheRight:
                    dot = Vector3.Dot(-transform.right, dirToCamera);
                    break;
                case FaceDirection.ToTheLeft:
                    dot = Vector3.Dot(transform.right, dirToCamera);
                    break;
                case FaceDirection.Below:
                    dot = Vector3.Dot(transform.up, dirToCamera);
                    break;
                case FaceDirection.Above:
                    dot = Vector3.Dot(-transform.up, dirToCamera);
                    break;
                default: break;
            }

            // Update the renderer
            if (wallRenderer.enabled != (dot > 0)) {
                wallRenderer.enabled = dot > 0;
            }
        }

        Transform GetActiveCamera () {
            if (Application.isPlaying) return Camera.main?.transform;
            
            // This is the key for Editor movement
            if (SceneView.lastActiveSceneView != null) {
                return SceneView.lastActiveSceneView.camera.transform;
            }

            return null;
        }
    }
}
