using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> categoryList = _db.Categories.ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name.ToLower() == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "both cannot be same");
            }
            if(ModelState.IsValid)
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category cat =_db.Categories.Find(id);
            if (cat != null)
            {
                return View(cat);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (category.Name.ToLower() == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "both cannot be same");
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category cat = _db.Categories.Find(id);
            if (cat != null)
            {
                return View(cat);
            }
            return NotFound();
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(Category category)
        {
            if (ModelState.IsValid)
            {
                Category? obj = _db.Categories.Find(category.Id);
                if (obj != null)
                {
                    _db.Categories.Remove(obj);
                    _db.SaveChanges();
                    TempData["success"] = "Category delete successfully";
                    return RedirectToAction("Index");
                }

            }
            return View("Index");
        }

    }
}
