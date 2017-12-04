using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BillPay.Models
{
    public class Bill
    {
        [Key]
        public int BillID { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public decimal Cost { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DueDate { get; set; }
        public string Color { get; set; }

        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }
    }
}