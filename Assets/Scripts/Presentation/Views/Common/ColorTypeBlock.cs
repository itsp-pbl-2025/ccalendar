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
            private const string kHighlightedColor = "m_HighlightedColor";
            private const string kPressedColor = "m_PressedColor";
            private const string kSelectedColor = "m_SelectedColor";
            private const string kDisabledColor = "m_DisabledColor";
            private const string kFadeDuration = "m_FadeDuration";

            public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
            {
                var drawRect = rect;
                drawRect.height = EditorGUIUtility.singleLineHeight;

                var normalColor = prop.FindPropertyRelative(kNormalColor);
                var highlighted = prop.FindPropertyRelative(kHighlightedColor);
                var pressedColor = prop.FindPropertyRelative(kPressedColor);
                var selectedColor = prop.FindPropertyRelative(kSelectedColor);
                var disabledColor = prop.FindPropertyRelative(kDisabledColor);
                var fadeDuration = prop.FindPropertyRelative(kFadeDuration);

                EditorGUI.PropertyField(drawRect, normalColor);
                drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(drawRect, highlighted);
                drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(drawRect, pressedColor);
                drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(drawRect, selectedColor);
                drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(drawRect, disabledColor);
                drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(drawRect, fadeDuration);
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
                    property.FindPropertyRelative(kHighlightedColor),
                    property.FindPropertyRelative(kPressedColor),
                    property.FindPropertyRelative(kSelectedColor),
                    property.FindPropertyRelative(kDisabledColor),
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
        [FormerlySerializedAs("normalColor")]
        [SerializeField]
        private ColorOf m_NormalColor;

        [FormerlySerializedAs("highlightedColor")]
        [SerializeField]
        private ColorOf m_HighlightedColor;

        [FormerlySerializedAs("pressedColor")]
        [SerializeField]
        private ColorOf m_PressedColor;

        [FormerlySerializedAs("m_HighlightedColor")]
        [SerializeField]
        private ColorOf m_SelectedColor;

        [FormerlySerializedAs("disabledColor")]
        [SerializeField]
        private ColorOf m_DisabledColor;

        [FormerlySerializedAs("fadeDuration")]
        [SerializeField]
        private float m_FadeDuration;
        
        public ColorOf normalColor       { get => m_NormalColor; set => m_NormalColor = value; }
        
        public ColorOf highlightedColor  { get => m_HighlightedColor; set => m_HighlightedColor = value; }
        
        public ColorOf pressedColor      { get => m_PressedColor; set => m_PressedColor = value; }
        
        public ColorOf selectedColor     { get => m_SelectedColor; set => m_SelectedColor = value; }
        
        public ColorOf disabledColor     { get => m_DisabledColor; set => m_DisabledColor = value; }

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
                m_HighlightedColor = ColorOf.Surface,
                m_PressedColor     = ColorOf.Background,
                m_SelectedColor    = ColorOf.Surface,
                m_DisabledColor    = ColorOf.BackTertiary,
                fadeDuration       = 0.1f
            };
        }

        public override bool Equals(object obj)
        {
            return obj is ColorTypeBlock block && Equals(block);
        }

        public bool Equals(ColorTypeBlock other)
        {
            return normalColor == other.normalColor &&
                highlightedColor == other.highlightedColor &&
                pressedColor == other.pressedColor &&
                selectedColor == other.selectedColor &&
                disabledColor == other.disabledColor &&
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
            return base.GetHashCode();
        }
    }
}
