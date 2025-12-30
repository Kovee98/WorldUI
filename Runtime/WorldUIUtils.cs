using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Threading;
using System.Collections;

namespace WorldUI {
    public static class WorldUIUtils {
        public static void DrawGizmoCircle (Vector3 circleCenter, Vector3 circleNormal, float circleRadius, int segments = 100) {
            Vector3 radiusVector = Mathf.Abs(Vector3.Dot(circleNormal, Vector3.right)) - 1f <= Mathf.Epsilon
                ? Vector3.Cross(circleNormal, Vector3.forward).normalized
                : Vector3.Cross(circleNormal, Vector3.right).normalized;
            radiusVector *= circleRadius;
            float angleBetweenSegments = 360f / segments;
            Vector3 previousCircumferencePoint = circleCenter + radiusVector;
            for (int i = 0; i < segments; ++i) {
                radiusVector = Quaternion.AngleAxis(angleBetweenSegments, circleNormal) * radiusVector;
                Vector3 newCircumferencePoint = circleCenter + radiusVector;
                Gizmos.DrawLine(previousCircumferencePoint, newCircumferencePoint);
                previousCircumferencePoint = newCircumferencePoint;
            }
        }


        public static Bounds CalculateLocalBounds (Transform parent) {
            return CalculateLocalBounds(parent, WorldUIConstants.DEFAULT_RENDERER_FILTER);
        }

        // Calculates the axis-aligned bounding box that encloses all Renderer bounds of the parent’s children in the parent’s local space.
        public static Bounds CalculateLocalBounds (Transform parent, WorldUIRendererFilter filter) {
            // Initialize empty bounds at parent origin
            Bounds localBounds = new Bounds(Vector3.zero, Vector3.zero);
            bool first = true;

            // Iterate through every Renderer on children
            foreach (Renderer rend in parent.GetComponentsInChildren<Renderer>()) {
                if (rend is BillboardRenderer && !filter.HasFlag(WorldUIRendererFilter.BillboardRenderer)) continue;
                if (rend is LineRenderer && !filter.HasFlag(WorldUIRendererFilter.LineRenderer)) continue;
                if (rend is MeshRenderer && !filter.HasFlag(WorldUIRendererFilter.MeshRenderer)) continue;
                if (rend is ParticleSystemRenderer && !filter.HasFlag(WorldUIRendererFilter.ParticleSystemRenderer)) continue;
                if (rend is SkinnedMeshRenderer && !filter.HasFlag(WorldUIRendererFilter.SkinnedMeshRenderer)) continue;
                if (rend is SpriteMask && !filter.HasFlag(WorldUIRendererFilter.SpriteMask)) continue;
                if (rend is SpriteRenderer && !filter.HasFlag(WorldUIRendererFilter.SpriteRenderer)) continue;
                if (rend is TrailRenderer && !filter.HasFlag(WorldUIRendererFilter.TrailRenderer)) continue;
                if (rend is UnityEngine.U2D.SpriteShapeRenderer && !filter.HasFlag(WorldUIRendererFilter.SpriteShapeRenderer)) continue;
                if (!rend.enabled && !filter.HasFlag(WorldUIRendererFilter.Disabled)) continue;

                // Get the world-space AABB
                Bounds worldBounds = rend.bounds;

                // For each corner of the AABB…
                foreach (Vector3 corner in GetCorners(worldBounds)) {
                    // Convert to parent-local space
                    Vector3 localCorner = parent.InverseTransformPoint(corner);

                    // On first corner, initialize; afterwards expand
                    if (first) {
                        localBounds = new Bounds(localCorner, Vector3.zero);
                        first = false;
                    } else {
                        localBounds.Encapsulate(localCorner);
                    }
                }
            }

            return localBounds;
        }

        // Helper: returns the 8 corners of a Bounds
        public static Vector3[] GetCorners (Bounds b) {
            Vector3 c = b.center;
            Vector3 e = b.extents;
            return new[] {
                c + new Vector3( e.x,  e.y,  e.z),
                c + new Vector3( e.x,  e.y, -e.z),
                c + new Vector3( e.x, -e.y,  e.z),
                c + new Vector3( e.x, -e.y, -e.z),
                c + new Vector3(-e.x,  e.y,  e.z),
                c + new Vector3(-e.x,  e.y, -e.z),
                c + new Vector3(-e.x, -e.y,  e.z),
                c + new Vector3(-e.x, -e.y, -e.z),
            };
        }

        // rectint for pixel-perfect
        public static RectInt WorldBoundsToScreenRect (Transform transform, Camera cam, WorldUIRendererFilter rendererFilter) {
            // public static Rect WorldBoundsToScreenRect (Bounds bounds, Camera cam) {
            Bounds bounds = CalculateLocalBounds(transform, rendererFilter);
            Vector3[] corners = GetCorners(bounds);

            List<float> xValues = new List<float>();
            List<float> yValues = new List<float>();
            foreach (var corner in corners) {
                Vector3 worldPoint = transform.TransformPoint(corner);
                Vector3 screenPoint = cam.WorldToScreenPoint(worldPoint);
                xValues.Add(screenPoint.x);
                yValues.Add(screenPoint.y);
            }

            float minX = xValues.Min();
            float maxX = xValues.Max();
            float minY = yValues.Min();
            float maxY = yValues.Max();

            // return new Rect(minX, minY, maxX - minX, maxY - minY);
            return new RectInt((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
        }

        public static Rect GetScreenRect (Renderer rend, Camera cam) {
            // 1. Local bounds
            Bounds localB = (rend is MeshRenderer)
                ? rend.GetComponent<MeshFilter>().sharedMesh.bounds
                : rend.localBounds;

            Transform t = rend.transform;
            Vector3[] corners = new Vector3[8];
            int i = 0;

            // 2. Build local corners (±X, ±Y, ±Z)
            for (int x = -1; x <= 1; x += 2) {
                for (int y = -1; y <= 1; y += 2) {
                    for (int z = -1; z <= 1; z += 2) {
                        Vector3 sign = new Vector3(x, y, z);
                        Vector3 localCorner = localB.center + Vector3.Scale(localB.extents, sign);
                        corners[i++] = t.TransformPoint(localCorner);
                    }
                }
            }

            // 3. Project & find min/max
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (var worldPt in corners) {
                Vector3 sp = cam.WorldToScreenPoint(worldPt);
                if (sp.z < 0) continue;  // optionally skip behind‐camera points

                minX = Mathf.Min(minX, sp.x);
                maxX = Mathf.Max(maxX, sp.x);
                minY = Mathf.Min(minY, sp.y);
                maxY = Mathf.Max(maxY, sp.y);
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        public static void TraverseDepthFirst<T> (Transform parent, System.Action<T> onFound)
        where T : Component {
            foreach (Transform child in parent) {
                var comp = child.GetComponent<T>();
                if (comp != null)
                    onFound(comp);

                // Recurse into this child’s subtree
                TraverseDepthFirst(child, onFound);
            }
        }
    }
}
