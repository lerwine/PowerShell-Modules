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

$SourceFileName = Read-FilePath -Title 'Source File Path' -Filter @{ '*.txt' = 'Text Files (*.txt)' } -CheckFileExists -OpenFile;
if ([String]::IsNullOrEmpty($SourceFileName)) { return }

$TempPath = [System.IO.Path]::GetTempFileName();
$TempStream = $null;
try {
    Write-Progress -Activity 'Decoding File' -Status 'Initializing' -Id 1 -PercentComplete 0 -CurrentOperation 'Opening temp file';
	$TempStream = New-Object -TypeName 'System.IO.FileStream' -ArgumentList $TempPath, ([System.IO.FileMode]::OpenOrCreate), ([System.IO.FileAccess]::ReadWrite) -ErrorAction Stop;
} catch {
    Write-Progress -Activity 'Decoding File' -Status 'Failed' -Id 1 -PercentComplete 0 -CurrentOperation 'Cleaning up temp file';
	if ([System.IO.File]::Exists($TempPath)) { [System.IO.File]::Delete($TempPath) }
	throw;
}
try {
	# decode
    Write-Progress -Activity 'Decoding File' -Status 'Working' -Id 1 -PercentComplete 0 -CurrentOperation ('Opening ' + $SourceFileName);
	$InputStream = New-Object -TypeName 'System.IO.FileStream' -ArgumentList $SourceFileName, ([System.IO.FileMode]::Open), ([System.IO.FileAccess]::Read) -ErrorAction Stop;
	$StreamReader = $null;
	try {
		$StreamReader = New-Object -TypeName 'System.IO.StreamReader' -ArgumentList $InputStream, ([System.Text.Encoding]::UTF8);
	} catch {
		$InputStream.Close();
		$InputStream.Dispose();
		throw;
	}
		
    Write-Progress -Activity 'Decoding File' -Status 'Working' -Id 1 -PercentComplete 0 -CurrentOperation ('Reading ' + $SourceFileName);
	try {
        $percentComplete = 0;
	    for ($s = $StreamReader.ReadLine(); $s -ne $null; $s = $StreamReader.ReadLine()) {
            $t = $s.Trim();
            if ($t.Length -eq 0) { continue }
            try {
                $Buffer = ConvertFrom-Base64String -InputString $t;
                if ($Buffer -ne $null) { Write-DataBuffer -Buffer $Buffer -Stream $TempStream }
            } catch { }
            
            [int]$pct = (($InputStream.Position * 100) / $InputStream.Length) / 2;
            if ($pct -ne $percentComplete) {
                $percentComplete = $pct;
                Write-Progress -Activity 'Decoding File' -Status 'Working' -Id 1 -PercentComplete $percentComplete -CurrentOperation ('Decoding {0} bytes of {1}' -f $InputStream.Position, $InputStream.Length);
            }
		}
		$TempStream.Flush();
		$TempStream.Seek([long]0, [System.IO.SeekOrigin]::Begin);
	} catch {
		throw;
	} finally {
		$InputStream.Close();
		$InputStream.Dispose();
	}
		
    $TargetFileName = [System.Text.Encoding]::UTF8.GetString((Read-LengthEncodedBytes -Stream $TempStream));
    
    Write-Progress -Activity 'Decoding File' -Status 'Success' -Id 1 -PercentComplete 100 -Completed;

    $ext = [System.IO.Path]::GetExtension($TargetFileName);
    if ($ext -eq '') {
        $TargetFileName = Read-FilePath -Title 'Target File Path' -FileName $TargetFileName -SaveFile;
    } else {
        $TargetFileName = Read-FilePath -Title 'Target File Path' -Filter @{ ('*{0}' -f $ext) = ('{0} Files (*{1})' -f $ext.Substring(1).ToUpper(), $ext) } -FileName $TargetFileName -SaveFile;
    }
    if (-not [String]::IsNullOrEmpty($TargetFileName)) {
        #decrypt
        Write-Progress -Activity 'Decrypting File' -Status 'Working' -Id 1 -PercentComplete 50 -CurrentOperation ('Opening ' + $TargetFileName);
		$OutputStream = New-Object -TypeName 'System.IO.FileStream' -ArgumentList $TargetFileName, ([System.IO.FileMode]::Create), ([System.IO.FileAccess]::Write) -ErrorAction Stop;
		try {
		    $bytes = Unprotect-WithSymmetricAlgorithm -Certificate $X509Certificate2 -InputStream $TempStream -OutputStream $OutputStream -ParentProgressId 1 -ProgressId 2;
		    '{0} bytes encrypted' -f $bytes;
		} catch {
		    throw;
		} finally {
		    $OutputStream.Flush();
		    $OutputStream.Dispose();
		}
    }
    Write-Progress -Activity 'Decrypting File' -Status 'Success' -Id 1 -PercentComplete 100 -Completed;
} catch {
    Write-Progress -Activity 'Decrypting File' -Status 'Success' -Id 1 -PercentComplete 100 -Completed;
	throw;
} finally {
	if ($TempStream -ne $null) {
		$TempStream.Close();
		$TempStream.Dispose();
	}
	if ([System.IO.File]::Exists($TempPath)) { [System.IO.File]::Delete($TempPath) }
}