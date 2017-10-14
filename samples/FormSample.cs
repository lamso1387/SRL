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
            MigrateDatabase();
            Publics.CheckSetting();
            string user = Public.CheckLogin();

            InitializeComponent();
            CheckAccess();
            lblStaffName.Text = user;

        }

        private void MigrateDatabase()
        {
            // version 2 released
            Dictionary<string, string> migration_version_query = new Dictionary<string, string>();

            // migration_version_query["2"] = "...";
            // migration_version_query["1"] = "ALTER TABLE WorksTB ADD progress_status nvarchar(50);"+migration_version_query["2"];


            Publics.srlsetting.MigrateDatabase(migration_version_query);
        }
        private void CheckAccess()
        {
            SRL.TreeMenuAccess.CheckAccess(Publics.srl_session.role, Publics.dbGlobal, typeof(PermissionTB).Name, menuStrip1);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
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

        internal static void CheckSetting()
        {

            if (!Publics.srl_setting_class.CheckSettingIsSet())
            {
                Dictionary<string, string> kv = new Dictionary<string, string>();
                kv["setting_is_set"] = "true";
                kv["font_factor"] = "0/95";
                kv["base_address"] = "https://app.nwms.ir/v2/b2b-api/";
                kv["api_key"] = "2050130318";
                srlsetting.InitiateSetting(kv);
            }

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
