<xsl:stylesheet exclude-result-prefixes="msxsl" version="1.0" xmlns:msxsl="urn:schemas-microsoft.com:xslt" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="text" />
  <xsl:template match="/ManifestData">
    <xsl:text><![CDATA[TOPIC
    about_]]></xsl:text>
<xsl:value-of select="@Name" />
<xsl:text><![CDATA[

SHORT DESCRIPTION
    ]]></xsl:text>
<xsl:value-of select="@Description" />
<xsl:text><![CDATA[

LONG DESCRIPTION
    ]]></xsl:text>
<xsl:value-of select="LongDescription" />
<xsl:text><![CDATA[

EXPORTED COMMANDS
    Following is a list of commands exported by this module:]]></xsl:text>
    <xsl:apply-templates select="FunctionsToExport/Function" mode="ExportedCommands" />
        <xsl:text><![CDATA[
SEE ALSO
]]></xsl:text>
    <xsl:apply-templates select="FunctionsToExport/Function" mode="SeeAlso" />
  </xsl:template>

  <xsl:template match="Function" mode="ExportedCommands">
    <xsl:text><![CDATA[

    ]]></xsl:text>
    <xsl:value-of select="@Name" />
    <xsl:text><![CDATA[
        ]]></xsl:text>
    <xsl:value-of select="." />
    <xsl:text><![CDATA[
]]></xsl:text>
  </xsl:template>

  <xsl:template match="AssemblyReference">
    <xsl:text><![CDATA[    ]]></xsl:text>
    <xsl:value-of select="@Name" />
    <xsl:text><![CDATA[
]]></xsl:text>
  </xsl:template>
</xsl:stylesheet>