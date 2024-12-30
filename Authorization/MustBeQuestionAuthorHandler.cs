using backend.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace backend.Authorization
{
    public class MustBeQuestionAuthorHandler: AuthorizationHandler<MustBeQuestionAuthorRequirement>
    {
        private readonly IDataRepository _dataRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MustBeQuestionAuthorHandler(IDataRepository dataRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dataRepository = dataRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async override Task
            HandleRequirementAsync(AuthorizationHandlerContext context, MustBeQuestionAuthorRequirement requirement)
        {
            if (!(context.User.Identity?.IsAuthenticated ?? false))
            {
                context.Fail();
                return;
            }

            if (_httpContextAccessor.HttpContext == null)
            {
                context.Fail();
                return;
            }
            var questionId = _httpContextAccessor.HttpContext.Request.RouteValues["questionId"];
            if (questionId == null || !int.TryParse(questionId.ToString(), out int questionIdAsInt))
            {
                context.Fail();
                return;
            }

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                context.Fail();
                return;
            }

            var question = await _dataRepository.GetQuestion(questionIdAsInt);
            if (question == null)
            {
                // let it through so that the controller can return a 404
                context.Succeed(requirement);
                return;
            }

            if (question.UserId != userId)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }
    }
}
