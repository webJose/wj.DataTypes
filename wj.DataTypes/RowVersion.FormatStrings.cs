using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    public partial struct RowVersion
    {
        public static class FormatStrings
        {
            public const string GeneralUppercase = "G";
            public const string GeneralLowercase = "g";
            public const string MsSqlUppercase = "S";
            public const string MsSqlLowercase = "s";
            public const string OracleUppercase = "O";
            public const string OracleLowercase = "o";
            public const string MySqlUppercase = "M";
            public const string MySqlLowercase = "m";
            public const string HexViewerUppercase = "H";
            public const string HexViewerLowercase = "h";
            public const string Default = GeneralLowercase;
        }
    }
}
