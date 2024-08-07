﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MornBeat
{
    public abstract class MornBeatConverterBase : ScriptableObject
    {
        protected abstract (int, char)[] ConvertArray { get; }

        public Dictionary<int, MornBeatAction<TEnum>> GetDictionary<TEnum>(TextAsset textAsset) where TEnum : Enum
        {
            var dictionary = new Dictionary<int, MornBeatAction<TEnum>>();
            var lines = textAsset.text.Split('\n');
            var tick = 0;
            for (var measure = 0; measure < lines.Length; measure++)
            {
                var text = lines[measure];

                // IndexとLengthを無視
                var tmpLineNotes = new List<MornBeatAction<TEnum>>();
                for (var index = 0; index < text.Length; index++)
                {
                    var c = text[index];
                    if (c == MornBeatUtil.OpenSplit)
                    {
                        var endIndex = text.IndexOf(MornBeatUtil.CloseSplit, index);
                        if (endIndex == -1)
                        {
                            MornBeatUtil.LogWarning("閉じられていません。");
                            break;
                        }

                        var lengthText = text.Substring(index + 1, endIndex - index - 1);
                        var flag = 0;
                        foreach (var c2 in lengthText)
                        {
                            flag |= ConvertToNoteType(c2);
                        }

                        tmpLineNotes.Add(new MornBeatAction<TEnum>(measure, -1, (TEnum)(flag as object)));
                    }
                    else
                    {
                        var noteType = ConvertToNoteType(c);
                        tmpLineNotes.Add(new MornBeatAction<TEnum>(measure, -1, (TEnum)(noteType as object)));
                    }
                }

                // IndexとLengthを再計算
                var length = tmpLineNotes.Count;
                for (var i = 0; i < length; i++)
                {
                    var note = tmpLineNotes[i];
                    if ((int)(object)note.BeatActionType != 0)
                    {
                        var newNote = new MornBeatAction<TEnum>(note.Measure, i, note.BeatActionType);
                        dictionary.Add(tick, newNote);
                    }

                    tick++;
                }
            }

            return dictionary;
        }

        private int ConvertToNoteType(char c)
        {
            foreach (var pair in ConvertArray)
            {
                if (pair.Item2 == c)
                {
                    return pair.Item1;
                }
            }

            return 0;
        }

        public string ConvertToText(int noteType)
        {
            var list = new List<char>();
            foreach (var pair in ConvertArray)
            {
                if (noteType.BitHas(pair.Item1))
                {
                    list.Add(pair.Item2);
                }
            }

            if (list.Count == 0)
            {
                return "0";
            }

            if (list.Count == 1)
            {
                return list[0].ToString();
            }

            var sb = new StringBuilder();
            sb.Append(MornBeatUtil.OpenSplit);
            foreach (var c in list)
            {
                sb.Append(c);
            }

            sb.Append(MornBeatUtil.CloseSplit);
            return sb.ToString();
        }
    }
}