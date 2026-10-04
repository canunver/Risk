# DbKopru.ps1 - runs .sql files dropped into _db\sorgu against the local Risk database
# and writes each result as JSON into _db\sonuc. Every query runs inside a transaction
# that is always rolled back, so nothing is changed in the database.
param(
    [string]$Root = $PSScriptRoot,
    [int]$MaxRows = 5000,
    [int]$MaxCell = 20000
)
$ErrorActionPreference = 'Stop'
$inDir   = Join-Path $Root 'sorgu'
$outDir  = Join-Path $Root 'sonuc'
$doneDir = Join-Path $Root 'islenen'
foreach ($d in @($inDir, $outDir, $doneDir)) { New-Item -ItemType Directory -Force -Path $d | Out-Null }
$appsettings = Join-Path (Split-Path $Root -Parent) 'Risk.net.WebUI\appsettings.json'
$utf8 = New-Object System.Text.UTF8Encoding($false)

function Get-ConnStr {
    $j = [System.IO.File]::ReadAllText($appsettings, [System.Text.Encoding]::UTF8) | ConvertFrom-Json
    return $j.BaglantiSatirlari.ConnectionSqlServer
}

function Convert-Cell($v) {
    if ($null -eq $v -or $v -is [System.DBNull]) { return $null }
    if ($v -is [byte[]]) { return "<binary $($v.Length) bytes>" }
    if ($v -is [datetime]) { return $v.ToString('yyyy-MM-dd HH:mm:ss') }
    if ($v -is [bool] -or $v -is [int] -or $v -is [long] -or $v -is [int16] -or $v -is [byte] -or $v -is [decimal] -or $v -is [double] -or $v -is [single]) { return $v }
    $s = [string]$v
    if ($s.Length -gt $MaxCell) { return $s.Substring(0, $MaxCell) + "...<truncated, $($s.Length) chars>" }
    return $s
}

function Invoke-Sql([string]$sql) {
    $script:msgs = New-Object System.Collections.ArrayList
    $sets = New-Object System.Collections.ArrayList
    $conn = New-Object System.Data.SqlClient.SqlConnection (Get-ConnStr)
    $conn.add_InfoMessage([System.Data.SqlClient.SqlInfoMessageEventHandler] { param($s, $e) [void]$script:msgs.Add($e.Message) })
    $conn.Open()
    try {
        $tx = $conn.BeginTransaction()
        try {
            $batches = [regex]::Split($sql, '(?im)^\s*GO\s*;?\s*$') | Where-Object { $_.Trim() -ne '' }
            foreach ($b in $batches) {
                $cmd = $conn.CreateCommand()
                $cmd.Transaction = $tx
                $cmd.CommandText = $b
                $cmd.CommandTimeout = 120
                $r = $cmd.ExecuteReader()
                try {
                    do {
                        if ($r.FieldCount -gt 0) {
                            $cols = New-Object System.Collections.ArrayList
                            for ($i = 0; $i -lt $r.FieldCount; $i++) { [void]$cols.Add($r.GetName($i)) }
                            $rows = New-Object System.Collections.ArrayList
                            $truncated = $false
                            while ($r.Read()) {
                                if ($rows.Count -ge $MaxRows) { $truncated = $true; break }
                                $row = New-Object object[] $r.FieldCount
                                for ($i = 0; $i -lt $r.FieldCount; $i++) { $row[$i] = Convert-Cell $r.GetValue($i) }
                                [void]$rows.Add($row)
                            }
                            [void]$sets.Add(@{ columns = $cols; rowCount = $rows.Count; truncated = $truncated; rows = $rows })
                        }
                    } while ($r.NextResult())
                }
                finally { $r.Close() }
            }
        }
        finally { try { $tx.Rollback() } catch { } }
    }
    finally { $conn.Close() }
    return @{ ok = $true; messages = $script:msgs; resultSets = $sets }
}

Write-Host "DbKopru calisiyor. Kapatmak icin bu pencereyi kapatin veya Ctrl+C." -ForegroundColor Green
Write-Host "Sorgu klasoru: $inDir"
while ($true) {
    $files = @(Get-ChildItem -Path $inDir -Filter *.sql -File -ErrorAction SilentlyContinue | Sort-Object Name)
    foreach ($f in $files) {
        try { $sql = [System.IO.File]::ReadAllText($f.FullName, [System.Text.Encoding]::UTF8) } catch { continue }
        $name = [System.IO.Path]::GetFileNameWithoutExtension($f.Name)
        $started = Get-Date
        try { $res = Invoke-Sql $sql }
        catch { $res = @{ ok = $false; error = $_.Exception.Message; messages = $script:msgs } }
        $res.query = $f.Name
        $res.ms = [int]((Get-Date) - $started).TotalMilliseconds
        $json = $res | ConvertTo-Json -Depth 8 -Compress
        $tmp = Join-Path $outDir ($name + '.tmp')
        $final = Join-Path $outDir ($name + '.json')
        [System.IO.File]::WriteAllText($tmp, $json, $utf8)
        Move-Item -Force -Path $tmp -Destination $final
        Move-Item -Force -Path $f.FullName -Destination (Join-Path $doneDir $f.Name)
        $color = 'Gray'; if (-not $res.ok) { $color = 'Red' }
        Write-Host ("{0:HH:mm:ss}  {1}  ok={2}  {3} ms" -f (Get-Date), $f.Name, $res.ok, $res.ms) -ForegroundColor $color
    }
    Start-Sleep -Milliseconds 700
}
