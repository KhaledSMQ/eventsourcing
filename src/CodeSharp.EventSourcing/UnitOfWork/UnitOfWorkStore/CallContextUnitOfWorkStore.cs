//Copyright (c) CodeSharp.  All rights reserved.

using System.Collections;
using System.Runtime.Remoting.Messaging;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// ����CallContextʵ�ֵ�UnitOfWork�������ڹ������ִ洢��ʾUnitOfWork������������
    /// ��ǰ�߼��̵߳ĵ��������ġ������ǰ�Ƿ�WebӦ�����ø������洢UnitOfWork���������ڣ�
    /// ʵ�ַ�ʽͬ���ο���CallContextSessionStore�ĳ���������
    /// </summary>
    [Component(LifeStyle.Singleton)]
    public class CallContextUnitOfWorkStore : AbstractDictStackUnitOfWorkStore
    {
        protected override IDictionary GetDictionary()
        {
            return CallContext.GetData(this.SlotKey) as IDictionary;
        }
        protected override void StoreDictionary(IDictionary dictionary)
        {
            CallContext.SetData(this.SlotKey, dictionary);
        }
    }
}