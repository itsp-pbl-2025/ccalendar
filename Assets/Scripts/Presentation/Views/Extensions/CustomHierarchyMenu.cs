#if UNITY_EDITOR
using Presentation.Utilities;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Presentation.Views.Extensions
{
    public static class CustomHierarchyMenu
    {
        #region ImageRx

        private static bool MayReplacingImage()
        {
            return Selection.activeGameObject && Selection.activeGameObject.GetComponent<Image>() && !Selection.activeGameObject.GetComponent<ImageRx>();
        }

        [MenuItem("GameObject/UI/Image Theme Reactive", true, 1001)]
        public static bool ValidateAddImageRx()
        {
            return !MayReplacingImage();
        }
        
        [MenuItem("GameObject/UI/Image Theme Reactive", false, 1001)]
        public static void AddImageRx()
        {
            var selectedGameObject = Selection.activeGameObject;
            if (!selectedGameObject)
            {
                selectedGameObject = FindCanvasForUIObject().gameObject;
            }
            
            var imageRxNew = new GameObject("ImageRx", typeof(RectTransform), typeof(CanvasRenderer), typeof(ImageRx));
            
            var rectTransform = imageRxNew.GetComponent<RectTransform>();
            rectTransform.SetParent(selectedGameObject.transform, false);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(100, 100);

            var imageRx = imageRxNew.GetComponent<ImageRx>();
            imageRx.colorType = ColorOf.Background;

            Selection.activeGameObject = imageRxNew;
            Undo.RegisterCreatedObjectUndo(imageRxNew, "Create Image Rx");
        }

        [MenuItem("GameObject/UI/Replace Image To Reactive", true, 1002)]
        public static bool ValidateReplaceImageToReactive()
        {
            return MayReplacingImage();
        }

        // メニュー項目がクリックされたときに実行されるメソッド
        [MenuItem("GameObject/UI/Replace Image To Reactive", false, 1002)]
        public static void ReplaceImageToReactive()
        {
            var selectedGameObject = Selection.activeGameObject;
            if (!selectedGameObject)
            {
                Debug.LogWarning("No GameObject selected for Image replacement.");
                return;
            }

            if (!selectedGameObject.TryGetComponent<Image>(out var oldImage))
            {
                Debug.LogWarning($"Selected GameObject '{selectedGameObject.name}' does not have an Image component.");
                return;
            }

            Undo.RegisterCompleteObjectUndo(selectedGameObject, "Replace Image with ImageRx");
            
            // 元のImageコンポーネントを削除して、ReactiveImageコンポーネントを追加する
            var serialize = JsonUtility.ToJson(oldImage);
            Object.DestroyImmediate(oldImage);
            var imageRx = selectedGameObject.AddComponent<ImageRx>();
            JsonUtility.FromJsonOverwrite(serialize, imageRx);

            imageRx.colorType = AssetInEditor.Theme.FindNearestColorType(imageRx.color);

            EditorUtility.SetDirty(selectedGameObject);
        }

        #endregion
        
        #region LabelRx

        private static bool MayReplacingLabel()
        {
            return Selection.activeGameObject && Selection.activeGameObject.GetComponent<TextMeshProUGUI>() && !Selection.activeGameObject.GetComponent<LabelRx>();
        }

        [MenuItem("GameObject/UI/Label Theme Reactive", true, 1011)]
        public static bool ValidateAddLabelRx()
        {
            return !MayReplacingLabel();
        }

        [MenuItem("GameObject/UI/Label Theme Reactive", false, 1011)]
        public static void AddLabelRx()
        {
            var selectedGameObject = Selection.activeGameObject;
            if (!selectedGameObject)
            {
                selectedGameObject = FindCanvasForUIObject().gameObject;
            }
            
            var labelRxObj = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(LabelRx));
            
            var rectTransform = labelRxObj.GetComponent<RectTransform>();
            rectTransform.SetParent(selectedGameObject.transform, false);
            rectTransform.anchoredPosition = Vector2.zero;

            var labelRx = labelRxObj.GetComponent<LabelRx>();
            
            labelRx.colorType = ColorOf.TextDefault;
            labelRx.text = "Hello, Onsche!";
            
            labelRx.fontSize = TMP_Settings.defaultFontSize;
            labelRx.font = TMP_Settings.defaultFontAsset;
            labelRx.alignment = TextAlignmentOptions.Center;
            
            if (TMP_Settings.autoSizeTextContainer)
            {
                var size = labelRx.GetPreferredValues(TMP_Math.FLOAT_MAX, TMP_Math.FLOAT_MAX);
                labelRx.rectTransform.sizeDelta = size;
            }
            else
            {
                rectTransform.sizeDelta = TMP_Settings.defaultTextMeshProUITextContainerSize;
            }

            Selection.activeGameObject = labelRxObj;
            Undo.RegisterCreatedObjectUndo(labelRxObj, "Create Label Rx");
        }

        [MenuItem("GameObject/UI/Replace Label To Reactive", true, 1012)]
        public static bool ValidateReplaceLabelRx()
        {
            return MayReplacingLabel();
        }

        [MenuItem("GameObject/UI/Replace Label To Reactive", false, 1012)]
        public static void ReplaceLabelRx()
        {
            var selectedGameObject = Selection.activeGameObject;
            if (!selectedGameObject)
            {
                Debug.LogWarning("No GameObject selected for Label replacement.");
                return;
            }

            if (!selectedGameObject.TryGetComponent<TextMeshProUGUI>(out var oldLabel))
            {
                Debug.LogWarning($"Selected GameObject '{selectedGameObject.name}' does not have an Label component.");
                return;
            }

            Undo.RegisterCompleteObjectUndo(selectedGameObject, "Replace Label with LabelRx");
            
            // 元のImageコンポーネントを削除して、ReactiveImageコンポーネントを追加する
            var serialize = JsonUtility.ToJson(oldLabel);
            Object.DestroyImmediate(oldLabel);
            var labelRx = selectedGameObject.AddComponent<LabelRx>();
            JsonUtility.FromJsonOverwrite(serialize, labelRx);
            
            labelRx.colorType = AssetInEditor.Theme.FindNearestColorType(labelRx.color);

            EditorUtility.SetDirty(selectedGameObject);
        }
        
        #endregion

        #region ScalableScrollRect

        private static bool MayReplacingScrollRect()
        {
            return Selection.activeGameObject && Selection.activeGameObject.GetComponent<ScrollRect>() && !Selection.activeGameObject.GetComponent<ScalableScrollRect>();
        }

        [MenuItem("GameObject/UI/Replace ScrollRect As Scalable", true, 1031)]
        public static bool ValidateReplaceScrollRect()
        {
            return !MayReplacingLabel();
        }

        [MenuItem("GameObject/UI/Replace ScrollRect As Scalable", false, 1031)]
        public static void ReplaceScrollRect()
        {
            var selectedGameObject = Selection.activeGameObject;
            if (!selectedGameObject)
            {
                Debug.LogWarning("No GameObject selected for ScrollRect replacement.");
                return;
            }

            if (!selectedGameObject.TryGetComponent<ScrollRect>(out var oldScrollRect))
            {
                Debug.LogWarning($"Selected GameObject '{selectedGameObject.name}' does not have an ScrollRect component.");
                return;
            }

            Undo.RegisterCompleteObjectUndo(selectedGameObject, "Replace ScrollRect with ScalableScrollRect");
            
            // 元のImageコンポーネントを削除して、ReactiveImageコンポーネントを追加する
            var serialize = JsonUtility.ToJson(oldScrollRect);
            Object.DestroyImmediate(oldScrollRect);
            var labelRx = selectedGameObject.AddComponent<ScalableScrollRect>();
            JsonUtility.FromJsonOverwrite(serialize, labelRx);

            EditorUtility.SetDirty(selectedGameObject);
        }

        #endregion
        
        private static Canvas FindCanvasForUIObject()
        {
            var canvas = Object.FindAnyObjectByType<Canvas>();
            if (!canvas)
            {
                // UnityのデフォルトUIオブジェクト生成パスを使用してCanvasを作成
                var canvasObj = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Frames/Common/AutoAspectCanvas.prefab"));
                canvas = canvasObj.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera; // デフォルトはOverlay

                Debug.Log("No Canvas found. A new Canvas have been created.");
            }

            return canvas;
        }
        
        [MenuItem("Tools/OnscheUI/Fix RegularPrefab RxAlpha", false, 50)]
        public static void FixImageRxLabelRxAlphaOverrides()
        {
            var guids = AssetDatabase.FindAssets("t:GameObject");
            var initializedCount = 0;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (PrefabUtility.GetPrefabAssetType(prefabAsset) != PrefabAssetType.Regular) continue;
                
                var prefabRoot = PrefabUtility.LoadPrefabContents(path);

                var changed = false;
                var imageRxs = prefabRoot.GetComponentsInChildren<ImageRx>(true);
                foreach (var imageRx in imageRxs)
                {
                    var serializedImageRx = new SerializedObject(imageRx);
                    var mAlphaProp = serializedImageRx.FindProperty("mAlpha");

                    if (mAlphaProp != null)
                    {
                        mAlphaProp.floatValue = mAlphaProp.floatValue; // 値を.prefabファイルに明示的に書き込む
                        serializedImageRx.ApplyModifiedProperties();
                        changed = true;
                        Debug.Log($"Initialized mAlpha to 1f for ImageRx on original prefab: {prefabAsset.name} (Component: {imageRx.name})");
                    }
                    else
                    {
                        Debug.LogWarning($"mAlpha property not found on ImageRx component of prefab: {prefabAsset.name} (Component: {imageRx.name}). Check field name.");
                    }
                }

                // Prefab の変更を保存
                if (changed)
                {
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                    initializedCount++;
                }

                // Prefab の内容をアンロード
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }

            if (initializedCount > 0)
            {
                AssetDatabase.SaveAssets(); // すべての変更をアセットデータベースに保存
                Debug.Log($"Finished initializing mAlpha to 1f for {initializedCount} Original Prefabs.");
            }
            else
            {
                Debug.Log("No Original Prefabs found needing mAlpha initialization, or all were already 1f.");
            }
        }
    }
}
#endif