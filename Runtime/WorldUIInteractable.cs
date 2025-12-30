using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

namespace WorldUI {
    public class WorldUIInteractable : MonoBehaviour {
        public WorldUIGlyphDatabase glyphDatabase;
        [SerializeField] private GameObject promptObj;
        private TMP_Text promptText;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] string inputActionName = "Player/Interact";
        private InputAction interactAction;
        [SerializeField] string prefixText;
        [SerializeField] string suffixText;
        private bool isHovering = false;

        public UnityEvent<GameObject> onInitEvent;
        public UnityEvent<GameObject> onStartHoverEvent;
        public UnityEvent<GameObject> onInteractionEvent;
        public UnityEvent<GameObject> onStopHoverEvent;

        protected virtual void Awake () {
            interactAction = playerInput.actions[inputActionName];
            if (interactAction == null) {
                Debug.LogError($"Input Action '{inputActionName}' not found in PlayerInput actions.");
            }

            promptText = promptObj?.GetComponentInChildren<TMP_Text>();
            promptObj?.SetActive(false);

            onInitEvent?.Invoke(gameObject.GetComponent<GameObject>());
        }

        public void Interact () {
            onInteractionEvent?.Invoke(gameObject.GetComponent<GameObject>());
        }

        public void StartHover () {
            if (isHovering) return;
            isHovering = true;

            onStartHoverEvent?.Invoke(gameObject.GetComponent<GameObject>());

            // set the hover text in the interaction popup
            promptText.spriteAsset = glyphDatabase.masterSpriteAsset;
            promptText.text = GetInteractionPrompt();

            promptObj?.SetActive(true);
        }

        public void StopHover () {
            if (!isHovering) return;
            isHovering = false;
            promptObj?.SetActive(false);

            onStopHoverEvent?.Invoke(gameObject.GetComponent<GameObject>());
        }

        private string GetInteractionPrompt () {
            if (interactAction == null) return "";
            string prompt;

            int bindingIndex = interactAction.GetBindingIndex(
                InputBinding.MaskByGroup(playerInput.currentControlScheme)
            );

            // Get the current binding path for the active control scheme
            string bindingPath = interactAction.GetBindingDisplayString(
                bindingIndex: bindingIndex,
                options: InputBinding.DisplayStringOptions.DontOmitDevice
            );

            // Fallback if no binding is found
            if (string.IsNullOrEmpty(bindingPath)) return "";

            // Use the path to find the corresponding glyph
            string glyphName = glyphDatabase.GetGlyphNameForPath(bindingPath);

            if (!string.IsNullOrEmpty(glyphName)) {
                prompt = $"{prefixText}<sprite name={glyphName}>{suffixText}";
            } else {
                // Fallback for paths without a specific glyph (e.g., keyboard letters)
                string displayString = InputControlPath.ToHumanReadableString(
                    bindingPath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice
                );
                prompt = $"{prefixText}{displayString}{suffixText}";
            }

            return prompt;
        }
    }
}