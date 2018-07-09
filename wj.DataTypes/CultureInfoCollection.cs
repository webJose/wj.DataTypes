using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    internal class CultureInfoCollection : KeyedCollection<string, CultureInfo>
    {
        #region Methods
        protected override string GetKeyForItem(CultureInfo item)
        {
            return item.Name;
        }
        #endregion
    }
}
