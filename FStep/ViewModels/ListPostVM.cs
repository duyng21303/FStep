using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using X.PagedList;

namespace FStep.ViewModels
{
	public class ManagePostsVM
	{
		public IPagedList<ListPostVM> PendingPosts { get; set; }
		public IPagedList<ListPostVM> ApprovedPosts { get; set; }
		public int PendingPostsCount { get; set; }
		public int ApprovedPostsCount { get; set; }
		public string PendingQuery { get; set; }
		public string ApprovedQuery { get; set; }
	}
	public class ApprovedPostsResultVM
	{
		public IPagedList<ListPostVM> Posts { get; set; }
		public int Count { get; set; }
	}

	public class PendingPostsResultVM
	{
		public IPagedList<ListPostVM> Posts { get; set; }
		public int Count { get; set; }
	}

}


	public class ListPostVM
	{
		public int PostId { get; set; }

		public string PostTitle { get; set; } = default!;

		public string PostBody { get; set; }

		public int Quantity { get; set; }


		public String Status { get; set; }

		public String Category { get; set; }

		public float Price { get; set; }

		public String Type { get; set; } = default!;


		public String? Location { get; set; }

		public String StudentId { get; set; } = default!;

		public String Image { get; set; } = default!;

		public int TotalPosts { get; set; }
        public DateTime CreateDate { get; set; }

		public String Email { get; set; }

		// Thuộc tính tính toán DaysRemaining cho Pending
		public int DaysRemaining => (Status == "false") ? (7 - (DateTime.Now.Date - CreateDate.Date).Days) : (30 - (DateTime.Now.Date - CreateDate.Date).Days);
	}
}

