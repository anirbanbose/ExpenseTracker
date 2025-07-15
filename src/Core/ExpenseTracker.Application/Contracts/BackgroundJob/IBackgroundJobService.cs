
using System.Linq.Expressions;

namespace ExpenseTracker.Application.Contracts.BackgroundJob;

public interface IBackgroundJobService
{
    void Enqueue<TJob>(Expression<Func<TJob, Task>> methodCall) where TJob : IBackgroundJob;
}
