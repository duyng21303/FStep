using FStep.Data;
using FStep.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class DatabaseChangeService
{
	private readonly FstepDbContext _context;
	private readonly IHubContext<NotificationHub> _hubContext;

	public DatabaseChangeService(FstepDbContext context, IHubContext<NotificationHub> hubContext)
	{
		_context = context;
		_hubContext = hubContext;
	}

	public void OnCommentUpdated(Comment comment)
	{
		Task.Run(async () =>
		{
			await _hubContext.Clients.All.SendAsync("displayAlert", "Product updated");
		});
	}
}
