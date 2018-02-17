using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using hesabdari_app.Model;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FormSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Publics.CheckFont();
            Publics.CheckLicense();
            Publics.CheckConnection(this);

            InitializeComponent();

            string user = Public.CheckLogin();//before any window, first show login. but consider migration
            app_setting_class = new Public.AppSettingClass();
            
            CheckAccess();
            lblStaffName.Text = user;
        }

        private void MigrateDatabase()
        {
            // version 1 released
            Dictionary<string, string> migration_version_query = new Dictionary<string, string>();

            // migration_version_query["2"] = "...";
            // migration_version_query["1"] = "ALTER TABLE WorksTB ADD progress_status nvarchar(50);"+migration_version_query["2"];

            if (Public.srl_setting_class.MigrateDatabase(migration_version_query, Assembly.GetExecutingAssembly(), Public.AppSettingClass.SettingKeys.db_version.ToString()) == false)
            {
                SRL.MessageBoxForm2.Show("نسخه پایگاه داده یافت نشد و نرم افزار ممکن است با خطا مواجه شود. با پشتیبان نرم افزار تماس بگیرید.");
            }
        }

        private void CheckAccess()
        {
            SRL.TreeMenuAccess.CheckAccess(Publics.srl_session.role, Publics.dbGlobal, typeof(PermissionTB).Name, menuStrip1);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            Public.AppSettingClass.CheckSetting(menuStrip1.Items);
            //important: check setting before migration because of db_version
            MigrateDatabase();

            this.AutoScroll = true;
            this.Text = "app_name v" + SRL.Security.GetAppVersion().ToString() + " By SRL";
            SRL.ActionManagement.FormActions.ForceExitOnClose(this);
            var progress = new SRL.ProgressControl();
            pnlProgress.Controls.Add(progress);
            Publics.form_progress_bar_label = progress.lbl_progress;
            Publics.form_progress_bar = progress.progress_bar;
            Publics.form_progress_bar_label.Parent.Visible = false;

        }




    }

    public class Publics : SRL.ControlLoad
    {
        public static SRL.WinSessionId srl_session = new SRL.WinSessionId(" عدم احراز", "هویت ");
        public static SRL.SettingClass<SettingTB> srl_setting_class = new SRL.SettingClass<SettingTB>(dbGlobal);

        public static Label form_progress_bar_label;
        public static ProgressBar form_progress_bar ;
        public static SRL.FontClass srl_font = new SRL.FontClass();
        public static string FontFactor
        {
            get
            {
                var f = srl_setting_class.SqlQuerySettingTable("font_factor");
                return f;
            }
            set
            {
                srl_setting_class.ExecuteUpdateSettingTable("font_factor", value);
            }
        }

        public enum SettingKeys
        {
            setting_is_set,
            font_factor,
            base_address,
            api_key,
            pms_key,
            pms_base_address

        }

        public Publics()
        {
        }
        public Publics(Button btn)
            : base(btn)
        {
        }

        public static string CheckLogin()
        {

            Publics.srl_session.IsLogined = false;
            var login = new SRL.WinLogin(dbGlobal, typeof(Personnel).Name, Publics.srl_session);
            login.BackgroundImage = Properties.Resources.main_logo_black;
            login.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            login.Width = Properties.Resources.main_logo_black.Width;
            login.Height = Properties.Resources.main_logo_black.Height * 5;

            login.ChangeLoginForm(true, 250, 0, System.Drawing.Color.Black);
            login.ChangeFootNote("All rights reserved for MGH", true, 7F, 0, 0, System.Drawing.Color.Black);
            login.ChangeTitle("نرم افزار استعلام وضعیت واحد", 150, 150, System.Drawing.Color.Black);
            login.ChangeExit(System.Drawing.Color.Black);
            login.ShowDialog();
            if (!Publics.srl_session.IsLogined) Environment.Exit(0);
            return Publics.srl_session.user_name + " " + Publics.srl_session.user_family;
        }

        internal static void CheckSetting(ToolStripItemCollection menu)
        {// put setting_is_set to last
            Dictionary<string, string> kv = new Dictionary<string, string>();

            kv[SettingKeys.point_factor.ToString()] = "0/1";
            kv[SettingKeys.font_factor.ToString()] = "0/95";
            kv[SettingKeys.printer_name.ToString()] = new PrinterSettings().PrinterName;
            kv[SettingKeys.db_version.ToString()] = SRL.Security.GetAppVersion(Assembly.GetExecutingAssembly()).Major.ToString();

            kv[SettingKeys.setting_is_set.ToString()] = "true";

            if (!Public.srl_setting_class.CheckSetting(SettingKeys.setting_is_set.ToString(), typeof(SettingKeys), kv))
            {
                SRL.MessageBoxForm2.Show("به نرم افزار حامی خوش آمدید. تنظیمات پیش فرض در نرم افزار ذخیره شد. برای تغییر به منوی تنظیمات بروید.", "تغییرات پیش فرض اعمال شد", MessageBoxForm2.Buttons.OK, MessageBoxForm2.Icon.Info, MessageBoxForm2.AnimateStyle.FadeIn);
                menu["miSetting"].PerformClick();

            }
            else menu["miSell"].PerformClick();
        }

        internal static void CheckLicense()
        {
            SRL.WinLicenseForm li_form = new SRL.WinLicenseForm(Assembly.GetExecutingAssembly(), true);
            li_form.CheckLicenseKey<AppLicenseClass>();
            if (!li_form.IsDisposed && !li_form.is_activated_before) li_form.ShowDialog();

        }

        internal static void CheckFont()
        {
            srl_font.InstallFont(srl_font.GetContentFontNameFromByte(Properties.Resources.irsan, "irsan.ttf"));
        }

        internal static void CheckConnection(Control control)
        {
            string db_path = System.AppDomain.CurrentDomain.BaseDirectory + "MyDatabase.sqlite";
            string con_str = @"metadata=res://*/Model.Model1.csdl|res://*/Model.Model1.ssdl|res://*/Model.Model1.msl;provider=System.Data.SQLite.EF6;provider connection string='data source=" + db_path + "'";

            SRL.Database.UpdateConnectionStringAndRestart(con_str, typeof(HesabdariEntities).Name, control);

        }


    }


}
