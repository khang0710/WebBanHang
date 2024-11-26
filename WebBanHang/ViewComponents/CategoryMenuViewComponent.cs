using Microsoft.AspNetCore.Mvc;
using WebBanHang.Repository;

namespace WebBanHang.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _category;

        public CategoryMenuViewComponent(ICategoryRepository category)
        {
            _category = category;
        }

        public IViewComponentResult Invoke()
        {
            var category = _category.GetAll().OrderBy(x => x.CategoryId);
            return View(category);
        }
    }
}
