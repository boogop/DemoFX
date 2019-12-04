using System;
using System.Collections.Generic;
using System.Text;

namespace DemoFX
{
    /// <summary>
    /// Generic class to check for null or blank values in an object T.
    /// Should work for anything that inherits from System.Nullable.     
    /// 
    /// This also gives us a little better standardization on how we check
    /// string types for null or empty given that .NET gives us the following
    /// ways to do it:
    /// 
    ///  if (str == "")                                     : common  
    ///  if (string.Equals(str, ""))        
    ///  if (string.IsNullOrEmpty(str))        
    ///  if (str.Length == 0)                               : best     
    ///  if (str.Equals(""))
    ///  if (Convert.ToString((object)stringVar) == "")     : most stupid
    ///  if ((String.Compare (a, "") > 0)
    /// 
    /// How'd you like to see those peppered throughout your code?
    /// </summary>
    
    
    static class chkNull
    {
        /// <summary>
        /// Used in cases where you need a boolean response on a nullable object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool isNull<T>(T obj)
        {   
            // entire object is null
            if (obj == null)
            {
                return true;
            }

            // object has properties
            if (obj.Equals(System.DBNull.Value))
            {
                return true;
            }           

            // empty strings may pass the first test
            if (obj is string)
            {
                if (obj.ToString().Length == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Used in cases where you need a string back from a nullable object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns> 
        public static string whenNull<T>(T obj)
        {            
            if (obj == null)
            {
                return "";
            }

            if (obj is string)
            {
                if (obj.ToString().Length == 0)
                {
                    return "";
                }
            }

            return obj.ToString();
        }

        /// <summary>
        /// Used in cases where you need a number back from a nullable object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double numNull<T>(T obj)
        {
            try
            {
                if (obj == null)
                {
                    return 0;
                }

                if (obj is string)
                {
                    if (obj.ToString().Length == 0)
                    {
                        return 0;
                    }
                }

                return Convert.ToDouble(obj);
            }
            catch 
            {
                // not a proper use of the catch block since
                // I'm using it to verify we passed in a number
                return 0;
            }
        }


    }
}
