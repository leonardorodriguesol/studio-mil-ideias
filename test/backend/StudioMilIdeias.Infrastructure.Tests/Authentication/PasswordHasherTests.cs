using StudioMilIdeias.Infrastructure.Authentication;

namespace StudioMilIdeias.Infrastructure.Tests;

public sealed class PasswordHasherTests
{
    [Fact]
    public void Hash_ShouldGenerateValueThatCanBeVerified()
    {
        var hasher = new PasswordHasher();
        const string password = "MySecurePassword#2026";

        var hash = hasher.Hash(password);

        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.NotEqual(password, hash);
        Assert.True(hasher.Verify(password, hash));
    }
}
