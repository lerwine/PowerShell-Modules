using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MsExcelUtil.Commands
{
    [Cmdlet(VerbsCommon.Get, "ExcelRange", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(PSExcelRange))]
    public class Get_ExcelRange : PSCmdlet
    {
    }
}
