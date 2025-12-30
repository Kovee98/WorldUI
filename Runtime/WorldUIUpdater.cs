using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace WorldUI {
    [ExecuteAlways]
    public class WorldUIUpdater : MonoBehaviour {
        [Header("General")]
        [SerializeField] private WorldUIUpdaterMode updateMode = WorldUIConstants.DEFAULT_UPDATER_MODE;
        [ShowIf("updateMode", equals: WorldUIUpdaterMode.Manual)]
        [SerializeField] private List<WorldUILayout> worldUILayouts = new List<WorldUILayout>();
        [Tooltip("Minimum time (in ms) between automatic recalculations to prevent excessive updates.")]
        [SerializeField] private float autoRecalculateThrottle = 10f;

        void Start () {
            UpdateUIThrottled(false);
        }

        void Awake () {
            UpdateUIThrottled(false);
        }

        private void OnValidate () {
            UpdateUIThrottled(false);
        }

        private void OnGUI () {
            UpdateUIThrottled(true);
        }

        // [ExecuteAlways] on the class means this will execute even when not in play mode
        private void Update () {
            UpdateUIThrottled(true);
        }

        // called on every redraw of the Scene view or Game view
        private void OnRenderObject () {
            UpdateUIThrottled(true);
        }

        public void UpdateUIThrottled (bool continuousUpdate = false) {
            if (Application.IsPlaying(gameObject)) {
                // throttle that respects timeScale/play/pause
                PlayerThrottle(() => UpdateUI(continuousUpdate), autoRecalculateThrottle / 1000);
            } else {
                // throttle that doesn't respect timeScale/play/pause
                EditorThrottle(() => UpdateUI(continuousUpdate), autoRecalculateThrottle / 1000);
            }
        }

        public void UpdateUI (bool continuousUpdate = false) {
            if (updateMode == WorldUIUpdaterMode.Manual) {
                if (worldUILayouts.Count > 0) {
                    foreach (var uiObject in worldUILayouts) {
                        uiObject.UpdateUI();
                    }
                }
            } else if (updateMode == WorldUIUpdaterMode.All) {
                var allWorldUILayouts = FindObjectsByType<WorldUILayout>(
                    findObjectsInactive: FindObjectsInactive.Exclude,
                    sortMode: FindObjectsSortMode.None
                );

                // sort by child depth
                var sortedWorldUILayouts = allWorldUILayouts
                    .OrderByDescending((comp) => GetDepth(comp.transform))
                    .ToList();

                // 'comp' starts at the deepest child
                foreach (var worldUILayout in sortedWorldUILayouts) {
                    // Debug.Log($"{worldUILayout.gameObject.name} at depth {GetDepth(worldUILayout.transform)}");
                    worldUILayout.UpdateUI(continuousUpdate);
                }
            } else if (updateMode == WorldUIUpdaterMode.SelfAndChildren) {
                // trigger recalculations for all children starting with the deepest child that has a WorldUILayoutBase component
                UpdateUIChildren(continuousUpdate);

                // finally, after repositioning all the children, reposition this object
                UpdateUISelf(continuousUpdate);
            } else if (updateMode == WorldUIUpdaterMode.Children) {
                UpdateUIChildren(continuousUpdate);
            } else if (updateMode == WorldUIUpdaterMode.Self) {
                UpdateUISelf(continuousUpdate);
            }

            return;
        }

        void UpdateUISelf (bool continuousUpdate = false) {
            if (TryGetComponent(out WorldUILayout layout)) {
                layout.UpdateUI(continuousUpdate);
            }
        }

        void UpdateUIChildren (bool continuousUpdate = false) {
            WorldUIUtils.TraverseDepthFirst<WorldUILayout>(transform, child => {
                if (child.TryGetComponent(out WorldUILayout childLayout)) {
                    childLayout.UpdateUI(continuousUpdate);
                }
            });
        }

        int GetDepth (Transform t) {
            int depth = 0;

            while (t.parent != null) {
                depth++;
                t = t.parent;
            }

            return depth;
        }

        /*
            Throttling
        */

        private float lastCallTime = 0f;
        private Coroutine throttleCoroutine;

        public void PlayerThrottle (System.Action functionToCall, float interval) {
            if (throttleCoroutine != null) {
                StopCoroutine(throttleCoroutine);
            }

            // Start a new coroutine to handle the throttle logic
            throttleCoroutine = StartCoroutine(ThrottleCoroutine(functionToCall, interval));
        }

        private IEnumerator ThrottleCoroutine (System.Action functionToCall, float interval) {
            float elapsedTime = Time.unscaledTime - lastCallTime;

            // If the elapsed time is less than the interval, wait for the remaining time
            if (elapsedTime < interval) {
                yield return new WaitForSecondsRealtime(interval - elapsedTime);
            }

            // Execute the function and update the last call time
            functionToCall.Invoke();
            lastCallTime = Time.unscaledTime;
        }

        private double _lastCallTime;
        private Action _trailingAction;
        private double _trailingInterval;

        public void EditorThrottle (Action functionToCall, double interval) {
            double currentTime = EditorApplication.timeSinceStartup;

            if (currentTime - _lastCallTime >= interval) {
                functionToCall.Invoke();
                _lastCallTime = currentTime;
                return;
            }

            if (_trailingAction == null) {
                _trailingAction = functionToCall;
                _trailingInterval = interval;
                EditorApplication.delayCall += ProcessTrailingCall;
            } else {
                _trailingAction = functionToCall;
            }
        }

        private void ProcessTrailingCall () {
            double currentTime = EditorApplication.timeSinceStartup;

            if (_trailingAction != null && currentTime - _lastCallTime >= _trailingInterval) {
                _trailingAction.Invoke();
                _lastCallTime = currentTime;
                _trailingAction = null;
                EditorApplication.delayCall -= ProcessTrailingCall;
            } else {
                EditorApplication.delayCall += ProcessTrailingCall;
            }

            // This is the key to forcing updates!
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}
