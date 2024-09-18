using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MsExcelUtil.Commands
{
    [Cmdlet(VerbsCommon.Get, "ExcelCell", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(PSExcelCell))]
    public class Get_ExcelCell : PSCmdlet
    {
    }
}
