using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Unit tests for SecurityScan entity.
/// This demonstrates comprehensive testing of domain entities with proper AAA structure.
/// </summary>
public class SecurityScanTests {
    [Fact]
    public void SecurityScan_Should_Be_Created_With_Valid_Properties() {
        // Arrange
        var serverVersionId = Guid.NewGuid();
        var scanType = ScanType.StaticAnalysis;
        var scannerVersion = "Scanner v1.0";

        // Act
        var securityScan = new SecurityScan(serverVersionId, scanType, scannerVersion);

        // Assert
        securityScan.Id.Should().NotBeEmpty();
        securityScan.VersionId.Should().Be(serverVersionId);
        securityScan.ServerVersionId.Should().Be(serverVersionId);
        securityScan.ScanType.Should().Be(scanType);
        securityScan.ScannerVersion.Should().Be(scannerVersion);
        securityScan.ScanStartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        securityScan.Status.Should().Be(ScanStatus.InProgress);
        securityScan.CriticalIssues.Should().Be(0);
        securityScan.ScanCompletedAt.Should().BeNull();
        securityScan.Result.Should().BeNull();
        securityScan.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void SecurityScan_Should_Throw_ArgumentNullException_When_ScannerVersion_Is_Null() {
        // Arrange
        var serverVersionId = Guid.NewGuid();
        var scanType = ScanType.StaticAnalysis;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new SecurityScan(serverVersionId, scanType, null!));
        exception.ParamName.Should().Be("scannerVersion");
    }

    [Fact]
    public void Complete_Should_Set_Result_And_Update_Status() {
        // Arrange
        var securityScan = new SecurityScan(Guid.NewGuid(), ScanType.StaticAnalysis, "Scanner v1.0");
        var scanResult = new SecurityScanResult(
            SecurityScanStatus.Passed,
            new List<SecurityVulnerability>(),
            "Scanner v1.0");

        // Act
        securityScan.Complete(scanResult);

        // Assert
        securityScan.Result.Should().Be(scanResult);
        securityScan.Status.Should().Be(ScanStatus.Completed);
        securityScan.ScanCompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        securityScan.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Complete_Should_Throw_ArgumentNullException_When_Result_Is_Null() {
        // Arrange
        var securityScan = new SecurityScan(Guid.NewGuid(), ScanType.StaticAnalysis, "Scanner v1.0");

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            securityScan.Complete(null!));
        exception.ParamName.Should().Be("result");
    }

    [Fact]
    public void Fail_Should_Set_ErrorMessage_And_Update_Status() {
        // Arrange
        var securityScan = new SecurityScan(Guid.NewGuid(), ScanType.StaticAnalysis, "Scanner v1.0");
        var errorMessage = "Scan failed due to timeout";

        // Act
        securityScan.Fail(errorMessage);

        // Assert
        securityScan.ErrorMessage.Should().Be(errorMessage);
        securityScan.Status.Should().Be(ScanStatus.Failed);
        securityScan.ScanCompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Fail_Should_Throw_ArgumentNullException_When_ErrorMessage_Is_Null() {
        // Arrange
        var securityScan = new SecurityScan(Guid.NewGuid(), ScanType.StaticAnalysis, "Scanner v1.0");

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            securityScan.Fail(null!));
        exception.ParamName.Should().Be("errorMessage");
    }

    [Fact]
    public void AddMetadata_Should_Add_Key_Value_Pair_To_Metadata() {
        // Arrange
        var securityScan = new SecurityScan(Guid.NewGuid(), ScanType.StaticAnalysis, "Scanner v1.0");
        var key = "test_key";
        var value = "test_value";

        // Act
        securityScan.AddMetadata(key, value);

        // Assert
        securityScan.Metadata.Should().ContainKey(key);
        securityScan.Metadata[key].Should().Be(value);
    }

    [Fact]
    public void AddMetadata_Should_Update_Existing_Key() {
        // Arrange
        var securityScan = new SecurityScan(Guid.NewGuid(), ScanType.StaticAnalysis, "Scanner v1.0");
        var key = "test_key";
        var originalValue = "original_value";
        var newValue = "new_value";

        // Act
        securityScan.AddMetadata(key, originalValue);
        securityScan.AddMetadata(key, newValue);

        // Assert
        securityScan.Metadata[key].Should().Be(newValue);
        securityScan.Metadata.Should().HaveCount(1);
    }

    [Fact]
    public void SecurityScan_Should_Initialize_Empty_Metadata_Dictionary() {
        // Arrange & Act
        var securityScan = new SecurityScan(Guid.NewGuid(), ScanType.StaticAnalysis, "Scanner v1.0");

        // Assert
        securityScan.Metadata.Should().NotBeNull();
        securityScan.Metadata.Should().BeEmpty();
    }

    [Theory]
    [InlineData(ScanType.StaticAnalysis)]
    [InlineData(ScanType.DynamicTesting)]
    [InlineData(ScanType.Compliance)]
    public void SecurityScan_Should_Handle_Different_ScanTypes(ScanType scanType) {
        // Arrange
        var serverVersionId = Guid.NewGuid();
        var scannerVersion = "Scanner v1.0";

        // Act
        var securityScan = new SecurityScan(serverVersionId, scanType, scannerVersion);

        // Assert
        securityScan.ScanType.Should().Be(scanType);
    }

    [Fact]
    public void SecurityScan_Should_Set_Both_VersionId_And_ServerVersionId_To_Same_Value() {
        // Arrange
        var serverVersionId = Guid.NewGuid();
        var scanType = ScanType.StaticAnalysis;
        var scannerVersion = "Scanner v1.0";

        // Act
        var securityScan = new SecurityScan(serverVersionId, scanType, scannerVersion);

        // Assert
        securityScan.VersionId.Should().Be(serverVersionId);
        securityScan.ServerVersionId.Should().Be(serverVersionId);
        securityScan.VersionId.Should().Be(securityScan.ServerVersionId);
    }

    [Fact]
    public void SecurityScan_Should_Default_To_InProgress_Status() {
        // Arrange & Act
        var securityScan = new SecurityScan(Guid.NewGuid(), ScanType.StaticAnalysis, "Scanner v1.0");

        // Assert
        securityScan.Status.Should().Be(ScanStatus.InProgress);
    }

    [Fact]
    public void SecurityScan_Should_Default_To_Zero_CriticalIssues() {
        // Arrange & Act
        var securityScan = new SecurityScan(Guid.NewGuid(), ScanType.StaticAnalysis, "Scanner v1.0");

        // Assert
        securityScan.CriticalIssues.Should().Be(0);
    }
}