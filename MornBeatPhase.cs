using System;
using UnityEditor;
using UnityEngine;

namespace MornBeat
{
    internal enum BpmType
    {
        Constant,
        Lerp,
    }

    [Serializable]
    internal class MornBeatPhase
    {
        public double StartBpm;
        public double EndBpm;
        public BpmType BpmType;
        public int Beat;
        public int BeatType;
        public int Measure;
    }

    [CustomPropertyDrawer(typeof(MornBeatPhase))]
    internal class BpmAndTimeInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            // Bpmを表示
            {
                // Property
                var startBpm = property.FindPropertyRelative(nameof(MornBeatPhase.StartBpm));
                var endBpm = property.FindPropertyRelative(nameof(MornBeatPhase.EndBpm));
                var bpmType = property.FindPropertyRelative(nameof(MornBeatPhase.BpmType));

                // Rect
                var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                labelRect = EditorGUI.PrefixLabel(labelRect, new GUIContent("Bpm"));
                var valueRect = new Rect(labelRect.x, labelRect.y, labelRect.width - 20, labelRect.height);
                var bpmTypeRect = new Rect(labelRect.x + labelRect.width - 20, labelRect.y, 20, labelRect.height);
                switch (bpmType.enumValueIndex)
                {
                    case (int)BpmType.Constant:
                        EditorGUI.PropertyField(valueRect, startBpm, GUIContent.none);
                        endBpm.doubleValue = startBpm.doubleValue;
                        break;
                    case (int)BpmType.Lerp:
                        // Value
                        var valueY = valueRect.y;
                        var valueWidth = valueRect.width / 2 - 20;
                        var valueHeight = valueRect.height;

                        // Rect
                        var valueRectL = new Rect(valueRect.x, valueY, valueWidth, valueHeight);
                        var valueBarRect = new Rect(valueRectL.x + valueWidth, valueY, 20, valueHeight);
                        var valueRectR = new Rect(valueBarRect.x + 20, valueY, valueWidth, valueHeight);

                        // Draw
                        EditorGUI.PropertyField(valueRectL, startBpm, GUIContent.none);
                        EditorGUI.LabelField(valueBarRect, "  -");
                        EditorGUI.PropertyField(valueRectR, endBpm, GUIContent.none);
                        break;
                }

                EditorGUI.PropertyField(bpmTypeRect, bpmType, GUIContent.none);
            }
            // Beatを表示
            {
                // Property
                var beat = property.FindPropertyRelative(nameof(MornBeatPhase.Beat));
                var beatType = property.FindPropertyRelative(nameof(MornBeatPhase.BeatType));
                var measureCount = property.FindPropertyRelative(nameof(MornBeatPhase.Measure));

                // Rect
                var labelRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                labelRect = EditorGUI.PrefixLabel(labelRect, new GUIContent("Beat"));

                // Value
                var valueY = labelRect.y;
                var valueWidth = labelRect.width / 3 - 20;
                var valueHeight = labelRect.height;

                // Rect
                var beatValueRect = new Rect(labelRect.x, valueY, valueWidth, valueHeight);
                var divideRect = new Rect(beatValueRect.x + valueWidth, valueY, 20, valueHeight);
                var beatBaseValueRect = new Rect(divideRect.x + 20, valueY, valueWidth, valueHeight);
                var multiRect = new Rect(beatBaseValueRect.x + valueWidth, valueY, 20, valueHeight);
                var measureCountValueRect = new Rect(multiRect.x + 20, valueY, valueWidth, valueHeight);

                // Draw
                EditorGUI.PropertyField(beatValueRect, beat, GUIContent.none);
                EditorGUI.LabelField(divideRect, "  /");
                EditorGUI.PropertyField(beatBaseValueRect, beatType, GUIContent.none);
                EditorGUI.LabelField(multiRect, "  *");
                EditorGUI.PropertyField(measureCountValueRect, measureCount, GUIContent.none);
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}