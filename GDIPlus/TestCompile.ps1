$Path = @(('ExifPropertyDetailAttribute.cs', 'ExifPropertyTag.cs', 'ExifPropertyType.cs') | ForEach-Object { $PSScriptRoot | Join-Path -ChildPath $_ });
$ReferencedAssemblies = @(([System.Drawing.Image], [System.Windows.Forms.Form]) | ForEach-Object { $_.Assembly.Location });

Add-Type -Path $Path -ReferencedAssemblies $ReferencedAssemblies;