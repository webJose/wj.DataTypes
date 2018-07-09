using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    public class CurrencyInfo
    {
        #region Properties
        public string IsoCurrencyCode { get; private set; }

        public MoneyInfo MoneyInfos { get; private set; }
        #endregion

        #region Constructors
        internal CurrencyInfo(string isoCurrencyCode)
        {
            IsoCurrencyCode = isoCurrencyCode;
            MoneyInfos = new MoneyInfo(isoCurrencyCode);
        }
        #endregion
    }
}
