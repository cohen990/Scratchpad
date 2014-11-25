// -----------------------------------------------------------------------
// <copyright company="MoPowered">
// Copyright (c) MoPowered. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace scratchpad.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Add generic return data to the <see cref="InteractionModel"/>.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    /// <remarks>
    ///     If you want to use an <see cref="InteractionModel{TData}"/> to return 
    ///     <c>InteractionModel{TData}.Successful(null)</c> or <c>InteractionModel{TData}.Successful(null)</c> 
    ///     or anything similar to that, use <see cref="InteractionModel"/> instead.
    /// </remarks>
    public class InteractionModel<TData> : InteractionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionModel{TData}"/> class.
        /// Assumes no errors as you have data to return, so must be a success
        /// </summary>
        /// <param name="data">The data to initialize with.</param>
        /// <param name="errors">(Optional) Errors to include in the result.</param>
        /// <param name="responseUri">Optionally set the remote response uri.</param>
        private InteractionModel(TData data, IReadOnlyList<string> errors = null, Uri responseUri = null)
            : base(errors, responseUri)
        {
            Data = data;
        }

        /// <summary>
        /// Gets or sets the generic data.
        /// </summary>
        public TData Data { get; protected set; }

        /// <summary>
        /// Spins up a <see cref="InteractionModel{TData}"/> without errors.
        /// </summary>
        /// <param name="data">An instance of <typeparamref name="TData"/> which is stored in <see cref="InteractionModel{TData}"/>.</param>
        /// <param name="responseUri">An optional parameter for a responseUri of type <see cref="Uri"/>.</param>
        /// <returns>A successful <see cref="InteractionModel{TData}"/>.</returns>
        /// <remarks>
        ///     This should always return useful information. <c>string.Empty</c> and <c>null</c> are not useful. If 
        ///     you intend to return them, use <see cref="InteractionModel"/> instead.
        /// </remarks>
        public static InteractionModel<TData> Successful(TData data, Uri responseUri = null)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var result = new InteractionModel<TData>(data, responseUri: responseUri);
            return result;
        }

        /// <summary>
        ///     Spins up a failed <see cref="InteractionModel{TData}"/> with the default for 
        ///     <typeparamref name="TData"/> with errors.
        /// </summary>
        /// <param name="errors">
        ///     A <see cref="T:string[]"/> containing an errors relating to a failed 
        ///     <see cref="InteractionModel{TData}"/>.
        /// </param>
        /// <returns>A failed <see cref="InteractionModel{TData}"/>.</returns>
        public static InteractionModel<TData> Failure(params string[] errors)
        {
            if (errors == null || errors.Length == 0)
            {
                throw new ArgumentNullException("errors");
            }

            var data = default(TData);
            var result = new InteractionModel<TData>(data, errors);

            return result;
        }

        /// <summary>
        ///     Spins up a failed <see cref="InteractionModel{TData}"/> with the default for 
        ///     <typeparamref name="TData"/> with errors.
        /// </summary>
        /// <param name="error">
        ///     A <see cref="String"/> containing the error relating to a failed 
        ///     <see cref="InteractionModel{TData}"/>.
        /// </param>
        /// <param name="responseUri">An optional parameter for a responseUri of type <see cref="Uri"/>.</param>
        /// <returns>A failed <see cref="InteractionModel{TData}"/>.</returns>
        public static InteractionModel<TData> Failure(string error, Uri responseUri = null)
        {
            if (string.IsNullOrEmpty(error))
            {
                throw new ArgumentNullException("error");
            }

            var data = default(TData);
            var errors = new List<string> { error };
            var result = new InteractionModel<TData>(data, errors, responseUri);

            return result;
        }

        /// <summary>
        ///     Spins up a failed <see cref="InteractionModel{TData}"/> with the default for 
        ///     <typeparamref name="TData"/> with errors.
        /// </summary>
        /// <param name="errors">
        ///     A <see cref="IList{String}"/> containing all errors relating to a failed 
        ///     <see cref="InteractionModel{TData}"/>.
        /// </param>
        /// <param name="responseUri">An optional parameter for a responseUri of type <see cref="Uri"/>.</param>
        /// <returns>A failed <see cref="InteractionModel{TData}"/>.</returns>
        public static InteractionModel<TData> Failure(IReadOnlyList<string> errors, Uri responseUri = null)
        {
            if (errors == null || !errors.Any())
            {
                throw new ArgumentNullException("errors");
            }

            var data = default(TData);
            var result = new InteractionModel<TData>(data, errors, responseUri);

            return result;
        }
    }
}