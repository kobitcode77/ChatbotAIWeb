using ChatbotAI_BE.Dtos;
using ChatbotAI_BE.Dtos.Activity;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotAI_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class ActivityController : BaseController
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        /// Lấy toàn bộ hoạt động (ADMIN)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllActivities(CancellationToken cancellationToken)
        {
            var activities = await _activityService.GetAllActivitiesAsync(cancellationToken);
            return Success(activities);
        }

        /// Lấy hoạt động của người dùng hiện tại
        [HttpGet("my")]
        public async Task<IActionResult> GetMyActivities(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var activities = await _activityService.GetActivitiesByUserIdAsync(userId, cancellationToken);
            return Success(activities);
        }

        /// Lấy hoạt động theo UserId (ADMIN)
        [HttpGet("user/{userId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActivitiesByUser(Guid userId, CancellationToken cancellationToken)
        {
            var activities = await _activityService.GetActivitiesByUserIdAsync(userId, cancellationToken);
            return Success(activities);
        }

        /// Lấy hoạt động theo ModelId
        [HttpGet("model/{modelId:guid}")]
        public async Task<IActionResult> GetActivitiesByModel(Guid modelId, CancellationToken cancellationToken)
        {
            var activities = await _activityService.GetActivitiesByModelIdAsync(modelId, cancellationToken);
            return Success(activities);
        }

        /// Lấy hoạt động theo User và Model
        [HttpGet("user/{userId:guid}/model/{modelId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActivitiesByUserAndModel(Guid userId, Guid modelId, CancellationToken cancellationToken)
        {
            var activities = await _activityService.GetActivitiesByUserAndModelAsync(userId, modelId, cancellationToken);
            return Success(activities);
        }

        /// Lấy chi tiết một hoạt động
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetActivityById(Guid id, CancellationToken cancellationToken)
        {
            var activity = await _activityService.GetActivityByIdAsync(id, cancellationToken);
            if (activity == null)
                return Fail("Không tìm thấy hoạt động.", 404);

            return Success(activity);
        }

        /// Ghi nhận một hoạt động mới
        [HttpPost]
        public async Task<IActionResult> AddActivity([FromBody] AddActivityRequest request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            var activity = await _activityService.AddActivityAsync(
                userId,
                request.ModelId,
                request.InputTokens,
                request.OutputTokens,
                cancellationToken
            );

            return Success(activity, "Ghi nhận hoạt động thành công", 201);
        }

        /// Xóa một hoạt động (ADMIN)
        [HttpDelete("{activityId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteActivity(Guid activityId, CancellationToken cancellationToken)
        {
            var result = await _activityService.DeleteActivityAsync(activityId, cancellationToken);
            if (!result)
                return Fail("Không tìm thấy hoạt động để xóa.", 404);

            return Success<object>(null, "Xóa hoạt động thành công");
        }

        /// Thống kê token theo ngày của người dùng hiện tại
        [HttpGet("my/daily-usage")]
        public async Task<IActionResult> GetMyDailyTokenUsage([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var stats = await _activityService.GetDailyTokenUsageByUserAsync(userId, startDate, endDate, cancellationToken);
            return Success(stats);
        }

        /// Thống kê token theo tháng của tất cả người dùng (ADMIN)
        [HttpGet("admin/monthly-usage/{year:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMonthlyTokenUsageAllUsers(int year, CancellationToken cancellationToken)
        {
            var stats = await _activityService.GetMonthlyTokenUsageAllUsersAsync(year, cancellationToken);
            return Success(stats);
        }
    }
}
