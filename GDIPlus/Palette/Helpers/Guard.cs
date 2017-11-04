using System;

namespace Erwine.Leonard.T.GDIPlus.Palette.Helpers
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Guard
    {
        /// <summary>
        /// Checks if an argument is null
        /// </summary>
        /// <param name="argument">argument</param>
        /// <param name="argumentName">argument name</param>
        public static void CheckNull(Object argument, String argumentName)
        {
            if (argument == null)
            {
                String message = string.Format("Cannot use '{0}' when it is null!", argumentName);
                throw new ArgumentNullException(message);
            }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
