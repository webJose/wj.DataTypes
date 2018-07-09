using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wj.DataTypes
{
    /// <summary>
    /// Provides a simple implementation of the <see cref="IFormatProvider"/> interface based on 
    /// a dictionary of either format data, or functions that return format data.
    /// </summary>
    public class GenericFormatProvider : IFormatProvider
    {
        #region Properties

        /// <summary>
        /// Gets the collection of format data contained in this instance.
        /// </summary>
        private Dictionary<Type, object> Sources { get; } = new Dictionary<Type, object>();
        #endregion

        #region Methods

        /// <summary>
        /// Adds format data to the internal dictionary.
        /// </summary>
        /// <typeparam name="TData">The type of format data to store and later provide.</typeparam>
        /// <param name="data">The actual format data to provide whenever is needed.</param>
        public void AddFormatData<TData>(TData data) => Sources[typeof(TData)] = data;

        /// <summary>
        /// Adds a function delegate that returns format data.
        /// </summary>
        /// <typeparam name="TData">The type of format data to store and later provide.</typeparam>
        /// <param name="dataFn">The function delegate that returns the actual format data to 
        /// provide whenever is needed.</param>
        public void AddFormatData<TData>(Func<TData> dataFn) => Sources[typeof(TData)] = new Func<object>(() => dataFn);

        /// <summary>
        /// Removes format data (or format data function) associated with the given type.
        /// </summary>
        /// <param name="type">The type that identifies the format data to remove from the 
        /// internal dictionary.</param>
        public void RemoveFormatData(Type type) => Sources.Remove(type);

        /// <summary>
        /// Removes format data (or format data function) associated with the given type.
        /// </summary>
        /// <typeparam name="TData">The type that identifies the format data to remove from the 
        /// internal dictionary.</typeparam>
        public void RemoveFormatData<TData>()
        {
            RemoveFormatData(typeof(TData));
        }
        #endregion

        #region IFormatProvider
        public object GetFormat(Type formatType)
        {
            if (Sources.ContainsKey(formatType))
            {
                object value = Sources[formatType];
                if (value.GetType() == formatType)
                {
                    return value;
                }
                //Not of the correct type. Assume it is a delegate.
                return ((Func<object>)value)();
            }
            //No coincidence.
            return null;
        }
        #endregion
    }
}
