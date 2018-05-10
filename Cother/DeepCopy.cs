using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
/*
 * Code by Kevin, http://stackoverflow.com/questions/13198658/deep-copy-using-reflection-in-an-extension-method-for-silverlight
 * 
 */

namespace Cother
{
    /// <summary>
    /// Contains static function that perform copying.
    /// </summary>
    public static class DeepCopy
    {  
        private readonly static object _lock = new object();

        /// <summary>
        /// Creates a semi-deep of an object. Its type must have a parameterless constructor. All fields and properties are copied. Even Lists and arrays are only copied by reference.
        /// </summary>
        /// <typeparam name="T">Type of the object to copy.</typeparam>
        /// <param name="original">The object to copy.</param>
        /// <returns>The deep copy.</returns>
        public static T MemberwiseCopy<T>(T original)
        {
            try
            {
                Monitor.Enter(_lock);
                T copy = Activator.CreateInstance<T>();
                PropertyInfo[] piList = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (PropertyInfo pi in piList)
                {
                    if (pi.CanWrite && pi.GetValue(copy, null) != pi.GetValue(original, null))
                    {
                        pi.SetValue(copy, pi.GetValue(original, null), null);
                    }
                }
                FieldInfo[] fiList = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo fi in fiList)
                {
                    object oldValue = fi.GetValue(original);
                    if (oldValue == null)
                    {
                        fi.SetValue(copy, null);
                        continue;
                    }
                    Type oldType = oldValue.GetType();
                    if (oldType.Name == "List`1" && oldType.GetGenericArguments().Length == 1)
                    {
                        fi.SetValue(copy, oldValue);
                        
                    }
                    else
                    {
                        fi.SetValue(copy, oldValue);
                    }
                }
                return copy;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }
}
