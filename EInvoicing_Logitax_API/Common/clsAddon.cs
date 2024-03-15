using EInvoicing_Logitax_API.Business_Objects;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Data;

namespace EInvoicing_Logitax_API.Common
{
    class clsAddon
    {
        public clsMenuEvent objmenuevent;
        public SAPbouiCOM.Application objapplication;
        public SAPbobsCOM.Company objcompany;
        SAPbouiCOM.EventFilter oFilter;
        public clsRightClickEvent objrightclickevent;
        public clsGlobalMethods objglobalmethods;
        public ClsARInvoice objInvoice;              
        public string[] HWKEY = { "L1653539483", "X1211807750","K1600107675" };
        #region Constructor
        public clsAddon()
        {

        }
        #endregion

        public void Intialize(string[] args)
        {
            try
            {
                Application oapplication;
                if ((args.Length < 1))
                    oapplication = new Application();
                else
                    oapplication = new Application(args[0]);
                objapplication = Application.SBO_Application;

              

              
                if (isValidLicense())
                {
                    objapplication.StatusBar.SetText("Establishing Company Connection Please Wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    objcompany = (SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany();

                    Create_DatabaseFields(); // UDF & UDO Creation Part    
                    Menu(); // Menu Creation Part
                    Create_Objects(); // Object Creation Part




                    string strSQL = @"Select  ""U_DBType"" ,""U_EwayNo"" ,""U_VehNo"" ,""U_TransID"" ,""U_TransName"",""U_Distance"",""U_ItemDesc"",""U_GSTCol"" ";
                    strSQL += @" from ""@ATEICFG"" where ""Code""='01'";

                    DataTable dt = objglobalmethods.GetmultipleValue(strSQL);
                    if (dt.Rows.Count > 0)
                    {
                        clsModule.HANA = Convert.ToString(dt.Rows[0]["U_DBType"]) == "Y" ? true : false;
                        clsModule.EwayNo = Convert.ToString(dt.Rows[0]["U_EwayNo"]);
                        clsModule.EwayUDF = Convert.ToString(dt.Rows[0]["U_VehNo"]);
                        clsModule.EwayTransportId = Convert.ToString(dt.Rows[0]["U_TransID"]);
                        clsModule.EwayTransportName = Convert.ToString(dt.Rows[0]["U_TransName"]);
                        clsModule.EwayDistance = Convert.ToString(dt.Rows[0]["U_Distance"]);
                        clsModule.ItemDsc = Convert.ToString(dt.Rows[0]["U_ItemDesc"]);
                        clsModule.GSTCol = Convert.ToString(dt.Rows[0]["U_GSTCol"]);

                    }

                    if (string.IsNullOrEmpty(clsModule.EwayNo))
                    {
                        Create_DatabaseFieldsAfter();
                    }

                    SetFilters();
                    objapplication.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(objapplication_AppEvent);
                    objapplication.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(objapplication_MenuEvent);
                    objapplication.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(objapplication_ItemEvent);               
                    objapplication.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(ref FormDataEvent);
                    objapplication.RightClickEvent += new SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler(objapplication_RightClickEvent);

                  

                    objapplication.StatusBar.SetText("Addon Connected Successfully..!!!"+ (clsModule.HANA ?"HANA":"SQL"), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    oapplication.Run();
                }
                else
                {
                    objapplication.StatusBar.SetText("Addon Disconnected due to license mismatch..!!!", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return;
                }
            }          
            catch (Exception ex)
            {
                objapplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        public bool isValidLicense()
        {
            return true;
            try
            {
                if (clsModule.HANA)
                {
                    try
                    {
                        if (objapplication.Forms.ActiveForm.TypeCount > 0)
                        {
                            for (int i = 0; i <= objapplication.Forms.ActiveForm.TypeCount - 1; i++)
                                objapplication.Forms.ActiveForm.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                objapplication.Menus.Item("257").Activate();
                SAPbouiCOM.EditText objedit = (SAPbouiCOM.EditText)objapplication.Forms.ActiveForm.Items.Item("79").Specific;

                string CrrHWKEY = objedit.Value.ToString();
                objapplication.Forms.ActiveForm.Close();

                for (int i = 0; i <= HWKEY.Length - 1; i++)
                {
                    if (HWKEY[i] == CrrHWKEY)
                    {
                        return true;
                    }

                }

                System.Windows.Forms.MessageBox.Show("Installing Add-On failed due to License mismatch");
                return false;
            }
            catch (Exception ex)
            {
                objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            return true;
        }

        private void FilterForm(SAPbouiCOM.EventFilters oFilters,  SAPbouiCOM.BoEventTypes eventFilter, List<string> Forms)
        {
            oFilter= oFilters.Add(eventFilter);
            foreach (string item in Forms)
            {
                oFilter.AddEx(item);
            }
        }
        private void SetFilters()
        {
            SAPbouiCOM.EventFilters oFilters;                       
            oFilters = new SAPbouiCOM.EventFilters();

            List<string> array =new List<string>() ;

            if (string.IsNullOrEmpty(clsModule.EwayNo))
            {
                array.AddRange(new[] { "133", "179", "940", "140", "141" } );            
            }
            else
            {
                array.AddRange(new[] { "133", "179",  "140", "141" });
            }

            List<string>  Allevent =new List<string> (new []{ "GSTUPLOAD", "EINV", "EINVDIS", "UOMMAP", "138", "256000803", "FormSelect" });
            


            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_CLICK, array);
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD, array);
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE, array);
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_FORM_LOAD, array);
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_GOT_FOCUS, array);
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_FORM_CLOSE, array);
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_FORM_DRAW, array);
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_COMBO_SELECT, array);
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_FORM_ACTIVATE, array);            
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD, array);                                    
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST, array);                                    
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_KEY_DOWN, array);                                    
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_LOST_FOCUS, array);                                    
            FilterForm(oFilters,  SAPbouiCOM.BoEventTypes.et_ALL_EVENTS, Allevent);        
            
            objapplication.SetFilter(oFilters);

        }
        public void Create_Objects()
        {
            objmenuevent = new clsMenuEvent();
            objrightclickevent = new clsRightClickEvent();
            objglobalmethods = new clsGlobalMethods();
            objInvoice = new ClsARInvoice();          
        }

        private void Create_DatabaseFields()
        {
            try
            {
                objapplication.StatusBar.SetText("Creating Database Fields.Please Wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                var objtable = new clsTable();
                objtable.FieldCreation();
                objapplication.StatusBar.SetText(" Database Created Successfully...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            catch (Exception)
            {
                
            }
           
        }

        private void Create_DatabaseFieldsAfter()
        {
            try
            {
                objapplication.StatusBar.SetText("Creating Database Fields.Please Wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            var objtable = new clsTable();
            objtable.FieldCreationNewversion();
            objapplication.StatusBar.SetText(" Database Created Successfully...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            catch (Exception)
            {

            }

        }

        #region Menu Creation Details

        private void Menu()
        {
            int Menucount = 0;
            if (objapplication.Menus.Item("43545").SubMenus.Exists("E-Invoice"))
                return;
            Menucount = objapplication.Menus.Item("43545").SubMenus.Count;
            Menucount += 1;
            CreateMenu("", Menucount, "E-Invoice", SAPbouiCOM.BoMenuType.mt_STRING, "EINV", "43545");
            Menucount += 1;
            CreateMenu("", Menucount, "UOM Mapping", SAPbouiCOM.BoMenuType.mt_STRING, "UOMMAP", "43545");
            Menucount += 1;
           CreateMenu("", Menucount, "GST Upload", SAPbouiCOM.BoMenuType.mt_STRING, "GSTUPLOAD", "43545");
        }

        private void CreateMenu(string ImagePath, int Position, string DisplayName, SAPbouiCOM.BoMenuType MenuType, string UniqueID, string ParentMenuID)
        {
            try
            {
                SAPbouiCOM.MenuCreationParams oMenuPackage;
                SAPbouiCOM.MenuItem parentmenu;
                parentmenu = objapplication.Menus.Item(ParentMenuID);
                if (parentmenu.SubMenus.Exists(UniqueID.ToString()))
                    return;
                oMenuPackage = (SAPbouiCOM.MenuCreationParams)objapplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams);
                oMenuPackage.Image = ImagePath;
                oMenuPackage.Position = Position;
                oMenuPackage.Type = MenuType;
                oMenuPackage.UniqueID = UniqueID;
                oMenuPackage.String = DisplayName;
                parentmenu.SubMenus.AddEx(oMenuPackage);
            }
            catch (Exception ex)
            {
                objapplication.StatusBar.SetText("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_None);
            }
        }

        #endregion

        public bool FormExist(string FormID)
        {
            bool FormExistRet = false;
            try
            {
                FormExistRet = false;
                foreach (SAPbouiCOM.Form uid in clsModule.objaddon.objapplication.Forms)
                {
                    if (uid.TypeEx == FormID)
                    {
                        FormExistRet = true;
                        break;
                    }
                }
                if (FormExistRet)
                {
                    clsModule.objaddon.objapplication.Forms.Item(FormID).Visible = true;
                    clsModule.objaddon.objapplication.Forms.Item(FormID).Select();
                }
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }

            return FormExistRet;

        }

        #region VIRTUAL FUNCTIONS
        public virtual void Menu_Event(ref SAPbouiCOM.MenuEvent pVal, ref bool BubbleEvent)
        { }

        public virtual void Item_Event(string oFormUID, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        { }

        public virtual void RightClick_Event(ref SAPbouiCOM.ContextMenuInfo oEventInfo, ref bool BubbleEvent)
        { }

        public virtual void FormData_Event(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        { }


        #endregion

        #region ItemEvent

        private void objapplication_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
              
                if (pVal.BeforeAction)
                {
                    objInvoice.Item_Event(FormUID, ref pVal, ref BubbleEvent);

                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                {
                                    SAPbouiCOM.BoEventTypes EventEnum;
                                    EventEnum = pVal.EventType;
                                    break;
                                }
                            case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                                {                                   
                                    break;
                                }
                            case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT:
                                {
                                    break;
                                }
                            
                        }
                    }

                }
                else
                {
                    objInvoice.Item_Event(FormUID, ref pVal, ref BubbleEvent);
                    switch (pVal.FormTypeEx)
                    {
                        case "133":
                        case "-133":
                        case "179":
                        case "-179":
                            objInvoice.EnabledMenu(pVal.FormTypeEx);
                            break;
                    }
                    switch (pVal.EventType)
                    {
                       
                        case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                            {
                                break;
                            }
                        case SAPbouiCOM.BoEventTypes.et_CLICK:
                            {
                              
                                break;
                            }
                    }
                }

            }
            catch (Exception ex)
            {
                return;
            }
        }

        #endregion

        #region FormDataEvent

        private void FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {               
            }
            catch (Exception ex)
            {
               //throw ex;
            }
        }

        #endregion
        
        #region MenuEvent
        private void objapplication_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.BeforeAction == false)
            {
                switch (pVal.MenuUID)
                {
                    case "EINV":
                        Einvoice Einvoice = new Einvoice();
                        Einvoice.Show();
                        break;
                    case "EINVDIS":
                        EInvoice_Display EInvoice_Display = new EInvoice_Display();
                        EInvoice_Display.Show();
                        break;
                    case "UOMMAP":
                        UOMMapping UOMMapping = new UOMMapping();
                        UOMMapping.Show();
                        break;
                    case "GSTUPLOAD":
                        GST_Upload gstupload = new GST_Upload();
                        gstupload.Show();
                        break;
                    case "1292":
                       SAPbouiCOM.Form objform = clsModule.objaddon.objapplication.Forms.ActiveForm;
                        string ss=objform.ActiveItem;
                        clsModule.objaddon.objglobalmethods.Matrix_Addrow((SAPbouiCOM.Matrix)objapplication.Forms.ActiveForm.Items.Item(ss).Specific, "#", "#");
                        break;
                    case "1287":
                        SAPbouiCOM.Form activefrm = clsModule.objaddon.objapplication.Forms.ActiveForm;
                        switch (activefrm.Type.ToString())
                        {
                            case "133":
                            case "179":
                                Cleartext(activefrm);
                                activefrm.Items.Item("112").Click();
                                break;

                        }
                        break;


                }
            }          
        }

        #endregion
        public void Cleartext(SAPbouiCOM.Form oForm)
        {
            switch (oForm.Type.ToString())
            {
                case "133":
                case "179":
                    SAPbouiCOM.Form oUDFForm = clsModule.objaddon.objapplication.Forms.Item(oForm.UDFFormUID);
                    clsModule.objaddon.objInvoice.EnabledMenu(oUDFForm.TypeEx, true, oForm.UDFFormUID);
                    ((SAPbouiCOM.EditText)oUDFForm.Items.Item("U_IRNNo").Specific).String = "";
                    ((SAPbouiCOM.EditText)oUDFForm.Items.Item("U_QRCode").Specific).String = "";
                    ((SAPbouiCOM.EditText)oUDFForm.Items.Item("U_AckDate").Specific).String = "";
                    ((SAPbouiCOM.EditText)oUDFForm.Items.Item("U_AckNo").Specific).String = "";                    
                    ((SAPbouiCOM.EditText)oUDFForm.Items.Item("U_IRNStatus").Specific).String = "";                    
                    ((SAPbouiCOM.EditText)oUDFForm.Items.Item("U_Ewaypdf").Specific).String = "";                    
                    ((SAPbouiCOM.EditText)oUDFForm.Items.Item("U_EwayDetpdf").Specific).String = "";                    
                    break;                               
            }

        }
        #region RightClickEvent

        private void objapplication_RightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                switch (objapplication.Forms.ActiveForm.TypeEx)
                {
                    case "138":
                        objrightclickevent.RightClickEvent(ref eventInfo, ref BubbleEvent);
                        break;
                    case "UOMMAP":
                        objrightclickevent.RightClickEvent(ref eventInfo, ref BubbleEvent);
                        break;
                }

            }
            catch (Exception ex) { }

        }

        #endregion

        #region AppEvent

        private void objapplication_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    try
                    {
                        System.Windows.Forms.Application.Exit();
                        if (objapplication != null)
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(objapplication);
                        if (objcompany != null)
                        {
                            if (objcompany.Connected)
                                objcompany.Disconnect();
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(objcompany);
                        }
                        GC.Collect();

                    }
                    catch (Exception ex)
                    {
                    }
                    break;

            }
        }

        #endregion

    }


}
