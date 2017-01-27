<xsl:stylesheet exclude-result-prefixes="msxsl" version="1.0" xmlns:msxsl="urn:schemas-microsoft.com:xslt" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="text" />
  <xsl:param name="PowerShellVersion" />
  <xsl:param name="CLRVersion" />
  <xsl:param name="DotNetFrameworkVersion" />
  <xsl:param name="ConditionalCompilationSymbols" />
  <xsl:template match="/ManifestData">
    <xsl:text><![CDATA[@{

# Script module or binary module file associated with this manifest.
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="@PowerShellVersion='2.0' or $PowerShellVersion='2.0'">
        <xsl:text><![CDATA[ModuleToProcess]]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[RootModule]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[ = ']]></xsl:text>
    <xsl:value-of select="@Name" />
    <xsl:text><![CDATA[.psm1'

# Version number of this module.
ModuleVersion = ']]></xsl:text>
    <xsl:value-of select="@Version" />
    <xsl:text><![CDATA['

# ID used to uniquely identify this module
GUID = ']]></xsl:text>
    <xsl:value-of select="@Guid" />
    <xsl:text><![CDATA['

# Author of this module
Author = ']]></xsl:text>
    <xsl:value-of select="@Author" />
    <xsl:text><![CDATA['

# Company or vendor of this module
CompanyName = ']]></xsl:text>
    <xsl:value-of select="@CompanyName" />
    <xsl:text><![CDATA['

# Copyright statement for this module
Copyright = ']]></xsl:text>
    <xsl:value-of select="@Copyright" />
    <xsl:text><![CDATA['

# Description of the functionality provided by this module
Description = ']]></xsl:text>
    <xsl:value-of select="@Description" />
    <xsl:text><![CDATA['

# Minimum version of the Windows PowerShell engine required by this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="not(normalize-space(string(@PowerShellVersion))='')">
        <xsl:text><![CDATA[PowerShellVersion = ']]></xsl:text>
        <xsl:value-of select="normalize-space(string(@PowerShellVersion))" />
      </xsl:when>
      <xsl:when test="not(normalize-space(string($PowerShellVersion))='')">
        <xsl:text><![CDATA[PowerShellVersion = ']]></xsl:text>
        <xsl:value-of select="normalize-space(string($PowerShellVersion))" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[# PowerShellVersion = ']]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA['

# Name of the Windows PowerShell host required by this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="not(normalize-space(string(PowerShellHost/@Name))='')">
        <xsl:text><![CDATA[PowerShellHostName = ']]></xsl:text>
        <xsl:value-of select="normalize-space(string(PowerShellHost/@Name))" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[# PowerShellHostName = ']]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA['

# Minimum version of the Windows PowerShell host required by this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="not(normalize-space(string(PowerShellHost/@Version))='')">
        <xsl:text><![CDATA[PowerShellHostVersion = ']]></xsl:text>
        <xsl:value-of select="normalize-space(string(PowerShellHost/@Version))" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[# PowerShellHostVersion = ']]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA['

# Minimum version of Microsoft .NET Framework required by this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="not(normalize-space(string(@DotNetFrameworkVersion))='')">
        <xsl:text><![CDATA[DotNetFrameworkVersion = ']]></xsl:text>
        <xsl:value-of select="normalize-space(string(@DotNetFrameworkVersion))" />
      </xsl:when>
      <xsl:when test="not(normalize-space(string($DotNetFrameworkVersion))='')">
        <xsl:text><![CDATA[DotNetFrameworkVersion = ']]></xsl:text>
        <xsl:value-of select="normalize-space(string($DotNetFrameworkVersion))" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[# DotNetFrameworkVersion = ']]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA['

# Minimum version of the common language runtime (CLR) required by this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="not(normalize-space(string(@CLRVersion))='')">
        <xsl:text><![CDATA[CLRVersion = ']]></xsl:text>
        <xsl:value-of select="normalize-space(string(@CLRVersion))" />
      </xsl:when>
      <xsl:when test="not(normalize-space(string($CLRVersion))='')">
        <xsl:text><![CDATA[CLRVersion = ']]></xsl:text>
        <xsl:value-of select="normalize-space(string($CLRVersion))" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[# CLRVersion = ']]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA['

# Processor architecture (None, X86, Amd64) required by this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="not(normalize-space(string(@ProcessorArchitecture))='')">
        <xsl:text><![CDATA[ProcessorArchitecture = ']]></xsl:text>
        <xsl:value-of select="normalize-space(string(@ProcessorArchitecture))" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[# ProcessorArchitecture = ']]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA['

# Modules that must be imported into the global environment prior to importing this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="count(RequiredModules/Name)=0">
        <xsl:text><![CDATA[# RequiredModules = @()]]></xsl:text>
      </xsl:when>
      <xsl:when test="count(RequiredModules/Name)=1 and normalize-space(string(RequiredModules/Name))='*'">
        <xsl:text><![CDATA[RequiredModules = '*']]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[RequiredModules = @(]]></xsl:text>
        <xsl:apply-templates select="RequiredModules/Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# Assemblies that must be loaded prior to importing this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="count(RequiredAssemblies/Name)=0">
        <xsl:text><![CDATA[# RequiredAssemblies = @()]]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[RequiredAssemblies = @(]]></xsl:text>
        <xsl:apply-templates select="RequiredAssemblies/Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="count(ScriptsToProcess/Name)=0">
        <xsl:text><![CDATA[# ScriptsToProcess = @()]]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[ScriptsToProcess = @(]]></xsl:text>
        <xsl:apply-templates select="ScriptsToProcess/Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# Type files (.ps1xml) to be loaded when importing this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="count(TypesToProcess/Name)=0">
        <xsl:text><![CDATA[# TypesToProcess = @()]]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[TypesToProcess = @(]]></xsl:text>
        <xsl:apply-templates select="TypesToProcess/Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# Format files (.ps1xml) to be loaded when importing this module
]]></xsl:text>
      <xsl:choose>
      <xsl:when test="count(FormatsToProcess/Name)=0">
        <xsl:text><![CDATA[# FormatsToProcess = @()]]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[FormatsToProcess = @(]]></xsl:text>
        <xsl:apply-templates select="FormatsToProcess/Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="count(NestedModules/Name)=0">
        <xsl:text><![CDATA[# NestedModules = @()]]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[NestedModules = @(]]></xsl:text>
        <xsl:apply-templates select="NestedModules/Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# Functions to export from this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="count(FunctionsToExport/Function/@Name)=0">
        <xsl:text><![CDATA[# FunctionsToExport = @()]]></xsl:text>
      </xsl:when>
      <xsl:when test="count(FunctionsToExport/Function/@Name)=1 and normalize-space(string(FunctionsToExport/Function/@Name))='*'">
        <xsl:text><![CDATA[FunctionsToExport = '*']]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[FunctionsToExport = @(]]></xsl:text>
        <xsl:apply-templates select="FunctionsToExport/Function/@Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# Cmdlets to export from this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="count(CmdletsToExport/Name)=0">
        <xsl:text><![CDATA[# CmdletsToExport = @()]]></xsl:text>
      </xsl:when>
      <xsl:when test="count(CmdletsToExport/Name)=1 and normalize-space(string(CmdletsToExport/Name))='*'">
        <xsl:text><![CDATA[CmdletsToExport = '*']]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[CmdletsToExport = @(]]></xsl:text>
        <xsl:apply-templates select="CmdletsToExport/Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# Variables to export from this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="count(VariablesToExport/Name)=0">
        <xsl:text><![CDATA[# VariablesToExport = @()]]></xsl:text>
      </xsl:when>
      <xsl:when test="count(VariablesToExport/Name)=1 and normalize-space(string(VariablesToExport/Name))='*'">
        <xsl:text><![CDATA[VariablesToExport = '*']]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[VariablesToExport = @(]]></xsl:text>
        <xsl:apply-templates select="VariablesToExport/Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# Aliases to export from this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="count(AliasesToExport/Name)=0">
        <xsl:text><![CDATA[# AliasesToExport = @()]]></xsl:text>
      </xsl:when>
      <xsl:when test="count(AliasesToExport/Name)=1 and normalize-space(string(AliasesToExport/Name))='*'">
        <xsl:text><![CDATA[AliasesToExport = '*']]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[AliasesToExport = @(]]></xsl:text>
        <xsl:apply-templates select="AliasesToExport/Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# List of all modules packaged with this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="count(ModuleList/Name)=0">
        <xsl:text><![CDATA[# ModuleList = @()]]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[ModuleList = @(]]></xsl:text>
        <xsl:apply-templates select="ModuleList/Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# List of all files packaged with this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="count(FileList/Name)=0">
        <xsl:text><![CDATA[# FileList = @()]]></xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[FileList = @(]]></xsl:text>
        <xsl:apply-templates select="FileList/Name" />
        <xsl:text><![CDATA[)]]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA[

# Private data to pass to the module specified in RootModule/ModuleToProcess
PrivateData = @{]]></xsl:text>
    <xsl:if test="not(count(FileList/Name[@Compile='true'])=0)">
      <xsl:text><![CDATA[
	CompilerOptions = @{
		IncludeDebugInformation = $]]></xsl:text>
      <xsl:choose>
        <xsl:when test="normalize-space(string(CompilerOptions/@IncludeDebugInformation))=''">
          <xsl:text>false</xsl:text>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="CompilerOptions/@IncludeDebugInformation" />
        </xsl:otherwise>
      </xsl:choose>
        <xsl:text><![CDATA[;
		ConditionalCompilationSymbols = ']]></xsl:text>
      <xsl:choose>
        <xsl:when test="normalize-space(string(CompilerOptions/@IncludeDebugInformation))='true'">
          <xsl:text>DEBUG</xsl:text>
          <xsl:if test="not(normalize-space(string($ConditionalCompilationSymbols))='')">
            <xsl:text>;</xsl:text>
            <xsl:value-of select="normalize-space(string($ConditionalCompilationSymbols))" />
          </xsl:if>
        </xsl:when>
        <xsl:when test="not(normalize-space(string($ConditionalCompilationSymbols))='')">
          <xsl:value-of select="normalize-space(string($ConditionalCompilationSymbols))" />
        </xsl:when>
      </xsl:choose>
      <xsl:text><![CDATA[';
        CustomTypeSourceFiles = @(]]></xsl:text>
      <xsl:apply-templates select="FileList/Name[@Compile='true']" />
      <xsl:text><![CDATA[);
        AssemblyReferences = @(]]></xsl:text>
      <xsl:apply-templates select="CompilerOptions/AssemblyReference" />
      <xsl:text><![CDATA[);
	};]]></xsl:text>
    </xsl:if>
    <xsl:apply-templates select="PrivateDataCode/Property" />
    <xsl:text><![CDATA[
}

# HelpInfo URI of this module
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="not(normalize-space(string(@HelpInfoURI))='')">
        <xsl:text><![CDATA[HelpInfoURI = ']]></xsl:text>
        <xsl:value-of select="normalize-space(string(@HelpInfoURI))" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[# HelpInfoURI = ']]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA['

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
]]></xsl:text>
    <xsl:choose>
      <xsl:when test="not(normalize-space(string(@DefaultCommandPrefix))='')">
        <xsl:text><![CDATA[DefaultCommandPrefix = ']]></xsl:text>
        <xsl:value-of select="normalize-space(string(@DefaultCommandPrefix))" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:text><![CDATA[# DefaultCommandPrefix = ']]></xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text><![CDATA['

}
]]></xsl:text>
  </xsl:template>

  <xsl:template match="Name">
    <xsl:if test="not(position()=1)">
      <xsl:text>, </xsl:text>
    </xsl:if>
    <xsl:text><![CDATA[']]></xsl:text>
    <xsl:value-of select="normalize-space(string())" />
    <xsl:text><![CDATA[']]></xsl:text>
  </xsl:template>

  <xsl:template match="AssemblyReference">
    <xsl:if test="not(position()=1)">
      <xsl:text>, </xsl:text>
    </xsl:if>
    <xsl:text><![CDATA[']]></xsl:text>
    <xsl:value-of select="normalize-space(string(.))" />
    <xsl:text><![CDATA[']]></xsl:text>
  </xsl:template>

  <xsl:template match="Property">
    <xsl:text><![CDATA[
	]]></xsl:text>
    <xsl:value-of select="@Name" />
    <xsl:text><![CDATA[ = ]]></xsl:text>
    <xsl:value-of select="." />
    <xsl:text><![CDATA[;]]></xsl:text>
  </xsl:template>
</xsl:stylesheet>