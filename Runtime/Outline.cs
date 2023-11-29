namespace romanlee17.MeshOutline {
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class Outline : MonoBehaviour {

        [SerializeField, HideInInspector] GameObject _outlineObject = null;
        [SerializeField, HideInInspector] MeshFilter _outlineFilter = null;
        [SerializeField, HideInInspector] MeshRenderer _outlineRenderer = null;

        /// <summary>
        /// Returns true if outline has been created.
        /// </summary>
        internal bool Created {
            get { return _outlineObject != null; }
        }

        /// <summary>
        /// Defines the visibility of outline by boolean value.
        /// </summary>
        public bool Visible {
            get { return _outlineObject.activeSelf; }
            set { _outlineObject.SetActive(value); }
        }

        private MeshFilter MeshFilter {
            get => _meshFilter ? _meshFilter :
                _meshFilter = GetComponent<MeshFilter>();
        }
        private MeshFilter _meshFilter = null;

        // Functions:

        /// <summary>
        /// Creates a new child and assigns to it generated outline mesh with shaders.
        /// </summary>
        internal void Create(Material outlineMask, Material outlineFill) {
            _outlineObject = new GameObject("Outline");
            _outlineObject.transform.SetParent(transform);
            _outlineObject.transform.localPosition = Vector3.zero;
            _outlineObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _outlineObject.transform.localScale = Vector3.one;
            _outlineFilter = _outlineObject.AddComponent<MeshFilter>();
            Mesh origin = MeshFilter.sharedMesh;
            Mesh outline = new() {
                vertices = origin.vertices,
                triangles = origin.triangles
            };
            outline.RecalculateBounds();
            outline.RecalculateNormals();
            outline.RecalculateTangents();
            outline.SetUVs(
                channel: 3,
                CalculateNormals(origin)
            );
            outline.name = "Outline (Procedural)";
            _outlineFilter.sharedMesh = outline;
            _outlineRenderer = _outlineObject.AddComponent<MeshRenderer>();
            _outlineRenderer.materials = new Material[] {
                outlineMask,
                outlineFill
            };
        }

        /// <summary>
        /// Recalculates mesh in previously created outline object.
        /// </summary>
        internal void Recalculate(Material outlineMask, Material outlineFill) {
            Mesh origin = MeshFilter.sharedMesh;
            Mesh outline = new() {
                vertices = origin.vertices,
                triangles = origin.triangles
            };
            outline.RecalculateBounds();
            outline.RecalculateNormals();
            outline.RecalculateTangents();
            outline.SetUVs(
                channel: 3,
                CalculateNormals(origin)
            );
            outline.name = "Outline (Procedural)";
            _outlineFilter.sharedMesh = outline;
            // Reassign materials.
            _outlineRenderer.materials = new Material[] {
                outlineMask,
                outlineFill
            };
        }

        /// <summary>
        /// Corrects normals in copy of original mesh for outline shader.
        /// </summary>
        private List<Vector3> CalculateNormals(Mesh mesh) {
            var groups = mesh.vertices.Select(
                (vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)
            ).GroupBy(pair => pair.Key);
            List<Vector3> smoothNormals = new(mesh.normals);
            foreach (var group in groups) {
                if (group.Count() == 1) continue;
                Vector3 smoothNormal = Vector3.zero;
                foreach (var pair in group) {
                    smoothNormal += mesh.normals[pair.Value];
                }
                smoothNormal.Normalize();
                foreach (var pair in group) {
                    smoothNormals[pair.Value] = smoothNormal;
                }
            }
            return smoothNormals;
        }

    }
}