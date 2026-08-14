$root = "C:\sources\asp.net\Risk.net"

$extensions = @(
    ".cs",
    ".cshtml",
    ".js",
    ".css",
    ".json",
    ".html",
    ".resx",
    ".xml"
)

$excludedFolders = @(
    "\bin\",
    "\obj\",
    "\.git\",
    "\.vs\",
    "\packages\",
    "\node_modules\"
)

$win1254 = [System.Text.Encoding]::GetEncoding(1254)

# BOM'suz UTF-8
$utf8 = New-Object System.Text.UTF8Encoding($false)

# Hatalı UTF-8 byte dizisinde exception verir
$strictUtf8 = New-Object System.Text.UTF8Encoding($false, $true)

$converted = 0
$alreadyUtf8 = 0
$crcrlfFound = 0

Get-ChildItem -Path $root -Recurse -File | Where-Object {

    $file = $_

    $isExcluded = $false

    foreach ($folder in $excludedFolders) {
        if ($file.FullName.Contains($folder)) {
            $isExcluded = $true
            break
        }
    }

    ($extensions -contains $file.Extension.ToLower()) -and -not $isExcluded

} | ForEach-Object {

    $file = $_
    $bytes = [System.IO.File]::ReadAllBytes($file.FullName)

    if ($bytes.Length -eq 0) {
        return
    }

    # Önceden bozuk CR CR LF var mı?
    $hasCRCRLF = $false

    for ($i = 0; $i -lt $bytes.Length - 2; $i++) {
        if (
            $bytes[$i]     -eq 13 -and
            $bytes[$i + 1] -eq 13 -and
            $bytes[$i + 2] -eq 10
        ) {
            $hasCRCRLF = $true
            break
        }
    }

    if ($hasCRCRLF) {
        Write-Host "CRCRLF BULUNDU : $($file.FullName)" -ForegroundColor Yellow
        $crcrlfFound++
    }

    # Dosya zaten UTF-8 mi?
    $isUtf8 = $true

    try {
        $null = $strictUtf8.GetString($bytes)
    }
    catch {
        $isUtf8 = $false
    }

    if ($isUtf8) {

        $alreadyUtf8++

    }
    else {

        # Byte -> Windows-1254 karakterleri
        $text = $win1254.GetString($bytes)

        # Karakterleri UTF-8 byte dizisine dönüştür
        $newBytes = $utf8.GetBytes($text)

		# ReadOnly ise kaldır
		if ($file.IsReadOnly) {
			$file.IsReadOnly = $false
			Write-Host "READONLY KALDIRILDI : $($file.FullName)" -ForegroundColor Cyan
		}

        # Byte dizisini doğrudan yaz.
        # PowerShell satır sonlarına müdahale etmez.
        [System.IO.File]::WriteAllBytes(
            $file.FullName,
            $newBytes
        )

        Write-Host "DONUSTURULDU : $($file.FullName)" -ForegroundColor Green
        $converted++
    }
}

Write-Host ""
Write-Host "--------------------------------"
Write-Host "Donusturulen       : $converted"
Write-Host "Zaten UTF-8        : $alreadyUtf8"
Write-Host "CRCRLF bulunan     : $crcrlfFound"
Write-Host "--------------------------------"