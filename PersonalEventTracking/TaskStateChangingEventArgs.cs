using System;

namespace PersonalEventTracking
{
    /// <summary>
    /// Represents the values of a pending <seealso cref="TaskDefinition.State" /> change.
    /// </summary>
    public class TaskStateChangingEventArgs : TaskStateChangeEventArgs
    {
        /// <summary>
        /// True if change should be canceled; otherwise false.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// A verbose message stating why the change was canceled.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TaskStateChangingEventArgs" />
        /// </summary>
        /// <param name="previousState"><seealso cref="TaskState" /> value being changed from</param>
        /// <param name="newState"><seealso cref="TaskState" /> value being changed to.</param>
        public TaskStateChangingEventArgs(TaskState previousState, TaskState newState) : base(previousState, newState) { }
        
        /// <summary>
        /// Initializes a new instance of <see cref="TaskStateChangingEventArgs" />
        /// </summary>
        public TaskStateChangingEventArgs() { }
}