using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Linq.Mapping;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using StrippedXMLToSQL.AlkenMaes;
using StrippedXMLToSQL.BoschSP;
using StrippedXMLToSQL.DHL;
using StrippedXMLToSQL.FSS;
using StrippedXMLToSQL.Nuon;
using StrippedXMLToSQL.Planon;
using StrippedXMLToSQL.Volta;
using StrippedXMLToSQL.Jaski;
using StrippedXMLToSQL.Geveke;
using StrippedXMLToSQL.Worksphere;

namespace StrippedXMLToSQL
{
    public partial class frmMain : Form
    {
        private readonly Regex regexSplitObjects;
        private readonly Regex regexTableIDAndFields;

        public frmMain()
        {
            InitializeComponent();
            regexSplitObjects = new Regex(@"(?<entry><?[^<]+(/>|>))+", RegexOptions.Compiled);
            regexTableIDAndFields = new Regex(@"<(?<table>[0-9]*) ((?<field>""[^""']*"")( |>|/>))*", RegexOptions.Compiled);
        }

        private void FrmMainDragDrop(object sender, DragEventArgs e)
        {
            foreach (string strFileName in (string[]) e.Data.GetData(DataFormats.FileDrop, false))
            {
                ProcessFile(strFileName);
            }
        }

        private void FrmMainDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
        }

        private void ProcessFile(string name)
        {
            DateTime startTime = DateTime.Now;
            string strName = "";
            if (rbtnByChunks.Checked)
            {
                strName = "Convert By Chunks";
                ParseByStream(name);
            }
            else if (rbtnByCompiledRegEx.Checked)
            {
                strName = "Convert By Compiled Regex";
                ParseByCompiledRegEx(name);
            }
            TimeSpan duration = DateTime.Now - startTime;
            toolStripStatusLabel1.Text = String.Format("Process '{0}' took: {1}", strName, duration);
        }

        public class GevekeRepositoryFactory: IRepositoryFactory
        {
            public Stream CreateXmlMapReader()
            {
                return new FileStream(@"FieldVision_Geveke_Ontw.xml", FileMode.Open);
            }

            public IRepositoryDataContext CreateAdapter(XmlMappingSource objSource)
            {
                return new MobileWorkflow_Geveke_Ontw(
                ConfigurationManager.ConnectionStrings["Fieldvision_Geveke_Ontw"].ConnectionString, objSource);
            }

            public IEnumerable<ICommField> CreateCommFieldEnumeration(IRepositoryDataContext repositoryDataContext, int intTableID)
            {
                var ret = new Collection<ICommField>();
                foreach (StrippedXMLToSQL.Geveke.Fv_commfield fields in from c in repositoryDataContext.GetTable<Geveke.Fv_commfield>()
                                                                        where c.TableID == intTableID
                                                                        select c)
                {
                    ret.Add(fields);
                }
                return ret;
            }
            public IEnumerable<ICommTable> CreateCommTableEnumeration(IRepositoryDataContext repositoryDataContext, int intTableID)
            {
                var ret = new Collection<ICommTable>();
                foreach (StrippedXMLToSQL.Geveke.Fv_commtable table in from c in repositoryDataContext.GetTable<Geveke.Fv_commtable>()
                                                                       where c.TableID == intTableID
                                                                       select c)
                {
                    ret.Add(table);
                }
                return ret;
            }
        }

        public class SimpleRepositoryFactory
        {
            public static IRepositoryFactory CreateRepositoryFactory(String type)
            {
                IRepositoryFactory repositoryFactory = null;
                if(type == "Geveke")
                {
                    repositoryFactory = new GevekeRepositoryFactory();
                }
                if(type == "FSS")
                {
                    repositoryFactory = new FSSRepositoryFactory();
                }
                if (type == "Worksphere")
                {
                    repositoryFactory = new WorksphereRepositoryFactory();
                }
                if (type == "Planon")
                {
                    repositoryFactory = new PlanonRepositoryFactory();
                }
                if (type == "Nuon")
                {
                    repositoryFactory = new NuonRepositoryFactory();
                }
                if (type == "DHL")
                {
                    repositoryFactory = new DHLRepositoryFactory();
                }
                if (type == "Assa Abloy")
                {
                    repositoryFactory = new AssaAbloyRepositoryFactory();
                }
                if (type == "Bosch SP")
                {
                    repositoryFactory = new BoschSPRepositoryFactory();
                }
                return repositoryFactory;
            }
        }

        private void ParseByCompiledRegEx(string name)
        {
            IRepositoryFactory repositoryFactory = SimpleRepositoryFactory.CreateRepositoryFactory(cboDatabase.SelectedItem.ToString());

            using (var objReader = new StreamReader(name))
            {
                txtInsert.Text = "";
                txtUpdate.Text = "";
                XmlMappingSource objSource;
                using ( 
                    Stream objXMLMapReader = repositoryFactory.CreateXmlMapReader()
                        )
                {
                    objSource = XmlMappingSource.FromStream(objXMLMapReader);
                }

                var db = repositoryFactory.CreateAdapter(objSource);

                var objStringBuilder = new StringBuilder();
                var objStringBuilderInsert = new StringBuilder();
                var objStringBuilderUpdate = new StringBuilder();
                objStringBuilder.Append(objReader.ReadToEnd());

                CaptureCollection objCaptureCollection =
                    regexSplitObjects.Match(objStringBuilder.ToString()).Groups["entry"].Captures;
                foreach (Capture objCapture in objCaptureCollection)
                {
                    GroupCollection objGroupCollection = regexTableIDAndFields.Match(objCapture.Value).Groups;
                    int intTableID = int.Parse(objGroupCollection["table"].Value);

                    foreach (ICommTable table in repositoryFactory.CreateCommTableEnumeration(db,intTableID))
                    {
                        objStringBuilderInsert.Append(string.Format("INSERT INTO {0}{1}", table.TableName,
                            Environment.NewLine));
                        objStringBuilderUpdate.Append(string.Format("UPDATE {0} SET{1}", table.TableName, Environment.NewLine));
                        string strFields = "";
                        string strUpdateFields = "";
                        string strWhereStatement = "";
                        int intIndex = 0;
                        foreach (ICommField fields in (IEnumerable<ICommField>) repositoryFactory.CreateCommFieldEnumeration(db,intTableID))
                        {
                            strFields += string.Format(",[{0}] ", fields.FieldName);
                            string strUpdateValue = objGroupCollection["field"].Captures[intIndex].Value.Replace('"','\'');
                            strUpdateValue = strUpdateValue == "''" ? "null" : strUpdateValue;
                            string strUpdateField = string.Format(",{0} = {1}", fields.FieldName,
                                                          strUpdateValue);
                            strUpdateFields += (fields.FieldType == 131)?strUpdateField.Replace(',', '.'):strUpdateField;
                            
                            if (fields.FieldName.ToUpper() == "ID")
                            {
                                strWhereStatement = strUpdateField.Remove(0, 1);
                            }
                            intIndex++;
                        }
                        objStringBuilderUpdate.Append(strUpdateFields.Remove(0, 1));
                        objStringBuilderUpdate.Append(string.Format("{0}WHERE {1};{2}", Environment.NewLine, strWhereStatement,
                                                        Environment.NewLine));
                        objStringBuilderInsert.Append(string.Format("({0}){1}", strFields.Remove(0, 1), Environment.NewLine));
                        objStringBuilderInsert.Append(string.Format("VALUES{0}(", Environment.NewLine));
                        StringBuilder objStringBuilderValues = new StringBuilder();
                        foreach (Capture objFieldCapture in objGroupCollection["field"].Captures)
                        {
                            string strValue = objFieldCapture.Value.Replace('"', '\'');
                            strValue = strValue == "''" ? "null" : strValue;
                            objStringBuilderValues.Append(string.Format(",{0}", strValue));
                        }
                        objStringBuilderInsert.Append(string.Format("{0});{1}", objStringBuilderValues.ToString().Remove(0, 1), Environment.NewLine));
                    }
                }
                txtInsert.Text = objStringBuilderInsert.ToString();
                txtUpdate.Text = objStringBuilderUpdate.ToString();
            }
        }

        private void ParseByStream(string name)
        {
            using (var objReader = new StreamReader(name))
            {
                txtInsert.Text = "";
                txtUpdate.Text = "";
                XmlMappingSource objSource;
                using (
                    Stream objXMLMapReader =
                        new FileStream(@"c:\Mobility\Projects\StrippedXMLToSQL\StrippedXMLToSQL\FieldVision_Geveke_Ontw.xml", FileMode.Open))
                {
                    objSource = XmlMappingSource.FromStream(objXMLMapReader);
                }
                var db =
                    new MobileWorkflow_Geveke_Ontw(
                        ConfigurationManager.ConnectionStrings["FieldVision_Geveke_Ontw"].ConnectionString, objSource);
                string strLine;
                do
                {
                    strLine = objReader.ReadLine();
                    if (strLine.StartsWith("<"))
                    {
                        foreach (string strChunk in strLine.Split('<'))
                        {
                            if (strChunk.EndsWith(">"))
                            {
                                int intTableId = int.Parse(strChunk.Split(' ')[0]);
                                foreach (StrippedXMLToSQL.Geveke.Fv_commtable table in from c in db.GetTable<Geveke.Fv_commtable>()
                                                               where c.TableID == intTableId
                                                               select c)
                                {
                                    txtInsert.Text += "INSERT INTO " + table.TableName + Environment.NewLine;
                                    txtUpdate.Text += "UPDATE " + table.TableName + " SET" + Environment.NewLine;
                                    string strFields = "";
                                    foreach (StrippedXMLToSQL.Geveke.Fv_commfield fields in from c in db.GetTable<Geveke.Fv_commfield>()
                                                                    where c.TableID == intTableId
                                                                    select c)
                                    {
                                        strFields += ",[" + fields.FieldName + "] ";
                                    }
                                    txtInsert.Text += "(" + strFields.Remove(0, 1) + ")" + Environment.NewLine;
                                    txtInsert.Text += "VALUES" + Environment.NewLine + "(";
                                    int intFieldIndex = 0;
                                    string strWhere = "";
                                    string strValue = "";
                                    bool blnHasIgnoredFirstValue = false;
                                    foreach (string strValueChunk in strChunk.Split(' '))
                                    {
                                        if (!blnHasIgnoredFirstValue)
                                        {
                                            blnHasIgnoredFirstValue = true;
                                            continue;
                                        }
                                        if (strValueChunk.StartsWith("\"") && strValueChunk.EndsWith("\""))
                                        {
                                            strValue = strValueChunk;
                                        }
                                        if (strValueChunk.StartsWith("\"") && strValueChunk.EndsWith("\">"))
                                        {
                                            strValue = strValueChunk;
                                        }
                                        if (strValueChunk.StartsWith("\"") &&
                                            !(strValueChunk.EndsWith("\">") || strValueChunk.EndsWith("\"")))
                                        {
                                            strValue = strValueChunk;
                                            continue;
                                        }
                                        if (!strValueChunk.StartsWith("\""))
                                        {
                                            strValue += " " + strValueChunk;
                                            if (!(strValueChunk.EndsWith("\">") || strValueChunk.EndsWith("\"")))
                                            {
                                                continue;
                                            }
                                        }
                                        intFieldIndex++;
                                        int intFieldIndex2 = 0;
                                        string strField = "";
                                        foreach (Jaski.Fv_commfield fields in from c in db.GetTable<Jaski.Fv_commfield>()
                                                                        where c.TableID == intTableId
                                                                        select c)
                                        {
                                            intFieldIndex2++;
                                            if (intFieldIndex2 == intFieldIndex)
                                            {
                                                strField = fields.FieldName;
                                            }
                                        }
                                        if (intFieldIndex == 1)
                                        {
                                            strWhere = strField + " = '" +
                                                       strValue.Remove(strValue.Length - 1, 1).Remove(0, 1) + "'" +
                                                       Environment.NewLine;
                                        }
                                        if (strValueChunk.EndsWith(">"))
                                        {
                                            if (intFieldIndex == 1)
                                            {
                                                strWhere = strField + " = '" +
                                                           strValue.Remove(strValue.Length - 2, 2).Remove(0, 1) + "'" +
                                                           Environment.NewLine;
                                            }
                                            if (strValueChunk == "\"\">")
                                            {
                                                txtInsert.Text += "null);" + Environment.NewLine;
                                                txtUpdate.Text += strField + " = null" + Environment.NewLine;
                                            }
                                            else
                                            {
                                                txtInsert.Text += "'" +
                                                                  strValue.Remove(strValue.Length - 2, 2).Remove(0, 1) +
                                                                  "');" + Environment.NewLine;
                                                txtUpdate.Text += strField + " = '" +
                                                                  strValue.Remove(strValue.Length - 2, 2).Remove(0, 1) +
                                                                  "'" + Environment.NewLine;
                                            }
                                        }
                                        else
                                        {
                                            if (strValueChunk == "\"\"")
                                            {
                                                txtInsert.Text += "null,";
                                            }
                                            else
                                            {
                                                txtInsert.Text += "'" +
                                                                  strValue.Remove(strValue.Length - 1, 1).Remove(0, 1) +
                                                                  "',";
                                            }
                                            if (intFieldIndex == 1)
                                            {
                                                strWhere = strField + " = '" +
                                                           strValue.Remove(strValue.Length - 1, 1).Remove(0, 1) + "'";
                                            }
                                            else
                                            {
                                                if (strValueChunk == "\"\"")
                                                {
                                                    txtUpdate.Text += strField + " = null,";
                                                }
                                                else
                                                {
                                                    txtUpdate.Text += strField + " = '" +
                                                                      strValue.Remove(strValue.Length - 1, 1).Remove(0,
                                                                                                                     1) +
                                                                      "',";
                                                }
                                            }
                                        }
                                    }
                                    txtUpdate.Text += "WHERE " + strWhere + ";" + Environment.NewLine;
                                }
                            }
                        }
                    }
                } while (objReader.Peek() != -1);
            }
        }

        private void Statement_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control) && (e.KeyCode == Keys.A))
            {
                if (sender != null)
                {
                    var objTextBox = ((TextBox) sender);
                    objTextBox.SelectionStart = 0;
                    objTextBox.SelectionLength = objTextBox.TextLength;
                }
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
        }

        private void rbtnCompiledRegexCapture_CheckedChanged(object sender, EventArgs e)
        {
        }
    }

    public class BoschSPRepositoryFactory : IRepositoryFactory
    {
        public Stream CreateXmlMapReader()
        {
            return new FileStream(@"FieldVision_Bosch_SP_ontw.map", FileMode.Open);
        }

        public IRepositoryDataContext CreateAdapter(XmlMappingSource objSource)
        {
            return
                new FieldVision_Bosch_SP_ontw(ConfigurationManager.ConnectionStrings["Bosch_SP_ontw"].ConnectionString,
                    objSource);
        }

        public IEnumerable<ICommField> CreateCommFieldEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommField>();
            foreach (StrippedXMLToSQL.BoschSP.Fv_commfield fields in from c in repositoryDataContext.GetTable<BoschSP.Fv_commfield>()
                                                                       where c.TableID == intTableId
                                                                       select c)
            {
                ret.Add(fields);
            }
            return ret;
        }

        public IEnumerable<ICommTable> CreateCommTableEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommTable>();
            foreach (StrippedXMLToSQL.BoschSP.Fv_commtable table in from c in repositoryDataContext.GetTable<BoschSP.Fv_commtable>()
                                                                      where c.TableID == intTableId
                                                                      select c)
            {
                ret.Add(table);
            }
            return ret;
        }
    }

    public class AssaAbloyRepositoryFactory : IRepositoryFactory
    {
        public Stream CreateXmlMapReader()
        {
            return new FileStream(@"FieldVisionAssaAbloy.map", FileMode.Open);
        }

        public IRepositoryDataContext CreateAdapter(XmlMappingSource objSource)
        {
            return new DHL_ontw(ConfigurationManager.ConnectionStrings["AssaAbloy_Ontw"].ConnectionString, objSource);
        }

        public IEnumerable<ICommField> CreateCommFieldEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommField>();
            foreach (StrippedXMLToSQL.AssaAbloy.Fv_commfield fields in from c in repositoryDataContext.GetTable<AssaAbloy.Fv_commfield>()
                                                                 where c.TableID == intTableId
                                                                 select c)
            {
                ret.Add(fields);
            }
            return ret;
        }

        public IEnumerable<ICommTable> CreateCommTableEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommTable>();
            foreach (StrippedXMLToSQL.AssaAbloy.Fv_commtable table in from c in repositoryDataContext.GetTable<AssaAbloy.Fv_commtable>()
                                                                where c.TableID == intTableId
                                                                select c)
            {
                ret.Add(table);
            }
            return ret;
        }
    }

    public class DHLRepositoryFactory : IRepositoryFactory
    {
        public Stream CreateXmlMapReader()
        {
            return new FileStream(@"DHL.map",FileMode.Open);
        }

        public IRepositoryDataContext CreateAdapter(XmlMappingSource objSource)
        {
            return new DHL_ontw(ConfigurationManager.ConnectionStrings["DHL_Ontw"].ConnectionString, objSource);
        }

        public IEnumerable<ICommField> CreateCommFieldEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommField>();
            foreach (StrippedXMLToSQL.DHL.Fv_commfield fields in from c in repositoryDataContext.GetTable<DHL.Fv_commfield>()
                                                                  where c.TableID == intTableId
                                                                  select c)
            {
                ret.Add(fields);
            }
            return ret;
        }

        public IEnumerable<ICommTable> CreateCommTableEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommTable>();
            foreach (StrippedXMLToSQL.DHL.Fv_commtable table in from c in repositoryDataContext.GetTable<DHL.Fv_commtable>()
                                                                 where c.TableID == intTableId
                                                                 select c)
            {
                ret.Add(table);
            }
            return ret;
        }
    }

    public class NuonRepositoryFactory : IRepositoryFactory
    {
        public Stream CreateXmlMapReader()
        {
            return new FileStream(@"FieldvisionNuon.map", FileMode.Open);
        }

        public IRepositoryDataContext CreateAdapter(XmlMappingSource objSource)
        {
            return
                new Fieldvision_Nuon_ontw(
                    ConfigurationManager.ConnectionStrings["Fieldvision_Nuon_Ontw"].ConnectionString, objSource);
        }

        public IEnumerable<ICommField> CreateCommFieldEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommField>();
            foreach (StrippedXMLToSQL.Nuon.Fv_commfield fields in from c in repositoryDataContext.GetTable<Nuon.Fv_commfield>()
                                                                    where c.TableID == intTableId
                                                                    select c)
            {
                ret.Add(fields);
            }
            return ret;
        }

        public IEnumerable<ICommTable> CreateCommTableEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommTable>();
            foreach (StrippedXMLToSQL.Nuon.Fv_commtable table in from c in repositoryDataContext.GetTable<Nuon.Fv_commtable>()
                                                                   where c.TableID == intTableId
                                                                   select c)
            {
                ret.Add(table);
            }
            return ret;
        }
    }

    public class PlanonRepositoryFactory : IRepositoryFactory
    {
        public Stream CreateXmlMapReader()
        {
            return new FileStream(@"MobilityPlatformPlanon.map", FileMode.Open);
        }

        public IRepositoryDataContext CreateAdapter(XmlMappingSource objSource)
        {
            return
                new MobilityPlatform_Planon_Ontw(
                    ConfigurationManager.ConnectionStrings["MobilityPlatform_Planon_Ontw"].ConnectionString, objSource);
        }

        public IEnumerable<ICommField> CreateCommFieldEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommField>();
            foreach (StrippedXMLToSQL.Planon.Fv_commfield fields in from c in repositoryDataContext.GetTable<Planon.Fv_commfield>()
                                                                        where c.TableID == intTableId
                                                                        select c)
            {
                ret.Add(fields);
            }
            return ret;
        }

        public IEnumerable<ICommTable> CreateCommTableEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommTable>();
            foreach (StrippedXMLToSQL.Planon.Fv_commtable table in from c in repositoryDataContext.GetTable<Planon.Fv_commtable>()
                                                                       where c.TableID == intTableId
                                                                       select c)
            {
                ret.Add(table);
            }
            return ret;
        }
    }

    public class WorksphereRepositoryFactory : IRepositoryFactory
    {
        public Stream CreateXmlMapReader()
        {
            return new FileStream(@"Fieldvision_Worksphere.map", FileMode.Open);
        }

        public IRepositoryDataContext CreateAdapter(XmlMappingSource objSource)
        {
            return new FieldVision(ConfigurationManager.ConnectionStrings["Fieldvision_Worksphere"].ConnectionString, objSource);
        }

        public IEnumerable<ICommField> CreateCommFieldEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommField>();
            foreach (StrippedXMLToSQL.Worksphere.Fv_commfield fields in from c in repositoryDataContext.GetTable<Worksphere.Fv_commfield>()
                                                                 where c.TableID == intTableId
                                                                 select c)
            {
                ret.Add(fields);
            }
            return ret;
        }

        public IEnumerable<ICommTable> CreateCommTableEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommTable>();
            foreach (StrippedXMLToSQL.Worksphere.Fv_commtable table in from c in repositoryDataContext.GetTable<Worksphere.Fv_commtable>()
                                                                where c.TableID == intTableId
                                                                select c)
            {
                ret.Add(table);
            }
            return ret;
        }
    }

    public class FSSRepositoryFactory : IRepositoryFactory
    {
        public Stream CreateXmlMapReader()
        {
            return new FileStream(@"FSS.map", FileMode.Open);
        }

        public IRepositoryDataContext CreateAdapter(XmlMappingSource objSource)
        {
            return new MobilityPlatform_FSS_Dev_ONTW(
                ConfigurationManager.ConnectionStrings["FSS-dev_Ontw"].ConnectionString, objSource);
        }

        public IEnumerable<ICommField> CreateCommFieldEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommField>();
            foreach (StrippedXMLToSQL.FSS.Fv_commfield fields in from c in repositoryDataContext.GetTable<FSS.Fv_commfield>()
                                                                 where c.TableID == intTableId
                                                                    select c)
            {
                ret.Add(fields);
            }
            return ret;
        }

        public IEnumerable<ICommTable> CreateCommTableEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId)
        {
            var ret = new Collection<ICommTable>();
            foreach (StrippedXMLToSQL.FSS.Fv_commtable table in from c in repositoryDataContext.GetTable<FSS.Fv_commtable>()
                                                                where c.TableID == intTableId
                                                                   select c)
            {
                ret.Add(table);
            }
            return ret;
        }
    }
}