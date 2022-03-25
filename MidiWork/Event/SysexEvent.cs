using MidiWork.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MidiWork.Event
{
    /***
     * A Sysex mindíg 0xF7 bytra ér véget! (0xF7 = 247)
     */
    class SysexEvent : Event
    {
        public bool byte_F0;

        /// <summary>
        /// SysexEvent-et hoz létre a <paramref name="startIndex"/> kezdőponttól a <paramref name="tr"/>-en.
        /// </summary>
        /// <param name="startIndex">Az eventhez tartozó v_time első bájtjának indexe</param>
        /// <param name="tr">TrackChunk objektum, amihez az event tartozik. </param>ű
        /// <exception cref="EndOfEventException">Akkor dobja, ha váratlanul véget ér a fájl.</exception>
        public SysexEvent(int startIndex, TrackChunk tr)
        {
            int currIndex = startIndex;
            this.startIndex = startIndex;
            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
            this.tr = tr;
            this.byte_F0 = (tr.mf.readedMidiBytes[startIndex] == 0xF0);
            this.v_time = calculateVariableLenthValue(startIndex, tr);
            int toAdd = readVariableLength(startIndex, tr);
            currIndex += toAdd;
            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();

            currIndex++; //Túllépünk a típuson
            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();

            while (tr.mf.readedMidiBytes[currIndex] != 247)
            {
                this.addDataByte(tr.mf.readedMidiBytes[currIndex]);
                currIndex++;
                if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
            }

            currIndex++;
            if (currIndex >= tr.mf.readedMidiBytes.Count()) throw new EndOfEventException();
            this.totalLen = currIndex - this.startIndex;
        }

        /// <summary>
        /// Lásd: <see cref="Event.toByteList(bool, bool)"/>
        /// </summary>
        /// <param name="runningStatusEnabled"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public override List<byte> toByteList(bool runningStatusEnabled, bool deltaTime)
        {
            List<byte> back = new List<byte>();
            back.AddAll(this.v_time.toVariableLength());
            if (byte_F0) back.Add(0xF0);
            else back.Add(0xF7);
            back.AddAll(this.containData);
            back.Add(0xF7);
            return back;
        }

        /// <summary>
        /// Lásd: <see cref="Event.WriteLine()"/>
        /// </summary>
        public override void WriteLine() => base.WriteLine();
    }
}
