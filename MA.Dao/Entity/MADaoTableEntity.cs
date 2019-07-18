using MA.Dal;
using MA.Dal.BaseTypes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity
{
    public class MADaoTableEntity<T> : MADaoTable<T> where T : MADaoTableEntity<T>
    {
        #region Methods
        #region Protected Convert To Public With Entity
        public int Insert(MAEntity db)
        {
            return db.Run(() =>
            {
                return base.InsertProtected(db.Connection, db.Transaction);
            });
        }
        public int Update(MAEntity db)
        {
            return db.Run(() =>
            {
                return base.UpdateProtected(db.Connection, db.Transaction);
            });
        }
        public int Delete(MAEntity db)
        {
            return db.Run(() =>
            {
                return base.DeleteProtected(db.Connection, db.Transaction);
            });
        }
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
