using System.Collections.Generic;
using UnityEngine;

namespace MornBeat
{
    public abstract class MornBeatConverterBase : ScriptableObject
    {
        public List<MornBeatNote> ConvertToNotes(TextAsset textAsset)
        {
            var list = new List<MornBeatNote>();
            var lines = textAsset.text.Split('\n');
            for (var measure = 0; measure < lines.Length; measure++)
            {
                var text = lines[measure];
                var length = text.Length;
                for (var index = 0; index < text.Length; index++)
                {
                    var noteType = ConvertToNoteType(text[index]);
                    var note = new MornBeatNote(measure, index, length, noteType);
                    list.Add(note);
                }
            }

            return list;
        }

        public abstract int ConvertToNoteType(char c);
        public abstract char ConvertToChar(int noteType);
    }
}