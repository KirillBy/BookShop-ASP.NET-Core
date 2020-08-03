using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.Models
{
    public class AuthorizationCounter
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Number of authorization")]
        public int AuthNumber { get; set; }
    }
}
