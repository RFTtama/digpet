# digpet
## コンセプト
デジタルなペットとして動作するアプリケーション。
## 機能
- CPU使用率が高いほどペットがなつく(高度な動作は設定不可能)
- ペットは個人でカスタマイズ可能(高度なカスタマイズは不可能)
## 対応OS
- [x] Windows(11のみ動作確認済み) x86, x64

# 仕様
## インストール手順
1. [Releaseページ](https://github.com/RFTtama/digpet/releases)へアクセスします。
2. ダウンロードしたいバージョンのアセットから、使用しているCPUアーキテクチャに対応したzipファイルをダウンロードします。
3. ダウンロードしたzipファイルを解凍します。
4. 解凍されたフォルダ内にある**digpet.exe**をダブルクリックで実行します。
> [!NOTE]
> 実行がブロックされる場合は、アプリの**プロパティ**から**許可する**にチェックを入れて、再実行してください。
## 動作手順
1. **digpet.exe**をダブルクリックするとアプリが起動します。
2. トークンのリセット時刻の設定画面が表示されるため、適当な時刻を設定してください。
> [!NOTE]
> 普段PCを使用しない時刻の設定をおすすめします。
## キャラファイル設定方法
**digpet**は配布されている(もしくは作成した)キャラファイルを読み込むことで、任意のキャラクターをペットにすることができます。

# 依存関係
This project uses OpenCV, which is licensed under the Apache License 2.0.
The OpenCV package was built by [schimatk].
See [OpenCvSharp4](https://github.com/shimat/opencvsharp) and [OpenCV GitHub repository](https://github.com/opencv/opencv) for details.
