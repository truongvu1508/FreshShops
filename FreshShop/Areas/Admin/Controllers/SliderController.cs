using FreshShop.Models;
using FreshShop.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
[Route("Admin/Slider")]
public class SliderController : Controller
{
    private readonly DataContext _dataContext;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SliderController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
    {
        _dataContext = dataContext;
        _webHostEnvironment = webHostEnvironment;
    }

    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        return View(await _dataContext.Sliders.OrderByDescending(s => s.Id).ToListAsync());
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SliderModel slider)
    {
        if (ModelState.IsValid)
        {
            if (slider.ImageUpload != null)
            {
                string uploaddir = Path.Combine(_webHostEnvironment.WebRootPath, "media/slider");
                string imgname = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                string filepath = Path.Combine(uploaddir, imgname);

                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                {
                    await slider.ImageUpload.CopyToAsync(fs);
                }

                slider.Image = imgname;
            }

            _dataContext.Add(slider);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Tạo slider thành công.";
            return RedirectToAction("Index");
        }

        TempData["error"] = "Có lỗi trong quá trình nhập liệu.";
        return View(slider);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var slider = await _dataContext.Sliders.FirstAsync(s => s.Id == id);
        if (slider == null)
        {
            return NotFound();
        }
        return View(slider);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SliderModel slider)
    {
        var slider_existed = await _dataContext.Sliders.FirstOrDefaultAsync(s => s.Id == id);
        if (slider_existed == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (slider.ImageUpload != null)
            {
                string uploaddir = Path.Combine(_webHostEnvironment.WebRootPath, "media/slider");
                string imgname = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                string filepath = Path.Combine(uploaddir, imgname);

                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                {
                    await slider.ImageUpload.CopyToAsync(fs);
                }

                slider_existed.Image = imgname;
            }

            slider_existed.Name = slider.Name;
            slider_existed.Description = slider.Description;
            slider_existed.Status = slider.Status;

            _dataContext.Update(slider_existed);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật slider thành công.";
            return RedirectToAction("Index");
        }

        TempData["error"] = "Có lỗi trong quá trình nhập liệu.";
        return View(slider);
    }


    public async Task<IActionResult> Delete(int Id)
    {
        SliderModel slider = await _dataContext.Sliders.FirstAsync(s => s.Id == Id);
        string uploaddir = Path.Combine(_webHostEnvironment.WebRootPath, "media/slider");
        string oldfile = Path.Combine(uploaddir, slider.Image);
        if (System.IO.File.Exists(oldfile))
        {
            System.IO.File.Delete(oldfile);
        }
        _dataContext.Sliders.Remove(slider);
        await _dataContext.SaveChangesAsync();
        return RedirectToAction("Index");
    }

}
