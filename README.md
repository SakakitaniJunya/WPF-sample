# ローカルバックエンド連携 WPF アプリケーション

## 設計書

### システム概要

本システムは、ローカルPCで起動するバックエンドサーバーとWPFクライアント間の通信を実現するアプリケーションです。ドメイン駆動設計(DDD)とクリーンアーキテクチャを採用し、REST API、gRPC、SignalRによる双方向通信を実装しています。

### アーキテクチャ図

```
+---------------------+      HTTP/gRPC/WebSocket      +---------------------+
|   WPF Client        |  <--------------------------> | Local Backend API   |
| (.NET 9 / MVVM)     |                               | (.NET 9 / ASP.NET   |
|                     |                               |        Core)        |
+---------------------+                               +---------------------+
         |                                                      |
         v                                                      v
+---------------------+                               +---------------------+
| API Client Service  |                               | Application Layer   |
| (REST+gRPC+SignalR) |                               | (MediatR Commands)  |
+---------------------+                               +---------------------+
                                                                |
                                                                v
                                                      +---------------------+
                                                      | Domain Layer        |
                                                      | (Entities, Services)|
                                                      +---------------------+
                                                                |
                                                                v
                                                      +---------------------+
                                                      | Infrastructure      |
                                                      | (EF Core, Repos)    |
                                                      +---------------------+
```

### プロジェクト構成図

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

### 使用技術

| 区分 | 技術 |
|------|------|
| バックエンド | ASP.NET Core, EF Core InMemory, MediatR, gRPC, SignalR |
| フロントエンド | WPF (.NET 9), MVVM, CommunityToolkit.Mvvm |
| 通信 | REST API, gRPC, SignalR (WebSocket) |
| コード生成 | OpenAPI Generator |
| 依存性注入 | Microsoft.Extensions.DependencyInjection |

### 主要コンポーネント詳細

#### ドメイン層 (Domain)

**TodoItem**：Todoアイテムのエンティティ
- 属性：Id, Title, IsDone, CreatedAt, UpdatedAt
- 操作：MarkAsDone, MarkAsUndone, UpdateTitle

**ITodoRepository**：リポジトリインターフェース
- 操作：GetAllAsync, GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync, SaveChangesAsync

#### アプリケーション層 (Application)

**MediatRコマンド/クエリ**
- GetAllTodosQuery：Todo一覧取得
- GetTodoByIdQuery：ID指定でTodo取得
- CreateTodoCommand：新規Todo作成
- UpdateTodoCommand：既存Todo更新
- DeleteTodoCommand：Todo削除

#### バックエンド層 (Backend)

**TodoDbContext**：EF Core DbContext
- インメモリデータベース使用

**TodoRepository**：リポジトリ実装
- ITodoRepositoryの実装

**API実装**
- REST API：TodosController (RestApi/Controllers)
- gRPC：TodoGrpcService (GrpcServices)
- SignalR：TodoHub (WebSockets/Hubs)

#### API抽象化レイヤー (ApiClients/Abstractions)

**ITodoApiClient**：REST API抽象化インターフェース
- 操作：GetAllTodosAsync, GetTodoByIdAsync, CreateTodoAsync, UpdateTodoAsync, DeleteTodoAsync

**ITodoStreamClient**：gRPCストリーミング抽象化インターフェース
- 操作：StreamTodosAsync

**ITodoNotificationClient**：SignalR通知抽象化インターフェース
- イベント：TodoAdded, TodoUpdated, TodoDeleted
- 操作：ConnectAsync, DisconnectAsync

#### APIクライアント実装

**OpenApiTodoClient**：REST APIクライアント (ApiClients/RestClient)
- OpenAPI Generator生成コードのラッパー

**GrpcTodoClient**：gRPCクライアント (ApiClients/GrpcClient)
- Grpc.Net.Clientを使用した実装

**SignalRTodoClient**：SignalRクライアント (ApiClients/WebSocketClient)
- SignalR.Clientを使用した実装

**TodoApiGateway**：統合ゲートウェイ (ApiClients/ClientCore)
- 複数通信方式を統合したAPI窓口

#### WPFフロントエンド (FrontEnd.Wpf)

**MainViewModel**：メイン画面ViewModel
- APIゲートウェイ経由でバックエンドと通信

**TodoItemViewModel**：Todo個別項目ViewModel
- Todo要素の表示と操作を担当

**MainWindow**：メイン画面View
- TodoリストUI、操作ボタン、ステータス表示

### 通信フロー図

#### REST API

| メソッド | パス | 説明 |
|----------|------|------|
| GET | /api/todos | Todo一覧取得 |
| GET | /api/todos/{id} | ID指定でTodo取得 |
| POST | /api/todos | 新規Todo追加 |
| PUT | /api/todos/{id} | Todo更新 |
| DELETE | /api/todos/{id} | Todo削除 |

#### gRPC

```
service TodoService {
  rpc StreamTodos (google.protobuf.Empty) returns (stream TodoDto);
}
```

#### WebSocket (SignalR)

| イベント | 説明 |
|----------|------|
| TodoAdded | 新規Todoが追加された |
| TodoUpdated | Todoが更新された |
| TodoDeleted | Todoが削除された |

### OpenAPI生成フロー

```
┌────────────────┐   swagger.json   ┌───────────────────┐
│  Backend API   │ ──────────────> │ OpenAPI Generator │
│  (SwaggerUI)   │                  │       Tool        │
└────────────────┘                  └─────────┬─────────┘
                                              │
                                              │ 生成
                                              ▼
                                   ┌───────────────────────┐
                                   │  RestClient/Generated │
                                   │     C# Client Code    │
                                   └───────────────────────┘
```

### 拡張性と保守性のポイント

1. **レイヤー分離**：各レイヤーが明確に分離されており、変更影響が限定的
2. **インターフェース抽象化**：具体的な実装から抽象化が分離されている
3. **依存性の方向**：依存は常に内側（ドメイン方向）を向いている
4. **API抽象化**：通信プロトコルの詳細がクライアントから隠蔽されている
5. **イベント駆動**：SignalRを用いたイベント駆動設計により、リアルタイム性を確保

## 起動手順

### 環境要件

- .NET 9 SDK
- Visual Studio 2022 または VS Code（推奨）

### 初回セットアップ

プロジェクトをダウンロードまたはクローンしたら、最初にNuGetパッケージを復元します。

```bash
cd /Users/sakaki/projects/TodoApp
dotnet restore
```

### ビルド方法

```bash
# プロジェクト全体をビルド
dotnet build
```

### アプリケーション実行（方法1: 個別起動）

#### 1. バックエンドサーバーの起動

```bash
cd /Users/sakaki/projects/TodoApp/src/Backend
dotnet run
```

バックエンドが起動したら、以下のURLでSwagger UIにアクセスできます：
http://localhost:5000/swagger

#### 2. WPFクライアントの起動（別ターミナルで）

```bash
cd /Users/sakaki/projects/TodoApp/src/FrontEnd.Wpf
dotnet run
```

WPFアプリケーションが起動し、自動的にバックエンドに接続します。

### アプリケーション実行（方法2: スクリプト使用）

提供されているスクリプトを使用して、両方のコンポーネントを同時に起動できます。

```bash
cd /Users/sakaki/projects/TodoApp
chmod +x run-local.sh  # 初回のみ必要（実行権限付与）
./run-local.sh
```

このスクリプトは、バックエンドを起動し、その後WPFクライアントを起動します。WPFクライアントを終了すると、バックエンドも自動的に終了します。

### OpenAPIクライアント生成

バックエンドが実行中の状態で、以下のコマンドを実行してREST APIクライアントコードを生成します：

```bash
cd /Users/sakaki/projects/TodoApp/tools/OpenApiGenerator
dotnet run
```

生成されたコードは `src/ApiClients/RestClient/Generated` ディレクトリに保存されます。

## アプリケーションの使用方法

1. アプリケーション起動後、「接続」ボタンをクリックしてバックエンドに接続します
2. 上部のテキストボックスにTodoタイトルを入力し、「追加」ボタンでTodoを作成
3. 一覧からTodoアイテムのチェックボックスをオン/オフしてステータスを変更
4. 「編集」ボタンでTodoのタイトルを変更
5. 「削除」ボタンでTodoを削除
6. 「更新」ボタンで最新のTodoリストを取得

すべての変更はリアルタイムで他のクライアントにも反映されます（SignalR経由）。
