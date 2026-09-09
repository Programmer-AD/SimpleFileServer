using SimpleFileServer.Web.Utils;

namespace SimpleFileServer.UnitTests.Web.Utils;

public class SplitCookieHandlerTests
{
    private const string NamePrefix = "test_cookie";

    #region ToSplitCookies

    [Fact]
    public void ToSplitCookies_WhenValueIsEmpty_ReturnsOnlyLengthCookie()
    {
        string value = string.Empty;
        var expectedResult = new Dictionary<string, string>
        {
            ["test_cookie_length"] = "0",
        };

        Dictionary<string, string> result = SplitCookieHandler.ToSplitCookies(NamePrefix, value);

        Assert.Equivalent(expectedResult, result, strict: true);
    }

    [Fact]
    public void ToSplitCookies_WhenValueFitsInOneCookie_ReturnsOnePart()
    {
        const string value = "shortTestValue";
        var expectedResult = new Dictionary<string, string>
        {
            ["test_cookie_length"] = "1",
            ["test_cookie_0"] = "c2hvcnRUZXN0VmFsdWU=",
        };

        Dictionary<string, string> result = SplitCookieHandler.ToSplitCookies(NamePrefix, value);

        Assert.Equivalent(expectedResult, result, strict: true);
    }

    [Fact]
    public void ToSplitCookies_WhenValueDoesNotFitInOneCookie_ReturnsMultipleParts()
    {
        (string value, string fullChunkValue, string lastChunkValue) = GetLongTestValue();

        var expectedResult = new Dictionary<string, string>
        {
            ["test_cookie_length"] = "3",
            ["test_cookie_0"] = fullChunkValue,
            ["test_cookie_1"] = fullChunkValue,
            ["test_cookie_2"] = lastChunkValue,
        };

        Dictionary<string, string> result = SplitCookieHandler.ToSplitCookies(NamePrefix, value);

        Assert.Equivalent(expectedResult, result, strict: true);
    }

    #endregion

    #region TryGetSplitCookieValue

    [Fact]
    public void TryGetSplitCookieValue_WhenLengthCookieIsMissing_ReturnsFalse()
    {
        var requestCookies = new Dictionary<string, string>();

        bool result = SplitCookieHandler.TryGetSplitCookieValue(requestCookies, NamePrefix, out string? _);

        Assert.False(result);
    }

    [Fact]
    public void TryGetSplitCookieValue_WhenLengthCookieIsNotNumber_ReturnsFalse()
    {
        var requestCookies = new Dictionary<string, string>()
        {
            ["test_cookie_length"] = "notNumber"
        };

        bool result = SplitCookieHandler.TryGetSplitCookieValue(requestCookies, NamePrefix, out string? _);

        Assert.False(result);
    }

    [Fact]
    public void TryGetSplitCookieValue_WhenLengthIsNegative_ReturnsFalse()
    {
        var requestCookies = new Dictionary<string, string>()
        {
            ["test_cookie_length"] = "-1"
        };

        bool result = SplitCookieHandler.TryGetSplitCookieValue(requestCookies, NamePrefix, out string? _);

        Assert.False(result);
    }

    [Fact]
    public void TryGetSplitCookieValue_WhenLengthIsZero_ReturnsEmptyValue()
    {
        var requestCookies = new Dictionary<string, string>()
        {
            ["test_cookie_length"] = "0"
        };

        bool result = SplitCookieHandler.TryGetSplitCookieValue(requestCookies, NamePrefix, out string? resultValue);

        Assert.True(result);
        Assert.Empty(resultValue!);
    }


    [Fact]
    public void TryGetSplitCookieValue_WhenThereIsOnePart_ReturnsCorrectValue()
    {
        const string expectedResult = "shortTestValue";
        var requestCookies = new Dictionary<string, string>()
        {
            ["test_cookie_length"] = "1",
            ["test_cookie_0"] = "c2hvcnRUZXN0VmFsdWU=",
        };

        bool result = SplitCookieHandler.TryGetSplitCookieValue(requestCookies, NamePrefix, out string? resultValue);

        Assert.True(result);
        Assert.Equal(expectedResult, resultValue!);
    }

    [Fact]
    public void TryGetSplitCookieValue_WhenThereAreFewParts_ReturnsCorrectValue()
    {
        (string expectedResult, string fullChunkValue, string lastChunkValue) = GetLongTestValue();

        var requestCookies = new Dictionary<string, string>()
        {
            ["test_cookie_length"] = "3",
            ["test_cookie_0"] = fullChunkValue,
            ["test_cookie_1"] = fullChunkValue,
            ["test_cookie_2"] = lastChunkValue,
        };

        bool result = SplitCookieHandler.TryGetSplitCookieValue(requestCookies, NamePrefix, out string? resultValue);

        Assert.True(result);
        Assert.Equal(expectedResult, resultValue!);
    }

    [Fact]
    public void TryGetSplitCookieValue_WhenThereAreOtherCookies_ReturnsCorrectValue()
    {
        (string expectedResult, string fullChunkValue, string lastChunkValue) = GetLongTestValue();

        var requestCookies = new Dictionary<string, string>()
        {
            ["garbage_cookie"] = "notUsed",
            ["test_cookie_length"] = "3",
            ["test_cookie_0"] = fullChunkValue,
            ["test_cookie_1"] = fullChunkValue,
            ["test_cookie_2"] = lastChunkValue,
            ["test_cookie"] = "notUsed",
            ["test_cookie_3"] = "notUsed",
        };

        bool result = SplitCookieHandler.TryGetSplitCookieValue(requestCookies, NamePrefix, out string? resultValue);

        Assert.True(result);
        Assert.Equal(expectedResult, resultValue!);
    }

    [Fact]
    public void TryGetSplitCookieValue_WhenSomePartIsMissing_ReturnsFalse()
    {
        (string _, string fullChunkValue, string lastChunkValue) = GetLongTestValue();

        var requestCookies = new Dictionary<string, string>()
        {
            ["test_cookie_length"] = "3",
            ["test_cookie_0"] = fullChunkValue,
            ["test_cookie_2"] = lastChunkValue,
        };

        bool result = SplitCookieHandler.TryGetSplitCookieValue(requestCookies, NamePrefix, out string? _);

        Assert.False(result);
    }

    #endregion

    [Theory]
    [InlineData("")]
    [InlineData("mySpecialTestValue")]
    [InlineData("myVeryVerySpecialTestValue")]
    public void CrossTest_EncodeAndThenDecode_ShouldReturnOriginalValue(string originalValue)
    {
        Dictionary<string, string> splitCookies = SplitCookieHandler.ToSplitCookies(NamePrefix, originalValue);
        bool wasConvertedBack = SplitCookieHandler.TryGetSplitCookieValue(splitCookies, NamePrefix, out string? resultValue);

        Assert.True(wasConvertedBack);
        Assert.Equal(originalValue, resultValue);
    }

    private static (string value, string fullChunkValue, string lastChunkValue) GetLongTestValue()
    {
        // Hardcoding few KB strings in tests is awful, so we do it smart way
        // Numbers are selected to be divisable without remaining
        const int testValueLength = 6000;

        const int encodedValueLength = testValueLength * 4 / 3;
        const int chunkCount = encodedValueLength / SplitCookieHandler.SafeCookieSizeLimit;
        const int lastChunkLength = encodedValueLength - chunkCount * SplitCookieHandler.SafeCookieSizeLimit;

        string value = new('a', testValueLength);

        // base64("aaa") = "YWFh"
        const string repeatableVase64Value = "YWFh";
        string fullChunkValue = string.Join(string.Empty, Enumerable.Repeat(repeatableVase64Value, SplitCookieHandler.SafeCookieSizeLimit / repeatableVase64Value.Length));

        return (value, fullChunkValue, fullChunkValue[..lastChunkLength]);
    }
}
