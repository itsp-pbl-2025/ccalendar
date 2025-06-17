## GeminiCodeAssist機能について

GeminiCodeAssistは、GoogleのGemini AIを利用してコードの補完や提案を行う機能です。本リポジトリにも導入されており、Pull Requestのコードレビューを効率化します。

### 機能の概要

GeminiCodeAssistは、以下のような機能を提供します：

- **PRの自動レビュー**: Pull Requestに対して自動的にコードレビューを行い、改善点や提案を提示します。
- **コマンドの実行**: 特定のコマンドをPull Requestのコメントとして投稿することでGeminiCodeAssistとの対話が可能です。コマンドの一覧は次のとおりです：

  - `/gemini summary`: Pull Requestの概要を生成します。
  - `/gemini review`: Pull Request全体のコードレビューを行います。
  - `@gemini-code-assist`: Pull Requestのコメントでメンションすることで、
    GeminiCodeAssistに対して質問や特定のコードに関するフィードバックを求めることができます。

### 設定

GeminiCodeAssistの設定には、`.gemini`ディレクトリを用います。
`.gemini`ディレクトリには、以下のファイルが含まれています：

- `styleguide.md`: コードレビューのスタイルガイド。レビューの基準や方針を定義しています。
- `config.yaml`: GeminiCodeAssistの設定ファイル。APIキーや動作モードなどを指定します。