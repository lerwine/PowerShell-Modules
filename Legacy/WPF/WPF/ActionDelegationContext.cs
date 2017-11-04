using System;
using System.Collections.Generic;
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
    /// PowerShell script delegation for delegates that do not return a value and take no parameters.
    /// </summary>
    public class ActionDelegationContext : DelegationContext
    {
        /// <summary>
        /// Defines delegate handler supported by <see cref="ActionDelegationContext"/>.
        /// </summary>
        public delegate void ActionDelegate();

        /// <summary>
        /// Handler method for delegate.
        /// </summary>
        public void Handler() { RaiseDelegateInvoked(InvokeScriptBlock()); }

        /// <summary>
        /// Create delegate method to handle event.
        /// </summary>
        /// <returns><seealso cref="ActionDelegate"/> which will handle the event.</returns>
        protected override Delegate CreateHandlerDelegate() { return new ActionDelegate(Handler); }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, PSHost host, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, host, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, PSHost host) : base(handler, thisObject, host) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject) : base(handler, thisObject) { }
    }

    /// <summary>
    /// PowerShell script delegation for delegates that do not return a value and takes 1 parameter.
    /// </summary>
    /// <typeparam name="T">Type of parameter passed to the supported delegate.</typeparam>
    public class ActionDelegationContext<T> : DelegationContext
    {
        /// <summary>
        /// Defines delegate handler supported by <see cref="ActionDelegationContext{T}"/>.
        /// </summary>
        /// <param name="arg">Argument to pass to <see cref="Handler(T)"/></param>
        public delegate void ActionDelegate(T arg);

        /// <summary>
        /// Handler method for delegate.
        /// </summary>
        /// <param name="arg">Argument to pass to the inner <seealso cref="ScriptBlock"/>.</param>
        public void Handler(T arg) { RaiseDelegateInvoked(InvokeScriptBlock(arg)); }

        /// <summary>
        /// Create delegate method to handle event.
        /// </summary>
        /// <returns><seealso cref="ActionDelegate"/> which will handle the event.</returns>
        protected override Delegate CreateHandlerDelegate() { return new ActionDelegate(Handler); }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{T}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, PSHost host, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, host, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{T}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{T}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, PSHost host) : base(handler, thisObject, host) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{T}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject) : base(handler, thisObject) { }
    }

    /// <summary>
    /// PowerShell script delegation for delegates that do not return a value and takes 2 parameters.
    /// </summary>
    /// <typeparam name="TArg1">Type of first parameter passed to the supported delegate.</typeparam>
    /// <typeparam name="TArg2">Type of second parameter passed to the supported delegate.</typeparam>
    public class ActionDelegationContext<TArg1, TArg2> : DelegationContext
    {
        /// <summary>
        /// Defines delegate handler supported by <see cref="ActionDelegationContext{TArg1, TArg}"/>.
        /// </summary>
        /// <param name="arg1">First argument to pass to <see cref="Handler(TArg1, TArg2)"/>.</param>
        /// <param name="arg2">Second argument to pass to <see cref="Handler(TArg1, TArg2)"/>.</param>
        public delegate void ActionDelegate(TArg1 arg1, TArg2 arg2);

        /// <summary>
        /// Handler method for delegate.
        /// </summary>
        /// <param name="arg1">First argument to pass to the inner <seealso cref="ScriptBlock"/>.</param>
        /// <param name="arg2">Second argument to pass to the inner <seealso cref="ScriptBlock"/>.</param>
        public void Handler(TArg1 arg1, TArg2 arg2) { RaiseDelegateInvoked(InvokeScriptBlock(arg1, arg2)); }

        /// <summary>
        /// Create delegate method to handle event.
        /// </summary>
        /// <returns><seealso cref="ActionDelegate"/> which will handle the event.</returns>
        protected override Delegate CreateHandlerDelegate() { return new ActionDelegate(Handler); }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{TArg1, TARg2}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, PSHost host, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, host, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{TArg1, TARg2}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{TArg1, TARg2}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, PSHost host) : base(handler, thisObject, host) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{TArg1, TARg2}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject) : base(handler, thisObject) { }
    }

    /// <summary>
    /// PowerShell script delegation for delegates that do not return a value and takes 2 parameters.
    /// </summary>
    /// <typeparam name="TArg1">Type of first parameter passed to the supported delegate.</typeparam>
    /// <typeparam name="TArg2">Type of second parameter passed to the supported delegate.</typeparam>
    /// <typeparam name="TArg3">Type of third parameter passed to the supported delegate.</typeparam>
    public class ActionDelegationContext<TArg1, TArg2, TArg3> : DelegationContext
    {
        /// <summary>
        /// Defines delegate handler supported by <see cref="ActionDelegationContext{TArg1, TArg, TArg3}"/>.
        /// </summary>
        /// <param name="arg1">First argument to pass to <see cref="Handler(TArg1, TArg2, TArg3)"/>.</param>
        /// <param name="arg2">Second argument to pass to <see cref="Handler(TArg1, TArg2, TArg3)"/>.</param>
        /// <param name="arg3">Third argument to pass to <see cref="Handler(TArg1, TArg2, TArg3)"/>.</param>
        public delegate void ActionDelegate(TArg1 arg1, TArg2 arg2, TArg3 arg3);

        /// <summary>
        /// Handler method for delegate.
        /// </summary>
        /// <param name="arg1">First argument to pass to the inner <seealso cref="ScriptBlock"/>.</param>
        /// <param name="arg2">Second argument to pass to the inner <seealso cref="ScriptBlock"/>.</param>
        /// <param name="arg3">Third argument to pass to the inner <seealso cref="ScriptBlock"/>.</param>
        public void Handler(TArg1 arg1, TArg2 arg2, TArg3 arg3) { RaiseDelegateInvoked(InvokeScriptBlock(arg1, arg2, arg3)); }

        /// <summary>
        /// Create delegate method to handle event.
        /// </summary>
        /// <returns><seealso cref="ActionDelegate"/> which will handle the event.</returns>
        protected override Delegate CreateHandlerDelegate() { return new ActionDelegate(Handler); }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{TArg1, TARg2, TArg3}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, PSHost host, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, host, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{TArg1, TARg2, TArg3}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="apartmentState">Apartment state to use when invoking <paramref name="handler"/>.</param>
        /// <param name="threadOptions">Thread options to use when invoking <paramref name="handler"/>.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, ApartmentState apartmentState, PSThreadOptions threadOptions) : base(handler, thisObject, apartmentState, threadOptions) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{TArg1, TARg2, TArg3}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        /// <param name="host">PowerShell host to use.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject, PSHost host) : base(handler, thisObject, host) { }

        /// <summary>
        /// Initialize new <see cref="ActionDelegationContext{TArg1, TARg2, TArg3}"/> object.
        /// </summary>
        /// <param name="handler"><seealso cref="ScriptBlock"/> which will handle the delegate invocation.</param>
        /// <param name="thisObject">Object which becomes the &quot;$this&quot; variable when the <paramref name="handler"/> is invoked.</param>
        public ActionDelegationContext(ScriptBlock handler, object thisObject) : base(handler, thisObject) { }
    }
}
