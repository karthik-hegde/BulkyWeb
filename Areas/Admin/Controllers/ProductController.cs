using Bulky.DataAccess.Repository.IRepository;
using Bulky.DataAccess.ViewModels;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnv;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnv)
        {
            _unitOfWork = unitOfWork;
            _webHostEnv = webHostEnv;
        }
        public IActionResult Index()
        {
            List<Product> ProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(ProductList);
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),
            });
            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }

        }

        [HttpPost, ActionName("Upsert")]
        public IActionResult UpsertPOST(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string assetPath = _webHostEnv.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(assetPath, @"images\product");
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // delete old image
                        var oldImagePath = Path.Combine(assetPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.CreateNew))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
                productVM.CategoryList = CategoryList;
            }
            return View(productVM);
        }

        //public IActionResult Edit(int id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    Product product = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (product != null)
        //    {
        //        return View(product);
        //    }
        //    return NotFound();
        //}

        //[HttpPost]
        //public IActionResult Edit(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //            _unitOfWork.Product.Update(product);
        //            _unitOfWork.Save();
        //            TempData["success"] = "Product Updated successfully";
        //            return RedirectToAction("Index");

        //    }
        //    return View(product);
        //}
        public IActionResult Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product product = _unitOfWork.Product.Get(u => u.Id == id);
            if (product != null)
            {
                return View(product);
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(Product product)
        {
            if (ModelState.IsValid)
            {
                Product? obj = _unitOfWork.Product.Get(u => u.Id == product.Id);
                if (obj != null)
                {
                    _unitOfWork.Product.Remove(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Product delete successfully";
                    return RedirectToAction("Index");
                }

            }
            return View("Index");
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> ProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return Json(new { data = ProductList });
        } 

        #endregion

    }
}
