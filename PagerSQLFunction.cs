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
 * function: 分页算法的基类
 * history:  created by 金洋 
 *           2011-01-28 简单整理
 *           2011-4-11 整理
 * **********************************************
 */

using System;
using System.Text;

//using System.Collections.Generic;

namespace Nature.UI.WebControl.QuickPagerSQL
{
    /// <summary>
    /// 生成分页的方式提取数据的SQL语句，可以选择不同的分页算法
    /// 这个是基类，各种分页算法比较公用的部分
    /// </summary>
    public class PagerSQLFunction
    {
        /// <summary>
        /// 通过它来调用属性
        /// </summary>
        internal PagerSQL MyPagerSql;

        #region 生成分页用的SQL语句的模版
        #region 生成统计记录数的SQL语句
        /// <summary>
        /// 生成统计记录数的SQL语句
        /// </summary>
        internal virtual void CreateRecordCountSQL()
        {
            string tableName = MyPagerSql.TableName ;
            string tableQuery = MyPagerSql.TableQuery;

            var sql = new StringBuilder();
            sql.Append("select count(1) from ");
            sql.Append(tableName);
            if (tableQuery.Length > 0)
            {
                sql.Append(" where ");
                sql.Append(tableQuery);
            }

            MyPagerSql.GetRecordCountSQL = sql.ToString();
            sql.Length = 0;

        }
        #endregion

        #region 生成首页的SQL语句
        /// <summary>
        /// 生成首页的SQL语句
        /// </summary>
        internal virtual void CreateFirstPageSQL()
        {
            string tableName = MyPagerSql.TableName;
            string tableQuery = MyPagerSql.TableQuery;
            int pageSize = MyPagerSql.PageSize;
            string tableShowColumns = MyPagerSql.TableShowColumns;
            string tableOrderByColumns = MyPagerSql.TableOrderByColumns;


            //第一页的SQL语句，
            //select top PageSize * from table where  order by 
            var sql = new StringBuilder(100);
            //sql.Append("set nocount on; ");
            sql.Append("select top ");
            sql.Append(pageSize );
            sql.Append(" ");
            sql.Append(tableShowColumns);
            sql.Append(" from ");
            sql.Append(tableName );
            sql.Append(" ");

            //查询条件
            if (tableQuery.Length > 0)
            {
                sql.Append(" where ");
                sql.Append(tableQuery);
            }

            sql.Append(" order by ");
            sql.Append(tableOrderByColumns );
            //sql.Append(" set nocount off; ");
            
            //保存
            MyPagerSql.GetFirstPagerSQL = sql.ToString();
            sql.Length = 0;

        }
        #endregion

        #region 生成任意页的SQL语句
        /// <summary>
        /// 生成任意页的SQL语句的模版，分页算法不同，区别也很大，所以这里就不实现了。
        /// </summary>
        internal virtual void CreateNextPageSQL()
        {
           
        }
        #endregion

        #region 生成最后一页的SQL语句
        /// <summary>
        /// 最后一页的SQL语句。依据算法而定，不是所有的情况都需要实现
        /// </summary>
        internal virtual void CreateLastPageSQL()
        {
         
        }
        #endregion

        #endregion

        #region 获取分页用的SQL语句
        /// <summary>
        /// 传入页号，返回指定页号的SQL语句
        /// </summary>
        /// <param name="pageIndex">页号</param>
        internal virtual string GetSQLByPageIndex(Int32 pageIndex)
        {

            if (pageIndex == 1)
            {
                //显示第一页，返回第一页的SQL语句
                return MyPagerSql.GetFirstPagerSQL;
            }

            //判断页号是否在有效范围内，如果小于第一页，显示第一页。
            if (pageIndex < 1)
                pageIndex = 1;

            //如果大于最后一页，显示最后一页
            if (pageIndex > MyPagerSql.PageCount)
                pageIndex = MyPagerSql.PageCount;

            //指定页号

            Int32 p1 = MyPagerSql.PageSize*(pageIndex - 1) + 1;
            Int32 p2 = p1 + MyPagerSql.PageSize - 1;

            return string.Format(MyPagerSql.GetNextPagerSQL, p1, p2);

        }

        #endregion


    }
}
