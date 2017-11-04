@('PresentationCore', 'PresentationFramework', 'System', 'System.Core', 'System.Xaml', 'System.Xml', 'WindowsBase') | ForEach-Object { Add-Type -AssemblyName $_ }
Add-Type -Path ($PSScriptRoot | Join-Path -ChildPath 'Erwine.Leonard.T.IOUtility.dll');
Add-Type -Path ($PSScriptRoot | Join-Path -ChildPath 'Erwine.Leonard.T.WPF.dll');
Add-Type -Path ($PSScriptRoot | Join-Path -ChildPath 'Erwine.Leonard.T.Speech.dll');
$XmlEditorWindow = New-Object -TypeName 'Speech.GUI.XmlEditorWindow';
$XmlEditorWindow.ShowDialog();