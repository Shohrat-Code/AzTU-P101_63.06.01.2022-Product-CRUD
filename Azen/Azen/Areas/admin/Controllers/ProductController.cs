using Azen.Data;
using Azen.Models;
using Azen.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Azen.Areas.admin.Controllers
{
    [Area("admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Products.Include(c => c.ProductCategory).Include(b => b.Brand).ToList());
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.ProductCategories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product model)
        {
            if (ModelState.IsValid)
            {
                VmPrdAll vmPrdAll = new VmPrdAll();
                vmPrdAll.Product = model;
                string prdModel = JsonConvert.SerializeObject(vmPrdAll);
                HttpContext.Session.SetString("Product", prdModel);
                return RedirectToAction("CreateColorToProduct");
            }

            ViewBag.Categories = _context.ProductCategories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            return View(model);
        }

        public IActionResult CreateColorToProduct()
        {
            ViewBag.Colors = _context.Colors.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult CreateColorToProduct(List<VmColorImage> model)
        {
            string prdModelString = HttpContext.Session.GetString("Product");
            VmPrdAll prdModel = JsonConvert.DeserializeObject<VmPrdAll>(prdModelString);

            foreach (var item in model)
            {
                foreach (var image in item.Image)
                {
                    string s = null;
                    using (var ms = new MemoryStream())
                    {
                        image.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        s = Convert.ToBase64String(fileBytes);
                    }

                    item.ImageBase64.Add(s);
                }


                item.Image = null;
            }

            prdModel.ColorImages = model;
            HttpContext.Session.SetString("Product", JsonConvert.SerializeObject(prdModel));

            return RedirectToAction("CreateSizeToColorToProduct");
        }

        public IActionResult CreateSizeToColorToProduct()
        {
            string prdModelString = HttpContext.Session.GetString("Product");
            VmPrdAll prdModel = JsonConvert.DeserializeObject<VmPrdAll>(prdModelString);

            List<Color> colors = new List<Color>();
            foreach (var item in prdModel.ColorImages)
            {
                colors.Add(_context.Colors.FirstOrDefault(c => c.Id == item.ColorId));
            }
            //ViewBag.Colors = _context.Colors.Where(c => prdModel.ColorImages.FirstOrDefault(ci => ci.ColorId == c.Id)!=null).ToList();
            ViewBag.Colors = colors;
            ViewBag.Sizes = _context.Sizes.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult CreateSizeToColorToProduct(List<VmColorImage> model)
        {
            string prdModelString = HttpContext.Session.GetString("Product");
            VmPrdAll prdModel = JsonConvert.DeserializeObject<VmPrdAll>(prdModelString);
            prdModel.ColorImages = model;
            HttpContext.Session.SetString("Product", JsonConvert.SerializeObject(prdModel));

            return View();
        }
    }
}
