$ScriptBlock = {
    $ScriptBlock = {
        [DateTime]$Birthday = '2/1/1967';
        [DateTime]::Now
    }
    &$ScriptBlock;
    $Hash = @{
        Object = [long]1, 'two', # Comment here? 
            'three'
        Separator = $null
    }

    Write-Host -Separator "`t" `
		-ForegroundColor Black -Object @{ One = 'Two'; Three = 'four' }
    Write-Host @Hash;
    . c:\scripts\sample.ps1;
    $(if ($var -eq $null) { 'Nully' } else { "Not!" });
    [ordered]@{a=1; b=2; c=3}
    "This is a string, this $variable is expanded as is $(2+2)"
    $global:a = 1;
    # strongly-typed variable (can contain only integers)
    [int]$number=8
    # attributes can be used on variables
    [ValidateRange(1,10)][int]$number = 1
    $number = 11
    #returns an error
    # flip variables
    $a=1;$b=2; $a,$b = $b,$a
    # multi assignment
    $a,$b,$c = 0
    $a,$b,$c = 'a','b','c'
    $a,$b,$c = 'a b c'.split();
    $MyArr = $arr[2..20];
    Function Get-Test {
        Param(
            [Parameter(Mandatory = $true)]
            [string]$Test
        )

        return $Test + $Test;
    }
};
$Text = $ScriptBlock.ToString();
$ParseErrors = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.PSParseError]';
$Tokens = [System.Management.Automation.PSParser]::Tokenize($Text, [ref]$ParseErrors);
$Tokens | Select-Object -Property @{Name = 'InnerContent'; Expression = { '"' + $_.Content + '"' } }, @{Name = 'InnerLength'; Expression = { $_.Content.Length } }, 'Type', @{Name = 'OuterContent'; Expression = { '"' + $Text.Substring($_.Start, $_.Length) + '"' } }, @{Name = 'OuterLength'; Expression = { $_.Length } } | Out-GridView;