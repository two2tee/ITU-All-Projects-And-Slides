// SexConverter.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using LiftLog.Core.Enums;

namespace LiftLog.Service.Utilities
{
    public class SexConverter
    {
        /// <summary>
        ///     Returns a sex string in english
        /// </summary>
        /// <param name="sex">sex enum</param>
        /// <returns>string of sex name</returns>
        public static string GetSexEnglish(Sex sex)
        {
            switch (sex)
            {
                case Sex.Male:
                    return "Male";
                case Sex.Female:
                    return "Female";
                default:
                    return "Unknown";
            }
        }
    }
}