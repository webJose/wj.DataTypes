using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    /// <summary>
    /// Represents an association between a culture and a region as defined by the operating 
    /// system.
    /// </summary>
    public class RegionCulture
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> object this association object is about.
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        /// Gets the <see cref="RegionInfo"/> object associated to the contained culture.
        /// </summary>
        public RegionInfo Region { get; }
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of this class using the specified <see cref="CultureInfo"/> 
        /// object to create the region-culture association.
        /// </summary>
        /// <param name="culture">The cuture object to associate with a region object.</param>
        /// <exception cref="ArgumentException">Thrown if the culture does not have a 
        /// corresponding <code>RegionInfo</code> object and therefore cannot be used to infer 
        /// currency.</exception>
        internal RegionCulture(CultureInfo culture)
        {
            Culture = culture;
            //Not all LCID's have a corresponding culture.  When that happens, an exception is 
            //thrown of type ArgumentException.
            Region = new RegionInfo(culture.LCID);
        }
        #endregion
    }
}
