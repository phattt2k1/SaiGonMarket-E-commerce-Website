﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaiGonMarket.Extension;
using SaiGonMarket.Models;
using SaiGonMarket.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaiGonMarket.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly dbMarketsContext _context;
        public INotyfService _notyfService { get; }
        public ShoppingCartController(dbMarketsContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }

        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            List<CartItem> cart = GioHang;
            try
            {
                //Them san pham vao gio hang
                CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)//da co san pham -> cap nhat so luong
                {
                    item.amount = item.amount + amount.Value;
                    // Luu lai session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                else
                {
                    Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = hh
                    };
                    cart.Add(item);//them vao gio hang
                }
                //Luu lai session
                HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                _notyfService.Success("Add to cart successfully");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            //lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                    if (item != null && amount.HasValue)//da co san pham -> cap nhat so luong
                    {
                        item.amount = amount.Value;
                    }
                    //luu lai vao session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult Remove(int productID)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                //Luu lai session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
    }
}
