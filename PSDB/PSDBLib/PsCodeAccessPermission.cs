using System;
using System.Security;
using System.Security.Permissions;

namespace PSDBLib
{
    public class PsCodeAccessPermission : CodeAccessPermission
    {
        public PsCodeAccessPermission(PermissionState state)
        {
            throw new NotImplementedException();
        }

        public override IPermission Copy()
        {
            throw new NotImplementedException();
        }

        public override void FromXml(SecurityElement elem)
        {
            throw new NotImplementedException();
        }

        public override IPermission Intersect(IPermission target)
        {
            throw new NotImplementedException();
        }

        public override bool IsSubsetOf(IPermission target)
        {
            throw new NotImplementedException();
        }

        public override SecurityElement ToXml()
        {
            throw new NotImplementedException();
        }
    }
}