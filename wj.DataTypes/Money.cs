using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    /// <summary>
    /// Delegate definition for the <see cref="Money.MoneyRegionSelection"/> event listeners.
    /// </summary>
    /// <param name="e">Event data used to transmit <see cref="RegionCulture"/> selection.</param>
    public delegate void MoneyRegionSelectionEventHandler(MoneyRegionSelectionEventArgs e);

    /// <summary>
    /// Represents a monetary value.
    /// </summary>
    public partial struct Money : IEquatable<Money>, IComparable<Money>, IFormattable, IConvertible
    {
        #region Static Section

        #region Fields

        /// <summary>
        /// Lazily-initialized object used to collect currency information from all system-defined 
        /// cultures so the information is indexed and readily available to construct 
        /// <see cref="Money"/> values.
        /// </summary>
        private static Lazy<(CultureInfoCollection, MoneyRegionCollection, MoneyInfoCollection)> _allMoneyData =
            new Lazy<(CultureInfoCollection, MoneyRegionCollection, MoneyInfoCollection)>(() =>
        {
            //There are 3 indexes:  One for cultures, one for MoneyRegions based on culture name,
            //and one for MoneyInfos based on currency code.  They support the Money constructors.
            CultureInfoCollection allCultures = new CultureInfoCollection();
            MoneyRegionCollection allMoneyRegions = new MoneyRegionCollection();
            MoneyInfoCollection allMoneyInfo = new MoneyInfoCollection();
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            for (int i = 0; i < cultures.Length; ++i)
            {
                CultureInfo ci = cultures[i];
                allCultures.Add(ci);
                try
                {
                    RegionCulture mr = new RegionCulture(ci);
                    allMoneyRegions.Add(mr);
                    MoneyInfo mi = allMoneyInfo.Contains(mr.Region.ISOCurrencySymbol) ?
                        allMoneyInfo[mr.Region.ISOCurrencySymbol] : new MoneyInfo(mr.Region.ISOCurrencySymbol);
                    mi.Add(mr);
                }
                catch (ArgumentException) { }
            }
            return (allCultures, allMoneyRegions, allMoneyInfo);
        });
        #endregion

        #region Properties

        /// <summary>
        /// Gets all currency-related data fetched from the operating system.  This data is used 
        /// to support the <see cref="Money"/> constructors.
        /// </summary>
        private static (CultureInfoCollection AllCultures, MoneyRegionCollection AllMoneyRegions, MoneyInfoCollection AllMoneyInfo) AllMoneyData
        {
            get { return _allMoneyData.Value; }
        }

        /// <summary>
        /// Gets all <see cref="RegionCulture"/> objects defined by the operating system.
        /// </summary>
        public static MoneyRegionCollection AllMoneyRegions => AllMoneyData.AllMoneyRegions;

        /// <summary>
        /// Gets all <see cref="MoneyInfo"/> objects derived from information in the operating 
        /// system.
        /// </summary>
        public static MoneyInfoCollection AllMoneyInfo => AllMoneyData.AllMoneyInfo;

        /// <summary>
        /// Gets the current options used for construction and operation of all <see cref="Money"/> 
        /// related operations.
        /// </summary>
        public static MoneyOptions MoneyOptions { get; } = new MoneyOptions();
        #endregion

        #region Methods

        /// <summary>
        /// Fires whenever a <see cref="Money"/> constructor executes with insufficient 
        /// information to uniquely select a <see cref="RegionCulture"/> object.
        /// </summary>
        private static void RaiseMoneyRegionSelection(MoneyRegionSelectionEventArgs e)
        {
            MoneyOptions.RaiseMoneyRegionSelection(e);
        }

        /// <summary>
        /// Obtains the <see cref="RegionCulture"/> object corresponding to the current culture for 
        /// the benefit of constructors of the <see cref="Money"/> data type that do not accept 
        /// currency code or culture data.
        /// </summary>
        /// <returns>The <see cref="RegionCulture"/> object for the current culture or an 
        /// <see cref="InvalidOperationException"/> exception is thrown.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the current culture does not 
        /// have a corresponding <see cref="RegionInfo"/> object.</exception>
        private static RegionCulture GetCurrentCultureRegionCulture()
            => AllMoneyData.AllMoneyRegions.Contains(CultureInfo.CurrentCulture.Name) ?
                AllMoneyData.AllMoneyRegions[CultureInfo.CurrentCulture.Name] :
                throw new InvalidOperationException($"Cannot construct {nameof(Money)} values using the current culture because there is insufficient data to produce it.  Explicitly specify another culture or an ISO currency code.");

        /// <summary>
        /// Selects the currency information (<see cref="RegionCulture"/>) object to use for 
        /// construction of a <see cref="Money"/> value using the specified ISO currency code.
        /// </summary>
        /// <param name="isoCurrencyCode">The ISO currency code of the desired currency.</param>
        /// <returns>The <see cref="RegionCulture"/> object belonging to the specified currency 
        /// code according to the automatic selection algorithm or a consumer override.</returns>
        private static RegionCulture SelectRegionCulture(string isoCurrencyCode)
        {
            if (!AllMoneyData.AllMoneyInfo.Contains(isoCurrencyCode))
            {
                throw new ArgumentException($"The specified currency code ({isoCurrencyCode}) is not a recognized ISO currency code.", nameof(isoCurrencyCode));
            }
            MoneyRegionSelectionEventArgs e = new MoneyRegionSelectionEventArgs(AllMoneyData.AllMoneyInfo[isoCurrencyCode]);
            //Select the MoneyRegion object for the current culture, if possible.
            if (e.MoneyInfo.Contains(CultureInfo.CurrentCulture.Name))
            {
                e.SelectedValue = e.MoneyInfo[CultureInfo.CurrentCulture.Name];
            }
            else
            {
                //Select the first MoneyRegion object with a region-specific culture.
                e.SelectedValue = e.MoneyInfo.Where(mr => mr.Culture.Name.IndexOf('-') > 0).FirstOrDefault();
                //If still no selection, select the first item in the list.
                if (e.SelectedValue == null)
                {
                    e.SelectedValue = e.MoneyInfo[0];
                }
            }
            RaiseMoneyRegionSelection(e);
            return e.SelectedValue ??
                throw new InvalidOperationException($"Cannot construct {nameof(Money)} values because there is insufficient data to produce it.  Handle the {nameof(MoneyOptions.MoneyRegionSelection)} event to provide a suitable value.");
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implicitly casts a <see cref="Money"/> value to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="op"><see cref="Money"/> value to cast.</param>
        public static implicit operator decimal(Money op)
        {
            return op.Amount;
        }

        /// <summary>
        /// Implicitly casts a <see cref="decimal"/> value to a <see cref="Money"/> value using 
        /// the current culture's currency.
        /// </summary>
        /// <param name="op"><see cref="decimal"/> value to cast.</param>
        public static implicit operator Money(decimal op)
        {
            return new Money(op);
        }

        /// <summary>
        /// Compares two <see cref="Money"/> values for equality.
        /// </summary>
        /// <param name="op1">Left hand side <see cref="Money"/> value.</param>
        /// <param name="op2">Right hand side <see cref="Money"/> value.</param>
        /// <returns>True if the two <see cref="Money"/> values are deemed equal; false 
        /// otherwise.</returns>
        public static bool operator ==(Money op1, Money op2) => op1.Equals(op2);

        /// <summary>
        /// Compares two <see cref="Money"/> values for inequality.
        /// </summary>
        /// <param name="op1">Left hand side <see cref="Money"/> value.</param>
        /// <param name="op2">Right hand side <see cref="Money"/> value.</param>
        /// <returns>True if the two <see cref="Money"/> values differ; false otherwise.</returns>
        public static bool operator !=(Money op1, Money op2) => !op1.Equals(op2);

        /// <summary>
        /// Compares a <see cref="Money"/> value against another to determine if it is smaller.
        /// </summary>
        /// <param name="op1">Potentially smaller, left hand <see cref="Money"/> value.</param>
        /// <param name="op2">Potentially lager, right hand <see cref="Money"/> value.</param>
        /// <returns>True if the left hand value is smaller than the right hand value; false 
        /// otherwise.</returns>
        public static bool operator <(Money op1, Money op2) => op1.CompareTo(op2) < 0;

        /// <summary>
        /// Compares a <see cref="Money"/> value against another to determine if it is larger.
        /// </summary>
        /// <param name="op1">Potentially larger, left hand <see cref="Money"/> value.</param>
        /// <param name="op2">Potentially smaller, right hand <see cref="Money"/> value.</param>
        /// <returns>True if the left hand value is larger than the right hand value; false 
        /// otherwise.</returns>
        public static bool operator >(Money op1, Money op2) => op1.CompareTo(op2) > 0;

        /// <summary>
        /// Compares a <see cref="Money"/> value against another to determine if it is larger or 
        /// equal.
        /// </summary>
        /// <param name="op1">Potentially larger (or equal), left hand <see cref="Money"/> value.</param>
        /// <param name="op2">Potentially smaller (or equal), right hand <see cref="Money"/> value.</param>
        /// <returns>True if the left hand value is larger than or equal to the right hand value; 
        /// false otherwise.</returns>
        public static bool operator >=(Money op1, Money op2) => op1 == op2 || op1 > op2;

        /// <summary>
        /// Compares a <see cref="Money"/> value against another to determine if it is smaller or 
        /// equal.
        /// </summary>
        /// <param name="op1">Potentially smaller (or equal), left hand <see cref="Money"/> value.</param>
        /// <param name="op2">Potentially larger (or equal), right hand <see cref="Money"/> value.</param>
        /// <returns>True if the left hand value is smaller than or equal to the right hand value; 
        /// false otherwise.</returns>
        public static bool operator <=(Money op1, Money op2) => op1 == op2 || op1 < op2;

        /// <summary>
        /// Sums the provided <see cref="Money"/> values and returns a <see cref="Money"/> value that 
        /// represents the sum of both.
        /// </summary>
        /// <param name="op1">First value to sum.</param>
        /// <param name="op2">Second value to sum.</param>
        /// <returns>A <see cref="Money"/> value representing the aggregate monetary value of the 
        /// provided 2 values.</returns>
        public static Money operator +(Money op1, Money op2) =>
            op1.IsoCurrencyCode == op1.IsoCurrencyCode ?
            new Money(op1.Amount + op2.Amount, op1.RegionCulture) :
            throw new ArgumentException($"The provided {nameof(Money)} values have different currency codes and cannot be summed.");

        /// <summary>
        /// Subtracts the second <see cref="Money"/> value from the first and returns a new 
        /// <see cref="Money"/> value representing the result.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to subtract from.</param>
        /// <param name="op2">The <see cref="Money"/> value to be subtracted.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the subtraction.</returns>
        public static Money operator -(Money op1, Money op2) =>
            op1.IsoCurrencyCode == op1.IsoCurrencyCode ?
            new Money(op1.Amount - op2.Amount, op1.RegionCulture) :
            throw new ArgumentException($"The provided {nameof(Money)} values have different currency codes and cannot be subtracted.");

        /// <summary>
        /// Multiplies the specified <see cref="Money"/> value by the given multiplication factor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be multiplied.</param>
        /// <param name="op2">The multiplication factor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the multiplication.</returns>
        public static Money operator *(Money op1, double op2) => new Money((decimal)((double)op1.Amount * op2), op1.RegionCulture);

        /// <summary>
        /// Multiplies the specified <see cref="Money"/> value by the given multiplication factor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be multiplied.</param>
        /// <param name="op2">The multiplication factor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the multiplication.</returns>
        public static Money operator *(Money op1, decimal op2) => new Money(op1.Amount * op2, op1.RegionCulture);

        /// <summary>
        /// Multiplies the specified <see cref="Money"/> value by the given multiplication factor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be multiplied.</param>
        /// <param name="op2">The multiplication factor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the multiplication.</returns>
        public static Money operator *(Money op1, int op2) => new Money(op1.Amount * op2, op1.RegionCulture);

        /// <summary>
        /// Multiplies the specified <see cref="Money"/> value by the given multiplication factor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be multiplied.</param>
        /// <param name="op2">The multiplication factor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the multiplication.</returns>
        public static Money operator *(Money op1, uint op2) => new Money(op1.Amount * op2, op1.RegionCulture);

        /// <summary>
        /// Multiplies the specified <see cref="Money"/> value by the given multiplication factor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be multiplied.</param>
        /// <param name="op2">The multiplication factor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the multiplication.</returns>
        public static Money operator *(Money op1, float op2) => new Money((decimal)((float)op1.Amount * op2), op1.RegionCulture);

        /// <summary>
        /// Multiplies the specified <see cref="Money"/> value by the given multiplication factor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be multiplied.</param>
        /// <param name="op2">The multiplication factor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the multiplication.</returns>
        public static Money operator *(Money op1, long op2) => new Money(op1.Amount * op2, op1.RegionCulture);

        /// <summary>
        /// Calculates the ratio between two <see cref="Money"/> values.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value used as dividend.</param>
        /// <param name="op2">The <see cref="Money"/> value used as divisor.</param>
        /// <returns>A double value representing the ratio between the two <see cref="Money"/> 
        /// values.</returns>
        public static double operator /(Money op1, Money op2) =>
            op1.IsoCurrencyCode == op2.IsoCurrencyCode ?
            (double)op1.Amount / (double)op2.Amount :
            throw new ArgumentException($"The provided {nameof(Money)} values have different currency codes and cannot be divided.");

        /// <summary>
        /// Divides the specified <see cref="Money"/> value by the given divisor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be divided.</param>
        /// <param name="op2">The divisor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the division.</returns>
        public static Money operator /(Money op1, double op2) => new Money((decimal)((double)op1.Amount / op2), op1.RegionCulture);

        /// <summary>
        /// Divides the specified <see cref="Money"/> value by the given divisor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be divided.</param>
        /// <param name="op2">The divisor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the division.</returns>
        public static Money operator /(Money op1, decimal op2) => new Money(op1.Amount / op2, op1.RegionCulture);

        /// <summary>
        /// Divides the specified <see cref="Money"/> value by the given divisor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be divided.</param>
        /// <param name="op2">The divisor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the division.</returns>
        public static Money operator /(Money op1, int op2) => new Money(op1.Amount / op2, op1.RegionCulture);

        /// <summary>
        /// Divides the specified <see cref="Money"/> value by the given divisor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be divided.</param>
        /// <param name="op2">The divisor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the division.</returns>
        public static Money operator /(Money op1, uint op2) => new Money(op1.Amount / op2, op1.RegionCulture);

        /// <summary>
        /// Divides the specified <see cref="Money"/> value by the given divisor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be divided.</param>
        /// <param name="op2">The divisor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the division.</returns>
        public static Money operator /(Money op1, float op2) => new Money((decimal)((float)op1.Amount / op2), op1.RegionCulture);

        /// <summary>
        /// Divides the specified <see cref="Money"/> value by the given divisor.
        /// </summary>
        /// <param name="op1">The <see cref="Money"/> value to be divided.</param>
        /// <param name="op2">The divisor to use.</param>
        /// <returns>A <see cref="Money"/> value representing the result of the division.</returns>
        public static Money operator /(Money op1, long op2) => new Money(op1.Amount / op2, op1.RegionCulture);
        #endregion
        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="RegionCulture"/> object that was used to create this 
        /// <see cref="Money"/> value.
        /// </summary>
        public RegionCulture RegionCulture { get; }

        /// <summary>
        /// Gets the amount of this <see cref="Money"/> value.
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Gets the ISO currency code for this <see cref="Money"/> value.
        /// </summary>
        public string IsoCurrencyCode => RegionCulture == null ? "USD" : RegionCulture.Region.ISOCurrencySymbol;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="Money"/> value of the specified amount using the specified 
        /// <see cref="RegionCulture"/> object to define the currency.
        /// </summary>
        /// <param name="amount">The amount to be represented.</param>
        /// <param name="regionCulture">The <see cref="RegionCulture"/> object that defines the 
        /// currency for this value.</param>
        public Money(decimal amount, RegionCulture regionCulture)
            : this()
        {
            Amount = amount;
            RegionCulture = regionCulture;
        }

        /// <summary>
        /// Creats a new <see cref="Money"/> value of the specified amount and the specified ISO 
        /// currency code.  This constructor triggers the <see cref="MoneyOptions.MoneyRegionSelection"/> 
        /// event to give consumers of <see cref="Money"/> data type a chance to override the 
        /// automatically selected <see cref="RegionCulture"/> object for currency codes used in 
        /// more than one region of the world.
        /// </summary>
        /// <param name="amount">The amount to be represented.</param>
        /// <param name="isoCurrencyCode">The desired ISO currency code for this value.</param>
        public Money(decimal amount, string isoCurrencyCode)
            : this(amount, SelectRegionCulture(isoCurrencyCode))
        { }

        /// <summary>
        /// Creates a new <see cref="Money"/> value of the specified amount and the currency for 
        /// the current culture.  It is important to note that this constructor may fail if the 
        /// current culture does not have a region associated to it.
        /// </summary>
        /// <param name="amount">The amount to be represented.</param>
        /// <exception cref="InvalidOperationException">Thrown if the current culture does not 
        /// have a corresponding <see cref="RegionInfo"/> object.</exception>
        public Money(decimal amount)
            : this(amount, GetCurrentCultureRegionCulture())
        { }
        #endregion

        #region Methods

        public override bool Equals(object obj) =>
            obj == null || GetType() != obj.GetType() ?
            false :
            Equals((Money)obj);

        public override int GetHashCode() => Amount.GetHashCode();

        private IFormatProvider CreateFormatProvider(IFormatProvider baseFormatProvider) =>
            new CustomizedFormatProvider(RegionCulture, baseFormatProvider);

        public override string ToString() => ToString(FormatStrings.Default);

        public string ToString(string formatString) => ToString(formatString, null);

        public string ToString(IFormatProvider formatProvider) => 
            ToString(FormatStrings.Default, formatProvider);
        #endregion

        #region IComparable<Money>

        /// <summary>
        /// Compares this <see cref="Money"/> value with another to determine the order in which 
        /// they should be placed relative to one another.
        /// </summary>
        /// <param name="other">The other value to compare to.</param>
        /// <returns>A value less than zero if this value should be placed before the other value; 
        /// a value greater than zero if thsi value should be placed after the other value; zero 
        /// if both values can be deemed to occupy the same order position.</returns>
        public int CompareTo(Money other) =>
            IsoCurrencyCode == other.IsoCurrencyCode ?
            Amount.CompareTo(other.Amount) :
            throw new InvalidOperationException($"Cannot compare 2 {nameof(Money)} values when their currency codes are different.");
        #endregion

        #region IEquatable<Money>

        /// <summary>
        /// Determines if this <see cref="Money"/> value equals another.
        /// </summary>
        /// <param name="other">The other value to compare this value to.</param>
        /// <returns>True if both <see cref="Money"/> values are considered equal; false otherwise.</returns>
        public bool Equals(Money other) => (Amount == other.Amount && Amount == 0) ||
                (Amount == other.Amount && IsoCurrencyCode == other.IsoCurrencyCode);
        #endregion

        #region IFormattable

        /// <summary>
        /// Formats this value according to the specified format string and using the specified 
        /// format provider.
        /// </summary>
        /// <param name="format">The desired format string defining the desired format.</param>
        /// <param name="formatProvider">A format provider object that provides formatting 
        /// information, such as decimal and group separators.</param>
        /// <returns>A string that represents the this <see cref="Money"/> value in accordance to 
        /// the specified format string.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            string formatSelector = FormatStrings.Default;
            bool original = false;
            string formatModifier = null;
            if (!String.IsNullOrWhiteSpace(format))
            {
                Match m = FormatStrings.FormatStringRegEx.Match(format);
                if (!m.Success)
                {
                    throw new FormatException($"The format string '{format}' is invalid.");
                }
                formatSelector = m.Groups[1].Value;
                if (formatSelector == FormatStrings.General)
                {
                    formatSelector = FormatStrings.Currency;
                }
                original = m.Groups[2].Value.Length == 1;
                formatModifier = m.Groups[3].Value;
            }
            //If Original is requested, we override any format provider.
            if (original)
            {
                formatProvider = RegionCulture.Culture;
            }
            else
            {
                formatProvider = CreateFormatProvider(formatProvider);
            }
            string result = null;
            switch (formatSelector)
            {
                case FormatStrings.General:
                case FormatStrings.Currency:
                    result = Amount.ToString($"{FormatStrings.Currency}{formatModifier}", formatProvider);
                    break;
                case FormatStrings.Iso:
                    result = $"{Amount.ToString($"N{formatModifier}", formatProvider)} {IsoCurrencyCode}";
                    break;
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
            return ((IConvertible)Amount).ToBoolean(provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToByte(provider);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToChar(provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToDateTime(provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToDecimal(provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToDouble(provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToInt16(provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToInt32(provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToInt64(provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToSByte(provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToSingle(provider);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString(provider);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToType(conversionType, provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToUInt16(provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToUInt32(provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)Amount).ToUInt64(provider);
        }
        #endregion
    }
}
