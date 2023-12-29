using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EInvoicing_Logitax_API.Common;
using SAPbouiCOM.Framework;

namespace EInvoicing_Logitax_API.Business_Objects
{
    //before 10.0 comment and give exe
    [FormAttribute("940", "Business_Objects/Inven_Transfer.b1f")]
    class Inven_Transfer : SystemFormBase
    {
       
        private string FormName = "940";
        private string strSQL;
        public static SAPbouiCOM.Form oForm;
        SAPbouiCOM.ISBOChooseFromListEventArg pCFL;
        public SAPbouiCOM.DBDataSource Matrix1DB;
        public Inven_Transfer()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Folder0 = ((SAPbouiCOM.Folder)(this.GetItem("Item_0").Specific));
            this.Folder0.ClickBefore += new SAPbouiCOM._IFolderEvents_ClickBeforeEventHandler(this.Folder0_ClickBefore);
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0007").Specific));
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("ET00010").Specific));
            this.StaticText5 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00012").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("ET00016").Specific));
            this.StaticText6 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_5").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("ET00015").Specific));
            this.StaticText7 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00010").Specific));
            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("ET0008").Specific));
            this.StaticText8 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0009").Specific));
            this.StaticText9 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0001").Specific));
            this.EditText4 = ((SAPbouiCOM.EditText)(this.GetItem("ET00011").Specific));
            this.StaticText10 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00013").Specific));
            this.EditText5 = ((SAPbouiCOM.EditText)(this.GetItem("ET0005").Specific));
            this.StaticText11 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00017").Specific));
            this.EditText6 = ((SAPbouiCOM.EditText)(this.GetItem("ET0009").Specific));
            this.EditText6.KeyDownAfter += new SAPbouiCOM._IEditTextEvents_KeyDownAfterEventHandler(this.EditText6_KeyDownAfter);
            this.StaticText12 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00011").Specific));
            this.ComboBox3 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET0003").Specific));
            this.StaticText13 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0006").Specific));
            this.EditText7 = ((SAPbouiCOM.EditText)(this.GetItem("ET00017").Specific));
            this.StaticText14 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00019").Specific));
            this.EditText8 = ((SAPbouiCOM.EditText)(this.GetItem("ET00014").Specific));
            this.StaticText15 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00016").Specific));
            this.ComboBox4 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET00013").Specific));
            this.StaticText16 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00015").Specific));
            this.ComboBox5 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET00012").Specific));
            this.ComboBox6 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET0004").Specific));
            this.StaticText17 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0005").Specific));
            this.StaticText18 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00014").Specific));
            this.EditText9 = ((SAPbouiCOM.EditText)(this.GetItem("ET0007").Specific));
            this.ComboBox7 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET0002").Specific));
            this.ComboBox7.ComboSelectBefore += new SAPbouiCOM._IComboBoxEvents_ComboSelectBeforeEventHandler(this.ComboBox7_ComboSelectBefore);
            this.ComboBox7.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.ComboBox7_ComboSelectAfter);
            this.StaticText19 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0003").Specific));
            this.StaticText20 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0002").Specific));
            this.StaticText21 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0008").Specific));
            this.EditText10 = ((SAPbouiCOM.EditText)(this.GetItem("ET0006").Specific));
            this.ComboBox8 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET0001").Specific));
            this.StaticText22 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0004").Specific));
            this.Matrix0 = ((SAPbouiCOM.Matrix)(this.GetItem("23").Specific));
            this.Matrix0.ChooseFromListAfter += new SAPbouiCOM._IMatrixEvents_ChooseFromListAfterEventHandler(this.Matrix0_ChooseFromListAfter);
            this.Matrix0.KeyDownBefore += new SAPbouiCOM._IMatrixEvents_KeyDownBeforeEventHandler(this.Matrix0_KeyDownBefore);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new SAPbouiCOM.Framework.FormBase.LoadAfterHandler(this.Form_LoadAfter);
            this.VisibleAfter += new VisibleAfterHandler(this.Form_VisibleAfter);

        }

        private SAPbouiCOM.Folder Folder0;

        private void OnCustomInitialize()
        {
            oForm = clsModule.objaddon.objapplication.Forms.GetForm("940",1);
            this.Folder0.GroupWith("1320000081");

            Matrix0.Columns.Item("U_UTL_ST_TAXCD").ChooseFromListUID = "CFLTax";
            Matrix0.Columns.Item("U_UTL_ST_TAXCD").ChooseFromListAlias = "Code";

            Matrix1DB = oForm.DataSources.DBDataSources.Item("WTR1");



        }

        private void Folder0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            
            oForm.PaneLevel = 26;
            int offset = oForm.DataSources.DBDataSources.Item(0).Offset;
            string DocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", offset);

            string cnnt = clsModule.objaddon.objglobalmethods.getSingleValue("select 1 from wtr26 where \"DocEntry\" ='" + DocEntry + "'");

            if (cnnt != "1")
            {
                clsModule.objaddon.objglobalmethods.ExecuteQuery("  INSERT INTO WTR26 (\"DocEntry\") VALUES('" + DocEntry + "'); ");

            }
            //strSQL = "SELECT \"SubID\" ,\"SubType\" FROM OEST o  ;";
            //clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET0002").Specific), strSQL, new[] { "-,-" });

            //strSQL = "SELECT  \"TypeCode\",\"TypeName\" FROM OEDT o;";
            //clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET0003").Specific), strSQL, new[] { "-,-" });


            //strSQL = "SELECT  \"ModeCode\" ,\"ModeName\"  FROM OETM o;";
            //clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET00012").Specific), strSQL, new[] { "-,-" });

            //strSQL = "SELECT  \"TypeCode\",\"TypeName\" FROM OEVT o;";
            //clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET00013").Specific), strSQL, new[] { "-,-" });



        }
        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.StaticText StaticText5;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.StaticText StaticText6;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.StaticText StaticText7;
        private SAPbouiCOM.EditText EditText3;
        private SAPbouiCOM.StaticText StaticText8;
        private SAPbouiCOM.StaticText StaticText9;
        private SAPbouiCOM.EditText EditText4;
        private SAPbouiCOM.StaticText StaticText10;
        private SAPbouiCOM.EditText EditText5;
        private SAPbouiCOM.StaticText StaticText11;
        private SAPbouiCOM.EditText EditText6;
        private SAPbouiCOM.StaticText StaticText12;
        private SAPbouiCOM.ComboBox ComboBox3;
        private SAPbouiCOM.StaticText StaticText13;
        private SAPbouiCOM.EditText EditText7;
        private SAPbouiCOM.StaticText StaticText14;
        private SAPbouiCOM.EditText EditText8;
        private SAPbouiCOM.StaticText StaticText15;
        private SAPbouiCOM.ComboBox ComboBox4;
        private SAPbouiCOM.StaticText StaticText16;
        private SAPbouiCOM.ComboBox ComboBox5;
        private SAPbouiCOM.ComboBox ComboBox6;
        private SAPbouiCOM.StaticText StaticText17;
        private SAPbouiCOM.StaticText StaticText18;
        private SAPbouiCOM.EditText EditText9;
        private SAPbouiCOM.ComboBox ComboBox7;
        private SAPbouiCOM.StaticText StaticText19;
        private SAPbouiCOM.StaticText StaticText20;
        private SAPbouiCOM.StaticText StaticText21;
        private SAPbouiCOM.EditText EditText10;
        private SAPbouiCOM.ComboBox ComboBox8;
        private SAPbouiCOM.StaticText StaticText22;

        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {

            
            oForm = clsModule.objaddon.objapplication.Forms.GetForm("940", pVal.FormTypeCount);
           
            strSQL = "SELECT \"SubID\" ,\"SubType\" FROM OEST o  ;";
            clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET0002").Specific), strSQL, new[] { "-,-" });

            strSQL = "SELECT  \"TypeCode\",\"TypeName\" FROM OEDT o;";
            clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET0003").Specific), strSQL, new[] { "-,-" });


            strSQL = "SELECT  \"ModeCode\" ,\"ModeName\"  FROM OETM o;";
            clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET00012").Specific), strSQL, new[] { "-,-" });

            strSQL = "SELECT  \"TypeCode\",\"TypeName\" FROM OEVT o;";
            clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET00013").Specific), strSQL, new[] { "-,-" });

        }

        private void EditText6_KeyDownAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.CharPressed == 9)
            {
                Choose form1 = new Choose();                
                form1.ActualForm = oForm;
                form1.Query = " SELECT \"TransCode\" ,\"TransName\" ,\"TransID\",t.\"TransMode\" ,t.\"VehicleTyp\", t.\"VehicleNo\"  FROM OTSP o " +
                        " left JOIN TSP1 t on o.\"AbsEntry\" = t.\"AbsEntry\"";


                form1.Show();

              
         



            }
        }

        private void Form_VisibleAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
           

        }

        private void ComboBox7_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            
        }

        private void ComboBox7_ComboSelectBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
         
        }

        private SAPbouiCOM.Matrix Matrix0;

        private void Matrix0_KeyDownBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            

        }

        private void Matrix0_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {

            string lstrquery = "";
            
            switch (pVal.ColUID)
            {
                case "U_UTL_ST_TAXCD":                    
                    pCFL = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;
                    if (pCFL.SelectedObjects != null)
                    {
                        
                        try
                        {
                            columnEdit( true, Matrix0);
                            string Tax = Convert.ToString(pCFL.SelectedObjects.Columns.Item("Code").Cells.Item(0).Value);
                            ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_TAXCD").Cells.Item(pVal.Row).Specific).Value = Tax;

                            lstrquery += "SELECT t2.\"EfctivRate\",t2.\"STAType\" FROM ostc t1  LEFT JOIN stc1 t2 ON t1.\"Code\" = t2.\"STCCode\" WHERE t1.\"Code\" = '" + Tax +"'; ";

                            SAPbobsCOM.Recordset Rc;
                            Rc = clsModule.objaddon.objglobalmethods.GetmultipleRS(lstrquery);

                            ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_CGST").Cells.Item(pVal.Row).Specific).Value = "0";
                            ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_CGAMT").Cells.Item(pVal.Row).Specific).Value = "0";
                            ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_SGST").Cells.Item(pVal.Row).Specific).Value = "0";
                            ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_SGAMT").Cells.Item(pVal.Row).Specific).Value = "0";
                            ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_IGST").Cells.Item(pVal.Row).Specific).Value = "0";
                            ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_IGAMT").Cells.Item(pVal.Row).Specific).Value = "0";

                            for (int i = 0; i < Rc.RecordCount; i++)
                            {
                                decimal Amount = clsModule.objaddon.objglobalmethods.GetDecimalVal(((SAPbouiCOM.EditText)Matrix0.Columns.Item("11").Cells.Item(pVal.Row).Specific).Value);
                                Amount = clsModule.objaddon.objglobalmethods.GetDecimalVal(((SAPbouiCOM.EditText)Matrix0.Columns.Item("10").Cells.Item(pVal.Row).Specific).Value) * Amount;

                                switch (Rc.Fields.Item("STAType").Value.ToString())
                                {
                                    
                                    case "-100":                                    
                                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_CGST").Cells.Item(pVal.Row).Specific).Value = (clsModule.objaddon.objglobalmethods.GetDecimalVal(Rc.Fields.Item("EfctivRate").Value)).ToString();
                                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_CGAMT").Cells.Item(pVal.Row).Specific).Value = (TaxCalculation(Amount, (clsModule.objaddon.objglobalmethods.GetDecimalVal(Rc.Fields.Item("EfctivRate").Value)))).ToString();
                                        break;
                                    case "-110":
                                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_SGST").Cells.Item(pVal.Row).Specific).Value = (clsModule.objaddon.objglobalmethods.GetDecimalVal(Rc.Fields.Item("EfctivRate").Value)).ToString();
                                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_SGAMT").Cells.Item(pVal.Row).Specific).Value = (TaxCalculation(Amount, (clsModule.objaddon.objglobalmethods.GetDecimalVal(Rc.Fields.Item("EfctivRate").Value)))).ToString();
                                        break;
                                    case "-120":
                                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_IGST").Cells.Item(pVal.Row).Specific).Value = (clsModule.objaddon.objglobalmethods.GetDecimalVal(Rc.Fields.Item("EfctivRate").Value)).ToString();
                                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_IGAMT").Cells.Item(pVal.Row).Specific).Value = (TaxCalculation(Amount, (clsModule.objaddon.objglobalmethods.GetDecimalVal(Rc.Fields.Item("EfctivRate").Value)))).ToString();
                                        break;

                                }
                                decimal LineTotal = 0;
                                LineTotal += Amount;
                                LineTotal += clsModule.objaddon.objglobalmethods.GetDecimalVal(((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_CGAMT").Cells.Item(pVal.Row).Specific).Value);
                                LineTotal += clsModule.objaddon.objglobalmethods.GetDecimalVal(((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_SGAMT").Cells.Item(pVal.Row).Specific).Value);
                                LineTotal += clsModule.objaddon.objglobalmethods.GetDecimalVal(((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_IGAMT").Cells.Item(pVal.Row).Specific).Value);

                                ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_LINETOTAL").Cells.Item(pVal.Row).Specific).Value = LineTotal.ToString();
                                Rc.MoveNext();
                            }

                            ((SAPbouiCOM.EditText)Matrix0.Columns.Item("U_UTL_ST_TAXCD").Cells.Item(pVal.Row).Specific).Value = Tax;
                            columnEdit(false, Matrix0);
                        }
                        catch (Exception ex)
                        {
                        }
                        finally
                        {
                            columnEdit(false, Matrix0);
                        }
                    }                    
                    break;
                default:
                    break;
            }

        }



        private void columnEdit(bool pblnenable, SAPbouiCOM.Matrix mt)
        {

    

            mt.Columns.Item("U_UTL_ST_CGST").Editable = pblnenable;
            mt.Columns.Item("U_UTL_ST_CGAMT").Editable = pblnenable;
            mt.Columns.Item("U_UTL_ST_SGST").Editable = pblnenable;
            mt.Columns.Item("U_UTL_ST_SGAMT").Editable = pblnenable;
            mt.Columns.Item("U_UTL_ST_IGST").Editable = pblnenable;
            mt.Columns.Item("U_UTL_ST_IGAMT").Editable = pblnenable;
            mt.Columns.Item("U_UTL_ST_LINETOTAL").Editable = pblnenable;
        }

        private decimal TaxCalculation(decimal Amount,decimal TaxRate )
        {
            decimal Taxvalue = 0;

            Taxvalue = (Amount * TaxRate) / 100;


            return Taxvalue; 
        }
    }
}
