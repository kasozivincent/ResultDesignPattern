using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;

namespace ResultDesignPattern;

public class User
{
    public int Id { get; set; }
    public bool HasPublicProfile { get; set; }
}

public sealed class FollowerService
{
    private readonly IFollowerRepository _followerRepository;

    public FollowerService(IFollowerRepository followerRepository)
        => _followerRepository = followerRepository;
    
    public async Task<Either<Error, Unit>> StartFollowingAsync(User user, User followed, DateTime utcNow,
        CancellationToken cancellationToken = default)
       =>  await Task.FromResult(ValidateUser(user, followed))
            .Bind(_ => CheckIfAlreadyFollowing(user, followed, cancellationToken))
            .BindAsync(_ => CreateAndInsertFollower(user.Id, followed.Id, utcNow));
    
    private async Task<Either<Error, Unit>> CreateAndInsertFollower(int userId, int followedId, DateTime utcNow)
    {
        var follower = Follower.Create(userId, followedId, utcNow);
        return await _followerRepository.Insert(follower);
    }
    private async Task<Either<Error, Unit>> CheckIfAlreadyFollowing(User user, User followed, CancellationToken cancellationToken)
    {
        var isAlreadyFollowing = await _followerRepository.IsAlreadyFollowingAsync(user.Id, followed.Id, cancellationToken);
        return isAlreadyFollowing ? FollowerErrors.AlreadyFollowing() : Unit.Default;
    }
    private static Either<Error, Unit> ValidateUser(User user, User followed)
    {
        return user.Id == followed.Id
            ? FollowerErrors.SameUser()
            : !followed.HasPublicProfile
                ? FollowerErrors.NonPublicProfile()
                : Unit.Default;
    }
}

public class Follower
{
}