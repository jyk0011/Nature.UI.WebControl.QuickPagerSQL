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
 * function: 颠倒Top
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
    /// 颠倒Top（Top嵌套）的分页算法
    /// </summary>
    public class SQLTopTop : PagerSQLFunction
    {
        string _tableNames = "";        //表名
        string _query = "";            //条件
        string _idColumn = "";         //主键

        string _orderCol = "";   //排序字段
        string _orderColA = "";  //排序字段A
        string _orderColB = "";  //排序字段B

        string[] _arrOrderCol;


        #region 处理排序字段
        /// <summary>
        /// 处理排序字段
        /// </summary>
        private void TopTopInit(string t1,string t2)
        {
            if (_tableNames.Length == 0)
            {
                _tableNames = MyPagerSql.TableName;        //表名
                _query = MyPagerSql.TableQuery;            //条件

                //主键
                _idColumn = MyPagerSql.TablePKColumn.ToLower().Trim();
            
                //转换成小写形式，判断是正序还是倒序，然后按照,号拆分
                _arrOrderCol = MyPagerSql.TableOrderByColumns.ToLower().Split(',');
            }

            _orderCol = "";   //排序字段
            _orderColA = "";  //排序字段A
            _orderColB = "";  //排序字段B

            foreach (string a in _arrOrderCol)
            {
                string tmpCol ;
                if (a.Contains("desc"))
                {
                    //倒序
                    tmpCol = a.Replace("desc", "").Trim();
                    _orderColA += t1 + tmpCol + " desc,";
                    _orderColB += t2 + tmpCol + ",";
                    _orderCol += tmpCol + ",";
                }
                else
                {
                    //正序
                    tmpCol = a.Replace("asc", "").Trim();
                    _orderColA += t1 + tmpCol + ",";
                    _orderColB += t2 + tmpCol + " desc,";
                    _orderCol += tmpCol + ",";
                }
            }

            _orderColA = _orderColA.TrimEnd(',');
            _orderColB = _orderColB.TrimEnd(',');
            _orderCol = _orderCol.TrimEnd(',');

            if (!_orderCol.Contains(_idColumn))    //排序字段没有主键，添加主键
                _orderCol += "," + _idColumn;

        }
        #endregion

        #region 生成下一页的SQL语句
        /// <summary>
        /// 生成下一页的SQL语句
        /// </summary>
        internal override void CreateNextPageSQL()
        {
            int pageSize = MyPagerSql.PageSize;
            string tableShowColumns = MyPagerSql.TableShowColumns;

            //指定页号的SQL语句的模版

            //处理排序字段
            TopTopInit("", "t.");

            //select * from table where customerID  in (
            //select top 10 customerID  from 
            //    (
            //         select top 300 customerID ,departMentID from table
            //            order by customerID, departMentID
            //    ) as t order by customerID desc ,departMentID desc 
            //)order by customerID ,departMentID

            var sql = new StringBuilder(600);
            //select * from news where newsID  in (
            //sql.Append("set nocount on  ");
            sql.Append(" select " );
            sql.Append(tableShowColumns);
            sql.Append(" from "); //加显示字段
            sql.Append(_tableNames);        //加表名
            sql.Append(" where ");
            sql.Append(_idColumn);     //加表的主键  uio
            sql.Append(" in ( ");

            // select top 10 newsID  from (
            sql.Append(" select top ");
            sql.Append(pageSize);           //加页大小
            sql.Append(" ");      //
            sql.Append(_idColumn);            //加表的主键
            sql.Append("  from (");

            // select top 20 newsID ,AddedDate from news
            sql.Append(" select top {0} ");
            sql.Append(_orderCol);           //加主键和排序字段
            sql.Append(" from ");
            sql.Append(_tableNames);      //加表名

            //判断查询条件  where Query
            if (_query.Length > 1)
            {    //有查询条件
                sql.Append(" where ");
                sql.Append(_query);
            }

            // order by AddedDate desc,newsID desc
            sql.Append(" order by ");     //PageSize * PageIndex 
            sql.Append(_orderColA);        //加排序字段 正

            //) as aa order by AddedDate ,newsID 
            sql.Append(" ) as t order by ");
            sql.Append(_orderColB);        //加排序字段 反

            //)order by AddedDate desc,newsID desc
            sql.Append(" ) ");

            //判断查询条件  where Query
            if (_query.Length > 1)
            {    //有查询条件
                sql.Append(" and ");
                sql.Append(_query);
            }

            sql.Append(" order by ");
            sql.Append(_orderColA);       //加排序字段 正
            //sql.Append(" set nocount off ");

            //保存
            MyPagerSql.GetNextPagerSQL = sql.ToString();
            sql.Length = 0;
        }
        #endregion

        #region TopTop颠倒法的最后一页的SQL语句
        /// <summary>
        /// TopTop颠倒法的最后一页的SQL语句，一是提高效率，一是修改了最后一页总是显示PageSize条记录的bug
        /// </summary>
        /// <returns></returns>
        internal override void CreateLastPageSQL()
        {
            //int PageSize = myPagerSQL.PageSize;
            string tableShowColumns = MyPagerSql.TableShowColumns;


            //处理排序字段
            TopTopInit("t.","");

            //select * from (
            //select top 5 * from TableName order by FunctionID desc ) as t 
            //order by t.FunctionID

            var sql = new StringBuilder(600);
            //select * from ( select top 5 * from TableName
            sql.Append(" select ");
            sql.Append(tableShowColumns);   //加显示字段
            sql.Append(" from ( select top {0} * from "); 
            sql.Append(_tableNames);        //加表名
             
            //判断查询条件  where Query
            if (_query.Length > 1)
            {    //有查询条件
                sql.Append(" where ");
                sql.Append(_query);
            }

            // order by AddedDate desc,newsID desc
            sql.Append(" order by ");     //
            sql.Append(_orderColB);        //加排序字段 正

            //) as aa order by AddedDate ,newsID 
            sql.Append(" ) as t order by ");
            sql.Append(_orderColA);        //加排序字段 反

            //保存
            MyPagerSql.GetLastPagerSQL = sql.ToString();
            sql.Length = 0;
        }
        #endregion

        #region 重写父类的获取分页用的SQL语句的函数，判断是否是最后一页
        /// <summary>
        /// 重写父类的获取分页用的SQL语句的函数，判断是否是最后一页
        /// </summary>
        /// <param name="pageIndex">页号，从1开始计数</param>
        /// <returns></returns>
        internal override string GetSQLByPageIndex(int pageIndex)
        {
            if (pageIndex < 1)
                pageIndex = 1;

            if (pageIndex > MyPagerSql.PageCount)
                pageIndex = MyPagerSql.PageCount;

            //指定页号
           
            if (pageIndex == 1)
            {
                return MyPagerSql.GetFirstPagerSQL;
            }
            
            if (pageIndex == MyPagerSql.PageCount)
            {
                //最后一页
                Int32 p1 = MyPagerSql.RecordCount % MyPagerSql.PageSize;
                if (p1 == 0)
                    p1 = MyPagerSql.PageSize;

                return string.Format(MyPagerSql.GetLastPagerSQL, p1);
            }
            else
            {
                Int32 p1 = MyPagerSql.PageSize * pageIndex;
                return string.Format(MyPagerSql.GetNextPagerSQL, p1);
            }
        }
        #endregion
    }
}
