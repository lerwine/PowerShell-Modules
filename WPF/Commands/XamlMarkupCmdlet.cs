using System;
using System.Collections;
using System.Management.Automation;

namespace Erwine.Leonard.T.WPF.Commands
{
    /// <summary>
    /// Base class for cmdlets which process XAML markup.
    /// </summary>
    public abstract class XamlMarkupCmdlet : PSCmdlet
    {
        /// <summary>
        /// Returns source value as an enumerable collection.
        /// </summary>
        /// <param name="value">Value to convert to an enumerable collection.</param>
        /// <returns>An <seealso cref="IEnumerable"/> collection created from <paramref name="value"/>.</returns>
        /// <remarks>If <paramref name="value"/> is null, then an empty collection is returned.
        /// If <paramref name="value"/> implements <seealso cref="IEnumerable"/> and it is not a string, then <paramref name="value"/> is returned, cast as <seealso cref="IEnumerable"/>;
        /// otherwise, a collection with a single element containing <paramref name="value"/> is returned.
        /// If <paramref name="value"/> is a <seealso cref="PSObject"/>, then <seealso cref="PSObject.BaseObject"/> will be used in place of <paramref name="value"/>.</remarks>
        protected internal static IEnumerable AsEnumerableValue(object value)
        {
            if (value == null)
                return new object[0];

            if (value is PSObject)
                return AsEnumerableValue((value as PSObject).BaseObject);

            if (value is string || !(value is IEnumerable))
                return new object[] { value };

            return value as IEnumerable;
        }
    }
}