//Copyright (c) CodeSharp.  All rights reserved.

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// ����һ���ӿڣ������û���ȡһ����ǰ�����ĵ�UnitOfWorkʵ��
    /// </summary>
    public interface IUnitOfWorkManager
    {
        /// <summary>
        /// ����һ�����õ�UnitOfWorkʵ��
        /// </summary>
        IUnitOfWork GetUnitOfWork();
    }
}
