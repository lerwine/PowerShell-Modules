Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.IOUtility.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'Testing Optimize-DateTime Function' {
    It 'Test isn''t being conducted in same time zone as UTC' {
        $DateTime = [DateTime]::Now;
        [DateTime]::SpecifyKind($DateTime, [DateTimeKind]::Utc).ToLocalTime() | Should -Not -Be $DateTime;
    }

    Context 'Utc' {
        It 'Local Time is converted to Universal time' {
            $InputValue = [DateTime]::new(2022, 8, 22, 14, 39, 19, 682, 485, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -Utc;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $Expected.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Utc, but all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2023, 2, 20, 6, 44, 0, 422, 950, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -Utc;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Universal time value is unchanged' {
            $InputValue = [DateTime]::new(2024, 1, 23, 23, 29, 32, 331, 981, [DateTimeKind]::Utc);
            $Actual = $InputValue | Optimize-DateTime -Utc;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }
    }

    Context 'Utc, TruncateTo' {
        It 'Local Time is converted to Universal time; Hour, minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2021, 5, 2, 14, 18, 44, 372, 88, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Days;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Local Time is converted to Universal time; Minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2022, 8, 12, 6, 46, 13, 512, 598, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Hours;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Local Time is converted to Universal time; Second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2023, 3, 14, 6, 23, 15, 497, 973, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Minutes;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Local Time is converted to Universal time; Millisecond and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2022, 12, 31, 7, 8, 7, 877, 689, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Seconds;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Local Time is converted to Universal time; Microsecond property is set to zero' {
            $InputValue = [DateTime]::new(2021, 12, 21, 14, 49, 52, 606, 728, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Milliseconds;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'No time conversion: Unspecifed Kind property is changed to Utc; Hour, minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2024, 10, 21, 10, 38, 23, 568, 763, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Days;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'No time conversion: Unspecifed Kind property is changed to Utc; Minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2023, 5, 26, 16, 9, 28, 520, 630, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Hours;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'No time conversion: Unspecifed Kind property is changed to Utc; Second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2023, 6, 21, 18, 29, 44, 575, 468, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Minutes;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'No time conversion: Unspecifed Kind property is changed to Utc; Millisecond and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2021, 3, 30, 0, 18, 43, 564, 908, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Seconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'No time conversion: Unspecifed Kind property is changed to Utc; Microsecond property is set to zero' {
            $InputValue = [DateTime]::new(2021, 9, 2, 2, 51, 19, 128, 795, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Milliseconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'No time conversion: Hour, minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2019, 11, 24, 19, 58, 13, 22, 836, [DateTimeKind]::Utc);
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Days;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'No time conversion: Minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2023, 10, 11, 8, 15, 2, 195, 49, [DateTimeKind]::Utc);
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Hours;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'No time conversion: Second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2020, 6, 17, 12, 20, 20, 645, 957, [DateTimeKind]::Utc);
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Minutes;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'No time conversion: Millisecond and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2024, 2, 7, 18, 56, 11, 309, 583, [DateTimeKind]::Utc);
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Seconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'No time conversion: Microsecond property is set to zero' {
            $InputValue = [DateTime]::new(2024, 2, 17, 17, 57, 17, 292, 344, [DateTimeKind]::Utc);
            $Actual = $InputValue | Optimize-DateTime -Utc -TruncateTo Milliseconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }
    }

    Context 'Utc, AssumeKind' {
        It 'Local Time is converted to Universal time' {
            $InputValue = [DateTime]::new(2021, 8, 6, 5, 21, 23, 536, 471, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be $Expected.Microsecond -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2020, 11, 27, 2, 19, 14, 971, 582, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be $Expected.Microsecond -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Unspecifed Kind property is changed to Utc, but all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2023, 3, 6, 1, 12, 48, 713, 391, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -Utc;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Local and converted to Universal time' {
            $InputValue = [DateTime]::new(2023, 3, 6, 1, 12, 48, 713, 391, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Local).ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -Utc;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $Expected.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Universal time value is unchanged' {
            $InputValue = [DateTime]::new(2020, 12, 19, 10, 41, 7, 683, 698, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2020, 6, 2, 22, 13, 52, 714, 154, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }
    }

    Context 'Utc, TruncateTo, AssumeKind' {
        It 'Local Time is converted to Universal time; Hour, minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2023, 7, 21, 9, 6, 8, 458, 547, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2023, 8, 14, 8, 29, 14, 431, 499, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Local Time is converted to Universal time; Minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2022, 3, 6, 20, 43, 35, 76, 43, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2020, 7, 12, 12, 59, 40, 440, 906, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Local Time is converted to Universal time; Second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2021, 5, 1, 7, 40, 34, 937, 928, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2024, 3, 3, 14, 7, 18, 205, 119, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Local Time is converted to Universal time; Millisecond and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2021, 2, 9, 21, 30, 23, 298, 793, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2020, 10, 22, 11, 54, 27, 543, 727, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Local Time is converted to Universal time; Microsecond property is set to zero' {
            $InputValue = [DateTime]::new(2022, 3, 26, 17, 29, 13, 772, 647, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2022, 6, 28, 11, 30, 38, 945, 132, [DateTimeKind]::Local);
            $Expected = $InputValue.ToUniversalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Unspecifed Kind property is changed to Utc; Hour, minute, second, millisecond, and microsecond properties are set to zero; all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2019, 12, 17, 23, 55, 36, 478, 956, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Days -Utc;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Utc; Minute, second, millisecond, and microsecond properties are set to zero; all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2022, 12, 1, 5, 45, 28, 345, 304, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Hours -Utc;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Utc; Second, millisecond, and microsecond properties are set to zero; all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2024, 4, 19, 3, 2, 16, 449, 496, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Minutes -Utc;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Utc; Millisecond and microsecond properties are set to zero; all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2022, 9, 5, 16, 10, 36, 732, 422, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Seconds -Utc;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Utc; Microsecond property is set to zero; all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2024, 9, 1, 22, 55, 58, 699, 322, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Milliseconds -Utc;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Local and converted to Universal time; Hour, minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2020, 5, 5, 12, 39, 8, 506, 221, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Local).ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Days -Utc;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Local and converted to Universal time; Minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2023, 4, 18, 2, 35, 18, 908, 535, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Local).ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Hours -Utc;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Local and converted to Universal time; Second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2020, 2, 27, 6, 17, 48, 195, 575, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Local).ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Minutes -Utc;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Local and converted to Universal time; Millisecond and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2020, 9, 3, 6, 58, 21, 667, 654, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Local).ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Seconds -Utc;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Local and converted to Universal time; Microsecond property is set to zero' {
            $InputValue = [DateTime]::new(2024, 8, 30, 16, 23, 37, 179, 528, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Local).ToUniversalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Milliseconds -Utc;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Universal time value is unchanged; Hour, minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2022, 3, 21, 2, 8, 51, 322, 963, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2023, 7, 15, 5, 51, 31, 477, 436, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Days -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Universal time value is unchanged; Minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2023, 1, 28, 10, 41, 24, 22, 297, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Hours -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2021, 10, 24, 12, 50, 41, 308, 356, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Hours -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Universal time value is unchanged; Second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2020, 8, 9, 20, 7, 32, 113, 499, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Minutes -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2022, 12, 25, 19, 33, 48, 315, 626, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Minutes -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Universal time value is unchanged; Millisecond and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2022, 7, 4, 23, 11, 56, 210, 602, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Seconds -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2024, 9, 22, 22, 51, 2, 623, 405, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Seconds -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Universal time value is unchanged; Microsecond property is set to zero' {
            $InputValue = [DateTime]::new(2022, 7, 25, 12, 41, 25, 802, 222, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Milliseconds -Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2023, 9, 26, 0, 38, 56, 913, 32, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Milliseconds -Utc } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }
    }

    Context 'Local' {
        It 'Universal time is converted to Local time' {
            $InputValue = [DateTime]::new(2024, 7, 22, 2, 49, 4, 691, 434, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $Expected.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Utc and then converted to Local time' {
            $InputValue = [DateTime]::new(2023, 10, 19, 8, 52, 29, 595, 487, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $Expected.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Local time value is unchanged' {
            $InputValue = [DateTime]::new(2022, 10, 18, 18, 57, 7, 758, 814, [DateTimeKind]::Local);
            $Actual = $InputValue | Optimize-DateTime -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }
    }

    Context 'Local, TruncateTo' {
        It 'Universal time is converted to Local time; Hour, minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2024, 5, 30, 12, 25, 22, 766, 832, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Days -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Universal time is converted to Local time; Minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2024, 5, 30, 12, 25, 22, 766, 832, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Hours -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Universal time is converted to Local time; Second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2024, 5, 30, 12, 25, 22, 766, 832, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Minutes -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Universal time is converted to Local time; Millisecond and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2024, 5, 30, 12, 25, 22, 766, 832, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Seconds -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Universal time is converted to Local time; Microsecond property is set to zero' {
            $InputValue = [DateTime]::new(2024, 5, 30, 12, 25, 22, 766, 832, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Milliseconds -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Utc and then converted to Local time; Hour, minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2024, 3, 8, 8, 58, 41, 497, 324, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Days -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Utc and then converted to Local time; Minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2024, 4, 13, 15, 55, 9, 58, 727, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Hours -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Utc and then converted to Local time; Second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2020, 4, 17, 10, 58, 21, 360, 74, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Minutes -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Utc and then converted to Local time; Millisecond and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2022, 2, 2, 23, 23, 32, 52, 553, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Seconds -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Utc and then converted to Local time; Microsecond property is set to zero' {
            $InputValue = [DateTime]::new(2021, 3, 18, 20, 51, 12, 702, 212, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Milliseconds -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Hour, minute, second, millisecond, and microsecond properties of Local time value are set to zero; All other properties are unchanged' {
            $InputValue = [DateTime]::new(2022, 10, 18, 18, 57, 7, 758, 814, [DateTimeKind]::Local);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Days -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Minute, second, millisecond, and microsecond properties of Local time value are set to zero; All other properties are unchanged' {
            $InputValue = [DateTime]::new(2022, 10, 18, 18, 57, 7, 758, 814, [DateTimeKind]::Local);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Hours -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Second, millisecond, and microsecond properties of Local time value are set to zero; All other properties are unchanged' {
            $InputValue = [DateTime]::new(2022, 10, 18, 18, 57, 7, 758, 814, [DateTimeKind]::Local);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Minutes -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Millisecond and microsecond properties of Local time value are set to zero; All other properties are unchanged' {
            $InputValue = [DateTime]::new(2022, 10, 18, 18, 57, 7, 758, 814, [DateTimeKind]::Local);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Seconds -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Microsecond property of Local time value is set to zero; All other properties are unchanged' {
            $InputValue = [DateTime]::new(2022, 10, 18, 18, 57, 7, 758, 814, [DateTimeKind]::Local);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Milliseconds -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }
    }

    Context 'Local, AssumeKind' {
        It 'Universal Time is converted to Local time' {
            $InputValue = [DateTime]::new(2021, 7, 22, 0, 37, 29, 902, 729, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be $Expected.Microsecond -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2022, 11, 11, 6, 27, 32, 353, 343, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be $Expected.Microsecond -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }

        It 'Unspecifed Kind property is changed to Local Time, but all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2022, 9, 8, 23, 5, 9, 811, 635, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Universal Time and converted to Local time' {
            $InputValue = [DateTime]::new(2020, 9, 16, 5, 14, 14, 398, 131, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $Expected.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Local time value is unchanged' {
            $InputValue = [DateTime]::new(2024, 10, 16, 2, 38, 42, 604, 310, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2023, 9, 10, 22, 36, 35, 347, 860, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }
    }

    Context 'Local, TruncateTo, AssumeKind' {
        It 'Universal Time is converted to Local time; Hour, minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2019, 12, 19, 15, 10, 52, 875, 38, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Days -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2024, 8, 4, 20, 10, 50, 868, 278, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Days -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }

        It 'Universal Time is converted to Local time; Minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2022, 1, 22, 11, 6, 46, 850, 644, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Hours -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2022, 5, 14, 9, 48, 34, 726, 171, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Hours -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }

        It 'Universal Time is converted to Local time; Second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2020, 5, 27, 9, 35, 16, 170, 79, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Minutes -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2024, 10, 21, 11, 50, 19, 497, 547, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Minutes -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }

        It 'Universal Time is converted to Local time; Millisecond and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2024, 3, 5, 13, 7, 45, 165, 404, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Seconds -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2020, 7, 31, 14, 39, 57, 510, 71, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Seconds -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }

        It 'Universal Time is converted to Local time; Microsecond property is set to zero' {
            $InputValue = [DateTime]::new(2024, 10, 22, 14, 16, 21, 931, 729, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Milliseconds -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2021, 10, 9, 21, 2, 55, 224, 628, [DateTimeKind]::Utc);
            $Expected = $InputValue.ToLocalTime();
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Milliseconds -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $Expected.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }

        It 'Unspecifed Kind property is changed to Local Time; Hour, minute, second, millisecond, and microsecond properties are set to zero; all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2022, 1, 13, 1, 43, 21, 458, 496, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Days -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Local Time; Minute, second, millisecond, and microsecond properties are set to zero; all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2021, 6, 7, 9, 59, 9, 559, 328, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Hours -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Local Time; Second, millisecond, and microsecond properties are set to zero; all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2021, 9, 29, 13, 55, 8, 91, 410, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Minutes -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Local Time; Millisecond and microsecond properties are set to zero; all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2023, 2, 20, 12, 46, 4, 25, 584, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Seconds -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Local Time; Microsecond property is set to zero; all other Date/Time values remain the same' {
            $InputValue = [DateTime]::new(2022, 2, 19, 16, 51, 38, 113, 625, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Milliseconds -Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Universal Time and converted to Local time; Hour, minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2023, 9, 14, 15, 48, 2, 222, 234, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Days -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Universal Time and converted to Local time; Minute, second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2021, 4, 25, 2, 50, 15, 422, 615, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Hours -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Universal Time and converted to Local time; Second, millisecond, and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2021, 5, 13, 2, 38, 55, 458, 663, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Minutes -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Universal Time and converted to Local time; Millisecond and microsecond properties are set to zero' {
            $InputValue = [DateTime]::new(2020, 2, 4, 1, 17, 25, 887, 496, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Seconds -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecifed Kind property is changed to Universal Time and converted to Local time; Microsecond property is set to zero' {
            $InputValue = [DateTime]::new(2021, 12, 28, 18, 17, 18, 838, 54, [DateTimeKind]::Unspecified);
            $Expected = [DateTime]::SpecifyKind($InputValue, [DateTimeKind]::Utc).ToLocalTime();
            $Actual = $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Milliseconds -Local;
            $Actual.Year | Should -Be $Expected.Year -Because 'Year';
            $Actual.Month | Should -Be $Expected.Month -Because 'Month';
            $Actual.Day | Should -Be $Expected.Day -Because 'Day';
            $Actual.Hour | Should -Be $Expected.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $Expected.Minute -Because 'Minute';
            $Actual.Second | Should -Be $Expected.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $Expected.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Hour, minute, second, millisecond, and microsecond properties of Local time value are set to zero; All other properties are unchanged' {
            $InputValue = [DateTime]::new(2023, 3, 6, 11, 58, 28, 406, 371, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Days -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2020, 8, 27, 13, 37, 41, 280, 560, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Days -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }

        It 'Minute, second, millisecond, and microsecond properties of Local time value are set to zero; All other properties are unchanged' {
            $InputValue = [DateTime]::new(2020, 7, 24, 22, 33, 51, 696, 279, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Hours -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2022, 2, 22, 16, 11, 59, 192, 605, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Hours -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }

        It 'Second, millisecond, and microsecond properties of Local time value are set to zero; All other properties are unchanged' {
            $InputValue = [DateTime]::new(2021, 5, 14, 11, 40, 46, 534, 28, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Minutes -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2021, 2, 19, 0, 35, 22, 163, 979, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Minutes -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }

        It 'Millisecond and microsecond properties of Local time value are set to zero; All other properties are unchanged' {
            $InputValue = [DateTime]::new(2023, 5, 12, 21, 5, 23, 571, 883, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Seconds -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2020, 5, 3, 8, 48, 56, 441, 149, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Seconds -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }

        It 'Microsecond property of Local time value is set to zero; All other properties are unchanged' {
            $InputValue = [DateTime]::new(2021, 11, 24, 3, 33, 8, 62, 218, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Utc -TruncateTo Milliseconds -Local } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2023, 10, 25, 19, 8, 47, 894, 690, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -AssumeKind Local -TruncateTo Milliseconds -Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';
        }
    }

    Context 'TruncateTo' {
        It 'Hour, minute, second, millisecond, and microsecond properties of Local time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2020, 9, 20, 6, 33, 43, 125, 618, [DateTimeKind]::Local);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Days;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Minute, second, millisecond, and microsecond properties of Local time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2022, 4, 30, 17, 24, 11, 676, 654, [DateTimeKind]::Local);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Hours;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Second, millisecond, and microsecond properties of Local time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2024, 1, 30, 14, 1, 59, 115, 650, [DateTimeKind]::Local);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Minutes;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Millisecond and microsecond properties of Local time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2020, 4, 1, 5, 8, 27, 76, 529, [DateTimeKind]::Local);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Seconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Microsecond property of Local time is set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2022, 4, 24, 9, 11, 48, 625, 541, [DateTimeKind]::Local);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Milliseconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Hour, minute, second, millisecond, and microsecond properties of Universal time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2023, 9, 29, 10, 27, 51, 551, 496, [DateTimeKind]::Utc);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Days;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Minute, second, millisecond, and microsecond properties of Universal time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2020, 3, 24, 18, 25, 33, 922, 526, [DateTimeKind]::Utc);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Hours;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Second, millisecond, and microsecond properties of Universal time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2024, 7, 5, 12, 6, 50, 35, 886, [DateTimeKind]::Utc);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Minutes;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Millisecond and microsecond properties of Universal time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2020, 10, 1, 19, 56, 52, 185, 377, [DateTimeKind]::Utc);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Seconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Microsecond property of Universal time is set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2023, 8, 29, 7, 21, 47, 508, 887, [DateTimeKind]::Utc);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Milliseconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Hour, minute, second, millisecond, and microsecond properties of Unspecified time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2020, 4, 14, 20, 55, 3, 762, 526, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Days;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Unspecified) -Because 'Kind';
        }

        It 'Minute, second, millisecond, and microsecond properties of Unspecified time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2022, 5, 10, 15, 40, 35, 446, 807, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Hours;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Unspecified) -Because 'Kind';
        }

        It 'Second, millisecond, and microsecond properties of Unspecified time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2023, 9, 20, 23, 46, 17, 782, 370, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Minutes;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Unspecified) -Because 'Kind';
        }

        It 'Millisecond and microsecond properties of Unspecified time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2021, 1, 5, 18, 57, 32, 437, 243, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Seconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Unspecified) -Because 'Kind';
        }

        It 'Millisecond and microsecond property of Unspecified time is set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2019, 11, 21, 10, 6, 58, 63, 947, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -TruncateTo Milliseconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Unspecified) -Because 'Kind';
        }
    }

    Context 'UnspecifiedTo' {
        It 'Local time is unchanged' {
            $InputValue = [DateTime]::new(2019, 12, 31, 4, 21, 26, 439, 537, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';

            $InputValue = [DateTime]::new(2022, 12, 4, 22, 37, 46, 792, 638, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';
        }

        It 'Universal time is unchanged' {
            $InputValue = [DateTime]::new(2022, 10, 10, 11, 43, 7, 729, 85, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2020, 12, 14, 16, 7, 13, 922, 355, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Unspecified time is changed to Utc; All other properties remain the same' {
            $InputValue = [DateTime]::new(2019, 12, 8, 7, 36, 16, 250, 532, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Utc;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecified time is changed to Local; All other properties remain the same' {
            $InputValue = [DateTime]::new(2022, 9, 28, 8, 42, 43, 235, 514, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Local;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be $InputValue.Microsecond -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }
    }

    Context 'TruncateTo, UnspecifiedTo' {
        It 'Hour, minute, second, millisecond, and microsecond properties of Local time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2023, 9, 27, 13, 15, 54, 798, 639, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Days } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';

            $InputValue = [DateTime]::new(2020, 11, 1, 11, 13, 29, 869, 647, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Days } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';
        }

        It 'Minute, second, millisecond, and microsecond properties of Local time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2024, 2, 3, 14, 29, 15, 461, 545, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Hours } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';

            $InputValue = [DateTime]::new(2021, 6, 11, 18, 2, 32, 599, 310, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Hours } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';
        }

        It 'Second, millisecond, and microsecond properties of Local time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2021, 7, 17, 23, 29, 25, 19, 788, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Minutes } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';

            $InputValue = [DateTime]::new(2022, 9, 4, 16, 22, 29, 680, 23, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Minutes } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';
        }

        It 'Millisecond and microsecond properties of Local time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2019, 12, 30, 10, 30, 36, 167, 640, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Seconds } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';

            $InputValue = [DateTime]::new(2021, 3, 13, 23, 47, 27, 662, 299, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Seconds } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';
        }

        It 'Microsecond properties of Local time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2022, 8, 28, 16, 56, 10, 137, 664, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Milliseconds } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Local=>Kind';

            $InputValue = [DateTime]::new(2023, 9, 7, 3, 37, 33, 699, 744, [DateTimeKind]::Local);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Milliseconds } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Utc=>Kind';
        }

        It 'Hour, minute, second, millisecond, and microsecond properties of Universal time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2024, 2, 15, 5, 23, 3, 349, 335, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Days } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2022, 7, 3, 8, 33, 3, 331, 38, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Days } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be 0 -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Minute, second, millisecond, and microsecond properties of Universal time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2021, 5, 26, 20, 22, 27, 521, 621, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Hours } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2023, 3, 16, 7, 48, 36, 883, 593, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Hours } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be 0 -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Second, millisecond, and microsecond properties of Universal time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2024, 2, 25, 11, 41, 4, 122, 584, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Minutes } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2020, 12, 20, 3, 16, 41, 663, 42, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Minutes } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be 0 -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Millisecond and microsecond properties of Universal time are set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2021, 11, 2, 20, 44, 48, 835, 36, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Seconds } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2024, 8, 9, 1, 59, 5, 823, 865, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Seconds } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Microsecond property of Universal time is set to zero; all other properties remain the same' {
            $InputValue = [DateTime]::new(2021, 1, 1, 5, 31, 10, 842, 185, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Milliseconds } | Should -Not -Throw -Because 'Utc' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Utc=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Utc=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Utc=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Utc=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Utc=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Utc=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Utc=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Utc=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Utc=>Kind';

            $InputValue = [DateTime]::new(2019, 11, 24, 10, 13, 43, 178, 483, [DateTimeKind]::Utc);
            $Actual = { $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Milliseconds } | Should -Not -Throw -Because 'Local' -PassThru;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Local=>Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Local=>Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Local=>Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Local=>Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Local=>Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Local=>Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Local=>Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Local=>Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Local=>Kind';
        }

        It 'Unspecified time is changed to Utc; Hour, minute, second, millisecond, and microsecond properties are set to zero; All other properties remain the same' {
            $InputValue = [DateTime]::new(2019, 11, 24, 10, 13, 43, 178, 483, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Days;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecified time is changed to Utc; Minute, second, millisecond, and microsecond properties are set to zero; All other properties remain the same' {
            $InputValue = [DateTime]::new(2021, 1, 1, 5, 31, 10, 842, 185, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Hours;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecified time is changed to Utc; Second, millisecond, and microsecond properties are set to zero; All other properties remain the same' {
            $InputValue = [DateTime]::new(2022, 2, 18, 21, 26, 54, 357, 521, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Minutes;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecified time is changed to Utc; Millisecond and microsecond properties are set to zero; All other properties remain the same' {
            $InputValue = [DateTime]::new(2021, 10, 11, 11, 14, 37, 761, 271, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Seconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecified time is changed to Utc; Microsecond property is set to zero; All other properties remain the same' {
            $InputValue = [DateTime]::new(2020, 12, 9, 1, 14, 32, 256, 879, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Utc -TruncateTo Milliseconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Utc) -Because 'Kind';
        }

        It 'Unspecified time is changed to Local; Hour, minute, second, millisecond, and microsecond properties are set to zero; All other properties remain the same' {
            $InputValue = [DateTime]::new(2023, 7, 26, 5, 48, 39, 529, 407, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Days;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be 0 -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecified time is changed to Local; Minute, second, millisecond, and microsecond properties are set to zero; All other properties remain the same' {
            $InputValue = [DateTime]::new(2019, 12, 25, 12, 16, 5, 976, 221, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Hours;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be 0 -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecified time is changed to Local; Second, millisecond, and microsecond properties are set to zero; All other properties remain the same' {
            $InputValue = [DateTime]::new(2023, 3, 28, 13, 27, 58, 230, 629, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Minutes;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be 0 -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecified time is changed to Local; Millisecond and microsecond properties are set to zero; All other properties remain the same' {
            $InputValue = [DateTime]::new(2021, 4, 6, 7, 44, 7, 156, 923, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Seconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be 0 -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }

        It 'Unspecified time is changed to Local; Microsecond property is set to zero; All other properties remain the same' {
            $InputValue = [DateTime]::new(2020, 3, 13, 3, 19, 6, 979, 540, [DateTimeKind]::Unspecified);
            $Actual = $InputValue | Optimize-DateTime -UnspecifiedTo Local -TruncateTo Milliseconds;
            $Actual.Year | Should -Be $InputValue.Year -Because 'Year';
            $Actual.Month | Should -Be $InputValue.Month -Because 'Month';
            $Actual.Day | Should -Be $InputValue.Day -Because 'Day';
            $Actual.Hour | Should -Be $InputValue.Hour -Because 'Hour';
            $Actual.Minute | Should -Be $InputValue.Minute -Because 'Minute';
            $Actual.Second | Should -Be $InputValue.Second -Because 'Second';
            $Actual.Millisecond | Should -Be $InputValue.Millisecond -Because 'Millisecond';
            $Actual.Microsecond | Should -Be 0 -Because 'Microsecond';
            $Actual.Kind | Should -Be ([DateTimeKind]::Local) -Because 'Kind';
        }
    }
}
