using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS3QueryLib.Core.Common;

namespace GW2PAO.TS3.Util
{
    public static class DecodeUtility
    {
        /// <summary>
        /// Decodes a uint property out of the given input string
        /// </summary>
        /// <param name="input">the input string to parse</param>
        /// <param name="propertyName">the full list of possible property names to parse the value of. the first one that works is used.</param>
        /// <returns>The parsed value of the property.</returns>
        /// <exception cref="InvalidOperationException">Thrown when none of the given property names results in a valid uint value</exception>
        public static uint DecodeUIntProperty(string input, params string[] propertyNames)
        {
            var properties = input.Split(' ', '\n', '\r', '|');

            foreach (var propertyName in propertyNames)
            {
                string value = properties.FirstOrDefault(id => id.StartsWith(propertyName));
                if (value != null)
                {
                    value = value.Substring(propertyName.Length + 1);
                    return uint.Parse(value);
                }
            }

#if DEBUG
            throw new InvalidOperationException("Invalid propertyNames for given input string");
#else
            return 0;
#endif
        }

        /// <summary>
        /// Decodes a uint property out of the given input string
        /// </summary>
        /// <param name="input">the input string to parse</param>
        /// <param name="decodeValue">True to decode the string, else false to return it as-is</param>
        /// <param name="propertyName">the full list of possible property names to parse the value of. the first one that works is used.</param>
        /// <returns>The parsed value of the property.</returns>
        /// <exception cref="InvalidOperationException">Thrown when none of the given property names results in a valid uint value</exception>
        public static string DecodeStringProperty(string input, bool decodeValue, params string[] propertyNames)
        {
            var properties = input.Split(' ', '\n', '\r', '|');

            foreach (var propertyName in propertyNames)
            {
                string value = properties.FirstOrDefault(id => id.StartsWith(propertyName));
                if (value != null)
                {
                    if (value.Length > propertyName.Length)
                        value = value.Substring(propertyName.Length + 1);
                    else
                        value = string.Empty;

                    if (decodeValue)
                        return DecodeString(value);
                    else
                        return value;
                }
            }

#if DEBUG
            throw new InvalidOperationException("Invalid propertyNames for given input string");
#else
            return string.Empty;
#endif
        }

        /// <summary>
        /// Parses out all special characters (such as /s) from the input string
        /// </summary>
        /// <param name="input">String to clean up</param>
        /// <returns>The normal string representation of the input</returns>
        public static string DecodeString(string input)
        {
            return Ts3Util.DecodeString(input);
        }

        /// <summary>
        /// Encodes special characters (such as ' ') using the input string
        /// </summary>
        /// <param name="input">String to encode</param>
        /// <returns>The encoded string representation of the input</returns>
        public static string EncodeString(string input)
        {
            return Ts3Util.EncodeString(input);
        }
    }
}
