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

namespace SRL
{
    public class DateTimeLanguageClass
    {
        System.Windows.Forms.Timer timer = null;
        Control control_to_show_time;
        public void StartTimer(Control control, string full_or_long_or_short)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler((sender, e) => timer_Tick(sender, e, full_or_long_or_short));
            timer.Enabled = true;
            control_to_show_time = control;
        }

        void timer_Tick(object sender, EventArgs e, string show_type)
        {
            switch (show_type)
            {
                case "full":
                    control_to_show_time.Text = DateTime.Now.ToString();
                    break;
                case "long":
                    control_to_show_time.Text = DateTime.Now.ToLongTimeString();
                    break;
                case "short":
                    control_to_show_time.Text = DateTime.Now.ToShortTimeString();
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
          return  dt.Value.ToString(format);
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
           public string key {get;set;}
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
        public void ShowSettingInControls(Control form_font_size = null, Control menu_font_size = null,
            Control child_width_relative = null, Control child_height_relative = null, Control font_name = null,
            Control form_back_color = null, Control menu_back_color = null, Control font_factor = null,
            Control print_width1 = null, Control print_height1 = null, Control print_width_container_plus1 = null,
            Control print_height_container_plus1 = null, Control db_version = null)
        {
            if (form_font_size != null) form_font_size.Text = SqlQuerySettingTable("form_font_size");
            if (menu_font_size != null) menu_font_size.Text = SqlQuerySettingTable("menu_font_size");
            if (child_width_relative != null) child_width_relative.Text = SqlQuerySettingTable("child_width_relative");
            if (child_height_relative != null) child_height_relative.Text = SqlQuerySettingTable("child_height_relative");
            if (font_name != null) font_name.Text = SqlQuerySettingTable("font_name");
            if (form_back_color != null) form_back_color.BackColor = Color.FromName(SqlQuerySettingTable("form_back_color"));
            if (menu_back_color != null) menu_back_color.BackColor = Color.FromName(SqlQuerySettingTable("menu_back_color"));
            if (font_factor != null) font_factor.Text = SqlQuerySettingTable("font_factor");
            if (print_width1 != null) print_width1.Text = SqlQuerySettingTable("print_width1");
            if (print_height1 != null) print_height1.Text = SqlQuerySettingTable("print_height1");
            if (print_width_container_plus1 != null) print_width_container_plus1.Text = SqlQuerySettingTable("print_width_container_plus1");
            if (print_height_container_plus1 != null) print_height_container_plus1.Text = SqlQuerySettingTable("print_height_container_plus1");
            if (db_version != null) db_version.Text = SqlQuerySettingTable("db_version");
        }

        public string UpdateSetting(string form_font_size = null, string menu_font_size = null,
            string child_width_relative = null, string child_height_relative = null, string font_name = null,
            string form_back_color = null, string menu_back_color = null, string font_factor = null,
            string print_width1 = null, string print_height1 = null, string print_width_container_plus1 = null,
            string print_height_container_plus1 = null)
        {
            string error = string.Empty;

            if (form_font_size != null) error = ExecuteUpdateSettingTable("form_font_size", form_font_size);
            if (menu_font_size != null) error = ExecuteUpdateSettingTable("menu_font_size", menu_font_size);
            if (child_width_relative != null) error = ExecuteUpdateSettingTable("child_width_relative", child_width_relative);
            if (child_height_relative != null) error = ExecuteUpdateSettingTable("child_height_relative", child_height_relative);
            if (font_name != null) error = ExecuteUpdateSettingTable("font_name", font_name);
            if (form_back_color != null) error = ExecuteUpdateSettingTable("form_back_color", form_back_color);
            if (menu_back_color != null) error = ExecuteUpdateSettingTable("menu_back_color", menu_back_color);
            if (font_factor != null) error = ExecuteUpdateSettingTable("font_factor", font_factor);
            if (print_width1 != null) error = ExecuteUpdateSettingTable("print_width1", print_width1);
            if (print_height1 != null) error = ExecuteUpdateSettingTable("print_height1", print_height1);
            if (print_width_container_plus1 != null) error = ExecuteUpdateSettingTable("print_width_container_plus1", print_width_container_plus1);
            if (print_height_container_plus1 != null) error = ExecuteUpdateSettingTable("print_height_container_plus1", print_height_container_plus1);

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
                    if (item == typeof(ComboBox)) ClearControlsValue<ComboBox>(childs, "Text", string.Empty);
                    if (item == typeof(TextBox)) ClearControlsValue<TextBox>(childs, "Text", string.Empty);
                    if (item == typeof(RadioButton)) ClearControlsValue<RadioButton>(childs, "Checked", false);
                    if (item == typeof(CheckBox)) ClearControlsValue<CheckBox>(childs, "Checked", false);
                }

            if (controls_to_refresh != null)
                foreach (dynamic control in controls_to_refresh)
                {
                    if (control is ComboBox) control.Text = string.Empty;
                    if (control is TextBox) control.Text = string.Empty;
                    if (control is RadioButton) control.Checked = false;
                    if (control is CheckBox) control.Checked = false;
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
                pb.MouseHover += new System.EventHandler(pb_MouseHover);
                pb.MouseLeave += new System.EventHandler(pb_MouseLeave);
            }

            private void pb_MouseHover(object sender, EventArgs e)
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
        public void StyleDatagridview(DataGridView dataGridView1, string style_mode)
        {
            switch (style_mode)
            {
                case "1":
                    dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Aqua;
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Red;
                    dataGridView1.ColumnHeadersHeight = 30;
                    dataGridView1.RightToLeft = RightToLeft.Yes;
                    dataGridView1.RowHeadersVisible = false;
                    dataGridView1.EnableHeadersVisualStyles = false;
                    dataGridView1.Width = dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.None) + 3;
                    break;

                default:
                    break;
            }

        }
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
    public class WinTools
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
                if (e.Button == MouseButtons.Right && e.RowIndex == -1 && e.ColumnIndex == -1)
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
                    if (mDataGridView.RightToLeft == RightToLeft.Yes) x_location +=  mDataGridView.Width;

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

            public DataGridViewColumnSelector(DataGridView dgv)
                : this()
            {
                this.DataGridView = dgv;
            }

            // When user checks / unchecks a checkbox, the related column visibility is 
            // switched.
            void mCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
            {
                mDataGridView.Columns[e.Index].Visible = (e.NewValue == CheckState.Checked);
            }
        }

        public class ComboItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
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
            cb.Items.Clear();
            cb.DisplayMember = "Text";
            cb.ValueMember = "Value";
            data_source.Insert(0, new { Text = "", Value = empty_row_value });
            cb.DataSource = data_source;

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

        public void FullScreenNoTaskbar(Control control)
        {
            control.Left = control.Top = 0;
            control.Width = Screen.PrimaryScreen.WorkingArea.Width;
            control.Height = Screen.PrimaryScreen.WorkingArea.Height;
        }
        public void AddChildToParentControlsAliagn(Control parent, Control child, bool reset_child_font = false)
        {
            if (reset_child_font) child.Font = default(Font);
            parent.Controls.Clear();
            parent.Controls.Add(child);
            AliagnChildToParent(parent, child);
        }
        public void AddChildToParentControlsZoomAndAliagn(Control parent, Control child, decimal font_factor = 1, bool use_parent_font_family = false)
        {
            FontFamily font_family = child.Font.FontFamily;
            FontStyle font_style = child.Font.Style;
            if (use_parent_font_family)
            {
                font_family = parent.Font.FontFamily;
                font_style = parent.Font.Style;
            }


            if (!font_family.IsStyleAvailable(font_style))
            {
                font_style = font_family.IsStyleAvailable(FontStyle.Regular) ? FontStyle.Regular :
                    font_family.IsStyleAvailable(FontStyle.Bold) ? FontStyle.Bold :
                    font_family.IsStyleAvailable(FontStyle.Italic) ? FontStyle.Italic :
                    font_family.IsStyleAvailable(FontStyle.Underline) ? FontStyle.Underline : FontStyle.Strikeout;
            }

            //    child.Font = new Font(font_family, child.Font.Size * (float)font_factor, font_style);

            decimal x_relative = Decimal.Divide(parent.Width, child.Width);
            decimal y_relative = Decimal.Divide(parent.Height, child.Height);
            var f = (x_relative + y_relative) / 2;
            f *= font_factor;



            child.Font = new Font(font_family, child.Font.Size * (float)f, font_style);

            AddChildToParentControlsAliagn(parent, child);
        }

        public void AliagnChildToParent(Control parent, Control child)
        {
            child.Location = new Point(
    parent.ClientSize.Width / 2 - child.Size.Width / 2,
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
            public Modal(Control user_control, string title, int width_ = 1000, int height_ = 500)
            {
                this.Text = title;

                this.StartPosition = FormStartPosition.CenterScreen;

                var borders = GetFormBorderSizes(this);

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
            UserControl user_control;
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
                DecimalInput_NotNull = 6

            }


            public UserControlValidation(UserControl uc, ErrorProvider errorProvider, bool force_cancel_)
            {
                errorProvider1 = errorProvider;
                user_control = uc;
                force_cancel = force_cancel_;
            }

            public void CheckAllField(List<Control> controls, out bool validation_result)
            {
                bool main_force_cancel = force_cancel;
                force_cancel = true;
                foreach (Control control in controls)
                {
                    control.Focus();
                }
                validation_result = user_control.ValidateChildren(ValidationConstraints.Enabled);
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

            private void mask_date_pattern_Validating(object sender, CancelEventArgs e)
            {
                MaskedTextBox control = sender as MaskedTextBox;

                var dt = new DateTime();

                if (new WinTools().GetRawStringMaskedTextBox(control).Any())
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
        public float StringToFloat(string str)
        {
            return float.Parse(str, NumberStyles.Any);
        }
        public Decimal StringToDecimal(string str)
        {
            return Decimal.Parse(str, NumberStyles.Any);
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
        public void UpdateConnectionString(string conStr, string conStrName, Control control_to_load)
        {
            //for sqlite: @"metadata=res://*/Model.Model1.csdl|res://*/Model.Model1.ssdl|res://*/Model.Model1.msl;provider=System.Data.SQLite.EF6;provider connection string='data source=C:\Program Files\hesabdari\MyDatabase.sqlite'"

            ControlLoader(control_to_load, "connecting database...");

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
            connectionStringsSection.ConnectionStrings[conStrName].ConnectionString = conStr;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
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
    }
    public class FileManagement
    {
        public FileManagement()
        {

        }
        /// <summary>
        /// get full path file and save or replace icon file
        /// </summary>
        /// <param name="filePath">@"C:\hesabdari.exe"</param>
        /// <param name="icon_full_path">@"e:\myfile.ico"</param>
        public void ExtractFileIcon(string filePath, string icon_full_path)
        {
            //@"e:\myfile.ico"
            //  var filePath = @"C:\Users\lamso1387\Documents\Visual Studio 2012\Projects\hesabdari\hesabdari\bin\Release\hesabdari.exe";
            var theIcon = Icon.ExtractAssociatedIcon(filePath);

            if (theIcon != null)
            {
                // Save it to disk, or do whatever you want with it.
                using (var stream = new System.IO.FileStream(icon_full_path, System.IO.FileMode.OpenOrCreate))
                {
                    theIcon.Save(stream);
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
        public void MakeShortcut(string shortcut_name, string shortcut_directory, string full_path_to_file)
        {
            WshShell wsh = new WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(
                shortcut_directory + "\\" + shortcut_name + ".lnk") as IWshRuntimeLibrary.IWshShortcut;
            // shortcut.Arguments = "c:\\app\\1.docx";
            shortcut.TargetPath = full_path_to_file;
            // not sure about what this is for
            shortcut.WindowStyle = 1;
            shortcut.Description = full_path_to_file;
            // shortcut.WorkingDirectory = "c:\\app";
            string icon_path = Path.GetDirectoryName(full_path_to_file) + "\\" + shortcut_name + ".ico";
            ExtractFileIcon(full_path_to_file, icon_path);
            shortcut.IconLocation = icon_path;
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
            //System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            //System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //System.IO.Directory.GetCurrentDirectory(); 
            //Thread.GetDomain().BaseDirectory
            //Environment.CurrentDirectory
            return System.AppDomain.CurrentDomain.BaseDirectory;

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



    }

}
