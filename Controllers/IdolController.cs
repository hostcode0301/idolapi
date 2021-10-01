using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using idolapi.DB.DTOs;
using idolapi.DB.Models;
using idolapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using idolapi.Helper.File;

namespace idolapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IdolController : ControllerBase
    {
        private readonly IIdolService _idolService;
        private readonly IMapper _mapper;

        public IdolController(IIdolService idolService, IMapper mapper)
        {
            _idolService = idolService;
            _mapper = mapper;
        }

        /// <summary>
        /// List all idols in database
        /// </summary>
        /// <returns>200: List of idol</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<IdolDTO>>> ListAllIdol()
        {

            var idolList = await _idolService.ListAllIdol();

            return Ok(_mapper.Map<IEnumerable<IdolDTO>>(idolList));
        }

        /// <summary>
        /// List idols with pageParameters support for pagination in clients
        /// </summary>
        /// <param name="pageParameters">Query parameters (pageNumber, pageSize, searchName)</param>
        /// <returns>200: (totalReport, list idol) / 400: not found</returns>
        [AllowAnonymous]
        [HttpGet("page")]
        public async Task<ActionResult<PageResponseDTO<IEnumerable<IdolDTO>>>> GetIdols([FromQuery] PageParameters pageParameters)
        {
            // Deconstruct the return Tuple to parameter
            (int totalRecord, IEnumerable<Idol> idols) = await _idolService.GetIdols(pageParameters);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404));
            }

            return Ok(new PageResponseDTO<IEnumerable<IdolDTO>>(totalRecord, _mapper.Map<IEnumerable<IdolDTO>>(idols)));
        }

        /// <summary>
        /// Get an idol using idol Id
        /// </summary>
        /// <param name="idolId">Id of idol to get</param>
        /// <returns>200: Idol / 404: Not found</returns>
        [AllowAnonymous]
        [HttpGet("{idolId:int}")]
        public async Task<ActionResult<IdolDTO>> GetIdol(int idolId)
        {
            var idol = await _idolService.GetIdolById(idolId);

            if (idol == null)
            {
                return NotFound(new ResponseDTO(404, "Idol not found"));
            }

            return Ok(_mapper.Map<IdolDTO>(idol));
        }

        /// <summary>
        /// Add new idol to database
        /// </summary>
        /// <param name="idolInput"></param>
        /// <returns>201: Created / 400: Model fail / 409: Existed</returns>
        [HttpPost]
        public async Task<ActionResult> CreateIdol([FromBody] IdolInput idolInput)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Map input idol to idol model
            var idol = _mapper.Map<Idol>(idolInput);

            var rs = await _idolService.InsertIdol(idol);

            if (rs != 1)
            {
                return Conflict(new ResponseDTO(409, "Idol is already exist"));
            }

            return Created("", new ResponseDTO(201, "Insert done"));
        }

        /// <summary>
        /// Delete a idol with selected id
        /// </summary>
        /// <param name="idolId">If of idol want to delete</param>
        /// <returns>200: Deleted / 404: Not found</returns>
        [HttpDelete("{idolId:int}")]
        public async Task<ActionResult> DeleteIdol(int idolId)
        {
            var rs = await _idolService.DeleteIdol(idolId);

            if (rs != 1)
            {
                return NotFound(new ResponseDTO(404, "Idol not found"));
            }

            return Ok(new ResponseDTO(200, "Idol deleted"));
        }

        /// <summary>
        /// Update an existed Idol with new data
        /// </summary>
        /// <param name="idolInput">New idol data</param>
        /// <param name="idolId">Id of selected Idol</param>
        /// <returns>200: Updated / 404: Not found / 409: Bad request</returns>
        [HttpPut("{idolId:int}")]
        public async Task<ActionResult> UpdateIdol([FromBody] IdolInput idolInput, int idolId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var idol = _mapper.Map<Idol>(idolInput);

            var rs = await _idolService.UpdateIdol(idol, idolId);

            if (rs != 1)
            {
                return NotFound(new ResponseDTO(404, "Idol not found"));
            }

            return Ok(new ResponseDTO(200, "Idol updated"));
        }

        /// <summary>
        /// Import new Idol using excel file with supported format file
        /// </summary>
        /// <param name="file">Excel file</param>
        /// <returns>200: created / 409: Duplicate data / 415: Wrong in file</returns>
        [AllowAnonymous]
        [HttpPost("excel")]
        public async Task<IActionResult> ImportByExcel(IFormFile file)
        {
            var verify = FileHepler.CheckExcelExtension(file);
            if (verify != null)
            {
                var res = new ResponseDTO(415, verify);

                Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                return new JsonResult(res);
            }

            // Get file data
            var idol = new IdolInput();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var row = 2;

                    try
                    {
                        idol.IdolNameJP = worksheet.Cells[row, 1].Value.ToString().Trim();
                        idol.IdolNameRomaji = worksheet.Cells[row, 2].Value.ToString().Trim();
                        idol.IdolNameHira = worksheet.Cells[row, 3].Value.ToString().Trim();
                        idol.DOB = worksheet.Cells[row, 4].Value.ToString().Trim();
                        idol.Bust = float.Parse(worksheet.Cells[row, 5].Value.ToString().Trim());
                        idol.Waist = float.Parse(worksheet.Cells[row, 6].Value.ToString().Trim());
                        idol.Hip = float.Parse(worksheet.Cells[row, 7].Value.ToString().Trim());
                        idol.Height = float.Parse(worksheet.Cells[row, 8].Value.ToString().Trim());
                        idol.ImageURL = worksheet.Cells[row, 9].Value.ToString().Trim();
                    }
                    catch
                    {
                        Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                        return new JsonResult(new ResponseDTO(415, "Wrong format file"));
                    }

                }
            }

            // Map then insert to dbb
            var rs = await _idolService.InsertIdol(_mapper.Map<Idol>(idol));

            if (rs != 1) { return Conflict(new ResponseDTO(409, "Idol is already exist")); }

            return Created("", new ResponseDTO(201, "Insert done"));
        }

        /// <summary>
        /// Export list of idol to excel file
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>200: idol data file</returns>
        [AllowAnonymous]
        [HttpGet("excel")]
        public async Task<IActionResult> ExportToExcel(CancellationToken cancellationToken)
        {
            // Get data from database then map it to DTO
            var idolList = _mapper.Map<ICollection<IdolDTO>>(await _idolService.ListAllIdol());

            // Create a stream to work with file
            var stream = new MemoryStream();

            // Create excel file
            using (var package = new ExcelPackage(stream))
            {
                // Naming the sheet
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                // Insert data
                workSheet.Cells.LoadFromCollection(idolList, true);

                package.Save();
            }

            // Format response file
            stream.Position = 0;
            string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            // Return excel file
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}