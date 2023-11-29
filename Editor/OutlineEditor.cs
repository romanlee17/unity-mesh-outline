namespace romanlee17.MeshOutlineEditor {
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using romanlee17.MeshOutline;

    [CustomEditor(typeof(Outline))]
    internal class OutlineEditor : Editor {

        // Mask material path.
        private const string _packageMaskPath = "Packages/unity-mesh-outline/Runtime/Materials/OutlineMask.mat";
        private const string _assetsMaskPath = "Assets/unity-mesh-outline/Runtime/Materials/OutlineMask.mat";

        // Fill material path.
        private const string _packageFillPath = "Packages/unity-mesh-outline/Runtime/Materials/OutlineFill.mat";
        private const string _assetsFillPath = "Assets/unity-mesh-outline/Runtime/Materials/OutlineFill.mat";

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            Outline outline = (Outline)target;
            if (outline.Created) {
                if (GUILayout.Button(outline.Visible ? "Hide" : "Show")) {
                    outline.Visible = !outline.Visible;
                    MarkChanges();
                }
            }
            if (GUILayout.Button(outline.Created ? "Recalculate" : "Create")) {
                Material outlineMask = LoadMaterialAt(_packageMaskPath, _assetsMaskPath);
                Material outlineFill = LoadMaterialAt(_packageFillPath, _assetsFillPath);
                if (outline.Created) {
                    outline.Recalculate(outlineMask, outlineFill);
                    MarkChanges();
                    return;
                }
                outline.Create(outlineMask, outlineFill);
                MarkChanges();
            }
            if (!outline.Created) {
                EditorGUILayout.HelpBox("Create outline object first!", MessageType.Warning);
            }
        }

        private Material LoadMaterialAt(string packagePath, string assetsPath) {
            // Release path.
            Material material = AssetDatabase.LoadAssetAtPath<Material>(packagePath);
            if (material != null) {
                return material;
            }
            // Development path.
            material = AssetDatabase.LoadAssetAtPath<Material>(assetsPath);
            return material;
        }

        private void MarkChanges() {
            EditorSceneManager.MarkSceneDirty(
                EditorSceneManager.GetActiveScene()
            );
        }

    }
}