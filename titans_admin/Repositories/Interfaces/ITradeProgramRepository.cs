using titans_admin.Models.Entities;

namespace titans_admin.Repositories.Interfaces;

public interface ITradeProgramRepository : IRepository<TradeProgram>
{
    Task<IEnumerable<TradeProgram>> GetActiveAsync();
    Task<IEnumerable<TradeProgram>> GetByStatusAsync(string status);
    Task<IEnumerable<TradeProgram>> GetByTypeAsync(string programType);
}
