using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace PersonalEventTracking
{
    /// <summary>
    /// Defines an task
    /// </summary>
    public class TaskDefinition : EventDefinitionBase
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public event EventHandler<TaskStateChangingEventArgs> StateChanging;
        public event EventHandler<TaskStateChangeEventArgs> StateChanged;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private TaskState _state = TaskState.NotStarted;
        private bool _hasOpenDependencies = false;
        private Collection<TaskDefinition> _innerDependencies = new Collection<TaskDefinition>();
        private ReadOnlyCollection<TaskDefinition> _dependencies = null;

        /// <summary>
        /// True if any of the <see cref="Dependencies" /> have a <see cref="TaskDefinition.State" /> value that is
        /// not <seealso cref="TaskState.Completed" /> or <seealso cref="TaskState.Canceled" />. 
        /// </summary>
        public bool HasOpenDependencies { get { return _hasOpenDependencies; } }

        /// <summary>
        /// The state of the current <see cref="TaskDefinition" />.
        /// </summary>
        /// <returns>A <seealso cref="TaskState" /> value which represents the state of the current <see cref="TaskDefinition" />.</returns>
        public TaskState State
        {
            get { return _state; }
            private set
            {
                Monitor.Enter(SyncRoot);
                try
                {
                    if (_state != value)
                    {
                        TaskStateChangeEventArgs args = new TaskStateChangeEventArgs(_state, value);
                        _state = value;
                        try { OnStateChanged(args); }
                        finally
                        {
                            EventHandler<TaskStateChangeEventArgs> stateChanged = StateChanged;
                            if (stateChanged != null)
                                stateChanged(this, args);
                        }
                    }
                }
                finally { Monitor.Exit(SyncRoot); }
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override bool IsDueToday { get { return (_state != TaskState.Completed && _state != TaskState.Canceled && base.IsDueToday); } }
        
        public override bool IsDueNow { get { return (_state != TaskState.Completed && _state != TaskState.Canceled && base.IsDueNow); } }
        
        public override bool IsOverdue { get { return (_state != TaskState.Completed && _state != TaskState.Canceled && base.IsOverdue); } }
        
        public override bool IsActive { get { return (_state != TaskState.Completed && _state != TaskState.Canceled && _state != TaskState.Suspended && base.IsActive); } }
        
        public override bool IsClosed { get { return (_state == TaskState.Completed || _state == TaskState.Canceled); } }
        
        protected virtual void OnStateChanging(TaskStateChangingEventArgs args) { }
        
        protected virtual void OnStateChanged(TaskStateChangeEventArgs args) { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        
        /// <summary>
        /// Child tasks which must be completed before the current task can be completed.
        /// </summary>
        /// <returns>A collection of <see cref="TaskDefinition" /> objects.</returns>
        public ReadOnlyCollection<TaskDefinition> Dependencies
        {
            get
            {
                ReadOnlyCollection<TaskDefinition> dependencies = _dependencies;
                Monitor.Enter(SyncRoot);
                try
                {
                    dependencies = _dependencies;
                    if (dependencies == null)
                    {
                        dependencies = new ReadOnlyCollection<TaskDefinition>(_innerDependencies);
                        _dependencies = dependencies;
                    }
                }
                finally { Monitor.Exit(SyncRoot); }
                return dependencies;
            }
        }

        /// <summary>
        /// Determine if the <see cref="State" /> of the current <see cref="TaskDefinition" /> can be changed to
        /// a <seealso cref="TaskState" /> value.
        /// </summary>
        /// <param name="state">A <seealso cref="TaskState" /> value to test.</param>
        /// <param name="actualState">The actual <seealso cref="TaskState" /> value to that would have been applied.</param>
        /// <param name="reason">A string message which states the reason why the state cannot be changed as requested.</param>
        /// <returns>True if the <see cref="State" /> can be changed to new <paramref name="state" /> value; otherwise, false.
        /// If the current <see cref="State" /> already matches the <paramref name="state" /> value,
        /// then this will return true as well.</returns>
        public bool CanChangeToState(TaskState state, out TaskState actualState, out string reason)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                actualState = State;
                if (actualState == state)
                {
                    reason = "Task is already at that state.";
                    return true;
                }
                int count;
                reason = "";
                switch (state)
                {
                    case TaskState.NotStarted:
                        count = _innerDependencies.Count(t => t._state != TaskState.NotStarted);
                        if (count > 0)
                        {
                            reason = (count == 1) ? "1 dependency task has already been started." :
                                count.ToString() + " dependency tasks have alrady been started.";
                            return false;
                        }
                        break;
                    case TaskState.Suspended:
                        count = _innerDependencies.Count(t => t._state == TaskState.Active);
                        if (count > 0)
                        {
                            reason = (count == 1) ? "1 dependency task is still active." :
                                count.ToString() + " dependency tasks are still active.";
                            return false;
                        }
                        break;
                    case TaskState.PendingDependencies:
                        if (_innerDependencies.Count == 0)
                        {
                            reason = "Current task has no dependencies.";
                            return false;
                        }
                        if (!_innerDependencies.Any(t => t._state < TaskState.Completed))
                        {
                            reason = "Current task has no incomplete dependencies.";
                            return false;
                        }
                        break;
                    case TaskState.Completed:
                        count = _innerDependencies.Count(t => t._state < TaskState.Completed);
                        if (count > 0)
                        {
                            reason = (count == 1) ? "Current task still has 1 incomplete dependency task." :
                                "Current task still has " + count.ToString() + " incomplete dependency tasks.";
                            actualState = TaskState.PendingDependencies;
                        }
                        break;
                    default:
                        count = _innerDependencies.Count(t => t._state < TaskState.Completed);
                        if (count > 0)
                        {
                            reason = (count == 1) ? "Current task still has 1 incomplete dependency task." :
                                "Current task still has " + count.ToString() + " incomplete dependency tasks.";
                            return false;
                        }
                        break;
                }
            }
            finally { Monitor.Exit(SyncRoot); }

            return true;
        }
        
        /// <summary>
        /// Determine if the <see cref="State" /> of the current <see cref="TaskDefinition" /> can be changed to
        /// a <seealso cref="TaskState" /> value.
        /// </summary>
        /// <param name="state">A <seealso cref="TaskState" /> value to test.</param>
        /// <param name="actualState">The actual <seealso cref="TaskState" /> value to that would have been applied.</param>
        /// <returns></returns>
        public bool CanChangeToState(TaskState state, out TaskState actualState)
        {
            string reason;
            return CanChangeToState(state, out actualState, out reason);
        }
        
        /// <summary>
        /// Determine if the <see cref="State" /> of the current <see cref="TaskDefinition" /> can be changed to
        /// a <seealso cref="TaskState" /> value.
        /// </summary>
        /// <param name="state">A <seealso cref="TaskState" /> value to test.</param>
        /// <returns>True if the <see cref="State" /> can be changed to new <paramref name="state" /> value; otherwise, false.
        /// If the current <see cref="State" /> already matches the <paramref name="state" /> value,
        /// then this will return true as well.</returns>
        public bool CanChangeToState(TaskState state) { return CanChangeToState(state, out state); }
        
        /// <summary>
        /// Attempt to change the value of <see cref="State" />.
        /// </summary>
        /// <param name="state">The new <seealso cref="TaskState" /> for the current <see cref="TaskDefinition" />.</param>
        /// <param name="actualState">The actual <seealso cref="TaskState" /> that was applied to the
        /// current <see cref="TaskDefinition" />.
        /// If the state was not changed, then this will be the current value of <see cref="State" />.</param>
        /// <param name="reason">A string message which states the reason why the state could not be changed as requested or why the
        /// actual state value was different.</param>
        /// <returns>True if the <see cref="State" /> value was changed; otherwise, false if the state could not be changed.</returns>
        public bool TrySetState(TaskState state, out TaskState actualState, out string reason)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                if (!TryStateChanging(state, out actualState, out reason))
                    return false;
                State = actualState;
            }
            finally { Monitor.Exit(SyncRoot); }
            return true;
        }

        private bool TryStateChanging(TaskState state, out TaskState actualState, out string reason)
        {
            TaskStateChangingEventArgs args = null;
            Monitor.Enter(SyncRoot);
            try
            {
                if (!CanChangeToState(state, out actualState, out reason))
                    return false;
                args = new TaskStateChangingEventArgs(_state, state);
                try
                {
                    OnStateChanging(args);
                    if (!args.Cancel)
                    {
                        EventHandler<TaskStateChangingEventArgs> stateChanging = StateChanging;
                        if (stateChanging != null)
                            stateChanging(this, args);
                    }
                }
                catch (Exception err)
                {
                    if (String.IsNullOrEmpty(args.Reason) || !args.Cancel)
                        args.Reason = (String.IsNullOrEmpty(err.Message)) ? "Unexpected " + err.GetType().Name : err.Message;
                    args.Cancel = true;
                }

                if (args.Cancel && String.IsNullOrEmpty(args.Reason))
                    args.Reason = "State change canceled.";
            }
            finally { Monitor.Exit(SyncRoot); }
            return !args.Cancel;
        }

        /// <summary>
        /// Attempt to change the value of <see cref="State" />.
        /// </summary>
        /// <param name="state">The new <seealso cref="TaskState" /> for the current <see cref="TaskDefinition" />.</param>
        /// <param name="actualState">The actual <seealso cref="TaskState" /> that was applied to the
        /// current <see cref="TaskDefinition" />.
        /// If the state was not changed, then this will be the current value of <see cref="State" />.</param>
        /// <returns>True if the <see cref="State" /> value was changed; otherwise, false if the state could not be changed.</returns>
        public bool TrySetState(TaskState state, out TaskState actualState)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                string reason;
                if (!TryStateChanging(state, out actualState, out reason))
                    return false;
                State = actualState;
            }
            finally { Monitor.Exit(SyncRoot); }
            return true;
        }

        /// <summary>
        /// Change the value of <see cref="State" />.
        /// </summary>
        /// <param name="state">The new <seealso cref="TaskState" /> for the current <see cref="TaskDefinition" />.</param>
        /// <returns>The actual <seealso cref="TaskState" /> that was applied to the
        /// current <see cref="TaskDefinition" />.</returns>
        /// <exception cref="InvalidOperationException">The state of one of the <see cref="Dependencies" /> prevented the
        /// <see cref="State" /> from being changed as requested.</exception>
        public TaskState SetState(TaskState state)
        {
            string reason;
            if (!TrySetState(state, out state, out reason))
                throw new InvalidOperationException(reason);
            return state;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override bool CanClose(out string reason)
        {
            if (_state == TaskState.Canceled)
            {
                reason = "Calendar event is already canceled.";
                return true;
            }
            TaskState actualState;
            return CanChangeToState(TaskState.Completed, out actualState, out reason);
        }
        
        public override bool TryClose(out string reason)
        {
            if (_state == TaskState.Canceled)
            {
                reason = "Calendar event is already canceled.";
                return true;
            }
            TaskState actualState;
            return TrySetState(TaskState.Completed, out actualState, out reason);
        }        
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Determines whether the current <see cref="TaskDefinition" /> is dependent upon another <see cref="TaskDefinition" />.
        /// </summary>
        /// <param name="task"><see cref="TaskDefinition" /> to test.</param>
        /// <returns>True if the current <see cref="TaskDefinition" /> is dependent upon <paramref name="task" />;
        /// otherwise, false.</returns>
        public bool DependsUpon(TaskDefinition task)
        {
            if (task == null)
                return false;
            Monitor.Enter(SyncRoot);
            try
            {
                if (task == null || _innerDependencies.Count == 0)
                    return false;
                if (ReferenceEquals(task, this))
                    return true;
                if (_innerDependencies.Any(i => ReferenceEquals(i, task)))
                    return true;
                return _innerDependencies.Any(i => i.DependsUpon(task));
            }
            finally { Monitor.Exit(SyncRoot); }
        }

        /// <summary>
        /// /// Determines whether another <see cref="TaskDefinition" /> is dependent upon the current <see cref="TaskDefinition" />.
        /// </summary>
        /// <param name="task"><see cref="TaskDefinition" /> to test.</param>
        /// <returns>True if <paramref name="task" /> is dependent upon the current <see cref="TaskDefinition" />;
        /// otherwise, false.</returns>
        public bool IsDependencyOf(TaskDefinition task) { return task != null && task.DependsUpon(this); }

        /// <summary>
        /// Adds a <see cref="TaskDefinition" /> to the collection of <see cref="Dependencies" /> for the current
        /// <see cref="TaskDefinition" />.
        /// </summary>
        /// <param name="task">Dependency <see cref="TaskDefinition" /> to be added.</param>
        /// <exception cref="ArgumentNullException"><paramref name="task" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="task" /> already exists within (or is nested within)
        /// the current collection of <see cref="Dependencies" /> - or - the current <see cref="TaskDefinition" /> is a direct or
        /// indirect dependency of the <paramref name="task" />.</exception>
        /// <exception cref="ArgumentException">Current <see cref="TaskDefinition.State" /> is <seealso cref="TaskState.NotStarted" />
        /// and <see cref="TaskDefinition.State" /> of <paramref name="task" /> is not <seealso cref="TaskState.NotStarted" /> -- or --
        /// Current <see cref="TaskDefinition.State" /> is <seealso cref="TaskState.Suspended" /> and <see cref="TaskDefinition.State" />
        /// of <paramref name="task" /> is <seealso cref="TaskState.Active" /> -- or --
        /// Current <see cref="TaskDefinition.State" /> is <seealso cref="TaskState.Completed" />
        /// or <seealso cref="TaskState.Canceled" />.</exception>
        public void AddDependencyTask(TaskDefinition task)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            Monitor.Enter(SyncRoot);
            try
            {
                Monitor.Enter(task.SyncRoot);
                try
                {
                    AssertCanBeDependency(task);
                    _innerDependencies.Add(task);
                    try { DependencyTask_StateChanged(task, new TaskStateChangeEventArgs(_state, task._state)); }
                    catch
                    {
                        task.StateChanging -= DependencyTask_StateChanging;
                        task.StateChanged -= DependencyTask_StateChanged;
                        int index = _innerDependencies.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                        if (index < _innerDependencies.Count)
                            _innerDependencies.RemoveAt(index);
                        throw;
                    }
                }
                finally { Monitor.Exit(task.SyncRoot); }
            }
            finally { Monitor.Exit(SyncRoot); }
        }

        /// <summary>
        /// Inserts a <see cref="TaskDefinition" /> to the collection of <see cref="Dependencies" /> for the current
        /// <see cref="TaskDefinition" />.
        /// </summary>
        /// <param name="index">Zero-based index at which the <see cref="TaskDefinition" /> is inserted as a dependency.</param>
        /// <param name="task">Dependency <see cref="TaskDefinition" /> to be inserted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="task" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is less than zero or greater than the current count
        /// of <see cref="Dependencies" />.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="task" /> already exists within (or is nested within)
        /// the current collection of <see cref="Dependencies" /> - or - the current <see cref="TaskDefinition" /> is a direct or
        /// indirect dependency of the <paramref name="task" />.</exception>
        /// <exception cref="ArgumentException">Current <see cref="TaskDefinition.State" /> is <seealso cref="TaskState.NotStarted" />
        /// and <see cref="TaskDefinition.State" /> of <paramref name="task" /> is not <seealso cref="TaskState.NotStarted" /> -- or --
        /// Current <see cref="TaskDefinition.State" /> is <seealso cref="TaskState.Suspended" /> and <see cref="TaskDefinition.State" />
        /// of <paramref name="task" /> is <seealso cref="TaskState.Active" /> -- or --
        /// Current <see cref="TaskDefinition.State" /> is <seealso cref="TaskState.Completed" />
        /// or <seealso cref="TaskState.Canceled" />.</exception>
        public void InsertDependencyTask(int index, TaskDefinition task)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "Index value must be greater than or equal to zero.");
            Monitor.Enter(SyncRoot);
            try
            {
                if (index == _innerDependencies.Count)
                    AddDependencyTask(task);
                else
                {
                    if (index > _innerDependencies.Count)
                        throw new ArgumentOutOfRangeException("index", "Index must not be greater than the count of dependencies.");
                    Monitor.Enter(task.SyncRoot);
                    try
                    {
                        AssertCanBeDependency(task);
                        _innerDependencies.Insert(index, task);
                        try { DependencyTask_StateChanged(task, new TaskStateChangeEventArgs(_state, task._state)); }
                        catch
                        {
                            task.StateChanging -= DependencyTask_StateChanging;
                            task.StateChanged -= DependencyTask_StateChanged;
                            int idx = _innerDependencies.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                            if (idx < _innerDependencies.Count)
                                _innerDependencies.RemoveAt(idx);
                            throw;
                        }
                    }
                    finally { Monitor.Exit(task.SyncRoot); }
                }
            }
            finally { Monitor.Exit(SyncRoot); }
        }

        /// <summary>
        /// Removes a <see cref="TaskDefinition" /> dependency from the current <see cref="TaskDefinition" />.
        /// </summary>
        /// <param name="task"><see cref="TaskDefinition" /> to be removed from the collection of <see cref="Dependencies" />
        /// for the current <see cref="TaskDefinition" />.</param>
        /// <returns>True if the <paramref name="task" /> was removed; otherwise, false if <paramref name="task" /> was not a
        /// direct dependency of the current <see cref="TaskDefinition" /> (contained in <see cref="Dependencies" /> collection).</returns>
        public bool RemoveDependentTask(TaskDefinition task)
        {
            if (task == null)
                return false;
            Monitor.Enter(SyncRoot);
            try
            {
                int index = _innerDependencies.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                if (index == _innerDependencies.Count)
                    return false;
                task.StateChanging -= DependencyTask_StateChanging;
                task.StateChanged -= DependencyTask_StateChanged;
                _innerDependencies.RemoveAt(index);
                DependencyTask_StateChanged(task, new TaskStateChangeEventArgs(task._state, TaskState.Canceled));
            }
            finally { Monitor.Exit(SyncRoot); }
            return true;
        }

        /// <summary>
        /// Gets the order index for a dependency task.
        /// </summary>
        /// <param name="task"><see cref="TaskDefinition" /> to get the order value for.</param>
        /// <returns>The zero-based index of <paramref name="task" /> in <see cref="Dependencies" />
        /// or -1 if <paramref name="task" /> is not a direct dependency of the current <see cref="TaskDefinition" />.</returns>
        public int GetOrderOfDependency(TaskDefinition task)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerDependencies.Count > 0)
                {
                    int index = _innerDependencies.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                    if (index < _innerDependencies.Count)
                        return index;
                }
            }
            finally { Monitor.Exit(SyncRoot); }
            return -1;
        }

        /// <summary>
        /// Moves a dependency <see cref="TaskDefinition" /> to the top (zero index) in <see cref="Dependencies" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved to the top.</param>
        /// <returns>true if <paramref name="task" /> was moved to the top of the <see cref="Dependencies" />;
        /// otherwise false, to indicate that <paramref name="task" /> is not a direct dependency of the
        /// current <see cref="TaskDefinition" />.</returns>
        public bool MoveDependencyToTop(TaskDefinition task)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerDependencies.Count > 0)
                {
                    int index = _innerDependencies.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                    if (index == 0)
                        return true;
                    if (index < _innerDependencies.Count)
                    {
                        Monitor.Enter(SyncRoot);
                        try
                        {
                            _innerDependencies.Insert(0, task);
                            _innerDependencies.RemoveAt(index + 1);
                        }
                        finally { Monitor.Exit(SyncRoot); }
                        return true;
                    }
                }
            }
            finally { Monitor.Exit(SyncRoot); }
            return false;
        }

        /// <summary>
        /// Moves a dependency <see cref="TaskDefinition" /> to the bottom (last index) in <see cref="Dependencies" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved to the bottom.</param>
        /// <returns>true if <paramref name="task" /> was moved to the bottom of the <see cref="Dependencies" />;
        /// otherwise false, to indicate that <paramref name="task" /> is not a direct dependency of the
        /// current <see cref="TaskDefinition" />.</returns>
        public bool MoveDependencyToBottom(TaskDefinition task)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerDependencies.Count > 0)
                {
                    int index = _innerDependencies.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                    if (index == _innerDependencies.Count - 1)
                        return true;
                    if (index < _innerDependencies.Count)
                    {
                        Monitor.Enter(SyncRoot);
                        try
                        {
                            _innerDependencies.Add(task);
                            _innerDependencies.RemoveAt(index);
                        }
                        finally { Monitor.Exit(SyncRoot); }
                        return true;
                    }
                }
            }
            finally { Monitor.Exit(SyncRoot); }
            return false;
        }

        /// <summary>
        /// Moves a dependency <see cref="TaskDefinition" /> up in order (toward the zero index) within <see cref="Dependencies" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved up.</param>
        /// <param name="count">The number of levels to move upward. If this is greater than the current order index,
        /// then <paramref name="task" /> will be moved to the top of the order.</param>
        /// <returns>The new order index of the <see cref="TaskDefinition" /> or -1 if <paramref name="task" />
        /// is not a direct dependency of the current <see cref="TaskDefinition" />.</returns>
        public int MoveDependencyUp(TaskDefinition task, int count)
        {
            if (count < 0)
                return MoveDependencyDown(task, 0 - count);
            
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerDependencies.Count > 0)
                {
                    int currentIndex = _innerDependencies.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                    if (currentIndex == 0)
                        return currentIndex;
                    if (currentIndex < _innerDependencies.Count)
                    {
                        if (count == 0)
                            return currentIndex;

                        int newIndex = currentIndex - count;
                        if (newIndex < 0)
                            newIndex = 0;
                        Monitor.Enter(SyncRoot);
                        try
                        {
                            _innerDependencies.Insert(newIndex, task);
                            _innerDependencies.RemoveAt(currentIndex + 1);
                        }
                        finally { Monitor.Exit(SyncRoot); }
                        return newIndex;
                    }
                }
            }
            finally { Monitor.Exit(SyncRoot); }
            return -1;
        }

        /// <summary>
        /// Moves a dependency <see cref="TaskDefinition" /> up in order (toward the zero index) within <see cref="Dependencies" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved up.</param>
        /// <returns>The new order index of the <see cref="TaskDefinition" /> or -1 if <paramref name="task" />
        /// is not a direct dependency of the current <see cref="TaskDefinition" />.</returns>
        public int MoveDependencyUp(TaskDefinition task) { return MoveDependencyUp(task, 1); }

        /// <summary>
        /// Moves a dependency <see cref="TaskDefinition" /> down in order (away from the zero index) within <see cref="Dependencies" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved down.</param>
        /// <param name="count">The number of levels to move upward. If this is greater than the remaining count
        /// in <see cref="Dependencies" />, then <paramref name="task" /> will be moved to the bottom of the order.</param>
        /// <returns>The new order index of the <see cref="TaskDefinition" /> or -1 if <paramref name="task" />
        /// is not a direct dependency of the current <see cref="TaskDefinition" />.</returns>
        public int MoveDependencyDown(TaskDefinition task, int count)
        {
            if (count < 0)
                return MoveDependencyUp(task, 0 - count);
            
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerDependencies.Count > 0)
                {
                    int currentIndex = _innerDependencies.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                    if (currentIndex < _innerDependencies.Count)
                    {
                        int lastIndex = _innerDependencies.Count - 1;
                        if (count == 0 || currentIndex == lastIndex)
                            return currentIndex;

                        int newIndex = currentIndex + count;
                        if (newIndex > lastIndex)
                            newIndex = lastIndex;
                        Monitor.Enter(SyncRoot);
                        try
                        {
                            if (newIndex == lastIndex)
                                _innerDependencies.Add(task);
                            else
                                _innerDependencies.Insert(newIndex, task);
                            _innerDependencies.RemoveAt(currentIndex);
                        }
                        finally { Monitor.Exit(SyncRoot); }
                        return newIndex;
                    }
                }
            }
            finally { Monitor.Exit(SyncRoot); }
            return -1;
        }

        /// <summary>
        /// Moves a dependency <see cref="TaskDefinition" /> down in order (away from the zero index) within <see cref="Dependencies" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved down.</param>
        /// <returns>The new order index of the <see cref="TaskDefinition" /> or -1 if <paramref name="task" />
        /// is not a direct dependency of the current <see cref="TaskDefinition" />.</returns>
        public int MoveDependencyDown(TaskDefinition task) { return MoveDependencyDown(task, 1); }

        class TaskDefinitionComparerProxy : IComparer<TaskDefinition>
        {
            private Func<TaskDefinition, TaskDefinition, int> _comparer;
            internal TaskDefinitionComparerProxy(Func<TaskDefinition, TaskDefinition, int> comparer) { _comparer = comparer; }
            public int Compare(TaskDefinition x, TaskDefinition y) { return _comparer(x, y); }
        }
        
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public void SortDependencies(Func<TaskDefinition, TaskDefinition, int> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");
            
            SortDependencies(new TaskDefinitionComparerProxy(comparer));
        }
        
        public void SortDependencies(IComparer<TaskDefinition> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");
            
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerDependencies.Count > 1)
                {
                    TaskDefinition[] sortedItems = _innerDependencies.OrderBy(i => i, comparer).ToArray();
                    for (int i = 0; i < sortedItems.Length; i++)
                        _innerDependencies[i] = sortedItems[i];
                }
            }
            finally { Monitor.Exit(SyncRoot); }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        
        private void DependencyTask_StateChanging(object sender, TaskStateChangingEventArgs e)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                if (_state == TaskState.Canceled && e.NewState != TaskState.Completed && e.NewState != TaskState.Canceled)
                {
                    e.Cancel = true;
                    e.Reason = "Dependent task is already canceled.";
                }
            }
            finally { Monitor.Exit(SyncRoot); }
        }
        
        private void DependencyTask_StateChanged(object sender, TaskStateChangeEventArgs e)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                if (e.NewState < TaskState.Completed)
                    _hasOpenDependencies = true;
                else if (_hasOpenDependencies)
                    _hasOpenDependencies = _innerDependencies.Any(t => t._state < TaskState.Completed);
                switch (_state)
                {
                    case TaskState.PendingDependencies:
                        if (!_hasOpenDependencies)
                            State = TaskState.Completed;
                        break;
                    case TaskState.Completed:
                        if (e.NewState < TaskState.Completed)
                            State = TaskState.PendingDependencies;
                        break;
                    case TaskState.NotStarted:
                        if (e.NewState == TaskState.Active)
                            State = TaskState.Active;
                        break;
                }
            }
            finally { Monitor.Exit(SyncRoot); }
        }

        private void AssertCanBeDependency(TaskDefinition task)
        {
            if (ReferenceEquals(task, this))
                throw new InvalidOperationException("Circular dependencies not allowed. A task cannot be a dependency of itself.");
            if (_innerDependencies.Count == 0)
                return;
            if (IsDependencyOf(task))
                throw new InvalidOperationException("Circular dependencies not allowed. The current task is a dependency of that task.");
            if (_innerDependencies.Any(i => ReferenceEquals(i, task)))
                throw new InvalidOperationException("The task is already a dependency of the current task.");
            if (DependsUpon(task))
                throw new InvalidOperationException("The task is already a nested dependency of the current task.");
            if (_state == TaskState.Canceled && task._state != TaskState.Completed && task._state != TaskState.Canceled)
                throw new InvalidOperationException("Dependent task is already canceled.");
            task.StateChanging += DependencyTask_StateChanging;
            task.StateChanged += DependencyTask_StateChanged;
        }
    }
}