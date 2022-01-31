using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class Enums
    {
    }
    public enum MenuPermission
    {
        [Display(Name = "Görüntüleme")]
        Read,
        [Display(Name = "Yeni Oluşturma")]
        Create,
        [Display(Name = "Güncelleme")]
        Update,
        [Display(Name = "Silme")]
        Delete


    }
}