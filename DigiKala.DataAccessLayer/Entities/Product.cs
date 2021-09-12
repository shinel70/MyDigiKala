using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigiKala.DataAccessLayer.Entities
{
	public class Product
	{
		[Key]
		public int Id { get; set; }
		public int BrandId { get; set; }
		public int CategoryId { get; set; }
		public int StoreId { get; set; }
		[Display(Name = "تاریخ و زمان عضویت")]
		[Required(ErrorMessage = "نباید بدون مقدار باشد")]
		public DateTime DateTime { get; set; }
		[Display(Name = "نام محصول")]
		[MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
		public string Name { get; set; }
		[Display(Name = "تصویر شاخص")]
		[MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
		public string Img { get; set; }
		[Display(Name = "قیمت")]
		public ulong Price { get; set; }
		[Display(Name = "قیمت قبل")]
		public ulong DeletePrice { get; set; }
		[Display(Name = "موجودی")]
		public int Exist { get; set; }
		[Display(Name = "عدم نمایش")]
		public bool NotShow { get; set; }
		[Display(Name = "سایر توضیحات")]
		[DataType(DataType.MultilineText)]
		public string Desc { get; set; }
		[Range(0, 5)]
		public float Rating { get; set; } = 5;
		[ForeignKey("BrandId")]
		public virtual Brand Brand { get; set; }
		[ForeignKey("CategoryId")]
		public virtual Category Category { get; set; }
		[ForeignKey("StoreId")]
		public virtual Store Store { get; set; }
		public virtual ICollection<ProductGallery> ProductGalleries { get; set; }
		public virtual ICollection<ProductField> ProductFields { get; set; }
		public virtual ICollection<ProductSeen> ProductSeens { get; set; }
		public virtual IList<Comment> Comments { get; set; }
		public virtual IList<OrderProduct> OrderProducts { get; set; }
	}
}
