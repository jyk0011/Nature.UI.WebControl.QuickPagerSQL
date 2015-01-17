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
 * function: Row_number的分页算法
 * history:  created by 金洋 
 *           2011-01-28 简单整理
 *           2011-4-11 整理
 * **********************************************
 */

using System.Text;

namespace Nature.UI.WebControl.QuickPagerSQL
{
    /// <summary>
    /// Row_Number的分页算法
    /// </summary>
    public class SQLRowNumber : PagerSQLFunction
    {
        internal override void CreateNextPageSQL()
        {
            string tableName = MyPagerSql.TableName;
            string tableQuery = MyPagerSql.TableQuery;
            //int PageSize = myPagerSQL.PageSize;
            string tableShowColumns = MyPagerSql.TableShowColumns;
            string tableOrderByColumns = MyPagerSql.TableOrderByColumns;
            //string TableIDColumn = myPagerSQL.TableIDColumn;


            //指定页号的SQL语句的模版
            //SQL 2005 数据库，使用 Row_Number()分页
            //set nocount on;
            //with t_pager as (
            //   select *,rn = ROW_NUMBER() OVER (ORDER BY id desc) FROM test_indexorder
            // )
            //SELECT id,name,content,co1,co2,co3,co4,co5 from t_rn WHERE rn between 19007 and 19057;


            //另一种方法
            //select *
            //from (
            //    select row_number()over(order by __tc__)__rn__,*
            //    from (select top 开始位置+10 0 __tc__,* from Student where Age>18 order by Age)t
            //)tt
            //where __rn__>开始位置


            var sql = new StringBuilder(500);
            //sql.Append("set nocount on; ");
            sql.Append("with t_pager as (select myIndex = ROW_NUMBER() OVER (ORDER BY ");
            sql.Append(tableOrderByColumns);
            sql.Append(" ),* from ");
            sql.Append(tableName);

            //查询条件
            if (tableQuery.Length > 0)
            {
                sql.Append(" where ");
                sql.Append(tableQuery);
            }

            sql.Append(" ) select  ");
            sql.Append(tableShowColumns);
            sql.Append("  from t_pager where myIndex between {0} and {1} ");

            //sql.Append(" set nocount off; ");

            //保存
            MyPagerSql.GetNextPagerSQL = sql.ToString();
            sql.Length = 0;
        }
    }
}
