#!/usr/bin/env bash
# ⚡ AgroRegulate İzole Test Çalıştırıcısı
# ---------------------------------------------------------------
#  Bu script hiçbir şeyi PC'ye kalıcı kurmaz:
#   - .NET sadece geçici ~/agro-dotnet klasörüne indirilir (varsa kullanılır)
#   - PostgreSQL GEREKMEZ (test modu bellek içi veritabanı kullanır)
#   - İş bitince: Ctrl+C ile durdur, ~/agro-dotnet klasörünü sil -> temiz
# ---------------------------------------------------------------
set -e

DOTNET_DIR="$HOME/agro-dotnet"
PORT=5226

# 1) .NET yoksa geçici klasöre otomatik indir
if [ ! -x "$DOTNET_DIR/dotnet" ]; then
    echo "[run.sh] .NET bulunamadı -> geçici klasöre indiriliyor: $DOTNET_DIR"
    curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
    bash /tmp/dotnet-install.sh --channel 8.0 --install-dir "$DOTNET_DIR"
fi

export DOTNET_ROOT="$DOTNET_DIR"
export PATH="$DOTNET_DIR:$PATH"
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS="http://localhost:$PORT"

cd "$(dirname "$0")"

echo ""
echo "  ╔══════════════════════════════════════════════════════╗"
echo "  ║   AgroRegulate - İzole Test Ortamı                    ║"
echo "  ║   Tarayıcıda aç:  http://localhost:$PORT               ║"
echo "  ║   Durdurmak için: Ctrl + C                            ║"
echo "  ╚══════════════════════════════════════════════════════╝"
echo ""

dotnet run --project AgroRegulate.API --no-launch-profile
