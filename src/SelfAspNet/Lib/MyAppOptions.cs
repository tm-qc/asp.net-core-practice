using System;
using System.ComponentModel.DataAnnotations;

namespace SelfAspNet.Lib;

/// <summary>
/// 構成情報を取得する際に型をマッピングするためのクラス
/// </summary>
public class MyAppOptions
{
    [MaxLength(50)]
    public string Title{get; set;} = string.Empty;
    public int No{get; set;} = 0;
    public DateTime Published{get; set;} = DateTime.MinValue;// DateTime の最小値で初期化
    public List<string> Projects{get; set;} = [];
    public string About{get; set;} = string.Empty;
}
