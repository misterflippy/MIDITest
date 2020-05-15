using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDITest
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Test2();
            //BuildTestFile();
            //CodeFromThread();
            //BuildRandomMidiFile();

            BuildWithSplitBuilder();
        }

        private static void BuildWithSplitBuilder()
        {
 	        SplitBuilder builder = new SplitBuilder(){
                FileName = "Split.mid",
                NumNotes = 50,
                MinNoteValue = 24,      //c2
                MaxNoteValue = 72,      //c6
                MinNoteLength = 10,
                MaxNoteLength = 50,
                SplitChanceBase = 30,
                SplitChanceDecay = 10
            };

            builder.Build();
        }

        private static void BuildRandomMidiFile()
        {
            Random rand = new Random();

            Sequence seq = new Sequence();

            Track track0 = new Track();
            Track fifthTrack = new Track();

            seq.Add(track0);
            seq.Add(fifthTrack);

            ChannelMessageBuilder channelBuilder = new ChannelMessageBuilder();
            channelBuilder.MidiChannel = 0;

            int numNotes = 100;
            //length in ticks
            int minLength = 10;
            int maxLength = 50;
            int minNote = 24;    //c2
            int maxNote = 72;    //c6

            //percent chance
            int fifthChance = 25;

            int curTick = 0;

            for (int a = 0; a < numNotes; a++)
            {
                int note = rand.Next(minNote, maxNote);
                int noteLength = rand.Next(minLength, maxLength);
                int endTick = curTick + noteLength;

                //note
                channelBuilder.Command = ChannelCommand.NoteOn;
                channelBuilder.Data1 = note;
                channelBuilder.Data2 = 127;
                channelBuilder.Build();

                track0.Insert(curTick, channelBuilder.Result);

                //note off
                channelBuilder.Command = ChannelCommand.NoteOff;
                channelBuilder.Data2 = 0;
                channelBuilder.Build();

                track0.Insert(endTick, channelBuilder.Result);

                //add 5th note
                if (rand.Next(100) <= fifthChance)
                {
                    //TODO: make sure fifth isn't too high or low
                    int fifth = rand.Next(1) == 1 ? note + 7 : note - 7;        //fifth is 7 intervals

                    channelBuilder.Command = ChannelCommand.NoteOn;
                    channelBuilder.Data1 = fifth;
                    channelBuilder.Data2 = 127;
                    channelBuilder.Build();

                    fifthTrack.Insert(curTick, channelBuilder.Result);

                    channelBuilder.Command = ChannelCommand.NoteOff;
                    channelBuilder.Data1 = fifth;
                    channelBuilder.Data2 = 0;
                    channelBuilder.Build();

                    fifthTrack.Insert(endTick, channelBuilder.Result);
                }

                //update curTick
                curTick = endTick;
            }

            seq.Save("random.mid");
        }

        private static void Test2()
        {
            Sequence seq = new Sequence();

            Track track0 = new Track();

            seq.Add(track0);

            ChannelMessageBuilder channelBuilder = new ChannelMessageBuilder();
            channelBuilder.MidiChannel = 0;

            //on
            channelBuilder.Command = ChannelCommand.NoteOn;
            channelBuilder.Data1 = 60;
            channelBuilder.Data2 = 127;
            channelBuilder.Build();

            track0.Insert(0, channelBuilder.Result);
            //track0.Insert(100, channelBuilder.Result);
            //track0.Insert(200, channelBuilder.Result);
            //track0.Insert(300, channelBuilder.Result);

            //off
            channelBuilder.Command = ChannelCommand.NoteOn;
            channelBuilder.Data1 = 60;
            channelBuilder.Data2 = 0;
            channelBuilder.Build();

            track0.Insert(100, channelBuilder.Result);
            track0.Insert(100, channelBuilder.Result);
            //track0.Insert(100, channelBuilder.Result);
            //track0.Insert(1000, channelBuilder.Result);
            
            //track0.Insert(98, channelBuilder.Result);
            //track0.Insert(299, channelBuilder.Result);


            

            //save file
            seq.Save("test2.mid");
        }

        private static void BuildTestFile()
        {
            Sequencer s = new Sequencer();
            //sequence
            //Sequence seq = new Sequence();
            s.Sequence = new Sequence();
            //seq.Format = 1;
            
            //tracks
            Track track0 = new Track();
            Track track1 = new Track();

            //add tracks to the sequence
            s.Sequence.Add(track0);
            s.Sequence.Add(track1);

            //tempo?
            //TempoChangeBuilder tempoBuilder = new TempoChangeBuilder();
            //tempoBuilder.Tempo = 500000;
            //tempoBuilder.Build();
            //track0.Insert(0, tempoBuilder.Result);

            //add key
            //KeySignatureBuilder keyBuilder = new KeySignatureBuilder();
            //keyBuilder.Key = Sanford.Multimedia.Key.CMajor;
            //keyBuilder.Build();

            //track0.Insert(0, keyBuilder.Result);

            ChannelMessageBuilder builder = new ChannelMessageBuilder();

            builder.MidiChannel = 2;

            builder.Command = ChannelCommand.ProgramChange;
            builder.Data1 = (int)GeneralMidiInstrument.AcousticGrandPiano;
            builder.Data2 = 0;
            builder.Build();
            track1.Insert(0, builder.Result);

            //add notes
            builder.Command = ChannelCommand.NoteOn;
            builder.Data1 = 60;
            builder.Data2 = 127;
            builder.Build();

            //insert the event
            track1.Insert(1, builder.Result);
            //track1.Insert(500, builder.Result);

            builder.Command = ChannelCommand.NoteOff;
            builder.Data1 = 60;
            builder.Data2 = 0;
            builder.Build();

            track1.Insert(480, builder.Result);
            //track1.Insert(479, builder.Result);

            //add notes
            builder.Command = ChannelCommand.NoteOn;
            builder.Data1 = 67;
            builder.Data2 = 127;
            builder.Build();

            //insert the event
            //track1.Insert(0, builder.Result);

            builder.Command = ChannelCommand.NoteOff;
            builder.Data1 = 67;
            builder.Data2 = 0;
            builder.Build();

            //track1.Insert(480, builder.Result);

            //save file
            s.Sequence.Save("test_out.mid");

            //s.Start();
        }

        private static void CodeFromThread()
        {
            ChannelMessageBuilder channelBuilder = new ChannelMessageBuilder();
            TempoChangeBuilder tempoBuilder = new TempoChangeBuilder();
            Sequencer s = new Sequencer();
            s.Sequence = new Sequence();

            Track track0 = new Track();
            Track track1 = new Track();
            Track track2 = new Track();

            s.Sequence.Add(track0);
            s.Sequence.Add(track1);
            s.Sequence.Add(track2);

            //tempoBuilder.Tempo = (int)(1 / 150.0 * 60000000);
            //tempoBuilder.Build();
            //track0.Insert(0, tempoBuilder.Result);

            channelBuilder.MidiChannel = 1;

            channelBuilder.Command = ChannelCommand.ProgramChange;
            channelBuilder.Data1 = (int)GeneralMidiInstrument.AcousticGrandPiano;
            channelBuilder.Data2 = 0;
            channelBuilder.Build();
            track1.Insert(0, channelBuilder.Result);

            channelBuilder.Command = ChannelCommand.NoteOn;
            channelBuilder.Data1 = 60; // note C
            channelBuilder.Data2 = 127; // velocity 127
            channelBuilder.Build();
            track1.Insert(0, channelBuilder.Result);

            channelBuilder.Command = ChannelCommand.NoteOff;
            channelBuilder.Data1 = 60; // note C
            channelBuilder.Data2 = 0; // note off, so velocity 0
            channelBuilder.Build();
            track1.Insert(479, channelBuilder.Result);

            channelBuilder.MidiChannel = 2;

            channelBuilder.Command = ChannelCommand.ProgramChange;
            channelBuilder.Data1 = (int)GeneralMidiInstrument.AcousticBass;
            channelBuilder.Data2 = 0;
            channelBuilder.Build();
            track2.Insert(0, channelBuilder.Result);

            channelBuilder.Command = ChannelCommand.NoteOn;
            channelBuilder.Data1 = 67; // note G
            channelBuilder.Data2 = 60; // velocity 60
            channelBuilder.Build();
            track2.Insert(480, channelBuilder.Result);

            //channelBuilder.Command = ChannelCommand.NoteOff;
            //channelBuilder.Data1 = 67; // note G
            //channelBuilder.Data2 = 0; // note off, so velocity 0
            //channelBuilder.Build();
            //track2.Insert(480 + 760, channelBuilder.Result);

            s.Sequence.Save("test.mid");
            //s.Start();
        }
    }
}
