# VRCLocalVideoPlayer

VRChatのログから動画URLを読み取り、WebView2ブラウザ、またはm3u8の場合VLCプレイヤーで表示するアプリケーション。

## 用途

VRChatの動画プレイヤーが謎の理由で再生できない場合に、URLを直接開いて動画を再生します。

ログを確認してURL入力まで自動で処理します。

## 注意事項

- 未完成です。
- VLCプレイヤーの起動は検証不足です。
- ログファイルに再生時間の情報がないので途中から再生の場合は同期できません。
- VRCのインスタンス内に動画を正常に再生できる人がいないと次の動画に進みません。
- YouTubeの仕様変更にuBlockOrigin(Edge)が追い付かない場合、広告が出る可能性があります。
- 拡張機能の自動更新は未対応です。Extensionsの中を削除すると再ダウンロードします。
- 連続で同じ動画の再生には未対応です。
- VRC内での動画停止は反映されません。
- 同じVRCインスタンス内に動画プレイヤーが2個以上ある場合、正常に再生できません。
- YouTube以外のURLは動作未検証です。

## 開発環境

Microsoft Visual Studio Community 2022 (64 ビット) - Current

C# / .NET Framework 4.8.1

## 誰が使うねん

本当だよ  
私みたいな謎のエラーに悩まされている人しか使わんわ
