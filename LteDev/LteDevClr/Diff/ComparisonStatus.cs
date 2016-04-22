using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LteDevClr.Diff
{
    public enum ComparisonStatus
    {
        Success,
        MasterNotFound,
        TargetNotFound,
        Changed
    }
}