using ExcelCreator.Web.Database;
using ExcelCreator.Web.Entities;
using ExcelCreator.Web.PubSub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExcelCreator.Web.Controllers
{

    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppIdentityDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public ProductController(AppIdentityDbContext dbContext, UserManager<IdentityUser> userManager, RabbitMQPublisher rabbitMQPublisher)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel()
        {
            // user name cookie'den alınır
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            var fileName = $"product-excel-{Guid.NewGuid()}";

            UserFile userFile = new()
            {
                UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating,
            };

            await _dbContext.UserFiles.AddAsync(userFile);

            await _dbContext.SaveChangesAsync();

            // file ekleme yapilince rabbitmq'a mesaj gonder
            _rabbitMQPublisher.Publish(new()
            {
                FileId = userFile.Id,
                UserId = user.Id,
            });

            // bir request'ten diger bir request'te veri tasiyabilirim (cookie ile tasiyor)
            // viewbag aynı request'te datayi modele tasinir
            TempData["StartCreatingExcel"] = true;

            return RedirectToAction(nameof(Files));
        }

        public async Task<IActionResult> Files()
        {
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            return View(await _dbContext.UserFiles.Where(x => x.UserId == user.Id).ToListAsync());
        }
    }
}
