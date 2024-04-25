using EST.MIT.Web.Helpers;

namespace EST.MIT.Web.Test.Helpers;

public class ErrorMessageHelperTests
{
    [Fact]
    public void GetErrorMessagesForField_Returns_Null_For_Invalid_Key()
    {
        //Arrange
        var errors = new Dictionary<string, List<string>>
        {
            { "testKey1", new List<string>() { "TestErrorMessage1" } },
            { "testKey2", new List<string>() { "TestErrorMessage2" } }
        };

        var fieldKey = "testKey3";

        //Act
        var result = ErrorMessageHelper.ErrorMessagesForField(errors, fieldKey);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetErrorMessagesForField_Returns_String_For_Valid_Key()
    {
        //Arrange
        var errors = new Dictionary<string, List<string>>
        {
            { "testKey1", new List<string>() { "TestErrorMessage1" } },
            { "testKey2", new List<string>() { "TestErrorMessage2" } }
        };

        var fieldKey = "testKey1";

        //Act
        var result = ErrorMessageHelper.ErrorMessagesForField(errors, fieldKey);

        //Assert
        result.Should().Be("TestErrorMessage1");
    }
}
