Import-Module -Name 'Erwine.Leonard.T.XmlUtility';

$SchemaPath = 'C:\Users\lerwi\GitHub\PowerShell-Modules-p2\src\XmlUtility\Schemas\developerManaged.xsd';

$XmlReader = [System.Xml.XmlReader]::Create($SchemaPath);
$ValidationEvents = @();
$XmlSchema = [System.Xml.Schema.XmlSchema]::Read($XmlReader, {
    Param([object]$Sender, [System.Xml.Schema.ValidationEventArgs]$e)
    $ValidationEvents = $ValidationEvents + @($e);
});
$XmlSchemaSet = New-Object -TypeName 'System.Xml.Schema.XmlSchemaSet';
$XmlSchemaSet.Add($XmlSchema);
$XmlSchemaSet.add_ValidationEventHandler{
    Param([object]$Sender, [System.Xml.Schema.ValidationEventArgs]$e)
    $ValidationEvents = $ValidationEvents + @($e);
};
$XmlSchemaSet.Compile();
$XmlSchemaSet.GlobalElements.Names;
$QName = $XmlSchemaSet.GlobalElements.Names | Where-Object { $_.Name -eq 'command' -and $_.Namespace -eq 'http://schemas.microsoft.com/maml/dev/command/2004/10' };
$CommandElement = $XmlSchemaSet.GlobalElements[$QName];
$CommandElement | Get-Member;
