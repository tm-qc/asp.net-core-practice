namespace SelfAspNet.Lib;

/// <summary>
/// プロバイダをProgram.csで登録するための拡張クラス
/// </summary>
public static class FileLoggerExtensions
{

    /// <summary>
    /// プロバイダをProgram.csで登録するためのメソッド
    /// ※これはお作法なので、このまま使うことが多い
    /// ※FileLogProviderはSelfAspNet\Lib\FileLogProvider.cs
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="filePath"></param>
    /// <returns>ILoggingBuilder</returns>
    public static ILoggingBuilder AddFile(
      this ILoggingBuilder builder, string filePath)
    {
        builder.AddProvider(new FileLogProvider(filePath));
        return builder;
    }
}