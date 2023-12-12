using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EInvoicing_Logitax_API.Common;
using SAPbouiCOM.Framework;

namespace EInvoicing_Logitax_API.Business_Objects
{
    //9408978 -- other than IRIS
    //9408978--IRIS No Need
   // [FormAttribute("9408978", "samplefloder/Invtransfer.b1f")]
    class Invtransfer : SystemFormBase
    {
        private string FormName = "940";
        private string strSQL;
        public static SAPbouiCOM.Form oForm;

        public Invtransfer()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Folder4 = ((SAPbouiCOM.Folder)(this.GetItem("FlodEway").Specific));
            this.Folder4.PressedBefore += new SAPbouiCOM._IFolderEvents_PressedBeforeEventHandler(this.Folder4_PressedBefore);
            this.Folder4.PressedAfter += new SAPbouiCOM._IFolderEvents_PressedAfterEventHandler(this.Folder4_PressedAfter);
            this.StaticText71 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0002").Specific));
            this.ComboBox25 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET0001").Specific));
            this.StaticText72 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0003").Specific));
            this.ComboBox26 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET0002").Specific));
            this.StaticText73 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0004").Specific));
            this.ComboBox27 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET0003").Specific));
            this.StaticText74 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00011").Specific));
            this.EditText36 = ((SAPbouiCOM.EditText)(this.GetItem("ET0009").Specific));
            this.EditText36.KeyDownAfter += new SAPbouiCOM._IEditTextEvents_KeyDownAfterEventHandler(this.EditText36_KeyDownAfter);
            this.StaticText75 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00017").Specific));
            this.EditText37 = ((SAPbouiCOM.EditText)(this.GetItem("ET00015").Specific));
            this.StaticText76 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00018").Specific));
            this.EditText38 = ((SAPbouiCOM.EditText)(this.GetItem("ET00016").Specific));
            this.StaticText77 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00012").Specific));
            this.EditText39 = ((SAPbouiCOM.EditText)(this.GetItem("ET00010").Specific));
            this.StaticText78 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0007").Specific));
            this.EditText40 = ((SAPbouiCOM.EditText)(this.GetItem("ET0006").Specific));
            this.StaticText79 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0008").Specific));
            this.EditText41 = ((SAPbouiCOM.EditText)(this.GetItem("ET0007").Specific));
            this.StaticText80 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00014").Specific));
            this.ComboBox28 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET00012").Specific));
            this.StaticText81 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00015").Specific));
            this.ComboBox29 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET00013").Specific));
            this.StaticText82 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00016").Specific));
            this.EditText42 = ((SAPbouiCOM.EditText)(this.GetItem("ET00014").Specific));
            this.StaticText83 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00019").Specific));
            this.EditText43 = ((SAPbouiCOM.EditText)(this.GetItem("ET00017").Specific));
            this.StaticText84 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0006").Specific));
            this.EditText44 = ((SAPbouiCOM.EditText)(this.GetItem("ET0005").Specific));
            this.StaticText85 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00013").Specific));
            this.EditText45 = ((SAPbouiCOM.EditText)(this.GetItem("ET00011").Specific));
            this.StaticText86 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0001").Specific));
            this.StaticText87 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0009").Specific));
            this.EditText46 = ((SAPbouiCOM.EditText)(this.GetItem("ET0008").Specific));
            this.StaticText88 = ((SAPbouiCOM.StaticText)(this.GetItem("EL00010").Specific));
            this.StaticText89 = ((SAPbouiCOM.StaticText)(this.GetItem("EL0005").Specific));
            this.ComboBox30 = ((SAPbouiCOM.ComboBox)(this.GetItem("ET0004").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new SAPbouiCOM.Framework.FormBase.LoadAfterHandler(this.Form_LoadAfter);
            this.ActivateBefore += new ActivateBeforeHandler(this.Form_ActivateBefore);

        }

        private void OnCustomInitialize()
        {
            this.Folder4.GroupWith("1320000081");
           
        }

        private SAPbouiCOM.Folder Folder4;

        private void Folder4_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
           



        }

        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
           

        }

        private SAPbouiCOM.StaticText StaticText71;
        private SAPbouiCOM.ComboBox ComboBox25;
        private SAPbouiCOM.StaticText StaticText72;
        private SAPbouiCOM.ComboBox ComboBox26;
        private SAPbouiCOM.StaticText StaticText73;
        private SAPbouiCOM.ComboBox ComboBox27;
        private SAPbouiCOM.StaticText StaticText74;
        private SAPbouiCOM.EditText EditText36;
        private SAPbouiCOM.StaticText StaticText75;
        private SAPbouiCOM.EditText EditText37;
        private SAPbouiCOM.StaticText StaticText76;
        private SAPbouiCOM.EditText EditText38;
        private SAPbouiCOM.StaticText StaticText77;
        private SAPbouiCOM.EditText EditText39;
        private SAPbouiCOM.StaticText StaticText78;
        private SAPbouiCOM.EditText EditText40;
        private SAPbouiCOM.StaticText StaticText79;
        private SAPbouiCOM.EditText EditText41;
        private SAPbouiCOM.StaticText StaticText80;
        private SAPbouiCOM.ComboBox ComboBox28;
        private SAPbouiCOM.StaticText StaticText81;
        private SAPbouiCOM.ComboBox ComboBox29;
        private SAPbouiCOM.StaticText StaticText82;
        private SAPbouiCOM.EditText EditText42;
        private SAPbouiCOM.StaticText StaticText83;
        private SAPbouiCOM.EditText EditText43;
        private SAPbouiCOM.StaticText StaticText84;
        private SAPbouiCOM.EditText EditText44;
        private SAPbouiCOM.StaticText StaticText85;
        private SAPbouiCOM.EditText EditText45;
        private SAPbouiCOM.StaticText StaticText86;
        private SAPbouiCOM.StaticText StaticText87;
        private SAPbouiCOM.EditText EditText46;
        private SAPbouiCOM.StaticText StaticText88;
        private SAPbouiCOM.StaticText StaticText89;
        private SAPbouiCOM.ComboBox ComboBox30;

        private void Folder4_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            oForm = clsModule.objaddon.objapplication.Forms.ActiveForm;



             Enable(false);

            Align();
            Enable(true);
            oForm.PaneLevel = 26;






            strSQL = "SELECT \"SubID\" ,\"SubType\" FROM OEST o  ;";
            clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET0002").Specific), strSQL, new[] { "-,-" });
            
            strSQL = "SELECT  \"TypeCode\",\"TypeName\" FROM OEDT o;";
            clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET0003").Specific), strSQL, new[] { "-,-" });


            strSQL = "SELECT  \"ModeCode\" ,\"ModeName\"  FROM OETM o;";
            clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET00012").Specific), strSQL, new[] { "-,-" });

            strSQL = "SELECT  \"TypeCode\",\"TypeName\" FROM OEVT o;";
            clsModule.objaddon.objglobalmethods.Load_Combo(oForm.UniqueID, ((SAPbouiCOM.ComboBox)oForm.Items.Item("ET00013").Specific), strSQL, new[] { "-,-" });

        }

        private void Align()
        {
            int lblleft = oForm.Items.Item("1320000081").Left + 5;

            oForm.Items.Item("EL0001").Top = oForm.Items.Item("FlodEway").Top + 25;
            oForm.Items.Item("EL0001").Left = lblleft + 60;

            oForm.Items.Item("EL0002").Top = oForm.Items.Item("EL0001").Top + 15;
            oForm.Items.Item("EL0002").Left = lblleft;
            oForm.Items.Item("ET0001").Top = oForm.Items.Item("EL0001").Top + 15;

            oForm.Items.Item("EL0003").Top = oForm.Items.Item("EL0002").Top + 15;
            oForm.Items.Item("EL0003").Left = lblleft;
            oForm.Items.Item("ET0002").Top = oForm.Items.Item("EL0002").Top + 15;

            oForm.Items.Item("EL0004").Top = oForm.Items.Item("EL0003").Top + 15;
            oForm.Items.Item("EL0004").Left = lblleft;
            oForm.Items.Item("ET0003").Top = oForm.Items.Item("EL0003").Top + 15;

            oForm.Items.Item("EL0005").Top = oForm.Items.Item("EL0004").Top + 15;
            oForm.Items.Item("EL0005").Left = lblleft;
            oForm.Items.Item("ET0004").Top = oForm.Items.Item("EL0004").Top + 15;

            oForm.Items.Item("EL0006").Top = oForm.Items.Item("EL0005").Top + 15;
            oForm.Items.Item("EL0006").Left = lblleft;
            oForm.Items.Item("ET0005").Top = oForm.Items.Item("EL0005").Top + 15;

            oForm.Items.Item("EL0007").Top = oForm.Items.Item("EL0006").Top + 15;
            oForm.Items.Item("EL0007").Left = lblleft;
            oForm.Items.Item("ET0006").Top = oForm.Items.Item("EL0006").Top + 15;

            oForm.Items.Item("EL0008").Top = oForm.Items.Item("EL0007").Top + 15;
            oForm.Items.Item("EL0008").Left = lblleft;
            oForm.Items.Item("ET0007").Top = oForm.Items.Item("EL0007").Top + 15;

            oForm.Items.Item("EL0009").Top = oForm.Items.Item("EL0008").Top + 15;
            oForm.Items.Item("EL0009").Left = lblleft;
            oForm.Items.Item("ET0008").Top = oForm.Items.Item("EL0008").Top + 15;


            lblleft = oForm.Items.Item("ET0001").Left + oForm.Items.Item("ET0001").Width + 15;

            oForm.Items.Item("EL00010").Top = oForm.Items.Item("FlodEway").Top + 25;
            oForm.Items.Item("EL00010").Left = lblleft + 60;

            oForm.Items.Item("EL00011").Top = oForm.Items.Item("EL00010").Top + 15;
            oForm.Items.Item("EL00011").Left = lblleft;
            oForm.Items.Item("ET0009").Top = oForm.Items.Item("EL00010").Top + 15;
            oForm.Items.Item("ET0009").Left = lblleft + oForm.Items.Item("EL00011").Width + 25;

            oForm.Items.Item("EL00012").Top = oForm.Items.Item("EL00011").Top + 15;
            oForm.Items.Item("EL00012").Left = lblleft;
            oForm.Items.Item("ET00010").Top = oForm.Items.Item("EL00011").Top + 15;
            oForm.Items.Item("ET00010").Left = lblleft + oForm.Items.Item("EL00012").Width + 25;

            oForm.Items.Item("EL00013").Top = oForm.Items.Item("EL00012").Top + 15;
            oForm.Items.Item("EL00013").Left = lblleft;
            oForm.Items.Item("ET00011").Top = oForm.Items.Item("EL00012").Top + 15;
            oForm.Items.Item("ET00011").Left = lblleft + oForm.Items.Item("EL00013").Width + 25;

            oForm.Items.Item("EL00014").Top = oForm.Items.Item("EL00013").Top + 15;
            oForm.Items.Item("EL00014").Left = lblleft;
            oForm.Items.Item("ET00012").Top = oForm.Items.Item("EL00013").Top + 15;
            oForm.Items.Item("ET00012").Left = lblleft + oForm.Items.Item("EL00014").Width + 25;

            oForm.Items.Item("EL00015").Top = oForm.Items.Item("EL00014").Top + 15;
            oForm.Items.Item("EL00015").Left = lblleft;
            oForm.Items.Item("ET00013").Top = oForm.Items.Item("EL00014").Top + 15;
            oForm.Items.Item("ET00013").Left = lblleft + oForm.Items.Item("EL00015").Width + 25;

            oForm.Items.Item("EL00016").Top = oForm.Items.Item("EL00015").Top + 15;
            oForm.Items.Item("EL00016").Left = lblleft;
            oForm.Items.Item("ET00014").Top = oForm.Items.Item("EL00015").Top + 15;
            oForm.Items.Item("ET00014").Left = lblleft + oForm.Items.Item("EL00016").Width + 25;

            oForm.Items.Item("EL00017").Top = oForm.Items.Item("EL00016").Top + 15;
            oForm.Items.Item("EL00017").Left = lblleft;
            oForm.Items.Item("ET00015").Top = oForm.Items.Item("EL00016").Top + 15;
            oForm.Items.Item("ET00015").Left = lblleft + oForm.Items.Item("EL00017").Width + 25;

            oForm.Items.Item("EL00018").Top = oForm.Items.Item("EL00017").Top + 15;
            oForm.Items.Item("EL00018").Left = lblleft;
            oForm.Items.Item("ET00016").Top = oForm.Items.Item("EL00017").Top + 15;
            oForm.Items.Item("ET00016").Left = lblleft + oForm.Items.Item("EL00018").Width + 25;


            oForm.Items.Item("EL00019").Top = oForm.Items.Item("EL00018").Top + 15;
            oForm.Items.Item("EL00019").Left = lblleft;
            oForm.Items.Item("ET00017").Top = oForm.Items.Item("EL00018").Top + 15;
            oForm.Items.Item("ET00017").Left = lblleft + oForm.Items.Item("EL00019").Width + 25;
        }
        private void Enable(bool pblvalue)
        {

            oForm.Items.Item("EL0001").Visible = pblvalue;
            oForm.Items.Item("EL0002").Visible = pblvalue;
            oForm.Items.Item("EL0003").Visible = pblvalue;
            oForm.Items.Item("EL0004").Visible = pblvalue;
            oForm.Items.Item("EL0005").Visible = pblvalue;
            oForm.Items.Item("EL0006").Visible = pblvalue;
            oForm.Items.Item("EL0007").Visible = pblvalue;
            oForm.Items.Item("EL0008").Visible = pblvalue;
            oForm.Items.Item("EL0009").Visible = pblvalue;
            oForm.Items.Item("EL00010").Visible = pblvalue;
            oForm.Items.Item("EL00011").Visible = pblvalue;
            oForm.Items.Item("EL00012").Visible = pblvalue;
            oForm.Items.Item("EL00013").Visible = pblvalue;
            oForm.Items.Item("EL00014").Visible = pblvalue;
            oForm.Items.Item("EL00015").Visible = pblvalue;
            oForm.Items.Item("EL00016").Visible = pblvalue;
            oForm.Items.Item("EL00017").Visible = pblvalue;
            oForm.Items.Item("EL00018").Visible = pblvalue;
            oForm.Items.Item("EL00019").Visible = pblvalue;

          
            oForm.Items.Item("ET0001").Visible = pblvalue;
            oForm.Items.Item("ET0002").Visible = pblvalue;
            oForm.Items.Item("ET0003").Visible = pblvalue;
            oForm.Items.Item("ET0004").Visible = pblvalue;
            oForm.Items.Item("ET0005").Visible = pblvalue;
            oForm.Items.Item("ET0006").Visible = pblvalue;
            oForm.Items.Item("ET0007").Visible = pblvalue;
            oForm.Items.Item("ET0008").Visible = pblvalue;
            oForm.Items.Item("ET0009").Visible = pblvalue;
            oForm.Items.Item("ET00010").Visible = pblvalue;
            oForm.Items.Item("ET00011").Visible = pblvalue;
            oForm.Items.Item("ET00012").Visible = pblvalue;
            oForm.Items.Item("ET00013").Visible = pblvalue;
            oForm.Items.Item("ET00014").Visible = pblvalue;
            oForm.Items.Item("ET00015").Visible = pblvalue;
            oForm.Items.Item("ET00016").Visible = pblvalue;
            oForm.Items.Item("ET00017").Visible = pblvalue;
        }

        private void EditText36_KeyDownAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.CharPressed == 9)
            {
                Choose form1 = new Choose();
                form1. ActualForm = oForm;
                form1.Query = " SELECT \"TransCode\" ,\"TransName\" ,\"TransID\",t.\"TransMode\" ,t.\"VehicleTyp\", t.\"VehicleNo\"  FROM OTSP o " +
                        " left JOIN TSP1 t on o.\"AbsEntry\" = t.\"AbsEntry\"";


                form1.Show();


             


            }
        }

        private void Form_ActivateBefore(SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            throw new System.NotImplementedException();

        }
    }
}
