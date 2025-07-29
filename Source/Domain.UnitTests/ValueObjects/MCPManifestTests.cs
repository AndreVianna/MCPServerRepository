using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.UnitTests.ValueObjects;

/// <summary>
/// Unit tests for MCPManifest value object
/// </summary>
public class MCPManifestTests {
    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPManifest")]
    public void Validate_WithValidManifest_ShouldReturnSuccess() {
        // Arrange
        var manifest = new MCPManifest {
            Name = "test-package",
            Version = "1.0.0",
            Description = "A test MCP package",
            Author = new MCPAuthor { Name = "Test Author", Email = "test@example.com" },
            License = "MIT",
            Homepage = "https://example.com",
            Repository = "https://github.com/test/package",
            Capabilities = new MCPCapabilities {
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
        Assert.True(result.IsValid);
        Assert.Equal(0, result.Errors.Count);
        Assert.Equal("manifest", result.Context);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPManifest")]
    public void Validate_WithMissingRequiredFields_ShouldReturnErrors() {
        // Arrange
        var manifest = new MCPManifest {
            // Missing name, version, description, license, author
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 4); // name, version, description, license, author
        Assert.True(result.Errors.Any(e => e.Contains("name")));
        Assert.True(result.Errors.Any(e => e.Contains("version")));
        Assert.True(result.Errors.Any(e => e.Contains("description")));
        Assert.True(result.Errors.Any(e => e.Contains("license")));
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPManifest")]
    public void Validate_WithInvalidPackageName_ShouldReturnError() {
        // Arrange
        var manifest = new MCPManifest {
            Name = "Invalid-Package-Name-With-Capitals",
            Version = "1.0.0",
            Description = "Test package",
            Author = new MCPAuthor { Name = "Test Author" },
            License = "MIT"
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Any(e => e.Contains("Package name")));
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPManifest")]
    public void Validate_WithValidNamespacedPackageName_ShouldReturnSuccess() {
        // Arrange
        var manifest = new MCPManifest {
            Name = "@namespace/package-name",
            Version = "1.0.0",
            Description = "Test package",
            Author = new MCPAuthor { Name = "Test Author" },
            License = "MIT"
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal(0, result.Errors.Count);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPManifest")]
    public void Validate_WithInvalidSemanticVersion_ShouldReturnError() {
        // Arrange
        var manifest = new MCPManifest {
            Name = "test-package",
            Version = "invalid-version",
            Description = "Test package",
            Author = new MCPAuthor { Name = "Test Author" },
            License = "MIT"
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Any(e => e.Contains("semantic versioning")));
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPManifest")]
    public void Validate_WithValidSemanticVersions_ShouldReturnSuccess() {
        // Arrange
        var versions = new[] { "1.0.0", "1.2.3", "2.0.0-beta.1", "1.0.0-alpha.1" };

        foreach (var version in versions) {
            var manifest = new MCPManifest {
                Name = "test-package",
                Version = version,
                Description = "Test package",
                Author = new MCPAuthor { Name = "Test Author" },
                License = "MIT"
            };

            // Act
            var result = manifest.Validate();

            // Assert
            Assert.True(result.IsValid, $"Version {version} should be valid");
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPManifest")]
    public void Validate_WithInvalidUrls_ShouldReturnErrors() {
        // Arrange
        var manifest = new MCPManifest {
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
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3); // homepage, repository, bugs
        Assert.True(result.Errors.Any(e => e.Contains("Homepage")));
        Assert.True(result.Errors.Any(e => e.Contains("Repository")));
        Assert.True(result.Errors.Any(e => e.Contains("Bugs")));
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPManifest")]
    public void Validate_WithNoCapabilities_ShouldReturnWarning() {
        // Arrange
        var manifest = new MCPManifest {
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
        Assert.True(result.IsValid); // Should still be valid
        Assert.True(result.Warnings.Count > 0);
        Assert.True(result.Warnings.Any(w => w.Contains("capability")));
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPManifest")]
    public void Validate_WithMissingAuthorName_ShouldReturnError() {
        // Arrange
        var manifest = new MCPManifest {
            Name = "test-package",
            Version = "1.0.0",
            Description = "Test package",
            Author = new MCPAuthor { Email = "test@example.com" }, // Missing name
            License = "MIT"
        };

        // Act
        var result = manifest.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Any(e => e.Contains("Author name")));
    }
}

/// <summary>
/// Unit tests for MCPCapabilities
/// </summary>
public class MCPCapabilitiesTests {
    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPCapabilities")]
    public void HasAnyCapability_WithNoCapabilities_ShouldReturnFalse() {
        // Arrange
        var capabilities = new MCPCapabilities();

        // Act
        var result = capabilities.HasAnyCapability();

        // Assert
        Assert.False(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPCapabilities")]
    public void HasAnyCapability_WithTools_ShouldReturnTrue() {
        // Arrange
        var capabilities = new MCPCapabilities {
            Tools = [new MCPTool { Name = "test-tool", Description = "Test" }]
        };

        // Act
        var result = capabilities.HasAnyCapability();

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPCapabilities")]
    public void HasAnyCapability_WithResources_ShouldReturnTrue() {
        // Arrange
        var capabilities = new MCPCapabilities {
            Resources = [new MCPResource { Uri = "test://resource", Name = "test-resource", Description = "Test" }]
        };

        // Act
        var result = capabilities.HasAnyCapability();

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "MCPCapabilities")]
    public void HasAnyCapability_WithPrompts_ShouldReturnTrue() {
        // Arrange
        var capabilities = new MCPCapabilities {
            Prompts = [new MCPPrompt { Name = "test-prompt", Description = "Test" }]
        };

        // Act
        var result = capabilities.HasAnyCapability();

        // Assert
        Assert.True(result);
    }
}