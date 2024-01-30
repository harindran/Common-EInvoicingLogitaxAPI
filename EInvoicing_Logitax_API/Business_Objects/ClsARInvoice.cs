using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoicing_Logitax_API.Common;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Diagnostics;
using static EInvoicing_Logitax_API.Common.clsGlobalMethods;
using Newtonsoft.Json.Linq;

namespace EInvoicing_Logitax_API.Business_Objects
{
    class ClsARInvoice : clsAddon
    {
        private SAPbouiCOM.Form oForm;
        private string strSQL;
        private SAPbobsCOM.Recordset objRs;
        private bool blnRefresh;
        SAPbouiCOM.ButtonCombo buttonCombo;
        private bool FromArinv;

        #region ITEM EVENT
        public override void Item_Event(string oFormUID, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            try
            {
                oForm = clsModule.objaddon.objapplication.Forms.Item(oFormUID);
                ClsARInvoice.EinvoiceMethod einvoiceMethod = ClsARInvoice.EinvoiceMethod.Default;
                string DocEntry = "";
                string TransType = "";
                string Type = "";
                SAPbouiCOM.ButtonCombo buttonCombo = null;
                if (pVal.BeforeAction)
                {
                    switch (pVal.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                            Create_Customize_Fields(oFormUID);
                            break;
                        case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT:
                            buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneinv").Specific;
                            if (pVal.ItemUID == "10000329")
                            {
                                FromArinv = true;
                            }


                            break;
                        case SAPbouiCOM.BoEventTypes.et_CLICK:
                            EnabledMenu(pVal.FormType.ToString());
                            break;
                    }
                }
                else
                {
                    switch (pVal.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_CLICK:
                            if (pVal.ItemUID == "einv")
                            {
                                oForm.PaneLevel = 26;
                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_ACTIVATE:
                            if (FromArinv)
                            {
                                SAPbouiCOM.Form activefrm = clsModule.objaddon.objapplication.Forms.ActiveForm;
                                Cleartext(activefrm);
                                FromArinv = false;
                            }

                            break;
                        case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT:
                            switch (pVal.FormType)
                            {

                                case 133:
                                    if (pVal.ItemUID == "btneinv" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        DocEntry = oForm.DataSources.DBDataSources.Item("OINV").GetValue("DocEntry", 0);
                                        TransType = "INV";
                                        buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneinv").Specific;
                                        if (buttonCombo.Selected.Value == "Create IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CreateIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Create Eway")
                                        {
                                            string irn = oForm.DataSources.DBDataSources.Item("OINV").GetValue("U_IRNNo", 0);
                                            string eway;
                                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                                            {
                                                eway = oForm.DataSources.DBDataSources.Item("INV26").GetValue("EWayBillNo", 0);
                                            }
                                            else
                                            {
                                                eway = oForm.DataSources.DBDataSources.Item("OINV").GetValue(clsModule.EwayNo, 0);

                                            }
                                            einvoiceMethod = irn == "" ? ClsARInvoice.EinvoiceMethod.CreateEway : ClsARInvoice.EinvoiceMethod.GetEwayByIRN;
                                            Type = irn == "" ? "E-way" : "E-way IRN";
                                            Type = eway == "" ? Type : "Update E-way";
                                            einvoiceMethod = eway == "" ? einvoiceMethod : ClsARInvoice.EinvoiceMethod.UpdateEway;
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel Eway")
                                        {
                                            string irn = oForm.DataSources.DBDataSources.Item("OINV").GetValue("U_IRNNo", 0);                                            
                                            einvoiceMethod = irn == "" ? ClsARInvoice.EinvoiceMethod.CancelEway : ClsARInvoice.EinvoiceMethod.CancelEwayIRN;                                                                                                                                                                              
                                            Type = "E-way";
                                        }
                                        else if (buttonCombo.Selected.Value == "Get Details IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.GetIrnByDocnum;
                                            Type = "E-Invoice";
                                        }
                                    }
                                    else if (pVal.ItemUID == "btneway" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneway").Specific;
                                        SAPbouiCOM.Form oUDFForm;
                                        string webpageURL = "";
                                        oUDFForm = clsModule.objaddon.objapplication.Forms.Item(oForm.UDFFormUID);

                                        if (buttonCombo.Selected.Value == "Eway Print")
                                        {
                                            webpageURL = ((SAPbouiCOM.EditText)(oUDFForm.Items.Item("U_Ewaypdf").Specific)).Value.ToString();
                                        }
                                        else if (buttonCombo.Selected.Value == "Eway Detail")
                                        {
                                            webpageURL = ((SAPbouiCOM.EditText)(oUDFForm.Items.Item("U_EwayDetpdf").Specific)).Value.ToString();
                                        }
                                        if (!string.IsNullOrEmpty(webpageURL))
                                        {
                                            Process.Start(webpageURL);
                                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                                        }
                                    }
                                    break;
                                case 179:
                                    if (pVal.ItemUID == "btneinv" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        DocEntry = oForm.DataSources.DBDataSources.Item("ORIN").GetValue("DocEntry", 0);
                                        TransType = "CRN";
                                        buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneinv").Specific;
                                        if (buttonCombo.Selected.Value == "Create IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CreateIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Create Eway")
                                        {
                                            string irn = oForm.DataSources.DBDataSources.Item("ORIN").GetValue("U_IRNNo", 0);
                                            string eway;
                                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                                            {
                                                eway = oForm.DataSources.DBDataSources.Item("RIN26").GetValue("EWayBillNo", 0);
                                            }
                                            else
                                            {
                                                eway = oForm.DataSources.DBDataSources.Item("ORIN").GetValue(clsModule.EwayNo, 0);

                                            }
                                            einvoiceMethod = irn == "" ? ClsARInvoice.EinvoiceMethod.CreateEway : ClsARInvoice.EinvoiceMethod.GetEwayByIRN;
                                            Type = irn == "" ? "E-way" : "E-way IRN";
                                            Type = eway == "" ? Type : "Update E-way";
                                            einvoiceMethod = eway == "" ? einvoiceMethod : ClsARInvoice.EinvoiceMethod.UpdateEway;
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel IRN")
                                        {                                          
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel Eway")
                                        {
                                            string irn = oForm.DataSources.DBDataSources.Item("ORIN").GetValue("U_IRNNo", 0);
                                            einvoiceMethod = irn == "" ? ClsARInvoice.EinvoiceMethod.CancelEway : ClsARInvoice.EinvoiceMethod.CancelEwayIRN;

                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelEway;
                                            Type = "E-way";
                                        }

                                    }
                                    else if (pVal.ItemUID == "btneway" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneway").Specific;
                                        SAPbouiCOM.Form oUDFForm;
                                        string webpageURL = "";
                                        oUDFForm = clsModule.objaddon.objapplication.Forms.Item(oForm.UDFFormUID);

                                        if (buttonCombo.Selected.Value == "Eway Print")
                                        {
                                            webpageURL = ((SAPbouiCOM.EditText)(oUDFForm.Items.Item("U_Ewaypdf").Specific)).Value.ToString();
                                        }
                                        else if (buttonCombo.Selected.Value == "Eway Detail")
                                        {
                                            webpageURL = ((SAPbouiCOM.EditText)(oUDFForm.Items.Item("U_EwayDetpdf").Specific)).Value.ToString();
                                        }
                                        if (!string.IsNullOrEmpty(webpageURL))
                                        {
                                            Process.Start(webpageURL);
                                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                                        }
                                    }
                                    break;

                                case 940:
                                    if (pVal.ItemUID == "btneinv" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        DocEntry = oForm.DataSources.DBDataSources.Item("OWTR").GetValue("DocEntry", 0);
                                        TransType = "WTR";
                                        buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneinv").Specific;
                                        if (buttonCombo.Selected.Value == "Create IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CreateIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Create Eway")
                                        {
                                            string irn = oForm.DataSources.DBDataSources.Item("OWTR").GetValue("U_IRNNo", 0);
                                            string eway;
                                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                                            {
                                                eway = oForm.DataSources.DBDataSources.Item("WTR26").GetValue("EWayBillNo", 0);
                                            }
                                            else
                                            {
                                                eway = oForm.DataSources.DBDataSources.Item("OWTR").GetValue(clsModule.EwayNo, 0);

                                            }
                                            einvoiceMethod = irn == "" ? ClsARInvoice.EinvoiceMethod.CreateEway : ClsARInvoice.EinvoiceMethod.GetEwayByIRN;
                                            Type = irn == "" ? "E-way" : "E-way IRN";
                                            Type = eway == "" ? Type : "Update E-way";
                                            einvoiceMethod = eway == "" ? einvoiceMethod : ClsARInvoice.EinvoiceMethod.UpdateEway;
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel Eway")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelEway;
                                            Type = "E-way";
                                        }

                                    }
                                    else if (pVal.ItemUID == "btneway" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneway").Specific;
                                        SAPbouiCOM.Form oUDFForm;
                                        string webpageURL = "";
                                        oUDFForm = clsModule.objaddon.objapplication.Forms.Item(oForm.UDFFormUID);

                                        if (buttonCombo.Selected.Value == "Eway Print")
                                        {
                                            webpageURL = ((SAPbouiCOM.EditText)(oUDFForm.Items.Item("U_Ewaypdf").Specific)).Value.ToString();
                                        }
                                        else if (buttonCombo.Selected.Value == "Eway Detail")
                                        {
                                            webpageURL = ((SAPbouiCOM.EditText)(oUDFForm.Items.Item("U_EwayDetpdf").Specific)).Value.ToString();
                                        }
                                        if (!string.IsNullOrEmpty(webpageURL))
                                        {
                                            Process.Start(webpageURL);
                                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                                        }
                                    }
                                    break;

                                case 140:
                                    if (pVal.ItemUID == "btneinv" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        DocEntry = oForm.DataSources.DBDataSources.Item("ODLN").GetValue("DocEntry", 0);
                                        TransType = "DLN";
                                        buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneinv").Specific;
                                        if (buttonCombo.Selected.Value == "Create IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CreateIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Create Eway")
                                        {
                                            string irn = oForm.DataSources.DBDataSources.Item("ODLN").GetValue("U_IRNNo", 0);
                                            string eway;
                                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                                            {
                                                eway = oForm.DataSources.DBDataSources.Item("DLN26").GetValue("EWayBillNo", 0);
                                            }
                                            else
                                            {
                                                eway = oForm.DataSources.DBDataSources.Item("ODLN").GetValue(clsModule.EwayNo, 0);

                                            }
                                            einvoiceMethod = irn == "" ? ClsARInvoice.EinvoiceMethod.CreateEway : ClsARInvoice.EinvoiceMethod.GetEwayByIRN;
                                            Type = irn == "" ? "E-way" : "E-way IRN";
                                            Type = eway == "" ? Type : "Update E-way";
                                            einvoiceMethod = eway == "" ? einvoiceMethod : ClsARInvoice.EinvoiceMethod.UpdateEway;
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel Eway")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelEway;
                                            Type = "E-way";
                                        }

                                    }
                                    else if (pVal.ItemUID == "btneway" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneway").Specific;
                                        SAPbouiCOM.Form oUDFForm;
                                        string webpageURL = "";
                                        oUDFForm = clsModule.objaddon.objapplication.Forms.Item(oForm.UDFFormUID);

                                        if (buttonCombo.Selected.Value == "Eway Print")
                                        {
                                            webpageURL = ((SAPbouiCOM.EditText)(oUDFForm.Items.Item("U_Ewaypdf").Specific)).Value.ToString();
                                        }
                                        else if (buttonCombo.Selected.Value == "Eway Detail")
                                        {
                                            webpageURL = ((SAPbouiCOM.EditText)(oUDFForm.Items.Item("U_EwayDetpdf").Specific)).Value.ToString();
                                        }
                                        if (!string.IsNullOrEmpty(webpageURL))
                                        {
                                            Process.Start(webpageURL);
                                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                                        }
                                    }
                                    break;
                                case 141:
                                    if (pVal.ItemUID == "btneinv" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        DocEntry = oForm.DataSources.DBDataSources.Item("OPCH").GetValue("DocEntry", 0);
                                        TransType = "PCH";
                                        buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneinv").Specific;
                                        if (buttonCombo.Selected.Value == "Create IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CreateIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Create Eway")
                                        {
                                            string irn = oForm.DataSources.DBDataSources.Item("OPCH").GetValue("U_IRNNo", 0);
                                            string eway;
                                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                                            {
                                                eway = oForm.DataSources.DBDataSources.Item("PCH26").GetValue("EWayBillNo", 0);
                                            }
                                            else
                                            {
                                                eway = oForm.DataSources.DBDataSources.Item("OPCH").GetValue(clsModule.EwayNo, 0);

                                            }
                                            einvoiceMethod = irn == "" ? ClsARInvoice.EinvoiceMethod.CreateEway : ClsARInvoice.EinvoiceMethod.GetEwayByIRN;
                                            Type = irn == "" ? "E-way" : "E-way IRN";
                                            Type = eway == "" ? Type : "Update E-way";
                                            einvoiceMethod = eway == "" ? einvoiceMethod : ClsARInvoice.EinvoiceMethod.UpdateEway;
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel Eway")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelEway;
                                            Type = "E-way";
                                        }

                                    }
                                    else if (pVal.ItemUID == "btneway" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneway").Specific;
                                        SAPbouiCOM.Form oUDFForm;
                                        string webpageURL = "";
                                        oUDFForm = clsModule.objaddon.objapplication.Forms.Item(oForm.UDFFormUID);

                                        if (buttonCombo.Selected.Value == "Eway Print")
                                        {
                                            webpageURL = ((SAPbouiCOM.EditText)(oUDFForm.Items.Item("U_Ewaypdf").Specific)).Value.ToString();
                                        }
                                        else if (buttonCombo.Selected.Value == "Eway Detail")
                                        {
                                            webpageURL = ((SAPbouiCOM.EditText)(oUDFForm.Items.Item("U_EwayDetpdf").Specific)).Value.ToString();
                                        }
                                        if (!string.IsNullOrEmpty(webpageURL))
                                        {
                                            Process.Start(webpageURL);
                                            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                                        }
                                    }
                                    break;
                            }
                            if (DocEntry != "" && TransType != "" && Type != "")
                            {
                                DataTable dt = new DataTable();
                                string jsonstring = "";
                                Generate_Cancel_IRN(einvoiceMethod, DocEntry, TransType, Type, ref dt,false,ref jsonstring,false);
                                buttonCombo.Caption = "Generate E-invoice";
                                if (dt.Rows.Count > 0)
                                {
                                    if (dt.Rows[0]["flag"].ToString() == "True" || blnRefresh)
                                    {
                                        // oForm.Items.Item("1").Click();
                                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                                        clsModule.objaddon.objapplication.Menus.Item("1304").Activate();
                                        clsModule.objaddon.objapplication.StatusBar.SetText("Operation Compeletly successfully...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);


                                    }
                                }
                            }

                            break;
                    }
                }

            }
            catch (Exception Ex)
            {
                return;
            }
            finally
            {

            }
        }
        #endregion

        public void EnabledMenu(string oFormUID, bool Penable = false, string UDFormID = "")
        {
            switch (oFormUID)
            {
                case "133":
                case "179":                
                    oForm.Items.Item("txtIrn").Enabled = Penable;
                    oForm.Items.Item("txtqrcode").Enabled = Penable;
                    oForm.Items.Item("txtAckNo").Enabled = Penable;
                    oForm.Items.Item("txtstatus").Enabled = Penable;
                    break;
                case "-133":
                case "-179":                
                    SAPbouiCOM.Form oUDFForm;
                    if (UDFormID == "")
                    {
                        if (oForm.UDFFormUID != "")
                        {
                            oUDFForm = clsModule.objaddon.objapplication.Forms.Item(oForm.UDFFormUID);
                        }
                    }
                    if (UDFormID != "")
                    {
                        oUDFForm = clsModule.objaddon.objapplication.Forms.Item(UDFormID);
                        oUDFForm.Items.Item("U_IRNNo").Enabled = Penable;
                        oUDFForm.Items.Item("U_QRCode").Enabled = Penable;
                        oUDFForm.Items.Item("U_AckDate").Enabled = Penable;
                        oUDFForm.Items.Item("U_AckNo").Enabled = Penable;
                        oUDFForm.Items.Item("U_IRNStatus").Enabled = Penable;

                    }
                    break;
            }

        }
        #region FORM DATA EVENT
        public override void FormData_Event(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
            try
            {

                if (BusinessObjectInfo.BeforeAction)
                {
                    switch (BusinessObjectInfo.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                            break;
                    }
                }
                else
                {
                    switch (BusinessObjectInfo.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD:
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText(Ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                BubbleEvent = false;
                return;
            }
            finally
            {
                // oForm.Freeze(false);
            }
        }
        #endregion

        public string GetInvoiceData(string DocEntry, string TransType)
        {
            DataTable dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(@"SELECT ""U_HSNL"",""U_SERCONFIG"" FROM ""@ATEICFG""");

            Querycls qcls = new Querycls();
            if (dt.Rows.Count > 0)
            {
                if (!String.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["U_HSNL"])))
                {
                    qcls.HSNLength = clsModule.objaddon.objglobalmethods.Ctoint(dt.Rows[0]["U_HSNL"]);
                }

                if (!String.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["U_SERCONFIG"])))
                {
                    qcls.docseries = Convert.ToString(dt.Rows[0]["U_SERCONFIG"]);
                }
            }

            switch (TransType)
            {
                case "INV":
                    strSQL = qcls.InvoiceQuery(DocEntry);
                    break;
                case "CRN":
                    strSQL = qcls.CreditNoteQuery(DocEntry);
                    break;
                case "WTR":
                    strSQL = qcls.InventoryTransferQuery(DocEntry);
                    break;
                case "DLN":
                    strSQL = qcls.DeliveryQuery(DocEntry);
                    break;
                case "PCH":
                    strSQL = qcls.PurchaseQuery(DocEntry);
                    break;
                case "RPC":
                    strSQL = qcls.DebitNoteQuery(DocEntry);
                    break;
                case "DPO":
                    strSQL = qcls.InvoiceDownQuery(DocEntry);
                    break; 
                case "DPI":
                    strSQL = qcls.PurchaseDownQuery(DocEntry);
                    break;

            }
            if (!clsModule.HANA)
            {
                strSQL = clsModule.objaddon.objglobalmethods.ChangeHANAtoSql(strSQL);
            }
            return strSQL;
        }
        public string GetFrightData(string DocEntry,string Transtype)
        {
            string maintb = "";
            string subtb1 = "";
            switch (Transtype)
            {
                case "INV":
                    maintb = "INV3";
                    subtb1 = "INV4";
                    break;
                case "CRN":
                    maintb = "RIN3";
                    subtb1 = "RIN4";
                    break;
                case "WTR":
                    maintb = "WTR3";
                    subtb1 = "WTR4";
                    break;
                case "DLN":
                    maintb = "DLN3";
                    subtb1 = "DLN4";
                    break;
                case "PCH":
                    maintb = "PCH3";
                    subtb1 = "PCH4";
                    break;
                case "RPC":
                    maintb = "RPC3";
                    subtb1 = "RPC4";
                    break;
                case "DPO":
                    maintb = "DPO3";
                    subtb1 = "DPO4";
                    break;
                case "DPI":
                    maintb = "DPI3";
                    subtb1 = "DPI4";
                    break;
            }

            //strSQL = @" Select 'Freight' as Dscription,1 as Quantity,'9965' as HSN,TF.""VatPrcnt"",TF.""LineTotal"",TF.""GrsAmount"" as ""Total Value"",";
            //strSQL += @" IFNULL((select sum(""TaxSum"") from INV4 where ""DocEntry"" = TF.""DocEntry"" and ""LineNum"" = TF.""LineNum"" and ""staType"" = '-100' AND ""ExpnsCode"" <> '-1'),0) as CGSTAmt,IFNULL((select sum(""TaxSum"") from INV4 where ""DocEntry"" = TF.""DocEntry"" and ""LineNum"" = TF.""LineNum"" and ""staType"" = -110 and ""ExpnsCode"" <> '-1'),0) as SGSTAmt,";
            //strSQL += @"IFNULL((select sum(""TaxSum"") from INV4 where ""DocEntry"" = TF.""DocEntry"" and ""LineNum"" = TF.""LineNum"" and ""staType"" = '-120' AND  ""ExpnsCode"" <> '-1'),0) as IGSTAmt from INV3 TF where TF.""DocEntry"" = " + DocEntry + @" and TF.""ExpnsCode"" <> '-1'";


            strSQL = @" Select 'Freight' as Dscription,1 as Quantity,'9965' as HSN,TF.""VatPrcnt"",TF.""LineTotal"",TF.""GrsAmount"" as ""Total Value"",";
            strSQL += @" IFNULL((select sum(""TaxSum"") from " + subtb1 + @" where ""DocEntry"" = TF.""DocEntry"" and ""LineNum"" = TF.""LineNum"" and ""staType"" = '-100'
                        AND ""ExpnsCode"" <> '-1'),0) as CGSTAmt,IFNULL((select sum(""TaxSum"") from " + subtb1 + @" where ""DocEntry"" = TF.""DocEntry"" 
                        and ""LineNum"" = TF.""LineNum"" and ""staType"" = -110 and ""ExpnsCode"" <> '-1'),0) as SGSTAmt,";
            strSQL += @"IFNULL((select sum(""TaxSum"") from "+ subtb1 + @" where ""DocEntry"" = TF.""DocEntry"" and ""LineNum"" = TF.""LineNum"" and ""staType"" = '-120'
                        AND  ""ExpnsCode"" <> '-1'),0) as IGSTAmt from "+ maintb + @" TF where TF.""DocEntry"" = " + DocEntry + @" and TF.""ExpnsCode"" <> '-1'";


            if (!clsModule.HANA)
            {
                strSQL = clsModule.objaddon.objglobalmethods.ChangeHANAtoSql(strSQL);
            }

            return strSQL;
        }
        public enum EinvoiceMethod
        {
            Default = 0,
            CreateIRN = 1,
            CancelIRN = 2,
            GetIrnByDocnum = 3,
            GETIRNDetails = 4,
            CreateEway = 5,
            CancelEway = 6,
            GetEwayByIRN = 7,
            UpdateEway = 8,
            CancelEwayIRN = 9,


        }

        private void Create_Customize_Fields(string oFormUID)
        {
            oForm = clsModule.objaddon.objapplication.Forms.Item(oFormUID);
            try
            {
                switch (oForm.TypeEx)
                {
                    case "133":
                        break;
                    case "179":
                        break;
                    case "940":
                        break;
                    case "140":
                        break;
                    case "141":
                        break;
                    default:
                        return;
                }

                SAPbouiCOM.Item oItem;


                try
                {
                    if (oForm.Items.Item("btneinv").UniqueID == "btneinv")
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    //throw;
                }
                switch (oForm.TypeEx)
                {
                    case "133":
                    case "179":
                        SAPbouiCOM.Folder objfolder;
                        oItem = oForm.Items.Add("einv", SAPbouiCOM.BoFormItemTypes.it_FOLDER);
                        objfolder = (SAPbouiCOM.Folder)oItem.Specific;
                        oItem.AffectsFormMode = false;
                        objfolder.Caption = "E-Invoice Details";
                        objfolder.GroupWith("1320002137");
                        objfolder.Pane = 26;
                        oItem.Width = 125;
                        oItem.Visible = true;
                       // oForm.PaneLevel = 1;
                        oItem.Left = oForm.Items.Item("1320002137").Left + oForm.Items.Item("1320002137").Width;
                        oItem.Enabled = true;
                        break;
                    case "940":
                    case "140":
                    case "141":
                        break;
                }

                oItem = oForm.Items.Add("btneinv", SAPbouiCOM.BoFormItemTypes.it_BUTTON_COMBO);
                buttonCombo = (SAPbouiCOM.ButtonCombo)oItem.Specific;
                buttonCombo.Caption = "Generate E-invoice";
                oItem.Left = oForm.Items.Item("2").Left + oForm.Items.Item("2").Width + clsModule.objaddon.objglobalmethods.Ctoint(clsModule.objaddon.objglobalmethods.getSingleValue("select \"U_BtnPos\" from \"@ATEICFG\" where \"Code\"=01" ));
                oItem.Top = oForm.Items.Item("2").Top;
                oItem.Height = oForm.Items.Item("2").Height;
                oItem.LinkTo = "2";
                Size Fieldsize = System.Windows.Forms.TextRenderer.MeasureText("Generate E-Invoice", new Font("Arial", 12.0f));
                oItem.Width = Fieldsize.Width;

                switch (oForm.TypeEx)
                {
                    case "133":
                    case "179":
                        buttonCombo.ValidValues.Add("Create IRN", "Create E-invoice");
                        buttonCombo.ValidValues.Add("Cancel IRN", "Cancel E-invoice");
                        break;
                    case "940":
                    case "140":
                    case "141":
                        break;
                }
                buttonCombo.ValidValues.Add("Create Eway", "Create Eway");
                buttonCombo.ValidValues.Add("Cancel Eway", "Cancel Eway");
                buttonCombo.ValidValues.Add("Get Details IRN", "Get Details IRN");


                buttonCombo.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
                oForm.Items.Item("btneinv").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, clsModule.objaddon.objglobalmethods.Ctoint(SAPbouiCOM.BoAutoFormMode.afm_Add), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("btneinv").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, clsModule.objaddon.objglobalmethods.Ctoint(SAPbouiCOM.BoAutoFormMode.afm_Find), SAPbouiCOM.BoModeVisualBehavior.mvb_False);


                oItem = oForm.Items.Add("btneway", SAPbouiCOM.BoFormItemTypes.it_BUTTON_COMBO);
                buttonCombo = (SAPbouiCOM.ButtonCombo)oItem.Specific;
                buttonCombo.Caption = "Print E-Way";
                oItem.Left = oForm.Items.Item("btneinv").Left + oForm.Items.Item("btneinv").Width + 5;
                oItem.Top = oForm.Items.Item("btneinv").Top;
                oItem.Height = oForm.Items.Item("btneinv").Height;
                oItem.LinkTo = "2";
                Fieldsize = System.Windows.Forms.TextRenderer.MeasureText("Print E-Way", new Font("Arial", 12.0f));
                oItem.Width = Fieldsize.Width;
                buttonCombo.ValidValues.Add("Eway Print", "Print Eway");
                buttonCombo.ValidValues.Add("Eway Detail", "Detail Eway");

                buttonCombo.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
                oForm.Items.Item("btneway").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, clsModule.objaddon.objglobalmethods.Ctoint(SAPbouiCOM.BoAutoFormMode.afm_Add), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("btneway").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, clsModule.objaddon.objglobalmethods.Ctoint(SAPbouiCOM.BoAutoFormMode.afm_Find), SAPbouiCOM.BoModeVisualBehavior.mvb_False);


                SAPbouiCOM.Item newTextBox;
                SAPbouiCOM.EditText otxt;
                SAPbouiCOM.StaticText olbl;
                string tablename = "";
                oForm.Freeze(true);

                switch (oForm.TypeEx)
                {
                    case "133":
                        tablename = "OINV";
                        break;
                    case "179":
                        tablename = "ORIN";
                        break;
                    default:
                        return;

                }
                #region "IRN"
                newTextBox = oForm.Items.Add("lblIrn", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("112").Left + 20;
                newTextBox.Top = oForm.Items.Item("112").Top + 25;
                newTextBox.Width = 250;
                olbl = (SAPbouiCOM.StaticText)oForm.Items.Item("lblIrn").Specific;
                ((SAPbouiCOM.StaticText)(olbl.Item.Specific)).Caption = "IRN No";


                newTextBox = oForm.Items.Add("txtIrn", SAPbouiCOM.BoFormItemTypes.it_EXTEDIT);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("lblIrn").Left + 80;
                newTextBox.Top = oForm.Items.Item("112").Top + 25;
                newTextBox.Width = 500;
                otxt = (SAPbouiCOM.EditText)oForm.Items.Item("txtIrn").Specific;
                otxt.DataBind.SetBound(true, tablename, "U_IRNNo");
                #endregion
                #region "QRCode"
                newTextBox = oForm.Items.Add("lblQrcode", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("112").Left + 20;
                newTextBox.Top = oForm.Items.Item("txtIrn").Top + oForm.Items.Item("txtIrn").Height + 2;
                newTextBox.Width = 250;
                olbl = (SAPbouiCOM.StaticText)oForm.Items.Item("lblQrcode").Specific;
                ((SAPbouiCOM.StaticText)(olbl.Item.Specific)).Caption = "Qrcode";

                newTextBox = oForm.Items.Add("txtqrcode", SAPbouiCOM.BoFormItemTypes.it_EXTEDIT);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("lblQrcode").Left + 80;
                newTextBox.Top = oForm.Items.Item("txtIrn").Top + oForm.Items.Item("txtIrn").Height + 2;
                newTextBox.Width = 500;
                otxt = (SAPbouiCOM.EditText)oForm.Items.Item("txtqrcode").Specific;
                otxt.DataBind.SetBound(true, tablename, "U_QRCode");
                #endregion
                #region "Ack No"
                newTextBox = oForm.Items.Add("lblAckNo", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("112").Left + 20;
                newTextBox.Top = oForm.Items.Item("txtqrcode").Top + oForm.Items.Item("txtqrcode").Height + 2;
                newTextBox.Width = 250;
                olbl = (SAPbouiCOM.StaticText)oForm.Items.Item("lblAckNo").Specific;
                ((SAPbouiCOM.StaticText)(olbl.Item.Specific)).Caption = "Ack No";

                newTextBox = oForm.Items.Add("txtAckNo", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("lblAckNo").Left + 80;
                newTextBox.Top = oForm.Items.Item("txtqrcode").Top + oForm.Items.Item("txtqrcode").Height + 2;
                newTextBox.Width = 200;
                otxt = (SAPbouiCOM.EditText)oForm.Items.Item("txtAckNo").Specific;
                otxt.DataBind.SetBound(true, tablename, "U_AckNo");
                #endregion
                #region "IRN Status"
                newTextBox = oForm.Items.Add("IRNStatus", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("112").Left + 20;
                newTextBox.Top = oForm.Items.Item("txtAckNo").Top + oForm.Items.Item("txtAckNo").Height + 2;
                newTextBox.Width = 250;
                olbl = (SAPbouiCOM.StaticText)oForm.Items.Item("IRNStatus").Specific;
                ((SAPbouiCOM.StaticText)(olbl.Item.Specific)).Caption = "IRN Status";

                newTextBox = oForm.Items.Add("txtstatus", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("IRNStatus").Left + 80;
                newTextBox.Top = oForm.Items.Item("txtAckNo").Top + oForm.Items.Item("txtAckNo").Height + 2;
                newTextBox.Width = 200;
                otxt = (SAPbouiCOM.EditText)oForm.Items.Item("txtstatus").Specific;
                otxt.DataBind.SetBound(true, tablename, "U_IRNStatus");
                #endregion

                oForm.Freeze(false);

            }
            catch (Exception ex)
            {
            }
        }




        public bool Generate_Cancel_IRN(EinvoiceMethod Create_Cancel, string DocEntry, string TransType, string Type, ref DataTable datatable,
            bool fromGst,ref string jsonstring,bool frommul)
        {
            string requestParams;
            string SapMessage;

            string ClientCode = "";
            string UserCode = "";
            string Password = "";
            bool Getbranch = false;
                    
            try
            {


                SAPbobsCOM.Recordset invrecordset, Freightrecset,disRecset,RSbranch;
                objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                Freightrecset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"" from ""@ATEICFG"" T0  where T0.""Code"" ='01'";

                objRs.DoQuery(strSQL);
                if (objRs.RecordCount > 0)
                {
                    ClientCode= objRs.Fields.Item("U_ClientCode").Value.ToString();
                    UserCode = objRs.Fields.Item("U_UserCode").Value.ToString();
                    Password = objRs.Fields.Item("U_Password").Value.ToString();
                }

             


                if (Create_Cancel == EinvoiceMethod.CreateIRN)
                {
                    GenerateIRN GenerateIRNGetJson = new GenerateIRN();

                    strSQL = GetInvoiceData(DocEntry, TransType);
                    invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                    clsModule.objaddon.objapplication.StatusBar.SetText("Generating Einvoice. Please Wait....", SAPbouiCOM.BoMessageTime.bmt_Medium, !(fromGst|| frommul )? SAPbouiCOM.BoStatusBarMessageType.smt_Warning:SAPbouiCOM.BoStatusBarMessageType.smt_None);
                    invrecordset.DoQuery(strSQL);
                    if (invrecordset.RecordCount > 0)
                    {

                        strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"" from ""OBPL"" T0 ";
                        strSQL += @" where T0.""BPLId"" ='" + invrecordset.Fields.Item("BPLId").Value.ToString() + "'";

                        objRs.DoQuery(strSQL);
                        if (objRs.RecordCount > 0)
                        {
                            if (!string.IsNullOrEmpty(objRs.Fields.Item("U_ClientCode").Value.ToString()))
                            {
                                ClientCode = objRs.Fields.Item("U_ClientCode").Value.ToString();
                                UserCode = objRs.Fields.Item("U_UserCode").Value.ToString();
                                Password = objRs.Fields.Item("U_Password").Value.ToString();
                                Getbranch = true;
                            }
                        }





                        Double Calcdistance = 0;
                        strSQL = @"Select T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                        strSQL += @" ,""U_GetCompAdd"",""U_Gettran"",""U_GetDisAddWare"",""U_ShipToInvName"",""U_BillToWare"",""U_GettrnShp"", ";
                        strSQL += @" Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                        strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01' and T1.""U_URLType"" ='Generate IRN' and T1.""U_Type""='E-Invoice' Order by ""LineId"" Desc";
                        objRs.DoQuery(strSQL);
                        if (objRs.RecordCount == 0)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Create IRN\". Please update in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }

                        if (string.IsNullOrEmpty(invrecordset.Fields.Item("Buyer GSTN").Value.ToString()) && !fromGst)
                        {
                            if (!string.IsNullOrEmpty(clsModule.GSTCol))
                            {
                                if (string.IsNullOrEmpty(invrecordset.Fields.Item(clsModule.GSTCol).Value.ToString()))                                    
                                {
                                    clsModule.objaddon.objapplication.StatusBar.SetText("GST No is Missing for \"Create GSTNo\"...for " + clsModule.GSTCol , SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    return false;
                                }
                            }
                            else
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("GST No is Missing for \"Create GSTNo\"...for " + invrecordset.Fields.Item("PayToCode").Value.ToString(), SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                return false;
                            }
                          
                        }

                        string AssignEinvunit = invrecordset.Fields.Item("Unit").Value.ToString();
                        string Isservice = "N";
                        string ServiceHSN = invrecordset.Fields.Item("HSN").Value.ToString();
                        if (invrecordset.Fields.Item("IsServc").Value.ToString() == "Y")
                        {
                            Isservice = "Y";
                            ServiceHSN = invrecordset.Fields.Item("SacCode").Value.ToString();
                        }
                        else
                        {
                            if (invrecordset.Fields.Item("ItemClass").Value.ToString() == "1")
                            {
                                Isservice = "Y";
                                ServiceHSN = invrecordset.Fields.Item("SacCode").Value.ToString();
                            }
                        }

                        if (Isservice == "N")
                        {
                            strSQL = "SELECT \"U_GUnitCod\"  FROM \"@UOMMAP\" u WHERE u.\"U_UOMCod\" ='" + AssignEinvunit + "'";
                            DataTable dt1 = new DataTable();
                            dt1 = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);
                            if (dt1.Rows.Count > 0 && AssignEinvunit != "")
                            {
                                AssignEinvunit = dt1.Rows[0]["U_GUnitCod"].ToString();
                            }
                            else
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Unit UOM Name(" + AssignEinvunit + ") Not Mapped please Map Unit... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                return false;
                            }
                        }
                        if (String.IsNullOrEmpty(clsModule.EwayNo))
                        {

                            if (clsModule.objaddon.objglobalmethods.CtoD(invrecordset.Fields.Item("Distance").Value) > 0)
                            {
                                Generate_EWay distanceEway = new Generate_EWay();
                                distanceEway.CLIENTCODE = ClientCode;
                                distanceEway.USERCODE = UserCode;
                                distanceEway.PASSWORD = Password;
                                distanceEway.frompincode = invrecordset.Fields.Item("FrmZipCode").Value.ToString();
                                distanceEway.topincode = invrecordset.Fields.Item("ToZipCode").Value.ToString();


                                if (string.IsNullOrEmpty(invrecordset.Fields.Item("FrmZipCode").Value.ToString()) || string.IsNullOrEmpty(invrecordset.Fields.Item("ToZipCode").Value.ToString()))
                                {
                                    clsModule.objaddon.objapplication.StatusBar.SetText("Zip Code Missing E-way Details Page.... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    return false;
                                }
                                if (invrecordset.Fields.Item("FrmZipCode").Value.ToString() == invrecordset.Fields.Item("ToZipCode").Value.ToString())
                                {
                                    Calcdistance = 1;
                                }
                                Calcdistance = clsModule.objaddon.objglobalmethods.Ctoint(invrecordset.Fields.Item("Distance").Value);
                            }
                        }

                        GenerateIRNGetJson.client_code = ClientCode;
                        GenerateIRNGetJson.user_code = UserCode;
                        GenerateIRNGetJson.password = Password;
                        GenerateIRNGetJson.json_data.Version = "1.1";
                        GenerateIRNGetJson.json_data.TranDtls.TaxSch = invrecordset.Fields.Item("TaxSch").Value.ToString();
                        GenerateIRNGetJson.json_data.TranDtls.SupTyp = invrecordset.Fields.Item("SupTyp").Value.ToString();
                        GenerateIRNGetJson.json_data.TranDtls.RegRev = "N";
                        GenerateIRNGetJson.json_data.TranDtls.EcmGstin = "";
                        GenerateIRNGetJson.json_data.TranDtls.IgstOnIntra = "";

                        GenerateIRNGetJson.json_data.DocDtls.Typ = invrecordset.Fields.Item("Type").Value.ToString();
                        GenerateIRNGetJson.json_data.DocDtls.No = invrecordset.Fields.Item("Inv_No").Value.ToString();
                        GenerateIRNGetJson.json_data.DocDtls.Dt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv_Doc_Date").Value.ToString());

                       
                        string sellerAddress1 = "";
                        string sellerAddress2 = "";

                        string SellerconcatAddress = string.Concat(invrecordset.Fields.Item("Seller_Building").Value.ToString()," ",
                                                            invrecordset.Fields.Item("Seller_Block").Value.ToString(), " ",
                                                            invrecordset.Fields.Item("Seller_Street").Value.ToString());

                        List<string> Sellersubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(SellerconcatAddress, 70);

                        foreach (string substring in Sellersubstrings)
                        {
                            if (string.IsNullOrEmpty(sellerAddress1))
                            {
                                sellerAddress1 = substring;
                                continue;
                            }
                            if (string.IsNullOrEmpty(sellerAddress2))
                            {
                                sellerAddress2 = substring;
                                continue;
                            }
                        }

                        #region "Seller"
                        if (objRs.Fields.Item("U_GetCompAdd").Value.ToString() != "Y")
                        {
                            if (Getbranch)
                            {

                                string BranchAddress1 = "";
                                string BranchAddress2 = "";

                             
                                strSQL = "Select \"TaxIdNum\" AS \"GST\",\"BPLName\"  AS \"SellerName\", ";
                                strSQL +=" \"Building\" ,\"Block\" ,\"Street\",\"City\" ,\"ZipCode\",  COALESCE(\"GSTCode\", '96') AS \"Statecode\" from \"OBPL\" T0 ";
                                strSQL = strSQL + " LEFT JOIN OCST st on st.\"Code\"=T0.\"State\" and st.\"Country\"=T0.\"Country\" ";
                                strSQL += @" where T0.""BPLId"" ='" + invrecordset.Fields.Item("BPLId").Value.ToString() + "'";

                                

                                RSbranch = clsModule.objaddon.objglobalmethods.GetmultipleRS(strSQL);
                                if (RSbranch.RecordCount > 0)
                                {

                                    string BranchrconcatAddress = string.Concat(RSbranch.Fields.Item("Building").Value.ToString(), " ",
                                                                 RSbranch.Fields.Item("Block").Value.ToString(), " ",
                                                                 RSbranch.Fields.Item("Street").Value.ToString());

                                    List<string> Branchsubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(BranchrconcatAddress, 70);

                                    foreach (string substring in Branchsubstrings)
                                    {
                                        if (string.IsNullOrEmpty(BranchAddress1))
                                        {
                                            BranchAddress1 = substring;
                                            continue;
                                        }
                                        if (string.IsNullOrEmpty(BranchAddress2))
                                        {
                                            BranchAddress2 = substring;
                                            continue;
                                        }
                                    }

                                    GenerateIRNGetJson.json_data.SellerDtls.Gstin = RSbranch.Fields.Item("GST").Value.ToString();
                                    GenerateIRNGetJson.json_data.SellerDtls.LglNm = RSbranch.Fields.Item("SellerName").Value.ToString();
                                    GenerateIRNGetJson.json_data.SellerDtls.Addr1 = BranchAddress1;
                                    GenerateIRNGetJson.json_data.SellerDtls.Addr2 = BranchAddress2;
                                    GenerateIRNGetJson.json_data.SellerDtls.Loc = RSbranch.Fields.Item("City").Value.ToString();
                                    GenerateIRNGetJson.json_data.SellerDtls.Pin = RSbranch.Fields.Item("ZipCode").Value.ToString();
                                    GenerateIRNGetJson.json_data.SellerDtls.Stcd = RSbranch.Fields.Item("Statecode").Value.ToString();
                                }

                            }
                            else if (objRs.Fields.Item("U_BillToWare").Value.ToString() == "Y")
                            {
                                string Line1 = "";
                               
                                switch (TransType)
                                {
                                    case "INV":
                                        Line1 = "INV1";
                                        break;
                                    case "CRN":
                                        Line1 = "RIN1";
                                        break;
                                    case "WTR":
                                        Line1 = "WTR1";
                                        break;
                                    case "DLN":
                                        Line1 = "DLN1";
                                        break;
                                    case "PCH":
                                        Line1 = "PCH1";
                                        break;
                                    case "RPC":
                                        Line1 = "RPC1";
                                        break;
                                    case "DPO":
                                        Line1 = "DPO1";
                                        break;
                                    case "DPI":
                                        Line1 = "DPI1";
                                        break;
                                }

                                string diswarequery = "SELECT \"Building\" ,\"Block\" ,\"Street\" ,\"Address2\" ,\"Address3\" , ";
                                diswarequery += " \"City\" ,\"ZipCode\" , COALESCE(\"GSTCode\",'96') as \"Statecode\"  ";
                                diswarequery += "FROM OWHS o ";
                                diswarequery += " LEFT JOIN OCST st1 on st1.\"Code\"=o.\"State\" and st1.\"Country\"=o.\"Country\" ";
                                diswarequery += " WHERE \"WhsCode\" IN(SELECT \"WhsCode\"  FROM "+ Line1 + " WHERE  \"DocEntry\" = '" + DocEntry + "'); ";

                                disRecset = clsModule.objaddon.objglobalmethods.GetmultipleRS(diswarequery);
                                if (disRecset.RecordCount > 0)
                                {
                                    string DisAddress1 = "";
                                    string DisAddress2 = "";
                                    string DisconcatAddress = string.Concat(disRecset.Fields.Item("Building").Value.ToString(), " ",
                                                                        disRecset.Fields.Item("Block").Value.ToString(), " ",
                                                                        disRecset.Fields.Item("Street").Value.ToString(), " ",
                                                                        disRecset.Fields.Item("Address2").Value.ToString(), " ",
                                                                        disRecset.Fields.Item("Address3").Value.ToString());
                                    List<string> Dissubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(DisconcatAddress, 70);
                                    foreach (string substring in Dissubstrings)
                                    {
                                        if (string.IsNullOrEmpty(DisAddress1))
                                        {
                                            DisAddress1 = substring;
                                            continue;
                                        }
                                        if (string.IsNullOrEmpty(DisAddress2))
                                        {
                                            DisAddress2 = substring;
                                            continue;
                                        }
                                    }

                                    GenerateIRNGetJson.json_data.SellerDtls.Gstin = invrecordset.Fields.Item("Seller GSTN").Value.ToString();
                                    GenerateIRNGetJson.json_data.SellerDtls.LglNm = invrecordset.Fields.Item("Seller_Legal Name").Value.ToString();
                                    GenerateIRNGetJson.json_data.SellerDtls.Addr1 = DisAddress1;
                                    GenerateIRNGetJson.json_data.SellerDtls.Addr2 = DisAddress2;
                                    GenerateIRNGetJson.json_data.SellerDtls.Loc = disRecset.Fields.Item("City").Value.ToString();
                                    GenerateIRNGetJson.json_data.SellerDtls.Pin = disRecset.Fields.Item("ZipCode").Value.ToString();
                                    GenerateIRNGetJson.json_data.SellerDtls.Stcd = disRecset.Fields.Item("Statecode").Value.ToString();
                                }

                            }

                            else
                            {
                                GenerateIRNGetJson.json_data.SellerDtls.Gstin = invrecordset.Fields.Item("Seller GSTN").Value.ToString();
                                GenerateIRNGetJson.json_data.SellerDtls.LglNm = invrecordset.Fields.Item("Seller_Legal Name").Value.ToString();
                                GenerateIRNGetJson.json_data.SellerDtls.Addr1 = sellerAddress1;
                                GenerateIRNGetJson.json_data.SellerDtls.Addr2 = sellerAddress2;
                                GenerateIRNGetJson.json_data.SellerDtls.Loc = invrecordset.Fields.Item("Seller Location Name").Value.ToString();
                                GenerateIRNGetJson.json_data.SellerDtls.Pin = invrecordset.Fields.Item("Seller_PIN code").Value.ToString();
                                GenerateIRNGetJson.json_data.SellerDtls.Stcd = invrecordset.Fields.Item("Seller_State_code").Value.ToString();
                            }
                        }
                        else
                        {
                            sellerAddress1 = "";
                            sellerAddress2 = "";
                            SellerconcatAddress = invrecordset.Fields.Item("CompnyAddr").Value.ToString();

                            Sellersubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(SellerconcatAddress, 70);

                            foreach (string substring in Sellersubstrings)
                            {
                                if (string.IsNullOrEmpty(sellerAddress1))
                                {
                                    sellerAddress1 = substring;
                                    continue;
                                }
                                if (string.IsNullOrEmpty(sellerAddress2))
                                {
                                    sellerAddress2 = substring;
                                    continue;
                                }
                            }

                            GenerateIRNGetJson.json_data.SellerDtls.Gstin = invrecordset.Fields.Item("CompnyGSTIN").Value.ToString();
                            GenerateIRNGetJson.json_data.SellerDtls.LglNm = invrecordset.Fields.Item("Seller_Legal Name").Value.ToString();
                            GenerateIRNGetJson.json_data.SellerDtls.Addr1 = sellerAddress1;
                            GenerateIRNGetJson.json_data.SellerDtls.Addr2 = sellerAddress2;
                            GenerateIRNGetJson.json_data.SellerDtls.Loc = invrecordset.Fields.Item("CompnyCity").Value.ToString();
                            GenerateIRNGetJson.json_data.SellerDtls.Pin = invrecordset.Fields.Item("CompnyPincode").Value.ToString();
                            GenerateIRNGetJson.json_data.SellerDtls.Stcd = invrecordset.Fields.Item("Compnystatecode").Value.ToString();
                        }
                     
                        if (String.IsNullOrEmpty(clsModule.EwayNo))
                        {
                            if (!(string.IsNullOrEmpty(invrecordset.Fields.Item("FrmGSTN").Value.ToString()) || string.IsNullOrEmpty(invrecordset.Fields.Item("FrmAddres1").Value.ToString())))
                            {
                                if (!(invrecordset.Fields.Item("TransType").Value.ToString() == "1" || invrecordset.Fields.Item("TransType").Value.ToString() == "2"))
                                {

                                    string FRMAddress1 = "";
                                    string FRMAddress2 = "";

                                    string FRMconcatAddress = string.Concat(invrecordset.Fields.Item("FrmAddres1").Value.ToString(),
                                                                        invrecordset.Fields.Item("FrmAddres2").Value.ToString());
                                    List<string> FRMsubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(FRMconcatAddress, 70);

                                    foreach (string substring in FRMsubstrings)
                                    {
                                        if (string.IsNullOrEmpty(FRMAddress1))
                                        {
                                            FRMAddress1 = substring;
                                            continue;
                                        }
                                        if (string.IsNullOrEmpty(FRMAddress2))
                                        {
                                            FRMAddress2 = substring;
                                            continue;
                                        }
                                    }

                                    if (invrecordset.Fields.Item("DisEway").Value.ToString() == "Y")
                                    {

                                        GenerateIRNGetJson.json_data.DispDtls.Nm = invrecordset.Fields.Item("FrmTraName").Value.ToString();
                                        if (!string.IsNullOrEmpty(invrecordset.Fields.Item("DisName").Value.ToString()))
                                        {
                                            GenerateIRNGetJson.json_data.DispDtls.Nm = invrecordset.Fields.Item("DisName").Value.ToString();
                                        }

                                        GenerateIRNGetJson.json_data.DispDtls.Addr1 = FRMAddress1;
                                        GenerateIRNGetJson.json_data.DispDtls.Addr2 = FRMAddress2;
                                        GenerateIRNGetJson.json_data.DispDtls.Loc = invrecordset.Fields.Item("FrmPlace").Value.ToString();
                                        GenerateIRNGetJson.json_data.DispDtls.Pin = invrecordset.Fields.Item("FrmZipCode").Value.ToString();
                                        GenerateIRNGetJson.json_data.DispDtls.Stcd = invrecordset.Fields.Item("ActFrmStat").Value.ToString();

                                    }
                                    else
                                    {

                                        if (objRs.Fields.Item("U_GetDisAddWare").Value.ToString() == "Y")
                                        {

                                            string Line1 = "";

                                            switch (TransType)
                                            {
                                                case "INV":
                                                    Line1 = "INV1";
                                                    break;
                                                case "CRN":
                                                    Line1 = "RIN1";
                                                    break;
                                                case "WTR":
                                                    Line1 = "WTR1";
                                                    break;
                                                case "DLN":
                                                    Line1 = "DLN1";
                                                    break;
                                                case "PCH":
                                                    Line1 = "PCH1";
                                                    break;
                                                case "RPC":
                                                    Line1 = "RPC1";
                                                    break;
                                                case "DPO":
                                                    Line1 = "DPO1";
                                                    break;
                                                case "DPI":
                                                    Line1 = "DPI1";
                                                    break;
                                            }
                                            string diswarequery = "SELECT \"Building\" ,\"Block\" ,\"Street\" ,\"Address2\" ,\"Address3\" , ";
                                            diswarequery += " \"City\" ,\"ZipCode\" , COALESCE(\"GSTCode\",'96') as \"Statecode\"  ";
                                            diswarequery += "FROM OWHS o ";
                                            diswarequery += " LEFT JOIN OCST st1 on st1.\"Code\"=o.\"State\" and st1.\"Country\"=o.\"Country\" ";
                                            diswarequery += " WHERE \"WhsCode\" IN(SELECT \"WhsCode\"  FROM  "+ Line1 + " WHERE  \"DocEntry\" = '" + DocEntry + "'); ";

                                            disRecset = clsModule.objaddon.objglobalmethods.GetmultipleRS(diswarequery);
                                            if (disRecset.RecordCount > 0)
                                            {
                                                string DisAddress1 = "";
                                                string DisAddress2 = "";
                                                string DisconcatAddress = string.Concat(disRecset.Fields.Item("Building").Value.ToString(), " ",
                                                                                    disRecset.Fields.Item("Block").Value.ToString(), " ",
                                                                                    disRecset.Fields.Item("Street").Value.ToString(), " ",
                                                                                    disRecset.Fields.Item("Address2").Value.ToString(), " ",
                                                                                    disRecset.Fields.Item("Address3").Value.ToString());
                                                List<string> Dissubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(DisconcatAddress, 70);
                                                foreach (string substring in Dissubstrings)
                                                {
                                                    if (string.IsNullOrEmpty(DisAddress1))
                                                    {
                                                        DisAddress1 = substring;
                                                        continue;
                                                    }
                                                    if (string.IsNullOrEmpty(DisAddress2))
                                                    {
                                                        DisAddress2 = substring;
                                                        continue;
                                                    }
                                                }

                                                GenerateIRNGetJson.json_data.DispDtls.Nm = invrecordset.Fields.Item("Seller_Legal Name").Value.ToString();
                                                GenerateIRNGetJson.json_data.DispDtls.Addr1 = DisAddress1;
                                                GenerateIRNGetJson.json_data.DispDtls.Addr2 = DisAddress2;
                                                GenerateIRNGetJson.json_data.DispDtls.Loc = disRecset.Fields.Item("City").Value.ToString();
                                                GenerateIRNGetJson.json_data.DispDtls.Pin = disRecset.Fields.Item("ZipCode").Value.ToString();
                                                GenerateIRNGetJson.json_data.DispDtls.Stcd = disRecset.Fields.Item("Statecode").Value.ToString();
                                            }

                                        }
                                        else
                                        {
                                            GenerateIRNGetJson.json_data.DispDtls.Nm = invrecordset.Fields.Item("Seller_Legal Name").Value.ToString();
                                            GenerateIRNGetJson.json_data.DispDtls.Addr1 = sellerAddress1;
                                            GenerateIRNGetJson.json_data.DispDtls.Addr2 = sellerAddress2;
                                            GenerateIRNGetJson.json_data.DispDtls.Loc = invrecordset.Fields.Item("CompnyCity").Value.ToString();
                                            GenerateIRNGetJson.json_data.DispDtls.Pin = invrecordset.Fields.Item("CompnyPincode").Value.ToString();
                                            GenerateIRNGetJson.json_data.DispDtls.Stcd = invrecordset.Fields.Item("Compnystatecode").Value.ToString();
                                        }

                                    }
                                }


                                    
                            }
                        }
                        #endregion

                        #region"Buyer"
                        string BuyerAddress1 = "";
                        string BuyerAddress2 = "";
                        string BuyerconcatAddress = string.Concat(invrecordset.Fields.Item("Buyer_Building").Value.ToString(), " ",
                                                            invrecordset.Fields.Item("Buyer_Block").Value.ToString(), " ",
                                                            invrecordset.Fields.Item("Buyer_Street").Value.ToString(), " ",
                                                            invrecordset.Fields.Item("Buyer_Address2").Value.ToString(), " ",
                                                            invrecordset.Fields.Item("Buyer_Address3").Value.ToString());
                        List<string> Buyersubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(BuyerconcatAddress, 70);
                        foreach (string substring in Buyersubstrings)
                        {
                            if (string.IsNullOrEmpty(BuyerAddress1))
                            {
                                BuyerAddress1 = substring;
                                continue;
                            }
                            if (string.IsNullOrEmpty(BuyerAddress2))
                            {
                                BuyerAddress2 = substring;
                                continue;
                            }
                        }

                        GenerateIRNGetJson.json_data.BuyerDtls.Gstin = invrecordset.Fields.Item("Buyer GSTN").Value.ToString();

                        if (!string.IsNullOrEmpty(clsModule.GSTCol))
                        {
                            GenerateIRNGetJson.json_data.BuyerDtls.Gstin = invrecordset.Fields.Item(clsModule.GSTCol).Value.ToString();
                        }
                        GenerateIRNGetJson.json_data.BuyerDtls.LglNm = invrecordset.Fields.Item("Buyer_Legal Name").Value.ToString();                        
                        GenerateIRNGetJson.json_data.BuyerDtls.Addr1 = BuyerAddress1;
                        GenerateIRNGetJson.json_data.BuyerDtls.Addr2 = BuyerAddress2;
                        GenerateIRNGetJson.json_data.BuyerDtls.Loc = invrecordset.Fields.Item("BCity").Value.ToString();
                        GenerateIRNGetJson.json_data.BuyerDtls.Pin = invrecordset.Fields.Item("BZipCode").Value.ToString();
                        GenerateIRNGetJson.json_data.BuyerDtls.Stcd = invrecordset.Fields.Item("Bill to State Code").Value.ToString();
                        GenerateIRNGetJson.json_data.BuyerDtls.Pos = invrecordset.Fields.Item("Bill to State Code").Value.ToString();


                        if (objRs.Fields.Item("U_Gettran").Value.ToString() == "Y")
                        {

                            string TBuyerAddress1 = "";
                            string TBuyerAddress2 = "";
                            string TBuyerconcatAddress = string.Concat(invrecordset.Fields.Item("T_Buyer_Building").Value.ToString(),
                                                                invrecordset.Fields.Item("T_Buyer_Block").Value.ToString(),
                                                                invrecordset.Fields.Item("T_Buyer_Street").Value.ToString(),
                                                                invrecordset.Fields.Item("T_Buyer_Address2").Value.ToString(),
                                                                invrecordset.Fields.Item("T_Buyer_Address3").Value.ToString());
                            List<string> TBuyersubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(TBuyerconcatAddress, 70);
                            foreach (string substring in TBuyersubstrings)
                            {
                                if (string.IsNullOrEmpty(TBuyerAddress1))
                                {
                                    TBuyerAddress1 = substring;
                                    continue;
                                }
                                if (string.IsNullOrEmpty(TBuyerAddress2))
                                {
                                    TBuyerAddress2 = substring;
                                    continue;
                                }
                            }
                                                                                  
                            GenerateIRNGetJson.json_data.BuyerDtls.Addr1 = TBuyerAddress1;
                            GenerateIRNGetJson.json_data.BuyerDtls.Addr2 = TBuyerAddress2;
                            GenerateIRNGetJson.json_data.BuyerDtls.Loc = invrecordset.Fields.Item("T_BCity").Value.ToString();
                            GenerateIRNGetJson.json_data.BuyerDtls.Pin = invrecordset.Fields.Item("T_BZipCode").Value.ToString();
                            GenerateIRNGetJson.json_data.BuyerDtls.Stcd = invrecordset.Fields.Item("T_Bill to State Code").Value.ToString();
                            GenerateIRNGetJson.json_data.BuyerDtls.Pos = invrecordset.Fields.Item("T_Bill to State Code").Value.ToString();
                        }



                        
                        if (String.IsNullOrEmpty(clsModule.EwayNo))
                        {
                            if (!(string.IsNullOrEmpty(invrecordset.Fields.Item("ToGSTN").Value.ToString()) || string.IsNullOrEmpty(invrecordset.Fields.Item("ToAddres1").Value.ToString())))
                            {
                                if (!(invrecordset.Fields.Item("TransType").Value.ToString() == "1" || invrecordset.Fields.Item("TransType").Value.ToString() == "3"))
                                {

                                    string TOAddress1 = "";
                                    string TOAddress2 = "";
                                    string TOconcatAddress = string.Concat(invrecordset.Fields.Item("ToAddres1").Value.ToString(),
                                                                        invrecordset.Fields.Item("ToAddres2").Value.ToString());
                                    List<string> TOsubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(TOconcatAddress, 70);
                                    foreach (string substring in TOsubstrings)
                                    {
                                        if (string.IsNullOrEmpty(TOAddress1))
                                        {
                                            TOAddress1 = substring;
                                            continue;
                                        }
                                        if (string.IsNullOrEmpty(TOAddress2))
                                        {
                                            TOAddress2 = substring;
                                            continue;
                                        }
                                    }

                                    GenerateIRNGetJson.json_data.ShipDtls.Gstin = invrecordset.Fields.Item("ToGSTN").Value.ToString();
                                    GenerateIRNGetJson.json_data.ShipDtls.LglNm = invrecordset.Fields.Item("ToTraName").Value.ToString();

                                    if (objRs.Fields.Item("U_ShipToInvName").Value.ToString() == "Y")
                                    {
                                        GenerateIRNGetJson.json_data.ShipDtls.LglNm = invrecordset.Fields.Item("ShipToCode").Value.ToString();
                                    }

                                    GenerateIRNGetJson.json_data.ShipDtls.Addr1 = TOAddress1;
                                    GenerateIRNGetJson.json_data.ShipDtls.Addr2 = TOAddress2;
                                    GenerateIRNGetJson.json_data.ShipDtls.Loc = invrecordset.Fields.Item("ToPlace").Value.ToString();
                                    GenerateIRNGetJson.json_data.ShipDtls.Pin = invrecordset.Fields.Item("ToZipCode").Value.ToString();
                                    GenerateIRNGetJson.json_data.ShipDtls.Stcd = invrecordset.Fields.Item("ActToState").Value.ToString();



                                    if (objRs.Fields.Item("U_GettrnShp").Value.ToString() == "Y")
                                    {
                                        string TshipAddress1 = "";
                                        string TshipAddress2 = "";
                                        string TBuyerconcatAddress = string.Concat(invrecordset.Fields.Item("T_Ship_Building").Value.ToString(),
                                                                            invrecordset.Fields.Item("T_Ship_Block").Value.ToString(),
                                                                            invrecordset.Fields.Item("T_Ship_Street").Value.ToString(),
                                                                            invrecordset.Fields.Item("T_Ship_Address2").Value.ToString(),
                                                                            invrecordset.Fields.Item("T_Ship_Address3").Value.ToString());
                                        List<string> Tshipsubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(TBuyerconcatAddress, 70);
                                        foreach (string substring in Tshipsubstrings)
                                        {
                                            if (string.IsNullOrEmpty(TshipAddress1))
                                            {
                                                TshipAddress1 = substring;
                                                continue;
                                            }
                                            if (string.IsNullOrEmpty(TshipAddress2))
                                            {
                                                TshipAddress2 = substring;
                                                continue;
                                            }
                                        }
                                        GenerateIRNGetJson.json_data.ShipDtls.Gstin = invrecordset.Fields.Item("Shipto GSTN").Value.ToString();
                                        GenerateIRNGetJson.json_data.ShipDtls.Addr1 = TshipAddress1;
                                        GenerateIRNGetJson.json_data.ShipDtls.Addr2 = TshipAddress2;
                                        GenerateIRNGetJson.json_data.ShipDtls.Loc = invrecordset.Fields.Item("T_SCity").Value.ToString();
                                        GenerateIRNGetJson.json_data.ShipDtls.Pin = invrecordset.Fields.Item("T_SZipCode").Value.ToString();
                                        GenerateIRNGetJson.json_data.ShipDtls.Stcd = invrecordset.Fields.Item("T_Shipp to State Code").Value.ToString();
                                        
                                    }



                                }
                            }
                        }

                        #endregion


                        for (int i = 0; i < invrecordset.RecordCount; i++)
                        {
                            string prdouctdesc = "";
                            if (!string.IsNullOrEmpty(clsModule.ItemDsc))
                            {                                
                                prdouctdesc = invrecordset.Fields.Item(clsModule.ItemDsc).Value.ToString();
                            }
                            if (string.IsNullOrEmpty(prdouctdesc))
                            {
                                prdouctdesc = invrecordset.Fields.Item("Dscription").Value.ToString();
                            }


                            GenerateIRNGetJson.json_data.ItemList.Add(new ItemList
                            {
                                SlNo = invrecordset.Fields.Item("SINo").Value.ToString(),
                                PrdDesc = prdouctdesc,
                                IsServc = Isservice,
                                HsnCd = ServiceHSN,//"9965" for Service Invoice,
                                Qty =invrecordset.Fields.Item("Quantity").Value.ToString(),
                                Discount =invrecordset.Fields.Item("LineDiscountAmt").Value.ToString().StartsWith("-")?"0": Math.Round(Convert.ToDecimal(invrecordset.Fields.Item("LineDiscountAmt").Value),4).ToString(),//LineDisc
                                Unit = AssignEinvunit,
                                UnitPrice =invrecordset.Fields.Item("UnitPrice").Value.ToString(),
                                TotAmt =invrecordset.Fields.Item("Tot Amt").Value.ToString(),
                                AssAmt =invrecordset.Fields.Item("AssAmt").Value.ToString(),//AssAmt
                                GstRt =invrecordset.Fields.Item("GSTRATE").Value.ToString(),
                                TotItemVal =invrecordset.Fields.Item("Total Item Value").Value.ToString(),
                                CgstAmt =invrecordset.Fields.Item("CGSTAmt").Value.ToString(),
                                SgstAmt =invrecordset.Fields.Item("SGSTAmt").Value.ToString(),
                                IgstAmt =invrecordset.Fields.Item("IGSTAmt").Value.ToString(),
                                BchDtls = new BchDtls()
                                {
                                    Nm = invrecordset.Fields.Item("BatchNum").Value.ToString().Length > 3 ? invrecordset.Fields.Item("BatchNum").Value.ToString() : "",
                                    ExpDt = "",
                                    WrDt = ""
                                },
                                AttribDtls = { new AttribDtl() { Nm = "", Val = "" } },

                               ItcClaimType= TransType== "PCH" ? "INPUTSER":"",
                               eligibilityofitc= TransType == "PCH" ? "Y" : "",
                                

                            });
                            invrecordset.MoveNext();
                        }

                        strSQL = GetFrightData(DocEntry,TransType);
                        Freightrecset.DoQuery(strSQL);
                        if (Freightrecset.RecordCount > 0)
                        {
                            for (int i = 0; i < Freightrecset.RecordCount; i++)
                            {
                                int row = GenerateIRNGetJson.json_data.ItemList.Count + 1;
                                GenerateIRNGetJson.json_data.ItemList.Add(new ItemList
                                {
                                    SlNo = Convert.ToString(row),
                                    PrdDesc = Freightrecset.Fields.Item("Dscription").Value.ToString(),
                                    IsServc = "Y",
                                    HsnCd = Freightrecset.Fields.Item("HSN").Value.ToString(),
                                    Qty =Freightrecset.Fields.Item("Quantity").Value.ToString(),
                                    UnitPrice =Freightrecset.Fields.Item("LineTotal").Value.ToString(),
                                    TotAmt =Freightrecset.Fields.Item("LineTotal").Value.ToString(),
                                    AssAmt =Freightrecset.Fields.Item("LineTotal").Value.ToString(),//AssAmt
                                    GstRt =Freightrecset.Fields.Item("VatPrcnt").Value.ToString(),
                                    TotItemVal =Freightrecset.Fields.Item("Total Value").Value.ToString(),
                                    CgstAmt =Freightrecset.Fields.Item("CGSTAmt").Value.ToString(),
                                    SgstAmt =Freightrecset.Fields.Item("SGSTAmt").Value.ToString(),
                                    IgstAmt =Freightrecset.Fields.Item("IGSTAmt").Value.ToString(),
                                    

                                    

                                });
                                Freightrecset.MoveNext();
                            }
                        }


                        invrecordset.MoveFirst();
                        GenerateIRNGetJson.json_data.ValDtls.CgstVal =Convert.ToDecimal(invrecordset.Fields.Item("CGSTVal").Value.ToString());
                        GenerateIRNGetJson.json_data.ValDtls.SgstVal =Convert.ToDecimal(invrecordset.Fields.Item("SGSTVal").Value.ToString());
                        GenerateIRNGetJson.json_data.ValDtls.IgstVal =Convert.ToDecimal(invrecordset.Fields.Item("IGSTVal").Value.ToString());
                        //GenerateIRNGetJson.json_data.ValDtls.Discount =Convert.ToDecimal(invrecordset.Fields.Item("Disc Total").Value.ToString()); no uncommet

                        GenerateIRNGetJson.json_data.ValDtls.AssVal =Convert.ToDecimal(invrecordset.Fields.Item("AssValN").Value.ToString());

                        GenerateIRNGetJson.json_data.ValDtls.OthChrg =Convert.ToDecimal(invrecordset.Fields.Item("OthrAmt").Value.ToString());

                        GenerateIRNGetJson.json_data.ValDtls.TotInvVal =Convert.ToDecimal(invrecordset.Fields.Item("Doc Total").Value.ToString());
                        GenerateIRNGetJson.json_data.ValDtls.RndOffAmt =Convert.ToDecimal(invrecordset.Fields.Item("RoundDif").Value.ToString());

                    


                        GenerateIRNGetJson.json_data.PayDtls.Nm = invrecordset.Fields.Item("CAcctName").Value.ToString();
                        GenerateIRNGetJson.json_data.PayDtls.AccDet = invrecordset.Fields.Item("CAccount").Value.ToString();
                        GenerateIRNGetJson.json_data.PayDtls.FinInsBr = invrecordset.Fields.Item("CIFSCNo").Value.ToString();

                        GenerateIRNGetJson.json_data.RefDtls.InvRm = "";
                        GenerateIRNGetJson.json_data.RefDtls.DocPerdDtls.InvStDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv_Doc_Date").Value.ToString());
                        GenerateIRNGetJson.json_data.RefDtls.DocPerdDtls.InvEndDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv Due Date").Value.ToString());


                        GenerateIRNGetJson.json_data.RefDtls.PrecDocDtls.Add(new PrecDocDtl
                        {
                            InvNo = invrecordset.Fields.Item("Inv_No").Value.ToString(),
                            InvDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv_Doc_Date").Value.ToString()),
                            OthRefNo = ""

                        });
                        GenerateIRNGetJson.json_data.RefDtls.ContrDtls.Add(new ContrDtl
                        {
                            ContrRefr = ""
                        });

                        GenerateIRNGetJson.json_data.AddlDocDtls.Add(new AddlDocDtl
                        {
                            Docs = ""
                        });

                        GenerateIRNGetJson.json_data.ExpDtls.CntCode = invrecordset.Fields.Item("CCode").Value.ToString();
                        GenerateIRNGetJson.json_data.ExpDtls.Port = invrecordset.Fields.Item("PortCode").Value.ToString();
                        GenerateIRNGetJson.json_data.ExpDtls.ShipBNo = invrecordset.Fields.Item("ImpExpNo").Value.ToString();
                        GenerateIRNGetJson.json_data.ExpDtls.ShipBDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("ImpExpDate").Value.ToString());


                        if (String.IsNullOrEmpty(clsModule.EwayNo))
                        {
                            if (!(string.IsNullOrEmpty(invrecordset.Fields.Item("TransID").Value.ToString()) && string.IsNullOrEmpty(invrecordset.Fields.Item("VehicleNo").Value.ToString())))
                            {
                                GenerateIRNGetJson.json_data.ExpDtls.RefClm = invrecordset.Fields.Item("ClaimRefun").Value.ToString();

                                GenerateIRNGetJson.json_data.EwbDtls.Distance = clsModule.objaddon.objglobalmethods.Ctoint(Calcdistance);
                                GenerateIRNGetJson.json_data.EwbDtls.TransDocDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("TransDate").Value.ToString());
                                GenerateIRNGetJson.json_data.EwbDtls.TransDocNo = invrecordset.Fields.Item("TransDocNo").Value.ToString();
                                GenerateIRNGetJson.json_data.EwbDtls.TransId = invrecordset.Fields.Item("TransID").Value.ToString();
                                GenerateIRNGetJson.json_data.EwbDtls.TransMode = invrecordset.Fields.Item("TransMode").Value.ToString() == "-1" || invrecordset.Fields.Item("TransMode").Value.ToString() == "0" || string.IsNullOrEmpty(invrecordset.Fields.Item("TransMode").Value.ToString()) ? "" : invrecordset.Fields.Item("TransMode").Value.ToString();
                                GenerateIRNGetJson.json_data.EwbDtls.TransName = invrecordset.Fields.Item("TransName").Value.ToString();
                                GenerateIRNGetJson.json_data.EwbDtls.VehNo = invrecordset.Fields.Item("VehicleNo").Value.ToString();
                                GenerateIRNGetJson.json_data.EwbDtls.VehType = invrecordset.Fields.Item("VehicleTyp").Value.ToString() == "-1" || string.IsNullOrEmpty(invrecordset.Fields.Item("VehicleTyp").Value.ToString()) ? "" : invrecordset.Fields.Item("VehicleTyp").Value.ToString();

                            }
                        }
                        else
                        {
                            decimal distance = 0;
                            if (GenerateIRNGetJson.json_data.BuyerDtls.Pin.ToString() == GenerateIRNGetJson.json_data.SellerDtls.Pin.ToString())
                            {
                                distance = 1;
                            }

                            if (!string.IsNullOrEmpty(clsModule.EwayDistance))
                            {
                                if (!string.IsNullOrEmpty(invrecordset.Fields.Item(clsModule.EwayDistance).Value.ToString()))
                                {
                                    distance = clsModule.objaddon.objglobalmethods.CtoD(invrecordset.Fields.Item(clsModule.EwayDistance).Value);
                                }
                            }
                            if (!string.IsNullOrEmpty(clsModule.EwayUDF))
                            {
                                if (!string.IsNullOrEmpty(invrecordset.Fields.Item(clsModule.EwayUDF).Value.ToString()))
                                {
                                    GenerateIRNGetJson.json_data.EwbDtls.VehNo = Convert.ToString(invrecordset.Fields.Item(clsModule.EwayUDF).Value);
                                    GenerateIRNGetJson.json_data.EwbDtls.VehType = "R";
                                    GenerateIRNGetJson.json_data.EwbDtls.TransMode = "1";
                                }
                            }
                            if (!string.IsNullOrEmpty(clsModule.EwayTransportId))
                            {
                                if (!string.IsNullOrEmpty(invrecordset.Fields.Item(clsModule.EwayTransportId).Value.ToString()))
                                {
                                    GenerateIRNGetJson.json_data.EwbDtls.TransId = Convert.ToString(invrecordset.Fields.Item(clsModule.EwayTransportId).Value);
                                    GenerateIRNGetJson.json_data.EwbDtls.TransName = Convert.ToString(invrecordset.Fields.Item(clsModule.EwayTransportName).Value);
                                }
                            }
                            GenerateIRNGetJson.json_data.EwbDtls.Distance = distance;
                        }

                    
                        requestParams = JsonConvert.SerializeObject(GenerateIRNGetJson);
                        if (fromGst)
                        {
                            switch (TransType)
                            {
                                case "PCH":
                                case "RPC":
                                case "DPO":
                                    GenerateIRNGetJson.json_data.DocDtls.Typ = invrecordset.Fields.Item("Type").Value.ToString();
                                    if (!string.IsNullOrEmpty(invrecordset.Fields.Item("NumAtCard").Value.ToString()))
                                    {
                                        GenerateIRNGetJson.json_data.DocDtls.No = invrecordset.Fields.Item("NumAtCard").Value.ToString();
                                    }
                                    else
                                    {
                                        GenerateIRNGetJson.json_data.DocDtls.No = invrecordset.Fields.Item("Inv_No").Value.ToString(); 
                                    }
                                    
                                    GenerateIRNGetJson.json_data.DocDtls.Dt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Cust Order Date").Value.ToString());
                                    GenerateIRNGetJson.json_data.DocDtls.ErpInvNo = invrecordset.Fields.Item("Inv_No").Value.ToString();
                                    GenerateIRNGetJson.json_data.DocDtls.AccountingDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv_Doc_Date").Value.ToString());
                                    GenerateIRNGetJson.json_data.TranDtls.IsPurchase = "PURTAX";
                                    GenerateIRNGetJson.json_data.ExpDtls.billofentryno = invrecordset.Fields.Item("ImpExpNo").Value.ToString(); 
                                    GenerateIRNGetJson.json_data.ExpDtls.billofentrydate = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("ImpExpDate").Value.ToString()); 
                                    GenerateIRNGetJson.json_data.Extdtls.returnfilingmonth = clsModule.objaddon.objglobalmethods.DateFormat(clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv_Doc_Date").Value.ToString()), "dd/MM/yyyy", "MMyyyy");



                                    (GenerateIRNGetJson.json_data.SellerDtls.Gstin, GenerateIRNGetJson.json_data.BuyerDtls.Gstin) = (GenerateIRNGetJson.json_data.BuyerDtls.Gstin, GenerateIRNGetJson.json_data.SellerDtls.Gstin);
                                    (GenerateIRNGetJson.json_data.SellerDtls.LglNm, GenerateIRNGetJson.json_data.BuyerDtls.LglNm) = (GenerateIRNGetJson.json_data.BuyerDtls.LglNm, GenerateIRNGetJson.json_data.SellerDtls.LglNm);
                                    (GenerateIRNGetJson.json_data.SellerDtls.Addr1, GenerateIRNGetJson.json_data.BuyerDtls.Addr1) = (GenerateIRNGetJson.json_data.BuyerDtls.Addr1, GenerateIRNGetJson.json_data.SellerDtls.Addr1);
                                    (GenerateIRNGetJson.json_data.SellerDtls.Addr2, GenerateIRNGetJson.json_data.BuyerDtls.Addr2) = (GenerateIRNGetJson.json_data.BuyerDtls.Addr2, GenerateIRNGetJson.json_data.SellerDtls.Addr2);
                                    (GenerateIRNGetJson.json_data.SellerDtls.Loc, GenerateIRNGetJson.json_data.BuyerDtls.Loc) = (GenerateIRNGetJson.json_data.BuyerDtls.Loc, GenerateIRNGetJson.json_data.SellerDtls.Loc);
                                    (GenerateIRNGetJson.json_data.SellerDtls.Pin, GenerateIRNGetJson.json_data.BuyerDtls.Pin) = (GenerateIRNGetJson.json_data.BuyerDtls.Pin, GenerateIRNGetJson.json_data.SellerDtls.Pin);
                                    (GenerateIRNGetJson.json_data.SellerDtls.Stcd, GenerateIRNGetJson.json_data.BuyerDtls.Stcd) = (GenerateIRNGetJson.json_data.BuyerDtls.Stcd, GenerateIRNGetJson.json_data.SellerDtls.Stcd);
                                    GenerateIRNGetJson.json_data.SellerDtls.Stcd= GenerateIRNGetJson.json_data.SellerDtls.Stcd == "0" ? null : GenerateIRNGetJson.json_data.SellerDtls.Stcd;
                                    GenerateIRNGetJson.json_data.BuyerDtls.Pos= GenerateIRNGetJson.json_data.BuyerDtls.Stcd == "0" ? null : GenerateIRNGetJson.json_data.BuyerDtls.Stcd; 
                                    
                                   

                                    switch (GenerateIRNGetJson.json_data.TranDtls.SupTyp)
                                    {
                                        case "B2B":
                                            if (GenerateIRNGetJson.json_data.SellerDtls.Gstin.Length == 15 && GenerateIRNGetJson.json_data.BuyerDtls.Gstin.Length == 15)
                                            {
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "B2B";
                                            }                                           
                                            else
                                            {                                              
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "B2C";
                                                GenerateIRNGetJson.json_data.TranDtls.RegRev = "Y";
                                            }
                                            break;
                                        case "EXPWP":                                          
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "IMPG";                                          
                                            break;
                                        case "SEZWP":
                                            if (GenerateIRNGetJson.json_data.ValDtls.AssVal == GenerateIRNGetJson.json_data.ValDtls.TotInvVal)
                                            {
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "IMPGSEZWOP";
                                            }
                                            else
                                            {
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "IMPGSEZWP";
                                            }
                                            break;

                                    }
                                    break;
                                case "INV":                                    
                                case "CRN":                                    
                                case "DPI":                                    
                                   switch (GenerateIRNGetJson.json_data.TranDtls.SupTyp)
                                    {
                                        case "B2B":
                                            if (GenerateIRNGetJson.json_data.SellerDtls.Gstin.Length == 15 && GenerateIRNGetJson.json_data.BuyerDtls.Gstin.Length == 15)
                                            {
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "R";
                                            }                                                                                     
                                            else if (GenerateIRNGetJson.json_data.ValDtls.TotInvVal == 0)
                                            {
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "NILAMT";
                                            }
                                            else
                                            {
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "B2C";                                                
                                            }
                                            break;
                                        case "EXPWP":
                                             if (GenerateIRNGetJson.json_data.ValDtls.AssVal == GenerateIRNGetJson.json_data.ValDtls.TotInvVal)
                                            {
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "WOPAY";
                                            }
                                             else
                                            {
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "WPAY";
                                            }

                                            break;
                                        case "SEZWP":
                                            if (GenerateIRNGetJson.json_data.ValDtls.AssVal == GenerateIRNGetJson.json_data.ValDtls.TotInvVal)
                                            {
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "SEWOP";
                                            }
                                            else
                                            {
                                                GenerateIRNGetJson.json_data.TranDtls.SupTyp = "SEWP";
                                            }
                                            break;

                                    }
                                    break;
                            }

                           

                            GenerateIRNGetJson.json_data.Extdtls.irnnumber = invrecordset.Fields.Item("U_IRNNo").Value.ToString();
                            GenerateIRNGetJson.json_data.Extdtls.irndate = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("U_AckDate").Value.ToString());
                            GenerateIRNGetJson.json_data.EwbDtls.EwayBillNo = invrecordset.Fields.Item("EwayNo").Value.ToString();
                            GenerateIRNGetJson.json_data.EwbDtls.EwayBillDate = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("EwbDate").Value.ToString());

                            GenerateIRNGetJson.json_data.SellerDtls.Gstin = GenerateIRNGetJson.json_data.SellerDtls.Gstin == "" ? "URP" : GenerateIRNGetJson.json_data.SellerDtls.Gstin;
                            GenerateIRNGetJson.json_data.BuyerDtls.Gstin = GenerateIRNGetJson.json_data.BuyerDtls.Gstin == "" ? "URP" : GenerateIRNGetJson.json_data.BuyerDtls.Gstin;

                            jsonstring = GSTSales(GenerateIRNGetJson.json_data);

                            jsonstring = "[" + jsonstring + "]";


                            jsonstring = "["+ JsonConvert.SerializeObject(GenerateIRNGetJson.json_data) +"]";

                            return true;
                        }
                        datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                        E_Invoice_Logs(DocEntry, datatable, TransType, "Create", Type, requestParams);

                        string msg =columnFind(datatable,"message",0);
                        msg += columnFind(datatable, "error", 0);
                        msg += columnFind(datatable, "error_log_ids", 0);

                        if (string.IsNullOrEmpty(msg))
                        {
                            
                          
                            clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_None);

                        }
                        else
                        {
                            if (!blnRefresh)
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                clsModule.objaddon.objapplication.MessageBox("Generate: " + msg);
                            }
                        }
                    }
                    else
                    {
                        clsModule.objaddon.objapplication.StatusBar.SetText("No data found for this invoice...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_None);
                    }
                    GenerateIRNGetJson = null;

                    
                }
                else if (Create_Cancel == EinvoiceMethod.CreateEway)
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Generating E-Way. Please wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    Generate_EWay GenerateIRNGetJson = new Generate_EWay();
                    strSQL = GetInvoiceData(DocEntry, TransType);
                    invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    invrecordset.DoQuery(strSQL);

                    DataTable dt = new DataTable();
                    dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);
                    if (invrecordset.RecordCount > 0)
                    {
                        decimal Calcdistance = 0;
                        strSQL = @"Select T1.""LineId"",T0.""U_UATUrl"",T1.""U_URLType"",T1.""U_Type"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                        strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                        strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01' and T1.""U_URLType"" ='Generate IRN' and T1.""U_Type""='E-Way' Order by ""LineId"" Desc";
                        objRs.DoQuery(strSQL);
                        if (objRs.RecordCount == 0)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Create IRN\". Please up  in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }

                        if (string.IsNullOrEmpty(invrecordset.Fields.Item("FrmGSTN").Value.ToString()) && !fromGst)
                        {
                                clsModule.objaddon.objapplication.StatusBar.SetText("GST No is Missing for \"Create GSTNo\"...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                return false;
                        }

                        Generate_EWay distanceEway = new Generate_EWay();
                        distanceEway.CLIENTCODE = ClientCode;
                        distanceEway.USERCODE = UserCode;
                        distanceEway.PASSWORD = Password;
                        distanceEway.frompincode = invrecordset.Fields.Item("FrmZipCode").Value.ToString();
                        distanceEway.topincode = invrecordset.Fields.Item("ToZipCode").Value.ToString();
                        if (string.IsNullOrEmpty(invrecordset.Fields.Item("FrmZipCode").Value.ToString()) || string.IsNullOrEmpty(invrecordset.Fields.Item("ToZipCode").Value.ToString()))
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Zip Code Missing ... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }
                        DataTable deway = Get_API_Response(JsonConvert.SerializeObject(distanceEway), objRs.Fields.Item("BaseURL").Value.ToString() + "/TransactionAPI/GetPincodeDistance");


                        if (invrecordset.Fields.Item("FrmZipCode").Value.ToString() == invrecordset.Fields.Item("ToZipCode").Value.ToString())
                        {
                            Calcdistance = 1;
                        }
                    
                        Calcdistance = clsModule.objaddon.objglobalmethods.CtoD(invrecordset.Fields.Item("Distance").Value);

                        string AssignEwayunit = invrecordset.Fields.Item("Unit").Value.ToString();

                        strSQL = "SELECT \"U_GUnitCod\"  FROM \"@UOMMAP\" u WHERE u.\"U_UOMCod\" ='" + AssignEwayunit + "'";
                        DataTable dt1 = new DataTable();
                        dt1 = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);
                        if (dt1.Rows.Count > 0)
                        {
                            AssignEwayunit = dt1.Rows[0]["U_GUnitCod"].ToString();
                        }
                        else
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Unit(" + AssignEwayunit + ") Not Mapped please Map Unit... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }

                        GenerateIRNGetJson.CLIENTCODE = ClientCode;
                        GenerateIRNGetJson.USERCODE = UserCode;
                        GenerateIRNGetJson.PASSWORD = Password;
                        GenerateIRNGetJson.version = "1.1";

                        string FRMAddress1 = "";
                        string FRMAddress2 = "";

                        string FRMconcatAddress = string.Concat(invrecordset.Fields.Item("FrmAddres1").Value.ToString(),
                                                            invrecordset.Fields.Item("FrmAddres2").Value.ToString());
                        List<string> FRMsubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(FRMconcatAddress, 70);

                        foreach (string substring in FRMsubstrings)
                        {
                            if (string.IsNullOrEmpty(FRMAddress1))
                            {
                                FRMAddress1 = substring;
                                continue;
                            }
                            if (string.IsNullOrEmpty(FRMAddress2))
                            {
                                FRMAddress2 = substring;
                                continue;
                            }
                        }

                        string TOAddress1 = "";
                        string TOAddress2 = "";
                        string TOconcatAddress = string.Concat(invrecordset.Fields.Item("ToAddres1").Value.ToString(),
                                                            invrecordset.Fields.Item("ToAddres2").Value.ToString());
                        List<string> TOsubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(TOconcatAddress, 70);
                        foreach (string substring in TOsubstrings)
                        {
                            if (string.IsNullOrEmpty(TOAddress1))
                            {
                                TOAddress1 = substring;
                                continue;
                            }
                            if (string.IsNullOrEmpty(TOAddress2))
                            {
                                TOAddress2 = substring;
                                continue;
                            }
                        }



                        GenerateIRNGetJson.billLists.Add(new Generate_EWay.EwayList
                        {
                            userGstin = invrecordset.Fields.Item("Seller GSTN").Value.ToString(),
                            supplyType = invrecordset.Fields.Item("SuplyType").Value.ToString(),
                            subSupplyType = invrecordset.Fields.Item("SubSplyTyp").Value.ToString(),
                            subSupplyTypeDesc = invrecordset.Fields.Item("SubtypeDescription").Value.ToString(),
                            docType = invrecordset.Fields.Item("EDocType").Value.ToString(),
                            docNo = invrecordset.Fields.Item("Inv_No").Value.ToString(),
                            docDate = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv_Doc_Date").Value.ToString()),
                            TransType = invrecordset.Fields.Item("TransType").Value.ToString(),
                            fromGstin = invrecordset.Fields.Item("FrmGSTN").Value.ToString(),
                            fromTrdName = invrecordset.Fields.Item("FrmTraName").Value.ToString(),



                            fromAddr1 = FRMAddress1,
                            fromAddr2 = FRMAddress2,
                            fromPlace = invrecordset.Fields.Item("FrmPlace").Value.ToString(),
                            fromPincode = invrecordset.Fields.Item("FrmZipCode").Value.ToString(),
                            fromStateCode = invrecordset.Fields.Item("FrmState").Value.ToString(),
                            actualFromStateCode = invrecordset.Fields.Item("ActFrmStat").Value.ToString(),
                            toGstin = invrecordset.Fields.Item("ToGSTN").Value.ToString(),
                            toTrdName = invrecordset.Fields.Item("ToTraName").Value.ToString(),
                            toAddr1 = TOAddress1,
                            toAddr2 = TOAddress2,
                            toPlace = invrecordset.Fields.Item("ToPlace").Value.ToString(),
                            toPincode = invrecordset.Fields.Item("ToZipCode").Value.ToString(),
                            actualToStateCode = invrecordset.Fields.Item("ActToState").Value.ToString(),
                            toStateCode = invrecordset.Fields.Item("ToState").Value.ToString(),
                            totalValue = invrecordset.Fields.Item("AssValN").Value.ToString(),
                            cgstValue = invrecordset.Fields.Item("CGSTVal").Value.ToString(),
                            sgstValue = invrecordset.Fields.Item("SGSTVal").Value.ToString(),
                            igstValue = invrecordset.Fields.Item("IGSTVal").Value.ToString(),
                            transDocNo = invrecordset.Fields.Item("TransDocNo").Value.ToString(),
                            transDocDate = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("TransDate").Value.ToString()),
                            totInvValue = invrecordset.Fields.Item("Doc Total").Value.ToString(),
                            transporterId = invrecordset.Fields.Item("TransID").Value.ToString(),
                            transporterName = invrecordset.Fields.Item("TransName").Value.ToString(),
                            transMode = invrecordset.Fields.Item("TransMode").Value.ToString(),
                            transDistance = Calcdistance.ToString(),
                            vehicleNo = invrecordset.Fields.Item("VehicleNo").Value.ToString(),
                            vehicleType = invrecordset.Fields.Item("VehicleTyp").Value.ToString(),
                            shipToGSTIN = invrecordset.Fields.Item("ToGSTN").Value.ToString(),
                            dispatchFromGSTIN = invrecordset.Fields.Item("FrmGSTN").Value.ToString(),
                            dispatchFromTradeName = invrecordset.Fields.Item("FrmTraName").Value.ToString(),
                            portPin = invrecordset.Fields.Item("PortCode").Value.ToString()
                    });


                        for (int i = 0; i < invrecordset.RecordCount; i++)
                        {
                            string prdouctdesc = "";
                            if (!string.IsNullOrEmpty(clsModule.ItemDsc))
                            {
                                prdouctdesc = invrecordset.Fields.Item(clsModule.ItemDsc).Value.ToString();
                            }
                            if (string.IsNullOrEmpty(prdouctdesc))
                            {
                                prdouctdesc = invrecordset.Fields.Item("Dscription").Value.ToString();
                            }

                            GenerateIRNGetJson.billLists[0].itemList.Add(new Generate_EWay.Ewayitemlist
                            {
                                itemNo = clsModule.objaddon.objglobalmethods.Ctoint(invrecordset.Fields.Item("SINo").Value),
                                productName = prdouctdesc,
                                productDesc = prdouctdesc,
                                hsnCode = invrecordset.Fields.Item("HSN").Value.ToString(),//"9965" for Service Invoice,
                                quantity = invrecordset.Fields.Item("Quantity").Value.ToString(),
                                qtyUnit = AssignEwayunit,
                                taxableAmount = invrecordset.Fields.Item("AssAmt").Value.ToString(),
                                sgstRate = (Convert.ToDecimal(invrecordset.Fields.Item("IGSTVal").Value.ToString()) == 0) ? Convert.ToDecimal(invrecordset.Fields.Item("GSTRATE").Value.ToString()) / 2 : 0,
                                cgstRate = (Convert.ToDecimal(invrecordset.Fields.Item("IGSTVal").Value.ToString()) == 0) ? Convert.ToDecimal(invrecordset.Fields.Item("GSTRATE").Value.ToString()) / 2 : 0,
                                igstRate = (Convert.ToDecimal(invrecordset.Fields.Item("IGSTVal").Value.ToString()) == 0) ? 0 : Convert.ToDecimal(invrecordset.Fields.Item("GSTRATE").Value.ToString()),
                            });
                            invrecordset.MoveNext();
                        }

                        requestParams = JsonConvert.SerializeObject(GenerateIRNGetJson);
                        if (fromGst)
                        {
                            jsonstring = requestParams;
                            return true;
                        }
                        datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());

                        E_Invoice_Logs(DocEntry, datatable, TransType, "Create", Type, requestParams);

                        string msg = datatable.Rows[0]["message"].ToString();
                        if (datatable.Rows[0]["error_log_id"].ToString() == string.Empty)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        }
                        else
                        {
                            if (!blnRefresh)
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                clsModule.objaddon.objapplication.MessageBox("Generate: " + msg);
                            }
                        }
                    }
                    else
                    {
                        clsModule.objaddon.objapplication.StatusBar.SetText("No data found for this invoice...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }


                    GenerateIRNGetJson = null;

                }
                else if (Create_Cancel == EinvoiceMethod.CancelIRN)
                {

                    if (!(clsModule.objaddon.objapplication.MessageBox("Do you want Canel IRN..? ",2,"Yes","No")==1))
                     {
                        return true;
                    }

                    clsModule.objaddon.objapplication.StatusBar.SetText("Cancelling E-Invoice. Please wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    ClientCred_Cancel ClientCred = new ClientCred_Cancel();
                    {
                        strSQL = @"Select T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",Case when T0.""U_Live""='N' " +
                                  @"then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                        strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                        strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'" +
                                  @"and T1.""U_URLType"" ='Cancel IRN' and T1.""U_Type""='E-Invoice' Order by ""LineId"" Desc";

                        objRs.DoQuery(strSQL);
                        if (objRs.RecordCount == 0)
                        {
                            SapMessage = "API is Missing for Cancel IRN. Please update in general settings... ";
                            clsModule.objaddon.objapplication.StatusBar.SetText(SapMessage, SAPbouiCOM.BoMessageTime.bmt_Long);
                            return false;
                        }

                        if (objRs.RecordCount > 0)
                        {
                            ClientCred.CLIENTCODE = ClientCode;
                            ClientCred.USERCODE = UserCode;
                            ClientCred.PASSWORD = Password;
                        }

                        switch (TransType)
                        {
                            case "INV":
                                strSQL = @"SELECT o.""U_IRNNo""  FROM OINV o  where o.""DocEntry"" ='" + DocEntry + "'";
                                break;
                            case "CRN":
                                strSQL = @"SELECT o.""U_IRNNo""  FROM ORIN o where o.""DocEntry"" ='" + DocEntry + "' ";
                                break;
                        }
                        string IRN = clsModule.objaddon.objglobalmethods.getSingleValue(strSQL);
                        ClientCred.cancelledeinvoicelist.Add(new Cancelledeinvoicelist
                        {
                            Irn = IRN,
                            CnlRem = "Cancelling against DocNum",//docnum include
                            CnlRsn = 4

                        });
                    }
                    requestParams = JsonConvert.SerializeObject(ClientCred);
                    if (fromGst)
                    {
                        jsonstring = requestParams;
                        return true;
                    }
                    datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                    E_Invoice_Logs(DocEntry, datatable, TransType, "Cancel", Type, requestParams);
                    string Emsg = datatable.Rows[0]["message"].ToString();
                    if (datatable.Rows[0]["error_log_id"].ToString() == string.Empty)
                    {
                        clsModule.objaddon.objapplication.StatusBar.SetText("Cancel_IRN: " + Emsg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    }
                    else
                    {
                        if (!blnRefresh)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Cancel_IRN: " + Emsg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            clsModule.objaddon.objapplication.MessageBox("Cancel_IRN: " + Emsg);
                        }
                    }
                    ClientCred = null;

                }
                else if (Create_Cancel == EinvoiceMethod.CancelEway || Create_Cancel == EinvoiceMethod.CancelEwayIRN)
                {

                    if (!(clsModule.objaddon.objapplication.MessageBox("Do you want Canel Eway..? ", 2, "Yes", "No") == 1))
                    {
                        return true;
                    }


                    clsModule.objaddon.objapplication.StatusBar.SetText("Cancelling E-Way Please wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    ClientCred_Cancel ClientCred = new ClientCred_Cancel();
                    {
                        string method = Create_Cancel == EinvoiceMethod.CancelEway ? "Cancel IRN" : "Cancel Eway By IRN";

                        strSQL = @"Select T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",Case when T0.""U_Live""='N' " +
                                     @"then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                        strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                        strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'" +
                                  @"and T1.""U_URLType"" ='"+ method + @"' and T1.""U_Type""='E-Way' Order by ""LineId"" Desc";

                       
                       
                        objRs.DoQuery(strSQL);
                        if (objRs.RecordCount == 0)
                        {
                            SapMessage = "API is Missing for "+ method + ". Please update in general settings... ";
                            clsModule.objaddon.objapplication.StatusBar.SetText(SapMessage, SAPbouiCOM.BoMessageTime.bmt_Long);
                            return false;
                        }


                        if (objRs.RecordCount > 0)
                        {
                            ClientCred.CLIENTCODE = ClientCode;
                            ClientCred.USERCODE = UserCode;
                            ClientCred.PASSWORD = Password;
                        }

                        if (String.IsNullOrEmpty(clsModule.EwayNo))
                        {
                            switch (TransType)
                            {

                                case "INV":
                                    strSQL = @"SELECT i.""EWayBillNo""  FROM OINV o  inner JOIN INV26 i  ON o.""DocEntry"" =i.""DocEntry"" WHERE o.""DocEntry""='" + DocEntry + "'";
                                    break;
                                case "CRN":

                                    strSQL = @"SELECT i.""EWayBillNo"" FROM ORIN o JOIN RIN26 i ON o.""DocEntry"" =i.""DocEntry"" WHERE o.""DocEntry""='" + DocEntry + "'";
                                    break;

                                case "WTR":

                                    strSQL = @"SELECT i.""EWayBillNo"" FROM OWTR o JOIN WTR26 i ON o.""DocEntry"" =i.""DocEntry"" WHERE o.""DocEntry""='" + DocEntry + "'";
                                    break;
                                case "DLN":

                                    strSQL = @"SELECT i.""EWayBillNo"" FROM ODLN o JOIN DLN26 i ON o.""DocEntry"" =i.""DocEntry"" WHERE o.""DocEntry""='" + DocEntry + "'";
                                    break;

                            }
                        }
                        else
                        {
                            switch (TransType)
                            {

                                case "INV":
                                    strSQL = @"SELECT """ + clsModule.EwayNo + @""" FROM OINV o  WHERE o.""DocEntry""='" + DocEntry + "'";
                                    break;
                                case "CRN":
                                    strSQL = @"SELECT """ + clsModule.EwayNo + @"""  FROM ORIN o WHERE o.""DocEntry""='" + DocEntry + "'";
                                    break;
                                case "WTR":
                                    strSQL = @"SELECT """ + clsModule.EwayNo + @"""  FROM OWTR o WHERE o.""DocEntry""='" + DocEntry + "'";
                                    break;
                                case "DLN":
                                    strSQL = @"SELECT """ + clsModule.EwayNo + @"""  FROM ODLN o WHERE o.""DocEntry""='" + DocEntry + "'";
                                    break;
                            }
                        }

                        string EwbNo = clsModule.objaddon.objglobalmethods.getSingleValue(strSQL);

                        if (Create_Cancel == EinvoiceMethod.CancelEway) {
                            ClientCred.cancelledewblist.Add(new cancelledewblist
                            {
                                EwbNo = EwbNo,
                                CancelledReason = "2",
                                CancelledRemarks = "OK"
                            });
                        }
                        else if (Create_Cancel == EinvoiceMethod.CancelEwayIRN) {
                            ClientCred.cancelledeinvoiceewblist.Add(new cancelledeinvoiceewblist
                            {
                                ewbNo = EwbNo,
                                cancelRsnCode = "2",
                                cancelRmrk = "OK"
                            });
                        } 
                        
                    }
                    requestParams = JsonConvert.SerializeObject(ClientCred);
                    if (fromGst)
                    {
                        jsonstring = requestParams;
                        return true;
                    }
                    datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                    E_Invoice_Logs(DocEntry, datatable, TransType, "Cancel", Type, requestParams);

                    string Emsg = datatable.Rows[0]["message"].ToString();
                    if (datatable.Rows[0]["error_log_id"].ToString() == string.Empty)
                    {
                        clsModule.objaddon.objapplication.StatusBar.SetText("Cancel_Eay: " + Emsg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    }
                    else
                    {
                        if (!blnRefresh)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Cancel_Eway: " + Emsg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            clsModule.objaddon.objapplication.MessageBox("Cancel_Eway: " + Emsg);
                        }
                    }
                    ClientCred = null;

                }
                else if (Create_Cancel == EinvoiceMethod.GetEwayByIRN)
                {
                    string Docseries;
                    clsModule.objaddon.objapplication.StatusBar.SetText("Generating E-Way by IRN. Please wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                    strSQL = @"Select T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",T0.""U_UATUrl"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                    strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL,T0.""U_SERCONFIG"",T0.""U_GetDisAddWare"",""U_ShipToInvName"",""U_BillToWare"",""U_GettrnShp"" ";
                    strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'  and T1.""U_URLType"" ='Generate Eway by IRN' and T1.""U_Type""='E-Way' Order by ""LineId"" Desc";
                    objRs.DoQuery(strSQL);
                    if (objRs.RecordCount == 0) { clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Generate Eway by IRN\". Please update in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error); return false; }
                    Docseries = "CONCAT(CAST(COALESCE( nnm1.\"BeginStr\",'') AS varchar(100)), CAST(a.\"DocNum\" AS varchar(100)))";

                    GetEwayByIRN clienCred_GetIRN_DocNum = new GetEwayByIRN();
                    if (objRs.RecordCount > 0)
                    {
                        clienCred_GetIRN_DocNum.CLIENTCODE = ClientCode;
                        clienCred_GetIRN_DocNum.USERCODE = UserCode;
                        clienCred_GetIRN_DocNum.PASSWORD = Password;
                        Docseries = objRs.Fields.Item("U_SERCONFIG").Value.ToString();
                        invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        string tb = "";
                        string eway = "";
                        string Linetb = "";
                        string Line12 = "";
                        switch (TransType)
                        {
                            case "INV":
                                tb = "OINV";
                                eway = "inv26";
                                Linetb = "inv1";
                                Line12 = "inv12";
                                break;
                            case "CRN":
                                tb = "ORIN";
                                eway = "rin26";
                                Linetb = "rin1";
                                Line12 = "rin12";
                                break;
                        }
                        if (String.IsNullOrEmpty(clsModule.EwayNo))
                        {
                            strSQL = @"SELECT t1.""U_IRNNo"" ,t2.""Distance"" ,t2.""TransMode"" ,t2.""TransID"" ,t2.""TransName"" ,t2.""TransDocNo"" ,t2.""TransDate"",t2.""FrmZipCode"",t2.""ToZipCode"",";

                            strSQL += "  t2.\"FrmGSTN\",t2.\"FrmAddres1\",t2.\"FrmAddres2\" ,t2.\"FrmPlace\" ,Replace(t2.\"FrmZipCode\",' ','') as \"FrmZipCode\" , t2.\"ActFrmStat\", ";
                            strSQL += " t2.\"ToGSTN\",t2.\"ToAddres1\" ,t2.\"ToAddres2\" ,t2.\"ToPlace\" ,Replace(t2.\"ToZipCode\",' ','') as \"FrmZipCode\" , t2.\"ActToState\", ";

                            strSQL += " t2.\"TransType\",t2.\"FrmTraName\",t2.\"ToTraName\", ";                       
                            strSQL += " t2.\"U_Dispatch_Eway\" as \"DisEway\" ,";
                            strSQL += " t2.\"U_Dispatch_Name\" as \"DisName\" ,";
                            strSQL += " t1.\"ShipToCode\" as \"ShipToCode\" ,";
                            

                            strSQL += "  B1.\"CompnyAddr\",B1.\"CompnyName\" \"Seller_Legal Name\", B3.\"City\" \"CompnyCity\", Replace(B3.\"ZipCode\",' ','') \"CompnyPincode\", ";
                            strSQL += " (select COALESCE(\"GSTCode\",'96') from OCST where \"Country\"=B3.\"Country\" and \"Code\"=B3.\"State\") \"Compnystatecode\", ";


                            strSQL += @"t2.""VehicleNo"" ,t2.""VehicleTyp""  FROM " + tb + @" t1 LEFT JOIN " + eway + @" t2 ON t1.""DocEntry"" =t2.""DocEntry"" ";
                            strSQL = strSQL + " CROSS JOIN OADM B1";
                            strSQL = strSQL + " CROSS JOIN ADM1 B3";
                            strSQL += @" where t1.""DocEntry""='" + DocEntry + @"'";
                            strSQL += @" and (t2.""TransID"" <>'' or t2.""VehicleNo"" <>'')";

                        }
                        else
                        {
                            strSQL = @"SELECT a.""U_IRNNo"" ,'' ""Distance"" ,Replace(ss.""ZipCode"",' ','') ""FrmZipCode"",";
                            strSQL += @"Replace(CRD1.""ZipCode"",' ','') ""ToZipCode"",'' ""TransMode"" ,a.""" + clsModule.EwayTransportId + @""" ,a.""" + clsModule.EwayTransportName + @""",a.""" + clsModule.EwayDistance + @"""  ,'' ""TransDate"",";
                            strSQL += "'' \"TransDocNo\"";
                            strSQL += @" ,a.""" + clsModule.EwayUDF + @""" ,'' ""VehicleTyp""  FROM " + tb + @" a  ";
                            strSQL += @" INNER JOIN  "+ Linetb + @" b on b.""DocEntry"" = a.""DocEntry"" ";
                            strSQL += @" LEFT JOIN OLCT ss on ss.""Code"" = b.""LocCode""";
                            strSQL += " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\" =a.\"CardCode\" and CRD1.\"Address\" =a.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
                            strSQL += @" LEFT JOIN NNM1 nnm1 ON a.""Series"" =nnm1.""Series"" ";
                            strSQL += @" where a.""DocEntry""='" + DocEntry + @"'  AND (a.""" + clsModule.EwayUDF + @""" <>''";
                            strSQL += @" or a.""" + clsModule.EwayTransportId + @""" <>'')";
                        }

                        invrecordset.DoQuery(strSQL);
                        if (invrecordset.RecordCount > 0)
                        {
                            if (!String.IsNullOrEmpty(clsModule.EwayNo))
                            {
                                string distance = "0";
                               
                                if (clsModule.objaddon.objglobalmethods.Ctoint(invrecordset.Fields.Item(clsModule.EwayDistance).Value.ToString()) > 0)
                                {
                                    distance = invrecordset.Fields.Item(clsModule.EwayDistance).Value.ToString();
                                }
                                if (!string.IsNullOrEmpty(clsModule.EwayDistance))
                                {
                                    if (!string.IsNullOrEmpty(invrecordset.Fields.Item(clsModule.EwayDistance).Value.ToString()))
                                    {
                                        distance = invrecordset.Fields.Item(clsModule.EwayDistance).Value.ToString();
                                    }
                                }


                                clienCred_GetIRN_DocNum.ewbeinvoicelist.Add(new Ewbeinvoicelist
                                {
                                    Irn = invrecordset.Fields.Item("U_IRNNo").Value.ToString(),
                                    Distance = distance,
                                    VehType = invrecordset.Fields.Item(clsModule.EwayUDF).Value.ToString() != "" ? "R" : "",
                                    TransMode = invrecordset.Fields.Item(clsModule.EwayUDF).Value.ToString() != "" ? "1" : "",
                                    VehNo = invrecordset.Fields.Item(clsModule.EwayUDF).Value.ToString(),
                                    TransName = invrecordset.Fields.Item(clsModule.EwayTransportName).Value.ToString(),
                                    TransId = invrecordset.Fields.Item(clsModule.EwayTransportId).Value.ToString()
                                });
                            }
                            else
                            {
                                decimal CalcDistance = 0;
                                Generate_EWay distanceEway = new Generate_EWay();
                                distanceEway.CLIENTCODE = ClientCode;
                                distanceEway.USERCODE = UserCode;
                                distanceEway.PASSWORD = Password;
                                distanceEway.frompincode = invrecordset.Fields.Item("FrmZipCode").Value.ToString();
                                distanceEway.topincode = invrecordset.Fields.Item("ToZipCode").Value.ToString();
                                if (string.IsNullOrEmpty(invrecordset.Fields.Item("FrmZipCode").Value.ToString()) || string.IsNullOrEmpty(invrecordset.Fields.Item("ToZipCode").Value.ToString()))
                                {
                                    clsModule.objaddon.objapplication.StatusBar.SetText("Zip Code Missing E-way Details Page.... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    return false;
                                }
                                
                                if (clsModule.objaddon.objglobalmethods.Ctoint(invrecordset.Fields.Item("Distance").Value.ToString()) > 0)
                                {
                                    CalcDistance = clsModule.objaddon.objglobalmethods.CtoD(invrecordset.Fields.Item("Distance").Value.ToString());
                                }

                                clienCred_GetIRN_DocNum.ewbeinvoicelist.Add(new Ewbeinvoicelist
                                {
                                    Irn = invrecordset.Fields.Item("U_IRNNo").Value.ToString(),
                                    Distance = CalcDistance.ToString(),
                                    TransDocNo = invrecordset.Fields.Item("TransDocNo").Value.ToString(),
                                    TransId = invrecordset.Fields.Item("TransID").Value.ToString(),
                                    TransMode = invrecordset.Fields.Item("TransMode").Value.ToString() == "-1" || invrecordset.Fields.Item("TransMode").Value.ToString() == "0" || string.IsNullOrEmpty(invrecordset.Fields.Item("TransMode").Value.ToString()) ? "" : invrecordset.Fields.Item("TransMode").Value.ToString(),
                                    TransName = invrecordset.Fields.Item("TransName").Value.ToString(),
                                    VehNo = invrecordset.Fields.Item("VehicleNo").Value.ToString(),
                                    VehType = invrecordset.Fields.Item("VehicleTyp").Value.ToString() == "-1" || string.IsNullOrEmpty(invrecordset.Fields.Item("VehicleTyp").Value.ToString()) ? "" : invrecordset.Fields.Item("VehicleTyp").Value.ToString(),
                                    TransDocDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("TransDate").Value.ToString())
                                });

                                if (!(string.IsNullOrEmpty(invrecordset.Fields.Item("FrmGSTN").Value.ToString()) || string.IsNullOrEmpty(invrecordset.Fields.Item("FrmAddres1").Value.ToString())))
                                {
                                    if (!(invrecordset.Fields.Item("TransType").Value.ToString() == "1" || invrecordset.Fields.Item("TransType").Value.ToString() == "2"))
                                    {

                                        string sellerAddress1 = "";
                                        string sellerAddress2 = "";
                                        string SellerconcatAddress = invrecordset.Fields.Item("CompnyAddr").Value.ToString();

                                        List<string> Sellersubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(SellerconcatAddress, 70);

                                        foreach (string substring in Sellersubstrings)
                                        {
                                            if (string.IsNullOrEmpty(sellerAddress1))
                                            {
                                                sellerAddress1 = substring;
                                                continue;
                                            }
                                            if (string.IsNullOrEmpty(sellerAddress2))
                                            {
                                                sellerAddress2 = substring;
                                                continue;
                                            }
                                        }

                                        string FRMAddress1 = "";
                                        string FRMAddress2 = "";

                                        string FRMconcatAddress = string.Concat(invrecordset.Fields.Item("FrmAddres1").Value.ToString(),
                                                                            invrecordset.Fields.Item("FrmAddres2").Value.ToString());
                                        List<string> FRMsubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(FRMconcatAddress, 70);

                                        foreach (string substring in FRMsubstrings)
                                        {
                                            if (string.IsNullOrEmpty(FRMAddress1))
                                            {
                                                FRMAddress1 = substring;
                                                continue;
                                            }
                                            if (string.IsNullOrEmpty(FRMAddress2))
                                            {
                                                FRMAddress2 = substring;
                                                continue;
                                            }
                                        }
                                      

                                        if (invrecordset.Fields.Item("DisEway").Value.ToString() == "Y")
                                        {

                                            clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Nm = invrecordset.Fields.Item("FrmTraName").Value.ToString();
                                            if (!string.IsNullOrEmpty(invrecordset.Fields.Item("DisName").Value.ToString()))
                                            {
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Nm = invrecordset.Fields.Item("DisName").Value.ToString();
                                            }

                                            clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Addr1 = FRMAddress1;
                                            clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Addr2 = FRMAddress2;
                                            clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Loc = invrecordset.Fields.Item("FrmPlace").Value.ToString();
                                            clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Pin = invrecordset.Fields.Item("FrmZipCode").Value.ToString();
                                            clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Stcd = invrecordset.Fields.Item("ActFrmStat").Value.ToString();

                                        }
                                        else
                                        {

                                            if (objRs.Fields.Item("U_GetDisAddWare").Value.ToString() == "Y")
                                            {
                                                string diswarequery = "SELECT \"Building\" ,\"Block\" ,\"Street\" ,\"Address2\" ,\"Address3\" , ";
                                                diswarequery += " \"City\" ,\"ZipCode\" , COALESCE(\"GSTCode\",'96') as \"Statecode\"  ";
                                                diswarequery += "FROM OWHS o ";
                                                diswarequery += " LEFT JOIN OCST st1 on st1.\"Code\"=o.\"State\" and st1.\"Country\"=o.\"Country\" ";
                                                diswarequery += " WHERE \"WhsCode\" IN(SELECT \"WhsCode\"  FROM "+ Linetb + " WHERE  \"DocEntry\" = '" + DocEntry + "'); ";

                                                disRecset = clsModule.objaddon.objglobalmethods.GetmultipleRS(diswarequery);
                                                if (disRecset.RecordCount > 0)
                                                {
                                                    string DisAddress1 = "";
                                                    string DisAddress2 = "";
                                                    string DisconcatAddress = string.Concat(disRecset.Fields.Item("Building").Value.ToString(), " ",
                                                                                        disRecset.Fields.Item("Block").Value.ToString(), " ",
                                                                                        disRecset.Fields.Item("Street").Value.ToString(), " ",
                                                                                        disRecset.Fields.Item("Address2").Value.ToString(), " ",
                                                                                        disRecset.Fields.Item("Address3").Value.ToString());
                                                    List<string> Dissubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(DisconcatAddress, 70);
                                                    foreach (string substring in Dissubstrings)
                                                    {
                                                        if (string.IsNullOrEmpty(DisAddress1))
                                                        {
                                                            DisAddress1 = substring;
                                                            continue;
                                                        }
                                                        if (string.IsNullOrEmpty(DisAddress2))
                                                        {
                                                            DisAddress2 = substring;
                                                            continue;
                                                        }
                                                    }

                                                    clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Nm = invrecordset.Fields.Item("Seller_Legal Name").Value.ToString();
                                                    clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Addr1 = DisAddress1;
                                                    clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Addr2 = DisAddress2;
                                                    clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Loc = disRecset.Fields.Item("City").Value.ToString();
                                                    clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Pin = disRecset.Fields.Item("ZipCode").Value.ToString();
                                                    clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Stcd = disRecset.Fields.Item("Statecode").Value.ToString();
                                                }

                                            }
                                            else
                                            {
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Nm = invrecordset.Fields.Item("Seller_Legal Name").Value.ToString();
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Addr1 = sellerAddress1;
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Addr2 = sellerAddress2;
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Loc = invrecordset.Fields.Item("CompnyCity").Value.ToString();
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Pin = invrecordset.Fields.Item("CompnyPincode").Value.ToString();
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].DispDtls.Stcd = invrecordset.Fields.Item("Compnystatecode").Value.ToString();
                                            }

                                        }

                                    }
                                }
                                if (!(string.IsNullOrEmpty(invrecordset.Fields.Item("ToGSTN").Value.ToString()) || string.IsNullOrEmpty(invrecordset.Fields.Item("ToAddres1").Value.ToString())))
                                {
                                    if (!(invrecordset.Fields.Item("TransType").Value.ToString() == "1" || invrecordset.Fields.Item("TransType").Value.ToString() == "3"))
                                    {

                                        string TOAddress1 = "";
                                        string TOAddress2 = "";
                                        string TOconcatAddress = string.Concat(invrecordset.Fields.Item("ToAddres1").Value.ToString(),
                                                                            invrecordset.Fields.Item("ToAddres2").Value.ToString());
                                        List<string> TOsubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(TOconcatAddress, 70);
                                        foreach (string substring in TOsubstrings)
                                        {
                                            if (string.IsNullOrEmpty(TOAddress1))
                                            {
                                                TOAddress1 = substring;
                                                continue;
                                            }
                                            if (string.IsNullOrEmpty(TOAddress2))
                                            {
                                                TOAddress2 = substring;
                                                continue;
                                            }
                                        }

                                        clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Gstin = invrecordset.Fields.Item("ToGSTN").Value.ToString();

                                        if (objRs.Fields.Item("U_ShipToInvName").Value.ToString() == "Y")
                                        {
                                            clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.LglNm = invrecordset.Fields.Item("ShipToCode").Value.ToString();
                                        }
                                        clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.LglNm = invrecordset.Fields.Item("ToTraName").Value.ToString();
                                        clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Addr1 = TOAddress1;
                                        clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Addr2 = TOAddress2;
                                        clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Loc = invrecordset.Fields.Item("ToPlace").Value.ToString();
                                        clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Pin = invrecordset.Fields.Item("ToZipCode").Value.ToString();
                                        clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Stcd = invrecordset.Fields.Item("ActToState").Value.ToString();




                                        if (objRs.Fields.Item("U_GettrnShp").Value.ToString() == "Y")
                                        {
                                            string diswarequery = "select ";
                                            diswarequery += " CASE WHEN COALESCE(Crd1.\"GSTRegnNo\",'') = '' THEN CASE WHEN T.\"ImpORExp\" = 'Y' THEN 'URP' ELSE '' END ELSE Crd1.\"GSTRegnNo\" END as \"Shipto GSTN\",";
                                            diswarequery += " T.\"BuildingS\" \"T_Ship_Building\",T.\"BlockS\" \"T_Ship_Block\",T.\"StreetS\" \"T_Ship_Street\",T.\"Address2S\" \"T_Ship_Address2\",T.\"Address3S\" \"T_Ship_Address3\",";
                                            diswarequery +=  " T.\"CityS\" \"T_SCity\" , CASE WHEN T.\"ExportType\" = 'E' AND T.\"ImpORExp\" = 'Y' THEN '999999' ELSE Replace(T.\"ZipCodeS\",' ','') END as  \"T_SZipCode\", ";
                                            diswarequery += " (select COALESCE(\"GSTCode\",'96') from OCST where \"Code\"=T.\"StateS\" and \"Country\"=T.\"CountryS\") \"T_Shipp to State Code\" ";
                                            diswarequery += " FROM "+ tb + " a ";
                                            diswarequery += " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\" =a.\"CardCode\" and CRD1.\"Address\" =a.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
                                            diswarequery += " LEFT JOIN "+ Line12 + " T ON T.\"DocEntry\"=a.\"DocEntry\"";
                                            diswarequery += " WHERE a.\"DocEntry\"=" + DocEntry + " ";

                                            disRecset = clsModule.objaddon.objglobalmethods.GetmultipleRS(diswarequery);
                                            if (disRecset.RecordCount > 0)
                                            {



                                                string TshipAddress1 = "";
                                                string TshipAddress2 = "";
                                                string TBuyerconcatAddress = string.Concat(disRecset.Fields.Item("T_Ship_Building").Value.ToString(),
                                                                                    disRecset.Fields.Item("T_Ship_Block").Value.ToString(),
                                                                                    disRecset.Fields.Item("T_Ship_Street").Value.ToString(),
                                                                                    disRecset.Fields.Item("T_Ship_Address2").Value.ToString(),
                                                                                    disRecset.Fields.Item("T_Ship_Address3").Value.ToString());
                                                List<string> Tshipsubstrings = clsModule.objaddon.objglobalmethods.SplitByLength(TBuyerconcatAddress, 70);
                                                foreach (string substring in Tshipsubstrings)
                                                {
                                                    if (string.IsNullOrEmpty(TshipAddress1))
                                                    {
                                                        TshipAddress1 = substring;
                                                        continue;
                                                    }
                                                    if (string.IsNullOrEmpty(TshipAddress2))
                                                    {
                                                        TshipAddress2 = substring;
                                                        continue;
                                                    }
                                                }
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Gstin = disRecset.Fields.Item("Shipto GSTN").Value.ToString();
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Addr1 = TshipAddress1;
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Addr2 = TshipAddress2;
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Loc = disRecset.Fields.Item("T_SCity").Value.ToString();
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Pin = disRecset.Fields.Item("T_SZipCode").Value.ToString();
                                                clienCred_GetIRN_DocNum.ewbeinvoicelist[0].ExpShipDtls.Stcd = disRecset.Fields.Item("T_Shipp to State Code").Value.ToString();

                                            }

                                        }


                                    }
                                }

                            }

                            requestParams = JsonConvert.SerializeObject(clienCred_GetIRN_DocNum);
                            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(requestParams);
                            clsModule.objaddon.objglobalmethods.RemoveNullValues(jsonObject);

                             requestParams = jsonObject.ToString();

                            if (fromGst)
                            {
                                jsonstring = requestParams;
                                return true;
                            }
                            datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                            string mm = datatable.Rows[0]["message"].ToString();
                            E_Invoice_Logs(DocEntry, datatable, TransType, "Create", Type, requestParams);

                            string msg = datatable.Rows[0]["message"].ToString();
                            if (datatable.Rows[0]["error_log_id"].ToString() == string.Empty)
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            }
                            else
                            {
                                if (!blnRefresh)
                                {
                                    clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    clsModule.objaddon.objapplication.MessageBox("Generate: " + msg);
                                }
                            }
                        }
                        else
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Check Eway Details for this invoice...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }

                        clienCred_GetIRN_DocNum = null;

                    }
                }
                else if (Create_Cancel == EinvoiceMethod.UpdateEway)
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Generating E-Way Update Please wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                    strSQL = @"Select T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",T0.""U_UATUrl"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                    strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL,T1.""U_URL""";
                    strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'  and T1.""U_URLType"" ='Update Eway' and T1.""U_Type""='E-Way' Order by ""LineId"" Desc";
                    objRs.DoQuery(strSQL);
                    if (objRs.RecordCount == 0) { clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Update Eway\". Please update in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error); return false; }

                    UpdateEway clienCred_GetIRN_DocNum = new UpdateEway();
                    if (objRs.RecordCount > 0)
                    {

                        string[] splv = objRs.Fields.Item("U_URL").Value.ToString().Split('~');
                        if (splv.Length != 2)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Check API for \"Update Eway\". Need update Transport API and Update Eway API... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error); return false;
                        }
                        clienCred_GetIRN_DocNum.CLIENTCODE = ClientCode;
                        clienCred_GetIRN_DocNum.USERCODE = UserCode;
                        clienCred_GetIRN_DocNum.PASSWORD = Password;
                        invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        string tb = "";
                        string eway = "";
                        string linetb = "";
                        switch (TransType)
                        {
                            case "INV":
                                tb = "OINV";
                                eway = "inv26";
                                linetb = "inv1";
                                break;
                            case "CRN":
                                tb = "ORIN";
                                eway = "rin26";
                                linetb = "rin1";
                                break;
                            case "WTR":
                                tb = "OWTR";
                                eway = "WTR26";
                                linetb = "WTR1";
                                break;
                            case "DLN":
                                tb = "ODLN";
                                eway = "DLN26";
                                linetb = "DLN";
                                break;
                        }
                        if (String.IsNullOrEmpty(clsModule.EwayNo))
                        {
                            strSQL = @" SELECT t2.""VehicleTyp"",t1.""U_IRNNo"" ,t2.""Distance"" ,t2.""TransMode"" ,t2.""TransID"" ,t2.""TransName"" ,
                                       t2.""TransDocNo"" ,t2.""TransDate"",t2.""FrmZipCode"",t2.""ToZipCode"",";
                            strSQL += @" t2.""VehicleNo"" ,t2.""FrmState"" ,t2.""ToState"" ,
                                        t2.""EWayBillNo"",t2.""FrmPlace"" ,t2.""ToPlace"" ,t2.""ActFrmStat"" ,t2.""ActToState""
                                        FROM " + tb + @" t1 LEFT JOIN " + eway + @" t2 ON t1.""DocEntry"" =t2.""DocEntry"" ";                          
                            strSQL += @" where t1.""DocEntry""='" + DocEntry + @"'";
                        }
                        else
                        {
                            strSQL = @" SELECT 'R' as ""VehicleTyp"", a.""U_IRNNo"" , a.""" + clsModule.EwayDistance + @""" ""Distance"",1 as ""TransMode"",
                                        a.""" + clsModule.EwayTransportId + @""" as TransID,a.""" + clsModule.EwayTransportName + @""" as TransName,
                                        a.""" + clsModule.EwayNo + @""" as EWayBillNo,
                                        '' as TransDocNo,'' as TransDate,Replace(ss.""ZipCode"",' ','') ""FrmZipCode"",";
                            strSQL += @" Replace(CRD1.""ZipCode"",' ','') ""ToZipCode"", a.""" + clsModule.EwayUDF + @""" VehicleNo,";
                            strSQL += "(select COALESCE(\"GSTCode\",96) from OCST where \"Country\"=ss.\"Country\" and \"Code\"=ss.\"State\") \"FrmState\",";
                            strSQL += @" st.""Name"" as FrmStateN, cy.""Name"" as FrmPlace  ";
                            
                            strSQL += @"  FROM " + tb + @" a  ";
                            strSQL += @" INNER JOIN "+linetb +@" b on b.""DocEntry"" = a.""DocEntry"" ";
                            strSQL += @" LEFT JOIN OLCT ss on ss.""Code"" = b.""LocCode""";
                            strSQL += " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\" =a.\"CardCode\" and CRD1.\"Address\" =a.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
                            strSQL += " LEFT JOIN OCST st on st.\"Code\"=CRD1.\"State\" and st.\"Country\"=CRD1.\"Country\"";
                            strSQL += " LEFT JOIN OCRY cy on cy.\"Code\" =CRD1.\"Country\"";
                            strSQL += @" LEFT JOIN NNM1 nnm1 ON a.""Series"" =nnm1.""Series"" ";
                            strSQL += @" where a.""DocEntry""='" + DocEntry + @"'  AND (a.""" + clsModule.EwayUDF + @""" <>''";
                            strSQL += @" or a.""" + clsModule.EwayTransportId + @""" <>'')";
                        }
                        invrecordset.DoQuery(strSQL);
                        if (invrecordset.RecordCount > 0)
                        {

                            //Generate_EWay distanceEway = new Generate_EWay();
                            //distanceEway.CLIENTCODE = ClientCode;
                            //distanceEway.USERCODE = UserCode;
                            //distanceEway.PASSWORD = Password;
                            //distanceEway.frompincode = invrecordset.Fields.Item("FrmZipCode").Value.ToString();
                            //distanceEway.topincode = invrecordset.Fields.Item("ToZipCode").Value.ToString();
                            //if (string.IsNullOrEmpty(invrecordset.Fields.Item("FrmZipCode").Value.ToString()) || string.IsNullOrEmpty(invrecordset.Fields.Item("ToZipCode").Value.ToString()))
                            //    {
                            //    clsModule.objaddon.objapplication.StatusBar.SetText("Zip Code Missing E-way Details Page.... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            //    return false;
                            //}
                            string mm;
                            string msg="";
                            clienCred_GetIRN_DocNum.transporteridlist.Add(new transporteridlist
                            {
                                ewbNo = invrecordset.Fields.Item("EWayBillNo").Value.ToString(),
                                transporterId = invrecordset.Fields.Item("TransID").Value.ToString(),
                            });

                            requestParams = JsonConvert.SerializeObject(clienCred_GetIRN_DocNum);
                            if (fromGst)
                            {
                                jsonstring = requestParams;
                                return true;
                            }
                            datatable = Get_API_Response(requestParams, objRs.Fields.Item("BaseURL").Value.ToString() + splv[0]);
                            mm = datatable.Rows[0]["message"].ToString();

                            if (!string.IsNullOrEmpty(invrecordset.Fields.Item("VehicleNo").Value.ToString()))                                
                            {
                                clienCred_GetIRN_DocNum.Vehicleupdatelist.Add(new Vehicleupdatelist
                                {
                                    ewbNo = invrecordset.Fields.Item("EWayBillNo").Value.ToString(),
                                    fromPlace = invrecordset.Fields.Item("FrmPlace").Value.ToString(),
                                    fromState = invrecordset.Fields.Item("FrmState").Value.ToString(),
                                    reasonCode = "1",
                                    reasonRem = "Change",
                                    transDocDate = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("TransDate").Value.ToString()),
                                    transDocNo = invrecordset.Fields.Item("TransDocNo").Value.ToString(),
                                    transMode = invrecordset.Fields.Item("TransMode").Value.ToString(),
                                    vehicleNo = invrecordset.Fields.Item("VehicleNo").Value.ToString(),
                                    vehicleType = invrecordset.Fields.Item("VehicleTyp").Value.ToString(),

                                });
                                requestParams = JsonConvert.SerializeObject(clienCred_GetIRN_DocNum);
                                if (fromGst)
                                {
                                    jsonstring = requestParams;
                                    return true;
                                }
                                datatable = Get_API_Response(requestParams, objRs.Fields.Item("BaseURL").Value.ToString() + splv[1]);
                                msg = datatable.Rows[0]["message"].ToString();

                            }
                            
                            if (datatable.Rows[0]["error_log_id"].ToString() == string.Empty)
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            }
                            else
                            {
                                if (!blnRefresh)
                                {
                                    clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    clsModule.objaddon.objapplication.MessageBox("Generate: " + msg);
                                }
                            }
                        }
                        else
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Check Eway Details for this invoice...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }

                        clienCred_GetIRN_DocNum = null;

                    }
                }                
                else if (Create_Cancel == EinvoiceMethod.GetIrnByDocnum)
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Getting IRN. Please wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                    strSQL = @"Select T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",T0.""U_SERCONFIG"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                    strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                    strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'  and T1.""U_URLType"" ='Get IRN Details by Document number' and T1.""U_Type""='E-Invoice' Order by ""LineId"" Desc";

                    string docseries = "CONCAT(CAST(COALESCE( nnm1.\"BeginStr\",'') AS varchar(100)), CAST(a.\"DocNum\" AS varchar(100)))";
                    string Baseurl = "";

                    objRs.DoQuery(strSQL);
                    if (objRs.RecordCount == 0) { clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Get IRN Details by Document number\". Please update in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error); return false; }

                    ClienCred_GetIRN_DocNum clienCred_GetIRN_DocNum = new ClienCred_GetIRN_DocNum();
                    if (objRs.RecordCount > 0)
                    {

                        clienCred_GetIRN_DocNum.CLIENTCODE = ClientCode;
                        clienCred_GetIRN_DocNum.USERCODE = UserCode;
                        clienCred_GetIRN_DocNum.PASSWORD = Password;
                        Baseurl= objRs.Fields.Item("BaseURL").Value.ToString();
                        if (!String.IsNullOrEmpty(Convert.ToString(objRs.Fields.Item("U_SERCONFIG").Value)))
                        {
                            docseries = Convert.ToString(objRs.Fields.Item("U_SERCONFIG").Value);
                        }

                        invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);


                        string tb = "";
                        string tbline = "";
                        string doctype = "";
                        switch (TransType)
                        {
                            case "INV":
                                tb = "OINV";
                                tbline = "INV1";
                                doctype = "INV";
                                break;
                            case "CRN":
                                tb = "ORIN";
                                tbline = "RIN1";
                                doctype = "CRN";
                                break;
                        }

                        strSQL = @"Select T0.""GSTRegnNo"",a.""DocEntry"",a.""DocDate"" as DocDate,'" + doctype + @"' DocType,a.""U_IRNNo"",";

                        strSQL = strSQL += docseries + " \"Inv_No\"";

                        strSQL += @" from ""OLCT"" T0 left join " + tbline + @" T1 on T0.""Code""=T1.""LocCode"" left join " + tb + @" a on T1.""DocEntry""=a.""DocEntry""";
                        strSQL += @" LEFT JOIN NNM1 nnm1 ON a.""Series"" =nnm1.""Series"" where a.""DocEntry""=" + DocEntry + "";

                        invrecordset.DoQuery(strSQL);

                        clienCred_GetIRN_DocNum.RequestorGSTIN = invrecordset.Fields.Item("GSTRegnNo").Value.ToString();//"32ACRPV8768P1Z8";
                        clienCred_GetIRN_DocNum.docdetailslist.Add(new Docdetailslist
                        {
                            DocNum = invrecordset.Fields.Item("Inv_No").Value.ToString(),
                            DocType = invrecordset.Fields.Item("DocType").Value.ToString(),
                            DocDate = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("DocDate").Value.ToString())
                        });
                        requestParams = JsonConvert.SerializeObject(clienCred_GetIRN_DocNum);
                        if (fromGst)
                        {
                            jsonstring = requestParams;
                            return true;
                        }
                        datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                        E_Invoice_Logs(DocEntry, datatable, TransType, "Create", Type, requestParams);

                        GetIRN irn = new GetIRN();

                        irn.CLIENTCODE = clienCred_GetIRN_DocNum.CLIENTCODE;
                        irn.USERCODE = clienCred_GetIRN_DocNum.USERCODE;
                        irn.PASSWORD = clienCred_GetIRN_DocNum.PASSWORD;
                        irn.RequestorGSTIN = clienCred_GetIRN_DocNum.RequestorGSTIN;

                        irn.IRNList.Add(new Irnlist
                        {
                            IRN = invrecordset.Fields.Item("U_IRNNo").Value.ToString()
                        });
                        requestParams = JsonConvert.SerializeObject(irn);
                        datatable = Get_API_Response(requestParams, Baseurl + @"/TransactionAPI/GetEWaybillDetailsByIRN");

                        E_Invoice_Logs(DocEntry, datatable, TransType, "Create", "E-way IRN", requestParams);




                        string msg = datatable.Rows[0]["message"].ToString();
                        if (datatable.Rows[0]["flag"].ToString() == "False")
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                        clienCred_GetIRN_DocNum = null;

                    }
                }
                else if (Create_Cancel == EinvoiceMethod.GETIRNDetails)
                {
                    strSQL = @"Select T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",T0.""U_SERCONFIG"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                    strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL,T1.";
                    strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'  and T1.""U_URLType"" ='Get IRN Details' and T1.""U_Type""='E-Invoice' Order by ""LineId"" Desc";


                    objRs.DoQuery(strSQL);
                    if (objRs.RecordCount == 0) { clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Get IRN Details\". Please update in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error); return false; }
                    string docseries = "CONCAT(CAST(COALESCE( nnm1.\"BeginStr\",'') AS varchar(100)), CAST(a.\"DocNum\" AS varchar(100)))";

                    GetIRN getIRN = new GetIRN();
                    if (objRs.RecordCount > 0)
                    {
                        getIRN.CLIENTCODE =ClientCode;//"ptmuT";
                        getIRN.USERCODE = UserCode;// "Premier_DEMO";
                        getIRN.PASSWORD = Password;//"Premier@123";
                        invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        string tb = "";
                        string tbline = "";
                        switch (TransType)
                        {
                            case "INV":
                                tb = "OINV";
                                tbline = "INV1";
                                break;
                            case "CRN":
                                tb = "ORIN";
                                tbline = "RIN1";
                                break;
                        }

                        strSQL = @"Select T0.""GSTRegnNo"",a.""DocEntry"",a.""DocDate"" as DocDate,'INV' DocType,a.""U_IRNNo"",";

                        strSQL = strSQL += docseries + " \"Inv_No\"";

                        strSQL += @" from ""OLCT"" T0 left join " + tbline + @" T1 on T0.""Code""=T1.""LocCode"" left join " + tb + @" a on T1.""DocEntry""=a.""DocEntry""";
                        strSQL += @" LEFT JOIN NNM1 nnm1 ON a.""Series"" =nnm1.""Series"" where a.""DocEntry""=" + DocEntry + "";

                        invrecordset.DoQuery(strSQL);
                        getIRN.RequestorGSTIN = invrecordset.Fields.Item("GSTRegnNo").Value.ToString();//"32ACRPV8768P1Z8";
                        getIRN.IRNList.Add(new Irnlist
                        {
                            IRN = invrecordset.Fields.Item("U_IRNNo").Value.ToString()

                        });
                        requestParams = JsonConvert.SerializeObject(getIRN);
                        if (fromGst)
                        {
                            jsonstring = requestParams;
                            return true;
                        }
                        datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                        E_Invoice_Logs(DocEntry, datatable, TransType, "Create", Type, requestParams);

                        string msg = datatable.Rows[0]["message"].ToString();
                        if (datatable.Rows[0]["error_log_id"].ToString() == string.Empty)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        }
                        else
                        {
                            if (!blnRefresh)
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                clsModule.objaddon.objapplication.MessageBox("Generate: " + msg);
                            }
                        }
                        getIRN = null;
                    }
                }


            }
            catch (Exception ex)
            {
                clsModule.objaddon.objglobalmethods.WriteErrorLog(ex.StackTrace);
                clsModule.objaddon.objapplication.StatusBar.SetText("Error_IRN: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            return true;
        }

        private bool Einvoicelogs_9(string InvDocEntry, DataTable einvDT, string ObjType, string Type, string TranType, string requrl)
        {
            try
            {
                blnRefresh = false;
                bool Flag = false;
                string DocEntry;
                string obj = "";
                switch (ObjType)
                {
                    case "INV":
                        obj = "13";
                        break;
                    case "CRN":
                        obj = "14";
                        break;
                    case "WTR":
                        obj = "67";
                        break;
                    case "DLN":
                        obj = "15";
                        break;
                    case "PCH":
                        obj = "18";
                        break;
                }

                SAPbobsCOM.GeneralService oGeneralService;
                SAPbobsCOM.GeneralData oGeneralData;
                SAPbobsCOM.GeneralDataParams oGeneralParams;
                objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oGeneralService = clsModule.objaddon.objcompany.GetCompanyService().GetGeneralService("ATEINV");
                oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                oGeneralParams = (SAPbobsCOM.GeneralDataParams)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                try
                {
                    DocEntry = clsModule.objaddon.objglobalmethods.getSingleValue(@"Select ""DocEntry"" from ""@ATPL_EINV"" where ""U_BaseEntry""='" + InvDocEntry + @"'And ""U_DocObjType""='" + obj + @"'  Order by ""DocEntry"" Desc");
                    oGeneralParams.SetProperty("DocEntry", DocEntry);
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                    Flag = true;
                }
                catch (Exception ex)
                {
                    Flag = false;
                }
                if (Type == "Create")
                {
                    string lstrquery;

                    if (TranType == "E-Invoice")
                    {

                        oGeneralData.SetProperty("U_IRNNo", einvDT.Rows[0]["Irn"].ToString());
                        oGeneralData.SetProperty("U_QRCode", einvDT.Rows[0]["SignedQRCode"].ToString());
                        oGeneralData.SetProperty("U_SgnInv", einvDT.Rows[0]["SignedInvoice"].ToString());
                        oGeneralData.SetProperty("U_AckNum", einvDT.Rows[0]["AckNo"].ToString());
                        oGeneralData.SetProperty("U_IRNStat", einvDT.Rows[0]["Status"].ToString());
                        oGeneralData.SetProperty("U_DcrptInv", einvDT.Rows[0]["DcrySignedInvoice"].ToString());
                        oGeneralData.SetProperty("U_DcrptQRCode", einvDT.Rows[0]["DcrySignedQRCode"].ToString());
                        oGeneralData.SetProperty("U_ErrLogId", einvDT.Rows[0]["error_log_ids"].ToString());
                        oGeneralData.SetProperty("U_Einvreqjson", requrl);
                        oGeneralData.SetProperty("U_GenDate", einvDT.Rows[0]["DocDate"].ToString());
                        oGeneralData.SetProperty("U_BaseNo", einvDT.Rows[0]["DocNo"].ToString());
                        oGeneralData.SetProperty("U_BaseEntry", InvDocEntry);
                        oGeneralData.SetProperty("U_DocObjType", obj);
                        oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                        oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                        oGeneralData.SetProperty("U_EwbNum", einvDT.Rows[0]["EwbNo"].ToString());
                        oGeneralData.SetProperty("U_Ewaypdf", einvDT.Rows[0]["pdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwayDetpdf", einvDT.Rows[0]["detailedpdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwbDate", einvDT.Rows[0]["EwbDt"].ToString());
                        oGeneralData.SetProperty("U_EwbValidTill", einvDT.Rows[0]["EwbValidTill"].ToString());
                    }
                    else if (TranType == "E-way")
                    {

                        oGeneralData.SetProperty("U_GenDate", einvDT.Rows[0]["docDate"].ToString());
                        oGeneralData.SetProperty("U_BaseNo", einvDT.Rows[0]["docNo"].ToString());
                        oGeneralData.SetProperty("U_BaseEntry", InvDocEntry);
                        oGeneralData.SetProperty("U_DocObjType", obj);
                        oGeneralData.SetProperty("U_Ewayreqjson", requrl);
                        oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                        oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                        oGeneralData.SetProperty("U_EwbNum", einvDT.Rows[0]["ewayBillNo"].ToString());
                        oGeneralData.SetProperty("U_Ewaypdf", einvDT.Rows[0]["pdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwayDetpdf", einvDT.Rows[0]["detailedpdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwbDate", einvDT.Rows[0]["ewayBillDate"].ToString());
                        oGeneralData.SetProperty("U_EwbValidTill", einvDT.Rows[0]["validUpto"].ToString());
                    }
                    else if (TranType == "E-way IRN")
                    {
                        oGeneralData.SetProperty("U_BaseEntry", InvDocEntry);
                        oGeneralData.SetProperty("U_DocObjType", obj);
                        oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                        oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                        oGeneralData.SetProperty("U_EwbNum", einvDT.Rows[0]["EwbNo"].ToString());
                        oGeneralData.SetProperty("U_Ewaypdf", einvDT.Rows[0]["pdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwayDetpdf", einvDT.Rows[0]["detailedpdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwbDate", einvDT.Rows[0]["EwbDt"].ToString());
                        oGeneralData.SetProperty("U_EwbValidTill", einvDT.Rows[0]["EwbValidTill"].ToString());


                    }
                    if (TranType == "E-Invoice")
                    {
                        string tb = "";
                        switch (ObjType)
                        {
                            case "INV":
                                tb = "OINV";
                                break;
                            case "CRN":
                                tb = "ORIN";
                                break;
                        }
                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["Irn"].ToString()))
                        {
                            lstrquery = @"Update " + tb + @" Set ""U_IRNNo""='" + einvDT.Rows[0]["Irn"].ToString() + "',";
                            lstrquery += @"""U_QRCode""='" + einvDT.Rows[0]["SignedQRCode"].ToString() + "',";

                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                            {
                                lstrquery += @"""QRCodeSrc""='" + einvDT.Rows[0]["SignedQRCode"].ToString() + "',";
                            }

                            lstrquery += @"""U_AckDate""='" + einvDT.Rows[0]["AckDt"].ToString() + "',";
                            lstrquery += @"""U_AckNo""='" + einvDT.Rows[0]["AckNo"].ToString() + "',";
                            lstrquery += @"""U_Ewaypdf""='" + einvDT.Rows[0]["pdfUrl"].ToString() + "',";
                            lstrquery += @"""U_EwayDetpdf""='" + einvDT.Rows[0]["detailedpdfUrl"].ToString() + "',";

                            lstrquery += @"""U_IRNStatus""='" + einvDT.Rows[0]["Status"].ToString() + "'";

                            if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbNo"].ToString()))
                            {
                                if (!String.IsNullOrEmpty(clsModule.EwayNo))
                                {
                                    lstrquery += @",""" + clsModule.EwayNo + @"""='" + einvDT.Rows[0]["EwbNo"].ToString() + "'";
                                }
                            }

                            lstrquery += @"where ""DocEntry""='" + InvDocEntry + "'";
                            objRs.DoQuery(lstrquery);
                            blnRefresh = true;
                        }

                        switch (ObjType)
                        {
                            case "INV":
                                tb = "INV26";
                                break;
                            case "CRN":
                                tb = "RIN26";
                                break;
                            case "WTR":
                                tb = "WTR26";
                                break;
                            case "DLN":
                                tb = "DLN26";
                                break;
                            case "PCH":
                                tb = "PCH26";
                                break;
                        }
                        if (String.IsNullOrEmpty(clsModule.EwayNo))
                        {
                            if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbNo"].ToString()))
                            {

                                lstrquery = @"Update " + tb + @" set ""EWayBillNo""='" + einvDT.Rows[0]["EwbNo"].ToString() + "'";
                                if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbDt"].ToString()))
                                {
                                    lstrquery += @",""EwbDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["EwbDt"].ToString().Substring(0, 10), "yyyy-MM-dd") + "'";
                                }

                                if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbValidTill"].ToString()))
                                {
                                    lstrquery += @",""ExpireDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["EwbValidTill"].ToString().Substring(0, 10), "yyyy-MM-dd") + "'";
                                }

                                lstrquery += @" Where ""DocEntry""='" + InvDocEntry + "'";
                                objRs.DoQuery(lstrquery);
                                blnRefresh = true;
                            }
                        }
                    }
                    else if (TranType == "E-way")
                    {
                        string tb = "";
                        switch (ObjType)
                        {
                            case "INV":
                                tb = "INV26";
                                break;
                            case "CRN":
                                tb = "RIN26";
                                break;
                            case "WTR":
                                tb = "WTR26";
                                break;
                            case "DLN":
                                tb = "DLN26";
                                break;
                            case "PCH":
                                tb = "PCH26";
                                break;
                        }

                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["ewayBillNo"].ToString()))
                        {
                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                            {
                                lstrquery = @"Update " + tb + @" set ""EWayBillNo""='" + einvDT.Rows[0]["ewayBillNo"].ToString() + "'";

                                if (!string.IsNullOrEmpty(einvDT.Rows[0]["ewayBillDate"].ToString()))
                                {
                                    lstrquery += @",""EwbDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["ewayBillDate"].ToString().Substring(0, 10), "dd/MM/yyyy") + "'";
                                }
                                if (!string.IsNullOrEmpty(einvDT.Rows[0]["validUpto"].ToString()))
                                {
                                    lstrquery += @",""ExpireDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["validUpto"].ToString().Substring(0, 10), "dd/MM/yyyy") + "'";
                                }

                                lstrquery += @" Where ""DocEntry""='" + InvDocEntry + "'";
                                objRs.DoQuery(lstrquery);
                                blnRefresh = true;
                            }
                            else
                            {
                                switch (ObjType)
                                {
                                    case "INV":
                                        tb = "OINV";
                                        break;
                                    case "CRN":
                                        tb = "ORIN";
                                        break;
                                    case "WTR":
                                        tb = "OWTR";
                                        break;
                                    case "DLN":
                                        tb = "ODLN";
                                        break;
                                    case "PCH":
                                        tb = "OPCH";
                                        break;
                                }
                                lstrquery = @"Update " + tb + @" Set ";
                                lstrquery += @"""" + clsModule.EwayNo + @"""='" + einvDT.Rows[0]["EwbNo"].ToString() + "'";
                                lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                                objRs.DoQuery(lstrquery);
                                blnRefresh = true;
                            }

                            switch (ObjType)
                            {
                                case "INV":
                                    tb = "OINV";
                                    break;
                                case "CRN":
                                    tb = "ORIN";
                                    break;
                                case "WTR":
                                    tb = "OWTR";
                                    break;
                                case "DLN":
                                    tb = "ODLN";
                                    break;
                                case "PCH":
                                    tb = "OPCH";
                                    break;
                            }
                            lstrquery = @"Update " + tb + @" Set ";
                            lstrquery += @"""U_Ewaypdf""='" + einvDT.Rows[0]["pdfUrl"].ToString() + "',";
                            lstrquery += @"""U_EwayDetpdf""='" + einvDT.Rows[0]["detailedpdfUrl"].ToString() + "'";
                            lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                            objRs.DoQuery(lstrquery);
                        }
                    }
                    else if (TranType == "E-way IRN")
                    {
                        string tb = "";
                        switch (ObjType)
                        {
                            case "INV":
                                tb = "INV26";
                                break;
                            case "CRN":
                                tb = "RIN26";
                                break;
                            case "WTR":
                                tb = "WTR26";
                                break;
                            case "DLN":
                                tb = "DLN26";
                                break;
                            case "PCH":
                                tb = "PCH26";
                                break;
                        }
                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbNo"].ToString()))
                        {
                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                            {
                                lstrquery = @"Update " + tb + @" set ""EWayBillNo""='" + einvDT.Rows[0]["EwbNo"].ToString() + "'";
                                if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbDt"].ToString()))
                                {
                                    lstrquery += @",""EwbDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["EwbDt"].ToString().Substring(0, 10), "yyyy-MM-dd") + "'";
                                }
                                if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbValidTill"].ToString()))
                                {
                                    lstrquery += @",""ExpireDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["EwbValidTill"].ToString().Substring(0, 10), "yyyy-MM-dd") + "'";
                                }

                                lstrquery += @" Where ""DocEntry""='" + InvDocEntry + "'";
                                objRs.DoQuery(lstrquery);
                                blnRefresh = true;
                            }
                            else
                            {
                                switch (ObjType)
                                {
                                    case "INV":
                                        tb = "OINV";
                                        break;
                                    case "CRN":
                                        tb = "ORIN";
                                        break;
                                    case "WTR":
                                        tb = "OWTR";
                                        break;
                                    case "DLN":
                                        tb = "ODLN";
                                        break;
                                    case "PCH":
                                        tb = "OPCH";
                                        break;
                                }
                                lstrquery = @"Update " + tb + @" set """ + clsModule.EwayNo + @""" = '" + einvDT.Rows[0]["EwbNo"].ToString() + "' ";
                                lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                                objRs.DoQuery(lstrquery);
                                blnRefresh = true;
                            }
                            switch (ObjType)
                            {
                                case "INV":
                                    tb = "OINV";
                                    break;
                                case "CRN":
                                    tb = "ORIN";
                                    break;
                                case "WTR":
                                    tb = "OWTR";
                                    break;
                                case "DLN":
                                    tb = "ODLN";
                                    break;
                                case "PCH":
                                    tb = "OPCH";
                                    break;
                            }
                            lstrquery = @"Update " + tb + @" Set ";
                            lstrquery += @"""U_Ewaypdf""='" + einvDT.Rows[0]["pdfUrl"].ToString() + "',";
                            lstrquery += @"""U_EwayDetpdf""='" + einvDT.Rows[0]["detailedpdfUrl"].ToString() + "'";

                            if (!string.IsNullOrEmpty(columnFind(einvDT, "Status",0)))
                            {
                                lstrquery += @",""U_EwayStatus""='" + einvDT.Rows[0]["Status"].ToString() + "'";
                            }


                            lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                            objRs.DoQuery(lstrquery);
                        }
                    }
                }
                else if (Type == "Cancel")
                {
                    oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                    oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                    oGeneralData.SetProperty("U_CanDate", einvDT.Rows[0]["CancelDate"].ToString());
                    oGeneralData.SetProperty("U_ErrLogId", einvDT.Rows[0]["error_log_id"].ToString());

                    if (TranType == "E-Invoice")
                    {
                        string tb = "";
                        switch (ObjType)
                        {
                            case "INV":
                                tb = "OINV";
                                break;
                            case "CRN":
                                tb = "ORIN";
                                break;
                            case "WTR":
                                tb = "OWTR";
                                break;
                            case "DLN":
                                tb = "ODLN";
                                break;
                            case "PCH":
                                tb = "OPCH";
                                break;
                        }
                        if (einvDT.Rows[0]["flag"].ToString() == "True")
                        {
                            strSQL = @"Update " + tb + @" Set ""U_IRNNo""='',""U_QRCode""=''";
                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                            {
                                strSQL += @",""QRCodeSrc""=''";
                            }
                            strSQL += @",""U_AckDate""='',""U_AckNo""='',""U_EwayDetpdf""='',""U_Ewaypdf""='' where ""DocEntry""='" + InvDocEntry + "'";

                            objRs.DoQuery(strSQL);
                            blnRefresh = true;
                        }
                    }
                    else if (TranType == "E-way")
                    {
                        string lstrquery = "";
                        string tb = "";
                        switch (ObjType)
                        {
                            case "INV":
                                tb = "INV26";
                                break;
                            case "CRN":
                                tb = "RIN26";
                                break;
                            case "WTR":
                                tb = "WTR26";
                                break;
                            case "DLN":
                                tb = "DLN26";
                                break;
                            case "PCH":
                                tb = "PCH26";
                                break;
                        }
                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["ewayBillNo"].ToString()))
                        {
                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                            {
                                lstrquery = @"Update " + tb + @" set ""EWayBillNo""='',";
                                lstrquery += @"""EwbDate""='',";
                                lstrquery += @"""ExpireDate""=''";
                                lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                            }
                            else
                            {
                                switch (ObjType)
                                {
                                    case "INV":
                                        tb = "OINV";
                                        break;
                                    case "CRN":
                                        tb = "ORIN";
                                        break;
                                    case "WTR":
                                        tb = "OWTR";
                                        break;
                                    case "DLN":
                                        tb = "ODLN";
                                        break;
                                    case "PCH":
                                        tb = "OPCH";
                                        break;
                                }
                                lstrquery = @"Update " + tb + @" set """ + clsModule.EwayNo + @"""=''";
                                lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                            }
                        }

                        if (einvDT.Rows[0]["flag"].ToString() == "True")
                        {
                            objRs.DoQuery(lstrquery);
                            blnRefresh = true;

                            switch (ObjType)
                            {
                                case "INV":
                                    tb = "OINV";
                                    break;
                                case "CRN":
                                    tb = "ORIN";
                                    break;
                                case "WTR":
                                    tb = "OWTR";
                                    break;
                                case "DLN":
                                    tb = "ODLN";
                                    break;
                                case "PCH":
                                    tb = "OPCH";
                                    break;
                            }
                            lstrquery = @"Update " + tb + @" Set ";
                            lstrquery += @"""U_Ewaypdf""='',";
                            lstrquery += @"""U_EwayDetpdf""='',";
                            lstrquery += @"""U_EwayStatus""='CNL'";
                            lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                            objRs.DoQuery(lstrquery);

                        }
                    }
                }

                objRs = null;
                if (Flag == true)
                {
                    oGeneralService.Update(oGeneralData);
                    return true;
                }
                else
                {
                    oGeneralParams = oGeneralService.Add(oGeneralData);
                    return true;
                }
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objglobalmethods.WriteErrorLog(ex.ToString());
                clsModule.objaddon.objapplication.StatusBar.SetText("E_Invoice_Logs: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
        }

  


        private bool E_Invoice_Logs(string InvDocEntry, DataTable einvDT, string ObjType, string Type, string TranType, string requrl)
        {
            
            if (String.IsNullOrEmpty(clsModule.EwayNo))
            {
                return Einvoicelog_10(InvDocEntry, einvDT, ObjType, Type, TranType, requrl);
            }
            else
            {
                return Einvoicelogs_9(InvDocEntry, einvDT, ObjType, Type, TranType, requrl);
            }

        }

        private bool Einvoicelog_10(string InvDocEntry, DataTable einvDT, string ObjType, string Type, string TranType, string requrl)
        {
            try
            {
                blnRefresh = false;
                bool Flag = false;
                string DocEntry;
                string obj = "";
                string tb = "";
                string maintb = "";

                switch (ObjType)
                {
                    case "INV":
                        obj = "13";
                        tb = "INV26";
                        maintb = "OINV";
                        break;
                    case "CRN":
                        obj = "14";
                        tb = "RIN26";
                        maintb = "ORIN";
                        break;
                    case "WTR":
                        obj = "67";
                        tb = "WTR26";
                        maintb = "OWTR";
                        break;
                    case "DLN":
                        obj = "15";
                        tb = "DLN26";
                        maintb = "ODLN";
                        break;
                    case "PCH":
                        obj = "18";
                        tb = "PCH26";
                        maintb = "OPCH";
                        break;
                }
                clsModule.objaddon.objglobalmethods.WriteErrorLog("Transaction Connection Open before");
                SAPbobsCOM.Documents objsalesinvoice = null;
                switch (ObjType)
                {
                    case "INV":
                        objsalesinvoice = (SAPbobsCOM.Documents)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                        break;
                    case "CRN":
                        objsalesinvoice = (SAPbobsCOM.Documents)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
                        break;
                    case "DLN":
                        objsalesinvoice = (SAPbobsCOM.Documents)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);
                        break;
                    case "PCH":
                        objsalesinvoice = (SAPbobsCOM.Documents)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                        break;

                }
                clsModule.objaddon.objglobalmethods.WriteErrorLog("Transaction Connection Open");

                SAPbobsCOM.GeneralService oGeneralService;
                SAPbobsCOM.GeneralData oGeneralData;
                SAPbobsCOM.GeneralDataParams oGeneralParams;
                objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oGeneralService = clsModule.objaddon.objcompany.GetCompanyService().GetGeneralService("ATEINV");
                oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                oGeneralParams = (SAPbobsCOM.GeneralDataParams)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                try
                {
                    DocEntry = clsModule.objaddon.objglobalmethods.getSingleValue(@"Select ""DocEntry"" from ""@ATPL_EINV"" where ""U_BaseEntry""='" + InvDocEntry + @"'And ""U_DocObjType""='" + obj + @"'  Order by ""DocEntry"" Desc");
                    oGeneralParams.SetProperty("DocEntry", DocEntry);
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                    Flag = true;
                }
                catch (Exception ex)
                {
                    Flag = false;
                }
                if (Type == "Create")
                {
                    string lstrquery;

                    if (TranType == "E-Invoice")
                    {
                        oGeneralData.SetProperty("U_IRNNo", einvDT.Rows[0]["Irn"].ToString());
                        oGeneralData.SetProperty("U_QRCode", einvDT.Rows[0]["SignedQRCode"].ToString());
                        oGeneralData.SetProperty("U_SgnInv", einvDT.Rows[0]["SignedInvoice"].ToString());
                        oGeneralData.SetProperty("U_AckNum", einvDT.Rows[0]["AckNo"].ToString());
                        oGeneralData.SetProperty("U_IRNStat", einvDT.Rows[0]["Status"].ToString());
                        oGeneralData.SetProperty("U_DcrptInv", einvDT.Rows[0]["DcrySignedInvoice"].ToString());
                        oGeneralData.SetProperty("U_DcrptQRCode", einvDT.Rows[0]["DcrySignedQRCode"].ToString());
                        oGeneralData.SetProperty("U_ErrLogId", einvDT.Rows[0]["error_log_ids"].ToString());
                        oGeneralData.SetProperty("U_Einvreqjson", requrl);
                        oGeneralData.SetProperty("U_GenDate", einvDT.Rows[0]["DocDate"].ToString());
                        oGeneralData.SetProperty("U_BaseNo", einvDT.Rows[0]["DocNo"].ToString());
                        oGeneralData.SetProperty("U_BaseEntry", InvDocEntry);
                        oGeneralData.SetProperty("U_DocObjType", obj);
                        oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                        oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                        oGeneralData.SetProperty("U_EwbNum", einvDT.Rows[0]["EwbNo"].ToString());
                        oGeneralData.SetProperty("U_Ewaypdf", einvDT.Rows[0]["pdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwayDetpdf", einvDT.Rows[0]["detailedpdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwbDate", einvDT.Rows[0]["EwbDt"].ToString());
                        oGeneralData.SetProperty("U_EwbValidTill", einvDT.Rows[0]["EwbValidTill"].ToString());
                    }
                    else if (TranType == "E-way")
                    {
                        oGeneralData.SetProperty("U_GenDate", einvDT.Rows[0]["docDate"].ToString());
                        oGeneralData.SetProperty("U_BaseNo", einvDT.Rows[0]["docNo"].ToString());
                        oGeneralData.SetProperty("U_BaseEntry", InvDocEntry);
                        oGeneralData.SetProperty("U_DocObjType", obj);
                        oGeneralData.SetProperty("U_Ewayreqjson", requrl);
                        oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                        oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                        oGeneralData.SetProperty("U_EwbNum", einvDT.Rows[0]["ewayBillNo"].ToString());
                        oGeneralData.SetProperty("U_Ewaypdf", einvDT.Rows[0]["pdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwayDetpdf", einvDT.Rows[0]["detailedpdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwbDate", einvDT.Rows[0]["ewayBillDate"].ToString());
                        oGeneralData.SetProperty("U_EwbValidTill", einvDT.Rows[0]["validUpto"].ToString());
                    }
                    else if (TranType == "E-way IRN")
                    {
                        oGeneralData.SetProperty("U_BaseEntry", InvDocEntry);
                        oGeneralData.SetProperty("U_DocObjType", obj);
                        oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                        oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                        oGeneralData.SetProperty("U_EwbNum", einvDT.Rows[0]["EwbNo"].ToString());
                        oGeneralData.SetProperty("U_Ewaypdf", einvDT.Rows[0]["pdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwayDetpdf", einvDT.Rows[0]["detailedpdfUrl"].ToString());
                        oGeneralData.SetProperty("U_EwbDate", einvDT.Rows[0]["EwbDt"].ToString());
                        oGeneralData.SetProperty("U_EwbValidTill", einvDT.Rows[0]["EwbValidTill"].ToString());
                    }


                    if (TranType == "E-Invoice")
                    {
                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["Irn"].ToString()))
                        {

                            clsModule.objaddon.objglobalmethods.WriteErrorLog(InvDocEntry);

                            objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                            objsalesinvoice.UserFields.Fields.Item("U_IRNNo").Value = columnFind(einvDT, "Irn", 0);
                            objsalesinvoice.UserFields.Fields.Item("U_QRCode").Value = columnFind(einvDT, "SignedQRCode", 0);
                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                            {
                                objsalesinvoice.CreateQRCodeFrom = columnFind(einvDT, "SignedQRCode", 0);
                            }
                            objsalesinvoice.UserFields.Fields.Item("U_AckDate").Value = columnFind(einvDT, "AckDt", 0);
                            objsalesinvoice.UserFields.Fields.Item("U_AckNo").Value = columnFind(einvDT, "AckNo", 0);
                            objsalesinvoice.UserFields.Fields.Item("U_Ewaypdf").Value = columnFind(einvDT, "pdfUrl", 0);
                            objsalesinvoice.UserFields.Fields.Item("U_EwayDetpdf").Value = columnFind(einvDT, "detailedpdfUrl", 0);
                            objsalesinvoice.UserFields.Fields.Item("U_IRNStatus").Value = columnFind(einvDT, "Status", 0);

                            if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbNo"].ToString()))
                            {
                                if (!String.IsNullOrEmpty(clsModule.EwayNo))
                                {
                                    objsalesinvoice.UserFields.Fields.Item(clsModule.EwayNo).Value = columnFind(einvDT, "EwbNo", 0);
                                }
                            }

                            int iErrCode = objsalesinvoice.Update();
                            string strerr = "";
                            if (iErrCode != 0)
                            {
                                clsModule.objaddon.objcompany.GetLastError(out iErrCode, out strerr);
                                clsModule.objaddon.objapplication.MessageBox(strerr);
                            }
                            clsModule.objaddon.objglobalmethods.WriteErrorLog("E invoice Update Successfully");
                            blnRefresh = true;
                        }


                        if (String.IsNullOrEmpty(clsModule.EwayNo))
                        {
                            clsModule.objaddon.objglobalmethods.WriteErrorLog("Eway Details");

                            if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbNo"].ToString()))
                            {
                                switch (ObjType)
                                {
                                    case "WTR":
                                        lstrquery = @"Update " + tb + @" set ""EWayBillNo""='" + einvDT.Rows[0]["EwbNo"].ToString() + "'";
                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbDt"].ToString()))
                                        {
                                            lstrquery += @",""EwbDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["EwbDt"].ToString().Substring(0, 10), "yyyy-MM-dd") + "'";
                                        }
                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbValidTill"].ToString()))
                                        {
                                            if (Getdate(columnFind(einvDT, "EwbValidTill", 0).Substring(0, 10), "yyyy-MM-dd") != new DateTime(1900, 01, 01))
                                            {
                                                objsalesinvoice.EWayBillDetails.EWayBillExpirationDate = Getdate(columnFind(einvDT, "EwbValidTill", 0).Substring(0, 10), "yyyy-MM-dd");
                                            }
                                        }
                                        lstrquery += @" Where ""DocEntry""='" + InvDocEntry + "'";
                                        objRs.DoQuery(lstrquery);
                                        break;
                                    default:
                                        objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                                        objsalesinvoice.EWayBillDetails.EWayBillNo = columnFind(einvDT, "EwbNo", 0);
                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbDt"].ToString()))
                                        {
                                            if (Getdate(columnFind(einvDT, "EwbDt", 0).Substring(0, 10), "yyyy-MM-dd") != new DateTime(1900, 01, 01))
                                            {
                                                objsalesinvoice.EWayBillDetails.EWayBillDate = Getdate(columnFind(einvDT, "EwbDt", 0).Substring(0, 10), "yyyy-MM-dd");
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbValidTill"].ToString()))
                                        {
                                            if (Getdate(columnFind(einvDT, "EwbValidTill", 0).Substring(0, 10), "yyyy-MM-dd") != new DateTime(1900, 01, 01))
                                            {
                                                objsalesinvoice.EWayBillDetails.EWayBillExpirationDate = Getdate(columnFind(einvDT, "EwbValidTill", 0).Substring(0, 10), "yyyy-MM-dd");
                                            }
                                        }
                                        objsalesinvoice.Update();
                                        break;
                                }
                                blnRefresh = true;
                            }
                        }
                    }
                    else if (TranType == "E-way")
                    {
                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["ewayBillNo"].ToString()))
                        {
                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                            {

                                switch (ObjType)
                                {
                                    case "WTR":
                                        lstrquery = @"Update " + tb + @" set ""EWayBillNo""='" + einvDT.Rows[0]["ewayBillNo"].ToString() + "'";
                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["ewayBillDate"].ToString()))
                                        {
                                            lstrquery += @",""EwbDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["ewayBillDate"].ToString().Substring(0, 10), "dd/MM/yyyy") + "'";
                                        }
                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["validUpto"].ToString()))
                                        {
                                            lstrquery += @",""ExpireDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["validUpto"].ToString().Substring(0, 10), "dd/MM/yyyy") + "'";
                                        }
                                        lstrquery += @" Where ""DocEntry""='" + InvDocEntry + "'";
                                        objRs.DoQuery(lstrquery);
                                        break;
                                    default:

                                        objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                                        objsalesinvoice.EWayBillDetails.EWayBillNo = columnFind(einvDT, "ewayBillNo", 0);

                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["ewayBillDate"].ToString()))
                                        {
                                            if (Getdate(columnFind(einvDT, "ewayBillDate", 0).Substring(0, 10), "dd/MM/yyyy") != new DateTime(1900, 01, 01))
                                            {
                                                objsalesinvoice.EWayBillDetails.EWayBillDate = Getdate(columnFind(einvDT, "ewayBillDate", 0).Substring(0, 10), "dd/MM/yyyy");
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["validUpto"].ToString()))
                                        {
                                            if (Getdate(columnFind(einvDT, "validUpto", 0).Substring(0, 10), "dd/MM/yyyy") != new DateTime(1900, 01, 01))
                                            {
                                                objsalesinvoice.EWayBillDetails.EWayBillExpirationDate = Getdate(columnFind(einvDT, "validUpto", 0).Substring(0, 10), "dd/MM/yyyy");
                                            }
                                        }
                                        objsalesinvoice.Update();
                                        blnRefresh = true;
                                        break;
                                }

                            }
                            else
                            {
                                switch (ObjType)
                                {
                                    case "WTR":
                                        lstrquery = @"Update " + maintb + @" Set ";
                                        lstrquery += @"""" + clsModule.EwayNo + @"""='" + einvDT.Rows[0]["EwbNo"].ToString() + "'";
                                        lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                                        objRs.DoQuery(lstrquery);
                                        break;
                                    default:
                                        objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                                        objsalesinvoice.UserFields.Fields.Item(clsModule.EwayNo).Value = columnFind(einvDT, "EwbNo", 0);
                                        objsalesinvoice.Update();
                                        break;
                                }
                                blnRefresh = true;
                            }

                            switch (ObjType)
                            {
                                case "WTR":
                                    lstrquery = @"Update " + maintb + @" Set ";
                                    lstrquery += @"""U_Ewaypdf""='" + einvDT.Rows[0]["pdfUrl"].ToString() + "',";
                                    lstrquery += @"""U_EwayDetpdf""='" + einvDT.Rows[0]["detailedpdfUrl"].ToString() + "'";
                                    lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                                    objRs.DoQuery(lstrquery);
                                    break;
                                default:
                                    objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                                    objsalesinvoice.UserFields.Fields.Item("U_Ewaypdf").Value = columnFind(einvDT, "pdfUrl", 0);
                                    objsalesinvoice.UserFields.Fields.Item("U_EwayDetpdf").Value = columnFind(einvDT, "detailedpdfUrl", 0);
                                    objsalesinvoice.Update();
                                    break;
                            }
                        }
                    }
                    else if (TranType == "E-way IRN")
                    {
                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbNo"].ToString()))
                        {
                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                            {
                                switch (ObjType)
                                {
                                    case "WTR":
                                        lstrquery = @"Update " + tb + @" set ""EWayBillNo""='" + einvDT.Rows[0]["EwbNo"].ToString() + "'";
                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbDt"].ToString()))
                                        {
                                            lstrquery += @",""EwbDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["EwbDt"].ToString().Substring(0, 10), "yyyy-MM-dd") + "'";
                                        }
                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbValidTill"].ToString()))
                                        {
                                            lstrquery += @",""ExpireDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["EwbValidTill"].ToString().Substring(0, 10), "yyyy-MM-dd") + "'";
                                        }
                                        lstrquery += @" Where ""DocEntry""='" + InvDocEntry + "'";
                                        objRs.DoQuery(lstrquery);
                                        break;
                                    default:
                                        objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                                        objsalesinvoice.EWayBillDetails.EWayBillNo = columnFind(einvDT, "EwbNo", 0);
                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbDt"].ToString()))
                                        {
                                            if (Getdate(columnFind(einvDT, "EwbDt", 0).Substring(0, 10), "yyyy-MM-dd") != new DateTime(1900, 01, 01))
                                            {
                                                objsalesinvoice.EWayBillDetails.EWayBillDate = Getdate(columnFind(einvDT, "EwbDt", 0).Substring(0, 10), "yyyy-MM-dd");
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbValidTill"].ToString()))
                                        {
                                            if (Getdate(columnFind(einvDT, "EwbValidTill", 0).Substring(0, 10), "yyyy-MM-dd") != new DateTime(1900, 01, 01))
                                            {
                                                objsalesinvoice.EWayBillDetails.EWayBillExpirationDate = Getdate(columnFind(einvDT, "EwbValidTill", 0).Substring(0, 10), "yyyy-MM-dd");
                                            }
                                        }
                                        objsalesinvoice.Update();
                                        break;
                                }
                                blnRefresh = true;
                            }
                            else
                            {
                                switch (ObjType)
                                {
                                    case "WTR":
                                        lstrquery = @"Update " + maintb + @" set """ + clsModule.EwayNo + @""" = '" + einvDT.Rows[0]["EwbNo"].ToString() + "' ";
                                        lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                                        objRs.DoQuery(lstrquery);
                                        break;
                                    default:
                                        objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                                        objsalesinvoice.UserFields.Fields.Item(clsModule.EwayNo).Value = columnFind(einvDT, "EwbNo", 0);
                                        objsalesinvoice.Update();
                                        break;
                                }
                                blnRefresh = true;
                            }

                            switch (ObjType)
                            {
                                case "WTR":

                                    lstrquery = @"Update " + maintb + @" Set ";
                                    lstrquery += @"""U_Ewaypdf""='" + einvDT.Rows[0]["pdfUrl"].ToString() + "',";
                                    lstrquery += @"""U_EwayDetpdf""='" + einvDT.Rows[0]["detailedpdfUrl"].ToString() + "'";
                                    if (!string.IsNullOrEmpty(columnFind(einvDT, "Status", 0)))
                                    {
                                        lstrquery += @",""U_EwayStatus""='" + einvDT.Rows[0]["Status"].ToString() + "'";
                                    }
                                    lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                                    objRs.DoQuery(lstrquery);
                                    break;
                                default:
                                    objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                                    objsalesinvoice.UserFields.Fields.Item("U_Ewaypdf").Value = columnFind(einvDT, "pdfUrl", 0);
                                    objsalesinvoice.UserFields.Fields.Item("U_EwayDetpdf").Value = columnFind(einvDT, "detailedpdfUrl", 0);
                                    if (!string.IsNullOrEmpty(columnFind(einvDT, "Status", 0)))
                                    {
                                        objsalesinvoice.UserFields.Fields.Item("U_EwayStatus").Value = columnFind(einvDT, "Status", 0);
                                    }
                                    objsalesinvoice.Update();
                                    break;
                            }
                        }
                    }
                }
                else if (Type == "Cancel")
                {
                    oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                    oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                    oGeneralData.SetProperty("U_CanDate", einvDT.Rows[0]["CancelDate"].ToString());
                    oGeneralData.SetProperty("U_ErrLogId", einvDT.Rows[0]["error_log_id"].ToString());

                    if (TranType == "E-Invoice")
                    {

                        if (einvDT.Rows[0]["flag"].ToString() == "True")
                        {
                            switch (ObjType)
                            {
                                case "WTR":
                                    strSQL = @"Update " + maintb + @" Set ""U_IRNNo""='',""U_QRCode""=''";
                                    if (String.IsNullOrEmpty(clsModule.EwayNo))
                                    {
                                        strSQL += @",""QRCodeSrc""=''";
                                    }
                                    strSQL += @",""U_AckDate""='',""U_AckNo""='',""U_EwayDetpdf""='',""U_Ewaypdf""='' where ""DocEntry""='" + InvDocEntry + "'";
                                    objRs.DoQuery(strSQL);
                                    break;
                                default:
                                    objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                                    objsalesinvoice.UserFields.Fields.Item("U_IRNNo").Value = "";
                                    objsalesinvoice.UserFields.Fields.Item("U_QRCode").Value = "";
                                    if (String.IsNullOrEmpty(clsModule.EwayNo))
                                    {
                                        objsalesinvoice.CreateQRCodeFrom = "";
                                    }
                                    objsalesinvoice.UserFields.Fields.Item("U_AckDate").Value = "";
                                    objsalesinvoice.UserFields.Fields.Item("U_AckNo").Value = "";
                                    objsalesinvoice.UserFields.Fields.Item("U_Ewaypdf").Value = "";
                                    objsalesinvoice.UserFields.Fields.Item("U_EwayDetpdf").Value = "";
                                    objsalesinvoice.Update();
                                    break;
                            }
                            blnRefresh = true;
                        }
                    }
                    else if (TranType == "E-way")
                    {

                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["ewayBillNo"].ToString()))
                        {
                            if (String.IsNullOrEmpty(clsModule.EwayNo))
                            {
                                switch (ObjType)
                                {
                                    case "WTR":
                                        strSQL = @"Update " + tb + @" set ""EWayBillNo""='',";
                                        strSQL += @"""EwbDate""='',";
                                        strSQL += @"""ExpireDate""=''";
                                        strSQL += @"Where ""DocEntry""='" + InvDocEntry + "'";
                                        break;
                                    default:
                                        objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                                        objsalesinvoice.EWayBillDetails.EWayBillNo = "";
                                        objsalesinvoice.EWayBillDetails.EWayBillDate = new DateTime(1900, 01, 01);
                                        objsalesinvoice.EWayBillDetails.EWayBillExpirationDate = new DateTime(1900, 01, 01);
                                        break;
                                }
                            }
                            else
                            {
                                switch (ObjType)
                                {
                                    case "WTR":
                                        strSQL = @"Update " + maintb + @" set """ + clsModule.EwayNo + @"""=''";
                                        strSQL += @"Where ""DocEntry""='" + InvDocEntry + "'";
                                        break;
                                    default:
                                        objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                                        objsalesinvoice.UserFields.Fields.Item(clsModule.EwayNo).Value = "";
                                        break;
                                }
                            }
                        }

                        if (einvDT.Rows[0]["flag"].ToString() == "True")
                        {
                            switch (ObjType)
                            {
                                case "WTR":
                                    objRs.DoQuery(strSQL);
                                    blnRefresh = true;
                                    strSQL = @"Update " + maintb + @" Set ";
                                    strSQL += @"""U_Ewaypdf""='',";
                                    strSQL += @"""U_EwayDetpdf""='',";
                                    strSQL += @"""U_EwayStatus""='CNL'";
                                    strSQL += @"Where ""DocEntry""='" + InvDocEntry + "'";
                                    objRs.DoQuery(strSQL);
                                    break;
                                default:
                                    objsalesinvoice.Update();
                                    objsalesinvoice.GetByKey(clsModule.objaddon.objglobalmethods.Ctoint(InvDocEntry));
                                    objsalesinvoice.UserFields.Fields.Item("U_Ewaypdf").Value = "";
                                    objsalesinvoice.UserFields.Fields.Item("U_EwayDetpdf").Value = "";
                                    objsalesinvoice.UserFields.Fields.Item("U_EwayStatus").Value = "CNL";
                                    objsalesinvoice.Update();
                                    break;
                            }
                        }
                    }
                }

                objRs = null;
                if (Flag == true)
                {
                    oGeneralService.Update(oGeneralData);
                    return true;
                }
                else
                {
                    oGeneralParams = oGeneralService.Add(oGeneralData);
                    return true;
                }
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objglobalmethods.WriteErrorLog(ex.ToString());
                clsModule.objaddon.objapplication.StatusBar.SetText("E_Invoice_Logs: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
        }

        public class login
        {
            public string CompanyDB { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }


        }

        private DataTable Get_API_Response(string JSON, string URL, string httpMethod = "POST", string contenttype = "application/json")
        {
            try
            {
                clsModule.objaddon.objglobalmethods.WriteErrorLog(URL);
                clsModule.objaddon.objglobalmethods.WriteErrorLog(JSON);

                DataTable datatable = new DataTable();
                HttpWebRequest webRequest;
                webRequest = (HttpWebRequest)WebRequest.Create(URL);
                webRequest.Method = httpMethod;
                webRequest.ContentType = contenttype;
                byte[] byteArray = Encoding.UTF8.GetBytes(JSON);
                webRequest.ContentLength = byteArray.Length;
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(byteArray, 0, byteArray.Length);
                }
                // Get the response.
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
                return datatable;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string GSTSales(JsonData IRNJSON)
        {
            GSTSales gSTSales = new GSTSales();

            gSTSales.BuyerDtls.CusCode = IRNJSON.BuyerDtls.CusCode;
            gSTSales.BuyerDtls.Em= IRNJSON.BuyerDtls.CusCode;
            gSTSales.BuyerDtls.Gstin= IRNJSON.BuyerDtls.Gstin;
            gSTSales.BuyerDtls.LglNm = IRNJSON.BuyerDtls.LglNm;
            gSTSales.BuyerDtls.Pos = IRNJSON.BuyerDtls.Pos;
            gSTSales.BuyerDtls.Stcd = IRNJSON.BuyerDtls.Stcd;
            


            gSTSales.DocDtls.Dt = IRNJSON.DocDtls.Dt;
            gSTSales.DocDtls.No = IRNJSON.DocDtls.No;
            gSTSales.DocDtls.Typ = IRNJSON.DocDtls.Typ;


            gSTSales.EwbDtls.EwayBillDate = IRNJSON.EwbDtls.EwayBillDate;
            gSTSales.EwbDtls.EwayBillNo = IRNJSON.EwbDtls.EwayBillNo;

            gSTSales.ExpDtls.ShipBDt = IRNJSON.ExpDtls.ShipBDt;
            gSTSales.ExpDtls.ShipBNo = IRNJSON.ExpDtls.ShipBNo;



            gSTSales.Extdtls.irndate = IRNJSON.Extdtls.irndate;
            gSTSales.Extdtls.irnnumber = IRNJSON.Extdtls.irnnumber;

            gSTSales.SellerDtls.Em = IRNJSON.SellerDtls.Em;
            gSTSales.SellerDtls.Gstin = IRNJSON.SellerDtls.Gstin;
            gSTSales.SellerDtls.LglNm = IRNJSON.SellerDtls.LglNm;
            gSTSales.SellerDtls.Stcd = IRNJSON.SellerDtls.Stcd;


            gSTSales.TranDtls.IgstOnIntra = IRNJSON.TranDtls.IgstOnIntra;
            gSTSales.TranDtls.RegRev = IRNJSON.TranDtls.RegRev;
            gSTSales.TranDtls.SupTyp ="R";




            gSTSales.ValDtls.AssVal = IRNJSON.ValDtls.AssVal;
            gSTSales.ValDtls.CesNonAdVal = IRNJSON.ValDtls.CesNonAdVal;
            gSTSales.ValDtls.CesVal = IRNJSON.ValDtls.CesVal;
            gSTSales.ValDtls.CgstVal = IRNJSON.ValDtls.CgstVal;
            gSTSales.ValDtls.Discount = IRNJSON.ValDtls.Discount;
            gSTSales.ValDtls.IgstVal = IRNJSON.ValDtls.IgstVal;
            
            gSTSales.ValDtls.OthChrg = IRNJSON.ValDtls.OthChrg;
            gSTSales.ValDtls.RndOffAmt = IRNJSON.ValDtls.RndOffAmt;
            gSTSales.ValDtls.SgstVal = IRNJSON.ValDtls.SgstVal;
            gSTSales.ValDtls.StCesVal = IRNJSON.ValDtls.StCesVal;
            gSTSales.ValDtls.TotInvVal = IRNJSON.ValDtls.TotInvVal;
            gSTSales.ValDtls.TotInvValFc = IRNJSON.ValDtls.TotInvValFc;


            gSTSales.Version = IRNJSON.Version;

            foreach (var item in IRNJSON.ItemList)
            {
                gSTSales.ItemList.Add(new GItemList
                {
                    AssAmt = item.AssAmt,
                    CesAmt = item.CesAmt,
                    CesNonAdvlAmt = item.CesNonAdvlAmt,
                    CesRt = item.CesRt,
                    CgstAmt = item.CgstAmt,
                    GstRt = item.GstRt,
                    HsnCd = item.HsnCd,
                    IgstAmt = item.IgstAmt,
                    Qty = item.Qty,
                    SgstAmt = item.SgstAmt,
                    SlNo = item.SlNo,
                    StateCesAmt = item.StateCesAmt,
                    StateCesNonAdvlAmt = item.StateCesNonAdvlAmt,
                    StateCesRt = item.StateCesRt,
                    TotItemVal = item.TotItemVal,
                    Unit = item.Unit,
                    UnitPrice = item.UnitPrice
                });
            }

            return JsonConvert.SerializeObject(gSTSales);

        }



    }
}
