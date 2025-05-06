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

> [!NOTE]
> 誤った時刻を設定してしまった場合は、設定ファイル(settings.json)のResetHourを変更してください。
## キャラファイル設定方法
**digpet**は配布されている(もしくは作成した)キャラファイルを読み込むことで、任意のキャラクターをペットにすることができます。
1. digpet.exeを実行します。
2. "インポート"ボタンが表示されるまで"詳細表示切替"ボタンをクリックします。
3. ファイル選択ダイアログが表示されるので、対象のzipファイルを選択します。

サンプルキャラファイル: [Moni V1.00.02.zip](https://github.com/user-attachments/files/18813168/Moni.V1.00.02.zip)

## Webカメラを使用した機能の使用方法
1. アプリを終了します。(起動中の場合)
2. digpet.exeの保存されているディレクトリから、config.jsonを探します。
3. config.jsonをテキストエディタで開きます。
4. EnableCameraModeを探します。
5. EnableCameraModeの値を"false"から"true"に変更します。
6. アプリを起動します。

> [!NOTE]
> 機能を有効にしても、顔の検出が無しのままである場合は、有効なカメラが選択されていない可能性があります。
> config.jsonのCameraIdの値を変更して、再度起動してください。

> [!NOTE]
> 数秒間カメラモードになり、すぐにCPUモードに変わってしまう場合は、
> config.jsonのCameraDisableThresholdの数値を変更してください。

## Licenses
> This project uses OpenCV, which is licensed under the Apache License 2.0.<br>
> The OpenCV package was built by schimatk.<br>
> See [OpenCvSharp4](https://github.com/shimat/opencvsharp) and [OpenCV GitHub repository](https://github.com/opencv/opencv) for details.<br>

> This project uses the following libraries licensed under the MIT License:
> 
> - [scottPlot](https://github.com/ScottPlot/ScottPlot)
> 
> The copyright of these libraries belongs to their respective authors.  
> For details about the MIT License of each library, please refer to the official repository or the provided license file.
