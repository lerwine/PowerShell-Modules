Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../../dist/Erwine.Leonard.T.LteDev/Erwine.Leonard.T.IOUtility.psd1');
Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../../dist/Erwine.Leonard.T.LteDev/Erwine.Leonard.T.LteDev.psd1');

Describe 'Test-CsName' {
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

Describe 'Test-CsNamespace' {
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