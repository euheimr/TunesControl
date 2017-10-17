// 02/01/2017 - Milo Johnson - Updated TrimCleanPad(), TrimClean(), Clean()

using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace ExtensionMethods
{

    public static class BoolExtensionMethods
    {
        public static string ToString(this bool value, string yes, string no)
        {
            if (value)
            { return yes; }
            else
            { return no; }
        }
    }


    public static class DateTimeExtensionMethods
    {

        public static string ToSQLDateString(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static string ToDateString(this DateTime value)
        {
            return value.ToString("MM/dd/yyyy");
        }

        public static string ToTimeString(this DateTime value)
        {
            return value.ToString("hh:mm tt");
        }

        public static string ToDateTimeString(this DateTime value)
        {
            return value.ToString("MM/dd/yyyy hh:mm tt");
        }

        public static string ToFileDateTimeString(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd_hh-mm-ss-tt");
        }

        public static DateTime ToDate(this DateTime value)
        {
            return Convert.ToDateTime(value.ToString("MM/dd/yyyy"));
        }

        public static int Age(this DateTime birthday, DateTime? target = null, bool leapYearBirthdayMarchFirst = false)
        {
            // default to today if no date is specified
            if (target == null) { target = System.DateTime.Today; }

            // remove time
            birthday = birthday.Date;
            target = target.Value.Date;

            int age = target.Value.Year - birthday.Year;
            if (leapYearBirthdayMarchFirst)
            {
                if (birthday > target.Value.AddYears(-age)) age--;     // If born on Leap Year 02/29/2000, would turn 21 on 03/01/2021)
            }
            else
            {
                if (target.Value < birthday.AddYears(age)) age--;    // If born on Leap Year 02/29/2000, would turn 21 on 02/28/2021)
            }
            return age;

        }

    }

    public static class ByteExtensionMethods
    {

        public static bool ArraysEqual(this byte[] first, byte[] second)
        {

            if ((object.ReferenceEquals(first, second)))
                return true;
            if ((first == null || second == null))
                return false;
            if ((first.Length != second.Length))
                return false;

            for (int i = 0; i <= first.Length - 1; i++)
            {
                if (first[i].Equals(second[i]) == false)
                    return false;
            }
            return true;

        }
    }

    public static class StringExtensions
    {

        public enum PadDirection
        {
            Left,
            Right
        }


        public enum StringContent
        {
            LettersOnly,
            NumbersOnly,
            LettersSpaces,
            LettersNumbers,
            LettersNumbersSpaces,
            Any
        }

        /// <summary>
        /// Parses and separates a SQL delimited path and removes delimiters and escape characters.
        /// </summary>
        /// <param name="value">Delimited SQL path</param>
        /// <returns>Each SQL parts with delimiters and escape characters removed</returns>
        /// <example>[Database].[dbo].[Table]</example>
        /// <remarks>  Ref SELECT QUOTENAME('abc[]def')</remarks>
        public static string[] ParseSQLPath(this string value)
        {
            List<string> tokens = new List<string>();

            StringBuilder sb = new StringBuilder();
            bool escaped = false;
            char cLast = '\0';

            foreach (char c in value)
            {

                if (!escaped && c.Equals('.'))
                {
                    // we have a token
                    tokens.Add(sb.ToString().Trim());
                    sb.Clear();
                }
                else if (escaped && c.Equals('.'))
                {
                    // ignore but add to string
                    sb.Append(c);
                }
                else if (!escaped && c.Equals('['))
                {
                    // begin escape
                    escaped = true;
                }
                else if (escaped && c.Equals(']'))
                {
                    escaped = false;
                }
                else if (!escaped && c.Equals(']') && cLast.Equals(']'))
                {
                    // double ]] suprise!  we are still escaped add the ]
                    escaped = true;
                    sb.Append(c);
                }
                else
                {
                    sb.Append(c);
                }

                cLast = c;

            }
            tokens.Add(sb.ToString().Trim());
            return tokens.ToArray();
        }

        public static bool ToBool(this string value)
        {

            bool flag;
            Int32 i;

            if (Boolean.TryParse(value, out flag))
            {
                // parsed as bool
                return flag;
            }
            else if (Int32.TryParse(value, out i))
            {
                // parsed as int
                if (i == 0)
                { return false; }
                else
                { return true; }
            }
            else
            {
                // string?
                value = value.Trim().ToUpper();
                if (value.StartsWith("T") || value.StartsWith("Y"))
                    return true;
                else
                    return false;
            }

        }

        public static bool IsInt(this string value)
        {
            Int32 i = 0;
            if (Int32.TryParse(value, out i))
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public static int? ToIntNullable(this string value)
        {
            int outValue;
            return int.TryParse(value, out outValue) ? (int?)outValue : null;

        }

        public static bool IsNumeric(this string value)
        {
            double i = 0;
            if (double.TryParse(value, out i))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static string ToDirSlashString(this string value)
        {
            value = value.TrimEnd();
            if (value.EndsWith("\\") == true)
            {
                return value;
            }
            else
            {
                return value.Trim() + "\\";
            }
        }

        public static List<string> ParseCommaList(this string value)
        {

            List<string> list = new List<string>();

            if (value.Trim().Length > 0)
            {
                foreach (string v in value.Split(','))
                {
                    string item = v.Trim();
                    if (item.Length > 0)
                    {
                        list.Add(item);
                    }
                }
            }

            return list;

        }

        public static string EscapeRowFilterLikeValue(this string value)
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '*' || c == '%' || c == '[' || c == ']')
                {
                    sb.Append("[").Append(c).Append("]");
                }
                else if (c == '\'')
                {
                    sb.Append("''");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();

        }

        public static string EscapeWhereValue(this string value)
        {
            return value.Replace("'", "''");
        }

        public static string EscapeXMLData(this string value)
        {
            // this works as well
            //XmlDocument doc = new XmlDocument();
            //var node = doc.CreateElement("root");
            //node.InnerText = unescaped;
            //return node.InnerXml;

            return System.Security.SecurityElement.Escape(value);
        }

        public static bool DateCompare(this string a, string b)
        {

            DateTime dateA;
            DateTime dateB;
            DateTime.TryParse(a, out dateA);
            DateTime.TryParse(b, out dateB);
            if (dateA == dateB)
            {
                return true;
            }
            else
            {
                return false;
            }



        }

        public static bool IntCompare(this string valueA, string valueB)
        {
            Int32 a;
            Int32 b;

            if ((Int32.TryParse(valueA, out a) && Int32.TryParse(valueB, out b)))
            {
                if (a == b)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }


        public enum NameType
        {
            FirstName,
            MiddleName,
            LastName,
            WholeName,
            Address,
            PhoneNumber
        }
        public static string ToTitleCase(this string value, NameType nameType = NameType.WholeName)
        {

            if (nameType == NameType.PhoneNumber)
            { throw new Exception("This method does not support PhoneNumber at this time."); }

            if (value.Length == 0) { return value; }

            string nonAlpha = @"!""#$%&'()*+,-./:;<=>?@[]\^_`{|}~0123456789";
            string numeric = @"0123456789";

            TextInfo ti = new CultureInfo("en-US", false).TextInfo;

            string romanNumerals = "|II|III|IV|VI|VII|VIII|IX|XI|XII||XIII|XIV|XV|XVI|XVII|XVIII|XIX|XX|XXI|XXII|XXIII|XXIV|XXV|XXVI|XXVII|XXVIII|XXIX|XXX|";
            string caseExceptions = "|DDS|MD|RDA|MBA|JJ|" // SOME UPPER
            + "with|when|and|or|an|the|from|to|on|as|of|in|at|for|"  // some lower
            + "PhD|" // SoMe MiXeD
            + "MacBride|MacCullough|MacCracken|MacDermott|MacDonald|MacDougal|MacDougall|MacDowell|MacFarland|MacFarlane|MacGarvey|MacGowan|"
            + "MacKinnon|MacLennan|MacArthur|MacDula|MacDowell|MacGregor|MacDowell|MacMurray|MacNeil|MacPherson|MacPhee|"; // common Mac (There are way more Macblah names then MacBlah, so be careful)

            // if address, add some others
            if (nameType == NameType.Address)
            {
                caseExceptions += "PO|US|USA|SW|SE|NW|NE|N|E|W|S|LLP|LLC|";
            }


            // used for comparisons
            string romanNumeralsLower = romanNumerals.ToLower();
            string caseExceptionsLower = caseExceptions.ToLower();

            string name = value.ToLower().Trim();

            // space non-alphanumeric chars for better splitting and checking
            if (name.IndexOfAny(nonAlpha.ToCharArray()) != -1)
            {
                foreach (char c in nonAlpha.ToCharArray())
                {
                    name = name.Replace(c.ToString(), " " + c.ToString() + " ");
                }
                name = name.Replace(" ' s ", "'s ");  // unspace any possessive apostrophes
                name = name.Trim();
            }


            // fix each word
            int i = 1;
            string[] names = name.Split(' ');
            string nameBuild = string.Empty;
            foreach (string piece in names)
            {

                string pieceLower = piece.ToLower();
                string pieceBuild = string.Empty;

                if (piece.IsNullOrEmpty() || "0123456789".Contains(pieceLower.Substring(0, 1)))
                {
                    // if it is empty, or starts with a number (1st, 2nd, etc.), leave as lower case
                    pieceBuild = pieceLower;
                }
                else if (((nameType == NameType.Address) || (nameType == NameType.LastName) || (nameType == NameType.WholeName && i >= 2)) && caseExceptionsLower.Contains("|" + pieceLower + "|"))
                {
                    // substitute caseExceptions (address, last name)
                    pieceBuild = caseExceptions.Substring(caseExceptionsLower.IndexOf("|" + pieceLower + "|") + 1, pieceLower.Length);
                }
                else if (((nameType == NameType.Address) || (nameType == NameType.LastName && i >= 2) || (nameType == NameType.WholeName && i >= 3)) && romanNumeralsLower.Contains("|" + pieceLower + "|"))
                {
                    // substitute roman numeral (address, right of last name)
                    pieceBuild = romanNumerals.Substring(romanNumeralsLower.IndexOf("|" + pieceLower + "|") + 1, pieceLower.Length);
                }
                else if (((nameType == NameType.Address) || (nameType == NameType.LastName) || (nameType == NameType.WholeName && i >= 2)) && pieceLower.StartsWith("mc"))
                {
                    // substiture Mc (address, last name)
                    pieceBuild = pieceLower.Substring(2);
                    pieceBuild = "Mc" + ti.ToTitleCase(pieceBuild);
                }
                //else if (pieceLower.StartsWith("mac")) 
                //{
                //    pieceBuild = pieceLower.Substring(3);
                //    pieceBuild = "Mac" + ti.ToTitleCase(pieceBuild);
                //}
                else
                {
                    // regular title case this part
                    pieceBuild = ti.ToTitleCase(pieceLower);
                }

                if (nameBuild.IsNullOrEmpty())
                { nameBuild = pieceBuild; }
                else
                { nameBuild = nameBuild + " " + pieceBuild; }

                i += 1;

            }
            name = nameBuild;

            // un-space non-alphanumeric chars after splitting and checking
            if (name.IndexOfAny(nonAlpha.ToCharArray()) != -1)
            {
                name = " " + name + " ";
                foreach (char c in nonAlpha.ToCharArray())
                {
                    name = name.Replace(" " + c.ToString() + " ", c.ToString());
                }
                name = name.Trim();
            }

            // fix numeric suffix 1st, 2nd, 3rd, Xth
            if (name.IndexOfAny(numeric.ToCharArray()) != -1)
            {
                foreach (char c in numeric.ToCharArray())
                {
                    switch (c)
                    {
                        case '1':
                            name = name.Replace(c.ToString() + "St", c.ToString() + "st");
                            break;
                        case '2':
                            name = name.Replace(c.ToString() + "Nd", c.ToString() + "nd");
                            break;
                        case '3':
                            name = name.Replace(c.ToString() + "Rd", c.ToString() + "Rd");
                            break;
                        default:
                            name = name.Replace(c.ToString() + "Th", c.ToString() + "th");
                            break;
                    }

                }
            }


            // first letter should always be upper case
            if (name.Length == 1)
            {
                name = name.Substring(0, 1).ToUpper();
            }
            else if (name.Length > 1)
            {
                name = name.Substring(0, 1).ToUpper() + name.Substring(1);
            }


            return name;

        }

        public static bool IsOnlyChars(this string value, string charString)
        {

            bool returnValue = true;
            int i = 0;

            // for each char in searchString
            for (i = 0; i < value.Length; i++)
            {

                // does this char exist in the list of charString?
                if (charString.IndexOf(value.Substring(i, 1)) == -1)
                {
                    returnValue = false;
                    break;
                }

            }
            return returnValue;

        }

        public static bool IsOnlyNumbers(this string value)
        {
            return IsOnlyChars(value, "0123456789");
        }

        public static string TrimCleanPad(this string value, StringContent preserveContent, PadDirection padDirection, char padChar, int padLength, bool removeDoubleSpaces = false, bool forceNullToString = true, bool forceFixedLength = true)
        {

            // trim and clean
            value = value.TrimClean(preserveContent, removeDoubleSpaces, forceNullToString);

            // if value is still a null then nothing to do.
            if (value == null)
            { return value; }

            // pad
            switch (padDirection)
            {
                case PadDirection.Left:
                    value = value.PadLeft(padLength, padChar);
                    break;
                case PadDirection.Right:
                    value = value.PadRight(padLength, padChar);
                    break;
            }

            // force the length
            if (forceFixedLength && value.Length > padLength)
            {
                value = value.Substring(0, padLength);
            }

            return value;

        }

        public static string TrimClean(this string value, StringContent preserveContent, bool removeDoubleSpaces = false, bool forceNullToString = true)
        {

            value = value.Clean(preserveContent, removeDoubleSpaces, forceNullToString).Trim();

            if (value == null)
            {
                // cant trim a null
                return value;
            }
            else
            {
                // return trimmed
                return value.Trim();
            }
        }

        public static string Clean(this string value, StringContent preserveContent, bool removeDoubleSpaces = false, bool forceNullToString = true)
        {

            if (forceNullToString)
            { value = value.EmptyIfNull(); }

            // nothing to clean if null or empty
            if (value.IsNullOrEmpty())
            { return value; }

            StringBuilder sb = new StringBuilder();

            switch (preserveContent)
            {
                case StringContent.LettersOnly:
                    foreach (char c in value)
                    {
                        if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                        { sb.Append(c); }
                    }
                    break;
                case StringContent.LettersSpaces:
                    foreach (char c in value)
                    {

                        if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c == ' '))
                        { sb.Append(c); }

                    }
                    break;
                case StringContent.NumbersOnly:
                    foreach (char c in value)
                    {
                        if ((c >= '0' && c <= '9'))
                        { sb.Append(c); }
                    }
                    break;
                case StringContent.LettersNumbers:
                    foreach (char c in value)
                    {
                        if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                        { sb.Append(c); }

                    }
                    break;
                case StringContent.LettersNumbersSpaces:
                    foreach (char c in value)
                    {
                        if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || (c == ' '))
                        { sb.Append(c); }

                    }
                    break;
                case StringContent.Any:
                    {
                        sb = new StringBuilder(value);
                    }
                    break;

            }

            if (removeDoubleSpaces)
            {
                // remove double spaces
                return sb.ToString().RemoveDoubleSpaces();
            }
            else
            {
                return sb.ToString();
            }

        }

        public static string Chop(this string value, int length, bool trim = true, bool forceNullToString = true)
        {

            if (forceNullToString)
            { value = value.EmptyIfNull(); }

            // dont need to chop a null or empty
            if (value.IsNullOrEmpty())
            { return value; }

            if (value.Length > length)
            {
                value = value.Substring(0, length);
            }

            if (trim)
            { value = value.Trim(); }

            return value;

        }

        public static string EmptyIfNull(this string value)
        {
            if (value.IsNullOrEmpty())
            { return string.Empty; }
            else
            { return value; }

        }

        public static string GetLast(this string value, int tailLength, bool trim = true)
        {
            if (value.IsNullOrEmpty())
            { return value; }

            if (tailLength < value.Length)
            {
                value = value.Substring(value.Length - tailLength);
            }

            if (trim)
            { value = value.Trim(); }


            return value;

        }

        public static string CharDetails(this string value)
        {
            if (value.IsNullOrEmpty())
            { return value; }

            string returnValue = value.Length.ToString() + " chars" + Environment.NewLine;
            int i = 0;
            foreach (char c in value.ToCharArray())
            {
                returnValue += "" + i.ToString() + ":" + ((int)c).ToString() + ":" + c.ToString() + " ";
                i++;
            }

            return returnValue;
        }

        public static string RemoveDoubleSpaces(this string value)
        {
            while (value.Contains("  "))
            {
                value = value.Replace("  ", " ");
            }
            return value;

        }

        public static string SpaceOutNumbers(this string value)
        {

            if (value.IsNullOrEmpty())
            { return value; }

            StringBuilder sb = new StringBuilder();

            char l = ' ';
            foreach (char c in value)
            {
                if ((c >= '0' && c <= '9'))
                {
                    // current is a number and last wasnt a space or number
                    if (l != ' ' && !(l >= '0' && l <= '9')) { sb.Append(' '); }  // add space
                }
                else
                {
                    // current isnt a number. current isnt a space and last wasnt a space or number
                    if (c != ' ' && l != ' ' && (l >= '0' && l <= '9')) { sb.Append(' '); } // add space
                }

                sb.Append(c); // add char
                l = c; // hold current as last
            }

            return sb.ToString();
        }



        public enum PhoneFormat
        {
            Parentheses,
            Dashes,
            Dots,
        }
        public static string ToPhoneFormat(this string value, PhoneFormat phoneFormat = PhoneFormat.Parentheses, string extPrefix = " x")
        {

            // only numbers
            value = value.Clean(StringContent.NumbersOnly);

            if (value.IsNullOrEmpty()) { return value; }

            if (value.StartsWith("1"))
            {
                value = value.Substring(1, value.Length - 1);
            }

            // break out
            string phone = string.Empty;
            string ext = string.Empty;
            if (value.Length < 10)
            {
                // dont know what this is
                return value;
            }
            else if (value.Length == 10)
            {
                phone = value;
            }
            else if (value.Length > 10)
            {
                phone = value.Substring(0, 10);
                ext = value.Substring(10, value.Length - 10);
            }

            // format
            switch (phoneFormat)
            {
                case PhoneFormat.Dots:
                    phone = String.Format(@"{0:###\.###\.####}", double.Parse(phone));
                    break;
                case PhoneFormat.Dashes:
                    phone = String.Format(@"{0:###-###-####}", double.Parse(phone));
                    break;
                default:
                    phone = String.Format(@"{0:(###) ###-####}", double.Parse(phone));
                    break;

            }

            // add ext if there is one
            if (ext.Length > 0)
            {
                phone = phone + extPrefix + ext;
            }

            return phone;

        }


        public static bool IsValidEmailAddress(this string value, int maxLength)
        {
            value = value.Trim();

            // remember: email addresses dont require a "."
            // a@b is a valid email address
            // http://en.wikipedia.org/wiki/Email_address

            // length
            if (value.Length < 3)
            { return false; }

            // check max length
            if (value.Length > maxLength)
            { return false; }

            // only one @
            if (value.Where(x => x == '@').Count() != 1)
            { return false; }

            // @ cant be first char
            if (value.Substring(0, 1) == "@")
            { return false; }

            // @ cant be last char
            if (value.Substring(value.Length - 1, 1) == "@")
            { return false; }

            return true;
        }

    }

    public static class IntExtensions
    {

        public static string ToCommaString(this int value)
        {
            return value.ToString("#,#0");
        }

        public static string BoWantsAnS(this int value, string singular = "", string plural = "s")
        {

            if (value == 1)
            { return singular; }
            else
            { return plural; }

        }

    }

    public static class Int64Extensions
    {

        public static string ToCommaString(this Int64 value)
        {
            return value.ToString("#,#0");
        }

        public static string BoWantsAnS(this Int64 value, string singular = "", string plural = "s")
        {

            if (value == 1)
            { return singular; }
            else
            { return plural; }

        }

    }

    public static class DecimalExtensions
    {

        public static string ToCommaString(this decimal value)
        {
            return value.ToString("#,#0.################");
        }

        public static string BoWantsAnS(this decimal value, string singular = "", string plural = "s")
        {

            if (value == 1)
            { return singular; }
            else
            { return plural; }

        }

    }

    public static class DoubleExtensions
    {

        public static string ToCommaString(this double value)
        {
            return value.ToString("#,#0.################");
        }

        public static string BoWantsAnS(this double value, string singular = "", string plural = "s")
        {

            if (value == 1)
            { return singular; }
            else
            { return plural; }

        }
    }

    public static class DataTableExtensions
    {

        public static string ToHTMLTable(this DataTable data, string tableAttributes, string tableHeaderRowAttributes, string tableHeaderAttributes, string tableRowAttributes, string tableDataAttributes)
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(tableAttributes))
            { sb.AppendLine("<table>"); }
            else
            { sb.AppendLine("<table " + tableAttributes + ">"); }


            // headers
            if (string.IsNullOrEmpty(tableHeaderRowAttributes))
            { sb.AppendLine("<tr>"); }
            else
            { sb.AppendLine("<tr " + tableHeaderRowAttributes + ">"); }

            foreach (DataColumn dc in data.Columns)
            {

                if (string.IsNullOrEmpty(tableHeaderAttributes))
                { sb.AppendFormat("<th>{0}</th>", dc.ColumnName); }
                else
                { sb.AppendFormat("<th " + tableHeaderAttributes + ">{0}</th>", dc.ColumnName); }

            }

            sb.AppendLine("</tr>");

            // data rows
            foreach (DataRow dr in data.Rows)
            {
                if (string.IsNullOrEmpty(tableRowAttributes))
                { sb.AppendLine("<tr>"); }
                else
                { sb.AppendLine("<tr " + tableRowAttributes + ">"); }

                foreach (DataColumn dc in data.Columns)
                {
                    string cellValue = dr[dc] != null ? dr[dc].ToString() : "";

                    if (string.IsNullOrEmpty(tableDataAttributes))
                    { sb.AppendFormat("<td>{0}</td>", cellValue); }
                    else
                    { sb.AppendFormat("<td " + tableDataAttributes + ">{0}</td>", cellValue); }

                }

                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        public static bool IsNumeric(this DataColumn col)
        {
            if (col == null)
                return false;
            var numericTypes = new[] { typeof(Byte), typeof(Decimal), typeof(Double),
                typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte),
                typeof(Single), typeof(UInt16), typeof(UInt32), typeof(UInt64)};

            return numericTypes.Contains(col.DataType);
        }

        public static Dictionary<TF, List<TP>> CreateForeignKeyToPrimaryKeyLookup<TF, TP>(this DataTable data, string fkField, string pkField)
        {

            var fks2pks = new Dictionary<TF, List<TP>>();

            foreach (DataRow row in data.Rows)
            {

                // get the foreign key and primary key values
                var fkValue = (TF)row[fkField]; // (TF)Convert.ChangeType(row[fkField], typeof(TF));
                var pkValue = (TP)row[pkField]; // (TP)Convert.ChangeType(row[pkField], typeof(TP));

                // see if this fkValue exists in our list
                var pkValues = new List<TP>();
                fks2pks.TryGetValue(fkValue, out pkValues);

                if (pkValues == null)
                {
                    // nope, it doesnt, so add it with pkValue
                    fks2pks.Add(fkValue, new List<TP> { pkValue });
                }
                else if (pkValues.Contains(pkValue) == false)
                {
                    // it did exist, but didnt have this pkValue, so add it
                    pkValues.Add(pkValue);
                }

            }

            return fks2pks;

        }

        public static List<DataRow> GetRowsByForeignKey<TF, TP>(this DataTable data, Dictionary<TF, List<TP>> fks2pks, TF fkValue)
        {

            var rows = new List<DataRow>();

            List<TP> pkValues;
            fks2pks.TryGetValue(fkValue, out pkValues);

            if (pkValues == null)
            { return rows; }

            foreach (TP pkValue in pkValues)
            {
                DataRow row = data.Rows.Find(pkValue);
                if (row != null)
                {
                    rows.Add(row);
                }

            }

            return rows;

        }


    }

    public static class ListExtensions
    {

        public static string GetValue(this List<KeyValuePair<string, string>> list, string key)
        {

            foreach (var kv in list)
            {
                if (kv.Key == key)
                { return kv.Value; }
            }

            return null;

        }

        public static string[] GetValue(this List<KeyValuePair<string, string[]>> list, string key)
        {

            foreach (var kv in list)
            {
                if (kv.Key == key)
                { return kv.Value; }
            }

            return null;

        }


    }

    public static class CharExtensions
    {

        public static bool IsLetter(this char c)
        {
            return IsLowerLetter(c) || IsUpperLetter(c);
        }

        public static bool IsLowerLetter(this char c)
        {
            return (c >= 'a' && c <= 'z');
        }

        public static bool IsUpperLetter(this char c)
        {
            return (c >= 'A' && c <= 'Z');
        }

        public static bool IsDigit(this char c)
        {
            return c >= '0' && c <= '9';
        }

        public static bool IsSymbol(this char c)
        {
            return c > 32 && c < 127 && !c.IsDigit() && !c.IsLetter();
        }


    }

}


