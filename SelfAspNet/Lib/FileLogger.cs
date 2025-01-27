namespace SelfAspNet.Lib;

/// <summary>
/// ロガー本体
/// </summary>
public class FileLogger : ILogger
{
    private readonly object _lockobj = new();
    private readonly string _filePath;
    private readonly string _category;

    public FileLogger(string filePath, string category)
    {
        _filePath = filePath;
        _category = category;
    }

    // 簡単化のため一旦
    // null、trueは固定でログレベルが全てのプロバイダーに共通の設定に従うようになる
    public IDisposable? BeginScope<TState>(TState state)
      where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <summary>
    /// ログ記録の本体
    /// </summary>
    /// <typeparam name="TState">ログエントリーの型</typeparam>
    /// <param name="logLevel">ログレベル</param>
    /// <param name="eventId">イベントID</param>
    /// <param name="state">記録されるログエントリー</param>
    /// <param name="exception">例外情報</param>
    /// <param name="formatter">引数state/exceptionを整形するフォーマッター</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
      Exception? exception, Func<TState, Exception?, string> formatter)
    {
        lock (_lockobj)
        {
            var path = Path.Combine(_filePath,
              $"core_{DateTime.Now.ToString("yyyy-MM-dd")}.log");
            var except = exception == null ? "" :
              $"\n{exception.GetType()}:{exception.Message}\n{exception.StackTrace}";
            File.AppendAllText(path,
              $"{logLevel} - {_category}（{eventId}） {DateTime.Now.ToString()} \n" +
              $"{formatter(state, exception)}{except}\n"
            );
        }
    }
}