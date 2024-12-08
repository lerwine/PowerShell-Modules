. $PSScriptRoot/System.Reflection.ps1
. $PSScriptRoot/PSMaml.ps1
. $PSScriptRoot/PSEncode.ps1

Function Find-ProjectTypeInfo {
    <#
        .SYNOPSIS
            Searches for matching Visual Studio Project type information.
 
        .DESCRIPTION
			Searches module XML database for matching project type records.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Guid')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'key')]
        [string[]]$Key,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'FileExtension')]
		[AllowEmptyString()]
		[ValidatePattern('^(\..+)$')]
        [string[]]$FileExtension,
        
        [Parameter(ParameterSetName = 'Guid')]
        [System.Guid[]]$Guid
    )

    Begin {
        if ($null -eq $Script:ProjectTypesDocument) {
			$Script:ProjectTypesDocument = [System.Xml.XmlDocument]::new();
			$Script:ProjectTypesDocument.Load(($PSScriptRoot | Join-Path -ChildPath 'VsProjectTypes.xml'));
        }
    }

    Process {
		$ElementArray = @();
		switch ($PSCmdlet.ParameterSetName) {
			'key' {
				$ElementArray = @($Key | ForEach-Object { @($Script:ProjectTypesDocument.DocumentElement.SelectNodes(('ProjectType[translate(@Key,"ABCDEFGHIJKLMNOPQRSTUVWXYZ","abcdefghijklmnopqrstuvwxyz")="{0}"]|ProjectType/AltDescription[translate(@Key,"ABCDEFGHIJKLMNOPQRSTUVWXYZ","abcdefghijklmnopqrstuvwxyz")="{0}"]' -f $_.Replace('&', '&amp;').Replace('"', '&quot;').ToLower()))) });
				break;
			}
			'FileExtension' {
				$ElementArray = @($FileExtension | ForEach-Object { @($Script:ProjectTypesDocument.DocumentElement.SelectNodes(('ProjectType[translate(@Extension,"ABCDEFGHIJKLMNOPQRSTUVWXYZ","abcdefghijklmnopqrstuvwxyz")="{0}"]|ProjectType/AltExt[translate(.,"ABCDEFGHIJKLMNOPQRSTUVWXYZ","abcdefghijklmnopqrstuvwxyz")="{0}"]' -f $_.Replace('&', '&amp;').Replace('"', '&quot;').ToLower()))) });
				break;
			}
			default {
				if ($PSBoundParameters.ContainsKey('')) {
					$ElementArray = @($Guid | ForEach-Object { @($Script:ProjectTypesDocument.DocumentElement.SelectNodes(('ProjectType[@Guid="{0}"]|ProjectType/AltGuid[.="{0}"]' -f $_.ToString()))) });
				}
				break;
			}
		}

		$ElementArray | ForEach-Object {
			$e = $_;
			$Properties = @{ };
			switch ($e.LocalName) {
				'AltDescription' {
					$e = $_.ParentNode;
					$Properties['Guid']  = $e.SelectSingleNode('@Guid').Value;
					$Properties['Key'] = $_.SelectSingleNode('@Key').Value;
					$a = $_.SelectSingleNode('@Extension');
					if ($null -eq $a) { $Properties['Extension'] = '' } else { $Properties['Extension'] = $a.Value }
					$Properties['Description'] = $_.InnerText;
					$Properties['AltExtensions'] = @($_.SelectNodes('AltExt') | ForEach-Object { $_.InnerText });
					$Properties['AltDescriptions'] = @(@{
						Key = $e.SelectSingleNode('@Key').Value;
						Description = $e.SelectSingleNode('@Description').Value;
					}) + @($_.SelectNodes('preceding-sibling::AltDescription') | ForEach-Object {@{
						Key = $_.SelectSingleNode('@Key').Value;
						Description = $_.InnerText;
					}}) + @($_.SelectNodes('followng-sibling::AltDescription') | ForEach-Object {@{
						Key = $_.SelectSingleNode('@Key').Value;
						Description = $_.InnerText;
					}});
					$Properties['AltGuids'] = @($_.SelectNodes('AltGuid') | ForEach-Object { $_.InnerText });
					break;
				}
				'AltExt' {
					$e = $_.ParentNode;
					$Properties['Guid']  = $e.SelectSingleNode('@Guid').Value;
					$Properties['Key']  = $e.SelectSingleNode('@Key').Value;
					$Properties['Extension'] = $_.InnerText;
					$Properties['Description'] = $e.SelectSingleNode('@Description').Value;
					$Properties['AltExtensions'] = @($e.SelectSingleNode('@Extension').Value) + @($_.SelectNodes('preceding-sibling::AltExt') | ForEach-Object { $_.InnerText }) + @($_.SelectNodes('followng-sibling::AltExt') | ForEach-Object { $_.InnerText });
					$Properties['AltDescriptions'] = @($_.SelectNodes('AltDescription') | ForEach-Object {@{
						Key = $_.SelectSingleNode('@Key').Value;
						Description = $_.InnerText;
					}});
					$Properties['AltGuids'] = @($_.SelectNodes('AltGuid') | ForEach-Object { $_.InnerText });
					break;
				}
				'AltGuid' {
					$e = $_.ParentNode;
					$Properties['Guid'] = $_.InnerText;
					$Properties['Key']  = $e.SelectSingleNode('@Key').Value;
					$a = $_.SelectSingleNode('@Extension');
					if ($null -eq $a) { $Properties['Extension'] = '' } else { $Properties['Extension'] = $a.Value }
					$Properties['Description'] = $e.SelectSingleNode('@Description').Value;
					$Properties['AltExtensions'] = @($_.SelectNodes('AltExt') | ForEach-Object { $_.InnerText });
					$Properties['AltDescriptions'] = @($_.SelectNodes('AltDescription') | ForEach-Object {@{
						Key = $_.SelectSingleNode('@Key').Value;
						Description = $_.InnerText;
					}});
					$Properties['AltGuids'] = @($e.SelectSingleNode('@Guid').Value) + @($_.SelectNodes('preceding-sibling::AltGuid') | ForEach-Object { $_.InnerText }) + @($_.SelectNodes('followng-sibling::AltGuid') | ForEach-Object { $_.InnerText });
					break;
				}
				default {
					$e = $_.ParentNode;
					$Properties['Guid']  = $e.SelectSingleNode('@Guid').Value;
					$Properties['Key'] = $_.SelectSingleNode('@Key').Value;
					$a = $_.SelectSingleNode('@Extension');
					if ($null -eq $a) { $Properties['Extension'] = '' } else { $Properties['Extension'] = $a.Value }
					$Properties['Description'] = $e.SelectSingleNode('@Description').Value;
					$Properties['AltExtensions'] = @($_.SelectNodes('AltExt') | ForEach-Object { $_.InnerText });
					$Properties['AltDescriptions'] = @($_.SelectNodes('AltDescription') | ForEach-Object {@{
						Key = $_.SelectSingleNode('@Key').Value;
						Description = $_.InnerText;
					}});
					$Properties['AltGuids'] = @($_.SelectNodes('AltGuid') | ForEach-Object { $_.InnerText });
					break;
				}
			}
			$a = $_.SelectSingleNode('@Language');
			if ($null -eq $a) { $Properties['Language'] = '' } else { $Properties['Language'] = $a.Value }
			$a = $_.SelectSingleNode('@Package');
			if ($null -eq $a) { $Properties['Package'] = $null } else { $Properties['Package'] = [Guid]::Parse($a.Value) }
			$Properties['Guid'] = [Guid]::Parse($Properties['Guid']);
			$Properties['AltGuids'] = @($Properties['AltGuids'] | ForEach-Object { [Guid]::Parse($_) });
			New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
		}
    }
}