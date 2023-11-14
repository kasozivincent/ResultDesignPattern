using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace ResultDesignPattern;

public interface IFollowerRepository
{
    Task<bool> IsAlreadyFollowingAsync(int userId, int followerId, CancellationToken token = default);
    Task<Unit> Insert(Follower user);
}