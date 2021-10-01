using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace idolapi.Helper.File
{
    public class FileHepler
    {
        /// <summary>
        /// Check a file is has .xlsx extension or not
        /// </summary>
        /// <param name="file">A file want to check</param>
        /// <returns>true: null / false: error_message</returns>
        public static string CheckExcelExtension(IFormFile file)
        {
            // Check file length
            if (file == null || file.Length <= 0)
            {
                return "No upload file";
            }

            // Check file extension
            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return "Not support file extension";
            }

            // Check MIME type is XLSX
            Stream fs = file.OpenReadStream();
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((Int32)fs.Length);

            var mimeType = HeyRed.Mime.MimeGuesser.GuessMimeType(bytes);

            if (!mimeType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                return "Not support fake extension";
            }

            return null;
        }
    }
}