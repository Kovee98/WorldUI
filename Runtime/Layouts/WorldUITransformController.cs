using UnityEngine;

namespace WorldUI {
    public class WorldUITransformController : WorldUILayout {
        [Header("Position")]
        [SerializeField] private bool controlPosition;
        [ShowIf("controlPosition")]
        [SerializeField] private WorldUITransformControllerPositionMode positionMode = WorldUITransformControllerPositionMode.Local;
        [ShowIf("showScreenAnchor")]
        [SerializeField] private WorldUITransformControllerScreenAnchors screenAnchor = WorldUITransformControllerScreenAnchors.MiddleCenter;
        bool showScreenAnchor () {
            return controlPosition && positionMode == WorldUITransformControllerPositionMode.Screen;
        }
        [ShowIf("controlPosition")]
        [Tooltip("The anchor point on the X-axis to use for child positioning.")]
        [SerializeField] private WorldUIAlignment xAlignment = WorldUIAlignment.Center;
        [ShowIf("controlPosition")]
        [Tooltip("The anchor point on the Y-axis to use for child positioning.")]
        [SerializeField] private WorldUIAlignment yAlignment = WorldUIAlignment.Center;

        [ShowIf("showPositionTarget")]
        [SerializeField] private Transform positionTarget;
        bool showPositionTarget () {
            return controlPosition && positionMode != WorldUITransformControllerPositionMode.Screen;
        }

        [ShowIf("controlPosition")]
        [SerializeField] private Vector3 positionOffset = Vector3.zero;

        [Header("Rotation")]
        [SerializeField] private bool controlRotation;
        [ShowIf("controlRotation")]
        [SerializeField] private WorldUITransformControllerRotationMode rotationMode = WorldUITransformControllerRotationMode.Match;

        [ShowIf("showRotationTarget")]
        [SerializeField] private Transform rotationTarget;
        bool showRotationTarget () {
            return controlRotation &&
                rotationMode != WorldUITransformControllerRotationMode.LookAtScreen &&
                rotationMode != WorldUITransformControllerRotationMode.LookAwayFromScreen;
        }

        [ShowIf("controlRotation")]
        [SerializeField] private Vector3 rotationOffset = Vector3.zero;

        [Header("Scale")]
        [SerializeField] private bool controlScale;
        [ShowIf("controlScale")]
        [SerializeField] private WorldUITransformControllerScaleMode scaleMode = WorldUITransformControllerScaleMode.Match;
        [ShowIf("isScaleControlledButNotScreenMode")]
        [SerializeField] private Transform scaleTarget;
        [ShowIf("isScaleControlledButNotScreenMode")]
        [SerializeField] private Vector3 scaleOffset = Vector3.zero;
        bool isScaleControlledButNotScreenMode () {
            return controlScale && scaleMode != WorldUITransformControllerScaleMode.Screen;
        }
        [ShowIf("isScaleControlledAndScreenMode")]
        [SerializeField] private bool preserveAspectRatio = true;
        [ShowIf("isScaleControlledAndScreenMode")]
        [SerializeField] private bool usePercentages = false;
        [ShowIf("isScaleControlledAndScreenModeAndPixels")]
        [Label("Size (px)")]
        [SerializeField] private Vector2 desiredSize;
        [ShowIf("isScaleControlledAndScreenModeAndPercentage")]
        [Label("Size (%)")]
        [SerializeField] private Vector2 desiredSizePercent;
        private Vector3 prevLocalScale;
        private Vector2 prevDesiredSize;
        private Vector2 prevDesiredSizePercentage;
        private float prevObjScreenWidth;
        private float prevObjScreenHeight;
        bool isScaleControlledAndScreenMode () {
            return controlScale && scaleMode == WorldUITransformControllerScaleMode.Screen;
        }
        bool isScaleControlledAndScreenModeAndPixels () {
            return controlScale && scaleMode == WorldUITransformControllerScaleMode.Screen && !usePercentages;
        }
        bool isScaleControlledAndScreenModeAndPercentage () {
            return controlScale && scaleMode == WorldUITransformControllerScaleMode.Screen && usePercentages;
        }

        public override void UpdateUI (bool continuousUpdate = false) {
            if (!updateContinuously && continuousUpdate) return;

            // rotate
                Quaternion baseRot = transform.rotation;
            if (controlRotation) {
                if (positionTarget != null) baseRot = positionTarget.rotation;

                if (rotationMode == WorldUITransformControllerRotationMode.World) {
                    transform.eulerAngles = rotationOffset;
                } else if (rotationMode == WorldUITransformControllerRotationMode.Local) {
                    transform.localEulerAngles = rotationOffset;
                } else if (rotationMode == WorldUITransformControllerRotationMode.Match) {
                    transform.rotation = baseRot;
                    transform.localEulerAngles += rotationOffset;
                } else if (rotationMode == WorldUITransformControllerRotationMode.LookAt) {
                    transform.LookAt(rotationTarget);
                    transform.localEulerAngles += rotationOffset;
                } else if (rotationMode == WorldUITransformControllerRotationMode.LookAway) {
                    transform.LookAt(rotationTarget);
                    transform.Rotate(0f, 180f, 0f, Space.Self);
                    transform.localEulerAngles += rotationOffset;
                } else if (rotationMode == WorldUITransformControllerRotationMode.LookAtScreen) {
                    Camera cam = GetCamera();
                    transform.LookAt(cam.transform);
                    transform.localEulerAngles += rotationOffset;
                } else if (rotationMode == WorldUITransformControllerRotationMode.LookAwayFromScreen) {
                    Camera cam = GetCamera();
                    transform.LookAt(cam.transform);
                    transform.Rotate(0f, 180f, 0f, Space.Self);
                    transform.localEulerAngles += rotationOffset;
                }
            }

            // scale
            Vector3 baseScale = transform.localScale;
            if (controlScale) {
                if (scaleTarget != null) baseScale = scaleTarget.localScale;

                if (scaleMode == WorldUITransformControllerScaleMode.World) {
                    transform.localScale = Vector3.one + scaleOffset;
                } else if (scaleMode == WorldUITransformControllerScaleMode.Match) {
                    transform.localScale = baseScale + scaleOffset;
                } else if (scaleMode == WorldUITransformControllerScaleMode.Screen) {
                    Camera cam = GetCamera();
                    RectInt screenRect = WorldUIUtils.WorldBoundsToScreenRect(transform, cam, rendererFilter);

                    float objScreenWidth = screenRect.width;
                    float objScreenHeight = screenRect.height;

                    if (transform.localScale.x != prevLocalScale.x) {
                        desiredSize.x = objScreenWidth;
                        desiredSizePercent.x = objScreenWidth / Screen.width * 100;
                    }

                    if (transform.localScale.y != prevLocalScale.y) {
                        desiredSize.y = objScreenHeight;
                        desiredSizePercent.y = objScreenHeight / Screen.height * 100;
                    }

                    Vector3 newScale = transform.localScale;

                    float desiredWidth = desiredSize.x;
                    float desiredHeight = desiredSize.y;
                    if (usePercentages) {
                        desiredWidth = desiredSizePercent.x / 100 * Screen.width;
                        desiredHeight = desiredSizePercent.y / 100 * Screen.height;
                    }

                    float xRatio = desiredWidth / objScreenWidth;
                    float yRatio = desiredHeight / objScreenHeight;

                    if (preserveAspectRatio) {
                        if (prevDesiredSize.x != desiredSize.x || prevObjScreenWidth != objScreenWidth || prevDesiredSizePercentage.x != desiredSizePercent.x) {
                            newScale *= xRatio;
                            desiredSize.y *= xRatio;
                            desiredSizePercent.y *= xRatio;
                        } else if (prevDesiredSize.y != desiredSize.y || prevObjScreenHeight != objScreenHeight || prevDesiredSizePercentage.y != desiredSizePercent.y) {
                            newScale *= yRatio;
                            desiredSize.x *= yRatio;
                            desiredSizePercent.x *= yRatio;
                        }
                    } else {
                        newScale.x *= xRatio;
                        newScale.y *= yRatio;
                    }

                    transform.localScale = newScale;

                    prevLocalScale = newScale;
                    prevDesiredSize = desiredSize;
                    prevDesiredSizePercentage = desiredSizePercent;
                    prevObjScreenWidth = objScreenWidth;
                    prevObjScreenHeight = objScreenHeight;
                }
            }

            // position
            Vector3 basePos = Vector3.zero;
            if (controlPosition) {
                if (positionTarget != null) basePos = positionTarget.position;
                Vector3 newPos = basePos;

                if (positionMode == WorldUITransformControllerPositionMode.World) {
                    newPos = basePos + positionOffset;
                } else if (positionMode == WorldUITransformControllerPositionMode.Local) {
                    newPos = basePos + positionTarget.rotation * positionOffset;
                } else if (positionMode == WorldUITransformControllerPositionMode.Self) {
                    newPos = basePos + transform.rotation * positionOffset;
                } else if (positionMode == WorldUITransformControllerPositionMode.Screen) {
                    Camera cam = GetCamera();
                    RectInt screenRect = WorldUIUtils.WorldBoundsToScreenRect(transform, cam, rendererFilter);

                    Vector3 alignmentOffset = Vector3.zero;
                    if (xAlignment == WorldUIAlignment.Start) {
                        alignmentOffset.x += screenRect.width / 2;
                    } else if (xAlignment == WorldUIAlignment.End) {
                        alignmentOffset.x -= screenRect.width / 2;
                    }

                    if (yAlignment == WorldUIAlignment.Start) {
                        alignmentOffset.y -= screenRect.height / 2;
                    } else if (yAlignment == WorldUIAlignment.End) {
                        alignmentOffset.y += screenRect.height / 2;
                    }

                    Ray ray = GetScreenRay(screenAnchor, positionOffset + alignmentOffset);
                    basePos = ray.GetPoint(positionOffset.z);
                    newPos = basePos;
                }

                Bounds objBounds = WorldUIUtils.CalculateLocalBounds(transform, rendererFilter);
                if (positionMode != WorldUITransformControllerPositionMode.Screen) {
                    newPos -= objBounds.center;

                    if (xAlignment == WorldUIAlignment.Start) {
                        newPos.x -= objBounds.size.x / 2;
                    } else if (xAlignment == WorldUIAlignment.End) {
                        newPos.x += objBounds.size.x / 2;
                    }

                    if (yAlignment == WorldUIAlignment.Start) {
                        newPos.y -= objBounds.size.y / 2;
                    } else if (yAlignment == WorldUIAlignment.End) {
                        newPos.y += objBounds.size.y / 2;
                    }
                }

                transform.position = newPos;
            }
        }

        private Ray GetScreenRay (WorldUITransformControllerScreenAnchors anchor, Vector3 offset) {
            Camera cam = GetCamera();
            float maxWidth = Screen.width - 1;
            float maxHeight = Screen.height - 1;
            float halfWidth = Screen.width / 2;
            float halfHeight = Screen.height / 2;

            switch (anchor) {
                case WorldUITransformControllerScreenAnchors.TopLeft: return cam.ScreenPointToRay(new Vector2(0f + offset.x, maxHeight + offset.y));
                case WorldUITransformControllerScreenAnchors.TopCenter: return cam.ScreenPointToRay(new Vector2(halfWidth + offset.x, maxHeight + offset.y));
                case WorldUITransformControllerScreenAnchors.TopRight: return cam.ScreenPointToRay(new Vector2(maxWidth + offset.x, maxHeight + offset.y));

                case WorldUITransformControllerScreenAnchors.MiddleLeft: return cam.ScreenPointToRay(new Vector2(0f + offset.x, halfHeight + offset.y));
                case WorldUITransformControllerScreenAnchors.MiddleCenter: return cam.ScreenPointToRay(new Vector2(halfWidth + offset.x, halfHeight + offset.y));
                case WorldUITransformControllerScreenAnchors.MiddleRight: return cam.ScreenPointToRay(new Vector2(maxWidth + offset.x, halfHeight + offset.y));

                case WorldUITransformControllerScreenAnchors.BottomLeft: return cam.ScreenPointToRay(new Vector2(0f + offset.x, 0f + offset.y));
                case WorldUITransformControllerScreenAnchors.BottomCenter: return cam.ScreenPointToRay(new Vector2(halfWidth + offset.x, 0f + offset.y));
                case WorldUITransformControllerScreenAnchors.BottomRight: return cam.ScreenPointToRay(new Vector2(maxWidth + offset.x, 0f + offset.y));

                default: return cam.ScreenPointToRay(new Vector2(halfWidth, halfHeight));
            }
        }
    }
}
