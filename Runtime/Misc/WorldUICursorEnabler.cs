using UnityEngine;
using UnityEngine.InputSystem;

namespace WorldUI {
    public class EnableCursor : MonoBehaviour {
        void OnGUI () {
            _EnableCursor();
        }

        void _EnableCursor () {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
