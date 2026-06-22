namespace TravelsBE.Tests.Utils;

public class PasswordHelperTests
{
    [Fact]
    public void CreateSalt_ReturnsDifferentBase64Values()
    {
        var firstSalt = PasswordHelper.CreateSalt();
        var secondSalt = PasswordHelper.CreateSalt();

        Assert.NotEqual(firstSalt, secondSalt);
        Assert.Equal(16, Convert.FromBase64String(firstSalt).Length);
        Assert.Equal(16, Convert.FromBase64String(secondSalt).Length);
    }

    [Fact]
    public void HashPassword_ReturnsSameHash_ForSamePasswordAndSalt()
    {
        var salt = PasswordHelper.CreateSalt();

        var firstHash = PasswordHelper.HashPassword("Parola123!", salt);
        var secondHash = PasswordHelper.HashPassword("Parola123!", salt);

        Assert.Equal(firstHash, secondHash);
    }

    [Fact]
    public void VerifyPassword_ReturnsTrueOnlyForMatchingPassword()
    {
        var salt = PasswordHelper.CreateSalt();
        var hash = PasswordHelper.HashPassword("Parola123!", salt);

        Assert.True(PasswordHelper.VerifyPassword("Parola123!", hash, salt));
        Assert.False(PasswordHelper.VerifyPassword("ParolaGresita123!", hash, salt));
    }
}
