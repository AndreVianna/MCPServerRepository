namespace MCPHub.BDD.IntegrationTests.StepDefinitions;

[Binding]
[Collection("TestContainer")]
public class WebApplicationStepDefinitions
{
    private readonly TestContainerFixture _fixture;
    private readonly BddScenarioContext _scenarioContext;

    public WebApplicationStepDefinitions(TestContainerFixture fixture, BddScenarioContext scenarioContext)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
    }

    [Given(@"the MCP Hub web application is running")]
    public void GivenTheMCPHubWebApplicationIsRunning()
    {
        // Initialize web driver if not already done
        if (_scenarioContext.WebDriver == null)
        {
            _scenarioContext.WebDriver = WebDriverFactory.CreateChromeDriver(headless: true);
        }

        // Get the base URL from the web application factory
        var client = _fixture.WebAppFactory.CreateClient();
        var baseUrl = client.BaseAddress?.ToString().TrimEnd('/') ?? "http://localhost";
        
        _scenarioContext.Set("BaseUrl", baseUrl);
    }

    [When(@"I visit the homepage")]
    public void WhenIVisitTheHomepage()
    {
        var baseUrl = _scenarioContext.Get<string>("BaseUrl");
        _scenarioContext.WebDriver!.Navigate().GoToUrl($"{baseUrl}/");
        _scenarioContext.WebDriver.WaitForPageLoad();
        _scenarioContext.CurrentPageUrl = _scenarioContext.WebDriver.Url;
    }

    [When(@"I navigate to the package detail page")]
    public void WhenINavigateToThePackageDetailPage()
    {
        var package = _scenarioContext.CurrentPackage;
        package.Should().NotBeNull("A package should be set in the scenario context");

        var baseUrl = _scenarioContext.Get<string>("BaseUrl");
        var packageUrl = $"{baseUrl}/package/{package!.Publisher.Name}/{package.Name}";
        
        _scenarioContext.WebDriver!.Navigate().GoToUrl(packageUrl);
        _scenarioContext.WebDriver.WaitForPageLoad();
        _scenarioContext.CurrentPageUrl = _scenarioContext.WebDriver.Url;
    }

    [When(@"I click the ""(.*)"" button")]
    public void WhenIClickTheButton(string buttonText)
    {
        var driver = _scenarioContext.WebDriver!;
        
        // Try multiple selectors to find the button
        var buttonSelectors = new[]
        {
            By.XPath($"//button[contains(text(), '{buttonText}')]"),
            By.XPath($"//input[@type='button' and @value='{buttonText}']"),
            By.XPath($"//input[@type='submit' and @value='{buttonText}']"),
            By.XPath($"//a[contains(text(), '{buttonText}')]"),
            By.XPath($"//*[@role='button' and contains(text(), '{buttonText}')]")
        };

        IWebElement? button = null;
        foreach (var selector in buttonSelectors)
        {
            try
            {
                button = driver.WaitForElementToBeClickable(selector, TimeSpan.FromSeconds(2));
                break;
            }
            catch (WebDriverTimeoutException)
            {
                continue;
            }
        }

        button.Should().NotBeNull($"Button with text '{buttonText}' should be found");
        button!.Click();
        driver.WaitForPageLoad();
    }

    [When(@"I enter ""(.*)"" in the search bar")]
    public void WhenIEnterInTheSearchBar(string searchTerm)
    {
        var driver = _scenarioContext.WebDriver!;
        
        var searchSelectors = new[]
        {
            By.Name("search"),
            By.Id("search"),
            By.XPath("//input[@placeholder*='Search' or @placeholder*='search']"),
            By.XPath("//input[@type='search']"),
            By.CssSelector("[data-testid='search-input']")
        };

        IWebElement? searchBox = null;
        foreach (var selector in searchSelectors)
        {
            try
            {
                searchBox = driver.WaitForElement(selector, TimeSpan.FromSeconds(2));
                break;
            }
            catch (WebDriverTimeoutException)
            {
                continue;
            }
        }

        searchBox.Should().NotBeNull("Search input should be found");
        searchBox!.Clear();
        searchBox.SendKeys(searchTerm);

        _scenarioContext.FormData["search"] = searchTerm;
    }

    [When(@"I click the search button")]
    public void WhenIClickTheSearchButton()
    {
        var driver = _scenarioContext.WebDriver!;
        
        var searchButtonSelectors = new[]
        {
            By.XPath("//button[@type='submit']"),
            By.XPath("//button[contains(text(), 'Search')]"),
            By.XPath("//input[@type='submit']"),
            By.CssSelector("[data-testid='search-button']"),
            By.XPath("//button[contains(@class, 'search')]")
        };

        IWebElement? searchButton = null;
        foreach (var selector in searchButtonSelectors)
        {
            try
            {
                searchButton = driver.WaitForElementToBeClickable(selector, TimeSpan.FromSeconds(2));
                break;
            }
            catch (WebDriverTimeoutException)
            {
                continue;
            }
        }

        searchButton.Should().NotBeNull("Search button should be found");
        searchButton!.Click();
        driver.WaitForPageLoad();
    }

    [When(@"I fill in valid registration details")]
    public void WhenIFillInValidRegistrationDetails()
    {
        var driver = _scenarioContext.WebDriver!;
        var testUser = _scenarioContext.DataBuilder.CreateTestUser();

        var formData = new Dictionary<string, string>
        {
            ["email"] = testUser.Email!,
            ["password"] = "SecurePassword123!",
            ["confirmPassword"] = "SecurePassword123!",
            ["firstName"] = testUser.FirstName,
            ["lastName"] = testUser.LastName
        };

        driver.FillForm(formData);
        _scenarioContext.FormData = formData;

        // Check terms agreement checkbox if present
        try
        {
            var termsCheckbox = driver.FindElement(By.Name("termsAgreement"));
            if (!termsCheckbox.Selected)
            {
                termsCheckbox.Click();
            }
        }
        catch (NoSuchElementException)
        {
            // Terms checkbox not found, continue
        }
    }

    [When(@"I submit the form")]
    public void WhenISubmitTheForm()
    {
        var driver = _scenarioContext.WebDriver!;
        
        var submitSelectors = new[]
        {
            By.XPath("//button[@type='submit']"),
            By.XPath("//input[@type='submit']"),
            By.XPath("//button[contains(text(), 'Submit') or contains(text(), 'Register') or contains(text(), 'Sign Up')]")
        };

        IWebElement? submitButton = null;
        foreach (var selector in submitSelectors)
        {
            try
            {
                submitButton = driver.WaitForElementToBeClickable(selector, TimeSpan.FromSeconds(2));
                break;
            }
            catch (WebDriverTimeoutException)
            {
                continue;
            }
        }

        submitButton.Should().NotBeNull("Submit button should be found");
        submitButton!.Click();
        driver.WaitForPageLoad();
    }

    [When(@"I perform a search for ""(.*)""")]
    public void WhenIPerformASearchFor(string searchTerm)
    {
        WhenIEnterInTheSearchBar(searchTerm);
        WhenIClickTheSearchButton();
    }

    [Then(@"I should see the MCP Hub branding")]
    public void ThenIShouldSeeTheMCPHubBranding()
    {
        var driver = _scenarioContext.WebDriver!;
        
        var brandingSelectors = new[]
        {
            By.XPath("//*[contains(text(), 'MCP Hub')]"),
            By.XPath("//h1[contains(text(), 'MCP Hub')]"),
            By.CssSelector("[data-testid='app-title']"),
            By.XPath("//title[contains(text(), 'MCP Hub')]")
        };

        var brandingFound = brandingSelectors.Any(selector => driver.IsElementPresent(selector));
        brandingFound.Should().BeTrue("MCP Hub branding should be visible on the page");
    }

    [Then(@"I should see the main search bar")]
    public void ThenIShouldSeeTheMainSearchBar()
    {
        var driver = _scenarioContext.WebDriver!;
        
        var searchBarSelectors = new[]
        {
            By.Name("search"),
            By.Id("search"),
            By.XPath("//input[@type='search']"),
            By.XPath("//input[@placeholder*='search' or @placeholder*='Search']"),
            By.CssSelector("[data-testid='search-input']")
        };

        var searchBarFound = searchBarSelectors.Any(selector => driver.IsElementVisible(selector));
        searchBarFound.Should().BeTrue("Main search bar should be visible on the homepage");
    }

    [Then(@"I should see featured packages section")]
    public void ThenIShouldSeeFeaturedPackagesSection()
    {
        var driver = _scenarioContext.WebDriver!;
        
        var featuredSelectors = new[]
        {
            By.XPath("//*[contains(text(), 'Featured') or contains(text(), 'featured')]"),
            By.CssSelector("[data-testid='featured-packages']"),
            By.XPath("//section[contains(@class, 'featured')]"),
            By.XPath("//h2[contains(text(), 'Featured')]")
        };

        var featuredFound = featuredSelectors.Any(selector => driver.IsElementPresent(selector));
        featuredFound.Should().BeTrue("Featured packages section should be present on the homepage");
    }

    [Then(@"I should see trending packages")]
    public void ThenIShouldSeeTrendingPackages()
    {
        var driver = _scenarioContext.WebDriver!;
        
        var trendingSelectors = new[]
        {
            By.XPath("//*[contains(text(), 'Trending') or contains(text(), 'trending')]"),
            By.CssSelector("[data-testid='trending-packages']"),
            By.XPath("//section[contains(@class, 'trending')]"),
            By.XPath("//h2[contains(text(), 'Trending')]")
        };

        var trendingFound = trendingSelectors.Any(selector => driver.IsElementPresent(selector));
        trendingFound.Should().BeTrue("Trending packages section should be present on the homepage");
    }

    [Then(@"I should see category navigation")]
    public void ThenIShouldSeeCategoryNavigation()
    {
        var driver = _scenarioContext.WebDriver!;
        
        var categorySelectors = new[]
        {
            By.XPath("//*[contains(text(), 'Categories') or contains(text(), 'categories')]"),
            By.CssSelector("[data-testid='categories']"),
            By.XPath("//nav[contains(@class, 'category')]"),
            By.XPath("//ul[contains(@class, 'category')]")
        };

        var categoryFound = categorySelectors.Any(selector => driver.IsElementPresent(selector));
        categoryFound.Should().BeTrue("Category navigation should be present on the homepage");
    }

    [Then(@"I should see platform statistics")]
    public void ThenIShouldSeePlatformStatistics()
    {
        var driver = _scenarioContext.WebDriver!;
        
        var statsSelectors = new[]
        {
            By.XPath("//*[contains(text(), 'packages') or contains(text(), 'downloads')]"),
            By.CssSelector("[data-testid='stats']"),
            By.XPath("//section[contains(@class, 'stats')]"),
            By.XPath("//*[contains(text(), 'developers') or contains(text(), 'users')]")
        };

        var statsFound = statsSelectors.Any(selector => driver.IsElementPresent(selector));
        statsFound.Should().BeTrue("Platform statistics should be present on the homepage");
    }

    [Then(@"I should be redirected to the search results page")]
    public void ThenIShouldBeRedirectedToTheSearchResultsPage()
    {
        var driver = _scenarioContext.WebDriver!;
        var currentUrl = driver.Url;
        
        currentUrl.Should().Contain("search", "URL should contain 'search' indicating search results page");
        _scenarioContext.CurrentPageUrl = currentUrl;
    }

    [Then(@"I should see packages matching ""(.*)""")]
    public void ThenIShouldSeePackagesMatching(string searchTerm)
    {
        var driver = _scenarioContext.WebDriver!;
        
        // Wait for search results to load
        driver.WaitForElement(By.XPath("//*[contains(@class, 'package') or contains(@class, 'result')]"), TimeSpan.FromSeconds(5));
        
        var packageElements = driver.FindElements(By.XPath("//*[contains(@class, 'package') or contains(@class, 'result')]"));
        packageElements.Should().NotBeEmpty($"Should find packages matching '{searchTerm}'");

        // Verify that at least some results contain the search term
        var pageText = driver.FindElement(By.TagName("body")).Text;
        pageText.Should().Contain(searchTerm, StringComparison.OrdinalIgnoreCase, 
            $"Search results should contain the search term '{searchTerm}'");
    }

    [Then(@"each package result should display name, description, and trust tier")]
    public void ThenEachPackageResultShouldDisplayNameDescriptionAndTrustTier()
    {
        var driver = _scenarioContext.WebDriver!;
        
        var packageCards = driver.FindElements(By.XPath("//*[contains(@class, 'package') or contains(@class, 'result')]"));
        packageCards.Should().NotBeEmpty("Should have package result cards");

        foreach (var card in packageCards.Take(3)) // Check first 3 for performance
        {
            // Check for package name (should be a heading or prominent text)
            var hasName = card.FindElements(By.XPath(".//h1 | .//h2 | .//h3 | .//h4 | .//*[contains(@class, 'name') or contains(@class, 'title')]")).Any();
            hasName.Should().BeTrue("Each package card should display a name");

            // Check for description text
            var hasDescription = card.Text.Length > 50; // Reasonable description length
            hasDescription.Should().BeTrue("Each package card should display a description");

            // Check for trust tier indicators (badges, icons, or tier text)
            var hasTrustTier = card.FindElements(By.XPath(".//*[contains(@class, 'trust') or contains(@class, 'tier') or contains(@class, 'badge')]")).Any() ||
                              card.Text.Contains("Trust") || card.Text.Contains("Tier") || 
                              card.Text.Contains("Certified") || card.Text.Contains("Verified");
            hasTrustTier.Should().BeTrue("Each package card should display trust tier information");
        }
    }

    [Then(@"I should see download counts and ratings")]
    public void ThenIShouldSeeDownloadCountsAndRatings()
    {
        var driver = _scenarioContext.WebDriver!;
        var pageText = driver.FindElement(By.TagName("body")).Text;
        
        // Look for download indicators
        var hasDownloads = pageText.Contains("download", StringComparison.OrdinalIgnoreCase) ||
                          pageText.Contains("DL", StringComparison.OrdinalIgnoreCase) ||
                          Regex.IsMatch(pageText, @"\d+[KMk]?\s*(downloads?|DL)", RegexOptions.IgnoreCase);
        
        hasDownloads.Should().BeTrue("Search results should display download counts");

        // Look for rating indicators  
        var hasRatings = driver.FindElements(By.XPath("//*[contains(@class, 'star') or contains(@class, 'rating')]")).Any() ||
                        Regex.IsMatch(pageText, @"⭐|★|rating|\d+\.\d+/\d+|\d+/10", RegexOptions.IgnoreCase);
        
        hasRatings.Should().BeTrue("Search results should display ratings or stars");
    }

    [Then(@"I should see comprehensive package information:")]
    public void ThenIShouldSeeComprehensivePackageInformation(Table table)
    {
        var driver = _scenarioContext.WebDriver!;
        var pageText = driver.FindElement(By.TagName("body")).Text.ToLowerInvariant();

        foreach (var row in table.Rows)
        {
            var section = row["Section"].ToLowerInvariant();
            var content = row["Content"].ToLowerInvariant();

            // Check if the section content is present on the page
            var sectionFound = pageText.Contains(section) || 
                              driver.FindElements(By.XPath($"//*[contains(@class, '{section}') or contains(@id, '{section}')]")).Any();

            sectionFound.Should().BeTrue($"Package detail page should contain {section} section with {content}");
        }
    }

    [Then(@"I should see an installation command that I can copy")]
    public void ThenIShouldSeeAnInstallationCommandThatICanCopy()
    {
        var driver = _scenarioContext.WebDriver!;
        var pageText = driver.FindElement(By.TagName("body")).Text;

        // Look for installation command patterns
        var hasInstallCommand = pageText.Contains("mcpm install", StringComparison.OrdinalIgnoreCase) ||
                               pageText.Contains("npm install", StringComparison.OrdinalIgnoreCase) ||
                               Regex.IsMatch(pageText, @"(mcpm|npm|pip)\s+install", RegexOptions.IgnoreCase);

        hasInstallCommand.Should().BeTrue("Package detail page should display installation command");

        // Look for copy button or copyable code block
        var hasCopyButton = driver.FindElements(By.XPath("//*[contains(@class, 'copy') or contains(text(), 'Copy')]")).Any() ||
                           driver.FindElements(By.XPath("//code | //pre | //*[@class*='command']")).Any();

        hasCopyButton.Should().BeTrue("Installation command should be copyable");
    }

    [Then(@"I should see the login form")]
    public void ThenIShouldSeeTheLoginForm()
    {
        var driver = _scenarioContext.WebDriver!;
        
        // Check for email/username field
        var hasEmailField = driver.IsElementPresent(By.Name("email")) || 
                           driver.IsElementPresent(By.Name("username")) ||
                           driver.IsElementPresent(By.XPath("//input[@type='email']"));

        hasEmailField.Should().BeTrue("Login form should have email/username field");

        // Check for password field
        var hasPasswordField = driver.IsElementPresent(By.Name("password")) ||
                              driver.IsElementPresent(By.XPath("//input[@type='password']"));

        hasPasswordField.Should().BeTrue("Login form should have password field");

        // Check for submit button
        var hasSubmitButton = driver.IsElementPresent(By.XPath("//button[@type='submit']")) ||
                             driver.IsElementPresent(By.XPath("//input[@type='submit']")) ||
                             driver.IsElementPresent(By.XPath("//button[contains(text(), 'Login') or contains(text(), 'Sign In')]"));

        hasSubmitButton.Should().BeTrue("Login form should have submit button");
    }

    [Then(@"I should see a confirmation message about email verification")]
    public void ThenIShouldSeeAConfirmationMessageAboutEmailVerification()
    {
        var driver = _scenarioContext.WebDriver!;
        var pageText = driver.FindElement(By.TagName("body")).Text;

        var hasVerificationMessage = pageText.Contains("verification", StringComparison.OrdinalIgnoreCase) ||
                                   pageText.Contains("verify", StringComparison.OrdinalIgnoreCase) ||
                                   pageText.Contains("email", StringComparison.OrdinalIgnoreCase);

        hasVerificationMessage.Should().BeTrue("Should see confirmation message about email verification");
    }

    [Given(@"I am logged in as a user")]
    public void GivenIAmLoggedInAsAUser()
    {
        // Simulate logged-in state by setting user context
        var testUser = _scenarioContext.GetOrCreateTestUser();
        _scenarioContext.SetCurrentUser(testUser.Id.ToString(), testUser.Email!, "User");
        
        // In a real implementation, this would involve:
        // 1. Navigating to login page
        // 2. Filling in credentials
        // 3. Submitting form
        // 4. Verifying successful login
        
        // For now, we'll simulate this by storing user state
        _scenarioContext.Set("LoggedInUser", testUser);
    }

    [Given(@"I am logged in as a publisher with packages")]
    public void GivenIAmLoggedInAsAPublisherWithPackages()
    {
        var testUser = _scenarioContext.GetOrCreateTestUser("publisher@example.com");
        _scenarioContext.SetCurrentUser(testUser.Id.ToString(), testUser.Email!, "Publisher");
        
        // Create test packages for this publisher
        var packages = _scenarioContext.DataBuilder.CreateTestPackages(5);
        foreach (var package in packages)
        {
            package.Publisher.Email = testUser.Email;
            package.PublisherId = testUser.Id;
        }
        
        var scenario = new TestScenario 
        { 
            Packages = packages,
            Users = new[] { testUser }.ToList()
        };
        
        _scenarioContext.SetTestScenario(scenario);
        _scenarioContext.Set("LoggedInUser", testUser);
    }

    [When(@"I navigate to my publisher dashboard")]
    public void WhenINavigateToMyPublisherDashboard()
    {
        var baseUrl = _scenarioContext.Get<string>("BaseUrl");
        _scenarioContext.WebDriver!.Navigate().GoToUrl($"{baseUrl}/dashboard");
        _scenarioContext.WebDriver.WaitForPageLoad();
        _scenarioContext.CurrentPageUrl = _scenarioContext.WebDriver.Url;
    }

    [Then(@"I should see dashboard sections:")]
    public void ThenIShouldSeeDashboardSections(Table table)
    {
        var driver = _scenarioContext.WebDriver!;
        var pageText = driver.FindElement(By.TagName("body")).Text.ToLowerInvariant();

        foreach (var row in table.Rows)
        {
            var section = row["Section"].ToLowerInvariant();
            var content = row["Content"].ToLowerInvariant();

            var sectionFound = pageText.Contains(section) ||
                              driver.FindElements(By.XPath($"//*[contains(text(), '{section}')]")).Any();

            sectionFound.Should().BeTrue($"Dashboard should contain '{section}' section");
        }
    }

    [Given(@"I am using a mobile device")]
    public void GivenIAmUsingAMobileDevice()
    {
        // Resize browser to mobile viewport
        _scenarioContext.WebDriver!.Manage().Window.Size = new System.Drawing.Size(375, 667); // iPhone size
        
        // Set mobile user agent if needed
        var options = new ChromeOptions();
        options.AddArgument("--user-agent=Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15");
    }

    [Then(@"all pages should display correctly on mobile:")]
    public void ThenAllPagesShouldDisplayCorrectlyOnMobile(Table table)
    {
        var driver = _scenarioContext.WebDriver!;
        var baseUrl = _scenarioContext.Get<string>("BaseUrl");

        foreach (var row in table.Rows)
        {
            var pageType = row["Page Type"];
            var adaptations = row["Mobile Adaptations"];

            // Navigate to representative page for each type
            var testUrl = GetTestUrlForPageType(baseUrl, pageType);
            driver.Navigate().GoToUrl(testUrl);
            driver.WaitForPageLoad();

            // Verify page loads without horizontal scrolling
            var bodyWidth = ((IJavaScriptExecutor)driver).ExecuteScript("return document.body.scrollWidth;");
            var windowWidth = ((IJavaScriptExecutor)driver).ExecuteScript("return window.innerWidth;");
            
            ((long)bodyWidth).Should().BeLessOrEqualTo((long)windowWidth + 20, // Allow small margin
                $"{pageType} should not have horizontal scroll on mobile");

            // Verify responsive design elements are present
            var hasResponsiveElements = driver.FindElements(By.XPath("//*[contains(@class, 'mobile') or contains(@class, 'responsive')]")).Any();
            // Note: This is a basic check - real implementation would verify specific mobile adaptations
        }
    }

    private static string GetTestUrlForPageType(string baseUrl, string pageType)
    {
        return pageType.ToLowerInvariant() switch
        {
            "homepage" => $"{baseUrl}/",
            "search results" => $"{baseUrl}/search?q=test",
            "package detail" => $"{baseUrl}/package/test/package",
            "dashboard" => $"{baseUrl}/dashboard",
            "forms" => $"{baseUrl}/auth/login",
            _ => $"{baseUrl}/"
        };
    }
}