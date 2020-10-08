using System;
using System.Collections.Generic;

namespace EncryptionApp
{
    public static class Encryption
    {
        /// <summary>
        /// Encrypts a string following the Cesar Encryption Method.
        /// Takes account for alphabet only(Doesn't account for numbers, Special chars [' " (...)"'],Special alphabet ûÜîë...
        /// </summary>
        /// <param name="str">String to Encrypte</param>
        /// <param name="offset">Offset added to each char</param>
        /// <returns>String that contains the Cesar Ecryption by an offset to the String</returns>
        public static string CesarEncryption(string str, int offset = 1)
        {
            // we do not hardcode values instead we want to be able to change them easly :: NO MAGIC NUMBERS
            const int MAX_UTF8_VALUE = 122; // z value on UTF8
            const int MIN_UTF8_VALUE = 97; // a value on UTF8
            const int MAX_UPPER_UTF8_VALUE = 90; //Z value on UTF8
            const int MIN_UPPER_UTF8_VALUE = 65; // A value on UTF8

            string result = null; // final result will be stored here
            int diffMinMax = (MAX_UTF8_VALUE - MIN_UTF8_VALUE + 1);
            //removing loops on the offset (if it's 26 OR -26 then might as well just be 0)
            if ((int)Math.Abs(offset) > diffMinMax)
            {
                offset = RemoveLoops(offset, diffMinMax);
            }
            //Processing encryption
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] <= MAX_UTF8_VALUE && str[i] >= MIN_UTF8_VALUE)
                {
                    //offseting the lower chars 
                    if (str[i] + offset > MAX_UTF8_VALUE)
                    {
                        result = result + (char)(MIN_UTF8_VALUE + (str[i] + offset - MAX_UTF8_VALUE - 1));
                    }
                    else if (str[i] + offset < MIN_UTF8_VALUE)
                    {
                        result = result + (char)(MAX_UTF8_VALUE + (str[i] + offset - MIN_UTF8_VALUE + 1));
                    }
                    else
                    {
                        result = result + (char)(str[i] + offset);
                    }
                }
                else if (str[i] <= MAX_UPPER_UTF8_VALUE && str[i] >= MIN_UPPER_UTF8_VALUE)
                {
                    //offseting the upper chars 
                    if (str[i] + offset > MAX_UPPER_UTF8_VALUE)
                    {
                        result = result + (char)(MIN_UPPER_UTF8_VALUE + (str[i] + offset - MAX_UPPER_UTF8_VALUE - 1));
                    }
                    else if (str[i] + offset < MIN_UPPER_UTF8_VALUE)
                    {
                        result = result + (char)(MAX_UPPER_UTF8_VALUE + (str[i] + offset - MIN_UPPER_UTF8_VALUE + 1));
                    }
                    else
                    {
                        result = result + (char)(str[i] + offset);
                    }
                }
                // if the char is out of our bounds then we go ahead and store it as is
                else
                {
                    result = result + str[i];
                }
            }
            // returning result
            return result;
        }

        /// <summary>
        /// Encrypts a string following a string key Encryption Method
        /// Takes account for alphabet only(Doesn't account for numbers, Special chars [' " (...)"'],Special alphabet ûÜîë...
        /// </summary>
        /// <param name="str">The string to encrypt</param>
        /// <param name="encryptionKey">The key</param>
        /// <param name="encrypt">If true encrypt, if false decrypt</param>
        /// <returns>an encrypted string by a string key</returns>
        public static string TextBasedEncryption(string str, string encryptionKey, bool encrypt = true)
        {   
            const int MAX_UTF8_VALUE = 122; // z value on UTF8
            const int MIN_UTF8_VALUE = 97; // a value on UTF8
            const int MAX_UPPER_UTF8_VALUE = 90; //Z value on UTF8
            const int MIN_UPPER_UTF8_VALUE = 65; // A value on UTF8

            string result = null; // final result will be sotred here
            int currentKeyIndex = 0; // current index of the char used to encrypt the current char
            int currentValue; // current char index of str beign encrypted
            int offset = 0; // the offset that currentValue will be encrypted with

            encryptionKey = encryptionKey.ToLower(); // Logic ins't made to support upper case keys, though it doesn't
                                                     //matter because A = 1 && a = 1 in the current logic
            encryptionKey = encryptionKey.Replace(" ", "");
            //processing encryption
            for (int i = 0; i < str.Length; i++)
            {
                //if we reached the end of the key we go back to the first char
                if (currentKeyIndex > encryptionKey.Length - 1)
                {
                    currentKeyIndex = 0;
                }
                //getting the primary offset, this should always be between 0 && 25
                offset = (int)encryptionKey[currentKeyIndex] - (MIN_UTF8_VALUE - 1);
                //if the current Char is a char (wich means we wanna encrypt it) we can process
                if ((str[i] <= MAX_UTF8_VALUE && str[i] >= MIN_UTF8_VALUE) ||
                   (str[i] <= MAX_UPPER_UTF8_VALUE && str[i] >= MIN_UPPER_UTF8_VALUE))
                {
                    //Current state of the char, just to keep the same Case in result
                    bool currentIsUpper = Char.IsUpper(str[i]);
                    if (currentIsUpper)
                    {
                        //encryption logic for upper (Same as Lower except we change the final result to and Upper case)
                        currentValue = str[i] - MIN_UPPER_UTF8_VALUE;
                        if (currentValue + offset > 25)
                        {
                            offset = (currentValue + offset) - 25;
                            if (encrypt)//if encrypt then encrypt 
                                result = result + char.ToUpper((char)(MIN_UTF8_VALUE + offset - 1));
                            else // else decrypt
                                result = result + char.ToUpper((char)(MIN_UTF8_VALUE - offset - 1));
                        }
                        else
                        {
                            if (encrypt)
                                result = result + char.ToUpper(char.ToLower((char)(str[i] + offset)));
                            else
                                result = result + char.ToUpper(char.ToLower((char)(str[i] - offset)));
                        }
                    }
                    else
                    {
                        //encryption logic for lowercase chars
                        currentValue = str[i] - MIN_UTF8_VALUE;
                        if (currentValue + offset > 25)
                        {
                            offset = (currentValue + offset) - 25;
                            if (encrypt)
                                result = result + (char)(MIN_UTF8_VALUE + offset - 1);
                            else
                                result = result + (char)(MIN_UTF8_VALUE - offset - 1);
                        }
                        else
                        {
                            if (encrypt)
                                result = result + (char)(str[i] + offset);
                            else
                                result = result + (char)(str[i] - offset);
                        }
                    }
                }
                //if the char isn't an alphabet char we dont encrypt it, we add it to result as is
                else
                {
                    result = result + str[i];
                }
                //next char in the key as blank spaces aren't taken in account for this method we dont do that here as well !
                if (str[i] != ' ')
                    currentKeyIndex++;
            }
            //return result
            return result;
        }

        /// <summary>
        /// Encrypts a string following the substitution method
        /// Takes account  for alphabet only(Doesn't account for numbers, Special chars [' " (...)"'],Special alphabet ûÜîë...
        /// </summary>
        /// <param name="str">The string to enctypt</param>
        /// <param name="chars">List containing the chars to substitute with from a to z</param>
        /// <returns></returns>
        public static string SubstitutionEncryption(string str, List<char> chars)
        {
            //List MUST contain 26 chars
            if (chars.Count != 26)
            {
                throw new Exception("List of 26 chars Expected");
            }

            string result = null; //result will stored here

            //setting chars to lower for simlicity 
            for (int i = 0; i < chars.Capacity; i++)
            {
                chars[0] = char.ToLower(chars[0]);
            }
            //Encryption process, pretty simple (and ultra ugly implemtation i guess but it works so !) foreach char set it to 
            // the char in the list (that's why we need 26 chars size !) while keeping it's case
            #region ProcessEncryption
            foreach (char character in str)
            {
                bool isUpperCase = char.IsUpper(character);
                switch (char.ToLower(character))
                {
                    case 'a':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[0]);
                        }
                        else
                        {
                            result = result + chars[0];
                        }
                        break;
                    case 'b':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[1]);
                        }
                        else
                        {
                            result = result + chars[1];
                        }
                        break;
                    case 'c':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[2]);
                        }
                        else
                        {
                            result = result + chars[2];
                        }
                        break;
                    case 'd':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[3]);
                        }
                        else
                        {
                            result = result + chars[3];
                        }
                        break;
                    case 'e':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[4]);
                        }
                        else
                        {
                            result = result + chars[4];
                        }
                        break;
                    case 'f':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[5]);
                        }
                        else
                        {
                            result = result + chars[5];
                        }
                        break;
                    case 'g':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[6]);
                        }
                        else
                        {
                            result = result + chars[6];
                        }
                        break;
                    case 'h':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[7]);
                        }
                        else
                        {
                            result = result + chars[7];
                        }
                        break;
                    case 'i':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[8]);
                        }
                        else
                        {
                            result = result + chars[8];
                        }
                        break;
                    case 'j':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[9]);
                        }
                        else
                        {
                            result = result + chars[9];
                        }
                        break;
                    case 'k':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[10]);
                        }
                        else
                        {
                            result = result + chars[10];
                        }
                        break;
                    case 'l':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[11]);
                        }
                        else
                        {
                            result = result + chars[11];
                        }
                        break;
                    case 'm':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[12]);
                        }
                        else
                        {
                            result = result + chars[12];
                        }
                        break;
                    case 'n':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[13]);
                        }
                        else
                        {
                            result = result + chars[13];
                        }
                        break;
                    case 'o':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[14]);
                        }
                        else
                        {
                            result = result + chars[14];
                        }
                        break;
                    case 'p':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[15]);
                        }
                        else
                        {
                            result = result + chars[15];
                        }
                        break;
                    case 'q':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[16]);
                        }
                        else
                        {
                            result = result + chars[16];
                        }
                        break;
                    case 'r':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[17]);
                        }
                        else
                        {
                            result = result + chars[17];
                        }
                        break;
                    case 's':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[18]);
                        }
                        else
                        {
                            result = result + chars[18];
                        }
                        break;
                    case 't':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[19]);
                        }
                        else
                        {
                            result = result + chars[19];
                        }
                        break;
                    case 'u':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[20]);
                        }
                        else
                        {
                            result = result + chars[20];
                        }
                        break;
                    case 'v':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[21]);
                        }
                        else
                        {
                            result = result + chars[21];
                        }
                        break;
                    case 'w':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[22]);
                        }
                        else
                        {
                            result = result + chars[22];
                        }
                        break;
                    case 'x':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[23]);
                        }
                        else
                        {
                            result = result + chars[23];
                        }
                        break;
                    case 'y':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[24]);
                        }
                        else
                        {
                            result = result + chars[24];
                        }
                        break;
                    case 'z':
                        if (isUpperCase)
                        {
                            result = result + char.ToUpper(chars[25]);
                        }
                        else
                        {
                            result = result + chars[25];
                        }
                        break;
                    default:
                        result = result + character;
                        break;
                }
            }
            #endregion
            //return result
            return result;
        }

        /// <summary>
        /// Types used in Encryption.PolybeEncryption
        /// </summary>
        public enum PolybeType
        {
            LineColumn,
            ColumnLine
        }

        /// <summary>
        /// Encrypts a string following Polybe Method
        /// Takes account for all character defined in the sent table OR alphabet as default
        /// </summary>
        /// <param name="str">String to Encrypt</param>
        /// <param name="table">Table of Encryption, default 6x6 with alphabet only</param>
        /// <param name="type">Encryption Type Display</param>
        /// <returns></returns>
        public static string PolybeEncryption(string str, char [,] table = null, PolybeType type = PolybeType.LineColumn)
        {
            string result = null;//result will be stored here
            //Creating a default table of Polybe takes care of alphabet only
            if (table == null)
            {
                char[,] tableOfChars =
                {
               {'a','b','c','d','e','f'},
               {'g','h','i','j','k','l'},
               {'m','n','o','p','q','r'},
               {'s','t','u','v','w','x'},
               {'y','z', '\0','\0','\0','\0'}
                };
                table = tableOfChars;
            }
            //processing encryption getting i and j of each char that exist in table
            foreach (char character in str)
            {
                for (int i = 0; i < table.GetLength(0); i++)
                {
                    for (int j = 0; j < table.GetLength(1); j++)
                    {
                        if (char.ToLower(character).Equals(table[i, j]))
                        {
                            if (type.Equals(PolybeType.LineColumn))
                            {
                                result = result + (i + 1) + (j + 1);
                                break;
                            }
                            else if(type.Equals(PolybeType.ColumnLine))
                            {
                                result = result + (j + 1) + (i + 1);
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Available methods for a Binary Encryption
        /// </summary>
        public enum ToBinaryType
        {
            Normal,
            CP1,
            CP2
        }

        /// <summary>
        /// Encrypts a string by using it's UTF-8 Value and change it to it's binary value with diffrent methods
        /// Takes account for all characters that exist in UTF-8
        /// </summary>
        /// <param name="str">The String to Encrypt</param>
        /// <param name="toBinaryType">Method used to change the UTF-8 Value to Binary</param>
        /// <param name="fillBits">Number of 0 to get a certain length</param>
        /// <param name="seperator">A string between each Character</param>
        /// <returns></returns>
        public static string ToBinaryEncryption(string str, ToBinaryType toBinaryType = ToBinaryType.Normal, int fillBits = 0,
            string seperator = ",")
        {
            string result = null;

            foreach (char character in str)
            {
                string convertedCharacter = IntegerToBinary(Convert.ToInt32(character), toBinaryType);
                while(convertedCharacter.Length < fillBits)
                {
                    convertedCharacter = '0' + convertedCharacter;
                }
                result = result + convertedCharacter + seperator;
            }
            return result;
        }

        public enum ToHashCodeType
        {
            T1,
            T2,
            T3
        }

        public static string ToHashCode(string str)
        {
            throw new NotImplementedException();
        }

        static long GetGCD(long number1, long number2)
        {
            while (number1 != 0 && number2 != 0)
            {
                if (number1 > number2)
                {
                    number1 %= number2;
                }
                else
                {
                    number2 %= number1;
                }
            }

            return number1 == 0 ? number2 : number1;
        }

        static string IntegerToBinary(int number, ToBinaryType type)
        {
            string result = null;
            switch (type)
            {
                case ToBinaryType.Normal:
                    result = Convert.ToString(number, 2);
                    break;
                case ToBinaryType.CP1:
                    result = Convert.ToString(number, 2);
                    char[] resultToArray = result.ToCharArray();
                    for (int i = 0; i < resultToArray.Length; i++)
                    {
                        if (resultToArray[i].Equals('1'))
                        {
                            resultToArray[i] = '0';
                        }
                        else
                        {
                            resultToArray[i] = '1';
                        }
                    }
                    result = new string(resultToArray);
                    break;
                case ToBinaryType.CP2:
                    result = Convert.ToString(number, 2);
                    char[] toArray = result.ToCharArray();
                    for (int i = 0; i < toArray.Length; i++)
                    {
                        if (toArray[i].Equals('1'))
                        {
                            toArray[i] = '0';
                        }
                        else
                        {
                            toArray[i] = '1';
                        }
                    }                 
                    result = new string(toArray);
                    result = Convert.ToString(Convert.ToInt32(result, 2) + 1, 2);
                    break;
            }
            return result;
        }

        static int RemoveLoops(int number, int loop)
        {
            var numberOfLoops = Math.Abs(number) / loop; //Number of useless loops;
            for (int i = 0; i < numberOfLoops; i++)
            {
                if (number > 0) // if number is POSITIVE then we decrease it's value
                {
                    number -= loop;
                }
                else // if number is NEGATIVE then we increase it's value instead
                {
                    number += loop;
                }
            }

            return number;
        }
    }
}
