using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rocks.kfs.Eventbrite.Utility.ExtensionMethods
{
    public static partial class StringExtensions
    {
        /// <summary>
        /// Attempts to convert string to long.  Returns 0 if unsuccessful.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThrough()]
        public static long AsLong( this string str )
        {
            return str.AsLongOrNull() ?? 0;
        }

        /// <summary>
        /// Attempts to convert string to an long.  Returns null if unsuccessful.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThrough()]
        public static long? AsLongOrNull( this string str )
        {
            long value;
            if ( long.TryParse( str, out value ) )
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public static long ToLong( this object input, long defaultValue )
        {
            long retVar = defaultValue;
            if ( input != null )
            {
                StringBuilder stringBuilder = new StringBuilder();
                string str = input.ToString();
                int strCounter = 0;
                while ( strCounter < str.Length )
                {
                    char chr = str[strCounter];
                    switch ( chr )
                    {
                        case '(':
                            {
                                stringBuilder.Append( '-' );
                                goto case '/';
                            }
                        case ')':
                        case '*':
                        case '+':
                        case ',':
                        case '.':
                        case '/':
                            {
                                if ( chr == '.' )
                                {
                                    goto Parse;
                                }
                                strCounter++;
                                continue;
                            }
                        case '-':
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            {
                                stringBuilder.Append( chr );
                                goto case '/';
                            }
                        default:
                            {
                                goto case '/';
                            }
                    }
                }
            Parse:
                if ( !long.TryParse( stringBuilder.ToString(), out retVar ) )
                {
                    retVar = defaultValue;
                }
            }
            return retVar;
        }

    }
}
