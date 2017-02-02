using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfCLR
{
    /// <summary>
    /// PowerShell script delegation for delegates that take no parameters and return a value.
    /// </summary>
    /// <typeparam name="TResult">Type of value returned by delegate.</typeparam>
    public class FunctionDelegateContext<TResult> : DelegationContext
    {
        /// <summary>
        /// Extracts result value from collection.
        /// </summary>
        /// <param name="collection">Collection from which to extract result value.</param>
        /// <returns>Result value extracted from collection.</returns>
        public static TResult GetResult(Collection<PSObject> collection)
        {
            if (collection != null)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    object obj;
                    if (collection[i] != null && (obj = collection[i].BaseObject) != null && obj is TResult)
                    {
                        collection.RemoveAt(i);
                        return (TResult)obj;
                    }
                }
            }
            return default(TResult);
        }

        /// <summary>
        /// Defines delegate handler supported by <see cref="FunctionDelegateContext{TResult}"/>.
        /// </summary>
        /// <returns>Value returned from <see cref="Handler()"/>.</returns>
        public delegate TResult FunctionDelegate();

        /// <summary>
        /// Handler method for delegate.
        /// </summary>
        /// <returns><typeparamref name="TResult"/> value returned by inner <seealso cref="ScriptBlock"/>.</returns>
        public TResult Handler()
        {
            DelegateInvokedEventArgs args = InvokeScriptBlock();
            TResult result = GetResult(args.Output);
            RaiseDelegateInvoked(args);
            return result;
        }

        /// <summary>
        /// Create delegate method to handle event.
        /// </summary>
        /// <returns><seealso cref="FunctionDelegate"/> which will handle the event.</returns>
        protected override Delegate CreateHandlerDelegate() { return new FunctionDelegate(Handler); }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, PSHost host, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, host, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, PSHost host) : base(handler, thisObject, host) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="thisObject"></param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject) : base(handler, thisObject) { }
    }
    public class FunctionDelegateContext<TArg, TResult> : DelegationContext
    {
        /// <summary>
        /// Defines delegate handler supported by <see cref="FunctionDelegateContext{TArg, TResult}"/>.
        /// </summary>
        /// <param name="arg">Argument to pass to <see cref="Handler(TArg)"/>.</param>
        /// <returns>Value returned from <see cref="Handler(TArg)"/>.</returns>
        public delegate TResult FunctionDelegate(TArg arg);

        /// <summary>
        /// Handler method for delegate.
        /// </summary>
        /// <param name="arg">Argument to pass to inner <seealso cref="ScriptBlock"/>.</param>
        /// <returns><typeparamref name="TResult"/> value returned by inner <seealso cref="ScriptBlock"/>.</returns>
        public TResult Handler(TArg arg)
        {
            DelegateInvokedEventArgs args = InvokeScriptBlock(arg);
            TResult result = FunctionDelegateContext<TResult>.GetResult(args.Output);
            RaiseDelegateInvoked(args);
            return result;
        }

        /// <summary>
        /// Create delegate method to handle event.
        /// </summary>
        /// <returns><seealso cref="FunctionDelegate"/> which will handle the event.</returns>
        protected override Delegate CreateHandlerDelegate() { return new FunctionDelegate(Handler); }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, PSHost host, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, host, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, PSHost host) : base(handler, thisObject, host) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject) : base(handler, thisObject) { }
    }
    public class FunctionDelegateContext<TArg1, TArg2, TResult> : DelegationContext
    {
        /// <summary>
        /// Defines delegate handler supported by <see cref="FunctionDelegateContext{TArg1, TArg2, TResult}"/>.
        /// </summary>
        /// <param name="arg1">First argument to pass to <see cref="Handler(TArg1, TArg2)"/>.</param>
        /// <param name="arg2">Second argument to pass to <see cref="Handler(TArg1, TArg2)"/>.</param>
        /// <returns>Value returned from <see cref="Handler(TArg1, TArg2)"/>.</returns>
        public delegate TResult FunctionDelegate(TArg1 arg1, TArg2 arg2);
        
        /// <summary>
        /// Handler method for delegate.
        /// </summary>
        /// <param name="arg1">First argument to pass to inner <seealso cref="ScriptBlock"/>.</param>
        /// <param name="arg2">Second argument to pass to inner <seealso cref="ScriptBlock"/>.</param>
        /// <returns><typeparamref name="TResult"/> value returned by inner <seealso cref="ScriptBlock"/>.</returns>
        public TResult Handler(TArg1 arg1, TArg2 arg2)
        {
            DelegateInvokedEventArgs args = InvokeScriptBlock(arg1, arg2);
            TResult result = FunctionDelegateContext<TResult>.GetResult(args.Output);
            RaiseDelegateInvoked(args);
            return result;
        }

        /// <summary>
        /// Create delegate method to handle event.
        /// </summary>
        /// <returns><seealso cref="FunctionDelegate"/> which will handle the event.</returns>
        protected override Delegate CreateHandlerDelegate() { return new FunctionDelegate(Handler); }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg1, TArg2, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, PSHost host, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, host, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg1, TArg2, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg1, TArg2, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, PSHost host) : base(handler, thisObject, host) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg1, TArg2, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject) : base(handler, thisObject) { }
    }
    public class FunctionDelegateContext<TArg1, TArg2, TArg3, TResult> : DelegationContext
    {
        /// <summary>
        /// Defines delegate handler supported by <see cref="FunctionDelegateContext{TArg1, TArg2, TArg3, TResult}"/>.
        /// </summary>
        /// <param name="arg1">First argument to pass to <see cref="Handler(TArg1, TArg2, TArg3)"/>.</param>
        /// <param name="arg2">Second argument to pass to <see cref="Handler(TArg1, TArg2, TArg3)"/>.</param>
        /// <param name="arg3">Third argument to pass to <see cref="Handler(TArg1, TArg2, TArg3)"/>.</param>
        /// <returns>Value returned from <see cref="Handler(TArg1, TArg2, TArg3)"/>.</returns>
        public delegate TResult FunctionDelegate(TArg1 arg1, TArg2 arg2, TArg3 arg3);

        /// <summary>
        /// Handler method for delegate.
        /// </summary>
        /// <param name="arg1">First argument to pass to inner <seealso cref="ScriptBlock"/>.</param>
        /// <param name="arg2">Second argument to pass to inner <seealso cref="ScriptBlock"/>.</param>
        /// <param name="arg3">Third argument to pass to inner <seealso cref="ScriptBlock"/>.</param>
        /// <returns><typeparamref name="TResult"/> value returned by inner <seealso cref="ScriptBlock"/>.</returns>
        public TResult Handler(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            DelegateInvokedEventArgs args = InvokeScriptBlock(arg1, arg2, arg3);
            TResult result = FunctionDelegateContext<TResult>.GetResult(args.Output);
            RaiseDelegateInvoked(args);
            return result;
        }

        /// <summary>
        /// Create delegate method to handle event.
        /// </summary>
        /// <returns><seealso cref="FunctionDelegate"/> which will handle the event.</returns>
        protected override Delegate CreateHandlerDelegate() { return new FunctionDelegate(Handler); }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg1, TArg2, TArg3, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, PSHost host, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, host, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg1, TArg2, TArg3, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg1, TArg2, TArg3, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject, PSHost host) : base(handler, thisObject, host) { }

        /// <summary>
        /// Initialize new <see cref="FunctionDelegateContext{TArg1, TArg2, TArg3, TResult}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        public FunctionDelegateContext(ScriptBlock handler, object thisObject) : base(handler, thisObject) { }
    }
}
