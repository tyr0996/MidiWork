using MidiWork.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MidiWork.Event
{
    class MetaEvent : Event
    {
        /// <summary>
        /// A metaEvent típusát határozza meg. Az FF utáni byte tulajdonképpen.
        /// </summary>
        private Byte metaEventType;
        /// <summary>
        /// Tárolt adat hossza.
        /// </summary>
        private int dataLen;

        private static Dictionary<int, String> mLiveTags = new Dictionary<int, String>(){
            {1, "genre"},
            {2, "artist"},
            {3, "composer"},
            {4, "duration"},
            {5, "bpm(tempo)"}
        };


        /// <summary>
        /// MetaEvent típusú objektumot hoz létre, <paramref name="startIndex"/> kezdőponttól.
        /// </summary>
        /// <param name="startIndex">MetaEvent kezdőpontja</param>
        /// <param name="tr">Az a TrackChunk objektum, amihez hozzátartozik ez a MetaEvent</param>
        /// <param name="typeByte">Running Status esetén az előző Event <see cref="Event.identifier"/> mezője. </param>
        public MetaEvent(int startIndex, TrackChunk tr, Byte? typeByte)
        {
            int currIndex = startIndex;
            this.startIndex = startIndex;

            this.tr = tr;
            this.v_time = calculateVariableLenthValue(startIndex, tr);
            int toAdd = readVariableLength(startIndex, tr);
            currIndex += toAdd; //delta-time feldolgozval. 

            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();

            currIndex++; //Túllépünk az FF-n

            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();

            if (typeByte == null) this.metaEventType = this.tr.mf.readedMidiBytes[currIndex];
            else this.metaEventType = (Byte)typeByte;

            if (currIndex+1 >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
            int eventLen = Event.readVariableLength(currIndex+1, tr);
            
            this.dataLen = Event.calculateVariableLenthValue(currIndex+1, tr);
            currIndex += eventLen;


            currIndex++;
            for(int i = 1; i < dataLen+1; i++)
            {
                this.addDataByte(this.tr.mf.readedMidiBytes[currIndex]);
                currIndex++;
                if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
            }

            this.totalLen = (currIndex) - startIndex;
        }

        /// <summary>
        /// A MetaEventType alapján meghatározza, hogy az milyen MetaEvent. Használaton kívüli, későbbi fejlesztések miatt került be.
        /// </summary>
        /// <returns></returns>
        public String getCommand()
        {
            switch (this.metaEventType)
            {
                case 3: return "Track name";
                case 2: return "Copyright";
                case 4: return "Instrument name";
                case 5: return "Lyrics\t";
                case 6: return "Marker";
                case 7: return "Cue Point";
                case 1 : return "Text";
                case 8: return "Program name";
                case 9: return "Device (port) name";
                case 127: return "Sequencer specific";
                case 47: return "End of Track";
                case 0: return "Sequence number";
                case 81: return "Tempo";
                case 84: return "SMPTE offset";
                case 88: return "Time signature";
                case 89: return "Key signature";

                case 32: return "Channel prefix";
                case 33: return "MIDI port";
                case 75: return "M-Live tag";
                default: return "UNKNOWN";
            }
        }

        /// <summary>
        /// Lásd: <see cref="Event.WriteLine"/>
        /// </summary>
        public override void WriteLine()
        {
            //Console.WriteLine(this.v_time + "\t" + this.getCommand() + "\t" + this.getContainedText() + "\tStartIndex: " + this.startIndex);
        }

        /// <summary>
        /// <see cref="Event.toByteList(bool, bool)"/>
        /// </summary>
        /// <param name="runningStatusEnabled">Ál</param>
        /// <param name="deltaTimeNeeded"></param>
        /// <returns></returns>
        public override List<Byte> toByteList(bool runningStatusEnabled, bool deltaTimeNeeded)
        {
            List<Byte> back = new List<byte>();
            if(deltaTimeNeeded) back.AddAll(v_time.toVariableLength());
            back.Add(0xFF);
            back.Add(this.metaEventType);
            back.AddAll(this.dataLen.toVariableLength());
            back.AddAll(this.containData);
            return back;
        }

        /// <summary>
        /// A MetaEvent-ben tárolt adatot adja vissza "értelmezhető" formában
        /// </summary>
        /// <returns>Tárolt adat értelmes alakja</returns>
        public override string getContainedText()
        {
            switch(this.metaEventType){
                case 3: return base.getContainedText();
                case 2: return base.getContainedText();
                case 4: return base.getContainedText();
                case 5: return base.getContainedText();
                case 6: return base.getContainedText();
                case 7: return base.getContainedText();
                case 1: return base.getContainedText();
                case 8: return base.getContainedText();
                case 9: return base.getContainedText();
                case 127: return "Sequencer specific - NOT IMPLEMENTED";
                case 47: return ""; //End of Track - nincs neki adata
                case 0: return "Sequence number"; //TODO: Sequence number
                case 81: return "" + (int)(60000000 / base.getContainedNumber(0));
                case 84: return "SMPTE offset"; //TODO: SMPTE offse
                case 88: return this.containData[0] + "/" + (int) Math.Pow(2, this.containData[1]);
                case 89: return MidiFile.notes[this.containData[0].forceSigned()] + (this.containData[1] == 1 ? "m" : "");
                
                case 32: return "" + (byte) (this.getContainedNumber(0) + 1);
                case 33: return "" + this.getContainedNumber(0);
                case 75: return "M-LIVE TAG - bonyolult egyenlőre : FF4B len tagID text";
                default: return "UNKNOWN";
            }
        }
    }
    //TODO: kiegészíteni a többivel: https://www.mixagesoftware.com/en/midikit/help/HTML/meta_events.html
}
