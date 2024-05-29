using ThucHanhWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using ThucHanhWebMVC.Repository;
namespace ThucHanhWebMVC.ViewComponents
{
    public class LoaiSpMenuViewComponent : ViewComponent
    {
        private readonly LoaiSPRepository _loaiSp;
        public LoaiSpMenuViewComponent(LoaiSPRepository loaiSPRepository )
        {
            _loaiSp = loaiSPRepository;
        }
        public IViewComponentResult Invoke()
        {
            var loaisp = _loaiSp.GetAllLoaiSp().OrderBy(x => x.Loai);
            return View(loaisp);
        }
    }
}
