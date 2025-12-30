using UnityEngine;
using UnityEngine.Events;

namespace WorldUI {
    public class WorldUIButton : MonoBehaviour {
        [Header("General Settings")]
        [SerializeField] protected bool isDisabled = false;
        [SerializeField] protected bool isHighlighted = false;
        [SerializeField] protected bool isPressed = false;
        [SerializeField] protected Material defaultMaterial;
        [SerializeField] protected Material disabledMaterial;

        // highlight transform adjustments
        [Header("Highlight Adjustments")]
        [SerializeField] protected Material highlightedMaterial;
        [SerializeField] protected Vector3 highlightOffset = Vector3.zero;
        [SerializeField] protected Vector3 highlightRotation = Vector3.zero;
        [SerializeField] protected Vector3 highlightScale = Vector3.one;
        public UnityEvent onHighlight;
        public UnityEvent onUnhighlight;

        // pressed transform adjustments
        [Header("Pressed Adjustments")]
        [SerializeField] protected Material pressedMaterial;
        [SerializeField] protected Vector3 pressedOffset = Vector3.zero;
        [SerializeField] protected Vector3 pressedRotation = Vector3.zero;
        [SerializeField] protected Vector3 pressedScale = Vector3.one;
        public UnityEvent onPress;
        public UnityEvent onRelease;

        protected Vector3 ogPosition;
        protected Vector3 ogRotation;
        protected Vector3 ogScale;

        void Start () {
            Init();
        }

        void Awake () {
            Init();
        }

        private void OnValidate () {
            Init();
        }

        public virtual void Init () {
            isHighlighted = false;
            isPressed = false;
            ogPosition = transform.localPosition;
            ogRotation = transform.localEulerAngles;
            ogScale = transform.localScale;

            ApplyEffects();
        }

        public virtual void OnMouseEnter () {
            if (isDisabled) return;

            isHighlighted = true;
            onHighlight?.Invoke();

            ApplyEffects();
        }

        public virtual void OnMouseExit () {
            if (isDisabled) return;

            isHighlighted = false;
            onUnhighlight?.Invoke();

            ApplyEffects();
        }

        public virtual void OnMouseDown () {
            if (isDisabled) return;

            isPressed = true;
            onPress?.Invoke();

            ApplyEffects();
        }

        public virtual void OnMouseUp () {
            if (isDisabled) return;

            isPressed = false;
            onRelease?.Invoke();

            ApplyEffects();
        }

        public void SetDisabled (bool isDisabled) {
            this.isDisabled = isDisabled;

            // reset states
            if (isDisabled) {
                isHighlighted = false;
                isPressed = false;
            }

            ApplyEffects();
        }

        public bool GetDisabled () {
            return isDisabled;
        }

        public bool GetHighlighted () {
            return isHighlighted;
        }

        public bool GetPressed () {
            return isPressed;
        }

        public void ApplyEffects () {
            var renderer = GetComponent<Renderer>();

            if (isDisabled) {
                if (renderer != null && disabledMaterial != null) {
                    renderer.material = disabledMaterial;
                } else if (renderer != null && defaultMaterial != null) {
                    renderer.material = defaultMaterial;
                } else {
                    Debug.LogWarning($"{gameObject.name} is disabled but has no disabledMaterial or defaultMaterial assigned.");
                }

                // revert transform adjustments
                transform.localPosition = ogPosition;
                transform.localEulerAngles = ogRotation;
                transform.localScale = ogScale;
            } else if (isPressed) {
                if (renderer != null && pressedMaterial != null) {
                    renderer.material = pressedMaterial;
                } else if (renderer != null && defaultMaterial != null) {
                    renderer.material = defaultMaterial;
                } else {
                    Debug.LogWarning($"{gameObject.name} is pressed but has no pressedMaterial or defaultMaterial assigned.");
                }

                // apply pressed transform adjustments
                transform.localPosition = ogPosition + pressedOffset;
                transform.localEulerAngles = ogRotation + pressedRotation;
                transform.localScale = Vector3.Scale(ogScale, pressedScale);
            } else if (isHighlighted) {
                if (renderer != null && highlightedMaterial != null) {
                    renderer.material = highlightedMaterial;
                } else if (renderer != null && defaultMaterial != null) {
                    renderer.material = defaultMaterial;
                } else {
                    Debug.LogWarning($"{gameObject.name} is highlighted but has no highlightedMaterial or defaultMaterial assigned.");
                }

                // apply highlight transform adjustments
                transform.localPosition = ogPosition + highlightOffset;
                transform.localEulerAngles = ogRotation + highlightRotation;
                transform.localScale = Vector3.Scale(ogScale, highlightScale);
            } else {
                if (renderer != null && defaultMaterial != null) {
                    renderer.material = defaultMaterial;
                } else {
                    Debug.LogWarning($"{gameObject.name} is in default state but has no defaultMaterial assigned.");
                }

                // revert transform adjustments
                transform.localPosition = ogPosition;
                transform.localEulerAngles = ogRotation;
                transform.localScale = ogScale;
            }
        }
    }
}
