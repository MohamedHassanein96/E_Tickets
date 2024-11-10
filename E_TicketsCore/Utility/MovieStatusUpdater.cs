using E_TicketsCore.IUnitOfWorkRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class MovieStatusUpdater : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MovieStatusUpdater(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() => UpdateMovieStatuses(stoppingToken));
        return Task.CompletedTask; 
    }

    private void UpdateMovieStatuses(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWorkRepository>();
                var movies = unitOfWork.Movies.Get().ToList();

                foreach (var movie in movies)
                {
                    movie.UpdateStatus(); 
                }

                unitOfWork.Complete(); 
            }

            Thread.Sleep(TimeSpan.FromDays(1));
        }
    }
}
