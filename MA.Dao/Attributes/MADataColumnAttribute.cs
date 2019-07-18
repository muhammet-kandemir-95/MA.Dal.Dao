using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Attributes
{
    public class MADataColumnAttribute : Attribute
    {
        #region Constructs

        #endregion

        #region Variables
        string name = null;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        #endregion
    }
}
