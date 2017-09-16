using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SRL
{
   public class ConvertDate
    {
       public int Year
       {
           get;
           set;
       }
       public int Month
       {
           get;
           set;
       }
       public int Day
       {
           get;
           set;
       }
       public ConvertDate()
       { 
       }
       public ConvertDate(DateTime Date)
       { 
           System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
           this.Year = pc.GetYear(Date);
           this.Month = pc.GetMonth(Date);
           this.Day = pc.GetDayOfMonth(Date);
       }
       public string MiladiToShamsi(DateTime Mdate)
       {
           System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
           string Result = pc.GetYear(Mdate).ToString();
           string Month = Convert.ToString(pc.GetMonth(Mdate));
           string day = Convert.ToString(pc.GetDayOfMonth(Mdate));
           if (Month.Length == 1)
               Month = "0" + Month;
           if (day.Length == 1)
               day = "0" + day;
           Result += "/" + Month + "/" + day;
           return Result;
       }
       public string func_PersianDateFormat(DateTime date)
       {
           System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
           StringBuilder sb = new StringBuilder();
           sb.Append(pc.GetYear(date).ToString("0000"));
           sb.Append("/");
           sb.Append(pc.GetMonth(date).ToString("00"));
           sb.Append("/");
           sb.Append(pc.GetDayOfMonth(date).ToString("00"));
           return sb.ToString();
       }
       public void SetDate(DateTime Date)
       {
           System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
           this.Year = pc.GetYear(Date);
           this.Month = pc.GetMonth(Date);
           this.Day = pc.GetDayOfMonth(Date);
       }
       public int getYear()
       {
           return this.Year;
       }
       public int getMounth()
       {
           return this.Month;
       }
       public int getDay()
       {
           return this.Day;
       }

    }
    
}
