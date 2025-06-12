using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerApp.Models
{
    /// <summary>
    /// 任务分类实体
    /// </summary>
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}

// 修改1
//namespace TaskManagerApp.Models
//{
//    /// <summary>
//    /// 任务分类实体，继承 ObservableObject 以支持属性更改通知
//    /// </summary>
//    public class Category : ObservableObject
//    {
//        private int _id;
//        public int Id
//        {
//            get => _id;
//            set => SetProperty(ref _id, value);
//        }

//        private string _name;
//        public string Name
//        {
//            get => _name;
//            set => SetProperty(ref _name, value);
//        }
//    }
//}