using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigiKala.DataAccessLayer.Entities
{
    public class FieldCategory
    {
        [Key]
        public int Id { get; set; }

        public int FieldId { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("FieldId")]
        public virtual Field Field { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
