using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.UI;
//using System.Data.Entity;
//using System.Web.UI.WebControls;
//using System.Web.Services;
//using System.Web.Script.Services;
using System.Threading;
 //using System.Web.Http;
using System.Threading.Tasks;
using System.Net;
//using System.Net.Http;
using System.IO;
 //using System.Web.Mvc;


namespace GD2
{
    public partial class PublicForm // : System.Web.UI.Page
    {
   //     static GD2Entities db;
     //   static Page page = new Page();
        protected void Page_Load(object sender, EventArgs e)
        {
        }
       //  [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //[WebMethod]
        //public static Dictionary<string, object> TryLogin(string usernameLogin, string passwordLogin)
        //{
        //  //  Publics.Response response = new Publics.Response();
        //    int loginState;
        //    //db = new GD2Entities();
        //  //   var mainPassword = db.UserMembership.Where(w => w.User.username == usernameLogin).Select(x => x.password).DefaultIfEmpty(string.Empty).FirstOrDefault();
        //     if (mainPassword == string.Empty)
        //         loginState = -1;
        //     else if (mainPassword == passwordLogin)
        //     {
        //    //      var role = db.Roles.Where(x => x.User.Any(y => y.username == usernameLogin)).Select(z => z.roleName).First();
        //     //    response.AddItem("role", role);
        //       //  Publics.CreateSession("username", usernameLogin, page);
        //         //Publics.CreateSession("role", role, page);
        //         loginState = 1;
        //     }
        //     else loginState = 0;
        //     response.AddItem("loginState", loginState);
        //    return    response.result;
        //}
        //[WebMethod]
        //public static Dictionary<string, object> CheckLogin()
        //{
        //    var usernameSession = page.Session["username"];
        //    Publics.Response response = new Publics.Response();
        //    if (usernameSession == null)
        //        response.AddItem("isLogin", false);
        //    else
        //    {
        //        var roleSession = page.Session["role"];
        //        response.AddItem("isLogin", true);
        //        response.AddItem("username", usernameSession);
        //        response.AddItem("role", roleSession);
        //    }
        //    return response.result;
        //}
        //[WebMethod]
        //public static bool CheckUsernameUnique(string username)
        //{
        //    db = new GD2Entities();
        //    if (db.User.Any(x => x.username == username))
        //    {
        //        return false;
        //    }
        //    else return true;
        //}
        //[WebMethod]
        //public static void TryLogout()
        //{
        //    page.Session.Remove("username");
        //}
        //[WebMethod]
        //public static Array GetCategoryParents()
        //{
        //    db = new GD2Entities();
        //     var childsId = db.CategoryClass.Select(x => x.childID).ToArray();
        //     var parents = db.Category.Where(x => !childsId.Contains(x.ID)).Select(x => new { parentId = x.ID, parentName = x.categoryName }).ToArray();
        //    return    parents;
        //}
        //[WebMethod]
        //public static Array GetParentChilds(long parentId)
        //{
        //    db = new GD2Entities();
        //     var childsId = db.CategoryClass.Where(x => x.parentID == parentId).Select(x => x.childID).ToArray();
        //     var childs = db.Category.Where(x => childsId.Contains(x.ID)).Select(x => new { childId = x.ID, childName = x.categoryName }).Distinct().ToArray();
        //    return    childs;
        //}
        //[WebMethod]
        //public static string UserRegisterActivation(string username, string activationKey)
        //{
        //    string response = "notSet-UserRegisterActivation";
        //    if (Publics.MakeHashValue(username) == activationKey)
        //    {
        //        int activateStatus = Publics.UserActiveStatus(username);
        //        if (activateStatus == 0)
        //        {
        //             Publics.ActivateUser(username);
        //            response = "activated";
        //            Publics.CreateSession("username", username, page);
        //        }
        //        else if (activateStatus == 1)
        //        {
        //            response = "activatedBefore";
        //        }
        //        else
        //        {
        //            response = "noUser";
        //        }
        //    }
        //    else
        //    {
        //        response = "wrongActivationKey";
        //    }
        //    return response;
        //}
        //[WebMethod]
        //public static Dictionary<string, object> UserRegisteration(string fName, string lName, string username, string email, string sex, string nationalCode, string mobile, string phone, string state, string bigCity, string addressContinue, string password)
        //{
        //    Publics.Response response = new Publics.Response();
        //    if (CheckUsernameUnique(username))
        //    {
        //        response.AddItem("usernameDuplicate", false);
        //        db = new GD2Entities();
        //        User user = new User();
        //        user.name = fName;
        //        user.family = lName;
        //        user.username = username;
        //        user.sex = sex;
        //        user.nationalCode = nationalCode;
        //        user.email = email;
        //        user.mobile = mobile;
        //        user.phone = phone;
        //        user.ostanID = 2;
        //        user.bigCityID = 1;
        //        user.addressContinue = addressContinue;
        //         var role = db.Roles.Where(x => x.ID == 1).First();
        //         user.Roles.Add(role);
        //        db.User.Add(user);
        //        int userAdded = db.SaveChanges();
        //        if (userAdded > 0)
        //        {
        //            if (Publics.AddMembership(db, username, user.ID, password) > 0)
        //            {
        //                Publics.SendActivationEmail(user, response);
        //                response.AddItem("insertResult", true);
        //            }
        //            else
        //            {
        //                Publics.DeleteUser(user.ID);
        //            }
        //        }
        //        else response.AddItem("insertResult", false);
        //    }
        //    else
        //    {
        //        response.AddItem("usernameDuplicate", true);
        //    }
        //    return response.result;
        //}
        //[WebMethod]
        //public static Array GetStates()
        //{
        //    db = new GD2Entities();
        //     var states = db.Ostan.Select(x => new { name = x.ostan, id = x.ID }).Distinct().ToArray();
        //    return    states;

        //}
        //[WebMethod]
        //public static Array GetBigCities(long ostanId)
        //{
        //    db = new GD2Entities();
        //     var ostanName = db.Ostan.Where(x => x.ID == ostanId).Select(y => y.ostan).First();
        //     var bigCities = db.BigCity.Where(x => x.ostan == ostanName).Select(z => new { name = z.shahrestan, id = z.ID }).Distinct().ToArray();
        //     return;

        //}
        //[WebMethod]
        //public static Array GetAdData(string username)
        //{
        //    db = new GD2Entities();
        //     var states = db.BigCity.Select(x => new { name = x.shahrestan }).Distinct().ToArray();
        //    return    states;

        //}
        //[WebMethod]
        //public static void AddCategoryNode(string childName, long parentId)
        //{
        //    long newCategoryID = Publics.AddCategory(childName);
        //    if (parentId > 0)
        //    {
        //        Publics.AddChildParent(newCategoryID, parentId);
        //    }
        //}
        //[WebMethod]
        //public static void EditCatName(long nodeId, string nodeName)
        //{
        //    db = new GD2Entities();
        //     var category = db.Category.Where(x => x.ID == nodeId).Select(x => x).First();
        //     category.categoryName = nodeName;
        //    db.SaveChanges();
        //}
        //[WebMethod]
        //public static void DeleteCategory(long nodeId)
        //{
        //    GD2Entities database = new GD2Entities();
        //    Publics.DeleteNodeChilds(nodeId);
        //     var parentCategory = database.CategoryClass.Where(x => x.childID == nodeId).ToList();
        //     if (parentCategory.Count > 0)
        //     {
        //          database.CategoryClass.Remove(database.CategoryClass.First(x => x.childID == nodeId));
        //         database.SaveChanges();
        //     }
        //     Category category = database.Category.First(x => x.ID == nodeId);
        //     database.Category.Remove(category);
        //    database.SaveChanges();
        //}
        //[WebMethod]
        //public static Dictionary<string, object> GetUserAddressData()
        //{
        //    Publics.Response response = new Publics.Response();
        //    if (Publics.RedirectIfNotLogin(page, response, "/Default.aspx"))
        //    {
        //        string username = response.result["username"].ToString();
        //        db = new GD2Entities();
        //         var address = db.User.Where(x => x.username == username).Select(z => new { addressContinue = z.addressContinue, bigCityId = z.bigCityID, ostanId = z.ostanID, bigCityName = z.BigCity.shahrestan }).First();
        //         response.AddItem("ostanId", address.ostanId);
        //         response.AddItem("addressContinue", address.addressContinue);
        //         response.AddItem("bigCityId", address.bigCityId);
        //         response.AddItem("bigCityName", address.bigCityName);
        //    }
        //    return response.result;
        //}
        //[WebMethod]
        //public static Array GetUserAds()
        //{
        //    Publics.Response response = new Publics.Response();
        //    if (Publics.RedirectIfNotLogin(page, response, "/Default.aspx"))
        //    {
        //        db = new GD2Entities();
        //        var userId = Publics.GetUserId(page);
        //        var ads = db.User.Where(x => x.AdCreation.Any(y => y.AdCreator.userID == userId)).Select(
        //            z => new
        //            {
        //                title = z.title,
        //                price = z.price,
        //                explain = z.explain,
        //                emergent = z.emergent,
        //                addressContinue = z.addressContinue,
        //                category = z.Category.categoryName,
        //                priceType = z.PriceType.priceTypeName,
        //                state = z.Ostan.ostan,
        //                bigCity = z.BigCity.shahrestan
        //            }).ToArray();
        //        var ads = db.Ad.Select(z => new { title = z.title, id = z.ID }).ToArray();
        //        return null;
        //    }
        //    else return;
        //}
    }
}
