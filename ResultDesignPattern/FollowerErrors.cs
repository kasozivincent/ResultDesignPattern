namespace ResultDesignPattern;

public class Error
{
    public string Message { get; set; }
}
public static class FollowerErrors
{
    public static Error SameUser() => new Error { Message = "User can't follow themselves" };
    public static Error NonPublicProfile() => new Error { Message = "User has no public profile" };
    public static Error AlreadyFollowing() => new Error { Message = "User already a follower" };
    
    

}