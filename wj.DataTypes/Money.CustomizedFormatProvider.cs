using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    public partial struct Money
    {
        /// <summary>
        /// Constructs a custom <see cref="NumberFormatInfo"/> object from the one for the current 
        /// culture with the currency symbol replaced with the one in the 
        /// <see cref="RegionCulture.Culture"/> object that was given to the object during 
        /// construction.
        /// </summary>
        private class CustomizedFormatProvider : IFormatProvider
        {
            #region Properties

            /// <summary>
            /// Gets the <see cref="RegionCulture"/> object used to construct this instance.
            /// </summary>
            private RegionCulture RegionCulture { get; }

            /// <summary>
            /// Gets the overriding format provider object, if any is given when the object is 
            /// constructed.
            /// </summary>
            private IFormatProvider FormatProvider { get; }
            #endregion

            #region Constructors

            /// <summary>
            /// Creates a new instance of this class with the specified <see cref="RegionCulture"/> 
            /// object.
            /// </summary>
            /// <param name="regionCulture">The <see cref="RegionCulture"/> object used to 
            /// construct the custom <see cref="NumberFormatInfo"/> object on demand.</param>
            /// <param name="formatProvider">The overriding format provider object to be 
            /// preferred over the current culture, if any.</param>
            public CustomizedFormatProvider(RegionCulture regionCulture, IFormatProvider formatProvider)
            {
                RegionCulture = regionCulture;
                FormatProvider = formatProvider;
            }
            #endregion

            #region Methods
            private IFormatProvider GetBaseFormatProvider()
            {
                return FormatProvider ?? CultureInfo.CurrentCulture;
            }
            #endregion

            #region IFormatProvider

            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(NumberFormatInfo))
                {
                    IFormatProvider formatProvider = GetBaseFormatProvider();
                    NumberFormatInfo nfi = formatProvider.GetFormat(formatType) as NumberFormatInfo;
                    nfi = nfi.Clone() as NumberFormatInfo;
                    nfi.CurrencySymbol = RegionCulture.Culture.NumberFormat.CurrencySymbol;
                    //This one is used for ISO strings.
                    nfi.NumberDecimalDigits = RegionCulture.Culture.NumberFormat.NumberDecimalDigits;
                    return nfi;
                }
                return null;
            }
            #endregion
        }
    }
}
