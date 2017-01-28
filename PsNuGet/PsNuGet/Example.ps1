$PathToLocalRepository = 'C:\users\mylogin\Downloads\MyNugetRepo';
$ReportFilePath = $PathToLocalRepository | Join-Path -ChildPath 'Report.html';

Import-Module 'Erwine.Leonard.T.PsNuget';
Add-Type -AssemblyName 'System.Web';

$StreamWriter = New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $ReportFilePath;
$HtmlTextWriter = New-Object -TypeName 'System.Web.UI.HtmlTextWriter' -ArgumentList $StreamWriter;
$HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Html);
$HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Head);
$HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Title);
$HtmlTextWriter.WriteEncodedText('NuGet package content report');
$HtmlTextWriter.RenderEndTag();
$HtmlTextWriter.RenderEndTag();
$HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Body);
$HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::H1);
$HtmlTextWriter.WriteEncodedText('NuGet package content report');
$HtmlTextWriter.RenderEndTag();
$HtmlTextWriter.WriteEncodedText('Following is a report of NuGet package file contents. NuGet package files use the PKZIP compression/archive format.');
$HtmlTextWriter.WriteBreak();
$HtmlTextWriter.WriteEncodedText('NuGet package files are provided by Microsoft through the website ');
$HtmlTextWriter.AddAttribute([System.Web.UI.HtmlTextWriterAttribute]::Href, 'https://www.nuget.org');
$HtmlTextWriter.AddAttribute([System.Web.UI.HtmlTextWriterAttribute]::Target, '_blank');
$HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::A);
$HtmlTextWriter.WriteEncodedText('https://www.nuget.org');
$HtmlTextWriter.RenderEndTag();
$HtmlTextWriter.WriteEncodedText('.')

$AllFiles = @(Get-ChildItem -Path $PathToLocalRepository -Filter '*.nupkg');
for ($index = 0; $index -lt $AllFiles.Count; $index++) {
    $PercentComplete = [Convert]::ToInt32([Convert]::ToDouble($index * 100) / [Convert]::ToDouble($AllFiles.Count));
    Write-Progress -Activity 'Scanning packages' -Status 'Opening File' -PercentComplete $PercentComplete -CurrentOperation $AllFiles[$index].FullName;
    $Info = Get-NugetPackageInfo -Path $AllFiles[$index].FullName;
    Write-Progress -Activity 'Scanning packages' -Status 'Retrieving source information' -PercentComplete $PercentComplete -CurrentOperation $AllFiles[$index].FullName;
    $arr = @(Get-NugetPackageInfo -Identifier $Info.id -Version $Info.properties.version);
    if ($arr.Count -eq 0) {
        $arr = @(Get-NugetPackageInfo -Identifier $Info.id -LatestVersion);
        if ($arr.Count -eq 0) {
            $arr = @(Get-NugetPackageInfo -Identifier $Info.id);
        }
    }
    $NugetPackageInfo = $Info;
    if ($arr.Count -gt 0) { $NugetPackageInfo = $arr[0] }
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::H2);
    $HtmlTextWriter.WriteEncodedText($NugetPackageInfo.Title);
    $HtmlTextWriter.RenderEndTag();
    if ($NugetPackageInfo.properties.Summary -ne $null -and $NugetPackageInfo.properties.Summary.Trim().Length -gt 0) {
        $HtmlTextWriter.WriteEncodedText($NugetPackageInfo.properties.Summary);
    }
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Table);
    
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Tr);
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::WhiteSpace, 'nowrap');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::FontWeight, 'bold');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::TextAlign, 'right');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::VerticalAlign, 'top');
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText('ID:');
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText($NugetPackageInfo.properties.id);
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::WhiteSpace, 'nowrap');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::FontWeight, 'bold');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::TextAlign, 'right');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::VerticalAlign, 'top');
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText('Version:');
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText($NugetPackageInfo.properties.version);
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.RenderEndTag();
    
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Tr);
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::WhiteSpace, 'nowrap');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::FontWeight, 'bold');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::TextAlign, 'right');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::VerticalAlign, 'top');
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText('Authors:');
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText($NugetPackageInfo.properties.authors);
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::WhiteSpace, 'nowrap');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::FontWeight, 'bold');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::TextAlign, 'right');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::VerticalAlign, 'top');
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText('Owners:');
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText($Info.properties.owners);
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.RenderEndTag();
    
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Tr);
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::WhiteSpace, 'nowrap');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::FontWeight, 'bold');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::TextAlign, 'right');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::VerticalAlign, 'top');
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText('File Name:');
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText($AllFiles[$index].Name);
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::WhiteSpace, 'nowrap');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::FontWeight, 'bold');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::TextAlign, 'right');
    $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::VerticalAlign, 'top');
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText('Project URL:');
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
    $HtmlTextWriter.WriteEncodedText($NugetPackageInfo.properties.projectUrl);
    $HtmlTextWriter.RenderEndTag();
    $HtmlTextWriter.RenderEndTag();
    
    if ($NugetPackageInfo.properties.GalleryDetailsUrl -ne $null) {
        $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Tr);
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::WhiteSpace, 'nowrap');
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::FontWeight, 'bold');
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::TextAlign, 'right');
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::VerticalAlign, 'top');
        $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
        $HtmlTextWriter.WriteEncodedText('Details URL:');
        $HtmlTextWriter.RenderEndTag();
        $HtmlTextWriter.AddAttribute([System.Web.UI.HtmlTextWriterAttribute]::Colspan, '3');
        $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
        $HtmlTextWriter.WriteEncodedText($NugetPackageInfo.properties.GalleryDetailsUrl);
        $HtmlTextWriter.RenderEndTag();
        $HtmlTextWriter.RenderEndTag();
    }
    
    if ($NugetPackageInfo.properties.description -ne $null -and $NugetPackageInfo.properties.description.Trim().Length -gt 0) {
        $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Tr);
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::WhiteSpace, 'nowrap');
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::FontWeight, 'bold');
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::TextAlign, 'right');
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::VerticalAlign, 'top');
        $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
        $HtmlTextWriter.WriteEncodedText('Description:');
        $HtmlTextWriter.RenderEndTag();
        $HtmlTextWriter.AddAttribute([System.Web.UI.HtmlTextWriterAttribute]::Colspan, '3');
        $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
        
        $paragraphOpen = $false;
        $startNewParagraph = $false;
        $hasPreviousLine = $false;
        $Info.properties.description.Trim() -split '\r\n?|\n' | ForEach-Object {
            if ($_.Trim().Length -eq 0) {
                $hasPreviousLine = $false;
                if ($paragraphOpen) {
                    $HtmlTextWriter.RenderEndTag();
                    $paragraphOpen = $false;
                }
                $startNewParagraph = $true;
            } else {
                if ($startNewParagraph) {
                    $paragraphOpen = $true;
                    $startNewParagraph = $false;
                    $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::P);
                } else {
                    if ($hasPreviousLine) { $HtmlTextWriter.WriteBreak() }
                }
                $hasPreviousLine = $true;
                $HtmlTextWriter.WriteEncodedText($_.Trim());
            }
        }
        if ($paragraphOpen) { $HtmlTextWriter.RenderEndTag() }
        $HtmlTextWriter.RenderEndTag();
        $HtmlTextWriter.RenderEndTag();
    }
    if ($Info.BinaryExcutables.Count -gt 0) {
        $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Tr);
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::WhiteSpace, 'nowrap');
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::FontWeight, 'bold');
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::TextAlign, 'right');
        $HtmlTextWriter.AddStyleAttribute([System.Web.UI.HTMLTextWriterStyle]::VerticalAlign, 'top');
        $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
        $HtmlTextWriter.WriteEncodedText('Binary Executables:');
        $HtmlTextWriter.RenderEndTag();
        $HtmlTextWriter.AddAttribute([System.Web.UI.HtmlTextWriterAttribute]::Colspan, '3');
        $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Td);
        $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Ul);
        $Info.BinaryExcutables | ForEach-Object {
            $HtmlTextWriter.RenderBeginTag([System.Web.UI.HtmlTextWriterTag]::Li);
            $HtmlTextWriter.WriteEncodedText($_);
            $HtmlTextWriter.RenderEndTag();
        }
        $HtmlTextWriter.RenderEndTag();
        $HtmlTextWriter.RenderEndTag();
        $HtmlTextWriter.RenderEndTag();
    }
    
    $HtmlTextWriter.RenderEndTag();
}
$HtmlTextWriter.RenderEndTag();
$HtmlTextWriter.RenderEndTag();
$HtmlTextWriter.Flush();
$StreamWriter.Flush();
$HtmlTextWriter.Close();
$StreamWriter.Close();
Write-Progress -Activity 'Scanning packages' -Status 'Completed' -Completed;
