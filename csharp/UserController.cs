using HelloBear.Api.Attributes;
using HelloBear.Application.Common.Models.Paging;
using HelloBear.Application.Settings;
using HelloBear.Application.Users.Commands.CreateUser;
using HelloBear.Application.Users.Commands.DeleteUser;
using HelloBear.Application.Users.Commands.SendInvitation;
using HelloBear.Application.Users.Commands.UpdateUser;
using HelloBear.Application.Users.Queries.GetAllTeachers;
using HelloBear.Application.Users.Queries.GetRolesWithPagination;
using HelloBear.Application.Users.Queries.GetUsersWithPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloBear.Api.Controllers;

[Authorize]
public class UserController : ApiControllerBase
{
    [HttpGet]
    [ValidateUserRole(AppConstants.AdminRole, AppConstants.TeacherRole, AppConstants.SchoolOwner)]
    public Task<ActionResult<PaginatedList<UserListResponse>>> GetUsersWithPagination([FromQuery] GetUsersWithPaginationQuery query)
        => HandleRequest(query);

    [HttpGet("{Id}")]
    [ValidateUserRole(AppConstants.AdminRole, AppConstants.SchoolOwner)]
    public Task<ActionResult<UserDetailResponse?>> GetUserDetail([FromRoute] GetUserDetailQuery query)
        => HandleRequest(query);

    [HttpGet("roles")]
    [ValidateUserRole(AppConstants.AdminRole, AppConstants.SchoolOwner)]
    public Task<ActionResult<PaginatedList<RoleResponse>>> GetRoles([FromQuery] GetRolesWithPaginationQuery query)
        => HandleRequest(query);

    [HttpPost]
    [ValidateUserRole(AppConstants.AdminRole, AppConstants.SchoolOwner)]
    public Task<ActionResult<string>> Create(CreateUserCommand createUserCommand)
        => HandleRequest(createUserCommand);

    [HttpPut("{id}")]
    [ValidateUserRole(AppConstants.AdminRole, AppConstants.SchoolOwner)]
    public Task<IActionResult> Update(string id, UpdateUserBodyRequest bodyRequest)
        => HandleRequest(new UpdateUserCommand(id, bodyRequest));

    [HttpPut("{id}/resend-invitation")]
    [ValidateUserRole(AppConstants.AdminRole, AppConstants.SchoolOwner)]
    public Task<IActionResult> SendInvitation(string id)
        => HandleRequest(new SendInvitationCommand(id));

    [HttpDelete("{id}")]
    [ValidateUserRole(AppConstants.AdminRole, AppConstants.SchoolOwner)]
    public Task<IActionResult> Delete(string id)
        => HandleRequest(new DeleteUserCommand(id));

    [HttpGet("all-teachers")]
    [ValidateUserRole(AppConstants.AdminRole, AppConstants.SchoolOwner)]
    public Task<ActionResult<List<GetAllTeachersResponse>>> GetAllTeachers([FromQuery] GetAllTeachersQuery query)
        => HandleRequest(query);
}