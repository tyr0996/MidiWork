using System;
using System.Collections.Generic;
using System.Text;

namespace MidiWork
{
    class HexDouble
    {
        /// <summary>
        /// Az objektum első karaktere
        /// </summary>
        /// <example>0xF3 esetén 15</example>
        /// <example>0x52 esetén 5</example>
        private Byte first;

        /// <summary>
        /// Az objektum második karaktere
        /// </summary>
        /// <example>0xF3 esetén 3</para>
        /// <para>0x52 esetén 2</para>
        /// <para>0x4E esetén 14</example>
        private Byte second;

        public static implicit operator HexDouble(byte b) => new HexDouble(b);

        /// <summary>
        /// Létrehoz egy HexDouble értéket.
        /// </summary>
        /// <param name="completeByte">Objektum értéke</param>
        public HexDouble(Byte completeByte)
        {
            this.first = (Byte) (completeByte / 16);
            this.second = (Byte) (completeByte % 16);
        }

        /// <summary>
        /// Létrehoz egy HexDouble értéket.
        /// </summary>
        /// <param name="first">felső 4 bit</param>
        /// <param name="second">alsó 4 bit</param>
        /// <exception cref="ArgumentOutOfRangeException">Ha a <paramref name="first"/> vagy a <paramref name="second"/> negatív, vagy 16 (vagy annál nagyobb)</exception>
        public HexDouble(Byte first, Byte second)
        {
            if (first >= 16 || second >= 16 || first < 0 || second < 0) throw new ArgumentOutOfRangeException(String.Format("A HexDouble létrehozása sikertelen volt, mert nem megfelelő értéket kapott paraméterként! \n param1={0}, param2={1}", first, second));
            this.first = first;
            this.second = second;
        }


        /// <summary>
        /// Stringként megadott két byte-ot (<paramref name="hex"/>) alakítja át <see cref="HexDouble"/> elemmé. 
        /// </summary>
        /// <param name="hex"></param>
        public HexDouble(String hex)
        {
            byte first;
            byte second;
            switch (hex[0]) {
                case '0' : first = 0; break;
                case '1': first = 1; break;
                case '2': first = 2; break;
                case '3': first = 3; break;
                case '4': first = 4; break;
                case '5': first = 5; break;
                case '6': first = 6; break;
                case '7': first = 7; break;
                case '8': first = 8; break;
                case '9': first = 9; break;
                case 'A': first = 10; break;
                case 'B': first = 11; break;
                case 'C': first = 12; break;
                case 'D': first = 13; break;
                case 'E': first = 14; break;
                case 'F': first = 15; break;
                default:  first = Byte.MaxValue; break;
            }
            switch (hex[1])
            {
                case '0': second = 0; break;
                case '1': second = 1; break;
                case '2': second = 2; break;
                case '3': second = 3; break;
                case '4': second = 4; break;
                case '5': second = 5; break;
                case '6': second = 6; break;
                case '7': second = 7; break;
                case '8': second = 8; break;
                case '9': second = 9; break;
                case 'A': second = 10; break;
                case 'B': second = 11; break;
                case 'C': second = 12; break;
                case 'D': second = 13; break;
                case 'E': second = 14; break;
                case 'F': second = 15; break;
                default: second = Byte.MaxValue; break;
            }

            if (second != Byte.MaxValue && first != Byte.MaxValue)
            {
                this.first = first;
                this.second = second;
            }
            else
            {
                Console.WriteLine("Nem sikerült a konverzió! Nem várt karakter! ");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Az objektum egészértékét határzza meg. 
        /// </summary>
        /// <returns>egészérték</returns>
        public int getIntValue() => this.first* 16 + this.second;

        /// <summary>
        /// Az objektum bájtértékét határzza meg. 
        /// </summary>
        /// <returns>bájtérték</returns>
        public byte getByteValue() => (Byte) this.getIntValue();

        /// <summary>
        /// megnézi, hogy két HexDouble ugyanolyan értékű-e
        /// </summary>
        /// <param name="h"></param>
        /// <returns><c>true</c>, ha megegyezik a kettő, <c>false</c> ha nem</returns>
        public bool Equals(HexDouble h) => (h.first == this.first && h.second == this.second);

    }
}
