using System;
using System.Windows.Forms;

namespace PersonalEventTracking
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// Defines an event based off of specific block of time.
    /// </summary>
    public class CalendarEventDefinition : EventDefinitionBase
    {
        private bool _isClosed = false;
        private bool _hasOpenTasks = false;
        private Collection<TaskDefinition> _innerTaskList = new Collection<TaskDefinition>();
        private ReadOnlyCollection<TaskDefinition> _taskList = null;
        
        /// <summary>
        /// True if any of the <see cref="TaskList" /> have a <see cref="TaskDefinition.State" /> value that is
        /// not <seealso cref="TaskState.Completed" /> or <seealso cref="TaskState.Canceled" />. 
        /// </summary>
        public bool HasOpenTasks { get { return _hasOpenTasks; } }

        public override bool IsClosed { return _isClosed; }

        /// <summary>
        /// Child tasks which must be completed before the current task can be completed.
        /// </summary>
        /// <returns>A collection of <see cref="TaskDefinition" /> objects.</returns>
        public ReadOnlyCollection<TaskDefinition> TaskList
        {
            get
            {
                ReadOnlyCollection<TaskDefinition> taskList = _taskList;
                Monitor.Enter(SyncRoot);
                try
                {
                    taskList = _taskList;
                    if (taskList == null)
                    {
                        taskList = new ReadOnlyCollection<TaskDefinition>(_innerTaskList);
                        _taskList = taskList;
                    }
                }
                finally { Monitor.Exit(SyncRoot); }
                return taskList;
            }
        }
        
        public override bool CanClose(out string reason)
        {
            reason = "";
            Monitor.Enter(SyncRoot);
            try
            {
                if (_isClosed)
                    reason = "Calendar event is already closed.";
                else
                {
                    int count = _innerTaskList.Count(t => t._state != TaskState.NotStarted);
                    if (count > 0)
                    {
                        reason = (count == 1) ? "1 dependency task has already been started." :
                            count.ToString() + " dependency tasks have alrady been started.";
                        return false;
                    }
                }
            }
            finally { Monitor.Exit(SyncRoot); }

            return true;
        }
        
        public override bool TryClose(out string reason)
        {
            reason = "";
            Monitor.Enter(SyncRoot);
            try
            {
                if (!_isClosed)
                {
                    if (!CanClose(state, out actualState, out reason))
                        return false;
                    _isClosed = true;
                    UpdateExpiration();
                }
            }
            finally { Monitor.Exit(SyncRoot); }
            return true;
        }
        
        /// <summary>
        /// Determines whether the current <see cref="CalendarEventDefinition" /> is dependent upon another <see cref="TaskDefinition" />.
        /// </summary>
        /// <param name="task"><see cref="TaskDefinition" /> to test.</param>
        /// <returns>True if the current <see cref="CalendarEventDefinition" /> is dependent upon <paramref name="task" />;
        /// otherwise, false.</returns>
        public bool DependsUpon(TaskDefinition task)
        {
            if (task == null)
                return false;
            Monitor.Enter(SyncRoot);
            try
            {
                if (task == null || _innerTaskList.Count == 0)
                    return false;
                if (ReferenceEquals(task, this))
                    return true;
                if (_innerTaskList.Any(i => ReferenceEquals(i, task)))
                    return true;
                return _innerTaskList.Any(i => i.DependsUpon(task));
            }
            finally { Monitor.Exit(SyncRoot); }
        }

        /// <summary>
        /// Adds a <see cref="TaskDefinition" /> to the collection of <see cref="TaskList"> for the current
        /// <see cref="TaskDefinition" />.
        /// </summary>
        /// <param name="task"><see cref="TaskDefinition" /> to be added.</param>
        /// <exception cref="ArgumentNullException"><paramref name="task" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="task" /> already exists within (or is nested within)
        /// the current collection of <see cref="TaskList">.</exception>
        /// <exception cref="ArgumentException">Current <see cref="TaskDefinition.State" /> is <seealso cref="TaskState.NotStarted" />
        /// and <see cref="TaskDefinition.State" /> of <paramref name="task" /> is not <seealso cref="TaskState.NotStarted" /> -- or --
        /// Current <see cref="TaskDefinition.State" /> is <seealso cref="TaskState.Suspended" /> and <see cref="TaskDefinition.State" />
        /// of <paramref name="task" /> is <seealso cref="TaskState.Active" /> -- or --
        /// Current <see cref="TaskDefinition.State" /> is <seealso cref="TaskState.Completed" />
        /// or <seealso cref="TaskState.Canceled" />.</exception>
        public void AddTask(TaskDefinition task)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            Monitor.Enter(SyncRoot);
            try
            {
                Monitor.Enter(task.SyncRoot);
                try
                {
                    AssertCanAddTask(task);
                    _innerTaskList.Add(task);
                    try { Task_StateChanged(task, new TaskStateChangeEventArgs(_state, task._state)); }
                    catch
                    {
                        task.StateChanging -= Task_StateChanging;
                        task.StateChanged -= Task_StateChanged;
                        int index = _innerTaskList.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                        if (index < _innerTaskList.Count)
                            _innerTaskList.RemoveAt(index);
                        throw;
                    }
                }
                finally { Monitor.Exit(task.SyncRoot); }
            }
            finally { Monitor.Exit(SyncRoot); }
        }

        /// <summary>
        /// Inserts a <see cref="TaskDefinition" /> to the <see cref="TaskList"> for the current
        /// <see cref="TaskDefinition" />.
        /// </summary>
        /// <param name="index">Zero-based index at which the <see cref="TaskDefinition" /> is inserted.</param>
        /// <param name="task"><see cref="TaskDefinition" /> to be inserted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="task" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is less than zero or greater than the current count
        /// of <see cref="TaskList">.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="task" /> already exists within (or is nested within)
        /// the current <see cref="TaskList">.</exception>
        /// <exception cref="ArgumentException">Current <see cref="CalendarEventDefintion.IsClosed" /> is true.</exception>
        public void InsertTask(int index, TaskDefinition task)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "Index value must be greater than or equal to zero.");
            Monitor.Enter(SyncRoot);
            try
            {
                if (index == _innerTaskList.Count)
                    AddTask(task);
                else
                {
                    if (index > _innerTaskList.Count)
                        throw new ArgumentOutOfRangeException("index", "Index must not be greater than the count of tasks.");
                    Monitor.Enter(task.SyncRoot);
                    try
                    {
                        AssertCanAddTask(task);
                        _innerTaskList.Insert(index, task);
                        try { Task_StateChanged(task, new TaskStateChangeEventArgs(_state, task._state)); }
                        catch
                        {
                            task.StateChanging -= Task_StateChanging;
                            task.StateChanged -= Task_StateChanged;
                            int index = _innerTaskList.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                            if (index < _innerTaskList.Count)
                                _innerTaskList.RemoveAt(index);
                            throw;
                        }
                    }
                    finally { Monitor.Exit(task.SyncRoot); }
                }
            }
            finally { Monitor.Exit(SyncRoot); }
        }

        /// <summary>
        /// Removes a <see cref="TaskDefinition" /> from the current <see cref="CalendarEventDefinition" />.
        /// </summary>
        /// <param name="task"><see cref="TaskDefinition" /> to be removed from the collection of <see cref="TaskList">
        /// for the current <see cref="CalendarEventDefinition" />.</param>
        /// <returns>True if the <paramref name="task" /> was removed; otherwise, false if <paramref name="task" /> was not a
        /// direct dependency of the current <see cref="CalendarEventDefinition" /> (contained in <see cref="TaskList" /> collection).</returns>
        public bool RemoveTask(TaskDefinition task)
        {
            if (task == null)
                return false;
            Monitor.Enter(SyncRoot);
            try
            {
                int index = _innerTaskList.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                if (index == _innerTaskList.Count)
                    return false;
                task.StateChanging -= Task_StateChanging;
                task.StateChanged -= Task_StateChanged;
                _innerTaskList.RemoveAt(index);
                Task_StateChanged(task, new TaskStateChangeEventArgs(task._state, TaskState.Canceled));
            }
            finally { Monitor.Exit(SyncRoot); }
        }

        /// <summary>
        /// Gets the order index for a task.
        /// </summary>
        /// <param name="task"><see cref="TaskDefinition" /> to get the order value for.</param>
        /// <returns>The zero-based index of <paramref name="task" /> in <see cref="TaskList" />
        /// or -1 if <paramref name="task" /> is not a direct dependency of the current <see cref="CalendarEventDefinition" />.</returns>
        public int GetOrderOfTask(TaskDefinition task)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerTaskList.Count > 0)
                {
                    int index = _innerTaskList.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                    if (index < _innerTaskList.Count)
                        return index;
                }
            }
            finally { Monitor.Exit(SyncRoot); }
            return -1;
        }

        /// <summary>
        /// Moves a <see cref="TaskDefinition" /> to the top (zero index) in <see cref="TaskList" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved to the top.</param>
        /// <returns>true if <paramref name="task" /> was moved to the top of the <see cref="TaskList" />;
        /// otherwise false, to indicate that <paramref name="task" /> is not a direct dependency of the
        /// current <see cref="CalendarEventDefinition" />.</returns>
        public bool MoveTaskToTop(TaskDefinition task)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerTaskList.Count > 0)
                {
                    int index = _innerTaskList.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                    if (index == 0)
                        return true;
                    if (index < _innerTaskList.Count)
                    {
                        Monitor.Enter(SyncRoot);
                        try
                        {
                            _innerTaskList.Insert(0, task);
                            _innerTaskList.RemoveAt(index + 1);
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
        /// Moves a <see cref="TaskDefinition" /> to the bottom (last index) in <see cref="TaskList" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved to the bottom.</param>
        /// <returns>true if <paramref name="task" /> was moved to the bottom of the <see cref="TaskList" />;
        /// otherwise false, to indicate that <paramref name="task" /> is not a direct dependency of the
        /// current <see cref="CalendarEventDefinition" />.</returns>
        public bool MoveTaskToBottom(TaskDefinition task)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerTaskList.Count > 0)
                {
                    int index = _innerTaskList.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                    if (index == _innerTaskList.Count - 1)
                        return true;
                    if (index < _innerTaskList.Count)
                    {
                        Monitor.Enter(SyncRoot);
                        try
                        {
                            _innerTaskList.Add(task);
                            _innerTaskList.RemoveAt(index);
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
        /// Moves a <see cref="TaskDefinition" /> up in order (toward the zero index) within <see cref="TaskList" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved up.</param>
        /// <param name="count">The number of levels to move upward. If this is greater than the current order index,
        /// then <paramref name="task" /> will be moved to the top of the order.</param>
        /// <returns>The new order index of the <see cref="TaskDefinition" /> or -1 if <paramref name="task" />
        /// is not a direct dependency of the current <see cref="CalendarEventDefinition" />.</returns>
        public int MoveTaskUp(TaskDefinition task, int count)
        {
            if (count < 0)
                return MoveTaskDown(task, 0 - count);
            
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerTaskList.Count > 0)
                {
                    int currentIndex = _innerTaskList.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                    if (currentIndex == 0)
                        return currentIndex;
                    if (currentIndex < _innerTaskList.Count)
                    {
                        if (count == 0)
                            return currentIndex;

                        int newIndex = current - count;
                        if (newIndex < 0)
                            newIndex = 0;
                        Monitor.Enter(SyncRoot);
                        try
                        {
                            _innerTaskList.Insert(newIndex, task);
                            _innerTaskList.RemoveAt(currentIndex + 1);
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
        /// Moves a <see cref="TaskDefinition" /> up in order (toward the zero index) within <see cref="TaskList" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved up.</param>
        /// <returns>The new order index of the <see cref="TaskDefinition" /> or -1 if <paramref name="task" />
        /// is not a direct dependency of the current <see cref="CalendarEventDefinition" />.</returns>
        public int MoveTaskUp(TaskDefinition task) { return MoveTaskUp(task, 1); }

        /// <summary>
        /// Moves a <see cref="TaskDefinition" /> down in order (away from the zero index) within <see cref="TaskList" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved down.</param>
        /// <param name="count">The number of levels to move upward. If this is greater than the remaining count
        /// in <see cref="TaskList" />, then <paramref name="task" /> will be moved to the bottom of the order.</param>
        /// <returns>The new order index of the <see cref="TaskDefinition" /> or -1 if <paramref name="task" />
        /// is not a direct dependency of the current <see cref="CalendarEventDefinition" />.</returns>
        public int MoveTaskDown(TaskDefinition task, int count)
        {
            if (count < 0)
                return MoveTaskUp(task, 0 - count);
            
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerTaskList.Count > 0)
                {
                    int currentIndex = _innerTaskList.TakeWhile(i => !ReferenceEquals(i, task)).Count();
                    if (currentIndex < _innerTaskList.Count)
                    {
                        int lastIndex = _innerTaskList.Count - 1;
                        if (count == 0 || currentIndex == lastIndex)
                            return currentIndex;

                        int newIndex = current + count;
                        if (newIndex > lastIndex)
                            newIndex = lastIndex;
                        Monitor.Enter(SyncRoot);
                        try
                        {
                            if (lastIndex == lastIndex)
                                _innerTaskList.Add(task);
                            else
                                _innerTaskList.Insert(newIndex, task);
                            _innerTaskList.RemoveAt(currentIndex);
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
        /// Moves a <see cref="TaskDefinition" /> down in order (away from the zero index) within <see cref="TaskList" />.
        /// </summary>
        /// <param name="task">The <see cref="TaskDefinition" /> to be moved down.</param>
        /// <returns>The new order index of the <see cref="TaskDefinition" /> or -1 if <paramref name="task" />
        /// is not a direct dependency of the current <see cref="CalendarEventDefinition" />.</returns>
        public int MoveTaskDown(TaskDefinition task, int count) { return MoveTaskDown(task, 1); }

        class TaskDefinitionComparerProxy : IComparer<TaskDefinition>
        {
            private Func<TaskDefinition, TaskDefinition, int> _comparer;
            internal TaskDefinitionComparerProxy(Func<TaskDefinition, TaskDefinition, int> comparer) { _comparer = comparer; }
            public int Compare(TaskDefinition x, TaskDefinition y) { return _comparer(x, y); }
        }
        
        public void SortTaskList(Func<TaskDefinition, TaskDefinition, int> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");
            
            SortTaskList(new TaskDefinitionComparerProxy(comparer));
        }
        
        public void SortTaskList(IComparer<TaskDefinition> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");
            
            Monitor.Enter(SyncRoot);
            try
            {
                if (_innerTaskList.Count > 1)
                {
                    TaskDefinition[] sortedItems = _innerTaskList.OrderBy(i => i, comparer).ToArray();
                    for (int i = 0; i < sortedItems.Length; i++)
                        _innerTaskList[i] = sortedItems[i];
                }
            }
            finally { Monitor.Exit(SyncRoot); }
        }
        
        private void Task_StateChanging(object sender, TaskStateChangingEventArgs e)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                if (_isClosed && e.NewState !== TaskState.Completed && e.NewState !== TaskState.Canceled)
                {
                    e.Cancel = true;
                    e.Reason = "Parent calendar event is already closed.";
                }
            }
            finally { Monitor.Exit(SyncRoot); }
        }
        
        private void Task_StateChanged(object sender, TaskStateChangeEventArgs e)
        {
            Monitor.Enter(SyncRoot);
            try
            {
                if (e.NewState < TaskState.Completed)
                    _hasOpenTasks = true;
                else if (_hasOpenTasks)
                    _hasOpenTasks = _innerTaskList.Any(t => t._state < TaskState.Completed);
                if (!_hasOpenTasks)
                    TryClose();
            }
            finally { Monitor.Exit(SyncRoot); }
        }

        private void AssertCanAddTask(TaskDefinition task)
        {
            if (_innerTaskList.Count == 0)
                return;
            if (_innerTaskList.Any(i => ReferenceEquals(i, task)))
                throw new InvalidOperationException("The task is already a dependency of the current task.");
            if (DependsUpon(task))
                throw new InvalidOperationException("The task is already a nested dependency of the current task.");
            if (_isClosed && task._state !== TaskState.Completed && task._state !== TaskState.Canceled)
                throw new InvalidOperationException("Dependent task is already canceled.");
            task.StateChanging += Task_StateChanging;
            task.StateChanged += Task_StateChanged;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}