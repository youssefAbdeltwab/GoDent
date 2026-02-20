using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Data;
using GoDent.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoDent.BLL.Services
{
    public class ToolService : IToolService
    {
        private readonly AppDbContext _context;

        public ToolService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tool>> GetAllToolsAsync()
        {
            return await _context.Tools
                .Include(t => t.Department)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tool> GetToolsByDepartmentIdAsync(int departmentId)
        {
            return await _context.Tools
                .Include(t => t.Department)
                .Where(t => t.DepartmentId == departmentId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    
        public async Task<Tool?> GetToolByIdAsync(int id)
        {
            return await _context.Tools
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Tool>> GetToolsInShortageAsync()
        {
            return await _context.Tools
                .Include(t => t.Department)
                .Where(t => t.Quantity <= t.MinQuantity)
                .OrderBy(t => t.Quantity)
                .ToListAsync();
        }

        public async Task<Tool> CreateToolAsync(Tool tool)
        {
            _context.Tools.Add(tool);
            await _context.SaveChangesAsync();
            return tool;
        }

        public async Task<Tool?> UpdateToolAsync(Tool tool)
        {
            var existingTool = await _context.Tools.FindAsync(tool.Id);
            if (existingTool == null) return null;

            existingTool.Name = tool.Name;
            existingTool.Description = tool.Description;
            existingTool.Quantity = tool.Quantity;
            existingTool.MinQuantity = tool.MinQuantity;
            existingTool.Price = tool.Price;
            existingTool.DepartmentId = tool.DepartmentId;

            _context.Tools.Update(existingTool);
            await _context.SaveChangesAsync();
            return existingTool;
        }

        public async Task<bool> DeleteToolAsync(int id)
        {
            var tool = await _context.Tools.FindAsync(id);
            if (tool == null) return false;

            _context.Tools.Remove(tool);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
