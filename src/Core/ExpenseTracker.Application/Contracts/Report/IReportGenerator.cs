
namespace ExpenseTracker.Application.Contracts.Report;

public interface IReportGenerator<T>
{
    Task<byte[]> GenerateAsync(T reportData);
}

public interface IExcelReportGenerator<T> : IReportGenerator<T> { }
public interface IPdfReportGenerator<T> : IReportGenerator<T> { }

