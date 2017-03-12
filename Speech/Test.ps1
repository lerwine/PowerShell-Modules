Add-Type -AssemblyName 'System.speech';

Function Read-YesOrNo {
    Param(
        [Parameter(Mandatory = $true, Postion = 0)]
        [string]$Caption,
        
        [Parameter(Mandatory = $true, Postion = 0)]
        [string]$Message,

        [bool]$DefaultValue = $false
    )
    $ChoiceCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
    $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'Y', 'Yes'));
    $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'N', 'No'));
    $Index = 1;
    if ($DefaultValue) { $Index = 0 }
    $Response = $null;
    $Response = ($Host.UI.PromptForChoice($Caption, $Message, $ChoiceCollection, $Index));
    if ($Response -ne $null) { $Response -eq 0 }
}

Function Read-VoiceSelection {
    Param(
        [Parameter(Mandatory = $true, Postion = 0)]
        [string]$Caption,
        
        [Parameter(Mandatory = $true, Postion = 1)]
        [string]$Message,

        [Parameter(Mandatory = $true, Postion = 2)]
        [System.Speech.Synthesis.SpeechSynthesizer]$SpeechSynthesizer,

        [bool]$DefaultValue = $false
    )
    if ($InstalledVoices.Count -eq 0) {
        $SpeechSynthesizer.Voice | Write-Output;
    } else {
        if ($InstalledVoices.Count -eq 1) {
            $InstalledVoices[0] | Write-Output;
        } else {
            $Message = $null;
            $ChoiceCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
            if ($Script:InstalledVoices.Count -eq 2 -and $Script:InstalledVoices[0].VoiceInfo.Gender -ne $Script:InstalledVoices[1].VoiceInfo.Gender) {
                $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $Script:InstalledVoices[0].VoiceInfo.Gender.ToString('F'), $InstalledVoices[0].VoiceInfo.Description));
                $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $Script:InstalledVoices[1].VoiceInfo.Gender.ToString('F'), $InstalledVoices[1].VoiceInfo.Description));
                $Message = "Select voice gender to use ($(($ChoiceCollection | ForEach-Object { $_.Label }) -join ', '))";
            } else {
                $InstalledVoices | ForEach-Object {
                    $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $_.VoiceInfo.Name, $_.VoiceInfo.Description));
                }
                $Message = "Select voice name to use ($(($ChoiceCollection | ForEach-Object { $_.Label }) -join ', '))";
            }
            $Index = 0;
            for ($i = 0; $i -lt $Script:InstalledVoices.Count; $i++) {
                if ($SpeechSynthesizer.Voice.Id -eq $Script:InstalledVoices[$i].VoiceInfo.Id) {
                    $Index = $i;
                    break;
                }
            }
            $Response = $null;
            $Response = $Host.UI.PromptForChoice('Voice', $Message , $ChoiceCollection, $Index);
            if ($Response -ne $null -and $Response -ge 0 -and $Response -lt $InstalledVoices.Count) {
                $InstalledVoices[$Response] | Write-Output;
            }
        }
    }
}

Function Read-SpeechRateSelection {
    Param(
        [Parameter(Mandatory = $true, Postion = 0)]
        [string]$Caption,
        
        [Parameter(Mandatory = $true, Postion = 0)]
        [string]$Message,

        [bool]$DefaultValue = $false
    )
    $Index = $null;
    $Index = $Host.UI.PromptForChoice('Rate', "Select speech rate ($(($PromptRateCollection | ForEach-Object { $_.Label }) -join ', '))", $PromptRateCollection, $PromptRateCollection.IndexOf(($PromptRateCollection | Where-Object { $_.Label -eq 'Medium' })));
    if ($Index -eq $null -or $Index -lt 0 -or $Index -gt 10) { return }
    [System.Speech.Synthesis.PromptRate]$Script:Rate = $PromptRateCollection[$Index].Label;
}



            $Index = $null;
            $Index = $Host.UI.PromptForChoice('Volume', "Select speech volume ($(($PromptVolumeCollection | ForEach-Object { $_.Label }) -join ', '))", $PromptVolumeCollection, $PromptVolumeCollection.IndexOf(($PromptVolumeCollection | Where-Object { $_.Label -eq 'Medium' })));
            if ($Index -eq $null -or $Index -lt 0 -or $Index -gt 10) { return }
            [System.Speech.Synthesis.PromptVolume]$Script:Volume = $PromptVolumeCollection[$Index].Label;
$SpeechSynthesizer = New-Object 'System.Speech.Synthesis.SpeechSynthesizer';
$Script:InstalledVoices = @($SpeechSynthesizer.GetInstalledVoices() | Where-Object { $_.Enabled });
'Installed voices are:' | Write-Host;
$Script:InstalledVoices | ForEach-Object {
    @"
    Name: $($_.VoiceInfo.Name)
        Gender: $($_.VoiceInfo.Gender)
        Age: $($_.VoiceInfo.Age)
        Description: $($_.VoiceInfo.Description)
        Is Default: $($_.VoiceInfo.Id -eq $SpeechSynthesizer.Voice.Id)
"@ | Write-Host;
}
'' | Write-Host;

$PromptRateCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
[Enum]::GetNames([System.Speech.Synthesis.PromptRate]) | ForEach-Object {
    $PromptRateCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $_));
}
$PromptVolumeCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
[Enum]::GetNames([System.Speech.Synthesis.PromptVolume]) | ForEach-Object {
    $PromptVolumeCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $_));
}
$InstalledVoices = @($SpeechSynthesizer.GetInstalledVoices() | Where-Object { $_.Enabled });

try {
    $ChangeSettings = $Script:Volume -eq $null;
    do {
        if (-not $ChangeSettings) {
            @"

    Current Settings:
        Voice: $($Script:Voice.VoiceInfo.Name)
        Rate: $($Script:Rate.ToString('F'))
        Volume: $($Script:Volume.ToString('F'))
"@ | Write-Output;
            $ChoiceCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
            $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'Y', 'Yes'));
            $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'N', 'No'));
            $ChangeSettings = ($Host.UI.PromptForChoice('Settings', 'Change settings?', $ChoiceCollection, 1)) -eq 0;
        }
        if ($ChangeSettings) {
            if ($InstalledVoices.Count -gt 1) {
                if ($InstalledVoices.Count -eq 2 -and $InstalledVoices[0].VoiceInfo.Gender -ne $InstalledVoices[1].VoiceInfo.Gender) {
                    $ChoiceCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
                    $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $InstalledVoices[0].VoiceInfo.Gender.ToString('F'), $InstalledVoices[0].VoiceInfo.Description));
                    $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $InstalledVoices[1].VoiceInfo.Gender.ToString('F'), $InstalledVoices[1].VoiceInfo.Description));
                    $Index = 0;
                    for ($i = 0; $i -lt $InstalledVoices.Count; $i++) {
                        if ($SpeechSynthesizer.Voice.Id -eq $InstalledVoices[$i].VoiceInfo.Id) {
                            $Index = $i;
                            break;
                        }
                    }
                    $Index = $Host.UI.PromptForChoice('Voice', "Select voice gender to use ($(($ChoiceCollection | ForEach-Object { $_.Label }) -join ', '))", $ChoiceCollection, $Index);
                } else {
                    $ChoiceCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
                    $InstalledVoices | ForEach-Object {
                        $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $_.VoiceInfo.Name, $_.VoiceInfo.Description));
                    }
                    $Index = 0;
                    for ($i = 0; $i -lt $InstalledVoices.Count; $i++) {
                        if ($SpeechSynthesizer.Voice.Id -eq $InstalledVoices[$i].VoiceInfo.Id) {
                            $Index = $i;
                            break;
                        }
                    }
                    $Index = $Host.UI.PromptForChoice('Voice', "Select voice name to use ($(($ChoiceCollection | ForEach-Object { $_.Label }) -join ', '))", $ChoiceCollection, $Index);
                }
                if ($Index -eq $null -or $Index -lt 0 -or $Index -ge $InstalledVoices.Count) { return }
                $Script:Voice = $InstalledVoices[$Index];
            } else {
                $Script:Voice = $SpeechSynthesizer.Voice;
            }
        }
    
        $ChoiceCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
        $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'Speakers'));
        $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'WAV File'));
        $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'XML File'));
        $Index = $null;
        $Index = $Host.UI.PromptForChoice('Output', 'Where do you want to send the output?', $ChoiceCollection, 0);
        if ($Index -eq $null -or $Index -lt 0 -or $Index -gt 2) { return }
        $Script:OutputOption = $ChoiceCollection[$Index].Label;
        
        $FieldDescriptionCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.FieldDescription]';
        $FieldDescription = New-Object -TypeName 'System.Management.Automation.Host.FieldDescription' -ArgumentList 'Text' -Property @{
            HelpMessage = 'Enter text you wish to speak';
            DefaultValue = $SpeechSynthesizer.Volume;
            Label = 'Text to speak';
        };

        $FieldDescription.SetParameterType([string[]]);
        $FieldDescriptionCollection.Add($FieldDescription);

        if ($Script:OutputOption -ne 'XML File') {
            $FieldDescription = New-Object -TypeName 'System.Management.Automation.Host.FieldDescription' -ArgumentList 'IsSsml' -Property @{
                HelpMessage = 'Treat as SSML markup? (true or false)';
                DefaultValue = $false;
                Label = 'Is SSML';
            };
        }
        $FieldDescription.SetParameterType([bool]);
        $FieldDescriptionCollection.Add($FieldDescription);

        $TextToSpeak = $null;
        $TextToSpeak = $Host.UI.Prompt('Speech settings', 'What do you want to say?', $FieldDescriptionCollection);
        if ($TextToSpeak -eq $null -or $TextToSpeak['Text'] -eq $null -or @($TextToSpeak['Text'] | Where-Object { $_.Trim().Length -gt 0 }).Count -eq 0) { return }
    
        $PromptBuilder = New-Object -TypeName 'System.Speech.Synthesis.PromptBuilder';
        $PromptBuilder.StartVoice($Script:Voice);
        $Style = [System.Speech.Synthesis.PromptStyle]::new($Script:Rate);
        $Style.Volume = $Script:Volume;
        $PromptBuilder.StartStyle($Style);
        if ($Script:OutputOption -ne 'XML File' -and $TextToSpeak['IsSsml']) {
            $PromptBuilder.AppendSsmlMarkup(($TextToSpeak['Text'] | Out-String).Trim());
        } else {
            $TextToSpeak['Text'] | ForEach-Object {
                $t = $_.Trim();
                if ($t.Length -gt 0) {
                    $PromptBuilder.StartParagraph();
                    $PromptBuilder.AppendText($t)
                    $PromptBuilder.EndParagraph();
                }
            }
        }
        $PromptBuilder.EndStyle();
        $PromptBuilder.EndVoice();
        if ($Script:OutputOption -ne 'Speakers') {
            $Path = Read-Host -Prompt 'Output file path';
            if ($Path -eq $null -or ($Path = $Path.Trim()).Length -eq 0) { return }
            if ($Script:OutputOption -eq 'Xml') {
                [System.IO.File]::WriteAllText($Path, $PromptBuilder.ToXml());
            } else {
                $SpeechSynthesizer.SetOutputToWaveFile($Path);
            }
        } else {
            $SpeechSynthesizer.SetOutputToDefaultAudioDevice();
        
        }
        if ($Script:OutputOption -ne 'Xml') {
            $SpeechSynthesizer.SpeakAsync($PromptBuilder) | Out-Null;
        }
        $ChangeSettings = $false;
        $SpeakMoreText = $false;
        do {
            $ChoiceCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
            $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'Y', 'Yes'));
            $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'N', 'No'));
            $SpeakMoreText = $false;
            $SpeakMoreText = ($Host.UI.PromptForChoice('Repeat', 'Speak more text?', $ChoiceCollection, 1)) -eq 0;
            if ($SpeakMoreText -and $SpeechSynthesizer.State -ne [System.Speech.Synthesis.SynthesizerState]::Ready) {
                $ChoiceCollection = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
                $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'Y', 'Yes'));
                $ChoiceCollection.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList 'N', 'No'));
                if (($Host.UI.PromptForChoice('Cancel', 'This will cancel speech synthesis currently in progress. Continue?', $ChoiceCollection, 1)) -eq 0) {
                    'Cancelling speech...' | Write-Output;
                    $SpeechSynthesizer.SpeakAsyncCancelAll();
            
                    [System.Threading.Thread]::Sleep(100);
                    for ($i = 0; $i -lt 10 -and ($SpeechSynthesizer.State -ne [System.Speech.Synthesis.SynthesizerState]::Ready)) {
                        'Waiting for speech to cancel' | Write-Output;
                        [System.Threading.Thread]::Sleep(500);
                    }
                    if ($SpeechSynthesizer.State -ne [System.Speech.Synthesis.SynthesizerState]::Ready) {
                        Write-Warning -Message 'Speech synthesizer failed to return to Ready status, aborting.';
                        $SpeechSynthesizer.SetOutputToNull();
                        $SpeechSynthesizer.Dispose();
                        $SpeechSynthesizer = New-Object 'System.Speech.Synthesis.SpeechSynthesizer';
                    }
                    break;
                }
            }
        } while ($SpeechSynthesizer.State -ne [System.Speech.Synthesis.SynthesizerState]::Ready);
    } while ($SpeakMoreText);

} finally {
    if ($Script:OutputOption -ne 'Xml') {
        if ($SpeechSynthesizer.State -ne [System.Speech.Synthesis.SynthesizerState]::Ready) {
            'Cancelling speech...' | Write-Output;
            $SpeechSynthesizer.SpeakAsyncCancelAll();
            
            [System.Threading.Thread]::Sleep(100);
            for ($i = 0; $i -lt 10 -and ($SpeechSynthesizer.State -ne [System.Speech.Synthesis.SynthesizerState]::Ready)) {
                'Waiting for speech to cancel' | Write-Output;
                [System.Threading.Thread]::Sleep(500);
            }
            if ($SpeechSynthesizer.State -ne [System.Speech.Synthesis.SynthesizerState]::Ready) {
                Write-Warning -Message 'Speech synthesizer failed to return to Ready status, aborting.';
            }
        }
        $SpeechSynthesizer.SetOutputToNull();
    }
    $SpeechSynthesizer.Dispose();
}

<#$ParameterNames = @{
    promptBuilder = 'PromptBuilder';
    rate = 'int';
    gender = 'VoiceGender';
    age = 'VoiceAge';
    voice = 'string';
    volume = 'int'
    state = 'object';
    path = 'string';
    formatInfo = 'SpeechAudioFormatInfo';
    audioDestination = 'Stream';
}

$ParamSets = @(
    @('promptBuilder', 'rate', 'voice', 'volume', 'state'),
    @('promptBuilder', 'rate', 'voice', 'volume', 'path', 'formatInfo', 'state'),
    @('promptBuilder', 'rate', 'voice', 'volume', 'audioDestination', 'formatInfo', 'state'),
    @('promptBuilder', 'rate', 'gender', 'age', 'volume', 'state'),
    @('promptBuilder', 'rate', 'gender', 'age', 'volume', 'path', 'formatInfo', 'state'),
    @('promptBuilder', 'rate', 'gender', 'age', 'volume', 'audioDestination', 'formatInfo', 'state'),
    @('promptBuilder', 'rate', 'gender', 'volume', 'state'),
    @('promptBuilder', 'rate', 'gender', 'volume', 'path', 'formatInfo', 'state'),
    @('promptBuilder', 'rate', 'gender', 'volume', 'audioDestination', 'formatInfo', 'state')
) | Sort-Object -Property 'Count';
$Items = @();
$ParamSets | ForEach-Object { $Items = $Items + @(,@($_ | Where-Object { $_ -ne 'volume' })) };
$ParamSets = $ParamSets + $Items;
$Items = @();
$ParamSets | ForEach-Object { $Items = $Items + @(,@($_ | Where-Object { $_ -ne 'rate' })) };
$ParamSets = $ParamSets + $Items;
$Items = @();
$ParamSets | Where-Object { $_ -contains'formatInfo' } | ForEach-Object { $Items = $Items + @(,@($_ | Where-Object { $_ -ne 'formatInfo' })) };
$ParamSets = $ParamSets + $Items;
$ParamSets | Sort-Object -Property 'Count' | ForEach-Object {
    $CodeA = '';
    $CodeB = '';
    if ($_ -contains 'rate') {
        $CodeA += ', int rate';
        $CodeB += ', rate';
    } else {
        $CodeB += ', null';
    }
    if ($_ -contains 'gender') {
        $CodeA += ', VoiceGender gender';
        if ($_ -contains 'age') {
            $CodeA += ', VoiceAge age';
            $CodeB += ', new object[] { gender, age }';
        } else {
            $CodeB += ', gender';
        }
    } else {
        $CodeA += ', string voice';
        $CodeB += ', voice';
    }
    if ($_ -contains 'volume') {
        $CodeA += ', int volume';
        $CodeB += ', volume';
    } else {
        $CodeB += ', null';
    }
    if ($_ -contains 'path') {
        $CodeA += ', string path';
        $CodeB += ', path';
    }
    if ($_ -contains 'audioDestination') {
        $CodeA += ', Stream audioDestination';
        $CodeB += ', audioDestination';
    }
    if ($_ -contains 'formatInfo') {
        $CodeA += ', SpeechAudioFormatInfo formatInfo';
        $CodeB += ', formatInfo';
    } else {
        $CodeB += ', null';
    }
    @'
    
        public void StartSpeechAsync(PromptBuilder promptBuilder{0}, object state)
        {{
            _StartSpeechAsync(promptBuilder{1}, state);
        }}
'@ -f $CodeA, $CodeB;
}#>