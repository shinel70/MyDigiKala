using AutoMapper;
using DigiKala.Core.Dtoes;
using DigiKala.DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigiKala.Core.Classes
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<CommentLike, GetCommentLikeDto>();
			CreateMap<GetCommentLikeDto, CommentLike>();
			CreateMap<Comment, GetCommentDto>()
				.ForMember(cDto => cDto.ChildComments, c => c.MapFrom(c => c.ChildComments.Select(t => t).ToList()))
				.ForMember(cDto => cDto.CommentLikes, c => c.MapFrom(c => c.CommentLikes.Select(t => t).ToList()))
				.ForMember(dist => dist.UserFullName, opt => opt.MapFrom(src => src.User.FullName));
			CreateMap<AddCommentDto, Comment>();
			
		}
	}
}
