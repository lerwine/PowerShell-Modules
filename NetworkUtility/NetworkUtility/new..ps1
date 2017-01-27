@('System.Data', 'System.DirectoryServices.AccountManagement', 'System.Web', 'System.Web.Services') | ForEach-Object { Add-Type -AssemblyName $_ -ErrorAction Stop }
Add-Type -Path (('MimeUtility.cs', 'FormEncoder.cs') | ForEach-Object { $PSScriptRoot | Join-Path -ChildPath $_ }) `
	-ReferencedAssemblies 'System.Management.Automation', 'System.Web.Services', 'System.Xml';

$Script:Regex = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
    EndingNewline = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '(\r\n?|\n)$', ([System.Text.RegularExpressions.RegexOptions]::Compiled);
};

Function New-IPAddress {
    <#
        .SYNOPSIS
			Create IP address object.
         
        .DESCRIPTION
			Returns an object which represents an IP address.
        
        .OUTPUTS
			System.Net.IPAddress. An object which represents an IP address.
        
        .LINK
            Test-IPAddressIsLoopback
        
        .LINK
            ConvertTo-IPv6
        
        .LINK
            ConvertTo-IPv4
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.ipaddress.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'Bytes')]
    [OutputType([System.Net.IPAddress])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'NewAddress')]
        # The long value of the IP address.
        [long]$NewAddress,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Parse')]
        # A string that contains an IP address in dotted-quad notation for IPv4 and in colon-hexadecimal notation for IPv6. 
        [string]$IpString,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Bytes')]
        # The byte array value of the IP address.
        [byte[]]$Bytes,
        
        [Parameter(Position = 1, ParameterSetName = 'Bytes')]
        # The long value of the scope identifier.
        [long]$Scopeid
    )

    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'NewAdress' { New-Object -TypeName 'System.Net.IPAddress' -ArgumentList $NewAddress; break; }
            'IpString' { [System.Net.IPAddress]::Parse($IpString); break; }
            default {
                if ($PSBoundParameters.ContainsKey('Scopeid')) {
                    New-Object -TypeName 'System.Net.IPAddress' -ArgumentList (,$Bytes, $Scopeid);
                } else {
                    New-Object -TypeName 'System.Net.IPAddress' -ArgumentList (,$Bytes);
                }
                break;
            }
        }
    }
}

Function Test-IPAddressIsLoopback {
    <#
        .SYNOPSIS
			Determines whether IP address is the loopback address.
         
        .DESCRIPTION
			Returns true if the IP address is the loopback address, otherwise returns false.
        
        .OUTPUTS
			System.Boolean. True if the IP address is the loopback address, otherwise returns false.
        
        .LINK
            New-IPAddress
        
        .LINK
            ConvertTo-IPv6
        
        .LINK
            ConvertTo-IPv4
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.ipaddress.isloopback.aspx
    #>
    [CmdletBinding()]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The IP address to test.
        [System.Net.IPAddress]$Address
    )

    Process { [System.Net.IPAddress]::IsLoopback($Address) }
}

Function ConvertTo-IPv6 {
    <#
        .SYNOPSIS
			Convert IP address to IPv6.
         
        .DESCRIPTION
			Maps the IPAddress object to an IPv6 address.
        
        .OUTPUTS
			System.Net.IPAddress. IP address converted to IPv6.
        
        .LINK
            New-IPAddress
        
        .LINK
            ConvertTo-IPv4
        
        .LINK
            Test-IPAddressIsLoopback
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.ipaddress.maptoipv6.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Net.IPAddress])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The IP address to be converted.
        [System.Net.IPAddress]$IPAddress
    )

    Process { $IPAddress.MapToIPv6() }
}

Function ConvertTo-IPv4 {
    <#
        .SYNOPSIS
			Convert IP address to IPv4.
         
        .DESCRIPTION
			Maps the IPAddress object to an IPv4 address.
        
        .OUTPUTS
			System.Net.IPAddress. IP address converted to IPv4.
        
        .LINK
            New-IPAddress
        
        .LINK
            ConvertTo-IPv6
        
        .LINK
            Test-IPAddressIsLoopback
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.ipaddress.maptoipv6.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Net.IPAddress])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The IP address to be converted.
        [System.Net.IPAddress]$IPAddress
    )

    Process { $IPAddress.MapToIPv4() }
}

Function Resolve-DnsHost {
    <#
        .SYNOPSIS
			Resolve DNS host.
         
        .DESCRIPTION
			Resolves a DNS host name or IP address to an IPHostEntry instance.
        
        .OUTPUTS
			System.Net.IPHostEntry. An IPHostEntry instance that contains address information about the host specified in hostName.
        
        .LINK
            Get-DnsHostEntry
        
        .LINK
            Get-DnsHostAddresses
        
        .LINK
            New-IPAddress
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.dns.resolve.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Net.IPHostEntry])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Name')]
        [Alias('HostName', 'Host', 'HostNameOrAddress', 'ComputerName')]
        # A DNS-style host name or IP address
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Address')]
        [System.Net.IPAddress]$Address
    )

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'Name') {
            [System.Net.Dns]::Resolve($Name);
        } else {
            [System.Net.Dns]::Resolve($Address);
        }
    }
}

Function Get-DnsHostEntry {
    <#
        .SYNOPSIS
			Resolve DNS host.
         
        .DESCRIPTION
			Resolves a DNS host name or IP address to an IPHostEntry instance.
        
        .OUTPUTS
			System.Net.IPHostEntry. An IPHostEntry instance that contains address information about the host specified in hostName.
        
        .LINK
            Get-DnsHostAddresses
        
        .LINK
            New-IPAddress
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.dns.gethostentry.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'Name')]
    [OutputType([System.Net.IPHostEntry])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Name')]
        [Alias('HostName', 'Host', 'HostNameOrAddress', 'ComputerName')]
        # The host name or IP address to resolve.
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Address')]
        # An IP address to resolve.
        [System.Net.IPAddress]$Address
    )

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'Name') {
            [System.Net.Dns]::GetHostEntry($Name);
        } else {
            [System.Net.Dns]::GetHostEntry($Address);
        }
    }
}

Function Get-DnsHostAddresses {
    <#
        .SYNOPSIS
			Get IP addresses for host.
         
        .DESCRIPTION
			Returns the Internet Protocol (IP) addresses for the specified host.
        
        .OUTPUTS
			System.Net.IPAddress[]. An array of type IPAddress that holds the IP addresses for the host that is specified by the hostNameOrAddress parameter.
        
        .LINK
            Get-DnsHostEntry
        
        .LINK
            New-IPAddress
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.dns.gethostentry.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Net.IPAddress[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [Alias('HostName', 'Host', 'Name', 'Address', 'ComputerName')]
        # The host name or IP address to resolve.
        [string]$HostNameOrAddress
    )

    Process { [System.Net.Dns]::GetHostAddresses($HostNameOrAddress) }
}

Function Get-NetworkInterfaces {
    <#
        .SYNOPSIS
			Get network interfaces.
         
        .DESCRIPTION
			Returns objects that describe the network interfaces on the local computer.
        
        .OUTPUTS
			System.Net.NetworkInformation.NetworkInterface[]. A NetworkInterface array that contains objects that describe the available network interfaces, or an empty array if no interfaces are detected.
        
        .LINK
            Get-NetworkInterfaceIPProperties
        
        .LINK
            Get-NetworkInterfacePhysicalAddress
        
        .LINK
            Test-NetworkInterfaceSupports
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.networkinformation.networkinterface.getallnetworkinterfaces.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Net.NetworkInformation.NetworkInterface[]])]
    Param()

    [System.Net.NetworkInformation.NetworkInterface]::GetAllNetworkInterfaces();
}

Function Get-NetworkInterfaceIPProperties {
    <#
        .SYNOPSIS
			Get network interfaces.
         
        .DESCRIPTION
			Returns objects that describe the network interfaces on the local computer.
        
        .OUTPUTS
			System.Net.NetworkInformation.NetworkInterface[]. A NetworkInterface array that contains objects that describe the available network interfaces, or an empty array if no interfaces are detected.
        
        .LINK
            Get-NetworkInterfaces
        
        .LINK
            Get-NetworkInterfacePhysicalAddress
        
        .LINK
            Test-NetworkInterfaceSupports
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.networkinformation.networkinterface.getipproperties.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Net.NetworkInformation.IPInterfaceProperties])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # A Network interface.
        [System.Net.NetworkInformation.NetworkInterface]$NetworkInterface
    )

    Process { $NetworkInterface.GetIPProperties() }
}

<# Not supported in .NET 2.0
Function Get-NetworkInterfaceIPStatistics {
    [CmdletBinding()]
    [OutputType([System.Net.NetworkInformation.IPInterfaceStatistics])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Net.NetworkInformation.NetworkInterface]$NetworkInterface
    )

    Process { $NetworkInterface.GetIPStatistics() }
}
#>

Function Get-NetworkInterfacePhysicalAddress {
    <#
        .SYNOPSIS
			Get network physical address.
         
        .DESCRIPTION
			Returns the Media Access Control (MAC) or physical address for this adapter.
        
        .OUTPUTS
			System.Net.NetworkInformation.PhysicalAddress. A PhysicalAddress object that contains the physical address.
        
        .LINK
            Get-NetworkInterfaces
        
        .LINK
            Get-NetworkInterfaceIPProperties
        
        .LINK
            Test-NetworkInterfaceSupports
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.networkinformation.networkinterface.getphysicaladdress.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Net.NetworkInformation.PhysicalAddress])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # A Network interface.
        [System.Net.NetworkInformation.NetworkInterface]$NetworkInterface
    )

    Process { $NetworkInterface.GetPhysicalAddress() }
}

Function Test-NetworkInterfaceSupports {
    <#
        .SYNOPSIS
			Test whether the interface supports the specified protocol.
         
        .DESCRIPTION
			Returns a Boolean value that indicates whether the interface supports the specified protocol.
        
        .OUTPUTS
			System.Boolean. True if the specified protocol is supported; otherwise, false.
        
        .LINK
            Get-NetworkInterfaces
        
        .LINK
            Get-NetworkInterfaceIPProperties
        
        .LINK
            Get-NetworkInterfacePhysicalAddress
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.networkinformation.networkinterface.supports.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'IPv4')]
    [OutputType([System.Net.NetworkInformation.PhysicalAddress])]
    Param(
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # A Network interface to test.
        [System.Net.NetworkInformation.NetworkInterface]$NetworkInterface,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'IPv4')]
        # Tests whether network supports IPv4.
        [switch]$IPv4,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'IPv6')]
        # Tests whether network supports IPv6.
        [switch]$IPv6
    )

    Process {
        if ($IPv4) {
            $NetworkInterface.Supports([System.Net.NetworkInformation.NetworkInterfaceComponent]::IPv4);
        } else {
            $NetworkInterface.Supports([System.Net.NetworkInformation.NetworkInterfaceComponent]::IPv6);
        }
    }
}

Function New-PingOptions {
    <#
        .SYNOPSIS
			Create Ping options object.
         
        .DESCRIPTION
			Returns an object which is used to control how Ping data packets are transmitted.
        
        .OUTPUTS
			System.Net.NetworkInformation.PingOptions. An object which is used to control how Ping data packets are transmitted.
        
        .LINK
            Send-Ping
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.networkinformation.pingoptions.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Net.NetworkInformation.PingOptions])]
    Param(
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = $true)]
        # An Int32 value greater than zero that specifies the number of times that the Ping data packets can be forwarded.
        [int]$Ttl,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        # True to prevent data sent to the remote host from being fragmented; otherwise, false
        [switch]$DontFragment
    )
    
    Process {
        if ($PSBoundParameters.ContainsKey('Ttl')) {
            New-Object -TypeName 'System.Net.NetworkInformation.PingOptions' -ArgumentList $Ttl, $DontFragment;
        } else {
            $PingOptions = New-Object -TypeName 'System.Net.NetworkInformation.PingOptions';
            $PingOptions.DontFragment = $DontFragment;
            $PingOptions | Write-Output;
        }
    }
}

# 
Function Send-Ping {
    <#
        .SYNOPSIS
			Send ICMP Ping to host.
         
        .DESCRIPTION
			Attempts to send an Internet Control Message Protocol (ICMP) echo message to the specified host, and receive a corresponding ICMP echo reply message from that host.
        
        .OUTPUTS
			System.Net.NetworkInformation.PingReply. ICMP echo reply message from host.
        
        .LINK
            New-PingOptions
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.networkinformation.ping.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.networkinformation.pingreply.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'HostNameOptionParam')]
    [OutputType([System.Net.NetworkInformation.PingReply])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Name')]
        [Alias('HostName', 'Host', 'HostNameOrAddress', 'ComputerName')]
        # A String that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.
        [string]$Name,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Address')]
        # IP address that is the destination for the ICMP echo message.
        [System.Net.IPAddress]$Address,

        [ValidateRange(1, 2147483647)]
        # Number of ICMP pings to send.
        [int]$Count = 3,
        
        [ValidateRange(0, 2147483647)]
        # An Int32 value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message
        [int]$Timeout = 5000,
        
        # A Byte array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
        [byte[]]$Buffer,

        # A PingOptions object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.
        [System.Net.NetworkInformation.PingOptions]$Options,
        
        # Ping object to use
        [System.Net.NetworkInformation.Ping]$Pinger
    )
    
    Begin {
        if ($PSBoundParameters.ContainsKey('Pinger')) {
            $Ping = $Pinger;
        } else {
            $Ping = New-Object -TypeName 'System.Net.NetworkInformation.Ping';
        }
        if ($PSBoundParameters.ContainsKey('Options') -and -not $PSBoundParameters.ContainsKey('Buffer')) {
            $Buffer = [System.Text.Encoding]::ASCII.GetBytes('1234567890ABCDEFGHIJKLMNOPQRSTUV');
        }
    }
    
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'Name') {
            if ($PSBoundParameters.ContainsKey('Options')) {
                for ($i = 0; $i -lt $Count; $i++) { $Ping.Send($Name, $Timeout, $Buffer, $Options) }
            } else {
                if ($PSBoundParameters.ContainsKey('Buffer')) {
                    for ($i = 0; $i -lt $Count; $i++) { $Ping.Send($Name, $Timeout, $Buffer) }
                } else {
                    if ($PSBoundParameters.ContainsKey('Timeout')) {
                        for ($i = 0; $i -lt $Count; $i++) { $Ping.Send($Name, $Timeout) }
                    } else {
                        for ($i = 0; $i -lt $Count; $i++) { $Ping.Send($Name) }
                    }
                }
            }
        } else {
            if ($PSBoundParameters.ContainsKey('Options')) {
                for ($i = 0; $i -lt $Count; $i++) { $Ping.Send($Address, $Timeout, $Buffer, $Options) }
            } else {
                if ($PSBoundParameters.ContainsKey('Buffer')) {
                    for ($i = 0; $i -lt $Count; $i++) { $Ping.Send($Address, $Timeout, $Buffer) }
                } else {
                    if ($PSBoundParameters.ContainsKey('Timeout')) {
                        for ($i = 0; $i -lt $Count; $i++) { $Ping.Send($Address, $Timeout) }
                    } else {
                        for ($i = 0; $i -lt $Count; $i++) { $Ping.Send($Address) }
                    }
                }
            }
        }
    }
    
    End { if (-not $PSBoundParameters.ContainsKey('Pinger')) { $Ping.Dispose() } }
}

Function Trace-Route {
    <#
        .SYNOPSIS
			Trace route to host.
         
        .DESCRIPTION
			Attempts to send an Internet Control Message Protocol (ICMP) echo messages to computers in the route to the specified host, and receives corresponding ICMP echo reply messages from the hosts.
        
        .OUTPUTS
			System.Net.NetworkInformation.PingReply[]. ICMP echo reply messages from hosts.
        
        .LINK
            New-PingOptions
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.networkinformation.ping.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.networkinformation.pingreply.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'HostName')]
    [OutputType([System.Net.NetworkInformation.PingReply[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Name')]
        [Alias('HostName', 'Host', 'HostNameOrAddress', 'ComputerName')]
        # A String that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.
        [string]$Name,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Address')]
        # IP address that is the destination for the ICMP echo message.
        [System.Net.IPAddress]$Address,

        # An Int32 value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message
        [ValidateRange(0, 2147483647)]
        [int]$Timeout = 5000,
        
        # Maximum number of forwards in the trace.
        [ValidateRange(1, 2147483647)]
        [int]$MaxTtl = 128,
        
        # Ping object to use
        [System.Net.NetworkInformation.Ping]$Pinger
    )
    
    Begin {
        if ($PSBoundParameters.ContainsKey('Pinger')) {
            $Ping = $Pinger;
        } else {
            $Ping = New-Object -TypeName 'System.Net.NetworkInformation.Ping';
        }
    }
    
    Process {
        if ($PSBoundParameters.ContainsKey('Name')) {
            $Splat = @{ Name = $Name; Count = 1; };
        } else {
            $Splat = @{ Address = $Address; Count = 1; };
        }
        $Splat.Options = New-PingOptions -Ttl 1 -DontFragment;
        if ($PSBoundParameters.ContainsKey('Timeout')) { $Splat.Timeout = $Timeout }
        $Splat.Pinger = $Ping;
        while ($Splat.Options.Ttl -le $MaxTtl) {
            $PingReply = Send-Ping @splat;
            if ($PingReply.Status -eq [System.Net.NetworkInformation.IPStatus]::Success) {
                $PingReply | Write-Output;
                break;
            }
            $Splat.Options.Ttl++;
            if ($PingReply.Address -ne $null) {
                if ($PSBoundParameters.ContainsKey('Timeout')) {
                    Send-Ping -Address $PingReply.Address -Count 1 -Options $Splat.Options -Timeout $Timeout -Pinger $Ping;
                } else {
                    Send-Ping -Address $PingReply.Address -Count 1 -Options $Splat.Options -Pinger $Ping;
                }
            } else {
                $PingReply | Write-Output;
            }
            if ($PingReply.Status -ne [System.Net.NetworkInformation.IPStatus]::TtlExpired -and $PingReply.Status -ne [System.Net.NetworkInformation.IPStatus]::TimedOut) { break; }
        }
    }
    
    End { if (-not $PSBoundParameters.ContainsKey('Pinger')) { $Ping.Dispose() } }
}

Function Get-TraceRouteStatus {
    [CmdletBinding()]
    [OutputType([System.Net.NetworkInformation.IPStatus])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Net.NetworkInformation.PingReply[]]$PingReply
    )

    Begin {
        $LastTtl = $null;
        $LastStatus = [System.Net.NetworkInformation.IPStatus]::TtlExpired;
    }

    Process {
        if ($LastTtl -ne $null -and $PingReply.Options.Ttl -le $LastTtl) { $LastStatus | Write-Output }
        $LastTtl = $PingReply.Options.Ttl;
        $LastStatus = $PingReply.Status;
    }

    End { $LastStatus | Write-Output }
}

Function New-SqlConnection {
    <#
        .SYNOPSIS
			Get SQL database connection.
         
        .DESCRIPTION
			Returns an object which represents a connection to a SQL Server database.
        
        .OUTPUTS
			System.Data.SqlClient.SqlConnection. An object which represents a connection to a SQL Server database.
        
        .LINK
            New-SqlCommand
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Data.SqlClient.SqlConnection])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The connection used to open the SQL Server database.
        [string]$ConnectionString,
        
        # Indicates that the connection should not be actually opened.
        [switch]$DoNotOpen
    )

    $SqlConnection = New-Object -TypeName '' -ArgumentList $ConnectionString
    if ($DoNotOpen) {
        $SqlConnection | Write-Output;
    } else {
        try {
            $SqlConnection.Open();
            $SqlConnection | Write-Output;
        } catch {
            $SqlConnection.Dispose();
            throw;
        }
    }
}

Function New-SqlCommand {
    <#
        .SYNOPSIS
			Create new SQL command.
         
        .DESCRIPTION
			Creates and returns a SqlCommand object associated with the SqlConnection.
        
        .OUTPUTS
			System.Data.SqlClient.SqlConnection. An object which represents a connection to a SQL Server database.
        
        .LINK
            New-SqlConnection
        
        .LINK
            New-SqlParameter
        
        .LINK
            Read-SqlCommand
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlcommand.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.createcommand.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'Optional')]
    [OutputType([System.Data.SqlClient.SqlCommand])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The SqlConnection to be used by the new SqlCommand.
        [System.Data.SqlClient.SqlConnection]$SqlConnection,
        
        [Parameter(Position = 1, ParameterSetName = 'Optional')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'CommandType')]
        # The Transact-SQL statement, table name or stored procedure to execute at the data source.
        [string]$CommandText,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'CommandType')]
        # Indicates how the CommandText parameter is to be interpreted.
        [System.Data.CommandType]$CommandType = [System.Data.CommandType]::Text
    )

    $SqlCommand = $ConnectionString.CreateCommand();
    if ($PSBoundParameters.ContainsKey('CommandText')) {
        try {
            $SqlCommand.CommandText = $CommandText;
            $SqlCommand.CommandType = $CommandType;
            $SqlCommand | Write-Output;
        } catch {
            $SqlCommand.Dispose();
            throw;
        }
    } else {
        $SqlCommand | Write-Output;
    }
}

Function New-SqlParameter {
    <#
        .SYNOPSIS
			Create new SQL command.
         
        .DESCRIPTION
			Creates and returns a SqlCommand object associated with the SqlConnection.
        
        .OUTPUTS
			System.Data.SqlClient.SqlCommand. A SqlCommand object associated with the SqlConnection.
        
        .LINK
            New-SqlCommand
        
        .LINK
            Read-SqlCommand
        
        .LINK
            New-SqlConnection
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlparameter.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlcommand.createparameter.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'NotNull')]
    [OutputType([System.Data.SqlClient.SqlParameter])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The command to which the parameter is to be added.
        [System.Data.SqlClient.SqlCommand]$SqlCommand,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [Alias('ParameterName')]
        # The name of the SqlParameter.
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'NotNull')]
        [Alias('ParameterName')]
        [AllowEmptyString()]
        # The value of the parameter.
        [object]$Value,
        
        [Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'NotNull')]
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Null')]
        # The type of the parameter.
        [System.Data.SqlDbType]$DbType,
        
        # Indicates whether the parameter is input-only, output-only, bidirectional, or a stored procedure return value parameter.
        [System.Data.ParameterDirection]$Direction = [System.Data.ParameterDirection]::Input,
        
        # Name of the source column mapped to the DataSet and used for loading or returning the Value.
        [string]$SourceColumn,
        
        # Maximum size, in bytes, of the data within the column.
        [int]$Size,
        
        # Maximum number of digits used to represent the Value property.
        [byte]$Precision,
        
        # Number of decimal places to which Value is resolved.
        [byte]$Scale,
        
        [Parameter(ParameterSetName = 'NotNull')]
        # Indicates whether the parameter accepts null values.
        [switch]$IsNullable,
        
        [Parameter(ParameterSetName = 'Null')]
        # Indicates that the parameter is to contain a null value.
        [switch]$NullValue,
        
        # Do not actually add the parameter to the SqlCommand.
        [switch]$DoNotAdd
    )

    $SqlParameter = $SqlCommand.CreateParameter();
    $SqlParameter.ParameterName = $Name;
    $SqlParameter.SqlDbType = $DbType;
    $SqlParameter.Direction = $Direction;
    $SqlParameter.IsNullable = $IsNullable.IsPresent -or $NullValue.IsPresent;
    if ($PSBoundParameters.ContainsKey('SourceColumn')) { $SqlParameter.SourceColumn = $SourceColumn }
    if ($PSBoundParameters.ContainsKey('Size')) { $SqlParameter.Size = $Size }
    if ($PSBoundParameters.ContainsKey('Precision')) { $SqlParameter.Precision = $Precision }
    if ($PSBoundParameters.ContainsKey('Scale')) { $SqlParameter.Scale = $Scale }
    if ($NullValue) {
        $SqlParameter.Value = [System.DBNull]::Value;
    } else {
        $SqlParameter.Value = $Value;
    }
    if ($DoNotAdd) {
        $SqlParameter | Write-Output;
    } else {
        $SqlCommand.Parameters.Add($SqlParameter) | Write-Output;
    }
}

Function Read-SqlData {
    <#
        .SYNOPSIS
			Get data from an SQL data reader.
         
        .DESCRIPTION
			Returns data from the SqlDataReader.
        
        .OUTPUTS
			System.Management.Automation.PSObject[]. The data read from the SqlDataReader.
        
        .LINK
            Read-SqlCommand
        
        .LINK
            New-SqlCommand
        
        .LINK
            New-SqlParameter
        
        .LINK
            New-SqlConnection
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldatareader.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Management.Automation.PSObject[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The SQL Data Reader to be read from.
        [System.Data.SqlClient.SqlDataReader]$SqlDataReader
    )
    
    if ($SqlDataReader.HasRows) {
        while ($SqlDataReader.Read()) {
            $Properties = @{};
            for ($i = 0; $i -lt $SqlDataReader.FieldCount; $i++) {
                $name = $SqlDatReader.GetName($i);
                if ($SqlDataReader.IsDBNull($i)) {
                    $Properties[$name] = $null;
                } else {
                    $Properties[$name] = $SqlDataReader.Getvalue($i);
                }
            }
        }
    }
    New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
}

Function Read-SqlCommand {
    <#
        .SYNOPSIS
			Get data from an SQL command.
         
        .DESCRIPTION
			Sends the CommandText to the Connection and builds a SqlDataReader, returning the result set.
        
        .OUTPUTS
			System.Management.Automation.PSObject[]. The data read from the SqlDataReader.
        
        .LINK
            Read-SqlData
        
        .LINK
            New-SqlCommand
        
        .LINK
            New-SqlParameter
        
        .LINK
            New-SqlConnection
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlcommand.executereader.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldatareader.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'CommandText')]
    [OutputType([System.Management.Automation.PSObject[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'SqlCommand')]
        # The command to be executed.
        [System.Data.SqlClient.SqlCommand]$SqlCommand,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'CommandText')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'CommandType')]
        # The connection for the SQL command.
        [System.Data.SqlClient.SqlConnection]$SqlConnection,
        
        [Parameter(Position = 1, ParameterSetName = 'CommandText')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'CommandType')]
        # The Transact-SQL statement, table name or stored procedure to execute at the data source.
        [string]$CommandText,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'CommandType')]
        # Indicates how the CommandText parameter is to be interpreted.
        [System.Data.CommandType]$CommandType
    )
    
    if ($PSCmdlet.ParameterSetName -eq 'SqlCommand') {
        $SqlDataReader = $SqlCommand.ExecuteReader();
        try {
            Read-SqlData -SqlDataReader $SqlDataReader;
        } catch {
            throw;
        } finally {
            $SqlDataReader.Dispose();
        }
    } else {
        if ($PSCmdlet.ParameterSetName -eq 'CommandType') {
            $SqlCommand = New-SqlCommand -SqlConnection $SqlConnection -CommandText $CommandText -CommandType $CommandType;
        } else {
            $SqlCommand = New-SqlCommand -SqlConnection $SqlConnection -CommandText $CommandText;
        }
        try {
            Read-SqlCommand -SqlCommand $SqlCommand;
        } catch {
            throw;
        } finally {
            $SqlCommand.Dispose();
        }
    }
}

Function Get-SqlTableInfo {
    <#
        .SYNOPSIS
			Get information about tables.
         
        .DESCRIPTION
			Returns a list of objects that can be queried in the current environment. This means any table or view, except synonym objects.
        
        .OUTPUTS
			System.Management.Automation.PSObject[]. Information about data tables and views.
        
        .LINK
            Get-SqlTableColumnInfo
        
        .LINK
            https://msdn.microsoft.com/en-us/library/ms186250.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Management.Automation.PSObject[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [System.Data.SqlClient.SqlConnection]$SqlConnection,
        
        # Table/View name to filter by.
        [string]$Name,
        
        [Alias('Owner')]
        # Name of the schema (owner) to filter by.
        [string]$Schema,
        
        [Alias('Database')]
        # Name of the object qualifier to filter by.
        [string]$Qualifier,
        
        [ValidateSet('TABLE', 'SYSTEMTABLE', 'VIEW')]
        # Table types to be returned.
        [string[]]$Type
    )
    
    $SqlCommand = New-SqlCommand -SqlConnection $SqlConnection -CommandText = 'sp_tables' -CommandType StoredProcedure;
    try {
        if ($PSBoundParameters.ContainsKey('Name')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'table_name' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Name -Size 384 | Out-Null;
        }
        if ($PSBoundParameters.ContainsKey('Schema')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'table_owner' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Schema -Size 384 | Out-Null;
        }
        if ($PSBoundParameters.ContainsKey('Qualifier')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'table_qualifier' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Qualifier | Out-Null;
        }
        if ($PSBoundParameters.ContainsKey('Type')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'table_type' -DbType ([System.Data.SqlDbType]::NVarChar) -Value (($Type | Select-Object -Unique) -join ', ') -Size 100 | Out-Null;
        }
        Read-SqlCommand -SqlCommand $SqlCommand;
    } catch {
        throw;
    } finally {
        $SqlCommand.Dispose();
    }
}

Function Get-SqlTableColumnInfo {
    <#
        .SYNOPSIS
			Get table or view column information.
         
        .DESCRIPTION
			Returns column information for the specified objects that can be queried in the current environment.
        
        .OUTPUTS
			System.Management.Automation.PSObject[]. Table and/or View columns.
        
        .LINK
            Get-SqlTableInfo
        
        .LINK
            https://msdn.microsoft.com/en-us/library/ms176077.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Management.Automation.PSObject[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The SQL connection representing the environment to use.
        [System.Data.SqlClient.SqlConnection]$SqlConnection,
        
        # Name of the object that is used to return catalog information.
        [string]$TableName,
        
        [Alias('Owner')]
        # Name of the schema (owner) to filter by.
        [string]$Schema,
        
        [Alias('Database')]
        # Name of the object qualifier to filter by.
        [string]$Qualifier,
        
        [Alias('Column')]
        # Name of a single column to return.
        [string]$Name
    )
    
    $SqlCommand = New-SqlCommand -SqlConnection $SqlConnection -CommandText = 'sp_columns' -CommandType StoredProcedure;
    New-SqlParameter -SqlCommand $SqlCommand -Name 'table_name' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $TableName -Size 384 | Out-Null;
    try {
        if ($PSBoundParameters.ContainsKey('Schema')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'table_owner' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Schema -Size 384 | Out-Null;
        }
        if ($PSBoundParameters.ContainsKey('Qualifier')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'table_qualifier' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Qualifier | Out-Null;
        }
        if ($PSBoundParameters.ContainsKey('Name')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'column_name' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Name -Size 384 | Out-Null;
        }
        Read-SqlCommand -SqlCommand $SqlCommand;
    } catch {
        throw;
    } finally {
        $SqlCommand.Dispose();
    }
    
}

Function Get-SqlStoredProcedureInfo {
    <#
        .SYNOPSIS
			Get information about stored procedures.
         
        .DESCRIPTION
			Returns a list of stored procedures in the current environment.
        
        .OUTPUTS
			System.Management.Automation.PSObject[]. Information about stored procedures.
        
        .LINK
            Get-SqlStoredProcedureColumnInfo
        
        .LINK
            https://msdn.microsoft.com/en-us/library/ms190504.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Management.Automation.PSObject[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [System.Data.SqlClient.SqlConnection]$SqlConnection,
        
        # Name of the stored procedure to filter by.
        [string]$Name,
        
        [Alias('Owner')]
        # Name of the schema (owner) to filter by.
        [string]$Schema,
        
        [Alias('Database')]
        # Name of the procedure qualifier.
        [string]$Qualifier
    )
    
    $SqlCommand = New-SqlCommand -SqlConnection $SqlConnection -CommandText = 'sp_stored_procedures' -CommandType StoredProcedure;
    try {
        if ($PSBoundParameters.ContainsKey('Name')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'sp_name' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Name -Size 390 | Out-Null;
        }
        if ($PSBoundParameters.ContainsKey('Schema')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'sp_owner' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Schema -Size 384 | Out-Null;
        }
        if ($PSBoundParameters.ContainsKey('Qualifier')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'sp_qualifier' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Qualifier | Out-Null;
        }
        Read-SqlCommand -SqlCommand $SqlCommand;
    } catch {
        throw;
    } finally {
        $SqlCommand.Dispose();
    }
    
}

Function Get-SqlStoredProcedureColumnInfo {
    <#
        .SYNOPSIS
			Get stored procedure column information.
         
        .DESCRIPTION
			Returns column information for a single stored procedure or user-defined function in the current environment.
        
        .OUTPUTS
			System.Management.Automation.PSObject[]. Stored procedure columns.
        
        .LINK
            Get-SqlStoredProcedureInfo
        
        .LINK
            https://msdn.microsoft.com/en-us/library/ms182705.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Management.Automation.PSObject[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The SQL connection representing the environment to use.
        [System.Data.SqlClient.SqlConnection]$SqlConnection,
        
        # Name of the procedure used to return catalog information.
        [string]$StoredProcedure,
        
        [Alias('Owner')]
        # Name of the schema (owner) to filter by.
        [string]$Schema,
        
        [Alias('Database')]
        # Name of the owner of the procedure to filter by.
        [string]$Qualifier,
        
        [Alias('Column')]
        # Name of a single column to return.
        [string]$Name
    )
    
    $SqlCommand = New-SqlCommand -SqlConnection $SqlConnection -CommandText = 'sp_sproc_columns' -CommandType StoredProcedure;
    New-SqlParameter -SqlCommand $SqlCommand -Name 'procedure_name' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $StoredProcedure -Size 390 | Out-Null;
    try {
        if ($PSBoundParameters.ContainsKey('Schema')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'procedure_owner' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Schema -Size 384 | Out-Null;
        }
        if ($PSBoundParameters.ContainsKey('Qualifier')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'procedure_qualifier' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Qualifier | Out-Null;
        }
        if ($PSBoundParameters.ContainsKey('Name')) {
            New-SqlParameter -SqlCommand $SqlCommand -Name 'column_name' -DbType ([System.Data.SqlDbType]::NVarChar) -Value $Name -Size 384 | Out-Null;
        }
        Read-SqlCommand -SqlCommand $SqlCommand;
    } catch {
        throw;
    } finally {
        $SqlCommand.Dispose();
    }
    
}

Function Test-ContentType {
    <#
        .SYNOPSIS
			Check content type for validity.
         
        .DESCRIPTION
			Determines whether a content type matches specified criteria.
        
        .OUTPUTS
			System.Boolean. Indicates whether the content type matched the specified criteria.
        
        .LINK
            Get-WebResponse
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.mime.contenttype.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'Validate')]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Validate')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'String_String')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'String_ContentType')]
        [AllowNull()]
        [AllowEmptyString()]
        # MIME protocol Content Type strings to be tested.
        [string[]]$InputString,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ContentType_String')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ContentType_ContentType')]
        [AllowNull()]
        # MIME protocol Content-Type headers to be tested.
        [System.Net.Mime.ContentType[]]$InputObject,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'String_String')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'ContentType_String')]
        [ValidateScript({ Test-ContentType -InputString $_ })]
        # Content type that is expected.
        [string]$Expected,

        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'String_ContentType')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'ContentType_ContentType')]
        # MIME protocol Content-Type header to be tested.
        [System.Net.Mime.ContentType]$ContentType,
        
        [Parameter(ParameterSetName = 'String_String')]
        [Parameter(ParameterSetName = 'String_ContentType')]
        [Parameter(ParameterSetName = 'ContentType_String')]
        [Parameter(ParameterSetName = 'ContentType_ContentType')]
        # Only the media type is to be validated.
        [switch]$MediaTypeOnly,
        
        [Parameter(ParameterSetName = 'Validate')]
        # Allow empty media types.
        [switch]$AllowEmpty
    )

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'Validate') {
            if ([System.String]::IsNullOrEmpty($InputString)) {
                $AllowEmpty.IsPresent | Write-Output;
            } else {
                try { $c = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $InputString } catch { $c = $null }
                ($c -ne $null) | Write-Output;
            }
        } else {
            if ($PSBoundParameters.ContainsKey('Expected')) {
                $ContentType = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $Expected;
            }
            $success = $true;
            if ($PSBoundParameters.ContainsKey('InputString')) {
                if ($InputString -eq $null -or $InputString.Length -eq 0) {
                    $success = $false;
                } else {$ContentType = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $str
                    if ($MediaTypeOnly) {
                        foreach ($str in $InputString) {
                            if ([System.String]::IsNullOrEmpty($str)) { $success = $false; break; }
                            $c = $null;
                            try { $c = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $str } catch { }
                            if ($c -eq $null -or -not (Test-ContentType -InputObject $c -ContentType $ContentType -MediaTypeOnly)) { $success = $false; break; }
                        }
                    } else {
                        foreach ($str in $InputString) {
                            if ([System.String]::IsNullOrEmpty($str)) { $success = $false; break; }
                            $c = $null;
                            try { $c = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $str } catch { }
                            if ($c -eq $null -or -not (Test-ContentType -InputObject $c -ContentType $ContentType)) { $success = $false; break; }
                        }
                    }
                }
            } else {
                if ($InputObject -eq $null -or $InputObject.Length -eq 0) {
                    $success = $false;
                } else {
                    if ($MediaTypeOnly) {
                        foreach ($c in $InputObject) {
                            if ($c -eq $null -or $c.MediaType -ne $ContentType.MediaType) { $success = $false; break; }
                        }
                    } else {
                        foreach ($c in $InputObject) {
                            if ($c -eq $null -or $c.MediaType -ne $ContentType.MediaType) { $success = $false; break; }
                            if ([System.String]::IsNullOrEmpty($ContentType.CharSet)) {
                                if (-not [System.String]::IsNullOrEmpty($c.CharSet)) { $success = $false; break; }
                            } else {
                                if ([System.String]::IsNullOrEmpty($c.CharSet) -or $c.CharSet -ne $ContentType.CharSet) { $success = $false; break; }
                            }
                        }
                    }
                }
            }
        
            $success | Write-Output;
        }
    }
}

Function New-WebRequest {
    <#
        .SYNOPSIS
			Create new web request object.
         
        .DESCRIPTION
			Creates an object which represents a request to a Uniform Resource Identifier (URI).
        
        .OUTPUTS
			System.Boolean. Indicates whether the content type matched the specified criteria.
        
        .LINK
            Get-WebResponse
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.webrequest.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.filewebrequest.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'PSCredential')]
    [OutputType([System.Net.WebRequest])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # URI of the Internet resource associated with the request.
        [System.Uri]$Uri,

        # Default cache policy for the request.
        [System.Net.Cache.RequestCacheLevel]$CachePolicy,

        # Protocol method to use in the request.
        [string]$Method,

        # Name of the connection group for the request.
        [string]$ConnectionGroupName,

        # Header name/value pairs to be associated with the request
        [Hashtable]$Headers,

        # Indicates whether to pre-authenticate the request.
        [bool]$PreAuthenticate,

        [ValidateRange(0, 2147483647)]
        # Length of time, in milliseconds, before the request times out.
        [int]$Timeout,

        # Indicates the level of authentication and impersonation used for the request.
        [System.Net.Security.AuthenticationLevel]$AuthenticationLevel,

        # Impersonation level for the request.
        [System.Security.Principal.TokenImpersonationLevel]$ImpersonationLevel,

        [Parameter(ParameterSetName = 'PSCredential')]
        # Credentials used for authenticating the request with the Internet resource.
        [System.Management.Automation.PSCredential]$PSCredential,

        [Parameter(Mandatory = $true, ParameterSetName = 'ICredentials')]
        # Network credentials used for authenticating the request with the Internet resource.
        [System.Net.ICredentials]$Credentials,

        [Parameter(Mandatory = $true, ParameterSetName = 'UseDefaultCredentials')]
        # Send default credentials with the request.
        [switch]$UseDefaultCredentials
    )

    Process {
        $WebRequest = [System.Net.WebRequest]::Create($Uri);
        if ($WebRequest -ne $null) {
            $WebRequest.UseDefaultCredentials = $UseDefaultCredentials;
            if ($PSBoundParameters.ContainsKey('CachePolicy')) { $WebRequest.CachePolicy = New-Object -TypeName 'System.Net.Cache.RequestCachePolicy' -ArgumentList $CachePolicy }
            if ($PSBoundParameters.ContainsKey('Method')) { $WebRequest.Method = $Method }
            if ($PSBoundParameters.ContainsKey('ConnectionGroupName')) { $WebRequest.ConnectionGroupName = $ConnectionGroupName }
            if ($PSBoundParameters.ContainsKey('PreAuthenticate')) { $WebRequest.PreAuthenticate = $PreAuthenticate }
            if ($PSBoundParameters.ContainsKey('Timeout')) { $WebRequest.Timeout = $Timeout }
            if ($PSBoundParameters.ContainsKey('AuthenticationLevel')) { $WebRequest.AuthenticationLevel = $AuthenticationLevel }
            if ($PSBoundParameters.ContainsKey('ImpersonationLevel')) { $WebRequest.ImpersonationLevel = $ImpersonationLevel }
            if ($PSBoundParameters.ContainsKey('PSCredential')) { $WebRequest.Credentials = $PSCredential.GetNetworkCredential() }
            if ($PSBoundParameters.ContainsKey('Credentials')) { $WebRequest.Credentials = $Credentials }

            if ($PSBoundParameters.ContainsKey('Headers') -and $Headers.Count -gt 0) {
                $Headers.Keys | ForEach-Object {
                    if ($_ -is [string]) { $Key = $_ } else { $Key = $Script:Regex.EndingNewline.Replace(($_ | Out-String), '') }
                    if ($Headers[$_] -eq $null) {
                        $WebRequest.Headers.Add($Key, '');
                    } else {
                        if ($Headers[$_] -is [string]) {
                            $WebRequest.Headers.Add($Key, $Headers[$_]);
                        } else {
                            $WebRequest.Headers.Add($Key, $Script:Regex.EndingNewline.Replace(($Headers[$_] | Out-String), ''));
                        }
                    }
                }
            }
            $WebRequest | Write-Output;
        }
    }
}

Function Read-FormUrlEncoded {
    <#
        .SYNOPSIS
			Read Url-Encoded form data.
         
        .DESCRIPTION
			Reads application/x-www-form-urlencoded data.
        
        .OUTPUTS
			System.Collections.Specialized.NameValueCollection. Key/Value pairs representing decoded data.
        
        .LINK
            Write-FormUrlEncoded
            
        .LINK
            Get-WebResponse
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.webresponse.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.textreader.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.collections.specialized.namevaluecollection.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'InputString')]
    [OutputType([System.Collections.Specialized.NameValueCollection])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'InputString')]
        [AllowEmptyString()]
        # String containing URL-Encoded form data.
        [string]$InputString,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'WebResponse')]
        # Web response containing URL-Encoded form data.
        [System.Net.WebResponse]$WebResponse,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'TextReader')]
        # Text Reader containing URL-Encoded form data.
        [System.IO.TextReader]$Reader,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Stream')]
        # Stream containing URL-Encoded form data.
        [System.IO.Stream]$Stream,

        [Parameter(Position = 1, ParameterSetName = 'Stream')]
        # Character encoding to use.
        [System.Text.Encoding]$Encoding
	)

	Process {
        switch ($PSCmdlet.ParameterSetName) {
            'WebResponse' {
                [NetworkUtilityCLR.FormEncoder]::Decode($WebResponse) | Write-Output;
                break;
            }
            'TextReader' {
                [NetworkUtilityCLR.FormEncoder]::Decode($Reader) | Write-Output;
                break;
            }
            default {
                if ($PSBoundParameters.ContainsKey('Encoding')) {
                    [NetworkUtilityCLR.FormEncoder]::Decode($Reader, $null) | Write-Output;
                } else {
                    [NetworkUtilityCLR.FormEncoder]::Decode($Reader, $Encoding) | Write-Output;
                }
                break;
            }
        }
    }
}

Function New-TextWriter {
    <#
        .SYNOPSIS
			Create text writer.
         
        .DESCRIPTION
			Create object to write text data.
        
        .OUTPUTS
			System.IO.TextWriter. Writer object to write text data.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.textwriter.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.streamwriter.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.stringwriter.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.web.httpwriter.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.web.ui.htmltextwriter.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.collections.specialized.namevaluecollection.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'StringWriter')]
    [OutputType([System.IO.TextWriter])]
    Param(
        [Parameter(Position = 0, ParameterSetName = 'StringWriter')]
        # Create text writer which writes text to a string builder.
        [System.Text.StringBuilder]$StringBuilder,
        
        [Parameter(Position = 1, ParameterSetName = 'StringWriter')]
        # Format provider to use with string builder.
        [System.IFormatProvider]$FormatProvider,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Stream')]
        # Stream which text writer will write to.
        [System.IO.Stream]$Stream,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Path')]
        # Path which text writer will write to.
        [string]$Path,
        
        [Parameter(Position = 1, ParameterSetName = 'Stream')]
        [Parameter(Position = 1, ParameterSetName = 'Path')]
        # Character encoding to use when writing text.
        [System.Text.Encoding]$Encoding = [System.Text.Encoding]::UTF8,
        
        [Parameter(Position = 2, ParameterSetName = 'Stream')]
        [Parameter(Position = 2, ParameterSetName = 'Path')]
        # The write buffer size, in bytes.
        [int]$BufferSize = 32767,

        [Parameter(Position = 3, ParameterSetName = 'Stream')]
        # Leave stream open after stream writer is disposed.
        [switch]$LeaveOpen,

        [Parameter(Position = 3, ParameterSetName = 'Path')]
        # Append text to file.
        [switch]$Append
    )

    switch ($PSCmdlet.ParameterSetName) {
        'Path' {
            if ($PSBoundParameters.ContainsKey('BufferSize')) {
                New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Append.IsPresent, $Encoding, $BufferSize;
            } else {
                if ($PSBoundParameters.ContainsKey('Encoding')) {
                    New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Append.IsPresent, $Encoding;
                } else {
                    if ($PSBoundParameters.ContainsKey('Append')) {
                        New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Append.IsPresent;
                    } else {
                        New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream;
                    }
                }
            }
            break;
        }
        'Stream' {
            if ($PSBoundParameters.ContainsKey('LeaveOpen')) {
                New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Encoding, $BufferSize, $LeaveOpen.IsPresent;
            } else {
                if ($PSBoundParameters.ContainsKey('BufferSize')) {
                    New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Encoding, $BufferSize;
                } else {
                    if ($PSBoundParameters.ContainsKey('Encoding')) {
                        New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Encoding;
                    } else {
                        New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream;
                    }
                }
            }
            break;
        }
        default {
            if ($PSBoundParameters.ContainsKey('StringBuilder')) {
                if ($PSBoundParameters.ContainsKey('FormatProvider')) {
                    New-Object -TypeName 'System.IO.StringWriter' -ArgumentList $StringBuilder, $FormatProvider;
                } else {
                    New-Object -TypeName 'System.IO.StringWriter' -ArgumentList $StringBuilder;
                }
            } else {
                if ($PSBoundParameters.ContainsKey('FormatProvider')) {
                    New-Object -TypeName 'System.IO.StringWriter' -ArgumentList $FormatProvider;
                } else {
                    New-Object -TypeName 'System.IO.StringWriter';
                }
            }
        }
    }
}

Function Write-FormUrlEncoded {
    <#
        .SYNOPSIS
			Write Url-Encoded form data.
         
        .DESCRIPTION
			Writes application/x-www-form-urlencoded data.
        
        .OUTPUTS
			System.Collections.Specialized.NameValueCollection. Key/Value pairs representing decoded data.
        
        .LINK
            Read-FormUrlEncoded
            
        .LINK
            New-WebRequest
            
        .LINK
            Get-WebResponse
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.collections.specialized.namevaluecollection.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.webrequest.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.textreader.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'KeyValue_OutString')]
    [OutputType([string], ParameterSetName = 'KeyValue_OutString')]
    [OutputType([string], ParameterSetName = 'Nvc_OutString')]
    [OutputType([string], ParameterSetName = 'Hashtable_OutString')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Nvc_OutString')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Nvc_Stream')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Nvc_TextWriter')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Nvc_WebRequest')]
        [AllowEmptyCollection()]
        # Key/Value pairs to be encoded.
        [System.Collections.Specialized.NameValueCollection]$NameValueCollection,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Hashtable_OutString')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Hashtable_Stream')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Hashtable_TextWriter')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Hashtable_WebRequest')]
        [AllowEmptyCollection()]
        # Key/Value pairs to be encoded.
        [Hashtable]$Data,
		
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_OutString')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_Stream')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_TextWriter')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_WebRequest')]
		[AllowEmptyString()]
        # Key to encoded and written.
        [object]$Key,
		
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_OutString')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_Stream')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_TextWriter')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'KeyValue_WebRequest')]
		[AllowNull()]
		[AllowEmptyString()]
        # Value to encoded and written.
        [object]$Value,
        
        [Parameter(ParameterSetName = 'Hashtable_Stream')]
        [Parameter(ParameterSetName = 'KeyValue_Stream')]
        [Parameter(ParameterSetName = 'Hashtable_WebRequest')]
        [Parameter(ParameterSetName = 'KeyValue_WebRequest')]
        # Character encoding to use.
        [System.Text.Encoding]$Encoding,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Hashtable_TextWriter')]
        [Parameter(Mandatory = $true, ParameterSetName = 'KeyValue_TextWriter')]
        # Text writer to write encoded data to.
        [System.IO.TextWriter]$Writer,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Hashtable_WebRequest')]
        [Parameter(Mandatory = $true, ParameterSetName = 'KeyValue_WebRequest')]
        [Alias('Request')]
        # Web Request to write encoded data to. This will also set the content type and length.
        [System.Net.WebRequest]$WebRequest,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Hashtable_Stream')]
        [Parameter(Mandatory = $true, ParameterSetName = 'KeyValue_Stream')]
        # Stream to write encoded data to.
        [System.IO.Stream]$Stream,
        
        [Parameter(ParameterSetName = 'KeyValue_OutString')]
        [Parameter(Mandatory = $true, ParameterSetName = 'Nvc_OutString')]
        [Parameter(Mandatory = $true, ParameterSetName = 'Hashtable_OutString')]
        # Return encoded data as a string.
        [switch]$ToString
	)

    Begin {
        if (-not $PSBoundParameters.ContainsKey('NameValueCollection')) {
            $NameValueCollection = New-Object -TypeName 'System.Collections.Specialized.NameValueCollection';
        }
        $AllItems =  @();
        ($InputType, $OutpuType) = $PSCmdlet.ParameterSetName.Split('_');
    }

	Process {
		if ($InputType -eq 'Hashtable') {
            foreach ($Key in $Data.Keys) {
                $Value = $Data[$Key];
                if ($Key -is [string]) {
                    $k = $Key;
                } else {
                    $k = $Key.ToString();
                }
                if ($Value -ne $null) {
                    $NameValueCollection.Add($k, $null);
                } else {
                    $Value = @($Value);
                    if ($Value.Count -eq 0) {
                        $NameValueCollection.Add($k, '');
                    } else {
                        $Value | ForEach-Object {
                            if ($_ -eq $null -or $_ -is [string]) {
                                $NameValueCollection.Add($k, $_);
                            } else {
                                $NameValueCollection.Add($k, $_.ToString());
                            }
                        }
                    }
                }
            }
        } else {
            if ($InputType -eq 'KeyValue') {
                if ($Key -is [string]) {
                    $k = $Key;
                } else {
                    $k = $Key.ToString();
                }
                if ($Value -eq $null) {
                    $NameValueCollection.Add($k, $null);
                } else {
                    $v = @($Value);
                    if ($v.Count -eq 0) {
                        $NameValueCollection.Add($k, '');
                    } else {
                        $v | ForEach-Object {
                            if ($_ -eq $null -or $_ -is [string]) {
                                $NameValueCollection.Add($k, $_);
                            } else {
                                $NameValueCollection.Add($k, $_.ToString());
                            }
                        }
                    }
                }
            }
		}
	}

    End {
        switch ($OutpuType) {
            'Stream' {
                [NetworkUtilityCLR.FormEncoder]::Encode($NameValueCollection, $Stream, $false, $Encoding);
                break;
            }
            'WebRequest' {
                [NetworkUtilityCLR.FormEncoder]::Encode($NameValueCollection, $WebRequest, $false, $Encoding);
            }
            'TextWriter' {
                [NetworkUtilityCLR.FormEncoder]::Encode($NameValueCollection, $Writer, $false);
            }
            default { # OutString
                [NetworkUtilityCLR.FormEncoder]::Encode($NameValueCollection, $false) | Write-Output;
                break;
            }
        }
    }
}

Function Initialize-WebRequestPostXml {
    <#
        .SYNOPSIS
			Initialze web request for posting xml data.
         
        .DESCRIPTION
			Initializes a System.Net.WebRequest object for an XML POST request.
        
        .OUTPUTS
			System.Net.WebResponse. If GetResponse is used, the response to the request.
        
        .OUTPUTS
			System.Net.WebRequest. If PassThru is used, then the WebRequest that was passed to command is returned.
        
        .LINK
            New-WebRequest
            
        .LINK
            Get-WebResponse
            
        .LINK
            Write-FormUrlEncoded
            
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.webresponse.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.webrequest.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlnode.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'GetRequest')]
    [OutputType([System.Net.WebResponse], ParameterSetName = 'GetRequest')]
    [OutputType([System.Net.WebRequest], ParameterSetName = 'GetResponse')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [Alias('Request')]
        # Web request to be initialized.
        [System.Net.WebRequest]$WebRequest,
        
        [Parameter(Mandatory = $true)]
        # XML data to send.
        [System.Xml.XmlNode]$XmlData,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'GetRequest')]
        # Returns System.Net.WebRequest that was initialized.
        [switch]$PassThru,
        
        [Parameter(ParameterSetName = 'GetResponse')]
        # Whether to allow redirection.
        [bool]$AllowRedirect,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'GetResponse')]
        # Get response after initializing.
        [switch]$GetResponse
    )
    
    Process {
        $WebRequest.Method = 'POST';
        
        $WebRequest.ContentType = 'text/xml;charset=utf-8';
        $Stream = $WebRequest.GetRequestStream();
        try {
            $XmlWriterSettings = New-Object -TypeName 'System.Xml.XmlWriterSettings' -Property @{
                Indent = $true;
                CloseOutput = $true;
                Encoding = [System.Text.Encoding]::UTF8;
                OmitXmlDeclaration = $true;
            };
            $XmlWriter = [System.Xml.XmlWriter]::Create($Stream, $XmlWriterSettings);
            try {
                $XmlData.WriteTo($XmlWriter);
            } catch {
                throw;
            } finally {
                $XmlWriter.Flush();
                $Stream.Flush();
                $WebRequest.ContentLength = [int]$Stream.Position;
                $XmlWriter.Close();
                $XmlWriter = $null;
                $Stream = $null;
            }
        } catch {
            throw;
        } finally {
            if ($Stream -ne $null) { $Stream.Dispose() }
        }
        if ($GetResponse) {
            if ($PSBoundParameters.ContainsKey('AllowRedirect')) { $WebRequest.AllowRedirect = $AllowRedirect }
            $WebRequest.GetResponse() | Write-Output;
        } else {
            if ($PassThru) { $WebRequest | Write-Output };
        }
    }
}

Function Get-WebResponse {
    <#
        .SYNOPSIS
			Get response for a web request.
         
        .DESCRIPTION
			returns a response to an Internet request.
        
        .OUTPUTS
			System.Net.WebResponse. A WebResponse containing the response to the Internet request.
        
        .LINK
            New-WebRequest
            
        .LINK
            Initialize-WebRequestPostXml
            
        .LINK
            Write-FormUrlEncoded
            
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.webresponse.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.net.webrequest.getresponse.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Management.Automation.PSObject])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [Alias('Request')]
        # Object representing an internet request.
        [System.Net.WebRequest]$WebRequest,
        
        # Whether to allow redirection.
        [bool]$AllowRedirect
    )
    
    Begin {
        $SuccessCodes = @(
            [System.Net.HttpStatusCode]::Continue,
            [System.Net.HttpStatusCode]::SwitchingProtocols,
            [System.Net.HttpStatusCode]::OK,
            [System.Net.HttpStatusCode]::Created,
            [System.Net.HttpStatusCode]::Accepted,
            [System.Net.HttpStatusCode]::PartialContent,
            [System.Net.HttpStatusCode]::MultipleChoices,
            [System.Net.HttpStatusCode]::MovedPermanently,
            [System.Net.HttpStatusCode]::Moved,
            [System.Net.HttpStatusCode]::Found,
            [System.Net.HttpStatusCode]::Redirect,
            [System.Net.HttpStatusCode]::NotModified,
            [System.Net.HttpStatusCode]::TemporaryRedirect,
            [System.Net.HttpStatusCode]::RedirectKeepVerb,
            [System.Net.FtpStatusCode]::OpeningData,
            [System.Net.FtpStatusCode]::CommandOK,
            [System.Net.FtpStatusCode]::DirectoryStatus,
            [System.Net.FtpStatusCode]::FileStatus,
            [System.Net.FtpStatusCode]::SystemType,
            [System.Net.FtpStatusCode]::SendUserCommand,
            [System.Net.FtpStatusCode]::ClosingControl,
            [System.Net.FtpStatusCode]::ClosingData,
            [System.Net.FtpStatusCode]::EnteringPassive,
            [System.Net.FtpStatusCode]::LoggedInProceed,
            [System.Net.FtpStatusCode]::FileActionOK,
            [System.Net.FtpStatusCode]::PathnameCreated,
            [System.Net.FtpStatusCode]::SendPasswordCommand,
            [System.Net.FtpStatusCode]::FileCommandPending
        );
    }
    
    Process {
        if ($PSBoundParameters.ContainsKey('AllowRedirect')) { $WebRequest.AllowAutoRedirect = $AllowRedirect }
        $Response = @{
            Request = $WebRequest;
            ErrorStatus = [System.Net.WebExceptionStatus]::UnknownError;
            StatusCode = [System.Management.Automation.PSInvocationState]::NotStarted;
            Success = $false;
            StatusDescription = 'Request not sent.';
        };
        try {
            $Response['Response'] = $WebRequest.GetResponse();
            $Response['ErrorStatus'] = [System.Net.WebExceptionStatus]::Success;
            if ($Response['Response'].StatusCode -ne $null) {
                $Response['StatusCode'] = $Response['Response'].StatusCode;
                $Response['Success'] = $SuccessCodes -contains $Response['StatusCode'];
            } else {
                $Response['StatusCode'] = [System.Management.Automation.PSInvocationState]::Completed;
                $Response['Success'] = $true;
            }
            if ($Response['Response'].StatusDescription -eq $null -or $Response['Response'].StatusDescription.Trim() -eq '') {
                $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
            } else {
                $Response['StatusDescription'] = $Response['Response'].StatusDescription;
            }
            if ($Response['Response'] -ne $null -and $Response['Response'].ContentType -ne $null -and $Response['Response'].ContentType.Trim().Length -gt 0) {
                try { $Response['ContentType'] = New-Object -TypeName 'System.Net.Mime.ContentType' -ArgumentList $Response['Response'].ContentType }
                catch { }
            }
        } catch [System.Net.WebException] {
            $Response['ErrorStatus'] = $_.Exception.Status;
            $Response['Response'] = $_.Exception.Response;
            $Response['Error'] = $_;
            if ($Response['Response'] -ne $null -and $Response['Response'].StatusCode -ne $null) {
                $Response['StatusCode'] = $Response['Response'].StatusCode;
            } else {
                $Response['StatusCode'] = [System.Management.Automation.PSInvocationState]::Failed;
            }
            if ($Response['Response'] -eq $null -or $Response['Response'].StatusDescription -eq $null -or $Response['Response'].StatusDescription.Trim() -eq '') {
                if ($_.ErrorDetails -eq $null -or $_.ErrorDetails.Message -eq $null -or $_.ErrorDetails.Message.Trim() -eq '') {
                    if ($_.Exception -eq $null -or $_.Exception.Message -eq $null -or $_.Exception.Message.Trim() -eq '') {
                        $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
                    } else {
                        $Response['StatusDescription'] = $_.Exception.Message;
                    }
                } else {
                    $Response['StatusDescription'] = $_.ErrorDetails.Message;
                }
            } else {
                $Response['StatusDescription'] = $Response['Response'].StatusDescription;
            }
        } catch [System.Net.Sockets.SocketException] {
            $Response['StatusCode'] = $_.SocketErrorCode;
            $Response['Error'] = $_;
            if ($_.ErrorDetails -eq $null -or $_.ErrorDetails.Message -eq $null -or $_.ErrorDetails.Message.Trim() -eq '') {
                if ($_.Exception -eq $null -or $_.Exception.Message -eq $null -or $_.Exception.Message.Trim() -eq '') {
                    $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
                } else {
                    $Response['StatusDescription'] = $_.Exception.Message;
                }
            } else {
                $Response['StatusDescription'] = $_.ErrorDetails.Message;
            }
        } catch {
            $Response['StatusCode'] = [System.Management.Automation.PSInvocationState]::Failed;
            if ($_.Exception -ne $null -and $_.Exception -is [System.Management.Automation.MethodException]) {
                if ($_.Exception.ErrorRecord -ne $null) {
                    $Response['Error'] = $_.Exception.ErrorRecord;
                } else {
                    $Response['Error'] = $_;
                }
                if ($_.Exception.InnerException -ne $null) {
                    $e = $_.Exception.InnerException;
                } else {
                    $e = $null;
                }
            } else {
                $e = $null;
            }
            if ($e -ne $null -and $e.Message -ne $null -and $e.Message.Trim() -ne '') {
                $Response['StatusDescription'] = $e.Message;
            } else {
                if ($Response['Error'].ErrorDetails -eq $null -or $Response['Error'].ErrorDetails.Message -eq $null -or $Response['Error'].ErrorDetails.Message.Trim() -eq '') {
                    if ($Response['Error'].Exception -eq $null -or $Response['Error'].Exception.Message -eq $null -or $Response['Error'].Exception.Message.Trim() -eq '') {
                        $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
                    } else {
                        $Response['StatusDescription'] = $Response['Error'].Exception.Message;
                    }
                } else {
                    $Response['StatusDescription'] = $Response['Error'].ErrorDetails.Message;
                }
            }
        }
        New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Response;
    }
}

Function Test-SoapEnvelope {
    <#
        .SYNOPSIS
			Validate a soap envelope.
         
        .DESCRIPTION
			Returns true if the object represents a valid soap envelope; otherwise false, if it does not.
        
        .OUTPUTS
			System.Boolean. true if the object represents a valid soap envelope; otherwise false, if it does not.
        
        .LINK
            New-SoapEnvelope
            
        .LINK
            Initialize-WebRequestPostXml
    #>
    [CmdletBinding(DefaultParameterSetName = 'XmlNamespaceManager')]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObject')]
        # Object containing an XmlDocument property which contains the SOAP envelope XML.
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        # Xml document to be tested whether it represents a valid a SOAP envelope.
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [Parameter(Mandatory = $true, ParameterSetName = 'XmlNamespaceManager')]
        # Namespace manager to use when validating the SOAP envelope.
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager
    )
    
    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'PSObject' {
                if ($SoapEnvelope.XmlDocument -eq $null) { $false } else { $SoapEnvelope.XmlDocument | Test-SoapEnvelope }
                break;
            }
            'Properties' {
                if ($PSBoundParameters.ContainsKey('XmlNamespaceManager')) {
                    $XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $XmlDocument.NameTable;
                    $XmlNamespaceManager.AddNamespace('xsd', 'http://www.w3.org/2001/XMLSchema');
                    $XmlNamespaceManager.AddNamespace('xsi', 'http://www.w3.org/2001/XMLSchema-instance');
                    $XmlNamespaceManager.AddNamespace('soap12', 'http://www.w3.org/2003/05/soap-envelope');
                }
                if (Test-SoapEnvelope -XmlNamespaceManager $XmlNamespaceManager) {
                    $XPath = '/{0}:Envelope/{0}:Body' -f $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope');
                    if ($XmlDocument.SelectNodes($XPath, $XmlNamespaceManager).Count -eq 1) { $true } else { $false }
                } else {
                    $false;
                }
            }
            default {
                if ($XmlNamespaceManager.HasNamespace('http://www.w3.org/2001/XMLSchema') -and $XmlNamespaceManager.HasNamespace('http://www.w3.org/2001/XMLSchema-instance') -and `
                        $XmlNamespaceManager.HasNamespace('http://www.w3.org/2003/05/soap-envelope')) {
                    $true
                } else {
                    $false;
                }
            }
        }
    }
}

Function Get-SoapXmlNamespacePrefix {
    <#
        .SYNOPSIS
			Get XML name prefix for a SOAP namespace.
         
        .DESCRIPTION
			Returns the XML name prefix for a SOAP namespace.
        
        .OUTPUTS
			System.String. Name prefix of a SOAP namespace.
        
        .LINK
            New-SoapEnvelope
    #>
    [CmdletBinding(DefaultParameterSetname = 'PropertiesSoap')]
    [OutputType([string])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObjectSoap')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObjectSchema')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObjectInstance')]
        [ValidateScript({ $_ | Test-SoapEnvelope })]
        # Object containing an XmlDocument property which contains the SOAP envelope XML.
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'PropertiesSoap')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'PropertiesSchema')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'PropertiesInstance')]
        [ValidateScript({ Test-SoapEnvelope -XmlDocument $_ })]
        # XmlDocument which contains the SOAP envelope XML.
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlNamespaceManager $_ })]
        # Namespace manager to use with the SOAP envelope.
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager,
        
        [Parameter(ParameterSetname = 'PropertiesSoap')]
        [Parameter(Mandatory = $true, ParameterSetname = 'PSObjectSoap')]
        # Get prefix associated with the "http://www.w3.org/2003/05/soap-envelope" namespace.
        [switch]$Soap,
        
        [Parameter(Mandatory = $true, ParameterSetname = 'PSObjectSchema')]
        [Parameter(Mandatory = $true, ParameterSetname = 'PropertiesSchema')]
        # Get prefix associated with the "http://www.w3.org/2001/XMLSchema" namespace.
        [switch]$Schema,
        
        [Parameter(Mandatory = $true, ParameterSetname = 'PSObjectInstance')]
        [Parameter(Mandatory = $true, ParameterSetname = 'PropertiesInstance')]
        # Get prefix associated with the "http://www.w3.org/2001/XMLSchema-instance" namespace.
        [switch]$SchemaInstance
    )
    
    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'PSObjectSoap' { $SoapEnvelope | Get-SoapXmlNamespacePrefix -Soap; break; }
            'PSObjectSchema' { $SoapEnvelope | Get-SoapXmlNamespacePrefix -Schema; break; }
            'PSObjectInstance' { $SoapEnvelope | Get-SoapXmlNamespacePrefix -SchemaInstance; break; }
            'PropertiesSoap' { $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope'); break; }
            'PropertiesSchema' { $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2001/XMLSchema'); break; }
            default { $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2001/XMLSchema-instance'); break; }
        }
    }
}

Function Add-SoapBodyElement {
    <#
        .SYNOPSIS
			Add body to SOAP envelope.
         
        .DESCRIPTION
			Returns the XML name prefix for a SOAP namespace.
        
        .OUTPUTS
			System.Xml.XmlElement[]. Elements imported into the SOAP body.
        
        .LINK
            New-SoapEnvelope
        
        .LINK
            Get-SoapBodyElement
        
        .LINK
            Get-SoapXmlNamespacePrefix
    #>
    [CmdletBinding(DefaultParameterSetName = 'Properties')]
    [OutputType([System.Xml.XmlElement[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObject')]
        [ValidateScript({ $_ | Test-SoapEnvelope })]
        # Object containing an XmlDocument property which contains the SOAP envelope XML.
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlDocument $_ })]
        # XmlDocument which contains the SOAP envelope XML.
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlNamespaceManager $_ })]
        # Namespace manager to use with the SOAP envelope.
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetname = 'PSObject')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Properties')]
        # Elements to add to the SOAP body.
        [System.Xml.XmlElement[]]$Body
    )
    
    Begin { if ($PSCmdlet.ParameterSetName -eq 'PSObject') { $BodyElements = @() } }
    
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'PSObject') {
            $BodyElements = $BodyElements + $Body;
        } else {
            $XPath = '/{0}:Envelope/{0}:Body' -f $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope');
            $BodyElement = $XmlDocument.SelectSingleNode($XPath, $XmlNamespaceManager);
            foreach ($XmlElement in $Body) { $BodyElement.AppendChild($XmlDocument.ImportNode($XmlElement)) }
        }
    }
    
    End { if ($PSCmdlet.ParameterSetName -eq 'PSObject') { $SoapEnvelope | Add-SoapBodyElement -Body $BodyElements } }
}

Function Get-SoapBodyElement {
    <#
        .SYNOPSIS
			Get elements contained within the SOAP body.
         
        .DESCRIPTION
			Returns the elements contained within the SOAP body.
        
        .OUTPUTS
			System.Xml.XmlElement[]. Elements contained within the SOAP body.
        
        .LINK
            New-SoapEnvelope
        
        .LINK
            Add-SoapBodyElement
    #>
    [CmdletBinding(DefaultParameterSetName = 'Properties')]
    [OutputType([System.Xml.XmlElement[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObject')]
        [ValidateScript({ $_ | Test-SoapEnvelope })]
        # Object containing an XmlDocument property which contains the SOAP envelope XML.
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlDocument $_ })]
        # XmlDocument which contains the SOAP envelope XML.
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlNamespaceManager $_ })]
        # Namespace manager to use with the SOAP envelope.
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager
    )
    
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'PSObject') {
            $SoapEnvelope | Get-SoapBodyElement
        } else {
            $XPath = '/{0}:Envelope/{0}:Body/*' -f $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope');
            $XmlNodeList = $XmlDocument.SelectNodes($XPath, $XmlNamespaceManager);
            for ($n = 0; $n -lt $XmlNodeList.Count; $n++) { $XmlNodeList.Item($n) }
        }
    }
}

Function New-SoapEnvelope {
    <#
        .SYNOPSIS
			Create new SOAP envelope object.
         
        .DESCRIPTION
			Creates an XmlDocument which represents a SOAP envelope.
        
        .OUTPUTS
			System.Xml.XmlDocument. Object which represents a SOAP envelope.
        
        .LINK
            Get-SoapBodyElement
        
        .LINK
            Add-SoapBodyElement
    #>
    [CmdletBinding()]
    [OutputType([System.Xml.XmlDocument])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # Elements to include in the SOAP body.
        [System.Xml.XmlElement[]]$Body
    )
    
    [Xml]$XmlDocument = @'
<?xml version="1.0" encoding="utf-8"?>
<soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
  <soap12:Body />
</soap12:Envelope>
'@;
    $XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $SoapEnvelope.NameTable;
    $XmlNamespaceManager.AddNamespace('xsd', 'http://www.w3.org/2001/XMLSchema');
    $XmlNamespaceManager.AddNamespace('xsi', 'http://www.w3.org/2001/XMLSchema-instance');
    $XmlNamespaceManager.AddNamespace('soap12', 'http://www.w3.org/2003/05/soap-envelope');
    
    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ XmlDocument = $XmlDocument; XmlNamespaceManager = $XmlNamespaceManager; }
}

Function Get-ComputerDomain {
    <#
        .SYNOPSIS
			Get current computer AD domain.
         
        .DESCRIPTION
			Gets the Domain object that represents the domain to which the local computer is joined.
        
        .OUTPUTS
			System.DirectoryServices.ActiveDirectory.Domain. Object which represents an Active Directory domain.
			System.DirectoryServices.DirectoryEntry. Object which represents an Active Directory domain.
            System.String. Name of domain.
        
        .LINK
            Get-CurrentUserDomain
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.directoryservices.activedirectory.domain.getcomputerdomain.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'DomainObject')]
    [OutputType([System.DirectoryServices.ActiveDirectory.Domain], ParameterSetName = 'DomainObject')]
    [OutputType([System.DirectoryServices.DirectoryEntry], ParameterSetName = 'DirectoryEntry')]
    [OutputType([string], ParameterSetName = 'Name')]
    [OutputType([string], ParameterSetName = 'Fqdn')]
    Param(
        [Parameter(ParameterSetName = 'DomainObject')]
        # Returns the domain object. This is the default behavior if no switch is used.
        [switch]$DomainObject,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'DirectoryEntry')]
        # Gets the DirectoryEntry object associated with the domain.
        [switch]$DirectoryEntry,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Name')]
        # Gets the directory entry name (NetBIOS name) of the domain
        [switch]$Name,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Fqdn')]
        # Gets the FQDN (partition name) of the domain
        [switch]$Fqdn
    )

    $Domain = [System.DirectoryServices.ActiveDirectory.Domain]::GetComputerDomain();
    if ($Domain -eq $null) { return }
    if ($PSCmdlet.ParameterSetName -eq 'DomainObject') { return $Domain }
    $Obj = $null;
    try {
        if ($PSCmdlet.ParameterSetName -eq 'Fqdn') {
            $Obj = $Domain.Name;
        } else {
            $DirectoryEntry = $Domain.GetDirectoryEntry();
            if ($DirectoryEntry -eq $null -or $PSCmdlet.ParameterSetName -eq 'DirectoryEntry') {
                $Obj = $DirectoryEntry;
            } else {
                try {
                    $Obj = $DirectoryEntry.Name;
                } catch {
                    throw;
                } finally {
                    $DirectoryEntry.Dispose();
                }
            }
        }
    } catch {
        throw;
    } finally {
        $Domain.Dispose();
    }
    
    if ($Obj -ne $null) { return $Obj }
}

Function Get-CurrentUserDomain {
    <#
        .SYNOPSIS
			Get current user AD domain.
         
        .DESCRIPTION
			Gets the Domain object for the current user credentials.
        
        .OUTPUTS
			System.DirectoryServices.ActiveDirectory.Domain. Object which represents an Active Directory domain.
			System.DirectoryServices.DirectoryEntry. Object which represents an Active Directory domain.
            System.String. Name of domain.
        
        .LINK
            Get-ComputerDomain
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.directoryservices.activedirectory.domain.getcurrentdomain.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'DomainObject')]
    [OutputType([System.DirectoryServices.ActiveDirectory.Domain], ParameterSetName = 'DomainObject')]
    [OutputType([System.DirectoryServices.DirectoryEntry], ParameterSetName = 'DirectoryEntry')]
    [OutputType([string], ParameterSetName = 'Name')]
    [OutputType([string], ParameterSetName = 'Fqdn')]
    Param(
        [Parameter(ParameterSetName = 'DomainObject')]
        # Returns the domain object. This is the default behavior if no switch is used.
        [switch]$DomainObject,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'DirectoryEntry')]
        # Gets the DirectoryEntry object associated with the domain.
        [switch]$DirectoryEntry,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Name')]
        # Gets the directory entry name (NetBIOS name) of the domain
        [switch]$Name,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Fqdn')]
        # Gets the FQDN (partition name) of the domain
        [switch]$Fqdn
    )

    $Domain = [System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain();
    if ($Domain -eq $null) { return }
    if ($PSCmdlet.ParameterSetName -eq 'DomainObject') { return $Domain }
    $Obj = $null;
    try {
        if ($PSCmdlet.ParameterSetName -eq 'Fqdn') {
            $Obj = $Domain.Name;
        } else {
            $DirectoryEntry = $Domain.GetDirectoryEntry();
            if ($DirectoryEntry -eq $null -or $PSCmdlet.ParameterSetName -eq 'DirectoryEntry') {
                $Obj = $DirectoryEntry;
            } else {
                try {
                    $Obj = $DirectoryEntry.Name;
                } catch {
                    throw;
                } finally {
                    $DirectoryEntry.Dispose();
                }
            }
        }
    } catch {
        throw;
    } finally {
        $Domain.Dispose();
    }
    
    if ($Obj -ne $null) { return $Obj }
}

Function Test-DirectoryServiceLogin {
    <#
        .SYNOPSIS
			Tests login credentials.
         
        .DESCRIPTION
			Tests login credentials against Active Directory to make sure they authenticate.
        
        .OUTPUTS
			System.Xml.XmlElement[]. Elements contained within the SOAP body.
        
        .LINK
            Get-Credential
        
        .LINK
            Add-SoapBodyElement
    #>
    [CmdletBinding(DefaultParameterSetName = 'ImplicitDomain')]
    [OutputType([System.Xml.XmlElement[]])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'ExplicitDomain')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'ImplicitDomain')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'ContextDomain')]
        # Login to authenticate
        [string]$UserName,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'ExplicitDomain')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'ImplicitDomain')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'ContextDomain')]
        # Password to use for authentication
        [System.Security.SecureString]$Password,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'ExplicitDomain')]
        # Domain to use with authentication
        [string]$Domain,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'ExplicitDomain')]
        [Parameter(Mandatory = $true, ParameterSetName = 'ImplicitDomain')]
        [System.DirectoryServices.AccountManagement.ContextType]$ContextType = [System.DirectoryServices.AccountManagement.ContextType]::Domain,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'ContextDomain')]
        [System.DirectoryServices.AccountManagement.PrincipalContext]$PrincipalContext
    )
    
    Begin {
        if ($PSCmdlet.ParameterSetName -eq 'ContextDomain') {
            $CurrentContext = $PrincipalContext;
        } else {
            $CurrentContext = $null;
        }
        $CurrentUserDomain = $null;
    }
    
    Process {
        $PSCredential = New-Object -TypeName 'System.Management.Automation.PSCredential' -ArgumentList $UserName, $Password
        $NetworkCredential = $PSCredential.GetNetworkCredential();
        if ($PSCmdlet.ParameterSetName -ne 'ContextDomain') {
            if ($PSCmdlet.ParameterSetName -eq 'ImplicitDomain') {
                if ($NetworkCredential.Domain -eq '') {
                    if ($CurrentUserDomain -eq $null) { $CurrentUserDomain = Get-CurrentUserDomain -Name }
                    $Domain = $CurrentUserDomain;
                } else {
                    $Domain = $NetworkCredential.Domain;
                }
            }
            if ($CurrentContext -eq $null -or $CurrentContext.Name -ne $Domain) {
                if ($CurrentContext -ne $null) {
                    $CurrentContext.Dispose();
                    $CurrentContext = $null;
                }
                $CurrentContext = New-Object -TypeName 'System.DirectoryServices.AccountManagement.PrincipalContext' -ArgumentList $ContextType, $Domain;
            }
        }
        $CurrentContext.ValidateCredentials($NetworkCredential.UserName, $NetworkCredential.Password);
    }
    
    End { if ($PSCmdlet.ParameterSetName -ne 'ContextDomain' -and $CurrentContext -ne $null) { $CurrentContext.Dispose() } }
}

Function Get-NugetApiUrl {
    <#
        .SYNOPSIS
			Get URL for NuGet API request.
         
        .DESCRIPTION
			returns URI for API request.
        
        .OUTPUTS
			System.Uri. A Uri for the NuGet API request.
    #>
    [CmdletBinding()]
    [OutputType([System.Uri])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # Path of request.
        [string]$Path,
        
        [string]$Query
    )
    
    $UriBuilder = New-Object -TypeName 'System.UriBuilder' -ArgumentList 'http://nuget.org/api/v2';
    if ($Path.StartsWith('/')) {
        $UriBuilder.Path = $UriBuilder.Path + $Path;
    } else {
        $UriBuilder.Path = $UriBuilder.Path + '/' + $Path;
    }
    
    if ($PSBoundParameters.ContainsKey('Query')) { $UriBuilder.Query = $Query }
    $UriBuilder.Uri | Write-Output;
}

Function Get-NugetPackageVersions {
    <#
        .SYNOPSIS
			Get versions for NuGet packages.
         
        .DESCRIPTION
			returns a list of NuGet package versions
        
        .OUTPUTS
			System.String[]. An array of package versions
    #>
    [CmdletBinding()]
    [OutputType([string[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # NuGet package identifier.
        [string]$Identifier
    )
    $WebRequest = New-WebRequest -Uri (Get-NugetApiUrl -Path ('package-versions/' + $Identifier));
    $WebResponse = Get-WebResponse -WebRequest $WebRequest -AllowRedirect $true;
    $Encoding = [System.Text.Encoding]::UTF8;
    if ($WebResponse.ContentType -ne $null -and $WebResponse.ContentType.CharSet -ne $null -and $WebResponse.ContentType.CharSet.Trim().Length -gt 0) {
        $e = $Encoding;
        try { $e = [System.Text.Encoding]::GetEncoding($WebResponse.ContentType.CharSet) } catch { }
        if ($e -ne $null) { $Encoding = $e }
    }
    $Stream = $WebResponse.Response.GetResponseStream();
    $Json = $null;
    try {
        $Reader = New-Object -TypeName 'System.IO.StreamReader' -ArgumentList $Stream, $Encoding;
        try {
            $Json = $Reader.ReadToEnd();
        } finally { $Stream.Dispose() }
    } finally { $Stream.Dispose() }

    $JavaScriptSerializer = New-Object -TypeName 'System.Web.Script.Serialization.JavaScriptSerializer';
    #[System.Web.Script.Serialization.JavaScriptConverter[]]$Converters = @((New-Object -TypeName 'NugetSerialization2.JsonConverter'));
    #$JavaScriptSerializer.RegisterConverters($Converters);
    $JavaScriptSerializer.DeserializeObject($Json) | Write-Output;
}

Function Get-NugetPackageInfo {
    <#
        .SYNOPSIS
			Get versions for NuGet packages.
         
        .DESCRIPTION
			returns a list of NuGet package versions
        
        .OUTPUTS
			System.String[]. An array of package versions
    #>
    [CmdletBinding()]
    [OutputType([string[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # NuGet package identifier.
        [string]$Identifier,
        
        [Parameter(Position = 1, ParameterSetName = 'ByVersion')]
        # NuGet package version.
        [string]$Version,
        
        [Parameter(ParameterSetName = 'LatestVersion')]
        # Returns latest stable version.
        [switch]$LatestVersion
        
    )
    $Url = Get-NugetApiUrl -Path 'FindPackagesById()' -Query ('id=''' + $Identifier + '''');
    $WebRequest = New-WebRequest -Uri $Url;
    $WebResponse = Get-WebResponse -WebRequest $WebRequest -AllowRedirect $true;
    $Encoding = [System.Text.Encoding]::UTF8;
    if ($WebResponse.ContentType -ne $null -and $WebResponse.ContentType.CharSet -ne $null -and $WebResponse.ContentType.CharSet.Trim().Length -gt 0) {
        $e = $Encoding;
        try { $e = [System.Text.Encoding]::GetEncoding($WebResponse.ContentType.CharSet) } catch { }
        if ($e -ne $null) { $Encoding = $e }
    }
    $Stream = $WebResponse.Response.GetResponseStream();
    $XmlDocment = New-Object -TypeName 'System.Xml.XmlDocument';
    try {
        $XmlDocment.Load($Stream);
    } finally { $Stream.Dispose() }

    $XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $XmlDocument.NameTable;
    $XmlNamespaceManager.AddNamespace('a', 'http://www.w3.org/2005/Atom');
    $XmlNamespaceManager.AddNamespace('d', 'http://schemas.microsoft.com/ado/2007/08/dataservices');
    $XmlNamespaceManager.AddNamespace('m', 'http://schemas.microsoft.com/ado/2007/08/dataservices/metadata');
    $XmlNamespaceManager.AddNamespace('georss', 'http://www.georss.org/georss');
    $XmlNamespaceManager.AddNamespace('gml', 'http://www.opengis.net/gml');
    $Results = @($XmlDocument.SelectNodes('/a:feed/a:entry', $XmlNamespaceManager) | ForEach-Object {
        $EntryElement = $_;
        $Properties = @{};
        $XmlElement = $_.SelectSingleNode('a:id', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) { $Properties['id'] = $XmlElement.InnerText }
        $XmlElement = $_.SelectSingleNode('a:title', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) { $Properties['title'] = $XmlElement.InnerText }
        $XmlElement = $_.SelectSingleNode('a:summary', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) { $Properties['summary'] = $XmlElement.InnerText }
        $XmlElement = $_.SelectSingleNode('a:updated', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) {
            $s = $XmlElement.InnerText.Trim();
            if ($s.Length -gt 0) { try { $Properties['updated'] = [System.Xml.XmlConvert]::ToDateTime($s, 'yyyy-MM-ddTHH:mm:sszzzzzz') } catch { } }
        }
        $XmlElement = $_.SelectSingleNode('a:author', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) {
            $p = @{};
            foreach ($e in $XmlElement.SelectNodes('a:*', $XmlNamespaceManager)) {
                if (-not $e.IsEmpty) { $p[$e.LocalName] = $e.InnerText }
            }
            if ($p.Count -gt 0) { $Properties['author'] = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $p }
        }
        
        $XmlElement = $_.SelectSingleNode('a:content', $XmlNamespaceManager);
        if ($XmlElement -ne $null) {
            $p = @{};
            foreach ($a in $XmlElement.SelectNodes('@*')) {
                $p[([System.Xml.XmlConvert]::EncodeLocalName($a.Name))] = $a.Value;
            }
            if ($p.Count -gt 0) { $Properties['content'] = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $p }
        }
        $XmlElement = $_.SelectSingleNode('m:properties', $XmlNamespaceManager);
        if ($XmlElement -ne $null -and -not $XmlElement.IsEmpty) {
            $p = @{};
            foreach ($e in $XmlElement.SelectNodes('d:*', $XmlNamespaceManager)) {
                if (-not $e.IsEmpty) {
                    switch ($e.LocalName) {
                        'Created' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['Created'] = [System.Xml.XmlConvert]::ToDateTime($s, 'yyyy-MM-ddTHH:mm:sszzzzzz') } catch { } }
                            break;
                        }
                        'LastUpdated' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['LastUpdated'] = [System.Xml.XmlConvert]::ToDateTime($s, 'yyyy-MM-ddTHH:mm:sszzzzzz') } catch { } }
                            break;
                        }
                        'Published' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['Published'] = [System.Xml.XmlConvert]::ToDateTime($s, 'yyyy-MM-ddTHH:mm:sszzzzzz') } catch { } }
                            break;
                        }
                        'LastEdited' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['LastEdited'] = [System.Xml.XmlConvert]::ToDateTime($s, 'yyyy-MM-ddTHH:mm:sszzzzzz') } catch { } }
                            break;
                        }
                        'IsLatestVersion' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['IsLatestVersion'] = [System.Xml.XmlConvert]::ToBoolean($s) } catch { } }
                            break;
                        }
                        'IsAbsoluteLatestVersion' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['IsAbsoluteLatestVersion'] = [System.Xml.XmlConvert]::ToBoolean($s) } catch { } }
                            break;
                        }
                        'IsPrerelease' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['IsPrerelease'] = [System.Xml.XmlConvert]::ToBoolean($s) } catch { } }
                            break;
                        }
                        'PackageSize' {
                            $s = $e.InnerText.Trim();
                            if ($s.Length -gt 0) { try { $p['PackageSize'] = [System.Xml.XmlConvert]::ToInt64($s) } catch { } }
                            break;
                        }
                        default {
                            $p[$e.LocalName] = $e.InnerText;
                            break;
                        }
                    }
                }
            }
            if ($p.Count -gt 0) { $Properties['properties'] = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $p }
        }
        New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties
    });
    if ($LatestVersion) {
        $Results | Where-Object { $_.properties.IsLatestVersion  }
    } else {
        if ($PSBoundParameters.ContainsKey('Version')) {
            $Results | Where-Object { $_.properties.Version -eq $Version  }
        } else {
            $Results | Write-Output;
        }
    }
}
