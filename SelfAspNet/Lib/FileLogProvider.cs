namespace SelfAspNet.Lib;

/// <summary>
/// ロガー本体をインスタンス化するための、ログプロバイダークラス
/// ※ロガー本体：SelfAspNet\Lib\FileLogger.cs
/// </summary>
public class FileLogProvider : ILoggerProvider
{
    private readonly string _filePath;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="filePath">ログの保存先のパス</param>
    public FileLogProvider(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// ロガーをインスタンス化する
    /// </summary>
    /// <param name="categoryName">発生元のクラス名</param>
    /// <returns>ロガー本体のインスタンス</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_filePath, categoryName);
    }

    public void Dispose() { }
}
