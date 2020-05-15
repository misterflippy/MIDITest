using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDITest
{
    public class SplitBuilder
    {
        //TODO: doesn't check for empty values
        public string FileName { get; set; }
        public int NumNotes { get; set; }
        public int MinNoteValue { get; set; }
        public int MaxNoteValue { get; set; }
        public int MinNoteLength { get; set; }
        public int MaxNoteLength { get; set; }
        //base percentage chance to split
        public int SplitChanceBase { get; set; }
        //decay this much every split
        public int SplitChanceDecay { get; set; }
        //public int MinSplitNotes { get; set; }
        //public int MaxSplitNotes { get; set; }

        Random rand = new Random();

        Sequence seq;

        Track mainNotes = new Track();
        Track subNotes = new Track();

        ChannelMessageBuilder channelBuilder = new ChannelMessageBuilder();

        public void Build()
        {
            //create sequence
            seq = new Sequence();

            mainNotes = new Track();
            //subNotes = new Track();

            seq.Add(mainNotes);

            int curTick = 0;

            curTick = GenerateRandomNotes(1, mainNotes, curTick, NumNotes, MinNoteValue, MaxNoteValue, MinNoteLength, MaxNoteLength, SplitChanceBase);

            seq.Save(FileName);
        }

        private int GenerateRandomNotes(int depth, Track track, int curTick, int numNotes, int minNoteValue, int maxNoteValue, int minNoteLength, int maxNoteLength, int splitChance)
        {
            for (int a = 0; a < numNotes; a++)
            {
                curTick = GenerateRandomNote(depth, track, curTick, minNoteValue, maxNoteValue, minNoteLength, maxNoteLength, splitChance);
            }
            return curTick;
        }

        private int GenerateRandomNote(int depth, Track noteTrack, int curTick, int minNoteValue, int maxNoteValue, int minLength, int maxLength, int splitChance)
        {
            int note = rand.Next(minNoteValue, maxNoteValue);
            int noteLength = rand.Next(minLength, maxLength);
            int endTick = curTick + noteLength;

            channelBuilder.Command = ChannelCommand.NoteOn;
            channelBuilder.Data1 = note;
            channelBuilder.Data2 = 127;
            channelBuilder.Build();

            noteTrack.Insert(curTick, channelBuilder.Result);

            //note off
            channelBuilder.Command = ChannelCommand.NoteOff;
            channelBuilder.Data2 = 0;
            channelBuilder.Build();

            noteTrack.Insert(endTick, channelBuilder.Result);

            //check for split notes
            if (rand.Next(100) <= splitChance)
            {
                int numNotes = depth;
                int newLength = noteLength / numNotes;
                GenerateRandomNotes(depth + 1, noteTrack, curTick, numNotes, note - 7, note + 7, newLength, newLength, splitChance - SplitChanceDecay);
            }

            return endTick;
        }
    }
}
