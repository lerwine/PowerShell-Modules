Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../../dist/Erwine.Leonard.T.IOUtility') -ErrorAction Stop;
Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../../dist/Erwine.Leonard.T.LteDev') -ErrorAction Stop;

$ValidNames = @({'NormalName'}, {'lcFirst'}, {'has_underscore'}, {'_startsWithUndersore'}, {'hasNumber3'}, {'x'}, {'__'}, {'_'}, {'_123'},
    {'__Has_itAll4_the_last1__'});
$InvalidNonNil = @({' '}, {'5'}, {'$'}, {'0StartsWithNumber'}, {'Is.Namespace'}, {' LeadsWithWs'}, {'HasTrailingWs '}, {'Contains Space'});
$InvalidNil = @({$null}, {''});
$InvalidNames = $InvalidNil + $InvalidNonNil
$Position = 0;
$ValidMulti = @($ValidNames | ForEach-Object {
    $Position++;
    if (($Position % 2) -eq 0) {
        [scriptblock]::Create($_.ToString().Trim() + ', ' + (@($ValidNames | Select-Object -Last $Position | ForEach-Object { $_.ToString().Trim() }) -join ', '));
    } else {
        [scriptblock]::Create((@($ValidNames | Select-Object -Last $Position | ForEach-Object { $_.ToString().Trim() }) -join ', ') + ', ' + $_.ToString().Trim());
    }
});
$Position = 0;
$InvalidMulti = @($InvalidNames | ForEach-Object {
    $Position++;
    [scriptblock]::Create($_.ToString() + ', "ValidName"');
    [scriptblock]::Create('"ValidName", ' + $_.ToString());
    if (($Position % 2) -eq 0) {
        [scriptblock]::Create($_.ToString().Trim() + ', ' + (@($ValidNames | Select-Object -Last $Position | ForEach-Object { $_.ToString().Trim() }) -join ', '));
    } else {
        [scriptblock]::Create((@($ValidNames | Select-Object -Last $Position | ForEach-Object { $_.ToString().Trim() }) -join ', ') + ', ' + $_.ToString().Trim());
    }
});

Describe 'Testing Test-CsName Cmdlet' {
    Context 'Invoking using parameter with single string' {
        foreach ($Target in $ValidNames) {
            It "(Test-CsName -Name $Target) should return true" {
                $Actual = Test-CsName -Name $Target.Invoke();
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $true;
            }
        }
        foreach ($Target in $InvalidNames) {
            It "(Test-CsName -Name $Target) should return false" {
                $Actual = Test-CsName -Name $Target.Invoke();
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $false;
            }
        }
    }
    Context 'Invoking using single string through pipeline' {
        foreach ($Target in $ValidNames) {
            It "($Target | Test-CsName) should return true" {
                $Actual = $Target.Invoke() | Test-CsName;
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $true;
            }
        }
        foreach ($Target in $InvalidNames) {
            It "($Target | Test-CsName) should return false" {
                $Actual = $Target.Invoke() | Test-CsName;
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $false;
            }
        }
    }
    Context "Invoking using parameter with multiple strings" {
        foreach ($Target in $ValidMulti) {
            It "(Test-CsName -Name $Target) should return true" {
                $Actual = Test-CsName -Name @($Target.Invoke());
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $true;
            }
        }
        foreach ($Target in $InvalidMulti) {
            It "(Test-CsName -Name $Target) should return false" {
                $Actual = Test-CsName -Name @($Target.Invoke());
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $false;
            }
        }
    }
    Context "Invoking using multiple strings through pipeline" {
        foreach ($Target in $ValidMulti) {
            It "($Target | Test-CsName) should return true" {
                $Actual = @($Target.Invoke()) | Test-CsName;
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $true;
            }
        }
        foreach ($Target in $InvalidMulti) {
            It "($Target | Test-CsName) should return false" {
                $Actual = @($Target.Invoke()) | Test-CsName;
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $false;
            }
        }
    }
}
$ValidMulti = @($ValidNames | ForEach-Object {
    $Position++;
    if (($Position % 2) -eq 0) {
        [scriptblock]::Create($_.ToString().Trim() + ', ' + (@($ValidNames | Select-Object -Last $Position | ForEach-Object { $_.ToString().Trim() }) -join ', '));
    } else {
        
    }
});
$ValidNs = $ValidNames + @({'NormalName.lcFirst'}, {'has_underscore._startsWithUndersore.hasNumber3'}, {'x.__._._123.__Has_itAll4_the_last1__'});
$InvalidNs = ($InvalidNames | Where-Object { $_.ToString() -ne "'Is.Namespace'" }) + @({' .5'}, {'$.0StartsWithNumber.Is.Namespace'}, {' LeadsWithWs.HasTrailingWs .Contains Space'});
Describe 'Testing Test-CsNamespace Cmdlet' {
    Context 'Invoking using parameter with single string' {
        foreach ($Target in $ValidNs) {
            It "(Test-CsNamespace -Name $Target) should return true" {
                $Actual = Test-CsNamespace -Name $Target.Invoke();
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $true;
            }
        }
        foreach ($Target in (@({$null}) + $InvalidNs)) {
            It "(Test-CsNamespace -Name $Target) string should return false" {
                $Actual = Test-CsNamespace -Name $Target.Invoke();
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $false;
            }
        }
    }
    Context 'Invoking using single string through pipeline' {
        foreach ($Target in @({'TestName'}, {'test_Name'}, {'x'}, {'_'}, {'_test'}, {'_123'}, {'TestName.test_Name'}, {'x._._test._123'})) {
            It "($Target | Test-CsNamespace) should return true" {
                $Actual = $Target.Invoke() | Test-CsNamespace;
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $true;
            }
        }
        foreach ($Target in @({$null}, {''}, {' '}, {'5'}, {'$'}, {'0TestName'}, {' TestName'}, {'TestName '}, {'Test..Name'}, {'Test Name'},
                {'.TestName'}, {'TestName.'}, {'TestName.5'}, {'$.TestName'}, {'TestName.0TestName'}, {' TestName.TestName'}, {'TestName.TestName '},
                {'TestName. Test.Name'}, {'Test Name'})) {
            It "($Target | Test-CsNamespace) should return false" {
                $Actual = $Target.Invoke() | Test-CsNamespace;
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $false;
            }
        }
    }
    Context "Invoking using parameter with multiple strings" {
        foreach ($Target in @({'TestName', 'test_Name'}, {'x', '_', '_test'}, {'_123', 'TestName.test_Name', 'x._._test._123'})) {
            It "(Test-CsNamespace -Name $Target) should return true" {
                $Actual = Test-CsNamespace -Name @($Target.Invoke());
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $true;
            }
        }
        foreach ($Target in @({$null, 'TestName'}, {'', 'TestName'}, {'TestName', ' '}, {'5', 'TestName'}, {'TestName', '$'},
                {'0TestName', 'TestName'}, {'TestName', ' TestName'}, {'TestName ', 'TestName'}, {'Test Name', 'TestName'},
                {'.TestName', 'TestName'}, {'TestName', 'TestName.'}, {'TestName.5', 'TestName'}, {'TestName', '$.TestName'}, {'TestName.0TestName', 'TestName'},
                {'TestName', ' TestName..TestName'}, {'TestName..TestName ', 'TestName'}, {'TestName', 'TestName. Test.Name'}, {'Test Name', 'TestName'})) {
            It "(Test-CsNamespace -Name $Target) should return false" {
                $Actual = Test-CsNamespace -Name @($Target.Invoke());
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $false;
            }
        }
    }
    Context "Invoking using multiple strings through pipeline" {
        foreach ($Target in @({'TestName', 'test_Name'}, {'x', '_', '_test'}, {'_123', 'TestName.test_Name', 'x._._test._123'})) {
            It "($Target | Test-CsNamespace) should return true" {
                $Actual = @($Target.Invoke()) | Test-CsNamespace;
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $true;
            }
        }
        foreach ($Target in @({$null, 'TestName'}, {'', 'TestName'}, {'TestName', ' '}, {'5', 'TestName'}, {'TestName', '$'},
                {'0TestName', 'TestName'}, {'TestName', ' TestName'}, {'TestName ', 'TestName'}, {'Test Name', 'TestName'},
                {'.TestName', 'TestName'}, {'TestName', 'TestName.'}, {'TestName.5', 'TestName'}, {'TestName', '$.TestName'}, {'TestName.0TestName', 'TestName'},
                {'TestName', ' TestName..TestName'}, {'TestName..TestName ', 'TestName'}, {'TestName', 'TestName. Test.Name'}, {'Test Name', 'TestName'})) {
            It "($Target | Test-CsNamespace) should return false" {
                $Actual = @($Target.Invoke()) | Test-CsNamespace;
                $Actual | Should BeOfType [bool];
                $Actual | Should Be $false;
            }
        }
    }
}