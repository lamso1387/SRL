using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Data.Entity;
using Microsoft.Reporting.WinForms;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel;
using IWshRuntimeLibrary;

using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml.Serialization;
using System.Security;
using System.Management;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
using System.Drawing.Printing;
using System.Collections;
using System.Drawing.Text;
using System.Linq.Expressions;

namespace SRL
{
    public class Print
    {
        public void PrintPaperSize(PrintDialog print_dialog, string paper_name = "Custom", int height = 584, int width = 827)
        {
            PaperSize psz = new PaperSize();
            psz.PaperName = paper_name;
            psz.Height = height;
            psz.Width = width;

            print_dialog.Document.DefaultPageSettings.PaperSize = psz;

        }

        public class PrintFromDatagridView
        {
            public PrintDialog printDialog = new PrintDialog();
            public DataGridView dataGridView1;
            public PrintDocument printDocument1 = new PrintDocument();
            public PrintPreviewDialog printPriviewDialog = new PrintPreviewDialog();

            #region Member Variables
            const string strConnectionString = "data source=localhost;Integrated Security=SSPI;Initial Catalog=Northwind;";
            StringFormat strFormat; //Used to format the grid rows.
            ArrayList arrColumnLefts = new ArrayList();//Used to save left coordinates of columns
            ArrayList arrColumnWidths = new ArrayList();//Used to save column widths
            int iCellHeight = 0; //Used to get/set the datagridview cell height
            int iTotalWidth = 0; //
            int iRow = 0;//Used as counter
            bool bFirstPage = false; //Used to check whether we are printing first page
            bool bNewPage = false;// Used to check whether we are printing a new page
            int iHeaderHeight = 0; //Used for the header height
            #endregion

            public PrintFromDatagridView(DataGridView dataGridView1_, string printer_name = null)
            {
                dataGridView1 = dataGridView1_;
                printDialog.Document = printDocument1;
                if (printer_name != null) printDialog.PrinterSettings.PrinterName = printer_name;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;


                printDocument1.BeginPrint += new System.Drawing.Printing.PrintEventHandler(printDocument1_BeginPrint);

                printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument1_PrintPage);

                printPriviewDialog.Document = printDocument1;
                printPriviewDialog.PrintPreviewControl.Zoom = 1;

            }


            #region Print Button Click Event

            public void PrintDialogAndPrint(bool use_EX_dialog)
            {

                printDialog.Document = printDocument1;
                printDialog.UseEXDialog = use_EX_dialog;



                if (DialogResult.OK == printDialog.ShowDialog())
                {
                    printDocument1.DocumentName = "Test Page Print";
                    printDocument1.Print();
                }

            }

            public DialogResult PrintDialogShow()
            {
                return printDialog.ShowDialog();

            }

            public void PrintPreview()
            {
                printPriviewDialog.ShowDialog();  // print button do this: pd.Print() but in pdialog.ShowDialog() you should call  pd.Print()

            }

            public void PrintDialogAndPriview()
            {
                PrintDialogShow();
                PrintPreview();

            }

            #endregion

            #region Begin Print Event Handler
            /// <summary>
            /// Handles the begin print event of print document
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
            {
                try
                {
                    strFormat = new StringFormat();
                    strFormat.Alignment = StringAlignment.Center;
                    strFormat.LineAlignment = StringAlignment.Center;
                    strFormat.Trimming = StringTrimming.EllipsisCharacter;

                    arrColumnLefts.Clear();
                    arrColumnWidths.Clear();
                    iCellHeight = 0;
                    iRow = 0;
                    bFirstPage = true;
                    bNewPage = true;

                    // Calculating Total Widths
                    iTotalWidth = 0;
                    foreach (DataGridViewColumn dgvGridCol in dataGridView1.Columns)
                    {
                        iTotalWidth += dgvGridCol.Width;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            #endregion

            #region Print Page Event
            /// <summary>
            /// Handles the print page event of print document
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
            {
                try
                {
                    //Set the left margin
                    int iLeftMargin = e.MarginBounds.Left;
                    //Set the top margin
                    int iTopMargin = e.MarginBounds.Top;
                    //Whether more pages have to print or not
                    bool bMorePagesToPrint = false;
                    int iTmpWidth = 0;

                    //For the first page to print set the cell width and header height
                    if (bFirstPage)
                    {
                        foreach (DataGridViewColumn GridCol in dataGridView1.Columns)
                        {
                            iTmpWidth = (int)(Math.Floor((double)((double)GridCol.Width /
                                           (double)iTotalWidth * (double)iTotalWidth *
                                           ((double)e.MarginBounds.Width / (double)iTotalWidth))));

                            iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
                                        GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;

                            // Save width and height of headres
                            arrColumnLefts.Add(iLeftMargin);
                            arrColumnWidths.Add(iTmpWidth);
                            iLeftMargin += iTmpWidth;
                        }
                    }
                    //Loop till all the grid rows not get printed
                    while (iRow <= dataGridView1.Rows.Count - 1)
                    {
                        DataGridViewRow GridRow = dataGridView1.Rows[iRow];
                        //Set the cell height
                        iCellHeight = GridRow.Height + 5;
                        int iCount = 0;
                        //Check whether the current page settings allo more rows to print
                        if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                        {
                            bNewPage = true;
                            bFirstPage = false;
                            bMorePagesToPrint = true;
                            break;
                        }
                        else
                        {
                            if (bNewPage)
                            {
                                //Draw Header
                                e.Graphics.DrawString("Customer Summary 00", new Font(dataGridView1.Font, FontStyle.Bold),
                                        Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
                                        e.Graphics.MeasureString("Customer Summary 11", new Font(dataGridView1.Font,
                                        FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                                String strDate = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();
                                //Draw Date
                                e.Graphics.DrawString(strDate, new Font(dataGridView1.Font, FontStyle.Bold),
                                        Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width -
                                        e.Graphics.MeasureString(strDate, new Font(dataGridView1.Font,
                                        FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top -
                                        e.Graphics.MeasureString("Customer Summary 22", new Font(new Font(dataGridView1.Font,
                                        FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                                //Draw Columns                 
                                iTopMargin = e.MarginBounds.Top;
                                foreach (DataGridViewColumn GridCol in dataGridView1.Columns)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                        new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight));

                                    e.Graphics.DrawRectangle(Pens.Black,
                                        new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight));

                                    e.Graphics.DrawString(GridCol.HeaderText, GridCol.InheritedStyle.Font,
                                        new SolidBrush(GridCol.InheritedStyle.ForeColor),
                                        new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                        (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                                    iCount++;
                                }
                                bNewPage = false;
                                iTopMargin += iHeaderHeight;
                            }
                            iCount = 0;
                            //Draw Columns Contents                
                            foreach (DataGridViewCell Cel in GridRow.Cells)
                            {
                                if (Cel.Value != null)
                                {
                                    e.Graphics.DrawString(Cel.Value.ToString(), Cel.InheritedStyle.Font,
                                                new SolidBrush(Cel.InheritedStyle.ForeColor),
                                                new RectangleF((int)arrColumnLefts[iCount], (float)iTopMargin,
                                                (int)arrColumnWidths[iCount], (float)iCellHeight), strFormat);
                                }
                                //Drawing Cells Borders 
                                e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount],
                                        iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));

                                iCount++;
                            }
                        }
                        iRow++;
                        iTopMargin += iCellHeight;
                    }

                    //If more lines exist, print another page.
                    if (bMorePagesToPrint)
                        e.HasMorePages = true;
                    else
                        e.HasMorePages = false;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            #endregion


        }

        public class PrintFromControlScreen
        {
            public Control control_to_print;

            public PrintDocument pd = new PrintDocument();
            public PrintDialog pdialog = new PrintDialog();
            public PrintPreviewDialog ppd = new PrintPreviewDialog();


            public PrintFromControlScreen(Control control_to_print_from)
            {
                control_to_print = control_to_print_from;
                pd.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(pd_PrintPage);
                ppd.Document = pd;
                pdialog.Document = pd;
                ppd.PrintPreviewControl.Zoom = 1;

            }




            public void PrintPreview()
            {
                ppd.ShowDialog();  // print button do this: pd.Print() but in pdialog.ShowDialog() you should call  pd.Print()

            }

            public DialogResult PrintDialogShow()
            {
                return pdialog.ShowDialog();

            }

            public void PrintDialogAndPrint()
            {

                if (PrintDialogShow() == DialogResult.OK) pd.Print();

            }
            public void PrintDialogAndPriview()
            {
                PrintDialogShow();
                PrintPreview();

            }


            private void pd_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
            {
                Bitmap bm = new Bitmap(control_to_print.Width, control_to_print.Height);
                control_to_print.DrawToBitmap(bm, new Rectangle(0, 0, control_to_print.Width, control_to_print.Height));
                e.Graphics.DrawImage(bm, 0, 0);

            }



        }
    }

    public class DateTimeLanguageClass
    {
        public enum TimeFormat
        {
            Full,
            Long,
            Short,
            Custom

        }
        System.Windows.Forms.Timer timer = null;
        Control control_to_show_time;
        public void StartTimer(Control control, TimeFormat time_format, string custom_time_format = null)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler((sender, e) => timer_Tick(sender, e, time_format, custom_time_format));
            timer.Enabled = true;
            control_to_show_time = control;
        }

        void timer_Tick(object sender, EventArgs e, TimeFormat show_type, string custom_time_format)
        {
            switch (show_type)
            {
                case TimeFormat.Full:
                    control_to_show_time.Text = DateTime.Now.ToString();
                    break;
                case TimeFormat.Long:
                    control_to_show_time.Text = DateTime.Now.ToLongTimeString();
                    break;
                case TimeFormat.Short:
                    control_to_show_time.Text = DateTime.Now.ToShortTimeString();
                    break;
                case TimeFormat.Custom:
                    control_to_show_time.Text = custom_time_format != null ?
                        DateTime.Now.ToString(custom_time_format) :
                         DateTime.Now.ToLocalTime().ToString();
                    break;
                default:
                    break;
            }



        }

        /// <summary>
        /// full example for format:  .ToString("yyyy/MM/dd HH:mm:ss tt")
        /// hh is for 12 hour and HH is for 24 hour
        /// MM is for month and mm is for minute
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public string DateTimeToSring(DateTime? dt, string format)
        {
            return dt.Value.ToString(format);
        }
        public string GetCurrentKeyboardShort()
        {

            return InputLanguage.CurrentInputLanguage.Culture.Name;
        }
        public string ChangeKeyboardAltShift()
        {
            SendKeys.Send("%+");
            return GetCurrentKeyboardShort();
        }
        public DateTime? TryGetDateTimeValue(string datetime_string)
        {
            DateTime dt = new DateTime();
            if (!DateTime.TryParse(datetime_string, out dt)) return null;
            return dt;
        }

    }
    public class SettingClass<SettingEntity> where SettingEntity : class
    {
        class DefaultSetting
        {
            public string key { get; set; }
            public string value { get; set; }
        }
        SRL.Database srl_database = new Database();
        SRL.ClassManagement<SettingEntity> class_mgnt = new SRL.ClassManagement<SettingEntity>();
        string setting_table_name;
        static DbContext db;
        /// <summary>
        /// SettingEntity table must have  columns "key" and "value".
        /// </summary>
        /// <param name="db_"></param>
        public SettingClass(DbContext db_ = null)
        {
            db = db_;
            setting_table_name = typeof(SettingEntity).Name;
        }

        public void InitiateSetting(Dictionary<string, string> keyValuesetting)
        {

            srl_database.EntityRemoveAll<SettingEntity>(db);

            foreach (var item in keyValuesetting)
            {
                var instance = class_mgnt.CreateInstance();
                class_mgnt.SetProperty("key", instance, item.Key);
                class_mgnt.SetProperty("value", instance, item.Value);
                srl_database.EntityAdd<SettingEntity>(db, instance);
            }

            db.SaveChanges();
        }
        public Dictionary<string, string> CreateKeyValueSetting()
        {
            Dictionary<string, string> kv = new Dictionary<string, string>();
            kv["setting_is_set"] = "true";

            kv["form_font_size"] = "11";
            kv["menu_font_size"] = "11";
            kv["child_width_relative"] = "0/9";
            kv["child_height_relative"] = "0/9";
            kv["font_name"] = "B Koodak";
            kv["form_back_color"] = "Control";
            kv["menu_back_color"] = "Control";
            kv["font_factor"] = "1/003";
            kv["print_width1"] = "4";
            kv["print_height1"] = "3";
            kv["print_width_container_plus1"] = "3";
            kv["print_height_container_plus1"] = "76";
            kv["printer_name"] = "Fax";
            kv["db_version"] = "1";

            return kv;
        }
        public string SetDefaultSetting()
        {
            string error = string.Empty;

            try
            {

                Dictionary<string, string> default_setting = new Dictionary<string, string>();
                string sql = "select key, value from " + setting_table_name;
                var query_rows = srl_database.SqlQuery<DefaultSetting>(db, sql);
                foreach (var row in query_rows)
                {
                    default_setting[row.key] = row.value;
                }
                default_setting.Remove("default_setting");
                default_setting.Remove("db_version");
                string default_setting_json = Newtonsoft.Json.JsonConvert.SerializeObject(default_setting);

                sql = "select value from " + setting_table_name + " where key='default_setting' ";
                var query = srl_database.SqlQuery<string>(db, sql).DefaultIfEmpty(null).FirstOrDefault();

                if (query == null)
                {
                    sql = "insert into " + setting_table_name + " (key, value) values ('default_setting', '" + default_setting_json + "') ";
                    srl_database.ExecuteQuery(db, sql);
                }
                else
                {
                    sql = "update " + setting_table_name + " set value='" + default_setting_json + "' where  key ='default_setting' ";
                    srl_database.ExecuteQuery(db, sql);
                }
            }
            catch (Exception exc)
            {
                error = exc.Message;
            }
            return error;

        }
        public string RestoreDefaultSetting()
        {
            string error = string.Empty;

            try
            {
                string sql = "select value from " + setting_table_name + " where key='default_setting' ";
                var query = srl_database.SqlQuery<string>(db, sql).DefaultIfEmpty(null).FirstOrDefault();
                if (query == null)
                {
                    error = "no app default setting found";
                    return error;

                }
                if (!new SRL.Json().IsJson(query))
                {
                    error = "app default setting not saved in json format";
                    return error;
                }


                Dictionary<string, string> default_setting = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(query);
                default_setting["default_setting"] = query;
                default_setting["db_version"] = SqlQuerySettingTable("db_version");
                InitiateSetting(default_setting);
            }
            catch (Exception exc)
            {
                error = exc.Message;
            }
            return error;

        }
        public void ShowSettingInControls(Control form_font_size = null)
        {
            if (form_font_size != null) form_font_size.Text = SqlQuerySettingTable("form_font_size");
        }

        public string UpdateSetting(string form_font_size = null)
        {
            string error = string.Empty;

            if (form_font_size != null) error = ExecuteUpdateSettingTable("form_font_size", form_font_size);

            return error;
        }

        public string ExecuteUpdateSettingTable(string key, string value)
        {
            string sql = "update " + setting_table_name + " set value='" + value + "' where key='" + key + "'";
            return srl_database.ExecuteQuery(db, sql);
        }
        public bool CheckSettingIsSet()
        {
            string query = SqlQuerySettingTable("setting_is_set", null);
            int row_count = db.Set<SettingEntity>().Count();
            return query == null || query == "false" ? false : true;
        }
        public string SqlQuerySettingTable(string key, string default_if_empty = null)
        {

            string sql = "select value from " + setting_table_name + " where key='" + key + "'";
            var query = srl_database.SqlQuery<string>(db, sql).DefaultIfEmpty(default_if_empty).FirstOrDefault();
            return query;

        }

    }
    public class ChildParent
    {
        public IEnumerable<Control> GetAllChildrenControls(Control root)
        {
            var q = new Queue<Control>(root.Controls.Cast<Control>());
            while (q.Any())
            {
                var next = q.Dequeue();
                next.Controls.Cast<Control>().ToList().ForEach(q.Enqueue);

                yield return next;
            }
        }

        public void ClearControlsValue<ControlType>(IEnumerable<Control> controls_to_search, string property_to_clear, object clear_value)
        {
            SRL.ClassManagement<ControlType> class_mgnt = new ClassManagement<ControlType>();

            var q = new Queue<ControlType>();
            controls_to_search.OfType<ControlType>().ToList().ForEach(q.Enqueue);
            while (q.Any())
            {
                var next = q.Dequeue();
                class_mgnt.SetProperty(property_to_clear, next, clear_value);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent_to_refresh"></param>
        /// <param name="types_to_refresh"></param>
        /// <param name="controls_to_refresh">new List Type() { typeof(Button), typeof(TextBox),... }</param>
        public void RefreshFormControls(Control parent_to_refresh, List<Type> types_to_refresh = null, List<Control> controls_to_refresh = null)
        {
            IEnumerable<Control> childs = GetAllChildrenControls(parent_to_refresh);

            if (types_to_refresh != null)
                foreach (var item in types_to_refresh)
                {
                    if (item == typeof(ComboBox)) ClearControlsValue<ComboBox>(childs, "SelectedValue", -1);
                    if (item == typeof(TextBox)) ClearControlsValue<TextBox>(childs, "Text", string.Empty);
                    if (item == typeof(RadioButton)) ClearControlsValue<RadioButton>(childs, "Checked", false);
                    if (item == typeof(CheckBox)) ClearControlsValue<CheckBox>(childs, "Checked", false);
                }

            if (controls_to_refresh != null)
                foreach (dynamic control in controls_to_refresh)
                {
                    if (control is TextBox || control is ComboBox || control is MaskedTextBox) control.Text = string.Empty;
                    if (control is RadioButton || control is CheckBox) control.Checked = false;
                }
        }



        public object AddCategory<EntityT>(DbContext db, string categoryName, EntityT newCategory) where EntityT : class
        {
            SRL.ClassManagement<EntityT> class_mgnt = new ClassManagement<EntityT>();

            class_mgnt.SetProperty("categoryName", newCategory, categoryName);
            SRL.Database srl_database = new Database();
            srl_database.EntityAdd<EntityT>(db, newCategory);
            db.SaveChanges();
            return class_mgnt.GetProperty("ID", newCategory);
        }
        public void AddChildParent<EntityT>(DbContext db, long childId, long parentId, EntityT categoryClass) where EntityT : class
        {
            SRL.ClassManagement<EntityT> class_mgnt = new ClassManagement<EntityT>();
            class_mgnt.SetProperty("parentID", categoryClass, parentId);
            class_mgnt.SetProperty("childID", categoryClass, childId);
            SRL.Database srl_database = new Database();
            srl_database.EntityAdd<EntityT>(db, categoryClass);
            db.SaveChanges();
        }
        public static void DeleteNodeChilds(DbContext db, long parentId)
        {
            string childsId = null;// = db.CategoryClass.Where(x => x.parentID == parentId).Select(x => x.childID).ToArray();
            if (childsId.Length > 0)
            {
                foreach (var childId in childsId)
                {
                    //        CategoryClass categoryClass = db.CategoryClass.First(x => x.childID == childId);
                    //        db.CategoryClass.Remove(categoryClass);
                    //        db.SaveChanges();
                    //        DeleteNodeChilds(long.Parse(childId.ToString()));
                    //        Category category = db.Category.First(x => x.ID == childId);
                    //        db.Category.Remove(category);
                    //        db.SaveChanges();
                }
            }
        }
    }
    public class ActionManagement
    {
        public static void ParallelSend(System.Data.Entity.DbContext db, List<System.Data.Entity.DbSet> DBitems, string from, string parallel)
        {
            int all = DBitems.Count;
            List<Task> task_list = new List<Task>();
            int per_count = int.Parse(parallel);
            int take = all / per_count;
            int skip = 0;
            var DBquery = DBitems.AsQueryable();
            for (int j = 0; j < per_count; j++)
            {
                System.Windows.Forms.Application.DoEvents();
                var query = DBquery.Skip(skip).Take(take);
                skip += take;
                Task task = new Task(() => new Convertor()); //new Task(() => StartSending(db, query.ToList()));
                task_list.Add(task);
                task.Start();

            }
            Task.WaitAll(task_list.ToArray());
        }
    }
    public class ClassManagement<ClassType> // where ClassType : class 
    {

        public ClassType CreateInstance()
        {
            return (ClassType)Activator.CreateInstance(typeof(ClassType));
        }



        public void SetProperty(string property_name, ClassType instance, object value)
        {
            PropertyInfo propk = typeof(ClassType).GetProperty(property_name);
            propk.SetValue(instance, value, null);

        }
        public object GetProperty(string property_name, ClassType instance)
        {
            PropertyInfo propk = typeof(ClassType).GetProperty(property_name);
            return propk.GetValue(instance);

        }
        public string GetEnumDescription(ClassType enum_value)
        {
            //   enum with Description example:
            //       public enum MyEnum
            //            {
            //              [Description("label1")]
            //              name1=1,
            //
            //              [Description("label2")]
            //              Len=2
            //             }
            DescriptionAttribute[] attributes = (DescriptionAttribute[])enum_value.GetType().GetField(enum_value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
    public class WinUI
    {
        public class FormClass
        {
            private async void FadeInFormShowDialog(Form o, int interval = 50)
            {
                o.ShowDialog();
                o.Opacity = 0;
                //Object is not fully invisible. Fade it in
                while (o.Opacity < 1.0)
                {
                    await Task.Delay(interval);
                    o.Opacity += 0.05;
                }
                o.Opacity = 1; //make fully visible       
            }

            private async void FadeOutFormClose(Form o, int interval = 50)
            {
                //Object is fully visible. Fade it out
                while (o.Opacity > 0.0)
                {
                    await Task.Delay(interval);
                    o.Opacity -= 0.05;
                }
                o.Opacity = 0; //make fully invisible     
                o.Close();
            }

            public class EnableFadeShowForm
            {
                Form F = new Form();

                public EnableFadeShowForm(Form f)
                {
                    F = f;
                    //
                    // Required for Windows Form Designer support
                    //
                    InitializeComponent();

                    //
                    // TODO: Add any constructor code after InitializeComponent call
                    //
                    F.Opacity = 0;
                    f.Shown += F_Shown;
                }

                private void F_Shown(object sender, EventArgs e)
                {
                    tmrFade.Enabled = true;
                }


                protected internal System.Timers.Timer tmrFade;

                /// <summary>
                /// Required designer variable.
                /// </summary>
                private System.ComponentModel.Container components = null;

                #region Windows Form Designer generated code
                /// <summary>
                /// Required method for Designer support - do not modify
                /// the contents of this method with the code editor.
                /// </summary>
                private void InitializeComponent()
                {
                    tmrFade = new System.Timers.Timer();
                    ((System.ComponentModel.ISupportInitialize)(tmrFade)).BeginInit();
                    // 
                    // tmrFade
                    // 
                    tmrFade.Interval = 20;
                    tmrFade.SynchronizingObject = F;
                    tmrFade.Elapsed += new System.Timers.ElapsedEventHandler(this.tmrFade_Elapsed);
                    // 
                    // BaseForm
                    // 
                    F.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
                    F.ClientSize = new System.Drawing.Size(292, 266);
                    F.Name = "BaseForm";
                    F.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                    F.Text = "Form1";
                    ((System.ComponentModel.ISupportInitialize)(tmrFade)).EndInit();

                }
                #endregion

                private void tmrFade_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
                {
                    F.Opacity += 0.05;
                    if (F.Opacity >= .95)
                    {
                        F.Opacity = 1;
                        tmrFade.Enabled = false;
                    }
                }

            }

            public void MaximizeForm(Form form)
            {
                if (form.WindowState == FormWindowState.Normal)
                    form.WindowState = FormWindowState.Maximized;
                else form.WindowState = FormWindowState.Normal;
            }

            public void RoundBorderForm(Form frm)
            {
                //make a regtangular witd one point(x,y) and it's width and height:
                Rectangle Bounds = new Rectangle(0, 0, frm.Width, frm.Height);
                //size of a radius to disgh for a point arc
                int CornerRadius = 1;
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                //making regtangular width 4 arc
                path.AddArc(Bounds.X, Bounds.Y, CornerRadius, CornerRadius, 180, 90);
                path.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y, CornerRadius, CornerRadius, 270, 90);
                path.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
                path.AddArc(Bounds.X, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
                path.CloseAllFigures();

                frm.Region = new Region(path);
            }

        }
        public class TextBoxClass
        {
            public class TextBoxBorderColor
            {
                /// <summary>
                /// tb must have parent
                /// </summary>
                /// <param name="tb"></param>
                /// <param name="new_tb_"></param>
                /// <param name="border_color_"></param>
                /// <param name="border_focus_color_"></param>
                /// <param name="border_or_focus_or_both_"></param>
                public TextBoxBorderColor(TextBox tb, out TextBox new_tb_, Color border_color_, Color border_focus_color_, string border_or_focus_or_both_)
                {
                    TextBoxBorder new_tb = new TextBoxBorder(border_color_, border_focus_color_, border_or_focus_or_both_);

                    new_tb.Text = tb.Text;
                    new_tb.Size = new Size(tb.Width, tb.Height);

                    new_tb.Location = new Point(tb.Location.X, tb.Location.Y);

                    new_tb.Font = tb.Font;

                    Control parent = tb.Parent;
                    parent.Controls.Add(new_tb);
                    parent.Controls.Remove(tb);

                    new_tb_ = new_tb;


                }

                public class TextBoxBorder : TextBox
                {

                    public Color border_color;
                    public Color border_focus_color;
                    public string border_or_focus_or_both;
                    public TextBoxBorder(Color border_color_, Color border_focus_color_, string border_or_focus_or_both_)
                    {
                        border_color = border_color_;
                        border_focus_color = border_focus_color_;
                        border_or_focus_or_both = border_or_focus_or_both_;
                    }

                    [System.Runtime.InteropServices.DllImport("user32")]
                    private static extern IntPtr GetWindowDC(IntPtr hwnd);
                    private const int WM_NCPAINT = 0x85;
                    protected override void WndProc(ref Message m)
                    {
                        base.WndProc(ref m);
                        Pen pen;
                        switch (border_or_focus_or_both)
                        {
                            case "border":
                                if (m.Msg == WM_NCPAINT && !this.Focused)
                                {
                                    pen = new Pen(border_color);
                                    var dc = GetWindowDC(Handle);
                                    using (Graphics g = Graphics.FromHdc(dc))
                                    {
                                        g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                                    }
                                }
                                break;
                            case "focus":
                                if (m.Msg == WM_NCPAINT && this.Focused)
                                {
                                    pen = new Pen(border_focus_color);
                                    var dc = GetWindowDC(Handle);
                                    using (Graphics g = Graphics.FromHdc(dc))
                                    {
                                        g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                                    }
                                }
                                break;
                            case "both":
                                if (m.Msg == WM_NCPAINT && this.Focused)
                                {
                                    pen = new Pen(border_focus_color);
                                    var dc = GetWindowDC(Handle);
                                    using (Graphics g = Graphics.FromHdc(dc))
                                    {
                                        g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                                    }
                                }
                                else if (m.Msg == WM_NCPAINT && !this.Focused)
                                {
                                    pen = new Pen(border_color);
                                    var dc = GetWindowDC(Handle);
                                    using (Graphics g = Graphics.FromHdc(dc))
                                    {
                                        g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                                    }
                                }
                                break;

                        }

                    }
                }
            }

            public class TextBoxPlaceHolder
            {
                TextBox myTxtbx;


                public TextBoxPlaceHolder(TextBox tb_to_change)
                {
                    myTxtbx = tb_to_change;

                    myTxtbx.Text = "Enter text here...";
                    myTxtbx.GotFocus += MyTxtbx_GotFocus;
                    myTxtbx.LostFocus += MyTxtbx_LostFocus;
                }

                private void MyTxtbx_LostFocus(object sender, EventArgs e)
                {
                    if (String.IsNullOrWhiteSpace(myTxtbx.Text))
                        myTxtbx.Text = "Enter text here...";
                }

                private void MyTxtbx_GotFocus(object sender, EventArgs e)
                {
                    myTxtbx.Text = "";
                }


            }
        }

        public class LabelClass
        {
            public class LabelOrientation
            {
                public LabelOrientation(ref Label lbl_, out Label new_lbl_, LblOrientation textOrientation_ = LblOrientation.Rotate, double rotationAngle_ = 0d, LblDirection textDirection_ = LblDirection.NotSet)
                {
                    OrientedTextLabel new_lbl = new OrientedTextLabel(textOrientation_, rotationAngle_, textDirection_);

                    new_lbl.Text = lbl_.Text;
                    new_lbl.Size = new Size(lbl_.Height + lbl_.Width, lbl_.Width + lbl_.Height);

                    new_lbl.Location = new Point(
           lbl_.Location.X + lbl_.Size.Width / 2 - lbl_.Size.Height / 2,
           lbl_.Location.Y + lbl_.Size.Height / 2 - lbl_.Size.Width / 2);
                    lbl_.Anchor = AnchorStyles.None;

                    new_lbl.Font = lbl_.Font;

                    Control parent = lbl_.Parent;
                    parent.Controls.Remove(lbl_);
                    parent.Controls.Add(new_lbl);

                    new_lbl_ = new_lbl;

                }

                public enum LblOrientation
                {
                    Circle,
                    Arc,
                    Rotate
                }

                public enum LblDirection
                {
                    Clockwise,
                    AntiClockwise,
                    NotSet
                }

                public class OrientedTextLabel : System.Windows.Forms.Label
                {
                    #region Variables

                    private double rotationAngle;
                    private LblOrientation textOrientation;
                    private LblDirection textDirection;

                    #endregion

                    #region Constructor

                    public OrientedTextLabel(LblOrientation textOrientation_ = LblOrientation.Rotate, double rotationAngle = 0d, LblDirection textDirection_ = LblDirection.NotSet)
                    {
                        RotationAngle = rotationAngle;
                        TextOrientation = textOrientation_;
                        TextDirection = textDirection_;

                    }

                    #endregion

                    #region Properties

                    [Description("Rotation Angle"), Category("Appearance")]
                    public double RotationAngle
                    {
                        get
                        {
                            return rotationAngle;
                        }
                        set
                        {
                            rotationAngle = value;
                            this.Invalidate();
                        }
                    }

                    [Description("Kind of Text Orientation"), Category("Appearance")]
                    public LblOrientation TextOrientation
                    {
                        get
                        {
                            return textOrientation;
                        }
                        set
                        {
                            textOrientation = value;
                            this.Invalidate();
                        }
                    }

                    [Description("Direction of the Text"), Category("Appearance")]
                    public LblDirection TextDirection
                    {
                        get
                        {
                            return textDirection;
                        }
                        set
                        {
                            textDirection = value;
                            this.Invalidate();
                        }
                    }



                    #endregion

                    #region Method

                    protected override void OnPaint(PaintEventArgs e)
                    {
                        Graphics graphics = e.Graphics;

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.Trimming = StringTrimming.None;

                        Brush textBrush = new SolidBrush(this.ForeColor);

                        //Getting the width and height of the text, which we are going to write
                        float width = graphics.MeasureString(Text, this.Font).Width;
                        float height = graphics.MeasureString(Text, this.Font).Height;

                        //The radius is set to 0.9 of the width or height, b'cos not to
                        //hide and part of the text at any stage
                        float radius = 0f;
                        if (ClientRectangle.Width < ClientRectangle.Height)
                        {
                            radius = ClientRectangle.Width * 0.9f / 2;
                        }
                        else
                        {
                            radius = ClientRectangle.Height * 0.9f / 2;
                        }

                        //Setting the text according to the selection
                        switch (textOrientation)
                        {
                            case LblOrientation.Arc:
                                {
                                    //Arc angle must be get from the length of the text.
                                    float arcAngle = (2 * width / radius) / Text.Length;
                                    if (textDirection == LblDirection.Clockwise)
                                    {
                                        for (int i = 0; i < Text.Length; i++)
                                        {
                                            graphics.TranslateTransform(
                                                (float)(radius * (1 - Math.Cos(arcAngle * i + rotationAngle / 180 * Math.PI))),
                                                (float)(radius * (1 - Math.Sin(arcAngle * i + rotationAngle / 180 * Math.PI))));
                                            graphics.RotateTransform((-90 + (float)rotationAngle + 180 * arcAngle * i / (float)Math.PI));
                                            graphics.DrawString(Text[i].ToString(), this.Font, textBrush, 0, 0);
                                            graphics.ResetTransform();
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < Text.Length; i++)
                                        {
                                            graphics.TranslateTransform(
                                                (float)(radius * (1 - Math.Cos(arcAngle * i + rotationAngle / 180 * Math.PI))),
                                                (float)(radius * (1 + Math.Sin(arcAngle * i + rotationAngle / 180 * Math.PI))));
                                            graphics.RotateTransform((-90 - (float)rotationAngle - 180 * arcAngle * i / (float)Math.PI));
                                            graphics.DrawString(Text[i].ToString(), this.Font, textBrush, 0, 0);
                                            graphics.ResetTransform();
                                        }
                                    }
                                    break;
                                }
                            case LblOrientation.Circle:
                                {
                                    if (textDirection == LblDirection.Clockwise)
                                    {
                                        for (int i = 0; i < Text.Length; i++)
                                        {
                                            graphics.TranslateTransform(
                                                (float)(radius * (1 - Math.Cos((2 * Math.PI / Text.Length) * i + rotationAngle / 180 * Math.PI))),
                                                (float)(radius * (1 - Math.Sin((2 * Math.PI / Text.Length) * i + rotationAngle / 180 * Math.PI))));
                                            graphics.RotateTransform(-90 + (float)rotationAngle + (360 / Text.Length) * i);
                                            graphics.DrawString(Text[i].ToString(), this.Font, textBrush, 0, 0);
                                            graphics.ResetTransform();
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < Text.Length; i++)
                                        {
                                            graphics.TranslateTransform(
                                                (float)(radius * (1 - Math.Cos((2 * Math.PI / Text.Length) * i + rotationAngle / 180 * Math.PI))),
                                                (float)(radius * (1 + Math.Sin((2 * Math.PI / Text.Length) * i + rotationAngle / 180 * Math.PI))));
                                            graphics.RotateTransform(-90 - (float)rotationAngle - (360 / Text.Length) * i);
                                            graphics.DrawString(Text[i].ToString(), this.Font, textBrush, 0, 0);
                                            graphics.ResetTransform();
                                        }

                                    }
                                    break;
                                }

                            case LblOrientation.Rotate:
                                {
                                    //For rotation, who about rotation?
                                    double angle = (rotationAngle / 180) * Math.PI;
                                    graphics.TranslateTransform(
                                        (ClientRectangle.Width + (float)(height * Math.Sin(angle)) - (float)(width * Math.Cos(angle))) / 2,
                                        (ClientRectangle.Height - (float)(height * Math.Cos(angle)) - (float)(width * Math.Sin(angle))) / 2);
                                    graphics.RotateTransform((float)rotationAngle);
                                    graphics.DrawString(Text, this.Font, textBrush, 0, 0);
                                    graphics.ResetTransform();

                                    break;
                                }
                        }
                    }
                    #endregion
                }

            }

        }



        public void FullScreenNoTaskbar(Control control)
        {
            control.Left = control.Top = 0;
            control.Width = Screen.PrimaryScreen.WorkingArea.Width;
            control.Height = Screen.PrimaryScreen.WorkingArea.Height;
        }

        public class PictureBoxClass
        {
            public class PictureBoxHover
            {
                int width_magnify = 0;
                int height_magnify = 0;
                System.Windows.Forms.Cursor cursor;
                int opacity = 255;
                public PictureBoxHover()
                {
                }
                public void EnablePictureBoxHover(PictureBox pb, System.Windows.Forms.Cursor cursor_, int width_magnify_ = 0, int height_magnify_ = 0, int opacity_ = 255)
                {
                    width_magnify = width_magnify_;
                    height_magnify = height_magnify_;
                    opacity = opacity_;
                    cursor = cursor_;
                    pb.MouseEnter += new System.EventHandler(pb_MouseEnter);
                    pb.MouseLeave += new System.EventHandler(pb_MouseLeave);
                }

                public void PictureBoxOnlyHover(PictureBox pb, System.Windows.Forms.Cursor cursor_)
                {
                    cursor = cursor_;
                    pb.MouseEnter += new System.EventHandler(pb_MouseEnterOnlyHover);
                }

                private void pb_MouseEnter(object sender, EventArgs e)
                {
                    PictureBox pb = sender as PictureBox;

                    pb.Height += height_magnify;
                    pb.Width += width_magnify;
                    pb.Cursor = cursor;

                    SetPictueBoxOpacity(pb, opacity);

                }
                private void pb_MouseLeave(object sender, EventArgs e)
                {
                    PictureBox pb = sender as PictureBox;

                    pb.Height -= height_magnify;
                    pb.Width -= width_magnify;
                    SetPictueBoxOpacity(pb, 255);


                }

                private void pb_MouseEnterOnlyHover(object sender, EventArgs e)
                {
                    PictureBox pb = sender as PictureBox;

                    pb.Cursor = cursor;


                }


                public Bitmap CreateNonIndexedImage(Image src)
                {

                    Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    using (Graphics gfx = Graphics.FromImage(newBmp))
                    {
                        gfx.DrawImage(src, 0, 0);
                    }

                    return newBmp;

                }
                public void SetPictueBoxOpacity(PictureBox pb, int opc)
                {
                    Bitmap pic = (Bitmap)pb.Image;
                    for (int w = 0; w < pic.Width; w++)
                    {
                        for (int h = 0; h < pic.Height; h++)
                        {
                            Color c = pic.GetPixel(w, h);
                            Color newC = Color.FromArgb(opc, c);
                            try
                            {
                                pic.SetPixel(w, h, newC);
                            }
                            catch
                            {

                                pic = CreateNonIndexedImage(pic);
                                pic.SetPixel(w, h, newC);

                            }
                        }
                    }
                    pb.Image = pic;
                }
            }
        }


        public class AlterTitleBarColor : Form
        {
            /// <summary>
            /// add all of this class code (after constructor) to Form1.cs before it's constructor and call  DisableProcessWindowsGhosting() in that load event.
            /// </summary>
            public AlterTitleBarColor()
            {
                var accessHandle = this.Handle;
            }

            Color title_color = Color.Black;
            const int WM_NCPAINT = 0x85;

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetWindowDC(IntPtr hwnd);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern void DisableProcessWindowsGhosting();

            [DllImport("UxTheme.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

            protected override void OnHandleCreated(EventArgs e)
            {
                SetWindowTheme(this.Handle, "", "");
                base.OnHandleCreated(e);
            }


            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                switch (m.Msg)
                {
                    case WM_NCPAINT:
                        {
                            IntPtr hdc = GetWindowDC(m.HWnd);
                            using (Graphics g = Graphics.FromHdc(hdc))
                            {
                                Brush b = new SolidBrush(title_color);
                                g.FillRectangle(b, new Rectangle(0, 0, this.Width, this.Height)); //2000, 2000));
                            }
                            int r = ReleaseDC(m.HWnd, hdc);
                        }
                        break;
                }
            }


        }
        public Icon ResizeIcon(Icon icon, int width_multi_8 = 16, int height = 16)
        {
            Size size = new Size(width_multi_8, height);
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(icon.ToBitmap(), new Rectangle(Point.Empty, size));
            }
            return Icon.FromHandle(bitmap.GetHicon());
        }

        public class ButtonClass
        {
            public class StyleButton : Button
            {

                private Color _back = Color.Gray;
                private Color back_color;
                private Color back_color_disabled;
                private Color _activeBorder;
                private Color _fore = System.Drawing.Color.White;


                private Size _minSize;

                private bool _active;
                Button btn_change;

                public StyleButton(Button btn_to_change, Color back_color_, Color _activeBorder_, Color? back_color_disabled_ = null)
                {
                    btn_change = btn_to_change;
                    back_color = back_color_;
                    _activeBorder = _activeBorder_;

                    _minSize = btn_change.MinimumSize;


                    btn_change.BackColor = back_color;
                    btn_change.ForeColor = _fore;
                    btn_change.FlatAppearance.BorderColor = _back;
                    btn_change.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

                    btn_change.UseVisualStyleBackColor = false;
                    btn_change.MouseEnter += Btn_to_change_MouseEnter;
                    btn_change.MouseLeave += Btn_to_change_MouseLeave;
                    btn_change.EnabledChanged += btn_change_EnabledChanged;

                    back_color_disabled = back_color_disabled_ != null ? (Color)back_color_disabled_ : ControlPaint.LightLight(back_color);

                }

                private void btn_change_EnabledChanged(object sender, EventArgs e)
                {
                    if (btn_change.Enabled)
                    {
                        btn_change.BackColor = back_color;
                    }
                    else
                    {
                        btn_change.BackColor = back_color_disabled;
                    }

                }

                private void Btn_to_change_MouseLeave(object sender, EventArgs e)
                {
                    btn_change.Cursor = Cursors.Default;
                    if (!_active)
                        btn_change.FlatAppearance.BorderColor = back_color;
                }

                private void Btn_to_change_MouseEnter(object sender, EventArgs e)
                {
                    btn_change.Cursor = Cursors.Hand;

                    if (!_active)
                        btn_change.FlatAppearance.BorderColor = _activeBorder;
                }


                public void SetStateActive()
                {
                    _active = true;
                    btn_change.FlatAppearance.BorderColor = _activeBorder;
                }

                public void SetStateNormal()
                {
                    _active = false;
                    btn_change.FlatAppearance.BorderColor = back_color;
                }
            }
        }

        public class DatagridviewClass
        {
            public void StyleDatagridview(DataGridView dataGridView1, float cell_size = 10F, float header_size = 10F, int row_height = 25)
            {
                dataGridView1.DefaultCellStyle.Font = new Font(dataGridView1.DefaultCellStyle.Font.FontFamily, cell_size);

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.ColumnHeadersDefaultCellStyle.Font.FontFamily, header_size);

                dataGridView1.RowTemplate.Height = 25;
            }
        }

        public class MenuClass
        {
            public void MenuStripClickColoring(MenuStrip menu_strip, string item_name_to_alter_color, string basic_back_color_name)
            {
                foreach (ToolStripMenuItem item in menu_strip.Items.OfType<ToolStripMenuItem>())
                {
                    item.BackColor = default(Color);

                }
                menu_strip.Items[item_name_to_alter_color].BackColor = Color.FromName(basic_back_color_name);

            }
            public void MenuStripClickColoring(MenuStrip menu_strip, string item_name_to_alter_color, Color back_color, Color fore_color)
            {
                foreach (ToolStripMenuItem item in menu_strip.Items.OfType<ToolStripMenuItem>())
                {
                    item.BackColor = default(Color);
                    item.ForeColor = default(Color);

                }
                menu_strip.Items[item_name_to_alter_color].BackColor = back_color;
                menu_strip.Items[item_name_to_alter_color].ForeColor = fore_color;

            }

        }

    }
    public class WinTools
    {
        public class DataGridViewTool
        {
            /// <summary>
            /// Add column show/hide capability to a DataGridView. When user right-clicks 
            /// the cell origin a popup, containing a list of checkbox and column names, is
            /// shown. 
            /// example:  DataGridViewColumnSelector cs = new DataGridViewColumnSelector(dataGridView1);
            /// cs.PopupMaxHeight = 100;
            /// cs.PopupWidth = 110;
            /// </summary>
            public class DataGridViewColumnSelector
            {
                // the DataGridView to which the DataGridViewColumnSelector is attached
                private DataGridView mDataGridView = null;
                // a CheckedListBox containing the column header text and checkboxes
                private CheckedListBox mCheckedListBox;
                // a ToolStripDropDown object used to show the popup
                private ToolStripDropDown mPopup;

                private bool allow_all_cell_click = false;

                /// <summary>
                /// The max height of the popup
                /// </summary>
                public int PopupMaxHeight = 300;
                /// <summary>
                /// The width of the popup
                /// </summary>
                public int PopupWidth = 200;

                /// <summary>
                /// Gets or sets the DataGridView to which the DataGridViewColumnSelector is attached
                /// </summary>
                public DataGridView DataGridView
                {
                    get { return mDataGridView; }
                    set
                    {
                        // If any, remove handler from current DataGridView 
                        if (mDataGridView != null) mDataGridView.CellMouseClick -= new DataGridViewCellMouseEventHandler(mDataGridView_CellMouseClick);
                        // Set the new DataGridView
                        mDataGridView = value;
                        // Attach CellMouseClick handler to DataGridView
                        if (mDataGridView != null) mDataGridView.CellMouseClick += new DataGridViewCellMouseEventHandler(mDataGridView_CellMouseClick);
                    }
                }

                // When user right-clicks the cell origin, it clears and fill the CheckedListBox with
                // columns header text. Then it shows the popup. 
                // In this way the CheckedListBox items are always refreshed to reflect changes occurred in 
                // DataGridView columns (column additions or name changes and so on).
                void mDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
                {
                    bool click_cell = allow_all_cell_click ? true : (e.RowIndex == -1 && e.ColumnIndex == -1);
                    if (e.Button == MouseButtons.Right && click_cell)
                    {
                        mCheckedListBox.Items.Clear();
                        foreach (DataGridViewColumn c in mDataGridView.Columns)
                        {
                            mCheckedListBox.Items.Add(c.HeaderText, c.Visible);
                        }
                        int PreferredHeight = (mCheckedListBox.Items.Count * 16) + 7;
                        mCheckedListBox.Height = (PreferredHeight < PopupMaxHeight) ? PreferredHeight : PopupMaxHeight;
                        mCheckedListBox.Width = this.PopupWidth;
                        int x_location = e.X;
                        if (mDataGridView.RightToLeft == RightToLeft.Yes) x_location += mDataGridView.Width;

                        mPopup.Show(mDataGridView.PointToScreen(new Point(x_location, e.Y)));
                    }
                }

                // The constructor creates an instance of CheckedListBox and ToolStripDropDown.
                // the CheckedListBox is hosted by ToolStripControlHost, which in turn is
                // added to ToolStripDropDown.
                public DataGridViewColumnSelector()
                {
                    mCheckedListBox = new CheckedListBox();
                    mCheckedListBox.CheckOnClick = true;
                    mCheckedListBox.ItemCheck += new ItemCheckEventHandler(mCheckedListBox_ItemCheck);

                    ToolStripControlHost mControlHost = new ToolStripControlHost(mCheckedListBox);
                    mControlHost.Padding = Padding.Empty;
                    mControlHost.Margin = Padding.Empty;
                    mControlHost.AutoSize = false;

                    mPopup = new ToolStripDropDown();
                    mPopup.Padding = Padding.Empty;
                    mPopup.Items.Add(mControlHost);
                }

                public DataGridViewColumnSelector(DataGridView dgv, bool all_cell_click = false)
                    : this()
                {
                    this.DataGridView = dgv;
                    this.allow_all_cell_click = all_cell_click;
                }

                // When user checks / unchecks a checkbox, the related column visibility is 
                // switched.
                void mCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
                {
                    mDataGridView.Columns[e.Index].Visible = (e.NewValue == CheckState.Checked);
                }
            }

        }
        public class ComboTool
        {
            /// <summary>
            /// you should define Itemheight to best size
            /// </summary>
            /// <param name="cb"></param>
            /// <param name="height"></param>
            /// <param name="pad"></param>
            public void MakeComboBoxSizable(ComboBox cb, int height, Padding pad)
            {
                cb.DrawMode = DrawMode.OwnerDrawFixed;

                decimal factor1 = Decimal.Divide(cb.Height, cb.ItemHeight);
                decimal item_height1 = height / factor1;

                int factor2 = cb.Height - cb.ItemHeight;
                int item_height2 = height - factor2;

                int item_height = Decimal.ToInt32(Decimal.Divide(item_height1 + item_height2, 2));
                cb.ItemHeight = Decimal.ToInt32(item_height);
                cb.Margin = pad;
            }


            public class ComboItem
            {
                public string Text { get; set; }
                public object Value { get; set; }
            }

            /// <summary>
            /// this method make app slow. use it in your app rather than reference from SRL
            /// </summary>
            /// <typeparam name="ValueT"></typeparam>
            /// <param name="cb"></param>
            /// <param name="enumerable_data_source">enumerable_data_source is IEnumerable query of  new {string Text=?, object Value=? }</param>
            /// <param name="empty_row_value">empty row is added to top</param>
            public void ComboBoxDataBind<ValueT>(ComboBox cb, IEnumerable<dynamic> enumerable_data_source, ValueT empty_row_value)
            {
                var data_source = enumerable_data_source.ToList();
                // cb.Items.Clear();
                cb.DataSource = null;
                cb.DisplayMember = "Text";
                cb.ValueMember = "Value";
                data_source.Insert(0, new { Text = "", Value = empty_row_value });
                cb.DataSource = data_source;

            }


        }


        public string GetAppName(string default_app_name, string folder_containing_exe_path, List<string> file_not_searching, string app_extention_pattern = "*.exe")
        {
            string app_name = default_app_name;
            var dirs = System.IO.Directory.GetFiles(folder_containing_exe_path, app_extention_pattern).Select(x => x);
            foreach (var item in file_not_searching)
            {
                dirs = dirs.Where(name => !name.EndsWith(item));
            }

            app_name = System.IO.Path.GetFileNameWithoutExtension(dirs.DefaultIfEmpty(default_app_name).FirstOrDefault()).ToString();
            return app_name;
        }

        public T CloneControl<T>(T controlToClone)
            where T : Control
        {
            PropertyInfo[] controlProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            T instance = Activator.CreateInstance<T>();

            foreach (PropertyInfo propInfo in controlProperties)
            {
                if (propInfo.CanWrite)
                {
                    if (propInfo.Name != "WindowTarget")
                        propInfo.SetValue(instance, propInfo.GetValue(controlToClone, null), null);
                }
            }

            return instance;
        }


        public class TextBoxTool
        {
            public class Enable3DigitSeperation
            {
                TextBox tb;
                public Enable3DigitSeperation(TextBox tb_)
                {
                    tb = tb_;
                    tb_.TextChanged += new EventHandler(tb_TextChanged);
                }
                void tb_TextChanged(object sender, EventArgs e)
                {
                    string value = tb.Text.Replace(",", "");
                    ulong ul;
                    if (ulong.TryParse(value, out ul))
                    {
                        tb.TextChanged -= tb_TextChanged;
                        tb.Text = string.Format("{0:#,#}", ul);
                        tb.SelectionStart = tb.Text.Length;
                        tb.TextChanged += tb_TextChanged;
                    }

                }
            }

            /// <summary>
            /// return "" if control is empty or text with format if control is not empty
            /// </summary>
            /// <param name="control"></param>
            /// <returns></returns>
            public string GetRawStringMaskedTextBox(MaskedTextBox control)
            {
                string raw_text = "";
                MaskFormat format = control.TextMaskFormat;
                control.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                if (control.Text.Any())
                    control.TextMaskFormat = format;
                raw_text = control.Text;
                control.TextMaskFormat = format;
                return raw_text;
            }

        }





        public void AddChildToParentControlsAliagn(Control parent, Control child, bool reset_child_font = false)
        {
            if (reset_child_font) child.Font = default(Font);
            parent.Controls.Clear();
            parent.Controls.Add(child);
            AliagnChildToParent(parent, child);
        }
        public void AddChildToParentControlsZoomAndAliagn(Control parent, Control child, decimal font_factor = 1, bool use_parent_font_family = false, bool use_parent_font_size = false, bool reset_child_font = false)
        {
            FontFamily font_family = child.Font.FontFamily;
            FontStyle font_style = child.Font.Style;
            float child_font_size = child.Font.Size;

            if (use_parent_font_family)
            {
                font_family = parent.Font.FontFamily;
                font_style = parent.Font.Style;
            }

            if (use_parent_font_size)
            {
                child_font_size = parent.Font.Size;
            }

            if (!font_family.IsStyleAvailable(font_style))
            {
                font_style = font_family.IsStyleAvailable(FontStyle.Regular) ? FontStyle.Regular :
                    font_family.IsStyleAvailable(FontStyle.Bold) ? FontStyle.Bold :
                    font_family.IsStyleAvailable(FontStyle.Italic) ? FontStyle.Italic :
                    font_family.IsStyleAvailable(FontStyle.Underline) ? FontStyle.Underline : FontStyle.Strikeout;
            }


            decimal x_relative = Decimal.Divide(parent.Width, child.Width);
            decimal y_relative = Decimal.Divide(parent.Height, child.Height);
            var f = (x_relative + y_relative) / 2;
            f *= font_factor;


            child.Font = new Font(font_family, child_font_size * (float)f, font_style);

            AddChildToParentControlsAliagn(parent, child, reset_child_font);

        }

        public void AliagnChildToParent(Control parent, Control child)
        {
            child.Location = new Point(
    parent.ClientSize.Width / 2 - child.Size.Width / 2,
    parent.ClientSize.Height / 2 - child.Size.Height / 2);
            child.Anchor = AnchorStyles.None;

        }

        public void AliagnChildWidthToParent(Control parent, Control child)
        {
            child.Location = new Point(
    parent.ClientSize.Width / 2 - child.Size.Width / 2,
    child.Location.Y);
            child.Anchor = AnchorStyles.None;

        }
        public void AliagnChildHeightToParent(Control parent, Control child)
        {
            child.Location = new Point(
    child.Location.X,
    parent.ClientSize.Height / 2 - child.Size.Height / 2);
            child.Anchor = AnchorStyles.None;

        }
        public void AdjustChildToParent(Control parent_form, Control child, double child_width_relative, double child_height_relative)
        {
            int form_x = parent_form.Width;
            int form_y = parent_form.Height;

            child.Width = int.Parse(Math.Floor(child_width_relative * form_x).ToString());
            child.Height = int.Parse(Math.Floor(child_height_relative * form_y).ToString());
        }
        public void AdjustAndAliagnChildToParent(Control parent_form, Control child, double child_width_relative, double child_height_relative)
        {
            int form_x = parent_form.Width;
            int form_y = parent_form.Height;

            child.Width = int.Parse(Math.Floor(child_width_relative * form_x).ToString());
            child.Height = int.Parse(Math.Floor(child_height_relative * form_y).ToString());

            AliagnChildToParent(parent_form, child);
        }

        public class Media
        {

            public float GetScreenDpi(Control control, out float dpiX, out float dpiY)
            {
                Graphics graphics = control.CreateGraphics();
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
                return (dpiX + dpiY) / 2;
            }
            public float GetScreenDpi(Control control)
            {
                float dpiX, dpiY;
                Graphics graphics = control.CreateGraphics();
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
                return (dpiX + dpiY) / 2;
            }

            private Image CaptureScreen(Form form)
            {
                Bitmap memoryImage;
                Graphics myGraphics = form.CreateGraphics();

                Size s = form.Size;
                memoryImage = new Bitmap(s.Width, s.Height, myGraphics);
                return memoryImage;
            }


        }
        public class Modal : Form
        {
            public struct FormBorderSizes { public int top, bottom, left, right; }
            public FormBorderSizes GetFormBorderSizes(Form form)
            {
                Rectangle screenRectangle = RectangleToScreen(form.ClientRectangle);
                FormBorderSizes borders = new FormBorderSizes();
                borders.top = screenRectangle.Top - form.Top;
                borders.bottom = form.Bottom - screenRectangle.Bottom;
                borders.left = screenRectangle.Left - form.Left;
                borders.right = form.Right - screenRectangle.Right;
                return borders;
            }
            public Modal(Control user_control, string title, int width_ = 1000, int height_ = 500, Color? back_color = null)
            {
                this.Text = title;

                this.StartPosition = FormStartPosition.CenterScreen;

                var borders = GetFormBorderSizes(this);
                if (back_color != null) this.BackColor = (Color)back_color;
                this.Width = width_ + borders.right + borders.left;
                this.Height = height_ + borders.top + borders.bottom;

                new WinTools().AddChildToParentControlsAliagn(this, user_control);
            }


        }
        public bool ValidationInLabel(Label lblError, List<Control> fieldNotNull = null, List<TextBox> tbMobile = null)
        {
            lblError.Text = string.Empty;
            if (fieldNotNull != null)
                foreach (var item in fieldNotNull)
                {
                    if (string.IsNullOrWhiteSpace(item.Text))
                        lblError.Text += " فیلد " + item.Tag + " اجباری است. ";
                }
            if (tbMobile != null)
                foreach (var item in tbMobile)
                {
                    if (string.IsNullOrWhiteSpace(item.Text)) continue;
                    if (item.Text.Substring(0, 1) != "0" || item.Text.Length != 11)
                        lblError.Text += " فیلد " + item.Tag + " اشتباه است. ";
                }

            return string.IsNullOrWhiteSpace(lblError.Text) ? true : false;
        }
        public class UserControlValidation : UserControl
        {
            ErrorProvider errorProvider1;
            Control user_control;
            bool force_cancel = true;

            public enum ErrorTypes
            {
                [Description("اجباری")]
                NotNull = 1,

                [Description("11 رقمی شروع با صفر")]
                MobilePattern = 2,

                [Description("اجباری و 11 رقمی شروع با صفر")]
                NotNull_MobilePattern = 3,


                [Description("فرمت اشتباه")]
                MaskDatePattern = 4,

                [Description("عدد اعشاری مجاز است")]
                DecimalInput = 5,

                [Description("عدد اعشاری اجباری است")]
                DecimalInput_NotNull = 6,

                [Description("عدد صحیح اجباری است")]
                IntegerInput_NotNull = 7,

                [Description("ایمیل صحیح اجباری است")]
                EmailPattern_NotNull = 8

            }


            public UserControlValidation(Control uc, ErrorProvider errorProvider, bool force_cancel_)
            {
                errorProvider1 = errorProvider;
                user_control = uc;
                force_cancel = force_cancel_;
            }

            public void CheckAllField(List<Control> controls, out bool validation_result)
            {
                validation_result = false;
                bool main_force_cancel = force_cancel;
                force_cancel = true;
                foreach (Control control in controls)
                {
                    control.Focus();
                }
                if (user_control is UserControl)
                    validation_result = ((UserControl)user_control).ValidateChildren(ValidationConstraints.Enabled);
                else if (user_control is Form)
                    validation_result = ((Form)user_control).ValidateChildren(ValidationConstraints.Enabled);
                force_cancel = main_force_cancel;
                // user_control.Validate();
            }
            /// <summary>
            /// each control can use this method one time. eather add item to ErrorType enum source code or test another UserControlValidation object
            /// </summary>
            /// <param name="control"></param>
            /// <param name="error_position"></param>
            /// <param name="error_type"></param>
            /// <param name="padding"></param>
            public void ControlValidation(Control control, ErrorTypes error_type, ErrorIconAlignment error_position = ErrorIconAlignment.MiddleRight, int padding = 0)
            {
                errorProvider1.SetIconAlignment(control, error_position);
                errorProvider1.SetIconPadding(control, padding);
                switch (error_type)
                {
                    case ErrorTypes.NotNull:
                        control.Validating += new System.ComponentModel.CancelEventHandler(not_null_Validating);
                        break;
                    case ErrorTypes.MobilePattern:
                        control.Validating += new System.ComponentModel.CancelEventHandler(mobile_pattern_Validating);
                        control.KeyPress += new System.Windows.Forms.KeyPressEventHandler(number_input_KeyPress);
                        break;
                    case ErrorTypes.NotNull_MobilePattern:
                        control.Validating += new System.ComponentModel.CancelEventHandler(not_null_mobile_pattern_Validating);
                        control.KeyPress += new System.Windows.Forms.KeyPressEventHandler(number_input_KeyPress);
                        break;
                    case ErrorTypes.MaskDatePattern:
                        control.Validating += new System.ComponentModel.CancelEventHandler(mask_date_pattern_Validating);
                        break;
                    case ErrorTypes.DecimalInput:
                        control.KeyPress += new System.Windows.Forms.KeyPressEventHandler(decimal_input_KeyPress);
                        break;
                    case ErrorTypes.DecimalInput_NotNull:
                        control.KeyPress += new System.Windows.Forms.KeyPressEventHandler(decimal_input_KeyPress);
                        control.Validating += new System.ComponentModel.CancelEventHandler(not_null_Validating);
                        break;
                    case ErrorTypes.IntegerInput_NotNull:
                        control.KeyPress += new System.Windows.Forms.KeyPressEventHandler(integer_input_KeyPress);
                        control.Validating += new System.ComponentModel.CancelEventHandler(not_null_Validating);
                        break;
                    case ErrorTypes.EmailPattern_NotNull:
                        control.Validating += new System.ComponentModel.CancelEventHandler(not_null_email_pattern_Validating);
                        break;
                }

            }
            private void number_input_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8))
                {
                    e.Handled = true;
                    return;
                }
            }
            private void decimal_input_KeyPress(object sender, KeyPressEventArgs e)
            {
                //8	        BACKSPACE key
                //46        .  
                //47        /     
                //48        0 
                //49        1
                //50        2
                //51        3
                //52        4
                //53        5
                //54        6
                //55        7
                //56        8
                //57        9

                if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 47))
                {
                    e.Handled = true;
                    return;
                }

                // checks to make sure only 1 decimal is allowed
                if (e.KeyChar == 47)
                {
                    if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                        e.Handled = true;
                }
            }

            private void integer_input_KeyPress(object sender, KeyPressEventArgs e)
            {
                //8	        BACKSPACE key
                //46        .  
                //47        /     
                //48        0 
                //49        1
                //50        2
                //51        3
                //52        4
                //53        5
                //54        6
                //55        7
                //56        8
                //57        9

                if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8))
                {
                    e.Handled = true;
                    return;
                }

            }

            private void mask_date_pattern_Validating(object sender, CancelEventArgs e)
            {
                MaskedTextBox control = sender as MaskedTextBox;

                var dt = new DateTime();

                if (new WinTools.TextBoxTool().GetRawStringMaskedTextBox(control).Any())
                    if (!DateTime.TryParse(control.Text, out dt))
                    {
                        e.Cancel = force_cancel;
                        var msg = new SRL.ClassManagement<ErrorTypes>().GetEnumDescription(ErrorTypes.MaskDatePattern);

                        errorProvider1.SetError(control, msg);

                        return;
                    }

                errorProvider1.SetError(control, "");
            }
            private void not_null_mobile_pattern_Validating(object sender, CancelEventArgs e)
            {
                Control control = sender as Control;
                bool status = control.Text.Any() ?
                   ((control.Text.Length != 11 || control.Text.Substring(0, 1) != "0") ? true : false)
                   : true;
                if (status)
                {
                    e.Cancel = force_cancel;
                    var msg = new SRL.ClassManagement<ErrorTypes>().GetEnumDescription(ErrorTypes.NotNull_MobilePattern);

                    errorProvider1.SetError(control, msg);

                    return;
                }
                errorProvider1.SetError(control, "");
            }

            private void not_null_email_pattern_Validating(object sender, CancelEventArgs e)
            {
                Control control = sender as Control;
                bool status = control.Text.Any() ?
                   (!(new SRL.Convertor().IsValidEmail(control.Text)) ? true : false)
                   : true;
                if (status)
                {
                    e.Cancel = force_cancel;
                    var msg = new SRL.ClassManagement<ErrorTypes>().GetEnumDescription(ErrorTypes.EmailPattern_NotNull);

                    errorProvider1.SetError(control, msg);

                    return;
                }
                errorProvider1.SetError(control, "");
            }

            private void mobile_pattern_Validating(object sender, CancelEventArgs e)
            {
                Control control = sender as Control;
                if (control.Text.Any())
                    if (control.Text.Length != 11 || control.Text.Substring(0, 1) != "0")
                    {
                        e.Cancel = force_cancel;
                        var msg = new SRL.ClassManagement<ErrorTypes>().GetEnumDescription(ErrorTypes.MobilePattern);

                        errorProvider1.SetError(control, msg);

                        return;
                    }
                errorProvider1.SetError(control, "");
            }
            private void not_null_Validating(object sender, CancelEventArgs e)
            {
                Control control = sender as Control;
                if (control.Text.Trim() == "")
                {
                    e.Cancel = force_cancel;
                    var msg = new SRL.ClassManagement<ErrorTypes>().GetEnumDescription(ErrorTypes.NotNull);

                    errorProvider1.SetError(control, msg);

                    return;
                }
                errorProvider1.SetError(control, "");
            }

        }
    }
    public class Security
    {
        public Security() { }


        public enum UserRegistrationStatus
        {
            NotRegistered = 0,
            NotActivated = 1,
            Activated = 2
        }
        public void WinCheckLogin(DbContext db, string entity_name, WinSessionId session)
        {
            new SRL.WinLogin(db, entity_name, session).ShowDialog();

            if (!session.IsLogined) Environment.Exit(0);
        }
        public void CreateSession(string key, object value, System.Web.UI.Page page)
        {
            page.Session[key] = value;
        }
        public void LoginRedirect(System.Web.SessionState.HttpSessionState session, System.Web.HttpResponse response, string redirectUri)
        {
            if (session["username"] == null)
                response.Redirect(redirectUri);
            else
            {
                //  MessageBox(session["username"].ToString(), response);
            }
        }
        public void SendActivationEmail(string username, string registerHashValue, string registerActivationUri, string toMail, string subject, string body, Dictionary<string, object> response, string fromMail, string password)
        {
            try
            {
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(fromMail);
                mailMessage.To.Add(toMail);
                mailMessage.From = from;
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                SRL.Convertor convertor = new SRL.Convertor();
                string activationLink = convertor.MakeActivationLink(username, registerHashValue, registerActivationUri);
                mailMessage.Body += activationLink;
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential(fromMail, password);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
                response["emailSent"] = true;
            }
            catch (Exception ex)
            {
                response["emailSent"] = false;
                response["emailError"] = ex.Message;
            }
        }

        public string SendEmail(string username, string toMail, string subject, string body, string fromMail, string password, string attach_text_file_content = null, string attach_text_file_name = null)
        {
            string error = "";
            try
            {
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(fromMail);
                mailMessage.To.Add(toMail);
                mailMessage.From = from;
                mailMessage.Subject = subject;
                mailMessage.Body = body;

                if (attach_text_file_content != null)
                    mailMessage.Attachments.Add(Attachment.CreateAttachmentFromString(attach_text_file_content, attach_text_file_name));

                SRL.Convertor convertor = new SRL.Convertor();
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential(fromMail, password);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return error;
        }


        public bool RedirectIfNotLogin(System.Web.UI.Page page, Dictionary<string, object> response, string redirect)
        {
            var usernameSession = page.Session["username"];
            if (usernameSession == null)
            {
                response["redirect"] = redirect;
                return false;
            }
            else
            {
                response["username"] = usernameSession;
                return true;
            }
        }
        public string GetSHA1(string input)
        {
            using (System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();

            }
        }

    }
    public class KeyValue
    {
        public void AddItem(Dictionary<string, object> result, string key, object value)
        {
            result[key] = value;
        }
    }
    public class WebRequest
    {
        /// <summary>
        /// salam
        /// </summary>
        /// <param name="response">1</param>
        /// <param name="client">2</param>
        /// <param name="uri">3</param>
        /// <param name="input">4</param>

        public void PostAsJsonAsync(Dictionary<string, object> response, System.Net.Http.HttpClient client, string uri, object input)
        {
            System.Net.Http.HttpResponseMessage httpResponse = client.PostAsJsonAsync(uri, input).Result;
            string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
            response["httpResponse"] = httpResponse;
            response["responseContent"] = responseContent;
        }
    }
    public class WebResponse
    {


        public void WebMessageBox(string message, System.Web.HttpResponse response)
        {
            //ClientScript.RegisterStartupScript(GetType(), "Javascript", "javascript:alert('assa'); ", true);
            string msg = "<script type=\"text/javascript\" language=\"javascript\">";
            msg += "alert('" + message + "');";
            msg += "</script>";
            response.Write(message);
            // response.Write(msg);
        }
    }
    public class Convertor
    {
        /* input image with width = height is suggested to get the best result */
        /* png support in icon was introduced in Windows Vista */
        public bool ConvertImageToIcon(System.IO.Stream input_stream, System.IO.Stream output_stream, int size, bool keep_aspect_ratio = false)
        {
            System.Drawing.Bitmap input_bit = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(input_stream);
            if (input_bit != null)
            {
                int width, height;
                if (keep_aspect_ratio)
                {
                    width = size;
                    height = input_bit.Height / input_bit.Width * size;
                }
                else
                {
                    width = height = size;
                }
                System.Drawing.Bitmap new_bit = new System.Drawing.Bitmap(input_bit, new System.Drawing.Size(width, height));
                if (new_bit != null)
                {
                    // save the resized png into a memory stream for future use
                    System.IO.MemoryStream mem_data = new System.IO.MemoryStream();
                    new_bit.Save(mem_data, System.Drawing.Imaging.ImageFormat.Png);

                    System.IO.BinaryWriter icon_writer = new System.IO.BinaryWriter(output_stream);
                    if (output_stream != null && icon_writer != null)
                    {
                        // 0-1 reserved, 0
                        icon_writer.Write((byte)0);
                        icon_writer.Write((byte)0);

                        // 2-3 image type, 1 = icon, 2 = cursor
                        icon_writer.Write((short)1);

                        // 4-5 number of images
                        icon_writer.Write((short)1);

                        // image entry 1
                        // 0 image width
                        icon_writer.Write((byte)width);
                        // 1 image height
                        icon_writer.Write((byte)height);

                        // 2 number of colors
                        icon_writer.Write((byte)0);

                        // 3 reserved
                        icon_writer.Write((byte)0);

                        // 4-5 color planes
                        icon_writer.Write((short)0);

                        // 6-7 bits per pixel
                        icon_writer.Write((short)32);

                        // 8-11 size of image data
                        icon_writer.Write((int)mem_data.Length);

                        // 12-15 offset of image data
                        icon_writer.Write((int)(6 + 16));

                        // write image data
                        // png data must contain the whole png data file
                        icon_writer.Write(mem_data.ToArray());

                        icon_writer.Flush();

                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public bool ConvertImageToIcon(string input_image, string output_icon, int size, bool keep_aspect_ratio = false)
        {
            System.IO.FileStream input_stream = new System.IO.FileStream(input_image, System.IO.FileMode.Open);
            System.IO.FileStream output_stream = new System.IO.FileStream(output_icon, System.IO.FileMode.OpenOrCreate);

            bool result = ConvertImageToIcon(input_stream, output_stream, size, keep_aspect_ratio);

            input_stream.Close();
            output_stream.Close();

            return result;
        }

        public int InchToPixel(float inch, float dpi = 96)
        {
            return (int)(inch * dpi);
        }
        public float PixelToInch(int pixel, float dpi = 96)
        {
            return (pixel / dpi);
        }
        public decimal StringToDecimal(string value_to_parse, decimal? default_value = null, string app_decimal_symbol = "/", bool show_alarm_error = true)
        {
            if (string.IsNullOrWhiteSpace(value_to_parse)) return 0;

            decimal value_parsed = 0;
            bool parse_try = StringToDecimalTry(value_to_parse, out value_parsed);
            if (parse_try) return value_parsed;

            string current_decimal_symbol = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            value_to_parse = value_to_parse.Replace(app_decimal_symbol, current_decimal_symbol);
            parse_try = StringToDecimalTry(value_to_parse, out value_parsed);
            if (parse_try) return value_parsed;

            value_to_parse = value_to_parse.Replace(app_decimal_symbol, ".");
            parse_try = StringToDecimalTry(value_to_parse, out value_parsed);
            if (parse_try) return value_parsed;

            value_to_parse = value_to_parse.Replace(app_decimal_symbol, ",");
            parse_try = StringToDecimalTry(value_to_parse, out value_parsed);
            if (parse_try) return value_parsed;

            if (show_alarm_error) MessageBox.Show("نماد اعشاری سیستم را به / تغییر دهید. ترجیحا از فرمت فارسی استفاده کنید");
            if (default_value != null)
            {
                value_parsed = (decimal)default_value;
                return value_parsed;
            }
            if (show_alarm_error) MessageBox.Show("متغیر اعشاری دارای مقدار 0 است و خطا ایجاد خواهد شد. فرمت سیستم را بررسی کنید");
            return value_parsed;


        }

        public bool StringToFloatTry(string str, out float number)
        {

            return float.TryParse(str, NumberStyles.Any, CultureInfo.CurrentCulture, out number);
        }
        public bool StringToDecimalTry(string str, out Decimal number)
        {
            return Decimal.TryParse(str, NumberStyles.Any, CultureInfo.CurrentCulture, out number);
        }
        public DateTime EnglishToPersianDateTime(DateTime date)
        {

            PersianCalendar p = new System.Globalization.PersianCalendar();
            int year = p.GetYear(date);
            int month = p.GetMonth(date);
            int day = p.GetDayOfMonth(date);
            int h = p.GetHour(date);
            int m = p.GetMinute(date);
            int s = p.GetSecond(date);
            string str = string.Format("{0}/{1}/{2}  {3}:{4}:{5}", year, month, day, h, m, s);
            return DateTime.Parse(str);
        }
        public List<string> EnglishToPersianDateString(DateTime d)
        {
            List<string> date_list = new List<string>();
            PersianCalendar pc = new PersianCalendar();
            date_list.Add(string.Format("{0}/{1}/{2}", pc.GetYear(d), pc.GetMonth(d), pc.GetDayOfMonth(d)));
            int dayOfM = pc.GetDayOfMonth(d);
            dayOfM = pc.GetDayOfMonth(d) - 1 == 0 ? pc.GetDayOfMonth(d) : pc.GetDayOfMonth(d) - 1;
            var persianDate = new DateTime(pc.GetYear(d), pc.GetMonth(d), dayOfM);
            date_list.Add(persianDate.ToString("yyyy MMM ddd", CultureInfo.GetCultureInfo("fa-Ir")));

            date_list.Add(string.Format("{0}, {1}/{2}/{3} {4}:{5}:{6}\n",
                      pc.GetDayOfWeek(d),
                      pc.GetMonth(d),
                      pc.GetDayOfMonth(d),
                      pc.GetYear(d),
                      pc.GetHour(d),
                      pc.GetMinute(d),
                      pc.GetSecond(d)));

            string year = pc.GetYear(d).ToString();
            string mounth = pc.GetMonth(d) < 10 ? "0" + pc.GetMonth(d).ToString() : pc.GetMonth(d).ToString();
            string day = pc.GetDayOfMonth(d) < 10 ? "0" + pc.GetDayOfMonth(d).ToString() : pc.GetDayOfMonth(d).ToString();
            date_list.Add(year);
            date_list.Add(mounth);
            date_list.Add(day);

            return date_list;
        }
        public DateTime PersianToEnglishDate(int year, int month, int day)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime dt = new DateTime(year, month, day, pc);
            return dt;
        }

        public DateTime PersianToEnglishDate(int year, int month, int day, int hour, int minute, int second, int milsec)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime dt = pc.ToDateTime(year, month, day, hour, minute, second, milsec);

            return dt;
        }
        public void CopyDataTableToDataTable(DataTable dt_from, DataTable dt_to)
        {

            if (dt_from.Columns.Count > dt_to.Columns.Count)
            {
                dt_to.Columns.Clear();
                foreach (DataColumn col in dt_from.Columns)
                {
                    DataColumn col_ = new DataColumn(col.ColumnName);
                    dt_to.Columns.Add(col_);
                }
            }
            foreach (DataRow row in dt_from.Rows)
            {
                dt_to.Rows.Add(row.ItemArray);
            }
        }
        public void MakeDataTableFromDGV(DataGridView dgview, DataTable table, int devider, int index)
        {

            foreach (DataGridViewColumn col in dgview.Columns)
                table.Columns.Add(col.HeaderText, typeof(string));

            int row_index = 0;
            foreach (DataGridViewRow row in dgview.Rows)
            {
                Application.DoEvents();
                if (row.Index < index) continue;
                if (row.Index > index + devider - 1) break;
                table.Rows.Add();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    Application.DoEvents();

                    table.Rows[row_index][cell.ColumnIndex] = cell.Value;

                }
                row_index++;
            }

            int rowLeft = 45 - table.Rows.Count;
            for (int i = 0; i < rowLeft; i++)
            {
                table.Rows.Add();
            }
        }
        public string StringToRegx(string input)
        {
            return System.Text.RegularExpressions.Regex.Unescape(input);
        }
        public string MakeHashValue(string textToHash)
        {
            if (String.IsNullOrEmpty(textToHash))
                return String.Empty;
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] byteOfText = System.Text.Encoding.UTF8.GetBytes(textToHash);
                byte[] hashValue = sha.ComputeHash(byteOfText);
                return BitConverter.ToString(hashValue).Replace("-", String.Empty);
            }
        }
        public string MakeActivationLink(string username, string registerHashValue, string registerActivationUri)
        {
            //string host = HttpContext.Current.Request.Url.Authority;
            string host = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            String strPathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
            String strUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
            string activationLink = host + registerActivationUri + "?username=" + username + "&activationKey=" + registerHashValue;
            return activationLink;
        }
        public static string Mobile(string mobile)
        {
            mobile = mobile.Length == 10 ? "0" + mobile : mobile;
            return mobile;

        }
        public static string NationalId(string national_id)
        {
            national_id = national_id.Length == 8 ? "00" + national_id : (national_id.Length == 9 ? "0" + national_id : national_id);
            return national_id;
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
    public class Json : ControlLoad
    {
        public Json()
        {

        }
        public Json(Button btn)
            : base(btn)
        {

        }
        public T StringToJson<T>(string input) where T : new()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(input);
        }
        public bool IsJson(string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }

        public DataTable ConvertJsonToDataTable(List<Dictionary<string, object>> list)
        {
            DataTable dt = new DataTable();

            bool create_columns = false;

            int all_ = list.Count;

            foreach (var item in list)
            {
                ButtonLoader(all_);
                Dictionary<string, object> item_to_show = new Dictionary<string, object>();

                foreach (var item_ in item)
                {

                    if (item_.Value != null) item_to_show[item_.Key.ToString()] = item_.Value.ToString();
                    else item_to_show[item_.Key.ToString()] = null;
                }

                if (!create_columns)
                    foreach (var col in item_to_show.Keys)
                    {
                        dt.Columns.Add(col);
                        create_columns = true;
                    }
                int col_add = item_to_show.Values.Count - dt.Columns.Count;
                for (int i = 0; i < col_add; i++)
                {
                    dt.Columns.Add("added" + i);
                }
                dt.Rows.Add(item_to_show.Values.ToArray());
            }

            return dt;

        }

    }
    public class WinChart
    {
        public void MakeChart(Chart chart, string xValue, string yValue, IQueryable<object> query)
        {
            string chartName = "name";
            chart.ChartAreas.Clear();
            chart.ChartAreas.Add(chartName);
            chart.Series.Clear();
            chart.Series.Add(chartName);
            chart.Series[chartName].XValueMember = xValue;
            chart.Series[chartName].YValueMembers = yValue;

            chart.Series[chartName].IsValueShownAsLabel = true;

            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart.ChartAreas[0].AxisX.Interval = 1;

            chart.DataSource = query.ToList();
            chart.DataBind();
        }

        public void ShowDataOnChart(System.Windows.Forms.DataVisualization.Charting.Chart chart, string xValue, string yValue, IQueryable<object> query)
        {
            string chartName = "name";
            chart.ChartAreas.Clear();
            chart.ChartAreas.Add(chartName);
            chart.Series.Clear();
            chart.Series.Add(chartName);
            chart.Series[chartName].XValueMember = xValue;
            chart.Series[chartName].YValueMembers = yValue;

            chart.Series[chartName].IsValueShownAsLabel = true;

            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart.ChartAreas[0].AxisX.Interval = 1;

            chart.DataSource = query.ToList();
            chart.DataBind();
        }
    }
    public class Database : SRL.ControlLoad
    {
        public Database()
            : base()
        {

        }
        public Database(Button btn)
            : base(btn)
        {

        }



        public void EntityRemoveAll<EntityType>(DbContext db) where EntityType : class
        {
            foreach (var item in db.Set<EntityType>())
            {
                db.Set<EntityType>().Remove(item);
            }
            db.SaveChanges();

        }

        public void EntityAdd<EntityType>(DbContext db, EntityType instance) where EntityType : class
        {
            db.Set<EntityType>().Add(instance);
            db.SaveChanges();

        }
        public string ShowTableInDatagridview(string sql, DataGridView dgv, string connection_string)
        {
            string error = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(connection_string);
                SqlCommand com = new SqlCommand(sql, con);
                SqlDataAdapter data = new SqlDataAdapter(com);
                DataSet ds = new DataSet();
                data.Fill(ds);
                DataTable dt = ds.Tables[0];
                dgv.DataSource = dt;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return error;
        }
        public string ShowTablesInDB(string connection_string, DataTable dt, string where_cluase = null)
        {
            string sql = "SELECT table_name FROM INFORMATION_SCHEMA.TABLES   ";
            //WHERE TABLE_NAME LIKE '%TEST_%'
            if (!string.IsNullOrWhiteSpace(where_cluase)) sql += where_cluase;
            string error = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(connection_string);
                SqlCommand com = new SqlCommand(sql, con);
                SqlDataAdapter data = new SqlDataAdapter(com);
                DataSet ds = new DataSet();
                data.Fill(ds);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return error;
        }
        public List<string> AddTableToDB(DbContext db, string table_name, DataGridView dataGridView1)
        {
            string tb_name = table_name;
            string sql = "CREATE TABLE " + tb_name + " ( ID bigint IDENTITY(1,1) PRIMARY KEY)";
            db.Database.ExecuteSqlCommand(sql);

            List<string> columns = new List<string>();

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                columns.Add(col.HeaderText);
            }

            List<string> columns_add = new List<string>();

            foreach (var cola in columns)
            {
                columns_add.Add(cola + " nvarchar(255) ");
            }

            string columns_add_sql = string.Join(" ,", columns_add);

            sql = "alter table " + tb_name + " add  " + columns_add_sql;

            db.Database.ExecuteSqlCommand(sql);

            db.SaveChanges();
            return columns;
        }

        public string InserRowToTable(DbContext db, string table_name, DataGridView dataGridView1, Dictionary<string, string> other_value)
        {
            string error = string.Empty;

            List<string> columns = new List<string>();

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                columns.Add(col.HeaderText);
            }

            if (other_value != null) columns.AddRange(other_value.Keys.ToList());

            string columns_sql = string.Join(" ,", columns);

            string sql = "";
            int i = 0;
            int all_ = dataGridView1.Rows.Count;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                ButtonLoader(all_);
                List<string> cells = new List<string>();

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cells.Add(cell.Value == null ? "N''" : "N'" + cell.Value.ToString() + "'");
                }

                string other_sql = "";

                if (other_value != null) other_sql = " , N'" + string.Join("' , N'", other_value.Values.ToList()) + "'";

                string cells_sql = string.Join(" ,", cells) + other_sql;

                sql = "insert into " + table_name + " (" + columns_sql + ") values (" + cells_sql + ")";
                try
                {
                    i += db.Database.ExecuteSqlCommand(sql);
                }
                catch (Exception ex)
                {
                    error += ex.Message + " ";
                }
            }

            db.SaveChanges();

            return i.ToString() + (string.IsNullOrWhiteSpace(error) ? "" : " inserted correctly. other row errors: " + error);

        }

        public string SearchTable(DbContext db, string table_name, Dictionary<string, string> filter, DataGridView dgv, Label lblCount, string connection_string)
        {
            string error = string.Empty;

            List<string> where = new List<string>();
            where.Add("1=1");
            foreach (var item in filter)
            {
                where.Add(item.Key + "= '" + item.Value + "' ");
            }
            string where_sql = " where " + string.Join(" and ", where);
            string sql = "select * from " + table_name + where_sql;

            try
            {
                SqlConnection con = new SqlConnection(connection_string);
                SqlCommand com = new SqlCommand(sql, con);
                SqlDataAdapter data = new SqlDataAdapter(com);
                DataSet ds = new DataSet();
                data.Fill(ds);
                DataTable dt = ds.Tables[0];
                dgv.DataSource = dt;
                lblCount.Text = dgv.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return error;

        }

        public void TruncateTable(DbContext db, string table_name)
        {
            db.Database.ExecuteSqlCommand("truncate table " + table_name);
            db.SaveChanges();
        }
        public string ExecuteQuery(DbContext db, string query)
        {
            string error = string.Empty;

            try
            {
                db.Database.ExecuteSqlCommand(query);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return error;
        }
        public List<OutputType> SqlQuery<OutputType>(DbContext db, string query)
        {
            var result = db.Database.SqlQuery<OutputType>(query).ToList();
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conStr">metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string="data source=.\SOHEILLAMSO;initial catalog=Semnan;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework"</param>
        /// <param name="conStrName">e.g. SemnanEntity</param>
        /// <param name="control_to_load"></param>
        public bool UpdateConnectionString(string conStr, string conStrName, Control control_to_load = null)
        {
            //for sqlite: @"metadata=res://*/Model.Model1.csdl|res://*/Model.Model1.ssdl|res://*/Model.Model1.msl;provider=System.Data.SQLite.EF6;provider connection string='data source=C:\Program Files\hami\MyDatabase.sqlite'"
            // or :       @"metadata=res://*/Model.Model1.csdl|res://*/Model.Model1.ssdl|res://*/Model.Model1.msl;provider=System.Data.SQLite.EF6;provider connection string='data source=&quot;MyDatabase.sqlite&quot;'"

            if (control_to_load != null) ControlLoader(control_to_load, "connecting database...");

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
            string last_con = connectionStringsSection.ConnectionStrings[conStrName].ConnectionString;
            connectionStringsSection.ConnectionStrings[conStrName].ConnectionString = conStr;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");

            if (last_con != conStr) return false;
            else return true;

        }

        public void UpdateConnectionStringAndRestart(string conStr, string conStrName, Control control_to_load)
        {
            using (SRL.Database dbsrl = new SRL.Database())
            {
                if (!dbsrl.UpdateConnectionString(conStr, conStrName, control_to_load))
                {
                    Application.Restart();
                    Environment.Exit(0);
                }

            }

        }

    }
    public class ExcelManagement : SRL.ControlLoad
    {
        public ExcelManagement()
        {
        }
        public ExcelManagement(Button btn)
            : base(btn)
        {
        }
        public void LoadDGVFromExcelNoHead(OpenFileDialog ofDialog, Label lblFileName, DataGridView dgv)
        {
            ofDialog.Filter = "Only 97/2003 excel with one sheet|*.xls";
            ofDialog.ShowDialog();
            lblFileName.Text = ofDialog.FileName;

            ExcelLibrary.Office.Excel.Workbook excel_file = ExcelLibrary.Office.Excel.Workbook.Open(ofDialog.FileName);
            var worksheet = excel_file.Worksheets[0]; // assuming only 1 worksheet
            var cells = worksheet.Cells;

            if (true)
            {
                int file_last_column_index = cells.LastColIndex;
                // add columns
                foreach (var header in cells.GetRow(cells.FirstRowIndex))
                {
                    dgv.Columns.Add(header.Value.StringValue, header.Value.StringValue);
                }

                // add rows
                for (int rowIndex = cells.FirstRowIndex + 1; rowIndex <= cells.LastRowIndex; rowIndex++)
                {
                    ExcelLibrary.Office.Excel.Row file_row = cells.GetRow(rowIndex);
                    List<object> file_row_cells = new List<object>();
                    for (int i = 0; i < file_last_column_index + 1; i++)
                    {
                        file_row_cells.Add(file_row.GetCell(i).Value);
                    }
                    dgv.Rows.Add(file_row_cells.ToArray());

                    //dgv.Rows.Add(file_row.GetCell(0).Value, file_row.GetCell(1).Value, file_row.GetCell(2).Value, file_row.GetCell(3).Value, file_row.GetCell(4).Value,
                    //    file_row.GetCell(5).Value, file_row.GetCell(6).Value, file_row.GetCell(7).Value, file_row.GetCell(8).Value, file_row.GetCell(9).Value, file_row.GetCell(10).Value,
                    //    file_row.GetCell(11).Value, file_row.GetCell(12).Value, file_row.GetCell(13).Value, file_row.GetCell(14).Value, file_row.GetCell(15).Value, file_row.GetCell(16).Value,
                    //    file_row.GetCell(17).Value, file_row.GetCell(18).Value, file_row.GetCell(19).Value, file_row.GetCell(20).Value, file_row.GetCell(21).Value, file_row.GetCell(22).Value, file_row.GetCell(23).Value,
                    //    file_row.GetCell(24).Value, file_row.GetCell(25).Value, file_row.GetCell(26).Value, file_row.GetCell(27).Value, file_row.GetCell(28).Value, file_row.GetCell(29).Value, file_row.GetCell(30).Value
                    //    );
                }

            }
        }

        public void LoadDGVFromExcel(OpenFileDialog ofDialog, Label lblFileName, string[] main_headers, DataGridView dgv)
        {
            ofDialog.Filter = "Only 97/2003 excel with one sheet|*.xls";
            ofDialog.ShowDialog();
            lblFileName.Text = ofDialog.FileName;

            ExcelLibrary.Office.Excel.Workbook excel_file = ExcelLibrary.Office.Excel.Workbook.Open(ofDialog.FileName);
            var worksheet = excel_file.Worksheets[0]; // assuming only 1 worksheet
            var cells = worksheet.Cells;
            string check_header = CheckExcelHeaders(cells, main_headers);
            if (check_header == "true")
            {
                int file_last_column_index = cells.LastColIndex;
                // add columns
                foreach (var header in cells.GetRow(cells.FirstRowIndex))
                {
                    dgv.Columns.Add(header.Value.StringValue, header.Value.StringValue);
                }

                // add rows
                for (int rowIndex = cells.FirstRowIndex + 1; rowIndex <= cells.LastRowIndex; rowIndex++)
                {
                    ExcelLibrary.Office.Excel.Row file_row = cells.GetRow(rowIndex);
                    List<object> file_row_cells = new List<object>();
                    for (int i = 0; i < file_last_column_index + 1; i++)
                    {
                        file_row_cells.Add(file_row.GetCell(i).Value);
                    }
                    dgv.Rows.Add(file_row_cells.ToArray());

                    //dgv.Rows.Add(file_row.GetCell(0).Value, file_row.GetCell(1).Value, file_row.GetCell(2).Value, file_row.GetCell(3).Value, file_row.GetCell(4).Value,
                    //    file_row.GetCell(5).Value, file_row.GetCell(6).Value, file_row.GetCell(7).Value, file_row.GetCell(8).Value, file_row.GetCell(9).Value, file_row.GetCell(10).Value,
                    //    file_row.GetCell(11).Value, file_row.GetCell(12).Value, file_row.GetCell(13).Value, file_row.GetCell(14).Value, file_row.GetCell(15).Value, file_row.GetCell(16).Value,
                    //    file_row.GetCell(17).Value, file_row.GetCell(18).Value, file_row.GetCell(19).Value, file_row.GetCell(20).Value, file_row.GetCell(21).Value, file_row.GetCell(22).Value, file_row.GetCell(23).Value,
                    //    file_row.GetCell(24).Value, file_row.GetCell(25).Value, file_row.GetCell(26).Value, file_row.GetCell(27).Value, file_row.GetCell(28).Value, file_row.GetCell(29).Value, file_row.GetCell(30).Value
                    //    );
                }

            }

            else MessageBox.Show(check_header);
        }

        public string CheckExcelHeaders(ExcelLibrary.Office.Excel.CellCollection cells, string[] main_headers)
        {

            foreach (var file_header in cells.GetRow(cells.FirstRowIndex))
            {
                Application.DoEvents();
                if (!main_headers.Contains(file_header.Value.StringValue))
                {
                    return file_header.Value.StringValue + " is not valid.";
                }
                else continue;
            }

            List<string> file_headers = new List<string>();
            foreach (var file_header in cells.GetRow(cells.FirstRowIndex)) file_headers.Add(file_header.Value.StringValue);
            foreach (var main_header in main_headers)
            {
                Application.DoEvents();
                if (!file_headers.Contains(main_header))
                {
                    return "file does not have column: " + main_header;
                }
                else continue;
            }

            return "true";

        }
        public void ExportToExcell(DataGridView dgview, int devider, string fileFullName)
        {


            DataSet ds = new DataSet();
            int table_count = dgview.Rows.Count / devider;
            int index = 0;

            for (int i = 0; i < table_count; i++)
            {
                ButtonLoader(table_count);
                DataTable table = new DataTable(i.ToString());
                ds.Tables.Add(table);
                new Convertor().MakeDataTableFromDGV(dgview, table, devider, index);
                index += devider;
            }
            DataTable _table = new DataTable("else");
            ds.Tables.Add(_table);
            new Convertor().MakeDataTableFromDGV(dgview, _table, devider, index);

            //ExcelLibrary.DataSetHelper.CreateWorkbook(@Publics.desktop_root + "exported.xls", ds);
            ExcelLibrary.DataSetHelper.CreateWorkbook(@fileFullName, ds);
        }

    }
    public class ControlLoad : IDisposable
    {
        private Button btn_loader { get; set; }
        private Control control_to_load { get; set; }

        int foreach_looper = 0;

        public ControlLoad()
        {
        }

        public ControlLoad(Button btnLoader)
        {
            btn_loader = btnLoader;
            btn_loader.Tag = btn_loader.Text;
        }

        //public ControlLoad(Control controlToLoad)
        //{
        //    control_to_load = controlToLoad;
        //    control_to_load.Tag = control_to_load.Text;
        //}



        public void ButtonLoader(int all)
        {
            if (btn_loader != null)
            {
                btn_loader.Text = foreach_looper + " از " + all;
            }
            Application.DoEvents();

            foreach_looper++;
        }
        public void ButtonLoader()
        {
            if (btn_loader != null)
            {
                btn_loader.Text = foreach_looper.ToString();
            }
            Application.DoEvents();

            foreach_looper++;
        }
        public void ControlLoader(Control controlToLoad, string loading_text)
        {
            Application.DoEvents();
            control_to_load = controlToLoad;
            control_to_load.Tag = control_to_load.Text;
            control_to_load.Text = loading_text;
            Application.DoEvents();
        }

        public void Dispose()
        {
            if (btn_loader != null) btn_loader.Text = btn_loader.Tag.ToString();
            if (control_to_load != null) control_to_load.Text = control_to_load.Tag.ToString();
        }

    }
    public class WinReport<SubReportType>
    {
        public string DatasetName { get; set; }
        List<SubReportType> SubreportList;
        public WinReport(string dataset_name)
        {
            DatasetName = dataset_name;
        }

        public void MakePDFInDialog(Microsoft.Reporting.WinForms.ReportViewer reportViewer1)
        {
            Microsoft.Reporting.WinForms.Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;

            byte[] bytes = reportViewer1.LocalReport.Render(
                "PDF", null, out mimeType, out encoding, out filenameExtension,
                out streamids, out warnings);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "*PDF files (*.pdf)|*.pdf";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = "";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.FileStream newFile = new System.IO.FileStream(
                    saveFileDialog1.FileName, System.IO.FileMode.Create);
                newFile.Write(bytes, 0, bytes.Length);
                newFile.Close();
            }
        }

        public void MakePDF(Microsoft.Reporting.WinForms.ReportViewer reportViewer1, string fullFileName)
        {
            Microsoft.Reporting.WinForms.Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;

            byte[] bytes = reportViewer1.LocalReport.Render(
                "PDF", null, out mimeType, out encoding, out filenameExtension,
                out streamids, out warnings);

            System.IO.FileStream newFile = new System.IO.FileStream(
               fullFileName, System.IO.FileMode.Create);
            newFile.Write(bytes, 0, bytes.Length);
            newFile.Close();

        }

        public void LoadReport<ReportType>(Microsoft.Reporting.WinForms.ReportViewer reportViewer1
            , BindingSource report_binding_source, List<ReportType> report_list, List<SubReportType> sub_report_list)
        {
            if (sub_report_list != null)
                reportViewer1.LocalReport.SubreportProcessing +=
               new SubreportProcessingEventHandler(MySubreportEventHandler);
            SubreportList = sub_report_list;

            report_binding_source.DataSource = report_list;
            reportViewer1.RefreshReport();
        }

        private void MySubreportEventHandler(object sender
            , SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource(DatasetName, SubreportList));
        }

        public bool GetLocalReportRdlcSize(ReportViewer report_viewer, out float width, out float height)
        {
            System.Drawing.Printing.PaperSize paper_size = new System.Drawing.Printing.PaperSize();
            var report_setting = report_viewer.LocalReport.GetDefaultPageSettings();

            float width_ = report_setting.PaperSize.Width / 100F;
            float height_ = report_setting.PaperSize.Height / 100F;

            width = width_;
            height = height_;
            if (report_setting.IsLandscape)
            {
                width = height_;
                height = width_;

            }

            return report_setting.IsLandscape;

        }
    }
    public class FileManagement
    {
        public FileManagement()
        {

        }




        /// <summary>
        /// get full path file and save or replace icon file
        /// </summary>
        /// <param name="filePath">@"C:\hami.exe"</param>
        /// <param name="icon_full_path">@"e:\myfile.ico"</param>
        public void ExtractFileIcon(string filePath, string icon_full_path)
        {
            //@"e:\myfile.ico"
            //  var filePath = @"C:\Users\lamso1387\Documents\Visual Studio 2012\Projects\hami\hami\bin\Release\hami.exe";
            var theIcon = Icon.ExtractAssociatedIcon(filePath);

            if (theIcon != null)
            {
                // Save it to disk, or do whatever you want with it.
                using (var stream = new System.IO.FileStream(icon_full_path, System.IO.FileMode.OpenOrCreate))
                {
                    theIcon.Save(stream);
                    stream.Close();
                }
            }

        }

        public void MakeShortcutUrl(string shortcut_name, string shortcut_directory, string full_path_to_file)
        {

            using (StreamWriter writer = new StreamWriter(shortcut_directory + "\\" + shortcut_name + ".url"))
            {
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=" + full_path_to_file);
                writer.Flush();
            }
        }
        public void MakeShortcut(string shortcut_name, string shortcut_directory, string file_directory_path, string file_with_extention, string icon_full_path_from_file_directory = null)
        {
            string full_path_to_file = System.IO.Path.Combine(file_directory_path, file_with_extention);

            WshShell wsh = new WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(
                shortcut_directory + "\\" + shortcut_name + ".lnk") as IWshRuntimeLibrary.IWshShortcut;
            shortcut.Arguments = full_path_to_file;
            shortcut.TargetPath = full_path_to_file;
            // not sure about what this is for
            shortcut.WindowStyle = 1;
            shortcut.Description = full_path_to_file;
            shortcut.WorkingDirectory = file_directory_path;
            string icon_full_path = System.IO.Path.Combine(file_directory_path,
                icon_full_path_from_file_directory == null ? "" : icon_full_path_from_file_directory);
            if (!System.IO.File.Exists(icon_full_path))
            {
                icon_full_path = Path.GetDirectoryName(full_path_to_file) + "\\" + shortcut_name + ".ico";
                ExtractFileIcon(full_path_to_file, icon_full_path);
            }
            shortcut.IconLocation = icon_full_path;

            shortcut.Save();
        }
        public string ReplaceAllFilesFromDirToDir(string SourceFolderFullPath, string DestinationFolderFullPath)
        {
            try
            {

                foreach (string dirPath in Directory.GetDirectories(SourceFolderFullPath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(SourceFolderFullPath, DestinationFolderFullPath));

                CreateFolderOverwrite(DestinationFolderFullPath);

                foreach (string newPath in Directory.GetFiles(SourceFolderFullPath, "*.*",
                    SearchOption.AllDirectories))
                    System.IO.File.Copy(newPath, newPath.Replace(SourceFolderFullPath, DestinationFolderFullPath), true);
                return "";
            }
            catch (Exception exc)
            {
                return exc.Message;
            }

        }
        public void CreateFolderOverwrite(string FolderFullPath)
        {
            System.IO.Directory.CreateDirectory(FolderFullPath);
        }
        public string GetFilePathInRoot(string fileName)
        {
            string path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);

            path = Directory.GetParent(Directory.GetParent(path).FullName).FullName;

            path += @"\" + fileName;

            return path;
        }

        public string GetDesktopDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        }
        public string GetCurrentDirectory()
        {
            //System.IO.Path.GetDirectoryName(Application.ExecutablePath);  c:\users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug
            //System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); c:\users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug
            //System.IO.Directory.GetCurrentDirectory();    C:\Users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug
            //Thread.GetDomain().BaseDirectory  c:\users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug\
            //Environment.CurrentDirectory   C:\Users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug
            //Application.StartupPath  C:\Users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug
            return System.AppDomain.CurrentDomain.BaseDirectory; // c:\users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug\

        }
        public IEnumerable<string> GetFileLines(string fileFullName)
        {

            string path = @fileFullName;

            var get_line = System.IO.File.ReadLines(path);

            return get_line;
        }
        public string GetFileContent(string fileFullName, int line)
        {
            string path = @fileFullName;

            var get_line = System.IO.File.ReadLines(path).Skip(line - 1);

            return get_line.Count() < 1 ? "" : get_line.First();
        }

        public void SaveToFile(string fileFullName, string content, int line)
        {
            string path = @fileFullName;

            string[] arrLine = System.IO.File.ReadAllLines(path);
            while (arrLine.Count() < line)
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine("empty line created");
                tw.Close();
                arrLine = System.IO.File.ReadAllLines(path);
            }
            arrLine[line - 1] = content;
            System.IO.File.WriteAllLines(path, arrLine);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileFullName">Path.Combine(Application.StartupPath, fileName)</param>
        /// <param name="content"></param>
        public void SaveToFile(string fileFullName, string content)
        {
            System.IO.File.WriteAllText(fileFullName, content);

        }


        public void CopyToClipboard(string content)
        {
            Clipboard.SetText(content);
        }
        public void SaveToFileDialog(SaveFileDialog dlgSaveFile, string content)
        {
            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
            {
                //Save license data into local file
                System.IO.File.WriteAllText(dlgSaveFile.FileName, content.Trim(), Encoding.UTF8);
            }
        }




    }


    /// <summary>
    /// first call GetContentFontNameFromByte then call InstallFont
    /// </summary>
    public class FontClass
    {
        [DllImport("gdi32", EntryPoint = "AddFontResource")]
        private static extern int AddFontResourceA(string lpFileName);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern int AddFontResource(string lpszFilename);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern int CreateScalableFontResource(uint fdwHidden, string
        lpszFontRes, string lpszFontFile, string lpszCurrentPath);


        /// <summary>
        /// return font_name.ttf that is  WriteAllBytes from input bytes like Resources
        /// </summary>
        /// <param name="font_bytes">i.e. : Resources.irsan (irsan is resuource name)</param>
        /// <returns>font_name.ttf that is input of InstallFont</returns>
        public string GetContentFontNameFromByte(byte[] font_bytes, string filename_with_extention)
        {
            System.IO.File.WriteAllBytes(filename_with_extention, font_bytes);
            return filename_with_extention;
        }


        /// <summary>
        /// Installs font on the user's system and adds it to the registry so it's available on the next session
        /// Your font must be included in your project with its build path set to 'Content' and its Copy property
        /// set to 'Copy Always'
        /// </summary>
        /// <param name="contentFontName">Your font to be passed as a resource (i.e. "myfont.tff" that is WriteAllBytes). get it from </param>
        public void InstallFont(string contentFontName)
        {
            // Creates the full path where your font will be installed
            var fontDestination = Path.Combine(System.Environment.GetFolderPath
                                          (System.Environment.SpecialFolder.Fonts), contentFontName);

            if (!System.IO.File.Exists(fontDestination))
            {
                // Copies font to destination
                System.IO.File.Copy(Path.Combine(System.IO.Directory.GetCurrentDirectory(), contentFontName), fontDestination);

                // Retrieves font name
                // Makes sure you reference System.Drawing
                PrivateFontCollection fontCol = new PrivateFontCollection();
                fontCol.AddFontFile(fontDestination);
                var actualFontName = fontCol.Families[0].Name;

                //Add font
                AddFontResource(fontDestination);
                //Add registry entry   
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts",
          actualFontName, contentFontName, Microsoft.Win32.RegistryValueKind.String);
            }
        }
    }


}

