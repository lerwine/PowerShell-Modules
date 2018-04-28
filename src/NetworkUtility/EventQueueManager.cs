using System;
using System.Collections.Generic;
using System.Threading;

namespace NetworkUtility
{
    /// <summary>
    /// Allows raised events to be queued for redundant calls in thread-safe synchronized objects.
    /// </summary>
    public static class EventQueueManager
    {
        private static object _syncRoot = new object();
        private static Queue<Tuple<Delegate, Action<Exception, object[]>, object[]>> _queuedEvents = null;

        /// <summary>
        /// Invokes a function, which may cause events to be raised, and returns the result.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="func" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="func" />.</typeparam>
        /// <typeparam name="T3">Type of third argument to pass to <paramref name="func" />.</typeparam>
        /// <typeparam name="T4">Type of fourth argument to pass to <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">Type of value returned from <paramref name="func" />.</typeparam>
        /// <param name="arg1">Value of first argument to pass to <paramref name="func" />.</param>
        /// <param name="arg2">Value of second argument to pass to <paramref name="func" />.</param>
        /// <param name="arg3">Value of third argument to pass to <paramref name="func" />.</param>
        /// <param name="arg4">Value of fourth argument to pass to <paramref name="func" />.</param>
        /// <param name="syncRoot"The object on which to acquire an exclusive lock while <paramref name="func" /> is being invoked.</param>
        /// <param name="func">Function to be invoked, which may result in events being raised.</param>
        /// <returns><typeparamref name="TResult" /> value returned from <paramref name="func" />.</returns>
        /// <remarks>While <paramref name="func" /> is being invoked, any calls to the <see cref="EventQueueManager" /> <code>Raise</code>
        /// will result in the event handler being queued for invocation until the call to <paramref name="func" /> is finished.</remarks>
        public static TResult Get<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, object syncRoot,
            Func<T1, T2, T3, T4, TResult> func)
        {
            return (TResult)(InvokeGet(func, syncRoot, arg1, arg2, arg3, arg4));
        }

        /// <summary>
        /// Invokes a method which may cause events to be raised.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">Type of third argument to pass to <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">Type of fourth argument to pass to <paramref name="method" />.</typeparam>
        /// <param name="arg1">Value of first argument to pass to <paramref name="method" />.</param>
        /// <param name="arg2">Value of second argument to pass to <paramref name="method" />.</param>
        /// <param name="arg3">Value of third argument to pass to <paramref name="method" />.</param>
        /// <param name="arg4">Value of fourth argument to pass to <paramref name="method" />.</param>
        /// <param name="syncRoot"The object on which to acquire an exclusive lock while <paramref name="method" /> is being invoked.</param>
        /// <param name="method">Method to be invoked, which may result in events being raised.</param>
        /// <remarks>While <paramref name="method" /> is being invoked, any calls to the <see cref="EventQueueManager" /> <code>Raise</code>
        /// will result in the event handler being queued for invocation until the call to <paramref name="method" /> is finished.</remarks>
        public static void Invoke<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, object syncRoot, Action<T1, T2, T3, T4> method)
        {
            InvokeGet(method, syncRoot, arg1, arg2, arg3, arg4);
        }
        
        /// <summary>
        /// Invokes a function, which may cause events to be raised, and returns the result.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="func" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="func" />.</typeparam>
        /// <typeparam name="T3">Type of third argument to pass to <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">Type of value returned from <paramref name="func" />.</typeparam>
        /// <param name="arg1">Value of first argument to pass to <paramref name="func" />.</param>
        /// <param name="arg2">Value of second argument to pass to <paramref name="func" />.</param>
        /// <param name="arg3">Value of third argument to pass to <paramref name="func" />.</param>
        /// <param name="syncRoot">The object on which to acquire an exclusive lock while <paramref name="func" /> is being invoked.</param>
        /// <param name="func">Function to be invoked, which may result in events being raised.</param>
        /// <returns><typeparamref name="TResult" /> value returned from <paramref name="func" />.</returns>
        /// <remarks>While <paramref name="func" /> is being invoked, any calls to the <see cref="EventQueueManager" /> <code>Raise</code>
        /// will result in the event handler being queued for invocation until the call to <paramref name="func" /> is finished.</remarks>
        public static TResult Get<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3, object syncRoot, Func<T1, T2, T3, TResult> func)
        {
            return (TResult)(InvokeGet(func, syncRoot, arg1, arg2, arg3));
        }

        /// <summary>
        /// Invokes a method which may cause events to be raised.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">Type of third argument to pass to <paramref name="method" />.</typeparam>
        /// <param name="arg1">Value of first argument to pass to <paramref name="method" />.</param>
        /// <param name="arg2">Value of second argument to pass to <paramref name="method" />.</param>
        /// <param name="arg3">Value of third argument to pass to <paramref name="method" />.</param>
        /// <param name="syncRoot">The object on which to acquire an exclusive lock while <paramref name="method" /> is being invoked.</param>
        /// <param name="method">Method to be invoked, which may result in events being raised.</param>
        /// <remarks>While <paramref name="method" /> is being invoked, any calls to the <see cref="EventQueueManager" /> <code>Raise</code>
        /// will result in the event handler being queued for invocation until the call to <paramref name="method" /> is finished.</remarks>
        public static void Invoke<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3, object syncRoot, Action<T1, T2, T3> method)
        {
            InvokeGet(method, syncRoot, arg1, arg2, arg3);
        }

        /// <summary>
        /// Invokes a function, which may cause events to be raised, and returns the result.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="func" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">Type of value returned from <paramref name="func" />.</typeparam>
        /// <param name="arg1">Value of first argument to pass to <paramref name="func" />.</param>
        /// <param name="arg2">Value of second argument to pass to <paramref name="func" />.</param>
        /// <param name="syncRoot">The object on which to acquire an exclusive lock while <paramref name="func" /> is being invoked.</param>
        /// <param name="func">Function to be invoked, which may result in events being raised.</param>
        /// <returns><typeparamref name="TResult" /> value returned from <paramref name="func" />.</returns>
        /// <remarks>While <paramref name="func" /> is being invoked, any calls to the <see cref="EventQueueManager" /> <code>Raise</code>
        /// will result in the event handler being queued for invocation until the call to <paramref name="func" /> is finished.</remarks>
        public static TResult Get<T1, T2, TResult>(T1 arg1, T2 arg2, object syncRoot, Func<T1, T2, TResult> func)
        {
            return (TResult)(InvokeGet(func, syncRoot, arg1, arg2));
        }

        /// <summary>
        /// Invokes a method which may cause events to be raised.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="method" />.</typeparam>
        /// <param name="arg1">Value of first argument to pass to <paramref name="method" />.</param>
        /// <param name="arg2">Value of second argument to pass to <paramref name="method" />.</param>
        /// <param name="syncRoot">The object on which to acquire an exclusive lock while <paramref name="method" /> is being invoked.</param>
        /// <param name="method">Method to be invoked, which may result in events being raised.</param>
        /// <remarks>While <paramref name="method" /> is being invoked, any calls to the <see cref="EventQueueManager" /> <code>Raise</code>
        /// will result in the event handler being queued for invocation until the call to <paramref name="method" /> is finished.</remarks>
        public static void Invoke<T1, T2>(T1 arg1, T2 arg2, object syncRoot, Action<T1, T2> method)
        {
            InvokeGet(method, syncRoot, arg1, arg2);
        }

        /// <summary>
        /// Invokes a function, which may cause events to be raised, and returns the result.
        /// </summary>
        /// <typeparam name="T">Type of argument to pass to <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">Type of value returned from <paramref name="func" />.</typeparam>
        /// <param name="arg">Value of argument to pass to <paramref name="func" />.</param>
        /// <param name="syncRoot"The object on which to acquire an exclusive lock while <paramref name="func" /> is being invoked.</param>
        /// <param name="func">Function to be invoked, which may result in events being raised.</param>
        /// <returns><typeparamref name="TResult" /> value returned from <paramref name="func" />.</returns>
        /// <remarks>While <paramref name="func" /> is being invoked, any calls to the <see cref="EventQueueManager" /> <code>Raise</code>
        /// will result in the event handler being queued for invocation until the call to <paramref name="func" /> is finished.</remarks>
        public static TResult Get<T, TResult>(T arg, object syncRoot, Func<T, TResult> func)
        {
            return (TResult)(InvokeGet(func, syncRoot, arg));
        }

        /// <summary>
        /// Invokes a method which may cause events to be raised.
        /// </summary>
        /// <typeparam name="T">Type of argument to pass to <paramref name="method" />.</typeparam>
        /// <param name="arg">Value of argument to pass to <paramref name="method" />.</param>
        /// <param name="syncRoot">The object on which to acquire an exclusive lock while <paramref name="method" /> is being invoked.</param>
        /// <param name="method">Method to be invoked, which may result in events being raised.</param>
        /// <remarks>While <paramref name="method" /> is being invoked, any calls to the <see cref="EventQueueManager" /> <code>Raise</code>
        /// will result in the event handler being queued for invocation until the call to <paramref name="method" /> is finished.</remarks>
        public static void Invoke<T>(T arg, object syncRoot, Action<T> method) { InvokeGet(method, syncRoot, arg); }
        
        /// <summary>
        /// Invokes a function, which may cause events to be raised, and returns the result.
        /// </summary>
        /// <typeparam name="TResult">Type of value returned from <paramref name="func" />.</typeparam>
        /// <param name="syncRoot">The object on which to acquire an exclusive lock while <paramref name="func" /> is being invoked.</param>
        /// <param name="func">Function to be invoked, which may result in events being raised.</param>
        /// <returns><typeparamref name="TResult" /> value returned from <paramref name="func" />.</returns>
        /// <remarks>While <paramref name="func" /> is being invoked, any calls to the <see cref="EventQueueManager" /> <code>Raise</code>
        /// will result in the event handler being queued for invocation until the call to <paramref name="func" /> is finished.</remarks>
        public static TResult Get<TResult>(object syncRoot, Func<TResult> func) { return (TResult)(InvokeGet(func, syncRoot)); }

        /// <summary>
        /// Invokes a method which may cause events to be raised.
        /// </summary>
        /// <param name="syncRoot">The object on which to acquire an exclusive lock while <paramref name="method" /> is being invoked.</param>
        /// <param name="method">Method to be invoked, which may result in events being raised.</param>
        /// <remarks>While <paramref name="method" /> is being invoked, any calls to the <see cref="EventQueueManager" /> <code>Raise</code>
        /// will result in the event handler being queued for invocation until the call to <paramref name="method" /> is finished.</remarks>
        public static void Invoke(object syncRoot, Action method) { InvokeGet(method, syncRoot); }
        
        /// <summary>
        /// Invokes a method which may cause events to be raised.
        /// </summary>
        /// <param name="method">Method to be invoked, which may result in events being raised.</param>
        /// <param name="syncRoot">The object on which to acquire an exclusive lock while <paramref name="method" /> is being invoked.</param>
        /// <param name="args">Arguments to pass to <paramref name="method" />.</param>
        /// <returns>Value returned from <paramref name="method" />.</returns>
        /// <remarks>While <paramref name="method" /> is being invoked, any calls to the <see cref="EventQueueManager" /> <code>Raise</code>
        /// will result in the event handler being queued for invocation until the call to <paramref name="method" /> is finished.</remarks>
        public static object InvokeGet(Delegate method, object syncRoot, params object[] args)
        {
            Queue<Tuple<Delegate, Action<Exception, object[]>, object[]>> queuedEvents;
            Monitor.Enter(_syncRoot);
            try
            {
                if (_queuedEvents == null)
                {
                    queuedEvents = new Queue<Tuple<Delegate, Action<Exception, object[]>, object[]>>();
                    _queuedEvents = queuedEvents;
                }
                else
                    queuedEvents = null;
            }
            finally { Monitor.Exit(_syncRoot); }

            object result;
            try
            {
                Monitor.Enter(syncRoot);
                try { result = method.DynamicInvoke((args == null) ? new object[0] : args); }
                finally { Monitor.Exit(syncRoot); }
            }
            finally
            {
                if (queuedEvents != null)
                {
                    Monitor.Enter(_syncRoot);
                    try { _queuedEvents = null; } finally { Monitor.Exit(_syncRoot); }

                    while (queuedEvents.Count > 0)
                    {
                        Tuple<Delegate, Action<Exception, object[]>, object[]> current = queuedEvents.Dequeue();
                        try { current.Item1.DynamicInvoke(current.Item3); }
                        catch (Exception e)
                        {
                            if (current.Item2 != null)
                                try { current.Item2(e, current.Item3); } catch { }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T3">Type of third argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T4">Type of fourth argument to pass to <paramref name="handler" />.</typeparam>
        /// <param name="arg1">Value of first event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg2">Value of second event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg3">Value of third event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg4">Value of fourth event argument to pass to <paramref name="handler" />.</param>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <param name="onException">Method which gets invoked when <paramref name="handler" /> throws an exception.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action<T1, T2, T3, T4> handler,
            Action<Exception, T1, T2, T3, T4> onException)
        {
            if (onException != null)
                return RaiseHandler(handler, new Action<Exception, object[]>((e, a) => onException(e, (T1)(a[0]), (T2)(a[1]), (T3)(a[2]), (T4)(a[3]))),
                    arg1, arg2, arg3, arg4);
            return RaiseHandler(handler, null as Action<Exception, object[]>, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T3">Type of third argument to pass to <paramref name="handler" />.</typeparam>
        /// <param name="arg1">Value of first event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg2">Value of second event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg3">Value of third event argument to pass to <paramref name="handler" />.</param>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <param name="onException">Method which gets invoked when <paramref name="handler" /> throws an exception.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> handler, Action<Exception, T1, T2, T3> onException)
        {
            if (onException != null)
                return RaiseHandler(handler, new Action<Exception, object[]>((e, a) => onException(e, (T1)(a[0]), (T2)(a[1]), (T3)(a[2]))),
                    arg1, arg2, arg3);
            return RaiseHandler(handler, null as Action<Exception, object[]>, arg1, arg2, arg3);
        }
        
        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T3">Type of third argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T4">Type of fourth argument to pass to <paramref name="handler" />.</typeparam>
        /// <param name="arg1">Value of first event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg2">Value of second event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg3">Value of third event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg4">Value of fourth event argument to pass to <paramref name="handler" />.</param>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action<T1, T2, T3, T4> handler)
        {
            return Raise<T1, T2, T3, T4>(arg1, arg2, arg3, arg4, handler, null);
        }

        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="handler" />.</typeparam>
        /// <param name="arg1">Value of first event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg2">Value of second event argument to pass to <paramref name="handler" />.</param>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <param name="onException">Method which gets invoked when <paramref name="handler" /> throws an exception.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise<T1, T2>(T1 arg1, T2 arg2, Action<T1, T2> handler, Action<Exception, T1, T2> onException)
        {
            if (onException != null)
                return RaiseHandler(handler, new Action<Exception, object[]>((e, a) => onException(e, (T1)(a[0]), (T2)(a[1]))), arg1, arg2);
            return RaiseHandler(handler, null as Action<Exception, object[]>, arg1, arg2);
        }

        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <typeparam name="T">Type of argument to pass to <paramref name="handler" />.</typeparam>
        /// <param name="sender">Object which raised the event.</param>
        /// <param name="arg">Value of event argument to pass to <paramref name="handler" />.</param>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <param name="onException">Method which gets invoked when <paramref name="handler" /> throws an exception.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise<T>(object sender, T eventArgs, Action<object, T> handler, Action<Exception, object, T> onException)
            where T : EventArgs
        {
            if (onException != null)
                return RaiseHandler(handler, new Action<Exception, object[]>((e, a) => onException(e, a[0], (T)(a[1]))), sender, eventArgs);
            return RaiseHandler(handler, null as Action<Exception, object[]>, sender, eventArgs);
        }
        
        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T3">Type of third argument to pass to <paramref name="handler" />.</typeparam>
        /// <param name="arg1">Value of first event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg2">Value of second event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg3">Value of third event argument to pass to <paramref name="handler" />.</param>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> handler)
        {
            return Raise<T1, T2, T3>(arg1, arg2, arg3, handler, null);
        }

        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <typeparam name="T">Type of argument to pass to <paramref name="handler" />.</typeparam>
        /// <param name="arg">Value of event argument to pass to <paramref name="handler" />.</param>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <param name="onException">Method which gets invoked when <paramref name="handler" /> throws an exception.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise<T>(T arg, Action<T> handler, Action<Exception, T> onException)
        {
            if (onException != null)
                return RaiseHandler(handler, new Action<Exception, object[]>((e, a) => onException(e, (T)(a[0]))), arg);
            return RaiseHandler(handler, null as Action<Exception, object[]>, arg);
        }
        
        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <typeparam name="T">Type of argument to pass to <paramref name="handler" />.</typeparam>
        /// <param name="arg">Value of event argument to pass to <paramref name="handler" />.</param>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <param name="onException">Method which gets invoked when <paramref name="handler" /> throws an exception.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise(Action handler, Action<Exception> onException)
        {
            if (onException != null)
                return RaiseHandler(handler, new Action<Exception, object[]>((e, a) => onException(e)));
            return RaiseHandler(handler, null as Action<Exception, object[]>);
        }
        
        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <typeparam name="T1">Type of first argument to pass to <paramref name="handler" />.</typeparam>
        /// <typeparam name="T2">Type of second argument to pass to <paramref name="handler" />.</typeparam>
        /// <param name="arg1">Value of first event argument to pass to <paramref name="handler" />.</param>
        /// <param name="arg2">Value of second event argument to pass to <paramref name="handler" />.</param>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise<T1, T2>(T1 arg1, T2 arg2, Action<T1, T2> handler) { return Raise<T1, T2>(arg1, arg2, handler, null); }

        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <typeparam name="T">Type of argument to pass to <paramref name="handler" />.</typeparam>
        /// <param name="sender">Object which raised the event.</param>
        /// <param name="arg">Value of event argument to pass to <paramref name="handler" />.</param>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise<T>(object sender, T eventArgs, Action<object, T> handler)
            where T : EventArgs
        {
            return Raise<T>(sender, eventArgs, handler, null);
        }
        
        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <typeparam name="T">Type of argument to pass to <paramref name="handler" />.</typeparam>
        /// <param name="arg">Value of event argument to pass to <paramref name="handler" />.</param>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise<T>(T arg, Action<T> handler) { return Raise<T>(arg, handler, null); }

        /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
        public static bool Raise(Action handler) { return Raise(handler, null as Action<Exception>); }

         /// <summary>
        /// Queues a method invocation to handle an event.
        /// </summary>
        /// <param name="handler">Method which gets invoked to handle the event.</param>
        /// <param name="onException">Method which gets invoked when <paramref name="handler" /> throws an exception.</param>
        /// <param name="args">Event arguments to pass to <paramref name="handler" />.</param>
        /// <returns><code>true</code> if <paramref name="handler" /> was immediately invoked or <code>false</code>
        /// if <paramref name="handler" /> was queued for later execution.</returns>
        /// <remarks>If none of the <see cref="EventQueueManager" /> <code>Get</code> or <code>Invoke</code> methods are invoking their
        /// respective delegates, then <paramref name="handler" /> will be invoked immediately. Otherwise, it will be queued for invocation
        /// after the <code>Get</code> or <code>Invoke</code> method has finished executing the delegate.</remarks>
       public static bool RaiseHandler(Delegate handler, Action<Exception, object[]> onException, params object[] args)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");
            Monitor.Enter(_syncRoot);
            try
            {
                if (_queuedEvents != null)
                {
                    _queuedEvents.Enqueue(new Tuple<Delegate, Action<Exception, object[]>, object[]>(handler, onException, (args == null) ? new object[0] : args));
                    return false;
                }
            } finally { Monitor.Exit(_syncRoot); }
            
            object[] a = (args == null) ? new object[0] : args;
            try { handler.DynamicInvoke(a); }
            catch (Exception e)
            {
                if (onException != null)
                    try { onException(e, a); } catch { }
            }
            return true;
        }
    }
}