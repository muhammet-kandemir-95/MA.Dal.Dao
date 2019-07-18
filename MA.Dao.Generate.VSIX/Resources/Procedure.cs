using MA.Dao;
using MA.Dao.Entity;
using MA.Dao.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Database.Procedures
{
    [MADataName(Name = "procedure_database_name")]
    public class procedure_name : MADaoProcedureEntity<procedure_name, TResult>
    {
        #region Variables
        /*procedure_properties*/
        #endregion
    }
    /*
     Examples : 
        Execute Procedure : 
            A ->
                var result = new procedure_name()
                        {
                            Property1 = Value1,
                            Property2 = Value2,
                            ...
                        }.Exec(maEntity);
            B ->
                maEntity.Exec(new procedure_name()
                {
                    Property1 = Value1,
                    Property2 = Value2,
                    ...
                });
         */
}
