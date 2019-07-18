using MA.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity
{
    public class MADaoViewOrFuncEntity<T> : MADaoViewOrFunc<T> where T : MADaoViewOrFuncEntity<T>
    {
        #region Methods
        #region Protected Convert To Public With Entity
        public static List<T> Select(
            MAEntity db,
            string command,
            params MAEntityParameter[] parameters
            )
        {
            return db.Run(() =>
            {
                return SelectProtected(
                    db.Connection, 
                    db.Transaction,
                    command,
                    parameters.ToMAData()
                    );
            });
        }
        #endregion
        #endregion
    }
}
