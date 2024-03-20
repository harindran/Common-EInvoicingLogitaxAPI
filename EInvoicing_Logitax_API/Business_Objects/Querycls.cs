using EInvoicing_Logitax_API.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoicing_Logitax_API.Business_Objects
{
    public class Querycls
    {
        public int HSNLength = 4;

        public string docseries = "CONCAT(CAST(COALESCE( nnm1.\"BeginStr\",'') AS varchar(100)), CAST(a.\"DocNum\" AS varchar(100)))";
        public string InvoiceQuery(string Docentry)
        {
            string retstring = "";

            retstring = "SELECT Row_number() Over(Partition by b.\"DocEntry\" order by b.\"DocEntry\" Asc )\" SINo\",(B.\"AssVal\"+B.\"Freight Total\") \"AssValN\" ,";
            retstring = retstring + " (B.\"AssAmt\"+B.\"CGSTAmt\"+B.\"SGSTAmt\"+B.\"IGSTAmt\") \"Total Item Value\",CAST((B.\"Tot Amt\"-B.\"Tot Amt1\")AS nvarchar(200)) \"LineDiscountAmt\",B.* FROM(";
            retstring = retstring + "  Select B2.*, 'GST' as \"TaxSch\",T.\"BpGSTN\" as \"T_BpGSTN\",";

            retstring = retstring += docseries + " \"Inv_No\",";

            if (!string.IsNullOrEmpty(clsModule.ItemDsc))
            {
                retstring = retstring + "b.\"" + clsModule.ItemDsc + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.GSTCol))
            {
                retstring = retstring + "T.\"" + clsModule.GSTCol + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.HSNCol))
            {
                retstring = retstring + "b.\"" + clsModule.HSNCol + @""",";
            }
            retstring = retstring += " CASE WHEN  T.\"ExportType\" in('U','S') THEN case when a.\"DutyStatus\" ='Y' then 'SEZWP' else 'SEZWOP' End ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN  case when a.\"DutyStatus\" ='Y' then 'EXPWP' else 'EXPWOP' End  ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'D' THEN 'DEXP' ";
            retstring = retstring += "      ELSE 'B2B' END as \"SupTyp\",";
            
            retstring = retstring + " '' as \"RegRev\",'INV' Type,a.\"DocEntry\",a.\"DocType\",a.\"DocDate\" \"Inv_Doc_Date\",ss.\"GSTRegnNo\" \"Seller GSTN\",";

            retstring = retstring + " B1.\"CompnyName\" \"Seller_Legal Name\",ss.\"City\" \"Seller Location Name\", Replace(ss.\"ZipCode\",' ','') \"Seller_PIN code\", ";
            
            retstring = retstring + " (CASE WHEN ss.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ss.\"Country\" and \"Code\"=ss.\"State\") ";
            retstring = retstring + " ELSE '96' END) \"Seller_State_code\",";


            retstring = retstring + " B3.\"GlblLocNum\" as \"CompnyGSTIN\", ";
            retstring = retstring + " B1.\"CompnyAddr\",Replace(B3.\"ZipCode\",' ','') \"CompnyPincode\",B3.\"City\" \"CompnyCity\", ";

            retstring = retstring + " (CASE WHEN B3.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=B3.\"Country\" and \"Code\"=B3.\"State\") ";
            retstring = retstring + " ELSE '96' END ) \"Compnystatecode\", ";

            retstring = retstring += " CASE WHEN COALESCE(Crd11.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd11.\"GSTRegnNo\" END as \"Buyer GSTN\",";
            retstring = retstring += " CASE WHEN COALESCE(Crd1.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd1.\"GSTRegnNo\" END as \"Shipto GSTN\",";

            retstring = retstring += " T.\"PortCode\",T.\"ImpExpNo\" ,T.\"ImpExpDate\" ,COALESCE(st.\"Country\",cy.\"Code\") \"CCode\",  ";

            retstring = retstring += " a.\"BPLId\" ,";

            retstring = retstring + " a.\"CardCode\", a.\"CardName\" \"Buyer_Legal Name\",";
            retstring = retstring + " crd11.\"City\" \"BCity\",st1.\"Name\" \"BState\" ,cy1.\"Name\" \"BCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd11.\"ZipCode\",' ','') END as  \"BZipCode\",";

            retstring = retstring + " crd1.\"City\" \"SCity\",st.\"Name\" \"SState\", cy.\"Name\" \"SCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd1.\"ZipCode\",' ','') END as \"SZipCode\",";

            retstring = retstring += " ss.\"Building\" \"Seller_Building\",ss.\"Block\" \"Seller_Block\",ss.\"Street\" \"Seller_Street\",";

            retstring = retstring += " crd11.\"Building\" \"Buyer_Building\",crd11.\"Block\" \"Buyer_Block\",crd11.\"StreetNo\" \"Buyer_StreetNo\",crd11.\"Street\" \"Buyer_Street\",crd11.\"Address2\" \"Buyer_Address2\",crd11.\"Address3\" \"Buyer_Address3\",";
            retstring = retstring += " crd1.\"Building\" \"Ship_Building\",crd1.\"Block\" \"Ship_Block\",crd1.\"StreetNo\" \"Ship_StreetNo\",crd1.\"Street\" \"Ship_Street\",crd1.\"Address2\" \"Ship_Address2\",crd1.\"Address3\" \"Ship_Address3\",";


            retstring = retstring + " (case when Crd11.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=Crd11.\"State\" and \"Country\"=Crd11.\"Country\") ELSE '96' END) \"Bill to State Code\",";
            retstring = retstring + " (case when crd1.\"Country\"= 'IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=crd1.\"State\" and \"Country\"=crd1.\"Country\") ELSE '96' END)\"Shipp to State Code\",";


            retstring = retstring += " T.\"BuildingB\" \"T_Buyer_Building\",T.\"BlockB\" \"T_Buyer_Block\",T.\"StreetNoB\" \"T_Buyer_StreetNo\",T.\"StreetB\" \"T_Buyer_Street\",T.\"Address2B\" \"T_Buyer_Address2\",T.\"Address3B\" \"T_Buyer_Address3\",";
            retstring = retstring += " T.\"CityB\" \"T_BCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeB\",' ','') END as  \"T_BZipCode\", ";
            retstring = retstring += " (case when T.\"CountryB\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateB\" and \"Country\"=T.\"CountryB\") ElSE '96' END) \"T_Bill to State Code\", ";

            retstring = retstring += " T.\"BuildingS\" \"T_Ship_Building\",T.\"BlockS\" \"T_Ship_Block\",T.\"StreetNoS\" \"T_Ship_StreetNo\",T.\"StreetS\" \"T_Ship_Street\",T.\"Address2S\" \"T_Ship_Address2\",T.\"Address3S\" \"T_Ship_Address3\",";
            retstring = retstring += " T.\"CityS\" \"T_SCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeS\",' ','') END as  \"T_SZipCode\", ";
            retstring = retstring += " (case when T.\"CountryS\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateS\" and \"Country\"=T.\"CountryS\") ELSE '96' END) \"T_Shipp to State Code\", ";



            retstring = retstring + " b.\"ItemCode\", b.\"Dscription\",Case when a.\"DocType\"='S' then 'Y' Else 'N' End \"IsServc\",";

            retstring = retstring + " Case when a.\"DocType\"='S' then (Select Case when LEft(\"ServCode\",2) like '0%' then Replace(\"ServCode\",'0','') Else \"ServCode\" End from OSAC where b.\"SacEntry\"= \"AbsEntry\") Else Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") End \"HSN\", ";
            retstring = retstring + " Case when a.\"DocType\"='S' then 1 Else b.\"Quantity\" End \"Quantity\",COALESCE(b.\"unitMsr\",b.\"UomCode\") \"Unit\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"Price\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end  else b.\"PriceBefDi\" * a.\"DocRate\" end \"UnitPrice\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"PriceBefDi\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt1\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"AssAmt\",";

            retstring = retstring + " (Select Sum(X.\"TaxRate\") From INV4 X Where X.\"DocEntry\"=A.\"DocEntry\" And X.\"LineNum\"=B.\"LineNum\" and X.\"ExpnsCode\"='-1' ) \"GSTRATE\",IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\"and \"staType\"=-100),0) as \"CGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-110),0) as \"SGSTVal\", IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-120),0) as \"IGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-100 and \"ExpnsCode\"='-1'),0) as \"CGSTAmt\", IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-110 and \"ExpnsCode\"='-1'),0) as \"SGSTAmt\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-120 and \"ExpnsCode\"='-1'),0) as \"IGSTAmt\",";
            retstring = retstring + " (SELECT MAX(\"BatchNum\") from IBT1 where \"ItemCode\"=b.\"ItemCode\" and \"WhsCode\"=b.\"WhsCode\" and";
            retstring = retstring + " \"BaseType\" in ('13','15') and \"BaseEntry\"= CASE WHEN  b.\"BaseType\" =-1 THEN b.\"DocEntry\" ELSE b.\"BaseEntry\"  end  ) AS \"BatchNum\",";

            //retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from INV1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

            retstring = retstring + " Case when a.\"DocType\"='S' then  (Select Sum(IFNULL(\"LineTotal\",0))* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from INV1 where \"DocEntry\"=b.\"DocEntry\") " +
                                                                  "Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from INV1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

            retstring = retstring + " (a.\"DocTotal\" +a.\"DpmAmnt\") - ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN  0 ELSE COALESCE((Select Sum(INV5.\"WTAmnt\") from INV5 where INV5.\"AbsEntry\" = a.\"DocEntry\"),0.0) END ) AS \"Doc Total\", ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN   COALESCE((Select Sum(INV5.\"WTAmnt\") from INV5 where INV5.\"AbsEntry\" = a.\"DocEntry\"),0.0) ELSE 0 END ) AS \"OthrAmt\", ";

            retstring = retstring + " a.\"DocDueDate\" \"Inv Due Date\", a.\"NumAtCard\", a.\"Printed\",a.\"PayToCode\", a.\"ShipToCode\", a.\"Comments\" ,Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") \"ChapterID\" , A.\"DiscSum\", A.\"RoundDif\",";
            retstring = retstring + " b.\"DiscPrcnt\",((b.\"PriceBefDi\"*\"Quantity\") * (b.\"DiscPrcnt\"/100)) \"LineDisc\",l.\"InvntryUom\" ,IFNULL(a.\"RoundDif\",0) \"Rounding\", a.\"TaxDate\" \"Cust Order Date\", a.\"TotalExpns\",l.\"FrgnName\",a.\"DocCur\",A.\"VatSum\", a.\"TotalExpns\" \"Freight\",";
            retstring = retstring + " l.\"SalUnitMsr\",IFNULL(b.\"LineTotal\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Line Total\", IFNULL(a.\"TotalExpns\",0) \"Freight Total\",IFNULL(a.\"DiscSum\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Disc Total\",l.\"ItemClass\", ";

            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " i.\"TransID\" ,i.\"TransName\" ,i.\"TransDocNo\" ,i.\"Distance\" ,i.\"TransMode\" ,i.\"VehicleNo\" ,i.\"VehicleTyp\",";
                retstring = retstring + " i.\"TransType\",i.\"TransDate\",i.\"SuplyType\",";
                retstring = retstring + " i.\"FrmGSTN\" ,i.\"FrmTraName\" ,i.\"FrmAddres1\" ,i.\"FrmAddres2\" ,i.\"FrmPlace\" ,Replace(i.\"FrmZipCode\",' ','') as \"FrmZipCode\" ,";
                retstring = retstring + " i.\"ActFrmStat\" ,i.\"ToGSTN\" ,i.\"ToTraName\" ,i.\"ToAddres1\" ,i.\"ToAddres2\" ,i.\"ToPlace\" ,Replace(i.\"ToZipCode\",' ','') as \"ToZipCode\"  ,";
                retstring = retstring + " i.\"ActToState\",i.\"SubSplyTyp\",ES.\"SubType\" \"SubtypeDescription\", i.\"DocType\" \"EDocType\" ,";

                retstring += " i.\"U_Dispatch_Eway\" as \"DisEway\" ,";
                retstring += " i.\"U_Dispatch_Name\" as \"DisName\" ,";
                retstring = retstring + " i.\"EwbDate\" \"EwbDate\" ,i.\"EWayBillNo\" \"EwayNo\" ,";
                retstring = retstring + " i.\"FrmState\" \"FrmState\" ,i.\"ToState\" \"ToState\" ,";

                retstring = retstring + " T.\"ClaimRefun\",";
            }
            else
            {
                if (!string.IsNullOrEmpty(clsModule.EwayUDF))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayUDF + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportName))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportName + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportId))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportId + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayDistance))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayDistance + @""",";
                }

                if (!string.IsNullOrEmpty(clsModule.EwayNo))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayNo + @""" ""EwayNo"",";
                    retstring = retstring + " '' as  \"EwbDate\",";
                }
            }

            retstring = retstring + " (Select \"ServCode\" from OSAC where b.\"SacEntry\"= \"AbsEntry\")\"SacCode\" ";


          retstring = retstring + ",a.\"U_IRNNo\" ,a.\"U_AckDate\" ";

            retstring = retstring+ " FROM OINV a INNER JOIN INV1 b on b.\"DocEntry\" = a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
             {
                retstring = retstring + " LEFT JOIN INV26 i ON i.\"DocEntry\" =a.\"DocEntry\"";
             }

            retstring  = retstring+ " left JOIN OCRD g on g.\"CardCode\" = a.\"CardCode\"";
            retstring  = retstring+ " left JOIN OITM l on l.\"ItemCode\" = b.\"ItemCode\"";
            retstring  = retstring+ " left JOIN OCRD m on m.\"CardCode\" = a.\"CardCode\" ";
            retstring  = retstring+ " LEFT JOIN OCPR n on n.\"CardCode\" = a.\"CardCode\" and n.\"CntctCode\" = a.\"CntctCode\"";
            retstring  = retstring+ " LEFT JOIN OCHP o on o.\"AbsEntry\" = l.\"ChapterID\" ";
            retstring  = retstring+ " LEFT JOIN OLCT ss on ss.\"Code\" = b.\"LocCode\"";
            retstring  = retstring+ " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\" =a.\"CardCode\" and CRD1.\"Address\" =A.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
            retstring  = retstring+ " LEFT JOIN OCST st on st.\"Code\"=CRD1.\"State\" and st.\"Country\"=CRD1.\"Country\" ";
            retstring  = retstring+ " LEFT JOIN OCRY cy on cy.\"Code\" =CRD1.\"Country\"";
            retstring  = retstring+ " LEFT JOIN CRD1 crd11 on crd11.\"CardCode\" =a.\"CardCode\" and crd11.\"Address\" =A.\"PayToCode\" and crd11.\"AdresType\"='B'";
            retstring  = retstring+ " LEFT JOIN OCST st1 on st1.\"Code\"=crd11.\"State\" and st1.\"Country\"=crd11.\"Country\" ";
            retstring  = retstring+ " LEFT JOIN OCRY cy1 on cy1.\"Code\" =crd11.\"Country\"";
            retstring  = retstring+ " CROSS JOIN OADM B1";
            retstring  = retstring+ " CROSS JOIN ADM1 B3";
            retstring  = retstring+ " LEFT JOIN INV12 T ON T.\"DocEntry\"=a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN OEST ES ON ES.\"SubID\" =i.\"SubSplyTyp\"";
            }
            retstring  = retstring+ " LEFT JOIN NNM1 nnm1 ON a.\"Series\" =nnm1.\"Series\"";

            retstring  = retstring+ " LEFT JOIN(SELECT \"BankName\" \"CBankName\",Y.\"BankCode\" \"CBankCode\",\"Branch\" \"CBranch\", \"Account\" \"CAccount\",\"AcctName\" \"CAcctName\",";
            retstring  = retstring+ " X.\"SwiftNum\" \"CIFSCNo\" FROM DSC1 X,ODSC Y Where X.\"AbsEntry\"=Y.\"AbsEntry\" ) B2 On B2.\"CBankCode\"=B1.\"DflBnkCode\"";
            retstring  = retstring+ " WHERE a.\"DocEntry\"="+ Docentry + ")B";
            clsModule.objaddon.objglobalmethods.WriteErrorLog(retstring);
            return retstring;
         

        }



        public string CreditNoteQuery(string Docentry)
        {
            string retstring = "";

            retstring = "SELECT Row_number () Over (Partition by b.\"DocEntry\" order by b.\"DocEntry\" Asc )\" SINo\",(B.\"AssVal\"+B.\"Freight Total\") \"AssValN\" ,";
            retstring = retstring + " (B.\"AssAmt\"+B.\"CGSTAmt\"+B.\"SGSTAmt\"+B.\"IGSTAmt\") \"Total Item Value\",CAST((B.\"Tot Amt\"-B.\"Tot Amt1\")AS nvarchar(200)) \"LineDiscountAmt\",";
            retstring = retstring + " B.* FROM(";
            retstring = retstring + " Select B2.*, 'GST' as \"TaxSch\",T.\"BpGSTN\" as \"T_BpGSTN\",";

            retstring = retstring += docseries + " \"Inv_No\",";

            if (!string.IsNullOrEmpty(clsModule.ItemDsc))
            {
                retstring = retstring + "b.\"" + clsModule.ItemDsc + @""",";
            }

            if (!string.IsNullOrEmpty(clsModule.GSTCol))
            {
                retstring = retstring + "T.\"" + clsModule.GSTCol + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.HSNCol))
            {
                retstring = retstring + "b.\"" + clsModule.HSNCol + @""",";
            }
            retstring = retstring += " CASE WHEN  T.\"ExportType\" in('U','S') THEN case when a.\"DutyStatus\" ='Y' then 'SEZWP' else 'SEZWOP' End ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN  case when a.\"DutyStatus\" ='Y' then 'EXPWP' else 'EXPWOP' End  ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'D' THEN 'DEXP' ";
            retstring = retstring += "      ELSE 'B2B' END as \"SupTyp\",";

            retstring = retstring + " '' as \"RegRev\",'CRN' Type,a.\"DocEntry\",a.\"DocType\",a.\"DocDate\" \"Inv_Doc_Date\",ss.\"GSTRegnNo\" \"Seller GSTN\",";

            retstring = retstring + " B1.\"CompnyName\" \"Seller_Legal Name\",ss.\"City\" \"Seller Location Name\", Replace(ss.\"ZipCode\",' ','') \"Seller_PIN code\", ";        
            retstring = retstring + " (CASE WHEN ss.\"Country\"='IN' then ( select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ss.\"Country\" and \"Code\"=ss.\"State\" )";
            retstring = retstring + " ELSE '96' END) \"Seller_State_code\",";

            retstring = retstring + " B3.\"GlblLocNum\" as \"CompnyGSTIN\", ";
            retstring = retstring + " B1.\"CompnyAddr\",Replace(B3.\"ZipCode\",' ','') \"CompnyPincode\",B3.\"City\" \"CompnyCity\", ";

            retstring = retstring + " (CASE WHEN B3.\"Country\"='IN' then ( select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=B3.\"Country\" and \"Code\"=B3.\"State\") ";
            retstring = retstring + " ELSE '96' END ) \"Compnystatecode\", ";

            retstring = retstring += " CASE WHEN COALESCE(Crd11.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd11.\"GSTRegnNo\" END as \"Buyer GSTN\",";
            retstring = retstring += " CASE WHEN COALESCE(Crd1.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd1.\"GSTRegnNo\" END as \"Shipto GSTN\",";

            retstring = retstring += " T.\"PortCode\",T.\"ImpExpNo\" ,T.\"ImpExpDate\" ,COALESCE(st.\"Country\",cy.\"Code\") \"CCode\",  ";

            retstring = retstring += " a.\"BPLId\" ,";

            retstring = retstring + " a.\"CardCode\", a.\"CardName\" \"Buyer_Legal Name\",";
            retstring = retstring + " crd11.\"City\" \"BCity\",st1.\"Name\" \"BState\" ,cy1.\"Name\" \"BCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd11.\"ZipCode\",' ','') END as  \"BZipCode\",";

            retstring = retstring + " crd1.\"City\" \"SCity\",st.\"Name\" \"SState\", cy.\"Name\" \"SCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd1.\"ZipCode\",' ','') END as \"SZipCode\",";

            retstring = retstring += " ss.\"Building\" \"Seller_Building\",ss.\"Block\" \"Seller_Block\",ss.\"Street\" \"Seller_Street\",";
            retstring = retstring += " crd11.\"Building\" \"Buyer_Building\",crd11.\"Block\" \"Buyer_Block\",crd11.\"StreetNo\" \"Buyer_StreetNo\",crd11.\"Street\" \"Buyer_Street\",crd11.\"Address2\" \"Buyer_Address2\",crd11.\"Address3\" \"Buyer_Address3\",";
            retstring = retstring += " crd1.\"Building\" \"Ship_Building\",crd1.\"Block\" \"Ship_Block\",crd1.\"StreetNo\" \"Ship_StreetNo\",crd1.\"Street\" \"Ship_Street\",crd1.\"Address2\" \"Ship_Address2\",crd1.\"Address3\" \"Ship_Address3\",";


            retstring = retstring += " T.\"BuildingB\" \"T_Buyer_Building\",T.\"BlockB\" \"T_Buyer_Block\",T.\"StreetNoB\" \"T_Buyer_StreetNo\",T.\"StreetB\" \"T_Buyer_Street\",T.\"Address2B\" \"T_Buyer_Address2\",T.\"Address3B\" \"T_Buyer_Address3\",";
            retstring = retstring += " T.\"CityB\" \"T_BCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeB\",' ','') END as  \"T_BZipCode\", ";
            retstring = retstring += " (case when T.\"CountryB\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateB\" and \"Country\"=T.\"CountryB\") ElSE '96' END) \"T_Bill to State Code\", ";

            retstring = retstring += " T.\"BuildingS\" \"T_Ship_Building\",T.\"BlockS\" \"T_Ship_Block\",T.\"StreetNoS\" \"T_Ship_StreetNo\",T.\"StreetS\" \"T_Ship_Street\",T.\"Address2S\" \"T_Ship_Address2\",T.\"Address3S\" \"T_Ship_Address3\",";
            retstring = retstring += " T.\"CityS\" \"T_SCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeS\",' ','') END as  \"T_SZipCode\", ";
            retstring = retstring += " (case when T.\"CountryS\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateS\" and \"Country\"=T.\"CountryS\") ELSE '96' END) \"T_Shipp to State Code\", ";



            retstring = retstring + " (case when Crd11.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=Crd11.\"State\" and \"Country\"=Crd11.\"Country\") ELSE '96' END) \"Bill to State Code\", ";
            retstring = retstring + " (case when Crd1.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=crd1.\"State\" and \"Country\"=crd1.\"Country\") ELSE '96' END) \"Shipp to State Code\",";
            retstring = retstring + "  b.\"ItemCode\", b.\"Dscription\",Case when a.\"DocType\"='S' then 'Y' Else 'N' End \"IsServc\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then (Select Case when LEft(\"ServCode\",2) like '0%' then Replace(\"ServCode\",'0','') Else \"ServCode\" End from OSAC where b.\"SacEntry\"  = \"AbsEntry\") Else Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") End \"HSN\",Case when a.\"DocType\"='S' then 1 Else b.\"Quantity\" End \"Quantity\",b.\"UomCode\" \"Unit\",";

            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"Price\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end  else b.\"PriceBefDi\" * a.\"DocRate\" end \"UnitPrice\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"PriceBefDi\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt1\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"AssAmt\",";

            retstring = retstring + " (Select Sum(X.\"TaxRate\") From RIN4 X Where X.\"DocEntry\"=A.\"DocEntry\" And X.\"LineNum\"=B.\"LineNum\" and X.\"ExpnsCode\"='-1' ) \"GSTRATE\",IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\"and \"staType\"=-100),0) as \"CGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-110),0) as \"SGSTVal\", IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-120),0) as \"IGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-100 and \"ExpnsCode\"='-1'),0) as \"CGSTAmt\", IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-110 and \"ExpnsCode\"='-1'),0) as \"SGSTAmt\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-120 and \"ExpnsCode\"='-1'),0) as \"IGSTAmt\",";
            retstring = retstring + " (SELECT  MAX(\"BatchNum\") from IBT1 where \"ItemCode\"=b.\"ItemCode\" and \"WhsCode\"=b.\"WhsCode\" and";
            retstring = retstring + " \"BaseType\"='14'and \"BaseEntry\"=b.\"DocEntry\" ) AS   \"BatchNum\",";

            //retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from RIN1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

            retstring = retstring + " Case when a.\"DocType\"='S' then  (Select Sum(IFNULL(\"LineTotal\",0))* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from RIN1 where \"DocEntry\"=b.\"DocEntry\") " +
                                                              "Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from RIN1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";


         
            retstring = retstring + " (a.\"DocTotal\" +a.\"DpmAmnt\") - ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN  0 ELSE COALESCE((Select Sum(RIN5.\"WTAmnt\") from RIN5 where RIN5.\"AbsEntry\" = a.\"DocEntry\"),0.0) END ) AS \"Doc Total\", ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN   COALESCE((Select Sum(RIN5.\"WTAmnt\") from RIN5 where RIN5.\"AbsEntry\" = a.\"DocEntry\"),0.0) ELSE 0 END ) AS \"OthrAmt\", ";


            retstring = retstring + " a.\"DocDueDate\" \"Inv Due Date\", a.\"NumAtCard\", a.\"Printed\",a.\"PayToCode\", a.\"ShipToCode\", a.\"Comments\" ,Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") \"ChapterID\" , A.\"DiscSum\", A.\"RoundDif\",";
            retstring = retstring + " b.\"DiscPrcnt\",((b.\"PriceBefDi\"*\"Quantity\") * (b.\"DiscPrcnt\"/100)) \"LineDisc\",l.\"InvntryUom\" ,IFNULL(a.\"RoundDif\",0) \"Rounding\", a.\"TaxDate\" \"Cust Order Date\", a.\"TotalExpns\",l.\"FrgnName\",a.\"DocCur\",A.\"VatSum\", a.\"TotalExpns\" \"Freight\",";
            retstring = retstring + " l.\"SalUnitMsr\",  IFNULL(b.\"LineTotal\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Line Total\", IFNULL(a.\"TotalExpns\",0) \"Freight Total\",IFNULL(a.\"DiscSum\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Disc Total\",l.\"ItemClass\", ";
          
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " i.\"TransID\" ,i.\"TransName\" ,i.\"TransDocNo\" ,i.\"Distance\" ,i.\"TransMode\" ,i.\"VehicleNo\" ,i.\"VehicleTyp\",";
                retstring = retstring + " i.\"TransType\",i.\"TransDate\",i.\"SuplyType\",";
                retstring = retstring + " i.\"FrmGSTN\" ,i.\"FrmTraName\" ,i.\"FrmAddres1\" ,i.\"FrmAddres2\" ,i.\"FrmPlace\" ,Replace(i.\"FrmZipCode\",' ','') as \"FrmZipCode\" ,";
                retstring = retstring + " i.\"ActFrmStat\" ,i.\"ToGSTN\" ,i.\"ToTraName\" ,i.\"ToAddres1\" ,i.\"ToAddres2\" ,i.\"ToPlace\" ,Replace(i.\"ToZipCode\",' ','') as \"ToZipCode\" ,";
                retstring = retstring + " i.\"ActToState\",i.\"SubSplyTyp\",ES.\"SubType\" \"SubtypeDescription\" ,i.\"DocType\" \"EDocType\",";

                retstring += " i.\"U_Dispatch_Eway\" as \"DisEway\" ,";
                retstring += " i.\"U_Dispatch_Name\" as \"DisName\" ,";
                retstring = retstring + " i.\"EwbDate\" \"EwbDate\" ,i.\"EWayBillNo\" \"EwayNo\" ,";
                retstring = retstring + " i.\"FrmState\" \"FrmState\" ,i.\"ToState\" \"ToState\" ,";

                retstring = retstring + " T.\"ClaimRefun\",";
            }
            else
            {
                if (!string.IsNullOrEmpty(clsModule.EwayUDF))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayUDF + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportName))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportName + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportId))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportId + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayDistance))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayDistance + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayNo))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayNo + @""" ""EwayNo"",";
                    retstring = retstring + " '' as  \"EwbDate\",";
                }
            }
            retstring = retstring + " (Select \"ServCode\" from OSAC where b.\"SacEntry\"  = \"AbsEntry\")\"SacCode\" ";

          retstring = retstring + ",a.\"U_IRNNo\" ,a.\"U_AckDate\" ";

            retstring = retstring + " FROM ORIN a INNER JOIN RIN1 b on b.\"DocEntry\" = a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT  JOIN  RIN26 i  ON i.\"DocEntry\" =a.\"DocEntry\"";
            }
            retstring = retstring + " left JOIN OCRD g on g.\"CardCode\" = a.\"CardCode\"";
            retstring = retstring + " left JOIN OITM l on l.\"ItemCode\" = b.\"ItemCode\"";
            retstring = retstring + " left JOIN OCRD m on m.\"CardCode\" = a.\"CardCode\" LEFT JOIN OCPR n on n.\"CardCode\" = a.\"CardCode\" and n.\"CntctCode\" = a.\"CntctCode\"";
            retstring = retstring + " LEFT JOIN OCHP o on o.\"AbsEntry\" = l.\"ChapterID\" LEFT JOIN OLCT ss on ss.\"Code\" = b.\"LocCode\"";
            retstring = retstring + " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\"=a.\"CardCode\" and CRD1.\"Address\" =A.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
            retstring = retstring + " LEFT JOIN OCST st on st.\"Code\"=CRD1.\"State\" and st.\"Country\"=CRD1.\"Country\" LEFT JOIN OCRY cy on cy.\"Code\" =CRD1.\"Country\"";
            retstring = retstring + " LEFT JOIN CRD1 crd11 on crd11.\"CardCode\" =a.\"CardCode\" and crd11.\"Address\" =A.\"PayToCode\" and crd11.\"AdresType\"='B'";
            retstring = retstring + " LEFT JOIN OCST st1 on st1.\"Code\"=crd11.\"State\" and st1.\"Country\"=crd11.\"Country\" LEFT JOIN OCRY cy1 on cy1.\"Code\" =crd11.\"Country\"";
            retstring = retstring + " CROSS JOIN OADM B1";
            retstring = retstring + " CROSS JOIN ADM1 B3";
            retstring = retstring + " LEFT JOIN RIN12 T ON T.\"DocEntry\"=a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN OEST ES ON ES.\"SubID\" =i.\"SubSplyTyp\"";
            }
            retstring = retstring + " LEFT JOIN NNM1 nnm1 ON a.\"Series\" =nnm1.\"Series\"";

            retstring = retstring + " LEFT JOIN(SELECT \"BankName\" \"CBankName\",Y.\"BankCode\" \"CBankCode\",\"Branch\" \"CBranch\", \"Account\" \"CAccount\",\"AcctName\" \"CAcctName\",";
            retstring = retstring + " X.\"SwiftNum\" \"CIFSCNo\" FROM DSC1 X,ODSC Y Where X.\"AbsEntry\"=Y.\"AbsEntry\" ) B2 On B2.\"CBankCode\"=B1.\"DflBnkCode\"";
            retstring = retstring + " WHERE a.\"DocEntry\"=" + Docentry + ")B";
            clsModule.objaddon.objglobalmethods.WriteErrorLog(retstring);

            return retstring;
           
        }


        public string InventoryTransferQuery(string Docentry)
        {
            string retstring = "";

            retstring = "SELECT Row_number() Over(Partition by b.\"DocEntry\" order by b.\"DocEntry\" Asc )\" SINo\",(B.\"AssVal\"+B.\"Freight Total\") \"AssValN\" ,";
            retstring = retstring + " (B.\"AssAmt\"+B.\"CGSTAmt\"+B.\"SGSTAmt\"+B.\"IGSTAmt\") \"Total Item Value\",CAST((B.\"Tot Amt\"-B.\"Tot Amt1\")AS nvarchar(200)) \"LineDiscountAmt\",B.* FROM(";
            retstring = retstring + "  Select B2.*, 'GST' as \"TaxSch\",T.\"BpGSTN\" as \"T_BpGSTN\",";

            retstring = retstring += docseries + " \"Inv_No\",";


            if (!string.IsNullOrEmpty(clsModule.ItemDsc))
            {
                retstring = retstring + "b.\"" + clsModule.ItemDsc + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.GSTCol))
            {
                retstring = retstring + "T.\"" + clsModule.GSTCol + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.HSNCol))
            {
                retstring = retstring + "b.\"" + clsModule.HSNCol + @""",";
            }
            retstring = retstring += " CASE WHEN  T.\"ExportType\" in('U','S') THEN case when a.\"DutyStatus\" ='Y' then 'SEZWP' else 'SEZWOP' End ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN  case when a.\"DutyStatus\" ='Y' then 'EXPWP' else 'EXPWOP' End  ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'D' THEN 'DEXP' ";
            retstring = retstring += "      ELSE 'B2B' END as \"SupTyp\",";

            retstring = retstring + " '' as \"RegRev\",'INV' Type,a.\"DocEntry\",a.\"DocType\",a.\"DocDate\" \"Inv_Doc_Date\",ss.\"GSTRegnNo\" \"Seller GSTN\",";

            retstring = retstring + " B1.\"CompnyName\" \"Seller_Legal Name\",ss.\"City\" \"Seller Location Name\", Replace(ss.\"ZipCode\",' ','') \"Seller_PIN code\", ";
            retstring = retstring + " (CASE WHEN ss.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ss.\"Country\" and \"Code\"=ss.\"State\") ";
            retstring = retstring + " ELSE '96' END) \"Seller_State_code\",";

            retstring = retstring + " B3.\"GlblLocNum\" as \"CompnyGSTIN\", ";
            retstring = retstring + " B1.\"CompnyAddr\",Replace(B3.\"ZipCode\",' ','') \"CompnyPincode\",B3.\"City\" \"CompnyCity\", ";

            retstring = retstring + " (CASE WHEN B3.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=B3.\"Country\" and \"Code\"=B3.\"State\") ";
            retstring = retstring + " ELSE '96' END ) \"Compnystatecode\", ";

            retstring = retstring += " CASE WHEN COALESCE(Crd11.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd11.\"GSTRegnNo\" END as \"Buyer GSTN\",";
            retstring = retstring += " CASE WHEN COALESCE(Crd1.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd1.\"GSTRegnNo\" END as \"Shipto GSTN\",";


            retstring = retstring += " T.\"PortCode\",T.\"ImpExpNo\" ,T.\"ImpExpDate\" ,COALESCE(st.\"Country\",cy.\"Code\") \"CCode\",  ";

            retstring = retstring += " a.\"BPLId\" ,";

            retstring = retstring + " a.\"CardCode\", a.\"CardName\" \"Buyer_Legal Name\",";
            retstring = retstring + " crd11.\"City\" \"BCity\",st1.\"Name\" \"BState\" ,cy1.\"Name\" \"BCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd11.\"ZipCode\",' ','') END as  \"BZipCode\",";

            retstring = retstring + " crd1.\"City\" \"SCity\",st.\"Name\" \"SState\", cy.\"Name\" \"SCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd1.\"ZipCode\",' ','') END as \"SZipCode\",";

            retstring = retstring += " ss.\"Building\" \"Seller_Building\",ss.\"Block\" \"Seller_Block\",ss.\"Street\" \"Seller_Street\",";
            retstring = retstring += " crd11.\"Building\" \"Buyer_Building\",crd11.\"Block\" \"Buyer_Block\",crd11.\"StreetNo\" \"Buyer_StreetNo\",crd11.\"Street\" \"Buyer_Street\",crd11.\"Address2\" \"Buyer_Address2\",crd11.\"Address3\" \"Buyer_Address3\",";
            retstring = retstring += " crd1.\"Building\" \"Ship_Building\",crd1.\"Block\" \"Ship_Block\",crd1.\"StreetNo\" \"Ship_StreetNo\",crd1.\"Street\" \"Ship_Street\",crd1.\"Address2\" \"Ship_Address2\",crd1.\"Address3\" \"Ship_Address3\",";


            retstring = retstring += " T.\"BuildingB\" \"T_Buyer_Building\",T.\"BlockB\" \"T_Buyer_Block\",T.\"StreetNoB\" \"T_Buyer_StreetNo\",T.\"StreetB\" \"T_Buyer_Street\",T.\"Address2B\" \"T_Buyer_Address2\",T.\"Address3B\" \"T_Buyer_Address3\",";
            retstring = retstring += " T.\"CityB\" \"T_BCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeB\",' ','') END as  \"T_BZipCode\", ";
            retstring = retstring += " (case when T.\"CountryB\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateB\" and \"Country\"=T.\"CountryB\") ELSE '96' END) \"T_Bill to State Code\", ";

            retstring = retstring += " T.\"BuildingS\" \"T_Ship_Building\",T.\"BlockS\" \"T_Ship_Block\",T.\"StreetNoS\" \"T_Ship_StreetNo\",T.\"StreetS\" \"T_Ship_Street\",T.\"Address2S\" \"T_Ship_Address2\",T.\"Address3S\" \"T_Ship_Address3\",";
            retstring = retstring += " T.\"CityS\" \"T_SCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeS\",' ','') END as  \"T_SZipCode\", ";
            retstring = retstring += " (case when T.\"CountryS\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateS\" and \"Country\"=T.\"CountryS\") ELSE '96' END) \"T_Shipp to State Code\", ";



            retstring = retstring + " (case when Crd11.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=Crd11.\"State\" and \"Country\"=Crd11.\"Country\") ELSE '96' END) \"Bill to State Code\",";
            retstring = retstring + " (case when crd1.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=crd1.\"State\" and \"Country\"=crd1.\"Country\") ELSE '96' END ) \"Shipp to State Code\",";
            retstring = retstring + " b.\"ItemCode\", b.\"Dscription\",Case when a.\"DocType\"='S' then 'Y' Else 'N' End \"IsServc\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then (Select Case when LEft(\"ServCode\",2) like '0%' then Replace(\"ServCode\",'0','') Else \"ServCode\" End from OSAC where b.\"SacEntry\"= \"AbsEntry\") Else Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") End \"HSN\",Case when a.\"DocType\"='S' then 1 Else b.\"Quantity\" End \"Quantity\",COALESCE(b.\"unitMsr\",b.\"UomCode\") \"Unit\",";


            //retstring = retstring + " Case when a.\"DocType\"='S' then b.\"Price\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end  else b.\"PriceBefDi\" * a.\"DocRate\" end \"UnitPrice\",";
            //retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"PriceBefDi\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt\",";
            //retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt1\",";
            //retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"AssAmt\",";

            //retstring = retstring + " (Select Sum(X.\"TaxRate\") From WTR4 X Where X.\"DocEntry\"=A.\"DocEntry\" And  X.\"LineNum\"=B.\"LineNum\" and X.\"ExpnsCode\"='-1' ) \"GSTRATE\",IFNULL((select sum(\"TaxSum\") from WTR4 where \"DocEntry\"=a.\"DocEntry\"and \"staType\"=-100),0) as \"CGSTVal\",";
            //retstring = retstring + " IFNULL((select sum(\"TaxSum\") from WTR4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-110),0) as \"SGSTVal\", IFNULL((select sum(\"TaxSum\") from WTR4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-120),0) as \"IGSTVal\",";
            //retstring = retstring + " IFNULL((select sum(\"TaxSum\") from WTR4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-100 and \"ExpnsCode\"='-1'),0) as \"CGSTAmt\", IFNULL((select sum(\"TaxSum\") from WTR4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-110 and \"ExpnsCode\"='-1'),0) as \"SGSTAmt\",";
            //retstring = retstring + " IFNULL((select sum(\"TaxSum\") from WTR4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-120 and \"ExpnsCode\"='-1'),0) as \"IGSTAmt\",";


            retstring = retstring + " (SELECT MAX(\"BatchNum\") from IBT1 where \"ItemCode\"=b.\"ItemCode\" and \"WhsCode\"=b.\"WhsCode\" and";
            retstring = retstring + " \"BaseType\" in ('67') and \"BaseEntry\"= CASE WHEN  b.\"BaseType\" =-1 THEN b.\"DocEntry\" ELSE b.\"BaseEntry\"  end  ) AS \"BatchNum\",";

            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN   COALESCE((Select Sum(WTR5.\"WTAmnt\") from WTR5 where WTR5.\"AbsEntry\" = a.\"DocEntry\"),0.0) ELSE 0 END ) AS \"OthrAmt\", ";

      
            string Pricecol = "PriceBefDi";

            string Dbcol =clsModule.objaddon.objglobalmethods.getSingleValue("SELECT \"U_INVTranItemDB\" FROM \"@ATEICFG\" a  WHERE \"Code\" ='01'");
            if (!string.IsNullOrEmpty(Dbcol))
            {
                Pricecol = Dbcol;
            }

            retstring = retstring + " (Select Sum(COALESCE(\"" + Pricecol + "\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from WTR1 where \"DocEntry\"=b.\"DocEntry\")  \"AssVal\",";


            retstring = retstring + "  b.\"" + Pricecol + "\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end  \"UnitPrice\",";
            retstring = retstring + "  COALESCE( b.\"" + Pricecol + "\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end,0)*b.\"Quantity\"  \"Tot Amt\",";
            retstring = retstring + "  COALESCE( b.\"" + Pricecol + "\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end,0)*b.\"Quantity\"  \"Tot Amt1\",";
            retstring = retstring + "  COALESCE( b.\"" + Pricecol + "\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end,0)*b.\"Quantity\"  \"AssAmt\",";


            if (clsModule.objaddon.objglobalmethods.getSingleValue("SELECT \"U_InvUseQry\" FROM \"@ATEICFG\" a  WHERE \"Code\" ='01'") != "Y")
            {

                retstring = retstring + " \"U_UTL_ST_CGST\"+\"U_UTL_ST_SGST\"+\"U_UTL_ST_IGST\" as \"GSTRATE\",";
                retstring = retstring + " (select sum(\"U_UTL_ST_CGAMT\") from WTR1 where \"DocEntry\"=a.\"DocEntry\") as \"CGSTVal\",  ";
                retstring = retstring + " (select sum(\"U_UTL_ST_SGAMT\") from WTR1 where \"DocEntry\"=a.\"DocEntry\") as \"SGSTVal\",  ";
                retstring = retstring + " (select sum(\"U_UTL_ST_IGAMT\") from WTR1 where \"DocEntry\"=a.\"DocEntry\") as \"IGSTVal\",  ";
                retstring = retstring + " \"U_UTL_ST_CGAMT\" as  \"CGSTAmt\", \"U_UTL_ST_SGAMT\" as  \"SGSTAmt\",\"U_UTL_ST_IGAMT\" as  \"IGSTAmt\",";
                retstring = retstring + " (case when (select sum(\"U_UTL_ST_LINETOTAL\") from WTR1 where \"DocEntry\"=a.\"DocEntry\") =0 then a.\"DocTotal\"  ";
                retstring += " else  (select sum(\"U_UTL_ST_LINETOTAL\") from WTR1 where \"DocEntry\"=a.\"DocEntry\") end  +a.\"DpmAmnt\") - ";
                retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
                retstring += " THEN  0 ELSE COALESCE((Select Sum(WTR5.\"WTAmnt\") from WTR5 where WTR5.\"AbsEntry\" = a.\"DocEntry\"),0.0) END ) AS \"Doc Total\", ";
            }
            else
            {
                string taxrateccol = clsModule.objaddon.objglobalmethods.getSingleValue("SELECT \"U_InvTaxrt\" FROM \"@ATEICFG\" a  WHERE \"Code\" ='01'");

                retstring = retstring + " l.\"" + taxrateccol + "\"  as \"GSTRATE\", " ;


                retstring += " (SELECT sum( COALESCE(  b1.\"" + Pricecol + "\" * case when a1.\"DocRate\"=0 then 1 else a1.\"DocRate\" end,0)* b1.\"Quantity\")* ";
                retstring += " (CASE WHEN LEFT(FrmLoc.\"GSTRegnNo\", 2) = " +
                                         "LEFT(CASE WHEN COALESCE(a.\"CardCode\", '') = '' THEN  ToLoc.\"GSTRegnNo\" ELSE  ToLocSub.\"GSTRegnNo\"  END, 2) ";
                retstring += " THEN (CAST(l.\"" + taxrateccol + "\" AS Decimal(18, 6)) / 100) / 2  ELSE 0 END) ";
                retstring += "  from OWTR a1 INNER JOIN WTR1 b1 on b1.\"DocEntry\" = a1.\"DocEntry\"  where a1.\"DocEntry\" = a.\"DocEntry\") AS \"CGSTVal\", ";

                retstring += " (SELECT sum( COALESCE(  b1.\"" + Pricecol + "\" * case when a1.\"DocRate\"=0 then 1 else a1.\"DocRate\" end,0)* b1.\"Quantity\")* ";
                retstring += " (CASE WHEN LEFT(FrmLoc.\"GSTRegnNo\", 2) = " +
                                         "LEFT(CASE WHEN COALESCE(a.\"CardCode\", '') = '' THEN  ToLoc.\"GSTRegnNo\" ELSE  ToLocSub.\"GSTRegnNo\"  END, 2) ";
                retstring += " THEN (CAST(l.\"" + taxrateccol + "\" AS Decimal(18, 6)) / 100) / 2  ELSE 0 END) ";
                retstring += "  from OWTR a1 INNER JOIN WTR1 b1 on b1.\"DocEntry\" = a1.\"DocEntry\"  where a1.\"DocEntry\" = a.\"DocEntry\") AS \"SGSTVal\", ";

                retstring += " (SELECT sum( COALESCE(  b1.\"" + Pricecol + "\" * case when a1.\"DocRate\"=0 then 1 else a1.\"DocRate\" end,0)* b1.\"Quantity\")* ";
                retstring += " (CASE WHEN LEFT(FrmLoc.\"GSTRegnNo\", 2) = " +
                                         "LEFT(CASE WHEN COALESCE(a.\"CardCode\", '') = '' THEN  ToLoc.\"GSTRegnNo\" ELSE  ToLocSub.\"GSTRegnNo\"  END, 2) ";
                retstring += " THEN 0 ELSE (CAST(l.\"" + taxrateccol + "\" AS Decimal(18, 6)) / 100) END) ";
                retstring += "  from OWTR a1 INNER JOIN WTR1 b1 on b1.\"DocEntry\" = a1.\"DocEntry\"  where a1.\"DocEntry\" = a.\"DocEntry\") AS \"IGSTVal\", ";

                retstring += " ((COALESCE(b.\"" + Pricecol + "\" * case when a.\"DocRate\" = 0 then 1 else a.\"DocRate\" end, 0) * b.\"Quantity\")* ";
                retstring += " (CASE WHEN LEFT(FrmLoc.\"GSTRegnNo\",2)= " +
                                        " LEFT(CASE WHEN COALESCE(a.\"CardCode\", '') = '' THEN  ToLoc.\"GSTRegnNo\" ELSE  ToLocSub.\"GSTRegnNo\"  END, 2) ";
                retstring += " THEN (CAST(l.\"" + taxrateccol + "\"  AS Decimal(18, 6)) / 100) / 2  ELSE  0 END ) ) \"CGSTAmt\",";

                retstring += " ((COALESCE(b.\"" + Pricecol + "\" * case when a.\"DocRate\" = 0 then 1 else a.\"DocRate\" end, 0) * b.\"Quantity\")* ";
                retstring += " (CASE WHEN LEFT(FrmLoc.\"GSTRegnNo\",2)= " +
                                        " LEFT(CASE WHEN COALESCE(a.\"CardCode\", '') = '' THEN  ToLoc.\"GSTRegnNo\" ELSE  ToLocSub.\"GSTRegnNo\"  END, 2) ";
                retstring += " THEN (CAST(l.\"" + taxrateccol + "\"  AS Decimal(18, 6)) / 100) / 2  ELSE  0 END ) ) \"SGSTAmt\",";

                retstring += " ((COALESCE(b.\"" + Pricecol + "\" * case when a.\"DocRate\" = 0 then 1 else a.\"DocRate\" end, 0) * b.\"Quantity\")* ";
                retstring += " (CASE WHEN LEFT(FrmLoc.\"GSTRegnNo\",2)= " +
                                        " LEFT(CASE WHEN COALESCE(a.\"CardCode\", '') = '' THEN  ToLoc.\"GSTRegnNo\" ELSE  ToLocSub.\"GSTRegnNo\"  END, 2) ";
                retstring += " THEN 0 ELSE  (CAST(l.\"" + taxrateccol + "\"  AS Decimal(18, 6)) / 100)  END ) ) \"IGSTAmt\",";



                retstring += " (((SELECT sum(COALESCE(b1.\"" + Pricecol + "\" * case when a1.\"DocRate\" = 0 then 1 else a1.\"DocRate\" end, 0) * b1.\"Quantity\") ";
                retstring += "  from OWTR a1 INNER JOIN WTR1 b1 on b1.\"DocEntry\" = a1.\"DocEntry\"  where a1.\"DocEntry\" = a.\"DocEntry\") +";
                retstring += " (SELECT sum(COALESCE(b1.\"" + Pricecol + "\" * case when a1.\"DocRate\" = 0 then 1 else a1.\"DocRate\" end, 0) * b1.\"Quantity\") * ";
                retstring += " (CAST(l.\"" + taxrateccol + "\"  AS Decimal(18, 6)) / 100) ";
                retstring += "  from OWTR a1 INNER JOIN WTR1 b1 on b1.\"DocEntry\" = a1.\"DocEntry\"  where a1.\"DocEntry\" = a.\"DocEntry\" )) ";
                retstring += " + a.\"DpmAmnt\") -(CASE WHEN(SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" = '01') = 'Y' ";
                retstring += "   THEN  0 ELSE COALESCE((Select Sum(WTR5.\"WTAmnt\") from WTR5 where WTR5.\"AbsEntry\" = a.\"DocEntry\"),0.0) END ) AS \"Doc Total\", ";

            }
           





            retstring = retstring + " a.\"DocDueDate\" \"Inv Due Date\", a.\"NumAtCard\", a.\"Printed\",a.\"PayToCode\", a.\"ShipToCode\", a.\"Comments\" ,Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") \"ChapterID\" , A.\"DiscSum\", A.\"RoundDif\",";
            retstring = retstring + " b.\"DiscPrcnt\",((b.\"" + Pricecol + "\"*\"Quantity\") * (b.\"DiscPrcnt\"/100)) \"LineDisc\",l.\"InvntryUom\" ,IFNULL(a.\"RoundDif\",0) \"Rounding\", a.\"TaxDate\" \"Cust Order Date\", a.\"TotalExpns\",l.\"FrgnName\",a.\"DocCur\",A.\"VatSum\", a.\"TotalExpns\" \"Freight\",";
            retstring = retstring + " l.\"SalUnitMsr\",IFNULL(b.\"LineTotal\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Line Total\", IFNULL(a.\"TotalExpns\",0) \"Freight Total\",IFNULL(a.\"DiscSum\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Disc Total\",l.\"ItemClass\", ";

            if (clsModule.objaddon.objglobalmethods.getSingleValue("SELECT \"U_InvTranGetBrnchAdd\" FROM \"@ATEICFG\" a  WHERE \"Code\" ='01' ") == "Y")
            {
                retstring = retstring + " CONCAT(CONCAT(Cast(COALESCE(FrmLoc.\"Building\",'') AS Varchar(200)), COALESCE(FrmLoc.\"Street\",'')),CONCAT(COALESCE(FrmLoc.\"Block\",'') , COALESCE(FrmLoc.\"Address\",'')))  \"FrmAddres1\",";
                retstring = retstring + " FrmLoc.\"TaxIdNum\" \"FrmGSTN\", FrmLoc.\"BPLName\" \"FrmTraName\", ";
            }
            else
            {
                retstring = retstring + " FrmLoc.\"GSTRegnNo\" \"FrmGSTN\", B1.\"CompnyName\"  \"FrmTraName\", ";
                retstring = retstring + " CONCAT(CONCAT(Cast(COALESCE(FrmLoc.\"Building\",'') AS Varchar(200)) , COALESCE(FrmLoc.\"Street\",'') ), COALESCE(FrmLoc.\"Block\",''))  \"FrmAddres1\",";

            }

            

            


            retstring = retstring + " '' \"FrmAddres2\" , ";
            retstring = retstring + " FrmLoc.\"City\" \"FrmPlace\", Replace(FrmLoc.\"ZipCode\",' ','') \"FrmZipCode\",";
            retstring = retstring + " (case when FrmLoc.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=FrmLoc.\"Country\" and \"Code\"=FrmLoc.\"State\") ELSE '96' END) \"ActFrmStat\",";
            retstring = retstring + " (case when FrmLoc.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=FrmLoc.\"Country\" and \"Code\"=FrmLoc.\"State\") ELSE '96' END) \"FrmState\",";

            
            if (clsModule.objaddon.objglobalmethods.getSingleValue("SELECT \"U_InvTranGetcusAdd\" FROM \"@ATEICFG\" a  WHERE \"Code\" ='01'") == "Y")
            {
                retstring += " case when COALESCE(a.\"CardCode\",'') <>'' then ";
                retstring += " CONCAT(CONCAT(CONCAT(CONCAT(Cast(COALESCE(ToLoc.\"Building\",'') AS Varchar(200)) , ' '), CONCAT(COALESCE(ToLoc.\"Street\",'') , ' ')) , CONCAT(CONCAT( COALESCE(ToLoc.\"Block\",'') , ' '), CONCAT(COALESCE(ToLoc.\"Address2\",'') , ' '))),  COALESCE(ToLoc.\"Address3\",'')) ";
                retstring += " else ";
                retstring += " CONCAT(CONCAT(CONCAT(Cast(COALESCE(ToLocSub.\"Building\",'') AS Varchar(200)) ,' '), CONCAT(COALESCE(ToLocSub.\"Street\",'') , ' ')), COALESCE(ToLocSub.\"Block\",'')) ";
                retstring += " End ";
                retstring += " \"ToAddres1\",";

                retstring += " case when COALESCE(a.\"CardCode\",'') <>'' then ";
                retstring += " ToLoc.\"Address\" ";
                retstring += " else ";
                retstring += " B1.\"CompnyName\" ";
                retstring += " End ";
                retstring += " \"ToTraName\",";

                retstring += " case when COALESCE(a.\"CardCode\",'') <>'' then ";
                retstring += " ToLoc.\"GSTRegnNo\" ";
                retstring += " else ";
                retstring += " ToLocSub.\"GSTRegnNo\" ";
                retstring += " End ";
                retstring +=" \"ToGSTN\", ";

                retstring += " '' \"ToAddres2\", ";


                retstring += " case when COALESCE(a.\"CardCode\",'') <>'' then ";
                retstring += " ToLoc.\"City\" ";
                retstring += " else ";
                retstring += " ToLocSub.\"City\" ";
                retstring += " End ";
                retstring += " \"ToPlace\", ";

                retstring += " case when COALESCE(a.\"CardCode\",'') <>'' then ";
                retstring += " Replace(ToLoc.\"ZipCode\",' ','') ";
                retstring += " else ";
                retstring += " Replace(ToLocSub.\"ZipCode\",' ','') ";
                retstring += " End ";
                retstring += " \"ToZipCode\", ";


                retstring += " case when COALESCE(a.\"CardCode\",'') <>'' then ";
                retstring += " (case when ToLoc.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ToLoc.\"Country\" and \"Code\"=ToLoc.\"State\")  ELSE '96' END) ";
                retstring += " else ";
                retstring += " (case when ToLocSub.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ToLocSub.\"Country\" and \"Code\"=ToLocSub.\"State\")  ELSE '96' END) ";
                retstring += " End ";
                retstring += " \"ActToState\", ";

                retstring += " case when COALESCE(a.\"CardCode\",'') <>'' then ";
                retstring += " (case when ToLoc.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ToLoc.\"Country\" and \"Code\"=ToLoc.\"State\") ELSE '96' END) ";
                retstring += " else ";
                retstring += " (case when ToLocSub.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ToLocSub.\"Country\" and \"Code\"=ToLocSub.\"State\")  ELSE '96' END) ";
                retstring += " End ";
                retstring += " \"ToState\", ";
             
            }
            else
            {
                retstring = retstring + " CONCAT( CONCAT(CONCAT(Cast(COALESCE(ToLoc.\"Building\",'') AS Varchar(200)) , ' '), CONCAT(COALESCE(ToLoc.\"Street\",'') , ' ')),COALESCE(ToLoc.\"Block\",''))  \"ToAddres1\",";
                retstring = retstring + " B1.\"CompnyName\" \"ToTraName\",";

                retstring = retstring + " ToLoc.\"GSTRegnNo\" \"ToGSTN\",";
                retstring = retstring + " '' \"ToAddres2\", ";
                retstring = retstring + " ToLoc.\"City\" \"ToPlace\", Replace(ToLoc.\"ZipCode\",' ','') \"ToZipCode\",";
                retstring = retstring + " (case when ToLoc.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ToLoc.\"Country\" and \"Code\"=ToLoc.\"State\") ELSE '96' END) \"ActToState\",";
                retstring = retstring + " (case when ToLoc.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ToLoc.\"Country\" and \"Code\"=ToLoc.\"State\") ELSE '96' END) \"ToState\",";
            }


            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " i.\"TransID\" ,i.\"TransName\" ,i.\"TransDocNo\" ,i.\"Distance\" ,i.\"TransMode\" ,i.\"VehicleNo\" ,i.\"VehicleTyp\",";
                retstring = retstring + " i.\"TransType\",i.\"TransDate\",i.\"SuplyType\",";
               // retstring = retstring + " i.\"FrmGSTN\" ,i.\"FrmTraName\" ,i.\"FrmAddres1\" ,i.\"FrmAddres2\" ,i.\"FrmPlace\" ,";
               // retstring = retstring + " Replace(i.\"FrmZipCode\",' ','') as \"FrmZipCode\", i.\"ActFrmStat\" ,";                
               // retstring = retstring + " i.\"ToGSTN\" ,i.\"ToTraName\" ,i.\"ToAddres1\" ,i.\"ToAddres2\" ,i.\"ToPlace\" ,Replace(i.\"ToZipCode\",' ','') as \"ToZipCode\"  ,i.\"ActToState\",";
                retstring = retstring + " i.\"SubSplyTyp\",ES.\"SubType\" \"SubtypeDescription\", i.\"DocType\" \"EDocType\" ,";

                retstring += " i.\"U_Dispatch_Eway\" as \"DisEway\" ,";
                retstring += " i.\"U_Dispatch_Name\" as \"DisName\" ,";
                retstring = retstring + " i.\"EwbDate\" \"EwbDate\" ,i.\"EWayBillNo\" \"EwayNo\" ,";            
                retstring = retstring + " T.\"ClaimRefun\",";
            }
            else
            {
                if (!string.IsNullOrEmpty(clsModule.EwayUDF))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayUDF + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportName))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportName + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportId))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportId + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayDistance))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayDistance + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayNo))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayNo + @""" ""EwayNo"",";
                    retstring = retstring + " '' as  \"EwbDate\",";
                }


            }

            retstring = retstring + " (Select \"ServCode\" from OSAC where b.\"SacEntry\"= \"AbsEntry\")\"SacCode\" ";

          retstring = retstring + ",a.\"U_IRNNo\" ,a.\"U_AckDate\" ";

            retstring = retstring + " FROM OWTR a INNER JOIN WTR1 b on b.\"DocEntry\" = a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN WTR26 i ON i.\"DocEntry\" =a.\"DocEntry\"";
            }

            retstring = retstring + " left JOIN OCRD g on g.\"CardCode\" = a.\"CardCode\"";
            retstring = retstring + " left JOIN OITM l on l.\"ItemCode\" = b.\"ItemCode\"";
            retstring = retstring + " left JOIN OCRD m on m.\"CardCode\" = a.\"CardCode\" ";
            retstring = retstring + " LEFT JOIN OCPR n on n.\"CardCode\" = a.\"CardCode\" and n.\"CntctCode\" = a.\"CntctCode\"";
            retstring = retstring + " LEFT JOIN OCHP o on o.\"AbsEntry\" = l.\"ChapterID\" ";
            retstring = retstring + " LEFT JOIN OLCT ss on ss.\"Code\" = b.\"LocCode\"";
            retstring = retstring + " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\" =a.\"CardCode\" and CRD1.\"Address\" =A.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
            retstring = retstring + " LEFT JOIN OCST st on st.\"Code\"=CRD1.\"State\" and st.\"Country\"=CRD1.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy on cy.\"Code\" =CRD1.\"Country\"";
            retstring = retstring + " LEFT JOIN CRD1 crd11 on crd11.\"CardCode\" =a.\"CardCode\" and crd11.\"Address\" =A.\"PayToCode\" and crd11.\"AdresType\"='B'";
            retstring = retstring + " LEFT JOIN OCST st1 on st1.\"Code\"=crd11.\"State\" and st1.\"Country\"=crd11.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy1 on cy1.\"Code\" =crd11.\"Country\"";
            retstring = retstring + " CROSS JOIN OADM B1";
            retstring = retstring + " CROSS JOIN ADM1 B3";
            retstring = retstring + " LEFT JOIN WTR12 T ON T.\"DocEntry\"=a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN OEST ES ON ES.\"SubID\" =i.\"SubSplyTyp\"";
            }
            retstring = retstring + " LEFT JOIN NNM1 nnm1 ON a.\"Series\" =nnm1.\"Series\"";

            retstring = retstring + " LEFT JOIN OWHS FrmAdd ON FrmAdd.\"WhsCode\" =a.\"Filler\"";
            retstring = retstring + " LEFT JOIN OWHS ToAdd ON ToAdd.\"WhsCode\" =a.\"ToWhsCode\"";
          


            if (clsModule.objaddon.objglobalmethods.getSingleValue("SELECT \"U_InvTranGetBrnchAdd\" FROM \"@ATEICFG\" a  WHERE \"Code\" ='01' ") == "Y")
            {
                
                retstring = retstring + "LEFT JOIN OBPL FrmLoc ON FrmLoc.\"BPLId\" =FrmAdd.\"BPLid\" ";
            }
            else
            {
                retstring = retstring + " LEFT JOIN OLCT FrmLoc ON FrmLoc.\"Code\" =FrmAdd.\"Location\"";

            }


            if ( clsModule.objaddon.objglobalmethods.getSingleValue("SELECT \"U_InvTranGetcusAdd\" FROM \"@ATEICFG\" a  WHERE \"Code\" ='01' ")=="Y")
            {
                retstring = retstring + " LEFT JOIN CRD1 ToLoc on ToLoc.\"CardCode\" =a.\"CardCode\" and ToLoc.\"Address\" =A.\"ShipToCode\" and ToLoc.\"AdresType\"='S'";
            }
            else
            {
                retstring = retstring + " LEFT JOIN OLCT ToLoc ON ToLoc.\"Code\" =ToAdd.\"Location\"";
            }


            retstring = retstring + " LEFT JOIN OLCT ToLocSub ON ToLocSub.\"Code\" =ToAdd.\"Location\"";


            retstring = retstring + " LEFT JOIN(SELECT \"BankName\" \"CBankName\",Y.\"BankCode\" \"CBankCode\",\"Branch\" \"CBranch\", \"Account\" \"CAccount\",\"AcctName\" \"CAcctName\",";
            retstring = retstring + " X.\"SwiftNum\" \"CIFSCNo\" FROM DSC1 X,ODSC Y Where X.\"AbsEntry\"=Y.\"AbsEntry\" ) B2 On B2.\"CBankCode\"=B1.\"DflBnkCode\"";
            retstring = retstring + " WHERE a.\"DocEntry\"=" + Docentry + ")B";
            clsModule.objaddon.objglobalmethods.WriteErrorLog(retstring);
            return retstring;


        }


        public string DeliveryQuery(string Docentry)
        {
            string retstring = "";

            retstring = "SELECT Row_number() Over(Partition by b.\"DocEntry\" order by b.\"DocEntry\" Asc )\" SINo\",(B.\"AssVal\"+B.\"Freight Total\") \"AssValN\" ,";
            retstring = retstring + " (B.\"AssAmt\"+B.\"CGSTAmt\"+B.\"SGSTAmt\"+B.\"IGSTAmt\") \"Total Item Value\",CAST((B.\"Tot Amt\"-B.\"Tot Amt1\")AS nvarchar(200)) \"LineDiscountAmt\",B.* FROM(";
            retstring = retstring + "  Select B2.*, 'GST' as \"TaxSch\",T.\"BpGSTN\" as \"T_BpGSTN\",";

            retstring = retstring += docseries + " \"Inv_No\",";

            if (!string.IsNullOrEmpty(clsModule.ItemDsc))
            {
                retstring = retstring + "b.\"" + clsModule.ItemDsc + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.GSTCol))
            {
                retstring = retstring + "T.\"" + clsModule.GSTCol + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.HSNCol))
            {
                retstring = retstring + "b.\"" + clsModule.HSNCol + @""",";
            }

            retstring = retstring += " CASE WHEN  T.\"ExportType\" in('U','S') THEN case when a.\"DutyStatus\" ='Y' then 'SEZWP' else 'SEZWOP' End ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN  case when a.\"DutyStatus\" ='Y' then 'EXPWP' else 'EXPWOP' End  ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'D' THEN 'DEXP' ";
            retstring = retstring += "      ELSE 'B2B' END as \"SupTyp\",";

            retstring = retstring + " '' as \"RegRev\",'INV' Type,a.\"DocEntry\",a.\"DocType\",a.\"DocDate\" \"Inv_Doc_Date\",ss.\"GSTRegnNo\" \"Seller GSTN\",";

            retstring = retstring + " B1.\"CompnyName\" \"Seller_Legal Name\",ss.\"City\" \"Seller Location Name\", Replace(ss.\"ZipCode\",' ','') \"Seller_PIN code\", ";
            retstring = retstring + " (CASE WHEN ss.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ss.\"Country\" and \"Code\"=ss.\"State\") ";
            retstring = retstring + " ELSE '96' END) \"Seller_State_code\",";

            retstring = retstring + " B3.\"GlblLocNum\" as \"CompnyGSTIN\", ";
            retstring = retstring + " B1.\"CompnyAddr\",Replace(B3.\"ZipCode\",' ','') \"CompnyPincode\",B3.\"City\" \"CompnyCity\", ";

            retstring = retstring + " (CASE WHEN B3.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=B3.\"Country\" and \"Code\"=B3.\"State\") ";
            retstring = retstring + " ELSE '96' END ) \"Compnystatecode\", ";

            retstring = retstring += " CASE WHEN COALESCE(Crd11.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd11.\"GSTRegnNo\" END as \"Buyer GSTN\",";
            retstring = retstring += " CASE WHEN COALESCE(Crd1.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd1.\"GSTRegnNo\" END as \"Shipto GSTN\",";

            retstring = retstring += " T.\"PortCode\",T.\"ImpExpNo\" ,T.\"ImpExpDate\" ,COALESCE(st.\"Country\",cy.\"Code\") \"CCode\",   ";

            retstring = retstring += " a.\"BPLId\" ,";

            retstring = retstring + " a.\"CardCode\", a.\"CardName\" \"Buyer_Legal Name\",";
            retstring = retstring + " crd11.\"City\" \"BCity\",st1.\"Name\" \"BState\" ,cy1.\"Name\" \"BCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd11.\"ZipCode\",' ','') END as  \"BZipCode\",";

            retstring = retstring + " crd1.\"City\" \"SCity\",st.\"Name\" \"SState\", cy.\"Name\" \"SCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd1.\"ZipCode\",' ','') END as \"SZipCode\",";

            retstring = retstring += " ss.\"Building\" \"Seller_Building\",ss.\"Block\" \"Seller_Block\",ss.\"Street\" \"Seller_Street\",";
            retstring = retstring += " crd11.\"Building\" \"Buyer_Building\",crd11.\"Block\" \"Buyer_Block\",crd11.\"StreetNo\" \"Buyer_StreetNo\",crd11.\"Street\" \"Buyer_Street\",crd11.\"Address2\" \"Buyer_Address2\",crd11.\"Address3\" \"Buyer_Address3\",";
            retstring = retstring += " crd1.\"Building\" \"Ship_Building\",crd1.\"Block\" \"Ship_Block\",crd1.\"StreetNo\" \"Ship_StreetNo\",crd1.\"Street\" \"Ship_Street\",crd1.\"Address2\" \"Ship_Address2\",crd1.\"Address3\" \"Ship_Address3\",";


            retstring = retstring += " T.\"BuildingB\" \"T_Buyer_Building\",T.\"BlockB\" \"T_Buyer_Block\",T.\"StreetNoB\" \"T_Buyer_StreetNo\",T.\"StreetB\" \"T_Buyer_Street\",T.\"Address2B\" \"T_Buyer_Address2\",T.\"Address3B\" \"T_Buyer_Address3\",";
            retstring = retstring += " T.\"CityB\" \"T_BCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeB\",' ','') END as  \"T_BZipCode\", ";
            retstring = retstring += " (case when T.\"CountryB\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateB\" and \"Country\"=T.\"CountryB\") ElSE '96' END) \"T_Bill to State Code\", ";

            retstring = retstring += " T.\"BuildingS\" \"T_Ship_Building\",T.\"BlockS\" \"T_Ship_Block\",T.\"StreetNoS\" \"T_Ship_StreetNo\",T.\"StreetS\" \"T_Ship_Street\",T.\"Address2S\" \"T_Ship_Address2\",T.\"Address3S\" \"T_Ship_Address3\",";
            retstring = retstring += " T.\"CityS\" \"T_SCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeS\",' ','') END as  \"T_SZipCode\", ";
            retstring = retstring += " (case when T.\"CountryS\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateS\" and \"Country\"=T.\"CountryS\") ELSE '96' END) \"T_Shipp to State Code\", ";



            retstring = retstring + " (case when Crd11.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=Crd11.\"State\" and \"Country\"=Crd11.\"Country\") ELSE '96' END) \"Bill to State Code\",";
            retstring = retstring + " (case when crd1.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=crd1.\"State\" and \"Country\"=crd1.\"Country\") ELSE '96' END ) \"Shipp to State Code\",";
            retstring = retstring + " b.\"ItemCode\", b.\"Dscription\",Case when a.\"DocType\"='S' then 'Y' Else 'N' End \"IsServc\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then (Select Case when LEft(\"ServCode\",2) like '0%' then Replace(\"ServCode\",'0','') Else \"ServCode\" End from OSAC where b.\"SacEntry\"= \"AbsEntry\") Else Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") End \"HSN\",Case when a.\"DocType\"='S' then 1 Else b.\"Quantity\" End \"Quantity\",COALESCE(b.\"unitMsr\",b.\"UomCode\") \"Unit\",";

            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"Price\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end  else b.\"PriceBefDi\" * a.\"DocRate\" end \"UnitPrice\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"PriceBefDi\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt1\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"AssAmt\",";

            retstring = retstring + " (Select Sum(X.\"TaxRate\") From DLN4 X Where X.\"DocEntry\"=A.\"DocEntry\" And X.\"LineNum\"=B.\"LineNum\" and X.\"ExpnsCode\"='-1' ) \"GSTRATE\",IFNULL((select sum(\"TaxSum\") from DLN4 where \"DocEntry\"=a.\"DocEntry\"and \"staType\"=-100),0) as \"CGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from DLN4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-110),0) as \"SGSTVal\", IFNULL((select sum(\"TaxSum\") from DLN4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-120),0) as \"IGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from DLN4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-100 and \"ExpnsCode\"='-1'),0) as \"CGSTAmt\", IFNULL((select sum(\"TaxSum\") from DLN4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-110 and \"ExpnsCode\"='-1'),0) as \"SGSTAmt\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from DLN4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-120 and \"ExpnsCode\"='-1'),0) as \"IGSTAmt\",";
            retstring = retstring + " (SELECT MAX(\"BatchNum\") from IBT1 where \"ItemCode\"=b.\"ItemCode\" and \"WhsCode\"=b.\"WhsCode\" and";
            retstring = retstring + " \"BaseType\" in ('15') and \"BaseEntry\"= CASE WHEN  b.\"BaseType\" =-1 THEN b.\"DocEntry\" ELSE b.\"BaseEntry\"  end  ) AS \"BatchNum\",";

            //retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from DLN1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

            retstring = retstring + " Case when a.\"DocType\"='S' then  (Select Sum(IFNULL(\"LineTotal\",0))* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from DLN1 where \"DocEntry\"=b.\"DocEntry\") " +
                                                           "Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from DLN1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

         


            retstring = retstring + " (a.\"DocTotal\" +a.\"DpmAmnt\") - ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN  0 ELSE COALESCE((Select Sum(DLN5.\"WTAmnt\") from DLN5 where DLN5.\"AbsEntry\" = a.\"DocEntry\"),0.0) END ) AS \"Doc Total\", ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN   COALESCE((Select Sum(DLN5.\"WTAmnt\") from DLN5 where DLN5.\"AbsEntry\" = a.\"DocEntry\"),0.0) ELSE 0 END ) AS \"OthrAmt\", ";


            retstring = retstring + " a.\"DocDueDate\" \"Inv Due Date\", a.\"NumAtCard\", a.\"Printed\",a.\"PayToCode\", a.\"ShipToCode\", a.\"Comments\" ,Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") \"ChapterID\" , A.\"DiscSum\", A.\"RoundDif\",";
            retstring = retstring + " b.\"DiscPrcnt\",((b.\"PriceBefDi\"*\"Quantity\") * (b.\"DiscPrcnt\"/100)) \"LineDisc\",l.\"InvntryUom\" ,IFNULL(a.\"RoundDif\",0) \"Rounding\", a.\"TaxDate\" \"Cust Order Date\", a.\"TotalExpns\",l.\"FrgnName\",a.\"DocCur\",A.\"VatSum\", a.\"TotalExpns\" \"Freight\",";
            retstring = retstring + " l.\"SalUnitMsr\",IFNULL(b.\"LineTotal\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Line Total\", IFNULL(a.\"TotalExpns\",0) \"Freight Total\",IFNULL(a.\"DiscSum\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Disc Total\",l.\"ItemClass\", ";

            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " i.\"TransID\" ,i.\"TransName\" ,i.\"TransDocNo\" ,i.\"Distance\" ,i.\"TransMode\" ,i.\"VehicleNo\" ,i.\"VehicleTyp\",";
                retstring = retstring + " i.\"TransType\",i.\"TransDate\",i.\"SuplyType\",";
                retstring = retstring + " i.\"FrmGSTN\" ,i.\"FrmTraName\" ,i.\"FrmAddres1\" ,i.\"FrmAddres2\" ,i.\"FrmPlace\" ,Replace(i.\"FrmZipCode\",' ','') as \"FrmZipCode\" ,";
                retstring = retstring + " i.\"ActFrmStat\" ,i.\"ToGSTN\" ,i.\"ToTraName\" ,i.\"ToAddres1\" ,i.\"ToAddres2\" ,i.\"ToPlace\" ,Replace(i.\"ToZipCode\",' ','') as \"ToZipCode\"  ,";
                retstring = retstring + " i.\"ActToState\",i.\"SubSplyTyp\",ES.\"SubType\" \"SubtypeDescription\", i.\"DocType\" \"EDocType\" ,";

                retstring += " i.\"U_Dispatch_Eway\" as \"DisEway\" ,";
                retstring += " i.\"U_Dispatch_Name\" as \"DisName\" ,";
                retstring = retstring + " i.\"EwbDate\" \"EwbDate\" ,i.\"EWayBillNo\" \"EwayNo\" ,";
                retstring = retstring + " i.\"FrmState\" \"FrmState\" ,i.\"ToState\" \"ToState\" ,";

                retstring = retstring + " T.\"ClaimRefun\",";
            }
            else
            {
                if (!string.IsNullOrEmpty(clsModule.EwayUDF))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayUDF + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportName))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportName + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportId))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportId + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayDistance))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayDistance + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayNo))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayNo + @""" ""EwayNo"",";
                    retstring = retstring + " '' as  \"EwbDate\",";
                }
            }

            retstring = retstring + " (Select \"ServCode\" from OSAC where b.\"SacEntry\"= \"AbsEntry\")\"SacCode\" ";

          retstring = retstring + ",a.\"U_IRNNo\" ,a.\"U_AckDate\" ";

            retstring = retstring + " FROM ODLN a INNER JOIN DLN1 b on b.\"DocEntry\" = a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN DLN26 i ON i.\"DocEntry\" =a.\"DocEntry\"";
            }

            retstring = retstring + " left JOIN OCRD g on g.\"CardCode\" = a.\"CardCode\"";
            retstring = retstring + " left JOIN OITM l on l.\"ItemCode\" = b.\"ItemCode\"";
            retstring = retstring + " left JOIN OCRD m on m.\"CardCode\" = a.\"CardCode\" ";
            retstring = retstring + " LEFT JOIN OCPR n on n.\"CardCode\" = a.\"CardCode\" and n.\"CntctCode\" = a.\"CntctCode\"";
            retstring = retstring + " LEFT JOIN OCHP o on o.\"AbsEntry\" = l.\"ChapterID\" ";
            retstring = retstring + " LEFT JOIN OLCT ss on ss.\"Code\" = b.\"LocCode\"";
            retstring = retstring + " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\" =a.\"CardCode\" and CRD1.\"Address\" =A.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
            retstring = retstring + " LEFT JOIN OCST st on st.\"Code\"=CRD1.\"State\" and st.\"Country\"=CRD1.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy on cy.\"Code\" =CRD1.\"Country\"";
            retstring = retstring + " LEFT JOIN CRD1 crd11 on crd11.\"CardCode\" =a.\"CardCode\" and crd11.\"Address\" =A.\"PayToCode\" and crd11.\"AdresType\"='B'";
            retstring = retstring + " LEFT JOIN OCST st1 on st1.\"Code\"=crd11.\"State\" and st1.\"Country\"=crd11.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy1 on cy1.\"Code\" =crd11.\"Country\"";
            retstring = retstring + " CROSS JOIN OADM B1";
            retstring = retstring + " CROSS JOIN ADM1 B3";
            retstring = retstring + " LEFT JOIN DLN12 T ON T.\"DocEntry\"=a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN OEST ES ON ES.\"SubID\" =i.\"SubSplyTyp\"";
            }
            retstring = retstring + " LEFT JOIN NNM1 nnm1 ON a.\"Series\" =nnm1.\"Series\"";

            retstring = retstring + " LEFT JOIN(SELECT \"BankName\" \"CBankName\",Y.\"BankCode\" \"CBankCode\",\"Branch\" \"CBranch\", \"Account\" \"CAccount\",\"AcctName\" \"CAcctName\",";
            retstring = retstring + " X.\"SwiftNum\" \"CIFSCNo\" FROM DSC1 X,ODSC Y Where X.\"AbsEntry\"=Y.\"AbsEntry\" ) B2 On B2.\"CBankCode\"=B1.\"DflBnkCode\"";
            retstring = retstring + " WHERE a.\"DocEntry\"=" + Docentry + ")B";
            clsModule.objaddon.objglobalmethods.WriteErrorLog(retstring);
            return retstring;


        }


        public string DebitNoteQuery(string Docentry)
        {
            string retstring = "";

            retstring = "SELECT Row_number() Over(Partition by b.\"DocEntry\" order by b.\"DocEntry\" Asc )\" SINo\",(B.\"AssVal\"+B.\"Freight Total\") \"AssValN\" ,";
            retstring = retstring + " (B.\"AssAmt\"+B.\"CGSTAmt\"+B.\"SGSTAmt\"+B.\"IGSTAmt\") \"Total Item Value\",CAST((B.\"Tot Amt\"-B.\"Tot Amt1\")AS nvarchar(200)) \"LineDiscountAmt\",B.* FROM(";
            retstring = retstring + "  Select B2.*, 'GST' as \"TaxSch\",T.\"BpGSTN\" as \"T_BpGSTN\",";

            retstring = retstring += docseries + " \"Inv_No\",";


            if (!string.IsNullOrEmpty(clsModule.ItemDsc))
            {
                retstring = retstring + "b.\"" + clsModule.ItemDsc + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.GSTCol))
            {
                retstring = retstring + "T.\"" + clsModule.GSTCol + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.HSNCol))
            {
                retstring = retstring + "b.\"" + clsModule.HSNCol + @""",";
            }

            retstring = retstring += " CASE WHEN  T.\"ExportType\" in('U','S') THEN case when a.\"DutyStatus\" ='Y' then 'SEZWP' else 'SEZWOP' End ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN  case when a.\"DutyStatus\" ='Y' then 'EXPWP' else 'EXPWOP' End  ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'D' THEN 'DEXP' ";
            retstring = retstring += "      ELSE 'B2B' END as \"SupTyp\",";

            retstring = retstring + " '' as \"RegRev\",'DBN' Type,a.\"DocEntry\",a.\"DocType\",a.\"DocDate\" \"Inv_Doc_Date\",ss.\"GSTRegnNo\" \"Seller GSTN\",";

            retstring = retstring + " B1.\"CompnyName\" \"Seller_Legal Name\",ss.\"City\" \"Seller Location Name\", Replace(ss.\"ZipCode\",' ','') \"Seller_PIN code\", ";
            retstring = retstring + " (CASE WHEN ss.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ss.\"Country\" and \"Code\"=ss.\"State\") ";
            retstring = retstring + " ELSE '96' END) \"Seller_State_code\",";

            retstring = retstring + " B3.\"GlblLocNum\" as \"CompnyGSTIN\", ";
            retstring = retstring + " B1.\"CompnyAddr\",Replace(B3.\"ZipCode\",' ','') \"CompnyPincode\",B3.\"City\" \"CompnyCity\", ";

            retstring = retstring + " (CASE WHEN B3.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=B3.\"Country\" and \"Code\"=B3.\"State\") ";
            retstring = retstring + " ELSE '96' END ) \"Compnystatecode\", ";

            retstring = retstring += " CASE WHEN COALESCE(Crd11.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd11.\"GSTRegnNo\" END as \"Buyer GSTN\",";
            retstring = retstring += " CASE WHEN COALESCE(Crd1.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd1.\"GSTRegnNo\" END as \"Shipto GSTN\",";

            retstring = retstring += " T.\"PortCode\",T.\"ImpExpNo\" ,T.\"ImpExpDate\" ,COALESCE(st.\"Country\",cy.\"Code\") \"CCode\",  ";

            retstring = retstring += " a.\"BPLId\" ,";

            retstring = retstring + " a.\"CardCode\", a.\"CardName\" \"Buyer_Legal Name\",";
            retstring = retstring + " crd11.\"City\" \"BCity\",st1.\"Name\" \"BState\" ,cy1.\"Name\" \"BCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd11.\"ZipCode\",' ','') END as  \"BZipCode\",";

            retstring = retstring + " crd1.\"City\" \"SCity\",st.\"Name\" \"SState\", cy.\"Name\" \"SCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd1.\"ZipCode\",' ','') END as \"SZipCode\",";

            retstring = retstring += " ss.\"Building\" \"Seller_Building\",ss.\"Block\" \"Seller_Block\",ss.\"Street\" \"Seller_Street\",";
            retstring = retstring += " crd11.\"Building\" \"Buyer_Building\",crd11.\"Block\" \"Buyer_Block\",crd11.\"StreetNo\" \"Buyer_StreetNo\",crd11.\"Street\" \"Buyer_Street\",crd11.\"Address2\" \"Buyer_Address2\",crd11.\"Address3\" \"Buyer_Address3\",";
            retstring = retstring += " crd1.\"Building\" \"Ship_Building\",crd1.\"Block\" \"Ship_Block\",crd1.\"StreetNo\" \"Ship_StreetNo\",crd1.\"Street\" \"Ship_Street\",crd1.\"Address2\" \"Ship_Address2\",crd1.\"Address3\" \"Ship_Address3\",";



            retstring = retstring += " T.\"BuildingB\" \"T_Buyer_Building\",T.\"BlockB\" \"T_Buyer_Block\",T.\"StreetNoB\" \"T_Buyer_StreetNo\",T.\"StreetB\" \"T_Buyer_Street\",T.\"Address2B\" \"T_Buyer_Address2\",T.\"Address3B\" \"T_Buyer_Address3\",";
            retstring = retstring += " T.\"CityB\" \"T_BCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeB\",' ','') END as  \"T_BZipCode\", ";
            retstring = retstring += " (case when T.\"CountryB\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateB\" and \"Country\"=T.\"CountryB\") ElSE '96' END) \"T_Bill to State Code\", ";

            retstring = retstring += " T.\"BuildingS\" \"T_Ship_Building\",T.\"BlockS\" \"T_Ship_Block\",T.\"StreetNoS\" \"T_Ship_StreetNo\",T.\"StreetS\" \"T_Ship_Street\",T.\"Address2S\" \"T_Ship_Address2\",T.\"Address3S\" \"T_Ship_Address3\",";
            retstring = retstring += " T.\"CityS\" \"T_SCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeS\",' ','') END as  \"T_SZipCode\", ";
            retstring = retstring += " (case when T.\"CountryS\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateS\" and \"Country\"=T.\"CountryS\") ELSE '96' END) \"T_Shipp to State Code\", ";


            retstring = retstring + " (case when Crd11.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=Crd11.\"State\" and \"Country\"=Crd11.\"Country\") ELSE '96' END) \"Bill to State Code\",";
            retstring = retstring + " (case when crd1.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=crd1.\"State\" and \"Country\"=crd1.\"Country\") ELSE '96' END ) \"Shipp to State Code\",";
            retstring = retstring + " b.\"ItemCode\", b.\"Dscription\",Case when a.\"DocType\"='S' then 'Y' Else 'N' End \"IsServc\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then (Select Case when LEft(\"ServCode\",2) like '0%' then Replace(\"ServCode\",'0','') Else \"ServCode\" End from OSAC where b.\"SacEntry\"= \"AbsEntry\") Else Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") End \"HSN\",Case when a.\"DocType\"='S' then 1 Else b.\"Quantity\" End \"Quantity\",COALESCE(b.\"unitMsr\",b.\"UomCode\") \"Unit\",";

            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"Price\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end  else b.\"PriceBefDi\" * a.\"DocRate\" end \"UnitPrice\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"PriceBefDi\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt1\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"AssAmt\",";

            
            retstring = retstring + " (Select Sum(X.\"TaxRate\") From RPC4 X Where X.\"DocEntry\"=A.\"DocEntry\" And  X.\"LineNum\"=B.\"LineNum\" and X.\"ExpnsCode\"='-1' ) \"GSTRATE\",IFNULL((select sum(\"TaxSum\") from RPC4 where \"DocEntry\"=a.\"DocEntry\"and \"staType\"=-100),0) as \"CGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from RPC4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-110),0) as \"SGSTVal\", IFNULL((select sum(\"TaxSum\") from RPC4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-120),0) as \"IGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from RPC4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-100 and \"ExpnsCode\"='-1'),0) as \"CGSTAmt\", IFNULL((select sum(\"TaxSum\") from RPC4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-110 and \"ExpnsCode\"='-1'),0) as \"SGSTAmt\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from RPC4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-120 and \"ExpnsCode\"='-1'),0) as \"IGSTAmt\",";
            retstring = retstring + " (SELECT MAX(\"BatchNum\") from IBT1 where \"ItemCode\"=b.\"ItemCode\" and \"WhsCode\"=b.\"WhsCode\" and";
            retstring = retstring + " \"BaseType\" in ('19') and \"BaseEntry\"= CASE WHEN  b.\"BaseType\" =-1 THEN b.\"DocEntry\" ELSE b.\"BaseEntry\"  end  ) AS \"BatchNum\",";

            //retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from RPC1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

            retstring = retstring + " Case when a.\"DocType\"='S' then  (Select Sum(IFNULL(\"LineTotal\",0))* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from RPC1 where \"DocEntry\"=b.\"DocEntry\") " +
                                                           "Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from RPC1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

          
            retstring = retstring + " (a.\"DocTotal\" +a.\"DpmAmnt\") - ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN  0 ELSE COALESCE((Select Sum(RPC5.\"WTAmnt\") from RPC5 where RPC5.\"AbsEntry\" = a.\"DocEntry\"),0.0) END ) AS \"Doc Total\", ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN   COALESCE((Select Sum(RPC5.\"WTAmnt\") from RPC5 where RPC5.\"AbsEntry\" = a.\"DocEntry\"),0.0) ELSE 0 END ) AS \"OthrAmt\", ";



            retstring = retstring + " a.\"DocDueDate\" \"Inv Due Date\", a.\"NumAtCard\", a.\"Printed\",a.\"PayToCode\", a.\"ShipToCode\", a.\"Comments\" ,Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") \"ChapterID\" , A.\"DiscSum\", A.\"RoundDif\",";
            retstring = retstring + " b.\"DiscPrcnt\",((b.\"PriceBefDi\"*\"Quantity\") * (b.\"DiscPrcnt\"/100)) \"LineDisc\",l.\"InvntryUom\" ,IFNULL(a.\"RoundDif\",0) \"Rounding\", a.\"TaxDate\" \"Cust Order Date\", a.\"TotalExpns\",l.\"FrgnName\",a.\"DocCur\",A.\"VatSum\", a.\"TotalExpns\" \"Freight\",";
            retstring = retstring + " l.\"SalUnitMsr\",IFNULL(b.\"LineTotal\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Line Total\", IFNULL(a.\"TotalExpns\",0) \"Freight Total\",IFNULL(a.\"DiscSum\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Disc Total\",l.\"ItemClass\", ";

            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " i.\"TransID\" ,i.\"TransName\" ,i.\"TransDocNo\" ,i.\"Distance\" ,i.\"TransMode\" ,i.\"VehicleNo\" ,i.\"VehicleTyp\",";
                retstring = retstring + " i.\"TransType\",i.\"TransDate\",i.\"SuplyType\",";
                retstring = retstring + " i.\"FrmGSTN\" ,i.\"FrmTraName\" ,i.\"FrmAddres1\" ,i.\"FrmAddres2\" ,i.\"FrmPlace\" ,Replace(i.\"FrmZipCode\",' ','') as \"FrmZipCode\" ,";
                retstring = retstring + " i.\"ActFrmStat\" ,i.\"ToGSTN\" ,i.\"ToTraName\" ,i.\"ToAddres1\" ,i.\"ToAddres2\" ,i.\"ToPlace\" ,Replace(i.\"ToZipCode\",' ','') as \"ToZipCode\"  ,";
                retstring = retstring + " i.\"ActToState\",i.\"SubSplyTyp\",ES.\"SubType\" \"SubtypeDescription\", i.\"DocType\" \"EDocType\" ,";

                retstring += " i.\"U_Dispatch_Eway\" as \"DisEway\" ,";
                retstring += " i.\"U_Dispatch_Name\" as \"DisName\" ,";
                retstring = retstring + " i.\"EwbDate\" \"EwbDate\" ,i.\"EWayBillNo\" \"EwayNo\" ,";
                retstring = retstring + " i.\"FrmState\" \"FrmState\" ,i.\"ToState\" \"ToState\" ,";

                retstring = retstring + " T.\"ClaimRefun\",";
            }
            else
            {
                if (!string.IsNullOrEmpty(clsModule.EwayUDF))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayUDF + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportName))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportName + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportId))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportId + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayDistance))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayDistance + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayNo))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayNo + @""" ""EwayNo"",";
                    retstring = retstring + " '' as  \"EwbDate\",";
                }
            }

            retstring = retstring + " (Select \"ServCode\" from OSAC where b.\"SacEntry\"= \"AbsEntry\")\"SacCode\" ";


          retstring = retstring + ",a.\"U_IRNNo\" ,a.\"U_AckDate\" ";

            retstring = retstring + " FROM ORPC a INNER JOIN RPC1 b on b.\"DocEntry\" = a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN RPC26 i ON i.\"DocEntry\" =a.\"DocEntry\"";
            }

            retstring = retstring + " left JOIN OCRD g on g.\"CardCode\" = a.\"CardCode\"";
            retstring = retstring + " left JOIN OITM l on l.\"ItemCode\" = b.\"ItemCode\"";
            retstring = retstring + " left JOIN OCRD m on m.\"CardCode\" = a.\"CardCode\" ";
            retstring = retstring + " LEFT JOIN OCPR n on n.\"CardCode\" = a.\"CardCode\" and n.\"CntctCode\" = a.\"CntctCode\"";
            retstring = retstring + " LEFT JOIN OCHP o on o.\"AbsEntry\" = l.\"ChapterID\" ";
            retstring = retstring + " LEFT JOIN OLCT ss on ss.\"Code\" = b.\"LocCode\"";
            retstring = retstring + " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\" =a.\"CardCode\" and CRD1.\"Address\" =A.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
            retstring = retstring + " LEFT JOIN OCST st on st.\"Code\"=CRD1.\"State\" and st.\"Country\"=CRD1.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy on cy.\"Code\" =CRD1.\"Country\"";
            retstring = retstring + " LEFT JOIN CRD1 crd11 on crd11.\"CardCode\" =a.\"CardCode\" and crd11.\"Address\" =A.\"PayToCode\" and crd11.\"AdresType\"='B'";
            retstring = retstring + " LEFT JOIN OCST st1 on st1.\"Code\"=crd11.\"State\" and st1.\"Country\"=crd11.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy1 on cy1.\"Code\" =crd11.\"Country\"";
            retstring = retstring + " CROSS JOIN OADM B1";
            retstring = retstring + " CROSS JOIN ADM1 B3";
            retstring = retstring + " LEFT JOIN RPC12 T ON T.\"DocEntry\"=a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN OEST ES ON ES.\"SubID\" =i.\"SubSplyTyp\"";
            }
            retstring = retstring + " LEFT JOIN NNM1 nnm1 ON a.\"Series\" =nnm1.\"Series\"";

            retstring = retstring + " LEFT JOIN(SELECT \"BankName\" \"CBankName\",Y.\"BankCode\" \"CBankCode\",\"Branch\" \"CBranch\", \"Account\" \"CAccount\",\"AcctName\" \"CAcctName\",";
            retstring = retstring + " X.\"SwiftNum\" \"CIFSCNo\" FROM DSC1 X,ODSC Y Where X.\"AbsEntry\"=Y.\"AbsEntry\" ) B2 On B2.\"CBankCode\"=B1.\"DflBnkCode\"";
            retstring = retstring + " WHERE a.\"DocEntry\"=" + Docentry + ")B";
            clsModule.objaddon.objglobalmethods.WriteErrorLog(retstring);
            return retstring;


        }

        public string PurchaseQuery(string Docentry)
        {
            string retstring = "";

            retstring = "SELECT Row_number() Over(Partition by b.\"DocEntry\" order by b.\"DocEntry\" Asc )\" SINo\",(B.\"AssVal\"+B.\"Freight Total\") \"AssValN\" ,";
            retstring = retstring + " (B.\"AssAmt\"+B.\"CGSTAmt\"+B.\"SGSTAmt\"+B.\"IGSTAmt\") \"Total Item Value\",CAST((B.\"Tot Amt\"-B.\"Tot Amt1\")AS nvarchar(200)) \"LineDiscountAmt\",B.* FROM(";
            retstring = retstring + "  Select B2.*, 'GST' as \"TaxSch\",T.\"BpGSTN\" as \"T_BpGSTN\",";

            retstring = retstring += docseries + " \"Inv_No\",";

            if (!string.IsNullOrEmpty(clsModule.ItemDsc))
            {
                retstring = retstring + "b.\"" + clsModule.ItemDsc + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.GSTCol))
            {
                retstring = retstring + "T.\"" + clsModule.GSTCol + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.HSNCol))
            {
                retstring = retstring + "b.\"" + clsModule.HSNCol + @""",";
            }

            retstring = retstring += " CASE WHEN  T.\"ExportType\" in('U','S') THEN case when a.\"DutyStatus\" ='Y' then 'SEZWP' else 'SEZWOP' End ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN  case when a.\"DutyStatus\" ='Y' then 'EXPWP' else 'EXPWOP' End  ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'D' THEN 'DEXP' ";
            retstring = retstring += "      ELSE 'B2B' END as \"SupTyp\",";

            retstring = retstring + " '' as \"RegRev\",'INV' Type,a.\"DocEntry\",a.\"DocType\",a.\"DocDate\" \"Inv_Doc_Date\",ss.\"GSTRegnNo\" \"Seller GSTN\",";
            retstring = retstring + " B1.\"CompnyName\" \"Seller_Legal Name\",ss.\"City\" \"Seller Location Name\", Replace(ss.\"ZipCode\",' ','') \"Seller_PIN code\", ";
            retstring = retstring + " (CASE WHEN ss.\"Country\"='IN' then ( select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ss.\"Country\" and \"Code\"=ss.\"State\" ) ";
            retstring = retstring + " ELSE '96' END) \"Seller_State_code\",";


            retstring = retstring + " B3.\"GlblLocNum\" as \"CompnyGSTIN\", ";
            retstring = retstring + " B1.\"CompnyAddr\",Replace(B3.\"ZipCode\",' ','') \"CompnyPincode\",B3.\"City\" \"CompnyCity\", ";

            retstring = retstring + " (CASE WHEN B3.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=B3.\"Country\" and \"Code\"=B3.\"State\") ";
            retstring = retstring + " ELSE '96' END ) \"Compnystatecode\", ";


            retstring = retstring += " CASE WHEN COALESCE(Crd11.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd11.\"GSTRegnNo\" END as \"Buyer GSTN\",";
            retstring = retstring += " CASE WHEN COALESCE(Crd1.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd1.\"GSTRegnNo\" END as \"Shipto GSTN\",";

            retstring = retstring += " T.\"PortCode\",T.\"ImpExpNo\" ,T.\"ImpExpDate\", COALESCE(st.\"Country\",cy.\"Code\") \"CCode\",   ";

            retstring = retstring += " a.\"BPLId\" ,";

            retstring = retstring + " a.\"CardCode\", a.\"CardName\" \"Buyer_Legal Name\",";
            retstring = retstring + " crd11.\"City\" \"BCity\",st1.\"Name\" \"BState\" ,cy1.\"Name\" \"BCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd11.\"ZipCode\",' ','') END as  \"BZipCode\",";

            retstring = retstring + " crd1.\"City\" \"SCity\",st.\"Name\" \"SState\", cy.\"Name\" \"SCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd1.\"ZipCode\",' ','') END as \"SZipCode\",";

            retstring = retstring += " ss.\"Building\" \"Seller_Building\",ss.\"Block\" \"Seller_Block\",ss.\"Street\" \"Seller_Street\",";
            retstring = retstring += " crd11.\"Building\" \"Buyer_Building\",crd11.\"Block\" \"Buyer_Block\",crd11.\"StreetNo\" \"Buyer_StreetNo\",crd11.\"Street\" \"Buyer_Street\",crd11.\"Address2\" \"Buyer_Address2\",crd11.\"Address3\" \"Buyer_Address3\",";
            retstring = retstring += " crd1.\"Building\" \"Ship_Building\",crd1.\"Block\" \"Ship_Block\",crd1.\"StreetNo\" \"Ship_StreetNo\",crd1.\"Street\" \"Ship_Street\",crd1.\"Address2\" \"Ship_Address2\",crd1.\"Address3\" \"Ship_Address3\",";


            retstring = retstring += " T.\"BuildingB\" \"T_Buyer_Building\",T.\"BlockB\" \"T_Buyer_Block\",T.\"StreetNoB\" \"T_Buyer_StreetNo\",T.\"StreetB\" \"T_Buyer_Street\",T.\"Address2B\" \"T_Buyer_Address2\",T.\"Address3B\" \"T_Buyer_Address3\",";
            retstring = retstring += " T.\"CityB\" \"T_BCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeB\",' ','') END as  \"T_BZipCode\", ";
            retstring = retstring += " (case when T.\"CountryB\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateB\" and \"Country\"=T.\"CountryB\") ElSE '96' END) \"T_Bill to State Code\", ";

            retstring = retstring += " T.\"BuildingS\" \"T_Ship_Building\",T.\"BlockS\" \"T_Ship_Block\",T.\"StreetNoS\" \"T_Ship_StreetNo\",T.\"StreetS\" \"T_Ship_Street\",T.\"Address2S\" \"T_Ship_Address2\",T.\"Address3S\" \"T_Ship_Address3\",";
            retstring = retstring += " T.\"CityS\" \"T_SCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeS\",' ','') END as  \"T_SZipCode\", ";
            retstring = retstring += " (case when T.\"CountryS\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateS\" and \"Country\"=T.\"CountryS\") ELSE '96' END) \"T_Shipp to State Code\", ";


            retstring = retstring + " (case when Crd11.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=Crd11.\"State\" and \"Country\"=Crd11.\"Country\") ELSE '96' END) \"Bill to State Code\",";
            retstring = retstring + " (case when crd1.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=crd1.\"State\" and \"Country\"=crd1.\"Country\") ELSE '96' END ) \"Shipp to State Code\",";
            retstring = retstring + " b.\"ItemCode\", b.\"Dscription\",Case when a.\"DocType\"='S' then 'Y' Else 'N' End \"IsServc\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then (Select Case when LEft(\"ServCode\",2) like '0%' then Replace(\"ServCode\",'0','') Else \"ServCode\" End from OSAC where b.\"SacEntry\"= \"AbsEntry\") Else Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") End \"HSN\",Case when a.\"DocType\"='S' then 1 Else b.\"Quantity\" End \"Quantity\",COALESCE(b.\"unitMsr\",b.\"UomCode\") \"Unit\",";


            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"Price\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end  else b.\"PriceBefDi\" * a.\"DocRate\" end \"UnitPrice\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"PriceBefDi\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt1\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"AssAmt\",";

            retstring = retstring + " (Select Sum(X.\"TaxRate\") From PCH4 X Where X.\"DocEntry\"=A.\"DocEntry\" And X.\"LineNum\"=B.\"LineNum\" and X.\"ExpnsCode\"='-1' ) \"GSTRATE\",IFNULL((select sum(\"TaxSum\") from PCH4 where \"DocEntry\"=a.\"DocEntry\"and \"staType\"=-100),0) as \"CGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from PCH4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-110),0) as \"SGSTVal\", IFNULL((select sum(\"TaxSum\") from PCH4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-120),0) as \"IGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from PCH4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-100 and \"ExpnsCode\"='-1'),0) as \"CGSTAmt\", IFNULL((select sum(\"TaxSum\") from PCH4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-110 and \"ExpnsCode\"='-1'),0) as \"SGSTAmt\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from PCH4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-120 and \"ExpnsCode\"='-1'),0) as \"IGSTAmt\",";
            retstring = retstring + " (SELECT MAX(\"BatchNum\") from IBT1 where \"ItemCode\"=b.\"ItemCode\" and \"WhsCode\"=b.\"WhsCode\" and";
            retstring = retstring + " \"BaseType\" in ('18') and \"BaseEntry\"= CASE WHEN  b.\"BaseType\" =-1 THEN b.\"DocEntry\" ELSE b.\"BaseEntry\"  end  ) AS \"BatchNum\",";

            //retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from PCH1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

            retstring = retstring + " Case when a.\"DocType\"='S' then  (Select Sum(IFNULL(\"LineTotal\",0))* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from PCH1 where \"DocEntry\"=b.\"DocEntry\") " +
                                               "Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from PCH1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

            retstring = retstring + " (a.\"DocTotal\" +a.\"DpmAmnt\") - ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN  0 ELSE COALESCE((Select Sum(PCH5.\"WTAmnt\") from PCH5 where PCH5.\"AbsEntry\" = a.\"DocEntry\"),0.0) END ) AS \"Doc Total\", ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN   COALESCE((Select Sum(PCH5.\"WTAmnt\") from PCH5 where PCH5.\"AbsEntry\" = a.\"DocEntry\"),0.0) ELSE 0 END ) AS \"OthrAmt\", ";


            retstring = retstring + " a.\"DocDueDate\" \"Inv Due Date\", a.\"NumAtCard\", a.\"Printed\",a.\"PayToCode\", a.\"ShipToCode\", a.\"Comments\" ,Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") \"ChapterID\" , A.\"DiscSum\", A.\"RoundDif\",";
            retstring = retstring + " b.\"DiscPrcnt\",((b.\"PriceBefDi\"*\"Quantity\") * (b.\"DiscPrcnt\"/100)) \"LineDisc\",l.\"InvntryUom\" ,IFNULL(a.\"RoundDif\",0) \"Rounding\", a.\"TaxDate\" \"Cust Order Date\", a.\"TotalExpns\",l.\"FrgnName\",a.\"DocCur\",A.\"VatSum\", a.\"TotalExpns\" \"Freight\",";
            retstring = retstring + " l.\"SalUnitMsr\",IFNULL(b.\"LineTotal\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Line Total\", IFNULL(a.\"TotalExpns\",0) \"Freight Total\",IFNULL(a.\"DiscSum\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Disc Total\",l.\"ItemClass\", ";

            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " i.\"TransID\" ,i.\"TransName\" ,i.\"TransDocNo\" ,i.\"Distance\" ,i.\"TransMode\" ,i.\"VehicleNo\" ,i.\"VehicleTyp\",";
                retstring = retstring + " i.\"TransType\",i.\"TransDate\",i.\"SuplyType\",";
                retstring = retstring + " i.\"FrmGSTN\" ,i.\"FrmTraName\" ,i.\"FrmAddres1\" ,i.\"FrmAddres2\" ,i.\"FrmPlace\" ,Replace(i.\"FrmZipCode\",' ','') as \"FrmZipCode\" ,";
                retstring = retstring + " i.\"ActFrmStat\" ,i.\"ToGSTN\" ,i.\"ToTraName\" ,i.\"ToAddres1\" ,i.\"ToAddres2\" ,i.\"ToPlace\" ,Replace(i.\"ToZipCode\",' ','') as \"ToZipCode\"  ,";
                retstring = retstring + " i.\"ActToState\",i.\"SubSplyTyp\",ES.\"SubType\" \"SubtypeDescription\", i.\"DocType\" \"EDocType\" ,";

                retstring += " i.\"U_Dispatch_Eway\" as \"DisEway\" ,";
                retstring += " i.\"U_Dispatch_Name\" as \"DisName\" ,";
                retstring = retstring + " i.\"EwbDate\" \"EwbDate\" ,i.\"EWayBillNo\" \"EwayNo\" ,";
                retstring = retstring + " i.\"FrmState\" \"FrmState\" ,i.\"ToState\" \"ToState\" ,";

                retstring = retstring + " T.\"ClaimRefun\",";
            }
            else
            {
                if (!string.IsNullOrEmpty(clsModule.EwayUDF))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayUDF + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportName))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportName + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportId))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportId + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayDistance))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayDistance + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayNo))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayNo + @""" ""EwayNo"",";
                    retstring = retstring + " '' as  \"EwbDate\",";
                }
            }

            retstring = retstring + " (Select \"ServCode\" from OSAC where b.\"SacEntry\"= \"AbsEntry\")\"SacCode\" ";


          retstring = retstring + ",a.\"U_IRNNo\" ,a.\"U_AckDate\" ";

            retstring = retstring + " FROM OPCH a INNER JOIN PCH1 b on b.\"DocEntry\" = a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN PCH26 i ON i.\"DocEntry\" =a.\"DocEntry\"";
            }

            retstring = retstring + " left JOIN OCRD g on g.\"CardCode\" = a.\"CardCode\"";
            retstring = retstring + " left JOIN OITM l on l.\"ItemCode\" = b.\"ItemCode\"";
            retstring = retstring + " left JOIN OCRD m on m.\"CardCode\" = a.\"CardCode\" ";
            retstring = retstring + " LEFT JOIN OCPR n on n.\"CardCode\" = a.\"CardCode\" and n.\"CntctCode\" = a.\"CntctCode\"";
            retstring = retstring + " LEFT JOIN OCHP o on o.\"AbsEntry\" = l.\"ChapterID\" ";
            retstring = retstring + " LEFT JOIN OLCT ss on ss.\"Code\" = b.\"LocCode\"";
            retstring = retstring + " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\" =a.\"CardCode\" and CRD1.\"Address\" =A.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
            retstring = retstring + " LEFT JOIN OCST st on st.\"Code\"=CRD1.\"State\" and st.\"Country\"=CRD1.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy on cy.\"Code\" =CRD1.\"Country\"";
            retstring = retstring + " LEFT JOIN CRD1 crd11 on crd11.\"CardCode\" =a.\"CardCode\" and crd11.\"Address\" =A.\"PayToCode\" and crd11.\"AdresType\"='B'";
            retstring = retstring + " LEFT JOIN OCST st1 on st1.\"Code\"=crd11.\"State\" and st1.\"Country\"=crd11.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy1 on cy1.\"Code\" =crd11.\"Country\"";
            retstring = retstring + " CROSS JOIN OADM B1";
            retstring = retstring + " CROSS JOIN ADM1 B3";
            retstring = retstring + " LEFT JOIN PCH12 T ON T.\"DocEntry\"=a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN OEST ES ON ES.\"SubID\" =i.\"SubSplyTyp\"";
            }
            retstring = retstring + " LEFT JOIN NNM1 nnm1 ON a.\"Series\" =nnm1.\"Series\"";

            retstring = retstring + " LEFT JOIN(SELECT \"BankName\" \"CBankName\",Y.\"BankCode\" \"CBankCode\",\"Branch\" \"CBranch\", \"Account\" \"CAccount\",\"AcctName\" \"CAcctName\",";
            retstring = retstring + " X.\"SwiftNum\" \"CIFSCNo\" FROM DSC1 X,ODSC Y Where X.\"AbsEntry\"=Y.\"AbsEntry\" ) B2 On B2.\"CBankCode\"=B1.\"DflBnkCode\"";
            retstring = retstring + " WHERE a.\"DocEntry\"=" + Docentry + ")B";
            clsModule.objaddon.objglobalmethods.WriteErrorLog(retstring);
            return retstring;


        }


        public string InvoiceDownQuery(string Docentry)
        {
            string retstring = "";

            retstring = "SELECT Row_number() Over(Partition by b.\"DocEntry\" order by b.\"DocEntry\" Asc )\" SINo\",(B.\"AssVal\"+B.\"Freight Total\") \"AssValN\" ,";
            retstring = retstring + " (B.\"AssAmt\"+B.\"CGSTAmt\"+B.\"SGSTAmt\"+B.\"IGSTAmt\") \"Total Item Value\",CAST((B.\"Tot Amt\"-B.\"Tot Amt1\")AS nvarchar(200)) \"LineDiscountAmt\",B.* FROM(";
            retstring = retstring + "  Select B2.*, 'GST' as \"TaxSch\",T.\"BpGSTN\" as \"T_BpGSTN\",";

            retstring = retstring += docseries + " \"Inv_No\",";

            if (!string.IsNullOrEmpty(clsModule.ItemDsc))
            {
                retstring = retstring + "b.\"" + clsModule.ItemDsc + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.GSTCol))
            {
                retstring = retstring + "T.\"" + clsModule.GSTCol + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.HSNCol))
            {
                retstring = retstring + "b.\"" + clsModule.HSNCol + @""",";
            }

            retstring = retstring += " CASE WHEN  T.\"ExportType\" in('U','S') THEN case when a.\"DutyStatus\" ='Y' then 'SEZWP' else 'SEZWOP' End ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN  case when a.\"DutyStatus\" ='Y' then 'EXPWP' else 'EXPWOP' End  ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'D' THEN 'DEXP' ";
            retstring = retstring += "      ELSE 'B2B' END as \"SupTyp\",";

            retstring = retstring + " '' as \"RegRev\",'INV' Type,a.\"DocEntry\",a.\"DocType\",a.\"DocDate\" \"Inv_Doc_Date\",ss.\"GSTRegnNo\" \"Seller GSTN\",";

            retstring = retstring + " B1.\"CompnyName\" \"Seller_Legal Name\",ss.\"City\" \"Seller Location Name\", Replace(ss.\"ZipCode\",' ','') \"Seller_PIN code\", ";
            retstring = retstring + " (CASE WHEN ss.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ss.\"Country\" and \"Code\"=ss.\"State\") ";
            retstring = retstring + " ELSE '96' END) \"Seller_State_code\",";

            retstring = retstring + " B3.\"GlblLocNum\" as \"CompnyGSTIN\", ";
            retstring = retstring + " B1.\"CompnyAddr\",Replace(B3.\"ZipCode\",' ','') \"CompnyPincode\",B3.\"City\" \"CompnyCity\", ";

            retstring = retstring + " (CASE WHEN B3.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=B3.\"Country\" and \"Code\"=B3.\"State\") ";
            retstring = retstring + " ELSE '96' END ) \"Compnystatecode\", ";


            retstring = retstring += " CASE WHEN COALESCE(Crd11.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd11.\"GSTRegnNo\" END as \"Buyer GSTN\",";
            retstring = retstring += " CASE WHEN COALESCE(Crd1.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd1.\"GSTRegnNo\" END as \"Shipto GSTN\",";

            retstring = retstring += " T.\"PortCode\",T.\"ImpExpNo\" ,T.\"ImpExpDate\" ,COALESCE(st.\"Country\",cy.\"Code\") \"CCode\",   ";

            retstring = retstring += " a.\"BPLId\" ,";

            retstring = retstring + " a.\"CardCode\", a.\"CardName\" \"Buyer_Legal Name\",";
            retstring = retstring + " crd11.\"City\" \"BCity\",st1.\"Name\" \"BState\" ,cy1.\"Name\" \"BCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd11.\"ZipCode\",' ','') END as  \"BZipCode\",";

            retstring = retstring + " crd1.\"City\" \"SCity\",st.\"Name\" \"SState\", cy.\"Name\" \"SCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd1.\"ZipCode\",' ','') END as \"SZipCode\",";

            retstring = retstring += " ss.\"Building\" \"Seller_Building\",ss.\"Block\" \"Seller_Block\",ss.\"Street\" \"Seller_Street\",";
            retstring = retstring += " crd11.\"Building\" \"Buyer_Building\",crd11.\"Block\" \"Buyer_Block\",crd11.\"StreetNo\" \"Buyer_StreetNo\",crd11.\"Street\" \"Buyer_Street\",crd11.\"Address2\" \"Buyer_Address2\",crd11.\"Address3\" \"Buyer_Address3\",";
            retstring = retstring += " crd1.\"Building\" \"Ship_Building\",crd1.\"Block\" \"Ship_Block\",crd1.\"StreetNo\" \"Ship_StreetNo\",crd1.\"Street\" \"Ship_Street\",crd1.\"Address2\" \"Ship_Address2\",crd1.\"Address3\" \"Ship_Address3\",";


            retstring = retstring += " T.\"BuildingB\" \"T_Buyer_Building\",T.\"BlockB\" \"T_Buyer_Block\",T.\"StreetNoB\" \"T_Buyer_StreetNo\",T.\"StreetB\" \"T_Buyer_Street\",T.\"Address2B\" \"T_Buyer_Address2\",T.\"Address3B\" \"T_Buyer_Address3\",";
            retstring = retstring += " T.\"CityB\" \"T_BCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeB\",' ','') END as  \"T_BZipCode\", ";
            retstring = retstring += " (case when T.\"CountryB\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateB\" and \"Country\"=T.\"CountryB\") ElSE '96' END) \"T_Bill to State Code\", ";

            retstring = retstring += " T.\"BuildingS\" \"T_Ship_Building\",T.\"BlockS\" \"T_Ship_Block\",T.\"StreetNoS\" \"T_Ship_StreetNo\",T.\"StreetS\" \"T_Ship_Street\",T.\"Address2S\" \"T_Ship_Address2\",T.\"Address3S\" \"T_Ship_Address3\",";
            retstring = retstring += " T.\"CityS\" \"T_SCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeS\",' ','') END as  \"T_SZipCode\", ";
            retstring = retstring += " (case when T.\"CountryS\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateS\" and \"Country\"=T.\"CountryS\") ELSE '96' END) \"T_Shipp to State Code\", ";


            retstring = retstring + " (case when Crd11.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=Crd11.\"State\" and \"Country\"=Crd11.\"Country\") ELSE '96' END) \"Bill to State Code\",";
            retstring = retstring + " (case when crd1.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=crd1.\"State\" and \"Country\"=crd1.\"Country\") ELSE '96' END ) \"Shipp to State Code\",";
            retstring = retstring + " b.\"ItemCode\", b.\"Dscription\",Case when a.\"DocType\"='S' then 'Y' Else 'N' End \"IsServc\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then (Select Case when LEft(\"ServCode\",2) like '0%' then Replace(\"ServCode\",'0','') Else \"ServCode\" End from OSAC where b.\"SacEntry\"= \"AbsEntry\") Else Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") End \"HSN\",Case when a.\"DocType\"='S' then 1 Else b.\"Quantity\" End \"Quantity\",COALESCE(b.\"unitMsr\",b.\"UomCode\") \"Unit\",";

            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"Price\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end  else b.\"PriceBefDi\" * a.\"DocRate\" end \"UnitPrice\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"PriceBefDi\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt1\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"AssAmt\",";

            retstring = retstring + " (Select Sum(X.\"TaxRate\") From DPI4 X Where X.\"DocEntry\"=A.\"DocEntry\" And  X.\"LineNum\"=B.\"LineNum\" and X.\"ExpnsCode\"='-1' ) \"GSTRATE\",IFNULL((select sum(\"TaxSum\") from DPI4 where \"DocEntry\"=a.\"DocEntry\"and \"staType\"=-100),0) as \"CGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from DPI4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-110),0) as \"SGSTVal\", IFNULL((select sum(\"TaxSum\") from DPI4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-120),0) as \"IGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from DPI4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-100 and \"ExpnsCode\"='-1'),0) as \"CGSTAmt\", IFNULL((select sum(\"TaxSum\") from DPI4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-110 and \"ExpnsCode\"='-1'),0) as \"SGSTAmt\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from DPI4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-120 and \"ExpnsCode\"='-1'),0) as \"IGSTAmt\",";
            retstring = retstring + " (SELECT MAX(\"BatchNum\") from IBT1 where \"ItemCode\"=b.\"ItemCode\" and \"WhsCode\"=b.\"WhsCode\" and";
            retstring = retstring + " \"BaseType\" in ('203') and \"BaseEntry\"= CASE WHEN  b.\"BaseType\" =-1 THEN b.\"DocEntry\" ELSE b.\"BaseEntry\"  end  ) AS \"BatchNum\",";

            // retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from DPI1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

            retstring = retstring + " Case when a.\"DocType\"='S' then  (Select Sum(IFNULL(\"LineTotal\",0))* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from DPI1 where \"DocEntry\"=b.\"DocEntry\") " +
                                               "Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from DPI1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

            retstring = retstring + " (a.\"DocTotal\" +a.\"DpmAmnt\") - ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN  0 ELSE COALESCE((Select Sum(DPI5.\"WTAmnt\") from DPI5 where DPI5.\"AbsEntry\" = a.\"DocEntry\"),0.0) END ) AS \"Doc Total\", ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN   COALESCE((Select Sum(DPI5.\"WTAmnt\") from DPI5 where DPI5.\"AbsEntry\" = a.\"DocEntry\"),0.0) ELSE 0 END ) AS \"OthrAmt\", ";


            retstring = retstring + " a.\"DocDueDate\" \"Inv Due Date\", a.\"NumAtCard\", a.\"Printed\",a.\"PayToCode\", a.\"ShipToCode\", a.\"Comments\" ,Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") \"ChapterID\" , A.\"DiscSum\", A.\"RoundDif\",";
            retstring = retstring + " b.\"DiscPrcnt\",((b.\"PriceBefDi\"*\"Quantity\") * (b.\"DiscPrcnt\"/100)) \"LineDisc\",l.\"InvntryUom\" ,IFNULL(a.\"RoundDif\",0) \"Rounding\", a.\"TaxDate\" \"Cust Order Date\", a.\"TotalExpns\",l.\"FrgnName\",a.\"DocCur\",A.\"VatSum\", a.\"TotalExpns\" \"Freight\",";
            retstring = retstring + " l.\"SalUnitMsr\",IFNULL(b.\"LineTotal\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Line Total\", IFNULL(a.\"TotalExpns\",0) \"Freight Total\",IFNULL(a.\"DiscSum\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Disc Total\",l.\"ItemClass\", ";

            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " i.\"TransID\" ,i.\"TransName\" ,i.\"TransDocNo\" ,i.\"Distance\" ,i.\"TransMode\" ,i.\"VehicleNo\" ,i.\"VehicleTyp\",";
                retstring = retstring + " i.\"TransType\",i.\"TransDate\",i.\"SuplyType\",";
                retstring = retstring + " i.\"FrmGSTN\" ,i.\"FrmTraName\" ,i.\"FrmAddres1\" ,i.\"FrmAddres2\" ,i.\"FrmPlace\" ,Replace(i.\"FrmZipCode\",' ','') as \"FrmZipCode\" ,";
                retstring = retstring + " i.\"ActFrmStat\" ,i.\"ToGSTN\" ,i.\"ToTraName\" ,i.\"ToAddres1\" ,i.\"ToAddres2\" ,i.\"ToPlace\" ,Replace(i.\"ToZipCode\",' ','') as \"ToZipCode\"  ,";
                retstring = retstring + " i.\"ActToState\",i.\"SubSplyTyp\",ES.\"SubType\" \"SubtypeDescription\", i.\"DocType\" \"EDocType\" ,";

                retstring += " i.\"U_Dispatch_Eway\" as \"DisEway\" ,";
                retstring += " i.\"U_Dispatch_Name\" as \"DisName\" ,";
                retstring = retstring + " i.\"EwbDate\" \"EwbDate\" ,i.\"EWayBillNo\" \"EwayNo\" ,";
                retstring = retstring + " i.\"FrmState\" \"FrmState\" ,i.\"ToState\" \"ToState\" ,";

                retstring = retstring + " T.\"ClaimRefun\",";
            }
            else
            {
                if (!string.IsNullOrEmpty(clsModule.EwayUDF))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayUDF + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportName))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportName + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportId))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportId + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayDistance))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayDistance + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayNo))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayNo + @""" ""EwayNo"",";
                    retstring = retstring + " '' as  \"EwbDate\",";
                }
            }

            retstring = retstring + " (Select \"ServCode\" from OSAC where b.\"SacEntry\"= \"AbsEntry\")\"SacCode\" ";


          retstring = retstring + ",a.\"U_IRNNo\" ,a.\"U_AckDate\" ";

            retstring = retstring + " FROM ODPI a INNER JOIN DPI1 b on b.\"DocEntry\" = a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN DPI26 i ON i.\"DocEntry\" =a.\"DocEntry\"";
            }

            retstring = retstring + " left JOIN OCRD g on g.\"CardCode\" = a.\"CardCode\"";
            retstring = retstring + " left JOIN OITM l on l.\"ItemCode\" = b.\"ItemCode\"";
            retstring = retstring + " left JOIN OCRD m on m.\"CardCode\" = a.\"CardCode\" ";
            retstring = retstring + " LEFT JOIN OCPR n on n.\"CardCode\" = a.\"CardCode\" and n.\"CntctCode\" = a.\"CntctCode\"";
            retstring = retstring + " LEFT JOIN OCHP o on o.\"AbsEntry\" = l.\"ChapterID\" ";
            retstring = retstring + " LEFT JOIN OLCT ss on ss.\"Code\" = b.\"LocCode\"";
            retstring = retstring + " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\" =a.\"CardCode\" and CRD1.\"Address\" =A.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
            retstring = retstring + " LEFT JOIN OCST st on st.\"Code\"=CRD1.\"State\" and st.\"Country\"=CRD1.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy on cy.\"Code\" =CRD1.\"Country\"";
            retstring = retstring + " LEFT JOIN CRD1 crd11 on crd11.\"CardCode\" =a.\"CardCode\" and crd11.\"Address\" =A.\"PayToCode\" and crd11.\"AdresType\"='B'";
            retstring = retstring + " LEFT JOIN OCST st1 on st1.\"Code\"=crd11.\"State\" and st1.\"Country\"=crd11.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy1 on cy1.\"Code\" =crd11.\"Country\"";
            retstring = retstring + " CROSS JOIN OADM B1";
            retstring = retstring + " CROSS JOIN ADM1 B3";
            retstring = retstring + " LEFT JOIN DPI12 T ON T.\"DocEntry\"=a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN OEST ES ON ES.\"SubID\" =i.\"SubSplyTyp\"";
            }
            retstring = retstring + " LEFT JOIN NNM1 nnm1 ON a.\"Series\" =nnm1.\"Series\"";

            retstring = retstring + " LEFT JOIN(SELECT \"BankName\" \"CBankName\",Y.\"BankCode\" \"CBankCode\",\"Branch\" \"CBranch\", \"Account\" \"CAccount\",\"AcctName\" \"CAcctName\",";
            retstring = retstring + " X.\"SwiftNum\" \"CIFSCNo\" FROM DSC1 X,ODSC Y Where X.\"AbsEntry\"=Y.\"AbsEntry\" ) B2 On B2.\"CBankCode\"=B1.\"DflBnkCode\"";
            retstring = retstring + " WHERE a.\"DocEntry\"=" + Docentry + ")B";
            clsModule.objaddon.objglobalmethods.WriteErrorLog(retstring);
            return retstring;


        }

        public string PurchaseDownQuery(string Docentry)
        {
            string retstring = "";

            retstring = "SELECT Row_number() Over(Partition by b.\"DocEntry\" order by b.\"DocEntry\" Asc )\" SINo\",(B.\"AssVal\"+B.\"Freight Total\") \"AssValN\" ,";
            retstring = retstring + " (B.\"AssAmt\"+B.\"CGSTAmt\"+B.\"SGSTAmt\"+B.\"IGSTAmt\") \"Total Item Value\",CAST((B.\"Tot Amt\"-B.\"Tot Amt1\")AS nvarchar(200)) \"LineDiscountAmt\",B.* FROM(";
            retstring = retstring + "  Select B2.*, 'GST' as \"TaxSch\",T.\"BpGSTN\" as \"T_BpGSTN\",";

            retstring = retstring += docseries + " \"Inv_No\",";

            if (!string.IsNullOrEmpty(clsModule.ItemDsc))
            {
                retstring = retstring + "b.\"" + clsModule.ItemDsc + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.GSTCol))
            {
                retstring = retstring + "T.\"" + clsModule.GSTCol + @""",";
            }
            if (!string.IsNullOrEmpty(clsModule.HSNCol))
            {
                retstring = retstring + "b.\"" + clsModule.HSNCol + @""",";
            }

            retstring = retstring += " CASE WHEN  T.\"ExportType\" in('U','S') THEN case when a.\"DutyStatus\" ='Y' then 'SEZWP' else 'SEZWOP' End ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN  case when a.\"DutyStatus\" ='Y' then 'EXPWP' else 'EXPWOP' End  ";
            retstring = retstring += "      WHEN  T.\"ExportType\" = 'D' THEN 'DEXP' ";
            retstring = retstring += "      ELSE 'B2B' END as \"SupTyp\",";

            retstring = retstring + " '' as \"RegRev\",'INV' Type,a.\"DocEntry\",a.\"DocType\",a.\"DocDate\" \"Inv_Doc_Date\",ss.\"GSTRegnNo\" \"Seller GSTN\",";

            retstring = retstring + " B1.\"CompnyName\" \"Seller_Legal Name\",ss.\"City\" \"Seller Location Name\", Replace(ss.\"ZipCode\",' ','') \"Seller_PIN code\", ";
            retstring = retstring + " (CASE WHEN ss.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=ss.\"Country\" and \"Code\"=ss.\"State\") ";
            retstring = retstring + " ELSE '96' END) \"Seller_State_code\",";


            retstring = retstring + " B3.\"GlblLocNum\" as \"CompnyGSTIN\", ";
            retstring = retstring + " B1.\"CompnyAddr\",Replace(B3.\"ZipCode\",' ','') \"CompnyPincode\",B3.\"City\" \"CompnyCity\", ";

            retstring = retstring + " (CASE WHEN B3.\"Country\"='IN' then ( select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=B3.\"Country\" and \"Code\"=B3.\"State\") ";
            retstring = retstring + " ELSE '96' END ) \"Compnystatecode\", ";


            retstring = retstring += " CASE WHEN COALESCE(Crd11.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd11.\"GSTRegnNo\" END as \"Buyer GSTN\",";
            retstring = retstring += " CASE WHEN COALESCE(Crd1.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd1.\"GSTRegnNo\" END as \"Shipto GSTN\",";

            retstring = retstring += " T.\"PortCode\",T.\"ImpExpNo\" ,T.\"ImpExpDate\" ,COALESCE(st.\"Country\",cy.\"Code\") \"CCode\",  ";

            retstring = retstring += " a.\"BPLId\" ,"; 

            retstring = retstring + " a.\"CardCode\", a.\"CardName\" \"Buyer_Legal Name\",";
            retstring = retstring + " crd11.\"City\" \"BCity\",st1.\"Name\" \"BState\" ,cy1.\"Name\" \"BCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd11.\"ZipCode\",' ','') END as  \"BZipCode\",";

            retstring = retstring + " crd1.\"City\" \"SCity\",st.\"Name\" \"SState\", cy.\"Name\" \"SCountry\",";

            retstring = retstring += " CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(crd1.\"ZipCode\",' ','') END as \"SZipCode\",";

            retstring = retstring += " ss.\"Building\" \"Seller_Building\",ss.\"Block\" \"Seller_Block\",ss.\"Street\" \"Seller_Street\",";
            retstring = retstring += " crd11.\"Building\" \"Buyer_Building\",crd11.\"Block\" \"Buyer_Block\",crd11.\"StreetNo\" \"Buyer_StreetNo\",crd11.\"Street\" \"Buyer_Street\",crd11.\"Address2\" \"Buyer_Address2\",crd11.\"Address3\" \"Buyer_Address3\",";
            retstring = retstring += " crd1.\"Building\" \"Ship_Building\",crd1.\"Block\" \"Ship_Block\",crd1.\"StreetNo\" \"Ship_StreetNo\",crd1.\"Street\" \"Ship_Street\",crd1.\"Address2\" \"Ship_Address2\",crd1.\"Address3\" \"Ship_Address3\",";

            retstring = retstring += " T.\"BuildingB\" \"T_Buyer_Building\",T.\"BlockB\" \"T_Buyer_Block\",T.\"StreetNoB\" \"T_Buyer_StreetNo\",T.\"StreetB\" \"T_Buyer_Street\",T.\"Address2B\" \"T_Buyer_Address2\",T.\"Address3B\" \"T_Buyer_Address3\",";
            retstring = retstring += " T.\"CityB\" \"T_BCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeB\",' ','') END as  \"T_BZipCode\", ";
            retstring = retstring += " (case when T.\"CountryB\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateB\" and \"Country\"=T.\"CountryB\") ElSE '96' END) \"T_Bill to State Code\", ";

            retstring = retstring += " T.\"BuildingS\" \"T_Ship_Building\",T.\"BlockS\" \"T_Ship_Block\",T.\"StreetNoS\" \"T_Ship_StreetNo\",T.\"StreetS\" \"T_Ship_Street\",T.\"Address2S\" \"T_Ship_Address2\",T.\"Address3S\" \"T_Ship_Address3\",";
            retstring = retstring += " T.\"CityS\" \"T_SCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeS\",' ','') END as  \"T_SZipCode\", ";
            retstring = retstring += " (case when T.\"CountryS\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateS\" and \"Country\"=T.\"CountryS\") ELSE '96' END) \"T_Shipp to State Code\", ";


            retstring = retstring + " (case when Crd11.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=Crd11.\"State\" and \"Country\"=Crd11.\"Country\") ELSE '96' END) \"Bill to State Code\",";
            retstring = retstring + " (case when crd1.\"Country\"='IN' then (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=crd1.\"State\" and \"Country\"=crd1.\"Country\") ELSE '96' END ) \"Shipp to State Code\",";
            retstring = retstring + " b.\"ItemCode\", b.\"Dscription\",Case when a.\"DocType\"='S' then 'Y' Else 'N' End \"IsServc\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then (Select Case when LEft(\"ServCode\",2) like '0%' then Replace(\"ServCode\",'0','') Else \"ServCode\" End from OSAC where b.\"SacEntry\"= \"AbsEntry\") Else Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") End \"HSN\",Case when a.\"DocType\"='S' then 1 Else b.\"Quantity\" End \"Quantity\",COALESCE(b.\"unitMsr\",b.\"UomCode\") \"Unit\",";


            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"Price\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end  else b.\"PriceBefDi\" * a.\"DocRate\" end \"UnitPrice\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"PriceBefDi\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"Tot Amt1\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else IFNULL(b.\"INMPrice\" * a.\"DocRate\",0)*b.\"Quantity\" End \"AssAmt\",";

            retstring = retstring + " (Select Sum(X.\"TaxRate\") From DPO4 X Where X.\"DocEntry\"=A.\"DocEntry\" And  X.\"LineNum\"=B.\"LineNum\" and X.\"ExpnsCode\"='-1' ) \"GSTRATE\",IFNULL((select sum(\"TaxSum\") from DPO4 where \"DocEntry\"=a.\"DocEntry\"and \"staType\"=-100),0) as \"CGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from DPO4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-110),0) as \"SGSTVal\", IFNULL((select sum(\"TaxSum\") from DPO4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-120),0) as \"IGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from DPO4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-100 and \"ExpnsCode\"='-1'),0) as \"CGSTAmt\", IFNULL((select sum(\"TaxSum\") from DPO4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-110 and \"ExpnsCode\"='-1'),0) as \"SGSTAmt\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from DPO4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-120 and \"ExpnsCode\"='-1'),0) as \"IGSTAmt\",";
            retstring = retstring + " (SELECT MAX(\"BatchNum\") from IBT1 where \"ItemCode\"=b.\"ItemCode\" and \"WhsCode\"=b.\"WhsCode\" and";
            retstring = retstring + " \"BaseType\" in ('204') and \"BaseEntry\"= CASE WHEN  b.\"BaseType\" =-1 THEN b.\"DocEntry\" ELSE b.\"BaseEntry\"  end  ) AS \"BatchNum\",";

            //  retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" * case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from DPO1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then  (Select Sum(IFNULL(\"LineTotal\",0))* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from DPO1 where \"DocEntry\"=b.\"DocEntry\") " +
                                     "Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\")* case when a.\"DocRate\"=0 then 1 else a.\"DocRate\" end from DPO1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",";

        

            retstring = retstring + " (a.\"DocTotal\" +a.\"DpmAmnt\") - ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN  0 ELSE COALESCE((Select Sum(DPO5.\"WTAmnt\") from DPO5 where DPO5.\"AbsEntry\" = a.\"DocEntry\"),0.0) END ) AS \"Doc Total\", ";
            retstring += " (CASE WHEN (SELECT \"U_AddTCSOth\"  FROM \"@ATEICFG\" e WHERE e.\"Code\" ='01')='Y' ";
            retstring += " THEN   COALESCE((Select Sum(DPO5.\"WTAmnt\") from DPO5 where DPO5.\"AbsEntry\" = a.\"DocEntry\"),0.0) ELSE 0 END ) AS \"OthrAmt\", ";


            retstring = retstring + " a.\"DocDueDate\" \"Inv Due Date\", a.\"NumAtCard\", a.\"Printed\",a.\"PayToCode\", a.\"ShipToCode\", a.\"Comments\" ,Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") \"ChapterID\" , A.\"DiscSum\", A.\"RoundDif\",";
            retstring = retstring + " b.\"DiscPrcnt\",((b.\"PriceBefDi\"*\"Quantity\") * (b.\"DiscPrcnt\"/100)) \"LineDisc\",l.\"InvntryUom\" ,IFNULL(a.\"RoundDif\",0) \"Rounding\", a.\"TaxDate\" \"Cust Order Date\", a.\"TotalExpns\",l.\"FrgnName\",a.\"DocCur\",A.\"VatSum\", a.\"TotalExpns\" \"Freight\",";
            retstring = retstring + " l.\"SalUnitMsr\",IFNULL(b.\"LineTotal\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Line Total\", IFNULL(a.\"TotalExpns\",0) \"Freight Total\",IFNULL(a.\"DiscSum\"/case when a.\"DocRate\" =0 then 1 else a.\"DocRate\" End,0) \"Disc Total\",l.\"ItemClass\", ";

            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " i.\"TransID\" ,i.\"TransName\" ,i.\"TransDocNo\" ,i.\"Distance\" ,i.\"TransMode\" ,i.\"VehicleNo\" ,i.\"VehicleTyp\",";
                retstring = retstring + " i.\"TransType\",i.\"TransDate\",i.\"SuplyType\",";
                retstring = retstring + " i.\"FrmGSTN\" ,i.\"FrmTraName\" ,i.\"FrmAddres1\" ,i.\"FrmAddres2\" ,i.\"FrmPlace\" ,Replace(i.\"FrmZipCode\",' ','') as \"FrmZipCode\" ,";
                retstring = retstring + " i.\"ActFrmStat\" ,i.\"ToGSTN\" ,i.\"ToTraName\" ,i.\"ToAddres1\" ,i.\"ToAddres2\" ,i.\"ToPlace\" ,Replace(i.\"ToZipCode\",' ','') as \"ToZipCode\"  ,";
                retstring = retstring + " i.\"ActToState\",i.\"SubSplyTyp\",ES.\"SubType\" \"SubtypeDescription\", i.\"DocType\" \"EDocType\" ,";

                retstring += " i.\"U_Dispatch_Eway\" as \"DisEway\" ,";
                retstring += " i.\"U_Dispatch_Name\" as \"DisName\" ,";
                retstring = retstring + " i.\"EwbDate\" \"EwbDate\" ,i.\"EWayBillNo\" \"EwayNo\" ,";
                retstring = retstring + " i.\"FrmState\" \"FrmState\" ,i.\"ToState\" \"ToState\" ,";

                retstring = retstring + " T.\"ClaimRefun\",";
            }
            else
            {
                if (!string.IsNullOrEmpty(clsModule.EwayUDF))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayUDF + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportName))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportName + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayTransportId))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayTransportId + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayDistance))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayDistance + @""",";
                }
                if (!string.IsNullOrEmpty(clsModule.EwayNo))
                {
                    retstring = retstring + "a.\"" + clsModule.EwayNo + @""" ""EwayNo"",";
                    retstring = retstring + " '' as  \"EwbDate\",";
                }

            }

            retstring = retstring + " (Select \"ServCode\" from OSAC where b.\"SacEntry\"= \"AbsEntry\")\"SacCode\" ";


          retstring = retstring + ",a.\"U_IRNNo\" ,a.\"U_AckDate\" ";

            retstring = retstring + " FROM ODPO a INNER JOIN DPO1 b on b.\"DocEntry\" = a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN DPO26 i ON i.\"DocEntry\" =a.\"DocEntry\"";
            }

            retstring = retstring + " left JOIN OCRD g on g.\"CardCode\" = a.\"CardCode\"";
            retstring = retstring + " left JOIN OITM l on l.\"ItemCode\" = b.\"ItemCode\"";
            retstring = retstring + " left JOIN OCRD m on m.\"CardCode\" = a.\"CardCode\" ";
            retstring = retstring + " LEFT JOIN OCPR n on n.\"CardCode\" = a.\"CardCode\" and n.\"CntctCode\" = a.\"CntctCode\"";
            retstring = retstring + " LEFT JOIN OCHP o on o.\"AbsEntry\" = l.\"ChapterID\" ";
            retstring = retstring + " LEFT JOIN OLCT ss on ss.\"Code\" = b.\"LocCode\"";
            retstring = retstring + " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\" =a.\"CardCode\" and CRD1.\"Address\" =A.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
            retstring = retstring + " LEFT JOIN OCST st on st.\"Code\"=CRD1.\"State\" and st.\"Country\"=CRD1.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy on cy.\"Code\" =CRD1.\"Country\"";
            retstring = retstring + " LEFT JOIN CRD1 crd11 on crd11.\"CardCode\" =a.\"CardCode\" and crd11.\"Address\" =A.\"PayToCode\" and crd11.\"AdresType\"='B'";
            retstring = retstring + " LEFT JOIN OCST st1 on st1.\"Code\"=crd11.\"State\" and st1.\"Country\"=crd11.\"Country\" ";
            retstring = retstring + " LEFT JOIN OCRY cy1 on cy1.\"Code\" =crd11.\"Country\"";
            retstring = retstring + " CROSS JOIN OADM B1";
            retstring = retstring + " CROSS JOIN ADM1 B3";
            retstring = retstring + " LEFT JOIN DPO12 T ON T.\"DocEntry\"=a.\"DocEntry\"";
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                retstring = retstring + " LEFT JOIN OEST ES ON ES.\"SubID\" =i.\"SubSplyTyp\"";
            }
            retstring = retstring + " LEFT JOIN NNM1 nnm1 ON a.\"Series\" =nnm1.\"Series\"";

            retstring = retstring + " LEFT JOIN(SELECT \"BankName\" \"CBankName\",Y.\"BankCode\" \"CBankCode\",\"Branch\" \"CBranch\", \"Account\" \"CAccount\",\"AcctName\" \"CAcctName\",";
            retstring = retstring + " X.\"SwiftNum\" \"CIFSCNo\" FROM DSC1 X,ODSC Y Where X.\"AbsEntry\"=Y.\"AbsEntry\" ) B2 On B2.\"CBankCode\"=B1.\"DflBnkCode\"";
            retstring = retstring + " WHERE a.\"DocEntry\"=" + Docentry + ")B";
            clsModule.objaddon.objglobalmethods.WriteErrorLog(retstring);
            return retstring;


        }

    }
}
