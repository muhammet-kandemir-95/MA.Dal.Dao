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

namespace MA.Database.Tables
{
    [MADataName(Name = "table_database_name")]
    public class table_name : MADaoTableEntity<table_name>
    {
        #region Variables
        /*table_properties*/
        #endregion

        #region Methods
        public static MAEntityLinqQueryTable<table_name> Query()
        {
            MAEntityLinqQueryTable<table_name> query = new MAEntityLinqQueryTable<table_name>();
            return query;
        }
        #endregion
    }
    /*
     Examples : 
        Insert Row :
            A ->
                new table_name()
                {
                    Property1 = Value1,
                    Property2 = Value2,
                    ...
                }.Insert(maEntity);
            B ->
                maEntity.Insert(new table_name()
                {

                });
        Update Row :
            A ->
                new table_name()
                {
                    Property1 = Value1,
                    Property2 = Value2,
                    ...
                }.Update(maEntity);
            B ->
                maEntity.Update(new table_name()
                {
                    Property1 = Value1,
                    Property2 = Value2,
                    ...
                });
        Delete Row :
            A ->
                new table_name()
                {
                    KeyProperty1 = KeyValue1,
                    KeyProperty2 = KeyValue2,
                    ...
                }.Delete(maEntity);
            B ->
                maEntity.Delete(new table_name()
                {
                    Property1 = Value1,
                    Property2 = Value2,
                    ...
                });
            C ->
                maEntity.Delete(KeyValue1);
            D ->
                maEntity.Delete(new object[]
                {
                    KeyValue1,
                    KeyValue2,
                    ...
                });
        Select Query :
            A ->
                table_name.Select(
                    maEntity, 
                    "Command Text", 
                    new MAEntityParameter("parameter_name_1", parameter_value_1), 
                    new MAEntityParameter("parameter_name_2", parameter_value_2),
                    ...
                    );
            B ->
                maEntity.Select<table_name>(
                    "Command Text", 
                    new MAEntityParameter("parameter_name_1", parameter_value_1), 
                    new MAEntityParameter("parameter_name_2", parameter_value_2),
                    ...
                    );
         */
}
