using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    [Serializable]
    public partial struct RowVersion : IEquatable<RowVersion>, IComparable<RowVersion>, IFormattable, IConvertible
    {
        #region Static Section

        private const uint ValueSize = 8;

        #region Properties

        public static RowVersion Zero { get; } = new RowVersion();
        #endregion

        #region Operators

        public static bool operator ==(RowVersion op1 , RowVersion op2)
        {
            return op1.Equals(op2);
        }

        public static bool operator !=(RowVersion op1, RowVersion op2)
        {
            return !op1.Equals(op2);
        }

        public static explicit operator ulong(RowVersion rv)
        {
            return rv.ValueAsULong();
        }

        public static explicit operator DateTime(RowVersion rv)
        {
            return rv.ValueAsDateTime();
        }
        #endregion
        #endregion

        #region Properties

        private byte[] m_value;
        public byte[] Value
        {
            get
            {
                if (m_value == null)
                {
                    m_value = new byte[8];
                }
                return m_value;
            }
            private set
            {
                m_value = value;
            }
        }
        #endregion

        #region Constructors
        public RowVersion(byte[] value)
            : this()
        {
            value = value ?? new byte[ValueSize];
            if (value.Length != ValueSize)
            {
                throw new ArgumentException($"Timestamp values must be {ValueSize} bytes long.", nameof(value));
            }
            Value = value;
        }

        public RowVersion(ulong value)
            : this()
        {
            //Oracle ORA_ROWSCN value.
            Value = BitConverter.GetBytes(value);
        }

        public RowVersion(DateTime value)
            : this()
        {
            //MySQL.
            Value = BitConverter.GetBytes(value.Ticks);
        }
        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((RowVersion)obj);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int hash = 0;
            unchecked
            {
                foreach(byte b in Value)
                {
                    hash += b;
                }
            }
            return hash;
        }

        public override string ToString()
        {
            return ToString(FormatStrings.Default, null);
        }

        private string GetByteFormat(bool upperCase)
        {
            return (upperCase ? "X" : "x") + "2";
        }

        private string FormatAsMsSql(bool upperCase)
        {
            StringBuilder sb = new StringBuilder(((int)ValueSize + 1) * 2);
            string byteFormat = GetByteFormat(upperCase);
            sb.Append("0x");
            for (int i = Value.Length - 1; i >= 0; --i)
            {
                sb.Append(Value[i].ToString(byteFormat));
            }
            return sb.ToString();
        }

        private string FormatAsHexViewer(bool upperCase)
        {
            StringBuilder sb = new StringBuilder((int)ValueSize * 2 + Value.Length);
            string byteFormat = GetByteFormat(upperCase);
            for (int i = 0; i < Value.Length; ++i)
            {
                sb.Append(' ').Append(Value[i].ToString(byteFormat));
            }
            return sb.ToString(1, sb.Length - 1);
        }

        private ulong ValueAsULong()
        {
            return BitConverter.ToUInt64(Value, 0);
        }

        private DateTime ValueAsDateTime()
        {
            return new DateTime((long)ValueAsULong());
        }

        public string ToString(string formatString)
        {
            return ToString(formatString, System.Threading.Thread.CurrentThread.CurrentCulture);
        }
        #endregion

        #region IEquatable<TimeStamp>

        public bool Equals(RowVersion other)
        {
            return Value.SequenceEqual(other.Value);
        }
        #endregion

        #region IComparable<TimeStamp>

        public int CompareTo(RowVersion other)
        {
            return ValueAsULong().CompareTo(other.ValueAsULong());
        }
        #endregion

        #region IFormattable
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (String.IsNullOrWhiteSpace(format))
            {
                format = FormatStrings.GeneralUppercase;
            }
            string result = null;
            switch(format)
            {
                case FormatStrings.GeneralLowercase:
                case FormatStrings.GeneralUppercase:
                case FormatStrings.MsSqlLowercase:
                case FormatStrings.MsSqlUppercase:
                    result = FormatAsMsSql(Char.IsUpper(format, 0));
                    break;
                case FormatStrings.HexViewerLowercase:
                case FormatStrings.HexViewerUppercase:
                    result = FormatAsHexViewer(char.IsUpper(format, 0));
                    break;
                case FormatStrings.OracleLowercase:
                case FormatStrings.OracleUppercase:
                    result = ValueAsULong().ToString(formatProvider);
                    break;
                case FormatStrings.MySqlLowercase:
                case FormatStrings.MySqlUppercase:
                    result = ValueAsDateTime().ToString("yyyy-MM-dd HH:mm:ss", formatProvider);
                    break;
                default:
                    throw new FormatException($"Invalid format string '{format}'.");
            }
            return result;
        }
        #endregion

        #region IConvertible

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ValueAsULong() != 0;
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(ValueAsULong());
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(ValueAsULong(), provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(ValueAsULong(), provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(ValueAsULong(), provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(ValueAsULong(), provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(ValueAsULong(), provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(ValueAsULong(), provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(ValueAsULong(), provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return ValueAsULong();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(ValueAsULong(), provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(ValueAsULong(), provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(ValueAsULong(), provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ValueAsDateTime();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString(FormatStrings.Default, provider);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(byte[]))
            {
                return Value;
            }
            return Convert.ChangeType(ValueAsULong(), conversionType);
        }
        #endregion
    }
}
