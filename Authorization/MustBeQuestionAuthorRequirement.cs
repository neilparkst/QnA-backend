using Microsoft.AspNetCore.Authorization;

namespace backend.Authorization
{
    public class MustBeQuestionAuthorRequirement: IAuthorizationRequirement
    {
        public MustBeQuestionAuthorRequirement()
        {

        }
    }
}
