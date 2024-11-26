
using WebBanHang.Models;

namespace WebBanHang.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SnackStoreContext _context;

        public CategoryRepository(SnackStoreContext context)
        {
            _context = context;
        }
        public Category Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }

        public Category Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Category Get(int id)
        {
            return _context.Categories.Find(id);
        }

        public IEnumerable<Category> GetAll()
        {
            return _context.Categories;
        }


        public Category Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
            return category;
        }
    }
}
