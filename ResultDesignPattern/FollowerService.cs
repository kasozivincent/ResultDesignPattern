using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

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
    
    public async Task<Result<Unit>> StartFollowingAsync(User user, User followed, DateTime utcNow,
        CancellationToken cancellationToken = default)
       =>  await Task.FromResult(ValidateUser(user, followed))
            .Bind(_ => CheckIfAlreadyFollowing(user, followed, cancellationToken))
            .Bind(_ => CreateAndInsertFollower(user.Id, followed.Id, utcNow));
    
    private async Task<Result<Unit>> CreateAndInsertFollower(int userId, int followedId, DateTime utcNow)
        => await _followerRepository.Insert(new Follower(userId, followedId, utcNow));
    
    private async Task<Result<Unit>> CheckIfAlreadyFollowing(User user, User followed, CancellationToken cancellationToken)
        => await _followerRepository.IsAlreadyFollowingAsync(user.Id, followed.Id, cancellationToken)
          ? FollowerErrors.AlreadyFollowing() 
          : Unit.Default;
    
    private static Result<Unit> ValidateUser(User user, User followed)
         => user.Id == followed.Id
            ? FollowerErrors.SameUser()
            : !followed.HasPublicProfile
                ? FollowerErrors.NonPublicProfile()
                : Unit.Default;
}

public class Follower
{
    public Follower(int userId, int followerId, DateTime utcNow)
    {
    }
}