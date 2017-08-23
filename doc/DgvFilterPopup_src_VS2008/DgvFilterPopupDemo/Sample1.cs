using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DgvFilterPopup;
using System.Data.OleDb;

namespace DgvFilterPopupDemo {
    public partial class Sample1 : Form
        {
        public Sample1() {
            InitializeComponent();
        }

        public DataTable GetData(string strSQL)
        {
            OleDbDataAdapter DataAdapter = new OleDbDataAdapter(strSQL, Properties.Settings.Default.SampleDBConnectionString);
            DataTable dtable = new DataTable();
            DataAdapter.Fill(dtable);
            DataAdapter.SelectCommand.Connection.Close();
            return dtable;
        }
        
        private void Sample1_Load(object sender, EventArgs e) {

           
            dataGridView1.DataSource = GetData("SELECT * FROM Orders");

            new DgvFilterManager(dataGridView1);
        }
    }
}

