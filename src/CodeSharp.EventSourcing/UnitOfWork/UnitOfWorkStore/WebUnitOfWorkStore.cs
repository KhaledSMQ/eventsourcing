//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections;
using System.Web;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// ����HttpContextʵ�ֵ�UnitOfWork�������ڹ������ִ洢��ʾUnitOfWork������������
    /// ��ǰ��һ��Http����������������ڡ������ǰ��WebӦ�����ø������洢UnitOfWork���������ڣ�
    /// ʵ�ַ�ʽͬ���ο���WebSessionStore�ĳ���������
    /// </summary>
    [Component(LifeStyle.Singleton)]
    public class WebUnitOfWorkStore : AbstractDictStackUnitOfWorkStore
    {
        protected override IDictionary GetDictionary()
        {
            return GetCurrentHttpContext().Items[this.SlotKey] as IDictionary;
        }
        protected override void StoreDictionary(IDictionary dictionary)
        {
            GetCurrentHttpContext().Items[this.SlotKey] = dictionary;
        }

        private static HttpContext GetCurrentHttpContext()
        {
            HttpContext context = HttpContext.Current;

            if (context == null)
            {
                throw new EventSourcingException("WebUnitOfWorkStore: Could not obtain reference to HttpContext");
            }
            return context;
        }
    }
}