using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EInvoicing_Logitax_API.Common
{
    class clsGlobalMethods
    {
        string strsql;
        SAPbobsCOM.Recordset objrs;


        public void Load_Combo(string FormUID, SAPbouiCOM.ComboBox comboBox, string Query, string[] Validvalues = null)
        {
            try

            {
                SAPbobsCOM.Recordset objRs;

                string[] split_char;

            

                    if (comboBox.ValidValues.Count != 0) return;

                if (Validvalues != null)
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

                            comboBox.ValidValues.Add(split_char[0], split_char[1]);

                        }

                    }
                }



                objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                objRs.DoQuery(Query);
           
                if (objRs.RecordCount == 0) return;

                for (int i = 0; i < objRs.RecordCount; i++)

                {
                    
                    comboBox.ValidValues.Add(objRs.Fields.Item(0).Value.ToString(), objRs.Fields.Item(1).Value.ToString());

                    objRs.MoveNext();

                }

                if (Validvalues != null)
                {

                    comboBox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
                }

            }

            catch (Exception ex)

            {

                clsModule.objaddon.objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

            }

        }


        public string GetDocNum(string sUDOName, int Series)
        {
            string GetDocNumRet = "";
            string StrSQL;
            SAPbobsCOM.Recordset objRS;
            objRS = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            // If objAddOn.HANA Then
            if (Series == 0)
            {
                StrSQL = " select  \"NextNumber\"  from NNM1 where \"ObjectCode\"='" + sUDOName + "'";
            }
            else
            {
                StrSQL = " select  \"NextNumber\"  from NNM1 where \"ObjectCode\"='" + sUDOName + "' and \"Series\" = " + Series;
            }
            // Else
            // StrSQL = "select Autokey from onnm where objectcode='" & sUDOName & "'"
            // End If
            objRS.DoQuery(StrSQL);
            objRS.MoveFirst();
            if (!objRS.EoF)
            {
                return Convert.ToInt32(objRS.Fields.Item(0).Value.ToString()).ToString();
            }
            else
            {
                GetDocNumRet = "1";
            }

            return GetDocNumRet;
        }

        public string GetNextCode_Value(string Tablename)
        {
            try
            {
                if (string.IsNullOrEmpty(Tablename.ToString()))
                    return "";

                strsql = "select IFNULL(Max(CAST(\"Code\" As integer)),0)+1 from \"" + Tablename.ToString() + "\"";
                if (!clsModule.HANA)
                {
                    strsql = clsModule.objaddon.objglobalmethods.ChangeHANAtoSql(strsql);
                }

                objrs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                objrs.DoQuery(strsql);
                if (objrs.RecordCount > 0)
                    return Convert.ToString(objrs.Fields.Item(0).Value);
                else
                    return "";
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.SetStatusBarMessage("Error while getting next code numbe" + ex.ToString(), SAPbouiCOM.BoMessageTime.bmt_Medium, true);
                return "";
            }
        }

        public string GetNextDocNum_Value(string Tablename)
        {
            try
            {
                if (string.IsNullOrEmpty(Tablename.ToString()))
                    return "";
                strsql = "select IFNULL(Max(CAST(\"DocNum\" As integer)),0)+1 from \"" + Tablename.ToString() + "\"";
                if (!clsModule.HANA)
                {
                    strsql = clsModule.objaddon.objglobalmethods.ChangeHANAtoSql(strsql);
                }
                objrs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                objrs.DoQuery(strsql);
                if (objrs.RecordCount > 0)
                    return Convert.ToString(objrs.Fields.Item(0).Value);
                else
                    return "";
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.SetStatusBarMessage("Error while getting next code numbe" + ex.ToString(), SAPbouiCOM.BoMessageTime.bmt_Medium, true);
                return "";
            }
        }

        public List<string> SplitByLength(string str, int n)
        {
            List<string> substrings = new List<string>();
            int i = 0;
            while (i < str.Length)
            {
                int length = n;
                if (i + length < str.Length)
                {
                    while (length > 0 && !char.IsWhiteSpace(str[i + length - 1]))
                    {
                        length--;
                    }
                }
                else
                {
                    length = str.Length - i;
                }
                string insertvalue = str.Substring(i, length).Trim();
                if (insertvalue.Length <= 3) insertvalue ="  "+ insertvalue  ;
                substrings.Add(insertvalue);
                i += length;
            }
            return substrings;
        }

        public bool ExecuteQuery(string query)
        {
            try
            {
                objrs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                objrs.DoQuery(query);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }


            public string GetNextDocEntry_Value(string Tablename)
        {
            try
            {
                if (string.IsNullOrEmpty(Tablename.ToString()))
                    return "";
                strsql = "select IFNULL(Max(CAST(\"DocEntry\" As integer)),0)+1 from \"" + Tablename.ToString() + "\"";
                if (!clsModule.HANA)
                {
                    strsql = clsModule.objaddon.objglobalmethods.ChangeHANAtoSql(strsql);
                }
                objrs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                objrs.DoQuery(strsql);
                if (objrs.RecordCount > 0)
                    return Convert.ToString(objrs.Fields.Item(0).Value);
                else
                    return "";
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.SetStatusBarMessage("Error while getting next code numbe" + ex.ToString(), SAPbouiCOM.BoMessageTime.bmt_Medium, true);
                return "";
            }
        }

        public string Convert_String_TimeHHMM(string str)
        {
            str = "0000" + Regex.Replace(str, @"[^\d]", "");
            return str.PadRight(4);
        }

        public string GetDuration_BetWeenTime(string strFrom, string strTo)
        {
            DateTime Fromtime, Totime;
            TimeSpan Duration;
            strFrom = Convert_String_TimeHHMM(strFrom);
            strTo = Convert_String_TimeHHMM(strTo);
            Totime = new DateTime(2000, 1, 1, Convert.ToInt32(strTo.PadLeft(2)), Convert.ToInt32(strTo.PadLeft(2)), 0);
            Fromtime = new DateTime(2000, 1, 1, Convert.ToInt32(strFrom.PadLeft(2)), Convert.ToInt32(strFrom.PadRight(2)), 0);
            if (Totime < Fromtime)
                Totime = new DateTime(2000, 1, 2, Convert.ToInt32(strTo.PadLeft(2)), Convert.ToInt32(strTo.PadLeft(2)), 0);
            Duration = Totime - Fromtime;
            return Duration.Hours.ToString() + "." + Duration.Minutes.ToString() + "00".PadLeft(2);
        }


        public string getSingleValue(string StrSQL)
        {
            try
            {
                SAPbobsCOM.Recordset rset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                rset.DoQuery(StrSQL);
                return Convert.ToString((rset.RecordCount) > 0 ? rset.Fields.Item(0).Value.ToString() : "");
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText(" Get Single Value Function Failed :  " + ex.Message + StrSQL, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return "";
            }
        }

 


        //esta es la version original
        public DataTable RsTODataTabla(ref SAPbobsCOM.Recordset _rs)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < _rs.Fields.Count; i++)
                dt.Columns.Add(_rs.Fields.Item(i).Description);
            while (!_rs.EoF)
            {
                object[] array = new object[_rs.Fields.Count];
                for (int i = 0; i < _rs.Fields.Count; i++)
                    array[i] = _rs.Fields.Item(i).Value;
                dt.Rows.Add(array);
                _rs.MoveNext();
            }
            return dt;
        }
        public string Getdateformat(string Inv_Doc_Date)
        {
            string date = Inv_Doc_Date;
            if (string.IsNullOrEmpty(date))
                return "";
            List<DateTime> RemoveDate = new List<DateTime>();

            RemoveDate.Add(new DateTime(1999, 12, 30));
            RemoveDate.Add(new DateTime(1899, 12, 30));

         
          
            DateTime dateTime = Convert.ToDateTime(date);
            if (RemoveDate.Contains(dateTime.Date))
            {
                return "";
            }
            string dtformate = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            return dtformate;
        }
        public string Getdateformat(string Inv_Doc_Date, string format)
        {
            string date = Inv_Doc_Date;
            if (string.IsNullOrEmpty(date))
                return "";
            List<DateTime> RemoveDate = new List<DateTime>();
            
            RemoveDate.Add(new DateTime(1999, 12, 30));
            RemoveDate.Add(new DateTime(1899, 12, 30));


            DateTime dateTime = DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
            if (RemoveDate.Contains(dateTime.Date))
            {
                return "";
            }                  
            string dtformate = dateTime.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            return dtformate;
        }


        public decimal CtoD(object Pstring)
        {
            decimal CtoD = 0;
            try
            {
                decimal LdblResult;
                if (Pstring == null)
                    return CtoD;
                string Lstr = System.Convert.ToString(Pstring);

                if (decimal.TryParse(Lstr, out LdblResult))
                    CtoD = LdblResult;
                return CtoD;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal GetDecimalVal(object Pstring)
        {

            string value = objtostring(Pstring, "[a-zA-Z]");
            return CtoD(value);
        }

        public string objtostring(object Value,string Removepattern="")
        {
            string Returnstring = "";
            try
            {
               
                if (Value == null)
                    return Returnstring;

                Returnstring = Convert.ToString(Value);

                if (!string.IsNullOrEmpty(Removepattern))
                {
                    Returnstring = Regex.Replace(Returnstring, Removepattern, "");
                }
                return Returnstring;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string DateFormat(string Inv_Doc_Date, string Actualformat,string returnformat)
        {

            string date = Inv_Doc_Date;
            if (string.IsNullOrEmpty(date))
                return "";
            List<DateTime> RemoveDate = new List<DateTime>();

            RemoveDate.Add(new DateTime(1999, 12, 30));
            RemoveDate.Add(new DateTime(1899, 12, 30));


            DateTime dateTime = DateTime.ParseExact(date, Actualformat, CultureInfo.InvariantCulture);
            if (RemoveDate.Contains(dateTime.Date))
            {
                return "";
            }
          
            string dtformate = dateTime.ToString(returnformat, CultureInfo.InvariantCulture);
            return dtformate;
        }
        public DataTable GetmultipleValue(string StrSQL)
        {
            DataTable dt = new DataTable();
            try
            {
                SAPbobsCOM.Recordset rset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                rset.DoQuery(StrSQL);
                dt = RsTODataTabla(ref rset);
                return dt;
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText(" Get Multiple Value Function Failed :  " + ex.Message + StrSQL, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return dt;
            }
        }
        public SAPbobsCOM.Recordset GetmultipleRS(string StrSQL)
        {
            SAPbobsCOM.Recordset rset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {                
                rset.DoQuery(StrSQL);
                return rset;
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText(" Get Single Value Function Failed :  " + ex.Message + StrSQL, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return rset;
            }
        }

        public string ChangeHANAtoSql(string StrSQL)
        {
            string sql = StrSQL;
            try
            {
                sql = sql.Replace("IFNULL", "ISNULL");
                sql = sql.Replace("ifnull", "ISNULL");

                return sql;
            }
            catch (Exception ex)
            {
                return sql;
            }
        }

        public void LoadSeries(SAPbouiCOM.Form objform, SAPbouiCOM.DBDataSource DBSource, string ObjectType)
        {
            try
            {
                SAPbouiCOM.ComboBox ComboBox0;
                ComboBox0 = (SAPbouiCOM.ComboBox)objform.Items.Item("Series").Specific;
                ComboBox0.ValidValues.LoadSeries(ObjectType, SAPbouiCOM.BoSeriesMode.sf_Add);
                ComboBox0.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
                DBSource.SetValue("DocNum", 0, clsModule.objaddon.objglobalmethods.GetDocNum(ObjectType, Convert.ToInt32(ComboBox0.Selected.Value)));
            }
            catch (Exception ex)
            {

            }
        }
        public DataTable JsonFiletodt(string jsonpath)
        {
            DataTable dataTable = new DataTable();
            var json = System.IO.File.ReadAllText(jsonpath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return dataTable;
            }

            dataTable = Jsontodt(json);

            return dataTable;
        }

        public DataTable Jsontodt(string sampleJson)
        {
            DataTable dataTable = new DataTable();
            try
            {
                string json = sampleJson;
                JObject obj = JObject.Parse(json);

                // Create a DataTable and add the columns.

                foreach (JProperty property in obj.Properties())
                {
                    dataTable.Columns.Add(property.Name, property.Value.GetType());
                }

                DataRow row = dataTable.NewRow();
                foreach (JProperty property in obj.Properties())
                {
                    row[property.Name] = property.Value;
                }
                dataTable.Rows.Add(row);

                return dataTable;
            }
            catch (Exception)
            {
                dataTable = JsonConvert.DeserializeObject<DataTable>(sampleJson);
                return dataTable;
            }
        }




        public void WriteErrorLog(string Str)
        {
            try
            {

                string Foldername;
                Foldername = @"Log";
                if (Directory.Exists(Foldername))
                {
                }
                else
                {
                    Directory.CreateDirectory(Foldername);
                }

                FileStream fs;
                string chatlog = Foldername + @"\Log_" + DateTime.Now.ToString("ddMMyy") + ".txt";
                if (File.Exists(chatlog))
                {
                }
                else
                {
                    fs = new FileStream(chatlog, FileMode.Create, FileAccess.Write);
                    fs.Close();
                }
                string sdate;
                sdate = Convert.ToString(DateTime.Now);
                if (File.Exists(chatlog) == true)
                {
                    var objWriter = new StreamWriter(chatlog, true);
                    objWriter.WriteLine(sdate + " : " + Str);
                    objWriter.Close();
                }
                else
                {
                    var objWriter = new StreamWriter(chatlog, false);
                }
            }
            catch (Exception)
            {


            }
        }

        public void RemoveLastrow(SAPbouiCOM.Matrix omatrix, string Columname_check)
        {
            try
            {
                if (omatrix.VisualRowCount == 0)
                    return;
                if (string.IsNullOrEmpty(Columname_check.ToString()))
                    return;
                if (((SAPbouiCOM.EditText)omatrix.Columns.Item(Columname_check).Cells.Item(omatrix.VisualRowCount).Specific).String == "")
                {
                    omatrix.DeleteRow(omatrix.VisualRowCount);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void SetAutomanagedattribute_Editable(SAPbouiCOM.Form oform, string fieldname, bool add, bool find, bool update)
        {

            if (add == true)
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Add), SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            }
            else
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Add), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            }

            if (find == true)
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Find), SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            }
            else
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Find), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            }

            if (update)
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Ok), SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            }
            else
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Ok), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            }
        }

        public void SetAutomanagedattribute_Visible(SAPbouiCOM.Form oform, string fieldname, bool add, bool find, bool update)
        {

            if (add == true)
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Add), SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            }
            else
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Add), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            }

            if (find == true)
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Find), SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            }
            else
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Find), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            }

            if (update)
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Ok), SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            }
            else
            {
                oform.Items.Item(fieldname).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Ok), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            }

        }

        public void Matrix_Addrow(SAPbouiCOM.Matrix omatrix, string colname = "", string rowno_name = "", bool Error_Needed = false)
        {
            try
            {
                bool addrow = false;

                if (omatrix.VisualRowCount == 0)
                {
                    addrow = true;
                    goto addrow;
                }
                if (string.IsNullOrEmpty(colname))
                {
                    addrow = true;
                    goto addrow;
                }
                if (((SAPbouiCOM.EditText)omatrix.Columns.Item(colname).Cells.Item(omatrix.VisualRowCount).Specific).String != "")
                {
                    addrow = true;
                    goto addrow;
                }

            addrow:
                ;

                if (addrow == true)
                {
                    omatrix.AddRow(1);
                    omatrix.ClearRowData(omatrix.VisualRowCount);
                    if (!string.IsNullOrEmpty(rowno_name))
                        ((SAPbouiCOM.EditText)omatrix.Columns.Item("#").Cells.Item(omatrix.VisualRowCount).Specific).String = Convert.ToString(omatrix.VisualRowCount);
                }
                else if (Error_Needed == true)
                    clsModule.objaddon.objapplication.SetStatusBarMessage("Already Empty Row Available", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
            catch (Exception ex)
            {

            }
        }

        public int GetColumnindex(SAPbouiCOM.Matrix matrix, string uniquecode)
        {
            string columnName = uniquecode;
            for (int i = 0; i <= matrix.Columns.Count - 1; i++)
            {
                if (matrix.Columns.Item(i).UniqueID == columnName)
                {
                    return i;
                }
            }
            return -1;
        }


        public static string columnFind(DataTable dt, string Findcol, int Rowcount)
        {
            string Retavl = "";
            DataColumn foundColumn = dt.Columns.Cast<DataColumn>().FirstOrDefault(col => col.ColumnName == Findcol);
            if (foundColumn != null)
            {
                Retavl = ObjtoStr(dt.Rows[Rowcount][Findcol]);
            }

            return Retavl;
        }
        public static string ObjtoStr(object Value)
        {
            string Returnstring = "";

            if (Value == null)
                return Returnstring;



            Returnstring = Convert.ToString(Value);
            return Returnstring;
        }
        public static DateTime Getdate(string Date, string format)
        {
            if (string.IsNullOrEmpty(Date))
                return new DateTime(1900, 01, 01);

            DateTime dateTime = DateTime.ParseExact(Date, format, CultureInfo.InvariantCulture);

            return dateTime;
        }
        public int Ctoint(object Pstring)
        {
            int Ctoint = 0;
            try
            {
                int LdblResult;
                if (Pstring == null)
                    return Ctoint;
                string Lstr = System.Convert.ToString(Pstring);

                if (int.TryParse(Lstr, out LdblResult))
                    Ctoint = LdblResult;
                return Ctoint;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
