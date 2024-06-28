﻿namespace FStep.ViewModels.WareHouse
{
	public class WareHouseVM
	{

		public int IdPost { get; set; }

		public string CodeTransaction {  get; set; }
		public string NameProduct { get; set; }
		public string Status { get; set; }
		public string IdStudentBuyer { get; set; }
		public string IdStudentSeller { get; set; }
		public string IdBuyer { get; set; }
		public string IdSeller { get; set; }
		public DateTime Date { get; set; }
		public float? Amount { get; set; }
		public int Quantity { get; set; }

		public string Location { get; set; }


	}
}
