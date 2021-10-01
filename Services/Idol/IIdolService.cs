using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using idolapi.DB.DTOs;
using idolapi.DB.Models;

namespace idolapi.Services
{
    public interface IIdolService
    {
        Task<List<Idol>> ListAllIdol();
        Task<Tuple<int, IEnumerable<Idol>>> GetIdols(PageParameters pageParameters);
        Task<Idol> GetIdolById(int id);
        Task<int> InsertIdol(Idol idol);
        Task<int> DeleteIdol(int id);
        Task<int> UpdateIdol(Idol idol, int id);
    }
}