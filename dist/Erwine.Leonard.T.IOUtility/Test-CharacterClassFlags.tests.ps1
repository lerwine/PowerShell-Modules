Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.IOUtility.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'Test-CharacterClassFlags -Flags BinaryDigitNumber' {
    Context 'IsNot.Present = $false' {
        It '1 or 0 should return true' {
            $Actual = Test-CharacterClassFlags -Value '1' -Flags BinaryDigitNumber -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"1"';
            $Actual = Test-CharacterClassFlags -Value '0' -Flags BinaryDigitNumber -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"0"';
        }

        It 'Digits other than 1 or 0 should return false' {
            foreach ($Value in ('2', '3', '4', '5', '6', '7', '8', '9')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags BinaryDigitNumber -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }

        It 'Non-Digits should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', 'A', '^', '_', 'z', '|', '¡', '£', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags BinaryDigitNumber -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '1 or 0 should return false' {
            $Actual = Test-CharacterClassFlags -Value '1' -IsNot -Flags BinaryDigitNumber -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"1"';
            $Actual = Test-CharacterClassFlags -Value '0' -IsNot -Flags BinaryDigitNumber -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"0"';
        }

        It 'Digits other than 1 or 0 should return true' {
            foreach ($Value in ('2', '3', '4', '5', '6', '7', '8', '9')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags BinaryDigitNumber -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }

        It 'Non-Digits should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', 'A', '^', '_', 'z', '|', '¡', '£', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags BinaryDigitNumber -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags ModifierSymbol' {
    Context 'IsNot.Present = $false' {
        It '"^" should return true' {
            $Actual = Test-CharacterClassFlags -Value "^" -Flags ModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"^"';
        }
        
        It '"˅" should return true' {
            $Actual = Test-CharacterClassFlags -Value "˅" -Flags ModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"˅"';
        }

        It 'Non-Modifiers should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '_', 'z', '|', '¡', '£', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags ModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"^" should return false' {
            $Actual = Test-CharacterClassFlags -Value "^" -IsNot -Flags ModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"^"';
        }
        
        It '"˅" should return false' {
            $Actual = Test-CharacterClassFlags -Value "˅" -IsNot -Flags ModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"˅"';
        }

        It 'Non-Modifiers should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '_', 'z', '|', '¡', '£', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags ModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags NonAsciiModifierSymbol' {
    Context 'IsNot.Present = $false' {
        It '"˅" should return true' {
            $Actual = Test-CharacterClassFlags -Value "˅" -Flags NonAsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"˅"';
        }

        It '"^" should return false' {
            $Actual = Test-CharacterClassFlags -Value "^" -Flags NonAsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"^"';
        }
        
        It 'Non-NonAsciiModifierSymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '£', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags NonAsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"˅" should return false' {
            $Actual = Test-CharacterClassFlags -Value "˅" -IsNot -Flags NonAsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"˅"';
        }

        It '"^" should return true' {
            $Actual = Test-CharacterClassFlags -Value "^" -IsNot -Flags NonAsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"^"';
        }
        
        It 'Non-NonAsciiModifierSymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '£', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags NonAsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags AsciiModifierSymbol' {
    Context 'IsNot.Present = $false' {
        It '"^" should return true' {
            $Actual = Test-CharacterClassFlags -Value "^" -Flags AsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"^"';
        }
        
        It '"˅" should return false' {
            $Actual = Test-CharacterClassFlags -Value "˅" -Flags AsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"˅"';
        }

        It 'Non-AsciiModifierSymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '_', 'z', '|', '¡', '£', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags AsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"^" should return false' {
            $Actual = Test-CharacterClassFlags -Value "^" -IsNot -Flags AsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"^"';
        }
        
        It '"˅" should return true' {
            $Actual = Test-CharacterClassFlags -Value "˅" -IsNot -Flags AsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"˅"';
        }

        It 'Non-AsciiModifierSymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '_', 'z', '|', '¡', '£', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags AsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags Symbol' {
    Context 'IsNot.Present = $false' {
        It 'Symbols should return true' {
            foreach ($Value in ('$', '+', '^', '|', '£', '¦', '±', '˅')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags NonAsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
        
        It 'Non-Symbols should return true' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '-', '0', '2', '7', '9', 'A', '_', 'z', '¡', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags NonAsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
    }
}

Describe 'Test-CharacterClassFlags -Flags CurrencySymbol' {
    Context 'IsNot.Present = $false' {
        It '"$" should return true' {
            $Actual = Test-CharacterClassFlags -Value "$" -Flags CurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"$"';
        }
        
        It '"£" should return true' {
            $Actual = Test-CharacterClassFlags -Value "£" -Flags CurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"£"';
        }

        It 'Non-CurrencySymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags CurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"$" should return false' {
            $Actual = Test-CharacterClassFlags -Value "$" -IsNot -Flags CurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"$"';
        }
        
        It '"£" should return false' {
            $Actual = Test-CharacterClassFlags -Value "£" -IsNot -Flags CurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"£"';
        }

        It 'Non-CurrencySymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags CurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags NonAsciiCurrencySymbol' {
    Context 'IsNot.Present = $false' {
        It '"£" should return true' {
            $Actual = Test-CharacterClassFlags -Value "£" -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"£"';
        }

        It '"$" should return false' {
            $Actual = Test-CharacterClassFlags -Value "$" -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"$"';
        }
        
        It 'Non-CurrencySymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"£" should return false' {
            $Actual = Test-CharacterClassFlags -Value "£" -IsNot -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"£"';
        }

        It '"$" should return true' {
            $Actual = Test-CharacterClassFlags -Value "$" -IsNot -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"$"';
        }
        
        It 'Non-CurrencySymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags AsciiCurrencySymbol' {
    Context 'IsNot.Present = $false' {
        It '"$" should return true' {
            $Actual = Test-CharacterClassFlags -Value "$" -Flags AsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"$"';
        }
        
        It '"£" should return false' {
            $Actual = Test-CharacterClassFlags -Value "£" -Flags AsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"£"';
        }

        It 'Non-CurrencySymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags AsciiCurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"$" should return false' {
            $Actual = Test-CharacterClassFlags -Value "$" -IsNot -Flags AsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"$"';
        }
        
        It '"£" should return true' {
            $Actual = Test-CharacterClassFlags -Value "£" -IsNot -Flags AsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"£"';
        }

        It 'Non-CurrencySymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags AsciiCurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags MathSymbol' {
    Context 'IsNot.Present = $false' {
        It '"+" and "|" should return true' {
            $Actual = Test-CharacterClassFlags -Value "+" -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"+"';
            $Actual = Test-CharacterClassFlags -Value "|" -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"|"';
        }
        
        It '"±" should return true' {
            $Actual = Test-CharacterClassFlags -Value "±" -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"±"';
        }

        It 'Non-MathSymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags MathSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"+" and "|" should return false' {
            $Actual = Test-CharacterClassFlags -Value "+" -IsNot -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"+"';
            $Actual = Test-CharacterClassFlags -Value "|" -IsNot -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"|"';
        }
        
        It '"±" should return false' {
            $Actual = Test-CharacterClassFlags -Value "±" -IsNot -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"±"';
        }

        It 'Non-MathSymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags MathSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags NonAsciiMathSymbol' {
    Context 'IsNot.Present = $false' {
        It '"±" should return true' {
            $Actual = Test-CharacterClassFlags -Value "±" -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"±"';
        }

        It '"+" and "|" should return false' {
            $Actual = Test-CharacterClassFlags -Value "+" -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"+"';
            $Actual = Test-CharacterClassFlags -Value "|" -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"|"';
        }
        
        It 'Non-MathSymbols should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags NonAsciiMathSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"±" should return false' {
            $Actual = Test-CharacterClassFlags -Value "±" -IsNot -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"±"';
        }

        It '"+" and "|" should return true' {
            $Actual = Test-CharacterClassFlags -Value "+" -IsNot -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"+"';
            $Actual = Test-CharacterClassFlags -Value "|" -IsNot -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"|"';
        }
        
        It 'Non-MathSymbols should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags NonAsciiMathSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags AsciiMathSymbol' {
    Context 'IsNot.Present = $false' {
        It '"+" and "|" should return true' {
            $Actual = Test-CharacterClassFlags -Value "+" -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"+"';
            $Actual = Test-CharacterClassFlags -Value "|" -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"|"';
        }
        
        It '"±" should return false' {
            $Actual = Test-CharacterClassFlags -Value "±" -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"±"';
        }

        It 'Non-MathSymbols should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags AsciiMathSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"+" and "|" should return false' {
            $Actual = Test-CharacterClassFlags -Value "+" -IsNot -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"+"';
            $Actual = Test-CharacterClassFlags -Value "|" -IsNot -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"|"';
        }
        
        It '"±" should return true' {
            $Actual = Test-CharacterClassFlags -Value "±" -IsNot -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"±"';
        }

        It 'Non-MathSymbols should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags AsciiMathSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags OtherSymbol' {
    Context 'IsNot.Present = $false' {
        It '"¦" should return true' {
            $Actual = Test-CharacterClassFlags -Value "¦" -Flags OtherSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"¦"';
        }
        
        It 'Non-OtherSymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '£', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags OtherSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"¦" should return false' {
            $Actual = Test-CharacterClassFlags -Value "¦" -IsNot -Flags OtherSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"¦"';
        }
        
        It 'Non-OtherSymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '£', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags OtherSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags Punctuation' {
    Context 'IsNot.Present = $false' {
        It 'Punctuation characters should return true' {
            foreach ($Value in ('(', ')', '-', '_', '¡', '«', '»', '־')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags Punctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
        
        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags Punctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It 'Punctuation characters should return false' {
            foreach ($Value in ('(', ')', '-', '_', '¡', '«', '»', '־')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags Punctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
        
        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags Punctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags InitialQuotePunctuation' {
    Context 'IsNot.Present = $false' {
        It '"«" should return true' {
            $Actual = Test-CharacterClassFlags -Value "«" -Flags InitialQuotePunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"«"';
        }
        
        It 'Other Punctuation should return false' {
            foreach ($Value in '(', ')', '-', '_', '¡', '»', '־') {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
        
        It 'Non-InitialQuotePunctuation should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"«" should return true' {
            $Actual = Test-CharacterClassFlags -Value "«" -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"«"';
        }
        
        It 'Other Punctuation should return false' {
            foreach ($Value in '(', ')', '-', '_', '¡', '»', '־') {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
        
        It 'Non-InitialQuotePunctuation should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClassFlags -Flags FinalQuotePunctuation' {
    Context 'IsNot.Present = $false' {
        It '"»" should return true' {
            $Actual = Test-CharacterClassFlags -Value "»" -Flags InitialQuotePunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"»"';
        }
        
        It 'Other Punctuation should return false' {
            foreach ($Value in '(', ')', '-', '_', '¡', '«', '־') {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
        
        It 'Non-InitialQuotePunctuation should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $false' {
        It '"»" should return true' {
            $Actual = Test-CharacterClassFlags -Value "»" -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"»"';
        }
        
        It 'Other Punctuation should return true' {
            foreach ($Value in '(', ')', '-', '_', '¡', '«', '־') {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
        
        It 'Non-InitialQuotePunctuation should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClassFlags -Value $Value -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

# Describe 'Test-CharacterClassFlags -Flags OpenPunctuation' {
#     # [System.Globalization.UnicodeCategory]::OpenPunctuation
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonAsciiOpenPunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and -not [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiOpenPunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonAsciiClosePunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and -not [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiClosePunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags ConnectorPunctuation' {
#     # [System.Globalization.UnicodeCategory]::ConnectorPunctuation
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonAsciiConnectorPunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and -not [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiConnectorPunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags DashPunctuation' {
#     # [System.Globalization.UnicodeCategory]::DashPunctuation
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonAsciiDashPunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and -not [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiDashPunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags OtherPunctuation' {
#     # [System.Globalization.UnicodeCategory]::OtherPunctuation
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags OtherNonAsciiPunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and -not [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags OtherAsciiPunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags EnclosingMark' {
#     # [System.Globalization.UnicodeCategory]::EnclosingMark
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags Surrogate' {
#     # [char]::IsSurrogate($args[0])
#     # [System.Globalization.UnicodeCategory]::Surrogate
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags LowSurrogate' {
#     # [char]::IsLowSurrogate($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags HighSurrogate' {
#     # [char]::IsHighSurrogate($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags Format' {
#     # [System.Globalization.UnicodeCategory]::Format
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags WhiteSpace' {
#     # [char]::IsWhiteSpace($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags Control' {
#     # [char]::IsControl($args[0])
#     # [System.Globalization.UnicodeCategory]::Control
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags Ascii' {
#     # [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonSpacingMark' {
#     # [System.Globalization.UnicodeCategory]::NonSpacingMark
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonAsciiControl' {
#     # [char]::IsControl($args[0]) -and -not [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags Separator' {
#     # [char]::IsSeparator($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags ParagraphSeparator' {
#     # [System.Globalization.UnicodeCategory]::ParagraphSeparator
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags LineSeparator' {
#     # [System.Globalization.UnicodeCategory]::LineSeparator
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags SpaceSeparator' {
#     # [System.Globalization.UnicodeCategory]::SpaceSeparator
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonAsciiSpaceSeparator' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and -not [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiSpaceSeparator' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags SpacingCombiningMark' {
#     # [System.Globalization.UnicodeCategory]::SpacingCombiningMark
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiControl' {
#     # [char]::IsControl($args[0]) -and [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags OtherNumber' {
#     # [System.Globalization.UnicodeCategory]::OtherNumber
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags LetterNumber' {
#     # [System.Globalization.UnicodeCategory]::LetterNumber
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiHexDigit' {
#     # [char]::IsAsciiHexDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiHexDigitLower' {
#     # [char]::IsAsciiHexDigitLower($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags HexDigitLetterLower' {
#     # [char]::IsAsciiHexDigitLower($args[0]) -and -not [char]::IsAsciiDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiHexDigitUpper' {
#     # [char]::IsAsciiHexDigitUpper($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags HexDigitLetterUpper' {
#     # [char]::IsAsciiHexDigitUpper($args[0]) -and -not [char]::IsAsciiDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonHexAsciiUppercaseLetter' {
#     # [char]::IsAsciiLetterUpper($args[0]) -and -not [char]::IsAsciiHexDigitUpper($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags Number' {
#     # [char]::IsNumber($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags DecimalDigitNumber' {
#     # [System.Globalization.UnicodeCategory]::DecimalDigitNumber
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags Digit' {
#     # [char]::IsDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonAsciiDecimalDigit' {
#     # [char]::IsDigit($args[0]) -and -not [char]::IsAsciiDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiDigit' {
#     # [char]::IsAsciiDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonOctalDecimalDigit' {
#     # $args[0] -eq '8' -or $args[0] -eq '9'
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags OctalDigitNumber' {
#     # $args[0] -ge '0' -and $args[0] -lt '8'
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonBinaryOctalDigit' {
#     # $args[0] -gt '1' -and $args[0] -lt '8'
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonDigitNumber' {
#     # [char]::IsNumber($args[0]) -and -not [char]::IsDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiLetterUpper' {
#     # [char]::IsAsciiLetterUpper($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonAsciiUppercaseLetter' {
#     # [char]::IsUpper($args[0]) -and -not [char]::IsAsciiLetterUpper($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags UppercaseLetter' {
#     # [char]::IsUpper($args[0])
#     # [System.Globalization.UnicodeCategory]::UppercaseLetter
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags LetterOrDigit' {
#     # [char]::IsLetterOrDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiLetterOrDigit' {
#     # [char]::IsAsciiLetterOrDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags Letter' {
#     # [char]::IsLetter($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags OtherLetter' {
#     # [System.Globalization.UnicodeCategory]::OtherLetter
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags ModifierLetter' {
#     # [System.Globalization.UnicodeCategory]::ModifierLetter
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags TitlecaseLetter' {
#     # [System.Globalization.UnicodeCategory]::TitlecaseLetter
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiLetter' {
#     # [char]::IsAsciiLetter($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags LowercaseLetter' {
#     # [char]::IsLower($args[0])
#     # [System.Globalization.UnicodeCategory]::LowercaseLetter
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonAsciiLowercaseLetter' {
#     # [char]::IsLower($args[0]) -and -not [char]::IsAsciiLetterLower($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags AsciiLetterLower' {
#     # [char]::IsAsciiLetterLower($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags NonHexAsciiLowercaseLetter' {
#     # [char]::IsAsciiLetterLower($args[0]) -and -not [char]::IsAsciiHexDigitLower($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags PrivateUse' {
#     # [System.Globalization.UnicodeCategory]::PrivateUse
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClassFlags -Flags OtherNotAssigned' {
#     # [System.Globalization.UnicodeCategory]::OtherNotAssigned
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }