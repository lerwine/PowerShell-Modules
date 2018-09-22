using System;

namespace PersonalEventTracking
{
    /// <summary>
    /// Represents the values of a <seealso cref="TaskDefinition.State" /> change.
    /// </summary>
    public class TaskStateChangeEventArgs : EventArgs
    {
        private TaskState _previousState;
        private TaskState _newState;

        /// <summary>
        /// <seealso cref="TaskState" /> value being changed from.
        /// </summary>
        public TaskState PreviousState { get { return _previousState; } }

        /// <summary>
        /// <seealso cref="TaskState" /> value being changed to.
        /// </summary>
        public TaskState NewState { get { return _newState; } }

        /// <summary>
        /// Initializes a new instance of <see cref="TaskStateChangeEventArgs" />
        /// </summary>
        /// <param name="previousState"><seealso cref="TaskState" /> value being changed from</param>
        /// <param name="newState"><seealso cref="TaskState" /> value being changed to.</param>
        public TaskStateChangeEventArgs(TaskState previousState, TaskState newState)
        {
            _previousState = previousState;
            _newState = newState;
        }
        
        /// <summary>
        /// Initializes a new instance of <see cref="TaskStateChangeEventArgs" />
        /// </summary>
        public TaskStateChangeEventArgs() { }
    }
}