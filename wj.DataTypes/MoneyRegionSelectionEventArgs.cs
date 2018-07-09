using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    public class MoneyRegionSelectionEventArgs : EventArgs
    {
        #region Properties

        public MoneyInfo MoneyInfo { get; }

        private RegionCulture m_selectedValue;
        public RegionCulture SelectedValue
        {
            get => m_selectedValue;
            set => m_selectedValue = value == null || MoneyInfo.Contains(value) ? value : throw new ArgumentException($"The given {nameof(MoneyInfo)} value is not part of the possible choices.", nameof(value));
        }
        #endregion

        #region Constructors
        public MoneyRegionSelectionEventArgs(MoneyInfo moneyInfo, RegionCulture selectedValue = null)
        {
            MoneyInfo = moneyInfo;
            SelectedValue = selectedValue;
        }
        #endregion
    }
}
