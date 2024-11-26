using WebBanHang.Models;

namespace WebBanHang.Repository
{
    public interface ICategoryRepository
    {
        Category Add(Category category);

        Category Update(Category category);

        Category Delete(int id);

        Category Get(int id);

        IEnumerable<Category> GetAll();
    }
}
