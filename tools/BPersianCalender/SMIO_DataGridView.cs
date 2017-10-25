using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace SRL
{
    public partial class SMIO_DataGridView : DataGridView
    {
        public bool IsDate { get; set; }
        public SMIO_DataGridView()
        {
            InitializeComponent();
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            if (this.Columns[e.ColumnIndex].Name == "fDate")
            {
                ShortFormDateFormat(e);
            }

        }
        private static void ShortFormDateFormat(DataGridViewCellFormattingEventArgs formatting)
        {
            if (formatting.Value != null)
            {
                if (!string.IsNullOrEmpty(formatting.Value.ToString()))
                {


                    try
                    {
                        System.Text.StringBuilder dateString = new System.Text.StringBuilder();
                        string RecivedDate = formatting.Value.ToString();
                        if (!RecivedDate.Contains("/"))
                        {
                            dateString.Append(RecivedDate.Substring(0, 4));
                            dateString.Append("/");
                            dateString.Append(RecivedDate.Substring(4, 2));
                            dateString.Append("/");
                            dateString.Append(RecivedDate.Substring(6, 2));
                            formatting.Value = dateString.ToString();
                            formatting.FormattingApplied = true;

                        }
                    }
                    catch (FormatException)
                    {
                        formatting.FormattingApplied = false;
                    }
                }
            }
        }
    }
}
