using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace PSDB
{
    /// <summary>
    /// Represents a set of methods for creating instances of a provider's implementation of the data source classes.
    /// </summary>
    public class PsDbProviderFactory : DbProviderFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PsDbProviderFactory"/> class.
        /// </summary>
        public PsDbProviderFactory() { }

        /// <summary>
        /// Returns false, indicating that this <see cref="PsDbProviderFactory"/> does not support the <see cref="DbDataSourceEnumerator"/> class.
        /// </summary>
        public override bool CanCreateDataSourceEnumerator { get { return false; } }
        
        /// <summary>
        /// Returns a new instance of the <see cref="PsDbCommand"/> class.
        /// </summary>
        /// <returns>A new instance of <see cref="PsDbCommand"/>.</returns>
        public override DbCommand CreateCommand() { return new PsDbCommand(); }

        /// <summary>
        /// Returns a new instance of the <see cref="PsDbCommandBuilder"/> class.
        /// </summary>
        /// <returns>A new instance of <see cref="PsDbCommandBuilder"/>.</returns>
        public override DbCommandBuilder CreateCommandBuilder() { return new PsDbCommandBuilder(); }

        /// <summary>
        /// Returns a new instance of the <see cref="PsDbConnection"/> class.
        /// </summary>
        /// <returns>A new instance of <see cref="PsDbConnection"/>.</returns>
        public override DbConnection CreateConnection() { return new PsDbConnection(); }

        /// <summary>
        /// Returns a new instance of the <see cref="PsDbConnectionStringBuilder"/> class.
        /// </summary>
        /// <returns>A new instance of <see cref="PsDbConnectionStringBuilder"/>.</returns>
        public override DbConnectionStringBuilder CreateConnectionStringBuilder() { return new PsDbConnectionStringBuilder(); }

        /// <summary>
        /// Returns a new instance of the <see cref="PsDbDataAdapter"/> class.
        /// </summary>
        /// <returns>A new instance of <see cref="PsDbDataAdapter"/>.</returns>
        public override DbDataAdapter CreateDataAdapter() { return new PsDbDataAdapter(); }

        /// <summary>
        /// Returns null since this provider factory does not enumerate data sources.
        /// </summary>
        /// <returns>A null value.</returns>
        public override DbDataSourceEnumerator CreateDataSourceEnumerator() { return null; }

        /// <summary>
        /// Returns a new instance of the <see cref="PsDbParameter"/> class.
        /// </summary>
        /// <returns>A new instance of <see cref="PsDbParameter"/>.</returns>
        public override DbParameter CreateParameter() { return new PsDbParameter(); }

        /// <summary>
        /// Returns a new instance of the <see cref="PsCodeAccessPermission"/> class.
        /// </summary>
        /// <param name="state">One of the <see cref="PermissionState"/> values.</param>
        /// <returns>A <see cref="PsCodeAccessPermission"/> object for the specified <see cref="PermissionState"/>.</returns>
        public override CodeAccessPermission CreatePermission(PermissionState state) { return new PsCodeAccessPermission(state); }
    }
}
