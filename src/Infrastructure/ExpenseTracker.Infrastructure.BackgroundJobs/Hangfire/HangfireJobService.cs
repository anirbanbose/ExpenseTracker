using ExpenseTracker.Application.Contracts.BackgroundJob;
using Hangfire;
using System.Linq.Expressions;

namespace ExpenseTracker.Infrastructure.BackgroundJobs.Hangfire;

public class HangfireBackgroundJobService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireBackgroundJobService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall) where TJob : IBackgroundJob
    {
        _backgroundJobClient.Enqueue(methodCall);
    }
}
