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
 * function: 表变量的分页算法
 * history:  created by 金洋 
 *           2011-01-28 简单整理
 *           2011-4-11 整理
 * **********************************************
 */

using System.Text;

namespace Nature.UI.WebControl.QuickPagerSQL
{
    /// <summary>
    /// 基于表变量的分页算法
    /// </summary>
    public class SQLTableVar : PagerSQLFunction
    {
        internal override void CreateNextPageSQL()
        {
            string tableName = MyPagerSql.TableName;
            string tableQuery = MyPagerSql.TableQuery;
            //int PageSize = myPagerSQL.PageSize;
            string tableShowColumns = MyPagerSql.TableShowColumns;
            string tableOrderByColumns = MyPagerSql.TableOrderByColumns;
            string tableIDColumn = MyPagerSql.TablePKColumn;


            //指定页号的SQL语句的模版
            //SQL 2000、SQL 2005 数据库，使用 表变量的方式分页
            //难道必须让表有唯一主键，且必须是int?
            
            //set nocount on

            //declare @tt table(id int identity(1,1),nid int)
            //insert into @tt(nid) 
            //select top 30 ProductID from Products  --where SupplierID =1
            //order by ProductName asc

            //select * from Products O,@tt t where O.ProductID=t.nid
            //and t.id between 20 +1 and 30 order by t.id
             
            //set nocount off

            
            var sql = new StringBuilder(800);
            //sql.Append("set nocount on  ");
            sql.Append(" declare @tt table(id int identity(1,1),nid int)");
            sql.Append(" insert into @tt(nid) select top {1} ");
            sql.Append(tableIDColumn);
            sql.Append(" from ");
            sql.Append(tableName);
            
            //查询条件
            if (tableQuery.Length > 0)
            {
                sql.Append(" where ");
                sql.Append(tableQuery);
            }

            sql.Append(" order by ");
            sql.Append(tableOrderByColumns);

            sql.Append(" select  ");
            sql.Append(tableShowColumns);
            sql.Append("  from ");
            sql.Append(tableName );
            sql.Append(" t1, @tt t2 where t1.");
            sql.Append(tableIDColumn );
            sql.Append(" =t2.nid and t2.id between {0} and {1}  order by t2.id");

            //sql.Append(" set nocount off ");

            //保存
            MyPagerSql.GetNextPagerSQL = sql.ToString();
            sql.Length = 0;
        }
    }
}
