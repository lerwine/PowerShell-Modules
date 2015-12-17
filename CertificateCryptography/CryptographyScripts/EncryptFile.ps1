Import-Module UserFileUtils;
Import-Module CertificateCryptography;

$CertificateCollection = Select-X509Certificate -UsageFlags (New-X509KeyUsageFlags -KeyEncipherment);
$X509Certificate2 = $null;
if ($CertificateCollection.Count -eq 0) {
    'No certificates which support key encryption were found.' | Write-Warning;
} else {
    $X509Certificate2 = Read-X509Certificate -CertificateCollection $CertificateCollection -Message 'Select certificate for encryption';
    if ($X509Certificate2 -eq $null) { 'No certificate was selected.' | Write-Warning }
}

if ($X509Certificate2 -eq $null)  { return }

$SourceFileName = Read-FilePath -Title 'Source File Path' -CheckFileExists -OpenFile;
if ([String]::IsNullOrEmpty($SourceFileName)) { return }
$TargetFileName = Read-FilePath -Title 'Target File Path' -Filter @{ '*.txt' = 'Text Files (*.txt)' } -SaveFile;
if ([String]::IsNullOrEmpty($TargetFileName)) { return }

$TempPath = $null;
$TempStream = $null;
try {
    Write-Progress -Activity 'Encrypting File' -Status 'Initializing' -Id 1 -PercentComplete 0 -CurrentOperation 'Opening temp file';
    $TempPath = [System.IO.Path]::GetTempFileName();
    $TempStream = New-Object -TypeName 'System.IO.FileStream' -ArgumentList $TempPath, ([System.IO.FileMode]::OpenOrCreate), ([System.IO.FileAccess]::ReadWrite) -ErrorAction Stop;
} catch {
    Write-Progress -Activity 'Encrypting File' -Status 'Failed' -Id 1 -PercentComplete 0 -CurrentOperation 'Cleaning up temp file';
    if ($TempPath -ne $null -and [System.IO.File]::Exists($TempPath)) { [System.IO.File]::Delete($TempPath) }
    Write-Progress -Activity 'Encrypting File' -Status 'Failed' -Id 1 -PercentComplete 0 -Completed;
    throw;
}
try {
    # Encrypt
    Write-Progress -Activity 'Encrypting File' -Status 'Working' -Id 1 -PercentComplete 0 -CurrentOperation ('Opening ' + $SourceFileName);
    $InputStream = New-Object -TypeName 'System.IO.FileStream' -ArgumentList $SourceFileName, ([System.IO.FileMode]::Open), ([System.IO.FileAccess]::Read) -ErrorAction Stop;
    try {
        Write-LengthEncodedBytes -Stream $TempStream -Bytes ([System.Text.Encoding]::UTF8.GetBytes(($SourceFileName | Split-Path -Leaf)));
        $bytes = Protect-WithSymmetricAlgorithm -Certificate $X509Certificate2 -InputStream $InputStream -OutputStream $TempStream -ParentProgressId 1 -ProgressId 2;
        $TempStream.Flush();
        $TempStream.Seek([long]0, [System.IO.SeekOrigin]::Begin);
    } catch {
        throw;
    } finally {
        $InputStream.Close();
        $InputStream.Dispose();
    }
    
    #encode
    Write-Progress -Activity 'Encoding File' -Status 'Working' -Id 1 -PercentComplete 50 -CurrentOperation ('Opening ' + $TargetFileName);
    $OutputStream = New-Object -TypeName 'System.IO.FileStream' -ArgumentList $TargetFileName, ([System.IO.FileMode]::Create), ([System.IO.FileAccess]::Write) -ErrorAction Stop;
    try {
        $StreamWriter = New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $OutputStream, ([System.Text.Encoding]::UTF8);
    } catch {
        $OutputStream.Close();
        $OutputStream.Dispose();
        throw;
    }
        
    try {
        $DataBuffer = New-DataBuffer -Capacity 65535 -Base64Encoding;
        $percentComplete = 0;
        [long]$totalBytesRead = 0;
        for ($count = (Read-DataBuffer -Buffer $DataBuffer -Stream $TempStream); $count -gt 0; $count = (Read-DataBuffer -Buffer $DataBuffer -Stream $TempStream)) {
            $StreamWriter.WriteLine((ConvertTo-Base64String -Buffer $DataBuffer -Length $count -InsertLineBreaks).Trim());
            [long]$totalBytesRead = $totalBytesRead + [long]$count;
            [int]$pct = ((($totalBytesRead * 100) / $TempStream.Length) / 2) + 50;
            if ($pct -ne $percentComplete) {
                $percentComplete = $pct;
                Write-Progress -Activity 'Encoding File' -Status 'Working' -Id 1 -PercentComplete $percentComplete -CurrentOperation ('Encoding {0} bytes of {1}' -f $totalBytesRead, $TempStream.Length);
            }
        }
    } catch {
        throw;
    } finally {
        $StreamWriter.Flush();
        $StreamWriter.Dispose();
    }
    Write-Progress -Activity 'Encoding File' -Status 'Success' -Id 1 -PercentComplete 100 -Completed;
} catch {
    Write-Progress -Activity 'Encoding File' -Status 'Failed' -Id 1 -PercentComplete 100 -Completed;
    throw;
} finally {
    if ($TempStream -ne $null) {
        $TempStream.Close();
        $TempStream.Dispose();
    }
    if ([System.IO.File]::Exists($TempPath)) { [System.IO.File]::Delete($TempPath) }
}
