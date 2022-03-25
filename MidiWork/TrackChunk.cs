using MidiWork.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MidiWork
{
    class TrackChunk
    {
        #region members
        /// <summary>
        /// A track Chunk kezdőindexe az az index, ami az MTrk bájtsorozat 'M' bájtjára mutat.
        /// </summary>
        private int startIndex { get; set; }
        /// <summary>
        /// A TrackChunk legutolsó Eventjének legutolsó bájtjára mutató index
        /// </summary>
        private int endIndex { get; set; }
        /// <summary>
        /// Igaz-hamis érték, azt jelzi, hogy a TruckChunk rendelkezik-e lezáró bájtsorozattal. 
        /// </summary>
        private bool hasEndOfTrack { get; set; }
        /// <summary>
        /// Az a MidiFile objektum, amelyben a TrackChunk objektum megtalálható.
        /// </summary>
		public MidiFile mf { get; private set; }
        /// <summary>
        /// A TrackChunk-ban beolvasott bájtok listája
        /// </summary>
        private List<Byte> readedTrackBytes { get; set; } = new List<Byte>();
        /// <summary>
        /// A TrackChunkban lévő eventek listája. 
        /// </summary>
        public List<MidiWork.Event.Event> events { get; private set; }
        #endregion

        /// <summary>
        /// Ezzel a metódussal hozunk létre a TrackChunk-ról egy példányt.
        /// </summary>
        /// <param name="startIndex">Olyan kezdőindex, ami az MTrk 'M' bájtjára mutat</param>
        /// <param name="mf">Az a MidiFile objektum, amelyben a TrackChunk megtalálható</param>
        public TrackChunk(int startIndex, MidiFile mf)
        {
            this.startIndex = startIndex;
            startIndex += 4; //MTrk hossza
			this.mf = mf;
            this.events = new List<MidiWork.Event.Event>();

            int size = mf.readedMidiBytes[startIndex] * 16777216 + mf.readedMidiBytes[startIndex + 1] * 65536 + mf.readedMidiBytes[startIndex + 2] * 256 + mf.readedMidiBytes[startIndex + 3];
            int currIndex = startIndex + 4;
            this.endIndex = startIndex + size + 4 - 1;
            while (currIndex <= endIndex)
            {
                try
                {
                    MidiWork.Event.Event e = MidiWork.Event.Event.getEvent(currIndex, this, null);
                    events.Add(e);
                    currIndex += e.totalLen;
                }
                catch(EndOfEventException)
                {
                    Debugger.Log(5, null, "A Midi váratlanul ért véget: " + this.mf.fileName + "\n");
                    currIndex = endIndex+1;
                }
            }
            this.hasEndOfTrack = (this.events[this.events.Count - 1].EqualsWithNoDeltaTime(0xFF, 0x2F, 0x00));
            logPartion();
        }

        /// <summary>
        /// Beolvassa a soron következő TrackChunk-ot
        /// </summary>
        /// <returns>Ha létezik, akkor a TrackChunk objektummal, ha nem, akkor null-al tér vissza</returns>
        public TrackChunk readNext() => (this.mf.readedMidiBytes.Count >= this.endIndex + 1) ? new TrackChunk(this.endIndex + 1, this.mf) : null;



        /// <summary>
        /// Ellenőrzi, hogy TrackChunk kezdődik 
        /// </summary>
        /// <param name="startIndex">Ittől az indextől kezdve csinálja a vizsgálatot</param>
        /// <param name="mf">MidiFile objektum, ami a TrackChunk-ot tartalmazza</param>
        /// <returns>Hamis, ha a fájl végén hívjuk meg, vagy nem Track chunk jön.</returns>
        public bool nextIsTrackChunk(int startIndex, MidiFile mf)
		{
            int trackChunkMinimalSize = 8;

			if(startIndex + trackChunkMinimalSize >= this.mf.readedMidiBytes.Count) return false;

            byte[] trackChunkFlagCorrect = new byte[] {77, 84, 114, 107};
            byte[] nextBytes = new byte[] { mf.readedMidiBytes[startIndex], mf.readedMidiBytes[startIndex + 1], mf.readedMidiBytes[startIndex + 2], mf.readedMidiBytes[startIndex + 3] };
                return trackChunkFlagCorrect.SequenceEqual(nextBytes);
        }

        /// <summary>
        /// Ez a függvény kiírja a konzolra az összes eventet a beolvasott eventekből.
        /// </summary>
        public void Write()
        {
            if (this.events == null) return;
            foreach(Event.Event e in this.events) e.WriteLine();
        }

        /// <summary>
        /// A Track Chunkban megtalálható, egy ismert startIndex-el kezdődő Event helyét adja meg a TrackChunkon belül. 
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns>A <paramref name="startIndex"/> kezdőindexű Event indexe, ha van ilyen event; -1, ha nincs ilyen elem</returns>
        public int getIndexByStartIndex(int startIndex)
        {
            for(int i = 0; i < this.events.Count; i++) if (this.events[i].getStartIndex() == startIndex) return i;
            return -1;
        }

        /// <summary>
        /// A TrackChunk objektumot bájt-listává alakítja. Ha a Trackchunk nem rendelkezett lezáró bájtsorozattal, akkor hozzáfűzi automatikusan.
        /// </summary>
        /// <param name="runningStatusEnabled">Állítsuk true-ra, ha szeretnénk bájtokat spórolni a Running Status segítségével.</param>
        /// <returns></returns>
        public List<byte> toByteList(bool runningStatusEnabled)
        {
            List<byte> back = new List<byte> { 0x4d, 0x54, 0x72, 0x6b };
            foreach (Event.Event e in this.events) back.AddAll(e.toByteList(runningStatusEnabled, true));
            if (!this.hasEndOfTrack) back.AddAll((byte)0x00, (byte)0xFF, (byte)0x2F, (byte)0x00);
            return back;
        }

        #region logolás


        public void logPartion()
        {
            //Debugger.Log(5, null, "TrackChunk sikeresen beolvasva a " + this.startIndex + " - " + (int)(this.endIndex) + " intervallumon!\n");
        }
        #endregion

    }
}
