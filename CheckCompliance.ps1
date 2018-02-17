Add-Type -AssemblyName 'Microsoft.Build', 'Microsoft.Build.Framework', 'Microsoft.Build.Utilities.v4.0' -ErrorAction Stop;$SlnContent = [System.IO.File]::ReadAllText(($PSScriptRoot | Join-Path -ChildPath 'PowerShellModules.sln'));
$StartOfNewLine = '(^[\r\n]*|\G[\r\n]+)';
$KeyValuePairPattern = '(?<key>[^=\s]+)[ \t]*=[ \t]*(\"(?<value>[^"]*)\"|(?<value>[^\s\r\n]*([ \t]+[^\s\r\n]+)*))';
$ProjectPattern = 'Project\(\"\{(?<typeGuid>[\da-f]{8}(-[\da-f]{4}){4}[\da-f]{8})\}\"\)[ \t]*=[ \t]*\"(?<projectName>[^\"]*)\"[ \t]*,[ \t]*\"(?<projectPath>[^\"]*)\"[ \t]*,[ \t]*\"\{(?<projectGuid>[\da-f]{8}(-[\da-f]{4}){4}[\da-f]{8})\}\"(?<otherProperties>[^\r\n]+)?(?<content>([\r\n]+(?!EndProject)[^\r\n]*)*)(\r\n?|\n)EndProject';
$GlobalPattern = 'Global(?<properties>[^\r\n]+)?(?<globalContent>([\r\n]+(?!EndGlobal)[^\r\n]*)*)(\r\n?|\n)EndGlobal';
$OtherPattern = '[^\r\n]+';

<#
$ProjectRegex = [System.Text.RegularExpressions.Regex]::new(@'
(^
((?![\r\n]*Project)
    [\r\n]*[^\r\n]*
    (?=[\r\n])
)*

|\G(\r\n?|\n))Project\("\{(?<typeGuid>[a-f\d]{8}(-[a-f\d]{4})[4][a-f\d]{8})\}"\)[ \t]*=[ \t]*"(?<projName>[^"]*)"[ \t]*,[ \t]*"(?<projPath>[^"]*)"[ \t]*,[ \t]*"\{(?<projGuid>[a-f\d]{8}(-[a-f\d]{4})[4][a-f\d]{8})\}"(?<otherProperties>[^\r\n]+)?(?<content>[\r\n]+[ \t]+[^\r\n]*)*(\r\n?|\n)EndProject
'@, ([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase));
$ProjectRegex.Matches($SlnContent);

$Regex = [System.Text.RegularExpressions.Regex]::new($StartOfNewLine + $KeyValuePairPattern, ([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase));
$m = $Regex.Match('VisualStudioVersion = 14.0.25420.1');
$m.Success;
if ($m.Success) { "Key: `"$($m.Groups['key'].Value)`"; Value: `"$($m.Groups['value'].Value)`"" }

$Regex = [System.Text.RegularExpressions.Regex]::new($StartOfNewLine + $ProjectPattern, ([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase));
$m = $Regex.Match(@'
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Solution Items", "Solution Items", "{A19AF169-44DD-4737-82DD-AB83CF18B77A}"
	ProjectSection(SolutionItems) = preProject
		Common.ttinc = Common.ttinc
		CommonTypes.xsd = CommonTypes.xsd
		LICENSE = LICENSE
		ManifestToAbout.xslt = ManifestToAbout.xslt
		ManifestToPsd.xslt = ManifestToPsd.xslt
		PSModule.testsettings = PSModule.testsettings
		PsModuleManifestGeneration.ttinc = PsModuleManifestGeneration.ttinc
		README.md = README.md
		Setup-New.ps1 = Setup-New.ps1
		Setup.bat = Setup.bat
		Setup.ps1 = Setup.ps1
		SetupBindings.xsd = SetupBindings.xsd
		SetupManifestData.xsd = SetupManifestData.xsd
		VsProject.ps1 = VsProject.ps1
	EndProjectSection
EndProject
'@);
$m.Success;
if ($m.Success) { "typeGuid: `"$($m.Groups['typeGuid'].Value)`"; projectName: `"$($m.Groups['projectName'].Value)`"; projectPath: `"$($m.Groups['projectPath'].Value)`"; projectGuid: `"$($m.Groups['projectGuid'].Value)`"; content: `"$($m.Groups['content'].Value)`"" }

$Regex = [System.Text.RegularExpressions.Regex]::new($StartOfNewLine + $GlobalPattern, ([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase));
$m = $Regex.Match(@'
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		CD_ROM|Any CPU = CD_ROM|Any CPU
		Debug|Any CPU = Debug|Any CPU
		DebugPS2|Any CPU = DebugPS2|Any CPU
		DebugPS3|Any CPU = DebugPS3|Any CPU
		DVD-5|Any CPU = DVD-5|Any CPU
		Release|Any CPU = Release|Any CPU
		ReleasePS2|Any CPU = ReleasePS2|Any CPU
		ReleasePS3|Any CPU = ReleasePS3|Any CPU
		SingleImage|Any CPU = SingleImage|Any CPU
	EndGlobalSection
EndGlobal
'@);
$m.Success;
if ($m.Success) { "globalContent: `"$($m.Groups['globalContent'].Value)`"" }

#>

if ($VsProjectGuids -eq $null) {
    $XmlDocument = [System.Xml.XmlDocument]::new();
    $XmlDocument.Load(($PSScriptRoot | Join-Path -ChildPath 'LteDev\VsProjectTypes.xml'));
    $VsProjectGuids = @{};
    $XmlDocument.SelectNodes('/VsProjectTypes/ProjectType') | ForEach-Object {
        $Properties = @{
            Guid = [Guid]::Parse($_.SelectSingleNode('@Guid').Value);
            Key = $_.SelectSingleNode('@Key').Value;
            Description = $_.SelectSingleNode('@Description').Value;
            Extensions = [System.Collections.ObjectModel.Collection[System.String]]::new();
        };
        $a = $_.SelectSingleNode('@Extension');
        if ($a -ne $null) {
            $Properties.Extensions.Add($a.Value);
            $a = $_.SelectSingleNode('@AltExt');
            if ($a -ne $null) {
                $Properties.Extensions.Add($a.Value);
            }
        }
        $Item = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
        if (-not $VsProjectGuids.ContainsKey($Item.Guid)) {
            $VsProjectGuids[$Item.Guid] = [System.Collections.ObjectModel.Collection[System.Management.Automation.PSObject]]::new();
        }
        $VsProjectGuids[$Item.Guid].Add($Item);
    }
}

$FullPattern = "$StartOfNewLine($ProjectPattern|$GlobalPattern|$KeyValuePairPattern|[^\r\n]+)[ \t]*";
$Regex = [System.Text.RegularExpressions.Regex]::new($FullPattern, ([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase));
$MatchCollection = $Regex.Matches($SlnContent);
$MatchCollection.Count;
$MatchCollection | ForEach-Object {
    if ($_.Groups['typeGuid'].Success) {
        Write-Debug -Message "typeGuid: `"$($_.Groups['typeGuid'].Value)`"; projectName: `"$($_.Groups['projectName'].Value)`"; projectPath: `"$($_.Groups['projectPath'].Value)`"; projectGuid: `"$($_.Groups['projectGuid'].Value)`"; content.Length: `"$($_.Groups['content'].Length)`"";
        $TypeGuid = [Guid]::Parse($_.Groups['typeGuid'].Value);
        $ProjectGuid = [Guid]::Parse($_.Groups['projectGuid'].Value);
        $ProjectName = $_.Groups['projectName'].Value;
        $ProjectPath = $_.Groups['projectPath'].Value;
        $FullProjectPath = $PSScriptRoot | Join-Path -ChildPath $ProjectPath;
        $IsCsProject = $false;
        $ProjectDescription = "$ProjectGuid Project";
        if (-not $VsProjectGuids.ContainsKey($TypeGuid)) {
            Write-Warning -Message "Unknown project type guid: $TypeGUid";
        } else {
            $IsCsProject = (@($VsProjectGuids[$TypeGuid] | Where-Object { $_.Extensions -icontains '.csproj' }).Count -gt 0);
            $ProjectDescription = ($VsProjectGuids[$TypeGuid] | ForEach-Object { $_.Description }) -join ' / ';
        }
        if ($FullProjectPath | Test-Path -PathType Leaf) {
            [Microsoft.Build.Project]
        } else {
            Write-Warning -Message "$ProjectDescription Project ($ProjectGuid) not found at $ProjectPath";
        }
    } else {
        if ($_.Groups['globalContent'].Success) {
            Write-Debug -Message "globalContent.Length: `"$($_.Groups['globalContent'].Length)`"";
        } else {
            if ($_.Groups['key'].Success) {
                Write-Debug -Message "Key: `"$($_.Groups['key'].Value)`"; Value: `"$($_.Groups['value'].Value)`"";
            }
        }
    }
}