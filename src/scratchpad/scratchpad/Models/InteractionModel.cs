//-----------------------------------------------------------------------
// <copyright company="MoPowered">
//     Copyright (c) MoPowered. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace scratchpad.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a "result" from an interaction with a remote site. In it's rawest form, it simply returns <see cref="IsSuccessful"/> or <see cref="Errors"/>.<para/>
    /// If you want to add some data to it - check out <see cref="InteractionModel{TData}"/>.
    /// </summary>
    public class InteractionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionModel" /> class with the (optional) <paramref name="errors"/> given.
        /// </summary>
        /// <param name="errors">(Optional) Errors to include in the result.</param>
        public InteractionModel(params string[] errors)
        {
            if (errors == null || errors.Length == 0)
            {
                throw new ArgumentNullException("errors");
            }

            Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionModel" /> class with the (optional) <paramref name="error"/> given.
        /// </summary>
        /// <param name="error">(Optional) Error to include in the result.</param>
        /// <param name="responseUri">(Optional) Remote ResponseURI to include in the result.</param>
        public InteractionModel(string error, Uri responseUri = null)
        {
            if (string.IsNullOrEmpty(error))
            {
                throw new ArgumentNullException("error");
            }

            Errors = new List<string> { error };
            RemoteResponseUrl = responseUri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionModel" /> class with the (optional) <paramref name="errors"/> given.
        /// </summary>
        /// <param name="errors">(Optional) List of errors to include in the result.</param>
        /// <param name="responseUri">(Optional) Remote ResponseURI to include in the result.</param>
        public InteractionModel(IReadOnlyList<string> errors = null, Uri responseUri = null)
        {
            Errors = errors ?? new List<string>();
            RemoteResponseUrl = responseUri;
        }

        /// <summary>
        /// Gets a value indicating whether action completed with success
        /// </summary>
        public bool IsSuccessful
        {
            get { return !Errors.Any(); }
        }

        /// <summary>
        /// Gets or sets the list of errors in the result.
        /// </summary>
        public IReadOnlyList<string> Errors { get; protected set; }

        /// <summary>
        /// Gets or sets the remote response URL in the result.
        /// </summary>
        public Uri RemoteResponseUrl { get; protected set; }
    }
}