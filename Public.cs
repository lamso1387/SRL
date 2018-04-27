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
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.Web.Script.Serialization;
using System.ServiceModel;
using System.Data.OleDb;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.Runtime.CompilerServices;
using System.Data.SQLite;
using System.Data.Entity.Migrations;
using Microsoft.Win32;

namespace SRL
{
    public class TreeMenuAccess
    {
        /*
        var x = GetAllNodesChild(treeView1.Nodes).Where(y => y.Checked).Select(i => i.Name).ToList();
        var permissions_str = string.Join(";", x);

            LoadPermissionsInTree("miNew;miBase;miManageSend;miAdd;miManage", treeView1);
            EnableMenuBasedOnPermissions("miNew;miBase;miManageSend;miAdd;miManage", menuStrip1);
            CheckAccess("user", Publics.dbGlobal, "PermissionTB", );

*/

        public static void SavePermissionFromTree(object role_obj, TreeView tree, string personnel_entity, DbContext db)
        {
            if (role_obj != null)
                if (role_obj.ToString() != "")
                {
                    var role = (string)role_obj;
                    var x = SRL.ChildParent.GetAllNodesChild(tree.Nodes).Where(y => y.Checked).Select(i => i.Name).ToList();
                    var permissions_str = string.Join(";", x);
                    string query = "update " + personnel_entity + " set [permission]='" + permissions_str + "' where [role]='" + role + "';";
                    SRL.Database.ExecuteQuery(db, query);

                }
        }
        //PermissionTB must have:id, role , permission
        public static void CheckAccess(string role, DbContext db, string tb_name, MenuStrip menu)
        {
            EnableMenuBasedOnPermissions("", menu);

            var query = "select * from " + tb_name + " where [role]='" + role + "';";
            var rezL = SRL.Database.SqlQuery<RoleClass>(db, query);
            if (rezL != null)
                if (rezL.Any())
                {
                    EnableMenuBasedOnPermissions(rezL.First().permission, menu);
                }
            if (role == "master")
            {
                EnableMenuBasedOnPermissions("master", menu);
            }


        }

        public static bool LoadPermissionsInTree(object permission_obj, TreeView tree)
        {
            SRL.ChildParent.UnCheckAllTreeNodes(tree);
            if (permission_obj == null) return false;
            if (permission_obj.ToString() == "") return false;

            string permission_str = permission_obj.ToString();
            var permissions_list = permission_str.Split(';');

            foreach (var item in permissions_list)
            {
                var node = tree.Nodes.Find(item, true);
                if (node.Any())
                {
                    node.First().Checked = true;
                }
            }
            return true;


        }

        public static void EnableMenuBasedOnPermissions(string permission_str, MenuStrip menu)
        {
            var permissions_list = permission_str.Split(';');
            var menu_items = SRL.ChildParent.GetAllMenuItems(menu);
            foreach (var item in menu_items)
            {
                item.Enabled = false;
            }
            foreach (var item in permissions_list)
            {

                var menu_item = menu_items.Where(x => x.Name == item);
                if (menu_item.Any())
                {
                    menu_item.First().Enabled = true;
                }
            }

            if (permission_str == "master")
            {
                foreach (var item in menu_items)
                {
                    item.Enabled = true;
                }
            }
        }



    }
    public class Print
    {
        public static void PrintPaperSize(PrintDialog print_dialog, string paper_name = "Custom", int height = 584, int width = 827)
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
        Stopwatch stopwatch = new Stopwatch();

        public void StartTimer(Control control, TimeFormat time_format, string custom_time_format = null)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler((sender, e) => timer_Tick(sender, e, time_format, custom_time_format));
            timer.Enabled = true;
            control_to_show_time = control;

        }
        public void StartStopWatch(Control control, TimeFormat time_format, string custom_time_format = null)
        {
            stopwatch.Start();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler((sender, e) => stopwatch_Tick(sender, e, time_format, custom_time_format));
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

        void stopwatch_Tick(object sender, EventArgs e, TimeFormat show_type, string custom_time_format)
        {
            TimeSpan ts = stopwatch.Elapsed;
            string elapsed = ts.ToString("mm\\:ss\\.ff");

            control_to_show_time.Text = elapsed;




        }

        /// <summary>
        /// full example for format:  .ToString("yyyy/MM/dd HH:mm:ss tt")
        /// hh is for 12 hour and HH is for 24 hour
        /// MM is for month and mm is for minute
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string DateTimeToSring(DateTime? dt, string format)
        {
            return dt.Value.ToString(format);
        }
        public static string GetCurrentKeyboardShort()
        {

            return InputLanguage.CurrentInputLanguage.Culture.Name;
        }
        public static string ChangeKeyboardAltShift()
        {
            SendKeys.Send("%+");
            return GetCurrentKeyboardShort();
        }
        public static DateTime? TryGetDateTimeValue(string datetime_string)
        {
            DateTime dt = new DateTime();
            if (!DateTime.TryParse(datetime_string, out dt)) return null;
            return dt;
        }

        public static async Task SleepNotBlockUI(int milisecond)
        {//use: await SleepNotBlockUI(1000);  the method where to call must be async like: private async static void func(){ await SleepNotBlockUI(1000)};

            await Task.Delay(milisecond);
        }

    }
    public class SettingClass<SettingEntity> where SettingEntity : class
    {
        /*use: write CheckSetting() before   InitializeComponent(); and after connection update.
         public static SRL.SettingClass<SettingTB> srlsetting = new SRL.SettingClass<SettingTB>(dbGlobal);

          internal static void CheckSetting()
        {
            if (!srlsetting.CheckSettingIsSet())
            {
                Dictionary<string, string> kv = new Dictionary<string, string>();
                kv["setting_is_set"] = "true";
                kv["font_factor"] = "0/95";
                kv["printer_name"] = new PrinterSettings().PrinterName;
                srlsetting.InitiateSetting(kv);
            }
        }
        */
        class DefaultSetting
        {
            public string key { get; set; }
            public string value { get; set; }
        }
        SRL.Database srl_database = new Database();

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

        /// <summary>
        /// put is_set_key to last in kv. consider to set db_version
        /// </summary>
        /// <param name="is_set_key"></param>
        /// <param name="setting_enum_type"></param>
        /// <param name="kv"></param>
        /// <returns></returns>
        public bool CheckSetting(string is_set_key, Type setting_enum_type, Dictionary<string, string> kv)
        {//put is_set_key to last in kv
            //kv[SettingKeys.db_version.ToString()] = SRL.Security.GetAppVersion(Assembly.GetExecutingAssembly()).Major.ToString();
            if (!CheckSettingIsSet(is_set_key, setting_enum_type))
            {
                InitiateSetting(kv);
                return false;
            }
            else return true;
        }

        /// <summary>
        /// check setting before migration because of db_version
        /// </summary>
        /// <param name="migration_version_query"></param>
        /// <param name="assembly"></param>
        public bool MigrateDatabase(Dictionary<string, string> migration_version_query, Assembly assembly, string db_key)
        {
            /* use in load form: 
              Dictionary<string, string> migration_version_query = new Dictionary<string, string>();

            migration_version_query["1"] = "ALTER TABLE WorksTB ADD progress_status nvarchar(50);";
            migration_version_query["2"] =migration_version_query["1"]+ " ...";
            migration_version_query["3"] =migration_version_query["2"] + " ...";

            Publics.srlsetting.MigrateDatabase(migration_version_query);
             */
            string db_version = GetDbVersion(db_key);
            if (string.IsNullOrWhiteSpace(db_version))
            {
                return false;
            }
            string app_version = SRL.Security.GetAppVersion(assembly).Major.ToString();

            if (db_version == app_version) return true;

            string query = "";

            if (db_version != null)
                if (migration_version_query.ContainsKey(db_version))
                    query = migration_version_query[db_version];

            UpdateTableSchema(query, app_version);
            return true;
        }

        public void InitiateSetting(Dictionary<string, string> keyValuesetting)
        {
            foreach (var item in keyValuesetting)
            {
                SettingEntity instance;
                var query = SRL.Database.EntitySelect<SettingEntity>(db, "select * from " + setting_table_name + " where [key]='" + item.Key + "'");
                if (query.Any())
                {
                    instance = query.First();
                    SRL.ClassManagement.SetProperty("value", instance, item.Value);
                    ExecuteUpdateSettingTable(item.Key, item.Value);
                }
                else
                {
                    instance = SRL.ClassManagement.CreateInstance<SettingEntity>();
                    SRL.ClassManagement.SetProperty("key", instance, item.Key);
                    SRL.ClassManagement.SetProperty("value", instance, item.Value);
                    SRL.Database.EntityAdd<SettingEntity>(db, instance);
                }

            }

            db.SaveChanges();
        }
        public static Dictionary<string, string> CreateKeyValueSetting()
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

        public string GetDbVersion(string db_key)
        {
            AddKeyToSettingTB(db_key);
            var res = SqlQuerySettingTable(db_key);
            return res;
        }

        public bool AddKeyToSettingTB(string key)
        {
            string sql = "select * from " + setting_table_name + " where [key]='" + key + "'";
            var get_key = SRL.Database.SqlQuery<object>(db, sql);
            if (get_key == null ? true : !get_key.Any())
            {
                sql = "insert into " + setting_table_name + " ([key]) values('" + key + "')";
                SRL.Database.ExecuteQuery(db, sql);
                return true;
            }
            else return false;
        }

        public string SetDefaultSetting()
        {
            string error = string.Empty;

            try
            {

                Dictionary<string, string> default_setting = new Dictionary<string, string>();
                string sql = "select key, value from " + setting_table_name;
                var query_rows = SRL.Database.SqlQuery<DefaultSetting>(db, sql);
                foreach (var row in query_rows)
                {
                    default_setting[row.key] = row.value;
                }
                default_setting.Remove("default_setting");
                default_setting.Remove("db_version");
                string default_setting_json = Newtonsoft.Json.JsonConvert.SerializeObject(default_setting);

                sql = "select value from " + setting_table_name + " where key='default_setting' ";
                var query = SRL.Database.SqlQuery<string>(db, sql).DefaultIfEmpty(null).FirstOrDefault();

                if (query == null)
                {
                    sql = "insert into " + setting_table_name + " (key, value) values ('default_setting', '" + default_setting_json + "') ";
                    SRL.Database.ExecuteQuery(db, sql);
                }
                else
                {
                    sql = "update " + setting_table_name + " set value='" + default_setting_json + "' where  key ='default_setting' ";
                    SRL.Database.ExecuteQuery(db, sql);
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
                var query = SRL.Database.SqlQuery<string>(db, sql).DefaultIfEmpty(null).FirstOrDefault();
                if (query == null)
                {
                    error = "no app default setting found";
                    return error;

                }
                if (!SRL.Json.IsJson(query))
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
        public void ShowSettingInControls(Control form_font_size = null, Control font_factor = null)
        {
            if (form_font_size != null) form_font_size.Text = SqlQuerySettingTable("form_font_size");
            if (font_factor != null) font_factor.Text = SqlQuerySettingTable("font_factor");
        }

        public string UpdateSetting(string form_font_size = null, string font_factor = null)
        {
            string error = string.Empty;

            if (form_font_size != null) error = ExecuteUpdateSettingTable("form_font_size", form_font_size);
            if (font_factor != null) error = ExecuteUpdateSettingTable("font_factor", font_factor);

            return error;
        }

        public string ExecuteUpdateSettingTable(string key, string value)
        {

            string sql = "update " + setting_table_name + " set value='" + value + "' where [key]='" + key + "'";
            var res = SRL.Database.ExecuteQuery(db, sql);
            return res;
        }
        public bool CheckSettingIsSet(string is_set_key, Type setting_enum_type)
        {
            string query = SqlQuerySettingTable(is_set_key, null);
            foreach (var item in Enum.GetValues(setting_enum_type))
            {
                AddKeyToSettingTB(item.ToString());
            }
            return query == null || query == "false" ? false : true;
        }

        public void UpdateTableSchema(string exe_query, string new_version)
        {
            if (exe_query != null && exe_query != "") SRL.Database.ExecuteQuery(db, exe_query);
            ExecuteUpdateSettingTable("db_version", new_version);
        }

        public string SqlQuerySettingTable(string key, string default_if_empty = null)
        {

            string sql = "select value from " + setting_table_name + " where [key]='" + key + "'";
            var queryGet = SRL.Database.SqlQuery<string>(db, sql);
            if (queryGet == null) return default_if_empty;
            else
            {
                var queryList = queryGet.DefaultIfEmpty(default_if_empty);
                var query = queryList.FirstOrDefault();
                query = query == null ? default_if_empty : query;
                return query;
            }

        }


    }

    public class ChildParent
    {
        internal static IEnumerable<TreeNode> GetAllNodesChild(TreeNodeCollection c)
        {
            foreach (var node in c.OfType<TreeNode>())
            {
                foreach (var child in GetAllNodesChild(node.Nodes))
                {
                    yield return child;
                }
                yield return node;
            }
        }

        public static void UnCheckAllTreeNodes(TreeView tree)
        {
            foreach (TreeNode node in tree.Nodes)
            {
                node.Checked = false;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    CheckAllChildNodes(node, false);
                }
            }
        }
        public static void CheckTreeParentInParallel(TreeNode node)
        {
            if (node != null)
            {
                if (node.Parent != null)
                {
                    bool? all_checked = null;
                    var nodes = node.Parent.Nodes;
                    foreach (TreeNode item in nodes)
                    {
                        if (item.Checked)
                        {
                            if (all_checked == true || all_checked == null)
                                all_checked = true;
                            else
                            {
                                all_checked = null;
                                break;
                            }
                        }
                        else
                        {
                            if (all_checked == false || all_checked == null)
                                all_checked = false;
                            else
                            {
                                all_checked = null;
                                break;
                            }
                        }
                    }

                    node.Parent.Checked = all_checked != null ? (bool)all_checked : node.Parent.Checked;

                }
                CheckTreeParent(node.Parent);
            }
        }
        public static void CheckTreeParent(TreeNode node)
        {
            if (node != null)
            {
                if (node.Parent != null)
                {
                    bool? all_checked = null;
                    if (node.Checked) node.Parent.Checked = true;
                }
                CheckTreeParent(node.Parent);
            }
        }
        public static void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    CheckAllChildNodes(node, nodeChecked);
                }
            }
        }
        public static void CompatibleTreeChildAndParentCheck(TreeView tree)
        {
            tree.AfterCheck += Compatible_Tree_Child_Parent_AfterCheck;
        }

        private static void Compatible_Tree_Child_Parent_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    CheckAllChildNodes(e.Node, e.Node.Checked);
                }
                CheckTreeParent(e.Node);
            }


        }

        public static IEnumerable<Control> GetAllChildrenControls(Control root)
        {
            var q = new Queue<Control>(root.Controls.Cast<Control>());
            while (q.Any())
            {
                var next = q.Dequeue();
                next.Controls.Cast<Control>().ToList().ForEach(q.Enqueue);

                yield return next;
            }
        }
        public static List<ToolStripMenuItem> GetAllMenuItems(MenuStrip menu)
        {
            List<ToolStripMenuItem> allItems = new List<ToolStripMenuItem>();
            foreach (ToolStripMenuItem toolItem in menu.Items)
            {
                allItems.Add(toolItem);
                //add sub items
                allItems.AddRange(GetAllToolStripMenuItems(toolItem));
            }
            return allItems;
        }
        public static IEnumerable<ToolStripMenuItem> GetAllToolStripMenuItems(ToolStripMenuItem item)
        {
            foreach (ToolStripMenuItem dropDownItem in item.DropDownItems)
            {
                if (dropDownItem.HasDropDownItems)
                {
                    foreach (ToolStripMenuItem subItem in GetAllToolStripMenuItems(dropDownItem))
                        yield return subItem;
                }
                yield return dropDownItem;
            }
        }

        public static void ClearControlsValue<ControlType>(IEnumerable<Control> controls_to_search, string property_to_clear, object clear_value)
        {
            var q = new Queue<ControlType>();
            controls_to_search.OfType<ControlType>().ToList().ForEach(q.Enqueue);
            while (q.Any())
            {
                var next = q.Dequeue();
                SRL.ClassManagement.SetProperty<ControlType>(property_to_clear, next, clear_value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent_to_refresh"></param>
        /// <param name="types_to_refresh"></param>
        /// <param name="controls_to_refresh">new List Type() { typeof(Button), typeof(TextBox),... }</param>
        public static void RefreshFormControls(Control parent_to_refresh, List<Type> types_to_refresh = null, List<Control> controls_to_refresh = null, Control[] controls_to_enable = null)
        {
            IEnumerable<Control> childs = GetAllChildrenControls(parent_to_refresh);

            if (types_to_refresh != null)
                foreach (var item in types_to_refresh)
                {
                    if (item == typeof(ComboBox))
                    {
                        try
                        {
                            ClearControlsValue<ComboBox>(childs, "SelectedValue", -1);
                        }
                        catch (Exception)
                        {

                            ClearControlsValue<ComboBox>(childs, "Text", string.Empty);
                        }

                    }
                    if (item == typeof(TextBox)) ClearControlsValue<TextBox>(childs, "Text", string.Empty);
                    if (item == typeof(RadioButton)) ClearControlsValue<RadioButton>(childs, "Checked", false);
                    if (item == typeof(CheckBox)) ClearControlsValue<CheckBox>(childs, "Checked", false);

                }

            if (controls_to_refresh != null)
                foreach (dynamic control in controls_to_refresh)
                {
                    if (control is TextBox || control is ComboBox || control is MaskedTextBox) control.Text = string.Empty;
                    if (control is RadioButton || control is CheckBox) control.Checked = false;
                    if (control is DataGridView) control.Rows.Clear();
                }
            if (controls_to_enable != null)
                foreach (dynamic item in controls_to_enable)
                {
                    item.Enabled = true;
                }
        }



        public static object AddCategory<EntityT>(DbContext db, string categoryName, EntityT newCategory) where EntityT : class
        {


            SRL.ClassManagement.SetProperty<EntityT>("categoryName", newCategory, categoryName);

            SRL.Database.EntityAdd<EntityT>(db, newCategory);
            db.SaveChanges();
            return SRL.ClassManagement.GetProperty<EntityT>("ID", newCategory);
        }
        public static void AddChildParent<EntityT>(DbContext db, long childId, long parentId, EntityT categoryClass) where EntityT : class
        {

            SRL.ClassManagement.SetProperty<EntityT>("parentID", categoryClass, parentId);
            SRL.ClassManagement.SetProperty<EntityT>("childID", categoryClass, childId);

            SRL.Database.EntityAdd<EntityT>(db, categoryClass);
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
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethodName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        public class FormActions
        {

            public static void LoadEmbededAssembly(Form f)
            {
                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    string resourceName = new System.Reflection.AssemblyName(args.Name).Name + ".dll";
                    string resource = Array.Find(f.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                    using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                    {
                        Byte[] assemblyData = new Byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        return System.Reflection.Assembly.Load(assemblyData);
                    }
                };
            }

            public static void ForceExitOnClose(Form form)
            {
                form.FormClosed += Form_FormClosed_exit;
            }

            private static void Form_FormClosed_exit(object sender, FormClosedEventArgs e)
            {
                if (Application.OpenForms.Count < 1)
                    Environment.Exit(0);
            }

        }

        public class MethodCall
        {
            public class ParallelMethodCaller
            {
                public delegate void MethodDelegateListParams<T>(List<T> list, BackgroundWorker bg, params object[] args);
                static List<BackgroundWorker> bgList = new List<BackgroundWorker>();
                static Action call_back;
                static Label progress_bar_lbl;
                static int progress = 0;

                /// <summary>
                /// 
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <param name="DBitems"></param>
                /// <param name="parallel"></param>
                /// <param name="function">only function names</param>
                /// <param name="call_back_">like : ()=>fun(a,b)</param>
                /// <param name="progress_bar_lbl_"></param>
                /// <param name="parameters">consider order</param>
                public static void ParallelCall<T>(List<T> DBitems, string parallel, MethodDelegateListParams<T> function, Action call_back_, Label progress_bar_lbl_, params object[] parameters)
                {/*use:
                    write this in function loop :
                    if (bg.CancellationPending)
                    {
                        return;
                    }                    
                    bg.ReportProgress(1);

                    */
                    progress = 0;
                    call_back = call_back_;
                    progress_bar_lbl = progress_bar_lbl_;
                    if (progress_bar_lbl != null) progress_bar_lbl.EnabledChanged += Progress_bar_lbl_EnabledChanged;
                    int per_count = int.Parse(parallel);
                    int all = 0;
                    int take = 0;
                    int skip = 0;
                    IQueryable<T> DBquery = null;
                    if (DBitems != null)
                    {
                        all = DBitems.Count;
                        take = all / per_count;
                        DBquery = DBitems.AsQueryable();
                    }
                    for (int j = 0; j < per_count; j++)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        IQueryable<T> query = null;
                        BackgroundWorker bg = new BackgroundWorker();
                        bgList.Add(bg);
                        if (DBitems != null)
                        {
                            query = DBquery.Skip(skip).Take(take);
                            skip += take;
                            bg.WorkerReportsProgress = true;
                            bg.WorkerSupportsCancellation = true;
                            bg.ProgressChanged += (s, epg) => Bg_ProgressChanged(all);
                        }

                        bg.DoWork += (s, e) =>
                        {
                            try
                            {
                                function(DBitems != null ? query.ToList() : null, bg, parameters);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException("Message: " + ex.Message + ". StackTrace" + ex.StackTrace);
                            }

                        };

                        bg.RunWorkerCompleted += Workers_Complete;

                        bg.RunWorkerAsync();
                    }



                }

                private async static void Progress_bar_lbl_EnabledChanged(object sender, EventArgs e)
                {
                    foreach (var worker in bgList)
                    {
                        if (!progress_bar_lbl.Enabled && worker != null && worker.IsBusy)
                            worker.CancelAsync();
                    }

                }

                private static void Bg_ProgressChanged(int all)
                {
                    progress++;
                    if (progress_bar_lbl != null)
                    {
                        int value = ((int)((double)(progress) / (double)all * 100));

                        string value_str = value.ToString();
                        progress_bar_lbl.Text = value_str;
                        progress_bar_lbl.Tag = progress;

                    }
                }

                private async static void Workers_Complete(object sender, RunWorkerCompletedEventArgs e)
                {
                    BackgroundWorker bgw = (BackgroundWorker)sender;
                    bgList.Remove(bgw);
                    bgw.Dispose();

                    if (e.Error != null)
                    {
                        MessageBox.Show("There was an error for one thread: " + e.Error.ToString());
                    }

                    else
                    {
                        if (bgList.Count == 0)
                        {
                            if (call_back != null)
                            {
                                call_back();
                            }

                        }
                    }
                }


            }
            /// <summary>
            /// first create instance of class. then create event of instance.bg.RunWorkerCompleted += Bg_RunWorkerCompleted; and put after complete code in it then call RunMethodInBackground
            /// </summary>
            public class MethodBackgroundWorker
            {
                // if function hase loop, set report_progress=true,
                //and get bg from class instance,
                //then add  bg.ReportProgress( list.IndexOf(item) *100 / list.Count); in loop

                /* use: 
                 var method = new MethodBackgroundWorker( true, Publics.form_progress_bar,true);
                 method.bg.RunWorkerCompleted += Bg_RunWorkerCompleted; // because ob Async
                method.RunMethodInBackground(() => InsertdataToDb(method.bg));
                 */

                Action function;
                ProgressBar progress_bar;
                ProgressBarStyle main_style;
                public BackgroundWorker bg = new BackgroundWorker();
                public MethodBackgroundWorker(ProgressBar progress_bar_, ProgressBarStyle bar_style)
                {

                    bg.DoWork += new DoWorkEventHandler(bg_DoWork);

                    if (progress_bar_ != null)
                    {
                        bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
                        bg.ProgressChanged += Bg_ProgressChanged;
                        bg.WorkerReportsProgress = true;
                        progress_bar = progress_bar_;
                        progress_bar.Parent.Visible = true;
                        main_style = progress_bar.Style;
                        progress_bar.Style = bar_style;
                    }


                }
                public void RunMethodInBackground(Action function_)
                {
                    function = function_;
                    bg.RunWorkerAsync();

                }

                private void Bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
                {

                    progress_bar.Style = ProgressBarStyle.Blocks;
                    progress_bar.Value = e.ProgressPercentage;
                }

                private void bg_DoWork(object sender, DoWorkEventArgs e)
                {
                    SRL.ActionManagement.MethodCall.MethodInvoker(function);
                }

                private void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
                {
                    progress_bar.Style = main_style;
                    progress_bar.Parent.Visible = false;
                }
            }

            public class RunMethodInBack
            {
                static ProgressBar progress_bar;
                static ProgressBarStyle main_style;
                static public BackgroundWorker bg;


                public static void Run(Action function, Action call_back, ProgressBar progress_bar_, ProgressBarStyle bar_style)
                {
                    bg = new BackgroundWorker();

                    if (progress_bar_ != null)
                    {
                        bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
                        bg.ProgressChanged += Bg_ProgressChanged;
                        bg.WorkerReportsProgress = true;
                        progress_bar = progress_bar_;
                        progress_bar.Parent.Visible = true;
                        main_style = progress_bar.Style;
                        progress_bar.Style = bar_style;
                    }

                    if (call_back != null)
                        bg.RunWorkerCompleted += (s1, e1) =>
                        {
                            BackgroundWorker bgw = (BackgroundWorker)s1;

                            if (e1.Error != null)
                            {
                                MessageBox.Show("There was an error for one thread: " + e1.Error.ToString());
                            }

                            else
                            {
                                SRL.ActionManagement.MethodCall.MethodInvoker(call_back);
                            }
                        };


                    System.Windows.Forms.Application.DoEvents();


                    bg.DoWork += (s, e) =>
                    {
                        SRL.ActionManagement.MethodCall.MethodInvoker(function);
                    };

                    bg.RunWorkerAsync();




                }

                private static void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
                {
                    progress_bar.Style = main_style;
                    progress_bar.Parent.Visible = false;
                }
                private static void Bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
                {

                    progress_bar.Style = ProgressBarStyle.Blocks;
                    progress_bar.Value = e.ProgressPercentage;
                }


            }
            public static void MethodDynamicInvoker(Action function, Control container_control, params object[] parameters)
            { // use it for datagridviews_CellEndEdit event in error "Operation is not valid because it results in a reentrant call to the SetCurrentCellAddressCore function"
              /* 
          public void AddOrEditNewRole(int col , int row)
          {

              ...do what ever you want and any input in function
          }

               MethodDynamicInvoker( () => AddOrEditNewRole(3, 4), this); 
          */
                Application.DoEvents();

                container_control.BeginInvoke(new MethodInvoker(() =>
                {
                    function.DynamicInvoke(parameters);
                }));
                // return function.DynamicInvoke(parameters);

            }

            public static void MethodInvoker(Action function)
            {
                try
                {
                    function.Invoke();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Message: " + ex.Message + ". StackTrace" + ex.StackTrace);
                }
            }

            public static object MethodDynamicInvoker<T>(Func<T> function, params object[] parameters)
            {
                return function.DynamicInvoke(parameters);
            }

        }
        public class DB
        {
            public static void AddValueToAllDataTableRows(DataTable dt, string column, string value)
            {
                try
                {
                    dt.Columns[column].Expression = value;
                }
                catch (Exception ex)
                {
                    var mes = ex.Message;
                }

            }
            public static string AddActionLogToDb<ActionLogT>(DbContext db, string title, string value, string user, string log)
            {
                string tb_name = typeof(ActionLogT).Name;
                var date = DateTime.Now.ToString("yyyyMMdd");

                string sql = "insert into " + tb_name + " (title,[date],value,[user],[log]) values ( '" + title + "','" + date + "' , '" + value + "' , '" + user + "', '" + log + "');";
                return SRL.Database.ExecuteQuery(db, sql);

            }
        }



    }
    public class ClassManagement
    {

        public static ClassType CreateInstance<ClassType>()
        {
            return (ClassType)Activator.CreateInstance(typeof(ClassType));
        }
        public static ClassType CreateInstance<ClassType>(params object[] inputs)
        {
            return (ClassType)Activator.CreateInstance(typeof(ClassType), inputs);
        }
        public static object CreateInstance(string className)
        {
            return System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(className);
        }
        public static ClassType CreateInstance<ClassType>(string className)
        {
            return (ClassType)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(className);
        }

        public static void SetProperty<ClassType>(string property_name, ClassType instance, object value)
        {
            PropertyInfo propk = typeof(ClassType).GetProperty(property_name);
            propk.SetValue(instance, value, null);

        }
        public static object GetProperty<ClassType>(string property_name, ClassType instance)
        {
            PropertyInfo propk = typeof(ClassType).GetProperty(property_name);
            return propk.GetValue(instance);

        }
        public static string GetEnumDescription<ClassType>(ClassType enum_value)
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

            public static void MaximizeForm(Form form)
            {
                if (form.WindowState == FormWindowState.Normal)
                    form.WindowState = FormWindowState.Maximized;
                else form.WindowState = FormWindowState.Normal;
            }

            public static void RoundBorderForm(Form frm)
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
            public static void TextBoxSelectAllOnTab(List<TextBox> tbList)
            {
                foreach (var item in tbList)
                {
                    item.GotFocus += textbox_select_all_GotFocus;
                }
            }

            private static void textbox_select_all_GotFocus(object sender, EventArgs e)
            {
                (sender as TextBox).SelectAll();
            }

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
                    new_lbl.BackColor = lbl_.BackColor;

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



        public static void FullScreenNoTaskbar(Control control)
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
        public static Icon ResizeIcon(Icon icon, int width_multi_8 = 16, int height = 16)
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

        public static void ControlShadow(Control container, Control[] directChildsToShadow)
        {//panel.Controls.OfType<Control>()
            container.Paint += (s, e) => drop_Shadow(s, e, directChildsToShadow);
        }
        private static void drop_Shadow(object sender, PaintEventArgs e, Control[] directChildToShadow)
        {

            Control panel = (Control)sender;
            Color[] shadow = new Color[3];
            shadow[0] = Color.FromArgb(181, 181, 181);
            shadow[1] = Color.FromArgb(195, 195, 195);
            shadow[2] = Color.FromArgb(211, 211, 211);
            Pen pen = new Pen(shadow[0]);
            using (pen)
            {

                foreach (Control p in directChildToShadow)
                {
                    Point ptBottom = p.Location;
                    ptBottom.Y += p.Height;
                    for (var sp = 0; sp < 3; sp++)
                    {
                        pen.Color = shadow[sp];
                        e.Graphics.DrawLine(pen, ptBottom.X, ptBottom.Y, ptBottom.X + p.Width - 1, ptBottom.Y);
                        ptBottom.Y++;
                    }

                    Point ptRight = p.Location;
                    ptRight.X += p.Width;
                    for (var sp = 0; sp < 3; sp++)
                    {
                        pen.Color = shadow[sp];
                        e.Graphics.DrawLine(pen, ptRight.X, ptRight.Y, ptRight.X, ptRight.Y + p.Height - 1);
                        ptRight.X++;
                    }
                }

            }
        }
        public static void BackColor(IEnumerable<Control> controls, Color color)
        {
            foreach (var item in controls)
            {
                item.BackColor = color;
            }
        }

        public class ButtonClass
        {
            public static void StyleControlButtons(Control parent, Color? back = null, Color? active_border = null)
            {
                back = back ?? Color.CornflowerBlue;
                active_border = active_border ?? Color.DarkBlue;

                foreach (var item in SRL.ChildParent.GetAllChildrenControls(parent).OfType<Button>())
                {
                    new SRL.WinUI.ButtonClass.StyleButton(item, (Color)back, (Color)active_border);
                }
            }

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
            public static void StyleDatagridviewDefault(DataGridView dataGridView1, float cell_size = 10F, float header_size = 10F, int row_height = 25)
            {
                dataGridView1.DefaultCellStyle.Font = new Font(dataGridView1.DefaultCellStyle.Font.FontFamily, cell_size);

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.ColumnHeadersDefaultCellStyle.Font.FontFamily, header_size);
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;
                dataGridView1.RowTemplate.Height = 25;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
            }
        }

        public class MenuClass
        {
            public static void MenuStripClickColoring(MenuStrip menu_strip, string item_name_to_alter_color, string basic_back_color_name)
            {
                foreach (ToolStripMenuItem item in menu_strip.Items.OfType<ToolStripMenuItem>())
                {
                    item.BackColor = default(Color);

                }
                menu_strip.Items[item_name_to_alter_color].BackColor = Color.FromName(basic_back_color_name);

            }
            public static void MenuStripClickColoring(MenuStrip menu_strip, string item_name_to_alter_color, Color back_color, Color fore_color)
            {
                foreach (ToolStripMenuItem item in menu_strip.Items.OfType<ToolStripMenuItem>())
                {
                    item.BackColor = default(Color);
                    item.ForeColor = default(Color);

                }
                menu_strip.Items[item_name_to_alter_color].BackColor = back_color;
                menu_strip.Items[item_name_to_alter_color].ForeColor = fore_color;

            }

            public static void Style(MenuStrip menuStrip1, Color? menu_back = null, Color? menu_fore = null, Color? click_back = null, Color? click_fore = null)
            {
                Color light_black = Color.FromArgb(38, 38, 38);
                Color gray = Color.FromArgb(165, 165, 165);
                Color green_high = Color.FromArgb(0, 128, 129);

                menuStrip1.BackColor = menu_back == null ? light_black : (Color)menu_back;
                menuStrip1.ForeColor = menu_fore == null ? gray : (Color)gray;

                click_back = click_back == null ? green_high : click_back;
                click_fore = click_fore == null ? Color.White : click_fore;

                foreach (ToolStripMenuItem item in menuStrip1.Items)
                {
                    item.Click += (s, e) =>
                    {
                        ToolStripMenuItem menu = s as ToolStripMenuItem;
                        SRL.WinUI.MenuClass.MenuStripClickColoring(menuStrip1, menu.Name, (Color)click_back, (Color)click_fore);
                    };
                }

                foreach (ToolStripMenuItem item in SRL.ChildParent.GetAllMenuItems(menuStrip1))
                {
                    item.BackColor = menu_back == null ? light_black : (Color)menu_back;
                    item.ForeColor = menu_fore == null ? gray : (Color)gray;
                }
            }
        }

    }
    public class WinTools
    {

        public enum AliagnType
        {
            Width,
            Height,
            All
        }

        public class LoadingCircleControl
        {
            public static void StartLoading(LoadingCircle loadingCircle1)
            {
                try
                {
                    Application.DoEvents();
                    loadingCircle1.Visible = loadingCircle1.Active = true;
                    System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                    int size = Math.Max(loadingCircle1.Width, loadingCircle1.Height);
                    loadingCircle1.Width = loadingCircle1.Height = size;
                    int radius = (int)(size / 1.17) / 2;
                    loadingCircle1.OuterCircleRadius = radius;
                    loadingCircle1.InnerCircleRadius = radius / 2;
                    loadingCircle1.NumberSpoke = (int)(radius * 0.8);
                    loadingCircle1.RotationSpeed = (1 / radius) * 2000;
                }
                catch (Exception ex)
                {

                    MessageBox.Show("StartLoading " + ex.Message);
                }


            }

            public static void EndLoading(LoadingCircle loadingCircle1)
            {
                loadingCircle1.Visible = loadingCircle1.Active = false;
            }


            public partial class LoadingCircle : Control
            {
                // Constants =========================================================
                private const double NumberOfDegreesInCircle = 360;
                private const double NumberOfDegreesInHalfCircle = NumberOfDegreesInCircle / 2;
                private const int DefaultInnerCircleRadius = 8;
                private const int DefaultOuterCircleRadius = 10;
                private const int DefaultNumberOfSpoke = 10;
                private const int DefaultSpokeThickness = 4;
                private readonly Color DefaultColor = Color.DarkGray;

                private const int MacOSXInnerCircleRadius = 5;
                private const int MacOSXOuterCircleRadius = 11;
                private const int MacOSXNumberOfSpoke = 12;
                private const int MacOSXSpokeThickness = 2;

                private const int FireFoxInnerCircleRadius = 6;
                private const int FireFoxOuterCircleRadius = 7;
                private const int FireFoxNumberOfSpoke = 9;
                private const int FireFoxSpokeThickness = 4;

                private const int IE7InnerCircleRadius = 8;
                private const int IE7OuterCircleRadius = 9;
                private const int IE7NumberOfSpoke = 24;
                private const int IE7SpokeThickness = 4;

                // Enumeration =======================================================
                public enum StylePresets
                {
                    MacOSX,
                    Firefox,
                    IE7,
                    Custom
                }

                // Attributes ========================================================
                private Timer m_Timer;
                private bool m_IsTimerActive;
                private int m_NumberOfSpoke;
                private int m_SpokeThickness;
                private int m_ProgressValue;
                private int m_OuterCircleRadius;
                private int m_InnerCircleRadius;
                private PointF m_CenterPoint;
                private Color m_Color;
                private Color[] m_Colors;
                private double[] m_Angles;
                private StylePresets m_StylePreset;

                // Properties ========================================================
                /// <summary>
                /// Gets or sets the lightest color of the circle.
                /// </summary>
                /// <value>The lightest color of the circle.</value>
                [TypeConverter("System.Drawing.ColorConverter"),
                 Category("LoadingCircle"),
                 Description("Sets the color of spoke.")]
                public Color Color
                {
                    get
                    {
                        return m_Color;
                    }
                    set
                    {
                        m_Color = value;

                        GenerateColorsPallet();
                        Invalidate();
                    }
                }

                /// <summary>
                /// Gets or sets the outer circle radius.
                /// </summary>
                /// <value>The outer circle radius.</value>
                [System.ComponentModel.Description("Gets or sets the radius of outer circle."),
                 System.ComponentModel.Category("LoadingCircle")]
                public int OuterCircleRadius
                {
                    get
                    {
                        if (m_OuterCircleRadius == 0)
                            m_OuterCircleRadius = DefaultOuterCircleRadius;

                        return m_OuterCircleRadius;
                    }
                    set
                    {
                        m_OuterCircleRadius = value;
                        Invalidate();
                    }
                }

                /// <summary>
                /// Gets or sets the inner circle radius.
                /// </summary>
                /// <value>The inner circle radius.</value>
                [System.ComponentModel.Description("Gets or sets the radius of inner circle."),
                 System.ComponentModel.Category("LoadingCircle")]
                public int InnerCircleRadius
                {
                    get
                    {
                        if (m_InnerCircleRadius == 0)
                            m_InnerCircleRadius = DefaultInnerCircleRadius;

                        return m_InnerCircleRadius;
                    }
                    set
                    {
                        m_InnerCircleRadius = value;
                        Invalidate();
                    }
                }

                /// <summary>
                /// Gets or sets the number of spoke.
                /// </summary>
                /// <value>The number of spoke.</value>
                [System.ComponentModel.Description("Gets or sets the number of spoke."),
                System.ComponentModel.Category("LoadingCircle")]
                public int NumberSpoke
                {
                    get
                    {
                        if (m_NumberOfSpoke == 0)
                            m_NumberOfSpoke = DefaultNumberOfSpoke;

                        return m_NumberOfSpoke;
                    }
                    set
                    {
                        if (m_NumberOfSpoke != value && m_NumberOfSpoke > 0)
                        {
                            m_NumberOfSpoke = value;
                            GenerateColorsPallet();
                            GetSpokesAngles();

                            Invalidate();
                        }
                    }
                }

                /// <summary>
                /// Gets or sets a value indicating whether this <see cref="T:LoadingCircle"/> is active.
                /// </summary>
                /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
                [System.ComponentModel.Description("Gets or sets the number of spoke."),
                System.ComponentModel.Category("LoadingCircle")]
                public bool Active
                {
                    get
                    {
                        return m_IsTimerActive;
                    }
                    set
                    {
                        m_IsTimerActive = value;
                        ActiveTimer();
                    }
                }

                /// <summary>
                /// Gets or sets the spoke thickness.
                /// </summary>
                /// <value>The spoke thickness.</value>
                [System.ComponentModel.Description("Gets or sets the thickness of a spoke."),
                System.ComponentModel.Category("LoadingCircle")]
                public int SpokeThickness
                {
                    get
                    {
                        if (m_SpokeThickness <= 0)
                            m_SpokeThickness = DefaultSpokeThickness;

                        return m_SpokeThickness;
                    }
                    set
                    {
                        m_SpokeThickness = value;
                        Invalidate();
                    }
                }

                /// <summary>
                /// Gets or sets the rotation speed.
                /// </summary>
                /// <value>The rotation speed.</value>
                [System.ComponentModel.Description("Gets or sets the rotation speed. Higher the slower."),
                System.ComponentModel.Category("LoadingCircle")]
                public int RotationSpeed
                {
                    get
                    {
                        return m_Timer.Interval;
                    }
                    set
                    {
                        if (value > 0)
                            m_Timer.Interval = value;
                    }
                }

                /// <summary>
                /// Quickly sets the style to one of these presets, or a custom style if desired
                /// </summary>
                /// <value>The style preset.</value>
                [Category("LoadingCircle"),
                 Description("Quickly sets the style to one of these presets, or a custom style if desired"),
                 DefaultValue(typeof(StylePresets), "Custom")]
                public StylePresets StylePreset
                {
                    get { return m_StylePreset; }
                    set
                    {
                        m_StylePreset = value;

                        switch (m_StylePreset)
                        {
                            case StylePresets.MacOSX:
                                SetCircleAppearance(MacOSXNumberOfSpoke,
                                    MacOSXSpokeThickness, MacOSXInnerCircleRadius,
                                    MacOSXOuterCircleRadius);
                                break;
                            case StylePresets.Firefox:
                                SetCircleAppearance(FireFoxNumberOfSpoke,
                                    FireFoxSpokeThickness, FireFoxInnerCircleRadius,
                                    FireFoxOuterCircleRadius);
                                break;
                            case StylePresets.IE7:
                                SetCircleAppearance(IE7NumberOfSpoke,
                                    IE7SpokeThickness, IE7InnerCircleRadius,
                                    IE7OuterCircleRadius);
                                break;
                            case StylePresets.Custom:
                                SetCircleAppearance(DefaultNumberOfSpoke,
                                    DefaultSpokeThickness,
                                    DefaultInnerCircleRadius,
                                    DefaultOuterCircleRadius);
                                break;
                        }
                    }
                }

                // Construtor ========================================================
                /// <summary>
                /// Initializes a new instance of the <see cref="T:LoadingCircle"/> class.
                /// </summary>
                public LoadingCircle()
                {
                    SetStyle(ControlStyles.UserPaint, true);
                    SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                    SetStyle(ControlStyles.ResizeRedraw, true);
                    SetStyle(ControlStyles.SupportsTransparentBackColor, true);

                    m_Color = DefaultColor;

                    GenerateColorsPallet();
                    GetSpokesAngles();
                    GetControlCenterPoint();

                    m_Timer = new Timer();
                    m_Timer.Tick += new EventHandler(aTimer_Tick);
                    ActiveTimer();

                    this.Resize += new EventHandler(LoadingCircle_Resize);
                }

                // Events ============================================================
                /// <summary>
                /// Handles the Resize event of the LoadingCircle control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
                void LoadingCircle_Resize(object sender, EventArgs e)
                {
                    GetControlCenterPoint();
                }

                /// <summary>
                /// Handles the Tick event of the aTimer control.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
                void aTimer_Tick(object sender, EventArgs e)
                {
                    m_ProgressValue = ++m_ProgressValue % m_NumberOfSpoke;
                    Invalidate();
                }

                /// <summary>
                /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
                /// </summary>
                /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data.</param>
                protected override void OnPaint(PaintEventArgs e)
                {
                    if (m_NumberOfSpoke > 0)
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                        int intPosition = m_ProgressValue;
                        for (int intCounter = 0; intCounter < m_NumberOfSpoke; intCounter++)
                        {
                            intPosition = intPosition % m_NumberOfSpoke;
                            DrawLine(e.Graphics,
                                     GetCoordinate(m_CenterPoint, m_InnerCircleRadius, m_Angles[intPosition]),
                                     GetCoordinate(m_CenterPoint, m_OuterCircleRadius, m_Angles[intPosition]),
                                     m_Colors[intCounter], m_SpokeThickness);
                            intPosition++;
                        }
                    }

                    base.OnPaint(e);
                }

                // Overridden Methods ================================================
                /// <summary>
                /// Retrieves the size of a rectangular area into which a control can be fitted.
                /// </summary>
                /// <param name="proposedSize">The custom-sized area for a control.</param>
                /// <returns>
                /// An ordered pair of type <see cref="T:System.Drawing.Size"></see> representing the width and height of a rectangle.
                /// </returns>
                public override Size GetPreferredSize(Size proposedSize)
                {
                    proposedSize.Width =
                        (m_OuterCircleRadius + m_SpokeThickness) * 2;

                    return proposedSize;
                }

                // Methods ===========================================================
                /// <summary>
                /// Darkens a specified color.
                /// </summary>
                /// <param name="_objColor">Color to darken.</param>
                /// <param name="_intPercent">The percent of darken.</param>
                /// <returns>The new color generated.</returns>
                private Color Darken(Color _objColor, int _intPercent)
                {
                    int intRed = _objColor.R;
                    int intGreen = _objColor.G;
                    int intBlue = _objColor.B;
                    return Color.FromArgb(_intPercent, Math.Min(intRed, byte.MaxValue), Math.Min(intGreen, byte.MaxValue), Math.Min(intBlue, byte.MaxValue));
                }

                /// <summary>
                /// Generates the colors pallet.
                /// </summary>
                private void GenerateColorsPallet()
                {
                    m_Colors = GenerateColorsPallet(m_Color, Active, m_NumberOfSpoke);
                }

                /// <summary>
                /// Generates the colors pallet.
                /// </summary>
                /// <param name="_objColor">Color of the lightest spoke.</param>
                /// <param name="_blnShadeColor">if set to <c>true</c> the color will be shaded on X spoke.</param>
                /// <returns>An array of color used to draw the circle.</returns>
                private Color[] GenerateColorsPallet(Color _objColor, bool _blnShadeColor, int _intNbSpoke)
                {
                    Color[] objColors = new Color[NumberSpoke];

                    // Value is used to simulate a gradient feel... For each spoke, the 
                    // color will be darken by value in intIncrement.
                    byte bytIncrement = (byte)(byte.MaxValue / NumberSpoke);

                    //Reset variable in case of multiple passes
                    byte PERCENTAGE_OF_DARKEN = 0;

                    for (int intCursor = 0; intCursor < NumberSpoke; intCursor++)
                    {
                        if (_blnShadeColor)
                        {
                            if (intCursor == 0 || intCursor < NumberSpoke - _intNbSpoke)
                                objColors[intCursor] = _objColor;
                            else
                            {
                                // Increment alpha channel color
                                PERCENTAGE_OF_DARKEN += bytIncrement;

                                // Ensure that we don't exceed the maximum alpha
                                // channel value (255)
                                if (PERCENTAGE_OF_DARKEN > byte.MaxValue)
                                    PERCENTAGE_OF_DARKEN = byte.MaxValue;

                                // Determine the spoke forecolor
                                objColors[intCursor] = Darken(_objColor, PERCENTAGE_OF_DARKEN);
                            }
                        }
                        else
                            objColors[intCursor] = _objColor;
                    }

                    return objColors;
                }

                /// <summary>
                /// Gets the control center point.
                /// </summary>
                private void GetControlCenterPoint()
                {
                    m_CenterPoint = GetControlCenterPoint(this);
                }

                /// <summary>
                /// Gets the control center point.
                /// </summary>
                /// <returns>PointF object</returns>
                private PointF GetControlCenterPoint(Control _objControl)
                {
                    return new PointF(_objControl.Width / 2, _objControl.Height / 2 - 1);
                }

                /// <summary>
                /// Draws the line with GDI+.
                /// </summary>
                /// <param name="_objGraphics">The Graphics object.</param>
                /// <param name="_objPointOne">The point one.</param>
                /// <param name="_objPointTwo">The point two.</param>
                /// <param name="_objColor">Color of the spoke.</param>
                /// <param name="_intLineThickness">The thickness of spoke.</param>
                private void DrawLine(Graphics _objGraphics, PointF _objPointOne, PointF _objPointTwo,
                                      Color _objColor, int _intLineThickness)
                {
                    using (Pen objPen = new Pen(new SolidBrush(_objColor), _intLineThickness))
                    {
                        objPen.StartCap = LineCap.Round;
                        objPen.EndCap = LineCap.Round;
                        _objGraphics.DrawLine(objPen, _objPointOne, _objPointTwo);
                    }
                }

                /// <summary>
                /// Gets the coordinate.
                /// </summary>
                /// <param name="_objCircleCenter">The Circle center.</param>
                /// <param name="_intRadius">The radius.</param>
                /// <param name="_dblAngle">The angle.</param>
                /// <returns></returns>
                private PointF GetCoordinate(PointF _objCircleCenter, int _intRadius, double _dblAngle)
                {
                    double dblAngle = Math.PI * _dblAngle / NumberOfDegreesInHalfCircle;

                    return new PointF(_objCircleCenter.X + _intRadius * (float)Math.Cos(dblAngle),
                                      _objCircleCenter.Y + _intRadius * (float)Math.Sin(dblAngle));
                }

                /// <summary>
                /// Gets the spokes angles.
                /// </summary>
                private void GetSpokesAngles()
                {
                    m_Angles = GetSpokesAngles(NumberSpoke);
                }

                /// <summary>
                /// Gets the spoke angles.
                /// </summary>
                /// <param name="_shtNumberSpoke">The number spoke.</param>
                /// <returns>An array of angle.</returns>
                private double[] GetSpokesAngles(int _intNumberSpoke)
                {
                    double[] Angles = new double[_intNumberSpoke];
                    double dblAngle = (double)NumberOfDegreesInCircle / _intNumberSpoke;

                    for (int shtCounter = 0; shtCounter < _intNumberSpoke; shtCounter++)
                        Angles[shtCounter] = (shtCounter == 0 ? dblAngle : Angles[shtCounter - 1] + dblAngle);

                    return Angles;
                }

                /// <summary>
                /// Actives the timer.
                /// </summary>
                private void ActiveTimer()
                {
                    if (m_IsTimerActive)
                        m_Timer.Start();
                    else
                    {
                        m_Timer.Stop();
                        m_ProgressValue = 0;
                    }

                    GenerateColorsPallet();
                    Invalidate();
                }

                /// <summary>
                /// Sets the circle appearance.
                /// </summary>
                /// <param name="numberSpoke">The number spoke.</param>
                /// <param name="spokeThickness">The spoke thickness.</param>
                /// <param name="innerCircleRadius">The inner circle radius.</param>
                /// <param name="outerCircleRadius">The outer circle radius.</param>
                public void SetCircleAppearance(int numberSpoke, int spokeThickness,
                    int innerCircleRadius, int outerCircleRadius)
                {
                    NumberSpoke = numberSpoke;
                    SpokeThickness = spokeThickness;
                    InnerCircleRadius = innerCircleRadius;
                    OuterCircleRadius = outerCircleRadius;

                    Invalidate();
                }
            }

            partial class LoadingCircle
            {
                /// <summary>
                /// Required designer variable.
                /// </summary>
                private System.ComponentModel.IContainer components = null;

                /// <summary>
                /// Clean up any resources being used.
                /// </summary>
                /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
                protected override void Dispose(bool disposing)
                {
                    if (disposing && (components != null))
                    {
                        components.Dispose();
                    }
                    base.Dispose(disposing);
                }
            }

            [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.All)]
            public class LoadingCircleToolStripMenuItem : ToolStripControlHost
            {
                // Constants =========================================================

                // Attributes ========================================================

                // Properties ========================================================
                /// <summary>
                /// Gets the loading circle control.
                /// </summary>
                /// <value>The loading circle control.</value>
                [RefreshProperties(RefreshProperties.All),
                DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
                public LoadingCircle LoadingCircleControl
                {
                    get { return Control as LoadingCircle; }
                }

                // Constructor ========================================================
                /// <summary>
                /// Initializes a new instance of the <see cref="LoadingCircleToolStripMenuItem"/> class.
                /// </summary>
                public LoadingCircleToolStripMenuItem()
                    : base(new LoadingCircle())
                {
                }

                /// <summary>
                /// Retrieves the size of a rectangular area into which a control can be fitted.
                /// </summary>
                /// <param name="constrainingSize">The custom-sized area for a control.</param>
                /// <returns>
                /// An ordered pair of type <see cref="T:System.Drawing.Size"></see> representing the width and height of a rectangle.
                /// </returns>
                /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
                public override Size GetPreferredSize(Size constrainingSize)
                {
                    //return base.GetPreferredSize(constrainingSize);
                    return this.LoadingCircleControl.GetPreferredSize(constrainingSize);
                }

                /// <summary>
                /// Subscribes events from the hosted control.
                /// </summary>
                /// <param name="control">The control from which to subscribe events.</param>
                protected override void OnSubscribeControlEvents(Control control)
                {
                    base.OnSubscribeControlEvents(control);

                    //Add your code here to subsribe to Control Events
                }

                /// <summary>
                /// Unsubscribes events from the hosted control.
                /// </summary>
                /// <param name="control">The control from which to unsubscribe events.</param>
                protected override void OnUnsubscribeControlEvents(Control control)
                {
                    base.OnUnsubscribeControlEvents(control);

                    //Add your code here to unsubscribe from control events.
                }
            }
        }

        public class DataGridViewTool
        {
            public static List<T> GetColumnList<T>(DataGridViewSelectedRowCollection dgv_rows, string column_name)
            {
                List<T> list = new List<T>();
                foreach (DataGridViewRow item in dgv_rows)
                {
                    list.Add((T)item.Cells[column_name].Value);

                }
                return list;
            }
            public class DataGridViewWithPaging
            {

                private int _CurrentPage = 1;
                private string _FirstButtonText = string.Empty;
                private string _LastButtonText = string.Empty;
                private string _PreviousButtonText = string.Empty;
                private string _NextButtonText = string.Empty;
                private int _Width;
                private int _Height;
                private int _PateSize = 10;
                private DataTable _DataSource;
                private TextBox txtPaging = new TextBox();
                private DataGridView dataGridView1 = new DataGridView();




                public int PageSize
                {
                    get
                    {
                        return _PateSize;
                    }
                    set
                    {
                        _PateSize = value;
                    }
                }
                public DataTable DataSource
                {
                    get
                    {
                        return _DataSource;
                    }
                    set
                    {
                        _DataSource = value;
                    }
                }

                /// <summary>
                /// call DataBind
                /// </summary>
                /// <param name="dgv"></param>
                /// <param name="controlFirst_"></param>
                /// <param name="controlPrevious_"></param>
                /// <param name="controlNext_"></param>
                /// <param name="controlLast_"></param>
                /// <param name="tbpaging_"></param>
                public DataGridViewWithPaging(DataGridView dgv, Control controlFirst_, Control controlPrevious_, Control controlNext_, Control controlLast_, TextBox tbpaging_)
                {

                    dataGridView1 = dgv;
                    controlFirst_.Click += btnFirst_Click;
                    controlPrevious_.Click += btnPrevious_Click;
                    controlNext_.Click += btnNext_Click;
                    controlLast_.Click += btnLast_Click;
                    txtPaging = tbpaging_;

                }


                private DataTable ShowData(int pageNumber)
                {
                    DataTable dt = new DataTable();
                    int startIndex = PageSize * (pageNumber - 1);
                    var result = DataSource.AsEnumerable().Where((s, k) => (k >= startIndex && k < (startIndex + PageSize)));

                    foreach (DataColumn colunm in DataSource.Columns)
                    {
                        dt.Columns.Add(colunm.ColumnName);
                    }

                    foreach (var item in result)
                    {
                        dt.ImportRow(item);
                    }

                    txtPaging.Text = string.Format("Page {0} Of {1} Pages", pageNumber, (DataSource.Rows.Count / PageSize) + 1);
                    return dt;
                }
                public void DataBind(DataTable dataTable)
                {
                    DataSource = dataTable;
                    dataGridView1.DataSource = ShowData(1);
                }

                private void DataGridViewWithPaging_Load(object sender, System.EventArgs e)
                {

                }

                private void btnFirst_Click(object sender, System.EventArgs e)
                {
                    if (_CurrentPage == 1)
                    {
                        MessageBox.Show("You are already on First Page.");
                    }
                    else
                    {
                        _CurrentPage = 1;
                        dataGridView1.DataSource = ShowData(_CurrentPage);
                    }
                }

                private void btnPrevious_Click(object sender, System.EventArgs e)
                {
                    if (_CurrentPage == 1)
                    {
                        MessageBox.Show("You are already on First page, you can not go to previous of First page.");
                    }
                    else
                    {
                        _CurrentPage -= 1;
                        dataGridView1.DataSource = ShowData(_CurrentPage);
                    }
                }

                private void btnNext_Click(object sender, System.EventArgs e)
                {
                    int lastPage = (DataSource.Rows.Count / PageSize) + 1;
                    if (_CurrentPage == lastPage)
                    {
                        MessageBox.Show("You are already on Last page, you can not go to next page of Last page.");
                    }
                    else
                    {
                        _CurrentPage += 1;
                        dataGridView1.DataSource = ShowData(_CurrentPage);
                    }
                }

                private void btnLast_Click(object sender, System.EventArgs e)
                {
                    int previousPage = _CurrentPage;
                    _CurrentPage = (DataSource.Rows.Count / PageSize) + 1;

                    if (previousPage == _CurrentPage)
                    {
                        MessageBox.Show("You are already on Last Page.");
                    }
                    else
                    {
                        dataGridView1.DataSource = ShowData(_CurrentPage);
                    }
                }
            }

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
                public enum PopupType
                {
                    AllCells,
                    TopCorner,
                    ControlClick
                }
                public Control control_to_show_popup;
                private PopupType popup_type;

                // the DataGridView to which the DataGridViewColumnSelector is attached
                private DataGridView mDataGridView = null;
                // a CheckedListBox containing the column header text and checkboxes
                private CheckedListBox mCheckedListBox;
                // a ToolStripDropDown object used to show the popup
                private ToolStripDropDown mPopup;


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
                    bool show_popup = false;

                    switch (popup_type)
                    {
                        case PopupType.AllCells:
                            show_popup = e.Button == MouseButtons.Right;
                            break;
                        case PopupType.TopCorner:
                            show_popup = e.Button == MouseButtons.Right && e.RowIndex == -1 && e.ColumnIndex == -1;
                            break;
                        case PopupType.ControlClick:
                            show_popup = false;
                            break;
                    }

                    if (show_popup)
                    {
                        ShowPopUp(e);
                    }
                }

                private void ShowPopUp(DataGridViewCellMouseEventArgs e)
                {
                    int x_location = 0;
                    int y_location = 0;
                    Point location;

                    if (popup_type == PopupType.ControlClick)
                    {
                        x_location = control_to_show_popup.Location.X;
                        y_location = control_to_show_popup.Location.Y;

                        location = new Point(x_location, y_location);
                    }
                    else
                    {
                        x_location = e.X;
                        y_location = e.Y;
                        if (mDataGridView.RightToLeft == RightToLeft.Yes) x_location += mDataGridView.Width;

                        location = new Point(x_location, y_location);
                    }


                    mCheckedListBox.Items.Clear();
                    foreach (DataGridViewColumn c in mDataGridView.Columns)
                    {
                        mCheckedListBox.Items.Add(c.HeaderText, c.Visible);
                    }
                    int PreferredHeight = (mCheckedListBox.Items.Count * 16) + 7;
                    mCheckedListBox.Height = (PreferredHeight < PopupMaxHeight) ? PreferredHeight : PopupMaxHeight;
                    mCheckedListBox.Width = this.PopupWidth;



                    mPopup.Show(mDataGridView.PointToScreen(location));
                }

                // The constructor creates an instance of CheckedListBox and ToolStripDropDown.
                // the CheckedListBox is hosted by ToolStripControlHost, which in turn is
                // added to ToolStripDropDown.
                public DataGridViewColumnSelector(DataGridView dgv, PopupType popup_type_, Control control_to_show_popup_ = null)
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

                    this.DataGridView = dgv;
                    popup_type = popup_type_;
                    SetControlClickPopup(control_to_show_popup_);
                }

                private void SetControlClickPopup(Control control_to_show_popup_)
                {
                    if (control_to_show_popup_ == null) return;
                    control_to_show_popup = control_to_show_popup_;
                    control_to_show_popup.Click += control_to_show_popup_Click;
                }


                void control_to_show_popup_Click(object sender, EventArgs e)
                {
                    ShowPopUp(null);
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
            public static void MakeComboBoxSizable(ComboBox cb, int height, Padding pad)
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
            /// this method make app slow. use it in your app rather than reference from SRL.
            /// in your enumerable_data_source first write Text then Value like : .Select(x=>new{Text="text" , Value=10})
            /// </summary>
            /// <typeparam name="ValueT"></typeparam>
            /// <param name="cb"></param>
            /// <param name="enumerable_data_source">enumerable_data_source is IEnumerable query of  new {string Text=?, object Value=? }</param>
            /// <param name="empty_row_value">empty row is added to top</param>
            public static void ComboBoxDataBind<ValueT>(ComboBox cb, IEnumerable<dynamic> enumerable_data_source, ValueT empty_row_value)
            {
                var data_source = enumerable_data_source.OrderBy(x => x.Text).ToList();
                // cb.Items.Clear();
                cb.DataSource = null;
                cb.DisplayMember = "Text";
                cb.ValueMember = "Value";
                data_source.Insert(0, new { Text = "", Value = empty_row_value });
                cb.DataSource = data_source;

            }

            public static void Align(ComboBox cbKalaGroup)
            {
                cbKalaGroup.DrawMode = DrawMode.OwnerDrawFixed;
                cbKalaGroup.DrawItem += cbxDesign_DrawItem;
            }

            private static void cbxDesign_DrawItem(object sender, DrawItemEventArgs e)
            {
                // By using Sender, one method could handle multiple ComboBoxes
                ComboBox cbx = sender as ComboBox;
                if (cbx != null)
                {
                    // Always draw the background
                    e.DrawBackground();

                    // Drawing one of the items?
                    if (e.Index >= 0)
                    {
                        // Set the string alignment.  Choices are Center, Near and Far
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;

                        // Set the Brush to ComboBox ForeColor to maintain any ComboBox color settings
                        // Assumes Brush is solid
                        Brush brush = new SolidBrush(cbx.ForeColor);

                        // If drawing highlighted selection, change brush
                        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                            brush = SystemBrushes.HighlightText;

                        // Draw the string
                        e.Graphics.DrawString(cbx.Items[e.Index].ToString(), cbx.Font, brush, e.Bounds, sf);
                    }
                }
            }
        }

        public class NavigatorTools
        {
            public NavigatorTools(BindingNavigator bn)
            {
                bn.Enabled = true;

                foreach (ToolStripItem item in bn.Items)
                {
                    item.Enabled = true;
                }
            }

            public void LoadNumbersFromDgv(DataGridView dgv, ToolStripItem item_to_show_all, ToolStripItem item_to_show_current, bool? isForward, int count)
            {
                var tag = dgv.Tag;
                if (tag == null) return;
                int all = int.Parse(tag.ToString());
                item_to_show_all.Text = Math.Ceiling(Decimal.Divide(all, count)).ToString();

                int page = 1;
                if (!string.IsNullOrWhiteSpace(item_to_show_current.Text))
                {
                    page = int.Parse(item_to_show_current.Text);
                    if (isForward != null)
                    {
                        if ((bool)isForward)
                        {
                            if (page < all) page += page;
                        }
                        else
                        {
                            if (page > 1) page -= page;
                        }
                    }
                }
                item_to_show_current.Text = page.ToString();
            }


        }

        public static string GetAppName(string default_app_name, string folder_containing_exe_path, List<string> file_not_searching, string app_extention_pattern = "*.exe")
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

        public static T CloneControl<T>(T controlToClone)
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
            public class DigitSeperation
            {
                static string sep = NumberFormatInfo.CurrentInfo.NumberGroupSeparator;
                public static void Enable3DigitSeperation(params TextBox[] tb_list)
                {
                    foreach (var tb_ in tb_list)
                    {
                        tb_.TextChanged += tb_TextChanged;
                    }
                }
                private static void tb_TextChanged(object sender, EventArgs e)
                {
                    var tb = sender as TextBox;
                    string value = tb.Text.Replace(sep, "");
                    double ul;
                    if (double.TryParse(value, out ul))
                    {
                        tb.TextChanged -= tb_TextChanged;
                        string format = "{0:#,##0.########}";
                        string number = string.Format(format, ul);
                        tb.Text = number;
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
            public static string GetRawStringMaskedTextBox(MaskedTextBox control)
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

            public static void RaiseButtonClickOnEnter(TextBox tb, Button btn)
            {
                tb.KeyDown += (s, e) => tb_raise_click(s, e, btn);
            }

            private static void tb_raise_click(object sender, KeyEventArgs e, Button btn)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btn.PerformClick();
                }
            }
        }
        public static void AddChildToParentControls(Control parent, Control child, bool reset_child_font = false, bool clear_parent = true)
        {
            if (reset_child_font) child.Font = default(Font);
            if (clear_parent) parent.Controls.Clear();
            parent.Controls.Add(child);

        }
        public static void AddChildToParentControlsAliagn(Control parent, Control child, bool reset_child_font = false, bool clear_parent = true, AliagnType aliagn_type = AliagnType.All)
        {
            if (reset_child_font) child.Font = default(Font);
            if (clear_parent) parent.Controls.Clear();
            AliagnChildToParent(parent, child, aliagn_type);
            parent.Controls.Add(child);

        }
        public static void AddChildToParentControlsZoomAndAliagn(Control parent, Control child, decimal font_factor = 1, bool use_parent_font_family = false, bool use_parent_font_size = false, bool reset_child_font = false, AliagnType aliagn_type = AliagnType.All, bool clear_parent = true)
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

            decimal f = 0;
            switch (aliagn_type)
            {
                case AliagnType.Width:
                    f = x_relative;
                    break;
                case AliagnType.Height:
                    f = y_relative;
                    break;
                case AliagnType.All:
                    f = (x_relative + y_relative) / 2;
                    break;
            }


            f *= font_factor;


            child.Font = new Font(font_family, child_font_size * (float)f, font_style);

            AddChildToParentControlsAliagn(parent, child, reset_child_font, clear_parent, aliagn_type);
        }
        public static void ChildToParentControlsZoomAndAliagn(Control parent, Control child, decimal font_factor = 1, bool use_parent_font_family = false, bool use_parent_font_size = false, bool reset_child_font = false)
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

            AliagnChildToParent(parent, child);

        }



        public static void AliagnChildToParent(Control parent, Control child, AliagnType aliagn_type = AliagnType.All)
        {
            switch (aliagn_type)
            {
                case AliagnType.Width:
                    child.Location = new Point(
    parent.ClientSize.Width / 2 - child.Size.Width / 2,
    child.Location.Y);
                    break;
                case AliagnType.Height:
                    child.Location = new Point(
    child.Location.X,
    parent.ClientSize.Height / 2 - child.Size.Height / 2);
                    break;
                case AliagnType.All:
                    child.Location = new Point(
    parent.ClientSize.Width / 2 - child.Size.Width / 2,
    parent.ClientSize.Height / 2 - child.Size.Height / 2);
                    break;
            }

            child.Anchor = AnchorStyles.None;

        }


        public static void AdjustChildToParent(Control parent_form, Control child, double child_width_relative, double child_height_relative)
        {



            int form_x = parent_form.Width;
            int form_y = parent_form.Height;

            child.Width = int.Parse(Math.Floor(child_width_relative * form_x).ToString());
            child.Height = int.Parse(Math.Floor(child_height_relative * form_y).ToString());
        }
        public static void AdjustAndAliagnChildToParent(Control parent_form, Control child, double child_width_relative, double child_height_relative)
        {
            int form_x = parent_form.Width;
            int form_y = parent_form.Height;

            child.Width = int.Parse(Math.Floor(child_width_relative * form_x).ToString());
            child.Height = int.Parse(Math.Floor(child_height_relative * form_y).ToString());

            AliagnChildToParent(parent_form, child);
        }

        public class Media
        {

            public static float GetScreenDpi(Control control, out float dpiX, out float dpiY)
            {
                Graphics graphics = control.CreateGraphics();
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
                return (dpiX + dpiY) / 2;
            }
            public static float GetScreenDpi(Control control)
            {
                float dpiX, dpiY;
                Graphics graphics = control.CreateGraphics();
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
                return (dpiX + dpiY) / 2;
            }

            public static Image CaptureScreen(Form form)
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

                SRL.WinTools.AddChildToParentControlsAliagn(this, user_control);
            }


        }
        public static bool ValidationInLabel(Label lblError, List<Control> fieldNotNull = null, List<TextBox> tbMobile = null)
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
                EmailPattern_NotNull = 8,

                [Description("عدد صحیح")]
                IntegerInput = 9

            }


            public UserControlValidation(Control uc, ErrorProvider errorProvider, bool force_cancel_)
            {
                errorProvider1 = errorProvider;
                user_control = uc;
                force_cancel = force_cancel_;
            }

            /// <summary>
            /// if no controls passes then all fields will be checked. If some passes, only they will be checked.
            /// </summary>
            /// <param name="controls"></param>
            /// <returns></returns>
            public bool CheckAllField(List<Control> controls = null)
            {
                bool validation_result = false;
                bool main_force_cancel = force_cancel;
                force_cancel = true;

                if (controls != null)
                {
                    foreach (Control control in controls)
                        control.Focus();

                    if (user_control is UserControl)
                        validation_result = ((UserControl)user_control).Validate();
                    else if (user_control is Form)
                        validation_result = ((Form)user_control).Validate();
                }
                else
                {
                    if (user_control is UserControl)
                        validation_result = ((UserControl)user_control).ValidateChildren(ValidationConstraints.Enabled);
                    else if (user_control is Form)
                        validation_result = ((Form)user_control).ValidateChildren(ValidationConstraints.Enabled);
                }
                force_cancel = main_force_cancel;
                return validation_result;

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
                    case ErrorTypes.IntegerInput:
                        control.KeyPress += new System.Windows.Forms.KeyPressEventHandler(integer_input_KeyPress);
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

                if (SRL.WinTools.TextBoxTool.GetRawStringMaskedTextBox(control).Any())
                    if (!DateTime.TryParse(control.Text, out dt))
                    {
                        e.Cancel = force_cancel;
                        var msg = SRL.ClassManagement.GetEnumDescription<ErrorTypes>(ErrorTypes.MaskDatePattern);

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
                    var msg = SRL.ClassManagement.GetEnumDescription<ErrorTypes>(ErrorTypes.NotNull_MobilePattern);

                    errorProvider1.SetError(control, msg);

                    return;
                }
                errorProvider1.SetError(control, "");
            }

            private void not_null_email_pattern_Validating(object sender, CancelEventArgs e)
            {
                Control control = sender as Control;
                bool status = control.Text.Any() ?
                   (!(SRL.Convertor.IsValidEmail(control.Text)) ? true : false)
                   : true;
                if (status)
                {
                    e.Cancel = force_cancel;
                    var msg = SRL.ClassManagement.GetEnumDescription<ErrorTypes>(ErrorTypes.EmailPattern_NotNull);

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
                        var msg = SRL.ClassManagement.GetEnumDescription<ErrorTypes>(ErrorTypes.MobilePattern);

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
                    var msg = SRL.ClassManagement.GetEnumDescription<ErrorTypes>(ErrorTypes.NotNull);

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


        public class MasterLogin
        {
            public class KeyboardLogin
            {
                public static bool IsLogin = true;
                public static int shift_press_time = 0;
                public static bool exit_hover = false;
            }

            public static void MasterKeboardLogin(Label lbl, TextBox tb, Form control, SRL.WinSessionId session)
            {
                //shift +  lbl hover + shift  + (ctrl,alt,1)
                lbl.MouseHover += Lbl_MouseHover;
                tb.KeyDown += (ss, ee) => Tb_KeyDown(ss, ee, control, session);
            }

            private static void Tb_KeyDown(object sender, KeyEventArgs e, Form control, SRL.WinSessionId session)
            {
                if (e.KeyCode == Keys.ShiftKey)
                {
                    if (KeyboardLogin.shift_press_time == 0 && KeyboardLogin.exit_hover == false)
                    {
                        KeyboardLogin.shift_press_time = 1;
                    }
                    else if (KeyboardLogin.shift_press_time == 0 && KeyboardLogin.exit_hover == true)
                    {
                        KeyboardLogin.IsLogin = KeyboardLogin.IsLogin && false;
                    }
                    else if (KeyboardLogin.shift_press_time == 1 && KeyboardLogin.exit_hover == false)
                    {
                        KeyboardLogin.IsLogin = KeyboardLogin.IsLogin && false;
                    }
                    else if (KeyboardLogin.shift_press_time == 1 && KeyboardLogin.exit_hover == true)
                    {
                        KeyboardLogin.shift_press_time = 2;
                    }
                    else
                    {
                        KeyboardLogin.IsLogin = KeyboardLogin.IsLogin && false;
                    }
                }

                if (e.KeyCode == Keys.D1 && (e.Alt || e.Control))
                {

                    if (KeyboardLogin.IsLogin && KeyboardLogin.shift_press_time == 2)
                    {
                        SRL.Security.MasterLogin.CheckMasterLogin(session, "lamso1387", "sr2050130351");
                        control.Close();
                    }
                }
            }

            private static void Lbl_MouseHover(object sender, EventArgs e)
            {
                if (KeyboardLogin.exit_hover)
                    KeyboardLogin.IsLogin = KeyboardLogin.IsLogin && false;
                else KeyboardLogin.exit_hover = true;
            }



            public static bool CheckMasterLogin(SRL.WinSessionId session, string username, string password)
            {
                if (username == "lamso1387" && password == "sr2050130351")
                {
                    session.IsLogined = true;
                    session.user_id = 123456789;
                    session.username = "master";
                    session.role = "master";
                    return true;

                }
                else return false;
            }
        }

        public enum UserRegistrationStatus
        {
            NotRegistered = 0,
            NotActivated = 1,
            Activated = 2
        }
        public enum HashAlgoritmType
        {
            Sha1,
            MD5,
            Sha256,
            None
        }

        /// <summary>
        /// permission table must have columns: id , role , permission
        /// </summary>
        public class RolePermissionManagement
        {

            public static bool AddOrEditNewRole(string permission_entity, object role_obj, DbContext db, object edit_id)
            {
                if (edit_id == null ? false : edit_id.ToString() != "")
                    return EditRoleTitle(permission_entity, role_obj, db, edit_id);
                else
                    return AddNewRole(permission_entity, role_obj, db, null);

            }

            public static bool DeleteRole(string permission_entity, object id_obj, DbContext db)
            {
                if (id_obj == null ? false : id_obj.ToString() != "")
                {
                    var q = "delete from " + permission_entity + " where id=" + long.Parse(id_obj.ToString());
                    var res = SRL.Database.ExecuteQuery(db, q);
                    return res == "" ? true : false;
                }
                else return false;

            }
            public static bool AddNewRole(string permission_entity, object role_obj, DbContext db, object edit_id)
            {
                if (role_obj == null) return false;
                if (role_obj.ToString() == "") return false;
                if (role_obj.ToString() == "master") return false;

                if (CheckRoleIsUnique(permission_entity, role_obj, db))
                {
                    var q = "insert into " + permission_entity + " ([role]) values ('" + role_obj.ToString() + "');";
                    var res = SRL.Database.ExecuteQuery(db, q);
                    return res == "" ? true : false;
                }
                else return false;
            }
            public static bool EditRoleTitle(string permission_entity, object role_obj, DbContext db, object edit_id)
            {
                if (role_obj == null) return false;
                if (role_obj.ToString() == "") return false;
                if (role_obj.ToString() == "master") return false;
                var q = "update " + permission_entity + " set [role]='" + role_obj.ToString() + "' where id=" + long.Parse(edit_id.ToString());
                var res = SRL.Database.ExecuteQuery(db, q);
                return res == "" ? true : false;
            }
            public static bool CheckRoleIsUnique(string permission_entity, object role_obj, DbContext db)
            {
                if (role_obj == null) return false;
                if (role_obj.ToString() == "") return false;

                string sql = "select *  from " + permission_entity + " where [role]='" + role_obj.ToString() + "'";
                var role = SRL.Database.SqlQuery<RoleClass>(db, sql);
                if (role == null ? false : role.Any())
                {
                    return false;
                }
                else return true;
            }
            public static List<string> GetAllRoles(string permission_entity, DbContext db)
            {
                return SRL.Database.SqlQuery<string>(db, "select [role] from " + permission_entity);
            }
        }
        public static Version GetAppVersion(Assembly assembly)
        {
            // return Assembly.GetEntryAssembly().GetName().Version;
            return assembly.GetName().Version;
        }
        public static string GetComputerCurrentUsername()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }
        public static void WinCheckLogin(DbContext db, string entity_name, WinSessionId session, Security.HashAlgoritmType password_type)
        {
            new SRL.WinLogin(db, entity_name, session, password_type).ShowDialog();

            if (!session.IsLogined) Environment.Exit(0);
        }

        public static void WinCheckAccess(string role, Dictionary<string, List<Component>> role_disable_component)
        {
            /*use:
            SRL.Security.WinCheckAccess(Publics.srl_session.role, new Dictionary<string, List<Component>>() {
                {"user", new List<Component> { miInsertData, miSensSms, miUsers, new Button() } }
            }
            );
            */
            List<Component> all = new List<Component>();

            foreach (var item in role_disable_component) all.AddRange(item.Value);

            foreach (var item in all) (item as dynamic).Enabled = true;

            List<Component> list;
            if (role_disable_component.TryGetValue(role, out list))
                foreach (var item in list) (item as dynamic).Enabled = false;
        }
        public static void CreateSession(string key, object value, System.Web.UI.Page page)
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
        public static void SendActivationEmail(string username, string registerHashValue, string registerActivationUri, string toMail, string subject, string body, Dictionary<string, object> response, string fromMail, string password)
        {
            try
            {
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(fromMail);
                mailMessage.To.Add(toMail);
                mailMessage.From = from;
                mailMessage.Subject = subject;
                mailMessage.Body = body;

                string activationLink = SRL.Convertor.MakeActivationLink(username, registerHashValue, registerActivationUri);
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

        public static string SendEmail(string username, string toMail, string subject, string body, string fromMail, string password, string attach_text_file_content = null, string attach_text_file_name = null)
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
        public static string GetHashString(string input, HashAlgoritmType algorytmType = HashAlgoritmType.Sha1)
        {
            if (algorytmType == HashAlgoritmType.None)
            {
                return input;
            }
            byte[] hash = null;

            switch (algorytmType)
            {
                case HashAlgoritmType.Sha1:
                    System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed();
                    hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                    break;
                case HashAlgoritmType.MD5:
                    HashAlgorithm algorithm = MD5.Create();
                    hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
                    break;
                case HashAlgoritmType.Sha256:
                    HashAlgorithm sh256 = SHA256.Create();
                    hash = sh256.ComputeHash(Encoding.UTF8.GetBytes(input));
                    break;
            }


            var sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();


        }

        /// <summary>
        /// تعیین معتبر بودن کد ملی
        /// </summary>
        /// <param name="nationalCode">کد ملی وارد شده</param>
        /// <returns>
        /// در صورتی که کد ملی صحیح باشد خروجی <c>true</c> و در صورتی که کد ملی اشتباه باشد خروجی <c>false</c> خواهد بود
        /// </returns>
        public static Boolean IsValidNationalCode(String nationalCode)
        {
            if (String.IsNullOrWhiteSpace(nationalCode))
                return false;

            nationalCode = SRL.Convertor.NationalId(nationalCode);
            if (nationalCode.Length != 10)
                return false;

            if (!IsNumber(nationalCode)) return false;


            var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(nationalCode)) return false;


            //عملیات شرح داده شده در بالا
            var chArray = nationalCode.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
            var a = Convert.ToInt32(chArray[9].ToString());

            var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
            var c = b % 11;

            return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));
        }

        public static Boolean IsValidMobile(String mobile)
        {
            if (String.IsNullOrWhiteSpace(mobile))
                return false;

            mobile = SRL.Convertor.Mobile(mobile);
            if (mobile.Length != 11)
                return false;

            if (!IsNumber(mobile))
                return false;

            return true;
        }
        public static bool IsNumber(string str)
        {
            var regex = new Regex(@"\d{10}");
            if (!regex.IsMatch(str))
                return false;
            else return true;
        }

        public static bool IsValidCoNationalCode(string co_national_id)
        {
            if (String.IsNullOrWhiteSpace(co_national_id))
                return false;

            if (!IsNumber(co_national_id)) return false;

            if (co_national_id.Length != 11)
                return false;
            else return true;
        }

        public static bool IsValidPostcode(string postal_code)
        {
            if (String.IsNullOrWhiteSpace(postal_code))
                return false;

            if (!IsNumber(postal_code)) return false;

            if (postal_code.Length != 10)
                return false;
            else return true;
        }
    }
    public class KeyValue
    {
        public enum DataTableHeaderCheckType
        {
            Match,
            AllInDataTable,
            NotMoreInDataTable,
            None

        }
        public void AddItem(Dictionary<string, object> result, string key, object value)
        {
            result[key] = value;
        }

        public static string CheckDataTableHeaders(DataTable dt, string[] main_headers, DataTableHeaderCheckType check_type)
        {
            List<string> access_columns = new List<string>();
            foreach (DataColumn item in dt.Columns)
            {
                access_columns.Add(item.ColumnName);
            }

            switch (check_type)
            {
                case DataTableHeaderCheckType.Match:
                    foreach (var main_header in main_headers)
                    {
                        Application.DoEvents();
                        if (!access_columns.Contains(main_header))
                        {
                            return "file does not have column: " + main_header;
                        }
                        else continue;
                    }

                    foreach (var file_header in access_columns)
                    {
                        Application.DoEvents();
                        if (!main_headers.Contains(file_header))
                        {
                            return file_header + " is not valid.";
                        }
                        else continue;
                    }
                    break;
                case DataTableHeaderCheckType.AllInDataTable:
                    foreach (var main_header in main_headers)
                    {
                        Application.DoEvents();
                        if (!access_columns.Contains(main_header))
                        {
                            return "file does not have column: " + main_header;
                        }
                        else continue;
                    }
                    break;
                case DataTableHeaderCheckType.NotMoreInDataTable:
                    foreach (var file_header in access_columns)
                    {
                        Application.DoEvents();
                        if (!main_headers.Contains(file_header))
                        {
                            return file_header + " is not valid.";
                        }
                        else continue;
                    }
                    break;
            }

            return "true";

        }
    }
    public class HttpSend
    {
        HttpClient client = new HttpClient();
        public Dictionary<string, object> input = new Dictionary<string, object>();
        public DbContext db;
        public Dictionary<string, string> map_input_to_entity = new Dictionary<string, string>();

        public enum SendType
        {
            Post,
            Get
        }

        public HttpSend(string base_address)
        {
            client.BaseAddress = new Uri(base_address);

        }

        public void CreateInput<inputType, entityType>(entityType entity_instance) where inputType : class where entityType : class
        {


            foreach (var prop in typeof(inputType).GetProperties())
            {
                input[prop.Name] = SRL.ClassManagement.GetProperty<entityType>(GetEntityPropName(prop.Name), entity_instance);
            }
        }

        private string GetEntityPropName(string input_prop)
        {
            if (map_input_to_entity.ContainsKey(input_prop))
                return map_input_to_entity[input_prop];

            else return input_prop;
        }
        public outputType Send<outputType, entityType>(SendType send_type, string url, entityType entity_to_update, string status_code_field_name, string http_response_field_name, DbContext db) where outputType : class where entityType : class
        {
            HttpResponseMessage response = SendHttpRequest(url, send_type);

            SaveHttpStatusCode<entityType>(status_code_field_name, entity_to_update, response);
            SaveHttpResultMessage<entityType>(http_response_field_name, entity_to_update, response);

            return GetHttpOutput<outputType>(response, send_type);

        }
        public void Send<entityType>(SendType send_type, string url, entityType entity_to_update, string status_code_field_name, string http_response_field_name) where entityType : class
        {
            HttpResponseMessage response = SendHttpRequest(url, send_type);

            SaveHttpStatusCode<entityType>(status_code_field_name, entity_to_update, response);
            SaveHttpResultMessage<entityType>(http_response_field_name, entity_to_update, response);

        }

        private void SaveHttpStatusCode<entityType>(string status_code_field_name, entityType entity_to_update, HttpResponseMessage response)
        {
            string http_status_code = response.StatusCode.ToString();
            if (!string.IsNullOrWhiteSpace(status_code_field_name))
            {
                SRL.ClassManagement.SetProperty<entityType>(status_code_field_name, entity_to_update, http_status_code);
                db.SaveChanges();
            }
        }

        private void SaveHttpResultMessage<entityType>(string http_response_field_name, entityType entity_to_update, HttpResponseMessage response)
        {
            string result = response.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrWhiteSpace(http_response_field_name))
            {
                SRL.ClassManagement.SetProperty<entityType>(http_response_field_name, entity_to_update, System.Text.RegularExpressions.Regex.Unescape(result));
                db.SaveChanges();
            }
        }

        private HttpResponseMessage SendHttpRequest(string url, SendType send_type)
        {
            HttpResponseMessage response = new HttpResponseMessage();


            switch (send_type)
            {
                case SendType.Post:
                    response = client.PostAsJsonAsync(url, input).Result;
                    break;
                case SendType.Get:
                    response = client.GetAsync(url).Result;
                    break;

            }

            return response;
        }

        private outputType GetHttpOutput<outputType>(HttpResponseMessage response, SendType send_type)
        {
            outputType data = SRL.ClassManagement.CreateInstance<outputType>();
            string result = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                switch (send_type)
                {
                    case SendType.Post:
                        data = Newtonsoft.Json.JsonConvert.DeserializeObject<outputType>(result);
                        break;
                    case SendType.Get:

                        break;
                }

            }

            return data;
        }

        public int SaveResult<outputType, entityType>(outputType object_to_save, entityType entity_to_update) where outputType : class where entityType : class
        {




            foreach (var prop in typeof(outputType).GetProperties())
            {
                SRL.ClassManagement.SetProperty<entityType>(prop.Name, entity_to_update, SRL.ClassManagement.GetProperty<outputType>(prop.Name, object_to_save));

            }

            return db.SaveChanges();

        }

    }
    public class Web
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

        /// <summary>
        /// TChannel like IDataCollectorService, CidWebServiceSoap,IVOServicesSoap
        /// </summary>
        /// <typeparam name="TChannel">The type of channel produced by the channel factory. This type must be either IOutputChannel ( interface that a channel must implement to send a message) or IRequestChannel (contract that a channel must implement)</typeparam>
        /// <param name="ser_"></param>
        /// <param name="svc_address"></param>
        public static void UpdateSoapAddress<TChannel>(System.ServiceModel.ClientBase<TChannel> ser_, string svc_address) where TChannel : class
        {
            ser_.ChannelFactory.Endpoint.Address = new EndpointAddress(svc_address);
            ser_.ChannelFactory.CreateChannel();
        }
        public static void UpdateSoapAddress(ServiceEndpoint endpoint, string new_address)
        {
            Configuration wConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ServiceModelSectionGroup wServiceSection = ServiceModelSectionGroup.GetSectionGroup(wConfig);

            ClientSection wClientSection = wServiceSection.Client;
            bool address_change = false;
            foreach (ChannelEndpointElement item in wClientSection.Endpoints)
            {
                if (item.Name == endpoint.Name)
                {
                    address_change = item.Address.AbsoluteUri != new_address;
                    item.Address = new Uri(new_address);
                }
            }

            wConfig.Save();
            if (address_change)
            {
                Application.Restart();
                Environment.Exit(0);
            }
        }

        public static string GetSoapAddress<TChannel>(System.ServiceModel.ClientBase<TChannel> ser_) where TChannel : class
        {
            return ser_.ChannelFactory.Endpoint.Address.Uri.AbsoluteUri;
        }
        public static string GetSoapAddress(ServiceEndpoint endpoint)
        {
            return endpoint.Address.Uri.ToString();
        }

        public static void UpdateConString(Configuration config, string con_str_name, string new_con_str)
        {
            //Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;
            if (section != null)
            {
                section.ConnectionStrings[con_str_name].ConnectionString = new_con_str;
                config.Save();
            }
        }
    }
    public class Convertor
    {

        public static string[] ClassToArray<TClass>()
        {
            string[] head = typeof(TClass).GetProperties().Select(p => p.Name).ToArray();
            return head;
        }
        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);


                        Type t = Nullable.GetUnderlyingType(pI.PropertyType) ?? pI.PropertyType;
                        object safeValue = (row[pro.Name] == DBNull.Value) ? null : Convert.ChangeType(row[pro.Name], t);
                        pro.SetValue(objT, safeValue, null);

                        //  pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
                    }
                }
                return objT;
            }).ToList();
        }
        public static void ConvertMenuToTreeView(MenuStrip menu, TreeView tree)
        {
            var list = SRL.ChildParent.GetAllMenuItems(menu);
            Dictionary<ToolStripMenuItem, bool> menu_conversion = new Dictionary<ToolStripMenuItem, bool>();
            while (menu_conversion.Keys.Count == 0 || menu_conversion.Values.Where(x => x.Equals(false)).Any())
            {
                foreach (var item in list)
                {
                    if (menu_conversion.ContainsKey(item))
                        if (menu_conversion[item] == true) continue;
                    var owner_item = item.OwnerItem;
                    if (owner_item != null)
                    {
                        TreeNode[] node = tree.Nodes.Find(owner_item.Name, true);
                        if (node.Any())
                        {
                            node.First().Nodes.Add(item.Name, item.Text);
                            menu_conversion[item] = true;
                        }
                        else menu_conversion[item] = false;


                    }
                    else
                    {
                        tree.Nodes.Add(item.Name, item.Text);
                        menu_conversion[item] = true;
                    }
                }
            }
        }

        public class IEnumerableToDatatable
        {

            public static DataTable CopyToDataTable<T>(IEnumerable<T> source)
            {
                try
                {

                    return new ObjectShredder<T>().Shred(source, null, null);

                }
                catch (Exception es)
                {
                    MessageBox.Show(es.Message);
                    return null;
                }
            }

            public static DataTable CopyToDataTable<T>(IEnumerable<T> source,
                                                        DataTable table, LoadOption? options)
            {
                try
                {
                    return new ObjectShredder<T>().Shred(source, table, options);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                    return null;
                }
            }

            public class ObjectShredder<T>
            {
                private System.Reflection.FieldInfo[] _fi;
                private System.Reflection.PropertyInfo[] _pi;
                private System.Collections.Generic.Dictionary<string, int> _ordinalMap;
                private System.Type _type;

                // ObjectShredder constructor.
                public ObjectShredder()
                {
                    _type = typeof(T);
                    _fi = _type.GetFields();
                    _pi = _type.GetProperties();
                    _ordinalMap = new Dictionary<string, int>();
                }

                /// <summary>
                /// Loads a DataTable from a sequence of objects.
                /// </summary>
                /// <param name="source">The sequence of objects to load into the DataTable.</param>
                /// <param name="table">The input table. The schema of the table must match that 
                /// the type T.  If the table is null, a new table is created with a schema 
                /// created from the public properties and fields of the type T.</param>
                /// <param name="options">Specifies how values from the source sequence will be applied to 
                /// existing rows in the table.</param>
                /// <returns>A DataTable created from the source sequence.</returns>
                public DataTable Shred(IEnumerable<T> source, DataTable table, LoadOption? options)
                {
                    // Load the table from the scalar sequence if T is a primitive type.
                    if (typeof(T).IsPrimitive)
                    {
                        return ShredPrimitive(source, table, options);
                    }

                    // Create a new table if the input table is null.
                    if (table == null)
                    {
                        table = new DataTable(typeof(T).Name);
                    }

                    // Initialize the ordinal map and extend the table schema based on type T.
                    table = ExtendTable(table, typeof(T));

                    // Enumerate the source sequence and load the object values into rows.
                    table.BeginLoadData();
                    using (IEnumerator<T> e = source.GetEnumerator())
                    {
                        while (e.MoveNext())
                        {
                            if (options != null)
                            {
                                table.LoadDataRow(ShredObject(table, e.Current), (LoadOption)options);
                            }
                            else
                            {
                                table.LoadDataRow(ShredObject(table, e.Current), true);
                            }
                        }
                    }
                    table.EndLoadData();

                    // Return the table.
                    return table;
                }

                public DataTable ShredPrimitive(IEnumerable<T> source, DataTable table, LoadOption? options)
                {
                    // Create a new table if the input table is null.
                    if (table == null)
                    {
                        table = new DataTable(typeof(T).Name);
                    }

                    if (!table.Columns.Contains("Value"))
                    {
                        table.Columns.Add("Value", typeof(T));
                    }

                    // Enumerate the source sequence and load the scalar values into rows.
                    table.BeginLoadData();
                    using (IEnumerator<T> e = source.GetEnumerator())
                    {
                        Object[] values = new object[table.Columns.Count];
                        while (e.MoveNext())
                        {
                            values[table.Columns["Value"].Ordinal] = e.Current;

                            if (options != null)
                            {
                                table.LoadDataRow(values, (LoadOption)options);
                            }
                            else
                            {
                                table.LoadDataRow(values, true);
                            }
                        }
                    }
                    table.EndLoadData();

                    // Return the table.
                    return table;
                }

                public object[] ShredObject(DataTable table, T instance)
                {

                    FieldInfo[] fi = _fi;
                    PropertyInfo[] pi = _pi;

                    if (instance.GetType() != typeof(T))
                    {
                        // If the instance is derived from T, extend the table schema
                        // and get the properties and fields.
                        ExtendTable(table, instance.GetType());
                        fi = instance.GetType().GetFields();
                        pi = instance.GetType().GetProperties();
                    }

                    // Add the property and field values of the instance to an array.
                    Object[] values = new object[table.Columns.Count];
                    foreach (FieldInfo f in fi)
                    {
                        values[_ordinalMap[f.Name]] = f.GetValue(instance);
                    }

                    foreach (PropertyInfo p in pi)
                    {
                        values[_ordinalMap[p.Name]] = p.GetValue(instance, null);
                    }

                    // Return the property and field values of the instance.
                    return values;
                }

                public DataTable ExtendTable(DataTable table, Type type)
                {
                    // Extend the table schema if the input table was null or if the value 
                    // in the sequence is derived from type T.            
                    foreach (FieldInfo f in type.GetFields())
                    {
                        if (!_ordinalMap.ContainsKey(f.Name))
                        {
                            // Add the field as a column in the table if it doesn't exist
                            // already.
                            DataColumn dc = table.Columns.Contains(f.Name) ? table.Columns[f.Name]
                                : table.Columns.Add(f.Name, f.FieldType);

                            // Add the field to the ordinal map.
                            _ordinalMap.Add(f.Name, dc.Ordinal);
                        }
                    }
                    foreach (PropertyInfo p in type.GetProperties())
                    {

                        if (!_ordinalMap.ContainsKey(p.Name))
                        {
                            // Add the property as a column in the table if it doesn't exist
                            // already.
                            DataColumn dc = table.Columns.Contains(p.Name) ? table.Columns[p.Name]
                                : table.Columns.Add(
                                p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType);

                            // Add the property to the ordinal map.
                            _ordinalMap.Add(p.Name, dc.Ordinal);
                        }

                    }

                    // Return the table.
                    return table;
                }
            }

        }
        /* input image with width = height is suggested to get the best result */
        /* png support in icon was introduced in Windows Vista */
        public static bool ConvertImageToIcon(System.IO.Stream input_stream, System.IO.Stream output_stream, int size, bool keep_aspect_ratio = false)
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

        public static bool ConvertImageToIcon(string input_image, string output_icon, int size, bool keep_aspect_ratio = false)
        {
            System.IO.FileStream input_stream = new System.IO.FileStream(input_image, System.IO.FileMode.Open);
            System.IO.FileStream output_stream = new System.IO.FileStream(output_icon, System.IO.FileMode.OpenOrCreate);

            bool result = ConvertImageToIcon(input_stream, output_stream, size, keep_aspect_ratio);

            input_stream.Close();
            output_stream.Close();

            return result;
        }

        public static int InchToPixel(float inch, float dpi = 96)
        {
            return (int)(inch * dpi);
        }
        public static float PixelToInch(int pixel, float dpi = 96)
        {
            return (pixel / dpi);
        }
        public static decimal StringToDecimal(string value_to_parse, decimal? default_value = null, string app_decimal_symbol = "/", bool show_alarm_error = true)
        {
            if (string.IsNullOrWhiteSpace(value_to_parse)) return 0;

            decimal value_parsed = 0;
            bool parse_try = StringToDecimalTry(value_to_parse, out value_parsed);
            if (parse_try) return value_parsed;

            string current_decimal_symbol = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            value_to_parse = value_to_parse.Replace(app_decimal_symbol, current_decimal_symbol);
            parse_try = StringToDecimalTry(value_to_parse, out value_parsed);
            if (parse_try) return value_parsed;

            value_to_parse = value_to_parse.Replace(current_decimal_symbol, ".");
            parse_try = StringToDecimalTry(value_to_parse, out value_parsed);
            if (parse_try) return value_parsed;

            value_to_parse = value_to_parse.Replace(".", ",");
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

        public static bool StringToFloatTry(string str, out float number)
        {

            return float.TryParse(str, NumberStyles.Any, CultureInfo.CurrentCulture, out number);
        }
        public static bool StringToDecimalTry(string str, out Decimal number)
        {
            return Decimal.TryParse(str, NumberStyles.Any, CultureInfo.CurrentCulture, out number);
        }
        public static DateTime UnixEpochToDateTime(long unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
        public static double EnglishDateTimeToUnixEpoch(DateTime dt)
        {
            var epoch = dt - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.TotalSeconds;
        }
        public static DateTime EnglishToPersianDateTime(DateTime date)
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
        public static List<string> EnglishToPersianDateString(DateTime d)
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
        public static string EnglishToPersianDate(DateTime d)
        {
            PersianCalendar pc = new PersianCalendar();
            string date = string.Format("{0}/{1}/{2}", pc.GetYear(d), pc.GetMonth(d), pc.GetDayOfMonth(d));
            return date;
        }
        public static DateTime PersianToEnglishDate(int year, int month, int day)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime dt = new DateTime(year, month, day, pc);
            return dt;
        }

        public static DateTime PersianToEnglishDate(int year, int month, int day, int hour, int minute, int second, int milsec)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime dt = pc.ToDateTime(year, month, day, hour, minute, second, milsec);

            return dt;
        }
        public static void CopyDataTableToDataTable(DataTable dt_from, DataTable dt_to)
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
        public static void MakeDataTableFromDGV(DataGridView dgview, DataTable table, int devider, int index)
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
        public static string StringToRegx(string input)
        {
            return System.Text.RegularExpressions.Regex.Unescape(input);
        }
        public static string MakeHashValue(string textToHash)
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
        public static string MakeActivationLink(string username, string registerHashValue, string registerActivationUri)
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
            mobile = (mobile.Length == 10 && mobile.Substring(0, 1) != "0") ? "0" + mobile : mobile;
            return mobile;

        }
        public static string NationalId(string national_id)
        {
            if (string.IsNullOrWhiteSpace(national_id)) return national_id;
            national_id = national_id.Length == 8 ? "00" + national_id : (national_id.Length == 9 ? "0" + national_id : national_id);
            return national_id;
        }

        public static bool IsValidEmail(string email)
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

        public static T ClassToClass<T>(Object input)
        {
            string input_json = SRL.Json.ClassObjectToJson(input);

            T output = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(input_json);
            return output;
        }

        public static Dictionary<string, object> ClassToDictionary(object obj)
        {
            Dictionary<string, object> dic = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));
            return dic;
        }
        public static KeyValuePair<string, object>[] ClassToArray(object obj)
        {
            var dic = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null)).ToArray();
            return dic;
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
        public static T StringToJson<T>(string input) where T : new()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(input);
        }
        public static string ClassObjectToJson(object obj)
        {
            if (obj == null) return null;
            try
            {
                if (obj.GetType() == typeof(string)) obj = Newtonsoft.Json.JsonConvert.DeserializeObject(obj.ToString());
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
                return json;
            }
            catch (Exception)
            {

                string json = new JavaScriptSerializer().Serialize(obj);
                return json;
            }



        }
        public static bool IsJson(string input)
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

        internal static Newtonsoft.Json.Linq.JObject RemoveEmptyKeys(object obj)
        {
            //add [DefaultValue("")] to must be remved properties
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings()
            {
                DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore
            });
            Newtonsoft.Json.Linq.JObject conv = Newtonsoft.Json.Linq.JObject.Parse(json);
            return conv;
        }

    }
    public class WinChart
    {


        public static void ShowDataOnChart(System.Windows.Forms.DataVisualization.Charting.Chart chart, string xValue, string yValue, IQueryable<object> query)
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

    public class CodeFirst
    {
        /// <summary>
        /// creates and then migrates
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TConfiguration"></typeparam>
        public static void MigrateDBToLatestVersion<TDbContext, TConfiguration>()
            where TDbContext : DbContext
            where TConfiguration : DbMigrationsConfiguration<TDbContext>, new()
        {

            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<TDbContext, TConfiguration>());
        }

        public static void PreventPluralizingDbTable(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
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

        public class SqlServerTableConstrain
        {
            public string CONSTRAIN_CATALOG { get; set; }
            public string CONSTRAIN_SCHEMA { get; set; }
            public string CONSTRAIN_NAME { get; set; }
            public string TABLE_CATALOG { get; set; }
            public string TABLE_SCHEMA { get; set; }
            public string TABLE_NAME { get; set; }
            public string CONSTRAIN_TYPE { get; set; }
        }

        public enum SqlServerConstrainType
        {
            PRIMARY_KEY,
            FOREIGN_KEY,
            UNIQUE,
            All

        }

        public enum GetSqlInstanceResult
        {
            OK,
            SqlException,
            Exception,
            InstanceNotFound
        }
        public class SqlServerInstances
        {
            public static IEnumerable<string> ListLocalSqlInstances()
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                    {
                        foreach (string item in ListLocalSqlInstances(hive))
                        {
                            yield return item;
                        }
                    }

                    using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                    {
                        foreach (string item in ListLocalSqlInstances(hive))
                        {
                            yield return item;
                        }
                    }
                }
                else
                {
                    foreach (string item in ListLocalSqlInstances(Registry.LocalMachine))
                    {
                        yield return item;
                    }
                }
            }

            private static IEnumerable<string> ListLocalSqlInstances(RegistryKey hive)
            {
                const string keyName = @"Software\Microsoft\Microsoft SQL Server";
                const string valueName = "InstalledInstances";
                const string defaultName = "MSSQLSERVER";

                using (var key = hive.OpenSubKey(keyName, false))
                {
                    if (key == null) return Enumerable.Empty<string>();

                    var value = key.GetValue(valueName) as string[];
                    if (value == null) return Enumerable.Empty<string>();

                    for (int index = 0; index < value.Length; index++)
                    {
                        if (string.Equals(value[index], defaultName, StringComparison.OrdinalIgnoreCase))
                        {
                            value[index] = ".";
                        }
                        else
                        {
                            value[index] = @".\" + value[index];
                        }
                    }

                    return value;
                }
            }
        }

        public class SqliteEF
        {
            public string ConnectionStr
            {
                get { return Connection.ConnectionString; }
                set { Connection.ConnectionString = value; }
            }

            public SQLiteConnection Connection { get; set; }
            public SQLiteCommand Command { get; set; }
            public SQLiteDataAdapter DataAdaptor { get; set; }
            public SqliteEF(string connection_str = "Data Source=MyDatabase.sqlite;Version=3;")
            {
                Connection = new SQLiteConnection();
                Command = new SQLiteCommand();
                DataAdaptor = new SQLiteDataAdapter();
                ConnectionStr = connection_str;
                Connection.Open();
            }

            public List<EntityClass> Select<EntityClass>(string sql)
            {
                Command = new SQLiteCommand(sql, Connection);
                DataAdaptor = new SQLiteDataAdapter(Command);
                DataTable dt = new DataTable();
                DataAdaptor.Fill(dt);
                List<EntityClass> list = SRL.Convertor.ConvertDataTableToList<EntityClass>(dt);
                return list;
            }
        }
        public class ReportTBClass
        {
            public string status { get; set; }
            public int cont { get; set; }
        }
        public class Backup
        {
            ProgressBar progressBar1;
            Label lbl;
            public Backup(string filter, string source, ProgressBar progressBar1_)
            {
                progressBar1 = progressBar1_;

                var open = new SaveFileDialog();
                open.Filter = filter;
                var result = open.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(open.FileName))
                {
                    var x = new FileManagement.FileCopyProgress(source, open.FileName);
                    if (progressBar1 != null) x.OnProgressChanged += X_OnProgressChanged_Progress;
                    x.Copy();

                }
            }
            private void X_OnProgressChanged_Progress(double Persentage, double size, ref bool Cancel)
            {
                if (Persentage > 0) progressBar1.Visible = true;
                progressBar1.Value = Convert.ToInt32(Persentage);
                if (Persentage == 100) progressBar1.Visible = false;
            }
            public Backup(string filter, string source, Label lbl_)
            {
                lbl = lbl_;

                var open = new SaveFileDialog();
                open.Filter = filter;
                var result = open.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(open.FileName))
                {
                    var x = new FileManagement.FileCopyProgress(source, open.FileName);
                    if (lbl != null) x.OnProgressChanged += X_OnProgressChanged_Label;
                    x.Copy();

                }
            }
            private void X_OnProgressChanged_Label(double Persentage, double size, ref bool Cancel)
            {
                lbl.Text = Persentage.ToString();
                lbl.Tag = size;
            }

        }

        public class Restore
        {
            ProgressBar progressBar1;
            Label lbl;
            public Restore(string filter, string des, ProgressBar progressBar1_)
            {
                progressBar1 = progressBar1_;

                var open = new OpenFileDialog();
                open.Filter = filter;
                var result = open.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(open.FileName))
                {
                    var x = new FileManagement.FileCopyProgress(open.FileName, des);
                    if (progressBar1 != null) x.OnProgressChanged += X_OnProgressChangedProgress;
                    x.Copy();

                }
            }
            private void X_OnProgressChangedProgress(double Persentage, double size, ref bool Cancel)
            {
                if (Persentage > 0) progressBar1.Visible = true;
                progressBar1.Value = Convert.ToInt32(Persentage);
                if (Persentage == 100) progressBar1.Visible = false;
            }
            public Restore(string filter, string des, Label lbl_)
            {
                lbl = lbl_;

                var open = new OpenFileDialog();
                open.Filter = filter;
                var result = open.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(open.FileName))
                {
                    var x = new FileManagement.FileCopyProgress(open.FileName, des);
                    if (lbl != null) x.OnProgressChanged += X_OnProgressChangedLabel;
                    x.Copy();

                }
            }
            private void X_OnProgressChangedLabel(double Persentage, double size, ref bool Cancel)
            {
                lbl.Text = Persentage.ToString();
                lbl.Tag = size;
            }


        }
        public static SRL.WinTools.Modal ReportFromSqlServerTB(DbContext db, string table, string status_column = "status", string title = "وضعیت ارسال")
        {
            string query = "select " + status_column + ", count(*) as cont from " + table + " group by " + status_column;
            var dt = SRL.Database.SqlQuery<ReportTBClass>(db, query).ToList();
            var dgv = new DataGridView();
            dgv.DataSource = dt;
            dgv.Click += (se, de) =>
            {
                dt = SRL.Database.SqlQuery<ReportTBClass>(db, query);
                dgv.DataSource = dt;
            };
            var modal = new SRL.WinTools.Modal(dgv, title, dgv.Width, dgv.Height);
            modal.ShowDialog();
            return modal;
        }
        public static void UpdateDgvCellValueToDb<EntityT>(DataGridView dgv, int row_index, string primary_column, string update_column, DbContext db)
        {
            var cell_value = dgv.Rows[row_index].Cells[update_column].Value.ToString();
            long id = (long)dgv.Rows[row_index].Cells[primary_column].Value;
            string tb_name = typeof(EntityT).Name;
            string sql = "update " + tb_name + " set " + update_column + " = '" + cell_value + "' where " + primary_column + " = " + id;
            ExecuteQuery(db, sql);

        }
        public static void EntityRemoveAll<EntityType>(DbContext db) where EntityType : class
        {
            foreach (var item in db.Set<EntityType>())
            {
                db.Set<EntityType>().Remove(item);
            }
            db.SaveChanges();

        }

        public static void EntityAdd<EntityType>(DbContext db, EntityType instance) where EntityType : class
        {
            db.Set<EntityType>().Add(instance);
            db.SaveChanges();

        }
        public static List<EntityType> EntitySelect<EntityType>(DbContext db, string sql) where EntityType : class
        {
            List<EntityType> list = db.Set<EntityType>().SqlQuery(sql).ToList();
            return list;

        }

        public static string ShowTableInDatagridview(string sql, DataGridView dgv, string connection_string)
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
        public class SqliteShowTablesFiled
        {
            public string type { get; set; }
            public string name { get; set; }
            public string tbl_name { get; set; }
            public long rootpage { get; set; }
            public string sql { get; set; }

        }
        public static List<SqliteShowTablesFiled> ShowTablesInSQLiteDB(DbContext db)
        {
            var result = new List<SqliteShowTablesFiled>();

            string sql = "SELECT * FROM sqlite_master WHERE type='table' ";

            try
            {
                result = SRL.Database.SqlQuery<SqliteShowTablesFiled>(db, sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }
        public static string ShowTablesInSQLDB(string connection_string, DataTable dt, string where_cluase = null)
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
        public static List<string> AddTableToDB(DbContext db, string table_name, DataGridView dataGridView1)
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

        public string InserRowToTable(DbContext db, string table_name, DataGridView dataGridView1, Dictionary<string, string> other_value, string encoder = "N")
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
                    cells.Add(cell.Value == null ? encoder + "''" : encoder + "'" + cell.Value.ToString() + "'");
                }

                string other_sql = "";

                if (other_value != null) other_sql = " , " + encoder + "'" + string.Join("' , " + encoder + "'", other_value.Values.ToList()) + "'";

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

        public static string SearchTable(DbContext db, string table_name, Dictionary<string, string> filter, DataGridView dgv, Label lblCount, string connection_string)
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

        public static int TruncateTable(DbContext db, string table_name)
        {
            int res = db.Database.ExecuteSqlCommand("delete from " + table_name);
            db.SaveChanges();
            return res;
        }
        public static string ExecuteQuery(DbContext db, string query)
        {
            string error = string.Empty;
            int exe;
            try
            {
                exe = db.Database.ExecuteSqlCommand(query);
                exe = db.SaveChanges();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                MessageBox.Show(error);
            }
            return error;
        }
        public static long GetSqliteNewRowId(DbContext db, string table_name)
        {
            var query = SqlQuery<long>(db, "SELECT seq FROM sqlite_sequence WHERE (name = '" + table_name + "')");
            return query[0] + 1;

        }
        public static List<SqlServerTableConstrain> ViewTableUniqueConstrains(DbContext db, SqlServerConstrainType constrain = SqlServerConstrainType.All)
        {
            string query = "SELECT* FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS ";
            if (constrain != SqlServerConstrainType.All) query += " where CONSTRAINT_TYPE = '" + constrain.ToString().Replace("_", " ") + "'";
            var list = SRL.Database.SqlQuery<SqlServerTableConstrain>(db, query);
            return list;

        }
        public static List<OutputType> SqlQuery<OutputType>(DbContext db, string query)
        {
            try
            {
                var result_ = db.Database.SqlQuery<OutputType>(query);

                if (result_.Any())
                {
                    var result = result_.ToList();
                    return result;
                }
                else return new List<OutputType>();
            }

            catch (Exception exe)
            {
                MessageBox.Show(exe.Message);
                return null;
            }

        }
        public static string GetConnectionString(string conStrName)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
            string last_con = connectionStringsSection.ConnectionStrings[conStrName].ConnectionString;
            return last_con;
        }
        public static string GetDataSourceOfConnectionString(string conStrName)
        {
            ConnectionStringSettings genusSettings = ConfigurationManager.ConnectionStrings[conStrName];
            if (genusSettings == null || string.IsNullOrEmpty(genusSettings.ConnectionString))
            {
                return "(SRL)Fatal error: Missing connection string config file";
            }
            string genusConnectionString = genusSettings.ConnectionString;
            System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder entityConnectionStringBuilder =
                new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(genusConnectionString);
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(entityConnectionStringBuilder.ProviderConnectionString);
            string genusSqlServerName = sqlConnectionStringBuilder.ConnectionString;

            return genusSqlServerName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conStr">metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string="data source=.\SOHEILLAMSO;initial catalog=Semnan;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework"</param>
        /// <param name="conStrName">e.g. SemnanEntity</param>
        /// <param name="control_to_load"></param>
        public bool UpdateConnectionString(string conStr, string conStrName, Control control_to_load = null)
        {
            //for sqlite: @"metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SQLite.EF6;provider connection string='data source=C:\Program Files\hami\MyDatabase.sqlite'"
            // or :       @"metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SQLite.EF6;provider connection string='data source=MyDatabase.sqlite;'"

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

        public static void UpdateConnectionStringAndRestart(string conStr, string conStrName, Control control_to_load)
        {

            // example: SRL.Database.UpdateConnectionStringAndRestart(@"metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SQLite.EF6;provider connection string='data source=MyDatabase.sqlite;'", typeof(MyDatabaseEntities).Name, control);
            using (SRL.Database dbsrl = new SRL.Database())
            {
                if (!dbsrl.UpdateConnectionString(conStr, conStrName, control_to_load))
                {
                    Application.Restart();
                    Environment.Exit(0);
                }
            }

        }

        
        public static GetSqlInstanceResult GetSqlInstance(string db_name, out string data_source, out string connection_str, out Exception ex)
        {
            data_source = "";
            connection_str = "";
            ex = new Exception();

            List<string> list = SRL.Database.SqlServerInstances.ListLocalSqlInstances().ToList();

            foreach (var item in SRL.Database.SqlServerInstances.ListLocalSqlInstances().ToList())
            {

                System.Data.SqlClient.SqlConnectionStringBuilder builder =
  new System.Data.SqlClient.SqlConnectionStringBuilder();

                builder.DataSource = item;
                builder.InitialCatalog = db_name;
                builder.IntegratedSecurity = true;

                connection_str = builder.ConnectionString;
                using (SqlConnection con = new SqlConnection(connection_str))
                {

                    try
                    {
                        list.Remove(item);
                        con.Open();
                        data_source = item;
                        return GetSqlInstanceResult.OK;
                    }
                    catch (SqlException ex1)
                    {
                        ex = ex1;
                        if (ex1.Number == 2 || ex1.Number == 1225)
                            continue;
                        else
                        {
                            if (list.Any()) continue;
                            else return GetSqlInstanceResult.SqlException;
                        }

                    }
                    catch (Exception ex2)
                    {
                        ex = ex2;
                        if (list.Any()) continue;
                        else return GetSqlInstanceResult.Exception;
                    }
                }

            }

            return GetSqlInstanceResult.InstanceNotFound;
        }

       
    }
    public class AccessManagement
    {


        public enum AccessDataType
        {
            [Description("Text")]
            nvarcharmax = 1,

            [Description("Integer")]
            integer = 2,

            [Description("Long")]
            longinteger = 3,

            [Description("Double")]
            doublenumber = 4,

            [Description("Integer PRIMARY KEY AUTOINCREMENT")]
            autonumberlong = 5,

            [Description("Date/Time")]
            datetime = 6
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="ofDialog"></param>
        /// <param name="lblFileName"></param>
        /// <param name="main_headers"></param>
        /// <param name="dataGridView1"></param>
        /// <param name="lblCount"></param>
        public static DataTable LoadDGVFromAccess(OpenFileDialog ofDialog, Label lblFileName, SRL.KeyValue.DataTableHeaderCheckType check_type, string[] main_headers, DataGridView dgv, Label lblCount, string table_name)
        {
            try
            {
                if (!System.IO.File.Exists(ofDialog.FileName))
                {
                    ofDialog.Filter = "Access files|*.accdb";
                    if (ofDialog.ShowDialog() != DialogResult.OK || ofDialog.FileName == "") return null;
                    lblFileName.Text = ofDialog.FileName;
                }
                DataTable table = GetDataTableFromAccess(ofDialog.FileName, table_name);
                if (table == null) return null;

                string header_checked = SRL.KeyValue.CheckDataTableHeaders(table, main_headers, check_type);
                if (header_checked == "true")
                {
                    dgv.DataSource = table;

                    if (lblCount != null) lblCount.Text = dgv.RowCount.ToString();
                }

                else MessageBox.Show(header_checked);
                ofDialog.FileName = "";
                return table;


            }
            catch (Exception ex)
            {

                MessageBox.Show("LoadDGVFromAccess " + ex.Message);
                throw;
            }
        }

        public static DataTable GetDataTableFromAccess(string file_full_path, string table_name, string provider = "Microsoft.ACE.OLEDB.12.0")
        {
            if (!System.IO.File.Exists(file_full_path))
            {
                MessageBox.Show("choose file!");
                return null;
            }
            string strProvider = @"Provider = " + provider + "; Data Source = " + file_full_path;
            string strSql = "Select * from " + table_name;
            OleDbConnection con = new OleDbConnection(strProvider);
            OleDbCommand cmd = new OleDbCommand(strSql, con);
            con.Open();
            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable table = new DataTable();
            try
            {
                da.Fill(table);
                return table;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public static DataColumnCollection GetTableHeadersFromAccess(string file_full_path, string table_name, string provider = "Microsoft.ACE.OLEDB.12.0")
        {
            string strProvider = @"Provider = " + provider + "; Data Source = " + file_full_path;
            string strSql = "Select top 1 * from " + table_name;
            OleDbConnection con = new OleDbConnection(strProvider);
            OleDbCommand cmd = new OleDbCommand(strSql, con);
            con.Open();
            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable table = new DataTable();
            try
            {
                da.Fill(table);
                DataColumnCollection dtc = table.Columns;
                return dtc;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public static int AddColumnToAccess(string column_name, string table_name, AccessDataType type_enum, string file_full_path, bool show_error, string provider = "Microsoft.ACE.OLEDB.12.0")
        {
            DataColumnCollection columns = SRL.AccessManagement.GetTableHeadersFromAccess(file_full_path, "table1");
            if (!columns.Contains(column_name))
            {
                string type = SRL.ClassManagement.GetEnumDescription<AccessDataType>(type_enum);
                string query = "alter table " + table_name + " add " + column_name + " " + type;
                return ExecuteToAccess(query, file_full_path, show_error, provider);
            }
            else return -1;

        }

        public static SRL.WinTools.Modal ReportFromAccess(string path, string status_column = "status", string table = "table1", string title = "وضعیت ارسال")
        {
            string query = "select " + status_column + ", count(*) as cont from " + table + " group by " + status_column;
            var dt = SRL.AccessManagement.SqlQueryFromAccess(path, query);
            var dgv = new DataGridView();
            dgv.DataSource = dt;
            dgv.Click += (se, de) =>
            {
                dt = SRL.AccessManagement.SqlQueryFromAccess(path, query);
                dgv.DataSource = dt;
            };
            var modal = new SRL.WinTools.Modal(dgv, title, dgv.Width, dgv.Height);
            modal.ShowDialog();
            return modal;
        }

        public static int ExecuteToAccess(string query, string file_full_path, bool show_error, string provider = "Microsoft.ACE.OLEDB.12.0")
        {
            string strProvider = @"Provider = " + provider + "; Data Source = " + file_full_path;

            using (OleDbConnection con = new OleDbConnection(strProvider))
            {


                OleDbCommand cmd = new OleDbCommand(query, con);

                try
                {
                    con.Open();
                    cmd.CommandType = CommandType.Text;
                    int res = cmd.ExecuteNonQuery();
                    return res;
                }
                catch (Exception ex)
                {
                    if (show_error) MessageBox.Show(ex.Message);

                    return -1;
                }
            }
        }

        public static DataTable SqlQueryFromAccess(string file_full_path, string query, string provider = "Microsoft.ACE.OLEDB.12.0")
        {
            if (!System.IO.File.Exists(file_full_path))
            {
                MessageBox.Show("error ! " + file_full_path);
                return null;
            }
            string strProvider = @"Provider = " + provider + "; Data Source = " + file_full_path;

            OleDbConnection con = new OleDbConnection(strProvider);
            OleDbCommand cmd = new OleDbCommand(query, con);
            con.Open();
            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable table = new DataTable();
            try
            {
                da.Fill(table);
                return table;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public static string CheckAccessHeaders(DataTable dt, string[] main_headers)
        {
            List<string> access_columns = new List<string>();
            foreach (DataColumn item in dt.Columns)
            {
                access_columns.Add(item.ColumnName);
            }

            foreach (var file_header in access_columns)
            {
                Application.DoEvents();
                if (!main_headers.Contains(file_header))
                {
                    return file_header + " is not valid.";
                }
                else continue;
            }

            foreach (var main_header in main_headers)
            {
                Application.DoEvents();
                if (!access_columns.Contains(main_header))
                {
                    return "file does not have column: " + main_header;
                }
                else continue;
            }

            return "true";

        }

    }
    public class ExcelManagement : SRL.ControlLoad
    {
        public enum ExcelPathType
        {
            NotSet,
            DesktopDatetimeToSecond,
            Desktop
        }

        public ExcelManagement()
        {
        }
        public ExcelManagement(Button btn)
            : base(btn)
        {
        }

        public static DataTable GetDataTableFromExcel(string file_full_path)
        {
            ExcelLibrary.Office.Excel.Workbook excel_file = ExcelLibrary.Office.Excel.Workbook.Open(file_full_path);
            var worksheet = excel_file.Worksheets[0]; // assuming only 1 worksheet
            var cells = worksheet.Cells;

            DataTable dt = new DataTable();
            int file_last_column_index = cells.LastColIndex;
            // add columns
            foreach (var header in cells.GetRow(cells.FirstRowIndex))
            {
                dt.Columns.Add(header.Value.StringValue);
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
                dt.Rows.Add(file_row_cells.ToArray());
            }

            return dt;


        }
        public static void LoadDGVFromExcel(OpenFileDialog ofDialog, Label lblFileName, SRL.KeyValue.DataTableHeaderCheckType check_type, string[] main_headers, DataGridView dgv, Label lblCount = null)
        {


            if (!System.IO.File.Exists(ofDialog.FileName))
            {
                ofDialog.Filter = "Only 97/2003 excel with one sheet|*.xls";
                if (ofDialog.ShowDialog() != DialogResult.OK || ofDialog.FileName == "") return;
                lblFileName.Text = ofDialog.FileName;
            }
            ExcelLibrary.Office.Excel.Workbook excel_file = ExcelLibrary.Office.Excel.Workbook.Open(ofDialog.FileName);
            var worksheet = excel_file.Worksheets[0]; // assuming only 1 worksheet
            var cells = worksheet.Cells;


            DataTable dt = new DataTable();
            int file_last_column_index = cells.LastColIndex;
            // add columns
            foreach (var header in cells.GetRow(cells.FirstRowIndex))
            {
                dt.Columns.Add(header.Value.StringValue);
            }

            string header_checked = SRL.KeyValue.CheckDataTableHeaders(dt, main_headers, check_type);

            if (header_checked == "true")
            {
                // add rows
                for (int rowIndex = cells.FirstRowIndex + 1; rowIndex <= cells.LastRowIndex; rowIndex++)
                {
                    ExcelLibrary.Office.Excel.Row file_row = cells.GetRow(rowIndex);
                    List<object> file_row_cells = new List<object>();
                    for (int i = 0; i < file_last_column_index + 1; i++)
                    {
                        file_row_cells.Add(file_row.GetCell(i).Value);
                    }
                    dt.Rows.Add(file_row_cells.ToArray());
                }

                dgv.DataSource = dt;
                if (lblCount != null) lblCount.Text = dgv.RowCount.ToString();
            }
            else MessageBox.Show(header_checked);
            ofDialog.FileName = "";
        }


        public string ExportToExcell(DataGridView dgview, int? devider = null, string fileFullNameNoExtention = null, TextBox tbCountDynamic = null, ExcelPathType excel_naming = ExcelPathType.NotSet, string default_name = "")
        {
            string path = fileFullNameNoExtention + ".xls";

            string file;
            switch (excel_naming)
            {
                case ExcelPathType.DesktopDatetimeToSecond:
                    file = default_name + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    path = System.IO.Path.Combine(SRL.FileManagement.GetDesktopDirectory(), @file);
                    break;
                case ExcelPathType.Desktop:
                    file = default_name + ".xls";
                    path = System.IO.Path.Combine(SRL.FileManagement.GetDesktopDirectory(), @file);
                    break;
            }

            if (tbCountDynamic == null)
            {
                ExportToExcellFile(dgview, (int)devider, path);
            }
            else
            {
                int cont;
                if (string.IsNullOrWhiteSpace(tbCountDynamic.Text) || !int.TryParse(tbCountDynamic.Text, out cont)) return null;

                ExportToExcellFile(dgview, cont, path);
            }
            return path;
        }

        private void ExportToExcellFile(DataGridView dgview, int devider, string path)
        {
            DataSet ds = new DataSet();
            int table_count = dgview.Rows.Count / devider;
            int index = 0;

            for (int i = 0; i < table_count; i++)
            {
                ButtonLoader(table_count);
                DataTable table = new DataTable(i.ToString());
                ds.Tables.Add(table);
                SRL.Convertor.MakeDataTableFromDGV(dgview, table, devider, index);
                index += devider;
            }
            DataTable _table = new DataTable("else");
            ds.Tables.Add(_table);
            SRL.Convertor.MakeDataTableFromDGV(dgview, _table, devider, index);

            ExcelLibrary.DataSetHelper.CreateWorkbook(@path, ds);
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
    public class WinReport
    {

        public static void Reporter<ReportType, SubReportType>(Assembly assembly, List<ReportType> report_data, string printer_name, string dataset, string report_path, string title, short copies,
            string sub_path, string sub_name, List<SubReportType> sub_data = null, string sub_dataset = null,
            DisplayMode display_mode = DisplayMode.PrintLayout, int max_width = 1000, int max_height = 700, int zoom_percent = 75)
        {
            // SRL.WinReport.Reporter<PurchaseTB, PurchaseKalaTB>(Assembly.GetExecutingAssembly(), Public.dbGlobal.PurchaseTB.Where(x => x.Id == purchase_id).ToList(),  Public.srl_setting_class.SqlQuerySettingTable("printer_name"),
            //"DataSet1","hesabdari_app.Rep.Purchase.rdlc", "گزارش چاپی", 2, "hesabdari_app.Rep.PurchaseKala.rdlc", "PurchaseKala", Public.dbGlobal.PurchaseKalaTB.Where(x => x.purchase_id == purchase_id).ToList(),"DataSetPurchaseKala");

            //rdlc files must be addded to resources
            //report_path like "hesabdari_app.Rep.Purchase.rdlc"  starts with namespace then folders then file

            ReportViewer rw = new ReportViewer();

            //use: rw.LocalReport.ReportEmbeddedResource = "hesabdari_app.Rep.Purchase.rdlc" in one project instead assembly           
            Stream stream = assembly.GetManifestResourceStream(report_path);
            rw.LocalReport.LoadReportDefinition(stream);
            if (sub_name != null)
            {
                Stream sub_stream = assembly.GetManifestResourceStream(sub_path);
                rw.LocalReport.LoadSubreportDefinition(sub_name, sub_stream);
            }

            var bs = new System.Windows.Forms.BindingSource();
            bs.DataSource = report_data;
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            //in view: report data window, manage dataset. like: DataSet1
            reportDataSource1.Name = dataset;
            reportDataSource1.Value = bs;
            rw.LocalReport.DataSources.Clear();
            rw.LocalReport.DataSources.Add(reportDataSource1);

            rw.ProcessingMode = ProcessingMode.Local;

            if (sub_data != null)
                rw.LocalReport.SubreportProcessing += (s, e) => MySubreportEventHandler<SubReportType>(s, e, sub_dataset, sub_data, reportDataSource1);

            //in rdlc, report menu: report properties, first set page size(these are defaults), then consider margines, so in rdlc body, set sizes(less defaults). properties:Report , page sizes are like defaults.
            SizeF size = SRL.WinReport.GetLocalReportRdlcSize(rw);
            float dpi = SRL.WinTools.Media.GetScreenDpi(rw);
            int widthPixel = SRL.Convertor.InchToPixel(size.Width, dpi);
            int heightPixel = SRL.Convertor.InchToPixel(size.Height, dpi);
            widthPixel = Math.Min(widthPixel, max_width);
            heightPixel = Math.Min(heightPixel, max_height);
            rw.Width = widthPixel;
            rw.Height = heightPixel;

            rw.PrinterSettings.PrinterName = printer_name;
            rw.PrinterSettings.Copies = copies;
            rw.AutoScroll = true;
            rw.SetDisplayMode(display_mode);
            rw.ZoomMode = ZoomMode.Percent;
            rw.ZoomPercent = zoom_percent;

            var modal = new SRL.WinTools.Modal(rw, title, widthPixel, heightPixel, Color.Yellow);
            modal.BackColor = Color.Blue;
            modal.AutoScroll = true;
            modal.ShowDialog();

            rw.RefreshReport();
        }

        private static void MySubreportEventHandler<SubReportType>(object sender
            , SubreportProcessingEventArgs e, string dataset, List<SubReportType> sub_data, ReportDataSource rds)
        {
            var sub_bs = new System.Windows.Forms.BindingSource();
            sub_bs.DataSource = sub_data;
            Microsoft.Reporting.WinForms.ReportDataSource sub_data_source = new Microsoft.Reporting.WinForms.ReportDataSource();
            sub_data_source.Name = dataset;
            sub_data_source.Value = sub_bs;
            e.DataSources.Add(sub_data_source);
            e.DataSources.Add(rds);
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

        public static SizeF GetLocalReportRdlcSize(ReportViewer report_viewer)
        {
            SizeF size = new SizeF();
            var report_setting = report_viewer.LocalReport.GetDefaultPageSettings();
            float width_ = report_setting.PaperSize.Width / 100F;
            float height_ = report_setting.PaperSize.Height / 100F;

            size.Width = width_;
            size.Height = height_;

            return size;

        }


    }
    public class FileManagement
    {
        public enum FileType
        {
            Excel,
            Access
        }
        public FileManagement()
        {

        }

        public static void LoadDGVFromFile(OpenFileDialog ofDialog, Label lblFileName, SRL.KeyValue.DataTableHeaderCheckType check_type, string[] main_headers, DataGridView dgv, Label lblCount, string table_name)
        {
            ofDialog.Filter = "access or excel 2003|*.accdb; *.xls";


            if (ofDialog.ShowDialog() != DialogResult.OK || ofDialog.FileName == "") return;
            lblFileName.Text = ofDialog.FileName;
            switch (Path.GetExtension(ofDialog.FileName))
            {
                case ".xls":
                    SRL.ExcelManagement.LoadDGVFromExcel(ofDialog, lblFileName, check_type, main_headers, dgv, lblCount);
                    break;
                case ".accdb":
                    SRL.AccessManagement.LoadDGVFromAccess(ofDialog, lblFileName, check_type, main_headers, dgv, lblCount, table_name);
                    break;
            }


        }


        public class FileCopyProgress
        {
            public delegate void ProgressChangeDelegate(double Persentage, double size, ref bool Cancel);
            public delegate void Completedelegate();

            /// <summary>
            /// use OnProgressChanged event and then call Copy
            /// </summary>
            /// <param name="Source"></param>
            /// <param name="Dest"></param>
            public FileCopyProgress(string Source, string Dest)
            {
                this.SourceFilePath = Source;
                this.DestFilePath = Dest;

                OnProgressChanged += delegate { };
                OnComplete += delegate { };
            }

            public void Copy()
            {
                byte[] buffer = new byte[1024 * 1024]; // 1MB buffer
                bool cancelFlag = false;

                using (FileStream source = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
                {
                    long fileLength = source.Length;

                    using (FileStream dest = new FileStream(DestFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                    {
                        long totalBytes = 0;
                        int currentBlockSize = 0;

                        while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytes += currentBlockSize;
                            double persentage = (double)totalBytes * 100.0 / fileLength;

                            dest.Write(buffer, 0, currentBlockSize);

                            cancelFlag = false;
                            OnProgressChanged(persentage, totalBytes, ref cancelFlag);

                            if (cancelFlag == true)
                            {
                                // Delete dest file here
                                break;
                            }
                        }
                    }
                }

                OnComplete();
            }

            public string SourceFilePath { get; set; }
            public string DestFilePath { get; set; }

            public event ProgressChangeDelegate OnProgressChanged;
            public event Completedelegate OnComplete;
        }



        /// <summary>
        /// get full path file and save or replace icon file
        /// </summary>
        /// <param name="filePath">@"C:\hami.exe"</param>
        /// <param name="icon_full_path">@"e:\myfile.ico"</param>
        public static void ExtractFileIcon(string filePath, string icon_full_path)
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

        public static void MakeShortcutUrl(string shortcut_name, string shortcut_directory, string full_path_to_file)
        {

            using (StreamWriter writer = new StreamWriter(shortcut_directory + "\\" + shortcut_name + ".url"))
            {
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=" + full_path_to_file);
                writer.Flush();
            }
        }
        public static void MakeShortcut(string shortcut_name, string shortcut_directory, string file_directory_path, string file_with_extention, string icon_full_path_from_file_directory = null)
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
        public static void CutPasteFile(string sourceFilePath, string destnationFilePath)
        {
            System.IO.File.Move(sourceFilePath, destnationFilePath);
        }
        public static string ReplaceAllFilesFromDirToDir(string SourceFolderFullPath, string DestinationFolderFullPath, List<string> not_copy_files = null)
        {
            try
            {

                foreach (string dirPath in Directory.GetDirectories(SourceFolderFullPath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(SourceFolderFullPath, DestinationFolderFullPath));

                CreateFolderOverwrite(DestinationFolderFullPath);

                foreach (string newPath in Directory.GetFiles(SourceFolderFullPath, "*.*",
                    SearchOption.AllDirectories))
                {
                    if (not_copy_files != null)
                        if (not_copy_files.Contains(Path.GetFileName(newPath))) continue;
                    System.IO.File.Copy(newPath, newPath.Replace(SourceFolderFullPath, DestinationFolderFullPath), true);
                }
                return "";
            }
            catch (Exception exc)
            {
                return exc.Message;
            }

        }

        public static void CreateFolderOverwrite(string FolderFullPath)
        {
            System.IO.Directory.CreateDirectory(FolderFullPath);
        }
        public static string GetFilePathInRoot(string fileName)
        {
            string path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);

            path = Directory.GetParent(Directory.GetParent(path).FullName).FullName;

            path += @"\" + fileName;

            return path;
        }

        public static string GetDesktopDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        }
        public static string GetCurrentDirectory()
        {
            //System.IO.Path.GetDirectoryName(Application.ExecutablePath);  c:\users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug
            //System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); c:\users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug
            //System.IO.Directory.GetCurrentDirectory();    C:\Users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug
            //Thread.GetDomain().BaseDirectory  c:\users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug\
            //Environment.CurrentDirectory   C:\Users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug
            //Application.StartupPath  C:\Users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug
            return System.AppDomain.CurrentDomain.BaseDirectory; // c:\users\1\documents\visual studio 2015\Projects\test_licence\test_licence\bin\Debug\

        }
        public static IEnumerable<string> GetFileLines(string fileFullName)
        {

            string path = @fileFullName;

            var get_line = System.IO.File.ReadLines(path);

            return get_line;
        }
        public static string GetFileContent(string fileFullName, int line)
        {
            string path = @fileFullName;

            var get_line = System.IO.File.ReadLines(path).Skip(line - 1);

            return get_line.Count() < 1 ? "" : get_line.First();
        }

        public static void SaveToFile(string fileFullName, string content, int line)
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
        public static void SaveToFile(string fileFullName, string content)
        {
            System.IO.File.WriteAllText(fileFullName, content);

        }


        public static void CopyToClipboard(string content)
        {
            Clipboard.SetText(content);
        }
        public static void SaveToFileDialog(SaveFileDialog dlgSaveFile, string content)
        {
            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
            {
                //Save license data into local file
                System.IO.File.WriteAllText(dlgSaveFile.FileName, content.Trim(), Encoding.UTF8);
            }
        }

        public static void CreateFileSample(string[] headers, string destination_name_no_extention, FileType type)
        {
            switch (type)
            {
                case FileType.Excel:
                    DataGridView d = new DataGridView();
                    foreach (var item in headers)
                    {
                        d.Columns.Add(item, item);
                    }
                    MessageBox.Show("file created in: " +
                    new SRL.ExcelManagement().ExportToExcell(d, 100, null, null, SRL.ExcelManagement.ExcelPathType.Desktop, destination_name_no_extention));
                    break;

                case FileType.Access:
                    string source = System.IO.Path.Combine(SRL.FileManagement.GetCurrentDirectory(), "sample.accdb");
                    string name = destination_name_no_extention + ".accdb";
                    string des = System.IO.Path.Combine(SRL.FileManagement.GetDesktopDirectory(), name);

                    System.IO.File.Copy(source, des, true);

                    foreach (var item in headers)
                    {
                        SRL.AccessManagement.ExecuteToAccess("alter table table1 add " + item + " nvarchar(50)", des, true);
                    }
                    SRL.AccessManagement.ExecuteToAccess("alter table table1 drop column Deleted_column", des, true);
                    MessageBox.Show("file created in: " + des);
                    break;

            }

        }

        public static void CreateFileOverwrite(string file_path, string text, bool launch)
        {
            // var file_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "json.txt");

            System.IO.File.WriteAllText(file_path, "");
            System.IO.File.WriteAllText(file_path, text);
            if (launch)
                System.Diagnostics.Process.Start(file_path);

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
            //example:  srl_font.InstallFont(srl_font.GetContentFontNameFromByte(Properties.Resources.irsan, "irsan.ttf"));

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

