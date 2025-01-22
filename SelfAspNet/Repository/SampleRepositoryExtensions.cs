using System;

namespace SelfAspNet.Repository;

/// <summary>
/// ちなみに・・IServiceCollectionをつかったメソッドを定義しておくと、
/// Program.csでのリポジトリ登録が短く済むとのこと
/// 
/// 個人的にややこしい、直感的じゃないのであまり使わなくていい印象
/// </summary>
public static class SampleRepositoryExtensions
{
    public static IServiceCollection AddSampleRepository(
        this IServiceCollection services)
    {
        return services.AddTransient<ISampleRepository, SampleRepository>();
    }
}