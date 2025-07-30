namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Implementation of interactive user prompts using Spectre.Console
/// </summary>
public class InteractionService(McpmConfiguration configuration) : IInteractionService {
    private readonly McpmConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    /// <inheritdoc />
    public Task<bool> ConfirmAsync(string message, bool defaultValue = false, CancellationToken cancellationToken = default) {
        // Check if running in non-interactive mode
        if (_configuration.Ui.NonInteractive) {
            return Task.FromResult(defaultValue);
        }

        var prompt = new ConfirmationPrompt(message) {
            DefaultValue = defaultValue,
        };

        var result = AnsiConsole.Prompt(prompt);
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<T?> SelectFromListAsync<T>(string message, IEnumerable<T> options, Func<T, string> displaySelector, CancellationToken cancellationToken = default) where T : notnull {
        var optionsList = options.ToList();
        if (!optionsList.Any()) {
            return Task.FromResult<T?>(default);
        }

        // Check if running in non-interactive mode
        if (_configuration.Ui.NonInteractive) {
            return Task.FromResult<T?>(optionsList.First());
        }

        var prompt = new SelectionPrompt<T>()
            .Title(message)
            .AddChoices(optionsList)
            .UseConverter(displaySelector);

        if (optionsList.Count > 10) {
            prompt.PageSize(10);
        }

        var result = AnsiConsole.Prompt(prompt);
        return Task.FromResult<T?>(result);
    }

    /// <inheritdoc />
    public Task<IEnumerable<T>> SelectMultipleAsync<T>(string message, IEnumerable<T> options, Func<T, string> displaySelector, CancellationToken cancellationToken = default) where T : notnull {
        var optionsList = options.ToList();
        if (!optionsList.Any()) {
            return Task.FromResult<IEnumerable<T>>([]);
        }

        // Check if running in non-interactive mode
        if (_configuration.Ui.NonInteractive) {
            return Task.FromResult<IEnumerable<T>>([]);
        }

        var prompt = new MultiSelectionPrompt<T>()
            .Title(message)
            .AddChoices(optionsList)
            .UseConverter(displaySelector);

        if (optionsList.Count > 10) {
            prompt.PageSize(10);
        }

        var result = AnsiConsole.Prompt(prompt);
        return Task.FromResult<IEnumerable<T>>(result);
    }

    /// <inheritdoc />
    public Task<string?> PromptAsync(string message, string? defaultValue = null, Func<string, string?>? validator = null, CancellationToken cancellationToken = default) {
        // Check if running in non-interactive mode
        if (_configuration.Ui.NonInteractive) {
            return Task.FromResult(defaultValue);
        }

        var prompt = new TextPrompt<string>(message);

        if (!string.IsNullOrEmpty(defaultValue)) {
            prompt.DefaultValue(defaultValue);
        }

        if (validator != null) {
            prompt.Validate(input => {
                var validationResult = validator(input);
                return validationResult == null;
            });
        }

        var result = AnsiConsole.Prompt(prompt);
        return Task.FromResult<string?>(result);
    }

    /// <inheritdoc />
    public Task<string?> PromptSecretAsync(string message, Func<string, string?>? validator = null, CancellationToken cancellationToken = default) {
        // Check if running in non-interactive mode
        if (_configuration.Ui.NonInteractive) {
            return Task.FromResult<string?>(null);
        }

        var prompt = new TextPrompt<string>(message) {
            IsSecret = true,
        };

        if (validator != null) {
            prompt.Validate(input => {
                var validationResult = validator(input);
                return validationResult == null;
            });
        }

        var result = AnsiConsole.Prompt(prompt);
        return Task.FromResult<string?>(result);
    }

    /// <inheritdoc />
    public Task<string?> ShowMenuAsync(string title, Dictionary<string, string> options, CancellationToken cancellationToken = default) {
        if (!options.Any()) {
            return Task.FromResult<string?>(null);
        }

        // Check if running in non-interactive mode
        if (_configuration.Ui.NonInteractive) {
            return Task.FromResult<string?>(options.Keys.First());
        }

        var prompt = new SelectionPrompt<string>()
            .Title(title)
            .AddChoices(options.Keys)
            .UseConverter(key => options[key]);

        var result = AnsiConsole.Prompt(prompt);
        return Task.FromResult<string?>(result);
    }

    /// <inheritdoc />
    public Task<T?> SelectFromPagedListAsync<T>(string message, IEnumerable<T> items, Func<T, string> displaySelector, int pageSize = 10, CancellationToken cancellationToken = default) where T : notnull {
        var itemsList = items.ToList();
        if (!itemsList.Any()) {
            return Task.FromResult<T?>(default);
        }

        // Check if running in non-interactive mode
        if (_configuration.Ui.NonInteractive) {
            return Task.FromResult<T?>(itemsList.First());
        }

        var prompt = new SelectionPrompt<T>()
            .Title(message)
            .AddChoices(itemsList)
            .UseConverter(displaySelector)
            .PageSize(pageSize);

        var result = AnsiConsole.Prompt(prompt);
        return Task.FromResult<T?>(result);
    }

    /// <inheritdoc />
    public Task<bool> ShowConsentFlowAsync(string title, string details, IEnumerable<string> permissions, IEnumerable<string>? risks = null, CancellationToken cancellationToken = default) {
        // Check if running in non-interactive mode
        if (_configuration.Ui.NonInteractive) {
            return Task.FromResult(false);
        }

        // Display the consent information
        var panel = new Panel(BuildConsentContent(details, permissions, risks)) {
            Header = new PanelHeader(title),
            Border = BoxBorder.Rounded,
            BorderStyle = Style.Parse("yellow"),
        };

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();

        // Ask for consent
        var confirmPrompt = new ConfirmationPrompt("[yellow]Do you consent to these permissions and acknowledge the risks?[/]") {
            DefaultValue = false,
        };

        var result = AnsiConsole.Prompt(confirmPrompt);
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public async Task<T> ShowProgressAsync<T>(string message, Func<IProgress<string>, Task<T>> operation, CancellationToken cancellationToken = default) {
        if (_configuration.Ui.NonInteractive || !_configuration.Ui.ProgressBars) {
            var progress = new Progress<string>();
            return await operation(progress);
        }

        return await AnsiConsole.Progress()
            .StartAsync(async ctx => {
                var task = ctx.AddTask(message);
                var progress = new Progress<string>(status => task.Description = status);

                var result = await operation(progress);
                task.Value = 100;
                return result;
            });
    }

    /// <inheritdoc />
    public async Task<T> ShowSpinnerAsync<T>(string message, Func<Task<T>> operation, CancellationToken cancellationToken = default) => _configuration.Ui.NonInteractive || !_configuration.Ui.ProgressBars
            ? await operation()
            : await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(message, async _ => await operation());

    private static string BuildConsentContent(string details, IEnumerable<string> permissions, IEnumerable<string>? risks) {
        var content = new List<string> {
            $"[bold]Details:[/] {details}",
            "",
            "[bold]Required Permissions:[/]",
                                       };

        foreach (var permission in permissions) {
            content.Add($"  • {permission}");
        }

        if (risks?.Any() == true) {
            content.Add("");
            content.Add("[bold red]Potential Risks:[/]");
            foreach (var risk in risks) {
                content.Add($"  • [red]{risk}[/]");
            }
        }

        return string.Join("\n", content);
    }
}