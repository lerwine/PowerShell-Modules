$ProjectTypesPath = $PSScriptRoot | Join-Path -ChildPath 'VsProjectTypes.xml';
$t=@'
Windows (C#) {FAE04EC0-301F-11D3-BF4B-00C04F79EFBC} 
Windows (VB.NET) {F184B08F-C81C-45F6-A57F-5ABD9991F28F} 
Windows (Visual C++) {8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942} 
Web Application {349C5851-65DF-11DA-9384-00065B846F21} 
Web Site {E24C65DC-7377-472B-9ABA-BC803B73C61A} 
Distributed System {F135691A-BF7E-435D-8960-F99683D2D49C} 
Windows Communication Foundation (WCF) {3D9AD99F-2412-4246-B90B-4EAA41C64699} 
Windows Presentation Foundation (WPF) {60DC8134-EBA5-43B8-BCC9-BB4BC16C2548} 
Visual Database Tools {C252FEB5-A946-4202-B1D4-9916A0590387} 
Database {A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124} 
Database (other project types) {4F174C21-8C12-11D0-8340-0000F80270F8} 
Test {3AC096D0-A1C2-E12C-1390-A8335801FDAB} 
Legacy (2003) Smart Device (C#) {20D4826A-C6FA-45DB-90F4-C717570B9F32} 
Legacy (2003) Smart Device (VB.NET) {CB4CE8C6-1BDB-4DC7-A4D3-65A1999772F8} 
Smart Device (C#) {4D628B5B-2FBC-4AA6-8C16-197242AEB884} 
Smart Device (VB.NET) {68B1623D-7FB9-47D8-8664-7ECEA3297D4F} 
Solution Folder {66A26720-8FB5-11D2-AA7E-00C04F688DDE} 
Workflow (C#) {14822709-B5A1-4724-98CA-57A101D1B079} 
Workflow (VB.NET) {D59BE175-2ED0-4C54-BE3D-CDAA9F3214C8} 
Deployment Merge Module {06A35CCD-C46D-44D5-987B-CF40FF872267} 
Deployment Cab {3EA9E505-35AC-4774-B492-AD1749C4943A} 
Deployment Setup {978C614F-708E-4E1A-B201-565925725DBA} 
Deployment Smart Device Cab {AB322303-2255-48EF-A496-5904EB18DA55} 
Visual Studio Tools for Applications (VSTA) {A860303F-1F3F-4691-B57E-529FC101A107} 
Visual Studio Tools for Office (VSTO) {BAA0C2D2-18E2-41B9-852F-F413020CAA33} 
Visual J# {E6FDF86B-F3D1-11D4-8576-0002A516ECE8} 
SharePoint Workflow {F8810EC1-6754-47FC-A15F-DFABD2E3FA90} 
XNA (Windows) {6D335F3A-9D43-41b4-9D22-F6F17C4BE596} 
XNA (XBox) {2DF5C3F4-5A5F-47a9-8E94-23B4456F55E2} 
XNA (Zune) {D399B71A-8929-442a-A9AC-8BEC78BB2433} 
SharePoint (VB.NET) {EC05E597-79D4-47f3-ADA0-324C4F7C7484} 
SharePoint (C#) {593B0543-81F6-4436-BA1E-4747859CAAE2} 
Silverlight {A1591282-1198-4647-A2B1-27E5FF5F6F3B} 
ASP.NET MVC 1.0	{603C0E0B-DB56-11DC-BE95-000D561079B0}
ASP.NET MVC 2.0	{F85E285D-A4E0-4152-9332-AB1D724D3325}
ASP.NET MVC 3.0	{E53F8FEA-EAE0-44A6-8774-FFD645390401}
ASP.NET MVC 4.0	{E3E379DF-F4C6-4180-9B81-6769533ABE47}
C#	        {FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}
C++	        {8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}
Database	{A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124}
F#	        {F2A71F9B-5D33-465A-A702-920D77279786}
J#	        {E6FDF86B-F3D1-11D4-8576-0002A516ECE8}
Deployment Setup {978C614F-708E-4E1A-B201-565925725DBA}
Database (other project types)	{4F174C21-8C12-11D0-8340-0000F80270F8}
Deployment Cab	{3EA9E505-35AC-4774-B492-AD1749C4943A}
Deployment Merge Module	{06A35CCD-C46D-44D5-987B-CF40FF872267}
Deployment Smart Device Cab	{AB322303-2255-48EF-A496-5904EB18DA55}
Distributed System	{F135691A-BF7E-435D-8960-F99683D2D49C}
Dynamics 2012 AX C# in AOT	{BF6F8E12-879D-49E7-ADF0-5503146B24B8}
Legacy (2003) Smart Device (C#)	{20D4826A-C6FA-45DB-90F4-C717570B9F32}
Legacy (2003) Smart Device (VB.NET)	{CB4CE8C6-1BDB-4DC7-A4D3-65A1999772F8}
Model-View-Controller v2 (MVC2)	{F85E285D-A4E0-4152-9332-AB1D724D3325}
Model-View-Controller v3 (MVC3)	{E53F8FEA-EAE0-44A6-8774-FFD645390401}
Model-View-Controller v4 (MVC4)	{E3E379DF-F4C6-4180-9B81-6769533ABE47}
Mono for Android	{EFBA0AD7-5A72-4C68-AF49-83D382785DCF}
MonoTouch	{6BC8ED88-2882-458C-8E55-DFD12B67127B}
MonoTouch Binding	{F5B4F3BC-B597-4E2B-B552-EF5D8A32436F}
Portable Class Library	{786C830F-07A1-408B-BD7F-6EE04809D6DB}
SharePoint (C#)	{593B0543-81F6-4436-BA1E-4747859CAAE2}
SharePoint (VB.NET)	{EC05E597-79D4-47f3-ADA0-324C4F7C7484}
SharePoint Workflow	{F8810EC1-6754-47FC-A15F-DFABD2E3FA90}
Silverlight	{A1591282-1198-4647-A2B1-27E5FF5F6F3B}
Smart Device (C#)	{4D628B5B-2FBC-4AA6-8C16-197242AEB884}
Smart Device (VB.NET)	{68B1623D-7FB9-47D8-8664-7ECEA3297D4F}
Solution Folder	{2150E333-8FDC-42A3-9474-1A3956D46DE8}
Test	{3AC096D0-A1C2-E12C-1390-A8335801FDAB}
VB.NET	{F184B08F-C81C-45F6-A57F-5ABD9991F28F}
Visual Database Tools	{C252FEB5-A946-4202-B1D4-9916A0590387}
Visual Studio Tools for Applications (VSTA)	{A860303F-1F3F-4691-B57E-529FC101A107}
Visual Studio Tools for Office (VSTO)	{BAA0C2D2-18E2-41B9-852F-F413020CAA33}
Web Application	{349C5851-65DF-11DA-9384-00065B846F21}
Web Site	{E24C65DC-7377-472B-9ABA-BC803B73C61A}
Windows (C#)	{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}
Windows (VB.NET)	{F184B08F-C81C-45F6-A57F-5ABD9991F28F}
Windows (Visual C++)	{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}
Windows Communication Foundation (WCF)	{3D9AD99F-2412-4246-B90B-4EAA41C64699}
Windows Phone 8/8.1 Blank/Hub/Webview App	{76F1466A-8B6D-4E39-A767-685A06062A39}
Windows Phone 8/8.1 App (C#)	{C089C8C0-30E0-4E22-80C0-CE093F111A43}
Windows Phone 8/8.1 App (VB.NET)	{DB03555F-0C8B-43BE-9FF9-57896B3C5E56}
Windows Presentation Foundation (WPF)	{60DC8134-EBA5-43B8-BCC9-BB4BC16C2548}
Windows Store (Metro) Apps & Components	{BC8A1FFA-BEE3-4634-8014-F334798102B3}
Workflow (C#)	{14822709-B5A1-4724-98CA-57A101D1B079}
Workflow (VB.NET)	{D59BE175-2ED0-4C54-BE3D-CDAA9F3214C8}
Workflow Foundation	{32F31D43-81CC-4C15-9DE6-3FC5453562B6}
Xamarin.Android	{EFBA0AD7-5A72-4C68-AF49-83D382785DCF}
Xamarin.iOS	{6BC8ED88-2882-458C-8E55-DFD12B67127B}
XNA (Windows)	{6D335F3A-9D43-41b4-9D22-F6F17C4BE596}
XNA (XBox)	{2DF5C3F4-5A5F-47a9-8E94-23B4456F55E2}
XNA (Zune)	{D399B71A-8929-442a-A9AC-8BEC78BB2433}
'@
$x=[xml]'<VsProjectTypes/>';
$ProjectTypes = @{
		AspNet5 = @{
			Guid = "8BB2217D-0F2D-49D1-97BC-3654ED321F3B";
			Description = "ASP.NET 5";
		};
		MVC1 = @{
			Guid = "603C0E0B-DB56-11DC-BE95-000D561079B0";
			Description = "ASP.NET MVC 1";
		};
		AspNetMvc2 = @{
			Guid = "F85E285D-A4E0-4152-9332-AB1D724D3325";
			Description = "ASP.NET MVC 2";
		};
		AspNetMvc3 = @{
			Guid = "E53F8FEA-EAE0-44A6-8774-FFD645390401";
			Description = "ASP.NET MVC 3";
		};
		AspNetMvc4 = @{
			Guid = "E3E379DF-F4C6-4180-9B81-6769533ABE47";
			Description = "ASP.NET MVC 4";
		};
		AspNetMvc5 = @{
			Guid = "349C5851-65DF-11DA-9384-00065B846F21";
			Description = "ASP.NET MVC 5";
		};
		CSharp = @{
			Guid = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
			Description = "C#";
			Extension = '.csproj';
		};
		CPP = @{
			Guid = "8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942";
			Description = "C++";
		};
		Database = @{
			Guid = "A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124";
			Description = "Database";
		};
		DbOther = @{
			Guid = "4F174C21-8C12-11D0-8340-0000F80270F8";
			Description = "Database (other project types)";
		};
		DeploymentCab = @{
			Guid = "3EA9E505-35AC-4774-B492-AD1749C4943A";
			Description = "Deployment Cab";
		};
		DeploymentMergeModule = @{
			Guid = "06A35CCD-C46D-44D5-987B-CF40FF872267";
			Description = "Deployment Merge Module";
		};
		DeploymentSetup = @{
			Guid = "978C614F-708E-4E1A-B201-565925725DBA";
			Description = "Deployment Setup";
		};
		DeploymentSmartDeviceCab = @{
			Guid = "AB322303-2255-48EF-A496-5904EB18DA55";
			Description = "Deployment Smart Device Cab";
		};
		DistributedSystem = @{
			Guid = "F135691A-BF7E-435D-8960-F99683D2D49C";
			Description = "Distributed System";
		};
		Dynamics2012AxCInAot = @{
			Guid = "BF6F8E12-879D-49E7-ADF0-5503146B24B8";
			Description = "Dynamics 2012 AX C# in AOT";
		};
		Extensibility = @{
		   Guid = "82B43B9B-A64C-4715-B499-D71E9CA2BD60";
		   Description = "Extensibility";
		};
		FSharp = @{
			Guid = "F2A71F9B-5D33-465A-A702-920D77279786";
			Description = "F#";
			Extension = '.fsproj';
		};
		JScript = @{
		   Guid = "262852C6-CD72-467D-83FE-5EEB1973A190";
		   Description = "JScript";
		};
		JSharp = @{
			Guid = "E6FDF86B-F3D1-11D4-8576-0002A516ECE8";
			Description = "J#";
		};
		Legacy2003SmartDeviceCSharp = @{
			Guid = "20D4826A-C6FA-45DB-90F4-C717570B9F32";
			Description = "Legacy (2003) Smart Device (C#)";
		};
		Legacy2003SmartDeviceVbNet = @{
			Guid = "CB4CE8C6-1BDB-4DC7-A4D3-65A1999772F8";
			Description = "Legacy (2003) Smart Device (VB.NET)";
		};
		LightSwitch = @{
		   Guid = "8BB0C5E8-0616-4F60-8E55-A43933E57E9C";
		   Description = "LightSwitch";
		};
		LightSwitchProject = @{
		   Guid = "581633EB-B896-402F-8E60-36F3DA191C85";
		   Description = "LightSwitch Project";
		};
		MicroFramework = @{
			Guid = "b69e3092-b931-443c-abe7-7e7b65f2a37f";
			Description = "Micro Framework";
		};
		MVC2 = @{
			Guid = "F85E285D-A4E0-4152-9332-AB1D724D3325";
			Description = "Model-View-Controller v2 (MVC 2)";
			IsPrimary = $true;
		};
		MVC3 = @{
			Guid = "E53F8FEA-EAE0-44A6-8774-FFD645390401";
			Description = "Model-View-Controller v3 (MVC 3)";
			IsPrimary = $true;
		};
		MVC4 = @{
			Guid = "E3E379DF-F4C6-4180-9B81-6769533ABE47";
			Description = "Model-View-Controller v4 (MVC 4)";
			IsPrimary = $true;
		};
		MVC5 = @{
			Guid = "349C5851-65DF-11DA-9384-00065B846F21";
			Description = "Model-View-Controller v5 (MVC 5)";
			IsPrimary = $true;
		};
		MonoAndroid = @{
			Guid = "EFBA0AD7-5A72-4C68-AF49-83D382785DCF";
			Description = "Mono for Android";
		};
		MonoTouch = @{
			Guid = "6BC8ED88-2882-458C-8E55-DFD12B67127B";
			Description = "MonoTouch";
		};
		MonoTouchBinding = @{
			Guid = "F5B4F3BC-B597-4E2B-B552-EF5D8A32436F";
			Description = "MonoTouch Binding";
		};
		Node_js = @{
			Guid = '9092AA53-FB77-4645-B42D-1CCCA6BD08BD';
			Description = 'Node.js';
			Extension = '.njsproj';
		};
		OfficeSharePointApp = @{
			Guid = "C1CDDADD-2546-481F-9697-4EA41081F2FC";
			Description = "Office/SharePoint App";
		};
		PortableClassLibrary = @{
			Guid = "786C830F-07A1-408B-BD7F-6EE04809D6DB";
			Description = "Portable Class Library";
		};
		ProjectFolders = @{
			Guid = "66A26720-8FB5-11D2-AA7E-00C04F688DDE";
			Description = "Project Folders";
		};
		SharePointCSharp = @{
			Guid = "593B0543-81F6-4436-BA1E-4747859CAAE2";
			Description = "SharePoint (C#)";
		};
		SharePointVbNet = @{
			Guid = "EC05E597-79D4-47f3-ADA0-324C4F7C7484";
			Description = "SharePoint (VB.NET)";
		};
		SharePointWorkflow = @{
			Guid = "F8810EC1-6754-47FC-A15F-DFABD2E3FA90";
			Description = "SharePoint Workflow";
		};
		Silverlight = @{
			Guid = "A1591282-1198-4647-A2B1-27E5FF5F6F3B";
			Description = "Silverlight";
		};
		SmartDeviceCSharp = @{
			Guid = "4D628B5B-2FBC-4AA6-8C16-197242AEB884";
			Description = "Smart Device (C#)";
		};
		SmartDeviceVbNet = @{
			Guid = "68B1623D-7FB9-47D8-8664-7ECEA3297D4F";
			Description = "Smart Device (VB.NET)";
		};
		SolutionFolder = @{
			Guid = "2150E333-8FDC-42A3-9474-1A3956D46DE8";
			Description = "Solution Folder";
		};
		Test = @{
			Guid = "3AC096D0-A1C2-E12C-1390-A8335801FDAB";
			Description = "Test";
		};
		UniversalStoreApp = @{
			Guid = "D954291E-2A0B-460D-934E-DC6B0785DB48";
			Description = "Store App Universal";
			Extension = '.shproj';
		};
		UniversalWindowsClassLibrary = @{
			Guid = "A5A43C5B-DE2A-4C0C-9213-0A381AF9435A";
			Description = "Universal Windows Class Library";
		};
		VbNet = @{
			Guid = "F184B08F-C81C-45F6-A57F-5ABD9991F28F";
			Description = "VB.NET";
			Extension = '.vbproj';
		};
		VisualDatabaseTools = @{
			Guid = "C252FEB5-A946-4202-B1D4-9916A0590387";
			Description = "Visual Database Tools";
		};
		VS2015InstallerProjectExtension = @{
			Guid = "54435603-DBB4-11D2-8724-00A0C9A8B90C";
			Description = "Visual Studio 2015 Installer Project Extension";
		};
		VSTA = @{
			Guid = "A860303F-1F3F-4691-B57E-529FC101A107";
			Description = "Visual Studio Tools for Applications (VSTA)";
		};
		VSTO = @{
			Guid = "BAA0C2D2-18E2-41B9-852F-F413020CAA33";
			Description = "Visual Studio Tools for Office (VSTO)";
		};
		WCF = @{
			Guid = "3D9AD99F-2412-4246-B90B-4EAA41C64699";
			Description = "Windows Communication Foundation (WCF)";
		};
		WebApplication = @{
			Guid = "349C5851-65DF-11DA-9384-00065B846F21";
			Description = "Web Application";
		};
		WebSite = @{
			Guid = "E24C65DC-7377-472B-9ABA-BC803B73C61A";
			Description = "Web Site";
		};
		WindowsCSharp = @{
			Guid = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
			Description = "Windows (C#)";
			IsPrimary = $true;
			Extension = '.csproj';
		};
		WindowsPowerShell = @{
			Guid = 'F5034706-568F-408A-B7B3-4D38C6DB8A32'
			Description = 'Windows PowerShell Script';
			Extension = '.pssproj';
		};
		WindowsVbNet = @{
			Guid = "F184B08F-C81C-45F6-A57F-5ABD9991F28F";
			Description = "Windows (VB.NET)";
			IsPrimary = $true;
			Extension = '.vbproj';
		};
		WindowsVisualCPP = @{
			Guid = "8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942";
			Description = "Windows (Visual C++)";
			IsPrimary = $true;
		};
		WindowsPhone8Other = @{
			Guid = "76F1466A-8B6D-4E39-A767-685A06062A39";
			Description = "Windows Phone 8/8.1 Blank/Hub/Webview App";
		};
		Windows_Phone8AppCSharp = @{
			Guid = "C089C8C0-30E0-4E22-80C0-CE093F111A43";
			Description = "Windows Phone 8/8.1 App (C#)";
		};
		Windows_Phone8AppVbNet = @{
			Guid = "DB03555F-0C8B-43BE-9FF9-57896B3C5E56";
			Description = "Windows Phone 8/8.1 App (VB.NET)";
		};
		WPF = @{
			Guid = "60DC8134-EBA5-43B8-BCC9-BB4BC16C2548";
			Description = "Windows Presentation Foundation (WPF)";
		};
		Metro = @{
			Guid = "BC8A1FFA-BEE3-4634-8014-F334798102B3";
			Description = "Windows Store (Metro) Apps & Components";
		};
		WorkflowCSharp = @{
			Guid = "14822709-B5A1-4724-98CA-57A101D1B079";
			Description = "Workflow (C#)";
		};
		WorkflowVbNet = @{
			Guid = "D59BE175-2ED0-4C54-BE3D-CDAA9F3214C8";
			Description = "Workflow (VB.NET)";
		};
		WF = @{
			Guid = "32F31D43-81CC-4C15-9DE6-3FC5453562B6";
			Description = "Workflow Foundation";
		};
		XamarinAndroid = @{
			Guid = "EFBA0AD7-5A72-4C68-AF49-83D382785DCF";
			Description = "Xamarin.Android";
			IsPrimary = $true;
		};
		XamarinIOS = @{
			Guid = "6BC8ED88-2882-458C-8E55-DFD12B67127B";
			Description = "Xamarin.iOS";
			IsPrimary = $true;
		};
		XnaWindows = @{
			Guid = "6D335F3A-9D43-41b4-9D22-F6F17C4BE596";
			Description = "XNA (Windows)";
		};
		XnaXBox = @{
			Guid = "2DF5C3F4-5A5F-47a9-8E94-23B4456F55E2";
			Description = "XNA (XBox)";
		};
		XnaZune = @{
			Guid = "D399B71A-8929-442a-A9AC-8BEC78BB2433";
			Description = "XNA (Zune)";
		};
	};
<#
		WindowsCSharp = @{
			Guid = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
			Description = "Windows (C#)";
			IsPrimary = $true;
			Extension = '.csproj';
		};
#>

$vsk = [Microsoft.Win32.Registry]::Users.OpenSubKey('.DEFAULT\Software\Microsoft\VisualStudio');
if ($vsk -ne $null) {
    $vsk.GetSubKeyNames() | ForEach-Object {
        $vk = $vsk.OpenSubKey($_);
        if ($vk.GetSubKeyNames() -contains 'Projects') {
            $prjck = $vk.OpenSubKey('Projects');
            $prjck.GetSubKeyNames() | ForEach-Object {
                $prjk = $prjck.OpenSubKey($_);
            
                $d = $prjk.GetValue('DisplayName');
                $l = $prjk.GetValue('Language(VsTemplate)');
                $k = $prjk.GetValue('');
                if ($k -eq $null) { $k = $_ }
                if ([string]::IsNullOrEmpty($d)) {
                    if ([string]::IsNullOrEmpty($l)) { $d = $k } else { $d = $l }
                }
                $pg = [Guid]::Empty;
                $s = $prjk.GetValue('Package');
                if (-not [string]::IsNullOrEmpty($s)) {
                    [Guid]$pg = $s;
                }
                [Guid]$g = $_;
                $v = $g.ToString();
                $e=$x.DocumentElement.SelectSingleNode("ProjectType[@Guid='$v']");
                if($e -eq $null){
                    $e=$x.DocumentElement.AppendChild($x.CreateElement('ProjectType'));
                    $e.Attributes.Append($x.CreateAttribute('Key')).Value=$k;
                    $e.Attributes.Append($x.CreateAttribute('Guid')).Value=$v;
                    $e.Attributes.Append($x.CreateAttribute('Description')).Value=$d;
                    if (-not [string]::IsNullOrEmpty($l)) { $e.Attributes.Append($x.CreateAttribute('Language')).Value=$l }
                    if (-not ($pg.Equals([Guid]::Empty) -or $pg.Equals($g))) { $e.Attributes.Append($x.CreateAttribute('Package')).Value=$pg.ToString() }
                }else{
                    $n=$e.AppendChild($x.CreateElement('AltDescription'));
                    $n.Attributes.Append($x.CreateAttribute('Key')).Value=$k;
                    if (-not [string]::IsNullOrEmpty($l)) { $n.Attributes.Append($x.CreateAttribute('Language')).Value=$l }
                    if (-not ($pg.Equals([Guid]::Empty) -or $pg.Equals($g))) { $n.Attributes.Append($x.CreateAttribute('Package')).Value=$pg.ToString() }
                    $n.InnerText=$ProjectTypes[$k].Description;
                }
                $dpe = $prjk.GetValue('DefaultProjectExtension');
                $a = $e.SelectSingleNode('Extension');
                if ([string]::IsNullOrEmpty($dpe)) {
                    if ($a -eq $null) { $dpe = '' } else { $dpe = $a.Value }
                } else {
                    if (-not $dpe.StartsWith('.')) { $dpe = ".$dpe" }
                    if($a -ne $null){
                        if ($a.Value -ine $dpe) {
                            $e.AppendChild($x.CreateElement('AltExt')).InnerText = $dpe;
                            $dpe = $a.Value;
                        }
                    }else{
                        $e.Attributes.Append($x.CreateAttribute('Extension')).Value=$dpe;
                    }
                }
                $ape = $prjk.GetValue('PossibleProjectExtensions');
                if (-not [string]::IsNullOrEmpty($ape)) {
                    $ape.Split(';') | ForEach-Object {
                        $s = $_;
                        if (-not $s.StartsWith('.')) { $s = ".$s" }
                        if ($s -ine $dpe -and $e.SelectSingleNode("AltExt[.='$s']") -eq $null) { $e.AppendChild($x.CreateElement('AltExt')).InnerText = $s }
                    }
                }
                $prjk.Close();
            }
            $prjck.Close();
        }
        $vk.Close();

    }
    $vsk.Close();
};

foreach ($k in $ProjectTypes.Keys){
    [guid]$g=$ProjectTypes[$k].Guid;
    $v =$g.ToString();
    $e=$x.DocumentElement.SelectSingleNode("ProjectType[@Guid='$v']");
    if($e -eq $null){
        $e=$x.DocumentElement.AppendChild($x.CreateElement('ProjectType'));
        $e.Attributes.Append($x.CreateAttribute('Key')).Value=$k;
        $e.Attributes.Append($x.CreateAttribute('Guid')).Value=$v;
        $e.Attributes.Append($x.CreateAttribute('Description')).Value=$ProjectTypes[$k].Description;
    }else{
        if($ProjectTypes[$k].IsPrimary){
            $a1=$e.SelectSingleNode('@Description');
            $a2=$e.SelectSingleNode('@Key');
            $n=$e.AppendChild($x.CreateElement('AltDescription'));
            $n.Attributes.Append($x.CreateAttribute('Key')).Value=$a2.Value;
            $n.InnerText=$a1.Value;
            $a1.Value=$ProjectTypes[$k].Description;
            $a2.Value=$k;
        }else{
            $n=$e.AppendChild($x.CreateElement('AltDescription'));
            $n.Attributes.Append($x.CreateAttribute('Key')).Value=$k;
            $n.InnerText=$ProjectTypes[$k].Description;
        }
    }
    if ($ProjectTypes[$k].Extension -ne $null){
        $a = $e.SelectSingleNode('Extension');
        if ($a -eq $null) { $e.Attributes.Append($x.CreateAttribute('Extension')).Value=$ProjectTypes[$k].Extension; } else {
            if ($ProjectTypes[$k].Extension -ine $a.Value) { $e.AppendChild($x.CreateElement('AltExt')).InnerText = $ProjectTypes[$k].Extension }
        }

    }
}
($t -split '\r\n?|\n') |ForEach-Object{
$i=$_.indexof('{');
[guid]$g=$_.substring($i+1,($_.indexof('}')-$i)-1).trim();
$v =$g.ToString();
if($x.DocumentElement.SelectSingleNode("ProjectType[@Guid='$v']") -eq $null){
$e=$x.DocumentElement.AppendChild($x.CreateElement('ProjectType'));
$e.Attributes.Append($x.CreateAttribute('Guid')).Value=$v;
$e.Attributes.Append($x.CreateAttribute('Description')).Value=$_.substring(0,$i).trim();
}}
$ws=new-object -typename 'System.Xml.XmlWriterSettings'
$ws.Indent=$true;
$w=[System.Xml.XmlWriter]::Create($ProjectTypesPath,$ws);
$x.WriteTo($w);$w.Flush();
$w.Close();