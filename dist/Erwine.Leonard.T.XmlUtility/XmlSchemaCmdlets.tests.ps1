Import-Module -Name ($PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.XmlUtility.psd1') -ErrorAction Stop;

<#
Import-Module Pester
#>

Describe 'Test loading XML schema' {
    It 'Get-ModuleSchemaFiles should return multiple files' {
        $Files = @(Get-ModuleSchemaFiles);
        $Expected = @(  
            'base.xsd',
            'baseConditional.xsd',
            'block.xsd',
            'blockCommon.xsd',
            'blockSoftware.xsd',
            'command.xsd',
            'conditionSet.xsd',
            'developer.xsd',
            'developerCommand.rld',
            'developerCommand.xsd',
            'developerDscResource.xsd',
            'developerManaged.xsd',
            'developerManagedClass.xsd',
            'developerManagedConstructor.xsd',
            'developerManagedDelegate.xsd',
            'developerManagedEnumeration.xsd',
            'developerManagedEvent.xsd',
            'developerManagedField.xsd',
            'developerManagedInterface.xsd',
            'developerManagedMethod.xsd',
            'developerManagedNamespace.xsd',
            'developerManagedOperator.xsd',
            'developerManagedOverload.xsd',
            'developerManagedProperty.xsd',
            'developerManagedStructure.xsd',
            'developerReference.xsd',
            'developerStructure.xsd',
            'developerXaml.xsd',
            'endUser.xsd',
            'faq.xsd',
            'glossary.xsd',
            'helpItems.xsd',
            'hierarchy.xsd',
            'HTMLsymbol.ent',
            'inline.xsd',
            'inlineCommon.xsd',
            'inlineSoftware.xsd',
            'inlineUi.xsd',
            'ITPro.xsd',
            'Maml_HTML_Style.xsl',
            'Maml_HTML.xsl',
            'Maml.rld',
            'Maml.tbr',
            'Maml.xsd',
            'Maml.xsx',
            'ManagedDeveloper.xsd',
            'ManagedDeveloperStructure.xsd',
            'ProviderHelp.xsd',
            'README.md',
            'shellExecute.xsd',
            'soap-encoding.xsd',
            'soap-envelope.xsd',
            'soap11encoding.xsd',
            'soap11envelope.xsd',
            'structure.xsd',
            'structureGlossary.xsd',
            'structureList.xsd',
            'structureProcedure.xsd',
            'structureTable.xsd',
            'structureTaskExecution.xsd',
            'task.xsd',
            'troubleshooting.xsd',
            'TypeLibrary-array.xsd',
            'TypeLibrary-binary.xsd',
            'TypeLibrary-list.xsd',
            'TypeLibrary-math.xsd',
            'TypeLibrary-nn-array.xsd',
            'TypeLibrary-nn-binary.xsd',
            'TypeLibrary-nn-list.xsd',
            'TypeLibrary-nn-math.xsd',
            'TypeLibrary-nn-quantity.xsd',
            'TypeLibrary-nn-text.xsd',
            'TypeLibrary-quantity.xsd',
            'TypeLibrary-text.xsd',
            'TypeLibrary.xsd',
            'WindowsPhoneSynthesis-core.xsd',
            'WindowsPhoneSynthesis.xsd',
            'wsdl.xsd',
            'wsdl11html.xsd',
            'wsdl11mime.xsd',
            'wsdl11soap12.xsd',
            'xhtml-lat1.ent',
            'xhtml-special.ent',
            'xhtml-symbol.ent',
            'xml.xsd',
            'XMLSchema-instance.xsd',
            'XmlSchema.xsd',
            'Xslt.xsd'
        );
        $Files.Count | Should -Be $Expected.Count;
        foreach ($En in $Expected) {
            $Item = $Files | Where-Object { $_.Name -eq $En } | Select-Object -First 1;
            $Item | Should -Not -Be $null -Because "$En Item";
            $Item.PSPath | Should -Not -Be $null -Because "$En PSPath";
            (Test-Path -LiteralPath $Item.PSPath -PathType Leaf) | Should -BeTrue -Because "$En Exists";
        }
    }
};