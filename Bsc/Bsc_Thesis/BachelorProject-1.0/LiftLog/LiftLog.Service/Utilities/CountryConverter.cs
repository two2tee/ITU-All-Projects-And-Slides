// CountryConverter.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using LiftLog.Core.Enums;

namespace LiftLog.Service.Utilities
{
    public class CountryConverter
    {
        /// <summary>
        ///     Returns a country string in english
        /// </summary>
        /// <param name="country">country enum</param>
        /// <returns>string of country name</returns>
        public static string GetCountryEnglish(Country country)
        {
            switch (country)
            {
                case Country.Denmark:
                    return "Denmark";
                case Country.Usa:
                    return "USA";
                case Country.Japan:
                    return "Japan";
                default:
                    return "Unknown";
            }
        }
    }
}