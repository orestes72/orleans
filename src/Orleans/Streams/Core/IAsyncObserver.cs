/*
Project Orleans Cloud Service SDK ver. 1.0
 
Copyright (c) Microsoft Corporation
 
All rights reserved.
 
MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the ""Software""), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Streams
{
    /// <summary>
    /// This interface generalizes the standard .NET IObserver interface to allow asynchronous production of items.
    /// <para>
    /// Note that this interface is implemented by item consumers and invoked (used) by item producers.
    /// This means that the producer endpoint of a stream implements this interface.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of object consumed by the observer.</typeparam>
    public interface IAsyncObserver<in T>
    {
        /// <summary>
        /// Passes the next item to the consumer.
        /// <para>
        /// The Task returned from this method should be completed when the item's processing has been
        /// sufficiently processed by the consumer to meet any behavioral guarantees.
        /// </para>
        /// <para>
        /// When the consumer is the (producer endpoint of) a stream, the Task is completed when the stream implementation
        /// has accepted responsibility for the item and is assured of meeting its delivery guarantees.
        /// For instance, a stream based on a durable queue would complete the Task when the item has been durably saved.
        /// A stream that provides best-effort at most once delivery would return a Task that is already complete.
        /// </para>
        /// <para>
        /// When the producer is the (consumer endpoint of) a stream, the Task should be completed by the consumer code
        /// when it has accepted responsibility for the item. 
        /// In particular, if the stream provider guarantees at-least-once delivery, then the item should not be considered
        /// delivered until the Task returned by the consumer has been completed.
        /// </para>
        /// </summary>
        /// <param name="item">The item to be passed.</param>
        /// <param name="token">The stream sequence token of this item.</param>
        /// <returns>A Task that is completed when the item has been accepted.</returns>
        Task OnNextAsync(T item, StreamSequenceToken token = null);

        /// <summary>
        /// Notifies the consumer that the stream was completed.
        /// <para>
        /// The Task returned from this method should be completed when the consumer is done processing the stream closure.
        /// </para>
        /// </summary>
        /// <returns>A Task that is completed when the stream-complete operation has been accepted.</returns>
        Task OnCompletedAsync();

        /// <summary>
        /// Notifies the consumer that the stream had an error.
        /// <para>
        /// The Task returned from this method should be completed when the consumer is done processing the stream closure.
        /// </para>
        /// </summary>
        /// <param name="ex">An Exception that describes the error that occured on the stream.</param>
        /// <returns>A Task that is completed when the close has been accepted.</returns>
        Task OnErrorAsync(Exception ex);
    }

    /// <summary>
    /// This interface generalizes the IAsyncObserver interface to allow production and consumption of batches of items.
    /// <para>
    /// Note that this interface is implemented by item consumers and invoked (used) by item producers.
    /// This means that the producer endpoint of a stream implements this interface.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of object consumed by the observer.</typeparam>
    public interface IAsyncBatchObserver<in T> : IAsyncObserver<T>
    {
        /// <summary>
        /// Passes the next batch of items to the consumer.
        /// <para>
        /// The Task returned from this method should be completed when all items in the batch have been
        /// sufficiently processed by the consumer to meet any behavioral guarantees.
        /// </para>
        /// <para>
        /// That is, the semantics of the returned Task is the same as for <code>OnNextAsync</code>,
        /// extended for all items in the batch.
        /// </para>
        /// </summary>
        /// <param name="batch">The items to be passed.</param>
        /// <param name="token">The stream sequence token of this batch of items.</param>
        /// <returns>A Task that is completed when the batch has been accepted.</returns>
        Task OnNextBatchAsync(IEnumerable<T> batch, StreamSequenceToken token = null);
    }
}