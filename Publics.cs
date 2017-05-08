using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Forms;
using System.Data;
using System.Data.Entity;

namespace SRL
{
    public class Security
    {
        public enum UserRegistrationStatus
        {
            NotRegistered = 0,
            NotActivated = 1,
            Activated = 2
        }
        public void CreateSession(string key, object value, System.Web.UI.Page page)
        {
            page.Session[key] = value;
        }
        public void LoginRedirect(System.Web.SessionState.HttpSessionState session, System.Web.HttpResponse response,string redirectUri)
        {
            if (session["username"] == null)
                response.Redirect(redirectUri);
            else
            {
                //  MessageBox(session["username"].ToString(), response);
            }
        }
        public void SendActivationEmail(string username, string registerHashValue,string registerActivationUri, string toMail,string subject,string body, Dictionary<string, object> response, string fromMail, string password)
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
                string activationLink =convertor.MakeActivationLink(username,registerHashValue,registerActivationUri);
                mailMessage.Body += activationLink;
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential(fromMail, password);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
                response["emailSent"]= true;
            }
            catch (Exception ex)
            {
                response["emailSent"]= false;
                response["emailError"]= ex.Message;
            }
        }
        public bool RedirectIfNotLogin(System.Web.UI.Page page, Dictionary<string,object> response, string redirect)
        {
            var usernameSession = page.Session["username"];
            if (usernameSession == null)
            {
                response["redirect"]= redirect;
                return false;
            }
            else
            {
                response["username"]= usernameSession;
                return true;
            }
        }
      
    }
    public class KeyValue
    {
        public void AddItem(Dictionary<string, object> result,string key, object value)
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

        public void PostAsJsonAsync(Dictionary<string,object> response, System.Net.Http.HttpClient client,string uri, object input)
        {
            System.Net.Http.HttpResponseMessage httpResponse = client.PostAsJsonAsync(uri, input).Result;
            string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
            response["httpResponse"] = httpResponse;
            response["responseContent"] = responseContent;
        }
    }
    public class WebResponse
    {
            
            public  void WebMessageBox(string message, System.Web.HttpResponse response)
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

    }
    public class Json
    {
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
    }
    public class WinChart
    {
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
    public class Database
    {
        public void TruncateTable( DbContext db, string table_name)
        {
            db.Database.ExecuteSqlCommand("truncate table " + table_name);
            db.SaveChanges();
        }
        public void ExecuteQuery(DbContext db, string query)
        {
            db.Database.ExecuteSqlCommand(query);
            db.SaveChanges();

        }

    }
    public class Excel
    {
        public void LoadDGVFromExcel(OpenFileDialog ofDialog, Label lblFileName, string[] main_headers, DataGridView dgv)
            {
                ofDialog.Filter = "Only 97/2003 excel with one sheet|*.xls";
                ofDialog.ShowDialog();
                lblFileName.Text = ofDialog.FileName;

                ExcelLibrary.Office.Excel.Workbook excel_file = ExcelLibrary.Office.Excel.Workbook.Open(ofDialog.FileName);
                var worksheet = excel_file.Worksheets[0]; // assuming only 1 worksheet
                var cells = worksheet.Cells;

                if (CheckExcelHeaders(cells, main_headers))
                {

                    // add columns
                    foreach (var header in cells.GetRow(cells.FirstRowIndex))
                    {
                        dgv.Columns.Add(header.Value.StringValue, header.Value.StringValue);
                    }

                    // add rows
                    for (int rowIndex = cells.FirstRowIndex + 1; rowIndex <= cells.LastRowIndex; rowIndex++)
                    {
                        ExcelLibrary.Office.Excel.Row file_row = cells.GetRow(rowIndex);
                        List<object> file_row_list = new List<object>();
                        foreach (var file_row_cell in file_row)
                        {
                            file_row_list.Add(file_row_cell.Value.Value);
                        }
                        dgv.Rows.Add(file_row_list.ToArray());

                        //dgv.Rows.Add(file_row.GetCell(0).Value, file_row.GetCell(1).Value, file_row.GetCell(2).Value, file_row.GetCell(3).Value, file_row.GetCell(4).Value,
                        //    file_row.GetCell(5).Value, file_row.GetCell(6).Value, file_row.GetCell(7).Value, file_row.GetCell(8).Value, file_row.GetCell(9).Value, file_row.GetCell(10).Value,
                        //    file_row.GetCell(11).Value, file_row.GetCell(12).Value, file_row.GetCell(13).Value, file_row.GetCell(14).Value, file_row.GetCell(15).Value, file_row.GetCell(16).Value,
                        //    file_row.GetCell(17).Value, file_row.GetCell(18).Value, file_row.GetCell(19).Value, file_row.GetCell(20).Value, file_row.GetCell(21).Value, file_row.GetCell(22).Value, file_row.GetCell(23).Value,
                        //    file_row.GetCell(24).Value, file_row.GetCell(25).Value, file_row.GetCell(26).Value, file_row.GetCell(27).Value, file_row.GetCell(28).Value, file_row.GetCell(29).Value, file_row.GetCell(30).Value
                        //    );
                    }

                }
            }
        public bool CheckExcelHeaders(ExcelLibrary.Office.Excel.CellCollection cells, string[] main_headers)
        {
            foreach (var file_header in cells.GetRow(cells.FirstRowIndex))
            {
                Application.DoEvents();
                if (!main_headers.Contains(file_header.Value.StringValue))
                {
                    MessageBox.Show(file_header.Value.StringValue + " is not valid.");
                    return false;
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
                    MessageBox.Show("file does not have column: " + main_header);
                    return false;
                }
                else continue;
            }

            return true;

        }
        public void ExportToExcell(DataGridView dgview, int devider,string path)
        {//"C:\Users\project\Desktop\exported.xls"
            DataSet ds = new DataSet();
            int table_count = dgview.Rows.Count / devider;
            int index = 0;
            for (int i = 0; i < table_count; i++)
            {
                DataTable table = new DataTable(i.ToString());
                ds.Tables.Add(table);
                new Convertor().MakeDataTableFromDGV(dgview, table, devider, index);
                index += devider;
            }
            DataTable _table = new DataTable("else");
            ds.Tables.Add(_table);
            new Convertor().MakeDataTableFromDGV(dgview, _table, devider, index);

            ExcelLibrary.DataSetHelper.CreateWorkbook(@path, ds);
        }

        
    }

    public class SrlButton : IDisposable
    {
        private Button btn_loader { get; set; }

        int foreach_looper = 0;

        public SrlButton()
        {
        }

        public SrlButton(Button btnLoader)
        {
            btn_loader = btnLoader;
            btn_loader.Tag = btn_loader.Text;
        }

        public void Dispose()
        {
            if (btn_loader != null) btn_loader.Text = btn_loader.Tag.ToString();
        }

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

    }

}
