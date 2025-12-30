using UnityEngine;

namespace WorldUI {
    public class WorldUILayoutSimple : WorldUILayout {
        [Header("Ordering")]
        [Tooltip("Reverse the order of the children.")]
        [SerializeField] private bool reverseOrder = false;
        private int childIndex;
        private int childIndexInc;

        [Header("Positioning")]
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
        [Tooltip("The spacing for the children.")]
        [SerializeField] private Vector3 spacing = Vector3.zero;
        [Tooltip("Constant offset to apply to the entire group of children.")]
        [InspectorName("Offset")]
        [SerializeField] private Vector3 constantOffset = Vector3.zero;

        // positions children objects and returns whether this method ran (didn't get throttled)
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
                Vector3 newPosition = spacing * i;

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

                child.localPosition = newPosition;

                childIndex += childIndexInc;
            }

            // alignment of the entire group of children (needs to be done after alignment and spacing of each child)
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
                child.localPosition += offset + constantOffset;
            }
        }
    }
}
