using CetStudentBook.Data;
using CetStudentBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CetStudentBook.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var items = await _db.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .OrderBy(ci => ci.Id)
                .ToListAsync();

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return NotFound();

            var existing = await _db.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

            if (existing == null)
            {
                _db.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1,
                    UnitPrice = product.Price
                });
            }
            else
            {
                existing.Quantity += 1;
                existing.UnitPrice = product.Price;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var item = await _db.CartItems.FirstOrDefaultAsync(ci => ci.Id == id && ci.UserId == userId);
            if (item == null) return NotFound();

            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var cartItems = await _db.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
                return RedirectToAction("Index");

            using var tx = await _db.Database.BeginTransactionAsync();

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var ci in cartItems)
            {
                order.Items.Add(new OrderItem
                {
                    ProductName = ci.Product!.Name,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                });
            }

            order.TotalPrice = order.Items.Sum(i => i.Quantity * i.UnitPrice);

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cartItems);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return RedirectToAction("MyOrders", "Orders");
        }
    }
}