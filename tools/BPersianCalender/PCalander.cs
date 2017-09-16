using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SRL
{
    public partial class PCalander : Form
    {

        PersianCalendar pc = new PersianCalendar();
        private DateTime NowDate = DateTime.Now;
        static int mounthName;
        static int YearName;
        int selectedYear, selectdMonth, selectedDay; string sMonth;
        int rowIndex, columnIndex;
        string selectedShamsiDate;
        DateTime selectedMiladiDate;
        shamsiCalander objshamsiCalander = new shamsiCalander();
        static string key;

        public PCalander()
        {
            InitializeComponent();
            mounthName = pc.GetMonth(NowDate);
            YearName = pc.GetYear(NowDate);
           
        }

        private void frmCalander_Load(object sender, EventArgs e)
        {
            //Create Rows
            for (int i = 0; i < 4; i++)
            {
                dgCalander.Rows.Add();
                dgMonth.Rows.Add();
                dgYear.Rows.Add();
            }

            this.Text += " " + MiladiToShamsi(DateTime.Now);
            
            FillCalander();
            FindCurrentDate();
            getDates();


        }

       
        private void FillYear()
        {
            int currentYear = pc.GetYear(DateTime.Now);
            int PrevYears = currentYear - 6;
            for (int i = 0, m = PrevYears; i < 4; i++)
            {
                for (int j = 0; j < 3; j++, m++)
                {
                    dgYear[j, i].Value = m;
                    if (dgYear[j, i].Value.ToString() == YearName.ToString())
                    {
                        dgYear.ClearSelection();
                        dgYear[j, i].Selected = true;
                    }
                }
            }
        }

        private void FillMonth()
        {
            for (int i = 0, m = 1; i < 4; i++)
            {
                for (int j = 0; j < 3; j++, m++)
                {
                    dgMonth[j, i].Value = getMounth(m);
                    if (dgMonth[j, i].Value.ToString() == lblMounth.Text.ToString())
                    {
                        dgMonth.ClearSelection();
                        dgMonth[j, i].Selected = true;

                    }
                }
            }
        }

        public string MiladiToShamsi(DateTime Mdate)
        {
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

        private void FillCalander()
        {
            columnIndex = dgCalander.CurrentCell.ColumnIndex;
            rowIndex = dgCalander.CurrentCell.RowIndex;
            int DayOfWeekNameInMonth = 0;
            int daysInMonth = pc.GetDaysInMonth(YearName, mounthName);

            for (int i = 1, d = 1, j = 0; i <= 5; i++, j++)
            {
                if (d == daysInMonth)
                {
                    break;
                }
                for (int x = 1; x <= 7; x++, d++)
                {
                    DateTime dtt = pc.ToDateTime(YearName, mounthName, d, 1, 1, 1, 1, 1);

                    #region MyRegion
                    switch (pc.GetDayOfWeek(dtt))
                    {
                        case DayOfWeek.Saturday: DayOfWeekNameInMonth = 0;
                            break;
                        case DayOfWeek.Sunday: DayOfWeekNameInMonth = 1;
                            break;
                        case DayOfWeek.Monday: DayOfWeekNameInMonth = 2;
                            break;
                        case DayOfWeek.Tuesday: DayOfWeekNameInMonth = 3;
                            break;
                        case DayOfWeek.Wednesday: DayOfWeekNameInMonth = 4;
                            break;
                        case DayOfWeek.Thursday: DayOfWeekNameInMonth = 5;
                            break;
                        case DayOfWeek.Friday: DayOfWeekNameInMonth = 6;
                            break;
                    }
                    #endregion

                    dgCalander.Rows[j].Cells["c" + DayOfWeekNameInMonth.ToString()].Value = pc.GetDayOfMonth(dtt);
                    if (DayOfWeekNameInMonth == 6)
                    {
                        dgCalander.Rows[j].Cells["c" + DayOfWeekNameInMonth.ToString()].Style.ForeColor = Color.Red;
                    }
                    if (DayOfWeekNameInMonth == 6)
                    {
                        d++;
                        break;

                    }
                    if (d == daysInMonth)
                    {
                        break;
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////
            int lastRowData = 0;
            if (dgCalander.Rows[4].Cells["c6"].Value != null)
            {
                if (!string.IsNullOrEmpty(dgCalander.Rows[4].Cells["c6"].Value.ToString()))
                {
                    lastRowData = int.Parse(dgCalander.Rows[4].Cells["c6"].Value.ToString());
                }

            }
            if (lastRowData != 31 && lastRowData != 0)
            {
                if (daysInMonth >= 29 && lastRowData >= 29)
                {
                    int RemainDay = daysInMonth - lastRowData;
                    for (int i = 0; i < RemainDay; i++)
                    {
                        dgCalander.Rows[0].Cells["c" + i.ToString()].Value = ++lastRowData;
                    }
                }

            }
            FindCurrentCell();
        }

        private void btnNextM_Click(object sender, EventArgs e)
        {
            if (mounthName >= 12)
            {
                mounthName = 0;
                NextYear();
            }
            mounthName++;
            lblMounth.Text = getMounth(mounthName);
            ResetCalender();
            FillCalander();
            FindCurrentCell();
        }

        private void btnPrevM_Click(object sender, EventArgs e)
        {
            if (mounthName <= 1)
            {
                mounthName = 13;
                PrevYear();
            }
            mounthName--;
            lblMounth.Text = getMounth(mounthName);
            ResetCalender();
            FillCalander();
            FindCurrentCell();
        }

        private void btnNextY_Click(object sender, EventArgs e)
        {
            NextYear();
            ResetCalender();
            FillCalander();
            FindCurrentCell();
        }

        private void ResetCalender()
        {
            for (int i = 0; i <= 4; i++)
            {
                for (int x = 0; x < 7; x++)
                {
                    dgCalander.Rows[i].Cells["c" + x.ToString()].Value = "";
                }
            }
        }

        private void NextYear()
        {
            YearName++;
            lblYear.Text = YearName.ToString();
        }

        private void btnPrevY_Click(object sender, EventArgs e)
        {
            PrevYear();
            ResetCalender();
            FillCalander();
            FindCurrentCell();
        }

        private void PrevYear()
        {
            YearName--;
            lblYear.Text = YearName.ToString();
        }

        private void dgCalander_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //fillItems(e);
            FindCurrentCell();
        }

        //private void fillItems(DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex >= 0)
        //    {
        //        if (dgCalander.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
        //        {
        //            if (!string.IsNullOrEmpty(dgCalander.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
        //            {
        //                this.Text = null;
        //                selectedDay = int.Parse(dgCalander.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
        //                selectdMonth = mounthName;
        //                selectedYear = YearName;
        //                selectedShamsiDate = selectedYear.ToString() + "/" +
        //                    (selectdMonth.ToString().Length == 1 ? "0" + selectdMonth.ToString() : selectdMonth.ToString()) + "/" +
        //                    (selectedDay.ToString().Length == 1 ? "0" + selectedDay.ToString() : selectedDay.ToString());
        //                this.Text = "تقویم شمسی | تاریخ انتخاب شده :   " + selectedShamsiDate;
        //                selectedMiladiDate = pc.ToDateTime(selectedYear, selectdMonth, selectedDay, 0, 0, 0, 0);
        //                lblShamsi.Text = selectedShamsiDate;
        //                lblMiladi.Text = selectedMiladiDate.ToShortDateString();
        //            }
        //            else
        //            {

        //            }

        //        }
        //    }
        //}

        private void btnToday_Click(object sender, EventArgs e)
        {
            FindCurrentDate();
            getDates();
            this.Text = "تقویم شمسی | تاریخ انتخاب شده :   " + MiladiToShamsi(DateTime.Now);
        }

        private void getDates()
        {
            lblShamsi.Text = MiladiToShamsi(DateTime.Now);
            lblMiladi.Text = DateTime.Now.ToShortDateString();
        }

        private void FindCurrentDate()
        {
            int tYear, tMonth, tDay = 0;
            tYear = pc.GetYear(DateTime.Now);
            tMonth = pc.GetMonth(DateTime.Now);
            tDay = pc.GetDayOfMonth(DateTime.Now);
            mounthName = tMonth;
            YearName = tYear;
            lblYear.Text = tYear.ToString();
            lblMounth.Text = getMounth(tMonth);
            ResetCalender();
            FillCalander();
            for (int i = 0; i <= 4; i++)
            {
                for (int x = 0; x < 7; x++)
                {
                    if (dgCalander.Rows[i].Cells["c" + x.ToString()].Value.ToString() == tDay.ToString())
                    {
                        dgCalander.ClearSelection();
                        dgCalander.Rows[i].Cells["c" + x.ToString()].Selected = true;
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            objshamsiCalander.MiladiDate = selectedMiladiDate;
            objshamsiCalander.ShamsiDate = selectedShamsiDate;
            this.Close();
        }

        public shamsiCalander getshamsiCalander()
        {
            this.ShowDialog();
            return objshamsiCalander;
        }

        #region GetMounth
        private string getMounth(int Mounth)
        {
            string MounthName = string.Empty;
            switch (Mounth)
            {
                case 1: MounthName = "فروردین";
                    break;
                case 2: MounthName = "اردیبهشت";
                    break;
                case 3: MounthName = "خرداد";
                    break;
                case 4: MounthName = "تیر";
                    break;
                case 5: MounthName = "مرداد";
                    break;
                case 6: MounthName = "شهریور";
                    break;
                case 7: MounthName = "مهر";
                    break;
                case 8: MounthName = "آبان";
                    break;
                case 9: MounthName = "آذر";
                    break;
                case 10: MounthName = "دی";
                    break;
                case 11: MounthName = "بهمن";
                    break;
                case 12: MounthName = "اسفند";
                    break;
            }
            return MounthName;
        }

        #endregion

        private void dgCalander_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            objshamsiCalander.MiladiDate = selectedMiladiDate;
            objshamsiCalander.ShamsiDate = selectedShamsiDate;
            this.Close();
        }

        private void dgCalander_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(selectedShamsiDate))
                {
                    objshamsiCalander.MiladiDate = selectedMiladiDate;
                    objshamsiCalander.ShamsiDate = selectedShamsiDate;
                }
                else
                {
                    objshamsiCalander.MiladiDate = DateTime.Now;
                    objshamsiCalander.ShamsiDate = MiladiToShamsi(DateTime.Now);
                }
                this.Close();

            }
        }

        private void dgCalander_SelectionChanged(object sender, EventArgs e)
        {
            //FindCurrentCell();

        }

        private void FindCurrentCell()
        {
            if (dgCalander.CurrentCell.Value != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(dgCalander.CurrentCell.Value.ToString()))
                    {
                        this.Text = null;
                        selectedDay = int.Parse(dgCalander.CurrentCell.Value.ToString());
                        selectdMonth = mounthName;
                        selectedYear = YearName;
                        selectedShamsiDate = selectedYear.ToString() + "/" +
                            (selectdMonth.ToString().Length == 1 ? "0" + selectdMonth.ToString() : selectdMonth.ToString()) + "/" +
                            (selectedDay.ToString().Length == 1 ? "0" + selectedDay.ToString() : selectedDay.ToString());
                        this.Text = "تقویم شمسی | تاریخ انتخاب شده :   " + selectedShamsiDate;
                        selectedMiladiDate = pc.ToDateTime(selectedYear, selectdMonth, selectedDay, 0, 0, 0, 0);
                        lblShamsi.Text = selectedShamsiDate;
                        lblMiladi.Text = selectedMiladiDate.ToShortDateString();

                    }
                    else
                    {
                        selectedDay = 0;
                        selectdMonth = 0;
                        selectedYear = 0;
                        lblShamsi.Text = null;
                        lblMiladi.Text = null;
                        selectedShamsiDate = null;
                        selectedMiladiDate = new DateTime();
                        this.Text = "تقویم شمسی | تاریخ انتخاب شده :   " + selectedShamsiDate;
                    }
                }
                catch (Exception)
                {
                    return;
                }
                
            }

        }

        private void dgCalander_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            FindCurrentCell();
        }

        private void dgCalander_KeyPress(object sender, KeyPressEventArgs e)
        {
            key += e.KeyChar.ToString();
            if (key.Length > 2)
            {
                key = null;
            }
            for (int i = 0; i <= 4; i++)
            {
                for (int x = 0; x < 7; x++)
                {
                    if (dgCalander.Rows[i].Cells["c" + x.ToString()].Value.ToString() == key)
                    {
                        dgCalander.ClearSelection();
                        dgCalander.Rows[i].Cells["c" + x.ToString()].Selected = true;
                    }
                }
            }
        }

        private void lblMounth_Click(object sender, EventArgs e)
        {
            dgMonth.Dock = DockStyle.Fill;
            dgMonth.Visible = true;
            FillMonth();
            dgMonth.Focus(); dgMonth.Select();
        }

        private void dgMonth_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string selectedCell = e.ColumnIndex.ToString() + e.RowIndex.ToString();
            switch (selectedCell)
            {
                case "00": mounthName = 1; lblMounth.Text = getMounth(1);
                    break;
                case "10": mounthName = 2; lblMounth.Text = getMounth(2);
                    break;
                case "20": mounthName = 3; lblMounth.Text = getMounth(3);
                    break;
                case "01": mounthName = 4; lblMounth.Text = getMounth(4);
                    break;
                case "11": mounthName = 5; lblMounth.Text = getMounth(5);
                    break;
                case "21": mounthName = 6; lblMounth.Text = getMounth(6);
                    break;
                case "02": mounthName = 7; lblMounth.Text = getMounth(7);
                    break;
                case "12": mounthName = 8; lblMounth.Text = getMounth(8);
                    break;
                case "22": mounthName = 9; lblMounth.Text = getMounth(9);
                    break;
                case "03": mounthName = 10; lblMounth.Text = getMounth(10);
                    break;
                case "13": mounthName = 11; lblMounth.Text = getMounth(11);
                    break;
                case "23": mounthName = 12; lblMounth.Text = getMounth(12);
                    break;
            }
            dgMonth.Visible = false;
            ResetCalender();
            FillCalander();
        }

        private void lblYear_Click(object sender, EventArgs e)
        {

            dgYear.Dock = DockStyle.Fill;
            dgYear.Visible = true;
            FillYear();
            dgYear.Focus();
            dgYear.Select();
            btnPrevYears.Visible = true;
            btnNextYears.Visible = true;
            btnNextYears.BringToFront();
            btnPrevYears.BringToFront();
        }

        private void dgYear_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            YearName = int.Parse(dgYear[e.ColumnIndex, e.RowIndex].Value.ToString());
            lblYear.Text = YearName.ToString();
            ResetCalender();
            FillCalander();

            btnPrevYears.Visible = false;
            btnNextYears.Visible = false;

            dgYear.Visible = false;
        }

        private void btnNextYears_Click(object sender, EventArgs e)
        {
            int currentYear = int.Parse(dgYear[2, 3].Value.ToString());
            int PrevYears = currentYear + 1;
            for (int i = 0, m = PrevYears; i < 4; i++)
            {
                for (int j = 0; j < 3; j++, m++)
                {
                    dgYear[j, i].Value = m;
                }
            }
        }

        private void btnPrevYears_Click(object sender, EventArgs e)
        {
            int currentYear = int.Parse(dgYear[0, 0].Value.ToString());
            int PrevYears = currentYear - 12;
            for (int i = 0, m = PrevYears; i < 4; i++)
            {
                for (int j = 0; j < 3; j++, m++)
                {
                    dgYear[j, i].Value = m;
                }
            }
        }

    }

    public class shamsiCalander
    {
        public string ShamsiDate { get; set; }
        public DateTime MiladiDate { get; set; }
    }
}
