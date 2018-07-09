using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    public class MoneyInfoCollection : KeyedCollection<string, MoneyInfo>
    {
        #region Methods
        protected override string GetKeyForItem(MoneyInfo item)
        {
            return item.IsoCurrencyCode;
        }
        #endregion
    }
}
