using TravelSBE.Utils;

namespace TravelsBE.Tests.Utils;

public class ServiceResultTests
{
    [Fact]
    public void DataConstructor_CreatesSuccessfulResult()
    {
        var result = new ServiceResult<int>(42);

        Assert.True(result.IsSuccessful);
        Assert.Equal(42, result.Result);
        Assert.Null(result.ValidationMessage);
    }

    [Fact]
    public void ValidationConstructor_CreatesFailedResult()
    {
        var result = new ServiceResult<int>("Eroare validare");

        Assert.False(result.IsSuccessful);
        Assert.Equal(default, result.Result);
        Assert.Equal("Eroare validare", result.ValidationMessage);
    }
}
