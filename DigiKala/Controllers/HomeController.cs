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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using DigiKala.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace DigiKala.Controllers
{
	public class HomeController : Controller
	{
		private readonly ITemp _temp;
		private readonly DatabaseContext _context;
		private readonly IMapper _mapper;
		private readonly IMemoryCache _cache;

		public HomeController(ITemp temp, DatabaseContext context, IMapper mapper, IMemoryCache cache)
		{
			_temp = temp;
			_context = context;
			_mapper = mapper;
			_cache = cache;
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

			ProductSeen productSeen = new ProductSeen()
			{
				DateTime = DateTime.Now,
				IP = HttpContext.Connection.RemoteIpAddress.ToString(),
				ProductId = product.Id
			};
			if (!_cache.TryGetValue($"p{HttpContext.Connection.RemoteIpAddress.ToString()}", out var data))
			{
				_cache.Set($"p{HttpContext.Connection.RemoteIpAddress.ToString()}", "", new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
				_context.ProductSeens.Add(productSeen);
				_context.SaveChanges();
			}

			var sessionProductsSeen = new List<int>();
			if (string.IsNullOrEmpty(HttpContext.Session.GetString(StaticData.productSeen)))
			{
				sessionProductsSeen.Add(product.Id);
				HttpContext.Session.SetObject(StaticData.productSeen, sessionProductsSeen);
			}
			else
			{
				sessionProductsSeen = HttpContext.Session.GetObject<List<int>>(StaticData.productSeen);
				if (!sessionProductsSeen.Contains(product.Id))
				{
					sessionProductsSeen.Add(product.Id);
					HttpContext.Session.SetObject(StaticData.productSeen, sessionProductsSeen);
				}
			}

			viewModel.FillProduct = product;
			return View(viewModel);
		}
		public IActionResult ShowOrders()
		{
			if (!User.Identity.IsAuthenticated)
				return Redirect("/Accounts/Register");
			User user = _context.Users.FirstOrDefault(u => u.Mobile == User.Identity.Name);
			return Ok();
		}
		//[Authorize]
		[HttpGet]
		public IActionResult AddOrder([FromQuery(Name = "productIdQties")]string productIdQties)
		{
			var viewModels = JsonConvert.DeserializeObject<List<AddOrderHttpGetViewModel>>(productIdQties);
			AddOrderViewModel addOrderViewModel = new AddOrderViewModel();
			addOrderViewModel.Products = _context.Products.Where(p => viewModels.Any(vm => vm.Id == p.Id) && p.Exist > 0).ToList();
			addOrderViewModel.User = _context.Users.Include(u => u.Addresses).FirstOrDefault(u => u.Mobile == User.Identity.Name);
			return View(viewModels);
		}
		[HttpPost]
		public IActionResult AddOrder(AddOrderViewModel viewModel)
		{
			return View();
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
			if (!User.Identity.IsAuthenticated)
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