Add-Type -AssemblyName 'System.Windows.Forms';

Add-Type -TypeDefinition @'
namespace ChoiceEx
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Host;

    public class ChoiceCollection : Collection<ChoiceDescriptionEx>
    {
        public ChoiceCollection() : base() { }
        public ChoiceCollection(IList<ChoiceDescriptionEx> collection) : base(collection) { }
        public Collection<ChoiceDescription> GetPromptChoices()
        {
            Collection<ChoiceDescription> collection = new Collection<ChoiceDescription>();
            foreach (ChoiceDescriptionEx item in this)
                collection.Add((item == null) ? null : item.Choice);
            return collection;
        }
        public  Collection<PSObject> GetValues()
        {
            Collection<PSObject> collection = new Collection<PSObject>();
            foreach (ChoiceDescriptionEx item in this)
                collection.Add((item == null) ? PSObject.AsPSObject(null) : item.Value);
            return collection;
        }
    }
    public class ChoiceDescriptionEx
    {
        private ChoiceDescription _choice;
        public ChoiceDescription Choice { get { return this._choice; } }
        public string Label { get { return this._choice.Label; } }
        public string HelpMessage { get { return this._choice.HelpMessage; } }
        private PSObject _value;
        public PSObject Value { get { return this._value; } }
        public ChoiceDescriptionEx(string label) : this(label, label, null) { }
        public ChoiceDescriptionEx(string label, object value) : this(label, value, null) { }
        public ChoiceDescriptionEx(string label, object value, string helpMessage)
        {
            if (System.String.IsNullOrWhiteSpace(helpMessage))
                this._choice = new ChoiceDescription(label);
            else
                this._choice = new ChoiceDescription(label, helpMessage);
            this._value = PSObject.AsPSObject(value);
        }
        public ChoiceDescriptionEx(ChoiceDescription choice) : this(choice, (choice == null) ? null : choice.Label) { }
        public ChoiceDescriptionEx(ChoiceDescription choice, object value)
        {
            if (choice == null)
                throw new System.ArgumentNullException("choice");
            this._choice = choice;
            this._value = PSObject.AsPSObject(value);
        }
    }
}
'@ -ReferencedAssemblies 'System.Management.Automation', 'mscorlib';

Function New-ChoiceDescriptionEx {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Label,
        [Parameter(Mandatory = $false, Position = 1)]
        [AllowNull()]
        [object]$Value,
        [Parameter(Mandatory = $false, Position = 2)]
        [AllowEmptyString()]
        [AllowNull()]
        [string]$HelpMessage
    )

    if ($PSBoundParameters.ContainsKey('Value')) {
        if ([String]::IsNullOrWhiteSpace($HelpMessage)) {
            New-Object -TypeName 'ChoiceEx.ChoiceDescriptionEx' -ArgumentList $Label, $Value | Write-Output;
        } else {
            (New-Object -TypeName 'ChoiceEx.ChoiceDescriptionEx' -ArgumentList $Label, $Value, $HelpMessage) | Write-Output
        }
    } else {
        if ([String]::IsNullOrWhiteSpace($HelpMessage)) {
            New-Object -TypeName 'ChoiceEx.ChoiceDescriptionEx' -ArgumentList $Label | Write-Output;
        } else {
            (New-Object -TypeName 'ChoiceEx.ChoiceDescriptionEx' -ArgumentList $Label, $Label, $HelpMessage) | Write-Output
        }
    }
}

Function New-ChoiceCollection {
    [CmdletBinding()]
    Param()

    New-Object -TypeName 'ChoiceEx.ChoiceCollection' | Write-Output;
}

Function Add-ChoiceDescription {
    [CmdletBinding(DefaultParameterSetName = "LabelOnly")]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipelineByPropertyName = $false, ValueFromPipeline = $false)]
        [AllowEmptyCollection()]
        [ChoiceEx.ChoiceCollection]$ChoiceCollection,
        [Parameter(Mandatory = $false, Position = 1, ValueFromPipelineByPropertyName = $false, ValueFromPipeline = $true, ParameterSetName = "LabelOnly")]
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipelineByPropertyName = $true, ValueFromPipeline = $false, ParameterSetName = "LabelAndMessage")]
        [string]$Label,
        [Parameter(Mandatory = $false, Position = 2, ValueFromPipelineByPropertyName = $true, ValueFromPipeline = $false, ParameterSetName = "LabelAndMessage")]
        [AllowNull()]
        [object]$Value,
        [Parameter(Mandatory = $false, ValueFromPipelineByPropertyName = $true, ValueFromPipeline = $false, ParameterSetName = "LabelAndMessage")]
        [AllowEmptyString()]
        [AllowNull()]
        [string]$HelpMessage
    )

    Process {
        $ChoiceDescriptionEx = $null;
        if ($PSBoundParameters.ContainsKey('Value')) {
            if ([String]::IsNullOrWhiteSpace($HelpMessage)) {
                if ($PSBoundParameters.ContainsKey('Label')) {
                    $ChoiceCollection.Add((New-ChoiceDescriptionEx -Label $Label -Value $Value));
                }
            } else {
                $ChoiceCollection.Add((New-ChoiceDescriptionEx -Label $Label -Value $Value -HelpMessage $HelpMessage));
            }
        } else {
            if ([String]::IsNullOrWhiteSpace($HelpMessage)) {
                if ($PSBoundParameters.ContainsKey('Label')) {
                        $ChoiceCollection.Add((New-ChoiceDescriptionEx -Label $Label));
                }
            } else {
                $ChoiceCollection.Add((New-ChoiceDescriptionEx -Label $Label -HelpMessage $HelpMessage));
            }
        }
    }
}

Function Get-Choice {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [AllowEmptyCollection()]
        [ChoiceEx.ChoiceCollection]$ChoiceCollection,
        [Parameter(Mandatory = $false, Position = 1)]
        [int]$DefaultChoice = 0
    )

    $selected = $Host.UI.PromptForChoice('Command', 'Select option', $choices.GetPromptChoices(), 1);
    if ($selected -ne $null -and $selected -gt -1 -and $selected -lt $ChoiceCollection.Count) {
        $ChoiceCollection[$selected].Value | Write-Output;
    }
}

Function Get-InputFromWindow {
    [CmdletBinding()]
    Param()
    
    $Form = New-Object -TypeName 'System.Windows.Forms.Form';
    $Form.add_Load({
        [System.Windows.Forms.Form]$f = $args[0];
        $f.WindowState = [System.Windows.Forms.FormWindowState]::Maximized;
        $f.BringToFront();
    });
    $Form.SuspendLayout();
    $Form.DialogResult = [System.Windows.Forms.DialogResult]::Abort;
    $TableLayoutPanel = New-Object -TypeName 'System.Windows.Forms.TableLayoutPanel';
    $TableLayoutPanel.SuspendLayout();
    $TableLayoutPanel.Anchor = [System.Windows.Forms.AnchorStyles]::None;
    $TableLayoutPanel.Dock = [System.Windows.Forms.DockStyle]::Fill;
    $TableLayoutPanel.AutoSize = $true;
    $TableLayoutPanel.ColumnCount = 2;
    $ColumnStyle = New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, 100);
    $TableLayoutPanel.ColumnStyles.Add($ColumnStyle) | Out-Null;
    $ColumnStyle = New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize);
    $TableLayoutPanel.ColumnStyles.Add($ColumnStyle) | Out-Null;
    $TableLayoutPanel.RowCount = 2;
    $RowStyle = New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, 100);
    $TableLayoutPanel.RowStyles.Add($RowStyle) | Out-Null;
    $RowStyle = New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize);
    $TableLayoutPanel.RowStyles.Add($RowStyle) | Out-Null;
    $TextBox = New-Object -TypeName 'System.Windows.Forms.TextBox';
    $TextBox.AcceptsReturn = $true;
    $TextBox.AcceptsTab = $true;
    $TextBox.Anchor = [System.Windows.Forms.AnchorStyles]::None;
    $TextBox.AutoSize = $true;
    $TextBox.Dock = [System.Windows.Forms.DockStyle]::Fill;
    $TextBox.Multiline = $true;
    $TextBox.ScrollBars = [System.Windows.Forms.ScrollBars]::Vertical;
    $TableLayoutPanel.Controls.Add($TextBox, 0, 0);
    $TableLayoutPanel.SetColumnSpan($TextBox, 2);
    $Button = New-Object -TypeName 'System.Windows.Forms.Button';
    $Button.Anchor = [System.Windows.Forms.AnchorStyles]::Right -bor [System.Windows.Forms.AnchorStyles]::Top;
    $Button.AutoSize = $false;
    $Button.Dock = [System.Windows.Forms.DockStyle]::Right;
    $BUtton.Width = 75;
    $Button.Height = 25;
    $Button.Text = "OK";
    $Form.AcceptButton = $Button;
    $Button.add_Click({
        $f = $args[0].FindForm();
        $f.DialogResult = [System.Windows.Forms.DialogResult]::OK;
        $f.Close();
    });
    $TableLayoutPanel.Controls.Add($Button, 0, 1);
    $Button = New-Object -TypeName 'System.Windows.Forms.Button';
    $Button.Anchor = [System.Windows.Forms.AnchorStyles]::Right -bor [System.Windows.Forms.AnchorStyles]::Top;
    $Button.AutoSize = $false;
    $Button.Dock = [System.Windows.Forms.DockStyle]::Right;
    $BUtton.Width = 75;
    $Button.Height = 25;
    $Button.Text = "Cancel";
    $Form.CancelButton = $Button;
    $Button.add_Click({
        $f = $args[0].FindForm();
        $f.DialogResult = [System.Windows.Forms.DialogResult]::Cancel;
        $f.Close();
    });
    $TableLayoutPanel.Controls.Add($Button, 1, 1);
    $Form.Controls.Add($TableLayoutPanel);
    $Form.ResumeLayout();
    $TableLayoutPanel.ResumeLayout();
    $DialogResult = $Form.ShowDialog();
    if ($DialogResult -eq [System.Windows.Forms.DialogResult]::OK) {
        $TextBox.Text | Write-Output;
    }
    $Form.Dispose();
}

Function Get-NormalizedLines {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [string]$InputText
    )

    Begin {
        $CurrentLine = ""
    }

    Process {
        $m = [System.Text.RegularExpressions.Regex]::Match($InputText, '\s*-\s*$');
        if ($m.Success) {
            $NormalizedLine = $InputText.Substring(0, $m.Index);
        } else {
            $NormalizedLine = $InputText.Trim();
            if ($NormalizedLine.Length -gt 0) { $NormalizedLine = $NormalizedLine  + " " }
        }
        
        if ($NormalizedLine.Length -eq 0) {
            if ($CurrentLine.Length -gt 0) { $CurrentLine | Write-Output }
            $CurrentLine = "";
        } else {
            $m = [System.Text.RegularExpressions.Regex]::Match($NormalizedLine, '(?=[^\-])\p{Pd}');
            while ($m.Success) {
                $NormalizedLine = $NormalizedLine.Substring(0, $m.Index) + "-" + $NormalizedLine.Substring($m.Index + $m.Length);
                $m = [System.Text.RegularExpressions.Regex]::Match($NormalizedLine, '\p{Pd}');
            }

            $m = [System.Text.RegularExpressions.Regex]::Match($NormalizedLine, '(?<d1>\d+)\s*-\s*(?<d2>\d+)');
            while ($m.Success) {
                $NormalizedLine = $NormalizedLine.Substring(0, $m.Index) + ('{0} *dash* {1}' -f $m.Groups['d1'].Value, $m.Groups['d2'].Value) + $NormalizedLine.Substring($m.Index + $m.Length);
                $m = [System.Text.RegularExpressions.Regex]::Match($NormalizedLine, '(?<d1>\d+)\s*-\s*(?<d2>\d+)');
            }
            
            $m = [System.Text.RegularExpressions.Regex]::Match($NormalizedLine, '\p{Pi}(?<q>[^\p{Pi}|\p{Pf}]*)\p{Pf}');
            while ($m.Success) {
                $NormalizedLine = $NormalizedLine.Substring(0, $m.Index) + ('"{0}"' -f $m.Groups['q'].Value) + $NormalizedLine.Substring($m.Index + $m.Length);
                $m = [System.Text.RegularExpressions.Regex]::Match($NormalizedLine, '\p{Pi}(?<q>[^\p{Pi}|\p{Pf}]*)\p{Pf}');
            }
            
            $m = [System.Text.RegularExpressions.Regex]::Match($NormalizedLine, '(?=[^"''])\p{Pi}|\p{Pf}');
            while ($m.Success) {
                $NormalizedLine = $NormalizedLine.Substring(0, $m.Index) + "'" + $NormalizedLine.Substring($m.Index + $m.Length);
                $m = [System.Text.RegularExpressions.Regex]::Match($NormalizedLine, '\p{Pi}|\p{Pf}');
            }

            $m = [System.Text.RegularExpressions.Regex]::Match($NormalizedLine, '^(\p{P}|\p{S}|\d+(\s*(\p{P}|\p{S})\s|(\s*(\p{P}|\p{S})\s*\d+)*))');
            if ($m.Success -and $CurrentLine.Length -gt 0) {
                if ($CurrentLine.Length -gt 0) { $CurrentLine | Write-Output }
                $CurrentLine = $NormalizedLine;
            } else {
                $CurrentLine = $CurrentLine + $NormalizedLine;
            }
        }
    }

    End {
        if ($CurrentLine.Length -gt 0) { $CurrentLine | Write-Output }
    }
}

$choices = New-Object -TypeName 'ChoiceEx.ChoiceCollection';
Add-ChoiceDescription -ChoiceCollection $choices -Label 'Speak' -HelpMessage 'Speak Text';
Add-ChoiceDescription -ChoiceCollection $choices -Label 'Add' -HelpMessage 'Queue text using input box';
Add-ChoiceDescription -ChoiceCollection $choices -Label 'Clipboard' -HelpMessage 'Queue text From Clipboard';
Add-ChoiceDescription -ChoiceCollection $choices -Label 'Show' -HelpMessage 'Show current queue';
Add-ChoiceDescription -ChoiceCollection $choices -Label 'Undo' -HelpMessage 'Undo last addition to queue';
Add-ChoiceDescription -ChoiceCollection $choices -Label 'Exit' -HelpMessage 'Exit';

$selectedChoice = '';
$queue = @();
$voice = New-Object -ComObject 'SAPI.SpVoice';
while ($selectedChoice -ne $null) {
    $selectedChoice = Get-Choice -ChoiceCollection $choices;
    switch ($selectedChoice) {
        'Speak' {
            $content = @();
            if ($queue.Count -eq 0) {
                $content = @('Nothing to speak.');
                $content[0] | Write-Warning;
            } else {
                $content = $queue[0].Text + "`r`n";
                for ($i = 1; $i -lt $queue.Count; $i++) {
                    $content = $content + "`r`n";
                    $content = $content + $queue[$i].Text + "`r`n";
                }
            }
            $queue = @();
            $content = $content | Get-NormalizedLines;

            $totalChars = 0;
            foreach ($line in $content) {
                $totalChars = $totalChars + $line.Length;
            }

            $charsSpoken = 0;
            $previousSentence = '';
            [int]$lastPercentage = 0;
            Write-Progress -Activity "Speech to text" -Status $voice.Status.RunningState -PercentComplete 0 -CurrentOperation "Starting"
            foreach ($line in $content) {
                $voice.Speak($line, 1) | Out-Null

                do {
                    $status = $voice.Status
                    [int]$c = ($charsSpoken + $status.InputWordPosition) * 100 / $totalChars
                    if ($c -ne $lastPercentage) {
                        $lastPercentage = $c;
                        Write-Progress -Activity "Speech to text" -Status $status.RunningState -PercentComplete $lastPercentage -CurrentOperation "Speaking";
                    }
                    if ($status.InputSentenceLength -gt 0) {
                        $nextSentence = $line.Substring($status.InputSentencePosition, $status.InputSentenceLength).Trim();
                        if ($previousSentence -ne $nextSentence) {
                            $previousSentence = $nextSentence;
                            @(
                                '',
                                $nextSentence
                            ) | Write-Host;
                        }
                    }
                } while ($voice.WaitUntilDone(100) -ne $true);
                $charsSpoken += $line.Length;
            }
        
            Write-Progress -Activity "Speech to text" -Status $voice.Status.RunningState -PercentComplete 100 -CurrentOperation "Finished" -Completed;
            break;
        }
        'Add' {
            $t = Get-InputFromWindow;
            if ($t -eq $null) {
                'Abored.' | Write-Warning;
            } else {
                if ([String]::IsNullOrWhiteSpace($t)) { $t = @() } else { $t = $t.Trim() -split '(?(?=\r)\r\n?|\n)' | ForEach-Object { $_.Trim() } }
                if ($t.Count -gt 0) {
                    $queue = $queue + @{ Text = $t };
                    'Added to queue: ' | Write-Host -ForegroundColor Green;
                    $t | Write-Host;
                } else {
                    'No text entered' | Write-Warning;
                }
            }
            break;
        }
        'Clipboard' {
            $t = [System.Windows.Forms.Clipboard]::GetText([System.Windows.Forms.TextDataFormat]::UnicodeText);
            if ([String]::IsNullOrWhiteSpace($t)) { $t = @() } else { $t = $t.Trim() -split '(?(?=\r)\r\n?|\n)' | ForEach-Object { $_.Trim() } }
            if ($t.Count -gt 0) {
                $queue = $queue + @{ Text = $t };
                'Copied from clipboard: ' | Write-Host -ForegroundColor Green;
                $t | Write-Host;
            } else {
                'No text in clipboard' | Write-Warning;
            }
        }
        'Undo' {
            if ($queue.Count -eq 0) {
                'Nothing to undo' | Write-Warning;
            } else {
                $t = $queue[$queue.Count - 1].Text;
                $queue = $queue[0..($queue.Count - 2)];
                'Removed from queue: ' | Write-Host -ForegroundColor Green;
                $t | Write-Host;
            }
            break;
        }
        'Show' {
            ('{0} items:' -f $queue.Count) | Write-Host -ForegroundColor Green;
            for ($i = 0; $i -lt $queue.Count; $i++) {
                ('Item {0}: ' -f $i) | Write-Host -ForegroundColor Green;
                $queue[$i].Text | Write-Host;
            }
            break;
        }
        'Exit' {
            $selectedChoice = $null;
            break;
        }
    };
}

$content = [System.IO.File]::ReadAllLines(($PSScriptRoot | Join-Path -ChildPath "SpeakText.txt")) | Get-NormalizedLines;
