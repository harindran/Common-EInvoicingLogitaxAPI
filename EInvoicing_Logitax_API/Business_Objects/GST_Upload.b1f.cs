using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EInvoicing_Logitax_API.Common;
using Newtonsoft.Json;
using SAPbouiCOM.Framework;

using static EInvoicing_Logitax_API.Common.clsGlobalMethods;

namespace EInvoicing_Logitax_API.Business_Objects
{
    [FormAttribute("GSTUPLOAD", "Business_Objects/GST_Upload.b1f")]
    class GST_Upload : UserFormBase
    {
        private string strSQL;
        public static SAPbouiCOM.Form oForm;
        SAPbouiCOM.ProgressBar oProgBar;

        public GST_Upload()
        {
        }
        private SAPbobsCOM.Recordset objRs;

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.ComboBox0 = ((SAPbouiCOM.ComboBox)(this.GetItem("DcType").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_4").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("Item_5").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_6").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("Item_7").Specific));
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_8").Specific));
            this.StaticText5 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_10").Specific));
            this.ComboBox2 = ((SAPbouiCOM.ComboBox)(this.GetItem("Item_11").Specific));
            this.ComboBox2.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.ComboBox2_ComboSelectAfter);
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_12").Specific));
            this.Button0.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.Button0_ClickAfter);
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);
            this.ComboBox1 = ((SAPbouiCOM.ComboBox)(this.GetItem("GSTIN").Specific));
            this.ComboBox1.ClickBefore += new SAPbouiCOM._IComboBoxEvents_ClickBeforeEventHandler(this.ComboBox1_ClickBefore);
            this.ComboBox1.ComboSelectBefore += new SAPbouiCOM._IComboBoxEvents_ComboSelectBeforeEventHandler(this.ComboBox1_ComboSelectBefore);
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("Item_2").Specific));
            this.Grid0.DoubleClickBefore += new SAPbouiCOM._IGridEvents_DoubleClickBeforeEventHandler(this.Grid0_DoubleClickBefore);
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("Item_3").Specific));
            this.Button1.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button1_ClickBefore);
            this.Button2 = ((SAPbouiCOM.Button)(this.GetItem("Item_0").Specific));
            this.Button2.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button2_ClickBefore);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);

        }

        private void OnCustomInitialize()
        {
            objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            strSQL = "SELECT Distinct \"GSTRegnNo\",\"GSTRegnNo\"  FROM OLCT o  ;";

            clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, this.ComboBox1, strSQL);


        }

        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.ComboBox ComboBox0;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.StaticText StaticText5;
        private SAPbouiCOM.ComboBox ComboBox2;
        private SAPbouiCOM.Button Button0;

        private void Button0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            bool checkvalue = false; ;
            Grid0.Columns.Item("Checkbox").Visible = true;

          
            for (int i = 0; i < Grid0.Rows.Count; i++)
            {
                
                string ss2 = Grid0.DataTable.Columns.Item("Checkbox").Cells.Item(i).Value.ToString();
                if (ss2 == "Y")
                {
                    checkvalue = true;
                    break;
                }
               
            }
            if (!checkvalue)
            {
                Application.SBO_Application.SetStatusBarMessage("Please Select Checkbox !!!!", SAPbouiCOM.BoMessageTime.bmt_Short, true);
                BubbleEvent = false;
            }

        }

        static string BuildFormData(NameValueCollection formData,string boundary)
        {
            StringBuilder sb = new StringBuilder();
         

            foreach (string key in formData.AllKeys)
            {

                sb.AppendLine("--" + boundary);
                sb.AppendLine("Content-Disposition: form-data; name="+key+"");
                sb.AppendLine();
                sb.AppendLine(formData[key]);                             
            }
            sb.AppendLine("--" + boundary + "--");
            return sb.ToString();
        }

        private DataTable Get_API_Response(string JSON, string URL, string httpMethod = "POST", string contenttype = "application/json",
            Dictionary<string, string> headers = null, NameValueCollection formdata1 =null)
        {
            DataTable datatable = new DataTable();
            var content = new MultipartFormDataContent();

            try
            {
                clsModule.objaddon.objglobalmethods.WriteErrorLog(URL);
                clsModule.objaddon.objglobalmethods.WriteErrorLog(JSON);


                HttpWebRequest webRequest;
                webRequest = (HttpWebRequest)WebRequest.Create(URL);
                webRequest.Method = httpMethod;

                byte[] byteArray = Encoding.UTF8.GetBytes(JSON);


                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        webRequest.Headers.Add(item.Key, item.Value);
                    }
                }

                if (formdata1 != null)
                {
                    string boundary = "----" + Guid.NewGuid().ToString("N");

                    string formDataString = BuildFormData(formdata1, boundary);
                    

                    byte[] formDataBytes = Encoding.UTF8.GetBytes(formDataString);
                    webRequest.ContentType = contenttype+"; boundary=" + boundary;
                    
                    webRequest.ContentLength = formDataBytes.Length;
                    using (Stream requestStream = webRequest.GetRequestStream())
                    {
                        requestStream.Write(formDataBytes, 0, formDataBytes.Length);
                    }

                }


                else
                {
                    webRequest.ContentType = contenttype;
                    using (Stream requestStream = webRequest.GetRequestStream())
                    {
                        requestStream.Write(byteArray, 0, byteArray.Length);
                    }
                }

                try
                {
                    using (WebResponse response = webRequest.GetResponse())
                    {

                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                            string Json = rdr.ReadToEnd();
                            clsModule.objaddon.objglobalmethods.WriteErrorLog(Json);
                            datatable = clsModule.objaddon.objglobalmethods.Jsontodt(Json);
                        }
                    }
                }               
                catch (WebException ex)
                {

                    if (ex.Response is HttpWebResponse httpWebResponse)
                    {
                        using (Stream errorResponseStream = httpWebResponse.GetResponseStream())
                        {
                            using (StreamReader errorReader = new StreamReader(errorResponseStream, Encoding.UTF8))
                            {
                                string Json = errorReader.ReadToEnd();
                                datatable = clsModule.objaddon.objglobalmethods.Jsontodt(Json);
                            }
                        }
                    }                                                                      
                }                
                return datatable;
            }
            catch (Exception ex)
            {
                return datatable;
                //  throw ex;
            }
        }


        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            oForm = clsModule.objaddon.objapplication.Forms.GetForm("GSTUPLOAD", 0);


        }

        private SAPbouiCOM.ComboBox ComboBox1;

        private void ComboBox1_ComboSelectBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

        }

        private void ComboBox1_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;


        }

        private SAPbouiCOM.Grid Grid0;

        private void ComboBox2_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            

        }

        private string Loadquery(string filter="")
        {
            string formtype = "";
            string TransType = "";
            string table = "";
            string subtable = "";
            SAPbouiCOM.EditTextColumn oColumns;
            switch (ComboBox0.Selected.Value)
            {
                case "A/R Invoice":
                    formtype = "SalesRegister";
                    TransType = "INV";
                    table = "OINV";
                    subtable = "INV";
                    oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "13";
                    break;
                case "A/R Credit Memo":
                    formtype = "SalesRegister";
                    TransType = "CRN";
                    table = "ORIN";
                    subtable = "RIN";
                    oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "14";
                    break;
                case "A/P Invoice":
                    formtype = "PurchaseRegister";
                    TransType = "PCH";
                    table = "OPCH";
                    subtable = "PCH";
                    oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "18";
                    break;
                case "A/P Credit Memo":
                    formtype = "PurchaseRegister";
                    TransType = "RPC";
                    table = "ORPC";
                    subtable = "RPC";
                    oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "19";
                    break;
                case "A/R Invoice Down":
                    formtype = "SalesRegister";
                    TransType = "DPI";
                    table = "ODPI";
                    subtable = "DPI";
                    oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "203";
                    break;
                case "A/P Invoice Down":
                    formtype = "PurchaseRegister";
                    TransType = "DPO";
                    table = "ODPO";
                    subtable = "DPO";
                    oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "204";
                    break;
            }
            string fromPeriodCode = clsModule.objaddon.objglobalmethods.DateFormat(EditText1.Value.ToString(), "yyyyMMdd", "MMyyyy");
            string toPeriodCode = clsModule.objaddon.objglobalmethods.DateFormat(EditText2.Value.ToString(), "yyyyMMdd", "MMyyyy");
            string gstin = ComboBox1.Selected.Value;

            string Query;
            Query = "SELECT  a.\"DocEntry\",a.\"DocNum\",a.\"DocDate\",a.\"CardName\" ,a.\"CardCode\",a.\"DocTotal\",a.\"U_GST_RefNo\",a.\"U_GST_Remarks\",a.\"U_GST_status\" FROM " + table + " a  " +
                "  LEFT JOIN OLCT o  ON o.\"Code\" = (select Max(b.\"LocCode\") from "+ subtable + "1 b where b.\"DocEntry\" =a.\"DocEntry\") " +
                " WHERE a.\"DocDate\" BETWEEN '" + EditText1.Value.ToString() + "'";
            Query += "  AND '" + EditText2.Value.ToString() + "'";

            if (!(gstin == "-"))
            {
                Query += " and  o.\"GSTRegnNo\"='" + gstin + "'";
            }
            switch (ComboBox2.Selected.Value)
            {
                case "IRN Generate":
                    Query += " and  COALESCE(a.\"U_IRNNo\",'') <>'' ";

                    Query += " and a.\"CANCELED\" ='N' ";
                    break;
                case "Without IRN":
                    Query += " and COALESCE(a.\"U_IRNNo\",'') ='' ";

                    Query += " and a.\"CANCELED\" ='N' ";
                    break;
                case "Cancel Bill":
                    Query += " and a.\"CANCELED\" ='Y' ";
                    break;
                case "Not Upload":
                    Query += " and COALESCE(a.\"U_GST_status\",0) =0 ";

                    Query += " and a.\"CANCELED\" ='N' ";
                    break;
                case "ALL":
                    Query += " and a.\"CANCELED\" ='N' ";
                    break;
            }
            if (filter!="")
            {
                Query += filter;
            }
            Query += " order by a.\"DocEntry\"";
            return Query;

        }

        private SAPbouiCOM.Button Button1;

        private void Button1_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {

                Grid0.Columns.Item("Checkbox").Visible = true;
                DataTable dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(Loadquery());
                Grid0.DataTable.Rows.Clear();
                
                oProgBar = clsModule.objaddon.objapplication.StatusBar.CreateProgressBar("Loading Please Wait", dt.Rows.Count, true);
                
                oForm.Freeze(true);
                if (dt.Rows.Count > 0)
                {
                    int i = 0;
                    foreach (DataRow Drow in dt.Rows)
                    {
                        Grid0.DataTable.Rows.Add();
                        this.Grid0.RowHeaders.SetText(i, (i + 1).ToString());
                        Grid0.DataTable.SetValue("Checkbox", i, "N");
                        Grid0.DataTable.SetValue("Doc Number", i, Drow["DocNum"]);
                        Grid0.DataTable.SetValue("DocEntry", i, Drow["DocEntry"]);
                        Grid0.DataTable.SetValue("Doc Date", i, clsModule.objaddon.objglobalmethods.Getdateformat(Drow["DocDate"].ToString()));
                        Grid0.DataTable.SetValue("Customer", i, Drow["CardName"]);
                        Grid0.DataTable.SetValue("Total", i, Drow["DocTotal"]);
                        Grid0.DataTable.SetValue("Status", i, Drow["U_GST_status"]);
                        Grid0.DataTable.SetValue("Remarks", i, Drow["U_GST_Remarks"]);
                        this.GetItem("Item_2").LinkTo = "DocEntry";
                       
                        i++;
                        oProgBar.Value += 1;
                    }
                    Grid0.AutoResizeColumns();                   
                }
                
            }
            catch (Exception)
            {

               

            }
            finally
            {
                oProgBar.Stop();
                oForm.Freeze(false);
            }

        }

        private void Button0_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            string strSQL = "";
            string tokenapi = "";
            string fileapi = "";

         SAPbouiCOM.ProgressBar   oProgBar1 = clsModule.objaddon.objapplication.StatusBar.CreateProgressBar("Loading Please Wait", Grid0.Rows.Count,false);
            oForm.Freeze(true);
            if (Grid0.Rows.Count > 0)
            {
                for (int i = 0; i < Grid0.Rows.Count; i++)
                {
                    string lstrcheckbox = Grid0.DataTable.Columns.Item("Checkbox").Cells.Item(i).Value.ToString();
                    if (lstrcheckbox == "Y")
                    {
                        strSQL = @"Select T0.""U_GST_ClientCode"",T0.""U_GST_UserCode"",T0.""U_GST_Password"",T1.""LineId"",T1.""U_URLType"",T0.""U_GST_Token"",Case when T0.""U_GST_Live""='N' then CONCAT(T0.""U_GST_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_GST_LIVEUrl"",T1.""U_URL"") End as URL";
                        strSQL += @" ,Case when T0.""U_GST_Live""='N' then T0.""U_GST_UATUrl"" Else T0.""U_GST_LIVEUrl"" End as BaseURL";
                        strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01' and T1.""U_URLType"" ='Token Generate' and T1.""U_Type""='GST' Order by ""LineId"" Desc";
                        DataTable dt1 = new DataTable();
                        dt1 = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);

                        if (dt1.Rows.Count == 0)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Token API is Missing ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return;
                        }
                        tokenapi = dt1.Rows[0]["URL"].ToString();
                        string data = dt1.Rows[0]["U_GST_Token"].ToString();
                        string clientcode = dt1.Rows[0]["U_GST_ClientCode"].ToString();
                        string username = dt1.Rows[0]["U_GST_UserCode"].ToString();
                        string password = dt1.Rows[0]["U_GST_Password"].ToString();
                        string formData = "data=" + data;
                        DataTable datatable = Get_API_Response(formData, tokenapi, contenttype: "application/x-www-form-urlencoded");
                        string Accesstkn = datatable.Rows[0]["accessToken"].ToString();

                        strSQL = @"Select T0.""U_GST_ClientCode"",T0.""U_GST_UserCode"",T0.""U_GST_Password"",T1.""LineId"",T1.""U_URLType"",T0.""U_GST_Token"",Case when T0.""U_GST_Live""='N' then CONCAT(T0.""U_GST_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_GST_LIVEUrl"",T1.""U_URL"") End as URL";
                        strSQL += @" ,Case when T0.""U_GST_Live""='N' then T0.""U_GST_UATUrl"" Else T0.""U_GST_LIVEUrl"" End as BaseURL";
                        strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01' and T1.""U_URLType"" ='File Upload' and T1.""U_Type""='GST' Order by ""LineId"" Desc";
                        dt1 = new DataTable();
                        dt1 = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);

                        if (dt1.Rows.Count == 0)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("File Upload API is Missing ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return;
                        }
                        fileapi = dt1.Rows[0]["URL"].ToString();
                        string formtype = "";
                        string TransType = "";
                        string table = "";
                        string subtable = "";
                        switch (ComboBox0.Selected.Value)
                        {
                            case "A/R Invoice":
                                formtype = "SalesRegister";
                                TransType = "INV";
                                table = "OINV";
                                subtable = "INV";
                                break;
                            case "A/R Credit Memo":
                                formtype = "SalesRegister";
                                TransType = "CRN";
                                table = "ORIN";
                                subtable = "RIN";
                                break;
                            case "A/P Invoice":
                                formtype = "PurchaseRegister";
                                TransType = "PCH";
                                table = "OPCH";
                                subtable = "PCH";
                                break;
                            case "A/P Credit Memo":
                                formtype = "PurchaseRegister";
                                TransType = "RPC";
                                table = "ORPC";
                                subtable = "RPC";
                                break;
                            case "A/R Invoice Down":
                                formtype = "SalesRegister";
                                TransType = "DPI";
                                table = "ODPI";
                                subtable = "DPI";
                                break;
                            case "A/P Invoice Down":
                                formtype = "PurchaseRegister";
                                TransType = "DPO";
                                table = "ODPO";
                                subtable = "DPO";
                                break;
                        }
                        string fromPeriodCode = clsModule.objaddon.objglobalmethods.DateFormat(EditText1.Value.ToString(), "yyyyMMdd", "MMyyyy");
                        string toPeriodCode = clsModule.objaddon.objglobalmethods.DateFormat(EditText2.Value.ToString(), "yyyyMMdd", "MMyyyy");
                        string gstin = ComboBox1.Selected.Value;


                        DataTable dtjson = new DataTable();
                        string jsonstring = "";
                        MultipartFormDataContent formContent = new MultipartFormDataContent();
                        string lstrdocentry = Grid0.DataTable.Columns.Item("DocEntry").Cells.Item(i).Value.ToString();
                        clsModule.objaddon.objInvoice.Generate_Cancel_IRN(ClsARInvoice.EinvoiceMethod.CreateIRN, lstrdocentry, TransType, "E-invoice", ref dtjson, true, ref jsonstring,false);

                        formContent.Add(new StringContent(clientcode), "clientCode");
                        formContent.Add(new StringContent(username), "userCode");
                        formContent.Add(new StringContent(gstin), "gstin");
                        formContent.Add(new StringContent(formtype), "formType");
                        formContent.Add(new StringContent(fromPeriodCode), "fromPeriodCode");
                        formContent.Add(new StringContent(toPeriodCode), "toPeriodCode");
                        formContent.Add(new StringContent(jsonstring), "Json_Data");


                        Dictionary<string, string> head = new Dictionary<string, string>();
                        head.Add("authorization", "Bearer " + Accesstkn);

                        var formData1 = new NameValueCollection
                    {
                        { "clientCode", clientcode },
                        { "userCode", username },
                        { "gstin", gstin },
                        { "formType",formtype},
                        { "fromPeriodCode", fromPeriodCode},
                        { "toPeriodCode", toPeriodCode },
                        { "Json_Data",jsonstring },
                    };
                        clsModule.objaddon.objglobalmethods.WriteErrorLog(jsonstring);
                        datatable = Get_API_Response(formData, fileapi, contenttype: "multipart/form-data", headers: head, formdata1: formData1);
                        if (datatable.Rows.Count > 0)
                        {
                            clsModule.objaddon.objglobalmethods.getSingleValue("update " + table + " set \"U_GST_RefNo\" = '" + datatable.Rows[0][0] + "',\"U_GST_status\"=0 where \"DocEntry\"='" + lstrdocentry+"'");
                        }
                    }
                    try
                    {
                        oProgBar1.Value += 1;
                    }
                    catch (Exception)
                    {

                       
                    }

                  
                }
                
                clsModule.objaddon.objapplication.StatusBar.SetText("Operation Compeletly successfully...", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            oProgBar1.Stop();
            oForm.Freeze(false);
        }

        private void Grid0_DoubleClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            oProgBar = clsModule.objaddon.objapplication.StatusBar.CreateProgressBar("Loading Please Wait", Grid0.Rows.Count, true);

            oForm.Freeze(true);
            if (pVal.Row == -1)
            {
                for (int i = 0; i < Grid0.Rows.Count; i++)
                {
                    Grid0.DataTable.SetValue("Checkbox", i, "Y");
                    oProgBar.Value += 1;
                }

            }
            oProgBar.Stop();
            oForm.Freeze(false);

        }

        private SAPbouiCOM.Button Button2;

        private void Button2_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            string strSQL = "";
            string tokenapi = "";
            string fileapi = "";

            strSQL = @"Select T0.""U_GST_ClientCode"",T0.""U_GST_UserCode"",T0.""U_GST_Password"",T1.""LineId"",T1.""U_URLType"",T0.""U_GST_Token"",Case when T0.""U_GST_Live""='N' then CONCAT(T0.""U_GST_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_GST_LIVEUrl"",T1.""U_URL"") End as URL";
            strSQL += @" ,Case when T0.""U_GST_Live""='N' then T0.""U_GST_UATUrl"" Else T0.""U_GST_LIVEUrl"" End as BaseURL";
            strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01' and T1.""U_URLType"" ='Token Generate' and T1.""U_Type""='GST' Order by ""LineId"" Desc";
            DataTable dt1 = new DataTable();
            dt1 = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);

            if (dt1.Rows.Count == 0)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText("Token API is Missing ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return;
            }
            tokenapi = dt1.Rows[0]["URL"].ToString();
            string data = dt1.Rows[0]["U_GST_Token"].ToString();
            string clientcode = dt1.Rows[0]["U_GST_ClientCode"].ToString();
            string username = dt1.Rows[0]["U_GST_UserCode"].ToString();
            string password = dt1.Rows[0]["U_GST_Password"].ToString();
            string formData = "data=" + data;
            DataTable datatable = Get_API_Response(formData, tokenapi, contenttype: "application/x-www-form-urlencoded");
            string Accesstkn = datatable.Rows[0]["accessToken"].ToString();

            strSQL = @"Select T0.""U_GST_ClientCode"",T0.""U_GST_UserCode"",T0.""U_GST_Password"",T1.""LineId"",T1.""U_URLType"",T0.""U_GST_Token"",Case when T0.""U_GST_Live""='N' then CONCAT(T0.""U_GST_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_GST_LIVEUrl"",T1.""U_URL"") End as URL";
            strSQL += @" ,Case when T0.""U_GST_Live""='N' then T0.""U_GST_UATUrl"" Else T0.""U_GST_LIVEUrl"" End as BaseURL";
            strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01' and T1.""U_URLType"" ='Status Check' and T1.""U_Type""='GST' Order by ""LineId"" Desc";
            dt1 = new DataTable();
            dt1 = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);

            if (dt1.Rows.Count == 0)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText("Status Check API is Missing ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return;
            }
            fileapi = dt1.Rows[0]["URL"].ToString();
            string formtype = "";
            string TransType = "";
            string table = "";
            string subtable = "";
            SAPbouiCOM.EditTextColumn oColumns;
          
            switch (ComboBox0.Selected.Value)
            {
                case "A/R Invoice":
                    formtype = "SalesRegister";
                    TransType = "INV";
                    table = "OINV";
                    subtable = "INV";
                    oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "13";
                    break;
                case "A/R Credit Memo":
                    formtype = "SalesRegister";
                    TransType = "CRN";
                    table = "ORIN";
                    subtable = "RIN";
                    oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "14";
                    break;
                case "A/P Invoice":
                    formtype = "PurchaseRegister";
                    TransType = "PCH";
                    table = "OPCH";
                    subtable = "PCH";
                    oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "18";
                    break;
                case "A/P Credit Memo":
                    formtype = "PurchaseRegister";
                    TransType = "RPC";
                    table = "ORPC";
                    subtable = "RPC";
                    oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "19";
                    break;
                case "A/R Invoice Down":
                    formtype = "SalesRegister";
                    TransType = "DPI";
                    table = "ODPI";
                    subtable = "DPI";
                    oColumns = (SAPbouiCOM.EditTextColumn)Grid0.Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "203";
                    break;
                case "A/P Invoice Down":
                    formtype = "PurchaseRegister";
                    TransType = "DPO";
                    table = "ODPO";
                    subtable = "DPO";                   
                    break;
            }

            string fromPeriodCode = clsModule.objaddon.objglobalmethods.DateFormat(EditText1.Value.ToString(), "yyyyMMdd", "MMyyyy");
            string toPeriodCode = clsModule.objaddon.objglobalmethods.DateFormat(EditText2.Value.ToString(), "yyyyMMdd", "MMyyyy");
            string gstin = ComboBox1.Selected.Value;


            DataTable dtjson = new DataTable();
            string jsonstring = "";
            string lstrdocentry = "";
            MultipartFormDataContent formContent = new MultipartFormDataContent();
            string filter=  " and a.\"U_GST_RefNo\" <>'' and a.\"U_GST_status\" =0";
            jsonstring = Loadquery(filter);
            
            DataTable dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(jsonstring);
           
            if (dt.Rows.Count > 0)
            {
                Dictionary<string, string> head = new Dictionary<string, string>();
                head.Add("authorization", "Bearer " + Accesstkn);

                foreach (DataRow item in dt.Rows)
                {
                    Statuscheck statuscheck = new Statuscheck();

                    lstrdocentry = Convert.ToString(item["DoCEntry"]);
                    statuscheck.ClientCode = clientcode;
                    statuscheck.FormType = formtype;
                    statuscheck.GSTIN = gstin;
                    statuscheck.ReferenceId = Convert.ToString(item["U_GST_RefNo"]);
                    statuscheck.UserCode = username;
                   string requestParams = JsonConvert.SerializeObject(statuscheck);
                    datatable = Get_API_Response(requestParams, fileapi, headers: head);
                    int ss = 0;
                    string remarks = "";
                    if (datatable.Rows.Count > 0)
                    {
                        string status = "";
                        status = columnFind(datatable, "statusCode", 0);
                        if (string.IsNullOrEmpty(status))
                        {
                            status = (columnFind(datatable, "Status_Code", 0));

                        }
                        
                        switch (status)
                        {
                            case "P":
                                ss = 1;
                                remarks = "Upload Successfully";
                                break;
                            case "E":
                                ss = 0;
                                remarks = "Invalid Reference";
                                break;
                            case "PW":
                                ss = 1;
                                remarks = datatable.Rows[0][3].ToString();
                                break;
                            default:
                                remarks = datatable.Rows[0][3].ToString();
                                break;
                        }                        
                        clsModule.objaddon.objglobalmethods.getSingleValue("update " + table + " set \"U_GST_status\" = '" + ss + "',\"U_GST_Remarks\"='"+ remarks + "' where \"DocEntry\"='" + lstrdocentry + "'");
                    }

                }
            }
            filter = " and a.\"U_GST_RefNo\" <>''";
            jsonstring = Loadquery(filter);
       

             dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(jsonstring);
            Grid0.DataTable.Rows.Clear();
            oProgBar = clsModule.objaddon.objapplication.StatusBar.CreateProgressBar("Loading Please Wait", dt.Rows.Count, true);

            oForm.Freeze(true);
          
            if (dt.Rows.Count > 0)
            {
                int i = 0;
                foreach (DataRow Drow in dt.Rows)
                {
                    Grid0.DataTable.Rows.Add();

                    Grid0.DataTable.SetValue("Checkbox", i, "N");
                    Grid0.DataTable.SetValue("Doc Number", i, Drow["DocNum"]);
                    Grid0.DataTable.SetValue("DocEntry", i, Drow["DocEntry"]);
                    Grid0.DataTable.SetValue("Doc Date", i, clsModule.objaddon.objglobalmethods.Getdateformat(Drow["DocDate"].ToString()));
                    Grid0.DataTable.SetValue("Customer", i, Drow["CardName"]);
                    Grid0.DataTable.SetValue("Total", i, Drow["DocTotal"]);
                    string stus = "Not Upload";
                    switch(Drow["U_GST_status"])
                    {
                        case "1":
                            stus = "Upload";
                            break;
                    }

                    Grid0.DataTable.SetValue("Status", i, stus);
                    Grid0.DataTable.SetValue("Remarks", i, Drow["U_GST_Remarks"]);
                    this.GetItem("Item_2").LinkTo = "DocEntry";
                    i++;
                    oProgBar.Value += 1;
                }
               
            }
           
            Grid0.AutoResizeColumns();
            Grid0.Columns.Item("Checkbox").Visible = false;
            oProgBar.Stop();
            oForm.Freeze(false);

        }
    }
}
