namespace WpfCLR.PSInvocation
{
    public class EventResult
    {
        public string Name { get; private set; }
        public EventHandlerInvokedArgs Args { get; private set; }
        public EventResult(string name, EventHandlerInvokedArgs args)
        {
            Name = name;
            Args = args;
        }
    }
}