using GoDent.DAL.Entities;

namespace GoDent.BLL.Service.Abstraction
{
    public interface IToolService
    {
        Task<IEnumerable<Tool>> GetAllToolsAsync();
        Task<Tool?> GetToolByIdAsync(int id);
        Task<IEnumerable<Tool>> GetToolsByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<Tool>> GetToolsInShortageAsync();
        
        Task<Tool> CreateToolAsync(Tool tool);
        Task<Tool?> UpdateToolAsync(Tool tool);
        Task<bool> DeleteToolAsync(int id);
    }
}
