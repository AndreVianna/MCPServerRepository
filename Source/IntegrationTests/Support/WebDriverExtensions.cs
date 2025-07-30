namespace MCPHub.IntegrationTests.Support;

/// <summary>
/// Extension methods for WebDriver to support BDD test scenarios
/// </summary>
public static class WebDriverExtensions {
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    public static IWebElement WaitForElement(this IWebDriver driver, By by, TimeSpan? timeout = null) {
        var wait = new WebDriverWait(driver, timeout ?? DefaultTimeout);
        return wait.Until(d => d.FindElement(by));
    }

    public static IWebElement WaitForElementToBeClickable(this IWebDriver driver, By by, TimeSpan? timeout = null) {
        var wait = new WebDriverWait(driver, timeout ?? DefaultTimeout);
        return wait.Until(d => {
            var element = d.FindElement(by);
            return element.Displayed && element.Enabled ? element : null;
        })!;
    }

    public static void WaitForElementToDisappear(this IWebDriver driver, By by, TimeSpan? timeout = null) {
        var wait = new WebDriverWait(driver, timeout ?? DefaultTimeout);
        wait.Until(d => !d.FindElements(by).Any());
    }

    public static void WaitForPageLoad(this IWebDriver driver, TimeSpan? timeout = null) {
        var wait = new WebDriverWait(driver, timeout ?? DefaultTimeout);
        wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
    }

    public static void ScrollToElement(this IWebDriver driver, IWebElement element) {
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        Thread.Sleep(500); // Allow scroll animation to complete
    }

    public static void FillForm(this IWebDriver driver, Dictionary<string, string> formData) {
        foreach (var field in formData) {
            var element = driver.FindElement(By.Name(field.Key));
            element.Clear();
            element.SendKeys(field.Value);
        }
    }

    public static string GetElementText(this IWebDriver driver, By by) => driver.FindElement(by).Text;

    public static bool IsElementPresent(this IWebDriver driver, By by) {
        try {
            driver.FindElement(by);
            return true;
        }
        catch (NoSuchElementException) {
            return false;
        }
    }

    public static bool IsElementVisible(this IWebDriver driver, By by) {
        try {
            var element = driver.FindElement(by);
            return element.Displayed;
        }
        catch (NoSuchElementException) {
            return false;
        }
    }

    public static void SelectDropdownByText(this IWebDriver driver, By by, string text) {
        var element = driver.FindElement(by);
        var select = new SelectElement(element);
        select.SelectByText(text);
    }

    public static void SelectDropdownByValue(this IWebDriver driver, By by, string value) {
        var element = driver.FindElement(by);
        var select = new SelectElement(element);
        select.SelectByValue(value);
    }

    public static void TakeScreenshot(this IWebDriver driver, string filePath) {
        var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
        screenshot.SaveAsFile(filePath);
    }

    public static void WaitForAjaxToComplete(this IWebDriver driver, TimeSpan? timeout = null) {
        var wait = new WebDriverWait(driver, timeout ?? DefaultTimeout);
        wait.Until(d => (bool)((IJavaScriptExecutor)d).ExecuteScript("return jQuery.active == 0"));
    }

    public static void HandleAlert(this IWebDriver driver, bool accept = true) {
        try {
            var alert = driver.SwitchTo().Alert();
            if (accept)
                alert.Accept();
            else
                alert.Dismiss();
        }
        catch (NoAlertPresentException) {
            // No alert present, continue
        }
    }

    public static void SwitchToFrame(this IWebDriver driver, string frameNameOrId) => driver.SwitchTo().Frame(frameNameOrId);

    public static void SwitchToDefaultContent(this IWebDriver driver) => driver.SwitchTo().DefaultContent();

    public static void OpenNewTab(this IWebDriver driver, string url) {
        ((IJavaScriptExecutor)driver).ExecuteScript($"window.open('{url}', '_blank');");
        var handles = driver.WindowHandles;
        driver.SwitchTo().Window(handles.Last());
    }

    public static void CloseCurrentTab(this IWebDriver driver) {
        driver.Close();
        var handles = driver.WindowHandles;
        if (handles.Any()) {
            driver.SwitchTo().Window(handles.First());
        }
    }
}

/// <summary>
/// Page Object Model base class for web pages
/// </summary>
public abstract class BasePage(IWebDriver driver) {
    protected readonly IWebDriver Driver = driver ?? throw new ArgumentNullException(nameof(driver));
    protected readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    public virtual void WaitForPageLoad() => Driver.WaitForPageLoad();

    public virtual bool IsPageLoaded() => Driver.Url.Contains(GetPageUrl());

    protected abstract string GetPageUrl();

    protected IWebElement WaitForElement(By by, TimeSpan? timeout = null) => Driver.WaitForElement(by, timeout);

    protected void ClickElement(By by) {
        var element = Driver.WaitForElementToBeClickable(by);
        element.Click();
    }

    protected void EnterText(By by, string text) {
        var element = Driver.WaitForElement(by);
        element.Clear();
        element.SendKeys(text);
    }

    protected string GetText(By by) => Driver.GetElementText(by);

    protected bool IsElementPresent(By by) => Driver.IsElementPresent(by);

    protected bool IsElementVisible(By by) => Driver.IsElementVisible(by);
}

/// <summary>
/// Factory for creating WebDriver instances
/// </summary>
public static class WebDriverFactory {
    public static IWebDriver CreateChromeDriver(bool headless = true) {
        var options = new ChromeOptions();

        if (headless) {
            options.AddArgument("--headless");
        }

        options.AddArguments(
            "--no-sandbox",
            "--disable-dev-shm-usage",
            "--disable-gpu",
            "--disable-web-security",
            "--allow-running-insecure-content",
            "--disable-extensions",
            "--disable-plugins",
            "--disable-images",
            "--disable-javascript",
            "--window-size=1920,1080"
        );

        return new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
    }

    public static void QuitDriver(IWebDriver? driver) {
        try {
            driver?.Quit();
        }
        catch (Exception) {
            // Ignore cleanup errors
        }
        finally {
            driver?.Dispose();
        }
    }
}