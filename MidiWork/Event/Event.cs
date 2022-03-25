using MidiWork.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MidiWork.Event
{
    abstract class Event
    {
        /// <summary>
        /// A TrackChunk, amiben a Event megtalálható
        /// </summary>
        protected TrackChunk tr;
        /// <summary>
        /// Az event startIndex-e a v_time (deltaTime) kezdőindexe
        /// </summary>
        protected int startIndex;

        /// <summary>
        /// Az előző eventtől eltelt idő számbeli értéke
        /// </summary>
        
        protected int v_time;
        /// <summary>
        /// Az event típusát azonosítja be (MetaEvent, SysexEvent, vagy valamilyen MIDI-event). A Running Status miatt van rá szükség
        /// </summary>
        protected byte identifier { get; set; }

        /// <summary>
        /// Az event tényleges adatait tartalmazza (deltaTime, azonosítás)
        /// </summary>
        public List<Byte> containData { get; private set; } = new List<byte>(); //

        /// <summary>
        /// Az event teljes hossza, a v_time-tól egészen az utolsó bájtig. 
        /// </summary>
        /// <example><c>00 FF 2F 00</c> esetén 4</example>
        public int totalLen { get; protected set; } = 0;
        public int getStartIndex() => this.startIndex;

        /// <summary>
        /// Feldolgozza az adott kezdőpontú Event-et a megadott TrackChunk-on belül. Fontos, hogy ez 
        /// </summary>
        /// <param name="startindex">Az egész MidiFile-n belüli beolvasott byte-ok indexe</param>
        /// <param name="tr">Az a TrackChunk objektum, amiben az Event van </param>
        /// <param name="typeByte">Az a bájt, ami azonosítja, hogy ő milyen event legyen (Csak a RunningStatus esetén kap értéket). Alap használatkor használjunk <c>null</c>-t! </param>
        /// <returns>Az az event, amit az adott kezdőponttól olvashattunk</returns>
        public static Event getEvent(int startindex, TrackChunk tr, Byte? typeByte)
        {
            int currIndex = startindex;
            if (currIndex >= tr.mf.readedMidiBytes.Count) throw new EndOfEventException();

            int deltaTimeLen = readVariableLength(startindex, tr);
            if (currIndex + deltaTimeLen >= tr.mf.readedMidiBytes.Count) throw new EndOfEventException();
            
            if (((typeByte == 9) || typeByte == 11) && !MidiEvent.checkNote(currIndex + deltaTimeLen, tr)) typeByte = null;

            if (tr.mf.readedMidiBytes[currIndex + deltaTimeLen] == 255 || typeByte == 255) //METAEVENT
            {
                
                MetaEvent me = new MetaEvent(currIndex, tr, typeByte);
                if (typeByte == null) me.identifier = tr.mf.readedMidiBytes[currIndex + deltaTimeLen];
                else me.identifier = (byte)typeByte;
                return me;

                //meta_event = 0xFF + <meta_type>(1 byte) + <v_length> + <event_data_bytes>
            }
            else if (tr.mf.readedMidiBytes[currIndex + deltaTimeLen] == 240 || tr.mf.readedMidiBytes[currIndex + deltaTimeLen] == 247 || typeByte == 240 || typeByte == 247)
            {
                //Sysex = 0xF7 + <dataBytes> + 0xF7
                // VAGY
                //Sysex = 0xF0 + <dataBytes> + 0xF7
                SysexEvent se = new SysexEvent(startindex, tr);
                if (typeByte == null) se.identifier = tr.mf.readedMidiBytes[currIndex + deltaTimeLen];
                else se.identifier = (byte)typeByte;
                return se;
            }
            else if(tr.mf.readedMidiBytes[currIndex + deltaTimeLen] >= 128 || typeByte >=128)
            {
                MidiEvent me = new MidiEvent(startindex, tr, typeByte);
                //me.totalLen += deltaTimeLen;

                if (typeByte == null) me.identifier = tr.mf.readedMidiBytes[currIndex + deltaTimeLen];
                else me.identifier = (byte)typeByte;
                return me;
            }
            else //Running Status
            {
                int i = 1;
                while (tr.events[tr.events.Count - i].identifier == 0xFF) i++;
                return getEvent(startindex, tr, tr.events[tr.events.Count - i].identifier);
                
            }
        }

        #region Adatrész kiolvasása

        /// <summary>
        /// Az Event-ben eltárolt adatrész szöveg reprezentációját adja. 
        /// </summary>
        /// <param name="flagBytesNumber"></param>
        /// <returns></returns>
        public virtual String getContainedText()
        {
            byte[] subArray = containData.ToArray().SubArray(0);
            return Encoding.Default.GetString(subArray);
        }

        /// <summary>
        /// Az Event-ben eltárolt adatrész decimális szám reprezentációját adja. 
        /// </summary>
        /// <param name="flagBytesNumber">Az a szám, amennyi bájtot az elejéből ki szeretnénk hagyni. </param>
        /// <returns>Adatrész decimális reprezentációja</returns>
        public int getContainedNumber(int flagBytesNumber) => containData.ToArray().SubArray(0, containData.Count - flagBytesNumber).IntValue();

        /// <summary>
        /// Az Event-ben eltárolt adatrész byte-jait egyesével egy decimális számmal ábrázolja és egy Stringként adja vissza. (Amit akár ki lehet printelni vagy ilyesmi.)
        /// </summary>
        /// <returns>Adatrész decimális számként felírva, és egy String-be összefűzve</returns>
        public String getContainedData()
        {
            String back = "";
            foreach (Byte b in this.containData) back += b + " ";
            back += "\n";
            return back;
            
        }
        #endregion

        /// <summary>
        /// Kiír egy sort a konzolra erről az objektumról
        /// </summary>
        public virtual void WriteLine() { }

        /// <summary>
        /// Arra szolgál, hogyha valamiért valamelyik leszármazott osztálynál hiányzik a <see cref="toByteList(bool, bool)"/>, akkor alapértelmezetten a "NOT IMPLEMENTED "toByteList" " üzenetet adja vissza
        /// Ha létezik, akkor a <paramref name="runningStatusEnabled"/> alapján tömörít, ha az lehetséges, és alapértelmezetten true a <paramref name="needDeltaTime"/>, hiszen enélkül a deltaTime mezőt kihaszná a visszatérési értékből, ha viszont ezt elmentjük, akkor nem fogjuk tudni beolvasni.
        /// </summary>
        /// <param name="runningStatusEnabled"><c>true</c>, ha szeretnénk használni Running Status-t bájtok spórolására</param>
        /// <param name="needDeltaTime"><c>true</c>, ha szeretnénk, hogy legyen a bájtonkénti leírásban a v_time</param>
        /// <returns>Az event bájtonkénti leírása</returns>
        public virtual List<byte> toByteList(bool runningStatusEnabled, bool needDeltaTime) => new List<byte> { 0x4e, 0x4f, 0x54, 0x20, 0x49, 0x4d, 0x50, 0x4c, 0x45, 0x4d, 0x45, 0x4e, 0x54, 0x45, 0x44, 0x20, 0x22, 0x74, 0x6f, 0x42, 0x79, 0x74, 0x65, 0x4c, 0x69, 0x73, 0x74, 0x22 };

        /// <summary>
        /// Két eventről eldönti, hogy ugyan az-e a két event, figyelmen kívül hagyva a v_time értékét.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool EqualsWithNoDeltaTime(params byte[] bytes)
        {
            List<byte> b1 = this.toByteList(false, false);
            for (int i = 0; i < bytes.Length; i++) if (bytes[i] != b1[i]) return false;
            return true;
        }

        #region Adat hozzáfűzése

        /// <summary>
        /// Hozzáad egy byte-nyi adatot a tárolt adatokhoz
        /// </summary>
        /// <param name="data">Az adat, amit szeretnénk hozzáfűzni</param>
        protected void addDataByte(Byte data) => this.containData.Add(data);

        #endregion

        #region Változó hosszúságú eventek számolása

        /// <summary>
        /// Kiszámítja, hogy hány bájt hosszúságú lesz az adott kezdőponttól induló változó hosszuságú változó 
        /// </summary>
        /// <param name="startIndex">Az event startindexe a <see cref="MidiFile.readedMidiBytes"/>-on belül</param>
        /// <param name="tr">Az a TrackChunk objektum, amiben az aktuális event van</param>
        /// <returns>A kezdőponttal kezdődő, változó hosszúságú mező hossza</returns>
        /// <exception cref="ArgumentOutOfRangeException">A beolvasásban hiba keletkezett</exception>
        protected static int readVariableLength(int startIndex, TrackChunk tr)
        {
            int toStep = 0;
            try
            {
                while (tr.mf.readedMidiBytes[startIndex + toStep] >= 128)
                {
                    if (startIndex + toStep >= tr.mf.readedMidiBytes.Count) throw new EndOfEventException();
                    toStep++;
                }
                toStep++;
                return toStep;
            }
            catch(ArgumentOutOfRangeException aoore)
            {
                Console.WriteLine("Beolvasásban hiba keletkezett");
                throw aoore;
            }
            
        }

        /// <summary>
        /// Kiszámítja, hogy milyen értékű lesz az adott kezdőponttól induló változó hosszuságú változó
        /// </summary>
        /// <param name="startIndex">Az event startindexe a <see cref="MidiFile.readedMidiBytes"/>-on belül</param>
        /// <param name="tr">Az a TrackChunk objektum, amiben az aktuális event van</param>
        /// <returns></returns>
        protected static int calculateVariableLenthValue(int startIndex, TrackChunk tr)
        {
            //int value = 0;
            int len = readVariableLength(startIndex, tr);
            List<bool> value = new List<bool>();
            for(int i = startIndex + len - 1; i >= startIndex ; i--)
            {
                BitArray ba = new BitArray(new byte[] { tr.mf.readedMidiBytes[i] });
                for(int j = 0; j < 7; j++) value.Add(ba[j]);
            }
            int back = new BitArray(value.ToArray()).getIntValue();
            return back;
        }
        #endregion
    }

}
