using MA.Dal;
using MA.Dao;
using MA.Dao.Entity;
using MA.Dao.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Database.Functions.Scalar
{
    [MADataName(Name = "function_database_name")]
    public class function_name : MADaoViewOrFuncEntity<function_name>
    {
        #region Variables
        [MADataColumn(Name = "")]
        public function_type data { get; set; }
        #endregion

        #region Methods
        public static function_name Get(MAEntity db/*function_parameters*/)
        {
            return Select(
                    db,
                    MAData.Sql.SelectData(
                        ViewOrFunctionName
                        )/*function_sql_parameters*/
                ).FirstOrDefault();
        }
        [MADataName(Name = "function_database_name__attr")]
        public static function_type Linq(/*function_parameters_linq*/)
        {
            return default(function_type);
        }
        #endregion
    }
    /*
     Examples : 
        Get Value : 
            A ->
                var result = function_name.Get(maEntity, parameter_values).data;
         */
}
