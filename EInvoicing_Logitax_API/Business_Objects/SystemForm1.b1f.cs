using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        }

        private SAPbouiCOM.StaticText StaticText0;


    }
}
