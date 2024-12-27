DELETE FROM Samples;
DBCC CHECKIDENT ('Samples', RESEED, 0)

-- SelfAspNet データベースを使用（無くても動く）
-- USE SelfAspNet;

-- Samples テーブルにデータを挿入
-- (NはSQL Serverの場合(MySQLは不要)にunicode文字列を示すということをしないといけないので必要らしい)
-- 日本語のデータを扱うとき、N を付けないとエラーや文字化けが発生する可能性があるため、この書き方は推奨されます。

-- INSERTをまとめるメリット
-- Insert文をまとめることで、1つのトランザクションで処理を行うことができるので、整合性が保てる
-- (全部失敗 or　全部成功)
-- Insertを複数回の方がオーバーヘッドが少ない
INSERT INTO Samples (Title, SubTitle)
VALUES
    (N'1番目のタイトル', N'1番目のサブタイトル'),
    (N'2番目のタイトル', N'2番目のサブタイトル'),
    (N'3番目のタイトル', N'3番目のサブタイトル'),
    (N'4番目のタイトル', N'4番目のサブタイトル'),
    (N'5番目のタイトル', N'5番目のサブタイトル');

SELECT * FROM Samples;


DELETE FROM SampleRelation1;
DBCC CHECKIDENT ('SampleRelation1', RESEED, 0);

INSERT INTO SampleRelation1 (SampleId, RTitle)
SELECT Id, N'リレーションタイトル' + CAST(Id AS NVARCHAR(10))
FROM Samples;

SELECT * FROM SampleRelation1;