Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.IOUtility.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'Test-CharacterClass -Flags BinaryDigitNumber' {
    Context 'IsNot.Present = $false' {
        It '1 or 0 should return true' {
            $Actual = Test-CharacterClass -Value '1' -Flags BinaryDigitNumber -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"1"';
            $Actual = Test-CharacterClass -Value '0' -Flags BinaryDigitNumber -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"0"';
        }

        It 'Digits other than 1 or 0 should return false' {
            foreach ($Value in ('2', '3', '4', '5', '6', '7', '8', '9')) {
                $Actual = Test-CharacterClass -Value $Value -Flags BinaryDigitNumber -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }

        It 'Non-Digits should return false' {
            foreach ($Value in ("`t", "`n", ' ', '_', '-', '¡', '„', '«', '»', '(', ')', '₎', '‿', '־', '^', '˅', '+', '±', '|', '¦', '$', '£', '¼', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags BinaryDigitNumber -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '1 or 0 should return false' {
            $Actual = Test-CharacterClass -Value '1' -IsNot -Flags BinaryDigitNumber -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"1"';
            $Actual = Test-CharacterClass -Value '0' -IsNot -Flags BinaryDigitNumber -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"0"';
        }

        It 'Digits other than 1 or 0 should return true' {
            foreach ($Value in ('2', '3', '4', '5', '6', '7', '8', '9')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags BinaryDigitNumber -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }

        It 'Non-Digits should return true' {
            foreach ($Value in ("`t", "`n", ' ', '_', '-', '¡', '„', '«', '»', '(', ')', '₎', '‿', '־', '^', '˅', '+', '±', '|', '¦', '$', '£', '¼', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags BinaryDigitNumber -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags Symbol' {
    Context 'IsNot.Present = $false' {
        It 'Symbol characters should return true' {
            foreach ($Value in ('$', '+', '^', '|', '£', '¦', '±', '˅')) {
                $Actual = Test-CharacterClass -Value $Value -Flags Symbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Symbol characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '-', '0', '2', '7', '9', 'A', '_', 'z', '¡', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', '„', '‿', '₎', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags Symbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It 'Symbol characters should return false' {
            foreach ($Value in ('$', '+', '^', '|', '£', '¦', '±', '˅')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags Symbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Symbol characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '-', '0', '2', '7', '9', 'A', '_', 'z', '¡', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', '„', '‿', '₎', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags Symbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags ModifierSymbol' {
    Context 'IsNot.Present = $false' {
        It '"^" and "^" should return true' {
            foreach ($Value in ('^', '˅')) {
                $Actual = Test-CharacterClass -Value $Value -Flags ModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Other Symbol characters should return false' {
            foreach ($Value in ('$', '+', '|', '£', '¦', '±')) {
                $Actual = Test-CharacterClass -Value $Value -Flags ModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Symbol characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '-', '0', '2', '7', '9', 'A', '_', 'z', '¡', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', '„', '‿', '₎', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags ModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"^" and "^" should return true' {
            foreach ($Value in ('^', '˅')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags ModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Other Symbol characters should return true' {
            foreach ($Value in ('$', '+', '|', '£', '¦', '±')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags ModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Symbol characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '-', '0', '2', '7', '9', 'A', '_', 'z', '¡', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', '„', '‿', '₎', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags ModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags AsciiModifierSymbol' {
    Context 'IsNot.Present = $false' {

        It '"^" should return true' {
            $Actual = Test-CharacterClass -Value '^' -Flags AsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"^"';
        }

        It 'Other Symbol characters should return false' {
            foreach ($Value in ('$', '+', '|', '£', '¦', '±', '˅')) {
                $Actual = Test-CharacterClass -Value $Value -Flags AsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Symbol characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '-', '0', '2', '7', '9', 'A', '_', 'z', '¡', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', '„', '‿', '₎', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags AsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {

        It '"^" should return false' {
            $Actual = Test-CharacterClass -Value '^' -Flags AsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"^"';
        }

        It 'Other Symbol characters should return true' {
            foreach ($Value in ('$', '+', '|', '£', '¦', '±', '˅')) {
                $Actual = Test-CharacterClass -Value $Value -Flags AsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Symbol characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '-', '0', '2', '7', '9', 'A', '_', 'z', '¡', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', '„', '‿', '₎', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags AsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags NonAsciiModifierSymbol' {
    Context 'IsNot.Present = $false' {

        It '"˅" should return true' {
            $Actual = Test-CharacterClass -Value '˅' -Flags NonAsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"˅"';
        }

        It 'Other Symbol characters should return false' {
            foreach ($Value in ('$', '+', '^', '|', '£', '¦', '±')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Symbol characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '-', '0', '2', '7', '9', 'A', '_', 'z', '¡', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', '„', '‿', '₎', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {

        It '"˅" should return false' {
            $Actual = Test-CharacterClass -Value '˅' -Flags NonAsciiModifierSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"˅"';
        }

        It 'Other Symbol characters should return true' {
            foreach ($Value in ('$', '+', '^', '|', '£', '¦', '±')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Symbol characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '-', '0', '2', '7', '9', 'A', '_', 'z', '¡', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', 'ˮ', '־', '„', '‿', '₎', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiModifierSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags CurrencySymbol' {
    Context 'IsNot.Present = $false' {
        It '"$" should return true' {
            $Actual = Test-CharacterClass -Value "$" -Flags CurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"$"';
        }

        It '"£" should return true' {
            $Actual = Test-CharacterClass -Value "£" -Flags CurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"£"';
        }

        It 'Non-CurrencySymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags CurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"$" should return false' {
            $Actual = Test-CharacterClass -Value "$" -IsNot -Flags CurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"$"';
        }

        It '"£" should return false' {
            $Actual = Test-CharacterClass -Value "£" -IsNot -Flags CurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"£"';
        }

        It 'Non-CurrencySymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags CurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags NonAsciiCurrencySymbol' {
    Context 'IsNot.Present = $false' {
        It '"£" should return true' {
            $Actual = Test-CharacterClass -Value "£" -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"£"';
        }

        It '"$" should return false' {
            $Actual = Test-CharacterClass -Value "$" -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"$"';
        }

        It 'Non-CurrencySymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"£" should return false' {
            $Actual = Test-CharacterClass -Value "£" -IsNot -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"£"';
        }

        It '"$" should return true' {
            $Actual = Test-CharacterClass -Value "$" -IsNot -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"$"';
        }

        It 'Non-CurrencySymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags NonAsciiCurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags AsciiCurrencySymbol' {
    Context 'IsNot.Present = $false' {
        It '"$" should return true' {
            $Actual = Test-CharacterClass -Value "$" -Flags AsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"$"';
        }

        It '"£" should return false' {
            $Actual = Test-CharacterClass -Value "£" -Flags AsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"£"';
        }

        It 'Non-CurrencySymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags AsciiCurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"$" should return false' {
            $Actual = Test-CharacterClass -Value "$" -IsNot -Flags AsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"$"';
        }

        It '"£" should return true' {
            $Actual = Test-CharacterClass -Value "£" -IsNot -Flags AsciiCurrencySymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"£"';
        }

        It 'Non-CurrencySymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '¦', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags AsciiCurrencySymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags MathSymbol' {
    Context 'IsNot.Present = $false' {
        It '"+" and "|" should return true' {
            $Actual = Test-CharacterClass -Value "+" -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"+"';
            $Actual = Test-CharacterClass -Value "|" -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"|"';
        }

        It '"±" should return true' {
            $Actual = Test-CharacterClass -Value "±" -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"±"';
        }

        It 'Non-MathSymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags MathSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"+" and "|" should return false' {
            $Actual = Test-CharacterClass -Value "+" -IsNot -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"+"';
            $Actual = Test-CharacterClass -Value "|" -IsNot -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"|"';
        }

        It '"±" should return false' {
            $Actual = Test-CharacterClass -Value "±" -IsNot -Flags MathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"±"';
        }

        It 'Non-MathSymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags MathSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags NonAsciiMathSymbol' {
    Context 'IsNot.Present = $false' {
        It '"±" should return true' {
            $Actual = Test-CharacterClass -Value "±" -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"±"';
        }

        It '"+" and "|" should return false' {
            $Actual = Test-CharacterClass -Value "+" -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"+"';
            $Actual = Test-CharacterClass -Value "|" -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"|"';
        }

        It 'Non-MathSymbols should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiMathSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"±" should return false' {
            $Actual = Test-CharacterClass -Value "±" -IsNot -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"±"';
        }

        It '"+" and "|" should return true' {
            $Actual = Test-CharacterClass -Value "+" -IsNot -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"+"';
            $Actual = Test-CharacterClass -Value "|" -IsNot -Flags NonAsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"|"';
        }

        It 'Non-MathSymbols should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags NonAsciiMathSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags AsciiMathSymbol' {
    Context 'IsNot.Present = $false' {
        It '"+" and "|" should return true' {
            $Actual = Test-CharacterClass -Value "+" -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"+"';
            $Actual = Test-CharacterClass -Value "|" -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"|"';
        }

        It '"±" should return false' {
            $Actual = Test-CharacterClass -Value "±" -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"±"';
        }

        It 'Non-MathSymbols should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags AsciiMathSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"+" and "|" should return false' {
            $Actual = Test-CharacterClass -Value "+" -IsNot -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"+"';
            $Actual = Test-CharacterClass -Value "|" -IsNot -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"|"';
        }

        It '"±" should return true' {
            $Actual = Test-CharacterClass -Value "±" -IsNot -Flags AsciiMathSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"±"';
        }

        It 'Non-MathSymbols should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '¡', '£', '¦', '«', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags AsciiMathSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags OtherSymbol' {
    Context 'IsNot.Present = $false' {
        It '"¦" should return true' {
            $Actual = Test-CharacterClass -Value "¦" -Flags OtherSymbol -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"¦"';
        }

        It 'Non-OtherSymbol should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '£', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags OtherSymbol -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"¦" should return false' {
            $Actual = Test-CharacterClass -Value "¦" -IsNot -Flags OtherSymbol -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"¦"';
        }

        It 'Non-OtherSymbol should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '(', ')', '+', '-', '0', '2', '7', '9', 'A', '^', '_', 'z', '|', '¡', '£', '«', '±', 'µ', '»', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', '־', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags OtherSymbol -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags Punctuation' {
    Context 'IsNot.Present = $false' {
        It 'Punctuation characters should return true' {
            foreach ($Value in ('(', ')', '-', '_', '¡', '„', '«', '»', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -Flags Punctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }

        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags Punctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It 'Punctuation characters should return false' {
            foreach ($Value in ('(', ')', '-', '_', '¡', '„', '«', '»', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags Punctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }

        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags Punctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags InitialQuotePunctuation' {
    Context 'IsNot.Present = $false' {
        It '"«" should return true' {
            $Actual = Test-CharacterClass -Value "«" -Flags InitialQuotePunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"«"';
        }

        It 'Other Punctuation should return false' {
            foreach ($Value in '(', ')', '-', '_', '¡', '„', '»', '₎', '־') {
                $Actual = Test-CharacterClass -Value $Value -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }

        It 'Non-InitialQuotePunctuation should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"«" should return true' {
            $Actual = Test-CharacterClass -Value "«" -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"«"';
        }

        It 'Other Punctuation should return false' {
            foreach ($Value in '(', ')', '-', '_', '¡', '„', '»', '₎', '־') {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }

        It 'Non-InitialQuotePunctuation should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags FinalQuotePunctuation' {
    Context 'IsNot.Present = $false' {
        It '"»" should return true' {
            $Actual = Test-CharacterClass -Value "»" -Flags InitialQuotePunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"»"';
        }

        It 'Other Punctuation should return false' {
            foreach ($Value in '(', ')', '-', '_', '¡', '„', '«', '₎', '־') {
                $Actual = Test-CharacterClass -Value $Value -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }

        It 'Non-InitialQuotePunctuation should return false' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value`"";
            }
        }
    }

    Context 'IsNot.Present = $false' {
        It '"»" should return true' {
            $Actual = Test-CharacterClass -Value "»" -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"»"';
        }

        It 'Other Punctuation should return true' {
            foreach ($Value in '(', ')', '-', '_', '¡', '„', '«', '₎', '־') {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }

        It 'Non-InitialQuotePunctuation should return true' {
            foreach ($Value in ("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', 'ˮ', 'Ⅵ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags InitialQuotePunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value`"";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags OpenPunctuation' {
    Context 'IsNot.Present = $false' {
        It '"(" should return true' {
            $Actual = Test-CharacterClass -Value '(' -Flags OpenPunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"("';
        }

        It 'Other Punctuation characters should return false' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', ')', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -Flags OpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags OpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"(" should return false' {
            $Actual = Test-CharacterClass -Value '(' -IsNot -Flags OpenPunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"("';
        }

        It 'Other Punctuation characters should return true' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', ')', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags OpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags OpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags NonAsciiOpenPunctuation' {
    Context 'IsNot.Present = $false' {
        It '"„" should return true' {
            $Actual = Test-CharacterClass -Value '„' -Flags NonAsciiOpenPunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"„"';
        }

        It 'Other Punctuation characters should return false' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', '(', ')', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiOpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiOpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"„" should return false' {
            $Actual = Test-CharacterClass -Value '„' -IsNot -Flags NonAsciiOpenPunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"„"';
        }

        It 'Other Punctuation characters should return true' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', '(', ')', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags NonAsciiOpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags NonAsciiOpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags AsciiOpenPunctuation' {
    Context 'IsNot.Present = $false' {
        It '"(" should return true' {
            $Actual = Test-CharacterClass -Value '(' -Flags AsciiOpenPunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"("';
        }

        It 'Other Punctuation characters should return false' {
            foreach ($Value in ('_', '-', '¡', '„', '„', '«', '»', ')', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -Flags AsciiOpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags AsciiOpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"(" should return false' {
            $Actual = Test-CharacterClass -Value '(' -IsNot -Flags AsciiOpenPunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"("';
        }

        It 'Other Punctuation characters should return true' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', ')', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags AsciiOpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags AsciiOpenPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags NonAsciiClosePunctuation' {
    Context 'IsNot.Present = $false' {
        It '"₎" should return true' {
            $Actual = Test-CharacterClass -Value '₎' -Flags NonAsciiClosePunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"₎"';
        }

        It 'Other Punctuation characters should return false' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', '(', ')', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiClosePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiClosePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"₎" should return false' {
            $Actual = Test-CharacterClass -Value '₎' -IsNot -Flags NonAsciiClosePunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"₎"';
        }

        It 'Other Punctuation characters should return true' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', '(', ')', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags NonAsciiClosePunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags NonAsciiClosePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags AsciiClosePunctuation' {
    Context 'IsNot.Present = $false' {
        It '")" should return true' {
            $Actual = Test-CharacterClass -Value ')' -Flags AsciiClosePunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '")"';
        }

        It 'Other Punctuation characters should return false' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', '(', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -Flags AsciiClosePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags AsciiClosePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '")" should return false' {
            $Actual = Test-CharacterClass -Value ')' -IsNot -Flags AsciiClosePunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '")"';
        }

        It 'Other Punctuation characters should return true' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', '(', '₎', '־')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags AsciiClosePunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags AsciiClosePunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags ConnectorPunctuation' {
    Context 'IsNot.Present = $false' {
        It '"_" should return true' {
            $Actual = Test-CharacterClass -Value '_' -Flags ConnectorPunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"_"';
        }

        It 'Other Punctuation characters should return false' {
            foreach ($Value in ('-', '¡', '„', '«', '»', '(', '₎', ')', '־')) {
                $Actual = Test-CharacterClass -Value $Value -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"_" should return false' {
            $Actual = Test-CharacterClass -Value '_' -IsNot -Flags ConnectorPunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"_"';
        }

        It 'Other Punctuation characters should return true' {
            foreach ($Value in ('-', '¡', '„', '«', '»', '(', '₎', ')', '־')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags NonAsciiConnectorPunctuation' {
    Context 'IsNot.Present = $false' {
        # It 'ConnectorPunctuation characters should return false' {
        #     foreach ($Value in ()) {
        #         $Actual = Test-CharacterClass -Value $Value -Flags ConnectorPunctuation -ErrorAction Stop;
        #         $Actual | Should -BeFalse -Because "`"$Value"`";
        #     }
        # }

        It 'Other Punctuation characters should return false' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', '(', '₎', ')', '־')) {
                $Actual = Test-CharacterClass -Value $Value -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        # It 'ConnectorPunctuation characters should return true' {
        #     foreach ($Value in ()) {
        #         $Actual = Test-CharacterClass -Value $Value -IsNot -Flags ConnectorPunctuation -ErrorAction Stop;
        #         $Actual | Should -BeTrue -Because "`"$Value"`";
        #     }
        # }

        It 'Other Punctuation characters should return true' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', '(', '₎', ')', '־')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags ConnectorPunctuation' {
    Context 'IsNot.Present = $false' {
        It '"_" should return true' {
            $Actual = Test-CharacterClass -Value '_' -Flags ConnectorPunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"_"';
        }

        It 'Other Punctuation characters should return false' {
            foreach ($Value in ('-', '¡', '„', '«', '»', '(', '₎', ')', '־')) {
                $Actual = Test-CharacterClass -Value $Value -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"_" should return false' {
            $Actual = Test-CharacterClass -Value '_' -IsNot -Flags ConnectorPunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"_"';
        }

        It 'Other Punctuation characters should return true' {
            foreach ($Value in ('-', '¡', '„', '«', '»', '(', '₎', ')', '־')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags ConnectorPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags DashPunctuation' {
    Context 'IsNot.Present = $false' {
        It '"-" should return false' {
            $Actual = Test-CharacterClass -Value '-' -Flags DashPunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"-"';
        }

        It '"־" should return false' {
            $Actual = Test-CharacterClass -Value '־' -Flags DashPunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"־"';
        }

        It 'Other Punctuation characters should return false' {
            foreach ($Value in ('_', '¡', '„', '«', '»', '(', '₎', ')')) {
                $Actual = Test-CharacterClass -Value $Value -Flags DashPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags DashPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"-" should return true' {
            $Actual = Test-CharacterClass -Value '-' -IsNot -Flags DashPunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"-"';
        }

        It '"־" should return true' {
            $Actual = Test-CharacterClass -Value '־' -IsNot -Flags DashPunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"־"';
        }

        It 'Other Punctuation characters should return true' {
            foreach ($Value in ('_', '¡', '„', '«', '»', '(', '₎', ')')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags DashPunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags DashPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }
}

Describe 'Test-CharacterClass -Flags NonAsciiDashPunctuation' {
    Context 'IsNot.Present = $false' {
        It '"־" should return true' {
            $Actual = Test-CharacterClass -Value '־' -Flags NonAsciiDashPunctuation -ErrorAction Stop;
            $Actual | Should -BeTrue -Because '"־"';
        }

        It 'Other Punctuation characters should return false' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', '(', '₎', ')')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiDashPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return false' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -Flags NonAsciiDashPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }

    Context 'IsNot.Present = $true' {
        It '"־" should return false' {
            $Actual = Test-CharacterClass -Value '־' -IsNot -Flags NonAsciiDashPunctuation -ErrorAction Stop;
            $Actual | Should -BeFalse -Because '"־"';
        }

        It 'Other Punctuation characters should return true' {
            foreach ($Value in ('_', '-', '¡', '„', '«', '»', '(', '₎', ')')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags NonAsciiDashPunctuation -ErrorAction Stop;
                $Actual | Should -BeTrue -Because "`"$Value"`";
            }
        }

        It 'Non-Punctuation characters should return true' {
            foreach ($Value in ("`t", "`n", ' ', '^', '˅', '+', '±', '|', '¦', '$', '£', '0', '¼', '2', '7', '9', 'A', 'À', 'æ', 'Ⅵ', 'z', 'ˮ', 'ǂ', 'µ')) {
                $Actual = Test-CharacterClass -Value $Value -IsNot -Flags NonAsciiDashPunctuation -ErrorAction Stop;
                $Actual | Should -BeFalse -Because "`"$Value"`";
            }
        }
    }
}

# Describe 'Test-CharacterClass -Flags AsciiDashPunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags OtherPunctuation' {
#     # [System.Globalization.UnicodeCategory]::OtherPunctuation
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags OtherNonAsciiPunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and -not [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags OtherAsciiPunctuation' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags EnclosingMark' {
#     # [System.Globalization.UnicodeCategory]::EnclosingMark
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags Surrogate' {
#     # [char]::IsSurrogate($args[0])
#     # [System.Globalization.UnicodeCategory]::Surrogate
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags LowSurrogate' {
#     # [char]::IsLowSurrogate($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags HighSurrogate' {
#     # [char]::IsHighSurrogate($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags Format' {
#     # [System.Globalization.UnicodeCategory]::Format
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags WhiteSpace' {
#     # [char]::IsWhiteSpace($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags Control' {
#     # [char]::IsControl($args[0])
#     # [System.Globalization.UnicodeCategory]::Control
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags Ascii' {
#     # [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags NonSpacingMark' {
#     # [System.Globalization.UnicodeCategory]::NonSpacingMark
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags NonAsciiControl' {
#     # [char]::IsControl($args[0]) -and -not [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags Separator' {
#     # [char]::IsSeparator($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags ParagraphSeparator' {
#     # [System.Globalization.UnicodeCategory]::ParagraphSeparator
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags LineSeparator' {
#     # [System.Globalization.UnicodeCategory]::LineSeparator
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags SpaceSeparator' {
#     # [System.Globalization.UnicodeCategory]::SpaceSeparator
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags NonAsciiSpaceSeparator' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and -not [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags AsciiSpaceSeparator' {
#     # [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags SpacingCombiningMark' {
#     # [System.Globalization.UnicodeCategory]::SpacingCombiningMark
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags AsciiControl' {
#     # [char]::IsControl($args[0]) -and [char]::IsAscii($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags OtherNumber' {
#     # [System.Globalization.UnicodeCategory]::OtherNumber
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags LetterNumber' {
#     # [System.Globalization.UnicodeCategory]::LetterNumber
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags AsciiHexDigit' {
#     # [char]::IsAsciiHexDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags AsciiHexDigitLower' {
#     # [char]::IsAsciiHexDigitLower($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags HexDigitLetterLower' {
#     # [char]::IsAsciiHexDigitLower($args[0]) -and -not [char]::IsAsciiDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags AsciiHexDigitUpper' {
#     # [char]::IsAsciiHexDigitUpper($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags HexDigitLetterUpper' {
#     # [char]::IsAsciiHexDigitUpper($args[0]) -and -not [char]::IsAsciiDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags NonHexAsciiUppercaseLetter' {
#     # [char]::IsAsciiLetterUpper($args[0]) -and -not [char]::IsAsciiHexDigitUpper($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags Number' {
#     # [char]::IsNumber($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags DecimalDigitNumber' {
#     # [System.Globalization.UnicodeCategory]::DecimalDigitNumber
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags Digit' {
#     # [char]::IsDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags NonAsciiDecimalDigit' {
#     # [char]::IsDigit($args[0]) -and -not [char]::IsAsciiDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags AsciiDigit' {
#     # [char]::IsAsciiDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags NonOctalDecimalDigit' {
#     # $args[0] -eq '8' -or $args[0] -eq '9'
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags OctalDigitNumber' {
#     # $args[0] -ge '0' -and $args[0] -lt '8'
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags NonBinaryOctalDigit' {
#     # $args[0] -gt '1' -and $args[0] -lt '8'
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags NonDigitNumber' {
#     # [char]::IsNumber($args[0]) -and -not [char]::IsDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags AsciiLetterUpper' {
#     # [char]::IsAsciiLetterUpper($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags NonAsciiUppercaseLetter' {
#     # [char]::IsUpper($args[0]) -and -not [char]::IsAsciiLetterUpper($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags UppercaseLetter' {
#     # [char]::IsUpper($args[0])
#     # [System.Globalization.UnicodeCategory]::UppercaseLetter
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags LetterOrDigit' {
#     # [char]::IsLetterOrDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags AsciiLetterOrDigit' {
#     # [char]::IsAsciiLetterOrDigit($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags Letter' {
#     # [char]::IsLetter($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags OtherLetter' {
#     # [System.Globalization.UnicodeCategory]::OtherLetter
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags ModifierLetter' {
#     # [System.Globalization.UnicodeCategory]::ModifierLetter
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags TitlecaseLetter' {
#     # [System.Globalization.UnicodeCategory]::TitlecaseLetter
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags AsciiLetter' {
#     # [char]::IsAsciiLetter($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags LowercaseLetter' {
#     # [char]::IsLower($args[0])
#     # [System.Globalization.UnicodeCategory]::LowercaseLetter
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags NonAsciiLowercaseLetter' {
#     # [char]::IsLower($args[0]) -and -not [char]::IsAsciiLetterLower($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags AsciiLetterLower' {
#     # [char]::IsAsciiLetterLower($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags NonHexAsciiLowercaseLetter' {
#     # [char]::IsAsciiLetterLower($args[0]) -and -not [char]::IsAsciiHexDigitLower($args[0])
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags PrivateUse' {
#     # [System.Globalization.UnicodeCategory]::PrivateUse
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }

# Describe 'Test-CharacterClass -Flags OtherNotAssigned' {
#     # [System.Globalization.UnicodeCategory]::OtherNotAssigned
#     Context 'IsNot.Present = $false' {
#     }

#     Context 'IsNot.Present = $true' {
#     }
# }