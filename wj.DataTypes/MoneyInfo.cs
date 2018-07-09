using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    public class MoneyInfo : MoneyRegionCollection
    {
        #region Properties
        public string IsoCurrencyCode { get; private set; }
        #endregion

        #region Constructors
        public MoneyInfo(string isoCurrencyCode)
        {
            IsoCurrencyCode = isoCurrencyCode;
        }
        #endregion

        #region Methods

        private void CheckIsoCurrencyCode(RegionCulture item)
        {
            if (item.Region == null || item.Region.ISOCurrencySymbol != IsoCurrencyCode)
            {
                throw new ArgumentException("The specified object cannot be added to this collection because the ISO currency code does not match.");
            }
        }

        protected override void InsertItem(int index, RegionCulture item)
        {
            CheckIsoCurrencyCode(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, RegionCulture item)
        {
            CheckIsoCurrencyCode(item);
            base.SetItem(index, item);
        }
        #endregion
    }
}
