using System;
using System.Collections.Generic;
using System.Text;

namespace MidiWork
{
    class HexMultiple
    {
        /// <summary>
        /// Az objektumban tárolt byte-ok.
        /// </summary>
        List<Byte> hexes = new List<Byte>();

        /// <summary>
        /// Egy adott hexadecimális Stringből létrehoz egy HexMultiple objektumot. 
        /// </summary>
        /// <param name="hexString"></param>
        public HexMultiple(String hexString)
        {
            if(hexString.Length % 2 == 1)
            {
                Console.WriteLine("A megadott hexString nem megfelelő hosszúságú!" + hexString);
                Environment.Exit(1);
            }

            for (int i = 0; i < hexString.Length; i = i+2) this.hexes.Add(new HexDouble(hexString.Substring(i, 2)).getByteValue());
        }

        /// <summary>
        /// A <paramref name="byteArray"/>-t hasonlítja össze a több Hex-at tartalmazó objektummal. Ellenőrzi a méreteket is. 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns><c>true</c>, ha a <paramref name="byteArray"/> értéke megegyezik a HexMultiple-ben tárolt értékkel, ellenkező esetben <c>false</c></returns>
        public bool Equals(byte[] byteArray)
        {
            if (byteArray.Length != this.hexes.Count) return false;
            for(int i = 0; i < this.hexes.Count; i++) if (this.hexes[i] != byteArray[i]) return false;
            return true;
        }
    }
}
