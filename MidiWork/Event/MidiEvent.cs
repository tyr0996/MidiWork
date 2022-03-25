using MidiWork.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MidiWork.Event
{
    class MidiEvent : Event
    {
        /// <summary>
        /// Az event típusa, a v_time utáni első byte
        /// </summary>
        private byte midiEventType;

        /// <summary>
        /// Sáv (0-15)
        /// </summary>
        private byte channel;


        public static Dictionary<int, String> CCnumbers = new Dictionary<int, string>()
        {
            {0, "Bank Select MSB (GS)"},
            {1, "Modulation Wheel"},
            {2, "Breath"},
            {3, "Undefined"},
            {4, "Foot"},
            {5, "Portamento Time"},
            {6, "Data Entry MSB"},
            {7, "Volume"},
            {8, "Balance"},
            {9, "Undefined"},
            {10, "Pan"},
            {11, "Expression"},
            {12, "Effect control 1"},
            {13, "Effect control 2"},
            {14, "Undefined"},
            {15, "Undefined"},
            {16, "General Purpose #1"},
            {17, "General Purpose #2"},
            {18, "General Purpose #3"},
            {19, "General Purpose #4"},
            {20, "Undefined"},
            {21, "Undefined"},
            {22, "Undefined"},
            {23, "Undefined"},
            {24, "Undefined"},
            {25, "Undefined"},
            {26, "Undefined"},
            {27, "Undefined"},
            {28, "Undefined"},
            {29, "Undefined"},
            {30, "Undefined"},
            {31, "Undefined"},
            {32, "Bank Select MSB LSB"},
            {33, "Modulation Wheel LSB"},
            {34, "Breath LSB"},
            {35, "Undefined"},
            {36, "Foot LSB"},
            {37, "Portamento Time LSB"},
            {38, "Data Entry LSB"},
            {39, "Volume LSB"},
            {40, "Balance LSB"},
            {41, "Undefined"},
            {42, "Pan LSB"},
            {43, "Expression LSB"},
            {44, "Effect control 1 LSB"},
            {45, "Effect control 2 LSB"},
            {46, "Undefined"},
            {47, "Undefined"},
            {48, "General Purpose #1 LSB"},
            {49, "General Purpose #2 LSB"},
            {50, "General Purpose #3 LSB"},
            {51, "General Purpose #4 LSB"},
            {52, "Undefined"},
            {53, "Undefined"},
            {54, "Undefined"},
            {55, "Undefined"},
            {56, "Undefined"},
            {57, "Undefined"},
            {58, "Undefined"},
            {59, "Undefined"},
            {60, "Undefined"},
            {61, "Undefined"},
            {62, "Undefined"},
            {63, "Undefined"},
            {64, "Hold Pedal #1"},
            {65, "Portamento (GS)"},
            {66, "Sostenuto (GS)"},
            {67, "Soft Pedal (GS)"},
            {68, "Legato Pedal"},
            {69, "Hold Pedal #2"},
            {70, "Sound Variation"},
            {71, "Sound Timbre"},
            {72, "Sound Release Time"},
            {73, "Sound Attack Time"},
            {74, "Sound Brightness"},
            {75, "Sound Control #6"},
            {76, "Sound Control #7"},
            {77, "Sound Control #8"},
            {78, "Sound Control #9"},
            {79, "Sound Control #10"},
            {80, "GP Control #5"},
            {81, "GP Control #6"},
            {82, "GP Control #7"},
            {83, "GP Control #8"},
            {84, "Portamento Control (GS)"},
            {85, "Undefined"},
            {86, "Undefined"},
            {87, "Undefined"},
            {88, "Undefined"},
            {89, "Undefined"},
            {90, "Undefined"},
            {91, "Reverb Level (GS)"},
            {92, "Tremolo Depth"},
            {93, "Chorus Level (GS)"},
            {94, "Celeste Depth"},
            {95, "Phaser Depth"},
            {96, "Data Increment"},
            {97, "Data Decrement"},
            {98, "NRPN Parameter LSB (GS)"},
            {99, "NRPN Parameter MSB (GS)"},
            {100, "RPN Parameter LSB"},
            {101, "RPN Parameter MSB"},
            {102, "Undefined"},
            {103, "Undefined"},
            {104, "Undefined"},
            {105, "Undefined"},
            {106, "Undefined"},
            {107, "Undefined"},
            {108, "Undefined"},
            {109, "Undefined"},
            {110, "Undefined"},
            {111, "Undefined"},
            {112, "Undefined"},
            {113, "Undefined"},
            {114, "Undefined"},
            {115, "Undefined"},
            {116, "Undefined"},
            {117, "Undefined"},
            {118, "Undefined"},
            {119, "Undefined"},
            {120, "All Sound Off (GS)"},
            {121, "Reset All Controllers"},
            {122, "Local On/Off"},
            {123, "All Notes Off"},
            {124, "Omni Mode Off"},
            {125, "Omni Mode On"},
            {126, "Mono Mode On"},
            {127, "Poly Mode On"},
            {128, "Controller Type 1"},
            {129, "Controller Type 2"},
            {130, "Controller Type 3"},
            {131, "Controller Type 4"},
            {132, "Controller Type 5"},
            {133, "Controller Type 6"},
            {134, "Controller Type 7"},
            {135, "Controller Type 8"},
            {136, "Controller Type 9"},
            {137, "Controller Type 10"},
            {138, "Controller Type 11"},
            {139, "Controller Type 12"},
            {140, "Controller Type 13"},
            {141, "Controller Type 14"},
            {142, "Controller Type 15"},
            {143, "Controller Type 16"},
            {144, "Controller Type 17"},
            {145, "Controller Type 18"},
            {146, "Controller Type 19"},
            {147, "Controller Type 20"},
            {148, "Controller Type 21"},
            {149, "Controller Type 22"},
            {150, "Controller Type 23"},
            {151, "Controller Type 24"},
            {152, "Controller Type 25"},
            {153, "Controller Type 26"},
            {154, "Controller Type 27"},
            {155, "Controller Type 28"},
            {156, "Controller Type 29"},
            {157, "Controller Type 30"},
            {158, "Controller Type 31"},
            {159, "Controller Type 32"},
            {160, "Controller Type 33"},
            {161, "Controller Type 34"},
            {162, "Controller Type 35"},
            {163, "Controller Type 36"},
            {164, "Controller Type 37"},
            {165, "Controller Type 38"},
            {166, "Controller Type 39"},
            {167, "Controller Type 40"},
            {168, "Controller Type 41"},
            {169, "Controller Type 42"},
            {170, "Controller Type 43"},
            {171, "Controller Type 44"},
            {172, "Controller Type 45"},
            {173, "Controller Type 46"},
            {174, "Controller Type 47"},
            {175, "Controller Type 48"},
            {176, "Controller Type 49"},
            {177, "Controller Type 50"},
            {178, "Controller Type 51"},
            {179, "Controller Type 52"},
            {180, "Controller Type 53"},
            {181, "Controller Type 54"},
            {182, "Controller Type 55"},
            {183, "Controller Type 56"},
            {184, "Controller Type 57"},
            {185, "Controller Type 58"},
            {186, "Controller Type 59"},
            {187, "Controller Type 60"},
            {188, "Controller Type 61"},
            {189, "Controller Type 62"},
            {190, "Controller Type 63"},
            {191, "Controller Type 64"},
            {192, "Controller Type 65"},
            {193, "Controller Type 66"},
            {194, "Controller Type 67"},
            {195, "Controller Type 68"},
            {196, "Controller Type 69"},
            {197, "Controller Type 70"},
            {198, "Controller Type 71"},
            {199, "Controller Type 72"},
            {200, "Controller Type 73"},
            {201, "Controller Type 74"},
            {202, "Controller Type 75"},
            {203, "Controller Type 76"},
            {204, "Controller Type 77"},
            {205, "Controller Type 78"},
            {206, "Controller Type 79"},
            {207, "Controller Type 80"},
            {208, "Controller Type 81"},
            {209, "Controller Type 82"},
            {210, "Controller Type 83"},
            {211, "Controller Type 84"},
            {212, "Controller Type 85"},
            {213, "Controller Type 86"},
            {214, "Controller Type 87"},
            {215, "Controller Type 88"},
            {216, "Controller Type 89"},
            {217, "Controller Type 90"},
            {218, "Controller Type 91"},
            {219, "Controller Type 92"},
            {220, "Controller Type 93"},
            {221, "Controller Type 94"},
            {222, "Controller Type 95"},
            {223, "Controller Type 96"},
            {224, "Controller Type 97"},
            {225, "Controller Type 98"},
            {226, "Controller Type 99"},
            {227, "Controller Type 100"},
            {228, "Controller Type 101"},
            {229, "Controller Type 102"},
            {230, "Controller Type 103"},
            {231, "Controller Type 104"},
            {232, "Controller Type 105"},
            {233, "Controller Type 106"},
            {234, "Controller Type 107"},
            {235, "Controller Type 108"},
            {236, "Controller Type 109"},
            {237, "Controller Type 110"},
            {238, "Controller Type 111"},
            {239, "Controller Type 112"},
            {240, "Controller Type 113"},
            {241, "Controller Type 114"},
            {242, "Controller Type 115"},
            {243, "Controller Type 116"},
            {244, "Controller Type 117"},
            {245, "Controller Type 118"},
            {246, "Controller Type 119"},
            {247, "Controller Type 120"},
            {248, "Controller Type 121"},
            {249, "Controller Type 122"},
            {250, "Controller Type 123"},
            {251, "Controller Type 124"},
            {252, "Controller Type 125"},
            {253, "Controller Type 126"},
            {254, "Controller Type 127"},
            {255, "Controller Type 128"},
        };

        /// <summary>
        /// MidiEvent objektumot hoz létre a <paramref name="startindex"/>-től a <paramref name="tr"/>-be, és Running Status esetén <paramref name="typeByte"/> típussal.
        /// </summary>
        /// <param name="startindex">kezdőpont</param>
        /// <param name="tr">TrackChunk objektum, amihez a MidiEvent tartozik.</param>
        /// <param name="typeByte">Running Status esetén </param>
        public MidiEvent(int startindex, TrackChunk tr, Byte? typeByte)
        {
            this.tr = tr;
            int currIndex = startindex;

            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();

            this.startIndex = startindex;
            this.v_time = calculateVariableLenthValue(startindex, tr);
            currIndex += readVariableLength(startindex, tr);

            byte[] cutted;
            if (typeByte == null) cutted = tr.mf.readedMidiBytes[currIndex].cutInHalf();
            else cutted = ((byte)(typeByte)).cutInHalf();

            if (typeByte == null) currIndex++; //Itt lépünk be az adatrészbe.

            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
            this.midiEventType = cutted[0];

            switch(cutted[0])
            {
                case 8:
                    {
                        this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                        currIndex++;
                        if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                        this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                        currIndex++;
                        if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                        break;
                    }
                case 9:
                    {
                        this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                        currIndex++;
                        if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                        this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                        currIndex++;
                        if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                        break;
                    }
                case 10:
                    {
                        this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                        currIndex++;
                        if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                        this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                        currIndex++;
                        if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                        break;
                    }
                case 11:
                    {
                        if (this.tr.mf.readedMidiBytes[currIndex] == 98 || this.tr.mf.readedMidiBytes[currIndex] == 99)
                        {
                            this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                            currIndex++;
                            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                            this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                            currIndex++;
                            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                            break;
                        }
                        else
                        {
                            this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                            currIndex++;
                            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                            this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                            currIndex++;
                            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                            break;
                        }
                    }
                case 12: this.addDataByte(tr.mf.readedMidiBytes[currIndex]); currIndex++; if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException(); break;
                case 13: this.addDataByte(tr.mf.readedMidiBytes[currIndex]); currIndex++; if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException(); break;
                case 14:
                    {
                        this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                        currIndex++;
                        if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                        this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                        currIndex++;
                        if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
                        break;
                    }
                default:
                    {
                        //Debugger.Log(2, null, "Sajnos egy olyan MidiEvent típussal találkoztam, amire nem programozott még fel az íróm!\n"); 
                        Console.WriteLine("Hiba történt! - Ismeretlen EventTípus! KezdőByte indexe: " + currIndex);
                        throw new ArgumentOutOfRangeException();
                    }
            }
            this.channel = (Byte) (cutted[1] + 1);
            this.totalLen = (currIndex) - startIndex;
        }

        public static bool checkNote(int index, TrackChunk tr) => MidiFile.allNotes.ContainsKey(tr.mf.readedMidiBytes[index]);

        /// <summary>
        /// A MidiEvent utasítását adja meg 
        /// </summary>
        /// <returns>NoteOff, NoteOn, NoteAftertouch, Controller, PorgramChange, ChannelAftertouch stb.</returns>
        public String getCommand()
        {
            switch (this.midiEventType)
            {
                case 8: return "NoteOff";
                case 9: return "NoteOn\t";
                case 10: return "NoteAftertouch";
                case 11: return "Controller";
                case 12: return "ProgramChange";
                case 13: return "ChannelAftertouch";
                case 14: return "PitchBend";
                default: return "UNDEFINED";
            }
        }

        /// <summary>
        /// Lásd: <see cref="Event.WriteLine()"/>
        /// </summary>
        public override void WriteLine()
        {
            String toPrint = this.v_time + "\t" + this.getCommand() + "\t" + this.channel + "\t";
            if (this.midiEventType == 8 || this.midiEventType == 9)
            {
                toPrint += MidiFile.allNotes[this.containData[0]].ToString(); /*+ "\t\t\tVelocity: " + this.containData[1].ToString()*/
            }
            else if (this.midiEventType == 10)
            {
                toPrint += MidiFile.allNotes[this.containData[0]].ToString() + "\tPressure: " + this.containData[1].ToString();
            }
            else if (this.midiEventType == 11)
            {
                toPrint += CCnumbers[this.containData[0]].ToString() + "\t" + (this.containData[1] <= 63 ? "Off" : "On");
            }
            else if (this.midiEventType == 12)
            {
                toPrint += CCnumbers[this.containData[0]].ToString();
            }
            else if (this.midiEventType == 13)
            {
                toPrint += "Pressure: " + this.containData[0].ToString();
            }
            else if (this.midiEventType == 14)
            {
                toPrint += "CSUMIDA"; 
                //TODO
            }
            else
            {

            }


            //Console.WriteLine(toPrint + "\tStartindex: " + this.startIndex);
        }


        /// <summary>
        /// Lásd: <see cref="Event.toByteList(bool, bool)"/>
        /// </summary>
        /// <param name="runningStatusEnabled"><c>true</c> ha használni szeretnénk a Running Status-t/param>
        /// <param name="deltaTime">Állítsuk <c>false</c>-ra, ha a deltaTime-ot nem szeretnénk, hogy bennelegyen a bytetömbben.</param>
        /// <returns></returns>
        public override List<byte> toByteList(bool runningStatusEnabled, bool deltaTime)
        {
            List<byte> back = new List<byte>();
            back.AddAll(this.v_time.toVariableLength());
            if (runningStatusEnabled)
            {
                bool bePrevious = true;
                bool equalByte = false;
                int previousIndex = this.tr.getIndexByStartIndex(this.startIndex) - 1;
                if (previousIndex < 0 || previousIndex >= this.tr.events.Count) bePrevious = false;
                if (bePrevious) {
                    Event prevEvent = this.tr.events[this.tr.getIndexByStartIndex(this.startIndex) - 1];
                    if (prevEvent.GetType() == typeof(MidiEvent))
                    {
                        if (((MidiEvent)prevEvent).getTypeByte() == this.getTypeByte()) equalByte = true;
                    }
                }
                if (!equalByte) back.Add(this.getTypeByte());
            }

            back.AddAll(this.containData);
            return back;
        }

        /// <summary>
        /// A TypeByte-ot adja meg, amely alapján működik a Running Status (megegyezik a <see cref="Event.identifier"/>) mezővel.
        /// </summary>
        /// <returns></returns>
        public byte getTypeByte() => (byte)(this.midiEventType * 16 + this.channel);



    }   
}
