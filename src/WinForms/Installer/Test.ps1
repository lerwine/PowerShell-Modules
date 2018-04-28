Import-Module -Name:'WinForms'

$options = @('one', 'two', 'three')
$result = $options | Get-SelectionFromGUI -Title:'My FOrm' -HeadingText:'MY Heading Test';
$result.TerminatingAction;
$result.SelectedValues