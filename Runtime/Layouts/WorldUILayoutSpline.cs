using UnityEngine;
using UnityEngine.Splines;

namespace WorldUI {
    public class WorldUILayoutSpline : WorldUILayout {
        [Header("Spline")]
        [SerializeField] private SplineContainer splineContainer;

        [Header("Ordering")]
        [Tooltip("Reverse the order of the children.")]
        [SerializeField] private bool reverseOrder = false;
        private int childIndex;
        private int childIndexInc;

        [Header("Positioning")]
        [SerializeField] private WorldUISplineAlignment justifyContent = WorldUISplineAlignment.Center;

        [Space(10)]

        [Tooltip("The anchor point on the X-axis to use for child positioning.")]
        [SerializeField] private WorldUIAlignment xAlignmentChild = WorldUIAlignment.Center;
        [Tooltip("The anchor point on the Y-axis to use for child positioning.")]
        [SerializeField] private WorldUIAlignment yAlignmentChild = WorldUIAlignment.Center;
        [Tooltip("The anchor point on the Z-axis to use for child positioning.")]
        [SerializeField] private WorldUIAlignment zAlignmentChild = WorldUIAlignment.Center;

        [Space(10)]

        [Tooltip("The anchor point on the X-axis to use for the entire group of children.")]
        [SerializeField] private WorldUIAlignment xAlignmentGroup = WorldUIAlignment.Center;
        [Tooltip("The anchor point on the Y-axis to use for the entire group of children.")]
        [SerializeField] private WorldUIAlignment yAlignmentGroup = WorldUIAlignment.Center;
        [Tooltip("The anchor point on the Z-axis to use for the entire group of children.")]
        [SerializeField] private WorldUIAlignment zAlignmentGroup = WorldUIAlignment.Center;

        [Space(10)]

        [Tooltip("The method used for aligning the group of children together.")]
        [SerializeField] private WorldUISplineAlignmentType splineAlignmentType = WorldUISplineAlignmentType.Offset;
        [Range(0f, 1f)]
        [Tooltip("The starting offset of the spline used for alignment of all children together.")]
        [SerializeField] private float splineAlignmentOffset;
        [Tooltip("The spacing for the children on the spline.")]
        [Range(-1f, 1f)]
        [SerializeField] private float childSpacing = 0f;
        [Range(-1f, 1f)]
        [Tooltip("A constant ratio offset (percentage as a floating point decimal number) to apply to each child before the spline point is calculated.")]
        [SerializeField] private float childSplineOffset;
        [Tooltip("A final constant offset to apply to each child after the other calculations.")]
        [SerializeField] private Vector3 childPositionOffset = Vector3.zero;

        // positions children objects and returns whether this method ran (didn"t get throttled)
        public override void UpdateUI (bool continuousUpdate = false) {
            if (!updateContinuously && continuousUpdate) return;

            // ordering
            childIndex = transform.childCount - 1;
            childIndexInc = -1;
            if (reverseOrder) {
                childIndex = 0;
                childIndexInc = 1;
            }

            // alignment and spacing of each child
            for (int i = 0; i < transform.childCount; i++) {
                Transform child = transform.GetChild(childIndex);
                Bounds childBounds = WorldUIUtils.CalculateLocalBounds(child, childRendererfilter);

                float splineRatio;
                switch (justifyContent) {
                    case WorldUISplineAlignment.Center:
                        splineRatio = (1 - (transform.childCount - 1) * childSpacing) / 2 + i * childSpacing;
                        break;
                    case WorldUISplineAlignment.End:
                        splineRatio = 1 - (transform.childCount - 1) * childSpacing + i * childSpacing;
                        break;
                    case WorldUISplineAlignment.Between:
                        splineRatio = i / (transform.childCount - 1f);
                        break;
                    case WorldUISplineAlignment.Around:
                        splineRatio = (i + 0.5f) / transform.childCount;
                        break;
                    case WorldUISplineAlignment.Evenly:
                        splineRatio = (i + 1f) / (transform.childCount + 1f);
                        break;
                    default: // WorldUISplineAlignment.Evenly
                        splineRatio = childSpacing * i;
                        break;
                }

                // apply the constant childSplineOffset
                splineRatio += childSplineOffset;

                Vector3 splinePoint = splineContainer.EvaluatePosition(splineRatio);
                Vector3 newPosition = transform.InverseTransformPoint(splinePoint);

                // start off with centering to simplify the following adjustments
                newPosition -= childBounds.center;

                if (xAlignmentChild == WorldUIAlignment.Start) {
                    newPosition.x -= childBounds.size.x / 2;
                } else if (xAlignmentChild == WorldUIAlignment.End) {
                    newPosition.x += childBounds.size.x / 2;
                }

                if (yAlignmentChild == WorldUIAlignment.Start) {
                    newPosition.y -= childBounds.size.y / 2;
                } else if (yAlignmentChild == WorldUIAlignment.End) {
                    newPosition.y += childBounds.size.y / 2;
                }

                if (zAlignmentChild == WorldUIAlignment.Start) {
                    newPosition.z -= childBounds.size.z / 2;
                } else if (zAlignmentChild == WorldUIAlignment.End) {
                    newPosition.z += childBounds.size.z / 2;
                }

                child.localPosition = newPosition + childPositionOffset;

                childIndex += childIndexInc;
            }

            if (splineAlignmentType == WorldUISplineAlignmentType.Offset) {
                // spline container alignment
                if (splineContainer != null) {
                    Vector3 centerPoint = splineContainer.EvaluatePosition(splineAlignmentOffset);
                    Vector3 centerPointLocal = transform.InverseTransformPoint(centerPoint);
                    splineContainer.transform.localPosition -= centerPointLocal;
                } else {
                    Debug.LogError($"Spline container is null for {gameObject.name}");
                }
            } else if (splineAlignmentType == WorldUISplineAlignmentType.Bounds) {
                Bounds bounds = WorldUIUtils.CalculateLocalBounds(transform, rendererFilter);
                Vector3 offset = -bounds.center;

                if (xAlignmentGroup == WorldUIAlignment.Start) {
                    offset.x -= bounds.size.x / 2;
                } else if (xAlignmentGroup == WorldUIAlignment.End) {
                    offset.x += bounds.size.x / 2;
                }

                if (yAlignmentGroup == WorldUIAlignment.Start) {
                    offset.y -= bounds.size.y / 2;
                } else if (yAlignmentGroup == WorldUIAlignment.End) {
                    offset.y += bounds.size.y / 2;
                }

                if (zAlignmentGroup == WorldUIAlignment.Start) {
                    offset.z -= bounds.size.z / 2;
                } else if (zAlignmentGroup == WorldUIAlignment.End) {
                    offset.z += bounds.size.z / 2;
                }

                for (int i = 0; i < transform.childCount; i++) {
                    Transform child = transform.GetChild(i);
                    child.localPosition += offset;
                }
            }
        }
    }
}
