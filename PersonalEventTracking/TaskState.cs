using System;

namespace PersonalEventTracking
{
    /// <summary>
    /// Represents the state of a <see cref="TaskDefinition" />
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// Task has not been started.
        /// </summary>
        /// <remarks>This implies that the <see cref="TaskDefinition.State" /> of all task dependencies are <see cref="NotStarted" />
        /// as well.</remarks>
        NotStarted = 0,
        
        /// <summary>
        /// Task is active.
        /// </summary>
        Active = 1,
        
        /// <summary>
        /// Task is suspended.
        /// </summary>
        /// <remarks>This implies that none of the <see cref="TaskDefinition.State" /> of all task dependencies are
        /// either <see cref="Active" />.</remarks>
        Suspended = 2,
        
        /// <summary>
        /// Current task has dependences which have not been completed or canceled.
        /// </summary>
        /// <remarks>This implies that once the <see cref="TaskDefinition.State" /> of all task dependencies
        /// are <see cref="Completed" /> or <see cref="Canceled" />, then the current task will be <see cref="Completed" />.</remarks>
        PendingDependencies = 3,
        
        /// <summary>
        /// Task is completed.
        /// </summary>
        /// <remarks>This implies that the <see cref="TaskDefinition.State" /> of all task dependencies are <see cref="Completed" />
        /// or <see cref="Canceled" /> as well.</remarks>
        Completed = 4,
        
        /// <summary>
        /// Task was canceled.
        /// </summary>
        /// <remarks>This implies that all task dependencies have either been removed or their <see cref="TaskDefinition.State" />
        /// is <see cref="Completed" /> or <see cref="Canceled" />.</remarks>
        Canceled = 5
    }
}