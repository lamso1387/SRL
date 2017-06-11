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


        public string GetCurrentKeyboardShort()
        {

            return InputLanguage.CurrentInputLanguage.Culture.Name;
        }
        public string ChangeKeyboardAltShift()
        {
            SendKeys.Send("%+");
            return GetCurrentKeyboardShort();
        }
    }
    public class SettingClass<SettingEntity> where SettingEntity : class
    {
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
            SRL.Database srl_database = new Database();
            SRL.ClassManagement<SettingEntity> class_mgnt = new SRL.ClassManagement<SettingEntity>();

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
            kv["menu_back_color"] = "Control";
            kv["font_factor"] = "1/003";
            return kv;
        }
        public void ShowSettingInControls(Control form_font_size, Control menu_font_size,
            Control child_width_relative, Control child_height_relative, Control font_name,
            Control form_back_color, Control menu_back_color)
        {
            if (form_font_size != null) form_font_size.Text = SqlQuerySettingTable("form_font_size");
            if (menu_font_size != null) menu_font_size.Text = SqlQuerySettingTable("menu_font_size");
            if (child_width_relative != null) child_width_relative.Text = SqlQuerySettingTable("child_width_relative");
            if (child_height_relative != null) child_height_relative.Text = SqlQuerySettingTable("child_height_relative");
            if (font_name != null) font_name.Text = SqlQuerySettingTable("font_name");
            if (form_back_color != null) form_back_color.BackColor = Color.FromName(SqlQuerySettingTable("form_back_color"));
            if (menu_back_color != null) menu_back_color.BackColor = Color.FromName(SqlQuerySettingTable("menu_back_color"));
        }
        public string UpdateSetting(string form_font_size, string menu_font_size,
            string child_width_relative, string child_height_relative, string font_name,
            string form_back_color, string menu_back_color)
        {
            string error = string.Empty;
            if (form_font_size != null) error = ExecuteUpdateSettingTable("form_font_size", form_font_size);
            if (menu_font_size != null) error = ExecuteUpdateSettingTable("menu_font_size", menu_font_size);
            if (child_width_relative != null) error = ExecuteUpdateSettingTable("child_width_relative", child_width_relative);
            if (child_height_relative != null) error = ExecuteUpdateSettingTable("child_height_relative", child_height_relative);
            if (font_name != null) error = ExecuteUpdateSettingTable("font_name", font_name);
            if (form_back_color != null) error = ExecuteUpdateSettingTable("form_back_color", form_back_color);
            if (menu_back_color != null) error = ExecuteUpdateSettingTable("menu_back_color", menu_back_color);
            return error;
        }

        private string ExecuteUpdateSettingTable(string key, string value)
        {
            string sql = "update " + setting_table_name + " set value='" + value + "' where key='" + key + "'";
            return new SRL.Database().ExecuteQuery(db, sql);
        }
        public bool CheckSettingIsSet()
        {
            string query = SqlQuerySettingTable("setting_is_set", null);
            int row_count = db.Set<SettingEntity>().Count();
            return query == null || query == "false" ? false : true;
        }
        public void StartSetting(Control form, Control menuContainor, bool isfont = true, bool isform_back_color = true,
            bool ismenu_back_color = true, bool isrelative = true, bool isAliagn = true)
        {
            if (isfont)
            {
                string font = SqlQuerySettingTable("font_name");

                string query = SqlQuerySettingTable("form_font_size");
                form.Font = new Font(font, float.Parse(query));

                query = SqlQuerySettingTable("menu_font_size");
                menuContainor.Font = new Font(font, float.Parse(query));
            }
            if (isform_back_color)
            {
                string query = SqlQuerySettingTable("form_back_color");
                form.BackColor = Color.FromName(query);
            }

            if (ismenu_back_color)
            {
                string query = SqlQuerySettingTable("menu_back_color");
                menuContainor.BackColor = Color.FromName(query);
            }
            var wintool = new WinTools();
            if (isrelative)
            {
                string width = SqlQuerySettingTable("child_width_relative");
                string height = SqlQuerySettingTable("child_height_relative");
                wintool.AdjustChildToParent(form, menuContainor, double.Parse(width), double.Parse(height));
            }
            if (isAliagn) wintool.AliagnChildToParent(form, menuContainor);

        }

        private string SqlQuerySettingTable(string key, string default_if_empty = null)
        {

            string sql = "select value from " + setting_table_name + " where key='" + key + "'";
            var query = new SRL.Database().SqlQuery<string>(db, sql).DefaultIfEmpty(default_if_empty).FirstOrDefault();
            return query;

        }

    }
    public class ChildParent
    {
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
            public PictureBoxHover(PictureBox pb, System.Windows.Forms.Cursor cursor_, int width_magnify_ = 0, int height_magnify_ = 0, int opacity_ = 255)
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
            public void SetPictueBoxOpacity(PictureBox pb, int opc)
            {
                Bitmap pic = (Bitmap)pb.Image;
                for (int w = 0; w < pic.Width; w++)
                {
                    for (int h = 0; h < pic.Height; h++)
                    {
                        Color c = pic.GetPixel(w, h);
                        Color newC = Color.FromArgb(opc, c);
                        pic.SetPixel(w, h, newC);
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
            foreach (ToolStripMenuItem item in menu_strip.Items)
            {
                item.BackColor = default(Color);

            }
            menu_strip.Items[item_name_to_alter_color].BackColor = Color.FromName(basic_back_color_name);

        }
        public void MenuStripClickColoring(MenuStrip menu_strip, string item_name_to_alter_color, Color back_color, Color fore_color)
        {
            foreach (ToolStripMenuItem item in menu_strip.Items)
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
        public class TextBoxBorderColor : TextBox
        {
            public Color border_color;
            public Color border_focus_color;
            public string border_or_focus_or_both;

            public TextBoxBorderColor(Color border_color_, Color border_focus_color_, string border_or_focus_or_both_)
            {
                border_color = border_color_;
                border_focus_color = border_focus_color_;
                border_or_focus_or_both = border_or_focus_or_both_;
            }
            [DllImport("user32")]
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
        public void AddChildToParentControlsZoomAndAliagn(Control parent, Control child, decimal font_factor = 1)
        {
            decimal x_relative = Decimal.Divide(parent.Width, child.Width);
            decimal y_relative = Decimal.Divide(parent.Height, child.Height);
            var f = (x_relative + y_relative) / 2;
            f *= font_factor;

            child.Font = new Font(child.Font.FontFamily, child.Font.Size * (float)f);

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

        public class Media
        {
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
            public Modal(UserControl user_control, string title)
            {
                this.Text = title;

                this.Width = 1000;
                this.Height = 500;
                this.StartPosition = FormStartPosition.CenterScreen;

                Panel pnlModal = new Panel();

                pnlModal.Width = this.Width - 100;
                pnlModal.Height = this.Height - 100;

                new WinTools().AddChildToParentControlsAliagn(this, pnlModal, true);

                new WinTools().AddChildToParentControlsAliagn(pnlModal, user_control, true);
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
                DecimalInput = 5

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
                }

            }
            private void number_input_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 ))
                {
                    e.Handled = true;
                    return;
                }
            }
            private void decimal_input_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
                {
                    e.Handled = true;
                    return;
                }

                // checks to make sure only 1 decimal is allowed
                if (e.KeyChar == 46)
                {
                    if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                        e.Handled = true;
                }
            }

            private void mask_date_pattern_Validating(object sender, CancelEventArgs e)
            {
                MaskedTextBox control = sender as MaskedTextBox;
                MaskFormat format = control.TextMaskFormat;
                control.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                if (control.Text.Any())
                    control.TextMaskFormat = MaskFormat.IncludeLiterals;

                var dt = new DateTime();
                if (control.Text.Any())
                    if (!DateTime.TryParse(control.Text, out dt))
                    {
                        e.Cancel = force_cancel;
                        var msg = new SRL.ClassManagement<ErrorTypes>().GetEnumDescription(ErrorTypes.MaskDatePattern);

                        errorProvider1.SetError(control, msg);
                        control.TextMaskFormat = format;
                        return;
                    }
                control.TextMaskFormat = format;
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
        public List<string> EnglishToPersianDate(DateTime d)
        {
            List<string> date_list = new List<string>();
            //string GregorianDate = "Thursday, October 24, 2013";
            //DateTime d = DateTime.Parse(GregorianDate);
            PersianCalendar pc = new PersianCalendar();
            date_list.Add(string.Format("{0}/{1}/{2}", pc.GetYear(d), pc.GetMonth(d), pc.GetDayOfMonth(d)));
            var persianDate = new DateTime(pc.GetYear(d), pc.GetMonth(d), pc.GetDayOfMonth(d) - 1);
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
        public string GetFileContentInRoot(string fileName, int line)
        {
            string path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);

            path = Directory.GetParent(Directory.GetParent(path).FullName).FullName;

            path += @"\" + fileName;

            var get_line = File.ReadLines(path).Skip(line - 1);

            return get_line.Count() < 1 ? "" : get_line.First();
        }
        public string GetFileContent(string fileFullName, int line)
        {
            string path = @fileFullName;

            var get_line = File.ReadLines(path).Skip(line - 1);

            return get_line.Count() < 1 ? "" : get_line.First();
        }
        public void SaveToFileInRoot(string fileName, string content, int line)
        {
            string path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);

            path = Directory.GetParent(Directory.GetParent(path).FullName).FullName;

            path += @"\" + fileName;

            string[] arrLine = File.ReadAllLines(path);
            while (arrLine.Count() < line)
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine("empty line created");
                tw.Close();
                arrLine = File.ReadAllLines(path);
            }
            arrLine[line - 1] = content;
            File.WriteAllLines(path, arrLine);

        }
        public void SaveToFile(string fileFullName, string content, int line)
        {
            string path = @fileFullName;

            string[] arrLine = File.ReadAllLines(path);
            while (arrLine.Count() < line)
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine("empty line created");
                tw.Close();
                arrLine = File.ReadAllLines(path);
            }
            arrLine[line - 1] = content;
            File.WriteAllLines(path, arrLine);

        }


        /// <summary>
        /// Copy all the files in folder and Replaces any files with the same name
        /// </summary>
        /// <param name="SourcePath">full SourcePath</param>
        /// <param name="DestinationPath">full DestinationPath</param>
        /// <returns>returns "true" or error message</returns>
        public string CopyFolderAndReplaceSameFileName(string SourcePath, string DestinationPath)
        {
            try
            {
                foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
                return "true";
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }

    }
}
