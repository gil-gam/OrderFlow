using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    /// <summary>
    /// Provides data access operations for <see cref="Customer"/> entities.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Gets a customer by its unique identifier.
        /// </summary>
        /// <param name="id">The customer identifier.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The customer if found; otherwise, null.</returns>
        Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);

        /// <summary>
        /// Gets a customer by the associated user external identifier.
        /// </summary>
        /// <param name="userExternalId">The external user identifier.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The customer if found; otherwise, null.</returns>
        Task<Customer?> GetByUserExternalIdAsync(Guid userExternalId, CancellationToken ct);

        /// <summary>
        /// Gets all customers.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A collection of all customers.</returns>
        Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct);

        /// <summary>
        /// Adds a new customer to the repository.
        /// </summary>
        /// <param name="customer">The customer to add.</param>
        /// <param name="ct">The cancellation token.</param>
        Task AddAsync(Customer customer, CancellationToken ct);

        /// <summary>
        /// Updates an existing customer.
        /// </summary>
        /// <param name="customer">The customer to update.</param>
        void Update(Customer customer);

        /// <summary>
        /// Saves all pending changes to the data store.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        Task SaveChangesAsync(CancellationToken ct);
    }
}