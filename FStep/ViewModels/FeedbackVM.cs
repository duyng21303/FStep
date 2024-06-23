using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Blazor;
using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels
{
	public class FeedbackVM
	{
		public int IdFeedback { get; set; }

		[Display(Name = "Content")]
		[Required(ErrorMessage = "This is required")]
		[MaxLength(250, ErrorMessage = "Maximun 250 characters")]
		public string Content { get; set; }

		[Display(Name = "Rating")]
		[Required(ErrorMessage = "This is required")]
		public string Rating { get; set; }
		public string IdUser { get; set; }
		public int IdPost {  get; set; }
		public string Img {  get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string TypePost { get; set; }
		public int? Quantity { get; set; }
		public float? UnitPrice { get; set; }
		public float? Amount { get; set; }
	}
}
