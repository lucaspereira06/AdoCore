using System;

namespace Ado.Common
{
    public class SqlProvider : DefaultProvider
    {
        public override string FieldFormat
        {
            get { return "[{0}]"; }
        }

        public override string ParameterIdentifier
        {
            get { return "@"; }
        }

        public override int ExecuteGetIdentity(string command, object[] commandParams)
        {
            var identityQuery = string.Concat(command, "; SELECT SCOPE_IDENTITY()");
            var value = CreateDbCommand(identityQuery, commandParams).ExecuteScalar();
            return Convert.ToInt32(value);
        }
    }
}
