using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
namespace SRL
{
    public partial class BPersianCalenderTextBox : TextBox
    {

        public BPersianCalenderTextBox()
        {
            InitializeComponent();
            var btn = new Button(); 
            btn.Size = new Size(Properties.Resources.Calendar_1128083418.Width, Properties.Resources.Calendar_1128083418.Height);
            btn.Location = new Point(-1, -3);
            btn.Cursor = Cursors.Hand;
            btn.Image = Properties.Resources.Calendar_1128083418;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = Color.White;
            btn.FlatAppearance.MouseOverBackColor = Color.White;
            btn.FlatAppearance.CheckedBackColor = Color.White;
            btn.Click += new EventHandler(btn_Click);
            this.Controls.Add(btn);
            this.ReadOnly = true;
            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            SendMessage(this.Handle, 0xd3, (IntPtr)2, (IntPtr)(btn.Width << 16));
            this.TextAlign = HorizontalAlignment.Left;
            ContextMenuStrip cm = new ContextMenuStrip();
            cm.Font = new System.Drawing.Font("tahoma", 9);
            cm.Items.Add("خالی کردن");
            cm.Items.Add("-");
            cm.Items.Add("امروز");
            cm.Items.Add("روز بعد");
            cm.Items.Add("روز قبل");
            cm.Items.Add("-");
            cm.Items.Add("اول ماه جاری");
            cm.Items.Add("آخر ماه جاری");
            //cm.Items.Add("-");
            //cm.Items.Add("تعویض تاریخ");
            this.ContextMenuStrip = cm;
            cm.Items[0].Click += new EventHandler(Null_Click);
            cm.Items[2].Click += new EventHandler(Today_Click);
            cm.Items[3].Click += new EventHandler(NextDay_Click);
            cm.Items[4].Click += new EventHandler(PrevDay_Click);
           // cm.Items[9].Click += new EventHandler(SwitchDate_Click);
            cm.Items[6].Click += new EventHandler(FirstDayOfMonth_Click);
            cm.Items[7].Click += new EventHandler(LastDayOfMonth_Click);
            this.NowDateSelected = false;
            this.Invoke();
        }

        public void LastDayOfMonth_Click(object sender, EventArgs e)
        {
            this.Miladi = pc.ToDateTime(pc.GetYear(DateTime.Now), pc.GetMonth(DateTime.Now), pc.GetDaysInMonth(pc.GetYear(DateTime.Now), pc.GetMonth(DateTime.Now)), 0, 0, 0, 0);
            this.Text = new ConvertDate().MiladiToShamsi(this.Miladi);
            this.Shamsi = new ConvertDate().MiladiToShamsi(this.Miladi);
            this.SelectedDate = this.Shamsi.Replace("/", "");

        }

        public void FirstDayOfMonth_Click(object sender, EventArgs e)
        {
            this.Miladi = pc.ToDateTime(pc.GetYear(DateTime.Now), pc.GetMonth(DateTime.Now), 1, 0, 0, 0, 0);
            this.Text = new ConvertDate().MiladiToShamsi(this.Miladi);
            this.Shamsi = new ConvertDate().MiladiToShamsi(this.Miladi);
            this.SelectedDate = this.Shamsi.Replace("/", "");

        }

        public void Invoke()
        {
            if (this.NowDateSelected)
            {
                this.Text = new ConvertDate().MiladiToShamsi(DateTime.Now);
                this.Miladi = DateTime.Now.Date;
                this.Shamsi = new ConvertDate().MiladiToShamsi(this.Miladi);
                this.SelectedDate = this.Shamsi.Replace("/", "");
            }
        }

        public void SwitchDate_Click(object sender, EventArgs e)
        {
            if (!SwitchDateFlag)
            {
                SwitchDateFlag = true;
                this.Text = this.Miladi.ToShortDateString();
            }
            else
            {
                SwitchDateFlag = false;
                this.Text = this.Shamsi;
            }
        }

        public bool SwitchDateFlag = false;

        public DateTime Miladi { get; set; }

        public string Shamsi { get; set; }

        public bool NowDateSelected { get; set; }

        public string SelectedDate { get; set; }

        public PersianCalendar pc = new PersianCalendar();

        public void PrevDay_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.Text))
            {
                this.Miladi = this.Miladi.AddDays(-1);
                this.Text = new ConvertDate().MiladiToShamsi(this.Miladi);
                this.Shamsi = new ConvertDate().MiladiToShamsi(this.Miladi);
                this.SelectedDate = this.Shamsi.Replace("/", "");
            }
        }

        public void NextDay_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.Text))
            {
                this.Miladi = this.Miladi.AddDays(1);
                this.Text = new ConvertDate().MiladiToShamsi(this.Miladi);
                this.Shamsi = new ConvertDate().MiladiToShamsi(this.Miladi);
                this.SelectedDate = this.Shamsi.Replace("/", "");
            }
        }

        public void Today_Click(object sender, EventArgs e)
        {
            this.Text = new ConvertDate().MiladiToShamsi(DateTime.Now);
            this.Miladi = DateTime.Now.Date;
            this.Shamsi = new ConvertDate().MiladiToShamsi(this.Miladi);
            this.SelectedDate = this.Shamsi.Replace("/", "");
        }

        public void Null_Click(object sender, EventArgs e)
        {
            this.Text = null;
            this.Miladi = new DateTime().Date;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadCalender();
            }
        }

        //protected override void OnTextChanged(EventArgs e)
        //{
        //    //try
        //    //{
        //    //    System.Text.StringBuilder dateString = new System.Text.StringBuilder();

        //    //    if (!this.Text.Contains("/"))
        //    //    {
        //    //        dateString.Append(this.Text.Substring(0, 4));
        //    //        dateString.Append("/");
        //    //        dateString.Append(this.Text.Substring(4, 2));
        //    //        dateString.Append("/");
        //    //        dateString.Append(this.Text.Substring(6, 2));
        //    //        this.Text = dateString.ToString();
        //    //    }
        //    //    this.Miladi = new DateValidation().GetDate(this.Text);
        //    //    this.Shamsi = this.Text;
        //    //    this.SelectedDate = this.Shamsi.Replace("/", "");
        //    //}
        //    //catch (Exception)
        //    //{
        //    //    return;
        //    //}
        //}

        private void btn_Click(object sender, EventArgs e)
        {
            LoadCalender();
        }

        private void LoadCalender()
        {

           shamsiCalander sc = new shamsiCalander();
            sc = new PCalander().getshamsiCalander();
            if (sc.ShamsiDate == null)
            {
                this.Miladi = DateTime.Now.Date;
                this.Shamsi = new ConvertDate().MiladiToShamsi(this.Miladi);
                this.SelectedDate = this.Shamsi.Replace("/", "");
                this.Text = new ConvertDate().MiladiToShamsi(DateTime.Now);

            }
            else
            {
                this.Text = sc.ShamsiDate;
                this.Miladi = sc.MiladiDate;
                this.Shamsi = sc.ShamsiDate;
                if (!string.IsNullOrEmpty(this.Shamsi))
                {
                    this.SelectedDate = this.Shamsi.Replace("/", "");
                }

            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

    }
}
