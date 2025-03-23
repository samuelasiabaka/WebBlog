using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            // Validate the incoming request
            if (!ModelState.IsValid)
            {
                // Return the view with validation errors
                return View("Add", addTagRequest);
            }

            try
            {
                // Mapping AddTagRequest to Tag domain model
                var tag = new Tag
                {
                    Name = addTagRequest.Name,
                    DisplayName = addTagRequest.DisplayName
                };

                // Add the tag to the database
                await webBlogDbContext.Tags.AddAsync(tag);
                await webBlogDbContext.SaveChangesAsync();

                // Redirect to the list view on success
                return RedirectToAction("List");
            }
            catch (DbUpdateException)
            {

                // Handle database update errors (e.g., duplicate key, constraints violation)
                ModelState.AddModelError("", "An error occurred while saving the tag. Please try again.");
                return View("Add", addTagRequest);
            }
            catch (Exception)
            {
                // Log the exception
                // Example: _logger.LogError(ex, "An unexpected error occurred.");

                // Handle unexpected errors
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View("Add", addTagRequest);
            }
        }


        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            //use db context to read tags
            var tags = await webBlogDbContext.Tags.ToListAsync();

            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // Await the asynchronous operation to get the Tag object
            var tag = await webBlogDbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);

            if (tag != null)
            {
                // Map the Tag to the EditTagRequest ViewModel
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };

                // Return the view with the EditTagRequest model
                return View(editTagRequest);
            }

            // Handle the case where the tag is not found
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

            var existingTag = await webBlogDbContext.Tags.FindAsync(tag.Id);

            if (existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;

                await webBlogDbContext.SaveChangesAsync();

                return RedirectToAction("List");
            }

            return RedirectToAction("Edit", new { id = editTagRequest.Id });

        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Await the asynchronous operation to get the Tag object
            var tag = await webBlogDbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);

            if (tag != null)
            {
                
                // Return the view with the EditTagRequest model
                return View(tag);
            }

            // Handle the case where the tag is not found
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {

            var existingTag = await webBlogDbContext.Tags.FindAsync(editTagRequest.Id);

            if (existingTag != null)
            {

                webBlogDbContext.Remove(existingTag);
                await webBlogDbContext.SaveChangesAsync();

                return RedirectToAction("List");
            }

            return RedirectToAction("Edit", new { id = editTagRequest.Id });

        }


    }

}
