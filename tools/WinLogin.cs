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


    public partial class WinLogin : Form
    {
        DbContext db;
        string entity_name;
        WinSessionId session;

        /// <summary>
        /// user table must have column: ID (long or bigint),username, password, name_family
        /// </summary>
        /// <param name="db_"></param>
        /// <param name="entity_name_"></param>
        /// <param name="session_"></param>
        public WinLogin(DbContext db_, string entity_name_, WinSessionId session_)
        {
            InitializeComponent();
            db = db_;
            entity_name = entity_name_;
            session = session_;
        }


        private void btnEnter_Click(object sender, EventArgs e)
        {
            string main_str = btnEnter.Text;
            btnEnter.Text = "درحال بررسی...";
            Application.DoEvents();

            string sql = "select ID,username,password, name_family from " + entity_name + " where username='" + tbUsername.Text + "'";
            var user = new SRL.Database().SqlQuery<User>(db, sql);
            if (!user.Any() || !(tbPassword.Text == user.First().password))
            {
                MessageBox.Show("نام کاربری یا رمز عبور اشتباه است");
                btnEnter.Text = main_str;
                return;
            }
            session.IsLogined = true;
            session.user_id = user.First().ID;
            session.user_name_family = user.First().name_family;
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
        public string user_name_family;
        public bool IsLogined = false;
        public WinSessionId(string default_name_family)
        {
            user_name_family = default_name_family;

        }

    }
    public  class User
    {
        public long ID { get; set; }
        public string username { get; set; }
        public string name_family { get; set; }
        public string password { get; set; }

    }
}