using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FStep.Services
{
	public class AutoTriggerService : BackgroundService
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public AutoTriggerService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using (var client = _httpClientFactory.CreateClient())
				{
					// Thay đổi URL thành URL phù hợp với ứng dụng của bạn
					var response = await client.GetAsync("https://localhost:7171/Pay/AutoCheckTransaction");
					response.EnsureSuccessStatusCode();
				}

				// Đợi một khoảng thời gian trước khi thực hiện lại
				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Thực hiện mỗi 1 giờ
			}
		}
	}
}
