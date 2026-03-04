

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using GalleryApp.Data;
//using GalleryApp.Models;

//namespace GalleryApp.Controllers
//{
//    public class GalleryController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IWebHostEnvironment _env;

//        public GalleryController(ApplicationDbContext context, IWebHostEnvironment env)
//        {
//            _context = context;
//            _env = env;
//        }

//        // Anyone can view gallery
//        public IActionResult Index(string? category, string? search, string? sort)
//        {
//            var photos = _context.Photos.AsQueryable();

//            if (!string.IsNullOrEmpty(category))
//                photos = photos.Where(p => p.Category == category);

//            if (!string.IsNullOrEmpty(search))
//                photos = photos.Where(p =>
//                    (p.Title != null && p.Title.Contains(search)) ||
//                    (p.Description != null && p.Description.Contains(search)));

//            photos = sort switch
//            {
//                "oldest" => photos.OrderBy(p => p.CreatedDate),
//                "views" => photos.OrderByDescending(p => p.ViewCount),
//                _ => photos.OrderByDescending(p => p.CreatedDate)
//            };

//            ViewBag.Category = category;
//            ViewBag.Search = search;
//            ViewBag.Sort = sort;
//            ViewBag.Categories = _context.Photos
//                .Where(p => p.Category != null && p.Category != "")
//                .Select(p => p.Category)
//                .Distinct().ToList();
//            ViewBag.TotalPhotos = _context.Photos.Count();
//            ViewBag.TotalViews = _context.Photos.Sum(p => (int?)p.ViewCount) ?? 0;

//            return View(photos.ToList());
//        }

//        // Anyone can view details
//        public IActionResult Details(int id)
//        {
//            var photo = _context.Photos.Find(id);
//            if (photo == null) return NotFound();

//            photo.ViewCount++;
//            _context.SaveChanges();

//            ViewBag.Related = _context.Photos
//                .Where(p => p.Id != id && p.Category == photo.Category)
//                .OrderByDescending(p => p.CreatedDate)
//                .Take(4).ToList();

//            return View(photo);
//        }

//        // Only logged in users
//        [Authorize]
//        public IActionResult Upload()
//        {
//            return View();
//        }

//        [Authorize]
//        [HttpPost]
//        public async Task<IActionResult> Upload(Photo photo, IFormFile imageFile)
//        {
//            if (imageFile != null)
//            {
//                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
//                var path = Path.Combine(_env.WebRootPath, "images", fileName);

//                using (var stream = new FileStream(path, FileMode.Create))
//                    await imageFile.CopyToAsync(stream);

//                photo.ImagePath = "/images/" + fileName;
//                photo.CreatedDate = DateTime.Now;
//                photo.Title = photo.Title ?? "Untitled";
//                photo.ViewCount = 0;

//                _context.Photos.Add(photo);
//                await _context.SaveChangesAsync();
//            }

//            return RedirectToAction("Index");
//        }

//        [Authorize]
//        public IActionResult Edit(int id)
//        {
//            var photo = _context.Photos.Find(id);
//            if (photo == null) return NotFound();
//            return View(photo);
//        }

//        [Authorize]
//        [HttpPost]
//        public IActionResult Edit(Photo photo)
//        {
//            _context.Photos.Update(photo);
//            _context.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        [Authorize]
//        public IActionResult Delete(int id)
//        {
//            var photo = _context.Photos.Find(id);
//            if (photo != null)
//            {
//                var imagePath = Path.Combine(_env.WebRootPath,
//                    (photo.ImagePath ?? "").TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
//                if (System.IO.File.Exists(imagePath))
//                    System.IO.File.Delete(imagePath);

//                _context.Photos.Remove(photo);
//                _context.SaveChanges();
//            }
//            return RedirectToAction("Index");
//        }
//    }
//}







using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GalleryApp.Data;
using GalleryApp.Models;

namespace GalleryApp.Controllers
{
    public class GalleryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public GalleryController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        // Anyone can view
        public IActionResult Index(string? category, string? search, string? sort)
        {
            var photos = _context.Photos.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                photos = photos.Where(p => p.Category == category);

            if (!string.IsNullOrEmpty(search))
                photos = photos.Where(p =>
                    (p.Title != null && p.Title.Contains(search)) ||
                    (p.Description != null && p.Description.Contains(search)) ||
                    (p.UploaderEmail != null && p.UploaderEmail.Contains(search)));

            photos = sort switch
            {
                "oldest" => photos.OrderBy(p => p.CreatedDate),
                "views" => photos.OrderByDescending(p => p.ViewCount),
                _ => photos.OrderByDescending(p => p.CreatedDate)
            };

            ViewBag.Category = category;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.CurrentUser = _userManager.GetUserId(User);
            ViewBag.Categories = _context.Photos
                .Where(p => p.Category != null && p.Category != "")
                .Select(p => p.Category)
                .Distinct().ToList();
            ViewBag.TotalPhotos = _context.Photos.Count();
            ViewBag.TotalViews = _context.Photos.Sum(p => (int?)p.ViewCount) ?? 0;
            ViewBag.TotalUploaders = _context.Photos
                .Where(p => p.UserId != null)
                .Select(p => p.UserId)
                .Distinct().Count();

            return View(photos.ToList());
        }

        // Anyone can view details
        public IActionResult Details(int id)
        {
            var photo = _context.Photos.Find(id);
            if (photo == null) return NotFound();

            photo.ViewCount++;
            _context.SaveChanges();

            ViewBag.IsOwner = User.Identity != null &&
                              User.Identity.IsAuthenticated &&
                              _userManager.GetUserId(User) == photo.UserId;

            ViewBag.Related = _context.Photos
                .Where(p => p.Id != id && p.Category == photo.Category)
                .OrderByDescending(p => p.CreatedDate)
                .Take(4).ToList();

            return View(photo);
        }

        // Only logged in users can upload
        [Authorize]
        public IActionResult Upload()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Upload(Photo photo, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(_env.WebRootPath, "images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                    await imageFile.CopyToAsync(stream);

                var user = await _userManager.GetUserAsync(User);

                photo.ImagePath = "/images/" + fileName;
                photo.CreatedDate = DateTime.Now;
                photo.Title = photo.Title ?? "Untitled";
                photo.ViewCount = 0;
                photo.UserId = user?.Id;
                photo.UploaderEmail = user?.Email;

                _context.Photos.Add(photo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // Only owner can edit
        [Authorize]
        public IActionResult Edit(int id)
        {
            var photo = _context.Photos.Find(id);
            if (photo == null) return NotFound();

            if (photo.UserId != _userManager.GetUserId(User))
                return Forbid();

            return View(photo);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(Photo photo)
        {
            var existing = _context.Photos.Find(photo.Id);
            if (existing == null) return NotFound();

            if (existing.UserId != _userManager.GetUserId(User))
                return Forbid();

            existing.Title = photo.Title;
            existing.Description = photo.Description;
            existing.Category = photo.Category;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Only owner can delete
        [Authorize]
        public IActionResult Delete(int id)
        {
            var photo = _context.Photos.Find(id);
            if (photo == null) return NotFound();

            if (photo.UserId != _userManager.GetUserId(User))
                return Forbid();

            var imagePath = Path.Combine(_env.WebRootPath,
                (photo.ImagePath ?? "").TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            _context.Photos.Remove(photo);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}