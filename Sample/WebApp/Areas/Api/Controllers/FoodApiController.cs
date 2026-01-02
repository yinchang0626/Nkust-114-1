using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models.DTOs;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Areas.Api.Controllers
{
    /// <summary>
    /// 食品 API 控制器 - 提供食品的 CRUD 操作
    /// </summary>
    [Area("Api")]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FoodApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FoodApiController> _logger;

        public FoodApiController(ApplicationDbContext context, ILogger<FoodApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 取得食品列表,支援分頁和條件篩選
        /// </summary>
        /// <param name="page">頁碼,從1開始</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <param name="integratedNumber">整合編號</param>
        /// <param name="sampleName">樣品名稱</param>
        /// <param name="foodCategory">食品分類</param>
        /// <returns>食品列表</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FoodDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FoodDto>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<FoodDto>>>> GetFoods(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? integratedNumber = null,
            [FromQuery] string? sampleName = null,
            [FromQuery] string? foodCategory = null)
        {
            try
            {
                page = Math.Max(1, page);
                pageSize = Math.Clamp(pageSize, 1, 100);

                var query = _context.FoodEntities.AsQueryable();

                // 套用篩選
                if (!string.IsNullOrEmpty(integratedNumber))
                {
                    query = query.Where(x => x.IntegratedNumber.Contains(integratedNumber));
                }

                if (!string.IsNullOrEmpty(sampleName))
                {
                    query = query.Where(x => x.SampleName.Contains(sampleName));
                }

                if (!string.IsNullOrEmpty(foodCategory))
                {
                    query = query.Where(x => x.FoodCategory != null && x.FoodCategory.Contains(foodCategory));
                }

                // 取得總數
                var totalCount = await query.CountAsync();

                // 分頁查詢
                var foods = await query
                    .OrderBy(x => x.IntegratedNumber)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new FoodDto
                    {
                        Id = x.Id,
                        IntegratedNumber = x.IntegratedNumber,
                        SampleName = x.SampleName,
                        SampleEnglishName = x.SampleEnglishName,
                        CommonName = x.CommonName,
                        ContentDescription = x.ContentDescription,
                        FoodCategory = x.FoodCategory,
                        CreatedAt = x.CreatedAt
                    })
                    .ToListAsync();

                return Ok(ApiResponse<IEnumerable<FoodDto>>.SuccessResult(foods, totalCount, 
                    $"成功取得第 {page} 頁資料,共 {totalCount} 筆"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得食品列表時發生錯誤");
                return StatusCode(500, ApiResponse<IEnumerable<FoodDto>>.ErrorResult("取得食品列表失敗"));
            }
        }

        /// <summary>
        /// 根據 ID 取得單一食品
        /// </summary>
        /// <param name="id">食品 ID</param>
        /// <returns>食品資料</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<FoodDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<FoodDto>), 404)]
        [ProducesResponseType(typeof(ApiResponse<FoodDto>), 500)]
        public async Task<ActionResult<ApiResponse<FoodDto>>> GetFood(int id)
        {
            try
            {
                var food = await _context.FoodEntities
                    .Where(x => x.Id == id)
                    .Select(x => new FoodDto
                    {
                        Id = x.Id,
                        IntegratedNumber = x.IntegratedNumber,
                        SampleName = x.SampleName,
                        SampleEnglishName = x.SampleEnglishName,
                        CommonName = x.CommonName,
                        ContentDescription = x.ContentDescription,
                        FoodCategory = x.FoodCategory,
                        CreatedAt = x.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (food == null)
                {
                    return NotFound(ApiResponse<FoodDto>.ErrorResult($"找不到 ID 為 {id} 的食品"));
                }

                return Ok(ApiResponse<FoodDto>.SuccessResult(food, "查詢成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "根據ID取得食品時發生錯誤,ID: {Id}", id);
                return StatusCode(500, ApiResponse<FoodDto>.ErrorResult("取得食品資料失敗"));
            }
        }

        /// <summary>
        /// 新增食品資料
        /// </summary>
        /// <param name="createDto">新增食品資料</param>
        /// <returns>新增後的食品資料</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<FoodDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<FoodDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<FoodDto>), 500)]
        public async Task<ActionResult<ApiResponse<FoodDto>>> CreateFood([FromBody] CreateFoodDto createDto)
        {
            try
            {
                // 驗證模型
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(ApiResponse<FoodDto>.ErrorResult($"驗證失敗: {errors}"));
                }

                // 檢查整合編號是否已存在
                var existingFood = await _context.FoodEntities
                    .FirstOrDefaultAsync(x => x.IntegratedNumber == createDto.IntegratedNumber);

                if (existingFood != null)
                {
                    return BadRequest(ApiResponse<FoodDto>.ErrorResult($"整合編號 '{createDto.IntegratedNumber}' 已存在"));
                }

                // 建立新食品
                var newFood = new FoodEntity
                {
                    IntegratedNumber = createDto.IntegratedNumber.Trim(),
                    SampleName = createDto.SampleName.Trim(),
                    SampleEnglishName = createDto.SampleEnglishName?.Trim(),
                    CommonName = createDto.CommonName?.Trim(),
                    ContentDescription = createDto.ContentDescription?.Trim(),
                    FoodCategory = createDto.FoodCategory?.Trim(),
                    CreatedAt = DateTime.Now
                };

                _context.FoodEntities.Add(newFood);
                await _context.SaveChangesAsync();

                var foodDto = new FoodDto
                {
                    Id = newFood.Id,
                    IntegratedNumber = newFood.IntegratedNumber,
                    SampleName = newFood.SampleName,
                    SampleEnglishName = newFood.SampleEnglishName,
                    CommonName = newFood.CommonName,
                    ContentDescription = newFood.ContentDescription,
                    FoodCategory = newFood.FoodCategory,
                    CreatedAt = newFood.CreatedAt
                };

                return CreatedAtAction(nameof(GetFood), new { id = newFood.Id }, 
                    ApiResponse<FoodDto>.SuccessResult(foodDto, "新增成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增食品時發生錯誤");
                return StatusCode(500, ApiResponse<FoodDto>.ErrorResult("新增食品失敗"));
            }
        }

        /// <summary>
        /// 更新食品資料
        /// </summary>
        /// <param name="id">食品 ID</param>
        /// <param name="updateDto">更新食品資料</param>
        /// <returns>更新後食品資料</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<FoodDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<FoodDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<FoodDto>), 404)]
        [ProducesResponseType(typeof(ApiResponse<FoodDto>), 500)]
        public async Task<ActionResult<ApiResponse<FoodDto>>> UpdateFood(int id, [FromBody] UpdateFoodDto updateDto)
        {
            try
            {
                // 驗證模型
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(ApiResponse<FoodDto>.ErrorResult($"驗證失敗: {errors}"));
                }

                var food = await _context.FoodEntities.FindAsync(id);
                if (food == null)
                {
                    return NotFound(ApiResponse<FoodDto>.ErrorResult($"找不到 ID 為 {id} 的食品"));
                }

                // 檢查整合編號是否已被其他食品使用
                var existingFood = await _context.FoodEntities
                    .FirstOrDefaultAsync(x => x.IntegratedNumber == updateDto.IntegratedNumber && x.Id != id);

                if (existingFood != null)
                {
                    return BadRequest(ApiResponse<FoodDto>.ErrorResult($"整合編號 '{updateDto.IntegratedNumber}' 已被其他食品使用"));
                }

                // 更新食品
                food.IntegratedNumber = updateDto.IntegratedNumber.Trim();
                food.SampleName = updateDto.SampleName.Trim();
                food.SampleEnglishName = updateDto.SampleEnglishName?.Trim();
                food.CommonName = updateDto.CommonName?.Trim();
                food.ContentDescription = updateDto.ContentDescription?.Trim();
                food.FoodCategory = updateDto.FoodCategory?.Trim();

                await _context.SaveChangesAsync();

                var foodDto = new FoodDto
                {
                    Id = food.Id,
                    IntegratedNumber = food.IntegratedNumber,
                    SampleName = food.SampleName,
                    SampleEnglishName = food.SampleEnglishName,
                    CommonName = food.CommonName,
                    ContentDescription = food.ContentDescription,
                    FoodCategory = food.FoodCategory,
                    CreatedAt = food.CreatedAt
                };

                return Ok(ApiResponse<FoodDto>.SuccessResult(foodDto, "更新成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新食品時發生錯誤,ID: {Id}", id);
                return StatusCode(500, ApiResponse<FoodDto>.ErrorResult("更新食品失敗"));
            }
        }

        /// <summary>
        /// 刪除食品資料
        /// </summary>
        /// <param name="id">食品 ID</param>
        /// <returns>刪除結果</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteFood(int id)
        {
            try
            {
                var food = await _context.FoodEntities.FindAsync(id);
                if (food == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult($"找不到 ID 為 {id} 的食品"));
                }

                // 檢查是否有關聯資料
                var hasRelatedItems = await _context.FoodAnalysisItemEntities
                    .AnyAsync(x => x.FoodId == id);

                if (hasRelatedItems)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("該食品有關聯的分析項目,無法刪除"));
                }

                _context.FoodEntities.Remove(food);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.SuccessResult(null!, "刪除成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除食品時發生錯誤,ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("刪除食品失敗"));
            }
        }

        /// <summary>
        /// 檢查整合編號是否已存在
        /// </summary>
        /// <param name="integratedNumber">整合編號</param>
        /// <param name="excludeId">排除的食品 ID,用於編輯時檢查</param>
        /// <returns>檢查結果</returns>
        [HttpGet("check-integrated-number")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> CheckIntegratedNumber(
            [FromQuery, Required] string integratedNumber,
            [FromQuery] int? excludeId = null)
        {
            try
            {
                var query = _context.FoodEntities.Where(x => x.IntegratedNumber == integratedNumber);
                
                if (excludeId.HasValue)
                {
                    query = query.Where(x => x.Id != excludeId.Value);
                }

                var exists = await query.AnyAsync();

                return Ok(ApiResponse<bool>.SuccessResult(!exists, 
                    exists ? "編號已存在" : "編號可使用"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查整合編號時發生錯誤");
                return StatusCode(500, ApiResponse<bool>.ErrorResult("檢查編號失敗"));
            }
        }

        /// <summary>
        /// 批次匯入食品資料
        /// </summary>
        /// <param name="foods">食品列表</param>
        /// <returns>匯入結果</returns>
        [HttpPost("batch-import")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> BatchImport([FromBody] IEnumerable<CreateFoodDto> foods)
        {
            try
            {
                var foodList = foods.ToList();
                if (!foodList.Any())
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("匯入清單不可為空"));
                }

                var importedCount = 0;
                var skippedCount = 0;
                var errorList = new List<string>();

                foreach (var foodDto in foodList)
                {
                    try
                    {
                        // 檢查整合編號是否已存在
                        var exists = await _context.FoodEntities
                            .AnyAsync(x => x.IntegratedNumber == foodDto.IntegratedNumber);

                        if (exists)
                        {
                            skippedCount++;
                            continue;
                        }

                        var newFood = new FoodEntity
                        {
                            IntegratedNumber = foodDto.IntegratedNumber.Trim(),
                            SampleName = foodDto.SampleName.Trim(),
                            SampleEnglishName = foodDto.SampleEnglishName?.Trim(),
                            CommonName = foodDto.CommonName?.Trim(),
                            ContentDescription = foodDto.ContentDescription?.Trim(),
                            FoodCategory = foodDto.FoodCategory?.Trim(),
                            CreatedAt = DateTime.Now
                        };

                        _context.FoodEntities.Add(newFood);
                        importedCount++;
                    }
                    catch (Exception ex)
                    {
                        errorList.Add($"編號 {foodDto.IntegratedNumber}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync();

                var message = $"匯入完成:成功 {importedCount} 筆,跳過 {skippedCount} 筆";
                if (errorList.Any())
                {
                    message += $",失敗 {errorList.Count} 筆";
                }

                return Ok(ApiResponse<object>.SuccessResult(new { 
                    ImportedCount = importedCount, 
                    SkippedCount = skippedCount,
                    ErrorCount = errorList.Count,
                    Errors = errorList
                }, message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批次匯入食品時發生錯誤");
                return StatusCode(500, ApiResponse<object>.ErrorResult("匯入食品失敗"));
            }
        }
    }
}
