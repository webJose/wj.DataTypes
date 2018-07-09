using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    public class MoneyRegionCollection : KeyedCollection<string, RegionCulture>
    {
        #region Methods
        protected override string GetKeyForItem(RegionCulture item)
        {
            return item.Culture.Name;
        }
        #endregion
    }
}
