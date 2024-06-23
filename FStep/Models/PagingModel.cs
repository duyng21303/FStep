namespace FStep.Models
{
	public class PagingModel<T>
	{
		public IEnumerable<T> Items { get; set; }
		public PagingInfo PagingInfo { get; set; }
	}
}
