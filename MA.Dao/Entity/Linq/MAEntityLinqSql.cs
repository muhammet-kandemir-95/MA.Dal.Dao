using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity.Linq
{
    public class MAEntityLinqSql
    {
        #region Constructs
        public MAEntityLinqSql(string query, MAEntityParameter[] parameters)
        {
            this.Query = query;
            this.Parameters = parameters;
        }
        #endregion

        #region Variables
        public string Query { get; private set; }
        public MAEntityParameter[] Parameters { get;private set; }
        #endregion
    }
}
