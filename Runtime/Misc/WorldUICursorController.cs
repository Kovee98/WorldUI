using UnityEngine;
using UnityEngine.Events;

namespace WorldUI {
    public class WorldUICursorController : MonoBehaviour {
        [Header("General")]
        [SerializeField] CursorMode cursorMode = CursorMode.Auto;
        [SerializeField] bool changeCursor = true;
        [SerializeField] bool isEnabled = true;

        [Header("Hovering")]
        [SerializeField] Texture2D mouseEnterCursor;
        [SerializeField] Vector2 mouseEnterCursorHotspot;
        [SerializeField] Texture2D mouseExitCursor;
        [SerializeField] Vector2 mouseExitCursorHotspot;
        public bool isHovering = false;
        public bool isClicking = false;
        bool skipIsHoveringUpdate = false;

        [Header("Clicking")]
        [SerializeField] Texture2D mouseDownCursor;
        [SerializeField] Vector2 mouseDownCursorHotspot;
        [SerializeField] Texture2D mouseUpCursor;
        [SerializeField] Vector2 mouseUpCursorHotspot;

        [Header("Events")]
        [SerializeField] UnityEvent mouseEnterEvent;
        [SerializeField] UnityEvent mouseExitEvent;
        [SerializeField] UnityEvent mouseDownEvent;
        [SerializeField] UnityEvent mouseUpEvent;

        void OnMouseEnter () {
            if (!skipIsHoveringUpdate) isHovering = true;
            if (!isEnabled) return;

            if (changeCursor) {
                Cursor.SetCursor(mouseEnterCursor, mouseEnterCursorHotspot, cursorMode);
            }
            if (mouseEnterEvent != null) mouseEnterEvent.Invoke();
        }

        void OnMouseExit () {
            if (!skipIsHoveringUpdate) isHovering = false;
            if (!isEnabled) return;

            if (changeCursor) {
                if (isClicking) {
                    Cursor.SetCursor(mouseDownCursor, mouseDownCursorHotspot, cursorMode);
                } else {
                    Cursor.SetCursor(mouseExitCursor, mouseExitCursorHotspot, cursorMode);
                }
            }

            if (mouseExitEvent != null) mouseExitEvent.Invoke();
        }

        void OnMouseDown () {
            if (!isEnabled) return;

            isClicking = true;

            if (changeCursor) {
                Cursor.SetCursor(mouseDownCursor, mouseDownCursorHotspot, cursorMode);
            }

            if (mouseDownEvent != null) mouseDownEvent.Invoke();
        }

        void OnMouseUp () {
            if (!isEnabled) return;

            isClicking = false;

            if (changeCursor) {
                if (isHovering) {
                    Cursor.SetCursor(mouseEnterCursor, mouseEnterCursorHotspot, cursorMode);
                } else {
                    Cursor.SetCursor(mouseExitCursor, mouseExitCursorHotspot, cursorMode);
                }
            }

            if (mouseUpEvent != null) mouseUpEvent.Invoke();
        }

        public void SetIsEnabled (bool isEnabled) {
            bool wasEnabled = this.isEnabled;

            skipIsHoveringUpdate = true;

            // if disabling while hovering, trigger exit
            if (wasEnabled && !isEnabled && isHovering) OnMouseExit();

            this.isEnabled = isEnabled;

            // if enabling while hovering, trigger enter
            if (!wasEnabled && isEnabled && isHovering) OnMouseEnter();

            skipIsHoveringUpdate = false;
        }
    }
}
