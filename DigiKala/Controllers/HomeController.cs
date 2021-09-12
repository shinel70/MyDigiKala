using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Globalization;

using DigiKala.Core.ViewModels;
using DigiKala.Core.Interfaces;
using DigiKala.Core.Services;

using DigiKala.DataAccessLayer.Entities;

using DigiKala.Core.Classes;
using DigiKala.DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DigiKala.Core.Dtoes;
using Z.EntityFramework.Plus;

namespace DigiKala.Controllers
{
	public class HomeController : Controller
	{
		private readonly ITemp _temp;
		private readonly DatabaseContext _context;
		private readonly IMapper _mapper;

		public HomeController(ITemp temp, DatabaseContext context, IMapper mapper)
		{
			_temp = temp;
			_context = context;
			_mapper = mapper;
		}

		[Route("Home/SearchByCategory/{CategoryName}")]
		public IActionResult SearchByCategory(string categoryName)
		{
			Category selectCategory = _context.Categories.Include(c => c.Parent).ThenInclude(parent => parent.SubCategories).FirstOrDefault(c => c.Name == categoryName);
			List<Category> categories = _context.Categories.ToList();
			List<int> knownCategoryIds = new List<int>() { selectCategory.Id };
			List<Category> knownCategories = new List<Category>();
			if (selectCategory.Parent != null)
				knownCategories.AddRange(selectCategory.Parent.SubCategories);
			else
				knownCategories.AddRange(categories.Where(c => c.ParentId == null).ToList());
			foreach (var category in categories)
			{
				if (knownCategoryIds.Contains(category.ParentId ?? 0))
				{
					knownCategoryIds.Add(category.Id);
					knownCategories.Add(category);
				}
			}

			List<Product> products = _context.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Store)
				.Where(p => knownCategoryIds.Contains(p.CategoryId)).ToList();


			List<Brand> brands = products.Select(p => p.Brand).Distinct().ToList();
			List<Store> stores = products.Select(p => p.Store).Distinct().ToList();

			var viewmodel = new SearchCategoryViewModel();

			viewmodel.FillBrands = brands;
			viewmodel.FillCategories = knownCategories;
			viewmodel.FillProducts = products;
			viewmodel.FillParentCategory = selectCategory.Parent;
			viewmodel.FillSelectCategory = selectCategory;
			viewmodel.FillStores = stores;

			return View(viewmodel);
		}
		[Role]
		public IActionResult Dashboard()
		{
			return View();
		}

		public IActionResult Index()
		{
			List<BannerDetails> bannerDetails = _temp.GetBannerDetailsNoExpire();

			if (bannerDetails.Count() > 0)
			{
				foreach (var item in bannerDetails)
				{
					if (item.ExpireDateTime.CompareTo(DateTime.Now) < 0)
					{
						_temp.UpdateBannerExpire(item.Id);
					}
				}
			}

			return View();
		}

		public IActionResult Banners()
		{
			List<Banner> banners = _temp.GetBanners();

			return View(banners);
		}

		[Route("/Product/{id}/{title}")]
		public IActionResult Product(int id, string title)
		{
			var viewModel = new ProductDetailsViewModel();

			Product product = _temp.GetProductDetail(id);

			viewModel.FillProduct = product;

			return View(viewModel);
		}


		#region api
		public IActionResult GetComments(int productId)
		{
			int userId = 0;
			if (User.Identity.IsAuthenticated)
			{
				var mobile = User.Identity.Name;
				userId = _context.Users.FirstOrDefault(u => u.Mobile == mobile).Id;
			}
			List<Comment> comments = _context.Comments.Include(c => c.ChildComments)
				.IncludeFilter(c => c.CommentLikes.Where(cl => cl.UserId == userId))
				.Include(c => c.User)
				.Where(c => c.ProductId == productId)
				.ToList();

			List<Comment> replyProduct = comments.Where(c => c.ReplyCommentId == null).OrderByDescending(c => c.Id).ToList();

			var replyCommentsDto = replyProduct.Select(c => _mapper.Map<GetCommentDto>(c)).ToList();

			return Ok(replyCommentsDto);
		}
		public IActionResult LikeComment(int commentId)
		{
			if (!User.Identity.IsAuthenticated)
				return BadRequest(new { data = "لطفا ابتدا احراز هویت کنید" });
			if (!_context.Comments.Any(c => c.Id == commentId))
				return BadRequest(new { data = "لطفا ما را امتحان نکنید" });
			var mobileNumber = User.Identity.Name;
			var user = _context.Users.Include(u => u.CommentLikes).FirstOrDefault(u => u.Mobile == mobileNumber);
			var commentLike = user.CommentLikes.FirstOrDefault(cl => cl.CommentId == commentId);
			if (commentLike != null)
				_context.CommentLikes.Remove(commentLike);
			else
			{
				commentLike = new CommentLike() { UserId = user.Id, CommentId = commentId };
				_context.CommentLikes.Add(commentLike);
			}
			_context.SaveChanges();
			return Ok(new { data = "عملیات با موفقیت انجام شد" });
		}
		public IActionResult AddComment(AddCommentDto Dto)
		{
			ModelState.Remove("Depth");
			if(!User.Identity.IsAuthenticated)
				return Unauthorized();
			Dto.UserId = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name).Id;
			if (!ModelState.IsValid)
				return BadRequest();
			var comment = _mapper.Map<Comment>(Dto);
			_context.Add(comment);
			_context.SaveChanges();
			return NoContent();
		}

		#endregion
	}
}