using System;
using System.Data;
using System.Data.Common;

namespace PSDBLib
{
    public class PsDbCommandBuilder : DbCommandBuilder
    {
        public PsDbCommandBuilder() { }

        protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause)
        {
            throw new NotImplementedException();
        }

        protected override string GetParameterName(string parameterName)
        {
            throw new NotImplementedException();
        }

        protected override string GetParameterName(int parameterOrdinal)
        {
            throw new NotImplementedException();
        }

        protected override string GetParameterPlaceholder(int parameterOrdinal)
        {
            throw new NotImplementedException();
        }

        protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
        {
            throw new NotImplementedException();
        }
    }
}