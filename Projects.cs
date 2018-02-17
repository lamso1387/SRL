using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace SRL
{
    public class Projects
    {
        public class Nwms
        {
            public class Operations
            {
                //using System.Net.Http;
                //using  System.Net.Http.Formatting;
                //add referense : Newtonsoft.Json.dll , System.Net.Http.Formatting.dll

                //  Nwms.WarehouseSearch();

                //کدپستی و کلید امنیتی را قرار دهید
                //Nwms.GetComplexByPostalCode("7765215520", "");

                //متد ثبت موقت رسید
                //  string id = Nwms.AnbarOperation.Receipt("2050130318", "2050130351", "4713644457","");
                // Nwms.AnbarOperation.FinalizeReceipt(id,  "2050130318", "2050130351");

                // Nwms.GetWarehousesFromSearch("2050130318", 1455926400, 1514628894);


                public class WarehouseSearchResult
                {
                    public long create_date { get; set; }
                    public List<string> supervisor_org { get; set; }
                    public string account_status { get; set; }
                    public string postal_code { get; set; }
                    public string id { get; set; }
                    public List<string> activity_sector { get; set; }
                    public string type { get; set; }
                    public List<Contractor> contractors { get; set; }
                    public string org_creator_national_id { get; set; }
                    public string name { get; set; }

                    public class Contractor
                    {
                        public string name { get; set; }
                        public string national_id { get; set; }
                    }



                }

                public static List<WarehouseSearchResult> WarehouseSearch(string api_key,
                     long date_from, long date_to, int start, int size)
                {
                    try
                    {

                        Dictionary<string, object> filter = new Dictionary<string, object>();
                        filter["account_status"] = "1";
                        Dictionary<string, object> date_filter = new Dictionary<string, object>();
                        date_filter["$gte"] = date_from;
                        date_filter["$lte"] = date_to;
                        filter["create_date"] = date_filter;

                        HttpClient client = new HttpClient();

                        client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/warehouse/_search/");
                        client.Timeout = new TimeSpan(0, 30, 0);
                        var result = client.PostAsJsonAsync(start + "/" + size, filter).Result;
                        if (result.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            Console.Write(result.Content.ReadAsStringAsync().Result);
                            return null;
                        }
                        else
                        {
                            var response = result.Content.ReadAsStringAsync().Result;

                            string data =
                                        Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(response)["data"].ToString();
                            // data = System.Text.RegularExpressions.Regex.Unescape(data);
                            List<WarehouseSearchResult> list =
                                Newtonsoft.Json.JsonConvert.DeserializeObject<List<WarehouseSearchResult>>(data);

                            return list;

                        }


                    }
                    catch (Exception ex)
                    {

                        return null;
                    }
                }

                public class EstelamResult
                {

                    public bool warehouse_server { get; set; }
                    public bool postal_code_server { get; set; }
                    public string province { get; set; }
                    public string village { get; set; }
                    public string township { get; set; }
                    public string city { get; set; }
                    public string full_address { get; set; }

                    public string name { get; set; }
                    public string id { get; set; }
                    public string org_creator_national_id { get; set; }
                    public string gov_wh_number { get; set; }

                    public Agent agent { get; set; }
                    public List<Owner> owners { get; set; }
                    public List<Warehouse> warehouses { get; set; }

                    public class Agent
                    {
                        public string name { get; set; }
                        public string id { get; set; }
                        public string national_id { get; set; }
                        public string mobile { get; set; }
                    }


                    public class Warehouse
                    {
                        public string name { get; set; }
                        public string id { get; set; }
                        public string org_creator_national_id { get; set; }
                        public string type { get; set; }
                        public List<string> activity_sector { get; set; }
                        public List<string> supervisor_org { get; set; }
                        public List<Contractor> contractors { get; set; }
                    }

                    public class Owner
                    {

                        public string name { get; set; }
                        public string national_id { get; set; }
                        public string mobile { get; set; }
                    }

                    public class Contractor
                    {

                        public string name { get; set; }
                        public string id { get; set; }
                        public string national_id { get; set; }
                        public string mobile { get; set; }
                    }

                }
                public static void GetComplexByPostalCode(string postal_code, string api_key)
                {

                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api/");

                    HttpResponseMessage message = client.GetAsync(api_key + "/complex_by_post_code/" + postal_code).Result;
                    string result = message.Content.ReadAsStringAsync().Result;

                    string result_encode = System.Text.RegularExpressions.Regex.Unescape(result);
                    //  MessageBox.Show(result_encode);

                    if (message.StatusCode == System.Net.HttpStatusCode.OK)
                    {// در این حالت استعلام انجام می شود و خروجی بصورت زیر استخراج می گردد:

                        //add 'Newtonsoft.Json.dll' reference:
                        EstelamResult estelam_result = Newtonsoft.Json.JsonConvert.DeserializeObject<EstelamResult>(result_encode);

                        //اگر کدپستی در سامانه انبار ثبت نام شده باشد مقدار برابر ترو در غیر اینصورت فالس است:
                        //اگر مقدار زیر برابر ترو باشد، یعنی در سامانه انبار ثبت شده باشد، حتما در شرکت پست هم وجود دارد و صحیح است
                        bool warehouse_server = estelam_result.warehouse_server;


                        if (warehouse_server == false)
                        {
                            //اگر کدپستی در شرکت پست وجود داشته باشد مقدار برابر ترو در غیر اینصورت فالس است
                            bool postal_code_server = estelam_result.postal_code_server;
                        }

                        //اگر کدپستی در یکی از سامانه ها (سامانه انبار یا شرکت پست) وجود داشته باشند اطلاعات آدرسی مقدار دارند

                        //استان
                        string province = estelam_result.province;

                        //شهرستان
                        string township = estelam_result.province;

                        //شهر
                        string city = estelam_result.city;

                        //دهستان در صورت وجود
                        string village = estelam_result.village;

                        //ادرس کامل
                        string full_address = estelam_result.full_address;


                        //اگر کدپستی در سامانه انبار ثبت نام شده باشد اطلاعات واحدها و مالکین و بهره برداران قابل دریافت است:
                        if (warehouse_server)
                        {
                            //نام مجتمع ثبت نام شده -مجتمع معادل کدپستی ثبت نام شده است
                            string name = estelam_result.name;

                            //کد یکتای مجتمع :
                            string id = estelam_result.id;

                            //کد سازمان ثبت کننده مجتمع بصورت وب سرویسی که میتواند نال باشد:
                            string org_creator_national_id = estelam_result.org_creator_national_id;

                            // شماره مجتمع در سازمان که می تواند نال باشد
                            string gov_wh_number = estelam_result.gov_wh_number;

                            //اگر برای مجتمع مالک یا مالکین ثبت شده باشد:
                            if (estelam_result.agent != null)
                            {//یکی از مالکین بعنوان مالک اصلی یا نماینده مالکین شناخته می شود

                                //مالک اصلی
                                string agent_name = estelam_result.agent.name;

                                //کدیکتای نماینده
                                string agent_id = estelam_result.agent.id;

                                //کدملی نماینده
                                string agent_national_id = estelam_result.agent.national_id;

                                //موبایل نماینده:
                                string agent_mobile = estelam_result.agent.mobile;

                                //اگر مجتمع دارای بیش از یک مالک باشد می توان اطلاعات همه مالکین را دریافت کرد:
                                foreach (var owner in estelam_result.owners)
                                {//نماینده مالکین جز یکی از مالکان است

                                    //مالک
                                    string owner_name = owner.name;

                                    //کدملی مالک
                                    string owner_national_id = owner.national_id;

                                    //موبایل مالک
                                    string owner_mobile = owner.mobile;
                                }
                            }

                            //در یک کدپستی یا مجتمع می تواند چندین واحد ثبت شود برای مثال در یک کدپستی دو فرد می توانند واحد جداگانه ثبت کنند.

                            //اگر برای یک مجتمع یا کدپستی واحدهای زیر مجموعه ای ثبت شده باشد:
                            if (estelam_result.warehouses != null)
                                foreach (var warehouse in estelam_result.warehouses)
                                {
                                    //نام واحد:
                                    string warehouse_name = warehouse.name;

                                    //کدیکتای واحد
                                    string warehouse_id = warehouse.id;

                                    //کدسازمان ثبت کننده واحد بصورت وب سرویسی که می تواند نال باشد
                                    string warehouse_org_creator_national_id = warehouse.org_creator_national_id;

                                    //نوع واحد دارای یکی از مقادیر زیر:
                                    //0 برای اصناف
                                    //1 برای باغ و مزرعه
                                    //2 برای معدن
                                    // 3 برای تولیدی ها
                                    //4 برای انبار و مراکز نگهداری کالا
                                    //5 برای مراکز ارائه خدمات
                                    string warehouse_type = warehouse.type;

                                    //حوزه فعالیت واحد که دو رقم اول کد  ایسیک است و یک واحد می تواند چند حوزه فعالیت داشته باشد
                                    foreach (string activity_sector in warehouse.activity_sector) { string warehouse_activity_sector = activity_sector; }

                                    //کد دولتی یا خصوصی ناظر واحد که میتواند نال یا چند مورد باشد
                                    //نام متناظر با کد را از پشتیبان سامانه تحویل بگیرید
                                    if (warehouse.supervisor_org != null)
                                        foreach (string supervisor_org in warehouse.supervisor_org) { string warehouse_supervisor_org = supervisor_org; }

                                    //در سامانه انبار یک مجتمع یا کدپستی دارای مالک /مالکین است و یک واحد دارای بهره بردار (مستاجر) است 
                                    //مالک مجتمع میتواند همان بهره بردار باشد
                                    // یک واحد میتواند بهره بردار نداشته باشد یا چندین بهره بردار داشته باشد
                                    if (warehouse.contractors != null)
                                        foreach (var contractor in warehouse.contractors)
                                        {
                                            //بهره بردار
                                            string contractor_name = contractor.name;

                                            //کدیکتای بهره بردار
                                            string contractor_id = contractor.id;

                                            //کدملی بهره بردار
                                            string contractor_national_id = contractor.national_id;

                                            //موبایل بهره بردار
                                            string contractor_mobile = contractor.mobile;
                                        }
                                }
                        }

                    }
                    else
                    {// در این حالت استعلام انجام نشده و خروجی خطا بصورت زیر قابل مشاهده است:
                        Console.Write(System.Text.RegularExpressions.Regex.Unescape(result));
                    }

                }

                public class AnbarOperation
                {

                    public class AdditionalData
                    {
                        public string good_owneragent { get; set; }
                        public string owner_phone { get; set; }

                        //public AdditionalData()
                        //{
                        //    good_owneragent = "";
                        //    owner_phone = "";
                        //}
                    }

                    public class ReceiptItem
                    {
                        public string category_id { get; set; }
                        public string taxonomy_id { get; set; }
                        public string good_id { get; set; }
                        public string measurement_unit { get; set; }
                        public int count { get; set; }
                        public int total_weight { get; set; }
                        public string package_type { get; set; }
                        public int package_count { get; set; }
                        public int item_value { get; set; }
                        public string location { get; set; }
                        public int production_date { get; set; }
                        public int expire_date { get; set; }
                        public string description { get; set; }
                        public List<object> receipt_shares = new List<object>();
                        public List<object> tracking_list = new List<object>();

                        //public ReceiptItem()
                        //{
                        //    category_id = "";
                        //    taxonomy_id = "";
                        //    good_id = "";
                        //    measurement_unit = "";
                        //    package_type = "";
                        //    location = "";
                        //    description = "";
                        //}

                    }

                    public class SimpleReceipt
                    {
                        public string number { get; set; }
                        public string owner_name { get; set; }
                        public string owner { get; set; }
                        public double rcp_date { get; set; }
                        public string warehouse_id { get; set; }
                        public string postal_code { get; set; }
                        public string doc_type { get; set; }
                        public string vehicle_number { get; set; }
                        public string driver_national_id { get; set; }
                        public string driver { get; set; }
                        public string insurance_name { get; set; }
                        public int insurance_date { get; set; }
                        public string insurance_number { get; set; }
                        public int weight_impure { get; set; }
                        public int weight_pure { get; set; }
                        public AdditionalData additional_data { get; set; }
                        public List<ReceiptItem> receipt_items = new List<ReceiptItem>();
                    }

                    public class SimpleGoodIssue
                    {
                        public string doc_type { get; set; }
                        public int weight_pure { get; set; }
                        public string vehicle_number { get; set; }
                        public string number { get; set; }
                        public string warehouse_id { get; set; }
                        public string postal_code { get; set; }
                        public string owner { get; set; }
                        public string keeper { get; set; }
                        public double goods_issue_date { get; set; }
                        public int weight_impure { get; set; }
                        public string driver_national_id { get; set; }
                        public string driver { get; set; }
                        public string insurance_name { get; set; }
                        public int insurance_date { get; set; }
                        public string insurance_number { get; set; }
                        public string owner_name { get; set; }
                        public List<GoodsIssueItem> goods_issue_items = new List<GoodsIssueItem>();


                    }

                    public class GoodsIssueItem
                    {
                        public string category_id { get; set; }
                        public string taxonomy_id { get; set; }
                        public string good_id { get; set; }
                        public string measurement_unit { get; set; }
                        public int count { get; set; }
                        public int total_weight { get; set; }
                        public string package_type { get; set; }
                        public int package_count { get; set; }
                        public int production_date { get; set; }
                        public int expire_date { get; set; }
                        public string location { get; set; }

                        //public GoodsIssueItem()
                        //{
                        //    category_id = "";
                        //    taxonomy_id = "";
                        //}
                    }


                    public static bool Receipt(string api_key,string contractor_national_id, AnbarOperation.SimpleReceipt simple_receipt, out string result)
                    {
                        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                        client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api-imp/");
                        string input = JsonConvert.SerializeObject(simple_receipt, Formatting.Indented);
                        HttpResponseMessage response = client.PostAsJsonAsync(api_key + "/" + contractor_national_id + "/receipt/simple", simple_receipt).Result;

                        result = response.Content.ReadAsStringAsync().Result;
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            return false;
                        }
                        else
                        {
                            //ثبت سند بصورت موقت انجام شد
                            Dictionary<string, object> output = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                            string id = output["id"].ToString();
                            int version = int.Parse(output["version"].ToString());
                            result = id;
                            return true;
                        }


                    }

                    public static SimpleReceipt CreateSimpleReceipt(string postal_code, string warehouse_id)
                    {
                        AnbarOperation.SimpleReceipt simple_receipt = new AnbarOperation.SimpleReceipt();
                        //شماره داخلی رسید- این شماره توسط انبار وارد می شود و باید غیرتکراری باشد  *
                        simple_receipt.number = Guid.NewGuid().ToString();
                        //تاریخ صدور رسید از نوع epoch:*
                        //تاریخ صدور باید به میلادی تبدیل و سپس از 1970/1/1 کم شود و تعداد ثانیه ها بدست آید
                        simple_receipt.rcp_date = (new System.Globalization.PersianCalendar().ToDateTime(1396, 8, 15, 0, 0, 0, 0) - new DateTime(1970, 1, 1)).TotalSeconds;

                        //کد پستی انبار*
                        simple_receipt.postal_code = postal_code;


                        //در صورتی که برای کدملی و کدپستی بیش از یک انبار وجود داشته باشد باید متغیر زیر که کدانبار می باشد
                        //ارسال گردد. در صورتی که یک انبار ثبت شده است همان کدپستی کافی است
                        if (!string.IsNullOrWhiteSpace(warehouse_id)) simple_receipt.warehouse_id = warehouse_id;

                        //نوع رسید*
                        //0 برای بدون مرجع
                        //1 برای بارنامه
                        //2 برای حواله
                        simple_receipt.doc_type = "0";
                        //کدملی راننده:*
                        simple_receipt.driver_national_id = "";
                        //نام و نام خانوادگی راننده:
                        simple_receipt.driver = "سهیل رمضان زاده";
                        //شماره پلاک*
                        simple_receipt.vehicle_number = "45ب874ایران65";
                        //تاریخ صدور بیمه
                        simple_receipt.insurance_date = 1640118600;
                        //نام بیمه نامه
                        simple_receipt.insurance_name = "بیمه آسیا";
                        //شماره بیمه نامه
                        simple_receipt.insurance_number = "1254";
                        //کدملی یا شناسه ملی مالک کالا:*
                        simple_receipt.owner = "";
                        //نام کامل مالک کالا حقیقی یا حقوقی
                        simple_receipt.owner_name = "سهیل رمضان زاده";
                        //وزن بار بهمراه ناوگان- وزن ناخالص به کیلوگرم*
                        simple_receipt.weight_impure = 2500;
                        //وزن بار بدون ناوگان-وزن خالص به کیلوگرم*
                        simple_receipt.weight_pure = 500;

                        //اقلام کالایی رسید بصورت زیر بدست می آید
                        AnbarOperation.ReceiptItem receipt_item = new AnbarOperation.ReceiptItem();
                        //شناسه کالا:*
                        receipt_item.good_id = "0002";
                        //واحد اندازگیری:
                        // 0002	کیلوگرم	برای
                        //0003	مثقال	برای مثقال
                        //0004	لیتر	برای
                        //0005	تن	برای
                        //0001	برای عدد
                        //؟
                        receipt_item.measurement_unit = "0001";
                        //مقدار برحسب واحد اندازه گیری:*
                        receipt_item.count = 10;
                        //نوع بسته بندی:
                        //002	کارتن	
                        //001	پاکت	
                        //003	پالت	
                        //004	قراصه	
                        //005	گونی	
                        //006	بدون بسته بندی	
                        //007	فلّه
                        receipt_item.package_type = "002";
                        //تعداد بسته
                        receipt_item.package_count = 2;

                        receipt_item.location = "محل نگهداری";
                        //تاریخ تولید epoch:
                        receipt_item.production_date = 1324499400;
                        //تاریخ انقضا epoch:*
                        //باید بعد از زمان جاری و بعد از تاریخ تولید باشد
                        receipt_item.expire_date = 1684771400;
                        //وزن کل ردیف کالا به کیلوگرم
                        receipt_item.total_weight = 5000;

                        //می تواند چندین ریف کالایی در یک سند افزود
                        simple_receipt.receipt_items.Add(receipt_item);

                        //اطلاعات اختیاری:
                        AnbarOperation.AdditionalData additional_data = new AnbarOperation.AdditionalData();
                        additional_data.good_owneragent = "نام و نام خانوادگی نماینده مالک";
                        additional_data.owner_phone = "تلفن مالک یا نماینده مالک";

                        simple_receipt.additional_data = additional_data;

                        return simple_receipt;


                    }

                    public static bool FinalizeReceipt(string id, string api_key, string contractor_national_id, out string result_final)
                    {

                        //جهت نهایی سازی سند:
                        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                        client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api-imp/");
                        HttpResponseMessage response_final = client.PostAsync(api_key + "/" + contractor_national_id + "/receipt/" + id + "/finalize", null).Result;

                        result_final = response_final.Content.ReadAsStringAsync().Result;
                        if (response_final.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            return false;
                        }
                        else
                        {//در این صورت سند قطعی شده است
                            Dictionary<string, object> output_final = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result_final);
                            string status = output_final["status"].ToString(); // status == "PERMANENT"
                            if (status == "PERMANENT") return true;
                            else return false;

                        }

                    }

                    private void btnHavale_Click(object sender, EventArgs e)
                    {//متد ثبت موقت حواله
                        HttpResponseMessage response = SendGoodIssueSimple();
                        string result = response.Content.ReadAsStringAsync().Result;
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine(result);

                        }
                        else
                        {
                            //ثبت سند بصورت موقت انجام شد
                            Dictionary<string, object> output = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                            string id = output["id"].ToString();
                            int version = int.Parse(output["version"].ToString());
                            //جهت نهایی سازی سند:
                            HttpResponseMessage response_final = FinalizeGoodIsuue(id);
                            string result_final = response_final.Content.ReadAsStringAsync().Result;
                            if (response_final.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                Console.WriteLine(result_final);
                            }
                            else
                            {//در این صورت سند قطعی شده است
                                Dictionary<string, object> output_final = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result_final);
                                string status = output_final["status"].ToString(); // status == "PERMANENT"
                            }
                        }

                    }

                    private HttpResponseMessage FinalizeGoodIsuue(string id)
                    {
                        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                        client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api-imp/");
                        return client.PostAsync("2050130000/2050130000/goods_issue/" + id + "/finalize", null).Result;
                    }

                    private HttpResponseMessage SendGoodIssueSimple()
                    {
                        AnbarOperation.SimpleGoodIssue simple_good_issue = new AnbarOperation.SimpleGoodIssue();
                        //شماره داخلی حواله- این شماره توسط انبار وارد می شود و باید غیرتکراری باشد  *
                        simple_good_issue.number = Guid.NewGuid().ToString();
                        //تاریخ صدور رسید از نوع epoch:*
                        //تاریخ صدور باید به میلادی تبدیل و سپس از 1970/1/1 کم شود و تعداد ثانیه ها بدست آید
                        simple_good_issue.goods_issue_date = (new System.Globalization.PersianCalendar().ToDateTime(1395, 12, 2, 14, 12, 01, 0) - new DateTime(1970, 1, 1)).TotalSeconds;

                        //کد پستی انبار*
                        simple_good_issue.postal_code = "5691947637";
                        //در صورتی که برای کدملی و کدپستی بیش از یک انبار وجود داشته باشد باید متغیر زیر که کدانبار می باشد
                        //ارسال گردد. در صورتی که یک انبار ثبت شده است همان کدپستی کافی است
                        //simple_receipt.warehouse_id = "a8647d57dd784867ba5c172eb59156f4";

                        //نوع رسید*
                        //0 برای بدون مرجع
                        //1 برای رسید
                        //2 برای معرفی نامه
                        simple_good_issue.doc_type = "0";
                        //کدملی انباردار
                        simple_good_issue.keeper = "";
                        //کدملی راننده:*
                        simple_good_issue.driver_national_id = "";
                        //نام و نام خانوادگی راننده:
                        simple_good_issue.driver = "سهیل رمضان زاده";
                        //شماره پلاک*
                        simple_good_issue.vehicle_number = "45ب874ایران65";
                        //تاریخ صدور بیمه
                        simple_good_issue.insurance_date = 1640118600;
                        //نام بیمه نامه
                        simple_good_issue.insurance_name = "بیمه آسیا";
                        //شماره بیمه نامه
                        simple_good_issue.insurance_number = "1254";
                        //کدملی یا شناسه ملی مالک کالا:*
                        simple_good_issue.owner = "2050130351";
                        //نام کامل مالک کالا حقیقی یا حقوقی
                        simple_good_issue.owner_name = "سهیل رمضان زاده";
                        //وزن بار بهمراه ناوگان- وزن ناخالص به کیلوگرم*
                        simple_good_issue.weight_impure = 2500;
                        //وزن بار بدون ناوگان-وزن خالص به کیلوگرم*
                        simple_good_issue.weight_pure = 500;

                        //اقلام کالایی حواله بصورت زیر بدست می آید
                        AnbarOperation.GoodsIssueItem good_issue_item = new AnbarOperation.GoodsIssueItem();
                        //شناسه کالا:*
                        good_issue_item.good_id = "0002";
                        //واحد اندازگیری:
                        // 0002	کیلوگرم	برای
                        //0003	مثقال	برای مثقال
                        //0004	لیتر	برای
                        //0005	تن	برای
                        //0001	برای عدد
                        //؟
                        good_issue_item.measurement_unit = "0001";
                        //مقدار برحسب واحد اندازه گیری:*
                        good_issue_item.count = 10;
                        //نوع بسته بندی:
                        //002	کارتن	
                        //001	پاکت	
                        //003	پالت	
                        //004	قراصه	
                        //005	گونی	
                        //006	بدون بسته بندی	
                        //007	فلّه
                        good_issue_item.package_type = "002";
                        //تعداد بسته
                        good_issue_item.package_count = 2;

                        good_issue_item.location = "محل نگهداری";
                        //تاریخ تولید epoch:
                        good_issue_item.production_date = 1324499400;
                        //تاریخ انقضا epoch:*
                        //باید بعد از زمان جاری و بعد از تاریخ تولید باشد
                        good_issue_item.expire_date = 1684771400;
                        //وزن کل ردیف کالا به کیلوگرم
                        good_issue_item.total_weight = 5000;

                        //می تواند چندین ریف کالایی در یک سند افزود
                        simple_good_issue.goods_issue_items.Add(good_issue_item);


                        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                        client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api-imp/");
                        string input = JsonConvert.SerializeObject(simple_good_issue, Formatting.Indented);
                        return client.PostAsJsonAsync("2050130000/2050130000/goods_issue/simple", simple_good_issue).Result;
                    }

                    public static bool Havale(string api_key, string contractor_national_id, AnbarOperation.SimpleGoodIssue simple_havale, out string result)
                    {
                        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                        client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api-imp/");
                        string input = JsonConvert.SerializeObject(simple_havale, Formatting.Indented);
                        HttpResponseMessage response = client.PostAsJsonAsync(api_key + "/" + contractor_national_id + "/goods_issue/simple", simple_havale).Result;

                        result = response.Content.ReadAsStringAsync().Result;
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            return false;
                        }
                        else
                        {
                            //ثبت سند بصورت موقت انجام شد
                            Dictionary<string, object> output = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                            string id = output["id"].ToString();
                            int version = int.Parse(output["version"].ToString());
                            result = id;
                            return true;
                        }


                    
                }

                    public static bool FinalizeGoodIssue(string id, string api_key, string contractor_national_id, out string finalization)
                    {
                        //جهت نهایی سازی سند:
                        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                        client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api-imp/");
                        HttpResponseMessage response_final = client.PostAsync(api_key + "/" + contractor_national_id + "/goods_issue/" + id + "/finalize", null).Result;

                        finalization = response_final.Content.ReadAsStringAsync().Result;
                        if (response_final.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            return false;
                        }
                        else
                        {//در این صورت سند قطعی شده است
                            Dictionary<string, object> output_final = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(finalization);
                            string status = output_final["status"].ToString(); // status == "PERMANENT"
                            if (status == "PERMANENT") return true;
                            else return false;

                        }
                    }
                }

            }


            public class EstelamAccessFile
            {
                public class CoOrPersonClass
                {
                    public class CoOrPerson
                    {
                        public string code { get; set; }
                        public string name { get; set; }
                        public string family { get; set; }
                        public string error { get; set; }
                        public string status { get; set; }
                    }

                    public static void StartParallelCallCoOrPerson(List<CoOrPerson> list, BackgroundWorker worker, params object[] args)
                    {
                        string table_name = args[0].ToString();
                        string access_file_name = args[1].ToString();

                        foreach (var item in list)
                        {
                            HttpResponseMessage response = new HttpResponseMessage();
                            if (item.code.Length > 10)
                            {
                                var get = SRL.Projects.Nwms.GetCompanyByCoNationalId(item.code, out response);

                                if (get == null)
                                {
                                    string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "'  where code='" + item.code + "'";
                                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name, true);
                                }
                                else if (string.IsNullOrWhiteSpace(get.error_name))
                                {
                                    string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "' , name='" + get.name + "' , CoAddress='" + get.address + "' where code='" + item.code + "'";
                                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name, true);
                                }
                                else
                                {
                                    string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "' , error='" + get.error_name + "' where code='" + item.code + "'";
                                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name, true);
                                }
                            }
                            else
                            {
                                var get = SRL.Projects.Nwms.GetPersonByNationalId(item.code, out response);
                                if (string.IsNullOrWhiteSpace(get.ErrorDescription))
                                {
                                    string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "' , name='" + get.FirstName + "', family='" + get.LastName + "' where code='" + item.code + "'";
                                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name, true);
                                }
                                else
                                {
                                    string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "' , error='" + get.ErrorDescription + "' where code='" + item.code + "'";
                                    SRL.AccessManagement.ExecuteToAccess(query, access_file_name, true);
                                }
                            }
                            //}
                            //catch (Exception ex)
                            //{
                            //    MessageBox.Show(ex.Message);
                            //}
                        }
                    }

                    public static void CheckCoOrPersonIsCorrect(string access_file_name, string table_name, int paralel)
                    {
                        SRL.AccessManagement.AddColumnToAccess("name", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);
                        SRL.AccessManagement.AddColumnToAccess("family", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);
                        SRL.AccessManagement.AddColumnToAccess("error", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);
                        SRL.AccessManagement.AddColumnToAccess("status", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);
                        SRL.AccessManagement.AddColumnToAccess("CoAddress", table_name, SRL.AccessManagement.AccessDataType.nvarcharmax, access_file_name, true);

                        DataTable dt = SRL.AccessManagement.GetDataTableFromAccess(access_file_name, table_name);
                        var list_ = SRL.Convertor.ConvertDataTableToList<CoOrPerson>(dt);
                        var list = list_.Where(x => x.status == "" || x.status == null || x.status != "OK").ToList();

                        SRL.AccessManagement.ExecuteToAccess("update " + table_name + " set code=Trim(code)", access_file_name, true);

                        SRL.ActionManagement.MethodCall.ParallelMethodCaller.ParallelCall<CoOrPerson>(list, paralel.ToString(), StartParallelCallCoOrPerson, () => { MessageBox.Show("done"); }, null, table_name, access_file_name);
                    }
                }
                public class PostCodeEstelamResult
                {
                    public long ID { get; set; }
                    public string postal_code { get; set; }
                    public string status { get; set; }
                    public string exist_anbar { get; set; }
                    public string correct { get; set; }
                    public string province { get; set; }
                    public string township { get; set; }
                    public string city { get; set; }
                    public string address { get; set; }
                    public string error { get; set; }
                }

                public class GetAddressByPostServerResult
                {
                    public long ID { get; set; }
                    public string status { get; set; }
                    public string correct { get; set; }
                    public int ErrorCode { get; set; }
                    public string ErrorMessage { get; set; }
                    public string Location { get; set; }
                    public int LocationCode { get; set; }
                    public string LocationType { get; set; }
                    public string PostCode { get; set; }
                    public string State { get; set; }
                    public string TownShip { get; set; }
                    public string Village { get; set; }
                }
                public static void Estelam(string file_full_path, string table_name, string api_key)
                {
                    DataTable table = SRL.AccessManagement.GetDataTableFromAccess(file_full_path, table_name);

                    List<PostCodeEstelamResult> list = SRL.Convertor.ConvertDataTableToList<PostCodeEstelamResult>(table);

                    foreach (var item in list)
                    {
                        if (item.status == "OK") continue;

                        HttpResponseMessage response = new HttpResponseMessage();
                        string message = "";
                        SRL.Projects.Nwms.ComplexByPostCodeResult war =
                        SRL.Projects.Nwms.GetComplexByPostalCode(item.postal_code, api_key, out message);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            try
                            {
                                string query = "update " + table_name + " set status='OK' , exist_anbar='" + war.warehouse_server + "' , correct='" + war.postal_code_server + "' "
                                    + " , province='" + war.province + "'  , township='" + war.township + "'  , city='" + war.city + "'"
                                    + "  , address='" + war.full_address + "'"
                                    + " where postal_code='" + item.postal_code + "' ;";
                                SRL.AccessManagement.ExecuteToAccess(query, file_full_path, true);


                            }
                            catch (Exception ex)
                            {
                                string query = "update " + table_name + " set  error='" + ex.Message + "' where ID=" + item.ID + " ;";
                                SRL.AccessManagement.ExecuteToAccess(query, file_full_path, true);
                            }
                        }
                        else
                        {
                            string query = "update " + table_name + " set status='" + response.StatusCode.ToString() + "' , error='" + message + "' where ID=" + item.ID + " ;";
                            SRL.AccessManagement.ExecuteToAccess(query, file_full_path, true);
                        }

                    }
                }

                public static void ParallelAddressByPostServer(List<GetAddressByPostServerResult> list, BackgroundWorker bg, params object[] args)
                {
                    //args0: password, args1: client, args2: username, args3: file_full_path, args4: table_name
                    foreach (var item in list)
                    {
                        if (item.status == "OK") continue;
                        var time = new System.Diagnostics.Stopwatch();
                        time.Start();
                        var result = EstelamFromPostServer((PostCodeServiceReference.PostCodeClient)args[1], args[2].ToString(), item.PostCode, args[0].ToString());
                        time.Stop();
                        var sec = time.Elapsed.TotalSeconds;
                        time = new System.Diagnostics.Stopwatch();

                        try
                        {
                            string address = "";

                            if (result.ErrorCode == 0)

                            {
                                address += "استان " + result.State.Trim() + "- شهرستان " + result.TownShip.Trim() + "- بخش " + result.Zone.Trim() + "- " + result.LocationType.Trim() + " " + result.Location.Trim()
                                + "- " + result.Parish.Trim() + "- " + result.PreAvenue.Trim() + "- " + result.Avenue.Trim();
                                if (result.HouseNo != 0) address += "- پلاک " + result.HouseNo;
                                if (!string.IsNullOrWhiteSpace(result.BuildingName)) address += "- ساختمان " + result.BuildingName.Trim();
                                address += "- طبقه " + result.FloorNo.Trim();
                                if (!string.IsNullOrWhiteSpace(result.SideFloor)) address += "- واحد " + result.SideFloor.Trim();
                            }

                            string query = "update " + args[4] + " set status='OK', correct='" + (result.ErrorCode == 0 ?
                       "true" : "false") + "', ErrorCode=" + result.ErrorCode + " , ErrorMessage='" + result.ErrorMessage + "', "
                       + " Location='" + result.Location + "' , LocationCode=" + result.LocationCode + " "
                                + " , LocationType='" + result.LocationType + "'  , State='" + result.State + "'  , TownShip='" + result.TownShip + "'"
                                + "  , Village='" + result.Village + "' , Address='" + address + "'  where PostCode='" + item.PostCode + "' ";

                            var exce = SRL.AccessManagement.ExecuteToAccess(query, args[3].ToString(), false);


                        }
                        catch (Exception ex)
                        {
                            string query = "update " + args[4] + " set  status='" + ex.Message + "' where ID=" + item.ID + " ;";
                            SRL.AccessManagement.ExecuteToAccess(query, args[3].ToString(), true);
                        }
                    }
                }

                public static void ParallelAddressByPostESB(List<GetAddressByPostServerResult> list, BackgroundWorker bg, params object[] args)
                {
                    string service_password = args[0].ToString();
                    var client = (CIXGetAddressByPostcode.GetAddressByPostCodePortTypeClient)args[1];
                    string file_full_path = args[2].ToString();
                    string table_name = args[3].ToString();

                    foreach (var item in list)
                    {
                        if (item.status == "OK") continue;
                        var time = new System.Diagnostics.Stopwatch();
                        time.Start();
                        var result = EstelamFromPostESBServer(client, item.PostCode, service_password);
                        time.Stop();
                        var sec = time.Elapsed.TotalSeconds;
                        time = new System.Diagnostics.Stopwatch();
                        time.Start();
                        try
                        {
                            string address = "";

                            if (result.ErrorCode == 0)

                            {
                                address += "استان " + result.State.Trim() + "- شهرستان " + result.TownShip.Trim() + "- بخش " + result.Zone.Trim() + "- " + result.LocationType.Trim() + " " + result.Location.Trim()
                                + "- " + result.Parish.Trim() + "- " + result.PreAvenue.Trim() + "- " + result.Avenue.Trim();
                                if (result.HouseNo != 0) address += "- پلاک " + result.HouseNo;
                                if (!string.IsNullOrWhiteSpace(result.BuildingName)) address += "- ساختمان " + result.BuildingName.Trim();
                                address += "- طبقه " + result.FloorNo.Trim();
                                if (!string.IsNullOrWhiteSpace(result.SideFloor)) address += "- واحد " + result.SideFloor.Trim();

                                while (address.Contains("  "))
                                {
                                   address= address.Replace("  ", " ");
                                }

                            }

                            string query = "update " + table_name + " set status='OK', correct='" + (result.ErrorCode == 0 ?
                       "true" : "false") + "', ErrorCode=" + result.ErrorCode + " , ErrorMessage='" + result.ErrorMessage + "', "
                       + " Location='" + result.Location + "' , LocationCode=" + result.LocationCode + " "
                                + " , LocationType='" + result.LocationType + "'  , State='" + result.State + "'  , TownShip='" + result.TownShip + "'"
                                + "  , Village='" + result.Village + "' , Address='" + address + "'  where PostCode='" + item.PostCode + "' ";

                            var exce = SRL.AccessManagement.ExecuteToAccess(query, file_full_path, false);


                        }
                        catch (Exception ex)
                        {
                            string query = "update " + table_name + " set  status='" + ex.Message + "' where ID=" + item.ID + " ;";
                            SRL.AccessManagement.ExecuteToAccess(query, file_full_path, true);
                        }
                        time.Stop();
                        sec = time.Elapsed.TotalSeconds;
                        time = new System.Diagnostics.Stopwatch();
                    }
                }

                public static void EstelamFromPost(string file_full_path, string table_name, string api_key, string parallel, string password, string username)
                {
                    DataTable table = SRL.AccessManagement.GetDataTableFromAccess(file_full_path, table_name);
                    List<GetAddressByPostServerResult> list = SRL.Convertor.ConvertDataTableToList<GetAddressByPostServerResult>(table).Where(x => x.status != "OK" || x.status == null || x.status == "").ToList();
                    PostCodeServiceReference.PostCodeClient client = new PostCodeServiceReference.PostCodeClient();
                    SRL.ActionManagement.MethodCall.ParallelMethodCaller.ParallelCall<GetAddressByPostServerResult>(list, parallel, ParallelAddressByPostServer, null, null, password, client, username, file_full_path, table_name);
                }

                public static void EstelamFromPostESB(string file_full_path, string table_name, string api_key, string parallel, string service_password, string esb_username, string esb_pass, Action callback)
                {
                    SRL.AccessManagement.AddColumnToAccess("ErrorCode", "table1", SRL.AccessManagement.AccessDataType.integer, file_full_path, true);
                    SRL.AccessManagement.AddColumnToAccess("ErrorMessage", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path, true);
                    SRL.AccessManagement.AddColumnToAccess("Location", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path, true);
                    SRL.AccessManagement.AddColumnToAccess("LocationCode", "table1", SRL.AccessManagement.AccessDataType.integer, file_full_path, true);
                    SRL.AccessManagement.AddColumnToAccess("LocationType", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path, true);
                    SRL.AccessManagement.AddColumnToAccess("State", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path, true);
                    SRL.AccessManagement.AddColumnToAccess("TownShip", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path, true);
                    SRL.AccessManagement.AddColumnToAccess("Village", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path, true);
                    SRL.AccessManagement.AddColumnToAccess("Address", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path, true);
                    SRL.AccessManagement.AddColumnToAccess("status", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path, true);
                    SRL.AccessManagement.AddColumnToAccess("correct", "table1", SRL.AccessManagement.AccessDataType.nvarcharmax, file_full_path, true);

                    DataTable table = SRL.AccessManagement.GetDataTableFromAccess(file_full_path, table_name);
                    List<GetAddressByPostServerResult> list = SRL.Convertor.ConvertDataTableToList<GetAddressByPostServerResult>(table).Where(x => x.status != "OK" || x.status == null || x.status == "").ToList();
                    CIXGetAddressByPostcode.GetAddressByPostCodePortTypeClient client = new CIXGetAddressByPostcode.GetAddressByPostCodePortTypeClient();
                    client.ClientCredentials.UserName.UserName = esb_username;
                    client.ClientCredentials.UserName.Password = esb_pass;
                    SRL.ActionManagement.MethodCall.ParallelMethodCaller.ParallelCall<GetAddressByPostServerResult>(list, parallel, ParallelAddressByPostESB, callback, null, service_password, client, file_full_path, table_name);
                }
            }

            public class ShahkarInputClass
            {
                public string requestId { get; set; }
                public string serviceNumber { get; set; }
                public int serviceType { get; set; }
                public int identificationType { get; set; }
                public string identificationNo { get; set; }
            }

            public class ShahkarOutputClass
            {
                public int response { get; set; }
                public string requestId { get; set; }
                public string result { get; set; }
                public string comment { get; set; }
            }

            public static ShahkarOutputClass CallShahkar(string client_id, string national_id, string mobile, string basic_auth_value = "c2FuYXRfbWFkYW5fZ3NiOjcxc2RsdTU2")
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://sr-cix.ntsw.ir/");
                ShahkarInputClass input = new ShahkarInputClass();

                input.identificationNo = national_id;
                input.identificationType = 0;
                var date = DateTime.Now;

                string time = String.Format("{0:yyyy}{0:MM}{0:dd}{0:HH}{0:mm}{0:ss}{0:FFFFFF}", date);

                string id = client_id + time;

                input.requestId = id;
                input.serviceNumber = mobile;
                input.serviceType = 2;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basic_auth_value);

                HttpResponseMessage response = client.PostAsJsonAsync("Services/GetIDmatching-NWMS", input).Result;

                string result = response.Content.ReadAsStringAsync().Result;
                ShahkarOutputClass output = Newtonsoft.Json.JsonConvert.DeserializeObject<ShahkarOutputClass>(result);

                return output;
            }

            public static List<SearchResult.SearchWarehouseResult> SearchWarehouse(Dictionary<string, object> input_json, string api_key)
            {
                List<SearchResult.SearchWarehouseResult> warehouses = new List<SearchResult.SearchWarehouseResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/warehouse/_search/");
                List<SearchResult.SearchWarehouseResult> warehouse_list = new List<SearchResult.SearchWarehouseResult>();
                int from = 0;
                do
                {

                    HttpResponseMessage response = client.PostAsJsonAsync(from + "/10", input_json).Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return null;
                    }

                    string result = response.Content.ReadAsStringAsync().Result;
                    Dictionary<string, object> result_json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    string data = result_json["data"].ToString();
                    if (string.IsNullOrWhiteSpace(data)) break;
                    warehouse_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult.SearchWarehouseResult>>(data);

                    warehouses.AddRange(warehouse_list);
                    from += 10;
                } while (warehouse_list.Count == 10);

                return warehouses;
            }


            public static List<SearchResult.SearchComplexResult> SearchComplex(Dictionary<string, object> input_json, string api_key)
            {
                List<SearchResult.SearchComplexResult> complexes = new List<SearchResult.SearchComplexResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api/" + api_key + "/admin/complex/_search/");

                List<SearchResult.SearchComplexResult> data_list = new List<SearchResult.SearchComplexResult>();
                int from = 0;
                do
                {
                    HttpResponseMessage response = client.PostAsJsonAsync(from + "/10", input_json).Result;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return null;
                    }

                    string result = response.Content.ReadAsStringAsync().Result;
                    Dictionary<string, object> result_json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    string data = result_json["data"].ToString();
                    if (string.IsNullOrWhiteSpace(data)) break;
                    data_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult.SearchComplexResult>>(data);

                    complexes.AddRange(data_list);

                    from += 10;
                } while (data_list.Any());

                return complexes;
            }

            public static List<SearchResult.SearchComplexResult> GetAllWarComplexByNationalId(string api_key, string national_id)
            {

                List<SearchResult.SearchComplexResult> complexes = new List<SearchResult.SearchComplexResult>();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://app.nwms.ir");

                HttpResponseMessage response = client.GetAsync("/v2/b2b-api-imp/" + api_key + "/" + national_id + "/complex/_all").Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return null;
                }

                string result = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result_json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                string data = result_json["data"].ToString();
                complexes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult.SearchComplexResult>>(data);


                return complexes;
            }

            public class ComplexByPostCodeResult
            {

                public bool warehouse_server { get; set; }
                public bool postal_code_server { get; set; }
                public string province { get; set; }
                public string village { get; set; }
                public string township { get; set; }
                public string city { get; set; }
                public string full_address { get; set; }

                public string name { get; set; }
                public string id { get; set; }
                public string org_creator_national_id { get; set; }
                public string gov_wh_number { get; set; }

                public Agent agent { get; set; }
                public List<Owner> owners { get; set; }
                public List<Warehouse> warehouses { get; set; }

                public class Agent
                {
                    public string name { get; set; }
                    public string id { get; set; }
                    public string national_id { get; set; }
                    public string mobile { get; set; }
                }


                public class Warehouse
                {
                    public string name { get; set; }
                    public string id { get; set; }
                    public string org_creator_national_id { get; set; }
                    public string type { get; set; }
                    public List<string> activity_sector { get; set; }
                    public List<string> supervisor_org { get; set; }
                    public List<Contractor> contractors { get; set; }
                }

                public class Owner
                {

                    public string name { get; set; }
                    public string national_id { get; set; }
                    public string mobile { get; set; }
                }

                public class Contractor
                {

                    public string name { get; set; }
                    public string id { get; set; }
                    public string national_id { get; set; }
                    public string mobile { get; set; }
                }

            }
            public class PersonClass
            {
                public string national_id { get; set; }
                public string City { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string IdentitySerial { get; set; }
                public string IdentityNo { get; set; }
                public string BirthDate { get; set; }
                public string FatherName { get; set; }
                public string SupervisorNationalCode { get; set; }
                public string IdentitySeries { get; set; }
                public string last_sent { get; set; }
                public string Town { get; set; }
                public string Gender { get; set; }
                public string ErrorDescription { get; set; }

            }

            public class AnbarPersonClass
            {

                public int create_date { get; set; }
                public string national_id { get; set; }
                public string modifier_national_id { get; set; }
                public string phonenumber { get; set; }
                public string account_status { get; set; }
                public string alivestatus { get; set; }

                public string password { get; set; }
                public string creator_national_id { get; set; }
                public string lastname { get; set; }
                public object SSNN_serial { get; set; }
                public object email { get; set; }
                public object files { get; set; }
                public string firstname { get; set; }
                public string using_two_phase_password { get; set; }
                public string SSNN_no { get; set; }
                public object address { get; set; }
                public string postalcode { get; set; }
                public object org_creator_national_id { get; set; }
                public string birthepoch { get; set; }
                public object user_national_card_image { get; set; }
                public object user_profile_image { get; set; }
                public string mobile { get; set; }
                public string gender { get; set; }
                public string userid { get; set; }
                public string fathername { get; set; }


            }
            public class CompanyClass
            {
                public long ID { get; set; }
                public string co_national_id { get; set; }
                public string name { get; set; }
                public string error_name { get; set; }
                public string address { get; set; }
            }
            public class CompanyListClass
            {
                public object website { get; set; }
                public string register_code { get; set; }
                public string national_id { get; set; }
                public string commercial_code { get; set; }
                public string register_epoch { get; set; }
                public string account_status { get; set; }
                public string phonenumber { get; set; }
                public string modifier_national_id { get; set; }
                public object roozname_rasmi_url { get; set; }
                public string id { get; set; }
                public string en_name { get; set; }
                public object register_city { get; set; }
                public object email { get; set; }
                public object files { get; set; }
                public string fax { get; set; }
                public object ceo_expire_epoch { get; set; }
                public string ceo_national_id { get; set; }
                public object address { get; set; }
                public object org_creator_national_id { get; set; }
                public string name { get; set; }
                public string creator_national_id { get; set; }
                public string postal_code { get; set; }
                public Ceo ceo { get; set; }

                public class Ceo
                {
                    public object org_creator_national_id { get; set; }
                    public string name { get; set; }
                    public string title { get; set; }
                    public string national_id { get; set; }
                    public object creator_national_id { get; set; }
                    public int start_epoch { get; set; }
                    public string common_name { get; set; }
                    public string account_status { get; set; }
                    public object modifier_national_id { get; set; }
                    public int expire_epoch { get; set; }
                    public string id { get; set; }
                    public string ceo_mobile { get; set; }
                }


            }

            public class PostalCodeFromPostClass
            {
                public int ErrorCode { get; set; }
                public object ErrorMessage { get; set; }
                public string BuildingName { get; set; }
                public string Zone { get; set; }
                public string LocationType { get; set; }
                public int Error { get; set; }
                public bool raw_result { get; set; }
                public string Description { get; set; }
                public string FloorNo { get; set; }
                public string State { get; set; }
                public object SideFloor { get; set; }
                public string PostCode { get; set; }
                public double HouseNo { get; set; }
                public object Village { get; set; }
                public string PreAvenue { get; set; }
                public string Avenue { get; set; }
                public string TownShip { get; set; }
                public string Parish { get; set; }
                public string LocationCode { get; set; }
                public string Location { get; set; }
            }


            public static PersonClass GetPersonByNationalId(string national_id, out HttpResponseMessage response)
            {
                PersonClass person = new PersonClass();
                person = null;
                response = null;
                HttpClient client_ = new HttpClient();
                client_.BaseAddress = new Uri("https://admin-app.nwms.ir/v2/b2b-api/2050130318/admin/ext-service/");
                person = new PersonClass();
                
                national_id = SRL.Convertor.NationalId(national_id);
                Dictionary<string, object> input = new Dictionary<string, object>();
                input["national_id"] = national_id;
                response = client_.PostAsJsonAsync("person_by_national_id", input).Result;
                if (string.IsNullOrWhiteSpace(national_id))
                {
                    person.ErrorDescription = "کدملی خالی است";
                    person.national_id = national_id;
                    return person;
                }
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    if (data["FirstName"] != null)
                    {
                        person.FirstName = data["FirstName"].ToString();
                        person.LastName = data["LastName"].ToString();
                        person.last_sent = DateTime.Now.ToString();
                        person.national_id = national_id;
                    }
                    else
                    {
                        person.ErrorDescription = data["ErrorDescription"].ToString();
                        person.national_id = national_id;
                    }

                }

                return person;
            }

            public static AnbarPersonClass GetAnbarPersonByNationalId(string national_id, string api_key)
            {

                HttpClient client1 = new HttpClient();
                client1.BaseAddress = new Uri("https://app.nwms.ir");
                // Dictionary<string, object> input = new Dictionary<string, object>();
                // input.Add("account_status", "1");
                // input.Add("national_id", national_id);
                //result = client1.PostAsJsonAsync("/v2/b2b-api/2050130318/admin/users/_search/0/1", input).Result;
                HttpResponseMessage response = client1.GetAsync("/v2/b2b-api-imp/" + api_key + "/" + national_id + "/user").Result;
                string result = response.Content.ReadAsStringAsync().Result;
                AnbarPersonClass person = Newtonsoft.Json.JsonConvert.DeserializeObject<AnbarPersonClass>(result);
                return person;
            }
            public static string ComputePostCodeHash(string password, string param1 = null, string param2 = null, string param3 = null)
            {
                StringBuilder sb = new StringBuilder(password + "#");
                if (!string.IsNullOrEmpty(param1))
                    sb.Append(param1 + "#");
                if (!string.IsNullOrEmpty(param2))
                    sb.Append(param2 + "#");
                if (!string.IsNullOrEmpty(param3))
                    sb.Append(param3 + "#");
                sb.Append(DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));

                return SRL.Security.GetHashString(sb.ToString(), SRL.Security.HashAlgoritmType.Sha1);
            }


            public static PostCodeServiceReference.AddressResult EstelamFromPostServer
                   (PostCodeServiceReference.PostCodeClient client, string username, string post_code, string password)
            {
                string hash = ComputePostCodeHash(password, post_code);
                var res = client.GetAddressByPostcode(username, hash, post_code, "", "", "");
                return res;
            }

            public static CIXGetAddressByPostcode.AddressResult EstelamFromPostESBServer
                  (CIXGetAddressByPostcode.GetAddressByPostCodePortTypeClient client, string post_code, string service_password)
            {
                string hash = ComputePostCodeHash(service_password, post_code);
                var res = client.GetAddressByPostcode(hash, post_code, "?", "?", "?");
                return res;
            }
            public static PostalCodeFromPostClass EstelamPostalCodeFromPost(string postal_code, out HttpResponseMessage response)
            {
                PostalCodeFromPostClass post = new PostalCodeFromPostClass();
                post = null;
                response = null;
                HttpClient client_ = new HttpClient();
                client_.BaseAddress = new Uri("https://admin-app.nwms.ir/v2/b2b-api/2050130318/admin/ext-service/");
                if (string.IsNullOrWhiteSpace(postal_code)) return null;
                Dictionary<string, object> input = new Dictionary<string, object>();
                input["postal_code"] = postal_code;
                response = client_.PostAsJsonAsync("postal_code", input).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    post = Newtonsoft.Json.JsonConvert.DeserializeObject<PostalCodeFromPostClass>(result);

                }
                return post;
            }
            public static CompanyClass GetCompanyByCoNationalId(string co_national_id, out HttpResponseMessage response)
            {
                CompanyClass company = new CompanyClass();
                company = null;
                response = null;

                HttpClient client_ = new HttpClient();
                client_.BaseAddress = new Uri("https://admin-app.nwms.ir/v2/b2b-api/2050130318/admin/ext-service/");

                company = new CompanyClass();

                

                Dictionary<string, object> input = new Dictionary<string, object>();
                input["cmp_national_code"] = co_national_id;
                response = client_.PostAsJsonAsync("co_inq", input).Result;
                if (string.IsNullOrWhiteSpace(co_national_id))
                {
                    company.error_name = "شناسه ملی خالی است";
                    company.co_national_id = co_national_id;
                    return company;
                }
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    
                    string result = response.Content.ReadAsStringAsync().Result;
                    Dictionary<string, object> data1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                    if (data1["Successful"].ToString() == "true" || data1["Successful"].ToString() == "True")
                    {

                        company.name = data1["Name"].ToString();
                        company.co_national_id = co_national_id;
                        company.address = data1["Address"].ToString();
                    }
                    else
                    {
                        company.error_name = data1["Message"].ToString();
                        company.co_national_id = co_national_id;
                    }
                }
                return company;


            }

            public static List<CompanyListClass> GetCompanySeoByCoNationalId(string co_national_id, string api_key, out HttpResponseMessage response)
            {
                List<CompanyListClass> result_list = new List<CompanyListClass>();

                response = null;

                HttpClient client_list = new HttpClient();
                client_list.BaseAddress = new Uri("https://admin-app.nwms.ir/v2/b2b-api/" + api_key + "/admin/company/_search/");


                if (string.IsNullOrWhiteSpace(co_national_id)) return null;

                Dictionary<string, object> input = new Dictionary<string, object>();
                input["national_id"] = co_national_id;
                response = client_list.PostAsJsonAsync("0/99", input).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {


                    string result = response.Content.ReadAsStringAsync().Result;

                    string data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(result)["data"].ToString();

                    List<CompanyListClass> co_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CompanyListClass>>(data);

                    foreach (var item in co_list)
                    {
                        if (item.account_status == "3") continue;

                        item.ceo.ceo_mobile = Nwms.GetAnbarPersonByNationalId(item.ceo.national_id, api_key)?.mobile;
                        result_list.Add(item);

                    }
                }
                else
                {
                    result_list = null;
                }

                return result_list;


            }

            public static ComplexByPostCodeResult GetComplexByPostalCode(string postal_code, string api_key, out string result)
            {
                ComplexByPostCodeResult estelam_result = new ComplexByPostCodeResult();
                estelam_result = null;

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri("https://app.nwms.ir/v2/b2b-api/");

                var call = client.GetAsync(api_key + "/complex_by_post_code/" + postal_code);
                HttpResponseMessage response = call.Result;
                string result_ = response.Content.ReadAsStringAsync().Result;
                result = System.Text.RegularExpressions.Regex.Unescape(result_);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    estelam_result = Newtonsoft.Json.JsonConvert.DeserializeObject<ComplexByPostCodeResult>(result);

                }
                return estelam_result;

            }

            public class SearchResult
            {
                public class Polygon
                {
                    public double lat { get; set; }
                    public double lng { get; set; }
                }

                public class Person
                {
                    public string national_id { get; set; }
                    public string name { get; set; }
                    public string id { get; set; }
                }
                public class SearchComplexResult
                {
                    public int? create_date { get; set; }
                    public string telephone_number { get; set; }
                    public string account_status { get; set; }
                    public string postal_code { get; set; }
                    public string village { get; set; }
                    public string id { get; set; }
                    public string city { get; set; }
                    public IList<Polygon> polygon { get; set; }
                    public string zone { get; set; }
                    public object area { get; set; }
                    public string warehouse_usage_type { get; set; }
                    public string province { get; set; }
                    public object gov_wh_number { get; set; }
                    public string complex_set_type { get; set; }
                    public string full_address { get; set; }
                    public object address { get; set; }
                    public string township { get; set; }
                    public string org_creator_national_id { get; set; }
                    public string warehouse_ownership_type { get; set; }
                    public IList<Person> owners { get; set; }
                    public Person agent { get; set; }
                    public string name { get; set; }
                    public string country { get; set; }
                    public string rural { get; set; }
                }

                public class SearchWarehouseResult
                {
                    public string postal_code { get; set; }
                    public List<Person> contractors { get; set; }
                    public string org_creator_national_id { get; set; }
                    public string name { get; set; }
                    public string id { get; set; }

                }


            }


        }

        public class MeliSms
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string FromNumber { get; set; }
            public ServiceReferenceSendSms.SendSoapClient send_client;

            public MeliSms(string username, string password, string from_number)
            {
                //"09114452764", "2275",from_number = "500010604260"
                Username = username;
                Password = password;
                FromNumber = from_number;
                send_client = new ServiceReferenceSendSms.SendSoapClient("SendSoap12");
            }

            public Dictionary<string, string> SendSms(string text, string[] mobiles_to)
            {
                string[] result = send_client.SendSimpleSMS(Username, Password, mobiles_to, FromNumber, text, false);
                Dictionary<string, string> mobile_res = new Dictionary<string, string>();
                for (int i = 0; i < mobiles_to.Count(); i++)
                {
                    mobile_res[mobiles_to[i]] = GetErrorName(result[i]);
                }

                return mobile_res;
            }

            public double GetCredit()
            {
                double credit = send_client.GetCredit(Username, Password);
                return Math.Ceiling(credit);
            }

            private string GetErrorName(string code)
            {
                Dictionary<string, string> error_name = new Dictionary<string, string>();
                error_name["0"] = "نام کاربری یا رمز عبور اشتباه است";
                error_name["1"] = "درخواست با موفقیت انجام شد";
                error_name["2"] = "اعتبار کافی نمی باشد";
                error_name["3"] = "محدودیت در ارسال روزانه";
                error_name["4"] = "محدودیت در حجم ارسال";
                error_name["5"] = "شماره فرستنده معتبر نمی باشد";
                error_name["6"] = "سامانه در حال بروزرسانی می باشد";
                error_name["7"] = "متن حاوی کلمه فیلتر شده می باشد";
                error_name["9"] = "ارسال از خطوط عمومی از طریق وب سرویس مکانپذیر نمی باشد";
                error_name["10"] = "کاربر مورد نظر فعال نمی باشد";
                error_name["11"] = "ارسال نشده";
                error_name["0"] = "مدارک کابر کامل نمی باشد";
                if (error_name.ContainsKey(code))
                    return error_name[code];
                else return code;
            }

            public void ShowCreditInControl(Control control_to_show)
            {
                control_to_show.Text = "در حال بروزرسانی...";
                double credit = 0;
                SRL.ActionManagement.MethodCall.RunMethodInBack.Run(() =>
                {
                    credit = GetCredit();
                }, () =>
                {
                    control_to_show.Text = credit.ToString();
                }, null, ProgressBarStyle.Blocks);

            }
        }
    }
}
