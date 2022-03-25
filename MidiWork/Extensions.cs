using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MidiWork
{
    public static class Extensions
    {
        #region T[] extensions

        /// <summary>
        /// Egy Tömb egy résztömbjét adja vissza, amely a megadott offset-től (kezdőindex-től) indul és adott elemszámú
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="offset">kezdőérték</param>
        /// <param name="len">hossz</param>
        /// <returns>megfelelő résztömb</returns>
        public static T[] SubArray<T>(this T[] array, int offset, int len)
        {
			//TODO: ellenőrizni, hogy a tömb mérete nagyobb-e mint az offset és a len összege. Ellenkező esetben ArgumentExceptionOutOfINdex-t kell dobmi (vagy valammi hasonlót)
            return array.Skip(offset).Take(len).ToArray();
        }

        /// <summary>
        /// Egy tömb egy olyan résztömbjét adja eredményül amely a megadott <paramref name="offset"/>-től (kezdőindex-től) indul és az eredeti tömb végéig tart.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="offset">kezdőérték</param>
        /// <returns>megfelelő résztömb</returns>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] array, int offset)
        {
			//TODO: ellenőrizni, hogy a tömb mérete nagyobb-e mint az offset. Ellenkező esetben ArgumentExceptionOutOfINdex-t kell dobmi (vagy valammi hasonlót)
            return array.Skip(offset).Take(array.Length - offset).ToArray();
        }

        #endregion
        #region List<T> extensions

        /// <summary>
        /// Egy Tömb egy résztömbjét adja vissza, amely a megadott offset-től (kezdőindex-től) indul és adott elemszámú
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="offset">kezdőérték</param>
        /// <param name="len">hossz</param>
        /// <returns>megfelelő résztömb</returns>
        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> array, int offset, int len)
        {
            //TODO: ellenőrizni, hogy a tömb mérete nagyobb-e mint az offset és a len összege. Ellenkező esetben ArgumentExceptionOutOfINdex-t kell dobmi (vagy valammi hasonlót)
            return array.Skip(offset).Take(len);
        }


        /// <summary>
        /// Egy IEnumerable egy olyan részIEnumerable-t adja eredményül amely a megadott offset-től (kezdőindex-től) indul és az eredeti IEnumerable végéig tart.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="offset">kezdőérték</param>
        /// <returns>megfelelő résztömb</returns>
        /// <returns></returns>
        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> array, int offset)
        {
            //TODO: ellenőrizni, hogy a tömb mérete nagyobb-e mint az offset. Ellenkező esetben ArgumentExceptionOutOfINdex-t kell dobmi (vagy valammi hasonlót)
            return array.Skip(offset).Take(array.Count() - offset);
        }

        /// <summary>
        /// Egy IEnumerable<T>-hez hozzáadja az összes elemet egy másik, azonos típusú IEnumerable-ből. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">Ahova hozzá akarjuk adni</param>
        /// <param name="toAdd">Aminek az elemeit hozzá akarjuk adni</param>
        /// <returns></returns>
        public static IEnumerable<T> AddAll<T>(this IEnumerable<T> array, IEnumerable<T> toAdd)
        {
            foreach (T element in toAdd) array.ToList().Add(element);
            return array;
        }
        /// <summary>
        /// Egy IEnumerable<T>-hez hozzáadja az összes elemet egy másik, azonos típusú IEnumerable-ből. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">Ahova hozzá akarjuk adni</param>
        /// <param name="toAdd">Azok az elemek, amiket hozzá akarunk adni felsorolásszerűen. </param>
        /// <returns></returns>
        public static IEnumerable<T> AddAll<T>(this IEnumerable<T> array, params T[] toAdd)
        {
            foreach (T element in toAdd) array.ToList().Add(element);
            return array;
        }

        #endregion
        #region byte[] extensions

        /// <summary>
        /// Egy byte[] típusú változónak kiszámítja az int típusú értékét. 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int IntValue(this byte[] array)
        {
            int value = 0;
            int fok = 0;
            for(int i = array.Length-1; i >= 0; i--)
            {
                value += array[i] * (int)(Math.Pow(256, fok));
                fok++;
            }
            return value;
        }

        /// <summary>
        /// Egy byte típusú tömböt egészít ki megadott <paramref name="numberOfZeroBytes"/> darabszámú 0-val, úgy hogy az egész értéke ne változzon.   
        /// </summary>
        /// <param name="array"></param>
        /// <param name="numberOfZeroBytes">Az elejére hozzáfűzendő 0-k száma</param>
        /// <returns></returns>
        public static Byte[] ExtendWith(this Byte[] array, int numberOfZeroBytes)
        {
            List<Byte> newList = new List<Byte>();
            for (int i = 0; i < numberOfZeroBytes; i++) newList.Add(0);
            for (int i = 0; i < array.Length; i++) newList.Add(array[i]);
            return newList.ToArray();
        }

        /// <summary>
        /// Egy byte típusú tömböt egészít ki valamennyi 0-val, úgy hogy az egész értéke ne változzon, és a hossza <paramref name="size"/> legyen
        /// </summary>
        /// <param name="array"></param>
        /// <param name="size">Az elérni kívánt méret. </param>
        /// <returns></returns>
        public static Byte[] ExtendTo(this Byte[] array, int size)
        {
            List<Byte> newList = new List<Byte>();
            for (int i = 0; i < array.Length; i++) newList.Add(array[i]);
            for (int i = 0; i < size - array.Length; i++) newList.Add(0);
            
            return newList.ToArray();
        }
        #endregion
        #region BitArray extensions
		
		//TODO: a következőket lehet optimalizálni: Ha olyan a bit array, hogy 00001000, akkor ennek megfelelően carry nélkül a 00000000-t már ne lehessen shiftelni, hanem egyszerűen tájékoztassa a felhasználót, és adja vissza az eredetit. 

        /// <summary>
        /// BitArray kettővel való szorzása (jobbra Shiftelése)
        /// </summary>
        /// <param name="ba"></param>
        /// <returns>BitArray kettővel szorzott eredménye</returns>
		public static BitArray ShiftRight(this BitArray ba)
        {
			//TODO: Carry?
            bool[] newArr = new bool[(ba.Count)];
            for (int i = 0; i < ba.Count - 1; i++) newArr[i + 1] = ba[i];
            return new BitArray(newArr);
        }

        /// <summary>
        /// BitArray kettővel való osztása (balra Shiftelése)
        /// </summary>
        /// <param name="ba"></param>
        /// <returns>BitArray kettővel osztott eredménye</returns>
        public static BitArray ShiftLeft(this BitArray ba)
        {
			//TODO: Carry?
            bool[] newArr = new bool[(ba.Count)];
            for (int i = 0; i < ba.Count - 1; i++) newArr[i] = ba[i + 1];
            return new BitArray(newArr);
        }


        /// <summary>
        /// Megadott számú jobbra-shiftelést hajt végre (2 hatványokkal való szorzás)
        /// </summary>
        /// <param name="ba"></param>
        /// <param name="number">lépések száma</param>
        /// <returns>BitArray 2^(<paramref name="number"/>)-el szorzott változata</returns>
        public static BitArray ShiftRight(this BitArray ba, int number){
			//TODO: Carry?
			//TODO: if !Carry, then number can be infinity else 0<=number<=8
			for(int i = 0; i < number; i++) ba = ba.ShiftRight();
			return ba;
		}

        /// <summary>
        /// Megadott számú balra-shiftelést hajt végre (2 hatványokkal való osztás)
        /// </summary>
        /// <param name="ba"></param>
        /// <param name="number">lépések száma</param>
        /// <returns>BitArray 2^(<paramref name="number"/>)-el osztott változata</returns>
		public static BitArray ShiftLeft(this BitArray ba, int number){
			//TODO: Carry?
			//TODO: if !Carry, then number can be infinity else 0<=number<=8
			for(int i = 0; i < number; i++) ba = ba.ShiftLeft();
			return ba;
		}


        /// <summary>
        /// Egy byte-nak megfelelő BitArray-nek számolja ki a byte-beli értékét. 
        /// </summary>
        /// <param name="bits"></param>
        /// <returns>8 elemű BitArray-ben tárolt bájt érték</returns>
        /// <exception cref="ArgumentException">Nem megfelelő méretű a BitArray (tehát nem 8 bit van benne)</exception>
        public static byte ToByte(this BitArray bits)
        {
			//TODO: kivételdobás helyett meg lehetne próbálni a "kiegészítést". Ezt lehet, hogy nekem kell megírni, de ha jól emlékszem akkor van beépítve a BitArray-ben. 
            if (bits.Count != 8) throw new ArgumentException("A BitArray mérete nem megfelelő: A konvertáláshoz pontosan 8 bitet tartalmazó BitArray kell! ");
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }

        /// <summary>
        /// Egy byte-nak megfelelő BitArray-nek számolja ki a int-beli értékét. 
        /// </summary>
        /// <param name="bits"></param>
        /// <returns>BitArray-ben tárolt egész érték/returns>
        public static int getIntValue(this BitArray bits)
        {
            int value = 0;
            for(int i = 0; i < bits.Count; i++) if (bits[i]) value += (int)Math.Pow(2, i);
            return value;
        }


        public static bool isAllBetween(this BitArray bits, int i0, int i1, bool value)
        {
            for(int i = i0; i <= i1; i++) if (bits[i] != value) return false;

            return true;
        }
        #endregion
        #region Byte-extensions

        /// <summary>
        /// Ez a metódus azt csinálja, hogy kettéveszi a Byte-ot az alsó és a felső 4 bájtjára úgy hogy 2 darab 8-8 bitet tartalmazó BitArray-t ad vissza. Az első visszatérő bájt a jobb oldal, a második a bal. 
        /// </summary>
        /// <param name="toCut">Az a bájt, amit félbe akarunk vágni</param>
        /// <returns>Kettévágott két bájt</returns>
        public static byte[] cutInHalf(this Byte toCut)
        {
            BitArray ba = new BitArray(new byte[] { toCut }); //első négy bájtot hagyjuk meg belőle
            BitArray bb = new BitArray(new byte[] { toCut }); //lemásoljuk az eredetit, és ebből pedig az utolsó négy bájtot fogjuk meghagyni

            bb[0] = false;
            bb[1] = false;
            bb[2] = false;
            bb[3] = false;

            ba[4] = false;
            ba[5] = false;
            ba[6] = false;
            ba[7] = false;

            bb = bb.ShiftLeft().ShiftLeft().ShiftLeft().ShiftLeft();

            return new byte[] { bb.ToByte(), ba.ToByte() };
        }

        /// <summary>
        /// Egy bájt értékét váltja át úgy, hogy az tulajdonképpen egy előjeles byte
        /// </summary>
        /// <param name="b"></param>
        /// <returns>A bájt előjeles bájt megfelelője</returns>
        public static SByte forceSigned(this Byte b)
        {
            //művelet: Gimesi könyve
            BitArray ba = new BitArray(new byte[] { b });
            for(int i = 0; i < 8; i++) ba[i] = !ba[i];
            return (sbyte) (-1 * (ba.ToByte() + 1));
        }
        #endregion
        #region intExtensions

        /// <summary>
        /// Egy int érték bájtokban vett értékét adja meg úgy, hogy azok legfelső bitjei jelzőbitek, amik azt jelentik, hogy a következő byte még része-e a mezőnek.
        /// </summary>
        /// <param name="value">Az érték, amit változó hosszúságú adatként szeretnénk eltárolni</param>
        /// <returns>Változó hosszúságú adat bájtonként. </returns>
        public static byte[] toVariableLength(this int value)
        {
            List<Byte> result = new List<byte>();
            while(value != 0)
            {
                result.Add((byte)(value % 128));
                value /= 128;
            }
            result.Reverse();
            return result.ToArray();
        }
        #endregion
    }
}
