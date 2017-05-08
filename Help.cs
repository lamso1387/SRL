using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRL
{
    public static class Help
    {
        public static class ChildParent
        {
            public static void AddCategory(string categoryName)
            {
                // databaseEntity db = new databaseEntity();
                //tableEntity newCategory = new tableEntity();
                //newCategory.categoryName = categoryName;
                //db.Category.Add(newCategory);
                //db.SaveChanges();
                //return newCategory.ID;
            }
            public static void AddChildParent(long childId, long parentId)
            {
                //GD2Entities db = new GD2Entities();
                //CategoryClass categoryClass = new CategoryClass();
                //categoryClass.parentID = parentId;
                //categoryClass.childID = childId;
                //db.CategoryClass.Add(categoryClass);
                // db.SaveChanges();
            }
            public static void DeleteNodeChilds(long parentId)
            {
                // GD2Entities db = new GD2Entities();
                //var childsId = db.CategoryClass.Where(x => x.parentID == parentId).Select(x => x.childID).ToArray();
                //if (childsId.Length > 0)
                //{
                //    foreach (var childId in childsId)
                //    {
                //        CategoryClass categoryClass = db.CategoryClass.First(x => x.childID == childId);
                //        db.CategoryClass.Remove(categoryClass);
                //        db.SaveChanges();
                //        DeleteNodeChilds(long.Parse(childId.ToString()));
                //        Category category = db.Category.First(x => x.ID == childId);
                //        db.Category.Remove(category);
                //        db.SaveChanges();
                //    }
                //}
            }
        }
        public static class Parallel
        {
            public static void ParallelSend(System.Data.Entity.DbContext db, List<System.Data.Entity.DbSet> DBitems, string from, string parallel)
            {
                int all = DBitems.Count;
                List<Task> task_list = new List<Task>();
                int per_count = int.Parse(parallel);
                int take = all / per_count;
                int skip = 0;
                var DBquery = DBitems.AsQueryable();
                for (int j = 0; j < per_count; j++)
                {
                    System.Windows.Forms.Application.DoEvents();
                    var query = DBquery.Skip(skip).Take(take);
                    skip += take;
                    Task task = new Task(()=> new Convertor()); //new Task(() => StartSending(db, query.ToList()));
                    task_list.Add(task);
                    task.Start();

                }
                Task.WaitAll(task_list.ToArray());
            }

        }
        public static class UI
        {
            public static void ButtonLoading(Action<object, EventArgs> method)
            {
                //Loading loading = new Loading();
                //loading.Show();
                //loading.lblTime.Text = "";
                //loading.lblLoading.Text = "در حال اعمال اطلاعات...";
                //method(null, null);
                //loading.Close();
            }
        }
    }
}
