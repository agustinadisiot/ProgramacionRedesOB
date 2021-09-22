using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Utils
{
    public static class Validation
    {
        public static bool IsValidEntry(string word)
        {
            bool isEmptyString = word.Length == 0;
            bool containsDelimiter = word.Contains(Specification.delimiter); // agregar el otro delimiter
            return (!isEmptyString && !containsDelimiter);
        }

        public static bool IsValidNumber(string number, int min, int max)
        {
            int input;
            bool isANumber = int.TryParse(number, out input);
            bool inRange = (input >= min && input <= max);
            return isANumber && inRange;
        }
    }
}
