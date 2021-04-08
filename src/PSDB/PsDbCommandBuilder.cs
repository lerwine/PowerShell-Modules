using System;
using System.Data;
using System.Data.Common;

namespace PSDB
{
    public class PsDbCommandBuilder : DbCommandBuilder
    {
        public PsDbCommandBuilder() { }

        protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        protected override string GetParameterName(string parameterName)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        protected override string GetParameterName(int parameterOrdinal)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        protected override string GetParameterPlaceholder(int parameterOrdinal)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
        {
#warning Not implemented
            throw new NotImplementedException();
        }
    }
}