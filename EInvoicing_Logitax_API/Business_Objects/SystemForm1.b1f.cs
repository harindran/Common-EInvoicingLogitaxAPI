using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EInvoicing_Logitax_API.Common;
using SAPbouiCOM;
using SAPbouiCOM.Framework;

namespace EInvoicing_Logitax_API.Business_Objects
{
    [FormAttribute("256000803", "Business_Objects/SystemForm1.b1f")]
    class SystemForm1 : SystemFormBase
    {
        public SystemForm1()
        {
        }
        public SAPbouiCOM.Form oForm;
        SAPbouiCOM.DBDataSource odbdhead;
        public string Table;
      
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.CheckBox0 = ((SAPbouiCOM.CheckBox)(this.GetItem("LDis").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("256000030").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("256000060").Specific));
            this.EditText2.ClickAfter += new SAPbouiCOM._IEditTextEvents_ClickAfterEventHandler(this.EditText2_ClickAfter);
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("256000063").Specific));
            this.Button0.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.Button0_ClickAfter);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);

        }

        private SAPbouiCOM.CheckBox CheckBox0;

        private void OnCustomInitialize()
        {
            CheckBox0.Item.Top = StaticText0.Item.Top;
            CheckBox0.Item.Left = StaticText0.Item.Left+ StaticText0.Item.Width+5;
           

          
        }

        private SAPbouiCOM.StaticText StaticText0;

        private void EditText1_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            

        }

        private SAPbouiCOM.EditText EditText2;

        private void EditText2_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            

        }

        private void StaticText3_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            

        }

        private SAPbouiCOM.Button Button0;

        private void Button0_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
         
            string docEntryString = oForm.DataSources.DBDataSources.Item(Table).GetValue("DocEntry", 0).Trim();

           int DocEntry = int.Parse(docEntryString);

           
            switch (Table)
            {
         
                case "RIN26":
                    clsModule.objaddon.objglobalmethods.ExecuteQuery("update " + Table + " set  \"U_Dispatch_Eway\" = '" + Convert.ToString(((CheckBox0.Checked == true) ? 'Y' : 'N')) + "' WHERE \"DocEntry\" = " + DocEntry);
                    break;
              default:
                    return;

            }

          
              
}

        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            oForm = clsModule.objaddon.objapplication.Forms.GetForm("256000803", pVal.FormTypeCount);

            odbdhead = oForm.DataSources.DBDataSources.Item("INV26");

            bool identify = true;

            if (!string.IsNullOrEmpty(odbdhead.GetValue("DocEntry", 0)))
            {
                Table = "INV26";
                return;
              
            }

            if (identify)
            {
                odbdhead = oForm.DataSources.DBDataSources.Item("RIN26");
                if (!string.IsNullOrEmpty(odbdhead.GetValue("DocEntry", 0)))
                {
                    Table = "RIN26";
                    if (odbdhead.GetValue("U_Dispatch_Eway", 0) == "Y")
                    { CheckBox0.Item.Click(); }
                }
            }

        



        }
    }
}
