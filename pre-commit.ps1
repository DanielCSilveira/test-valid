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
    Write-Host "executando 'dotnet format $SOLUTION_PATH' para corrigir o estilo." -ForegroundColor Red
    Write-Host "=========================================================================" -ForegroundColor Red

    dotnet format $SOLUTION_PATH 
    exit 1
}

Write-Host "✅ Formatting check passed. Commit allowed." -ForegroundColor Green
exit 0


Write-Host "✅ Formatting check passed." -ForegroundColor Green

# ----------------------------------------
# 2. EXECUÇÃO DOS TESTES (UNIT & INTEGRATION)
# ----------------------------------------
Write-Host ""
Write-Host "----------------------------------------"
Write-Host "Running C# unit and integration tests..."
Write-Host "----------------------------------------"

# Executa todos os testes na solução
dotnet test $SOLUTION_PATH --verbosity normal
$TEST_EXIT_CODE = $LASTEXITCODE

if ($TEST_EXIT_CODE -ne 0) {
    Write-Host ""
    Write-Host "=========================================================================" -ForegroundColor Red
    Write-Host "❌ TEST FAILURE: C# tests failed!" -ForegroundColor Red
    Write-Host "Por favor, corrija os testes falhados antes de fazer o commit." -ForegroundColor Red
    Write-Host "=========================================================================" -ForegroundColor Red
    exit 1
}

# ----------------------------------------
# 3. SUCESSO
# ----------------------------------------
Write-Host "✅ All tests passed." -ForegroundColor Green
Write-Host ""
Write-Host "=========================================================================" -ForegroundColor Green
Write-Host "✅ All checks passed. Commit allowed." -ForegroundColor Green
Write-Host "=========================================================================" -ForegroundColor Green

exit 0
