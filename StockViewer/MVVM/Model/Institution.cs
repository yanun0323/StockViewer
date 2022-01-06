using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockViewer.MVVM.Model;
public struct Institution
{
    // [0] "證券代號"
    // [1] "證券名稱"
    // [2] "外資買進股數"
    // [3] "外資賣出股數"
    // [4] "外資買賣超股數"
    // [5] "投信買進股數"
    // [6] "投信賣出股數"
    // [7] "投信買賣超股數"
    // [8] "自營商買賣超股數"
    // [] "自營商買進股數"
    // [] "自營商賣出股數"
    // [last] "三大法人買賣超股數"

    // 2017/12/18 開始

    // [0]  "證券代號"                                 
    // [1]  "證券名稱"                               
    // [2]  "外陸資買進股數(不含外資自營商)"             
    // [3]  "外陸資賣出股數(不含外資自營商)"             
    // [4]  "外陸資買賣超股數(不含外資自營商)"          
    // [5]  "外資自營商買進股數"                       
    // [6]  "外資自營商賣出股數"                       
    // [7]  "外資自營商買賣超股數"                      
    // [8]  "投信買進股數"                             
    // [9]  "投信賣出股數"                             
    // [10] "投信買賣超股數"                           
    // [11] "自營商買賣超股數"                        
    // [12] "自營商買進股數(自行買賣)"                 
    // [13] "自營商賣出股數(自行買賣)"                 
    // [14] "自營商買賣超股數(自行買賣)"                
    // [15] "自營商買進股數(避險)"                     
    // [16] "自營商賣出股數(避險)"                     
    // [17] "自營商買賣超股數(避險)"                    
    // [last] "三大法人買賣超股數"                       

    public string ForeignBuy { get; init; }
    public string ForeignSell { get; init; }
    public string ForeignSuper { get; init; }
    public string ForeignDealerBuy { get; init; }
    public string ForeignDealerSell { get; init; }
    public string ForeignDealerSuper { get; init; }
    public string TrustBuy { get; init; }
    public string TrustSell { get; init; }
    public string TrustSuper { get; init; }
    public string DealerSuper { get; init; }
    public string InstitutionSuper { get; init; }

    [JsonIgnore]
    public int mForeignBuy { get => int.Parse(ForeignBuy.Replace(",", "")) / 1000; }
    [JsonIgnore]
    public int mForeignSell { get => int.Parse(ForeignSell.Replace(",", "")) / 1000; }
    [JsonIgnore]
    public int mForeignSuper { get => int.Parse(ForeignSuper.Replace(",", "")) / 1000; }
    [JsonIgnore]
    public int mForeignDealerBuy { get => int.Parse(ForeignDealerBuy.Replace(",", "")) / 1000; }
    [JsonIgnore]
    public int mForeignDealerSell { get => int.Parse(ForeignDealerSell.Replace(",", "")) / 1000; }
    [JsonIgnore]
    public int mForeignDealerSuper { get => int.Parse(ForeignDealerSuper.Replace(",", "")) / 1000; }
    [JsonIgnore]
    public int mTrustBuy { get => int.Parse(TrustBuy.Replace(",", "")) / 1000; }
    [JsonIgnore]
    public int mTrustSell { get => int.Parse(TrustSell.Replace(",", "")) / 1000; }
    [JsonIgnore]
    public int mTrustSuper { get => int.Parse(TrustSuper.Replace(",", "")) / 1000; }
    [JsonIgnore]
    public int mDealerSuper { get => int.Parse(DealerSuper.Replace(",", "")) / 1000; }
    [JsonIgnore]
    public int mInstitutionSuper { get => int.Parse(InstitutionSuper.Replace(",", "")) / 1000; }


}
