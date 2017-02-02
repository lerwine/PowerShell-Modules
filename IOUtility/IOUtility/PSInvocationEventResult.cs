namespace IOUtilityCLR
{
    public class PSInvocationEventResult
    {
        private string _name;
        private PSInvocationEventHandlerInvokedArgs _args;
        public string Name { get { return _name; } }
        public PSInvocationEventHandlerInvokedArgs Args { get { return _args; } }
        public PSInvocationEventResult(string name, PSInvocationEventHandlerInvokedArgs args)
        {
            _name = name;
            _args = args;
        }
    }
}
