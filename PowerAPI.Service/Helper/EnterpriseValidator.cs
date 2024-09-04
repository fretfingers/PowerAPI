using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Service.Helper
{
    internal class EnterpriseValidator
    {
        public static object Now { get; private set; }

        public static int GetDaysLeft(string regcode, string regname)
        {
            try
            {
                string MyCustomerName = "";
                string MyCustomerName1 = "";
                int MyLen = 0;
                int pcount = 0;
                int textStep = 1;
                string myxter = "";
                string Myxtercoded = "";
                string aMybox1 = "";
                string rMybox1 = "";
                string aMybox2 = "";
                string rMybox2 = "";
                string aMybox3 = "";
                string rMybox3 = "";
                string mmyname1 = "";
                string mmyname = "";
                string mm = "";
                int Aday = 0;
                int RDay = 0;
                int AMon = 0;
                int RMon = 0;
                int AYear = 0;
                int RYear = 0;
                string AppDate = "";
                string RegDate = "";
                DateTime AppDDate = DateTime.Now;
                DateTime RegDDate = DateTime.Now;
                int DueDateCount = 0;
                mmyname = regcode.Substring(15, 2);
                MyLen = mmyname.Length;
                if ((MyLen > 0))
                {
                    pcount = 0;
                    pcount = MyLen;
                    textStep = 1;
                    myxter = "";
                    Myxtercoded = "";
                    MyCustomerName1 = "";
                    while ((pcount > 0))
                    {
                        myxter = mmyname.ToUpper().Substring((textStep - 1), 1);
                        Myxtercoded = PowerEncode.FncEncodex(ref myxter);
                        MyCustomerName1 = (MyCustomerName1 + Myxtercoded);
                        textStep = (textStep + 1);
                        pcount = (pcount - 1);
                    }

                }
                else
                {
                    return 0;
                    // TODO: Exit Function: Warning!!! Need to return the value
                }

                // @@@@@@@@@YEAR
                rMybox1 = MyCustomerName1.Substring(0, 2);
                aMybox1 = DateTime.Now.Year.ToString().Substring(2, 2);
                // **********************************
                mmyname = regcode.Substring(24, 2);
                MyLen = mmyname.Length;
                MyLen = mmyname.Length;
                if ((MyLen > 0))
                {
                    pcount = 0;
                    pcount = MyLen;
                    textStep = 1;
                    myxter = "";
                    Myxtercoded = "";
                    MyCustomerName1 = "";
                    while ((pcount > 0))
                    {
                        myxter = mmyname.ToUpper().Substring((textStep - 1), 1);
                        Myxtercoded = PowerEncode.FncEncodex(ref myxter);
                        MyCustomerName1 = (MyCustomerName1 + Myxtercoded);
                        textStep = (textStep + 1);
                        pcount = (pcount - 1);
                    }

                }
                else
                {
                    return 0;
                    // TODO: Exit Function: Warning!!! Need to return the value
                }

                // @@@@@@@@@MONTH
                rMybox2 = MyCustomerName1.Substring(0, 2);
                mm = DateTime.Now.Month.ToString();
                if ((mm.Length == 1))
                {
                    mm = ("0" + mm);
                }
                else
                {
                    mm = mm;
                }

                aMybox2 = mm;
                // *****************************
                MyCustomerName1 = "";
                mmyname = regcode.Substring(33, 2);
                MyLen = mmyname.Length;
                if ((MyLen > 0))
                {
                    pcount = 0;
                    pcount = MyLen;
                    textStep = 1;
                    myxter = "";
                    Myxtercoded = "";
                    while ((pcount > 0))
                    {
                        myxter = mmyname.ToUpper().Substring((textStep - 1), 1);
                        Myxtercoded = PowerEncode.FncEncodex(ref myxter);
                        MyCustomerName1 = (MyCustomerName1 + Myxtercoded);
                        textStep = (textStep + 1);
                        pcount = (pcount - 1);
                    }

                }
                else
                {
                    return 0;
                    // TODO: Exit Function: Warning!!! Need to return the value
                }

                // @@@@@@@@@DAY
                rMybox3 = MyCustomerName1.Substring(0, 2);
                var isNumeric = int.TryParse(rMybox3, out int n);

                if (isNumeric)
                {

                }
                else
                {
                    return 0;
                    // TODO: Exit Function: Warning!!! Need to return the value
                }
                var isNumeric2 = int.TryParse(rMybox2, out int n2);
                if (isNumeric2)
                {

                }
                else
                {
                    return 0;
                    // TODO: Exit Function: Warning!!! Need to return the value
                }
                var IsNumeric3 = int.TryParse(rMybox1, out int n3);
                if (IsNumeric3)
                {

                }
                else
                {
                    return 0;
                    // TODO: Exit Function: Warning!!! Need to return the value
                }

                mm = DateTime.Now.Day.ToString();
                if ((mm.Length == 1))
                {
                    mm = ("0" + mm);
                }
                else
                {
                    mm = mm;
                }

                aMybox3 = mm;
                Aday = Convert.ToInt32(aMybox3);
                RDay = Convert.ToInt32(rMybox3);
                AMon = Convert.ToInt32(aMybox2);
                RMon = Convert.ToInt32(rMybox2);
                AYear = Convert.ToInt32(aMybox1);
                RYear = Convert.ToInt32(rMybox1);
                double myleap = 0;
                myleap = (AYear % 4);
                if (((AYear == RYear)
                            && (AMon == RMon)))
                {
                    return (RDay - Aday);
                }
                else if (((AYear == RYear)
                            && (((RMon - AMon)
                            == 1)
                            && ((AMon == 2)
                            && (myleap > 0)))))
                {
                    return (RDay + (29 - Aday));
                }
                else if (((AYear == RYear)
                            && (((RMon - AMon)
                            == 1)
                            && ((AMon == 2)
                            && (myleap <= 0)))))
                {
                    return (RDay + (28 - Aday));
                }
                else if (((AYear == RYear)
                            && (((RMon - AMon)
                            == 1)
                            && ((AMon == 9)
                            || ((AMon == 11)
                            || ((AMon == 4)
                            || (AMon == 6)))))))
                {
                    return (RDay + (30 - Aday));
                }
                else if (((AYear == RYear)
                            && (((RMon - AMon)
                            == 1)
                            && ((AMon == 1)
                            || ((AMon == 3)
                            || ((AMon == 5)
                            || ((AMon == 7)
                            || ((AMon == 8)
                            || ((AMon == 10)
                            || (AMon == 12))))))))))
                {
                    return (RDay + (31 - Aday));
                }
                else if ((((RYear - AYear)
                            == 1)
                            && (((RMon - AMon)
                            == 1)
                            && ((AMon == 2)
                            && (myleap > 0)))))
                {
                    return (RDay + (29 - Aday));
                }
                else if ((((RYear - AYear)
                            == 1)
                            && (((RMon - AMon)
                            == 1)
                            && ((AMon == 2)
                            && (myleap <= 0)))))
                {
                    return (RDay + (28 - Aday));
                }
                else if ((((RYear - AYear)
                            == 1)
                            && (((RMon - AMon)
                            == 1)
                            && ((AMon == 9)
                            || ((AMon == 11)
                            || ((AMon == 4)
                            || (AMon == 6)))))))
                {
                    return (RDay + (30 - Aday));
                }
                else if ((((RYear - AYear)
                            == 1)
                            && (((RMon - AMon)
                            == 1)
                            && ((AMon == 1)
                            || ((AMon == 3)
                            || ((AMon == 5)
                            || ((AMon == 7)
                            || ((AMon == 8)
                            || ((AMon == 10)
                            || (AMon == 12))))))))))
                {
                    return (RDay + (31 - Aday));
                }
                //else if ((RYear - AYear) < 0)
                //{
                //    return (RYear - AYear);
                //}
                else
                {
                    return 999;
                }

            }
            catch (Exception ex)
            {
                return 0;
            }

        }
    }
}
