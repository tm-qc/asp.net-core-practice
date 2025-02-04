// SamplesController のテストクラス
using Xunit;

// SamplesControllerをテストする場合にコンストラクタや処理に必要なusing文を追加
using Xunit; // xUnit (テストフレームワーク)
using Moq; // Moq (モック化ライブラリ)
using SelfAspNet.Controllers; // テスト対象のコントローラー
using SelfAspNet.Repository; // リポジトリインターフェース
using SelfAspNet.Models; // モデルを使用
using SelfAspNet.Lib; // Libを使用
using Microsoft.Extensions.Logging; // ロギングの依存関係
using Microsoft.Extensions.Options; // アプリ設定の依存関係
using Microsoft.EntityFrameworkCore; // データベースコンテキスト
using Microsoft.Extensions.Configuration; // 設定ファイル
using Microsoft.Extensions.Localization; // ローカライズの依存関係

namespace SelfAspNet.Tests
{
    public class SamplesControllerTests
    {
        private readonly SamplesController _samplesController;
        private readonly MyContext _context;

        public SamplesControllerTests()
        {

            // **テスト用の InMemory DB 作成**
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(databaseName: "SelfAspNet") // ✅ 修正
                .Options;

            _context = new MyContext(options); // ここで MyContext を適切に初期化

            // ✅ 依存関係を `Moq` でモック化
            var mockRepo = new Mock<ISampleRepository>();
            var mockOptions = new Mock<IOptions<MyAppOptions>>();
            var mockLogger = new Mock<ILogger<SamplesController>>();
            var mockLocalizer = new Mock<IStringLocalizer<SharedResource>>();

            // `Options` の `.Value` をセット
            mockOptions.Setup(opt => opt.Value).Returns(new MyAppOptions());

            // ✅ `SamplesController` にモックを渡す
            _samplesController = new SamplesController(
                _context,  // 実際の InMemoryDB を使った MyContext
                mockRepo.Object,
                new ConfigurationBuilder().Build(), // `IConfiguration` のモック
                mockOptions.Object,
                mockLogger.Object,
                mockLocalizer.Object
            );
        }

        [Fact]// テストメソッドであることを示す
        // 足し算のテスト
        public void AddWhenGivenTwoNumbersReturnsCorrectSum()
        {
            // Arrange（準備）
            int a = 5;
            int b = 3;

            // Act（実行）
            int result = _samplesController.Add(a, b);

            // Assert（検証）→ 期待する値と一致しているか
            Assert.Equal(8, result); // 5 + 3 = 8 になるはず
        }

        [Fact] 
        // 掛け算のテスト
        public void MultiplyWhenGivenTwoNumbersReturnsCorrectProduct()
        {
            // Arrange（準備）
            int a = 4;
            int b = 2;

            // Act（実行）
            int result = _samplesController.Multiply(a, b);

            // Assert（検証）
            Assert.Equal(8, result); // 4 * 2 = 8 になるはず
        }
    }
}