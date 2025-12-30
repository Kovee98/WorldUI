using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WorldUI {
    public class WorldUIBorder {
        public WorldUIBorderLocation location;
        public Vector3 position;
        public Vector3 size;
    }

    public class WorldUIBorders : MonoBehaviour {
        [SerializeField] private Dictionary<WorldUIBorderLocation, WorldUIBorder> borders = new Dictionary<WorldUIBorderLocation, WorldUIBorder>();
        [SerializeField] private Dictionary<WorldUIBorderLocation, GameObject> borderObjects = new Dictionary<WorldUIBorderLocation, GameObject>();
        [SerializeField] private bool hideChildrenBorders = false;
        [SerializeField] private Vector3 borderWidth = new Vector3(1f, 1f, 1f);
        // [SerializeField] private float borderRadius = 0.25f;

        private void OnValidate () {
            CalculateMeshBorders();
        }

        // Calculate the positions and sizes of the mesh borders based on the mesh's bounds.
        // This method should populate the 'borders' dictionary with WorldUIBorder objects.
        public void CalculateMeshBorders () {
            float xScale = borderWidth.x / transform.localScale.x;
            float yScale = borderWidth.y / transform.localScale.y;
            float zScale = borderWidth.z / transform.localScale.z;

            float xPositionOffset = 0.5f + (xScale / 2);
            float yPositionOffset = 0.5f + (yScale / 2);
            float zPositionOffset = 0.5f + (zScale / 2);

            /*
                Faces
            */
            borders[WorldUIBorderLocation.TopFace] = new WorldUIBorder {
                location = WorldUIBorderLocation.TopFace,
                position = new Vector3(0, yPositionOffset, 0),
                size = new Vector3(1, yScale, 1),
            };
            borders[WorldUIBorderLocation.BottomFace] = new WorldUIBorder {
                location = WorldUIBorderLocation.BottomFace,
                position = new Vector3(0, -yPositionOffset, 0),
                size = new Vector3(1, yScale, 1),
            };
            borders[WorldUIBorderLocation.LeftFace] = new WorldUIBorder {
                location = WorldUIBorderLocation.LeftFace,
                position = new Vector3(-xPositionOffset, 0, 0),
                size = new Vector3(xScale, 1, 1),
            };
            borders[WorldUIBorderLocation.RightFace] = new WorldUIBorder {
                location = WorldUIBorderLocation.RightFace,
                position = new Vector3(xPositionOffset, 0, 0),
                size = new Vector3(xScale, 1, 1),
            };
            borders[WorldUIBorderLocation.FrontFace] = new WorldUIBorder {
                location = WorldUIBorderLocation.FrontFace,
                position = new Vector3(0, 0, zPositionOffset),
                size = new Vector3(1, 1, zScale),
            };
            borders[WorldUIBorderLocation.BackFace] = new WorldUIBorder {
                location = WorldUIBorderLocation.BackFace,
                position = new Vector3(0, 0, -zPositionOffset),
                size = new Vector3(1, 1, zScale),
            };

            /*
                Edges
            */
            borders[WorldUIBorderLocation.TopFrontEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.TopFrontEdge,
                position = new Vector3(0, yPositionOffset, zPositionOffset),
                size = new Vector3(1, yScale, zScale),
            };
            borders[WorldUIBorderLocation.TopLeftEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.TopLeftEdge,
                position = new Vector3(-xPositionOffset, yPositionOffset, 0),
                size = new Vector3(xScale, yScale, 1),
            };
            borders[WorldUIBorderLocation.TopRightEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.TopRightEdge,
                position = new Vector3(xPositionOffset, yPositionOffset, 0),
                size = new Vector3(xScale, yScale, 1),
            };
            borders[WorldUIBorderLocation.TopBackEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.TopBackEdge,
                position = new Vector3(0, yPositionOffset, -zPositionOffset),
                size = new Vector3(1, yScale, zScale),
            };
            borders[WorldUIBorderLocation.BottomFrontEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.BottomFrontEdge,
                position = new Vector3(0, -yPositionOffset, zPositionOffset),
                size = new Vector3(1, yScale, zScale),
            };
            borders[WorldUIBorderLocation.BottomLeftEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.BottomLeftEdge,
                position = new Vector3(-xPositionOffset, -yPositionOffset, 0),
                size = new Vector3(xScale, yScale, 1),
            };
            borders[WorldUIBorderLocation.BottomRightEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.BottomRightEdge,
                position = new Vector3(xPositionOffset, -yPositionOffset, 0),
                size = new Vector3(xScale, yScale, 1),
            };
            borders[WorldUIBorderLocation.BottomBackEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.BottomBackEdge,
                position = new Vector3(0, -yPositionOffset, -zPositionOffset),
                size = new Vector3(1, yScale, zScale),
            };
            borders[WorldUIBorderLocation.MiddleFrontLeftEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.MiddleFrontLeftEdge,
                position = new Vector3(-xPositionOffset, 0, zPositionOffset),
                size = new Vector3(xScale, 1, zScale),
            };
            borders[WorldUIBorderLocation.MiddleFrontRightEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.MiddleFrontRightEdge,
                position = new Vector3(xPositionOffset, 0, zPositionOffset),
                size = new Vector3(xScale, 1, zScale),
            };
            borders[WorldUIBorderLocation.MiddleBackLeftEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.MiddleBackLeftEdge,
                position = new Vector3(-xPositionOffset, 0, -zPositionOffset),
                size = new Vector3(xScale, 1, zScale),
            };
            borders[WorldUIBorderLocation.MiddleBackRightEdge] = new WorldUIBorder {
                location = WorldUIBorderLocation.MiddleBackRightEdge,
                position = new Vector3(xPositionOffset, 0, -zPositionOffset),
                size = new Vector3(xScale, 1, zScale),
            };

            /*
                Corners
            */
            borders[WorldUIBorderLocation.TopFrontLeft] = new WorldUIBorder {
                location = WorldUIBorderLocation.TopFrontLeft,
                position = new Vector3(-xPositionOffset, yPositionOffset, zPositionOffset),
                size = new Vector3(xScale, yScale, zScale),
            };
            borders[WorldUIBorderLocation.TopFrontRight] = new WorldUIBorder {
                location = WorldUIBorderLocation.TopFrontRight,
                position = new Vector3(xPositionOffset, yPositionOffset, zPositionOffset),
                size = new Vector3(xScale, yScale, zScale),
            };
            borders[WorldUIBorderLocation.TopBackLeft] = new WorldUIBorder {
                location = WorldUIBorderLocation.TopBackLeft,
                position = new Vector3(-xPositionOffset, yPositionOffset, -zPositionOffset),
                size = new Vector3(xScale, yScale, zScale),
            };
            borders[WorldUIBorderLocation.TopBackRight] = new WorldUIBorder {
                location = WorldUIBorderLocation.TopBackRight,
                position = new Vector3(xPositionOffset, yPositionOffset, -zPositionOffset),
                size = new Vector3(xScale, yScale, zScale),
            };
            borders[WorldUIBorderLocation.BottomFrontLeft] = new WorldUIBorder {
                location = WorldUIBorderLocation.BottomFrontLeft,
                position = new Vector3(-xPositionOffset, -yPositionOffset, zPositionOffset),
                size = new Vector3(xScale, yScale, zScale),
            };
            borders[WorldUIBorderLocation.BottomFrontRight] = new WorldUIBorder {
                location = WorldUIBorderLocation.BottomFrontRight,
                position = new Vector3(xPositionOffset, -yPositionOffset, zPositionOffset),
                size = new Vector3(xScale, yScale, zScale),
            };
            borders[WorldUIBorderLocation.BottomBackLeft] = new WorldUIBorder {
                location = WorldUIBorderLocation.BottomBackLeft,
                position = new Vector3(-xPositionOffset, -yPositionOffset, -zPositionOffset),
                size = new Vector3(xScale, yScale, zScale),
            };
            borders[WorldUIBorderLocation.BottomBackRight] = new WorldUIBorder {
                location = WorldUIBorderLocation.BottomBackRight,
                position = new Vector3(xPositionOffset, -yPositionOffset, -zPositionOffset),
                size = new Vector3(xScale, yScale, zScale),
            };

            // Update existing borders and update their positions and sizes
            foreach (var location in borderObjects.Keys) {
                WorldUIBorder border = GetBorder(location);
                if (border == null) {
                    Debug.LogWarning($"Border for {location} not found in borders dictionary.");
                    continue;
                }

                GameObject borderObject = borderObjects[location];
                if (borderObject == null) {
                    Debug.LogWarning($"Border object for {location} not found in borderObjects dictionary.");
                    continue;
                }

                borderObject.transform.localPosition = border.position;
                borderObject.transform.localScale = border.size;
            }
        }

        public Dictionary<WorldUIBorderLocation, WorldUIBorder> GetBorders () {
            return borders;
        }

        public WorldUIBorder GetBorder (WorldUIBorderLocation location) {
            if (borders.TryGetValue(location, out WorldUIBorder border)) {
                return border;
            }

            return null;
        }

        public GameObject GetBorderObject (WorldUIBorderLocation location) {
            if (borderObjects.TryGetValue(location, out GameObject borderObject)) {
                return borderObject;
            }

            return null;
        }

        public void CreateBorder (WorldUIBorder border) {
            if (border == null) {
                Debug.LogError("Border is null");
                return;
            }

            // create primitive cube at the border position with the specified size
            GameObject borderObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderObject.name = $"Border_{border.location}";
            borderObject.transform.SetParent(transform);
            borderObject.transform.localPosition = border.position;
            borderObject.transform.localRotation = Quaternion.identity; // Reset rotation to identity
            borderObject.transform.localScale = border.size;
            if (hideChildrenBorders) borderObject.hideFlags = HideFlags.HideInHierarchy;
        }

        public bool HasBorder (WorldUIBorderLocation location) {
            return borderObjects.ContainsKey(location);
        }

        public void RemoveBorder (WorldUIBorderLocation location) {
            if (borderObjects.TryGetValue(location, out GameObject borderObject)) {
                DestroyImmediate(borderObject);
                borderObjects.Remove(location);
            } else {
                Debug.LogWarning($"No border found for location: {location}");
            }
        }
    }

    [CustomEditor(typeof(WorldUIBorders))]
    public class WorldUIBordersEditor : Editor {
        Vector3 lastScale;

        private void OnSceneGUI () {
            // Get the target object we are editing.
            WorldUIBorders meshBorders = (WorldUIBorders)target;

            if (!Vector3.Equals(meshBorders.transform.localScale, lastScale)) {
                meshBorders.CalculateMeshBorders();
                lastScale = meshBorders.transform.localScale;
            }

            // Make sure handles are occluded by other scene geometry.
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

            // Draw each mesh border as a button in the scene view.
            foreach (var border in meshBorders.GetBorders().Values) {
                // Check if the border already exists to avoid drawing it again.
                // If it does not exist, draw the button for creating the border.
                DrawMeshBorder(meshBorders, border);
            }

            // This part is crucial for making the hover effect work on the button itself.
            HandleUtility.Repaint();
        }

        void DrawMeshBorder (WorldUIBorders meshBorders, WorldUIBorder border) {
            // draw top face wire cube button
            Vector3 wireCubePosition = border.position;
            Vector3 wireCubeSize = border.size;
            Vector3 worldPosition = meshBorders.transform.TransformPoint(wireCubePosition);

            float buttonSize = HandleUtility.GetHandleSize(worldPosition) * 0.2f;
            float pickSize = buttonSize;

            // Don't show wire cube if behind the object.
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

            // This will apply position and rotation but ignore the object's scale.
            Matrix4x4 oldMat = Handles.matrix;
            Handles.matrix = Matrix4x4.TRS(meshBorders.transform.localPosition, meshBorders.transform.localRotation, meshBorders.transform.localScale);

            Handles.color = Color.white;
            Handles.DrawWireCube(wireCubePosition, wireCubeSize);
            // Reset the current matrix
            Handles.matrix = oldMat;

            bool hasBorder = meshBorders.HasBorder(border.location);
            if (hasBorder) {
                // Always show when the border exists.
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
                Handles.color = Color.red;
            } else {
                Handles.color = Color.green;
            }

            if (Handles.Button(worldPosition, meshBorders.transform.rotation, buttonSize, pickSize, Handles.CubeHandleCap)) {
                if (hasBorder) {
                    meshBorders.RemoveBorder(border.location);
                } else {
                    meshBorders.CreateBorder(border);
                }
            }
        }
    }
}
