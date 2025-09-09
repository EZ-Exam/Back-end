using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Context;

namespace teamseven.EzExam.Repository.Repository
{
    public class RoleRepository : GenericRepository<Role>
    {
        private readonly teamsevenezexamdbContext _context;

        public RoleRepository(teamsevenezexamdbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>?> GetRoleList()
        {
            return await GetAllAsync();
        }

        public async Task<Role?> GetByName(string name)
       {
        return await _context.Roles.FirstOrDefaultAsync(a => a.RoleName.Contains(name, StringComparison.OrdinalIgnoreCase));
       }

        public async Task<int> AddRoleAsync(Role role)
        {
            return await CreateAsync(role);
        }
        public async Task<int> UpdateRoleAsync(Role role)
        {
           return await UpdateAsync(role);
        }
        public async Task<bool> DeleteRoleAsync(Role role)
        {
            return await RemoveAsync(role);
        }

        public async Task<Role> GetRoleById(int id)
        {
            return await GetByIdAsync(id);
        }
    }


}
