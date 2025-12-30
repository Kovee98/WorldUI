using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace WorldUI {
    public class WorldUIInput : WorldUIButton {
        [Header("Input Settings")]
        [SerializeField] private TMP_Text textMeshPro;
        [SerializeField] private GameObject highlightObject;
        private bool isTextHighlighted = false;
        [SerializeField] private GameObject cursorObject;
        bool isFocused = false;
        string value = "";
        [SerializeField] int maxCharacters = 100;

        [SerializeField] private int startHighlightIndex = 0;
        [SerializeField] private int endHighlightIndex = -1;

        public InputActionReference moveAction;
        public InputActionReference backspaceAction;
        public InputActionReference deleteAction;

        [Header("Repeat Settings")]
        [Tooltip("Time before the cursor starts auto-repeating while held.")]
        public float repeatDelay = 0.25f;
        [Tooltip("Interval between movements once repeating.")]
        public float repeatRate = 0.05f;

        [Header("Buffering")]
        public float bufferWindow = 0.15f;

        private float nextRepeatTime;
        private Vector2 rawInput;
        private bool isRepeating;

        // Information about what we're currently repeating
        private System.Action currentAction;
        private InputAction.CallbackContext currentContext;

        [SerializeField] private float cursorBlinkDelay = 0f;
        [SerializeField] private float cursorBlinkRate = 0.5f;
        [SerializeField] private int cursorCurrentIndex = -1;

        private void OnEnable () {
            SetupAction(moveAction, HandleCursorMove);
            SetupAction(backspaceAction, HandleBackspace);
            SetupAction(deleteAction, HandleDelete);
        }

        private void OnDisable () {
            TeardownAction(moveAction, HandleCursorMove);
            TeardownAction(backspaceAction, HandleBackspace);
            TeardownAction(deleteAction, HandleDelete);

            if (Keyboard.current != null) Keyboard.current.onTextInput -= HandleTextInput;
        }

        private void SetupAction (InputActionReference reference, System.Action method) {
            if (reference == null) return;

            reference.action.performed += ctx => StartRepeat(method, ctx);
            reference.action.canceled += ctx => StopRepeat();

            reference.action.Enable();
        }

        private void TeardownAction (InputActionReference reference, System.Action method) {
            if (reference == null) return;

            reference.action.performed -= ctx => StartRepeat(method, ctx);
            reference.action.canceled -= ctx => StopRepeat();
        }

        private void StartRepeat (System.Action action, InputAction.CallbackContext context) {
            cursorObject.SetActive(true);
            currentContext = context;
            currentAction = action;
            currentAction.Invoke(); // Initial "tap"
            nextRepeatTime = Time.time + repeatDelay;
            isRepeating = true;
        }

        private void StopRepeat () {
            isRepeating = false;
            currentAction = null;
            currentContext = default;
        }

        private void HandleBackspace () {
            if (value.Length == 0) return;

            UnHighlightText();

            if (cursorCurrentIndex > -1 && cursorCurrentIndex <= value.Length - 1) {
                int removeIndex = Mathf.Max(0, cursorCurrentIndex);
                SetValue(value.Remove(removeIndex, 1));
                HandleMoveCursorLeft();
            }
        }

        private void HandleDelete () {
            if (value.Length == 0) return;

            if (cursorCurrentIndex >= -1 && cursorCurrentIndex < value.Length - 1) {
                int removeIndex = Mathf.Max(0, cursorCurrentIndex + 1);
                SetValue(value.Remove(removeIndex, 1));
                UpdateCursorPosition();
            }
        }

        private void HandleCursorMove () {
            if (currentContext.valueType == typeof(Vector2)) {
                rawInput = currentContext.ReadValue<Vector2>();
            } else {
                rawInput = new Vector2(currentContext.ReadValue<float>(), 0);
            }

            if (rawInput.x > 0.5f) HandleMoveCursorRight();
            else if (rawInput.x < -0.5f) HandleMoveCursorLeft();
        }

        private void HandleMoveCursorRight () {
            if (textMeshPro == null) return;

            if (isTextHighlighted) {
                // Move cursor to end of highlighted text
                cursorCurrentIndex = value.Length - 1;
            } else {
                cursorCurrentIndex = Mathf.Clamp(cursorCurrentIndex + 1, -1, value.Length - 1);
            }

            UpdateCursorPosition();
            UnHighlightText();
        }

        private void HandleMoveCursorLeft () {
            if (textMeshPro == null) return;

            if (isTextHighlighted) {
                // Move cursor to start of highlighted text
                cursorCurrentIndex = -1;
            } else {
                cursorCurrentIndex = Mathf.Clamp(cursorCurrentIndex - 1, -1, value.Length - 1);
            }

            UpdateCursorPosition();
            UnHighlightText();
        }

        private void HandleTextInput (char character) {
            if (value.Length >= maxCharacters) return;

            // Filter out non-printable characters if necessary (though onTextInput generally only fires for printable ones)
            if (char.IsControl(character)) return;

            // Remove highlighted text first
            if (isTextHighlighted) {
                int highlightStart = Mathf.Max(0, startHighlightIndex);
                int highlightEnd = Mathf.Min(value.Length - 1, endHighlightIndex);
                int lengthToRemove = highlightEnd - highlightStart + 1;

                SetValue(value.Remove(highlightStart, lengthToRemove));

                cursorCurrentIndex = highlightStart - 1; // Set cursor before the removed text
                UnHighlightText();
            }

            SetValue(value.Insert(cursorCurrentIndex + 1, character.ToString()));

            // Move cursor forward
            HandleMoveCursorRight();
        }

        private void Update () {
            if (isRepeating && Time.time >= nextRepeatTime) {
                currentAction?.Invoke();
                nextRepeatTime = Time.time + repeatRate;
            }
        }

        public override void Init () {
            base.Init();

            value = textMeshPro != null ? textMeshPro.text : "";

            OnUnfocus();
        }

        public override void OnMouseDown () {
            // TODO: handle double/triple clicks for word/line selection
            // TODO: handle click and drag for selection
            // TODO: handle clicking outside to unfocus

            base.OnMouseDown();

            if (isDisabled) return;

            OnFocus();
        }

        public override void OnMouseExit () {
            // leave text focused even if mouse exits (for keyboard input)
            if (isFocused) return;

            base.OnMouseExit();

            // if (isDisabled) return;

            OnUnfocus();
        }

        private void OnFocus () {
            if (isDisabled) return;

            // Only focus if not already focused
            if (!isFocused) {
                Debug.Log($"Clicked first time");
                // TODO: unfocus all other inputs in the scene

                HighlightFullText();
                StartCursorBlinking();
                isFocused = true;
                Keyboard.current.onTextInput += HandleTextInput;
            } else if (isTextHighlighted) {
                Debug.Log($"Clicked a second time");
                UnHighlightText();

                SetCursorToClickPosition();
            } else {
                Debug.Log($"Clicked a third time");
                SetCursorToClickPosition();
            }
        }

        private void OnUnfocus () {
            if (isDisabled) return;

            // Only unfocus if currently focused
            // if (isFocused) {
            StopCursorBlinking();

            UnHighlightText();
            isFocused = false;

            if (Keyboard.current != null) Keyboard.current.onTextInput -= HandleTextInput;
            // }
        }

        private void HighlightFullText () {
            startHighlightIndex = -1;
            endHighlightIndex = value.Length - 1;
            HighlightText();
            isTextHighlighted = true;
        }

        private void HighlightText () {
            if (textMeshPro == null || highlightObject == null) return;
            if (startHighlightIndex == endHighlightIndex || startHighlightIndex > endHighlightIndex || startHighlightIndex < -1 || endHighlightIndex < -1 || endHighlightIndex >= value.Length) return;

            Vector3 startPos = GetWorldPositionAtTextIndex(startHighlightIndex);
            Vector3 endPos = GetWorldPositionAtTextIndex(endHighlightIndex);
            Vector3 midPoint = (startPos + endPos) / 2f;
            midPoint.z = highlightObject.transform.position.z;
            float width = Mathf.Abs(endPos.x - startPos.x);

            highlightObject.transform.position = midPoint;
            highlightObject.transform.localScale = new Vector3(width, highlightObject.transform.localScale.y, highlightObject.transform.localScale.z);
            highlightObject.SetActive(true);
        }

        private void UnHighlightText () {
            highlightObject.SetActive(false);
            isTextHighlighted = false;
        }

        private void StartCursorBlinking (bool setActive = false) {
            // Ensure only one invoke is running
            CancelInvoke(nameof(ToggleCursor));
            InvokeRepeating(nameof(ToggleCursor), cursorBlinkDelay, cursorBlinkRate);
            if (setActive) cursorObject.SetActive(true);
        }

        private void StopCursorBlinking () {
            CancelInvoke(nameof(ToggleCursor));
            cursorObject.SetActive(false);
        }

        private void ToggleCursor () {
            // Hide cursor if text is highlighted
            if (isTextHighlighted) {
                cursorObject.SetActive(false);
                return;
            }

            // Keep cursor visible while moving
            if (isRepeating) {
                cursorObject.SetActive(true);
                return;
            }

            cursorObject.SetActive(!cursorObject.activeSelf);
        }

        private void SetInitialCursorPosition () {
            // Place cursor at end of text initially
            if (cursorObject == null || textMeshPro == null) return;

            cursorCurrentIndex = value.Length - 1;

            UpdateCursorPosition();
        }

        private void SetCursorToClickPosition () {
            // TODO: Raycast to find click position in text and set cursorCurrentIndex accordingly

        }

        private void UpdateCursorPosition () {
            if (cursorObject == null || textMeshPro == null) return;

            Vector3 cursorPos = GetCursorPosition();
            cursorPos.z = cursorObject.transform.position.z;
            cursorObject.transform.position = cursorPos;
        }

        // Get the world position for a specific text index (-1 for start)
        private Vector3 GetWorldPositionAtTextIndex (int index, float xOffset = 0f) {
            // Calculate the position at the specified index
            TMP_TextInfo textInfo = textMeshPro.textInfo;
            int charCount = textInfo.characterCount;

            if (charCount == 0 || index >= charCount) {
                return textMeshPro.transform.position;
            } else {
                bool isStart = false;
                if (index == -1) {
                    index = 0;
                    isStart = true;
                }

                TMP_CharacterInfo charInfo = textInfo.characterInfo[index];

                Vector3 charPos = charInfo.bottomRight;
                if (isStart) {
                    charPos = charInfo.bottomLeft;
                    charPos.x -= xOffset;
                } else {
                    charPos.x += xOffset;
                }

                charPos.y = 0f;

                // Convert local position to world position
                Vector3 worldPos = textMeshPro.transform.TransformPoint(charPos);

                return worldPos;
            }
        }

        // Get the world position for the cursor based on the current index
        private Vector3 GetCursorPosition () {
            return GetWorldPositionAtTextIndex(cursorCurrentIndex, cursorObject.transform.localScale.x / 2f);
        }

        public string GetValue () {
            return value;
        }

        public void SetValue (string newValue) {
            value = newValue;
            if (textMeshPro != null) {
                textMeshPro.text = value;
                textMeshPro.ForceMeshUpdate(true, true);
            }
        }

        public int GetMaxCharacters () {
            return maxCharacters;
        }

        public void SetMaxCharacters (int maxChars) {
            maxCharacters = maxChars;
        }
    }
}
