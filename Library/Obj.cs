using System;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Net;


namespace Library
{
    public class Obj
    {
        public static bool IsDate(object value)
        {
            if((object.ReferenceEquals(value, DBNull.Value)) || (value == null) || (Convert.ToString(value).Length == 0))
            {
                return false;
            }
            else
            {
                try
                {
                    DateTime o = Convert.ToDateTime(value);
                    return true;
                }
                catch(Exception ex)
                {
                    return false;
                }
            }
        }

        public static bool IsDecimal(object value)
        {
            if(object.ReferenceEquals(value, DBNull.Value) || (value == null) || (Convert.ToString(value).Length == 0))
            {
                return false;
            }
            else
            {
                try
                {
                    decimal d = Convert.ToDecimal(value);
                    return true;
                }
                catch(Exception ex)
                {
                    return false;
                }
            }
        }

        public static bool IsDouble(object value)
        {
            if (object.ReferenceEquals(value, DBNull.Value) || (value == null) || (Convert.ToString(value).Length == 0))
            {
                return false;
            }
            else
            {
                try
                {
                    double d = Convert.ToDouble(value);
                    return true;
                }
                catch(Exception ex)
                {
                    return false;
                }
            }
        }

        public static bool IsInt(object value)
        {
            if (object.ReferenceEquals(value, DBNull.Value) || (value == null) || (Convert.ToString(value).Length == 0))
            {
                return false;
            }
            else
            {
                try
                {
                    int i = Convert.ToInt32(value);
                    return true;
                }
                catch(Exception ex)
                {
                    return false;
                }
            }
        }

        public static bool ObjToBoolean(object value, bool defaultValue = false)
        {
            if (object.ReferenceEquals(value, DBNull.Value) || (value == null) || (Convert.ToString(value).Length == 0))
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToBoolean(value);
                }
                catch(Exception ex)
                {
                    return defaultValue;
                }
            }
        }

        public static decimal ObjToDecimal(object value, decimal defaultValue = 0)
        {
            if(object.ReferenceEquals(value, DBNull.Value) || (value == null) || (Convert.ToString(value).Length == 0))
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch(Exception ex)
                {
                    return defaultValue;
                }
            }
        }

        public static DateTime ObjToDate(object value, DateTime defaultValue = new DateTime())
        {
            if (object.ReferenceEquals(value, DBNull.Value) || (value == null) || (Convert.ToString(value).Length == 0))
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToDateTime(value);
                }
                catch(Exception ex)
                {
                    return defaultValue;
                }
            }
        }

        public static int ObjToInterger(object value, int defaultValue = 0)
        {
            if (object.ReferenceEquals(value, DBNull.Value) || (value == null) || (Convert.ToString(value).Length == 0))
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToInt32(value);
                }
                catch(Exception ex)
                {
                    return defaultValue;
                }
            }
        }
        
        public static Single ObjToSingle(object value, Single defaultValue = 0)
        {
            if (object.ReferenceEquals(value, DBNull.Value) || (value == null) || (Convert.ToString(value).Length == 0))
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToSingle(value);
                }
                catch(Exception ex)
                {
                    return defaultValue;
                }
            }
        }

        public static double ObjToDouble(object value, Single defaultValue = 0)
        {
            if (object.ReferenceEquals(value, DBNull.Value) || (value == null) || (Convert.ToString(value).Length == 0))
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToDouble(value);
                }
                catch(Exception ex)
                {
                    return defaultValue;
                }
            }
        }

        public static string ObjToString(object value, string defaultValue = "", bool trimValue = true)
        {
            if (object.ReferenceEquals(value, DBNull.Value) || (value == null))
            {
                if (trimValue)
                {
                    return defaultValue.Trim();
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                try
                {
                    if (trimValue)
                    {
                        return Convert.ToString(value).Trim();
                    }
                    else
                    {
                        return Convert.ToString(value);
                    }
                }
                catch(Exception ex)
                {
                    if (trimValue)
                    {
                        return defaultValue.Trim();
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
            }
        }

        public class StringLib
        {
            public enum Direction
            {
                Right = 0,
                Left = 1
            }

            public static bool IsOnlyChars(string searchString, string compareChars)
            {
                bool returnVal = true;
                int i = 0;

                //for each char in strSearch
                for (i = 0; i <= searchString.Length - 1; i++)
                {
                    //does this char exist in the list of strChars?
                    if(compareChars.IndexOf(searchString.Substring(i, 1)) == 0)
                    {
                        returnVal = false;
                    }
                }
                return returnVal;
            }

            public static bool IsOnlyAlphaNum(string searchString, string extraChars = "")
            {
                //build all num and letters
                string charList = "0123456789";
                charList += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                //add extra chars
                charList += extraChars;

                //check it (toUpper searches causes it to search lowercase too)
                return IsOnlyChars(searchString.ToUpper(), charList);
            }

            
            /*public static string GetOnlyChars(string searchString, string charString)
            {
                int i = 0;
                StringBuilder text = new StringBuilder();

                //loop thru each character
                for (i = 0; i <= searchString.Length - 1; i++)
                {
                    //if this is a good char, then add it
                    if(charString.IndexOf(searchString.Substring(i, 1) > -1))
                    {
                        text.Append(searchString.Substring(i, 1));
                    }
                }
                return text.ToString();
            }*/


            public static string SetStringLength(string stringData, int stringLength, char padChar, Direction padDirection)
            {
                if(stringData.Length > stringLength)
                {
                    stringData = stringData.Substring(0, stringLength);
                }

                if(padDirection == Direction.Right)
                {
                    stringData = stringData.PadRight(stringLength, padChar);
                }
                else
                {
                    stringData = stringData.PadLeft(stringLength, padChar);
                }
                return stringData;
            }

            public static string BoolToString(bool value, string trueString, string falseString)
            {
                if (value)
                {
                    return trueString;
                }
                else
                {
                    return falseString;
                }
            }
        }//end of the StringLib class

        //TODO - 
        /*
        public class Math
        {
            
            public static string WholeNumberToCurrency(object value, string defaultValue = "")
            {
                if (object.ReferenceEquals(value, DBNull.Value))
                {
                    return defaultValue;
                }
                else
                {
                    string returnValue = defaultValue;
                    try
                    {
                        returnValue = (ObjToDecimal(value) * 0.01).ToString("c");
                    }
                    catch (Exception ex)
                    {
                        //nothing
                    }
                    return returnValue;
                }
            }
        }//end of Math Class */

        
        public class Security
        {
            public static string CleanUserName(string logOnUserName)
            {
                string userName = String.Empty;
                Int32 backSlashLoc = 0;

                if(Strings.InStr(1, logOnUserName, " ", CompareMethod.Text) > 0)
                {
                    logOnUserName = Strings.Replace(logOnUserName, " ", "", 1, -1, CompareMethod.Text);
                }

                if(Strings.Len(logOnUserName) > 0)
                {
                    backSlashLoc = Strings.InStrRev(logOnUserName, "\\", Strings.Len(logOnUserName), CompareMethod.Text);
                    userName = (Strings.Right(logOnUserName, Strings.Len(logOnUserName) - backSlashLoc)); ;
                }
                return userName;
            }
        } //end of Security Class


        public class Application
        {
            public static bool PrevInstance()
            {
                if(Information.UBound(Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        } //end of Application class

        public class Computer
        {
            public static string GetIPAddress()
            {
                try
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                    return hostEntry.AddressList.GetValue(0).ToString();
                }
                catch(Exception ex)
                {
                    return String.Empty;
                }
            }
        } //end of Computer class

        public class Network
        {
            public static string GetIPAddress(string hostName)
            {
                string returnVal = String.Empty;
                try
                {
                    IPAddress[] addressList = Dns.GetHostEntry(hostName).AddressList;
                    returnVal = addressList[0].ToString();
                }
                catch (Exception ex)
                {
                    returnVal = String.Empty;
                }
                return returnVal;
            }
        } //end of Network class


        public class ComObjects
        {
            public static void ReleaseComObject(object value)
            {
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(value);
                }
                catch (Exception ex) { }
                finally
                {
                    value = null;
                }
                
            } 
        } //end of ComObjects class
        




    }
}
