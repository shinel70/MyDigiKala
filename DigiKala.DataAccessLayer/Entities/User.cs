﻿using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigiKala.DataAccessLayer.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public int RoleId { get; set; }

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(11, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Mobile { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Password { get; set; }

        [Display(Name = "کد ملی")]
        [MaxLength(10, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string Code { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [MaxLength(100, ErrorMessage = "مقدار {0} نباید بیش تر از {1} کاراکتر باشد")]
        public string FullName { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        [Display(Name = "تاریخ و زمان عضویت")]
        [Required(ErrorMessage = "نباید بدون مقدار باشد")]
        public DateTime DateTime { get; set; }

        public virtual Role Role { get; set; }
		public virtual Store Store { get; set; }
		public virtual IList<UserAddress> Addresses { get; set; }
		public virtual IList<Comment> Comments { get; set; }
		public virtual IList<CommentLike> CommentLikes { get; set; }
		public virtual IList<Order> Orders { get; set; }
	}
}
