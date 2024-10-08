﻿using AutoMapper;
using FStep.Data;
using FStep.ViewModels;

namespace FStep.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<RegisterVM, User>();
                //.ForMember(kh => kh.HoTen, option => option.MapFrom(RegisterVM => RegisterVM.HoTen))
                //.ReverseMap()
            CreateMap<PostVM,Post>();
            CreateMap<PostVM,Product>();
			CreateMap<ProfileVM, User>();
            CreateMap<TransactionVM, Transaction>();
			CreateMap<User, ProfileVM>();
			CreateMap<CommentVM, Comment>().ReverseMap();
			CreateMap<CheckoutVM, Payment>();
			CreateMap<CheckoutVM, Transaction>();

            CreateMap<TransactionVM, Transaction>();
            CreateMap<PaymentVM, Payment>().ReverseMap();

			CreateMap<Transaction, TransactionVM>().ForMember(trans => trans.TransactionId, option => option.MapFrom(RegisterVM => RegisterVM.IdTransaction));

		}

	}
}
