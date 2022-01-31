using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public int UpperId { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Parameter { get; set; }
        public string Icon { get; set; }
        public string Search { get; set; }
        public bool HasChild { get; set; }
        public bool IsReport { get; set; }
        public bool IsNewTab { get; set; }
    }

    public class UserMenu
    {
        public UserMenu()
        {
            HtmlMenu = new HtmlString("");
        }
        public IHtmlString HtmlMenu { get; set; }
        public int UserId { get; set; }
    }
    public static class Menus
    {
        public static List<UserMenu> menuList = new List<UserMenu>();
    }
    public class MenuProfileGroupModel
    {
        public List<SavedMenu> UserMenu { get; set; }
        public List<AllMenu> AllMenu { get; set; }
    }
    public class SavedMenu
    {
        public long SmpID { get; set; }
        public long MnuID { get; set; }
        public string Title { get; set; }
    }
    public class AllMenu
    {
        public long MenuId { get; set; }
        public string Title { get; set; }
        public long MenuUpperId { get; set; }
        public bool HasChild { get; set; }
        public bool IsReport { get; set; }
        public bool IsCreate { get; set; }
        public bool IsRead { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public string Path { get; set; }

    }
    public class ChildMenu
    {
        public long ParentId { get; set; }
        public List<AllMenu> Menu { get; set; }
    }
    public class SysMenu
    {

        public bool IsRead { get; set; }
        public bool IsCreate { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public int MenuId { get; set; }
        public int UserGroupId { get; set; }
        public bool IsReport { get; set; }


    }
}