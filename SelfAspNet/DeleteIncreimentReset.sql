-- SamplesとSampleRelation1テーブルを空にして、オートインクリメントをリセットする
-- DBCCはSQL Server専用なので、MySQLの場合は使えない
DELETE FROM SampleRelation1;
DBCC CHECKIDENT ('SampleRelation1', RESEED, 0);
DELETE FROM Samples;
DBCC CHECKIDENT ('Samples', RESEED, 0)