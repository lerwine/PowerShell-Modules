Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.SwPackage.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'No Optional Arguments' {
    Context 'Default ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '';
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-beta.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-beta.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
}

Describe '-SemVer' {
    Context 'Default ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -SemVer;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }

        It "Version strings with optional zero values should return zero" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = $null; RVersion = '0'; },
                [PSCustomObject]@{ LVersion = ''; RVersion = '0'; },
                [PSCustomObject]@{ LVersion = '0'; RVersion = $null; },
                [PSCustomObject]@{ LVersion = '0'; RVersion = ''; },
                [PSCustomObject]@{ LVersion = '1'; RVersion = '1'; },
                [PSCustomObject]@{ LVersion = '1.0'; RVersion = '1'; },
                [PSCustomObject]@{ LVersion = '1'; RVersion = '1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1'; },
                [PSCustomObject]@{ LVersion = '1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0'; },
                [PSCustomObject]@{ LVersion = '1.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0'; RVersion = '01'; },
                [PSCustomObject]@{ LVersion = '01'; RVersion = '1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -Be 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }
        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal -SemVer;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase -SemVer;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
}

Describe '-DotSeparated' {
    Context 'Default ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -DotSeparated;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }
        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal -DotSeparated;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }
        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase -DotSeparated;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
    }
}

Describe '-DontAssumeZeroElement' {
    Context 'Default ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-beta.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-beta.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-beta.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
}

Describe '-NullNotSameAsEmpty' {
    Context 'Default ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -NullNotSameAsEmpty;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -NullNotSameAsEmpty;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -NullNotSameAsEmpty;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-beta.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal -NullNotSameAsEmpty;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal -NullNotSameAsEmpty;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal -NullNotSameAsEmpty;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-beta.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-beta.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
}

Describe '-DontAssumeZeroElement -SemVer' {
    Context 'Default ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
}

Describe '-DontAssumeZeroElement -DotSeparated' {
    Context 'Default ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null compared to empty should return 0" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -Be 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
}

Describe '-NullNotSameAsEmpty -SemVer' {
    Context 'Default ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -NullNotSameAsEmpty -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -NullNotSameAsEmpty -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -NullNotSameAsEmpty -SemVer;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal -NullNotSameAsEmpty -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal -NullNotSameAsEmpty -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal -NullNotSameAsEmpty -SemVer;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -SemVer;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
}

Describe '-NullNotSameAsEmpty -DotSeparated' {
    Context 'Default ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -NullNotSameAsEmpty -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -NullNotSameAsEmpty -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -NullNotSameAsEmpty -DotSeparated;
                $Actual | Should -BeOfType [int -Because ($VersionString | ConvertTo-Json)];
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal -NullNotSameAsEmpty -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal -NullNotSameAsEmpty -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal -NullNotSameAsEmpty -DotSeparated;
                $Actual | Should -BeOfType [int -Because ($VersionString | ConvertTo-Json)];
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DotSeparated;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
}

Describe '-NullNotSameAsEmpty -DontAssumeZeroElement' {
    Context 'Default ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -NullNotSameAsEmpty -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -NullNotSameAsEmpty -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -NullNotSameAsEmpty -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-beta.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-beta.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', 'icons-12.8.0', 'vscode-icons-team.vscode-icons-12.8.0', 'redhat.vscode-xml-0.27.2024071808@win32-x64',
                    '1.0.0-x.7.z.92', '1.0.0-x-y-z.--', '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-beta.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'other-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-12.8.0'; RVersion = 'example-vendor.other-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-9.8.0'; RVersion = 'example-vendor.example-id-10.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.beta'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = 'other-vendor.example-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.other-id-12.8.0'; RVersion = 'example-vendor.example-id-12.8.0'; },
                [PSCustomObject]@{ LVersion = 'example-vendor.example-id-10.8.0'; RVersion = 'example-vendor.example-id-9.8.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
}

Describe '-NullNotSameAsEmpty -DontAssumeZeroElement -SemVer' {
    Context 'Default ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int];
                $Actual | Should -Be 0;
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9', '1.0.0-0.3.7', '1.0.0-x.7.z.92', '1.0.0-x-y-z.--',
                '1.0.0-alpha+001', '1.0.0+20130313144700', '1.0.0-beta+exp.sha.5114f85', '1.0.0+21AF26D3----117B344092BD')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int];
                $Actual | Should -Be 0;
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.1'; RVersion = '1.0.0-alpha'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha.beta'; RVersion = '1.0.0-alpha.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta'; RVersion = '1.0.0-alpha.beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.2'; RVersion = '1.0.0-beta'; },
                [PSCustomObject]@{ LVersion = '1.0.0-beta.11'; RVersion = '1.0.0-beta.2'; },
                [PSCustomObject]@{ LVersion = '1.0.0-rc.1'; RVersion = '1.0.0-beta.11'; },
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '1.0.0-rc.1'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement -SemVer;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
}

Describe '-NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated' {
    Context 'Default ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'Ordinal ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType Ordinal -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
    
    Context 'CurrentCultureIgnoreCase ComparisonType' {
        It "null should be less than empty" {
            $Actual = Compare-VersionStrings -LVersion $null -RVersion '' -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeLessThan 0;
            $Actual = Compare-VersionStrings -LVersion '' -RVersion $null -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
            $Actual | Should -BeOfType [int];
            $Actual | Should -BeGreaterThan 0;
        }

        It "Identical strings should return zero" {
            foreach ($VersionString in @('', '0', '1', '1.2', '12.3', '1.23', '2.3.4', '3.4.5.6', '4.56.78.9')) {
                $Actual = Compare-VersionStrings -LVersion $VersionString -RVersion $VersionString -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because ($VersionString | ConvertTo-Json);
                $Actual | Should -Be 0 -Because ($VersionString | ConvertTo-Json);
            }
        }
        
        It "LVersion should be less than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '1.0.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '2.1.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.1.1'; },
                [PSCustomObject]@{ LVersion = '1.0.0-alpha'; RVersion = '1.0.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeLessThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
        
        It "LVersion should be greater than RVersion" {
            foreach ($TestData in @(
                [PSCustomObject]@{ LVersion = '2.0.0'; RVersion = '1.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.0'; RVersion = '2.0.0'; },
                [PSCustomObject]@{ LVersion = '2.1.1'; RVersion = '2.1.0'; }
            )) {
                $Actual = Compare-VersionStrings -LVersion $_.LVersion -RVersion $_.RVersion -ComparisonType CurrentCultureIgnoreCase -NullNotSameAsEmpty -DontAssumeZeroElement -DotSeparated;
                $Actual | Should -BeOfType [int] -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
                $Actual | Should -BeGreaterThan 0 -Because "$($_.LVersion | ConvertTo-Json):$($_.RVersion | ConvertTo-Json)";
            }
        }
    }
}
