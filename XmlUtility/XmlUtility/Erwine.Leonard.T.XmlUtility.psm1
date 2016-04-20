Add-Type -Path (('SchemaSetCollection.cs', 'SchemaValidationError.cs', 'SchemaValidationHandler.cs') | ForEach-Object { $PSScriptRoot | Join-Path -ChildPath $_ }) `
	-ReferencedAssemblies 'System.Management.Automation', 'System.Xml';

Function New-XmlReaderSettings {
	<#
		.SYNOPSIS
			Creates an instance of the XmlReaderSettings class.
 
		.DESCRIPTION
			Initializes a new instance of the XmlReaderSettings class for use with an xml reader.
        
		.OUTPUTS
			System.Xml.XmlReaderSettings. Specifies a set of features to support on the XmlReader object created by the Create method.
        
        .LINK
            Read-XmlDocument
        
        .LINK
            New-XmlWriterSettings
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlreadersettings.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlnametable.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.conformancelevel.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.validationtype.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.schema.xmlschemavalidationflags.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.schema.xmlschemaset.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'New')]
    [OutputType([System.Xml.XmlReaderSettings])]
	Param(
        # Sets the XmlNameTable used for atomized string comparisons.
        [System.Xml.XmlNameTable]$NameTable,
        
        # Sets line number offset of the XmlReader object.
        [Int32]$LineNumberOffset,
        
        # Sets line position offset of the XmlReader object.
        [Int32]$LinePositionOffset,
        
        # Sets the level of conformance which the XmlReader will comply.
        [System.Xml.ConformanceLevel]$ConformanceLevel,
        
        # Sets a value indicating whether to do character checking.
        [bool]$CheckCharacters,
        
        # Sets a value indicating the maximum allowable number of characters in an XML document. A zero (0) value means no limits on the size of the XML document. A non-zero value specifies the maximum size, in characters.
        [Int64]$MaxCharactersInDocument,
        
        # Sets a value indicating the maximum allowable number of characters in a document that result from expanding entities.
        [Int64]$MaxCharactersFromEntities,
        
        # Sets a value indicating whether the XmlReader will perform validation or type assignment when reading.
        [System.Xml.ValidationType]$ValidationType,
        
        # Sets a value indicating the schema validation settings. This setting applies to XmlReader objects that validate schemas (ValidationType property set to ValidationType.Schema).
        [System.Xml.Schema.XmlSchemaValidationFlags]$ValidationFlags,
        
        # Sets the XmlSchemas to use when performing schema validation.
        [System.Xml.Schema.XmlSchema[]]$Schema,
        
        # Sets a value indicating whether to ignore insignificant white space.
        [bool]$IgnoreWhitespace,
        
        # Sets a value indicating whether to ignore processing instructions.
        [bool]$IgnoreProcessingInstructions,
        
        # Sets a value indicating whether to ignore comments.
        [bool]$IgnoreComments,
        
        # Sets a value indicating whether to prohibit document type definition (DTD) processing.
        [bool]$ProhibitDtd,
        
        # Sets a value indicating whether the underlying stream or TextReader should be closed when the reader is closed.
        [bool]$CloseInput,
        
        # XmlReaderSettings to be cloned.
        [System.Xml.XmlReaderSettings]$Settings
	)
    
    if ($PSBoundParameters.ContainsKey('Settings')) {
        $XmlReaderSettings = $Settings.Clone();
    } else {
        $XmlReaderSettings = New-Object -TypeName 'System.Xml.XmlReaderSettings';
    }
    if ($PSBoundParameters.ContainsKey('NameTable')) { $XmlReaderSettings.NameTable = $NameTable }
    if ($PSBoundParameters.ContainsKey('LineNumberOffset')) { $XmlReaderSettings.LineNumberOffset = $LineNumberOffset }
    if ($PSBoundParameters.ContainsKey('LinePositionOffset')) { $XmlReaderSettings.LinePositionOffset = $LinePositionOffset }
    if ($PSBoundParameters.ContainsKey('ConformanceLevel')) { $XmlReaderSettings.ConformanceLevel = $ConformanceLevel }
    if ($PSBoundParameters.ContainsKey('CheckCharacters')) { $XmlReaderSettings.CheckCharacters = $CheckCharacters }
    if ($PSBoundParameters.ContainsKey('MaxCharactersInDocument')) { $XmlReaderSettings.MaxCharactersInDocument = $MaxCharactersInDocument }
    if ($PSBoundParameters.ContainsKey('MaxCharactersFromEntities')) { $XmlReaderSettings.MaxCharactersFromEntities = $MaxCharactersFromEntities }
    if ($PSBoundParameters.ContainsKey('ValidationType')) { $XmlReaderSettings.ValidationType = $ValidationType }
    if ($PSBoundParameters.ContainsKey('ValidationFlags')) { $XmlReaderSettings.ValidationFlags = $ValidationFlags }
    if ($PSBoundParameters.ContainsKey('Schema')) { foreach ($XmlSchema in $Schema) { $XmlReaderSettings.Schemas.Add($XmlSchema) } }
    if ($PSBoundParameters.ContainsKey('IgnoreWhitespace')) { $XmlReaderSettings.IgnoreWhitespace = $IgnoreWhitespace }
    if ($PSBoundParameters.ContainsKey('IgnoreProcessingInstructions')) { $XmlReaderSettings.IgnoreProcessingInstructions = $IgnoreProcessingInstructions }
    if ($PSBoundParameters.ContainsKey('IgnoreComments')) { $XmlReaderSettings.IgnoreComments = $IgnoreComments }
    if ($PSBoundParameters.ContainsKey('ProhibitDtd')) { $XmlReaderSettings.ProhibitDtd = $ProhibitDtd }
    if ($PSBoundParameters.ContainsKey('CloseInput')) { $XmlReaderSettings.CloseInput = $CloseInput }
    $XmlReaderSettings | Write-Output;
}

Function New-XmlWriterSettings {
	<#
		.SYNOPSIS
			Creates an instance of the XmlWriterSettings class.
 
		.DESCRIPTION
			Initializes a new instance of the XmlWriterSettings class for use with an xml writer.
        
		.OUTPUTS
			System.Xml.XmlWriterSettings. Specifies a set of features to support on the XmlWriter object created by the XmlWriter.Create method.
        
        .LINK
            Write-XmlDocument
        
        .LINK
            Get-TextEncoding
        
        .LINK
            New-XmlReaderSettings
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlwritersettings.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.text.encoding.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.newlinehandling.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.conformancelevel.aspx
	#>
	[CmdletBinding()]
    [OutputType([System.Xml.XmlWriterSettings])]
	Param(
        # Sets the type of text encoding to use
		[System.Text.Encoding]$Encoding,
        
        # Sets a value that indicates whether the XmlWriter should remove duplicate namespace declarations when writing XML content. The default behavior is for the writer to output all namespace declarations that are present in the writer's namespace resolver.
		[System.Xml.NewLineHandling]$NewLineHandling,
        
        # Sets the character string to use for line breaks
		[string]$NewLineChars,
        
        # Sets a value indicating whether to indent elements
        [bool]$Indent,
        
        # Sets the character string to use when indenting. This setting is used when the Indent property is set to true.
		[string]$IndentChars,
        
        # Sets a value indicating whether to write attributes on a new line
        [bool]$NewLineOnAttributes,
        
        # Sets a value indicating whether the XmlWriter should also close the underlying stream or TextWriter when the Close method is called
        [bool]$CloseOutput,
        
        # Sets the level of conformance that the XML writer checks the XML output for
		[System.Xml.ConformanceLevel]$ConformanceLevel,
        
        # Sets a value that indicates whether the XML writer should check to ensure that all characters in the document conform to the "2.2 Characters" section of the W3C XML 1.0 Recommendation
        [bool]$CheckCharacters,
        
        # Sets a value that indicates whether the XmlWriter does not escape URI attributes
        [bool]$DoNotEscapeUriAttributes,
        
        # XmlWriterSettings object to be cloned.
        [System.Xml.XmlWriterSettings]$Settings
	)
    
    if ($PSBoundParameters.ContainsKey('Settings')) {
        $XmlWriterSettings = $Settings.Clone();
    } else {
        $XmlWriterSettings = New-Object -TypeName 'System.Xml.XmlWriterSettings';
    }
    
    if ($PSBoundParameters.ContainsKey('Encoding')) { $XmlWriterSettings.Encoding = $Encoding }
    if ($PSBoundParameters.ContainsKey('OmitXmlDeclaration')) { $XmlWriterSettings.OmitXmlDeclaration = $OmitXmlDeclaration }
    if ($PSBoundParameters.ContainsKey('NewLineHandling')) { $XmlWriterSettings.NewLineHandling = $NewLineHandling }
    if ($PSBoundParameters.ContainsKey('NewLineChars')) { $XmlWriterSettings.NewLineChars = $NewLineChars }
    if ($PSBoundParameters.ContainsKey('Indent')) { $XmlWriterSettings.Indent = $Indent }
    if ($PSBoundParameters.ContainsKey('IndentChars')) { $XmlWriterSettings.IndentChars = $IndentChars }
    if ($PSBoundParameters.ContainsKey('NewLineOnAttributes')) { $XmlWriterSettings.NewLineOnAttributes = $NewLineOnAttributes }
    if ($PSBoundParameters.ContainsKey('CloseOutput')) { $XmlWriterSettings.CloseOutput = $CloseOutput }
    if ($PSBoundParameters.ContainsKey('ConformanceLevel')) { $XmlWriterSettings.ConformanceLevel = $ConformanceLevel }
    if ($PSBoundParameters.ContainsKey('CheckCharacters')) { $XmlWriterSettings.CheckCharacters = $CheckCharacters }
    if ($PSBoundParameters.ContainsKey('DoNotEscapeUriAttributes')) { $XmlWriterSettings.DoNotEscapeUriAttributes = $DoNotEscapeUriAttributes }
    $XmlWriterSettings | Write-Output;
}

Function Read-XmlDocument {
	<#
		.SYNOPSIS
			Reads XML data.
 
		.DESCRIPTION
			Initializes a new instance of the XmlDocument class and loads the XML data.
        
		.OUTPUTS
			System.Xml.XmlDocument. The xml document which was read.
        
        .LINK
            New-XmlReaderSettings
        
        .LINK
            Write-XmlDocument
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.load.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlreadersettings.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlreader.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'File')]
	[OutputType([System.Xml.XmlDocument])]
	Param(
        # The URI for the file containing the XML data. The XmlResolver object on the XmlReaderSettings object is used to convert the path to a canonical data representation.
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'File')]
        [Alias('Path')]
		[string]$InputUri,
        
        # The stream that contains the XML data.
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Stream')]
		[System.IO.Stream]$Stream,
        
        # The stream that contains the XML data.
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'TextReader')]
		[System.IO.TextReader]$TextReader,
        
        # The stream that contains the XML data.
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'XmlReader')]
		[System.Xml.XmlReader]$XmlReader,
        
        # Byte array containing the XML data.
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Bytes')]
		[byte[]]$ByteArray,
        
        # The settings for the new XmlReader instance.
		[Parameter(Position = 1)]
		[System.Xml.XmlReaderSettings]$Settings,
        
        # The base URI for the entity or document being read.
		[Parameter(Position = 2, ParameterSetName = 'File')]
		[Parameter(Position = 2, ParameterSetName = 'Stream')]
		[Parameter(Position = 2, ParameterSetName = 'TextReader')]
		[Parameter(Position = 2, ParameterSetName = 'Bytes')]
		[string]$BaseUri
	)
    
	if ($PSCmdlet.ParameterSetName -eq 'Bytes') {
		$Stream = New-Object -TypeName 'System.IO.MemoryStream' -ArgumentList (,$ByteArray);
		if ($Stream -eq $null) { return }
	}
	
	switch ($PSCmdlet.ParameterSetName) {
		'File' { $InputObj = $InputUri; break; }
		'TextReader' { $InputObj = $TextReader; break; }
		'XmlReader' { $InputObj = $XmlReader; break; }
		default { $InputObj = $Stream; break; }
	}
	
	if ($PSBoundParameters.ContainsKey('Settings')) {
		if ($PSBoundParameters.ContainsKey('BaseUri')) {
			$Reader = [System.Xml.XmlReader]::Create($InputObj, $Settings, $BaseUri);
		} else {
			$Reader = [System.Xml.XmlReader]::Create($InputObj, $Settings);
		}
	} else {
		if ($PSBoundParameters.ContainsKey('BaseUri')) {
			$Reader = [System.Xml.XmlReader]::Create($InputObj, $null, $BaseUri);
		} else {
			$Reader = $InputObj;
		}
	}

	if ($Reader -eq $null -and -not ($PSBoundParameters.ContainsKey('Settings') -or $PSBoundParameters.ContainsKey('BaseUri'))) { return }
	
	try {
        $XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
		$XmlDocument.Load($Reader);
		if ($XmlDocument.DocumentElement -ne $null) { $XmlDocument | Write-Output }
	} catch {
		throw;
	} finally {
		if ($Reader -is [System.Xml.XmlReader] -and -not $PSBoundParameters.ContainsKey('XmlReader')) { $XmlReader.Close() }
		if ($Stream -ne $null -and -not $PSBoundParameters.ContainsKey('Stream')) { $Stream.Dispose() }
	}
}

Function Write-XmlDocument {
	<#
		.SYNOPSIS
			Writes XML data with custom options.
 
		.DESCRIPTION
			Writes XML data contained within an XmlDocument object with custom output options.
        
		.OUTPUTS
			System.Byte[]. The XML data as a byte array. This is only when using the AsByteArray switch.
        
		.OUTPUTS
			System.String. The XML data as a string. This is only when using the AsString switch.
        
        .LINK
            New-XmlWriterSettings
        
        .LINK
            New-XmlDocument
            
        .LINK
            Read-XmlDocument
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlwritersettings.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlwriter.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'File')]
    [OutputType([string], ParameterSetName = 'String')]
    [OutputType([byte[]], ParameterSetName = 'Bytes')]
	Param(
        # The XmlDocument containing the XML you wish to write.
		[Parameter(Mandatory = $true, Position = 0)]
		[Alias('XmlDocument')]
		[System.Xml.XmlDocument]$Document,
        
        # The file to which you want to write.
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'File')]
        [Alias('Path')]
		[string]$OutputFileName,
        
        # The stream to which you want to write.
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Stream')]
		[System.IO.Stream]$Stream,
        
        # The System.IO.TextWriter to which you want to write.
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'TextWriter')]
		[System.IO.TextWriter]$TextWriter,
        
        # The System.Xml.XmlWriter object that you want to use as the underlying writer.
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'XmlWriter')]
		[System.Xml.XmlWriter]$XmlWriter,
        
        # The XmlWriterSettings object used to configure the new XmlWriter instance. If this is not specified, a XmlWriterSettings with default settings is used.
		[Parameter(Position = 2)]
		[System.Xml.XmlWriterSettings]$Settings,
        
        # Returns the written XML data as an array of bytes.
		[Parameter(Mandatory = $true, ParameterSetName = 'Bytes')]
        [switch]$AsByteArray,
        
        # Returns the written XML data as a string.
		[Parameter(Mandatory = $true, ParameterSetName = 'String')]
        [switch]$AsString
	)
    
	if ($AsByteArray -or $AsString) {
		$Stream = New-Object -TypeName 'System.IO.MemoryStream';
		if ($Stream -eq $null) { return }
	}
	
	switch ($PSCmdlet.ParameterSetName) {
		'File' { $OutputObj = $OutputFileName; break; }
		'TextWriter' { $OutputObj = $TextWriter; break; }
		'XmlWriter' { $OutputObj = $XmlWriter; break; }
		default { $OutputObj = $Stream; break; }
	}
	
	if ($PSBoundParameters.ContainsKey('Settings')) {
		$Writer = [System.Xml.XmlWriter]::Create($OutputObj, $Settings);
	} else {
		$Writer = $OutputObj;
	}

    if ($Writer -eq $null -and $PSBoundParameters.ContainsKey('Settings')) {
        throw 'Unable to create xml writer.';
        return;
    }
    
    try {
        $XmlDocument.WriteTo($Writer);
		if ($Writer -isnot [string]) { $XmlWriter.Flush() }
        if ($AsByteArray) {
            Write-Output -InputObject (,$Stream.ToArray());
        } else {
            if ($AsString) {
				if ($XmlDocument.FirstChild.NodeType -eq [System.Xml.XmlNodeType]::XmlDeclaration) {
					try {
						$Encoding = [System.Text.Encoding]::GetEncoding($XmlDocument.FirstChild.Encoding);
					} catch {
						$Encoding = [System.Text.Encoding]::UTF8;
					}
				} else {
					$Encoding = [System.Text.Encoding]::UTF8;
				}
				$Encoding.GetString($Stream.ToArray()) | Write-Output;
			}
        }
    } catch {
        throw;
    } finally {
		if ($Writer -is [System.Xml.XmlWriter] -and -not $PSBoundParameters.ContainsKey('XmlWriter')) { $Writer.Close() }
		if ($Stream -ne $null -and -not $PSBoundParameters.ContainsKey('Stream')) { $Stream.Dispose() }
    }
}

Function ConvertTo-XmlEncodedNmToken {
	<#
		.SYNOPSIS
			Verifies the name is valid according to the XML specification.
 
		.DESCRIPTION
			This method guarantees that the name is valid according to the XML specification.
            For example, if you passed this method the invalid name 70+, it returns 70_x002b_ which is a valid XML name.
        
		.OUTPUTS
			System.String. The encoded name.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.encodenmtoken.aspx
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
        # The name to be encoded. 
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
    
	Process { [System.Xml.XmlConvert]::EncodeNmToken($Value) }
}

Function ConvertTo-XmlEncodedName {
	<#
		.SYNOPSIS
			Converts the name to a valid XML name.
 
		.DESCRIPTION
			This method translates invalid characters, such as spaces or half-width Katakana, that need to be mapped to XML names without the support or presence of schemas.
            The invalid characters are translated into escaped numeric entity encodings.

		.OUTPUTS
			System.String. The name with any invalid characters replaced by an escape string.
        
        .LINK
            ConvertFrom-XmlEncodedName
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.encodename.aspx
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
        # A name to be translated
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
    
	Process { [System.Xml.XmlConvert]::EncodeName($Value) }
}

Function ConvertTo-XmlEncodedLocalName {
	<#
		.SYNOPSIS
			Converts the name to a valid XML local name.
 
		.DESCRIPTION
			This method is similar to the EncodeName method except that it encodes the colon character, which guarantees that the name can be used as the local name part of a namespace qualified name.
            For example, if you passed this method the invalid name a:b, it returns a_x003a_b, which is a valid local name.

		.OUTPUTS
			System.String. The encoded name.
        
        .LINK
            ConvertFrom-XmlEncodedName
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.encodelocalname.aspx
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
        # The name to be encoded
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
    
	Process { [System.Xml.XmlConvert]::EncodeLocalName($Value) }
}

Function ConvertFrom-XmlEncodedName {
	<#
		.SYNOPSIS
			Decodes a name.
 
		.DESCRIPTION
			This method does the reverse of the ConvertTo-XmlEncodedName and ConvertTo-XmlEncodedLocalName Functions.

		.OUTPUTS
			System.String. The decoded  name.
        
        .LINK
            ConvertTo-XmlEncodedName
        
        .LINK
            ConvertTo-XmlEncodedLocalName
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.decodename.aspx
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
        # The name to be transformed
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
    
	Process { [System.Xml.XmlConvert]::DecodeName($Value) }
}

Function ConvertTo-XmlString {
	<#
		.SYNOPSIS
			Converts data to XML String.
 
		.DESCRIPTION
			Converts strongly typed data to an equivalent XML String representation.

		.OUTPUTS
			System.String. The converted value.
        
        .LINK
            ConvertFrom-XmlString
        
        .LINK
            ConvertTo-XmlBinary
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.tostring.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'DateTime')]
	[OutputType([string])]
	Param(
        # Converts boolean value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Boolean')]
        [System.Boolean]$Boolean,

        # Converts char value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Char')]
        [System.Char]$Char,

        # Converts decimal value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Decimal')]
        [System.Decimal]$Decimal,

        # Converts signed byte value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'SByte')]
        [System.SByte]$SByte,

        # Converts short value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Int16')]
        [Alias('Int16')]
        [System.Int16]$Short,

        # Converts integer value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Int32')]
        [Alias('Int32')]
        [System.Int32]$Int,

        # Converts long value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Int64')]
        [Alias('Int64')]
        [System.Int64]$Long,

        # Converts unsigned byte value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Byte')]
        [System.Byte]$Byte,

        # Converts unsigned short value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'UInt16')]
        [Alias('UInt16')]
        [System.UInt16]$UShort,

        # Converts unsigned integer value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'UInt32')]
        [Alias('UInt32')]
        [System.UInt32]$UInt,

        # Converts unsigned long value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'UInt64')]
        [Alias('UInt64')]
        [System.UInt64]$ULong,

        # Converts float value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Single')]
        [Alias('Single')]
        [float]$Float,

        # Converts double value to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Double')]
        [double]$Double,

        # Converts TimeSpan to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'TimeSpan')]
        [System.TimeSpan]$TimeSpan,
        
        # Converts DateTime to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'DateTime')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'DateTimeOption')]
        [System.DateTime]$DateTime,

        # DateTime value serialization option
        [Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOption')]
        [System.Xml.XmlDateTimeSerializationMode]$DateTimeOption,

        # Converts DateTime Offset to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'DateTimeOffset')]
        [System.DateTimeOffset]$DateTimeOffset,

        # Format for DateTime string value
        [Parameter(ParameterSetName = 'DateTime')]
        [Parameter(ParameterSetName = 'DateTimeOffset')]
        [System.String]$Format = 'yyyy-MM-ddTHH:mm:sszzzzzz',

        # Converts Guid to XML string
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Guid')]
        [System.Guid]$Guid
    )

    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'Boolean' {
                [System.Xml.XmlConvert]::ToString($Boolean);
                break;
            }
            'Char' {
                [System.Xml.XmlConvert]::ToString($Char);
                break;
            }
            'Decimal' {
                [System.Xml.XmlConvert]::ToString($Decimal);
                break;
            }
            'SByte' {
                [System.Xml.XmlConvert]::ToString($SByte);
                break;
            }
            'Int16' {
                [System.Xml.XmlConvert]::ToString($Short);
                break;
            }
            'Int32' {
                [System.Xml.XmlConvert]::ToString($Int);
                break;
            }
            'Int64' {
                [System.Xml.XmlConvert]::ToString($Long);
                break;
            }
            'Byte' {
                [System.Xml.XmlConvert]::ToString($Byte);
                break;
            }
            'UInt16' {
                [System.Xml.XmlConvert]::ToString($UShort);
                break;
            }
            'UInt32' {
                [System.Xml.XmlConvert]::ToString($UInt);
                break;
            }
            'UInt64' {
                [System.Xml.XmlConvert]::ToString($ULong);
                break;
            }
            'Single' {
                [System.Xml.XmlConvert]::ToString($Float);
                break;
            }
            'Double' {
                [System.Xml.XmlConvert]::ToString($Double);
                break;
            }
            'TimeSpan' {
                [System.Xml.XmlConvert]::ToString($TimeSpan);
                break;
            }
            'DateTimeOption' {
                [System.Xml.XmlConvert]::ToString($DateTime, $DateTimeOption);
                break;
            }
            'DateTimeOffset' {
                [System.Xml.XmlConvert]::ToString($DateTimeOffset, $Format);
                break;
            }
            'Guid' {
                [System.Xml.XmlConvert]::ToString($Guid);
                break;
            }
            default {
                [System.Xml.XmlConvert]::ToString($DateTime, $Format);
                break;
            }
        }
    }
}

Function ConvertFrom-XmlString {
	<#
		.SYNOPSIS
			Converts data from XML String.
 
		.DESCRIPTION
			Converts an XML string representation to strongly typed data.

		.OUTPUTS
			System.Object. The converted data.
        
        .LINK
            ConvertTo-XmlString
        
        .LINK
            ConvertFrom-XmlBinary
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'DateTime')]
    [OutputType([System.Boolean], ParameterSetName = 'Boolean')]
    [OutputType([System.Char], ParameterSetName = 'Char')]
    [OutputType([System.Decimal], ParameterSetName = 'Decimal')]
    [OutputType([System.SByte], ParameterSetName = 'SByte')]
    [OutputType([System.Int16], ParameterSetName = 'Int16')]
    [OutputType([System.Int32], ParameterSetName = 'Int32')]
    [OutputType([System.Int64], ParameterSetName = 'Int64')]
    [OutputType([System.Byte], ParameterSetName = 'Byte')]
    [OutputType([System.UInt16], ParameterSetName = 'UInt16')]
    [OutputType([System.UInt32], ParameterSetName = 'UInt32')]
    [OutputType([System.UInt64], ParameterSetName = 'UInt64')]
    [OutputType([System.Single], ParameterSetName = 'Single')]
    [OutputType([System.Double], ParameterSetName = 'Double')]
    [OutputType([System.TimeSpan], ParameterSetName = 'TimeSpan')]
    [OutputType([System.DateTime], ParameterSetName = 'DateTime')]
    [OutputType([System.DateTimeOffset], ParameterSetName = 'DateTimeOffset')]
    [OutputType([System.Guid], ParameterSetName = 'Guid')]
    Param(
        # Converts an XML string to a Boolean value.
        [Parameter(Mandatory = $true, ParameterSetName = 'Boolean')]
        [switch]$Boolean,

        # Converts an XML string to a Char value.
        [Parameter(Mandatory = $true, ParameterSetName = 'Char')]
        [switch]$Char,

        # Converts an XML string to a Decimal value.
        [Parameter(Mandatory = $true, ParameterSetName = 'Decimal')]
        [switch]$Decimal,

        # Converts an XML string to an SByte value.
        [Parameter(Mandatory = $true, ParameterSetName = 'SByte')]
        [switch]$SByte,

        # Converts an XML string to an Int16 value.
        [Parameter(Mandatory = $true, ParameterSetName = 'Int16')]
		[Alias('Int16')]
        [switch]$Short,

        # Converts an XML string to an Int32 value.
        [Parameter(Mandatory = $true, ParameterSetName = 'Int32')]
		[Alias('Int32')]
        [switch]$Int,

        # Converts an XML string to an Int64 value.
        [Parameter(Mandatory = $true, ParameterSetName = 'Int64')]
		[Alias('Int64')]
        [switch]$Long,

        # Converts an XML string to a Byte value.
        [Parameter(Mandatory = $true, ParameterSetName = 'Byte')]
        [switch]$Byte,

        # Converts an XML string to a UInt32 value.
        [Parameter(Mandatory = $true, ParameterSetName = 'UInt16')]
		[Alias('UInt16')]
        [switch]$UShort,

        # Converts an XML string to a UInt32 value.
        [Parameter(Mandatory = $true, ParameterSetName = 'UInt32')]
		[Alias('UInt32')]
        [switch]$UInt,

        # Converts an XML string to a UInt64 value.
        [Parameter(Mandatory = $true, ParameterSetName = 'UInt64')]
		[Alias('UInt64')]
        [switch]$ULong,

        # Converts an XML string to a Single value.
        [Parameter(Mandatory = $true, ParameterSetName = 'Single')]
		[Alias('Single')]
        [switch]$Float,

        # Converts an XML string to a Double value.
        [Parameter(Mandatory = $true, ParameterSetName = 'Double')]
        [switch]$Double,

        # Converts an XML string to a TimeSpan value.
        [Parameter(Mandatory = $true, ParameterSetName = 'TimeSpan')]
        [switch]$TimeSpan,

        # Converts an XML string to a Guid value.
        [Parameter(Mandatory = $true, ParameterSetName = 'Guid')]
        [switch]$Guid,

        # Converts an XML string to a DateTime value.
        [Parameter(Mandatory = $true, ParameterSetName = 'DateTime')]
        [Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOption')]
        [switch]$DateTime,

        # Converts an XML string to a DateTimeOffset value.
        [Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOffset')]
        [switch]$DateTimeOffset,
        
        # Format for DateTime input value
        [Parameter(ParameterSetName = 'DateTime')]
        [Parameter(ParameterSetName = 'DateTimeOffset')]
        [System.String[]]$Format,

        # DateTime value deserialization option
        [Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOption')]
        [System.Xml.XmlDateTimeSerializationMode]$DateTimeOption
    )

    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'Boolean' {
                [System.Xml.XmlConvert]::ToBoolean($InputString);
                break;
            }
            'Char' {
                [System.Xml.XmlConvert]::ToChar($InputString);
                break;
            }
            'Decimal' {
                [System.Xml.XmlConvert]::ToDecimal($InputString);
                break;
            }
            'SByte' {
                [System.Xml.XmlConvert]::ToSByte($InputString);
                break;
            }
            'Int16' {
                [System.Xml.XmlConvert]::ToInt16($InputString);
                break;
            }
            'Int32' {
                [System.Xml.XmlConvert]::ToInt32($InputString);
                break;
            }
            'Int64' {
                [System.Xml.XmlConvert]::ToInt64($InputString);
                break;
            }
            'Byte' {
                [System.Xml.XmlConvert]::ToByte($InputString);
                break;
            }
            'UInt16' {
                [System.Xml.XmlConvert]::ToUInt16($InputString);
                break;
            }
            'UInt32' {
                [System.Xml.XmlConvert]::ToUInt32($InputString);
                break;
            }
            'UInt64' {
                [System.Xml.XmlConvert]::ToUInt64($InputString);
                break;
            }
            'Single' {
                [System.Xml.XmlConvert]::ToSingle($InputString);
                break;
            }
            'Double' {
                [System.Xml.XmlConvert]::ToDouble($InputString);
                break;
            }
            'TimeSpan' {
                [System.Xml.XmlConvert]::ToTimeSpan($InputString);
                break;
            }
            'DateTimeOption' {
                [System.Xml.XmlConvert]::ToDateTime($InputString, $DateTimeOption);
                break;
            }
            'DateTimeOffset' {
                if ($PSBoundParameters.ContainsKey('Format')) {
                    if ($Format.Length -eq 1) {
                        [System.Xml.XmlConvert]::ToDateTimeOffset($InputString, $Format[0]);
                    } else {
                        [System.Xml.XmlConvert]::ToDateTimeOffset($InputString, $Format);
                    }
                } else {
                    [System.Xml.XmlConvert]::ToDateTimeOffset($InputString);
                }
                break;
            }
            'Guid' {
                [System.Xml.XmlConvert]::ToGuid($InputString);
                break;
            }
            default {
                if ($PSBoundParameters.ContainsKey('Format')) {
                    if ($Format.Length -eq 1) {
                        [System.Xml.XmlConvert]::ToDateTime($InputString, $Format[0]);
                    } else {
                        [System.Xml.XmlConvert]::ToDateTime($InputString, $Format);
                    }
                } else {
                    [System.Xml.XmlConvert]::ToDateTime($InputString);
                }
                break;
            }
        }
    }
}

Function ConvertTo-XmlBinary {
	<#
		.SYNOPSIS
			Converts byte array XML String.
 
		.DESCRIPTION
			Converts a byte array to an XML string representation.

		.OUTPUTS
			System.String. The converted binary data.
        
        .LINK
            ConvertFrom-XmlBinary
        
        .LINK
            ConvertTo-XmlString
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.convert.tobase64string.aspx
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		# The binary data to be encoded
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[AllowEmptyCollection()]
		[byte[]]$Bytes,
		
		# Encode binary data as base-64 text. This is the default.
		[Parameter(ParameterSetName = 'Base64')]
		[switch]$Base64,
		
		# Encode binary data as a hexidecimal string.
		[Parameter(Mandatory = $true, ParameterSetName = 'Hex')]
		[switch]$Hex
	)

	Process {
		if ($Bytes.Length -eq 0) {
			'' | Write-Output;
		} else {
			if ($Hex) {
				(-join ($Bytes | ForEach-Object { ([int]$_).ToString('X2') })) | Write-Output;
			} else {
				[System.Convert]::ToBase64String($Bytes, [System.Base64FormattingOptions]::InsertLineBreaks);
			}
		}
	}
}

Function ConvertFrom-XmlBinary {
	<#
		.SYNOPSIS
			Converts XML String to byte array.
 
		.DESCRIPTION
			Converts an XML string representation to a byte array.

		.OUTPUTS
			System.Byte[]. The converted binary data.
        
        .LINK
            ConvertTo-XmlBinary
        
        .LINK
            ConvertFrom-XmlString
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.convert.frombase64string.aspx
	#>
	[CmdletBinding()]
	[OutputType([byte[]])]
	Param(
		# The text to be decoded.
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[AllowEmptyString()]
		[string]$Text,
		
		# Decodes text as base-64
		[Parameter(ParameterSetName = 'Base64')]
		[switch]$Base64,
		
		# Decodes text as hexidecimal values
		[Parameter(Mandatory = $true, ParameterSetName = 'Hex')]
		[switch]$Hex
	)

	Process {
		$Text = $Text.Trim();
		if ($Text.Length -eq 0) {
			Write-Output -InputObject (,(New-Object -TypeName 'System.Byte[]' -ArgumentList 0));
		} else {
			if ($Hex) {
				$List = New-Object -TypeName 'System.Collections.Generic.List[char]';
				if ($Text.StartsWith('0x') -or $Text.StartsWith('0X')) { $Text = $Text.Substring(2) }
				$Text = [XmlUtilityCLR.XmlUtility]::WhitespaceRegex.Replace($Text, '');
				if ($Text.Length -gt 0) {
					for ($i = 0; $i -lt $Text.Length; $i+=2) {
						$List.Add(([char]([int]::Parse($Text.Substring($i, 2), [System.Globalization.NumberStyles]::HexNumber))));
						Write-Output -InputObject (,$List.ToArray());
					}
				}
				Write-Output -InputObject (,$List.ToArray());
			} else {
				Write-Output -InputObject (,[System.Convert]::FromBase64String($Text.Trim()));
			}
		}
	}
}

Function ConvertTo-XmlList {
	<#
		.SYNOPSIS
			Converts string values to an XML list.
 
		.DESCRIPTION
			Converts string values to an XML string representation of a list.

		.OUTPUTS
			System.String. The converted strings.
        
        .LINK
            ConvertFrom-XmlList
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		# Text values to be added to the XML list
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string[]]$Text
	)

	Begin { $Items = @(); }

	Process { $Items += $Text }

	End {
		($Items | ForEach-Object { [XmlUtilityCLR.XmlUtility]::EncodeSpace($_) }) -join ' ';
	}
}

Function ConvertFrom-XmlList {
	<#
		.SYNOPSIS
			Converts XML list string item values.
 
		.DESCRIPTION
			Converts an XML string representation of a list to individual string values.

		.OUTPUTS
			System.String[]. The list items.

        .LINK
            ConvertFrom-XmlList
	#>
	[CmdletBinding()]
	[OutputType([string[]])]
	Param(
		# Text representing an XML list
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text,

		# XML list items are not encoded
		[switch]$NoDecode
	)

	Process {
		if ($NoDecode) {
			$Items -split '\s+';
		} else {
			($Items -split '\s+') | ForEach-Object { [XmlUtilityCLR.XmlUtility]::Decode($_) }
		}
	}
}

Function Add-XmlAttribute {
	<#
		.SYNOPSIS
			Adds or sets an XML attribute.
 
		.DESCRIPTION
			If a matching attribute already exists, then the attribute's value is set. Otherwise, a new attribute is added as the last node in the XML element's attribute collection.

		.OUTPUTS
			System.Xml.XmlAttribute. The added or modified attribute, if $PassThru is set.
        
        .LINK
            Set-XmlAttribute
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.createattribute.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlattribute.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlelement.attributes.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlattributecollection.append.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'Name')]
	[OutputType([System.Xml.XmlAttribute])]
	Param(
        # The XML element to which the attribute is to be added.
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlElement]$XmlElement,
		
        # The value to assign to the attribute.
		[Parameter(Mandatory = $true, Position = 1)]
		[AllowEmptyString()]
		[string]$Value,
		
        # The prefix of the attribute.
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Prefix')]
        [ValidateScript({ $n = $null; try { $n = [System.Xml.XmlConvert]::VerifyNCName($_) } catch { }; $n -eq $_ })]
		[string]$Prefix,
		
        # The qualified name of the attribute.
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Name')]
        [ValidateScript({ $n = $null; try { $n = [System.Xml.XmlConvert]::VerifyName($_) } catch { }; $n -eq $_ })]
		[string]$Name,
		
        # The local name of the attribute.
		[Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Prefix')]
        [ValidateScript({ $n = $null; try { $n = [System.Xml.XmlConvert]::VerifyNCName($_) } catch { }; $n -eq $_ })]
		[string]$LocalName,
		
        # The namespaceURI of the attribute.
		[Parameter(Mandatory = $true, Position = 4, ParameterSetName = 'Prefix')]
		[Parameter(Position = 3, ParameterSetName = 'Name')]
		[AllowEmptyString()]
        [ValidateScript({ $_ -eq '' -or [System.Uri]::IsWellFormedUriString($_, [System.UriKind]::Absolute) })]
		[string]$NamespaceURI,
        
        # If set, then the attribute will be returned.
		[switch]$PassThru
	)
    
    if ($PSBoundParameters.ContainsKey('NamespaceURI')) {
        if ($PSBoundParameters.ContainsKey('Prefix')) {
            if ($PassThru) {
                Set-XmlAttribute -XmlElement $XmlElement -Value $Value -Prefix $Prefix -LocalName $LocalName -NamespaceURI $NamespaceURI -Create -PassThru;
            } else {
                Set-XmlAttribute -XmlElement $XmlElement -Value $Value -Prefix $Prefix -LocalName $LocalName -NamespaceURI $NamespaceURI -Create;
            }
        } else {
            if ($PassThru) {
                Set-XmlAttribute -XmlElement $XmlElement -Value $Value -Name $Name -NamespaceURI $NamespaceURI -Create -PassThru;
            } else {
                Set-XmlAttribute -XmlElement $XmlElement -Value $Value -Name $Name -NamespaceURI $NamespaceURI -Create;
            }
        }
    } else {
        if ($PassThru) {
            Set-XmlAttribute -XmlElement $XmlElement -Value $Value -Name $Name -Create -PassThru;
        } else {
            Set-XmlAttribute -XmlElement $XmlElement -Value $Value -Name $Name -Create;
        }
    }
}

Function Set-XmlAttribute {
	<#
		.SYNOPSIS
			Sets an XML attribute.
 
		.DESCRIPTION
			Sets the value of an XML attribute belonging to the XML element.
            If the attribute does not exist, and $Create is set, then a new attribute is added as the last node in the XML element's attribute collection. Otherwise an error is thrown;

		.OUTPUTS
			System.Xml.XmlAttribute. The modified or added attribute, if $PassThru is set.
        
        .LINK
            Add-XmlAttribute
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlelement.attributes.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlattribute.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.createattribute.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlattributecollection.append.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'Name')]
	[OutputType([System.Xml.XmlAttribute])]
	Param(
        # The XML element containing the attribute which is to be modified.
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlElement]$XmlElement,
		
        # The value to assign to the attribute.
		[Parameter(Mandatory = $true, Position = 1)]
		[AllowEmptyString()]
		[string]$Value,
		
        # The prefix of the attribute.
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Prefix')]
        [ValidateScript({ $n = $null; try { $n = [System.Xml.XmlConvert]::VerifyNCName($_) } catch { }; $n -eq $_ })]
		[string]$Prefix,
		
        # The qualified name of the attribute.
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Name')]
		[string]$Name,
        [ValidateScript({ $n = $null; try { $n = [System.Xml.XmlConvert]::VerifyName($_) } catch { }; $n -eq $_ })]
		
        # The local name of the attribute.
		[Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Prefix')]
        [ValidateScript({ $n = $null; try { $n = [System.Xml.XmlConvert]::VerifyNCName($_) } catch { }; $n -eq $_ })]
		[string]$LocalName,
		
        # The namespaceURI of the attribute.
		[Parameter(Mandatory = $true, Position = 4, ParameterSetName = 'Prefix')]
		[Parameter(Position = 3, ParameterSetName = 'Name')]
		[AllowEmptyString()]
        [ValidateScript({ $_ -eq '' -or [System.Uri]::IsWellFormedUriString($_, [System.UriKind]::Absolute) })]
		[string]$NamespaceURI,

        # If set, and the attribute does not exist, then it will be appended.
		[switch]$Create,

        # If set, then the attribute will be returned.
		[switch]$PassThru
	)
    
    if ($PSBoundParameters.ContainsKey('NamespaceURI')) {
        $XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $XmlElement.OwnerDocument.NameTable;
        if ($PSBoundParameters.ContainsKey('Prefix')) {
            $XmlNamespaceManager.AddNamespace($Prefix, $NamespaceURI);
            $XmlAttribute = $XmlElement.SelectSingleNode(('@{0}:{1}' -f $Prefix, $LocalName), $XmlNamespaceManager);
        } else {
            $XmlNamespaceManager.AddNamespace('ns1', $NamespaceURI);
            $XmlAttribute = $XmlElement.OwnerDocument.CreateAttribute($Name, $NamespaceURI);
            $XmlAttribute = $XmlElement.SelectSingleNode('@ns1:' + $XmlAttribute.LocalName, $XmlNamespaceManager);
        }
    } else {
        $XmlAttribute = $XmlElement.SelectSingleNode('@' + $Name);
    }
    
    if ($XmlAttribute -eq $null) {
        if (-not $Create) {
            throw 'Attribute not found.';
        } else {
            if ($PSBoundParameters.ContainsKey('NamespaceURI')) {
                if ($PSBoundParameters.ContainsKey('Prefix')) {
                    $XmlAttribute = $XmlElement.Attributes.Append($XmlElement.OwnerDocument.CreateAttribute($Prefix, $LocalName, $NamespaceURI));
                } else {
                    $XmlAttribute = $XmlElement.Attributes.Append($XmlElement.OwnerDocument.CreateAttribute($Name, $NamespaceURI));
                }
            } else {
                $XmlAttribute = $XmlElement.Attributes.Append($XmlElement.OwnerDocument.CreateAttribute($Name));
            }
        }
    }
    
    if ($XmlAttribute -ne $null) {
        $XmlAttribute.Value = $Value;
        if ($PassThru) { $XmlAttribute | Write-Output }
    }
}

Function Add-XmlElement {
	<#
		.SYNOPSIS
			Adds an XML element.
 
		.DESCRIPTION
			Adds an XML element to the specified parent XML element.

		.OUTPUTS
			System.Xml.XmlElement. The XML element which was added.
        
        .LINK
            Add-XmlAttribute
        
        .LINK
            Set-XmlText
        
        .LINK
            Add-XmlTextElement
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.createelement.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlelement.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'Name')]
	[OutputType([System.Xml.XmlElement])]
	Param(
        # The XML element to which the new child element is to be added.
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlElement]$ParentElement,
		
        # The prefix of the new child element.
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Prefix')]
		[string]$Prefix,
		
        # The qualified name of the new child element.
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Name')]
        [ValidateScript({ $n = $null; try { $n = [System.Xml.XmlConvert]::VerifyName($_) } catch { }; $n -eq $_ })]
		[string]$Name,
		
        # The local name of the new child element.
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Prefix')]
		[string]$LocalName,
		
        # The namespaceURI of the new child element.
		[Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Prefix')]
		[Parameter(Position = 2, ParameterSetName = 'Name')]
		[AllowEmptyString()]
        [ValidateScript({ $_ -eq '' -or [System.Uri]::IsWellFormedUriString($_, [System.UriKind]::Absolute) })]
		[string]$NamespaceURI
	)
    
    if ($PSBoundParameters.ContainsKey('Name')) {
        if ($PSBoundParameters.ContainsKey('NamespaceURI')) {
            $ParentElement.AppendChild($ParentElement.OwnerDocument.CreateElement($Name, $NamespaceURI));
        } else {
            $ParentElement.AppendChild($ParentElement.OwnerDocument.CreateElement($Name));
        }
    } else {
        $ParentElement.AppendChild($ParentElement.OwnerDocument.CreateElement($Prefix, $LocalName, $NamespaceURI));
    }
}

Function Add-XmlTextElement {
	<#
		.SYNOPSIS
			Adds an XML element with text content.
 
		.DESCRIPTION
			Adds an XML element, with text content, to the specified parent XML element.

		.OUTPUTS
			System.Xml.XmlElement. If $PassThru was set, the XML element which was added.
        
        .LINK
            Set-XmlText
        
        .LINK
            Add-XmlAttribute
        
        .LINK
            Add-XmlElement
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.createelement.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.createtextnode.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.createcdatasection.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlelement.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'Name')]
	[OutputType([System.Xml.XmlElement])]
	Param(
        # The XML element to which the new child element is to be added.
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlElement]$ParentElement,
		
        # The inner text for the new child element.
		[Parameter(Mandatory = $true, Position = 1)]
		[AllowEmptyString()]
		[string]$InnerText,
		
        # The prefix of the new child element.
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Prefix')]
        [ValidateScript({ $n = $null; try { $n = [System.Xml.XmlConvert]::VerifyNCName($_) } catch { }; $n -eq $_ })]
		[string]$Prefix,
		
        # The qualified name of the new child element.
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Name')]
        [ValidateScript({ $n = $null; try { $n = [System.Xml.XmlConvert]::VerifyName($_) } catch { }; $n -eq $_ })]
		[string]$Name,
		
        # The local name of the new child element.
		[Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Prefix')]
        [ValidateScript({ $n = $null; try { $n = [System.Xml.XmlConvert]::VerifyNCName($_) } catch { }; $n -eq $_ })]
		[string]$LocalName,
		
        # The namespaceURI of the new child element.
		[Parameter(Mandatory = $true, Position = 4, ParameterSetName = 'Prefix')]
		[Parameter(Position = 3, ParameterSetName = 'Name')]
		[AllowEmptyString()]
        [ValidateScript({ $_ -eq '' -or [System.Uri]::IsWellFormedUriString($_, [System.UriKind]::Absolute) })]
		[string]$NamespaceURI,

        # If set, then the new child element will be returned.
		[switch]$PassThru
	)
    
    if ($PSBoundParameters.ContainsKey('Name')) {
        if ($PSBoundParameters.ContainsKey('Namespace')) {
            $XmlElement = Add-XmlElement -Name $Name -NamespaceURI $NamespaceURI;
        } else {
            $XmlElement = Add-XmlElement -Name $Name;
        }
    } else {
        $XmlElement = Add-XmlElement -Prefix $Prefix -LocalName $LocalName -NamespaceURI $NamespaceURI;
    }
    
    Set-XmlText -XmlElement $XmlElement -InnerText $InnerText;
    
    if ($PassThru) { $XmlElement | Write-Output }
}

Function Set-XmlText {
	<#
		.SYNOPSIS
			Sets XML element inner text.
 
		.DESCRIPTION
			Sets the inner text, as an optimal text node, of an XML element. All child nodes, including child elements will be replaced with the new text.

		.OUTPUTS
			System.Xml.XmlElement. If $PassThru was set, the XML element which was added.
        
        .LINK
            Set-XmlText
        
        .LINK
            Add-XmlAttribute
        
        .LINK
            Add-XmlElement
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.createelement.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.createtextnode.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.createcdatasection.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlelement.aspx
    #>
	[CmdletBinding(DefaultParameterSetName = 'Text')]
	Param(
        # The XML element to which a text node will be added
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlElement]$XmlElement,
		
        # The inner text for the new child element. A null value will result in an empty element.
		[Parameter(Mandatory = $true, Position = 1)]
		[AllowEmptyString()]
        [AllowNull()]
		[string]$InnerText,
        
        # Forces the inner text to be added as a Text node.
		[Parameter(ParameterSetName = 'Text')]
        [switch]$ForceText,
        
        # Forces the inner text to be added as a CData node.
		[Parameter(Mandatory = $true, ParameterSetName = 'CData')]
        [switch]$ForceCData
	)
    
    if ($InnerText -eq $null) {
        if (-not $XmlElement.IsEmpty) {
            $XmlElement.RemoveAll();
            $XmlElement.IsEmpty = $true;
        }
    } else {
        if ($XmlElement.IsEmpty -or $XmlElement.InnerText -ne $InnerText) {
            if (-not $XmlElement.IsEmpty) { $XmlElement.RemoveAll() }
            if ($ForceText -or ($InnerText -eq '' -and -not $ForceCData)) {
                $XmlElement.Appendchild($XmlElement.OwnerDocument.CreateTextNode($InnerText)) | Out-Null;
            } else {
                if ($ForceCData -or [System.Char]::IsWhiteSpace($InnerText[0]) -or [System.Char]::IsWhiteSpace($InnerText[$InnerText.Length - 1])) {
                    $XmlElement.Appendchild($XmlElement.OwnerDocument.CreateCDataSection($InnerText)) | Out-Null;
                } else {
                    $XmlText = $XmlElement.OwnerDocument.CreateTextNode($InnerText);
                    $XmlCDataSection = $XmlElement.OwnerDocument.CreateCDataSection($InnerText);
                    if ($XmlText.OuterXml.Length -gt $XmlCDataSection.OuterXml.Length) {
                        $XmlElement.Appendchild($XmlCDataSection) | Out-Null;
                    } else {
                        $XmlElement.Appendchild($XmlText) | Out-Null;
                    }
                }
            }
        }
    }
}