using System;
using System.Globalization;
using System.Management.Automation;
using System.Speech.Synthesis;

namespace Erwine.Leonard.T.TextToSpeech.Commands
{
    [Cmdlet(VerbsCommon.New, "PromptBuilder", DefaultParameterSetName = PARAMETERSETNAME_CULTURE)]
    [OutputType(typeof(PromptBuilder))]
    public class New_PromptBuilder : PSCmdlet
    {
        public const string PARAMETERSETNAME_CULTURE = "Culture";
        public const string PARAMETERSETNAME_NAME = "Name";
        public const string PARAMETERSETNAME_CULTUREINFO = "CultureInfo";
        public const string ERRORID_CULTURENOTFOUND = "CultureNotFound";
        public const string ERRORID_UNEXPECTEDERROR = "UnexpectedError";

        [Parameter(Position = 0, ParameterSetName = PARAMETERSETNAME_CULTURE)]
        public int Culture { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_NAME)]
        public string CultureName { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = PARAMETERSETNAME_CULTUREINFO)]
        public CultureInfo CultureInfo { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            
            CultureInfo culture;
            if (ParameterSetName == PARAMETERSETNAME_CULTUREINFO)
                culture = CultureInfo;
            else
            {
                try
                {
                    if (ParameterSetName == PARAMETERSETNAME_NAME)
                        culture = new CultureInfo(CultureName);
                    else
                        culture = (MyInvocation.BoundParameters.ContainsKey("Culture")) ? new CultureInfo(Culture) : null;
                }
                catch (Exception exception)
                {
                    WriteError(new ErrorRecord(exception, ERRORID_CULTURENOTFOUND, ErrorCategory.ObjectNotFound, (ParameterSetName == PARAMETERSETNAME_NAME) ? CultureName as object : Culture));
                    return;
                }
            }
            try { WriteObject((culture == null) ? new PromptBuilder() : new PromptBuilder(culture)); }
            catch (Exception exception)
            {
                WriteError(new ErrorRecord(exception, ERRORID_UNEXPECTEDERROR, ErrorCategory.OpenError, culture));
                return;
            }
        }
    }
}
