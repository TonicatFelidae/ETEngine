using UnityEngine;
namespace ETEngine.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace Game.Services
    {
        /// <summary>
        /// A generic, reusable economy service interface suitable for various game projects.
        /// Handles balance queries, modifications, transaction checks, and events for multiple currencies.
        /// </summary>
        public interface IEconomyService
        {
            /// <summary>
            /// Occurs when any currency balance is changed.
            /// Parameters: 
            /// 1. Currency ID (string)
            /// 2. New Balance (long)
            /// 3. Delta/Change amount (long)
            /// </summary>
            event Action<string, long, long> OnBalanceChanged;

            /// <summary>
            /// Gets the current balance of a specific currency.
            /// </summary>
            /// <param name="currencyId">The unique identifier of the currency (e.g., "Gold", "Gem", "Coin").</param>
            /// <returns>The current balance amount.</returns>
            long GetBalance(string currencyId);

            /// <summary>
            /// Gets the current balance of a specific currency asynchronously.
            /// Useful if the balance needs to be fetched from a remote server or local disk.
            /// </summary>
            Task<long> GetBalanceAsync(string currencyId);

            /// <summary>
            /// Checks if the player has at least the specified amount of a currency.
            /// </summary>
            bool CanAfford(string currencyId, long amount);

            /// <summary>
            /// Checks if the player can afford multiple costs at once.
            /// </summary>
            /// <param name="costs">A dictionary mapping currency IDs to their required amounts.</param>
            bool CanAfford(IReadOnlyDictionary<string, long> costs);

            /// <summary>
            /// Checks if the player has at least the specified amount of a currency asynchronously.
            /// </summary>
            Task<bool> CanAffordAsync(string currencyId, long amount);

            /// <summary>
            /// Checks if the player can afford multiple costs at once asynchronously.
            /// </summary>
            Task<bool> CanAffordAsync(IReadOnlyDictionary<string, long> costs);

            /// <summary>
            /// Adds a specified amount of currency to the player's balance.
            /// </summary>
            /// <param name="currencyId">The unique identifier of the currency.</param>
            /// <param name="amount">The amount to add (must be non-negative).</param>
            /// <param name="reason">An optional description of why the currency was added (e.g., "DailyReward", "QuestComplete").</param>
            /// <returns>The updated balance.</returns>
            long AddBalance(string currencyId, long amount, string reason = null);

            /// <summary>
            /// Adds a specified amount of currency asynchronously.
            /// </summary>
            Task<long> AddBalanceAsync(string currencyId, long amount, string reason = null);

            /// <summary>
            /// Deducts a specified amount of currency from the player's balance.
            /// </summary>
            /// <param name="currencyId">The unique identifier of the currency.</param>
            /// <param name="amount">The amount to deduct (must be non-negative).</param>
            /// <param name="reason">An optional description of why the currency was deducted (e.g., "PurchaseItem", "GachaPull").</param>
            /// <returns>The updated balance.</returns>
            /// <exception cref="InvalidOperationException">Thrown if the player cannot afford the amount.</exception>
            long SpendBalance(string currencyId, long amount, string reason = null);

            /// <summary>
            /// Deducts a specified amount of currency asynchronously.
            /// </summary>
            Task<long> SpendBalanceAsync(string currencyId, long amount, string reason = null);

            /// <summary>
            /// Atomically checks if the player can afford the amount and deducts it if they can.
            /// </summary>
            /// <returns>True if the transaction succeeded (affordable and deducted), false otherwise.</returns>
            bool TrySpendBalance(string currencyId, long amount, string reason = null);

            /// <summary>
            /// Atomically checks and deducts multiple currency costs if they are affordable.
            /// </summary>
            /// <returns>True if all costs were affordable and deducted, false otherwise.</returns>
            bool TrySpendBalance(IReadOnlyDictionary<string, long> costs, string reason = null);

            /// <summary>
            /// Atomically checks and deducts currency asynchronously.
            /// </summary>
            Task<bool> TrySpendBalanceAsync(string currencyId, long amount, string reason = null);

            /// <summary>
            /// Atomically checks and deducts multiple currency costs asynchronously.
            /// </summary>
            Task<bool> TrySpendBalanceAsync(IReadOnlyDictionary<string, long> costs, string reason = null);

            /// <summary>
            /// Directly sets the balance of a specific currency (e.g. during server synchronization).
            /// </summary>
            void SetBalance(string currencyId, long amount, string reason = null);

            /// <summary>
            /// Directly sets the balance of a specific currency asynchronously.
            /// </summary>
            Task SetBalanceAsync(string currencyId, long amount, string reason = null);
        }
    }

}
