using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Porduct_Management.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? Price { get; set; }

    }
}
