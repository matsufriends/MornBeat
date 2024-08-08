using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MornBeat
{
    [CreateAssetMenu(menuName = "MornLib/" + nameof(MornBeatMusic))]
    public sealed class MornBeatMusic : ScriptableObject
    {
        [SerializeField] internal AudioClip MusicClip;
        [SerializeField] internal List<MornBeatMeasure> MeasureList;
        [SerializeField] internal List<MornBeatPhase> BpmList;
        [SerializeField] internal List<double> TimeList;
    }

    [CustomEditor(typeof(MornBeatMusic))]
    internal sealed class MornBeatMusicEditor : Editor
    {
        private bool _foldOutGenerateFromBpm;
        private bool _foldOutGenerateFromTime;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var music = (MornBeatMusic)target;
            { // AudioClipの表示
                var musicClip = serializedObject.FindProperty(nameof(MornBeatMusic.MusicClip));
                EditorGUILayout.PropertyField(musicClip);
                if (musicClip.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("MusicClip is required.", MessageType.Error);
                    return;
                }

                EditorGUI.indentLevel++;
                var audioClip = (AudioClip)musicClip.objectReferenceValue;
                EditorGUILayout.LabelField("Frequency:", audioClip.frequency.ToString());
                EditorGUILayout.LabelField("Samples", audioClip.samples.ToString());
                EditorGUI.indentLevel--;
            }
            { // 小節情報の可視化
                var measureList = serializedObject.FindProperty(nameof(MornBeatMusic.MeasureList));
                GUI.enabled = false;
                EditorGUILayout.PropertyField(measureList);
                GUI.enabled = true;
            }
            { // 自動生成:BPMから
                _foldOutGenerateFromBpm = EditorGUILayout.Foldout(_foldOutGenerateFromBpm, "Generate From Bpm");
                EditorGUI.indentLevel++;
                if (_foldOutGenerateFromBpm)
                {
                    var bpmList = serializedObject.FindProperty(nameof(MornBeatMusic.BpmList));
                    EditorGUILayout.PropertyField(bpmList);
                    if (GUILayout.Button("Generate From Bpm"))
                    {
                        var isSuccess = true;
                        MornBeatUtil.Log("Generate Start");
                        var result = new List<MornBeatMeasure>();
                        result.Add(new MornBeatMeasure(0));
                        var frequency = music.MusicClip.frequency;
                        var virtualTime = 0d;
                        const double deltaBeat = 0.000001d;
                        foreach (var phase in music.BpmList)
                        {
                            if (phase.StartBpm == 0)
                            {
                                isSuccess = false;
                                MornBeatUtil.LogError("StartBpm is 0");
                                break;
                            }

                            if (phase.EndBpm == 0)
                            {
                                isSuccess = false;
                                MornBeatUtil.LogError("EndBpm is 0");
                                break;
                            }

                            if (phase.Beat == 0)
                            {
                                isSuccess = false;
                                MornBeatUtil.LogError("BeatCount is 0");
                                break;
                            }

                            if (phase.BeatType == 0)
                            {
                                isSuccess = false;
                                MornBeatUtil.LogError("BeatType is 0");
                                break;
                            }

                            if (phase.Measure == 0)
                            {
                                isSuccess = false;
                                MornBeatUtil.LogError("MeasureCount is 0");
                                break;
                            }

                            var startBpm = phase.StartBpm;
                            var endBpm = phase.EndBpm;
                            var difBpm = endBpm - startBpm;
                            var phaseBeat = 0d;

                            // 1Beat = 4分音符１つ分
                            // n/m 拍子のとき
                            //      sum = n / m * 4 * 小節数
                            var phaseMeasureBeat = 4d * phase.Beat / phase.BeatType;
                            var phaseTotalBeat = phaseMeasureBeat * phase.Measure;

                            // 小節またぎ判定Beat
                            var nextPhaseBeat = phaseMeasureBeat;
                            for (var i = 0; i < phase.Measure; i++)
                            {
                                while (phaseBeat < nextPhaseBeat)
                                {
                                    // lerp
                                    var currentBpm = startBpm + difBpm * (phaseBeat / phaseTotalBeat);

                                    // BPM = 1小節の4分音符の数
                                    // 1Beatでの経過時間 = 60 / BPM
                                    virtualTime += 60d / currentBpm * deltaBeat;
                                    phaseBeat += deltaBeat;
                                }

                                result.Add(new MornBeatMeasure((long)(virtualTime * frequency)));
                                nextPhaseBeat += phaseMeasureBeat;
                            }
                        }

                        music.MeasureList.Clear();
                        music.MeasureList.AddRange(result);
                        if (isSuccess)
                        {
                            MornBeatUtil.Log("Generate Success");
                        }
                        else
                        {
                            MornBeatUtil.LogError("Generate Failed. Please check the log.");
                        }
                    }
                }

                EditorGUI.indentLevel--;
            }
            { // 自動生成:時間から
                _foldOutGenerateFromTime = EditorGUILayout.Foldout(_foldOutGenerateFromTime, "Generate From Time");
                EditorGUI.indentLevel++;
                if (_foldOutGenerateFromTime)
                {
                    var timeList = serializedObject.FindProperty(nameof(MornBeatMusic.TimeList));
                    EditorGUILayout.PropertyField(timeList);
                    if (GUILayout.Button("Generate From Time"))
                    {
                        MornBeatUtil.Log("Generate Start");
                        var result = new List<MornBeatMeasure>();
                        result.Add(new MornBeatMeasure(0));
                        var frequency = music.MusicClip.frequency;
                        foreach (var time in music.TimeList)
                        {
                            result.Add(new MornBeatMeasure((long)(time * frequency)));
                        }

                        music.MeasureList.Clear();
                        music.MeasureList.AddRange(result);
                        MornBeatUtil.Log("Generate End");
                    }
                }

                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}