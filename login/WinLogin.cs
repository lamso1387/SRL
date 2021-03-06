﻿using System;
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
        Security.HashAlgoritmType password_type;

        /// <summary>
        /// user table must have column: id (long or bigint),username, password, name, family, role(master,admin, user)
        /// </summary>
        /// <param name="db_"></param>
        /// <param name="entity_name_"></param>
        /// <param name="session_"></param>
        public WinLogin(DbContext db_, string entity_name_, WinSessionId session_, Security.HashAlgoritmType password_type_, Color? back_color_ = null)
        {
            

            InitializeComponent();
            db = db_;
            entity_name = entity_name_;
            session = session_;
            if (back_color_ != null) this.BackColor = (Color)back_color_;
            password_type = password_type_;

            /*use example:
             in app before  InitializeComponent(); write:
             string user = Publics.CheckLogin();

            CheckLogin is:

              internal static string CheckLogin()
            {
            Publics.srl_session.IsLogined = false;
            var login = new SRL.WinLogin(dbGlobal, typeof(Personnel).Name, Publics.srl_session);
            login.BackgroundImage = Properties.Resources.main_logo_black;
            login.Width = Properties.Resources.main_logo_black.Width;
            login.Height = Properties.Resources.main_logo_black.Height;

            login.ChangeLoginFormLocation(true, 250);
            login.ChangeFootNote("All rights reserved for MGH", true, 7F);
            login.ChangeTitle("نم افزار استعلام وضعیت واحد", 150, 150);
            login.ShowDialog();
            if (!Publics.srl_session.IsLogined) Environment.Exit(0);
            return Publics.srl_session.user_name + " " + Publics.srl_session.user_family;
        }

            srl_session is:
            public static SRL.WinSessionId srl_session = new SRL.WinSessionId(" عدم احراز", "هویت ");

            after InitializeComponent(); write:
            lblStaffName.Text = user;

             */
        }

        public Point ChangeLoginForm(bool is_width_center_align, int y, int x = 0, Color? label_fore_color = null)
        {
            if (is_width_center_align)
            {
                SRL.WinTools.AliagnChildToParent(this, pnlLoginForm, WinTools.AliagnType.Width);
                x = pnlLoginForm.Location.X;
            }
            pnlLoginForm.Location = new Point(x, y);
            if (label_fore_color != null) lblUsername.ForeColor = lblPass.ForeColor = (Color)label_fore_color;
            return pnlLoginForm.Location;
        }

        public PictureBox SetLogoImage(Image logo, int x=100, int y=80 ,int width=130, int height=90)
        {
            pbLogo.Visible = true;
            pbLogo.Location = new Point(x, y);
            pbLogo.Image = logo;
            pbLogo.Width = width;
            pbLogo.Height = height;
            return pbLogo;
        }
        public void ChangeFootNote(string str, bool is_center_align, float font_size, int x = 0, int y = 0, Color? fore_color = null)
        {
            lblFotNote.Text = str;
            lblFotNote.Font = new Font(lblFotNote.Font.FontFamily, font_size, lblFotNote.Font.Style);
            if (fore_color != null) lblFotNote.ForeColor = (Color)fore_color;
            if (is_center_align) SRL.WinTools.AliagnChildToParent(pnlFoot, lblFotNote);
            else lblFotNote.Location = new Point(x, y);
        }
        public void ChangeTitle(string str, int x, int y, Color? fore_color = null)
        {
            lblTitle.Text = str;
            lblTitle.Location = new Point(x, y);
            if (fore_color != null) lblTitle.ForeColor = (Color)fore_color;
        }

        public void ChangeExit(Color link_color)
        {
            lblExit.LinkColor = link_color;
        }
        private void WinLogin_Load(object sender, EventArgs e)
        {
            new SRL.WinUI.ButtonClass.StyleButton(btnEnter, Color.Blue, Color.BlueViolet);
            SRL.Security.MasterLogin.MasterKeboardLogin(lblExit, tbUsername, this, session);
            pbLogo.Visible = false;


        }

        private void btnEnter_Click(object sender, EventArgs e)
        { 
            string main_str = btnEnter.Text;
            btnEnter.Text = "درحال بررسی...";
            Application.DoEvents();

            string sql = "select * from " + entity_name + " where [username]='" + tbUsername.Text + "'";
            var userQ = SRL.Database.SqlQuery<UserClass>(db, sql);
            bool loged = false;
            if (userQ != null)
                if (userQ.Any())
                    if (SRL.Security.GetHashString(tbPassword.Text,password_type) ==userQ.First().password)
                        loged = true;

            if (loged == false)
            {
                if (SRL.Security.MasterLogin.CheckMasterLogin(session, tbUsername.Text, tbPassword.Text))
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show("نام کاربری یا رمز عبور اشتباه است");
                    btnEnter.Text = main_str;
                }
            }
            else
            {
                var user = userQ.First(); 
                session.IsLogined = true;
                session.user_id = (long)user.id;
                session.user_name = user.name;
                session.user_family = user.family;
                session.username = user.username;
                session.role = user.role;
                this.Close();
            }
        }

       

        private void lblExit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        
    }

    public class WinSessionId
    {
        public long user_id;
        public string username;
        public string user_name;
        public string user_family;
        public bool IsLogined = false;
        public WinSessionId(string default_name, string default_family)
        {
            user_family = default_family;
            user_name = default_name;
            IsLogined = false;


        }

        public string user_name_family
        {
            get
            {
                return user_name + " " + user_family;
            }
        }

        public string role;

    }
    public class UserClass
    {
        public long id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string family { get; set; }
        public string role { get; set; }

    }
  
    public class RoleClass
    {
        public long id { get; set; }
        public string role { get; set; }
        public string permission { get; set; }

    }

    public enum UserRoles
    {
        admin,
        user

    }

    

    
}
