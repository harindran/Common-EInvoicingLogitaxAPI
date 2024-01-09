
using System;
using System.Collections.Generic;

namespace EInvoicing_Logitax_API.Business_Objects
{
    #region Generate_IRN

    public class saplogin
    {
    public string CompanyDB { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
}

    public class GenerateIRN 
    {
        public string client_code { get; set; }
        public string user_code { get; set; }
        public string password { get; set; }
        public string Url { get; set; }
        public JsonData json_data { get; set; } = new JsonData();
    }

    public class AddlDocDtl
    {
        public string Docs { get; set; }
        public string Info { get; set; }

        public string Url { get; set; }
    }

    public class AttribDtl
    {
        public string Nm { get; set; }
        public string Val { get; set; }
    }

    public class BchDtls
    {
        public string Nm { get; set; }
        public string ExpDt { get; set; }
        public string WrDt { get; set; }
    }

    public class BuyerDtls 
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string TrdNm { get; set; }
        public string Pos { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public string Pin { get; set; }
        public string Stcd { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }

        public string CusCode { get; set; }

       
    }

    public class ContrDtl
    {
        public string RecAdvRefr { get; set; }
        public string RecAdvDt { get; set; }
        public string TendRefr { get; set; }
        public string ContrRefr { get; set; }
        public string ExtRefr { get; set; }
        public string ProjRefr { get; set; }
        public string PORefr { get; set; }
        public string PORefDt { get; set; }
    }

    public class DispDtls
    {
        public string Nm { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public string Pin { get; set; }
        public string Stcd { get; set; }

        public string Gstin { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }
    }

    public class DocDtls
    {
        public string Typ { get; set; }
        public string No { get; set; }
        public string Dt { get; set; }

        public string ErpInvNo { get; set; }
        public string AccountingDt { get; set; }
    }

    public class DocPerdDtls
    {
        public string InvStDt { get; set; }
        public string InvEndDt { get; set; }
    }

    public class EwbDtls
    {
        public string TransId { get; set; }
        public string TransName { get; set; }
        public decimal Distance { get; set; }
        public string TransDocNo { get; set; }
        public string TransDocDt { get; set; }
        public string VehNo { get; set; }
        public string VehType { get; set; }
        public string TransMode { get; set; }

        public  string EwayBillNo { get; set; }
        public  string EwayBillDate { get; set; }
    }

    public class ExpDtls
    {
        public string ShipBNo { get; set; }
        public string ShipBDt { get; set; }
        public string Port { get; set; }
        public string RefClm { get; set; }
        public string ForCur { get; set; }
        public string CntCode { get; set; }
        public object ExpDuty { get; set; }


        public string billofentryno { get; set; }
        public string billofentrydate { get; set; }

      
   
    }

    public class ItemList
    {
        public string SlNo { get; set; }
        public string PrdDesc { get; set; }
        public string IsServc { get; set; }
        public string HsnCd { get; set; }
        public string Barcde { get; set; }
        public string Qty { get; set; }
        public decimal FreeQty { get; set; }
        public string Unit { get; set; }
        public string UnitPrice { get; set; }
        public string TotAmt { get; set; }
        public string Discount { get; set; }
        public decimal PreTaxVal { get; set; }
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
        public decimal OthChrg { get; set; }
        public string TotItemVal { get; set; }
        public string OrdLineRef { get; set; }
        public string OrgCntry { get; set; }
        public string PrdSlNo { get; set; }
        public BchDtls BchDtls { get; set; } = new BchDtls();
        public List<AttribDtl> AttribDtls = new List<AttribDtl>();

        public string PrdNm { get; set; }
        public string ItcClaimType { get; set; }
        public string eligibilityofitc { get; set; }       
    }

    public class JsonData
    {
        public string Version { get; set; }
        public TranDtls TranDtls { get; set; } = new TranDtls();
        public DocDtls DocDtls { get; set; } = new DocDtls();
        public SellerDtls SellerDtls { get; set; } = new SellerDtls();
        public BuyerDtls BuyerDtls { get; set; } = new BuyerDtls();
        public DispDtls DispDtls { get; set; } = new DispDtls();
        public ShipDtls ShipDtls { get; set; } = new ShipDtls();
        public List<ItemList> ItemList = new List<ItemList>();
        public ValDtls ValDtls { get; set; } = new ValDtls();
        public PayDtls PayDtls { get; set; } = new PayDtls();
        public RefDtls RefDtls { get; set; } = new RefDtls();
        public List<AddlDocDtl> AddlDocDtls = new List<AddlDocDtl>();
        public ExpDtls ExpDtls { get; set; } = new ExpDtls();
        public EwbDtls EwbDtls { get; set; } = new EwbDtls();

        public Extdtls Extdtls { get; set; } = new Extdtls();

      
    }

    public class PayDtls
    {
        public string Nm { get; set; }
        public string AccDet { get; set; }
        public string Mode { get; set; }
        public string FinInsBr { get; set; }
        public string PayTerm { get; set; }
        public string PayInstr { get; set; }
        public string CrTrn { get; set; }
        public string DirDr { get; set; }
        public int CrDay { get; set; }
        public int PaidAmt { get; set; }
        public int PaymtDue { get; set; }

        public string PayDueDt { get; set; }
    }

    public class PrecDocDtl
    {
        public string InvNo { get; set; }
        public string InvDt { get; set; }
        public string OthRefNo { get; set; }
    }

    public class RefDtls
    {
        public string InvRm { get; set; }
        public DocPerdDtls DocPerdDtls { get; set; } = new DocPerdDtls();
        public List<PrecDocDtl> PrecDocDtls = new List<PrecDocDtl>();
        public List<ContrDtl> ContrDtls = new List<ContrDtl>();
    }

    public class SellerDtls
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string TrdNm { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public string Pin { get; set; }
        public string Stcd { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }
    }

    public class ShipDtls
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string TrdNm { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public string Pin { get; set; }
        public string Stcd { get; set; }
    }

    public class TranDtls
    {
        public string TaxSch { get; set; }
        public string SupTyp { get; set; }
        public string RegRev { get; set; }
        public object EcmGstin { get; set; }
        public string IgstOnIntra { get; set; }


        public string IsPurchase { get; set; }

        
    }

    public class ValDtls
    {
        public decimal AssVal { get; set; }
        public decimal CgstVal { get; set; }
        public decimal SgstVal { get; set; }
        public decimal IgstVal { get; set; }
        public decimal CesVal { get; set; }
        public decimal StCesVal { get; set; }
        public decimal Discount { get; set; }
        public decimal OthChrg { get; set; }
        public decimal RndOffAmt { get; set; }
        public decimal TotInvVal { get; set; }
        public decimal TotInvValFc { get; set; }

        public decimal CesNonAdVal { get; set; }
    }

    #endregion

    #region Cancel    
    
    public class Cancelledeinvoicelist
    {
        public string Irn { get; set; }
        public string CnlRem { get; set; }
        public int CnlRsn { get; set; }
    }
    public class cancelledewblist
    {
        public string EwbNo { get; set; }
        public string CancelledReason { get; set; }
        public string CancelledRemarks { get; set; }
    }
    public class cancelledeinvoiceewblist
        {
        public string ewbNo { get; set; }
        public string cancelRsnCode { get; set; }
        public string cancelRmrk { get; set; }
     }
public class ClientCred_Cancel
    {
        public string CLIENTCODE { get; set; }
        public string USERCODE { get; set; }
        public string PASSWORD { get; set; }
        public List<Cancelledeinvoicelist> cancelledeinvoicelist = new List<Cancelledeinvoicelist>();
        public List<cancelledewblist> cancelledewblist = new List<cancelledewblist>();
        public List<cancelledeinvoiceewblist> cancelledeinvoiceewblist= new List<cancelledeinvoiceewblist>();
    
}


class ClsCancelEInvoice
    {
        public Cancelledeinvoicelist Cancelledeinvoicelist { get; set; }
        public ClientCred_Cancel ClientCred { get; set; }
    }

    #endregion

    #region GetIRN_DocNum

    public class Docdetailslist
    {
        public string DocType { get; set; }
        public string DocNum { get; set; }
        public string DocDate { get; set; }
    }

    public class ClienCred_GetIRN_DocNum
    {
        public string CLIENTCODE { get; set; }
        public string USERCODE { get; set; }
        public string PASSWORD { get; set; }
        public string RequestorGSTIN { get; set; }
        public List<Docdetailslist> docdetailslist = new List<Docdetailslist>();      
    }

    #endregion

    #region GetIRN
    public class Irnlist
    {
        public string IRN { get; set; }
    }  
       
    public class GetIRN
    {
        public string USERCODE { get; set; }
        public string CLIENTCODE { get; set; }
        public string PASSWORD { get; set; }
        public string RequestorGSTIN { get; set; }        
        public List<Irnlist> IRNList = new List<Irnlist>();
    }
    #endregion

    public class Vehicleupdatelist
    {
        public string ewbNo { get; set; }
        public string vehicleNo { get; set; }
        public string fromPlace { get; set; }
        public string fromState { get; set; }
        public string reasonCode { get; set; }
        public string reasonRem { get; set; }
        public string transDocNo { get; set; }
        public string transDocDate { get; set; }
        public string transMode { get; set; }
        public string vehicleType { get; set; }
    }

    public class Ewbeinvoicelist
    {
        public string Irn { get; set; }
        public string Distance { get; set; }
        public string TransMode { get; set; }
        public string TransId { get; set; }
        public string TransName { get; set; }
        public string TransDocDt { get; set; }
        public string TransDocNo { get; set; }
        public string VehNo { get; set; }
        public string VehType { get; set; }
        public DispDtls DispDtls { get; set; } = new DispDtls();
        public ShipDtls ExpShipDtls { get; set; } = new ShipDtls();

    }

    public class GetEwayByIRN
    {
        public string CLIENTCODE { get; set; }
        public string USERCODE { get; set; }
        public string PASSWORD { get; set; }
        public List<Ewbeinvoicelist> ewbeinvoicelist = new List<Ewbeinvoicelist>();
    }
    public class UpdateEway
    {
        public string CLIENTCODE { get; set; }
        public string USERCODE { get; set; }
        public string PASSWORD { get; set; }
        public List<Vehicleupdatelist> Vehicleupdatelist = new List<Vehicleupdatelist>();
        public List<transporteridlist> transporteridlist = new List<transporteridlist>();
    }
    public class transporteridlist
    {
        public string ewbNo { get; set; }
        public string transporterId { get; set; }       
    }

    public class Extdtls
    {
        public string irnnumber { get; set; }
        public string irndate { get; set; }
        public string PlantCode { get; set; }
        public string glcodesales { get; set; }
        public string glcodeliability { get; set; }
        public string dateoflinkadvpayment { get; set; }
        public string vnooflinkedadvpayment { get; set; }
        public int adjamtoflinkadvpayment { get; set; }
        public string businessarea { get; set; }
        public string iscancelled { get; set; }
        public string is_ammendment { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }

        public string returnfilingmonth { get; set; }   
    }
    

    public class Token
    {
        public string data { get; set; }
    }

    public class Statuscheck
    {
        public string ClientCode { get; set; }
        public string UserCode { get; set; }
        public string GSTIN { get; set; }
        public string FormType { get; set; }
        public string ReferenceId { get; set; }
        public string FileCount { get; set; }
 

}
   
}

