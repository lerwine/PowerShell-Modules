using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IOUtilityCLR
{
    /// <summary>
    /// Result information about a handled event.
    /// </summary>
    public class PSInvocationEventResult
    {
        /// <summary>
        /// Arbitrary name associated with <seealso cref="PSEventScriptHandler{TEventArgs}"/> which handled the event.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Result of handling the event.
        /// </summary>
        public PSInvocationEventHandlerInvokedArgs Args { get; private set; }

        /// <summary>
        /// Initialize new <see cref="PSInvocationEventResult"/> object.
        /// </summary>
        /// <param name="name">Arbitrary name associated with <seealso cref="PSEventScriptHandler{TEventArgs}"/> which handled the event.</param>
        /// <param name="args">Result of handling the event.</param>
        public PSInvocationEventResult(string name, PSInvocationEventHandlerInvokedArgs args)
        {
            Name = name;
            Args = args;
        }
    }
}
