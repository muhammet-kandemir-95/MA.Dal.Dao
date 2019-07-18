using MA.Dao;
using MA.Dao.Entity;
using MA.Dao.Entity.Linq;
using MA.Dao.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Database.Functions.Table
{
    [MADataName(Name = "function_database_name")]
    public class function_name : MADaoViewOrFuncEntity<function_name>
    {
        #region Variables
        /*function_properties*/
        #endregion

        #region Methods
        public static MAEntityLinqQueryViewOrFunc<function_name> Query(/*function_parameters_sub_string*/)
        {
            MAEntityLinqQueryViewOrFunc<function_name> query = new MAEntityLinqQueryViewOrFunc<function_name>();
            query.ParametersDefault = new MAEntityParameter[]
                {
                    /*function_sql_parameters_sub_string*/
                };
            return query;
        }
        #endregion
    }
}
