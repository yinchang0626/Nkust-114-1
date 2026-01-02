using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.DTOs
{
    /// <summary>
    /// ????????
    /// </summary>
    public class FoodDto
    {
        public int Id { get; set; }
        
        /// <summary>
        /// ????
        /// </summary>
        public string IntegratedNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// ????
        /// </summary>
        public string SampleName { get; set; } = string.Empty;
        
        /// <summary>
        /// ??????
        /// </summary>
        public string? SampleEnglishName { get; set; }
        
        /// <summary>
        /// ??
        /// </summary>
        public string? CommonName { get; set; }
        
        /// <summary>
        /// ?????
        /// </summary>
        public string? ContentDescription { get; set; }
        
        /// <summary>
        /// ????
        /// </summary>
        public string? FoodCategory { get; set; }
        
        /// <summary>
        /// ????
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// ?????? DTO
    /// </summary>
    public class CreateFoodDto
    {
        /// <summary>
        /// ????
        /// </summary>
        [Required(ErrorMessage = "?????????")]
        [StringLength(100, ErrorMessage = "?????????? 100 ??")]
        public string IntegratedNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// ????
        /// </summary>
        [Required(ErrorMessage = "?????????")]
        [StringLength(500, ErrorMessage = "?????????? 500 ??")]
        public string SampleName { get; set; } = string.Empty;
        
        /// <summary>
        /// ??????
        /// </summary>
        [StringLength(500, ErrorMessage = "???????????? 500 ??")]
        public string? SampleEnglishName { get; set; }
        
        /// <summary>
        /// ??
        /// </summary>
        [StringLength(500, ErrorMessage = "???????? 500 ??")]
        public string? CommonName { get; set; }
        
        /// <summary>
        /// ?????
        /// </summary>
        public string? ContentDescription { get; set; }
        
        /// <summary>
        /// ????
        /// </summary>
        [StringLength(200, ErrorMessage = "?????????? 200 ??")]
        public string? FoodCategory { get; set; }
    }

    /// <summary>
    /// ?????? DTO
    /// </summary>
    public class UpdateFoodDto
    {
        /// <summary>
        /// ????
        /// </summary>
        [Required(ErrorMessage = "?????????")]
        [StringLength(100, ErrorMessage = "?????????? 100 ??")]
        public string IntegratedNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// ????
        /// </summary>
        [Required(ErrorMessage = "?????????")]
        [StringLength(500, ErrorMessage = "?????????? 500 ??")]
        public string SampleName { get; set; } = string.Empty;
        
        /// <summary>
        /// ??????
        /// </summary>
        [StringLength(500, ErrorMessage = "???????????? 500 ??")]
        public string? SampleEnglishName { get; set; }
        
        /// <summary>
        /// ??
        /// </summary>
        [StringLength(500, ErrorMessage = "???????? 500 ??")]
        public string? CommonName { get; set; }
        
        /// <summary>
        /// ?????
        /// </summary>
        public string? ContentDescription { get; set; }
        
        /// <summary>
        /// ????
        /// </summary>
        [StringLength(200, ErrorMessage = "?????????? 200 ??")]
        public string? FoodCategory { get; set; }
    }

    /// <summary>
    /// API ?????
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int? Count { get; set; }

        public static ApiResponse<T> SuccessResult(T data, string message = "????")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> SuccessResult(T data, int count, string message = "????")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Count = count
            };
        }

        public static ApiResponse<T> ErrorResult(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message
            };
        }
    }
}