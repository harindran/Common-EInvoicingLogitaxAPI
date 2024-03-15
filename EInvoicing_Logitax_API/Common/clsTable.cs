using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM.Framework;
namespace EInvoicing_Logitax_API.Common
{
    class clsTable
    {        
        public void FieldCreation()
        {
            AddTables("ATPL_EINV", "Document IRN", SAPbobsCOM.BoUTBTableType.bott_Document);

            AddFields("@ATPL_EINV", "QRCode", "Signed QR Code", SAPbobsCOM.BoFieldTypes.db_Memo);
            AddFields("@ATPL_EINV", "SgnInv", "Signed Invoice", SAPbobsCOM.BoFieldTypes.db_Memo);
            AddFields("@ATPL_EINV", "IRNNo", "IRN No", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
            AddFields("@ATPL_EINV", "GenDate", "Generation Date", SAPbobsCOM.BoFieldTypes.db_Date);
            AddFields("@ATPL_EINV", "CanDate", "Cancellation Date", SAPbobsCOM.BoFieldTypes.db_Date);
            AddFields("@ATPL_EINV", "BaseNo", "Base DocNo", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATPL_EINV", "BaseEntry", "Base DocEntry", SAPbobsCOM.BoFieldTypes.db_Alpha, 30); 
            AddFields("@ATPL_EINV", "DocObjType", "Doc Object Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);            
            AddFields("@ATPL_EINV", "Remarks", "Einv Remarks", SAPbobsCOM.BoFieldTypes.db_Memo);            
            AddFields("@ATPL_EINV", "AckNum", "Acknowledge No", SAPbobsCOM.BoFieldTypes.db_Alpha,100);
            AddFields("@ATPL_EINV", "IRNStat", "IRN Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 5);
            AddFields("@ATPL_EINV", "DcrptInv", "Decrypt Signed Invoice", SAPbobsCOM.BoFieldTypes.db_Memo);
            AddFields("@ATPL_EINV", "DcrptQRCode", "Decrypt Signed QR Code", SAPbobsCOM.BoFieldTypes.db_Memo);
            AddFields("@ATPL_EINV", "ErrLogId", "Error Log ID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATPL_EINV", "Flag", "Flag", SAPbobsCOM.BoFieldTypes.db_Alpha, 10);
            AddFields("@ATPL_EINV", "Einvreqjson", "EinvJson", SAPbobsCOM.BoFieldTypes.db_Memo);
            AddFields("@ATPL_EINV", "Ewayreqjson", "EwayJson", SAPbobsCOM.BoFieldTypes.db_Memo);
            AddFields("@ATPL_EINV", "Ewaypdf", "EwayPdf", SAPbobsCOM.BoFieldTypes.db_Memo);
            AddFields("@ATPL_EINV", "EwayDetpdf", "Eway detailed Pdf", SAPbobsCOM.BoFieldTypes.db_Memo);
            
            AddFields("@ATPL_EINV", "EwbNum", "Eway Bill No", SAPbobsCOM.BoFieldTypes.db_Alpha,100);
            AddFields("@ATPL_EINV", "EwbDate", "Eway Bill Date", SAPbobsCOM.BoFieldTypes.db_Date);
            AddFields("@ATPL_EINV", "EwbValidTill", "Eway Bill ValidTill", SAPbobsCOM.BoFieldTypes.db_Date);
            AddFields("@ATPL_EINV", "EwbCanDate", "Eway Cancellation Date", SAPbobsCOM.BoFieldTypes.db_Date);

            AddUDO("ATEINV", "Transaction IRN", SAPbobsCOM.BoUDOObjType.boud_Document, "ATPL_EINV", new[] { "" }, new[] { "DocEntry", "DocNum" }, true, true);
            
            AddFields("OINV", "IRNNo", "IRN No", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
            AddFields("OINV", "QRCode", "Signed QR Code", SAPbobsCOM.BoFieldTypes.db_Memo);
            AddFields("OINV", "AckDate", "Acknowledgement Date", SAPbobsCOM.BoFieldTypes.db_Date);
            AddFields("OINV", "AckNo", "Acknowledgement No", SAPbobsCOM.BoFieldTypes.db_Alpha,100);
            AddFields("OINV", "IRNStatus", "IRNStatus", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);            
            AddFields("OINV", "EwayStatus", "EwayStatus", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);            
            AddFields("OINV", "Ewaypdf", "EwayPdf", SAPbobsCOM.BoFieldTypes.db_Alpha, 200);            
            AddFields("OINV", "EwayDetpdf", "Eway detailed Pdf", SAPbobsCOM.BoFieldTypes.db_Alpha, 200);




            AddFields("WTR1", "UTL_ST_TAXCD", "Tax Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
            AddFields("WTR1", "UTL_ST_CGST", "CGST Rate", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Percentage);
            AddFields("WTR1", "UTL_ST_SGST", "SGST Rate", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Percentage);
            AddFields("WTR1", "UTL_ST_IGST", "IGST Rate", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Percentage);
            AddFields("WTR1", "UTL_ST_CGAMT", "CGST Amount", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Percentage);
            AddFields("WTR1", "UTL_ST_SGAMT", "SGST Amount", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Percentage);
            AddFields("WTR1", "UTL_ST_IGAMT", "IGST Amount", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Percentage);
            AddFields("WTR1", "UTL_ST_LINETOTAL", "Line Total", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Percentage);





            AddTables("ATEICFG", "E-Invoice Config Header", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            AddTables("ATEICFG1", "E-Invoice Config Lines", SAPbobsCOM.BoUTBTableType.bott_MasterDataLines);
            AddFields("@ATEICFG", "ClientCode", "Client Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATEICFG", "UserCode", "User Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATEICFG", "Password", "Password", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATEICFG", "Live", "Live", SAPbobsCOM.BoFieldTypes.db_Alpha, 5);
            AddFields("@ATEICFG", "UATUrl", "UAT Url", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
            AddFields("@ATEICFG", "LIVEUrl", "LIVE Url", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
            AddFields("@ATEICFG", "ItemDesc", "Item Desc", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
            AddFields("@ATEICFG", "HSNL", "HSN LENGTH", SAPbobsCOM.BoFieldTypes.db_Numeric);
            AddFields("@ATEICFG", "SERCONFIG", "Series Configuration", SAPbobsCOM.BoFieldTypes.db_Memo);
            AddFields("@ATEICFG", "GetCompAdd", "Get Company Address", SAPbobsCOM.BoFieldTypes.db_Alpha,100);
            AddFields("@ATEICFG", "InvTranGetcusAdd", "Inventory Transfer Get Customer Address", SAPbobsCOM.BoFieldTypes.db_Alpha,100);
            AddFields("@ATEICFG", "InvTranGetBrnchAdd", "Inventory Transfer Get Branch Address", SAPbobsCOM.BoFieldTypes.db_Alpha,100);
            AddFields("@ATEICFG", "GetDisAddWare", "Dispatch Address WareHouse", SAPbobsCOM.BoFieldTypes.db_Alpha,100);
            AddFields("@ATEICFG", "Gettran", "Get  Transaction Customer Address", SAPbobsCOM.BoFieldTypes.db_Alpha,100);
            AddFields("@ATEICFG", "GSTCol", "GST Column", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
            AddFields("@ATEICFG", "HSNCol", "HSN Column", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
            AddFields("@ATEICFG", "AddTCSOth", "Add TCS Amount in Other Charge", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);

            AddFields("@ATEICFG", "BillToWare", "Bill To warehouse", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATEICFG", "ShipToInvName", "Ship To Inventory Name", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATEICFG", "GettrnShp", "Get  Transaction Ship Address", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATEICFG", "NotUseQrcode", "QRcode Enable In Accounts Tab", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATEICFG", "BlockEway", "Block Eway Without Einvoice", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);

            AddFields("@ATEICFG", "DBType", "SQL Or HANA", SAPbobsCOM.BoFieldTypes.db_Alpha, 5);
            AddFields("@ATEICFG", "EwayNo", "UDF EwayNo", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
            AddFields("@ATEICFG", "VehNo", "UDF VehicleNo", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
            AddFields("@ATEICFG", "TransID", "UDF TransPortID", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
            AddFields("@ATEICFG", "TransName", "UDF Transport Name", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
            AddFields("@ATEICFG", "Distance", "UDF Distance", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
            AddFields("@ATEICFG", "BtnPos", "Button Position", SAPbobsCOM.BoFieldTypes.db_Numeric);



            AddFields("@ATEICFG1", "URLType", "URL Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
            AddFields("@ATEICFG1", "Type", "Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATEICFG1", "URL", "URL", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);

            AddUDO("ATCFG", "E-Invoice Config", SAPbobsCOM.BoUDOObjType.boud_MasterData, "ATEICFG", new[] { "ATEICFG1" }, new[] { "Code", "Name" }, true, false);


           
            AddTables("UOMMAP", "UOM Mapping", SAPbobsCOM.BoUTBTableType.bott_Document);
            

            AddFields("@UOMMAP", "GUnitCod", "Govt Unit Code", SAPbobsCOM.BoFieldTypes.db_Alpha,100);
            AddFields("@UOMMAP", "GUnitDes", "Govt Unit Description", SAPbobsCOM.BoFieldTypes.db_Alpha,100);
            AddFields("@UOMMAP", "UOMCod", "UOM Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
        
            AddUDO("AUOMMAP", "UOM Mapping", SAPbobsCOM.BoUDOObjType.boud_Document, "UOMMAP", new[] { "" }, new[] { "DocEntry", "DocNum" }, true, false);






            AddFields("@ATEICFG", "GST_ClientCode", "Client Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATEICFG", "GST_UserCode", "User Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATEICFG", "GST_Password", "Password", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("@ATEICFG", "GST_Live", "Live", SAPbobsCOM.BoFieldTypes.db_Alpha, 5);
            AddFields("@ATEICFG", "GST_UATUrl", "UAT Url", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
            AddFields("@ATEICFG", "GST_LIVEUrl", "LIVE Url", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
            AddFields("@ATEICFG", "GST_Token", "Token", SAPbobsCOM.BoFieldTypes.db_Memo);

            AddFields("OINV", "GST_RefNo", "GSTRefNum", SAPbobsCOM.BoFieldTypes.db_Alpha, 200);
            AddFields("OINV", "GST_status", "GSTStatus", SAPbobsCOM.BoFieldTypes.db_Numeric);
            AddFields("OINV", "GST_Remarks", "GSTRemarks", SAPbobsCOM.BoFieldTypes.db_Memo);

            AddFields("OBPL", "ClientCode", "Client Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("OBPL", "UserCode", "User Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddFields("OBPL", "Password", "Password", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);

        }

        public void FieldCreationNewversion()
        {
            AddFields("INV26", "Dispatch_Eway", "Dispatch From Eway Bill", SAPbobsCOM.BoFieldTypes.db_Alpha, 5);
            AddFields("INV26", "Dispatch_Name", "Dispatch Name", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
        }

            #region Table Creation Common Functions

            private void AddTables(string strTab, string strDesc, SAPbobsCOM.BoUTBTableType nType)
        {
            // var oUserTablesMD = default(SAPbobsCOM.UserTablesMD);
            SAPbobsCOM.UserTablesMD oUserTablesMD = null;
            try
            {
                oUserTablesMD = (SAPbobsCOM.UserTablesMD)clsModule. objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                // Adding Table
                if (!oUserTablesMD.GetByKey(strTab))
                {
                    oUserTablesMD.TableName = strTab;
                    oUserTablesMD.TableDescription = strDesc;
                    oUserTablesMD.TableType = nType;

                    if (oUserTablesMD.Add() != 0)
                    {
                        throw new Exception(clsModule.objaddon.objcompany.GetLastErrorDescription() + strTab);
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTablesMD);
                oUserTablesMD = null;
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void AddFields(string strTab, string strCol, string strDesc, SAPbobsCOM.BoFieldTypes nType, int nEditSize = 10, SAPbobsCOM.BoFldSubTypes nSubType = 0, SAPbobsCOM.BoYesNoEnum Mandatory = SAPbobsCOM.BoYesNoEnum.tNO, string defaultvalue = "", bool Yesno = false, string[] Validvalues = null)
        {
            SAPbobsCOM.UserFieldsMD oUserFieldMD1;
            oUserFieldMD1 = (SAPbobsCOM.UserFieldsMD)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
            try
            {
             
                if (!IsColumnExists(strTab, strCol))
                {                   
                    oUserFieldMD1.Description = strDesc;
                    oUserFieldMD1.Name = strCol;
                    oUserFieldMD1.Type = nType;
                    oUserFieldMD1.SubType = nSubType;
                    oUserFieldMD1.TableName = strTab;
                    oUserFieldMD1.EditSize = nEditSize;
                    oUserFieldMD1.Mandatory = Mandatory;
                    oUserFieldMD1.DefaultValue = defaultvalue;

                    if (Yesno == true)
                    {
                        oUserFieldMD1.ValidValues.Value = "Y";
                        oUserFieldMD1.ValidValues.Description = "Yes";
                        oUserFieldMD1.ValidValues.Add();
                        oUserFieldMD1.ValidValues.Value = "N";
                        oUserFieldMD1.ValidValues.Description = "No";
                        oUserFieldMD1.ValidValues.Add();
                    }
                    //if (LinkedSystemObject != 0)
                    //    oUserFieldMD1.LinkedSystemObject = LinkedSystemObject;

                    string[] split_char;
                    if (Validvalues !=null)
            {
                        if (Validvalues.Length > 0)
                        {
                            for (int i = 0, loopTo = Validvalues.Length - 1; i <= loopTo; i++)
                            {
                                if (string.IsNullOrEmpty(Validvalues[i]))
                                    continue;
                                split_char = Validvalues[i].Split(Convert.ToChar(","));
                                if (split_char.Length != 2)
                                    continue;
                                oUserFieldMD1.ValidValues.Value = split_char[0];
                                oUserFieldMD1.ValidValues.Description = split_char[1];
                                oUserFieldMD1.ValidValues.Add();
                            }
                        }
                    }
                    int val;
                    val = oUserFieldMD1.Add();
                    if (val != 0)
                    {
                        clsModule.objaddon.objapplication.SetStatusBarMessage(clsModule. objaddon.objcompany.GetLastErrorDescription() + " " + strTab + " " + strCol, SAPbouiCOM.BoMessageTime.bmt_Short,true);
                    }
                    // System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldMD1)
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldMD1);
                oUserFieldMD1 = null;
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private bool IsColumnExists(string Table, string Column)
        {
            SAPbobsCOM.Recordset oRecordSet=null;
            string strSQL;
            try
            {
               
                strSQL = "SELECT COUNT(*) FROM CUFD WHERE \"TableID\" = '" + Table + "' AND \"AliasID\" = '" + Column + "'";
                             
                oRecordSet = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRecordSet.DoQuery(strSQL);

                if (Convert.ToInt32( oRecordSet.Fields.Item(0).Value) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
                oRecordSet = null;
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void AddKey(string strTab, string strColumn, string strKey, int i)
        {
            var oUserKeysMD = default(SAPbobsCOM.UserKeysMD);

            try
            {
                // // The meta-data object must be initialized with a
                // // regular UserKeys object
                oUserKeysMD =(SAPbobsCOM.UserKeysMD)clsModule. objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserKeys);

                if (!oUserKeysMD.GetByKey("@" + strTab, i))
                {

                    // // Set the table name and the key name
                    oUserKeysMD.TableName = strTab;
                    oUserKeysMD.KeyName = strKey;

                    // // Set the column's alias
                    oUserKeysMD.Elements.ColumnAlias = strColumn;
                    oUserKeysMD.Elements.Add();
                    oUserKeysMD.Elements.ColumnAlias = "RentFac";

                    // // Determine whether the key is unique or not
                    oUserKeysMD.Unique = SAPbobsCOM.BoYesNoEnum.tYES;

                    // // Add the key
                    if (oUserKeysMD.Add() != 0)
                    {
                        throw new Exception(clsModule. objaddon.objcompany.GetLastErrorDescription());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserKeysMD);
                oUserKeysMD = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void AddUDO(string strUDO, string strUDODesc, SAPbobsCOM.BoUDOObjType nObjectType, string strTable, string[] childTable, string[] sFind, bool canlog = false, bool Manageseries = false)
        {

           SAPbobsCOM.UserObjectsMD oUserObjectMD=null;
            int tablecount = 0;
            try
            {
                oUserObjectMD = (SAPbobsCOM.UserObjectsMD)clsModule. objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
               
                if (!oUserObjectMD.GetByKey(strUDO)) //(oUserObjectMD.GetByKey(strUDO) == 0)
                {
                    oUserObjectMD.Code = strUDO;
                    oUserObjectMD.Name = strUDODesc;
                    oUserObjectMD.ObjectType = nObjectType;
                    oUserObjectMD.TableName = strTable;

                    oUserObjectMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectMD.CanClose = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES;

                    if (Manageseries)
                        oUserObjectMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tYES;
                    else
                        oUserObjectMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO;

                    if (canlog)
                    {
                        oUserObjectMD.CanLog = SAPbobsCOM.BoYesNoEnum.tYES;
                        oUserObjectMD.LogTableName = "A" + strTable.ToString();
                    }
                    else
                    {
                        oUserObjectMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO;
                        oUserObjectMD.LogTableName = "";
                    }

                    oUserObjectMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectMD.ExtensionName = "";

                    oUserObjectMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;
                    tablecount = 1;
                    if (sFind.Length > 0)
                    {
                        for (int i = 0, loopTo = sFind.Length - 1; i <= loopTo; i++)
                        {
                            if (string.IsNullOrEmpty(sFind[i]))
                                continue;
                            oUserObjectMD.FindColumns.ColumnAlias = sFind[i];
                            oUserObjectMD.FindColumns.Add();
                            oUserObjectMD.FindColumns.SetCurrentLine(tablecount);
                            tablecount = tablecount + 1;
                        }
                    }

                    tablecount = 0;
                    if (childTable != null)
            {
                        if (childTable.Length > 0)
                        {
                            for (int i = 0, loopTo1 = childTable.Length - 1; i <= loopTo1; i++)
                            {
                                if (string.IsNullOrEmpty(childTable[i]))
                                    continue;
                                oUserObjectMD.ChildTables.SetCurrentLine(tablecount);
                                oUserObjectMD.ChildTables.TableName = childTable[i];
                                oUserObjectMD.ChildTables.Add();
                                tablecount = tablecount + 1;
                            }
                        }
                    }

                    if (oUserObjectMD.Add() != 0)
                    {
                        throw new Exception(clsModule. objaddon.objcompany.GetLastErrorDescription());
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectMD);
                oUserObjectMD = null;
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

        }


        #endregion


        

    }
}
