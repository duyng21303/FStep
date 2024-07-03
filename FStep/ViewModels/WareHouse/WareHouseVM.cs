﻿using FStep.Data;
using X.PagedList;

namespace FStep.ViewModels.WareHouse
{
	public class WareHouseVM
	{

		public CommentExchangeVM? CommentExchangeVM { get; set; }
		public PostVM? PostVM { get; set; }
		public TransactionVM? TransactionVM { get; set; }
		public string CodeTransaction {  get; set; }
		public string NameProduct { get; set; }
		public string Status { get; set; }
		public string IdStudentBuyer { get; set; }
		public string IdStudentSeller { get; set; }
		public string IdBuyer { get; set; }
		public string IdSeller { get; set; }
		public User? UserBuyer { get; set; }
		public User? UserSeller { get; set; }
		public DateTime Date { get; set; }
		public float? Amount { get; set; }
		public int Quantity { get; set; }
		public string? Type { get; set; }
		public string Location { get; set; }

		public int ProcessCount { get; set; }
		public int FinishCount { get; set; }
		public int CancelCount { get; set; }
	}
	public class WareHouseServiceVM
	{
		public IPagedList<WareHouseVM> ExchangeList { get; set; }
		public IPagedList<WareHouseVM> SaleList { get; set; }
		public int ProcessCount { get; set; }
		public int FinishCount { get; set; }
		public int CancelCount { get; set; }
	}
}
