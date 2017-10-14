using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sahmiye
{
    public partial class ProgressControl : UserControl
    {
        public Label lbl_progress
        {
            get { return lblProgress; }
        }
        public ProgressBar progress_bar
        {
            get { return progressBar1; }
        }
        public ProgressControl()
        {/*use:
            var progress = new ProgressControl( );
            pnlProgress.Controls.Add(progress);
            Publics.form_progress_bar_label = progress.lbl_progress;
            Publics.form_progress_bar = progress.progress_bar; 
            Publics.form_progress_bar_label.Parent.Visible = false;
            */

            InitializeComponent();


        }


        private void ProgressControl_Load(object sender, EventArgs e)
        {
            this.VisibleChanged += ProgressControl_VisibleChanged;
        }

        private void ProgressControl_VisibleChanged(object sender, EventArgs e)
        {
           lblCount.Visible= lblPer.Visible = lblProgress.Visible = llCancelProgess.Visible = false;
        }
        

        private async void lblProgress_TextChanged(object sender, EventArgs e)
        {
            this.Visible = true;
            lblProgress.Enabled= lblCount.Visible = lblPer.Visible = lblProgress.Visible = llCancelProgess.Visible = true;

            int progress = (int)double.Parse(lblProgress.Text);
            if (progress > 0) this.Visible = true;
            progressBar1.Value = progress;
            if(lblProgress.Tag!=null) lblCount.Text = lblProgress.Tag.ToString();
            if (progress > 99)
            {
                await SRL.DateTimeLanguageClass.SleepNotBlockUI(1000);
                this.Visible = false;
            }
            
        }

        private void progressBar1_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void llCancelProgess_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lblProgress.Enabled = false;
           
        }
    }
}
