using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfCLR
{
    public class PsWpfWindowContext
    {
        private ThisObject _thisObj;
        private Collection<PSObject> _output;
        private Collection<ErrorRecord> _errors;
        private Collection<WarningRecord> _warnings;
        private Collection<InformationRecord> _information;
        private Collection<VerboseRecord> _verbose;
        private Collection<DebugRecord> _debug;
        private Window _mainWindow = null;
        private Hashtable _synchronizedData = Hashtable.Synchronized(new Hashtable());

        public Collection<PSObject> Output { get { return _output; } }
        public Collection<ErrorRecord> Errors { get { return _errors; } }
        public Collection<WarningRecord> Warnings { get { return _warnings; } }
        public Collection<InformationRecord> Uinformation { get { return _information; } }
        public Collection<VerboseRecord> Verbose { get { return _verbose; } }
        public Collection<DebugRecord> Debug { get { return _debug; } }
        public Window MainWindow { get { return _mainWindow; } }
        public Hashtable SynchronizedData { get { return _synchronizedData; } }

        public class ThisObject
        {
            private PsWpfWindowContext _context;
            private PSHost _host = null;
            public Window MainWindow { get { return _context.MainWindow; } }
            public Hashtable SynchronizedData { get { return _context.SynchronizedData; } }
            private Collection<DelegationContext> _attachedEvents = new Collection<DelegationContext>();
            public ThisObject(PsWpfWindowContext context, PSHost psHost)
            {
                _context = context;
                _host = psHost;
            }

            public void AttachEvent(Type arg1, Type arg2, object source, string eventName, ScriptBlock handler, PSHost host)
            {
                Type t = (typeof(ActionDelegationContext<,>)).MakeGenericType(arg1, arg2);
                DelegationContext ctx = Activator.CreateInstance(t, handler, this, _host) as DelegationContext;
                ctx.AttachInstanceEvent(source, eventName);
                ctx.DelegateInvoked += DelegationContext_DelegateInvoked;
                _attachedEvents.Add(ctx);
            }
            public void DetachEvent(Type arg1, Type arg2, object source, string eventName)
            {
                Type t = (typeof(ActionDelegationContext<,>)).MakeGenericType(arg1, arg2);
                foreach (DelegationContext ctx in _attachedEvents)
                {
                    if (t.IsInstanceOfType(ctx))
                    {
                        ctx.DetachInstanceEvents(eventName, source);
                        ctx.DelegateInvoked -= DelegationContext_DelegateInvoked;
                        _attachedEvents.Remove(ctx);
                    }
                }
            }
            private void DelegationContext_DelegateInvoked(object sender, DelegateInvokedEventArgs e)
            {
                AddRange<PSObject>(e.Output, _context.Output);
                AddRange<ErrorRecord>(e.ErrorRecords, _context.Errors);
                AddRange<WarningRecord>(e.WarningRecords, _context.Warnings);
                AddRange<VerboseRecord>(e.VerboseRecords, _context.Verbose);
                AddRange<DebugRecord>(e.DebugRecords, _context.Debug);
            }
        }
        
        public PsWpfWindowContext(PSHost psHost)
        {
            _thisObj = new ThisObject(this, psHost);
        }

        private static void AddRange<T>(Collection<T> source, Collection<T> target)
        {
            if (source != null && target != null)
            {
                foreach (T obj in source)
                    target.Add(obj);
            }
        }
    }
}
