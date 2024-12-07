using FreshShop.Models;
using FreshShop.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;

        public OrderController(DataContext context)
        {
            _dataContext = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
        }

        // GET: Create Order
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderModel order)
        {
            if (ModelState.IsValid)
            {
                // Get the last order code from the database and increment it
                var lastOrder = await _dataContext.Orders
                    .OrderByDescending(o => o.Id)
                    .FirstOrDefaultAsync();

                // Ensure the new order code follows the format "ORD000001", etc.
                int newOrderCode = (lastOrder != null) ? int.Parse(lastOrder.OrderCode.Substring(3)) + 1 : 1;
                order.OrderCode = $"ORD{newOrderCode:D6}";
                order.CreatedDate = DateTime.Now;

                _dataContext.Orders.Add(order);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Tạo đơn hàng thành công.";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Có lỗi trong quá trình nhập liệu.";
            return View(order);
        }

        // GET: Delete Order
        public async Task<IActionResult> Delete(int Id)
        {
            var order = await _dataContext.Orders.FindAsync(Id);

            if (order == null)
            {
                TempData["error"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }

            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Đơn hàng đã được xóa.";
            return RedirectToAction("Index");
        }

        // GET: View Order Details
        public async Task<IActionResult> ViewOrder(string orderCode)
        {
            if (string.IsNullOrEmpty(orderCode))
            {
                TempData["error"] = "Mã đơn hàng không hợp lệ.";
                return RedirectToAction("Index");
            }

            var order = await _dataContext.Orders
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            if (order == null)
            {
                TempData["error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Index");
            }

            var orderDetails = await _dataContext.OrderDetails
                .Where(o => o.OrderCode == orderCode)
                .Include(o => o.Product) // Including the product details if needed
                .ToListAsync();

            if (!orderDetails.Any())
            {
                TempData["error"] = "Không tìm thấy chi tiết đơn hàng.";
                return RedirectToAction("Index");
            }

            ViewData["OrderCode"] = orderCode;  // Set orderCode into ViewData
            ViewData["OrderStatus"] = order.Status; // Pass order status to ViewData

            return View(orderDetails);
        }

        // GET: ViewOrderEdit (to display the edit form)
        public async Task<IActionResult> ViewOrderEdit(int id)
        {
            var orderDetail = await _dataContext.OrderDetails
                .Include(o => o.Product) // Include product information
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orderDetail == null)
            {
                TempData["error"] = "Chi tiết đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }

            // Kiểm tra xem sản phẩm có tồn tại không
            if (orderDetail.Product == null)
            {
                TempData["error"] = "Không tìm thấy sản phẩm liên kết với đơn hàng.";
                return RedirectToAction("Index");
            }

            return View(orderDetail); // Pass the order detail to the view
        }

        // POST: ViewOrderEdit (to update the order detail)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewOrderEdit(OrderDetails model)
        {
            if (ModelState.IsValid)
            {
                var orderDetail = await _dataContext.OrderDetails
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                if (orderDetail == null)
                {
                    TempData["error"] = "Chi tiết đơn hàng không tồn tại.";
                    return RedirectToAction("Index");
                }

                // Kiểm tra xem ProductId có hợp lệ không
                var product = await _dataContext.Products
                    .FirstOrDefaultAsync(p => p.Id == model.ProductId);

                if (product == null)
                {
                    TempData["error"] = "Sản phẩm không tồn tại.";
                    return RedirectToAction("ViewOrderEdit", new { id = model.Id });
                }

                // Cập nhật các thông tin của đơn hàng
                orderDetail.Quantity = model.Quantity;
                orderDetail.Price = product.Price;  // Cập nhật giá từ sản phẩm
                orderDetail.ProductId = model.ProductId; // Cập nhật ProductId

                // Lưu thay đổi vào cơ sở dữ liệu
                _dataContext.OrderDetails.Update(orderDetail);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Cập nhật chi tiết đơn hàng thành công!";
                return RedirectToAction("ViewOrder", new { orderCode = orderDetail.OrderCode });
            }

            TempData["error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
            return View(model);
        }


        // GET: Update Order Status
        // GET: Update Order Status
        [HttpPost]
public async Task<IActionResult> UpdateOrderStatus(string orderCode, int status)
{
            if (string.IsNullOrEmpty(orderCode))
            {
                TempData["error"] = "Mã đơn hàng không hợp lệ.";
                return RedirectToAction("Index");
            }

            var order = await _dataContext.Orders
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            if (order == null)
            {
                TempData["error"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = status;
            await _dataContext.SaveChangesAsync();

            // Trả về trạng thái mới đã được cập nhật
            return Json(new { success = true, updatedStatus = status });
        }
       
    }
}
