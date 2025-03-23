using Microsoft.AspNetCore.Mvc;
using WebBlog.Data;
using WebBlog.Models.Domain;
using WebBlog.Models.ViewModels;

namespace WebBlog.Controllers
{
    public class AdminTagsController : Controller
    {
        private readonly WebBlogDbContext webBlogDbContext;

        public AdminTagsController(WebBlogDbContext webBlogDbContext)
        {
            this.webBlogDbContext = webBlogDbContext;
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddTagRequest addTagRequest)
        {
            //Mapping AddTagRequest to Tag domain model
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };

            webBlogDbContext.Tags.Add(tag);
            webBlogDbContext.SaveChanges();

            return View("Add");
        }
    }
}
