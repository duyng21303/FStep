using ThucHanhWebMVC.Models;
namespace ThucHanhWebMVC.Repository
{
    public interface LoaiSPRepository
    {
        TLoaiSp Add(TLoaiSp loaiSp);
        TLoaiSp Update(TLoaiSp loaiSp);
        TLoaiSp Delete(TLoaiSp loaiSp);
        TLoaiSp Get(TLoaiSp loaiSp);
        IEnumerable<TLoaiSp> GetAllLoaiSp();
    }
}
