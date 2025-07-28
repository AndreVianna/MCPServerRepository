namespace MCPHub.Domain.ValueObjects;

/// <summary>
/// Unit tests for SecurityScanResult value object.
/// This demonstrates testing value objects and their immutability.
/// </summary>
public class SecurityScanResultTests {
    [Fact]
    public void SecurityScanResult_Should_Be_Created_With_Valid_Properties() {
        // Arrange
        var status = SecurityScanStatus.Passed;
        var vulnerabilities = new List<SecurityVulnerability>
        {
            new("VULN-001", "Test vulnerability", "Test description", SecurityScanSeverity.Low)
        };
        var scannerVersion = "Scanner v1.0";
        var scanLog = "Test scan log";

        // Act
        var result = new SecurityScanResult(status, vulnerabilities, scannerVersion, scanLog);

        // Assert
        result.Status.Should().Be(status);
        result.Vulnerabilities.Should().BeEquivalentTo(vulnerabilities);
        result.VulnerabilityCount.Should().Be(vulnerabilities.Count);
        result.HighestSeverity.Should().Be(SecurityScanSeverity.Low);
        result.ScannerVersion.Should().Be(scannerVersion);
        result.ScanLog.Should().Be(scanLog);
        result.ScannedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SecurityScanResult_Should_Calculate_Correct_VulnerabilityCount() {
        // Arrange
        var vulnerabilities = new List<SecurityVulnerability>
        {
            new("VULN-001", "Vuln 1", "Description 1", SecurityScanSeverity.Low),
            new("VULN-002", "Vuln 2", "Description 2", SecurityScanSeverity.Medium),
            new("VULN-003", "Vuln 3", "Description 3", SecurityScanSeverity.High)
        };

        // Act
        var result = new SecurityScanResult(SecurityScanStatus.Failed, vulnerabilities, "Scanner v1.0");

        // Assert
        result.VulnerabilityCount.Should().Be(3);
    }

    [Fact]
    public void SecurityScanResult_Should_Calculate_Correct_HighestSeverity() {
        // Arrange
        var vulnerabilities = new List<SecurityVulnerability>
        {
            new("VULN-001", "Vuln 1", "Description 1", SecurityScanSeverity.Low),
            new("VULN-002", "Vuln 2", "Description 2", SecurityScanSeverity.Critical),
            new("VULN-003", "Vuln 3", "Description 3", SecurityScanSeverity.Medium)
        };

        // Act
        var result = new SecurityScanResult(SecurityScanStatus.Failed, vulnerabilities, "Scanner v1.0");

        // Assert
        result.HighestSeverity.Should().Be(SecurityScanSeverity.Critical);
    }

    [Fact]
    public void SecurityScanResult_Should_Set_HighestSeverity_To_None_When_No_Vulnerabilities() {
        // Arrange
        var vulnerabilities = new List<SecurityVulnerability>();

        // Act
        var result = new SecurityScanResult(SecurityScanStatus.Passed, vulnerabilities, "Scanner v1.0");

        // Assert
        result.HighestSeverity.Should().Be(SecurityScanSeverity.None);
        result.VulnerabilityCount.Should().Be(0);
    }

    [Fact]
    public void SecurityScanResult_Should_Handle_Null_Vulnerabilities() {
        // Arrange & Act
        var result = new SecurityScanResult(SecurityScanStatus.Passed, null!, "Scanner v1.0");

        // Assert
        result.Vulnerabilities.Should().BeEmpty();
        result.VulnerabilityCount.Should().Be(0);
        result.HighestSeverity.Should().Be(SecurityScanSeverity.None);
    }

    [Fact]
    public void SecurityScanResult_Should_Throw_ArgumentNullException_When_ScannerVersion_Is_Null() {
        // Arrange
        var vulnerabilities = new List<SecurityVulnerability>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new SecurityScanResult(SecurityScanStatus.Passed, vulnerabilities, null!));
        exception.ParamName.Should().Be("scannerVersion");
    }

    [Fact]
    public void IsClean_Should_Return_True_When_Passed_And_No_Vulnerabilities() {
        // Arrange
        var result = new SecurityScanResult(
            SecurityScanStatus.Passed,
            new List<SecurityVulnerability>(),
            "Scanner v1.0");

        // Act & Assert
        result.IsClean.Should().BeTrue();
    }

    [Fact]
    public void IsClean_Should_Return_False_When_Failed() {
        // Arrange
        var result = new SecurityScanResult(
            SecurityScanStatus.Failed,
            new List<SecurityVulnerability>(),
            "Scanner v1.0");

        // Act & Assert
        result.IsClean.Should().BeFalse();
    }

    [Fact]
    public void IsClean_Should_Return_False_When_Has_Vulnerabilities() {
        // Arrange
        var vulnerabilities = new List<SecurityVulnerability>
        {
            new("VULN-001", "Test", "Description", SecurityScanSeverity.Low)
        };
        var result = new SecurityScanResult(
            SecurityScanStatus.Passed,
            vulnerabilities,
            "Scanner v1.0");

        // Act & Assert
        result.IsClean.Should().BeFalse();
    }

    [Fact]
    public void HasCriticalVulnerabilities_Should_Return_True_When_Critical_Vulnerabilities_Exist() {
        // Arrange
        var vulnerabilities = new List<SecurityVulnerability>
        {
            new("VULN-001", "Test 1", "Description 1", SecurityScanSeverity.Low),
            new("VULN-002", "Test 2", "Description 2", SecurityScanSeverity.Critical)
        };
        var result = new SecurityScanResult(
            SecurityScanStatus.Failed,
            vulnerabilities,
            "Scanner v1.0");

        // Act & Assert
        result.HasCriticalVulnerabilities.Should().BeTrue();
    }

    [Fact]
    public void HasCriticalVulnerabilities_Should_Return_False_When_No_Critical_Vulnerabilities() {
        // Arrange
        var vulnerabilities = new List<SecurityVulnerability>
        {
            new("VULN-001", "Test 1", "Description 1", SecurityScanSeverity.Low),
            new("VULN-002", "Test 2", "Description 2", SecurityScanSeverity.Medium)
        };
        var result = new SecurityScanResult(
            SecurityScanStatus.Passed,
            vulnerabilities,
            "Scanner v1.0");

        // Act & Assert
        result.HasCriticalVulnerabilities.Should().BeFalse();
    }

    [Fact]
    public void HasHighVulnerabilities_Should_Return_True_When_High_Vulnerabilities_Exist() {
        // Arrange
        var vulnerabilities = new List<SecurityVulnerability>
        {
            new("VULN-001", "Test 1", "Description 1", SecurityScanSeverity.Low),
            new("VULN-002", "Test 2", "Description 2", SecurityScanSeverity.High)
        };
        var result = new SecurityScanResult(
            SecurityScanStatus.Failed,
            vulnerabilities,
            "Scanner v1.0");

        // Act & Assert
        result.HasHighVulnerabilities.Should().BeTrue();
    }

    [Fact]
    public void HasHighVulnerabilities_Should_Return_False_When_No_High_Vulnerabilities() {
        // Arrange
        var vulnerabilities = new List<SecurityVulnerability>
        {
            new("VULN-001", "Test 1", "Description 1", SecurityScanSeverity.Low),
            new("VULN-002", "Test 2", "Description 2", SecurityScanSeverity.Medium)
        };
        var result = new SecurityScanResult(
            SecurityScanStatus.Passed,
            vulnerabilities,
            "Scanner v1.0");

        // Act & Assert
        result.HasHighVulnerabilities.Should().BeFalse();
    }

    [Theory]
    [InlineData(SecurityScanStatus.Passed)]
    [InlineData(SecurityScanStatus.Failed)]
    [InlineData(SecurityScanStatus.Error)]
    public void SecurityScanResult_Should_Handle_Different_Statuses(SecurityScanStatus status) {
        // Arrange
        var vulnerabilities = new List<SecurityVulnerability>();

        // Act
        var result = new SecurityScanResult(status, vulnerabilities, "Scanner v1.0");

        // Assert
        result.Status.Should().Be(status);
    }

    [Fact]
    public void SecurityScanResult_Should_Handle_Null_ScanLog() {
        // Arrange & Act
        var result = new SecurityScanResult(
            SecurityScanStatus.Passed,
            new List<SecurityVulnerability>(),
            "Scanner v1.0",
            null);

        // Assert
        result.ScanLog.Should().BeNull();
    }
}