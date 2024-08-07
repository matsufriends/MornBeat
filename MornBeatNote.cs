using System;

namespace MornBeat
{
    [Serializable]
    public readonly struct MornBeatNote
    {
        public readonly int Measure;
        public readonly int Index;
        public readonly int Length;
        public readonly int NoteType;

        public MornBeatNote(int measure, int index, int length, int noteType)
        {
            Measure = measure;
            Index = index;
            Length = length;
            NoteType = noteType;
        }
    }
}