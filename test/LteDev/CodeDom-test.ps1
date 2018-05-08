Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../../dist/Erwine.Leonard.T.LteDev/Erwine.Leonard.T.IOUtility.psd1');
Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../../dist/Erwine.Leonard.T.LteDev/Erwine.Leonard.T.LteDev.psd1');

Describe 'Testing Test-CsName Cmdlet' {
    Context 'Invoking using parameter with single string' {
        foreach ($Target in @({'TestName'}, {'test_Name'}, {'x'}, {'_'}, {'_test'}, {'_123'})) {
            It "$($Target.ToString()) string should return true" {
                $Actual = Test-CsName -Name $Target.Invoke();
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $true;
            }
        }
        foreach ($Target in @({$null}, {[string[]]@()}, {''}, {' '}, {'5'}, {'$'}, {'0TestName'}, {'.TestName'}, {' TestName'}, {'TestName '}, {'Test.Name'}, {'Test Name'})) {
            It "($Target.ToString()) string should return false" {
                $Actual = Test-CsName -Name $Target.Invoke();
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $false;
            }
        }
    }
    Context 'Invoking using single string through pipeline' {
        foreach ($Target in @({'TestName'}, {'test_Name'}, {'x'}, {'_'}, {'_test'}, {'_123'})) {
            It "$($Target.ToString()) string should return true" {
                $Actual = $Target.Invoke() | Test-CsName;
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $true;
            }
        }
        foreach ($Target in @({$null}, {[string[]]@()}, {''}, {' '}, {'5'}, {'$'}, {'0TestName'}, {'.TestName'}, {' TestName'}, {'TestName '}, {'Test.Name'}, {'Test Name'})) {
            It "($Target.ToString()) string should return false" {
                $Actual = $Target.Invoke() | Test-CsName;
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $false;
            }
        }
    }
    Context "Invoking using parameter with multiple strings" {
        foreach ($Target in @({'TestName', 'test_Name'}, {'x', '_'}, {'_test', '_123'})) {
            It "$($Target.ToString()) string should return true" {
                $Actual = Test-CsName -Name @($Target.Invoke());
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $true;
            }
        }
        foreach ($Target in @({$null, 'TestName'}, {'TestName', [string[]]@()}, {'', 'TestName'}, {'TestName', ' '}, {'5', 'TestName'}, {'TestName', '$'}, {'0TestName', 'TestName'}, {'.TestName'}, {' TestName'}, {'TestName '}, {'Test.Name'}, {'Test Name'})) {
            It "($Target.ToString()) string should return false" {
                $Actual = Test-CsName -Name @($Target.Invoke());
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $false;
            }
        }
    }
    Context "Invoking using multiple strings through pipeline" {
        foreach ($Target in @({'TestName', 'test_Name'}, {'x', '_'}, {'_test', '_123'})) {
            It "$($Target.ToString()) string should return true" {
                $Actual = @($Target.Invoke()) | Test-CsName;
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $true;
            }
        }
        foreach ($Target in @({$null, 'TestName'}, {'TestName', [string[]]@()}, {'', 'TestName'}, {'TestName', ' '}, {'5', 'TestName'}, {'TestName', '$'}, {'0TestName', 'TestName'}, {'.TestName'}, {' TestName'}, {'TestName '}, {'Test.Name'}, {'Test Name'})) {
            It "($Target.ToString()) string should return false" {
                $Actual = @($Target.Invoke()) | Test-CsName;
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $false;
            }
        }
    }
}

Describe 'Testing Test-CsNamespace Cmdlet' {
    Context 'Invoking using parameter with single string' {
        foreach ($Target in @({'TestName'}, {'test_Name'}, {'x'}, {'_'}, {'_test'}, {'_123'}, {'TestName.test_Name'}, {'x._._test._123'})) {
            It "$($Target.ToString()) string should return true" {
                $Actual = Test-CsNamespace -Name $Target.Invoke();
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $true;
            }
        }
        foreach ($Target in @({$null}, {[string[]]@()}, {''}, {' '}, {'5'}, {'$'}, {'0TestName'}, {' TestName'}, {'TestName '}, {'Test.Name'}, {'Test Name'},
                {'.TestName'}, {'TestName.'}, {'TestName.5'}, {'$.TestName'}, {'TestName.0TestName'}, {' TestName.TestName'}, {'TestName.TestName '}, {'TestName. Test.Name'}, {'Test Name'})) {
            It "($Target.ToString()) string should return false" {
                $Actual = Test-CsNamespace -Name $Target.Invoke();
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $false;
            }
        }
    }
    Context 'Invoking using single string through pipeline' {
        foreach ($Target in @({'TestName'}, {'test_Name'}, {'x'}, {'_'}, {'_test'}, {'_123'}, {'TestName.test_Name'}, {'x._._test._123'})) {
            It "$($Target.ToString()) string should return true" {
                $Actual = $Target.Invoke() | Test-CsNamespace;
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $true;
            }
        }
        foreach ($Target in @({$null}, {[string[]]@()}, {''}, {' '}, {'5'}, {'$'}, {'0TestName'}, {' TestName'}, {'TestName '}, {'Test.Name'}, {'Test Name'},
                {'.TestName'}, {'TestName.'}, {'TestName.5'}, {'$.TestName'}, {'TestName.0TestName'}, {' TestName.TestName'}, {'TestName.TestName '}, {'TestName. Test.Name'}, {'Test Name'})) {
            It "($Target.ToString()) string should return false" {
                $Actual = $Target.Invoke() | Test-CsNamespace;
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $false;
            }
        }
    }
    Context "Invoking using parameter with multiple strings" {
        foreach ($Target in @({'TestName', 'test_Name'}, {'x', '_', '_test'}, {'_123', 'TestName.test_Name', 'x._._test._123'})) {
            It "$($Target.ToString()) string should return true" {
                $Actual = Test-CsNamespace -Name @($Target.Invoke());
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $true;
            }
        }
        foreach ($Target in @({$null, 'TestName'}, {'TestName', [string[]]@()}, {'', 'TestName'}, {'TestName', ' '}, {'5', 'TestName'}, {'TestName', '$'}, {'0TestName', 'TestName'}, {'TestName', ' TestName'}, {'TestName ', 'TestName'}, {'TestName', 'Test.Name'}, {'Test Name', 'TestName'},
                {'.TestName', 'TestName'}, {'TestName', 'TestName.'}, {'TestName.5', 'TestName'}, {'TestName', '$.TestName'}, {'TestName.0TestName', 'TestName'}, {'TestName', ' TestName.TestName'}, {'TestName.TestName ', 'TestName'}, {'TestName', 'TestName. Test.Name'}, {'Test Name', 'TestName'})) {
            It "($Target.ToString()) string should return false" {
                $Actual = Test-CsNamespace -Name @($Target.Invoke());
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $false;
            }
        }
    }
    Context "Invoking using multiple strings through pipeline" {
        foreach ($Target in @({'TestName', 'test_Name'}, {'x', '_', '_test'}, {'_123', 'TestName.test_Name', 'x._._test._123'})) {
            It "$($Target.ToString()) string should return true" {
                $Actual = @($Target.Invoke()) | Test-CsNamespace;
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $true;
            }
        }
        foreach ($Target in @({$null, 'TestName'}, {'TestName', [string[]]@()}, {'', 'TestName'}, {'TestName', ' '}, {'5', 'TestName'}, {'TestName', '$'}, {'0TestName', 'TestName'}, {'TestName', ' TestName'}, {'TestName ', 'TestName'}, {'TestName', 'Test.Name'}, {'Test Name', 'TestName'},
                {'.TestName', 'TestName'}, {'TestName', 'TestName.'}, {'TestName.5', 'TestName'}, {'TestName', '$.TestName'}, {'TestName.0TestName', 'TestName'}, {'TestName', ' TestName.TestName'}, {'TestName.TestName ', 'TestName'}, {'TestName', 'TestName. Test.Name'}, {'Test Name', 'TestName'})) {
            It "($Target.ToString()) string should return false" {
                $Actual = @($Target.Invoke()) | Test-CsNamespace;
                $Actual | Should -BeOfType [bool];
                $Actual | Should -Be $false;
            }
        }
    }
}

Push-Location -Path (Get-Location);
Set-Location -Path 'C:\Users\lerwi\GitHub\PowerShell-Modules-PR2\test\IOUtility';
@('Get-SpecialFolderNames', 'Get-SpecialFolder', 'ConvertTo-SafeFileName', 'ConvertFrom-SafeFileName', 'Get-AppDataPath', 'New-WindowOwner', 'Read-FileDialog',
	'Get-MinBase64BlockSize', 'Read-IntegerFromStream', 'Read-LongIntegerFromStream', 'Write-IntegerToStream', 'Write-LongIntegerToStream', 'Read-LengthEncodedBytes', 'Write-LengthEncodedBytes',
	'ConvertTo-Base64String', 'ConvertFrom-Base64String', 'Get-TextEncoding', 'New-MemoryStream', 'New-DataTable', 'Add-DataColumn', 'Test-IsNullOrWhitespace', 'Split-DelimitedText',
	'Out-NormalizedText', 'Out-IndentedText', 'Get-IndentLevel', 'Out-UnindentedText', 'Compare-FileSystemInfo', 'Test-PathsAreEqual') | ForEach-Object {
        
    }