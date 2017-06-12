using System;

namespace Refit.Insane.PowerPack.Caching
{
    public class RefitCachePrimaryKeyAttribute : Attribute
    {
        public RefitCachePrimaryKeyAttribute()
        {

        }

        public string PropertyName { get; }

        /// <summary>
        /// If you use non primitive type (like your ModelClass object) as Cache Primary key you should provide 
        /// property name of primitive primary Id, otherwise ToString() method will be used.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        public RefitCachePrimaryKeyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
