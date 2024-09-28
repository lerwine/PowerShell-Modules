Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.IOUtility.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

# Describe 'Testing ConvertTo-SafeFileName Function', {
# }

# Describe 'Testing Get-AppDataPath Function', {
# }

# Describe 'Testing Read-ShortIntegerFromStream Function', {
# }

# Describe 'Testing Read-UnsignedShortIntegerFromStream Function', {
# }

# Describe 'Testing Read-IntegerFromStream Function', {
# }

# Describe 'Testing Read-UnsignedIntegerFromStream Function', {
# }

# Describe 'Testing Read-LongIntegerFromStream Function', {
# }

# Describe 'Testing Read-UnsignedLongIntegerFromStream Function', {
# }

# Describe 'Testing Write-ShortIntegerToStream Function', {
# }

# Describe 'Testing Write-UnsignedShortIntegerToStream Function', {
# }

# Describe 'Testing Write-IntegerToStream Function', {
# }

# Describe 'Testing Write-UnsignedIntegerToStream Function', {
# }

# Describe 'Testing Write-LongIntegerToStream Function', {
# }

# Describe 'Testing Write-UnsignedLongIntegerToStream Function', {
# }

# Describe 'Testing Read-TinyLengthEncodedBytes Function', {
# }

# Describe 'Testing Read-ShortLengthEncodedBytes Function', {
# }

# Describe 'Testing Read-LengthEncodedBytes Function', {
# }

# Describe 'Testing Write-TinyLengthEncodedBytes Function', {
# }

# Describe 'Testing Write-ShortLengthEncodedBytes Function', {
# }

# Describe 'Testing Write-LengthEncodedBytes Function', {
# }

# Describe 'Testing Get-MinBase64BlockSize Function', {
# }

# Describe 'Testing ConvertTo-Base64String Function', {
# }

# Describe 'Testing ConvertFrom-Base64String Function', {
# }

# Describe 'Testing Get-TextEncoding Function', {
# }

# Describe 'Testing New-MemoryStream Function', {
# }

Describe 'Optimize-WhiteSpace Function' {
    Context 'Optimize-WhiteSpace -InputString <String>' {
        It "should return an unchanged value" {
            foreach ($InputString in @($null, '', '.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data')) {
                $Actual = Optimize-WhiteSpace -InputString $InputString;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize to an empty string" {
            foreach ($InputString in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                $Actual = Optimize-WhiteSpace -InputString $InputString;
                $Actual | Should -BeExactly '' -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize leading space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($TrailingText in @('.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data')) {
                    $InputString = "$LeadingSpace$TrailingText";
                    $Actual = Optimize-WhiteSpace -InputString $InputString;
                    $Actual | Should -BeExactly $TrailingText -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize trailing space" {
            foreach ($LeadingText in @('.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data')) {
                foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    $InputString = "$LeadingText$TrailingSpace";
                    $Actual = Optimize-WhiteSpace -InputString $InputString;
                    $Actual | Should -BeExactly $LeadingText -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize inner space" {
            foreach ($Word1 in @('?', 'Test')) {
                foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', 'Data')) {
                        $InputString = "$Word1$InnerSpace$Word2";
                        $Actual = Optimize-WhiteSpace -InputString $InputString;
                        $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize outer space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($InnerText in @('.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data')) {
                    foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        $InputString = "$LeadingSpace$InnerText$TrailingSpace";
                        $Actual = Optimize-WhiteSpace -InputString $InputString;
                        $Actual | Should -BeExactly $InnerText -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize leading and inner space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', 'Data')) {
                            $InputString = "$LeadingSpace$Word1$InnerSpace$Word2";
                            $Actual = Optimize-WhiteSpace -InputString $InputString;
                            $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize inner and trailing space" {
            foreach ($Word1 in @('?', 'Test')) {
                foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', 'Data')) {
                        foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                            $InputString = "$Word1$InnerSpace$Word2$TrailingSpace";
                            $Actual = Optimize-WhiteSpace -InputString $InputString;
                            $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize all space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', 'Data')) {
                            foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                                $InputString = "$LeadingSpace$Word1$InnerSpace$Word2$TrailingSpace";
                                $Actual = Optimize-WhiteSpace -InputString $InputString;
                                $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                            }
                        }
                    }
                }
            }
        }
    }

    Context 'Optimize-WhiteSpace -InputString <String> -DoNotTrim' {
        It "should return an unchanged value" {
            foreach ($InputString in @($null, '', ' ', '.', ' .', '. ', ' . ', 'Test', ' Test', 'Test ', ' Test ', '? !', ' ? !', '? ! ', ' ? ! ',
                    'Test !', ' Test !', 'Test ! ', ' Test ! ', '? Data', ' ? Data', '? Data ', ' ? Data ', 'Test !', ' Test !', 'Test ! ', ' Test ! ',
                    'Test Data', ' Test Data', 'Test Data ', ' Test Data ')) {
                $Actual = Optimize-WhiteSpace -InputString $InputString -DoNotTrim;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize to a single space" {
            foreach ($InputString in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                $Actual = Optimize-WhiteSpace -InputString $InputString -DoNotTrim;
                $Actual | Should -BeExactly ' ' -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize leading space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($TrailingText in @(".", ". ", "Test", "Test ", "? !", "? ! ", "Test !", "Test ! ", "? Data", "? Data ", "Test !", "Test ! ", "Test Data", "Test Data ")) {
                    $InputString = "$LeadingSpace$TrailingText";
                    $Actual = Optimize-WhiteSpace -InputString $InputString -DoNotTrim;
                    $Actual | Should -BeExactly " $TrailingText" -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize trailing space" {
            foreach ($LeadingText in @(".", " .", "Test", " Test", "? !", " ? !", "Test !", " Test !", "? Data", " ? Data", "Test !", " Test !", "Test Data", "Test Data" )) {
                foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    $InputString = "$LeadingText$TrailingSpace";
                    $Actual = Optimize-WhiteSpace -InputString $InputString -DoNotTrim;
                    $Actual | Should -BeExactly "$LeadingText " -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize inner space" {
            foreach ($Word1 in @('?', ' ?', 'Test', ' Test')) {
                foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', '! ', 'Data', 'Data ')) {
                        $InputString = "$Word1$InnerSpace$Word2";
                        $Actual = Optimize-WhiteSpace -InputString $InputString -DoNotTrim;
                        $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize outer space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($InnerText in @(".", "Test", "? !", "Test !", "? Data", "Test !", "Test Data")) {
                    foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        $InputString = "$LeadingSpace$InnerText$TrailingSpace";
                        $Actual = Optimize-WhiteSpace -InputString $InputString -DoNotTrim;
                        $Actual | Should -BeExactly " $InnerText " -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize leading and inner space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', '! ', 'Data', 'Data ')) {
                            $InputString = "$LeadingSpace$Word1$InnerSpace$Word2";
                            $Actual = Optimize-WhiteSpace -InputString $InputString -DoNotTrim;
                            $Actual | Should -BeExactly " $Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize inner and trailing space" {
            foreach ($Word1 in @('?', ' ?', 'Test', ' Test')) {
                foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', 'Data')) {
                        foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                            $InputString = "$Word1$InnerSpace$Word2$TrailingSpace";
                            $Actual = Optimize-WhiteSpace -InputString $InputString -DoNotTrim;
                            $Actual | Should -BeExactly "$Word1 $Word2 " -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize all space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', 'Data')) {
                            foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                                $InputString = "$LeadingSpace$Word1$InnerSpace$Word2$TrailingSpace";
                                $Actual = Optimize-WhiteSpace -InputString $InputString -DoNotTrim;
                                $Actual | Should -BeExactly " $Word1 $Word2 " -Because ($InputString | ConvertTo-Json);
                            }
                        }
                    }
                }
            }
        }
    }

    Context 'Optimize-WhiteSpace -InputString <String> -NullToEmpty' {
        It "should return an unchanged value" {
            foreach ($InputString in @('', '.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data')) {
                $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize to an empty string" {
            foreach ($InputString in @($null, ' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty;
                $Actual | Should -BeExactly '' -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize leading space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($TrailingText in @('.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data')) {
                    $InputString = "$LeadingSpace$TrailingText";
                    $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty;
                    $Actual | Should -BeExactly $TrailingText -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize trailing space" {
            foreach ($LeadingText in @('.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data')) {
                foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    $InputString = "$LeadingText$TrailingSpace";
                    $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty;
                    $Actual | Should -BeExactly $LeadingText -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize inner space" {
            foreach ($Word1 in @('?', 'Test')) {
                foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', 'Data')) {
                        $InputString = "$Word1$InnerSpace$Word2";
                        $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty;
                        $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize outer space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($InnerText in @('.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data')) {
                    foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        $InputString = "$LeadingSpace$InnerText$TrailingSpace";
                        $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty;
                        $Actual | Should -BeExactly $InnerText -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize leading and inner space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', 'Data')) {
                            $InputString = "$LeadingSpace$Word1$InnerSpace$Word2";
                            $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty;
                            $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize inner and trailing space" {
            foreach ($Word1 in @('?', 'Test')) {
                foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', 'Data')) {
                        foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                            $InputString = "$Word1$InnerSpace$Word2$TrailingSpace";
                            $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty;
                            $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize all space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', 'Data')) {
                            foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                                $InputString = "$LeadingSpace$Word1$InnerSpace$Word2$TrailingSpace";
                                $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty;
                                $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                            }
                        }
                    }
                }
            }
        }
    }

    Context 'Optimize-WhiteSpace -InputString <String> -NullToEmpty -DoNotTrim' {
        It "should return an unchanged value" {
            foreach ($InputString in @('', ' ', '.', ' .', '. ', ' . ', 'Test', ' Test', 'Test ', ' Test ', '? !', ' ? !', '? ! ', ' ? ! ',
                    'Test !', ' Test !', 'Test ! ', ' Test ! ', '? Data', ' ? Data', '? Data ', ' ? Data ', 'Test !', ' Test !', 'Test ! ', ' Test ! ',
                    'Test Data', ' Test Data', 'Test Data ', ' Test Data ')) {
                $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty -DoNotTrim;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize to an empty string" {
            $Actual = Optimize-WhiteSpace -InputString $null -NullToEmpty -DoNotTrim;
            $Actual | Should -BeExactly ''
        }
        It "should normalize to a single space" {
            foreach ($InputString in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty -DoNotTrim;
                $Actual | Should -BeExactly ' ' -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize leading space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($TrailingText in @(".", ". ", "Test", "Test ", "? !", "? ! ", "Test !", "Test ! ", "? Data", "? Data ", "Test !", "Test ! ", "Test Data", "Test Data ")) {
                    $InputString = "$LeadingSpace$TrailingText";
                    $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty -DoNotTrim;
                    $Actual | Should -BeExactly " $TrailingText" -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize trailing space" {
            foreach ($LeadingText in @(".", " .", "Test", " Test", "? !", " ? !", "Test !", " Test !", "? Data", " ? Data", "Test !", " Test !", "Test Data", "Test Data" )) {
                foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    $InputString = "$LeadingText$TrailingSpace ";
                    $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty -DoNotTrim;
                    $Actual | Should -BeExactly "$LeadingText " -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize inner space" {
            foreach ($Word1 in @('?', ' ?', 'Test', ' Test')) {
                foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', '! ', 'Data', 'Data ')) {
                        $InputString = "$Word1$InnerSpace$Word2";
                        $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty -DoNotTrim;
                        $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize outer space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($InnerText in @(".", "Test", "? !", "Test !", "? Data", "Test !", "Test Data")) {
                    foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        $InputString = "$LeadingSpace$InnerText$TrailingSpace";
                        $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty -DoNotTrim;
                        $Actual | Should -BeExactly " $InnerText " -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize leading and inner space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', '! ', 'Data', 'Data ')) {
                            $InputString = "$LeadingSpace$Word1$InnerSpace$Word2";
                            $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty -DoNotTrim;
                            $Actual | Should -BeExactly " $Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize inner and trailing space" {
            foreach ($Word1 in @('?', ' ?', 'Test', ' Test')) {
                foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', 'Data')) {
                        foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                            $InputString = "$Word1$InnerSpace$Word2$TrailingSpace";
                            $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty -DoNotTrim;
                            $Actual | Should -BeExactly "$Word1 $Word2 " -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize all space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', 'Data')) {
                            foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                                $InputString = "$LeadingSpace$Word1$InnerSpace$Word2$TrailingSpace";
                                $Actual = Optimize-WhiteSpace -InputString $InputString -NullToEmpty -DoNotTrim;
                                $Actual | Should -BeExactly " $Word1 $Word2 " -Because ($InputString | ConvertTo-Json);
                            }
                        }
                    }
                }
            }
        }
    }

    Context 'Optimize-WhiteSpace -InputString <String> -EmptyToNull' {
        It "should return an unchanged value" {
            foreach ($InputString in @($null, '.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data')) {
                $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize to null" {
            foreach ($InputString in @('', ' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull;
                $Actual | Should -BeExactly $null -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize leading space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($TrailingText in @('.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data')) {
                    $InputString = "$LeadingSpace$TrailingText";
                    $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull;
                    $Actual | Should -BeExactly $TrailingText -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize trailing space" {
            foreach ($LeadingText in @('.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data')) {
                foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    $InputString = "$LeadingText$TrailingSpace";
                    $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull;
                    $Actual | Should -BeExactly $LeadingText -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize inner space" {
            foreach ($Word1 in @('?', 'Test')) {
                foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', 'Data')) {
                        $InputString = "$Word1$InnerSpace$Word2";
                        $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull;
                        $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize outer space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($InnerText in '.', 'Test', '? !', 'Test !', '? Data', 'Test !', 'Test Data') {
                    foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        $InputString = "$LeadingSpace$InnerText$TrailingSpace";
                        $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull;
                        $Actual | Should -BeExactly $InnerText -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize leading and inner space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', 'Data')) {
                            $InputString = "$LeadingSpace$Word1$InnerSpace$Word2";
                            $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull;
                            $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize inner and trailing space" {
            foreach ($Word1 in @('?', 'Test')) {
                foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', 'Data')) {
                        foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                            $InputString = "$Word1$InnerSpace$Word2$TrailingSpace";
                            $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull;
                            $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize all space" {
            foreach ($LeadingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', 'Data')) {
                            foreach ($TrailingSpace in @(' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                                $InputString = "$LeadingSpace$Word1$InnerSpace$Word2$TrailingSpace";
                                $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull;
                                $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                            }
                        }
                    }
                }
            }
        }
    }

    Context 'Optimize-WhiteSpace -InputString <String> -EmptyToNull -DoNotTrim' {
        It "should return an unchanged value" {
            foreach ($InputString in @($null, '.', ' .', '. ', ' . ', 'Test', ' Test', 'Test ', ' Test ', '? !', ' ? !', '? ! ', ' ? ! ',
                    'Test !', ' Test !', 'Test ! ', ' Test ! ', '? Data', ' ? Data', '? Data ', ' ? Data ', 'Test !', ' Test !', 'Test ! ', ' Test ! ',
                    'Test Data', ' Test Data', 'Test Data ', ' Test Data ')) {
                $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull -DoNotTrim;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize to a null" {
            foreach ($InputString in @('', ' ', '  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull -DoNotTrim;
                $Actual | Should -BeExactly $null -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should normalize leading space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($TrailingText in @(".", ". ", "Test", "Test ", "? !", "? ! ", "Test !", "Test ! ", "? Data", "? Data ", "Test !", "Test ! ", "Test Data", "Test Data ")) {
                    $InputString = "$LeadingSpace$TrailingText";
                    $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull -DoNotTrim;
                    $Actual | Should -BeExactly " $TrailingText" -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize trailing space" {
            foreach ($LeadingText in @(".", " .", "Test", " Test", "? !", " ? !", "Test !", " Test !", "? Data", " ? Data", "Test !", " Test !", "Test Data", "Test Data" )) {
                foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    $InputString = "$LeadingText$TrailingSpace";
                    $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull -DoNotTrim;
                    $Actual | Should -BeExactly "$LeadingText " -Because ($InputString | ConvertTo-Json);
                }
            }
        }
        It "should normalize inner space" {
            foreach ($Word1 in @('?', ' ?', 'Test', ' Test')) {
                foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', '! ', 'Data', 'Data ')) {
                        $InputString = "$Word1$InnerSpace$Word2";
                        $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull -DoNotTrim;
                        $Actual | Should -BeExactly "$Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize outer space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($InnerText in @(".", "Test", "? !", "Test !", "? Data", "Test !", "Test Data")) {
                    foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        $InputString = "$LeadingSpace$InnerText$TrailingSpace";
                        $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull -DoNotTrim;
                        $Actual | Should -BeExactly " $InnerText " -Because ($InputString | ConvertTo-Json);
                    }
                }
            }
        }
        It "should normalize leading and inner space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', '! ', 'Data', 'Data ')) {
                            $InputString = "$LeadingSpace$Word1$InnerSpace$Word2";
                            $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull -DoNotTrim;
                            $Actual | Should -BeExactly " $Word1 $Word2" -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize inner and trailing space" {
            foreach ($Word1 in @('?', ' ?', 'Test', ' Test')) {
                foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                    foreach ($Word2 in @('!', 'Data')) {
                        foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                            $InputString = "$Word1$InnerSpace$Word2$TrailingSpace";
                            $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull -DoNotTrim;
                            $Actual | Should -BeExactly "$Word1 $Word2 " -Because ($InputString | ConvertTo-Json);
                        }
                    }
                }
            }
        }
        It "should normalize all space" {
            foreach ($LeadingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                foreach ($Word1 in @('?', 'Test')) {
                    foreach ($InnerSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                        foreach ($Word2 in @('!', 'Data')) {
                            foreach ($TrailingSpace in @('  ', "`n", " `n", "`n ", " `n ", "`n`t  `n")) {
                                $InputString = "$LeadingSpace$Word1$InnerSpace$Word2$TrailingSpace";
                                $Actual = Optimize-WhiteSpace -InputString $InputString -EmptyToNull -DoNotTrim;
                                $Actual | Should -BeExactly " $Word1 $Word2 " -Because ($InputString | ConvertTo-Json);
                            }
                        }
                    }
                }
            }
        }
    }
}

Describe 'Remove-ZeroPadding Function' {
    Context 'Remove-ZeroPadding -InputString <string>' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Value;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "+$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "+0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -AllowPositiveSign' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Value -AllowPositiveSign;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "-$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol and 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol and multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "+000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -AllowNegativeSign' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $Actual = Remove-ZeroPadding -InputString $Value -AllowNegativeSign;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "+$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "+0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($Expected in @('1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Expected -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($Expected | ConvertTo-Json);
                $InputString = "-$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol and 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol and multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "-000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -AllowPositiveSign -KeepPositiveSign' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Value -AllowPositiveSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "+$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave positive symbol and remove 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "+0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign;
                $Actual | Should -BeExactly "+$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave positive symbol and remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "+00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign;
                $Actual | Should -BeExactly "+$Expected" -Because ($InputString | ConvertTo-Json);
                $InputString = "+000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign;
                $Actual | Should -BeExactly "+$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -AllowNegativeSign -KeepNegativeSign' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Value -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "-$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "+$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "+0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave negative symbol and remove 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "-0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly "-$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave negative symbol and remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "-00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly "-$Expected" -Because ($InputString | ConvertTo-Json);
                $InputString = "-000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly "-$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -AllowPositiveSign -AllowNegativeSign' {
        It "should return an unchanged value" {
            foreach ($InputString in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($Expected in @('1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Expected -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($Expected | ConvertTo-Json);
                $InputString = "-$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol and 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol and multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "-000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol and 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol and multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "+000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign' {
        It "should return an unchanged value" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $Actual = Remove-ZeroPadding -InputString $Expected -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($Expected | ConvertTo-Json);
                $InputString = "+$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($Expected in @('1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Expected -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($Expected | ConvertTo-Json);
                $InputString = "-$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "+$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol and 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol and multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "-000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave positive symbol and remove 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly "+$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave positive symbol and remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly "+$Expected" -Because ($InputString | ConvertTo-Json);
                $InputString = "+000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly "+$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Value -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "-$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave negative symbol and remove 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly "-$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave negative symbol and remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly "-$Expected" -Because ($InputString | ConvertTo-Json);
                $InputString = "-000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly "-$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol and 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol and multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "+000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign -KeepNegativeSign' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Value -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "+$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave negative symbol and remove 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly "-$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave negative symbol and remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly "-$Expected" -Because ($InputString | ConvertTo-Json);
                $InputString = "-000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign -KeepNegativeSign;
                $Actual | Should -BeExactly "-$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave positive symbol and remove 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly "+$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should leave positive symbol and remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly "+$Expected" -Because ($InputString | ConvertTo-Json);
                $InputString = "+000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepPositiveSign -AllowNegativeSign;
                $Actual | Should -BeExactly "+$Expected" -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -AllowPositiveSign -KeepNegativeSign' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Value -AllowPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "-$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol and 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove positive symbol and multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "+00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "+000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -AllowNegativeSign -KeepPositiveSign' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $Actual = Remove-ZeroPadding -InputString $Value -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "+$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "+0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($Expected in @('1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Expected -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($Expected | ConvertTo-Json);
                $InputString = "-$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol and 1 zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove negative symbol and multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1')) {
                $InputString = "-00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "-000$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -AllowNegativeSign -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -KeepPositiveSign -KeepNegativeSign' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Value -KeepPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "+$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "+0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -KeepNegativeSign' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Value -KeepNegativeSign;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "+$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "+0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepNegativeSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepNegativeSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
    }

    Context 'Remove-ZeroPadding -InputString <string> -KeepPositiveSign' {
        It "should return an unchanged value" {
            foreach ($Value in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $Actual = Remove-ZeroPadding -InputString $Value -KeepPositiveSign;
                $Actual | Should -BeExactly $Value -Because ($Value | ConvertTo-Json);
                $InputString = "+$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "+0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
                $InputString = "-0$Value";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
            foreach ($InputString in @('Z', '-Z', '+Z')) {
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign;
                $Actual | Should -BeExactly $InputString -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove 1 leading zero" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "0$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
        It "should remove multiple leading zeros" {
            foreach ($Expected in @('0', '0.0', '0Z', '0x0', '0-1', '0+1', '1', '10', '1.0', '1Z')) {
                $InputString = "00$Expected";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
                $InputString = "0$InputString";
                $Actual = Remove-ZeroPadding -InputString $InputString -KeepPositiveSign;
                $Actual | Should -BeExactly $Expected -Because ($InputString | ConvertTo-Json);
            }
        }
    }
}

# Describe 'Testing Out-IndentedText Function', {
# }

# Describe 'Testing Get-IndentLevel Function', {
# }

# Describe 'Testing Out-UnindentedText Function', {
# }

# Describe 'Testing Expand-GZip Function', {
# }

# Describe 'Testing Use-TempFolder Function', {
# }
