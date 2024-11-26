using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Repository
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Kiểm tra xem giá trị có phải là IFormFile không
            if (value is IFormFile file)
            {
                // Lấy phần đuôi mở rộng của file (ví dụ: .jpg, .png)
                var extension = Path.GetExtension(file.FileName).ToLower(); // Sử dụng ToLower để tránh lỗi phân biệt chữ hoa thường
                                                                            // Các định dạng được chấp nhận
                string[] allowedExtensions = { ".jpg", ".png", ".jpeg", ".webp" };

                // Kiểm tra xem phần mở rộng có hợp lệ không
                if (!allowedExtensions.Contains(extension))
                {
                    // Nếu không hợp lệ, trả về lỗi
                    return new ValidationResult("Allowed extensions are .jpg, .png, or .jpeg");
                }
            }
            // Nếu không có file hoặc hợp lệ, trả về thành công
            return ValidationResult.Success;
        }
    }

}
