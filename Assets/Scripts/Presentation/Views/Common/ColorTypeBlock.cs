using System;
using Presentation.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

// ReSharper disable InconsistentNaming
namespace Presentation.Views.Common
{
    [Serializable]
    public struct ColorTypeBlock : IEquatable<ColorTypeBlock>
    {
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(ColorTypeBlock), true)]
        public class ColorBlockDrawer : PropertyDrawer
        {
            private const string kNormalColor = "m_NormalColor";
            private const string kNormalAlpha = "m_NormalAlpha";
            private const string kHighlightedColor = "m_HighlightedColor";
            private const string kHighlightedAlpha = "m_HighlightedAlpha";
            private const string kPressedColor = "m_PressedColor";
            private const string kPressedAlpha = "m_PressedAlpha";
            private const string kSelectedColor = "m_SelectedColor";
            private const string kSelectedAlpha = "m_SelectedAlpha";
            private const string kDisabledColor = "m_DisabledColor";
            private const string kDisabledAlpha = "m_DisabledAlpha";
            private const string kFadeDuration = "m_FadeDuration";

            public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
            {
                var drawRect = rect;
                var fullSize = new Vector2(rect.width, EditorGUIUtility.singleLineHeight);
                var colorSize = new Vector2(rect.width / 1.5f, EditorGUIUtility.singleLineHeight);
                var alphaSize = new Vector2(rect.width - colorSize.x - 4f, EditorGUIUtility.singleLineHeight);
                var colorOffset = new Vector2(rect.x, rect.y);
                var alphaOffset = new Vector2(rect.x + colorSize.x + 4f, rect.y);
                var offset = 0f;
                drawRect.height = EditorGUIUtility.singleLineHeight;

                var normalColor = prop.FindPropertyRelative(kNormalColor);
                var normalAlpha = prop.FindPropertyRelative(kNormalAlpha);
                var highlighted = prop.FindPropertyRelative(kHighlightedColor);
                var highlightedAlpha = prop.FindPropertyRelative(kHighlightedAlpha);
                var pressedColor = prop.FindPropertyRelative(kPressedColor);
                var pressedAlpha = prop.FindPropertyRelative(kPressedAlpha);
                var selectedColor = prop.FindPropertyRelative(kSelectedColor);
                var selectedAlpha = prop.FindPropertyRelative(kSelectedAlpha);
                var disabledColor = prop.FindPropertyRelative(kDisabledColor);
                var disabledAlpha = prop.FindPropertyRelative(kDisabledAlpha);
                var fadeDuration = prop.FindPropertyRelative(kFadeDuration);

                EditorGUI.PropertyField(new Rect(colorOffset + offset * Vector2.up, colorSize), normalColor);
                EditorGUI.PropertyField(new Rect(alphaOffset + offset * Vector2.up, alphaSize), normalAlpha, GUIContent.none);
                offset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(new Rect(colorOffset + offset * Vector2.up, colorSize), highlighted);
                EditorGUI.PropertyField(new Rect(alphaOffset + offset * Vector2.up, alphaSize), highlightedAlpha, GUIContent.none);
                offset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(new Rect(colorOffset + offset * Vector2.up, colorSize), pressedColor);
                EditorGUI.PropertyField(new Rect(alphaOffset + offset * Vector2.up, alphaSize), pressedAlpha, GUIContent.none);
                offset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(new Rect(colorOffset + offset * Vector2.up, colorSize), selectedColor);
                EditorGUI.PropertyField(new Rect(alphaOffset + offset * Vector2.up, alphaSize), selectedAlpha, GUIContent.none);
                offset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(new Rect(colorOffset + offset * Vector2.up, colorSize), disabledColor);
                EditorGUI.PropertyField(new Rect(alphaOffset + offset * Vector2.up, alphaSize), disabledAlpha, GUIContent.none);
                offset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(new Rect(colorOffset + offset * Vector2.up, fullSize), fadeDuration, GUIContent.none);
            }

            public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
            {
                return 7 * EditorGUIUtility.singleLineHeight + 6 * EditorGUIUtility.standardVerticalSpacing;
            }

            public override VisualElement CreatePropertyGUI(SerializedProperty property)
            {
                var container = new VisualElement();

                var properties = new[]
                {
                    property.FindPropertyRelative(kNormalColor),
                    property.FindPropertyRelative(kNormalAlpha),
                    property.FindPropertyRelative(kHighlightedColor),
                    property.FindPropertyRelative(kHighlightedAlpha),
                    property.FindPropertyRelative(kPressedColor),
                    property.FindPropertyRelative(kPressedAlpha),
                    property.FindPropertyRelative(kSelectedColor),
                    property.FindPropertyRelative(kSelectedAlpha),
                    property.FindPropertyRelative(kDisabledColor),
                    property.FindPropertyRelative(kDisabledAlpha),
                    property.FindPropertyRelative(kFadeDuration)
                };

                foreach (var prop in properties)
                {
                    var field = new PropertyField(prop);
                    container.Add(field);
                }

                return container;
            }
        }        
#endif
        [SerializeField] private ColorOf m_NormalColor;
        [SerializeField, Range(0f, 1f)] private float m_NormalAlpha;
        [SerializeField] private ColorOf m_HighlightedColor;
        [SerializeField, Range(0f, 1f)] private float m_HighlightedAlpha;
        [SerializeField] private ColorOf m_PressedColor;
        [SerializeField, Range(0f, 1f)] private float m_PressedAlpha;
        [SerializeField] private ColorOf m_SelectedColor;
        [SerializeField, Range(0f, 1f)] private float m_SelectedAlpha;
        [SerializeField] private ColorOf m_DisabledColor;
        [SerializeField, Range(0f, 1f)] private float m_DisabledAlpha;
        [SerializeField] private float m_FadeDuration;
        
        public ColorOf normalColor       { get => m_NormalColor; set => m_NormalColor = value; }
        public float   normalAlpha      { get => m_NormalAlpha; set => m_NormalAlpha = value; }
        
        public ColorOf highlightedColor  { get => m_HighlightedColor; set => m_HighlightedColor = value; }
        public float   highlightedAlpha  { get => m_HighlightedAlpha; set => m_HighlightedAlpha = value; }
        
        public ColorOf pressedColor      { get => m_PressedColor; set => m_PressedColor = value; }
        public float   pressedAlpha      { get => m_PressedAlpha; set => m_PressedAlpha = value; }
        
        public ColorOf selectedColor     { get => m_SelectedColor; set => m_SelectedColor = value; }
        public float   selectedAlpha      { get => m_SelectedAlpha; set => m_SelectedAlpha = value; }
        
        public ColorOf disabledColor     { get => m_DisabledColor; set => m_DisabledColor = value; }
        public float   disabledAlpha      { get => m_DisabledAlpha; set => m_DisabledAlpha = value; }

        /// <summary>
        /// How long a color transition between states should take.
        /// </summary>
        public float fadeDuration      { get => m_FadeDuration; set => m_FadeDuration = value; }

        /// <summary>
        /// Simple getter for a code generated default ColorBlock.
        /// </summary>
        public static ColorTypeBlock defaultColorTypeBlock;

        static ColorTypeBlock()
        {
            defaultColorTypeBlock = new ColorTypeBlock
            {
                m_NormalColor      = ColorOf.Surface,
                m_NormalAlpha      = 1f,
                m_HighlightedColor = ColorOf.Surface,
                m_HighlightedAlpha = 1f,
                m_PressedColor     = ColorOf.Background,
                m_PressedAlpha     = 1f,
                m_SelectedColor    = ColorOf.Surface,
                m_SelectedAlpha    = 1f, 
                m_DisabledColor    = ColorOf.BackTertiary,
                m_DisabledAlpha    = 1f,
                fadeDuration       = 0.1f
            };
        }

        public override bool Equals(object obj)
        {
            return obj is ColorTypeBlock block && Equals(block);
        }

        public bool Equals(ColorTypeBlock other)
        {
            return normalColor == other.normalColor && normalAlpha == other.normalAlpha &&
                highlightedColor == other.highlightedColor && highlightedAlpha == other.highlightedAlpha &&
                pressedColor == other.pressedColor && pressedAlpha == other.pressedAlpha &&
                selectedColor == other.selectedColor && selectedAlpha == other.selectedAlpha &&
                disabledColor == other.disabledColor && disabledAlpha == other.disabledAlpha &&
                fadeDuration == other.fadeDuration;
        }

        public static bool operator==(ColorTypeBlock point1, ColorTypeBlock point2)
        {
            return point1.Equals(point2);
        }

        public static bool operator!=(ColorTypeBlock point1, ColorTypeBlock point2)
        {
            return !point1.Equals(point2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_NormalColor, m_HighlightedColor, m_PressedColor, m_SelectedColor, m_DisabledColor, m_FadeDuration);
        }
    }
}
