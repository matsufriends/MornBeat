using System.IO;
using UnityEditor;
using UnityEngine;

namespace MornBeat
{
    [CreateAssetMenu(menuName = "MornBeat/MornBeatScore")]
    public sealed class MornBeatScore : ScriptableObject
    {
        [SerializeField] internal TextAsset ScoreText;
        [SerializeField] internal MornBeatConverterBase Converter;
    }

    [CustomEditor(typeof(MornBeatScore))]
    public sealed class MornBeatScoreEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var score = (MornBeatScore)target;
            if (score.ScoreText == null)
            {
                if (GUILayout.Button("Generate TextAsset"))
                {
                    var path = AssetDatabase.GetAssetPath(score);
                    path = path.Substring(0, path.LastIndexOf('.')) + ".txt";
                    File.WriteAllText(path, "");
                    AssetDatabase.Refresh();
                    score.ScoreText = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                }
            }
        }
    }
}