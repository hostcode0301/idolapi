using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using idolapi.DB;
using idolapi.DB.DTOs;
using idolapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace idolapi.Services
{
    public class IdolService : IIdolService
    {
        private DataContext _dataContext;

        public IdolService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// List all idol in database
        /// </summary>
        /// <returns>List of idols</returns>
        public async Task<List<Idol>> ListAllIdol()
        {
            return await _dataContext.Idols.ToListAsync();
        }

        /// <summary>
        /// Pagination method for client
        /// </summary>
        /// <param name="pageParameters">Query parameters (pageNumber, pageSize, searchName)</param>
        /// <returns>(number of record, list idols) / (0, null list)</returns>
        public async Task<Tuple<int, IEnumerable<Idol>>> GetIdols(PageParameters pageParameters)
        {
            var searchData = await _dataContext.Idols
                    .Where(i => i.IdolNameRomaji.ToLower().Contains(pageParameters.SearchName.ToLower()))
                    .ToListAsync();

            int totalRecord = searchData.Count();

            var rs = searchData.OrderBy(i => i.IdolNameJP)
                    .Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize)
                    .Take(pageParameters.PageSize);

            return Tuple.Create(totalRecord, rs);
        }

        /// <summary>
        /// Get idol method using id
        /// </summary>
        /// <param name="id">Idol id</param>
        /// <returns>Idol data</returns>
        public async Task<Idol> GetIdolById(int id)
        {
            return await _dataContext.Idols.Where(i => i.IdolId == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Insert new idol to database
        /// </summary>
        /// <param name="idol">Idol input data</param>
        /// <returns>-1: existed / 0: fail / 1: done</returns>
        public async Task<int> InsertIdol(Idol idol)
        {
            if (_dataContext.Idols.Any(i =>
                i.IdolNameJP == idol.IdolNameJP &&
                i.IdolNameHira == idol.IdolNameHira &&
                i.IdolNameRomaji == idol.IdolNameRomaji
            ))
            {
                return -1;
            }

            int rowInserted = 0;

            _dataContext.Idols.Add(idol);
            rowInserted = await _dataContext.SaveChangesAsync();

            return rowInserted;
        }

        /// <summary>
        /// Delete a idol in database
        /// </summary>
        /// <param name="id">Idol id</param>
        /// <returns>-1: not found / 0: fail / 1: done</returns>
        public async Task<int> DeleteIdol(int id)
        {

            var existedIdol = await _dataContext.Idols.Where(i => i.IdolId == id).FirstOrDefaultAsync();

            if (existedIdol == null)
            {
                return -1;
            }

            _dataContext.Idols.Remove(existedIdol);
            var rowDeleted = await _dataContext.SaveChangesAsync();

            return rowDeleted;
        }

        /// <summary>
        /// Update an idol with new input data
        /// </summary>
        /// <param name="idol">New idol data</param>
        /// <param name="id">The id to update</param>
        /// <returns>-1: not found / 0: fail / 1: done</returns>
        public async Task<int> UpdateIdol(Idol idol, int id)
        {
            var existedIdol = await _dataContext.Idols.Where(i => i.IdolId == id).FirstOrDefaultAsync();

            if (existedIdol == null)
            {
                return -1;
            }

            // Update every field data in Idol model
            existedIdol.IdolNameJP = idol.IdolNameJP;
            existedIdol.IdolNameRomaji = idol.IdolNameRomaji;
            existedIdol.IdolNameHira = idol.IdolNameHira;
            existedIdol.DOB = idol.DOB;
            existedIdol.Bust = idol.Bust;
            existedIdol.Waist = idol.Waist;
            existedIdol.Hip = idol.Hip;
            existedIdol.Height = idol.Height;
            existedIdol.ImageURL = idol.ImageURL;

            var rowUpdated = await _dataContext.SaveChangesAsync();

            return rowUpdated;
        }
    }
}