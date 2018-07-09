using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    public partial struct Money
    {
        /// <summary>
        /// Defines the possible format strings that can be used with the <see cref="Money"/> 
        /// datatype.
        /// </summary>
        public static class FormatStrings
        {
            /// <summary>
            /// Defines the original modifier.
            /// </summary>
            private const string Original = "O";

            /// <summary>
            /// General format.  It is also equivalent to no format specification.
            /// </summary>
            public const string General = "G";

            /// <summary>
            /// Currency format.  It is the same as the <see cref="General"/> specifier.
            /// </summary>
            public const string Currency = "C";

            /// <summary>
            /// ISO formatting.  Displays the amount followed by the 3-letter ISO currency code.
            /// </summary>
            public const string Iso = "I";

            /// <summary>
            ///  General format using the currency's original currency formatting options. 
            /// </summary>
            public const string GeneralOriginal = General + Original;

            /// <summary>
            /// Currency format using the currency's original currency formatting options.
            /// </summary>
            public const string CurrencyOriginal = Currency + Original;

            /// <summary>
            /// ISO formatting using the currency's original currency formatting options.
            /// </summary>
            public const string IsoOriginal = Iso + Original;

            /// <summary>
            /// Default formatting.  It is the same as the <see cref="General"/> specifier.
            /// </summary>
            public const string Default = General;

            /// <summary>
            /// Regular expression that validates the format string for the <see cref="Money"/> 
            /// data type.
            /// </summary>
            internal const string RegExString = "^([" + General + Currency + Iso + "])(" + Original + @"?)(\d*)$";

            /// <summary>
            /// Regular expression singleton object used to validate <see cref="Money"/> format 
            /// strings.
            /// </summary>
            internal static readonly Regex FormatStringRegEx = new Regex(RegExString, RegexOptions.Compiled);
        }
    }
}
