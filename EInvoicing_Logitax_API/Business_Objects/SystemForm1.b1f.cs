using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EInvoicing_Logitax_API.Common;
using SAPbouiCOM.Framework;

namespace EInvoicing_Logitax_API.Business_Objects
{
    [FormAttribute("256000803", "Business_Objects/SystemForm1.b1f")]
    class SystemForm1 : SystemFormBase
    {
        public SystemForm1()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.CheckBox0 = ((SAPbouiCOM.CheckBox)(this.GetItem("LDis").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("256000030").Specific));            
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("256000060").Specific));
            this.EditText2.ClickAfter += new SAPbouiCOM._IEditTextEvents_ClickAfterEventHandler(this.EditText2_ClickAfter);
            
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private SAPbouiCOM.CheckBox CheckBox0;

        private void OnCustomInitialize()
        {
            CheckBox0.Item.Top = StaticText0.Item.Top;
            CheckBox0.Item.Left = StaticText0.Item.Left+ StaticText0.Item.Width+5;
            if (clsModule.objaddon.objglobalmethods.getSingleValue("SELECT \"U_GetDisAddWare\" FROM \"@ATEICFG\" a  WHERE a.\"Code\" ='01'") != "Y")
            { CheckBox0.Item.Click(); }

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
    }
}
