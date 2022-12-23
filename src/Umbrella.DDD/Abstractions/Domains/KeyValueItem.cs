using System;

namespace Umbrella.DDD.Abstractions.Domains
{
    /// <summary>
    /// Base class for immutable value-object
    /// </summary>
    public abstract class KeyValueItem
    {
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string DisplayValue { get; private set; }


        /// <summary>
        /// Empty constructor for serialization
        /// </summary>
        public KeyValueItem()
        {
            Code = "TBD";
            DisplayValue = "Undefined";
        }

        /// <summary>
        /// Default Cosntructor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected KeyValueItem(string code, string value) : this()
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            Code = code;
            DisplayValue = value;
        }
    }
}