using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Entity;


namespace SRL
{

    /// <summary>
    /// example in app:
    /// public static string CheckLogin()
    ///  {
    ///     Public.srl_session.IsLogined = false;
    ///     new SRL.WinLogin(new HesabdariEntities(), "personnel", Public.srl_session).ShowDialog();
    ///     if (!Public.srl_session.IsLogined) Environment.Exit(0);
    ///     return Public.srl_session.user_name + " " + Public.srl_session.user_family;
    ///  }
    /// </summary>
    public partial class WinLogin : Form
    {
        DbContext db;
        string entity_name;
        WinSessionId session;

        /// <summary>
        /// user table must have column: ID (long or bigint),username, password, name, family, role(master, user)
        /// </summary>
        /// <param name="db_"></param>
        /// <param name="entity_name_"></param>
        /// <param name="session_"></param>
        public WinLogin(DbContext db_, string entity_name_, WinSessionId session_, Color? back_color_=null)
        {
            InitializeComponent();
            db = db_;
            entity_name = entity_name_;
            session = session_;
            if (back_color_ != null) this.BackColor = (Color) back_color_;
        }

        public Point ChangeLoginFormLocation(int x, int y)
        {
            pnlLoginForm.Location = new Point(x, y);
            return pnlLoginForm.Location;
        }
        public void ChangeFootNote(string str, int x, int y)
        {
            lblFotNote.Text = str;
            lblFotNote.Location = new Point(x, y);
        }
        public void ChangeTitle(string str, int x, int y)
        {
            lblTitle.Text = str;
            lblTitle.Location = new Point(x, y);
        }
        private void WinLogin_Load(object sender, EventArgs e)
        {
            new SRL.WinUI.StyleButton(btnEnter, Color.Blue, Color.BlueViolet);
            
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            string main_str = btnEnter.Text;
            btnEnter.Text = "درحال بررسی...";
            Application.DoEvents();

            string sql = "select ID,username,password, name, family from " + entity_name + " where username='" + tbUsername.Text + "'";
            var user = new SRL.Database().SqlQuery<UserClass>(db, sql);
            if (!user.Any() || !(tbPassword.Text == user.First().password))
            {
                MessageBox.Show("نام کاربری یا رمز عبور اشتباه است");
                btnEnter.Text = main_str;
                return;
            }
            session.IsLogined = true;
            session.user_id =(long) user.First().ID;
            session.user_name = user.First().name;
            session.user_family = user.First().family;
            this.Close();
        }

        private void lblExit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        
    }

    public class WinSessionId
    {
        public long user_id;
        public string user_name;
        public string user_family;
        public bool IsLogined = false;
        public WinSessionId(string default_name, string default_family)
        {
            user_family = default_family;
            user_name = default_name;

        }

    }
    public  class UserClass
    {
        public Nullable<long> ID { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string family { get; set; }
        public string password { get; set; }
        public string role { get; set; }

    }

    public enum UserRoles
    {
    admin,
    user

    }
}
