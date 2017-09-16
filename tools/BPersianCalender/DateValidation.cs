using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
namespace SRL
{
    public class DateValidation
    {
        public bool validate(string dateStr, bool dateCheck)
        {
            if (dateStr == "    /  /")
            {
                return true;
            }
            PersianCalendar pr = new PersianCalendar();
            int NowYear = pr.GetYear(DateTime.Now);
            int NowMonth = pr.GetMonth(DateTime.Now);
            int NowDay = pr.GetDayOfMonth(DateTime.Now);
            bool flag = false;
            if (dateStr.Length > 0)
            {
                int txtLenth = dateStr.Length;
                string[] str = dateStr.Split('/');
                int strLenght = str.Length;
                if (strLenght < 3 || strLenght < 2 || strLenght < 1 || strLenght < 0)
                {
                    return true;
                }
                string year = date(str[0]);
                string month = date(str[1]);
                string day = date(str[2]);
                if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(month) || string.IsNullOrEmpty(day))
                {
                    return true;
                }

                if (dateCheck)
                {
                    if (int.Parse(month) == 12 && int.Parse(day) > 29)
                        flag = true;
                    if (int.Parse(year) < 1370 || int.Parse(year) > 1399 || year.Length <= 1 || int.Parse(year) > NowYear)
                        flag = true;
                    if (int.Parse(month) < 1 || int.Parse(month) > 12 || month.Length <= 1)
                        flag = true;
                    if (int.Parse(day) < 1 || int.Parse(day) > 31 || day.Length <= 1)
                        flag = true;
                }
                else
                {
                    if (int.Parse(month) == 12 && int.Parse(day) > 29)
                        flag = true;
                    if (int.Parse(year) < 1370 || int.Parse(year) > 1399 || year.Length <= 1)
                        flag = true;
                    if (int.Parse(month) < 1 || int.Parse(month) > 12 || month.Length <= 1)
                        flag = true;
                    if (int.Parse(day) < 1 || int.Parse(day) > 31 || day.Length <= 1)
                        flag = true;
                }

            }
            if (flag)
            {
                return true;
            }
            else
                return false;
        }
        public string date(string sr)
        {
            char[] srr;
            srr = sr.ToArray();
            string c = string.Empty;
            for (int i = 0; i < srr.Length; i++)
            {
                string srss = srr[i].ToString();
                switch (srss)
                {
                    case "۰": c += "0";
                        break;
                    case "۱": c += "1";
                        break;
                    case "۲": c += "2";
                        break;
                    case "۳": c += "3";
                        break;
                    case "۴": c += "4";
                        break;
                    case "۵": c += "5";
                        break;
                    case "۶": c += "6";
                        break;
                    case "۷": c += "7";
                        break;
                    case "۸": c += "8";
                        break;
                    case "۹": c += "9";
                        break;
                    case "0": c += "0";
                        break;
                    case "1": c += "1";
                        break;
                    case "2": c += "2";
                        break;
                    case "3": c += "3";
                        break;
                    case "4": c += "4";
                        break;
                    case "5": c += "5";
                        break;
                    case "6": c += "6";
                        break;
                    case "7": c += "7";
                        break;
                    case "8": c += "8";
                        break;
                    case "9": c += "9";
                        break;
                    default:
                        c = sr;
                        break;
                }

            }
            return c;
        }
        public string GetDateStr(string str)
        {
            string st = str;
            string[] words = st.Split('/');
            string Result = string.Empty;
            string Year;
            string Month;
            string Day;
            Year = datestr(words[0]);
            Month = datestr(words[1]);
            Day = datestr(words[2]);
            Result = Year + "/" + Month + "/" + Day;
            return Result;
        }
        public string datestr(string sr)
        {
            char[] srr;
            srr = sr.ToArray();
            string c = string.Empty;
            for (int i = 0; i < srr.Length; i++)
            {
                string srss = srr[i].ToString();
                switch (srss)
                {
                    case "0": c += "۰";
                        break;
                    case "1": c += "۱";
                        break;
                    case "2": c += "۲";
                        break;
                    case "3": c += "۳";
                        break;
                    case "4": c += "۴";
                        break;
                    case "5": c += "۵";
                        break;
                    case "6": c += "۶";
                        break;
                    case "7": c += "۷";
                        break;
                    case "8": c += "۸";
                        break;
                    case "9": c += "۹";
                        break;
                    case "۰": c += "۰";
                        break;
                    case "۱": c += "۱";
                        break;
                    case "۲": c += "۲";
                        break;
                    case "۳": c += "۳";
                        break;
                    case "۴": c += "۴";
                        break;
                    case "۵": c += "۵";
                        break;
                    case "۶": c += "۶";
                        break;
                    case "۷": c += "۷";
                        break;
                    case "۸": c += "۸";
                        break;
                    case "۹": c += "۹";
                        break;
                    default:
                        c = sr;
                        break;
                }

            }
            return c;
        }
        DateTime dt;
        public DateTime GetDate(string str)
        {
            string st = str;
            string[] words = st.Split('/');
            string Result = string.Empty;
            string Year;
            string Month;
            string Day;
            Year = dateR(words[0]);
            Month = dateR(words[1]);
            Day = dateR(words[2]);
            if (int.Parse(Month) > 12 || int.Parse(Day) > 31)
            {
                MessageBox.Show("قالب تاریخ نادرست است", "خطا");
                return dt;
            }
            else
            {

                PersianCalendar pc = new PersianCalendar();
                dt = new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day), pc);
                return dt;
            }
        }
        public string dateR(string sr)
        {
            char[] srr;
            srr = sr.ToArray();
            string c = string.Empty;
            for (int i = 0; i < srr.Length; i++)
            {
                string srss = srr[i].ToString();
                switch (srss)
                {
                    case "۰": c += "0";
                        break;
                    case "۱": c += "1";
                        break;
                    case "۲": c += "2";
                        break;
                    case "۳": c += "3";
                        break;
                    case "۴": c += "4";
                        break;
                    case "۵": c += "5";
                        break;
                    case "۶": c += "6";
                        break;
                    case "۷": c += "7";
                        break;
                    case "۸": c += "8";
                        break;
                    case "۹": c += "9";
                        break;
                    case "0": c += "0";
                        break;
                    case "1": c += "1";
                        break;
                    case "2": c += "2";
                        break;
                    case "3": c += "3";
                        break;
                    case "4": c += "4";
                        break;
                    case "5": c += "5";
                        break;
                    case "6": c += "6";
                        break;
                    case "7": c += "7";
                        break;
                    case "8": c += "8";
                        break;
                    case "9": c += "9";
                        break;
                    default:
                        c = sr;
                        break;
                }

            }
            return c;
        }

    }
}