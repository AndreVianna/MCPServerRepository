namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Service interface for handling interactive user prompts and confirmations
/// </summary>
public interface IInteractionService {
    /// <summary>
    /// Prompts the user for a yes/no confirmation
    /// </summary>
    /// <param name="message">The confirmation message to display</param>
    /// <param name="defaultValue">The default value if user presses Enter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user confirms, false otherwise</returns>
    Task<bool> ConfirmAsync(string message, bool defaultValue = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prompts the user to select from a list of options
    /// </summary>
    /// <typeparam name="T">The type of options</typeparam>
    /// <param name="message">The prompt message</param>
    /// <param name="options">List of available options</param>
    /// <param name="displaySelector">Function to convert option to display string</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The selected option</returns>
    Task<T?> SelectFromListAsync<T>(string message, IEnumerable<T> options, Func<T, string> displaySelector, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prompts the user to select multiple items from a list
    /// </summary>
    /// <typeparam name="T">The type of options</typeparam>
    /// <param name="message">The prompt message</param>
    /// <param name="options">List of available options</param>
    /// <param name="displaySelector">Function to convert option to display string</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The selected options</returns>
    Task<IEnumerable<T>> SelectMultipleAsync<T>(string message, IEnumerable<T> options, Func<T, string> displaySelector, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prompts the user for text input
    /// </summary>
    /// <param name="message">The prompt message</param>
    /// <param name="defaultValue">Default value if user presses Enter</param>
    /// <param name="validator">Optional validation function</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user input</returns>
    Task<string?> PromptAsync(string message, string? defaultValue = null, Func<string, string?>? validator = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prompts the user for sensitive text input (e.g., passwords)
    /// </summary>
    /// <param name="message">The prompt message</param>
    /// <param name="validator">Optional validation function</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user input</returns>
    Task<string?> PromptSecretAsync(string message, Func<string, string?>? validator = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Displays a menu and prompts the user to select an option
    /// </summary>
    /// <param name="title">The menu title</param>
    /// <param name="options">Dictionary of option keys to display text</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The selected option key</returns>
    Task<string?> ShowMenuAsync(string title, Dictionary<string, string> options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a paged selection from a large list of items
    /// </summary>
    /// <typeparam name="T">The type of items</typeparam>
    /// <param name="message">The prompt message</param>
    /// <param name="items">List of items to page through</param>
    /// <param name="displaySelector">Function to convert item to display string</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The selected item</returns>
    Task<T?> SelectFromPagedListAsync<T>(string message, IEnumerable<T> items, Func<T, string> displaySelector, int pageSize = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a consent flow with detailed information
    /// </summary>
    /// <param name="title">The consent title</param>
    /// <param name="details">Detailed information about what user is consenting to</param>
    /// <param name="permissions">List of permissions being requested</param>
    /// <param name="risks">List of potential risks</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user consents, false otherwise</returns>
    Task<bool> ShowConsentFlowAsync(string title, string details, IEnumerable<string> permissions, IEnumerable<string>? risks = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a progress dialog while performing an operation
    /// </summary>
    /// <typeparam name="T">The return type of the operation</typeparam>
    /// <param name="message">The progress message</param>
    /// <param name="operation">The operation to perform</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the operation</returns>
    Task<T> ShowProgressAsync<T>(string message, Func<IProgress<string>, Task<T>> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a spinner for indeterminate progress
    /// </summary>
    /// <typeparam name="T">The return type of the operation</typeparam>
    /// <param name="message">The spinner message</param>
    /// <param name="operation">The operation to perform</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the operation</returns>
    Task<T> ShowSpinnerAsync<T>(string message, Func<Task<T>> operation, CancellationToken cancellationToken = default);
}