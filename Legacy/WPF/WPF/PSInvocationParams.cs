using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace WpfCLR
{
    public class PSInvocationParams
    {
        public PSInvocationParams()
        {
            Configuration = RunspaceConfiguration.Create();
            Variables = new Hashtable();
        }
        public string Location { get; set; }
        public bool? UseLocalScope { get; set; }
        public PSHost Host { get; set; }
        public ApartmentState? ApartmentState { get; set; }
        public PSThreadOptions? ThreadOptions { get; set; }
        public RunspaceConfiguration Configuration { get; private set; }
        public Hashtable Variables { get; private set; }
        public PSObject This { get; set; }
    }
}