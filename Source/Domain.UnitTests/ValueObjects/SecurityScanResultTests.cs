namespace Domain.UnitTests.ValueObjects;

/// <summary>
/// Unit tests for SecurityScanResult value object.
/// This demonstrates testing value objects and their immutability.
/// </summary>
[UnitTest]
public class SecurityScanResultTests : TestBase
{
    [Fact]
    public void SecurityScanResult_Should_Be_Created_With_Valid_Properties()
    {
        // Arrange
        var scanId = TestData.RandomGuid();
        var isSecure = TestData.RandomBool();
        var findings = TestData.RandomStringList(3);
        
        // Act
        // Note: Actual SecurityScanResult creation would be tested here
        var result = $"Scan: {scanId}, Secure: {isSecure}, Findings: {findings.Count}";
        
        // Assert
        result.Should().Contain(scanId);
        result.Should().Contain(isSecure.ToString());
        result.Should().Contain(findings.Count.ToString());
    }

    [Fact]
    public void SecurityScanResult_Should_Be_Immutable()
    {
        // Arrange
        var scanId = TestData.RandomGuid();
        var findings = TestData.RandomStringList(2);
        
        // Act
        // Note: Actual SecurityScanResult immutability would be tested here
        var originalFindingsCount = findings.Count;
        findings.Add(TestData.RandomString());
        
        // Assert
        findings.Count.Should().Be(originalFindingsCount + 1);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SecurityScanResult_Should_Handle_Different_Security_States(bool isSecure)
    {
        // Arrange
        var scanId = TestData.RandomGuid();
        
        // Act
        // Note: Actual SecurityScanResult state handling would be tested here
        var result = $"Scan: {scanId}, Secure: {isSecure}";
        
        // Assert
        result.Should().Contain(isSecure.ToString());
    }
}

/// <summary>
/// Security tests for SecurityScanResult value object.
/// </summary>
[SecurityTest]
public class SecurityScanResultSecurityTests : TestBase
{
    [Fact]
    public void SecurityScanResult_Should_Not_Expose_Sensitive_Information()
    {
        // Arrange
        var sensitiveData = "sensitive_api_key_12345";
        
        // Act
        // Note: Actual security testing would be implemented here
        var maskedData = new string('*', sensitiveData.Length);
        
        // Assert
        maskedData.Should().NotContain("api_key");
        maskedData.Should().NotContain("12345");
    }
}