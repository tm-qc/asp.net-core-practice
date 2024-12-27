using System;

namespace SelfAspNet.Models;
using System.ComponentModel.DataAnnotations.Schema;// データベースのテーブル名を指定するために必要

// データベースのテーブル名が単体→複数形の関係で書けないときは指定する
// （テーブル名が単体系の名前は複数形のテーブルマッピングするので不要）
 [Table("SampleRelation1")] 
public class SampleRelation1
{
    public int Id { get; set; }
    // PascalCaseIdで書くと外部キーを自動で認識してくれる
    // snake_case_idで書くと外部キーを認識してくれないので[ForeignKey("SampleSample")]とアノテーションを書く必要がある
    public int SampleId { get; set; }
    public string RTitle { get; set; } = "";

    //1対多のリレーション(子→親) 

    // 子から見たら親は1つなので単体系の名前を指定
    // null許容型の警告が出るが、既定値もないのでnull!として、既定値が入るまでnullを許容するようにしている
    public Sample Sample { get; set; } = null!;
}
