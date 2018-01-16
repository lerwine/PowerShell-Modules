using System;
using System.Data;
using System.Data.Common;

namespace PSDBLib
{
    public class PsDbCommand : DbCommand
    {
        public PsDbCommand() { }

#warning Not implemented
        public override string CommandText
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

#warning Not implemented
        public override int CommandTimeout
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override CommandType CommandType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

#warning Not implemented
        public override bool DesignTimeVisible
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

#warning Not implemented
        protected override DbConnection DbConnection
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

#warning Not implemented
        protected override DbParameterCollection DbParameterCollection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

#warning Not implemented
        protected override DbTransaction DbTransaction
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Cancel()
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        public override int ExecuteNonQuery()
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        public override object ExecuteScalar()
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        public override void Prepare()
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        protected override DbParameter CreateDbParameter()
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
#warning Not implemented
            throw new NotImplementedException();
        }
    }
}