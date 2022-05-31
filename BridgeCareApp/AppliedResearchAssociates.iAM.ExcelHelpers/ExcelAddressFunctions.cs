using System.Linq;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public static class ExcelAddressFunctions
    {
        /// <summary>
        /// Ignores whether or not the row and/or column of the input address are absolute.
        /// Examples:<br/>
        /// ChangeAbsolute(B3, false, false) = B3<br/>
        /// ChangeAbsolute(B3, true, false) = $B3<br/>
        /// ChangeAbsolute(B3, false, true) = B$3<br/>
        /// ChangeAbsolute(B3, true, true) = $B$3<br/>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="absoluteColumn"></param>
        /// <param name="absoluteRow"></param>
        /// <returns></returns>
        public static string ChangeAbsolute(string address, bool absoluteColumn, bool absoluteRow)
        {
            if (absoluteRow || absoluteColumn)
            {
                int x = 666;
            }
            var returnValue = new string(address.Where(x => x != '$').ToArray());
            if (absoluteRow)
            {
                var firstNumberIndex = 0;
                while (char.IsLetter(returnValue[firstNumberIndex]))
                {
                    firstNumberIndex++;
                }
                returnValue = $"{returnValue.Substring(0, firstNumberIndex)}${returnValue.Substring(firstNumberIndex)}";

            }
            if (absoluteColumn)
            {
                returnValue = $"${returnValue}";
            }
            return returnValue;
        }
    }
}
