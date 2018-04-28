Add-Type -AssemblyName 'System.Speech'

$Script:Text = @'
This is sentence one. This is sentence two
This is sentence three
'@;

$PromptBuilder = New-Object -TypeName 'System.Speech.Synthesis.PromptBuilder';
$PromptBuilder.AppendText($Script:Text);
$pb2 = New-Object -TypeName 'System.Speech.Synthesis.PromptBuilder';
$pb2.AppendText(" yeah");
$PromptBuilder.AppendBreak();
$PromptBuilder.AppendPromptBuilder($pb2);
$PromptBuilder.ToXml();
if ($SpeechSynthesizer -ne $null) { $SpeechSynthesizer.Dispose() }
$SpeechSynthesizer = New-Object -TypeName 'System.Speech.Synthesis.SpeechSynthesizer';
$SpeechSynthesizer.Speak($PromptBuilder);
$PromptBuilder.