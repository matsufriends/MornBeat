using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MornBeat
{
    public sealed class MornBeatScoreExportEditorWindow : EditorWindow
    {
        [SerializeField] private Object _beatMemo;
        [SerializeField] private Object _converter;
        private IMornBeatActionSettingSo _cachedBeatSettings;
        private MornBeatConverterBase _cachedConverter;

        [MenuItem("MornBeat/Score Exporter")]
        public static void ShowWindow()
        {
            GetWindow<MornBeatScoreExportEditorWindow>();
        }

        private void OnGUI()
        {
            _beatMemo = EditorGUILayout.ObjectField("BeatMemo", _beatMemo, typeof(ScriptableObject), false);
            _converter = EditorGUILayout.ObjectField("Converter", _converter, typeof(MornBeatConverterBase), false);
            try
            {
                _cachedBeatSettings = (IMornBeatActionSettingSo)_beatMemo;
            }
            catch
            {
                _beatMemo = null;
                _cachedBeatSettings = null;
                return;
            }

            try
            {
                _cachedConverter = (MornBeatConverterBase)_converter;
            }
            catch
            {
                _converter = null;
                _cachedConverter = null;
                return;
            }

            if (_cachedBeatSettings == null || _cachedConverter == null)
            {
                return;
            }

            if (GUILayout.Button("Export"))
            {
                MornBeatUtil.Log("Export Start");
                var dict = _cachedBeatSettings.GetDictionary();
                var measureTick = _cachedBeatSettings.MeasureTick;
                var sb = new StringBuilder();
                var measure = 0;
                for (var i = 0; i < 10000 && dict.Count > 0; i++)
                {
                    if (measure > 0)
                    {
                        sb.Append('\n');
                    }

                    for (var index = 0; index < measureTick; index++)
                    {
                        var tick = measure * measureTick + index;
                        if (dict.TryGetValue(tick, out var value))
                        {
                            sb.Append(_cachedConverter.ConvertToText(value));
                            dict.Remove(tick);
                        }
                        else
                        {
                            sb.Append(_cachedConverter.ConvertToText(0));
                        }
                    }

                    measure++;
                }

                var text = sb.ToString();
                var path = AssetDatabase.GetAssetPath(_beatMemo);
                path = path.Substring(0, path.LastIndexOf('.')) + "_Score.txt";
                File.WriteAllText(path, text);
                AssetDatabase.Refresh();
                MornBeatUtil.Log("Export Success");
            }
        }
    }
}