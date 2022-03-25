using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using MidiWork.Event;
using MidiWork.Exceptions;

namespace MidiWork
{
    /// <summary>
    /// Egy *.midi fájl reprezentációja
    /// </summary>
    class MidiFile
    {
        #region members
        /// <summary>
        /// A header Chunk mérete. Értéke mindig 6.
        /// </summary>
        private int chunkSize;
        /// <summary>
        /// A MIDI formátuma. Értéke 0, 1 vagy 2 lehet
        /// <para> Ha az értéke 0, akkor 1 darab TrackChunk-ra van írva minden</para>
        /// <para> Ha az értéke 1, akkor több TrackChunk is lehetséges</para>
        /// <para> Ha az értéke 2 (nem gyakori), akkor </para>
        /// </summary>
        private int formatType;
        /// <summary>
        /// A Midi fájlban megtalálható TrackChunk-ok száma.
        /// </summary>
        private int numberOfTracks;
        /// <summary>
        /// Időosztás típusa. Két értéket vehet fel: metrical és timecode.
        /// <para>Ha a két bájt - ami ezt leírja - közül legfelsőbb bájtnak a legfelsőbb bitje 0, akkor "metrikus" (azaz Ticks per Quarter-note van érvényben), ha viszont ez a bit 1, akkor "time division" (Frames per second)</para>
        /// <para>Frames per Second esetén az felsőbb bájt maradék 7 bitjén reprezentáljuk a FPS értékét, amely lehet 24, 25, 29.97 (29-el van reprezentálva), 30.
        /// </summary>
        private String timeDivison;
        /// <summary>
        /// Semmi másra nem való, hogy sok számolástól óvjuk meg a számítógépet. A <seealso cref="timeDivison"/>-t meghatározó 2 bájt felső bájtja. 
        /// </summary>
        private byte firstTimeDivison;
        /// <summary>
        /// Semmi másra nem való, hogy sok számolástól óvjuk meg a számítógépet. A <seealso cref="timeDivison"/>-t meghatározó 2 bájt alsó bájtja. 
        /// </summary>
        private byte lastTimeDivison;
        /// <summary>
        /// A <see cref="timeDivison"/>-nak megfelelő, maga a konkrét adatot tárolja. 
        /// </summary>
        private int timeDivisonValue;
        /// <summary>
        /// A MIDI-ből beolvasott bájtok listája.
        /// </summary>
        public List<Byte> readedMidiBytes;

        /// <summary>
        /// Az eredetileg beolvasott fájl neve kiterjesztés nélkül.
        /// </summary>
        /// <example>midifile(120)</example>
        public String fileName;
        /// <summary>
        /// Az eredetileg beolvasott fájl elérési útja 
        /// </summary>
        /// <example>//TODO</example>
        public String filePath;
        /// <summary>
        /// Az eredetileg beolvasott fájl kiterjesztése.
        /// </summary>
        /// <example>".midi"</example>
        public String fileExtension;
        /// <summary>
        /// A következő feldolgozatlan index. A TrackChunkok beolvasásánál van rá szükség
        /// </summary>
        public int nextUnprocessedByteIndex { get; private set; } = 0;
        #endregion

        public static Dictionary<int, String> notes = new Dictionary<int, String>(){
            {0 ,"C"},
            {7 ,"C#"},{-5 ,"Db"},
            {2 ,"D"},
            { -3,"Eb"},
            {4 ,"E"},
            {-1 ,"F"},
            {6 ,"F#"},{ -6,"Gb"},
            {1 ,"G"},
            { -4,"Ab"},
            {3 ,"A"},
            {-2 ,"Bb"},
            {5 ,"B"}, {-7, "Cb"}
        };

        public static Dictionary<int, String> allNotes = new Dictionary<int, string>()
        {
            {127, "G9"},
            {126, "F#9"},
            {125, "F9"},
            {124, "E9"},
            {123, "D#9"},
            {122, "D9"},
            {121, "C#9"},
            {120, "C9"},
            {119, "B8"},
            {118, "A#8"},
            {117, "A8"},
            {116, "G#8"},
            {115, "G8"},
            {114, "F#8"},
            {113, "F8"},
            {112, "E8"},
            {111, "D#8"},
            {110, "D8"},
            {109, "C#8"},
            {108, "C8"},
            {107, "B7"},
            {106, "A#7"},
            {105, "A7"},
            {104, "G#7"},
            {103, "G7"},
            {102, "F#7"},
            {101, "F7"},
            {100, "E7"},
            {99, "D#7"},
            {98, "D7"},
            {97, "C#7"},
            {96, "C7"},
            {95, "B6"},
            {94, "A#6"},
            {93, "A6"},
            {92, "G#6"},
            {91, "G6"},
            {90, "F#6"},
            {89, "F6"},
            {88, "E6"},
            {87, "D#6"},
            {86, "D6"},
            {85, "C#6"},
            {84, "C6"},
            {83, "B5"},
            {82, "A#5"},
            {81, "A5"},
            {80, "G#5"},
            {79, "G5"},
            {78, "F#5"},
            {77, "F5"},
            {76, "E5"},
            {75, "D#5"},
            {74, "D5"},
            {73, "C#5"},
            {72, "C5"},
            {71, "B4"},
            {70, "A#4"},
            {69, "A4"},
            {68, "G#4"},
            {67, "G4"},
            {66, "F#4"},
            {65, "F4"},
            {64, "E4"},
            {63, "D#4"},
            {62, "D4"},
            {61, "C#4"},
            {60, "C4"},
            {59, "B3"},
            {58, "A#3"},
            {57, "A3"},
            {56, "G#3"},
            {55, "G3"},
            {54, "F#3"},
            {53, "F3"},
            {52, "E3"},
            {51, "D#3"},
            {50, "D3"},
            {49, "C#3"},
            {48, "C3"},
            {47, "B2"},
            {46, "A#2"},
            {45, "A2"},
            {44, "G#2"},
            {43, "G2"},
            {42, "F#2"},
            {41, "F2"},
            {40, "E2"},
            {39, "D#2"},
            {38, "D2"},
            {37, "C#2"},
            {36, "C2"},
            {35, "B1"},
            {34, "A#1"},
            {33, "A1"},
            {32, "G#1"},
            {31, "G1"},
            {30, "F#1"},
            {29, "F1"},
            {28, "E1"},
            {27, "D#1"},
            {26, "D1"},
            {25, "C#1"},
            {24, "C1"},
            {23, "B0"},
            {22, "A#0"},
            {21, "A0"},
            {20, "G#0"},
            {19, "G0"},
            {18, "F#0"},
            {17, "F0"},
            {16, "E0"},
            {15, "D#0"},
            {14, "D0"},
            {13, "C#0"},
            {12, "C0"},
            {11, "B-1"},
            {10, "A#-1"},
            {9, "A-1"},
            {8, "G#-1"},
            {7, "G-1"},
            {6, "F#-1"},
            {5, "F-1"},
            {4, "E-1"},
            {3, "D#-1"},
            {2, "D-1"},
            {1, "C#-1"},
            {0, "C-1"}
        };
        #region computed
        private List<TrackChunk> trackChunks;
        #endregion


        /// <summary>
        /// A megadott index-ű TrackChunkot adja vissza a MidiFile-ból.
        /// </summary>
        /// <param name="index">e</param>
        /// <returns>A megadott indexű TrackChunk objekt</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public TrackChunk getTrackChunk(int index) 
        {
            try
            {
                return this.trackChunks[index];
            }
            catch(IndexOutOfRangeException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Látrehoz egy MidiFile példányt, beolvassa a byte-okat és az összes Chunkot
        /// </summary>
        /// <param name="fullPath">A MIDI fájl elérési útja</param>
        /// <exception cref="FileNotFoundException">A megadott fájl nem létezik!</exception>
        private MidiFile(String fullPath)
        {            
            init();

            if (File.Exists(fullPath)) throw new FileNotFoundException("A megadott " + fullPath + " fájl nem létezik!"); 

            this.filePath = Path.GetDirectoryName(fullPath);
            this.fileName = Path.GetFileNameWithoutExtension(fullPath);
            this.fileExtension = Path.GetExtension(fullPath);

            this.readedMidiBytes = Program.readFile(fullPath);
            readHeaderChunk();
            readTrackChunks();
            Console.WriteLine("A " + fullPath + " fájl beolvasása sikeresen megtörtént!");
        }

        /// <summary>
        /// A MidiFile objektumban lévő adatokat egy nagy List<byte>-ba foglalja.
        /// </summary>
        /// <param name="runningStatusEnabled">Állítsuk true-ra, ha szeretnénk használni a Running Status-t, és álítsuk false-ra, ha ezt nem szeretnénk (ugyanaz, mint a true változat, csak több helyet foglal redundáns bájtok miatt.)</param>
        /// <returns>Az objektum byte-tömb reprezentációja</returns>
        private List<byte> toByteList(bool runningStatusEnabled)
        {
            List<byte> back = new List<byte>();
            back.AddAll((byte)0x4d, (byte)0x54, (byte)0x68, (byte)0x64);
            back.AddAll((byte)0x00, (byte)0x00, (byte)0x00, (byte)chunkSize);
            back.AddAll((byte)0x00, (byte)formatType);
            back.AddAll((byte)(this.numberOfTracks/256), (byte)(numberOfTracks % 256));
            back.Add(this.firstTimeDivison);
            back.Add(this.lastTimeDivison);
            foreach (TrackChunk tr in this.trackChunks) back.AddAll(tr.toByteList(true));
            return back;
        }

        private void init()
        {
            this.trackChunks = new List<TrackChunk>();
        }

        /// <summary>
        /// Ellenőrzi, hogy a fájl végén van-e End of Track lezáró 3 bájt (0xFF 0x2F 0x00)
        /// </summary>
        /// <returns>true, ha rendben van a fájl vége, és false, ha a fájlnak "nincs vége"</returns>
        private Boolean checkEOF() => !(this.readedMidiBytes[^3] != 0xFF || this.readedMidiBytes[^2] != 0x2F || this.readedMidiBytes[^1] != 0x00);


        #region Mentés, betöltés

        /// <summary>
        /// Betölti a <paramref name="path"/>-on található MIDI fájlt.
        /// </summary>
        /// <seealso cref="MidiFile.MidiFile(string)"/>
        /// <param name="path"></param>
        /// <returns></returns>
        public static MidiFile load(String path) => new MidiFile(path);

        /// <summary>
        /// Elmenti <paramref name="path"/> fájlba a MidiFile objektumot úgy, hogy az érvényes legyen.
        /// </summary>
        /// <param name="path"></param>
        public void save(String path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Append))
            {
                List<byte> bytes = this.toByteList(true);
                fs.Write(bytes.ToArray(), 0, bytes.Count);
            }
        }

        /// <summary>
        /// Elmenti az eredeti fájl könyvtárába más néven a MidiFile objektumot úgy, hogy az érvényes legyen.
        /// </summary>
        /// <seealso cref="MidiFile.save(string)"/>
        public void save()
        {
            String path = this.filePath + "\\" + this.fileName + "_modified" + this.fileExtension;
            Console.WriteLine(path);
            this.save(path);
        }
        #endregion

        #region Head-chunkhoz kapcsolódó

        /// <summary>
        /// Ellenőrzi a Midi fájl "aláírását". (Az első 4 byte a MThd karaktersorozatot adja, hexadecimális formában 0x4D546864). Ez a négy byte jelzi, hogy ténylegesen egy Midi fájlról van szó.
        /// </summary>
        /// <returns>true, ha az első négy bájt megfelel a MThd bájtoknak, false ha nem</returns>
        private Boolean verifyHeader()
        {
            this.nextUnprocessedByteIndex += 4;
            return (readedMidiBytes[0] == 0x4D && readedMidiBytes[1] == 0x54 && readedMidiBytes[2] == 0x68 && readedMidiBytes[3] == 0x64);
        }

        /// <summary>
        /// Egyes MIDI fájlok nem a megszokott MThd bájtokkal kezdődnek, hanem a RIFF bájtokkal. Ez is érvényes MIDI fájl, azonban a program nem tudja kezelni
        /// </summary>
        /// <returns>true, ha az első 4 bájt RIFF, false ha nem</returns>
        private Boolean isRIFFBased() => readedMidiBytes[0] == 0x52 && readedMidiBytes[1] == 0x49 && readedMidiBytes[2] == 0x46 && readedMidiBytes[3] == 0x46;

        /// <summary>
        /// Kiolvassa a Midi fájlból a headerchunk méretet. Ennek az értéke mindig 6. 
        /// </summary>
        /// <exception cref="InvalidHeaderChunkException">"Nem várt Chunk méret! A HeaderChunk mérete mindig 6! "</exception>
        private void readChunkSize()
        {
            int result = readedMidiBytes[4] * 16777216 + readedMidiBytes[5] * 65536 + readedMidiBytes[6] * 256 + readedMidiBytes[7];
            if (result != 6) throw new InvalidHeaderChunkException("Nem várt Chunk méret! A HeaderChunk mérete mindig 6! " + result);
            this.chunkSize = result;
            this.nextUnprocessedByteIndex += 4;
        }

        /// <summary>
        /// Meghatározza a Midi fájlból, hogy 0-s, 1-es, vagy 2-es típusú. Ez a Track-ok miatt szükséges.
        /// </summary>
        /// <exception cref="InvalidMIDITypeException">Nem várt Midi-file formátum-fípus!</exception>
        private void readFormatType()
        {
            List<Byte> readedMidi = this.readedMidiBytes;
            int result = readedMidi[8] * 256 + readedMidi[9];
            if (result < 0 || result > 2) throw new InvalidMIDITypeException("Nem várt Midi-file formátum-típus! " + result);

            this.formatType = result;
            this.nextUnprocessedByteIndex += 2;
        }

        /// <summary>
        /// Kiolvassa a Midi fájlból, hogy hány TrackChunk-ot tartalmaz.
        /// </summary>
        /// <exception cref="InvalidTrackChunkSizeException">Nem megfelelő NumberOfTracks!</exception>
        private void readNumberOfTracks()
        {
            List<Byte> readedMidi = this.readedMidiBytes;
            int result = readedMidi[10] * 256 + readedMidi[11];
            if(result < 1 || result > 65535)
            {
                Debugger.Log(2, null, "Nem megfelelő NumberOfTracks! " + result + "\n");
                throw new InvalidTrackChunkSizeException("Nem megfelelő NumberOfTracks! " + result);
            }
            this.numberOfTracks = result;
            this.nextUnprocessedByteIndex += 2;
        }

        /// <summary>
        /// Meghatározza, hogy a midin belül a deltaTime-ot hogy kell értelmezni. Ha a szám negatív, akkor időkódolással van megoldva. Ha a szám pozitív (tehát a legelső bit 0) akkor viszont metrikus idővel kell számolni. Az ezt követő 15 bit pedig maga a konkrét adat. 
        /// </summary>
        private void readTimeDivisionAndValue()
        {

            List<Byte> readedMidi = this.readedMidiBytes;
            Byte b1 = readedMidi[12];
            this.firstTimeDivison = b1;
            Byte b2 = readedMidi[13];
            this.lastTimeDivison = b2;
            if (b1 < 128) this.timeDivison = "metrical"; //Kisebb, mint 128, tehát az legfelsőbb bit 0
            else
            {
                this.timeDivison = "timecode";
                b1 -= 128;
            }
            this.timeDivisonValue = b1 * 256 + b2;
            this.nextUnprocessedByteIndex += 2;
        }


        /// <summary>
        /// Beolvassa a midi fejrészt, amely tartalmazza a fej chunk méretét, a midi fájl típusát, a track-ok számát, és az időosztást.
        /// </summary>
        /// <exception cref="InvalidHeaderChunkException">A beolvasott fájl nem rendelkezik megfelelő HeaderChunk aláírással</exception>
        /// <exception cref="UnsupportedFileException">A beolvasott fájl RIFF-alapú MIDI fájl. Ezt a típust a program nem tudja kezelni.</exception>
        /// <exception cref="InvalidHeaderChunkException"><see cref="readChunkSize()"/></exception>
        /// <exception cref="InvalidMIDITypeException"><see cref="readFormatType()"/></exception>
        /// <exception cref="InvalidTrackChunkSizeException"><see cref="readNumberOfTracks()"/></exception>
        private void readHeaderChunk()
        {
            if (!verifyHeader())
            {
                if (!isRIFFBased())
                {
                    Console.WriteLine("A beolvasott fájl nem érvényes MIDI fájl!");
                    throw new InvalidHeaderChunkException("A beolvasott fájl nem MIDI fájl, mert nem rendelkezik megfelelő HeaderChunk aláírással! ");
                }
                else
                {
                    Console.WriteLine("A beolvasott fájl RIFF-alapú MIDI fájl. Ezt a típust nem tudom kezelni.");
                    throw new UnsupportedFileException("A beolvasott fájl RIFF-alapú MIDI fájl. Ezt a típust nem tudom kezelni.");
                }
            }
            else
            {
                try
                {
                    readChunkSize();
                    readFormatType();
                    readNumberOfTracks();
                    readTimeDivisionAndValue();
                }
                catch (InvalidHeaderChunkException ex) { throw ex; }
                catch (InvalidMIDITypeException ex) { throw ex; }
                catch (InvalidTrackChunkSizeException ex) { throw ex; }
                catch (Exception)
                {
                    Console.WriteLine("Ismeretlen hiba történt! ");
                    Environment.Exit(-1);
                }
            }
        }
        #endregion

        #region Track-chunkokhoz kapcsolódó

        /// <summary>
        /// A Track Chunkok beolvasását végzi, ezt beállítja a MidiFile objektum <
        /// </summary>
        public void readTrackChunks()
        {
            TrackChunk tr = new TrackChunk(nextUnprocessedByteIndex, this);
            trackChunks.Add(tr);
            for (int i = 1; i < this.numberOfTracks; i++)
            {
                tr = tr.readNext();
                trackChunks.Add(tr);
            }
        }

        /// <summary>
        /// Ez az függvény igaz vagy hamis attól függően, hogy a jelenleg feldolgozatlan byte indexétől kezdődően új Track-ot definiálunk. A TrackChunk az "MTrk" karaktersorozattal kezdődik, hexadecimális formában 0x4D54726B.
        /// </summary>
        /// <returns>true, ha feldolgozatlan bájttól kezdődően az MTrk olvasható, hamis minden más esetben.</returns>
        public bool isTrackChunkNext()
        {
            HexMultiple hm = new HexMultiple("4D54726B");
            for (int i = 0; i < 4; i++) if (hm.Equals(this.readedMidiBytes[nextUnprocessedByteIndex + i])) return false;
            return true;
        }
        #endregion
    }
}
