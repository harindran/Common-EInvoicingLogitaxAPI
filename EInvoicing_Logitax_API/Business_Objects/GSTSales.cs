using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoicing_Logitax_API.Business_Objects
{
    public class GSTSales
    {
        public string Version { get; set; }
        public GTranDtls TranDtls { get; set; } = new GTranDtls();
        public GDocDtls DocDtls { get; set; } = new GDocDtls();
        public GSellerDtls SellerDtls { get; set; } = new GSellerDtls();
        public GBuyerDtls BuyerDtls { get; set; } = new GBuyerDtls();
        public List<GItemList> ItemList { get; set; } = new List<GItemList>();
        public GValDtls ValDtls { get; set; } = new GValDtls();
        public GExpDtls ExpDtls { get; set; } = new GExpDtls();
        public GEwbDtls EwbDtls { get; set; } = new GEwbDtls();
        public GExtdtls Extdtls { get; set; } = new GExtdtls();
    }
    
    public class GBuyerDtls
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string CusCode { get; set; }
        public string Pos { get; set; }
        public string Stcd { get; set; }
        public string Em { get; set; }
    }

    public class GDocDtls
    {
        public string Typ { get; set; }
        public string No { get; set; }
        public string Dt { get; set; }
    }

    public class GEwbDtls
    {
        public string EwayBillNo { get; set; }
        public string EwayBillDate { get; set; }
    }

    public class GExpDtls
    {
        public string ShipBNo { get; set; }
        public string ShipBDt { get; set; }
    }

    public class GExtdtls
    {
        public string irnnumber { get; set; }
        public string irndate { get; set; }
    }

    public class GItemList
    {
        public string SlNo { get; set; }
        public string HsnCd { get; set; }
        public string Qty { get; set; }
        public string Unit { get; set; }
        public string UnitPrice { get; set; }
        public string AssAmt { get; set; }
        public string GstRt { get; set; }
        public string IgstAmt { get; set; }
        public string CgstAmt { get; set; }
        public string SgstAmt { get; set; }
        public decimal CesRt { get; set; }
        public decimal CesAmt { get; set; }
        public decimal CesNonAdvlAmt { get; set; }
        public decimal StateCesRt { get; set; }
        public decimal StateCesAmt { get; set; }
        public decimal StateCesNonAdvlAmt { get; set; }
        public string TotItemVal { get; set; }
    }
  
    public class GSellerDtls
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string Stcd { get; set; }
        public string Em { get; set; }
    }

    public class GTranDtls
    {
        public string SupTyp { get; set; }
        public string RegRev { get; set; }
        public string IgstOnIntra { get; set; }
    }

    public class GValDtls
    {
        public decimal AssVal { get; set; }
        public decimal CgstVal { get; set; }
        public decimal SgstVal { get; set; }
        public decimal IgstVal { get; set; }
        public decimal CesVal { get; set; }
        public decimal CesNonAdVal { get; set; }
        public decimal StCesVal { get; set; }
        public decimal Discount { get; set; }
        public decimal OthChrg { get; set; }
        public decimal RndOffAmt { get; set; }
        public decimal TotInvVal { get; set; }
        public decimal TotInvValFc { get; set; }
    }


}
