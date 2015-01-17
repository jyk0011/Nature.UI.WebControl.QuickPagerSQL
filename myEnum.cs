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
 * function: 分页方式、运行方式、记录集的枚举定义
 * history:  created by 金洋 
 *           2011-01-28 整理
 *           2011-4-11 整理
 * **********************************************
 */

namespace Nature.UI.WebControl.QuickPagerSQL
{
    /// <summary>
    /// 分页控件采用的分页算法
    /// </summary>
    public enum PagerSQLKind
    {
        /// <summary>
        /// 自动，跟随基类里的设置
        /// </summary>
        Auto = 501,
        /// <summary>
        /// SQL Server2005的Row_Number
        /// </summary>
        RowNumber = 502,
        /// <summary>
        /// 表变量
        /// </summary>
        TableVar = 503,
        /// <summary>
        /// Max
        /// </summary>
        MaxMin = 504,
        /// <summary>
        /// 颠倒Top
        /// </summary>
        TopTop = 505,
        /// <summary>
        /// Max、表变量自动切换
        /// </summary>
        MaxTableVar = 506,
        /// <summary>
        /// Max、颠倒Top自动切换
        /// </summary>
        MaxTopTop = 507 

    }

    
}
