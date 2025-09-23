using Auth.API.Model;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using MyGrpc;

namespace Auth.API.GrpcServices;

public class GrpcUserInfoServer : UserService.UserServiceBase
{
    private readonly UserDbContext _userDbContext;

    public GrpcUserInfoServer(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public override async Task<UserReply> GetUserInfo(UserRequest request, ServerCallContext context)
    {
        var myUser = await _userDbContext.Users.FirstOrDefaultAsync( u => u.Email == request.UserEmail);

        return myUser == null ? new UserReply() { Found = false } : new UserReply() { Found = true };
    }
}