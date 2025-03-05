using System.Security.Cryptography;
using System.Text;

namespace digpet.Modules
{
    internal static class CryptLib
    {
        /// <summary>
        /// 文字列を暗号化する
        /// </summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <returns>暗号化された文字列</returns>
        public static string EncryptString(string sourceString, string password)
        {
            //RijndaelManagedオブジェクトを作成
            Aes rijndael = Aes.Create();

            //パスワードから共有キーと初期化ベクタを作成
            byte[] key, iv;
            GenerateKeyFromPassword(
                password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
            rijndael.Key = key;
            rijndael.IV = iv;

            //文字列をバイト型配列に変換する
            byte[] strBytes = Encoding.UTF8.GetBytes(sourceString);

            //対称暗号化オブジェクトの作成
            ICryptoTransform encryptor =
                rijndael.CreateEncryptor();
            //バイト型配列を暗号化する
            byte[] encBytes = encryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
            //閉じる
            encryptor.Dispose();

            //バイト型配列を文字列に変換して返す
            return Convert.ToBase64String(encBytes);
        }

        /// <summary>
        /// 暗号化された文字列を復号化する
        /// </summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <returns>復号化された文字列</returns>
        public static string DecryptString(string sourceString, string password)
        {
            //RijndaelManagedオブジェクトを作成
            Aes rijndael = Aes.Create();

            //パスワードから共有キーと初期化ベクタを作成
            byte[] key, iv;
            GenerateKeyFromPassword(
                password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
            rijndael.Key = key;
            rijndael.IV = iv;

            //文字列をバイト型配列に戻す
            byte[] strBytes = Convert.FromBase64String(sourceString);

            //対称暗号化オブジェクトの作成
            ICryptoTransform decryptor =
                rijndael.CreateDecryptor();
            //バイト型配列を復号化する
            //復号化に失敗すると例外CryptographicExceptionが発生
            byte[] decBytes = decryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
            //閉じる
            decryptor.Dispose();

            //バイト型配列を文字列に戻して返す
            return Encoding.UTF8.GetString(decBytes);
        }

        /// <summary>
        /// パスワードから共有キーと初期化ベクタを生成する
        /// </summary>
        /// <param name="password">基になるパスワード</param>
        /// <param name="keySize">共有キーのサイズ（ビット）</param>
        /// <param name="key">作成された共有キー</param>
        /// <param name="blockSize">初期化ベクタのサイズ（ビット）</param>
        /// <param name="iv">作成された初期化ベクタ</param>
        private static void GenerateKeyFromPassword(string password,
            int keySize, out byte[] key, int blockSize, out byte[] iv)
        {
            //パスワードから共有キーと初期化ベクタを作成する
            //saltを決める
            byte[] salt = Encoding.UTF8.GetBytes("saltは必ず8バイト以上");
#if false
            //Rfc2898DeriveBytesオブジェクトを作成する
            Rfc2898DeriveBytes deriveBytes =
                new Rfc2898DeriveBytes(password, salt);
#endif

#if true
            //.NET Framework 1.1以下の時は、PasswordDeriveBytesを使用する
            PasswordDeriveBytes deriveBytes =
                new PasswordDeriveBytes(password, salt);
#endif
            //反復処理回数を指定する デフォルトで1000回
            deriveBytes.IterationCount = 1000;

            //共有キーと初期化ベクタを生成する
            key = deriveBytes.GetBytes(keySize / 8);
            iv = deriveBytes.GetBytes(blockSize / 8);
        }
    }
}
