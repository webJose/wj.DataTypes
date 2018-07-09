using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    /// <summary>
    /// Defines options and events related to the <see cref="Money"/> data type.  It is currently 
    /// used only for the Region-Culture selection for construction of values, and will be 
    /// expanded in a future version to include other options, especially for dealing with 
    /// the behavior of operators.
    /// </summary>
    public class MoneyOptions
    {
        #region Events

        /// <summary>
        /// Fires whenever a <see cref="Money"/> constructor executes with insufficient 
        /// information to uniquely select a Region-Culture (<see cref="RegionCulture"/>) object.
        /// </summary>
        public event MoneyRegionSelectionEventHandler MoneyRegionSelection;

        /// <summary>
        /// Fires the <see cref="MoneyRegionSelection"/> event.
        /// </summary>
        /// <param name="e">Event data object that listeners can use to review and even change 
        /// the <see cref="RegionCulture"/> selection.</param>
        internal void RaiseMoneyRegionSelection(MoneyRegionSelectionEventArgs e)
        {
            MoneyRegionSelection?.Invoke(e);
        }
        #endregion
    }
}
