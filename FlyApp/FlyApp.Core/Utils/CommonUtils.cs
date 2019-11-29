using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Decimal;

/*
 * CommonUtils: Support methods. All static.
 * Grouped in to many sections
 */

namespace FlyApp.Core.Utils
{
    public static class CommonUtils
    {
        #region String

        // Indent all lines (separated by '\n'), including the first line, 
        // by number of leadingSpaces space  characters.
        public static string IndentLines(string textBlock, int leadingSpaces = 2)
        {
            if (string.IsNullOrEmpty(textBlock)) return textBlock;

            // No-op
            if (leadingSpaces <= 0) return textBlock;

            // Generate prefix string
            var prefix = leadingSpaces == 2 ? "  " : new string(' ', leadingSpaces);

            // Apply prefix to the first line
            var s1 = prefix + textBlock;
            // Apply prefix to other lines
            var s = s1.Replace("\n", "\n" + prefix);

            return s;
        }

        // String from an IEnumerable collection
        // No Header line if header == null
        public static string CollectionToString<T>(IEnumerable<T> collection, string header)
        {
            var s = new StringBuilder();

            int n;
            if (collection != null)
            {
                n = 0;
                foreach (var item in collection) n++;
            }
            else
            {
                n = 0;
            }

            // Header line
            var indent = false;
            if (!string.IsNullOrEmpty(header))
            {
                if (collection != null)
                    s = s.Append($"{header} ({n}):");
                else
                    s = s.Append($"{header} (null):");

                indent = true;
            }

            // All collection items
            if (n > 0)
                foreach (var item in collection)
                {
                    var s1 = item.ToString();
                    if (indent) s1 = IndentLines(s1);

                    if (s.Length > 0) s = s.Append("\n");

                    s = s.Append(s1);
                }

            return s.ToString();
        }

        #endregion String

        #region Time

        // Convert Hours/Minutes/Seconds to seconds
        public static int HoursMinutesSecondsToSeconds((int hours, int minutes, int seconds) hms)
        {
            var nSeconds = hms.hours * 3600 + hms.minutes * 60 + hms.seconds;
            return nSeconds;
        }

        // Convert seconds number to hours/minutes/seconds parts
        public static (int, int, int) SecondsToHoursMinutesSeconds(int nSeconds)
        {
            var hours = nSeconds / 3600;
            var n = nSeconds - hours * 3600;
            var minutes = n / 60;
            var seconds = n - minutes * 60;

            return (hours, minutes, seconds);
        }

        // Convert seconds number to time string HH:mm:ss
        // showMinutes: false means skip minutes/seconds parts
        // showSeconds false means skip seconds part
        // skipPartIfZero: If the trailing part is zero, skip it
        public static string SecondsToString(int nSeconds, bool showMinutes, bool showSeconds,
            bool skipPartIfZero = false)
        {
            // Get hours/minutes/seconds parts
            (int hours, int minutes, int seconds) hms = SecondsToHoursMinutesSeconds(nSeconds);

            var s = "";

            if (showMinutes && showSeconds)
            {
                s = $"{hms.hours}:{hms.minutes:D2}:{hms.seconds:D2}";
                if (skipPartIfZero)
                {
                    // Skip Seconds if zero
                    if (s.EndsWith(":00")) s = s.Substring(0, s.Length - 3);

                    // Skip Minutes if zero
                    if (s.EndsWith(":00")) s = s.Substring(0, s.Length - 3);
                }
            }
            else if (showMinutes && !showSeconds)
            {
                s = $"{hms.hours}:{hms.minutes:D2}";
                // Skip Minutes if zero
                if (s.EndsWith(":00")) s = s.Substring(0, s.Length - 3);
            }
            else if (!showMinutes)
            {
                s = $"{hms.hours}";
            }

            return s;

            /* Archive
            // hours to string. Blank if 0.
            string s1 = (hms.hours == 0) ? "" : String.Format("{0}:", hms.hours);

            // Minutes to string.
            string s2;
            if (hms.minutes == 0)
            {
                // Blank if hours = minutes = 0
                if (hms.hours == 0)
                    s2 = "";
                // "00" if hours != 0
                else
                    s2 = "00:";
            }
            else
            {
                if (hms.hours == 0)
                    s2 = hms.minutes.ToString() + ":";
                else
                    s2 = hms.minutes.ToString("D2") + ":";
            }

            // Seconds to string
            string s3;
            // One or two digit seconds part if hours = minutes = 0
            if (hms.hours == 0 && hms.minutes == 0)
                s3 = hms.seconds.ToString();
            // Two digit seconds part
            else
                s3 = hms.seconds.ToString("D2");

            string s = s1 + s2 + s3;
            */
        }

        // Test SecondsToString(int nSeconds) method
        public static void Test_SecondsToString()
        {
            string s;

            s = SecondsToString(0, true, true); // "0:00:00"
            s = SecondsToString(0, true, false); // "0:00"
            s = SecondsToString(0, false, true); // "0"

            s = SecondsToString(1, true, true); // "0:00:01"
            s = SecondsToString(1, true, false); // "0:00"
            s = SecondsToString(1, false, true); // "0"

            s = SecondsToString(10, true, true); // "0:00:10"
            s = SecondsToString(10, true, false); // "0:00"
            s = SecondsToString(10, false, true); // "0"

            s = SecondsToString(60, true, true); // "0:01:00"
            s = SecondsToString(60, true, false); // "0:01"
            s = SecondsToString(60, false, true); // "0"

            s = SecondsToString(61, true, true); // "0:01:01"
            s = SecondsToString(61, true, false); // "0:01"
            s = SecondsToString(61, false, true); // "0"

            s = SecondsToString(71, true, true); // "0:01:11"
            s = SecondsToString(71, true, false); // "0:01"
            s = SecondsToString(71, false, true); // "0"

            s = SecondsToString(601, true, true); // "0:10:01"
            s = SecondsToString(601, true, false); // "0:10"
            s = SecondsToString(601, false, true); // "0"

            s = SecondsToString(3600, true, true); // "1:00:00"
            s = SecondsToString(3600, true, false); // "1:00"
            s = SecondsToString(3600, false, true); // "1"

            s = SecondsToString(3601, true, true); // "1:00:01"
            s = SecondsToString(3601, true, false); // "1:00"
            s = SecondsToString(3601, false, true); // "1"

            s = SecondsToString(3660, true, true); // "1:01:00"
            s = SecondsToString(3660, true, false); // "1:01"
            s = SecondsToString(3660, false, true); // "1"

            s = SecondsToString(36000, true, true); // "10:00:00"
            s = SecondsToString(36000, true, false); // "10:00"
            s = SecondsToString(36000, false, true); // "10"
        }

        #endregion Time

        #region Rounding

        public static int RoundNumberHigher(double number, double elasticity)
        {
            // Get floor
            var n = (int) Math.Floor(number);
            // number - floor <= elasticity, round lower; else higher
            int result;
            if (number - n <= elasticity)
                result = n;
            else
                result = (int) Math.Ceiling(number);

            return result;
        }


        public static int RoundToHours(int seconds, bool roudUp)
        {
            var hours = 0;
            if (roudUp)
                hours = (int) Math.Ceiling(seconds / 3600.0);
            else
                hours = (int) ((seconds + 30 * 60) / 3600.0);

            return hours * 3600;
        }

        public static int RoundTo30Minutes(int seconds, bool roudUp)
        {
            var minutes = 0;
            if (roudUp)
                minutes = (int) Math.Ceiling(seconds / (30.0 * 60));
            else
                minutes = (int) ((seconds + 30 * 60 / 2) / (30 * 60.0));

            return minutes * 30 * 60;
        }

        public static int RoundTo15Minutes(int seconds, bool roudUp)
        {
            var minutes = 0;
            if (roudUp)
                minutes = (int) Math.Ceiling(seconds / (15.0 * 60));
            else
                minutes = (int) ((seconds + 15 * 60 / 2) / (15 * 60.0));

            return minutes * 15 * 60;
        }

        public static int RoundToMinutes(int seconds, bool roudUp)
        {
            var minutes = 0;
            if (roudUp)
                minutes = (int) Math.Ceiling(seconds / 60.0);
            else
                minutes = (int) ((seconds + 30) / 60.0);

            return minutes * 60;
        }

        #endregion Rounding

        #region Various Validations

        // TODO: ValidateEmailAddress: Test this

        private static Regex validEmailRegex;

        public static bool ValidateEmailAddress(string emailAddress)
        {
            if (validEmailRegex == null)
            {
                var validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                                        + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                                        + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
                validEmailRegex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);

                TestValidateEmailAddress();
            }

            if (validEmailRegex == null) return true;

            var b = validEmailRegex.IsMatch(emailAddress);
            return b;
        }

        private static void TestValidateEmailAddress()
        {
            bool b;

            b = ValidateEmailAddress("a@a.com");
            b = ValidateEmailAddress("a@a.uk");
            b = ValidateEmailAddress("a@a.a");
            b = ValidateEmailAddress("a");
            b = ValidateEmailAddress("@");
            b = ValidateEmailAddress(".com");
        }

        #endregion Various Validations

        #region Misc

        // Given a list of names, generate a unique new name starting with the given prefix.
        // The unique pattern used is: "<prefix> <n>", where n starts from 1.
        public static string GenerateUniqueName<T>(this IEnumerable<T> items, Func<T, string> nameSelector,
            string prefix)
        {
            var entityName = "";

            // Loop for a large arbitrary number.
            for (var i = 1; i < 10000; i++)
            {
                // Generate a test unique name for the given iteration
                entityName = $"{prefix} {i}";
                // Match it with the list of names
                var entityNameExists =
                    items.Any(x => nameSelector(x).Equals(entityName, StringComparison.OrdinalIgnoreCase));
                // Does not match -> found a unique name
                if (!entityNameExists) return entityName;
            }

            // Return some arbitrary string. It may NOT be unique.
            entityName = "Use a different prefix";
            return entityName;
        }

        public static double BytesToMb(this long bytes)
        {
            return (double) bytes / 1024 / 1024;
        }

        #endregion Misc

        #region Decimal/Money

        public static string DecimalToString(decimal value, bool blankZero = true, bool blankZeroDecimal = true)
        {
            var s = "";

            // Check if Zero
            if (blankZero && value == 0) return s;

            // Check if Decimal part zero
            var hasZeroDecimal = value % 1 == 0;
            /*
            hasZeroDecimal = false;
            if (blankZeroDecimal)
            {
                int n = (int) money;
                hasZeroDecimal = (money == n);
            }
            */

            var format = blankZeroDecimal && hasZeroDecimal ? "N0" : "N2"; // With thousands separators
            s = value.ToString(format);
            return s;
        }

        public static string MoneyToString(decimal money, bool blankZero = true, bool blankZeroDecimal = true,
            bool showCurrencySymbol = true)
        {
            var s = DecimalToString(money, blankZero, blankZeroDecimal);
            if (showCurrencySymbol) s = "$" + s;

            return s;
        }

        public static void TestMoneyToString()
        {
            var s = "";

            s = MoneyToString(0, false, false, false); // 0.00
            s = MoneyToString(0, true, false, false); // ""
            s = MoneyToString(0, false, true, false); // 0
            s = MoneyToString(0, false, false); // $0.00
            s = MoneyToString(0, true, false); // ""
            s = MoneyToString(0, false); // $0

            s = MoneyToString(1, false, false, false); // 1.00
            s = MoneyToString(1, true, false, false); // 1.00
            s = MoneyToString(1, false, true, false); // 1

            s = MoneyToString(1.2m, false, false, false); // 1.20
            s = MoneyToString(1.2m, true, false, false); // 1.20
            s = MoneyToString(1.2m, false, true, false); // 1.20

            s = MoneyToString(1.23m, false, false, false); // 1.23
            s = MoneyToString(1.23m, true, false, false); // 1.23
            s = MoneyToString(1.23m, false, true, false); // 1.23

            s = MoneyToString(1.234m, false, false, false); // 1.23
            s = MoneyToString(1.234m, true, false, false); // 1.23
            s = MoneyToString(1.234m, false, true, false); // 1.23

            s = MoneyToString(1.236m, false, false, false); // 1.24
            s = MoneyToString(1.236m, true, false, false); // 1.24
            s = MoneyToString(1.236m, false, true, false); // 1.24

            s = MoneyToString(1234567.236m, false, false, false); // 1,234,567.24
            s = MoneyToString(1234.00m, true, true, false); // 1,234
            s = MoneyToString(12.34m, false, true, false); // 12.34
        }

        public static decimal RoundMoneyToTwoDecimalPlaces(decimal money)
        {
            var d = Math.Round(money, 2, MidpointRounding.ToEven);
            return d;
        }

        public static double RoundToDecimalPlaces(double value, int places)
        {
            return ToDouble(Math.Round((decimal) value, places, MidpointRounding.ToEven));
        }

        #endregion Decimal/Money

        #region Log Output

        public static void LogMessage(string message, string context = "")
        {
            Debug.WriteLine(message);
        }

        public static void LogError(string message, string context = "")
        {
            Debug.WriteLine(message);
        }

        #endregion Log Output
    }
}