Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath '../IOUtility/Erwine.Leonard.T.IOUtility.psd1') -ErrorAction Stop;
Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.SwPackage.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'ConvertTo-TargetVsixPlatform' {
    Context 'No Switch' {
        It "'' should return UNIVERSAL" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString '' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform '' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform '' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'Platform';
            $Actual = { '' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because '(pipeline)';
        }

        It "'win32-x64' should return WIN32_X64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'win32-x64' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_X64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'win32-x64' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_X64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'win32-x64' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_X64) -Because 'Platform';
            $Actual = { 'win32-x64' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_X64) -Because '(pipeline)';
        }

        It "'win32-arm64' should return WIN32_ARM64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'win32-arm64' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_ARM64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'win32-arm64' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_ARM64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'win32-arm64' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_ARM64) -Because 'Platform';
            $Actual = { 'win32-arm64' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_ARM64) -Because '(pipeline)';
        }

        It "'linux-x64' should return LINUX_X64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'linux-x64' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_X64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'linux-x64' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_X64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'linux-x64' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_X64) -Because 'Platform';
            $Actual = { 'linux-x64' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_X64) -Because '(pipeline)';
        }

        It "'linux-arm64' should return LINUX_ARM64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'linux-arm64' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARM64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'linux-arm64' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARM64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'linux-arm64' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARM64) -Because 'Platform';
            $Actual = { 'linux-arm64' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARM64) -Because '(pipeline)';
        }

        It "'linux-armhf' should return LINUX_ARMHF" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'linux-armhf' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARMHF) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'linux-armhf' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARMHF) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'linux-armhf' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARMHF) -Because 'Platform';
            $Actual = { 'linux-armhf' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARMHF) -Because '(pipeline)';
        }

        It "'alpine-x64' should return ALPINE_X64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'alpine-x64' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_X64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'alpine-x64' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_X64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'alpine-x64' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_X64) -Because 'Platform';
            $Actual = { 'alpine-x64' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_X64) -Because '(pipeline)';
        }

        It "'alpine-arm64' should return ALPINE_ARM64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'alpine-arm64' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_ARM64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'alpine-arm64' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_ARM64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'alpine-arm64' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_ARM64) -Because 'Platform';
            $Actual = { 'alpine-arm64' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_ARM64) -Because '(pipeline)';
        }

        It "'darwin-x64' should return DARWIN_X64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'darwin-x64' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_X64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'darwin-x64' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_X64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'darwin-x64' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_X64) -Because 'Platform';
            $Actual = { 'darwin-x64' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_X64) -Because '(pipeline)';
        }

        It "'darwin-arm64' should return DARWIN_ARM64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'darwin-arm64' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_ARM64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'darwin-arm64' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_ARM64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'darwin-arm64' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_ARM64) -Because 'Platform';
            $Actual = { 'darwin-arm64' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_ARM64) -Because '(pipeline)';
        }

        It "'web' should return WEB" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'web' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WEB) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'web' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WEB) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'web' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WEB) -Because 'Platform';
            $Actual = { 'web' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WEB) -Because '(pipeline)';
        }

        It "'universal' should return UNIVERSAL" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'universal' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'universal' } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'universal' } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'Platform';
            $Actual = { 'universal' | ConvertTo-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because '(pipeline)';
        }

	    It "'unknown' should throw error" {
            $ErrorRecord = { ConvertTo-TargetVsixPlatform -InputString 'unknown' -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Unknown platform type: "unknown"' -Because 'InputString' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'InputString=>Category';
            $ErrorRecord.TargetObject | Should -BeExactly 'unknown' -Because 'InputString=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputString' -Because 'InputString=>CategoryTargetName';

            $ErrorRecord = { ConvertTo-TargetVsixPlatform -TargetPlatform 'unknown' -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Unknown platform type: "unknown"' -Because 'TargetPlatform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'TargetPlatform=>Category';
            $ErrorRecord.TargetObject | Should -BeExactly 'unknown' -Because 'TargetPlatform=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputString' -Because 'TargetPlatform=>CategoryTargetName';

            $ErrorRecord = { ConvertTo-TargetVsixPlatform -Platform 'unknown' -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Unknown platform type: "unknown"' -Because 'Platform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'Platform=>Category';
            $ErrorRecord.TargetObject | Should -BeExactly 'unknown' -Because 'Platform=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputString' -Because 'Platform=>CategoryTargetName';

            $ErrorRecord = { 'unknown' | ConvertTo-TargetVsixPlatform -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Unknown platform type: "unknown"' -Because '(pipeline)' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because '(pipeline)=>Category';
            $ErrorRecord.TargetObject | Should -BeExactly 'unknown' -Because '(pipeline)=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputString' -Because '(pipeline)=>CategoryTargetName';
        }

	    It "'undefined' should throw error" {
            $ErrorRecord = { ConvertTo-TargetVsixPlatform -InputString 'undefined' -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Unknown platform type: "undefined"' -Because 'InputString' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'InputString=>Category';
            $ErrorRecord.TargetObject | Should -BeExactly 'undefined' -Because 'InputString=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputString' -Because 'InputString=>CategoryTargetName';

            $ErrorRecord = { ConvertTo-TargetVsixPlatform -TargetPlatform 'undefined' -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Unknown platform type: "undefined"' -Because 'TargetPlatform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because TargetPlatform=>'Category';
            $ErrorRecord.TargetObject | Should -BeExactly 'undefined' -Because 'TTargetPlatform=>argetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputString' -Because 'TargetPlatform=>CategoryTargetName';

            $ErrorRecord = { ConvertTo-TargetVsixPlatform -Platform 'undefined' -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Unknown platform type: "undefined"' -Because 'Platform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'Platform=>Category';
            $ErrorRecord.TargetObject | Should -BeExactly 'undefined' -Because 'Platform=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputString' -Because 'Platform=>CategoryTargetName';

            $ErrorRecord = { 'undefined' | ConvertTo-TargetVsixPlatform -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Unknown platform type: "undefined"' -Because '(pipeline)' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because '(pipeline)=>Category';
            $ErrorRecord.TargetObject | Should -BeExactly 'undefined' -Because '(pipeline)=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputString' -Because '(pipeline)=>CategoryTargetName';
        }
    }

    Context '-Force' {
        It "'' should return UNIVERSAL" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString '' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform '' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform '' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'Platform';
            $Actual = { '' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because '(pipeline)';
        }

        It "'win32-x64' should return WIN32_X64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'win32-x64' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_X64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'win32-x64' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_X64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'win32-x64' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_X64) -Because 'Platform';
            $Actual = { 'win32-x64' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_X64) -Because '(pipeline)';
        }

        It "'win32-arm64' should return WIN32_ARM64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'win32-arm64' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_ARM64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'win32-arm64' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_ARM64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'win32-arm64' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_ARM64) -Because 'Platform';
            $Actual = { 'win32-arm64' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WIN32_ARM64) -Because '(pipeline)';
        }

        It "'linux-x64' should return LINUX_X64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'linux-x64' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_X64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'linux-x64' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_X64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'linux-x64' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_X64) -Because 'Platform';
            $Actual = { 'linux-x64' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_X64) -Because '(pipeline)';
        }

        It "'linux-arm64' should return LINUX_ARM64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'linux-arm64' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARM64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'linux-arm64' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARM64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'linux-arm64' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARM64) -Because 'Platform';
            $Actual = { 'linux-arm64' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARM64) -Because '(pipeline)';
        }

        It "'linux-armhf' should return LINUX_ARMHF" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'linux-armhf' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARMHF) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'linux-armhf' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARMHF) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'linux-armhf' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARMHF) -Because 'Platform';
            $Actual = { 'linux-armhf' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::LINUX_ARMHF) -Because '(pipeline)';
        }

        It "'alpine-x64' should return ALPINE_X64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'alpine-x64' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_X64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'alpine-x64' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_X64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'alpine-x64' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_X64) -Because 'Platform';
            $Actual = { 'alpine-x64' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_X64) -Because '(pipeline)';
        }

        It "'alpine-arm64' should return ALPINE_ARM64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'alpine-arm64' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_ARM64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'alpine-arm64' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_ARM64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'alpine-arm64' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_ARM64) -Because 'Platform';
            $Actual = { 'alpine-arm64' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::ALPINE_ARM64) -Because '(pipeline)';
        }

        It "'darwin-x64' should return DARWIN_X64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'darwin-x64' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_X64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'darwin-x64' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_X64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'darwin-x64' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_X64) -Because 'Platform';
            $Actual = { 'darwin-x64' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_X64) -Because '(pipeline)';
        }

        It "'darwin-arm64' should return DARWIN_ARM64" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'darwin-arm64' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_ARM64) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'darwin-arm64' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_ARM64) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'darwin-arm64' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_ARM64) -Because 'Platform';
            $Actual = { 'darwin-arm64' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::DARWIN_ARM64) -Because '(pipeline)';
        }

        It "'web' should return WEB" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'web' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WEB) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'web' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WEB) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'web' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WEB) -Because 'Platform';
            $Actual = { 'web' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::WEB) -Because '(pipeline)';
        }

        It "'universal' should return UNIVERSAL" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'universal' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'universal' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'universal' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because 'Platform';
            $Actual = { 'universal' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNIVERSAL) -Because '(pipeline)';
        }

	    It "'unknown' should return UNKNOWN" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'unknown' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNKNOWN) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'unknown' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNKNOWN) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'unknown' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNKNOWN) -Because 'Platform';
            $Actual = { 'unknown' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNKNOWN) -Because '(pipeline)';
        }

	    It "'undefined' should return UNKNOWN" {
            $Actual = { ConvertTo-TargetVsixPlatform -InputString 'undefined' -Force } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNKNOWN) -Because 'InputString';
            $Actual = { ConvertTo-TargetVsixPlatform -TargetPlatform 'undefined' -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNKNOWN) -Because 'TargetPlatform';
            $Actual = { ConvertTo-TargetVsixPlatform -Platform 'undefined' -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNKNOWN) -Because 'Platform';
            $Actual = { 'undefined' | ConvertTo-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly ([TargetVsixPlatform]::UNKNOWN) -Because '(pipeline)';
        }
    }
}

Describe 'ConvertFrom-TargetVsixPlatform' {
    Context 'No switches' {
        It "WIN32_X64 should return 'win32-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_X64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'win32-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_X64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'win32-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_X64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'win32-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_X64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'win32-x64' -Because '(pipeline)';
        }

        It "WIN32_ARM64 should return 'win32-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_ARM64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_ARM64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_ARM64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_ARM64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'win32-arm64' -Because '(pipeline)';
        }

        It "LINUX_X64 should return 'linux-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_X64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_X64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_X64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_X64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-x64' -Because '(pipeline)';
        }

        It "LINUX_ARM64 should return 'linux-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARM64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARM64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARM64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARM64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-arm64' -Because '(pipeline)';
        }

        It "LINUX_ARMHF should return 'linux-armhf'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARMHF) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARMHF) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARMHF) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARMHF | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-armhf' -Because '(pipeline)';
        }

        It "ALPINE_X64 should return 'alpine-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_X64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_X64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_X64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_X64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'alpine-x64' -Because '(pipeline)';
        }

        It "ALPINE_ARM64 should return 'alpine-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_ARM64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_ARM64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_ARM64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_ARM64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'alpine-arm64' -Because '(pipeline)';
        }

        It "DARWIN_X64 should return 'darwin-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_X64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_X64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_X64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_X64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'darwin-x64' -Because '(pipeline)';
        }

        It "DARWIN_ARM64 should return 'darwin-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_ARM64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_ARM64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_ARM64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_ARM64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'darwin-arm64' -Because '(pipeline)';
        }

        It "WEB should return 'web'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WEB) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'web' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WEB) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'web' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WEB) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'web' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WEB | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'web' -Because '(pipeline)';
        }

        It "UNIVERSAL should return 'universal'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNIVERSAL) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'universal' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNIVERSAL) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'universal' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNIVERSAL) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'universal' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNIVERSAL | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'universal' -Because '(pipeline)';
        }

	    It "UNKNOWN should throw error" {
            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNKNOWN) -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'InputValue' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'InputValue=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'InputValue=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'InputValue=>CategoryTargetName';

            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNKNOWN) -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'TargetPlatform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because TargetPlatform=>'Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'TTargetPlatform=>argetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'TargetPlatform=>CategoryTargetName';

            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNKNOWN) -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'Platform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'Platform=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'Platform=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'Platform=>CategoryTargetName';

            $ErrorRecord = { [TargetVsixPlatform]::UNKNOWN | ConvertFrom-TargetVsixPlatform -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because '(pipeline)' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because '(pipeline)=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because '(pipeline)=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because '(pipeline)=>CategoryTargetName';
        }
    }

    Context '-Force' {
        It "WIN32_X64 should return 'win32-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_X64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'win32-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_X64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'win32-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_X64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'win32-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_X64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'win32-x64' -Because '(pipeline)';
        }

        It "WIN32_ARM64 should return 'win32-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_ARM64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_ARM64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_ARM64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_ARM64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'win32-arm64' -Because '(pipeline)';
        }

        It "LINUX_X64 should return 'linux-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_X64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_X64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_X64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_X64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-x64' -Because '(pipeline)';
        }

        It "LINUX_ARM64 should return 'linux-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARM64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARM64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARM64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARM64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-arm64' -Because '(pipeline)';
        }

        It "LINUX_ARMHF should return 'linux-armhf'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARMHF) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARMHF) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARMHF) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARMHF | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-armhf' -Because '(pipeline)';
        }

        It "ALPINE_X64 should return 'alpine-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_X64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_X64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_X64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_X64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'alpine-x64' -Because '(pipeline)';
        }

        It "ALPINE_ARM64 should return 'alpine-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_ARM64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_ARM64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_ARM64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_ARM64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'alpine-arm64' -Because '(pipeline)';
        }

        It "DARWIN_X64 should return 'darwin-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_X64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_X64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_X64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_X64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'darwin-x64' -Because '(pipeline)';
        }

        It "DARWIN_ARM64 should return 'darwin-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_ARM64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_ARM64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_ARM64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_ARM64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'darwin-arm64' -Because '(pipeline)';
        }

        It "WEB should return 'web'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WEB) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'web' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WEB) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'web' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WEB) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'web' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WEB | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'web' -Because '(pipeline)';
        }

        It "UNIVERSAL should return 'universal'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNIVERSAL) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'universal' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNIVERSAL) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'universal' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNIVERSAL) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'universal' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNIVERSAL | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'universal' -Because '(pipeline)';
        }

        It "UNKNOWN should return 'unknown'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNKNOWN) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'unknown' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNKNOWN) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'unknown' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNKNOWN) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'unknown' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNKNOWN | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'unknown' -Because '(pipeline)';
        }
    }

    Context '-UniversalReturnsBlank' {
        It "WIN32_X64 should return 'win32-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'win32-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'win32-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'win32-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_X64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'win32-x64' -Because '(pipeline)';
        }

        It "WIN32_ARM64 should return 'win32-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_ARM64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'win32-arm64' -Because '(pipeline)';
        }

        It "LINUX_X64 should return 'linux-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_X64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-x64' -Because '(pipeline)';
        }

        It "LINUX_ARM64 should return 'linux-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARM64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-arm64' -Because '(pipeline)';
        }

        It "LINUX_ARMHF should return 'linux-armhf'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARMHF) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARMHF) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARMHF) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARMHF | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-armhf' -Because '(pipeline)';
        }

        It "ALPINE_X64 should return 'alpine-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_X64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'alpine-x64' -Because '(pipeline)';
        }

        It "ALPINE_ARM64 should return 'alpine-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_ARM64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'alpine-arm64' -Because '(pipeline)';
        }

        It "DARWIN_X64 should return 'darwin-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_X64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'darwin-x64' -Because '(pipeline)';
        }

        It "DARWIN_ARM64 should return 'darwin-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_ARM64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'darwin-arm64' -Because '(pipeline)';
        }

        It "WEB should return 'web'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WEB) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'web' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WEB) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'web' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WEB) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'web' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WEB | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'web' -Because '(pipeline)';
        }

        It "UNIVERSAL should return ''" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNIVERSAL) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly '' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNIVERSAL) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly '' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNIVERSAL) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly '' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNIVERSAL | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly '' -Because '(pipeline)';
        }

	    It "UNKNOWN should throw error" {
            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNKNOWN) -UniversalReturnsBlank -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'InputValue' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'InputValue=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'InputValue=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'InputValue=>CategoryTargetName';

            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNKNOWN) -UniversalReturnsBlank -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'TargetPlatform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because TargetPlatform=>'Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'TTargetPlatform=>argetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'TargetPlatform=>CategoryTargetName';

            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNKNOWN) -UniversalReturnsBlank -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'Platform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'Platform=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'Platform=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'Platform=>CategoryTargetName';

            $ErrorRecord = { [TargetVsixPlatform]::UNKNOWN | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because '(pipeline)' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because '(pipeline)=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because '(pipeline)=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because '(pipeline)=>CategoryTargetName';
        }
    }

    Context '-Force -UniversalReturnsBlank' {
        It "WIN32_X64 should return 'win32-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'win32-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'win32-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'win32-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_X64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'win32-x64' -Because '(pipeline)';
        }

        It "WIN32_ARM64 should return 'win32-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'win32-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_ARM64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'win32-arm64' -Because '(pipeline)';
        }

        It "LINUX_X64 should return 'linux-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_X64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-x64' -Because '(pipeline)';
        }

        It "LINUX_ARM64 should return 'linux-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARM64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-arm64' -Because '(pipeline)';
        }

        It "LINUX_ARMHF should return 'linux-armhf'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARMHF) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARMHF) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARMHF) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'linux-armhf' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARMHF | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'linux-armhf' -Because '(pipeline)';
        }

        It "ALPINE_X64 should return 'alpine-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'alpine-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_X64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'alpine-x64' -Because '(pipeline)';
        }

        It "ALPINE_ARM64 should return 'alpine-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'alpine-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_ARM64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'alpine-arm64' -Because '(pipeline)';
        }

        It "DARWIN_X64 should return 'darwin-x64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'darwin-x64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_X64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'darwin-x64' -Because '(pipeline)';
        }

        It "DARWIN_ARM64 should return 'darwin-arm64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'darwin-arm64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_ARM64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'darwin-arm64' -Because '(pipeline)';
        }

        It "WEB should return 'web'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WEB) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'web' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WEB) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'web' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WEB) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'web' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WEB | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'web' -Because '(pipeline)';
        }

        It "UNIVERSAL should return ''" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNIVERSAL) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly '' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNIVERSAL) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly '' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNIVERSAL) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly '' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNIVERSAL | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly '' -Because '(pipeline)';
        }

        It "UNKNOWN should return 'unknown'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNKNOWN) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'unknown' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNKNOWN) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'unknown' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNKNOWN) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'unknown' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNKNOWN | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'unknown' -Because '(pipeline)';
        }
    }

    Context '-AsDisplayName' {
        It "WIN32_X64 should return 'Windows 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_X64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_X64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_X64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_X64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because '(pipeline)';
        }

        It "WIN32_ARM64 should return 'Windows ARM'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_ARM64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_ARM64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_ARM64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_ARM64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Windows ARM' -Because '(pipeline)';
        }

        It "LINUX_X64 should return 'Linux 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_X64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_X64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_X64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_X64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because '(pipeline)';
        }

        It "LINUX_ARM64 should return 'Linux ARM 64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARM64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARM64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARM64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARM64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because '(pipeline)';
        }

        It "LINUX_ARMHF should return 'Linux ARM'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARMHF) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARMHF) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARMHF) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARMHF | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux ARM' -Because '(pipeline)';
        }

        It "ALPINE_X64 should return 'Alpine Linux 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_X64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_X64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_X64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_X64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because '(pipeline)';
        }

        It "ALPINE_ARM64 should return 'Alpine ARM 64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_ARM64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_ARM64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_ARM64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_ARM64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because '(pipeline)';
        }

        It "DARWIN_X64 should return 'Mac'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_X64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Mac' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_X64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Mac' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_X64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Mac' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_X64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Mac' -Because '(pipeline)';
        }

        It "DARWIN_ARM64 should return 'Mac Silicon'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_ARM64) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_ARM64) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_ARM64) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_ARM64 | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Mac Silicon' -Because '(pipeline)';
        }

        It "WEB should return 'Web'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WEB) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Web' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WEB) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Web' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WEB) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Web' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WEB | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Web' -Because '(pipeline)';
        }

        It "UNIVERSAL should return 'universal'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNIVERSAL) } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'universal' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNIVERSAL) } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'universal' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNIVERSAL) } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'universal' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNIVERSAL | ConvertFrom-TargetVsixPlatform } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'universal' -Because '(pipeline)';
        }

	    It "UNKNOWN should throw error" {
            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNKNOWN) -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'InputValue' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'InputValue=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'InputValue=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'InputValue=>CategoryTargetName';

            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNKNOWN) -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'TargetPlatform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because TargetPlatform=>'Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'TTargetPlatform=>argetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'TargetPlatform=>CategoryTargetName';

            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNKNOWN) -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'Platform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'Platform=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'Platform=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'Platform=>CategoryTargetName';

            $ErrorRecord = { [TargetVsixPlatform]::UNKNOWN | ConvertFrom-TargetVsixPlatform -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because '(pipeline)' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because '(pipeline)=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because '(pipeline)=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because '(pipeline)=>CategoryTargetName';
        }
    }

    Context '-Force -AsDisplayName' {
        It "WIN32_X64 should return 'Windows 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_X64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_X64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_X64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_X64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because '(pipeline)';
        }

        It "WIN32_ARM64 should return 'Windows ARM'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_ARM64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_ARM64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_ARM64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_ARM64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Windows ARM' -Because '(pipeline)';
        }

        It "LINUX_X64 should return 'Linux 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_X64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_X64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_X64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_X64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because '(pipeline)';
        }

        It "LINUX_ARM64 should return 'Linux ARM 64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARM64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARM64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARM64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARM64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because '(pipeline)';
        }

        It "LINUX_ARMHF should return 'Linux ARM'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARMHF) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARMHF) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARMHF) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARMHF | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux ARM' -Because '(pipeline)';
        }

        It "ALPINE_X64 should return 'Alpine Linux 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_X64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_X64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_X64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_X64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because '(pipeline)';
        }

        It "ALPINE_ARM64 should return 'Alpine ARM 64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_ARM64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_ARM64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_ARM64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_ARM64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because '(pipeline)';
        }

        It "DARWIN_X64 should return 'Mac'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_X64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Mac' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_X64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Mac' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_X64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Mac' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_X64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Mac' -Because '(pipeline)';
        }

        It "DARWIN_ARM64 should return 'Mac Silicon'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_ARM64) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_ARM64) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_ARM64) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_ARM64 | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Mac Silicon' -Because '(pipeline)';
        }

        It "WEB should return 'Web'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WEB) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Web' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WEB) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Web' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WEB) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Web' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WEB | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Web' -Because '(pipeline)';
        }

        It "UNIVERSAL should return 'universal'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNIVERSAL) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'universal' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNIVERSAL) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'universal' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNIVERSAL) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'universal' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNIVERSAL | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'universal' -Because '(pipeline)';
        }

        It "UNKNOWN should return 'unknown'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNKNOWN) -Force } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'unknown' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNKNOWN) -Force } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'unknown' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNKNOWN) -Force } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'unknown' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNKNOWN | ConvertFrom-TargetVsixPlatform -Force } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'unknown' -Because '(pipeline)';
        }
    }

    Context '-UniversalReturnsBlank -AsDisplayName' {
        It "WIN32_X64 should return 'Windows 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_X64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because '(pipeline)';
        }

        It "WIN32_ARM64 should return 'Windows ARM'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_ARM64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Windows ARM' -Because '(pipeline)';
        }

        It "LINUX_X64 should return 'Linux 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_X64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because '(pipeline)';
        }

        It "LINUX_ARM64 should return 'Linux ARM 64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARM64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because '(pipeline)';
        }

        It "LINUX_ARMHF should return 'Linux ARM'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARMHF) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARMHF) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARMHF) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARMHF | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux ARM' -Because '(pipeline)';
        }

        It "ALPINE_X64 should return 'Alpine Linux 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_X64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because '(pipeline)';
        }

        It "ALPINE_ARM64 should return 'Alpine ARM 64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_ARM64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because '(pipeline)';
        }

        It "DARWIN_X64 should return 'Mac'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Mac' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Mac' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_X64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Mac' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_X64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Mac' -Because '(pipeline)';
        }

        It "DARWIN_ARM64 should return 'Mac Silicon'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_ARM64) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_ARM64 | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Mac Silicon' -Because '(pipeline)';
        }

        It "WEB should return 'Web'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WEB) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Web' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WEB) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Web' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WEB) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Web' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WEB | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Web' -Because '(pipeline)';
        }

        It "UNIVERSAL should return ''" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNIVERSAL) -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly '' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNIVERSAL) -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly '' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNIVERSAL) -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly '' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNIVERSAL | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly '' -Because '(pipeline)';
        }

	    It "UNKNOWN should throw error" {
            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNKNOWN) -UniversalReturnsBlank -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'InputValue' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'InputValue=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'InputValue=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'InputValue=>CategoryTargetName';

            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNKNOWN) -UniversalReturnsBlank -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'TargetPlatform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because TargetPlatform=>'Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'TTargetPlatform=>argetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'TargetPlatform=>CategoryTargetName';

            $ErrorRecord = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNKNOWN) -UniversalReturnsBlank -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because 'Platform' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because 'Platform=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because 'Platform=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because 'Platform=>CategoryTargetName';

            $ErrorRecord = { [TargetVsixPlatform]::UNKNOWN | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank -ErrorAction Stop } | Should -Throw -ErrorId 'InvalidTargetPlatform' -ExpectedMessage 'Value does not represent a valid Target VSIX Platform' -Because '(pipeline)' -PassThru;
            $ErrorRecord.CategoryInfo.Category | Should -Be ([System.Management.Automation.ErrorCategory]::InvalidArgument) -Because '(pipeline)=>Category';
            $ErrorRecord.TargetObject | Should -Be ([TargetVsixPlatform]::UNKNOWN) -Because '(pipeline)=>TargetObject';
            $ErrorRecord.CategoryInfo.TargetName | Should -BeExactly 'InputValue' -Because '(pipeline)=>CategoryTargetName';
        }
    }

    Context '-Force -UniversalReturnsBlank -AsDisplayName' {
        It "WIN32_X64 should return 'Windows 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_X64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Windows 64 bit' -Because '(pipeline)';
        }

        It "WIN32_ARM64 should return 'Windows ARM'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WIN32_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WIN32_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WIN32_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Windows ARM' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WIN32_ARM64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Windows ARM' -Because '(pipeline)';
        }

        It "LINUX_X64 should return 'Linux 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_X64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux 64 bit' -Because '(pipeline)';
        }

        It "LINUX_ARM64 should return 'Linux ARM 64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARM64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux ARM 64' -Because '(pipeline)';
        }

        It "LINUX_ARMHF should return 'Linux ARM'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::LINUX_ARMHF) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::LINUX_ARMHF) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::LINUX_ARMHF) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Linux ARM' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::LINUX_ARMHF | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Linux ARM' -Because '(pipeline)';
        }

        It "ALPINE_X64 should return 'Alpine Linux 64 bit'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_X64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Alpine Linux 64 bit' -Because '(pipeline)';
        }

        It "ALPINE_ARM64 should return 'Alpine ARM 64'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::ALPINE_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::ALPINE_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::ALPINE_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::ALPINE_ARM64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Alpine ARM 64' -Because '(pipeline)';
        }

        It "DARWIN_X64 should return 'Mac'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Mac' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Mac' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_X64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Mac' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_X64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Mac' -Because '(pipeline)';
        }

        It "DARWIN_ARM64 should return 'Mac Silicon'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::DARWIN_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::DARWIN_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::DARWIN_ARM64) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Mac Silicon' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::DARWIN_ARM64 | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Mac Silicon' -Because '(pipeline)';
        }

        It "WEB should return 'Web'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::WEB) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'Web' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::WEB) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'Web' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::WEB) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'Web' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::WEB | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'Web' -Because '(pipeline)';
        }

        It "UNIVERSAL should return ''" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNIVERSAL) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly '' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNIVERSAL) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly '' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNIVERSAL) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly '' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNIVERSAL | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly '' -Because '(pipeline)';
        }

        It "UNKNOWN should return 'unknown'" {
            $Actual = { ConvertFrom-TargetVsixPlatform -InputValue ([TargetVsixPlatform]::UNKNOWN) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'InputValue';
            $Actual | Should -BeExactly 'unknown' -Because 'InputValue';
            $Actual = { ConvertFrom-TargetVsixPlatform -TargetPlatform ([TargetVsixPlatform]::UNKNOWN) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'TargetPlatform';
            $Actual | Should -BeExactly 'unknown' -Because 'TargetPlatform';
            $Actual = { ConvertFrom-TargetVsixPlatform -Platform ([TargetVsixPlatform]::UNKNOWN) -Force -UniversalReturnsBlank } | Should -Not -Throw -Because 'Platform';
            $Actual | Should -BeExactly 'unknown' -Because 'Platform';
            $Actual = { [TargetVsixPlatform]::UNKNOWN | ConvertFrom-TargetVsixPlatform -Force -UniversalReturnsBlank } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeExactly 'unknown' -Because '(pipeline)';
        }
    }
}

Describe 'Split-VsCodeExtensionBaseFileName' {
    Context 'No switches' {
        It "'redhat.vscode-xml-0.27.2024111208' should return { Publisher = 'redhat'; Name = 'vscode-xml'; Version = 0.27.2024111208; Platform = UNIVERSAL }" {
            $Actual = { Split-VsCodeExtensionBaseFileName -InputString 'redhat.vscode-xml-0.27.2024111208' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'InputString=>Type';
            $Actual.Publisher | Should -BeExactly 'redhat' -Because 'InputString=>Publisher';
            $Actual.Name | Should -BeExactly 'vscode-xml' -Because 'InputString=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'InputString=>Version';
            $Actual.Version.Major | Should -Be 0 -Because 'InputString=>Version.Major';
            $Actual.Version.Minor | Should -Be 27 -Because 'InputString=>Version.Minor';
            $Actual.Version.Patch | Should -Be 2024111208 -Because 'InputString=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'InputString=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'InputString=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'InputString=>Platform';

            $Actual = $null;
            $Actual = { Split-VsCodeExtensionBaseFileName -BaseName 'redhat.vscode-xml-0.27.2024111208' } | Should -Not -Throw -Because 'BaseName';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'BaseName=>BaseName=>Type';
            $Actual.Publisher | Should -BeExactly 'redhat' -Because 'BaseName=>Publisher';
            $Actual.Name | Should -BeExactly 'vscode-xml' -Because 'BaseName=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'BaseName=>Version';
            $Actual.Version.Major | Should -Be 0 -Because 'BaseName=>Version.Major';
            $Actual.Version.Minor | Should -Be 27 -Because 'BaseName=>Version.Minor';
            $Actual.Version.Patch | Should -Be 2024111208 -Because 'BaseName=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'BaseName=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'BaseName=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'BaseName=>Platform';

            $Actual = $null;
            $Actual = { 'redhat.vscode-xml-0.27.2024111208' | Split-VsCodeExtensionBaseFileName } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because '(pipeline)=>Type';
            $Actual.Publisher | Should -BeExactly 'redhat' -Because '(pipeline)=>Publisher';
            $Actual.Name | Should -BeExactly 'vscode-xml' -Because '(pipeline)=>Name';
            $Actual.Version | Should -Not -Be $null -Because '(pipeline)=>Version';
            $Actual.Version.Major | Should -Be 0 -Because '(pipeline)=>Version.Major';
            $Actual.Version.Minor | Should -Be 27 -Because '(pipeline)=>Version.Minor';
            $Actual.Version.Patch | Should -Be 2024111208 -Because '(pipeline)=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because '(pipeline)=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because '(pipeline)=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because '(pipeline)=>Platform';
        }

        It "'redhat.vscode-xml-0.27.2024111208@win32-x64' should return { Publisher = 'redhat'; Name = 'vscode-xml'; Version = 0.27.2024111208; Platform = WIN32_X64 }" {
            $Actual = { Split-VsCodeExtensionBaseFileName -InputString 'redhat.vscode-xml-0.27.2024111208@win32-x64' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'InputString=>Type';
            $Actual.Publisher | Should -BeExactly 'redhat' -Because 'InputString=>Publisher';
            $Actual.Name | Should -BeExactly 'vscode-xml' -Because 'InputString=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'InputString=>Version';
            $Actual.Version.Major | Should -Be 0 -Because 'InputString=>Version.Major';
            $Actual.Version.Minor | Should -Be 27 -Because 'InputString=>Version.Minor';
            $Actual.Version.Patch | Should -Be 2024111208 -Because 'InputString=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'InputString=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'InputString=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::WIN32_X64) -Because 'InputString=>Platform';

            $Actual = $null;
            $Actual = { Split-VsCodeExtensionBaseFileName -BaseName 'redhat.vscode-xml-0.27.2024111208@win32-x64' } | Should -Not -Throw -Because 'BaseName';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'BaseName=>Type';
            $Actual.Publisher | Should -BeExactly 'redhat' -Because 'BaseName=>Publisher';
            $Actual.Name | Should -BeExactly 'vscode-xml' -Because 'BaseName=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'BaseName=>Version';
            $Actual.Version.Major | Should -Be 0 -Because 'BaseName=>Version.Major';
            $Actual.Version.Minor | Should -Be 27 -Because 'BaseName=>Version.Minor';
            $Actual.Version.Patch | Should -Be 2024111208 -Because 'BaseName=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'BaseName=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'BaseName=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::WIN32_X64) -Because 'BaseName=>Platform';

            $Actual = $null;
            $Actual = { 'redhat.vscode-xml-0.27.2024111208@win32-x64' | Split-VsCodeExtensionBaseFileName } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because '(pipeline)=>Type';
            $Actual.Publisher | Should -BeExactly 'redhat' -Because '(pipeline)=>Publisher';
            $Actual.Name | Should -BeExactly 'vscode-xml' -Because '(pipeline)=>Name';
            $Actual.Version | Should -Not -Be $null -Because '(pipeline)=>Version';
            $Actual.Version.Major | Should -Be 0 -Because '(pipeline)=>Version.Major';
            $Actual.Version.Minor | Should -Be 27 -Because '(pipeline)=>Version.Minor';
            $Actual.Version.Patch | Should -Be 2024111208 -Because '(pipeline)=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because '(pipeline)=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because '(pipeline)=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::WIN32_X64) -Because '(pipeline)=>Platform';
        }

        It "'ritwickdey.LiveServer-5.7.9' should return { Publisher = 'ritwickdey'; Name = 'LiveServer'; Version = 5.7.9; Platform = UNIVERSAL }" {
            $Actual = { Split-VsCodeExtensionBaseFileName -InputString 'ritwickdey.LiveServer-5.7.9' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'InputString=>Type';
            $Actual.Publisher | Should -BeExactly 'ritwickdey' -Because 'InputString=>Publisher';
            $Actual.Name | Should -BeExactly 'LiveServer' -Because 'InputString=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'InputString=>Version';
            $Actual.Version.Major | Should -Be 5 -Because 'InputString=>Version.Major';
            $Actual.Version.Minor | Should -Be 7 -Because 'InputString=>Version.Minor';
            $Actual.Version.Patch | Should -Be 9 -Because 'InputString=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'InputString=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'InputString=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'InputString=>Platform';

            $Actual = $null;
            $Actual = { Split-VsCodeExtensionBaseFileName -BaseName 'ritwickdey.LiveServer-5.7.9' } | Should -Not -Throw -Because 'BaseName';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'BaseName=>Type';
            $Actual.Publisher | Should -BeExactly 'ritwickdey' -Because 'BaseName=>Publisher';
            $Actual.Name | Should -BeExactly 'LiveServer' -Because 'BaseName=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'BaseName=>Version';
            $Actual.Version.Major | Should -Be 5 -Because 'BaseName=>Version.Major';
            $Actual.Version.Minor | Should -Be 7 -Because 'BaseName=>Version.Minor';
            $Actual.Version.Patch | Should -Be 9 -Because 'BaseName=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'BaseName=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'BaseName=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'BaseName=>Platform';

            $Actual = $null;
            $Actual = { 'ritwickdey.LiveServer-5.7.9' | Split-VsCodeExtensionBaseFileName } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because '(pipeline)=>Type';
            $Actual.Publisher | Should -BeExactly 'ritwickdey' -Because '(pipeline)=>Publisher';
            $Actual.Name | Should -BeExactly 'LiveServer' -Because '(pipeline)=>Name';
            $Actual.Version | Should -Not -Be $null -Because '(pipeline)=>Version';
            $Actual.Version.Major | Should -Be 5 -Because '(pipeline)=>Version.Major';
            $Actual.Version.Minor | Should -Be 7 -Because '(pipeline)=>Version.Minor';
            $Actual.Version.Patch | Should -Be 9 -Because '(pipeline)=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because '(pipeline)=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because '(pipeline)=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because '(pipeline)=>Platform';
        }

        It "'ritwickdey.liveserver-5.7.9' should return { Publisher = 'ritwickdey'; Name = 'liveserver'; Version = 5.7.9; Platform = UNIVERSAL }" {
            $Actual = { Split-VsCodeExtensionBaseFileName -InputString 'ritwickdey.liveserver-5.7.9' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'InputString=>Type';
            $Actual.Publisher | Should -BeExactly 'ritwickdey' -Because 'InputString=>Publisher';
            $Actual.Name | Should -BeExactly 'liveserver' -Because 'InputString=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'InputString=>Version';
            $Actual.Version.Major | Should -Be 5 -Because 'InputString=>Version.Major';
            $Actual.Version.Minor | Should -Be 7 -Because 'InputString=>Version.Minor';
            $Actual.Version.Patch | Should -Be 9 -Because 'InputString=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'InputString=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'InputString=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'InputString=>Platform';

            $Actual = $null;
            $Actual = { Split-VsCodeExtensionBaseFileName -BaseName 'ritwickdey.liveserver-5.7.9' } | Should -Not -Throw -Because 'BaseName';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'BaseName=>Type';
            $Actual.Publisher | Should -BeExactly 'ritwickdey' -Because 'BaseName=>Publisher';
            $Actual.Name | Should -BeExactly 'liveserver' -Because 'BaseName=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'BaseName=>Version';
            $Actual.Version.Major | Should -Be 5 -Because 'BaseName=>Version.Major';
            $Actual.Version.Minor | Should -Be 7 -Because 'BaseName=>Version.Minor';
            $Actual.Version.Patch | Should -Be 9 -Because 'BaseName=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'BaseName=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'BaseName=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'BaseName=>Platform';

            $Actual = $null;
            $Actual = { 'ritwickdey.liveserver-5.7.9' | Split-VsCodeExtensionBaseFileName } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because '(pipeline)=>Type';
            $Actual.Publisher | Should -BeExactly 'ritwickdey' -Because '(pipeline)=>Publisher';
            $Actual.Name | Should -BeExactly 'liveserver' -Because '(pipeline)=>Name';
            $Actual.Version | Should -Not -Be $null -Because '(pipeline)=>Version';
            $Actual.Version.Major | Should -Be 5 -Because '(pipeline)=>Version.Major';
            $Actual.Version.Minor | Should -Be 7 -Because '(pipeline)=>Version.Minor';
            $Actual.Version.Patch | Should -Be 9 -Because '(pipeline)=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because '(pipeline)=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because '(pipeline)=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because '(pipeline)=>Platform';
        }

        It "'ms-dotnettools.csharp-2.56.31@linux-x64' should return { Publisher = 'ms-dotnettools'; Name = 'csharp'; Version = 2.56.31; Platform = LINUX_X64 }" {
            $Actual = { Split-VsCodeExtensionBaseFileName -InputString 'ms-dotnettools.csharp-2.56.31@linux-x64' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'InputString=>Type';
            $Actual.Publisher | Should -BeExactly 'ms-dotnettools' -Because 'InputString=>Publisher';
            $Actual.Name | Should -BeExactly 'csharp' -Because 'InputString=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'InputString=>Version';
            $Actual.Version.Major | Should -Be 2 -Because 'InputString=>Version.Major';
            $Actual.Version.Minor | Should -Be 56 -Because 'InputString=>Version.Minor';
            $Actual.Version.Patch | Should -Be 31 -Because 'InputString=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'InputString=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'InputString=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::LINUX_X64) -Because 'InputString=>Platform';

            $Actual = $null;
            $Actual = { Split-VsCodeExtensionBaseFileName -BaseName 'ms-dotnettools.csharp-2.56.31@linux-x64' } | Should -Not -Throw -Because 'BaseName';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'BaseName=>Type';
            $Actual.Publisher | Should -BeExactly 'ms-dotnettools' -Because 'BaseName=>Publisher';
            $Actual.Name | Should -BeExactly 'csharp' -Because 'BaseName=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'BaseName=>Version';
            $Actual.Version.Major | Should -Be 2 -Because 'BaseName=>Version.Major';
            $Actual.Version.Minor | Should -Be 56 -Because 'BaseName=>Version.Minor';
            $Actual.Version.Patch | Should -Be 31 -Because 'BaseName=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'BaseName=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'BaseName=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::LINUX_X64) -Because 'BaseName=>Platform';

            $Actual = $null;
            $Actual = { 'ms-dotnettools.csharp-2.56.31@linux-x64' | Split-VsCodeExtensionBaseFileName } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because '(pipeline)=>Type';
            $Actual.Publisher | Should -BeExactly 'ms-dotnettools' -Because '(pipeline)=>Publisher';
            $Actual.Name | Should -BeExactly 'csharp' -Because '(pipeline)=>Name';
            $Actual.Version | Should -Not -Be $null -Because '(pipeline)=>Version';
            $Actual.Version.Major | Should -Be 2 -Because '(pipeline)=>Version.Major';
            $Actual.Version.Minor | Should -Be 56 -Because '(pipeline)=>Version.Minor';
            $Actual.Version.Patch | Should -Be 31 -Because '(pipeline)=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because '(pipeline)=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because '(pipeline)=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::LINUX_X64) -Because '(pipeline)=>Platform';
        }

        It "'k--kato.docomment-1.0.0' should return { Publisher = 'k--kato'; Name = 'docomment'; Version = 1.0.0; Platform = UNIVERSAL }" {
            $Actual = { Split-VsCodeExtensionBaseFileName -InputString 'k--kato.docomment-1.0.0' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'InputString=>Type';
            $Actual.Publisher | Should -BeExactly 'k--kato' -Because 'InputString=>Publisher';
            $Actual.Name | Should -BeExactly 'docomment' -Because 'InputString=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'InputString=>Version';
            $Actual.Version.Major | Should -Be 1 -Because 'InputString=>Version.Major';
            $Actual.Version.Minor | Should -Be 0 -Because 'InputString=>Version.Minor';
            $Actual.Version.Patch | Should -Be 0 -Because 'InputString=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'InputString=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'InputString=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'InputString=>Platform';

            $Actual = $null;
            $Actual = { Split-VsCodeExtensionBaseFileName -BaseName 'k--kato.docomment-1.0.0' } | Should -Not -Throw -Because 'BaseName';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'BaseName=>Type';
            $Actual.Publisher | Should -BeExactly 'k--kato' -Because 'BaseName=>Publisher';
            $Actual.Name | Should -BeExactly 'docomment' -Because 'BaseName=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'BaseName=>Version';
            $Actual.Version.Major | Should -Be 1 -Because 'BaseName=>Version.Major';
            $Actual.Version.Minor | Should -Be 0 -Because 'BaseName=>Version.Minor';
            $Actual.Version.Patch | Should -Be 0 -Because 'BaseName=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'BaseName=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'BaseName=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'BaseName=>Platform';

            $Actual = $null;
            $Actual = { 'k--kato.docomment-1.0.0' | Split-VsCodeExtensionBaseFileName } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because '(pipeline)=>Type';
            $Actual.Publisher | Should -BeExactly 'k--kato' -Because '(pipeline)=>Publisher';
            $Actual.Name | Should -BeExactly 'docomment' -Because '(pipeline)=>Name';
            $Actual.Version | Should -Not -Be $null -Because '(pipeline)=>Version';
            $Actual.Version.Major | Should -Be 1 -Because '(pipeline)=>Version.Major';
            $Actual.Version.Minor | Should -Be 0 -Because '(pipeline)=>Version.Minor';
            $Actual.Version.Patch | Should -Be 0 -Because '(pipeline)=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because '(pipeline)=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because '(pipeline)=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because '(pipeline)=>Platform';
        }

        It "'jorgeserrano.vscode-csharp-snippets-1.1.0' should return { Publisher = 'jorgeserrano'; Name = 'vscode-csharp-snippets'; Version = 1.1.0; Platform = UNIVERSAL }" {
            $Actual = { Split-VsCodeExtensionBaseFileName -InputString 'jorgeserrano.vscode-csharp-snippets-1.1.0' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'InputString=>Type';
            $Actual.Publisher | Should -BeExactly 'jorgeserrano' -Because 'InputString=>Publisher';
            $Actual.Name | Should -BeExactly 'vscode-csharp-snippets' -Because 'InputString=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'InputString=>Version';
            $Actual.Version.Major | Should -Be 1 -Because 'InputString=>Version.Major';
            $Actual.Version.Minor | Should -Be 1 -Because 'InputString=>Version.Minor';
            $Actual.Version.Patch | Should -Be 0 -Because 'InputString=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'InputString=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'InputString=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'InputString=>Platform';

            $Actual = $null;
            $Actual = { Split-VsCodeExtensionBaseFileName -BaseName 'jorgeserrano.vscode-csharp-snippets-1.1.0' } | Should -Not -Throw -Because 'BaseName';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'BaseName=>Type';
            $Actual.Publisher | Should -BeExactly 'jorgeserrano' -Because 'BaseName=>Publisher';
            $Actual.Name | Should -BeExactly 'vscode-csharp-snippets' -Because 'BaseName=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'BaseName=>Version';
            $Actual.Version.Major | Should -Be 1 -Because 'BaseName=>Version.Major';;
            $Actual.Version.Minor | Should -Be 1 -Because 'BaseName=>Version.Minor';
            $Actual.Version.Patch | Should -Be 0 -Because 'BaseName=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'BaseName=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'BaseName=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'BaseName=>Platform';

            $Actual = $null;
            $Actual = { 'jorgeserrano.vscode-csharp-snippets-1.1.0' | Split-VsCodeExtensionBaseFileName } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because '(pipeline)=>Type';
            $Actual.Publisher | Should -BeExactly 'jorgeserrano' -Because '(pipeline)=>Publisher';
            $Actual.Name | Should -BeExactly 'vscode-csharp-snippets' -Because '(pipeline)=>Name';
            $Actual.Version | Should -Not -Be $null -Because '(pipeline)=>Version';
            $Actual.Version.Major | Should -Be 1 -Because '(pipeline)=>Version.Major';;
            $Actual.Version.Minor | Should -Be 1 -Because '(pipeline)=>Version.Minor';
            $Actual.Version.Patch | Should -Be 0 -Because '(pipeline)=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because '(pipeline)=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because '(pipeline)=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because '(pipeline)=>Platform';
        }

        It "'ms-vscode-remote.remote-wsl-0.88.5' should return { Publisher = 'ms-vscode-remote'; Name = 'remote-wsl'; Version = 0.88.5; Platform = UNIVERSAL }" {
            $Actual = { Split-VsCodeExtensionBaseFileName -InputString 'ms-vscode-remote.remote-wsl-0.88.5' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'InputString=>Type';
            $Actual.Publisher | Should -BeExactly 'ms-vscode-remote' -Because 'InputString=>Publisher';
            $Actual.Name | Should -BeExactly 'remote-wsl' -Because 'InputString=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'InputString=>Version';
            $Actual.Version.Major | Should -Be 0 -Because 'InputString=>Version.Major';
            $Actual.Version.Minor | Should -Be 88 -Because 'InputString=>Version.Minor';
            $Actual.Version.Patch | Should -Be 5 -Because 'InputString=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'InputString=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'InputString=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'InputString=>Platform';

            $Actual = $null;
            $Actual = { Split-VsCodeExtensionBaseFileName -BaseName 'ms-vscode-remote.remote-wsl-0.88.5' } | Should -Not -Throw -Because 'BaseName';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'BaseName=>Type';
            $Actual.Publisher | Should -BeExactly 'ms-vscode-remote' -Because 'BaseName=>Publisher';
            $Actual.Name | Should -BeExactly 'remote-wsl' -Because 'BaseName=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'BaseName=>Version';
            $Actual.Version.Major | Should -Be 0 -Because 'BaseName=>Version.Major';
            $Actual.Version.Minor | Should -Be 88 -Because 'BaseName=>Version.Minor';
            $Actual.Version.Patch | Should -Be 5 -Because 'BaseName=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'BaseName=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'BaseName=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'BaseName=>Platform';

            $Actual = $null;
            $Actual = { 'ms-vscode-remote.remote-wsl-0.88.5' | Split-VsCodeExtensionBaseFileName } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because '(pipeline)=>Type';
            $Actual.Publisher | Should -BeExactly 'ms-vscode-remote' -Because '(pipeline)=>Publisher';
            $Actual.Name | Should -BeExactly 'remote-wsl' -Because '(pipeline)=>Name';
            $Actual.Version | Should -Not -Be $null -Because '(pipeline)=>Version';
            $Actual.Version.Major | Should -Be 0 -Because '(pipeline)=>Version.Major';
            $Actual.Version.Minor | Should -Be 88 -Because '(pipeline)=>Version.Minor';
            $Actual.Version.Patch | Should -Be 5 -Because '(pipeline)=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because '(pipeline)=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because '(pipeline)=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because '(pipeline)=>Platform';
        }

        It "'EditorConfig.EditorConfig-0.16.4' should return { Publisher = 'EditorConfig'; Name = 'EditorConfig'; Version = 0.16.4; Platform = UNIVERSAL }" {
            $Actual = { Split-VsCodeExtensionBaseFileName -InputString 'EditorConfig.EditorConfig-0.16.4' } | Should -Not -Throw -Because 'InputString';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'InputString=>Type';
            $Actual.Publisher | Should -BeExactly 'EditorConfig' -Because 'InputString=>Publisher';
            $Actual.Name | Should -BeExactly 'EditorConfig' -Because 'InputString=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'InputString=>Version';
            $Actual.Version.Major | Should -Be 0 -Because 'InputString=>Version.Major';
            $Actual.Version.Minor | Should -Be 16 -Because 'InputString=>Version.Minor';
            $Actual.Version.Patch | Should -Be 4 -Because 'InputString=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'InputString=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'InputString=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'InputString=>Platform';

            $Actual = $null;
            $Actual = { Split-VsCodeExtensionBaseFileName -BaseName 'EditorConfig.EditorConfig-0.16.4' } | Should -Not -Throw -Because 'BaseName';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because 'BaseName=>Type';
            $Actual.Publisher | Should -BeExactly 'EditorConfig' -Because 'BaseName=>Publisher';
            $Actual.Name | Should -BeExactly 'EditorConfig' -Because 'BaseName=>Name';
            $Actual.Version | Should -Not -Be $null -Because 'BaseName=>Version';
            $Actual.Version.Major | Should -Be 0 -Because 'BaseName=>Version.Major';
            $Actual.Version.Minor | Should -Be 16 -Because 'BaseName=>Version.Minor';
            $Actual.Version.Patch | Should -Be 4 -Because 'BaseName=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because 'BaseName=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because 'BaseName=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because 'BaseName=>Platform';

            $Actual = $null;
            $Actual = { 'EditorConfig.EditorConfig-0.16.4' | Split-VsCodeExtensionBaseFileName } | Should -Not -Throw -Because '(pipeline)';
            $Actual | Should -BeOfType [TargetVsixPlatform] -Because '(pipeline)=>Type';
            $Actual.Publisher | Should -BeExactly 'EditorConfig' -Because '(pipeline)=>Publisher';
            $Actual.Name | Should -BeExactly 'EditorConfig' -Because '(pipeline)=>Name';
            $Actual.Version | Should -Not -Be $null -Because '(pipeline)=>Version';
            $Actual.Version.Major | Should -Be 0 -Because '(pipeline)=>Version.Major';
            $Actual.Version.Minor | Should -Be 16 -Because '(pipeline)=>Version.Minor';
            $Actual.Version.Patch | Should -Be 4 -Because '(pipeline)=>Version.Patch';
            $Actual.Version.PreReleaseLabel | Should -Be $null -Because '(pipeline)=>Version.PreReleaseLabel';
            $Actual.Version.BuildLabel | Should -Be $null -Because '(pipeline)=>Version.BuildLabel';
            $Actual.Platform | Should -Be ([TargetVsixPlatform]::UNIVERSAL) -Because '(pipeline)=>Platform';
        }
    }
}

Describe 'Get-VsExtensionsGalleryServiceUri' {
    Context 'No Optional Parameters' {
        It '"extensionquery" returns "https://marketplace.visualstudio.com/_apis/public/gallery/extensionquery"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'extensionquery' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'https://marketplace.visualstudio.com/_apis/public/gallery/extensionquery' -Because 'AbsoluteUri';
        }

        It '"publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage" returns "https://marketplace.visualstudio.com/_apis/public/gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'https://marketplace.visualstudio.com/_apis/public/gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage' -Because 'AbsoluteUri';
        }

        It '["publishers", "ritwickdey", "vsextensions", "LiveServer", "5.7.9", "vspackage"] returns "https://marketplace.visualstudio.com/_apis/public/gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers', 'ritwickdey', 'vsextensions', 'LiveServer', '5.7.9', 'vspackage' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'https://marketplace.visualstudio.com/_apis/public/gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage' -Because 'AbsoluteUri';
        }
    }

    Context 'Query' {
        It '(-RelativePath "publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage" -Query "targetPlatform=win32-x64") returns "https://marketplace.visualstudio.com/_apis/public/gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage' -Query 'targetPlatform=win32-x64' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'https://marketplace.visualstudio.com/_apis/public/gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64' -Because 'AbsoluteUri';
        }

        It '(-RelativePath "publishers", "redhat", "vsextensions", "vscode-xml", "0.27.2024111208", "vspackage" -Query "targetPlatform=win32-x64") returns "https://marketplace.visualstudio.com/_apis/public/gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers', 'redhat', 'vsextensions', 'vscode-xml', '0.27.2024111208', 'vspackage' -Query 'targetPlatform=win32-x64' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'https://marketplace.visualstudio.com/_apis/public/gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64' -Because 'AbsoluteUri';
        }
    }

    Context 'ServiceUri' {
        It '(-RelativePath "extensionquery" -ServiceUri "http://tempuri.org/vscode-gallery/") returns "http://tempuri.org/vscode-gallery/extensionquery"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'extensionquery' -ServiceUri 'http://tempuri.org/vscode-gallery/' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'http://tempuri.org/vscode-gallery/extensionquery' -Because 'AbsoluteUri';
        }

        It '(-RelativePath "extensionquery" -ServiceUri "http://tempuri.org/vscode-gallery") returns "http://tempuri.org/vscode-gallery/extensionquery"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'extensionquery' -ServiceUri 'http://tempuri.org/vscode-gallery' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'http://tempuri.org/vscode-gallery/extensionquery' -Because 'AbsoluteUri';
        }

        It '(-RelativePath "publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage" -ServiceUri "http://tempuri.org/vscode-gallery/") returns "http://tempuri.org/vscode-gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage' -ServiceUri 'http://tempuri.org/vscode-gallery/' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'http://tempuri.org/vscode-gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage' -Because 'AbsoluteUri';
        }

        It '(-RelativePath "publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage" -ServiceUri "http://tempuri.org/vscode-gallery") returns "http://tempuri.org/vscode-gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage' -ServiceUri 'http://tempuri.org/vscode-gallery' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'http://tempuri.org/vscode-gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage' -Because 'AbsoluteUri';
        }

        It '(-RelativePath ["publishers", "ritwickdey", "vsextensions", "LiveServer", "5.7.9", "vspackage"] -ServiceUri "http://tempuri.org/vscode-gallery/") returns "http://tempuri.org/vscode-gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers', 'ritwickdey', 'vsextensions', 'LiveServer', '5.7.9', 'vspackage' -ServiceUri 'http://tempuri.org/vscode-gallery/' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'http://tempuri.org/vscode-gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage' -Because 'AbsoluteUri';
        }

        It '(-RelativePath ["publishers", "ritwickdey", "vsextensions", "LiveServer", "5.7.9", "vspackage"] -ServiceUri "http://tempuri.org/vscode-gallery") returns "http://tempuri.org/vscode-gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers', 'ritwickdey', 'vsextensions', 'LiveServer', '5.7.9', 'vspackage' -ServiceUri 'http://tempuri.org/vscode-gallery' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'http://tempuri.org/vscode-gallery/publishers/ritwickdey/vsextensions/LiveServer/5.7.9/vspackage' -Because 'AbsoluteUri';
        }
    }

    Context 'Query and ServiceUri' {
        It '(-RelativePath "publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage" -Query "targetPlatform=win32-x64" -ServiceUri "http://tempuri.org/vscode-gallery/") returns "http://tempuri.org/vscode-gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage' -Query 'targetPlatform=win32-x64' -ServiceUri 'http://tempuri.org/vscode-gallery/' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'http://tempuri.org/vscode-gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64' -Because 'AbsoluteUri';
        }

        It '(-RelativePath "publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage" -Query "targetPlatform=win32-x64" -ServiceUri "http://tempuri.org/vscode-gallery") returns "http://tempuri.org/vscode-gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage' -Query 'targetPlatform=win32-x64' -ServiceUri 'http://tempuri.org/vscode-gallery' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'http://tempuri.org/vscode-gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64' -Because 'AbsoluteUri';
        }

        It '(-RelativePath "publishers", "redhat", "vsextensions", "vscode-xml", "0.27.2024111208", "vspackage" -Query "targetPlatform=win32-x64" -ServiceUri "http://tempuri.org/vscode-gallery/") returns "http://tempuri.org/vscode-gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers', 'redhat', 'vsextensions', 'vscode-xml', '0.27.2024111208', 'vspackage' -Query 'targetPlatform=win32-x64' -ServiceUri 'http://tempuri.org/vscode-gallery/' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'http://tempuri.org/vscode-gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64' -Because 'AbsoluteUri';
        }

        It '(-RelativePath "publishers", "redhat", "vsextensions", "vscode-xml", "0.27.2024111208", "vspackage" -Query "targetPlatform=win32-x64" -ServiceUri "http://tempuri.org/vscode-gallery") returns "http://tempuri.org/vscode-gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64"' {
            $Actual = { Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers', 'redhat', 'vsextensions', 'vscode-xml', '0.27.2024111208', 'vspackage' -Query 'targetPlatform=win32-x64' -ServiceUri 'http://tempuri.org/vscode-gallery' } | Should -Not -Throw;
            $Actual | Should -BeOfType ([Uri]) -Because 'Type';
            ([Uri]$Actual).IsAbsoluteUri | Should -BeTrue -Because 'IsAbsoluteUri';
            ([Uri]$Actual).AbsoluteUri | Should -BeExactly 'http://tempuri.org/vscode-gallery/publishers/redhat/vsextensions/vscode-xml/0.27.2024111208/vspackage?targetPlatform=win32-x64' -Because 'AbsoluteUri';
        }
    }
}