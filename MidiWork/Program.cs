using MidiWork.Event;
using MidiWork.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MidiWork
{
    class Program
    {
        /// <summary>
        /// A program belépési pontja. 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            MidiFile f = MidiFile.load(@"C:\Users\Martin\Desktop\Argument out of range\midi (146).mid");
            //testRun(4472);
            readFile("asdf");
        }


        /// <summary>
        /// Midi fájlokat olvas be a <paramref name="folderPath"/> mappából, melynek az indexei a <paramref name="from"/> és a <paramref name="to"/> közöttiek.
        /// </summary>
        /// <param name="from">az első index</param>
        /// <param name="to">az utolsó index</param>
        /// <param name="folderPath">midiket tartalmazó mappa elérési útja</param>
        /// <example> <code> testRun("C:\Users\midi", 1, 5); </code> beolvassa az 1-es, 2-es, 3-as, 4-es, 5-ös midiket </example>
        /// <example> <code> testRun("C:\Users\midi", 3, 7); </code> beolvassa az 3-as, 4-es, 5-ös, 6-os, 7-es midiket </example>
        public static void testRun(string folderPath, int from, int to)
        {
            bool[] siker = new bool[to];
            string[] hiba = new string[to];
            int sikeres = 0;
            int sikertelen = 0;

            for (int i = from; i <= to; i++)
            {
                try
                {
                    MidiFile mf = MidiFile.load(folderPath + "\\midi (" + i + ").mid");
                    siker[i - 1] = true;
                    hiba[i - 1] = null;
                    sikeres++;
                }
                catch(InvalidMIDITypeException)
                {
                    siker[i - 1] = false;
                    hiba[i - 1] = "InvalidMIDITypeException";
                    sikertelen++;
                    Console.WriteLine("midi " + i + " beolvasása sikertelen volt. Érvénytelen a MIDI típusa a HeaderChunk-ben!");
                }
                catch (EndOfFileException)
                {
                    siker[i - 1] = false;
                    hiba[i - 1] = "End of File Exception";
                    sikertelen++;
                    Console.WriteLine("midi " + i + " beolvasása sikertelen volt. A midi váratlanul véget ért!");
                }
                catch (KeyNotFoundException)
                {
                    siker[i - 1] = false;
                    sikertelen++;
                    hiba[i - 1] = "Key Not Found Exception";
                    Console.WriteLine("midi " + i + " beolvasása sikertelen volt!\nKeyNotFound");
                }
                catch (InvalidHeaderChunkException)
                {
                    siker[i - 1] = false;
                    sikeres++;
                    hiba[i - 1] = "Nem rendelkezik érvényes headerrel! ";
                    Console.WriteLine("midi " + i + " beolvasása sikertelen volt. Nem rendelkezik érvényes headerrel");
                }
                catch(UnsupportedFileException)
                {
                    siker[i - 1] = false;
                    sikeres++;
                    hiba[i - 1] = "Nem támogatott formátum (RIFF)";
                    Console.WriteLine("midi " + i + " beolvasása sikertelen volt. Nem támogatott formátum (RIFF)");
                }
                catch (DirectoryNotFoundException)
                {
                    siker[i - 1] = false;
                    sikertelen++;
                    hiba[i - 1] = "A megadott mappa nem létezett!";
                }
                catch (FileNotFoundException)
                {
                    siker[i - 1] = false;
                    sikertelen++;
                    hiba[i - 1] = "A megadott fájl nem létezett!";
                }
                catch (IOException)
                {
                    siker[i - 1] = false;
                    sikertelen++;
                    hiba[i - 1] = "A megadott fájlt nem lehet megnyitni, mert az használatban volt!";
                }
            }

            String sikertelenS = "";
            for (int i = from; i <= to; i++) if (!siker[i - 1]) sikertelenS += i + ";  " + hiba[i - 1] + "\n";
            Console.WriteLine("Összes: " + (int)((int)to - (int)from) + ";  Sikeres: " + sikeres + ";  Sikertelen: " + sikertelen + "\n");
            Console.WriteLine("Sikertelen fájlok sorszáma: \n" + sikertelenS);
            Console.WriteLine("Hibaarány: " + (((double)sikertelen / (double)((double)sikertelen + (double)sikeres))) * (double)100);
        }

        /// <summary>
        /// Azonos a <seealso cref="testRun(string, int, int)"/> metódussal, csak az elsőtöl a <paramref name="numberOfMidis"/>-ig fut.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="numberOfMidis"></param>
        public static void testRun(String folderPath, int numberOfMidis)
        {
            testRun(folderPath, 1, numberOfMidis);
        }

        /// <summary>
        /// Ez a metódus beolvassa a fájlt amit megadtunk neki byte-onként majd egy byte-tömbként adja vissza.
        /// </summary>
        /// <param name="filePath">A Midi fájl teljes elérési útja</param>
        /// <returns>A fájl tartalma byte-onként kifejezve</returns>
        /// <exception cref="DirectoryNotFoundException">
        /// <paramref name="filePath"/> mappája nem létezik, vagy nem elérhető.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// <paramref name="filePath"/> fájl nem létezik. 
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// <paramref name="filePath"/> mappája nem létezik, vagy nem elérhető.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="filePath"/> fájl jelenleg nem nyitható meg, mert valószínű egy másik program használja. 
        /// </exception>
        public static List<Byte> readFile(String filePath)
        {            
            List<Byte> byteList = new List<byte>();
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    if (Path.GetExtension(fs.Name) != ".midi" && Path.GetExtension(fs.Name) != ".mid")
                    {
                        throw new ArgumentException("A megadott fájl nem midi kiterjesztéssel rendelkezik!\nA program kilép!");
                    }
                    Int32 readedByte = fs.ReadByte(); //FileStream.ReadByte metódus dokumentációjából vettem, hogy -1, ha végigértem a fájlon
                    while (readedByte != -1) //ameddig tudunk olvasni a fájlból
                    {
                        byteList.Add(Convert.ToByte(readedByte));
                        readedByte = fs.ReadByte();
                    }
                }
                return byteList;
            }
            catch(DirectoryNotFoundException ex)
            {
                Console.WriteLine("A megadott mappa nem létezik!");
                throw ex;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("A megadott fájl nem létezik!");
                throw ex;
            }
            catch (IOException ex)
            {
                Console.WriteLine("A megadott fájlt nem lehet megnyitni, mert jelenleg használatban van!");
                throw ex;
            }
        }
    }
}
