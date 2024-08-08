using System;
using UnityEditor;
using UnityEngine;

namespace MornBeat
{
    [Serializable]
    internal struct MornBeatMeasure
    {
        [SerializeField] internal long MeasureStartSamples;

        public MornBeatMeasure(long measureStartSamples)
        {
            MeasureStartSamples = measureStartSamples;
        }
    }

    [CustomPropertyDrawer(typeof(MornBeatMeasure))]
    internal sealed class MornBeatMeasureDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var measure = property.FindPropertyRelative(nameof(MornBeatMeasure.MeasureStartSamples));
            EditorGUI.PropertyField(position, measure, label);
        }
    }
}