Param(
	[Parameter(Mandatory = $true, Position = 0)]
	[object]$Sender,

	
	[Parameter(Mandatory = $true, Position = 1)]
	[System.ComponentModel.PropertyChangedEventArgs]$E
)

"Again" | Write-Output;
$SynchronizedData['xyz'] = $args.Count;
$TestVar2 = $E;
$TestVar = 12;
$this.Value = 40;