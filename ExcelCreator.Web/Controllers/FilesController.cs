using ExcelCreator.Web.Database;
using ExcelCreator.Web.Entities;
using ExcelCreator.Web.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ExcelCreator.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppIdentityDbContext _dbContext;
        private readonly IHubContext<FileHub> _hubContext;

        public FilesController(AppIdentityDbContext dbContext, IHubContext<FileHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }


        // worker service api uzerinden excel file upload edecek
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(IFormFile file, int fileId)
        {
            if(file is not { Length: > 0}) return BadRequest();

            UserFile userFile = await _dbContext.UserFiles.FirstAsync(x => x.Id == fileId);   

            string filePath = userFile.FileName + Path.GetExtension(file.FileName);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files",filePath);

            using FileStream stream = new(path, FileMode.Create);
            await file.CopyToAsync(stream);

            userFile.CreatedDate = DateTime.Now;
            userFile.FilePath = filePath;
            userFile.FileStatus = FileStatus.Completed;

            await _dbContext.SaveChangesAsync();

            // worker service excel'i upload edince client'a bildirim gonderilir
            // bildirim oturum acmis user'lar olusturan user'a bildirim gonderilir
            // signal r notification

            await _hubContext.Clients.User(userFile.UserId).SendAsync("CompletedFile");


            return Ok();
        }
    }
}
