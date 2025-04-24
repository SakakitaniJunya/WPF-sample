# ローカルバックエンド連携 WPF アプリケーション

## 概要

このプロジェクトは、ローカルPCで起動するバックエンドサーバーとWPFクライアント間の通信を実現するアプリケーションです。ドメイン駆動設計(DDD)とクリーンアーキテクチャを採用し、REST API、gRPC、SignalRによる双方向通信を実装しています。

## 技術スタック

- バックエンド: .NET 9, ASP.NET Core, EF Core InMemory, MediatR, gRPC, SignalR
- フロントエンド: WPF (.NET 9), MVVM, CommunityToolkit.Mvvm
- 通信: REST API, gRPC, SignalR (WebSocket)

## プロジェクト構成

```
TodoApp.sln
 └─ src/
    ├─ Domain/                    # ドメイン層
    │   └─ Todo/                  # Todoドメインモデル
    ├─ Application/               # アプリケーション層
    │   ├─ Commands/              # コマンドハンドラー
    │   ├─ Queries/               # クエリハンドラー
    │   └─ DTOs/                  # データ転送オブジェクト
    ├─ Backend/                   # バックエンド層
    │   ├─ Database/              # データベース実装
    │   ├─ RestApi/               # REST API実装
    │   ├─ GrpcServices/          # gRPC実装
    │   ├─ WebSockets/            # SignalR実装
    │   └─ Program.cs             # バックエンドエントリーポイント
    ├─ FrontEnd.Wpf/              # WPFフロントエンド
    │   ├─ Views/                 # XAML View
    │   ├─ ViewModels/            # MVVM ViewModels
    │   └─ Services/              # API通信サービス
    └─ ApiClients/                # API通信クライアント
        ├─ Abstractions/          # API抽象化レイヤー
        ├─ ClientCore/            # 統合APIゲートウェイ
        ├─ RestClient/            # OpenAPI生成クライアント
        ├─ GrpcClient/            # gRPCクライアント
        └─ WebSocketClient/       # WebSocket/SignalRクライアント
```

## 機能

- Todoアイテムの作成・一覧取得・更新・削除（CRUD操作）
- REST APIによる基本的なデータ操作
- gRPCによるストリーミングデータ取得
- SignalRによるリアルタイム通知

## 環境構築

### 必要な環境

- .NET 9 SDK
- Visual Studio 2022 または VS Code

### ビルドと実行

```bash
# リポジトリのクローン
git clone <repository-url>
cd TodoApp

# ビルド
dotnet build

# アプリケーション実行（バックエンドとフロントエンドを同時に起動）
./run-local.sh
```

### 個別の実行方法

```bash
# バックエンド起動
cd src/Backend
dotnet run

# WPFクライアント起動（別のターミナルで）
cd src/FrontEnd.Wpf
dotnet run
```

## アーキテクチャ詳細

### 通信フロー

- REST API: 基本的なCRUD操作
- gRPC: Todoアイテムのストリーミング取得
- SignalR: Todoアイテムの追加・更新・削除時のリアルタイム通知

### 依存関係の方向

各レイヤー間の依存関係は、常に内側（ドメイン層）に向かうように設計されています。これにより、外部の実装詳細がドメインロジックに影響を与えることを防ぎます。

## 今後の拡張可能性

- Docker対応
- ユーザー認証
- データベースの永続化
- マルチプラットフォーム対応（MAUI/Blazor）
