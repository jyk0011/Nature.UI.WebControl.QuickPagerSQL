/**
 * 自然框架之QuickPager分页控件
 * http://www.natureFW.com/
 *
 * @author
 * 金洋（金色海洋jyk）
 * 
 * @copyright
 * Copyright (C) 2005-2013 金洋.
 *
 * Licensed under a GNU Lesser General Public License.
 * http://creativecommons.org/licenses/LGPL/2.1/
 *
 * 自然框架之QuickPager分页控件 is free software. You are allowed to download, modify and distribute 
 * the source code in accordance with LGPL 2.1 license, however if you want to use 
 * 自然框架之QuickPager分页控件 on your site or include it in your commercial software, you must  be registered.
 * http://www.natureFW.com/registered
 */

/* ***********************************************
 * author :  金洋（金色海洋jyk）
 * email  :  jyk0011@live.cn  
 * function: Max分页算法
 * history:  created by 金洋 
 *           2011-01-28 简单整理
 *           2011-4-11 整理
 * **********************************************
 */

using System;
using System.Text;

namespace Nature.UI.WebControl.QuickPagerSQL
{
    /// <summary>
    /// Max分页算法
    /// </summary>
    public class SQLMinMax : PagerSQLFunction
    {
        internal override void CreateNextPageSQL()
        {
            string tableName = MyPagerSql.TableName;
            string tableQuery = MyPagerSql.TableQuery;
            int pageSize = MyPagerSql.PageSize;
            string tableShowColumns = MyPagerSql.TableShowColumns;
            string tableOrderByColumns = MyPagerSql.TableOrderByColumns;
            //string TableIDColumn = myPagerSQL.TablePKColumn;


            //指定页号的SQL语句的模版
            //Access、Excel、SQL 2000、SQL 2005 数据库，使用 Max的方式分页
            //难道必须让表有唯一主键，且必须是int?

            /*
            
            select top 10 * from table
            where customerID >= 
	            (SELECT max(customerID) FROM 
		            (select top 21 customerID from table order by customerID   ) as t
	            )

            ---------------------

            select top 10 * from table where customerID  >=(
	            select top 1 customerID from (
		            SELECT  top 21 customerID
			            FROM table  order by customerID ) as aa   order by customerID desc
		            )


            ------------
            declare @id int

            SELECT top 21 @id = customerID
            FROM table 

            select top 10 * from table where customerID  >=@id
            select @id

             */

            bool isDesc = true;     //倒序

            //select top 10 * from table
            //where customerID >= 
            //    (SELECT max(customerID) FROM 
            //        (select top 21 customerID from table order by customerID   ) as t
            //    )
            string orderCol = tableOrderByColumns.ToLower();
            if (orderCol.Contains("desc") )
            {
                //倒序
                orderCol = orderCol.Replace("desc", "");
            }
            else
            { 
                //正序
                orderCol = orderCol.Replace("asc", "");
                isDesc = false ;
            }

            var sql = new StringBuilder(600);
            //sql.Append("set nocount on  ");
            sql.Append(" select top ");//PageSize
            sql.Append(pageSize);    //select top 10 * from TableName
            sql.Append(" ");
            sql.Append(tableShowColumns);
            sql.Append(" from ");
            sql.Append(tableName);
           
            sql.Append(" where ");      //where OrderCol <= | >= 
            sql.Append(orderCol);

            //倒序//正序
            sql.Append(isDesc ? "<=" : ">=");
            sql.Append(" (SELECT ");   //(select max() | min() from (select top {1} OrderCol from TableName 
            sql.Append(isDesc ? "min(" : "max(");

            sql.Append(orderCol);
            sql.Append(" ) from (select top {0} ");     //pagesize * (pageindex-1)+1
            sql.Append(orderCol);
            sql.Append(" from ");
            sql.Append(tableName);

            //内部查询条件
            if (tableQuery.Length > 0)   // where ...
            {
                sql.Append(" where ");
                sql.Append(tableQuery);
            }

            //内部排序  
            sql.Append(" order by ");       //order by OrderCol desc | asc
            sql.Append(orderCol);

            //内部排序的正序或者倒序
            if (isDesc)
                //倒序
                sql.Append(" desc");


            sql.Append(" ) as t ) ");         //结束内部查询

            //外部查询条件
            if (tableQuery.Length > 0)   // and (...)
            {
                sql.Append(" and ( ");
                sql.Append(tableQuery);
                sql.Append(")");
            }

            //外部排序
            sql.Append(" order by ");      //order by OrderCol desc | asc 
            sql.Append(orderCol);

            //外部排序的正序或者倒序
            if (isDesc)
                //倒序
                sql.Append(" desc ");
            
            //sql.Append(" set nocount off ");

            //保存
            MyPagerSql.GetNextPagerSQL = sql.ToString();
            sql.Length = 0;
        }

        internal override string GetSQLByPageIndex(int pageIndex)
        {
            if (pageIndex == 1)
            {
                //指定页号
                return MyPagerSql.GetFirstPagerSQL;
            }

            if (pageIndex < 1)
                pageIndex = 1;

            if (pageIndex > MyPagerSql.PageCount)
                pageIndex = MyPagerSql.PageCount;

            //指定页号

            Int32 p1 = MyPagerSql.PageSize*(pageIndex - 1) + 1;

            return string.Format(MyPagerSql.GetNextPagerSQL, p1);

        }
    }
}
