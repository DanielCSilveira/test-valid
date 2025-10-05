# Nota: Este é o script de PowerShell que faz o trabalho de formatação.
# Ele será chamado pelo hook 'pre-commit' (sem extensão) do Git.

# Configurações
$SOLUTION_PATH = "src/ValidSolution.sln"

Write-Host "----------------------------------------"
Write-Host "Running C# code formatting verification..."
Write-Host "----------------------------------------"

# O comando para verificar a formatação sem aplicar as mudanças
# Se houver problemas, ele retorna um código de erro.
dotnet format $SOLUTION_PATH --verify-no-changes --verbosity normal

# Captura o código de saída do comando anterior ($LASTEXITCODE no PowerShell)
$EXIT_CODE = $LASTEXITCODE

if ($EXIT_CODE -ne 0) {
    Write-Host ""
    Write-Host "=========================================================================" -ForegroundColor Red
    Write-Host "❌ FORMATTING ERROR: Unformatted C# files detected!" -ForegroundColor Red
    Write-Host "Por favor, execute 'dotnet format $SOLUTION_PATH' para corrigir o estilo." -ForegroundColor Red
    Write-Host "=========================================================================" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Formatting check passed. Commit allowed." -ForegroundColor Green
exit 0
