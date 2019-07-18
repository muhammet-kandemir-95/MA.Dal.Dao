using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MA.Dao.Generate.VSIX;

namespace MA.Dao.Generate.VSIX.Models
{
    public class GenerateSQLItem
    {
        #region Constructs
        public GenerateSQLItem(string safeName, string code)
        {
            this.SafeName = safeName;
            this.Code = code;
        }
        #endregion

        #region Variables
        public string SafeName { get; set; }
        public string Code { get; set; }
        #endregion

        #region Methods
        public string CreateRow()
        {
            return SafeName + GenerateSQL.SPLIT_NAME + Code;
        }
        #endregion
    }
    public class GenerateSQL
    {
        #region Constructs
        public GenerateSQL()
        {
            this.Tables = new List<GenerateSQLItem>();
            this.Procedures = new List<GenerateSQLItem>();
            this.FunctionsScalar = new List<GenerateSQLItem>();
            this.FunctionsTable = new List<GenerateSQLItem>();
            this.Views = new List<GenerateSQLItem>();
        }
        #endregion

        #region Variables
        #region Consts
        public const string TABLES = "/*MA_GENERATE_TABLES*/";
        public const string PROCEDURES = "/*MA_GENERATE_PROCEDURES*/";
        public const string FUNCTIONS_SCALAR = "/*MA_GENERATE_FUNCTIONS_SCALAR*/";
        public const string FUNCTIONS_TABLE = "/*MA_GENERATE_FUNCTIONS_TABLE*/";
        public const string VIEWS = "/*MA_GENERATE_FUNCTIONS_VIEWS*/";
        public const string SPLIT_ROW = "/*MA_GENERATE_SPLIT_ROW*/";
        public const string SPLIT_NAME = "/*MA_GENERATE_SPLIT_NAME*/";
        #endregion
        
        public List<GenerateSQLItem> Tables { get; set; }
        public List<GenerateSQLItem> Procedures { get; set; }
        public List<GenerateSQLItem> FunctionsScalar { get; set; }
        public List<GenerateSQLItem> FunctionsTable { get; set; }
        public List<GenerateSQLItem> Views { get; set; }
        #endregion

        #region Methods
        public static GenerateSQL Load(string path)
        {
            var text = File.ReadAllText(path);
            return new GenerateSQL()
            {
                Tables = GetRows(text, TABLES),
                Procedures = GetRows(text, PROCEDURES),
                FunctionsScalar = GetRows(text, FUNCTIONS_SCALAR),
                FunctionsTable = GetRows(text, FUNCTIONS_TABLE),
                Views = GetRows(text, VIEWS)
            };
        }
        public void Save(string path)
        {
            string text = 
                TABLES + string.Join(SPLIT_ROW, Tables.Select(o => o.CreateRow()).ToArray()) + TABLES +
                FUNCTIONS_SCALAR + string.Join(SPLIT_ROW, FunctionsScalar.Select(o => o.CreateRow()).ToArray()) + FUNCTIONS_SCALAR +
                FUNCTIONS_TABLE + string.Join(SPLIT_ROW, FunctionsTable.Select(o => o.CreateRow()).ToArray()) + FUNCTIONS_TABLE +
                VIEWS + string.Join(SPLIT_ROW, Views.Select(o => o.CreateRow()).ToArray()) + VIEWS +
                PROCEDURES + string.Join(SPLIT_ROW, Procedures.Select(o => o.CreateRow()).ToArray()) + PROCEDURES;

            File.WriteAllText(path, text);
        }

        public static List<GenerateSQLItem> GetRows(string text, string splitGroup)
        {
            var split = text
                .Split(new string[] { splitGroup }, StringSplitOptions.None)[1]
                .Split(new string[] { SPLIT_ROW }, StringSplitOptions.RemoveEmptyEntries);
            return split.Select(row =>
            {
                var nameAndCode = row.Split(new string[] { SPLIT_NAME }, StringSplitOptions.None);
                return new GenerateSQLItem(nameAndCode[0], nameAndCode.Length < 2 ? "" : nameAndCode[1]);
            }).ToList();
        }
        #endregion
    }
}
