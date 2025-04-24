#!/bin/bash
# ローカルバックエンド連携Todoアプリの実行スクリプト

set -e

# バックエンドとフロントエンドのディレクトリ
BACKEND_DIR="./src/Backend"
FRONTEND_DIR="./src/FrontEnd.Wpf"

# プロジェクトのビルド
echo "ビルドしています..."
dotnet build

# バックエンド起動
echo "バックエンドを起動しています..."
cd "$BACKEND_DIR"
dotnet run &
BACKEND_PID=$!

# 少し待機してバックエンドが起動するのを待つ
echo "バックエンドの起動を待機しています..."
sleep 5

# フロントエンド起動
echo "WPFクライアントを起動しています..."
cd "../../$FRONTEND_DIR"
dotnet run

# バックエンドプロセスを終了
kill $BACKEND_PID
echo "バックエンドを終了しました。"
