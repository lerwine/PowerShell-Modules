Add-Type -AssemblyName 'System.Speech';

Function New-PromptBuilder {
    <#
        .SYNOPSIS
			Create new prompt builder for speech synthesis
         
        .DESCRIPTION
			Creates a new instance of the System.Speech.Synthesis.PromptBuilder class.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptBuilder. Empty prompt builder object for adding content, selecting voices, controlling voice attributes, and controlling the pronunciation of spoken words.
        
        .LINK
            Add-PromptBuilderAudio

        .LINK
			Add-PromptBuilderBookmark

        .LINK
			Add-PromptBuilderBreak

        .LINK
			Add-SSML

        .LINK
			Add-SSMLMarkup

        .LINK
			Add-PromptBuilderText

        .LINK
			Clear-PromptBuilder

        .LINK
			Push-PromptBuilder

        .LINK
			Pop-PromptBuilder

        .LINK
			ConvertTo-SSML

        .LINK
			ConvertFrom-SSML
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.aspx
    #>
	[CmdletBinding()]
	[OutputType([System.Speech.Synthesis.PromptBuilder])]
	Param(
		# Provides information about a specific culture, such as its language, the name of the culture, the writing system, the calendar used, and how to format dates and sort strings.
		[System.Globalization.CultureInfo]$Culture
	)

	if ($PSBoundParameters.ContainsKey('Culture')) {
		New-Object -TypeName 'System.Speech.Synthesis.PromptBuilder' -ArgumentList $Culture
	} else {
		New-Object -TypeName 'System.Speech.Synthesis.PromptBuilder';
	}
}

Function New-PromptStyle {
    <#
        .SYNOPSIS
			Create new prompt style object.
         
        .DESCRIPTION
			Initializes a new instance of the System.Speech.Synthesis.PromptStyle class.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptStyle. The newly created PromptStyle object.
        
        .LINK
            Push-PromptBuilder

        .LINK
            Pop-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptstyle.aspx
    #>
	[CmdletBinding()]
	[OutputType([System.Speech.Synthesis.PromptStyle])]
	Param(
		# The setting for the speaking rate of the style.
		[System.Speech.Synthesis.PromptRate]$Rate,
		
		# The setting for the volume (loudness) of the style.
		[System.Speech.Synthesis.PromptVolume]$Volume,
		
		# The setting for the emphasis of the style.
		[System.Speech.Synthesis.PromptEmphasis]$Emphasis
	)

	if ($PSBoundParameters.ContainsKey('Rate')) {
		$PromptStyle = New-Object -TypeName 'System.Speech.Synthesis.PromptStyle' -ArgumentList $Rate;
		if ($PSBoundParameters.ContainsKey('Volume')) { $PromptStyle.Volume = $Volume }
		if ($PSBoundParameters.ContainsKey('Emphasis')) { $PromptStyle.Emphasis = $Emphasis }
	} else {
		if ($PSBoundParameters.ContainsKey('Volume')) {
			$PromptStyle = New-Object -TypeName 'System.Speech.Synthesis.PromptStyle' -ArgumentList $Volume;
			if ($PSBoundParameters.ContainsKey('Emphasis')) { $PromptStyle.Emphasis = $Emphasis }
		} else {
			if ($PSBoundParameters.ContainsKey('Emphasis')) {
				return New-Object -TypeName 'System.Speech.Synthesis.PromptStyle' -ArgumentList $Emphasis;
			}
			return New-Object -TypeName 'System.Speech.Synthesis.PromptStyle';
		}
	}
	
	$PromptStyle | Write-Output;
}

Function Add-PromptBuilderAudio {
    <#
        .SYNOPSIS
			Append audio file to prompt builder.
         
        .DESCRIPTION
			Appends the audio file at the specified URI to the System.Speech.Synthesis.PromptBuilder.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptBuilder. This is returned only if the PassThru switch is used.
        
        .LINK
            New-PromptBuilder

        .LINK
            Clear-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendaudio.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'Path')]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# The Prompt Builder to append audio to.
		[System.Speech.Synthesis.PromptBuilder]$PromptBuilder,

		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Path')]
		# A fully qualified path to the audio file.
		[string]$Path,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Uri')]
		[Alias('Uri')]
		# URI for the audio file.
		[System.Uri]$AudioFile,
		
		[Parameter(Position = 2, ParameterSetName = 'Uri')]
		[Alias('Alt')]
		# A string containing alternate text representing the audio.
		[string]$AlternateText,

		# If this switch is set, then the PromptBuilder object is returned.
		[switch]$PassThru
	)
	
	Process {
		if ($PSCmdlet.ParameterSetName -eq 'Path') {
			$PromptBuilder.AppendAudio($Path);
		} else {
			if ($PSBoundParameters.ContainsKey('AlternateText')) {
				$PromptBuilder.AppendAudio($AudioFile, $AlternateText);
			} else {
				$PromptBuilder.AppendAudio($AudioFile);
			}
		}
	}

	End {
		if ($PassThru) { $PromptBuilder | Write-Output }
	}
}

Function Add-PromptBuilderBookmark {
    <#
        .SYNOPSIS
			Append bookmark to prompt builder.
         
        .DESCRIPTION
			Appends a bookmark to the System.Speech.Synthesis.PromptBuilder object.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptBuilder. This is returned only if the PassThru switch is used.
        
        .LINK
            New-PromptBuilder

        .LINK
            Clear-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendbookmark.aspx
    #>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# The Prompt Builder to append bookmarks to.
		[System.Speech.Synthesis.PromptBuilder]$PromptBuilder,

		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true)]
		# A string containing the name of the appended bookmark.
		[string]$BookmarkName,
		
		# If this switch is set, then the PromptBuilder object is returned.
		[switch]$PassThru
	)
	
	Process { $PromptBuilder.AppendBookmark($BookmarkName); }

	End {
		if ($PassThru) { $PromptBuilder | Write-Output }
	}
}

Function Add-PromptBuilderBreak {
    <#
        .SYNOPSIS
			Append break to prompt builder.
         
        .DESCRIPTION
			Appends a break to the System.Speech.Synthesis.PromptBuilder object.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptBuilder. This is returned only if the PassThru switch is used.
        
        .LINK
            New-PromptBuilder

        .LINK
            Clear-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendbreak.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'Strength')]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# The Prompt Builder to append breaks to.
		[System.Speech.Synthesis.PromptBuilder]$PromptBuilder,

		[Parameter(Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Strength')]
		# Specifies strength (duration) of break.
		[System.Speech.Synthesis.PromptBuilderBreak]$Strength,

		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Duration')]
		# Duration of break.
		[System.TimeSpan]$Duration,
		
		# If this switch is set, then the PromptBuilder object is returned.
		[switch]$PassThru
	)
	
	Process {
		if ($PSCmdlet.ParameterSetName -eq 'Duration') {
			$PromptBuilder.AppendBreak($Duration);
		} else {
			if ($PSBoundParameters.ContainsKey('Strength')) {
				$PromptBuilder.AppendBreak($Strength);
			} else {
				$PromptBuilder.AppendBreak();
			}
		}
	}

	End {
		if ($PassThru) { $PromptBuilder | Write-Output }
	}
}

Function Add-SSML {
    <#
        .SYNOPSIS
			Append SSML Markup to prompt builder.
         
        .DESCRIPTION
			Appends an object that references an SSML prompt to the System.Speech.Synthesis.PromptBuilder object.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptBuilder. This is returned only if the PassThru switch is used.
        
        .LINK
            New-PromptBuilder

        .LINK
            Clear-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendssml.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'Path')]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# The Prompt Builder to append SSML Markup to.
		[System.Speech.Synthesis.PromptBuilder]$PromptBuilder,

		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Path')]
		# A fully qualified path to the SSML file to append.
		[string]$Path,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Uri')]
		[Alias('Uri')]
		# A fully qualified URI to the SSML file to append.
		[System.Uri]$SSMLFile,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Reader')]
		[Alias('Reader')]
		# A System.Xml.XmlReader containing SSML markup to append.
		[System.Xml.XmlReader]$XmlReader,
		
		# If this switch is set, then the PromptBuilder object is returned.
		[switch]$PassThru
	)
	
	Process {
		switch ($PSCmdlet.ParameterSetName) {
			'Path' { $PromptBuilder.AppendSsml($Path); break; }
			'Uri' { $PromptBuilder.AppendSsml($SSMLFile); break; }
			default { $PromptBuilder.AppendSsml($XmlReader); break; }
		}
	}

	End {
		if ($PassThru) { $PromptBuilder | Write-Output }
	}
}

Function Add-SSMLMarkup {
    <#
        .SYNOPSIS
			Append SSML Markup to prompt builder.
         
        .DESCRIPTION
			Appends an object that references an SSML prompt to the System.Speech.Synthesis.PromptBuilder object.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptBuilder. This is returned only if the PassThru switch is used.
        
        .LINK
            New-PromptBuilder

        .LINK
            Clear-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendssmlmarkup.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendssml.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'String')]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# The Prompt Builder to append SSML Markup to.
		[System.Speech.Synthesis.PromptBuilder]$PromptBuilder,
		
		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'String')]
		[Alias('Markup')]
		# A string containing SSML markup.
		[string]$SsmlMarkup,
		
		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Document')]
		[Alias('XmlDocument', 'Document')]
		# An XML Document containing SSML markup.
		[System.Xml.XmlDocument]$Xml,
		
		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Element')]
		# An XML Element representing SSML markup.
		[Alias('Element')]
		[System.Xml.XmlElement]$XmlElement,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Reader')]
		[Alias('Reader')]
		# A System.Xml.XmlReader containing SSML markup to append.
		[System.Xml.XmlReader]$XmlReader,
		
		# If this switch is set, then the PromptBuilder object is returned.
		[switch]$PassThru
	)
	
	Process {
		switch ($PSCmdlet.ParameterSetName) {
			'Markup' { $PromptBuilder.AppendSsmlMarkup($SsmlMarkup); break; }
			'Document' { $PromptBuilder.AppendSsmlMarkup($XmlDocument.DocumentElement.OuterXml); break; }
			'Element' { $PromptBuilder.AppendSsmlMarkup($XmlElement.OuterXml); break; }
			default { $PromptBuilder.AppendSsml($XmlReader); break; }
		}
	}

	End {
		if ($PassThru) { $PromptBuilder | Write-Output }
	}
}

Function Add-PromptBuilderText {
    <#
        .SYNOPSIS
			Append text to prompt builder.
         
        .DESCRIPTION
			Appends text to the System.Speech.Synthesis.PromptBuilder object.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptBuilder. This is returned only if the PassThru switch is used.
        
        .LINK
            New-PromptBuilder

        .LINK
            Clear-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendtext.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendtextwithalias.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendtextwithhint.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendtextwithpronunciation.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'Emphasis')]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# The Prompt Builder to append SSML Markup to.
		[System.Speech.Synthesis.PromptBuilder]$PromptBuilder,
		
		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Emphasis')]
		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Volume')]
		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Rate')]
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Substitute')]
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'SayAs')]
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'As')]
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Pronunciation')]
		[Alias('Text')]
		# A string containing the text to be spoken.
		[string]$TextToSpeak,
		
		[Parameter(Position = 2, ParameterSetName = 'Emphasis')]
		# Specifies the degree of emphasis for the text
		[System.Speech.Synthesis.PromptEmphasis]$Emphasis,
		
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Volume')]
		# Specifies the volume to speak the text.
		[System.Speech.Synthesis.PromptVolume]$Volume,

		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Rate')]
		# Specifies the speaking rate for the text.
		[System.Speech.Synthesis.PromptRate]$Rate,

		[Parameter(Mandatory = $true, Position = 2,ParameterSetName = 'Substitute')]
		# A string containing the text to be spoken in place of TextToSpeak
		[string]$Substitute,

		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'As')]
		# The content type of the text.
		[System.Speech.Synthesis.SayAs]$As,

		[Parameter(Mandatory = $true, Position = 2,ParameterSetName = 'SayAs')]
		# The content type of the text.
		[string]$SayAs,

		[Parameter(Mandatory = $true, Position = 2,ParameterSetName = 'Pronunciation')]
		# A string containing phones to be spoken from the International Phonetic Alphabet (IPA).
		[string]$Pronunciation,
		
		# If this switch is set, then the PromptBuilder object is returned.
		[switch]$PassThru
	)
	
	Process {
		switch ($PSCmdlet.ParameterSetName) {
			'Volume' { $PromptBuilder.AppendText($TextToSpeak, $Volume); break; }
			'Rate' { $PromptBuilder.AppendText($TextToSpeak, $Rate); break; }
			'Substitute' { $PromptBuilder.AppendTextWithAlias($TextToSpeak, $Substitute); break; }
			'As' { $PromptBuilder.AppendTextWithHint($TextToSpeak, $As); break; }
			'SayAs' { $PromptBuilder.AppendTextWithHint($TextToSpeak, $SayAs); break; }
			'Pronunciation' { $PromptBuilder.AppendTextWithPronunciation($TextToSpeak, $Pronunciation); break; }
			default {
				if ($PSBoundParameters.ContainsKey('Emphasis')) {
					$PromptBuilder.AppendText($TextToSpeak, $Emphasis);
				} else {
					$PromptBuilder.AppendText($TextToSpeak);
				}
				break;
			}
		}
	}

	End {
		if ($PassThru) { $PromptBuilder | Write-Output }
	}
}

Function Clear-PromptBuilder {
    <#
        .SYNOPSIS
			Clear content from prompt builder.
         
        .DESCRIPTION
			Clears the content from the System.Speech.Synthesis.PromptBuilder object.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptBuilder. This is returned only if the PassThru switch is used.
        
        .LINK
            New-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.clearcontent.aspx
    #>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# The Prompt Builder to be cleared.
		[System.Speech.Synthesis.PromptBuilder]$PromptBuilder,
		
		# If this switch is set, then the PromptBuilder object is returned.
		[switch]$PassThru
	)
	
	Process {
		$PromptBuilder.ClearContent();
		if ($PassThru) { $PromptBuilder | Write-Output }
	}
}

Function Join-PromptBuilder {
    <#
        .SYNOPSIS
			Combines prompt builder objects.
         
        .DESCRIPTION
			Combines System.Speech.Synthesis.PromptBuilder objects into one System.Speech.Synthesis.PromptBuilder object.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptBuilder. This is the System.Speech.Synthesis.PromptBuilder containing all combined prompt builders.
        
        .LINK
            New-PromptBuilder

        .LINK
            Clear-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendpromptbuilder.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.appendbreak.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.startsentence.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.endsentence.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.startparagraph.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.endparagraph.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'Gender')]
	[OutputType([System.Speech.Synthesis.PromptBuilder])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[Alias('PromptBuilders')]
		# The Prompt Builders to be joined.
		[System.Speech.Synthesis.PromptBuilder[]]$PromptBuilder,
		
		[Parameter(Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Strength')]
		# Specifies strength (duration) of break between joined PromptBuilder objects.
		[System.Speech.Synthesis.PromptBuilderBreak]$Break,

		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Duration')]
		# Duration of break between each joined PromptBuilder object.
		[System.TimeSpan]$Duration,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Paragraph')]
		# Specifies that a paragraph should be started before each (and ended after each) System.Speech.Synthesis.PromptBuilder object.
		[switch]$Paragraph,

		[Parameter(Mandatory = $true, ParameterSetName = 'Sentence')]
		# Specifies that a sentence should be started before each (and ended after each) System.Speech.Synthesis.PromptBuilder object.
		[switch]$Sentence,
		
		# PromptBuilder to be appended to. If this is not specified, then a new PromptBuilder object will be used.
		[System.Speech.Synthesis.PromptBuilder]$Target
	)
	
	Begin {
		$LastPromptBuilder = $null;
		if (-not $PSBoundParameters.ContainsKey('Target')) { $Target = New-PromptBuilder }
	}

	Process {
		foreach ($p in $PromptBuilder) {
			if ($LastPromptBuilder -ne $null) {
				switch ($PSCmdlet.ParameterSetName) {
					'Duration' {
						if (-not $Target.IsEmpty) { $Target.AppendBreak($Duration) }
						$Target.AppendPromptBuilder($LastPromptBuilder);
						break;
					}
					'Paragraph' {
						$Target.StartParagraph();
						$Target.AppendPromptBuilder($LastPromptBuilder);
						$Target.EndParagraph();
					}
					'Sentence' {
						$Target.StartSentence();
						$Target.AppendPromptBuilder($LastPromptBuilder);
						$Target.EndSentence();
					}
					default {
						if ($PSBoundParameters.ContainsKey('Break') -and -not $Target.IsEmpty) { $Target.AppendBreak($Break) } 
						$Target.AppendPromptBuilder($LastPromptBuilder);
					}
				}
			}
			$LastPromptBuilder = $p;
		}
	}

	End {
		if ($Target.IsEmpty) {
			$Target.AppendPromptBuilder($LastPromptBuilder);
		} else {
			switch ($PSCmdlet.ParameterSetName) {
				'Duration' {
					$Target.AppendBreak($Duration);
					$Target.AppendPromptBuilder($LastPromptBuilder);
					break;
				}
				'Paragraph' {
					$Target.StartParagraph();
					$Target.AppendPromptBuilder($LastPromptBuilder);
					$Target.EndParagraph();
				}
				'Sentence' {
					$Target.StartSentence();
					$Target.AppendPromptBuilder($LastPromptBuilder);
					$Target.EndSentence();
				}
				default {
					if ($PSBoundParameters.ContainsKey('Break')) { $Target.AppendBreak($Break) } 
					$Target.AppendPromptBuilder($LastPromptBuilder);
				}
			}
		}

		$Target | Write-Output;
	}
}

Function Push-PromptBuilder {
    <#
        .SYNOPSIS
			Start new sentence, paragraph, voice, gender, style or culture.
         
        .DESCRIPTION
			Specifies the start of a sentence, paragraph, voice, gender, style or culture in the System.Speech.Synthesis.PromptBuilder object.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptBuilder. This is returned only if the PassThru switch is used.
        
        .LINK
            New-PromptStyle
			
        .LINK
			Get-InstalledVoices

        .LINK
            Pop-PromptBuilder

        .LINK
            Clear-PromptBuilder

        .LINK
            New-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.startsentence.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.startparagraph.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.startstyle.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.startvoice.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'Gender')]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# The Prompt Builder to be appended to.
		[System.Speech.Synthesis.PromptBuilder]$PromptBuilder,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'VoiceInfo')]
		[Alias('VoiceInfo')]
		# Instructs the synthesizer to change the voice in the System.Speech.Synthesis.PromptBuilder object according to the Voice criteria.
		[System.Speech.Synthesis.VoiceInfo]$Voice,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Gender')]
		# Instructs the synthesizer to change the voice in the System.Speech.Synthesis.PromptBuilder object according to the specified Gender.
		[System.Speech.Synthesis.VoiceGender]$Gender,
		
		[Parameter(Position = 2, ParameterSetName = 'Gender')]
		# Instructs the synthesizer to change the voice in the System.Speech.Synthesis.PromptBuilder object according to the specified Age.
		[System.Speech.Synthesis.VoiceAge]$Age,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Name')]
		[Alias('VoiceName')]
		# Instructs the synthesizer to change the voice in the System.Speech.Synthesis.PromptBuilder object according to the name of the voice specified.
		[string]$Name,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Style')]
		# Instructs the synthesizer to change the style in the System.Speech.Synthesis.PromptBuilder object according to the Style criteria.
		[System.Speech.Synthesis.PromptStyle]$Style,
		
		[Parameter(Position = 1, ParameterSetName = 'VoiceCulture')]
		# Instructs the synthesizer to change the voice culture in the System.Speech.Synthesis.PromptBuilder object according to the VoiceCulture criteria.
		[System.Globalization.CultureInfo]$VoiceCulture,

		[Parameter(Position = 1, ParameterSetName = 'Paragraph')]
		[Parameter(Position = 1, ParameterSetName = 'Sentence')]
		# Specifies the culture for the new sentence or paragraph.
		[System.Globalization.CultureInfo]$Culture,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Paragraph')]
		# Specifies the start of a paragraph in the System.Speech.Synthesis.PromptBuilder object.
		[switch]$Paragraph,

		[Parameter(Mandatory = $true, ParameterSetName = 'Sentence')]
		# Specifies the start of a sentence in the System.Speech.Synthesis.PromptBuilder object.
		[switch]$Sentence,
		
		# If this switch is set, then the PromptBuilder object is returned.
		[switch]$PassThru
	)
	
	switch ($PSCmdlet.ParameterSetName) {
		'VoiceInfo' { $PromptBuilder.StartVoice($Voice); break; }
		'Name' { $PromptBuilder.StartVoice($Name); break; }
		'VoiceCulture' { $PromptBuilder.StartVoice($VoiceCultures); break; }
		'Paragraph' {
			if ($PSBoundParameters.ContainsKey('Culture')) {
				$PromptBuilder.StartParagraph($Culture);
			} else {
				$PromptBuilder.StartParagraph();
			}
			break;
		}
		'Sentence' {
			if ($PSBoundParameters.ContainsKey('Culture')) {
				$PromptBuilder.StartSentence($Culture);
			} else {
				$PromptBuilder.StartSentence();
			}
			break;
		}
		default {
			if ($PSBoundParameters.ContainsKey('Age')) {
				$PromptBuilder.StartVoice($Gender, $Age);
			} else {
				$PromptBuilder.StartVoice($Gender);
			}
			break;
		}
	}

	if ($PassThru) { $PromptBuilder | Write-Output }
}

Function Pop-PromptBuilder {
    <#
        .SYNOPSIS
			End a sentence, paragraph, voice, gender, style or culture.
         
        .DESCRIPTION
			Specifies the end of a sentence, paragraph, voice, gender, style or culture in the System.Speech.Synthesis.PromptBuilder object.
        
        .OUTPUTS
			System.Speech.Synthesis.PromptBuilder. This is returned only if the PassThru switch is used.
        
        .LINK
            Push-PromptBuilder

        .LINK
            Clear-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.endsentence.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.endparagraph.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.endstyle.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.endvoice.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'Voice')]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# The Prompt Builder to be appended to.
		[System.Speech.Synthesis.PromptBuilder]$PromptBuilder,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Paragraph')]
		# Specifies the end of a paragraph in the System.Speech.Synthesis.PromptBuilder object.
		[switch]$Paragraph,

		[Parameter(Mandatory = $true, ParameterSetName = 'Sentence')]
		# Specifies the end of a sentence in the System.Speech.Synthesis.PromptBuilder object.
		[switch]$Sentence,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Style')]
		# Specifies the end of a style in the System.Speech.Synthesis.PromptBuilder object.
		[switch]$Style,

		[Parameter(Mandatory = $true, ParameterSetName = 'Voice')]
		[Alias('VoiceInfo', 'Gender', 'Name', 'VoiceName', 'VoiceCulture')]
		# Specifies the end of a voice, gender, or voice culture in the System.Speech.Synthesis.PromptBuilder object.
		[switch]$Voice,
		
		# If this switch is set, then the PromptBuilder object is returned.
		[switch]$PassThru
	)

	switch ($PSCmdlet.ParameterSetName) {
		'Paragraph' { $PromptBuilder.EndParagraph(); break; }
		'Sentence' { $PromptBuilder.EndSentence(); break; }
		'Style' { $PromptBuilder.EndStyle(); break; }
		default { $PromptBuilder.EndVoice(); break; }
	}

	if ($PassThru) { $PromptBuilder | Write-Output }
}

Function ConvertTo-SSML {
    <#
        .SYNOPSIS
			Convert Prompt Builder to SSML markup.
         
        .DESCRIPTION
			Returns the SSML Markup generated from the System.Speech.Synthesis.PromptBuilder object.
        
        .OUTPUTS
			System.Xml.XmlDocument. The Xml Document containing th e SSML markkup.
        
        .LINK
            New-PromptBuilder

        .LINK
            ConvertFrom-SSML

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.toxml.aspx
    #>
	[CmdletBinding()]
	[OutputType([System.Xml.XmlDocument])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# The Prompt Builder from which to get the SSML Markup.
		[System.Speech.Synthesis.PromptBuilder]$PromptBuilder
	)
	
	Process {
		$XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
		$XmlDocument.LoadXml($PromptBuilder.ToXml());
		$XmlDocument | Write-Output;
	}
}

Function ConvertFrom-SSML {
    <#
        .SYNOPSIS
			Creates a new Prompt Builder from SSML markup.
         
        .DESCRIPTION
			Returns a new System.Speech.Synthesis.PromptBuilder object from the specified SSML Markup.
        
        .OUTPUTS
			System.Xml.XmlDocument. The Xml Document containing the SSML markkup.
        
        .LINK
            ConvertTo-SSML
			
        .LINK
            New-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.promptbuilder.toxml.aspx
    #>
	[CmdletBinding()]
	[OutputType([System.Speech.Synthesis.PromptBuilder])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[Alias('XmlDocument', 'Document')]
		# The XML document containing the SSML Markup.
		[System.Xml.XmlDocument]$Xml
	)
	
	Begin {
		$PromptBuilder = New-Object -TypeName 'System.Speech.Synthesis.PromptBuilder';
	}
	Process {
		Add-SSMLMarkup -PromptBuilder $PromptBuilder -Xml $Xml;
		$XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
		$XmlDocument.LoadXml($PromptBuilder.ToXml());
		$XmlDocument | Write-Output;
	}
	End { $PromptBuilder | Write-Output }
}

Function New-SpeechSynthesizer {
    <#
        .SYNOPSIS
			Create new prompt queue for speech synthesis
         
        .DESCRIPTION
			Creates a new instance of the TextToSpeechCLR.SpeechQueue class to queue speech prompt objects.
        
        .OUTPUTS
			System.Management.Automation.PSObject. A custom object representing the speech synthesizer and queue.
        
        .LINK
			New-PromptBuilder

        .LINK
			Get-InstalledVoices

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.speechsynthesizer.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'ToDefaultAudioDevice')]
	[OutputType([System.Speech.Synthesis.SpeechSynthesizer])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'ToWaveFile')]
		[Alias('WavFile', 'WaveFile')]
		# Send output to a file that contains Waveform format audio.
		[stringm]$Path,
		
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'ToAudioStream')]
		[Alias('Stream', 'AudioStream')]
		# Send output to an audio stream.
		[System.Stream]$AudioDestination,
		
		[Parameter(Position = 1, ParameterSetName = 'ToWaveFile')]
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'ToAudioStream')]
		# The format to use for the synthesis output.
		[System.Speech.AudioFormat.SpeechAudioFormatInfo]$FormatInfo,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ToNull')]
		# Do not send output from synthesis operations to a device, file, or stream.
		[switch]$SetOutputToNull,
		
		[Parameter(ParameterSetName = 'ToDefaultAudioDevice')]
		# Send output to the default audio device. This is the default behavior.
		[switch]$DefaultAudioDevice
		
	)

	switch ($PSCmdlet.ParameterSetName) {
		'ToWaveFile' {
			if ($PSBoundParameters.ContainsKey('FormatInfo')) {
				[TextToSpeechCLR.SpeechHelper]::SpeechSynthesizerToWaveFile($Path, $FormatInfo);
			} else {
				[TextToSpeechCLR.SpeechHelper]::SpeechSynthesizerToWaveFile($Path);
			}
			break;
		}
		'ToAudioStream' { [TextToSpeechCLR.SpeechHelper]::SpeechSynthesizerToAudioStream($AudioDestination, $FormatInfo); break; }
		'ToNull' { [TextToSpeechCLR.SpeechHelper]::SpeechSynthesizerToNull(); break; }
		default { [TextToSpeechCLR.SpeechHelper]::SpeechSynthesizerToDefaultAudioDevice(); break; }
	}
}

Function Get-InstalledVoices {
    <#
        .SYNOPSIS
			Creates a new Prompt Builder from SSML markup.
         
        .DESCRIPTION
			Returns a read-only collection of the voices currently installed on the system.
        
        .OUTPUTS
			System.Xml.XmlDocument. The Xml Document containing the SSML markkup.
        
        .LINK
            ConvertTo-SSML
			
        .LINK
            New-PromptBuilder

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.speechsynthesizer.getinstalledvoices.aspx
    #>
	[CmdletBinding()]
	[OutputType([System.Speech.Synthesis.VoiceInfo[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[Alias('Queue', 'SpeechSynthesizer')]
		[ValidateScript({ $_.BaseObject -is [TextToSpeechCLR.SpeechQueue] -or  $_.BaseObject -is [System.Speech.Synthesis.SpeechSynthesizer] })]
		# The XML document containing the SSML Markup.
		[System.Management.Automation.PSObject]$SpeechQueue,

		# The locale that the voice must support.
		[System.Globalization.CultureInfo]$Culture,

		# Get disabled voices
		[switch]$Disabled
	)
	
	[TextToSpeechCLR.SpeechQueue]::GetInstalledVoices($SpeechQueue, $Culture, $Disabled.IsPresent);
}

Function Invoke-PauseSpeech {
    <#
        .SYNOPSIS
			Pauses speech synthesis.
         
        .DESCRIPTION
			Pauses the System.Speech.Synthesis.SpeechSynthesizer object and queue.
        
        .LINK
            Invoke-ResumeSpeech

        .LINK
            New-SpeechQueue

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.speechsynthesizer.pause.aspx
    #>
	[CmdletBinding()]
	[OutputType([System.Speech.Synthesis.PromptBuilder])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[Alias('Queue', 'SpeechSynthesizer')]
		[ValidateScript({ $_.BaseObject -is [TextToSpeechCLR.SpeechQueue] -or  $_.BaseObject -is [System.Speech.Synthesis.SpeechSynthesizer] })]
		# The XML document containing the SSML Markup.
		[System.Management.Automation.PSObject]$SpeechQueue
	)
	
	[TextToSpeechCLR.SpeechQueue]::Pause($SpeechQueue);
}

Function Invoke-ResumeSpeech {
    <#
        .SYNOPSIS
			Resumes paused speech synthesis.
         
        .DESCRIPTION
			Resumes the System.Speech.Synthesis.SpeechSynthesizer object and queue.
        
        .LINK
            Invoke-PauseSpeech

        .LINK
            New-SpeechQueue

        .LINK
            https://msdn.microsoft.com/en-us/library/system.speech.synthesis.speechsynthesizer.resume.aspx
    #>
	[CmdletBinding()]
	[OutputType([System.Speech.Synthesis.PromptBuilder])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[Alias('Queue', 'SpeechSynthesizer')]
		[ValidateScript({ $_.BaseObject -is [TextToSpeechCLR.SpeechQueue] -or  $_.BaseObject -is [System.Speech.Synthesis.SpeechSynthesizer] })]
		# The XML document containing the SSML Markup.
		[System.Management.Automation.PSObject]$SpeechQueue
	)
	
	[TextToSpeechCLR.SpeechQueue]::Resume($SpeechQueue);
}