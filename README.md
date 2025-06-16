# digpet
## コンセプト
デジタルなペットとして動作するアプリケーション。
## 機能
- CPU使用率orカメラで人が検出されるとペットの機嫌がよくなる
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

## キャラファイル設定方法
**digpet**は配布されている(もしくは作成した)キャラファイルを読み込むことで、任意のキャラクターをペットにすることができます。
1. digpet.exeを実行します。
2. "インポート"ボタンが表示されるまで"詳細表示切替"ボタンをクリックします。
3. ファイル選択ダイアログが表示されるので、対象のzipファイルを選択します。

サンプルキャラファイル: [Moni_v2.01.00_light.zip](https://github.com/user-attachments/files/20743950/Moni_v2.01.00_light.zip)



## Webカメラを使用した機能の使用方法
通常、Digpetが実行されているコンピュータのCPU使用率を使用して、感情の計算を行います。<br>
この時、Webカメラを使用することで、顔の検出状態でインタラクティブに計算を行うこともできます。<br>
(DigpetがWebカメラから取得した画像は顔検出処理後に破棄され、画像を無断で保存したり、外部に送信することは決してありません。)<br>
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

## アプリ設定一覧
1. **(string)CharSettingPath**<br>
キャラファイルのパスが設定されます。<br>
設定するパスは絶対参照/相対参照問いません。

2. **(int)WindowState**<br>
起動時のウィンドウ状態です。<br>
0: 通常<br>
1: 最大化<br>
2: 最小化

3. **(bool)TopMost**<br>
ウィンドウのTopMostを切り替えます。

4. **(Size)WindowSize**<br>
起動時のウィンドウサイズを設定します。

5. **(Point)WindowLocation**<br>
起動時のウィンドウ位置を設定します。

6. **(int)FontEnlargeSize**<br>
0以上の値を設定することで、<br>
そのポイント分UIのサイズを拡大します。

7. **(Size)ImageSize**<br>
キャラクター画像のサイズです。<br>
特別な理由がない限り変更しないでください。

8. **(bool)EnableCameraMode**<br>
カメラモードの有効無効を切り替えます。

9. **(int)CameraId**<br>
画像を取得するカメラのIDです。

10. **(uint)CameraDisableThreshold**<br>
カメラ起動失敗時にCPUモードへ切り替えるための閾値です。<br>
1-10の値を設定し、値が大きくするほどカメラの起動失敗に対し寛容になります。<br>
10より大きい値を設定するとCPUモードに切り替わらなくなります。

11. **(bool)SaveTokenPlot**<br>
トークンプロット保存の有効無効を切り替えます。

12. **(long)GcThreshold**<br>
マネージドメモリがこの数値(bytes)を超えた際に、ガーベジコレクタを実行します。<br>

13. **(int)LogDeleteDays**<br>
ログファイルを残しておく日数です。

14. **(int)TokenCompressArrayElementIndex**<br>
トークンの計算時に使用される配列のインデックスです。<br>
基本的にはこの数値を上げるほど、プラスの感情を維持しやすくなります。

15. **(int)TokenBackupInterval**<br>
トークンファイルを保存する時間間隔を秒単で設定します。

16. **(bool)EnableNonActiveMode**<br>
非アクティブ(アプリの操作を一定時間行わない)時に、UIを非表示にする機能の有効無効を切り替えます。

17. **(int)NonActiveModeStartTime**<br>
非アクティブモードを開始するための時間を秒単位で設定します。

18. **(bool)EnableCameraDetectSmoothingMode**<br>
カメラモード起動時に検出結果の平滑化を行う機能の有効無効を切り替えます。

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
