﻿#region Header

// <copyright file="SafeGCHandle.cs" company="Nito Programs">
//     Copyright (c) 2009 Nito Programs.
// </copyright>

#endregion Header

using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace TRock.FileReplicator.Commands
{
    /// <summary>
    /// Helper class to help with managing <see cref="GCHandle"/> resources.
    /// </summary>
    /// <remarks>
    /// <para>The only reason this isn't <c>public</c> is to prevent misuse by end users.</para>
    /// <para>Note that this class can only be used to represent <see cref="GCHandle"/> objects that should be freed when garbage collected (or disposed). This class cannot be used in several interop situations, such as passing ownership of an object to a callback function.</para>
    /// </remarks>
    internal sealed class SafeGCHandle : SafeHandle
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeGCHandle"/> class referring to the target in the given way.
        /// </summary>
        /// <param name="target">The object to reference.</param>
        /// <param name="type">The way to reference the object.</param>
        public SafeGCHandle(object target, GCHandleType type)
            : base(IntPtr.Zero, true)
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                // The StyleCop warning for this try block is incorrect; it is required to create a Constrained Execution Region
            }
            finally
            {
                this.SetHandle(GCHandle.ToIntPtr(GCHandle.Alloc(target, type)));
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the underlying allocated garbage collection handle.
        /// </summary>
        public GCHandle Handle
        {
            get
            {
                return GCHandle.FromIntPtr(this.handle);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the handle value is invalid.
        /// </summary>
        public override bool IsInvalid
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [PrePrepareMethod]
            get { return this.handle == IntPtr.Zero; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Frees the garbage collection handle.
        /// </summary>
        /// <returns>Whether the handle was released successfully.</returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [PrePrepareMethod]
        protected override bool ReleaseHandle()
        {
            GCHandle.FromIntPtr(this.handle).Free();
            return true;
        }

        #endregion Methods
    }
}