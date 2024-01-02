using System;
using SAPbouiCOM.Framework;
using EInvoicing_Logitax_API.Common;
using SAPbobsCOM;
using System.Data;  

namespace EInvoicing_Logitax_API.Business_Objects
{
    
    [FormAttribute("138", "Business_Objects/SysGenSettings.b1f")]
    class SysGenSettings : SystemFormBase
    {
        private string FormName = "138";
        private string strSQL;              
        public static SAPbouiCOM.Form oForm;
        private bool loaddata = false;

        #region "DESIGN PART"
        public override void OnInitializeComponent()
        {
            this.Folder0 = ((SAPbouiCOM.Folder)(this.GetItem("feinvcfg").Specific));
            this.Folder0.PressedAfter += new SAPbouiCOM._IFolderEvents_PressedAfterEventHandler(this.Folder0_PressedAfter);
            this.Matrix0 = ((SAPbouiCOM.Matrix)(this.GetItem("mtxconfig").Specific));
            this.Matrix0.ValidateAfter += new SAPbouiCOM._IMatrixEvents_ValidateAfterEventHandler(this.Matrix0_ValidateAfter);
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("lcntcod").Specific));
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("tcntcod").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("lusrcod").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("tusrcod").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_6").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("tpswd").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);
            this.Button0.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.Button0_ClickAfter);
            this.Folder1 = ((SAPbouiCOM.Folder)(this.GetItem("129").Specific));
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("luburl").Specific));
            this.EditText4 = ((SAPbouiCOM.EditText)(this.GetItem("tuburl").Specific));
            this.StaticText5 = ((SAPbouiCOM.StaticText)(this.GetItem("llburl").Specific));
            this.EditText5 = ((SAPbouiCOM.EditText)(this.GetItem("tlburl").Specific));
            this.OptionBtn0 = ((SAPbouiCOM.OptionBtn)(this.GetItem("ouat").Specific));
            this.OptionBtn1 = ((SAPbouiCOM.OptionBtn)(this.GetItem("olive").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("LHSN").Specific));
            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("thsnL").Specific));
            this.StaticText7 = ((SAPbouiCOM.StaticText)(this.GetItem("LSERCON").Specific));
            this.EditText7 = ((SAPbouiCOM.EditText)(this.GetItem("TSERCON").Specific));
            this.OptionBtn4 = ((SAPbouiCOM.OptionBtn)(this.GetItem("RHANA").Specific));
            this.OptionBtn5 = ((SAPbouiCOM.OptionBtn)(this.GetItem("RSQL").Specific));
            this.StaticText6 = ((SAPbouiCOM.StaticText)(this.GetItem("LEwayno").Specific));
            this.EditText6 = ((SAPbouiCOM.EditText)(this.GetItem("TUEwayNo").Specific));
            this.StaticText8 = ((SAPbouiCOM.StaticText)(this.GetItem("Lvehno").Specific));
            this.EditText8 = ((SAPbouiCOM.EditText)(this.GetItem("TUVehno").Specific));
            this.StaticText9 = ((SAPbouiCOM.StaticText)(this.GetItem("LtransID").Specific));
            this.EditText9 = ((SAPbouiCOM.EditText)(this.GetItem("TUtransID").Specific));
            this.StaticText11 = ((SAPbouiCOM.StaticText)(this.GetItem("LTranNm").Specific));
            this.EditText11 = ((SAPbouiCOM.EditText)(this.GetItem("TUtransNM").Specific));
            this.EditText11.KeyDownAfter += new SAPbouiCOM._IEditTextEvents_KeyDownAfterEventHandler(this.EditText11_KeyDownAfter);
            this.StaticText12 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_15").Specific));
            this.EditText10 = ((SAPbouiCOM.EditText)(this.GetItem("TDistance").Specific));
            this.StaticText10 = ((SAPbouiCOM.StaticText)(this.GetItem("LDistance").Specific));
            this.Folder2 = ((SAPbouiCOM.Folder)(this.GetItem("fgstcfg").Specific));
            this.Folder2.PressedAfter += new SAPbouiCOM._IFolderEvents_PressedAfterEventHandler(this.Folder2_PressedAfter);
            this.Folder2.ClickAfter += new SAPbouiCOM._IFolderEvents_ClickAfterEventHandler(this.Folder2_ClickAfter);
            this.StaticText13 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.EditText12 = ((SAPbouiCOM.EditText)(this.GetItem("TCCde").Specific));
            this.StaticText14 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_3").Specific));
            this.EditText13 = ((SAPbouiCOM.EditText)(this.GetItem("TUCode").Specific));
            this.StaticText15 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_5").Specific));
            this.EditText14 = ((SAPbouiCOM.EditText)(this.GetItem("TPass").Specific));
            this.StaticText16 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_8").Specific));
            this.EditText15 = ((SAPbouiCOM.EditText)(this.GetItem("TUUrl").Specific));
            this.StaticText17 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_10").Specific));
            this.EditText16 = ((SAPbouiCOM.EditText)(this.GetItem("TLUrl").Specific));
            this.OptionBtn2 = ((SAPbouiCOM.OptionBtn)(this.GetItem("GRUAT").Specific));
            this.OptionBtn3 = ((SAPbouiCOM.OptionBtn)(this.GetItem("GRLIVE").Specific));
            this.EditText17 = ((SAPbouiCOM.EditText)(this.GetItem("ELTok").Specific));
            this.StaticText18 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_16").Specific));
            this.Matrix3 = ((SAPbouiCOM.Matrix)(this.GetItem("Item_7").Specific));
            this.Matrix3.PressedAfter += new SAPbouiCOM._IMatrixEvents_PressedAfterEventHandler(this.Matrix3_PressedAfter);
            this.Matrix3.ValidateAfter += new SAPbouiCOM._IMatrixEvents_ValidateAfterEventHandler(this.Matrix3_ValidateAfter);
            this.CheckBox0 = ((SAPbouiCOM.CheckBox)(this.GetItem("Chkgetcmp").Specific));
            this.CheckBox1 = ((SAPbouiCOM.CheckBox)(this.GetItem("ChkGetBP").Specific));
            this.OnCustomInitialize();

        }

        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new SAPbouiCOM.Framework.FormBase.LoadAfterHandler(this.Form_LoadAfter);

        }
        #region Fields        
        private SAPbouiCOM.Folder Folder0;
        private SAPbouiCOM.Matrix Matrix0;
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.Button Button0;
        private SAPbouiCOM.Folder Folder1;
        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.EditText EditText4;
        private SAPbouiCOM.StaticText StaticText5;
        private SAPbouiCOM.EditText EditText5;
        private SAPbouiCOM.OptionBtn OptionBtn0;
        private SAPbouiCOM.OptionBtn OptionBtn1;
        #endregion
        #endregion

        private void OnCustomInitialize()
        {
            Folder0.GroupWith("129");            
            Folder2.GroupWith("129");            
            clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix0, "crurl", "#");
            OptionBtn0.GroupWith("olive");
            OptionBtn0.Item.Height = OptionBtn0.Item.Height + 2;
            OptionBtn0.Item.Width = OptionBtn0.Item.Width + 20;
            OptionBtn1.Item.Height = OptionBtn1.Item.Height + 2;
            OptionBtn1.Item.Width = OptionBtn1.Item.Width + 20;
            Folder1.Item.Click(SAPbouiCOM.BoCellClickType.ct_Regular);
            Matrix0.Columns.Item("urltype").ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
            Matrix0.Columns.Item("type").ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;

            OptionBtn4.GroupWith("RSQL");


            OptionBtn2.GroupWith("GRLIVE");                       
            Matrix3.Columns.Item("urltype").ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
            clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix3, "crurl", "#");

        }


        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                oForm = clsModule.objaddon.objapplication.Forms.GetForm(FormName, pVal.FormTypeCount);                
                oForm.PaneLevel = 26;
                strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"",T0.""U_Live"",T0.""U_UATUrl"",T0.""U_LIVEUrl"",
                              T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",T1.""U_URL"",T0.""U_HSNL"",T0.""U_SERCONFIG"",";
                strSQL += @" ""U_DBType"" ,""U_EwayNo"" ,""U_VehNo"" ,""U_TransID"" ,""U_Distance"",""U_TransName"",";

                strSQL += " \"U_GST_ClientCode\",\"U_GST_UserCode\",\"U_GST_Password\",\"U_GST_Live\",\"U_GST_UATUrl\",\"U_GST_LIVEUrl\",\"U_GST_Token\"";
                strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'";
                
                
               DataTable dt = new DataTable();
               dt= clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);
                
                if (dt.Rows.Count>0)
                {
                    Matrix0.Clear();
                    Matrix3.Clear();
                    loaddata = true;
                    foreach (DataRow Drow in dt.Rows)
                    {
                        oForm.DataSources.UserDataSources.Item("UD_ClnCod").Value = Drow["U_ClientCode"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_UsrCod").Value = Drow["U_UserCode"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_Pass").Value = Drow["U_Password"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_UbUrl").Value = Drow["U_UATUrl"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_LbUrl").Value = Drow["U_LIVEUrl"].ToString();
                    

                        oForm.DataSources.UserDataSources.Item("UD_HSNL").Value = Drow["U_HSNL"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_SERCON").Value = Drow["U_SERCONFIG"].ToString();
                       

                        oForm.DataSources.UserDataSources.Item("EwayNo").Value = Drow["U_EwayNo"].ToString();
                        oForm.DataSources.UserDataSources.Item("VehNo").Value = Drow["U_VehNo"].ToString();
                        oForm.DataSources.UserDataSources.Item("TransID").Value = Drow["U_TransID"].ToString();
                        oForm.DataSources.UserDataSources.Item("TransName").Value = Drow["U_TransName"].ToString();
                        oForm.DataSources.UserDataSources.Item("Distance").Value = Drow["U_Distance"].ToString();
                

                        oForm.DataSources.UserDataSources.Item("UD_GClcode").Value = Drow["U_GST_ClientCode"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_GUcode").Value = Drow["U_GST_UserCode"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_Gpass").Value = Drow["U_GST_Password"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_GUurl").Value = Drow["U_GST_UATUrl"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_GLUrl").Value = Drow["U_GST_LIVEUrl"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_Gtok").Value = Drow["U_GST_Token"].ToString();


                        if (Drow["U_Type"].ToString() != "GST")
                        {
                            Matrix0.AddRow();
                            ((SAPbouiCOM.EditText)Matrix0.Columns.Item("url").Cells.Item(Matrix0.VisualRowCount).Specific).String = Drow["U_URL"].ToString();
                            ((SAPbouiCOM.ComboBox)Matrix0.Columns.Item("urltype").Cells.Item(Matrix0.VisualRowCount).Specific).Select(Drow["U_URLType"].ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                            ((SAPbouiCOM.ComboBox)Matrix0.Columns.Item("type").Cells.Item(Matrix0.VisualRowCount).Specific).Select(Drow["U_Type"].ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                            ((SAPbouiCOM.EditText)Matrix0.Columns.Item("#").Cells.Item(Matrix0.VisualRowCount).Specific).String = Matrix0.VisualRowCount.ToString();
                           
 
                        }
                        else
                        {
                           Matrix3.AddRow();
                            ((SAPbouiCOM.EditText)Matrix3.Columns.Item("url").Cells.Item(Matrix3.VisualRowCount).Specific).String = Drow["U_URL"].ToString();
                            ((SAPbouiCOM.ComboBox)Matrix3.Columns.Item("urltype").Cells.Item(Matrix3.VisualRowCount).Specific).Select(Drow["U_URLType"].ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);                            
                            ((SAPbouiCOM.EditText)Matrix3.Columns.Item("#").Cells.Item(Matrix3.VisualRowCount).Specific).String = Matrix3.VisualRowCount.ToString();
                           
                            

                        }
                       
                    }
                  
                    if (Matrix3.RowCount == 0)
                    {
                        clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix3, "crurl", "#");
                    }
                    if (Matrix0.RowCount == 0)
                    {
                        clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix0, "crurl", "#");
                    }
                    if (!(OptionBtn0.Selected == true | OptionBtn1.Selected == true))
                    {
                        strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_Live\" from \"@ATEICFG\" where \"Code\"='01'");
                       if (strSQL == "Y")
                        {
                            OptionBtn1.Item.Click();
                        }
                        else
                        {
                            OptionBtn0.Selected = true;
                        }
                    }

                      strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_GetCompAdd\" from \"@ATEICFG\" where \"Code\"='01'");
                       if (strSQL == "Y")
                        {
                        CheckBox0.Checked = true;
                        }

                    strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_InvTranGetcusAdd\" from \"@ATEICFG\" where \"Code\"='01'");
                    if (strSQL == "Y")
                    {
                        CheckBox1.Checked = true;
                    }


                    if (!(OptionBtn4.Selected == true | OptionBtn5.Selected == true))
                    {
                        strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_DBType\" from \"@ATEICFG\" where \"Code\"='01'");
                        if (strSQL == "Y")
                        {
                            OptionBtn4.Item.Click();
                        }
                        else
                        {
                            OptionBtn5.Selected = true;
                        }
                    }
                }
               
                oForm.PaneLevel = 7;
               // Folder2.Item.Visible = false;
               
              //  oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;

                loaddata = false;
            }

            catch (Exception ex) 
            {
                loaddata = false;
                //throw;
            }
        }

        private void Folder0_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                oForm = clsModule.objaddon.objapplication.Forms.ActiveForm;                
                oForm.PaneLevel = 26;
                
                if (!(OptionBtn0.Selected == true | OptionBtn1.Selected == true))
                {
                    strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_Live\" from \"@ATEICFG\" where \"Code\"='01'");
                    if (strSQL == "Y")
                    {
                        OptionBtn1.Item.Click();
                    }
                    else
                    {
                        OptionBtn0.Selected = true;
                    }
                }

                if (!(OptionBtn4.Selected == true | OptionBtn5.Selected == true))
                {
                    strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_DBType\" from \"@ATEICFG\" where \"Code\"='01'");
                    if (strSQL == "Y")
                    {
                        OptionBtn4.Item.Click();
                    }
                    else
                    {
                        OptionBtn5.Selected = true;
                    }
                }

                strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_GetCompAdd\" from \"@ATEICFG\" where \"Code\"='01'");
                if (strSQL == "Y")
                {
                    CheckBox0.Checked = true;
                }


                strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_InvTranGetcusAdd\" from \"@ATEICFG\" where \"Code\"='01'");
                if (strSQL == "Y")
                {
                    CheckBox1.Checked = true;
                }

                Matrix0.AutoResizeColumns();
            
                oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;               
                oForm.Select();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private void Button0_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            E_Invoice_Config();
        }

        private void Matrix0_ValidateAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (loaddata) return;
                switch (pVal.ColUID)
                {
                    case "url":
                        clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix0, "url", "#");
                        break;
                }

            }
            catch (Exception)
            {
                throw;
            }

        }
        
        private bool E_Invoice_Config()
        {
            try
            {
                bool Flag = false;
                //string live;
                GeneralService oGeneralService;
                GeneralData oGeneralData;
                GeneralDataParams oGeneralParams;
                GeneralDataCollection oGeneralDataCollection;
                GeneralData oChild;

                oGeneralService = clsModule.objaddon.objcompany.GetCompanyService().GetGeneralService("ATCFG");
                oGeneralData = (GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                oGeneralDataCollection = oGeneralData.Child("ATEICFG1");
                try
                {
                    oGeneralParams.SetProperty("Code", "01");
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                    Flag = true;
                }
                catch (Exception ex)
                {
                    Flag = false;
                }

                //string ss = GetEditBoxValue("tcntcod");

                oGeneralData.SetProperty("Code", "01");
                oGeneralData.SetProperty("Name", "01");
                oGeneralData.SetProperty("U_ClientCode", oForm.DataSources.UserDataSources.Item("UD_ClnCod").Value);                
                oGeneralData.SetProperty("U_UserCode", oForm.DataSources.UserDataSources.Item("UD_UsrCod").Value);
                oGeneralData.SetProperty("U_Password", oForm.DataSources.UserDataSources.Item("UD_Pass").Value);              
                oGeneralData.SetProperty("U_Live", Convert.ToString(((OptionBtn0.Selected == true) ? 'N' : 'Y')));
                oGeneralData.SetProperty("U_UATUrl", oForm.DataSources.UserDataSources.Item("UD_UbUrl").Value);
                oGeneralData.SetProperty("U_LIVEUrl", oForm.DataSources.UserDataSources.Item("UD_LbUrl").Value);              
                oGeneralData.SetProperty("U_HSNL", oForm.DataSources.UserDataSources.Item("UD_HSNL").Value);
                oGeneralData.SetProperty("U_SERCONFIG", oForm.DataSources.UserDataSources.Item("UD_SERCON").Value);
                oGeneralData.SetProperty("U_GetCompAdd", Convert.ToString(((CheckBox0.Checked == true) ? 'Y' : 'N'))); 
                oGeneralData.SetProperty("U_InvTranGetcusAdd", Convert.ToString(((CheckBox1.Checked == true) ? 'Y' : 'N'))); 

                oGeneralData.SetProperty("U_DBType", Convert.ToString(((OptionBtn4.Selected == true) ? 'Y' : 'N')));
                oGeneralData.SetProperty("U_EwayNo", oForm.DataSources.UserDataSources.Item("EwayNo").Value);
                oGeneralData.SetProperty("U_VehNo", oForm.DataSources.UserDataSources.Item("VehNo").Value);
                oGeneralData.SetProperty("U_TransID", oForm.DataSources.UserDataSources.Item("TransID").Value);
                oGeneralData.SetProperty("U_TransName", oForm.DataSources.UserDataSources.Item("TransName").Value);
                oGeneralData.SetProperty("U_Distance", oForm.DataSources.UserDataSources.Item("Distance").Value);



                oGeneralData.SetProperty("U_GST_ClientCode", oForm.DataSources.UserDataSources.Item("UD_GClcode").Value);
                oGeneralData.SetProperty("U_GST_UserCode", oForm.DataSources.UserDataSources.Item("UD_GUcode").Value);
                oGeneralData.SetProperty("U_GST_Password", oForm.DataSources.UserDataSources.Item("UD_Gpass").Value);
                oGeneralData.SetProperty("U_GST_Live", Convert.ToString(((OptionBtn2.Selected == true) ? 'N' : 'Y')));
                oGeneralData.SetProperty("U_GST_UATUrl", oForm.DataSources.UserDataSources.Item("UD_GUurl").Value);
                oGeneralData.SetProperty("U_GST_LIVEUrl", oForm.DataSources.UserDataSources.Item("UD_GLUrl").Value);
                oGeneralData.SetProperty("U_GST_Token", oForm.DataSources.UserDataSources.Item("UD_Gtok").Value);


                oChild = oGeneralDataCollection.Add();
                int rowcount = 0;
                for (int i = 1; i <= Matrix0.VisualRowCount; i++)
                {
                    if (((SAPbouiCOM.EditText)Matrix0.Columns.Item("url").Cells.Item(i).Specific).String != "")
                    {
                       

                        if (rowcount+1 > oGeneralData.Child("ATEICFG1").Count)
                        {
                            oGeneralData.Child("ATEICFG1").Add();
                        }


                        oGeneralData.Child("ATEICFG1").Item(rowcount).SetProperty("U_URLType", ((SAPbouiCOM.ComboBox)Matrix0.Columns.Item("urltype").Cells.Item(i).Specific).Selected.Value);
                        oGeneralData.Child("ATEICFG1").Item(rowcount).SetProperty("U_Type", ((SAPbouiCOM.ComboBox)Matrix0.Columns.Item("type").Cells.Item(i).Specific).Selected.Value);
                        oGeneralData.Child("ATEICFG1").Item(rowcount).SetProperty("U_URL", ((SAPbouiCOM.EditText)Matrix0.Columns.Item("url").Cells.Item(i).Specific).String);
                        rowcount++;
                    }
                }

                for (int i = 1; i <= Matrix3.VisualRowCount; i++)
                {
                    if (((SAPbouiCOM.EditText)Matrix3.Columns.Item("url").Cells.Item(i).Specific).String != "")
                    {

                        if (rowcount + 1 > oGeneralData.Child("ATEICFG1").Count)
                        {
                            oGeneralData.Child("ATEICFG1").Add();
                        }

                        oGeneralData.Child("ATEICFG1").Item(rowcount).SetProperty("U_URLType", ((SAPbouiCOM.ComboBox)Matrix3.Columns.Item("urltype").Cells.Item(i).Specific).Selected.Value);
                        oGeneralData.Child("ATEICFG1").Item(rowcount).SetProperty("U_Type", "GST");
                        oGeneralData.Child("ATEICFG1").Item(rowcount).SetProperty("U_URL", ((SAPbouiCOM.EditText)Matrix3.Columns.Item("url").Cells.Item(i).Specific).String);
                        rowcount++;
                    }
                }

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
                clsModule.objaddon.objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
        }

        public string GetEditBoxValue(string uniqueid)
        {
            return  ((SAPbouiCOM.EditText)oForm.Items.Item(uniqueid).Specific).Value.ToString(); 
        }

        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.EditText EditText3;
        private SAPbouiCOM.StaticText StaticText7;
        private SAPbouiCOM.EditText EditText7;
        private SAPbouiCOM.OptionBtn OptionBtn4;
        private SAPbouiCOM.OptionBtn OptionBtn5;
        private SAPbouiCOM.StaticText StaticText6;
        private SAPbouiCOM.EditText EditText6;
        private SAPbouiCOM.StaticText StaticText8;
        private SAPbouiCOM.EditText EditText8;
        private SAPbouiCOM.StaticText StaticText9;
        private SAPbouiCOM.EditText EditText9;
        private SAPbouiCOM.StaticText StaticText11;
        private SAPbouiCOM.EditText EditText11;

        private void EditText11_KeyDownAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
           

        }

        private SAPbouiCOM.StaticText StaticText12;
        private SAPbouiCOM.EditText EditText10;
        private SAPbouiCOM.StaticText StaticText10;
        private SAPbouiCOM.Folder Folder2;
        private SAPbouiCOM.StaticText StaticText13;
        private SAPbouiCOM.EditText EditText12;
        private SAPbouiCOM.StaticText StaticText14;
        private SAPbouiCOM.EditText EditText13;
        private SAPbouiCOM.StaticText StaticText15;
        private SAPbouiCOM.EditText EditText14;
        private SAPbouiCOM.StaticText StaticText16;
        private SAPbouiCOM.EditText EditText15;
        private SAPbouiCOM.StaticText StaticText17;
        private SAPbouiCOM.EditText EditText16;
        private SAPbouiCOM.OptionBtn OptionBtn2;
        private SAPbouiCOM.OptionBtn OptionBtn3;
        private SAPbouiCOM.EditText EditText17;
        private SAPbouiCOM.StaticText StaticText18;

        private void Folder2_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
           
        }

        private SAPbouiCOM.Matrix Matrix3;

        private void Matrix3_ValidateAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (loaddata) return;
                switch (pVal.ColUID)
                {
                    case "url":
                        clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix3, "url", "#");
                        break;
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        private void Matrix3_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
           
          

        }

        private void Folder2_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            oForm = clsModule.objaddon.objapplication.Forms.ActiveForm;
            oForm.PaneLevel = 27;
            if (!(OptionBtn2.Selected == true | OptionBtn3.Selected == true))
            {
                strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_GST_Live\" from \"@ATEICFG\" where \"Code\"='01'");
                if (strSQL == "Y")
                {
                    OptionBtn3.Item.Click();
                }
                else
                {
                    OptionBtn3.Selected = true;
                }
            }
            Matrix3.AutoResizeColumns();

        }

        private void Button0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

           
        }

        private SAPbouiCOM.CheckBox CheckBox0;
        private SAPbouiCOM.CheckBox CheckBox1;
    }
}
 