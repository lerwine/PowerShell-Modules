namespace IOUtilityCLR
{
    /// <summary>
    /// Result information about a handled event.
    /// </summary>
    public class PSInvocationEventResult
    {
        private string _name;
        private PSInvocationEventHandlerInvokedArgs _args;
		
        /// <summary>
        /// Arbitrary name associated with <seealso cref="PSEventScriptHandler{TEventArgs}"/> which handled the event.
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Result of handling the event.
        /// </summary>
        public PSInvocationEventHandlerInvokedArgs Args { get { return _args; } }

        /// <summary>
        /// Initialize new <see cref="PSInvocationEventResult"/> object.
        /// </summary>
        /// <param name="name">Arbitrary name associated with <seealso cref="PSEventScriptHandler{TEventArgs}"/> which handled the event.</param>
        /// <param name="args">Result of handling the event.</param>
        public PSInvocationEventResult(string name, PSInvocationEventHandlerInvokedArgs args)
        {
            _name = name;
            _args = args;
        }
    }
}
