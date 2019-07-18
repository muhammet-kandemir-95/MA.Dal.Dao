using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dal.DatabaseSQLGenerator
{
    public abstract class MADataSQLGenerator
    {
        #region Classes
        public class NameAndValue
        {
            #region Constructs
            public NameAndValue(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }
            #endregion

            #region Variables
            public string Name { get; set; }
            public string Value { get; set; }
            #endregion
        }
        public class NameAndOrder
        {
            #region Constructs
            public NameAndOrder(string name, bool asc)
            {
                this.Name = name;
                this.Asc = asc;
            }
            #endregion

            #region Variables
            public string Name { get; set; }
            public bool Asc { get; set; }
            #endregion
        }
        public enum JoinType
        {
            Left = 1,
            Right = 2,
            Inner = 3,
            Full = 4
        }
        #endregion
    }
}
