using MCPHub.Domain.Common;
using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Domain.Services;

/// <summary>
/// Application service for package management operations
/// </summary>
/// <param name="unitOfWork">Unit of work for database operations</param>
public class PackageService(IUnitOfWork unitOfWork) : IPackageService {
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <inheritdoc />
    public async Task<IEnumerable<Package>> GetAllPackagesAsync(CancellationToken cancellationToken = default) => await _unitOfWork.Packages.GetAllAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Package?> GetPackageByIdAsync(Guid id, CancellationToken cancellationToken = default) => await _unitOfWork.Packages.GetByIdAsync(id, cancellationToken);

    /// <inheritdoc />
    public async Task<Package?> GetPackageByNameAsync(string name, CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _unitOfWork.Packages.GetByNameAsync(name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Package>> GetPackagesByPublisherAsync(Guid publisherId, CancellationToken cancellationToken = default) => await _unitOfWork.Packages.GetByPublisherIdAsync(publisherId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Package>> SearchPackagesAsync(string query, int pageSize = 20, int pageIndex = 0, CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        return await _unitOfWork.Packages.SearchAsync(query, pageIndex, pageSize, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SearchResult<Package>> SearchPackagesAsync(SearchRequest request, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(request);

        return !request.IsValid(out var errorMessage)
            ? throw new ArgumentException(errorMessage, nameof(request))
            : await _unitOfWork.Packages.SearchAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Package> CreatePackageAsync(Package package, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(package);

        // Add audit trail entry for package creation
        package.AuditTrail.Add(new AuditEntry {
            Action = "Package Created via PackageService",
            UserId = Guid.Empty, // System action for now
            DateTime = DateTimeOffset.UtcNow,
        });

        var createdPackage = await _unitOfWork.Packages.AddAsync(package, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return createdPackage;
    }

    /// <inheritdoc />
    public async Task<Package> UpdatePackageAsync(Package package, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(package);

        // Add audit trail entry for package update
        package.AuditTrail.Add(new AuditEntry {
            Action = "Package Updated via PackageService",
            UserId = Guid.Empty, // System action for now
            DateTime = DateTimeOffset.UtcNow,
        });

        var updatedPackage = await _unitOfWork.Packages.UpdateAsync(package, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return updatedPackage;
    }

    /// <inheritdoc />
    public async Task<bool> DeletePackageAsync(Guid id, CancellationToken cancellationToken = default) {
        var package = await _unitOfWork.Packages.GetByIdAsync(id, cancellationToken);
        if (package == null) {
            return false;
        }

        // Add audit trail entry for package deletion
        package.AuditTrail.Add(new AuditEntry {
            Action = "Package Deleted via PackageService",
            UserId = Guid.Empty, // System action for now
            DateTime = DateTimeOffset.UtcNow,
        });

        await _unitOfWork.Packages.DeleteAsync(package, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}