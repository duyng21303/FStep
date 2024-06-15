<<<<<<< HEAD
﻿using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
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

=======
﻿namespace FStep.ViewModels
{
>>>>>>> develop
	public class ListPostVM
	{
		public int PostId { get; set; }

		public string PostTitle { get; set; } = default!;

		public string PostBody { get; set; }

		public int Quantity { get; set; }

<<<<<<< HEAD
		public String Status { get; set; }

		public String Category { get; set; }
=======

		public string? Status { get; set; }

>>>>>>> develop
		public float Price { get; set; }

		public String Type { get; set; } = default!;

<<<<<<< HEAD
		public String? Location { get; set; }
=======
>>>>>>> develop
		public String StudentId { get; set; } = default!;

		public String Image { get; set; } = default!;

<<<<<<< HEAD
		public int TotalPosts { get; set; }
        public DateTime CreateDate { get; set; }

		public String Email { get; set; }

		// Thuộc tính tính toán DaysRemaining cho Pending
		public int DaysRemainingPending
		{
			get
			{
				if (CreateDate  != null)
				{
					return 7 - (DateTime.Now.Date - CreateDate.Date).Days;
				}
				return 0;
			}
		}

		// Thuộc tính tính toán DaysRemaining cho Approved
		public int DaysRemainingApproved
		{
			get
			{
				if (CreateDate != null)
				{
					return 30 - (DateTime.Now.Date - CreateDate.Date).Days;
				}
				return 0;
			}
		}
	}
}

=======
        public DateTime CreateDate { get; set; }
    }
}
>>>>>>> develop
