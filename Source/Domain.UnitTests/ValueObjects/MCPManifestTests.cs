using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.UnitTests.ValueObjects;

/// <summary>
/// Unit tests for MCPManifest value object
/// </summary>
[TestClass]
public class MCPManifestTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPManifest")]
    public void Validate_WithValidManifest_ShouldReturnSuccess()
    {
        // Arrange
        var manifest = new MCPManifest
        {
            Name = "test-package",
            Version = "1.0.0",
            Description = "A test MCP package",
            Author = new MCPAuthor { Name = "Test Author", Email = "test@example.com" },
            License = "MIT",
            Homepage = "https://example.com",
            Repository = "https://github.com/test/package",
            Capabilities = new MCPCapabilities
            {
                Tools = [
                    new MCPTool 
                    { 
                        Name = "test-tool", 
                        Description = "A test tool" 
                    }
                ]
            }
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
        Assert.AreEqual("manifest", result.Context);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPManifest")]
    public void Validate_WithMissingRequiredFields_ShouldReturnErrors()
    {
        // Arrange
        var manifest = new MCPManifest
        {
            // Missing name, version, description, license, author
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Count >= 4); // name, version, description, license, author
        Assert.IsTrue(result.Errors.Any(e => e.Contains("name")));
        Assert.IsTrue(result.Errors.Any(e => e.Contains("version")));
        Assert.IsTrue(result.Errors.Any(e => e.Contains("description")));
        Assert.IsTrue(result.Errors.Any(e => e.Contains("license")));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPManifest")]
    public void Validate_WithInvalidPackageName_ShouldReturnError()
    {
        // Arrange
        var manifest = new MCPManifest
        {
            Name = "Invalid-Package-Name-With-Capitals",
            Version = "1.0.0",
            Description = "Test package",
            Author = new MCPAuthor { Name = "Test Author" },
            License = "MIT"
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Package name")));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPManifest")]
    public void Validate_WithValidNamespacedPackageName_ShouldReturnSuccess()
    {
        // Arrange
        var manifest = new MCPManifest
        {
            Name = "@namespace/package-name",
            Version = "1.0.0",
            Description = "Test package",
            Author = new MCPAuthor { Name = "Test Author" },
            License = "MIT"
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPManifest")]
    public void Validate_WithInvalidSemanticVersion_ShouldReturnError()
    {
        // Arrange
        var manifest = new MCPManifest
        {
            Name = "test-package",
            Version = "invalid-version",
            Description = "Test package",
            Author = new MCPAuthor { Name = "Test Author" },
            License = "MIT"
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("semantic versioning")));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPManifest")]
    public void Validate_WithValidSemanticVersions_ShouldReturnSuccess()
    {
        // Arrange
        var versions = new[] { "1.0.0", "1.2.3", "2.0.0-beta.1", "1.0.0-alpha.1" };

        foreach (var version in versions)
        {
            var manifest = new MCPManifest
            {
                Name = "test-package",
                Version = version,
                Description = "Test package",
                Author = new MCPAuthor { Name = "Test Author" },
                License = "MIT"
            };

            // Act
            var result = manifest.Validate();

            // Assert
            Assert.IsTrue(result.IsValid, $"Version {version} should be valid");
        }
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPManifest")]
    public void Validate_WithInvalidUrls_ShouldReturnErrors()
    {
        // Arrange
        var manifest = new MCPManifest
        {
            Name = "test-package",
            Version = "1.0.0",
            Description = "Test package",
            Author = new MCPAuthor { Name = "Test Author" },
            License = "MIT",
            Homepage = "invalid-url",
            Repository = "not-a-url",
            Bugs = "also-invalid"
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Count >= 3); // homepage, repository, bugs
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Homepage")));
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Repository")));
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Bugs")));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPManifest")]
    public void Validate_WithNoCapabilities_ShouldReturnWarning()
    {
        // Arrange
        var manifest = new MCPManifest
        {
            Name = "test-package",
            Version = "1.0.0",
            Description = "Test package",
            Author = new MCPAuthor { Name = "Test Author" },
            License = "MIT",
            Capabilities = new MCPCapabilities() // Empty capabilities
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.IsTrue(result.IsValid); // Should still be valid
        Assert.IsTrue(result.Warnings.Count > 0);
        Assert.IsTrue(result.Warnings.Any(w => w.Contains("capability")));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPManifest")]
    public void Validate_WithMissingAuthorName_ShouldReturnError()
    {
        // Arrange
        var manifest = new MCPManifest
        {
            Name = "test-package",
            Version = "1.0.0",
            Description = "Test package",
            Author = new MCPAuthor { Email = "test@example.com" }, // Missing name
            License = "MIT"
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Author name")));
    }
}

/// <summary>
/// Unit tests for MCPCapabilities
/// </summary>
[TestClass]
public class MCPCapabilitiesTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPCapabilities")]
    public void HasAnyCapability_WithNoCapabilities_ShouldReturnFalse()
    {
        // Arrange
        var capabilities = new MCPCapabilities();

        // Act
        var result = capabilities.HasAnyCapability();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPCapabilities")]
    public void HasAnyCapability_WithTools_ShouldReturnTrue()
    {
        // Arrange
        var capabilities = new MCPCapabilities
        {
            Tools = [new MCPTool { Name = "test-tool", Description = "Test" }]
        };

        // Act
        var result = capabilities.HasAnyCapability();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPCapabilities")]
    public void HasAnyCapability_WithResources_ShouldReturnTrue()
    {
        // Arrange
        var capabilities = new MCPCapabilities
        {
            Resources = [new MCPResource { Uri = "test://resource", Name = "test-resource", Description = "Test" }]
        };

        // Act
        var result = capabilities.HasAnyCapability();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("MCPCapabilities")]
    public void HasAnyCapability_WithPrompts_ShouldReturnTrue()
    {
        // Arrange
        var capabilities = new MCPCapabilities
        {
            Prompts = [new MCPPrompt { Name = "test-prompt", Description = "Test" }]
        };

        // Act
        var result = capabilities.HasAnyCapability();

        // Assert
        Assert.IsTrue(result);
    }
}