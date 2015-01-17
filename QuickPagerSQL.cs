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
 * function: 分页算法的容器
 * history:  created by 金洋 
 *           2011-01-28 简单整理
 *           2011-4-11 整理
 * **********************************************
 */

using System;
using System.Configuration;
using System.Globalization;
using Nature.Common;

namespace Nature.UI.WebControl.QuickPagerSQL
{
    /// <summary>
    /// QuickPagerSQL 根据表名、字段名、排序字段等信息，生成各种分页算法。
    /// 记录属性，表名、字段名、排序字段、分页模板等。
    /// </summary>
    public class PagerSQL
    {
        #region 成员
        /// <summary>
        /// 内部的保存属性的容器
        /// </summary>
        private readonly MyViewState _myViewState = new MyViewState();

        /// <summary>
        /// 加密用的密钥,空字符串的话,表示不加密
        /// </summary>
        internal static readonly string SaveKeyWebconfig = ConfigurationManager.AppSettings["PagerSQLKey"];

        /// <summary>
        /// 单独的密钥
        /// </summary>
        internal string _saveKey;

        /// <summary>
        /// 生成分页用的SQL语句的函数库
        /// </summary>
        private PagerSQLFunction _sqlFunction;
        #endregion

        #region 属性
        #region Page
        /// <summary>
        /// 没有办法的办法，笨着，操作隐藏域和加载事件用
        /// </summary>
        public System.Web.UI.Page Page
        {
            set
            {
                _myViewState.Page = value;
                _myViewState.ClientID = "PagerSQL_2.0_" + value.ClientID;
            }
            get { return _myViewState.Page; }
        }
        #endregion

        #region 读取或者设置保存属性的位置，可以选择不保存。
        /// <summary>
        /// 读取或者设置保存属性的位置，可以选择不保存。
        /// </summary>
        public SaveViewStateLocation SaveLocation
        {
            get { return _myViewState.SaveLocation; }
            set { _myViewState.SaveLocation = value; }
        }
        #endregion

        #region 读取或者设置密钥，空字符串表示不加密，null表示采用默认密钥。
        /// <summary>
        /// 读取或者设置密钥，空字符串表示不加密，null表示采用默认密钥。
        /// </summary>
        public string SaveKey
        {
            get
            {
                if (_saveKey == null)
                    return SaveKeyWebconfig;
                return _saveKey;
            }

            set { _saveKey = value; }
        }
        #endregion

        #region 选择分页算法
        /// <summary>
        /// 设置分页算法，必须先设置排序字段。
        /// </summary>
        public PagerSQLKind SetPagerSQLKind
        {
            get
            {
                if (_myViewState["SetPagerSQLKind"] == null)
                    return PagerSQLKind.TopTop;

                return (PagerSQLKind) (Int32.Parse(_myViewState["SetPagerSQLKind"]));
            }
            set
            {
                _myViewState["SetPagerSQLKind"] = ((Int32)value).ToString(CultureInfo.InvariantCulture);
                SetSQLFunction(value);
            }

        }

        #region 获取分页算法的实例
        /// <summary>
        /// 获取分页算法的实例
        /// </summary>
        /// <param name="sqlKind"></param>
        private void SetSQLFunction(PagerSQLKind sqlKind)
        {
            switch (sqlKind)
            {
                case PagerSQLKind.RowNumber:
                    _sqlFunction = new SQLRowNumber();
                    break;

                case PagerSQLKind.TableVar:
                    _sqlFunction = new SQLTableVar();
                    break;

                case PagerSQLKind.MaxMin:
                    _sqlFunction = new SQLMinMax();
                    break;

                case PagerSQLKind.TopTop:
                    _sqlFunction = new SQLTopTop();
                    break;

                case PagerSQLKind.MaxTopTop:
                    if (TableOrderByColumns.Contains(","))
                        //多字段排序
                        _sqlFunction = new SQLTopTop();
                    else
                        //一个排序字段
                        _sqlFunction = new SQLMinMax();
                    break;

                case PagerSQLKind.MaxTableVar:
                    if (TableOrderByColumns.Contains(","))
                        //多字段排序
                        _sqlFunction = new SQLTableVar();
                    else
                        //一个排序字段
                        _sqlFunction = new SQLMinMax();
                    break;

                default:
                    if (TableOrderByColumns.Contains(","))
                        //多字段排序
                        _sqlFunction = new SQLTopTop();
                    else
                        //一个排序字段
                        _sqlFunction = new SQLMinMax();
                    break;
            }
            _sqlFunction.MyPagerSql = this;
        }

        #endregion
        #endregion

        //SQL语句的基础
        #region 分页算法需要的信息

        #region 模块ID _PagerInfo
        //private PagerInfo _PagerInfo;
        ///// <summary>
        ///// 模块ID
        ///// </summary>
        //public PagerInfo PagerInfo
        //{
        //    set { _PagerInfo = value; }
        //    get { return _PagerInfo; }
        //}
        #endregion

        #region 表名、视图名
        /// <summary>
        /// 表名、视图名
        /// </summary>
        public string TableName
        {
            set { _myViewState["TableName"] = value; }
            get
            {
                if (_myViewState["TableName"] == null)
                {
                    Functions.MsgBox("没有设置TableName属性，即提取数据用的表名或者视图名！", true);
                    return null;
                }
                return _myViewState["TableName"];
            }
        }
        #endregion

        #region 表的主键
        /// <summary>
        /// 表的主键字段名称
        /// </summary>
        public string TablePKColumn
        {
            set { _myViewState["TablePKColumn"] = value; }
            get
            {
                if (_myViewState["TablePKColumn"] == null)
                {
                    Functions.MsgBox("没有设置TablePKColumn属性，即主键字段名称！", true);
                    return null;
                }
                return _myViewState["TablePKColumn"];
            }
        }
        #endregion

        #region 显示的字段
        /// <summary>
        /// 显示的字段
        /// </summary>
        public string TableShowColumns
        {
            set { _myViewState["TableShowColumns"] = value; }
            get { return _myViewState["TableShowColumns"] ?? "*"; }
            //if (MyViewState["TableShowColumns"] == null)  return "*"; 
            //return MyViewState["TableShowColumns"];
        }
        #endregion

        #region 排序字段
        /// <summary>
        /// 排序字段，可以设置多个排序字段，可以设置升降序，比如col1 desc,col2,col3 desc 
        /// </summary>
        public string TableOrderByColumns
        {
            set { _myViewState["TableOrderByColumns"] = value; }
            get
            {
                if (_myViewState["TableOrderByColumns"] == null)
                {
                    Functions.MsgBox("没有设置TableOrderByColumns属性，即排序字段！", true);
                    return null;
                }
                return _myViewState["TableOrderByColumns"];
            }
        }

        #endregion

        #region 查询条件
        /// <summary>
        /// 查询条件
        /// </summary>
        public string TableQuery             
        {
            set
            {
                //不使用 _PagerInfo 里的 Query属性
                _myViewState["TableQuery"] = value;
            }
            get
            {
                //固定查询条件
                string tmpTableQueryAlways = TableQueryAlways;

                if (_myViewState["TableQuery"] == null || _myViewState["TableQuery"].Length == 0) //没有设置临时查询条件，返回固定查询条件
                    return tmpTableQueryAlways;

                //判断是否设置固定查询条件
                if (tmpTableQueryAlways.Length == 0)
                    //没有固定查询条件
                    return _myViewState["TableQuery"];

                //有固定查询条件
                return tmpTableQueryAlways + " and " + _myViewState["TableQuery"];
            }
        }

        #endregion

        #region 固定的查询条件
        /// <summary>
        /// 固定的查询条件，设置了就一直有效
        /// </summary>
        public string TableQueryAlways            //always
        {
            set { _myViewState["TableQueryAlways"] = value; }
            get { return _myViewState["TableQueryAlways"] ?? ""; }
        }

        #endregion

        #endregion

        //SQL模版
        #region 分页算法模版
        /// <summary>
        /// 生成统计记录数的SQL语句
        /// </summary>
        public string GetRecordCountSQL
        {
            set { _myViewState["RecordCountSQL"] = value; }
            get { return _myViewState["RecordCountSQL"]; }
        }
        #endregion

        #region 分页算法模版
        /// <summary>
        /// 第一页的分页算法模版
        /// </summary>
        internal string GetFirstPagerSQL
        {
            set { _myViewState["FirstPageSQL"] = value; }
            get { return _myViewState["FirstPageSQL"]; }
        }
        #endregion

        #region 指定页号的分页算法模版
        /// <summary>
        /// 指定页号的分页算法模版
        /// </summary>
        internal string GetNextPagerSQL
        {
            set{ _myViewState["NextPageSQL"] = value; }
            get { return _myViewState["NextPageSQL"]; }
        }
        #endregion

        #region 最后一页的分页算法模版
        /// <summary>
        /// 最后一页的分页算法模版
        /// </summary>
        internal string GetLastPagerSQL
        {
            set { _myViewState["LastPageSQL"] = value;  }
            get { return _myViewState["LastPageSQL"]; }
        }
        #endregion

        //GetData
       
        #region 总记录数
        /// <summary>
        /// 总记录记录数
        /// </summary>
        public Int32 RecordCount
        {
            set { _myViewState["RecordCount"] = value.ToString(CultureInfo.InvariantCulture); }
            //没有设置的话，返回 0 
            get { return _myViewState["RecordCount"] == null ? 0 : Int32.Parse(_myViewState["RecordCount"]); }
        }
        #endregion

        #region 一页的记录数
        /// <summary>
        /// 一页的记录数
        /// </summary>
        public Int32 PageSize
        {
            set
            {
                if (value <= 0)
                    _myViewState["PageSize"] = "20";
                else
                    _myViewState["PageSize"] = value.ToString(CultureInfo.InvariantCulture);
            }
            get
            {
                //没有设置的话，使用默认值：一页20条记录
                if (_myViewState["PageSize"] == null) { return 20; }
                 return Int32.Parse(_myViewState["PageSize"]); 
            }
        }
        #endregion

        #region 一共有多少页
        /// <summary>
        /// 一共有多少页，根据总记录数和一页的记录数自动计算
        /// </summary>
        public Int32 PageCount
        {
            set { _myViewState["PagerCount"] = value.ToString(CultureInfo.InvariantCulture); }
            get {
                //没有设置的话，返回 0 
                if (_myViewState["PagerCount"] == null) { return 0; }
                return Int32.Parse(_myViewState["PagerCount"]); 
            }
        }
        #endregion

        #region 当前的页号
        /// <summary>
        /// 当前的页号
        /// </summary>
        public Int32 PageIndex
        {
            set { _myViewState["PageIndex"] = value.ToString(CultureInfo.InvariantCulture); }
            get{
                if (_myViewState["PageIndex"] == null) { return 1; }
                return Int32.Parse(_myViewState["PageIndex"]); 
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// 初始化，设置存放属性值的位置，和密钥
        /// </summary>
        public PagerSQL()
        {
            //设置存放数据的默认位置——隐藏域
            _myViewState.SaveLocation = SaveViewStateLocation.Hidden;
            //空字符串表示不加密，没有设置的话，使用默认的密钥
            _myViewState.Key = SaveKey ?? "12128889";

        }

        #region 外部函数
        #region 传入页号，返回指定页号的SQL语句
        /// <summary>
        /// 传入页号，返回指定页号的SQL语句
        /// </summary>
        /// <param name="pageIndex">页号</param>
        /// <returns></returns>
        public string GetSQLByPageIndex(Int32 pageIndex)
        {
            if (_sqlFunction == null)
                SetSQLFunction(SetPagerSQLKind);

            return _sqlFunction.GetSQLByPageIndex(pageIndex);
        }
        #endregion

        #region 计算页数
        /// <summary>
        /// 通过总记录数、一页的记录数计算页数
        /// </summary>
        public Int32 ComputePageCount()
        {
            Int32 recordCount = RecordCount ;
            Int32 pageSize = PageSize;

            Int32 tmpPageCount = 1;

            if (recordCount > 0)
            {
                //计算页数
                tmpPageCount = recordCount / pageSize + (recordCount % pageSize == 0 ? 0 : 1);
            }

            PageCount = tmpPageCount;
            return tmpPageCount;
        }
        #endregion

        #region 生成SQL语句模版、生成总记录数的SQL语句。
        /// <summary>
        /// 调用函数，拼接需要的SQL语句
        /// </summary>
        public void CreateSQL()
        {
            if (_sqlFunction == null)
                SetSQLFunction(SetPagerSQLKind);

            //重新生成SQL语句模版
            _sqlFunction.CreateRecordCountSQL();     //统计记录数用的SQL语句
            _sqlFunction.CreateFirstPageSQL();       //第一页的SQL语句模板
            _sqlFunction.CreateNextPageSQL();        //任意页的SQL语句模板
            _sqlFunction.CreateLastPageSQL();        //最后一页的SQL语句模板
        }

        #endregion

        #endregion
    }
}
