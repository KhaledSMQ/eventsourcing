using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// ��ʾһ���洢UnitOfWork��Store���������UnitOfWork����������
    /// </summary>
    public interface IUnitOfWorkStore
    {
        /// <summary>
        /// ����ĳ����������һ�����õ�UnitOfWork.
        /// </summary>
        IUnitOfWork FindCompatibleUnitOfWork(string alias);

        /// <summary>
        /// �洢ĳ��UnitOfWork
        /// </summary>
        void Store(string alias, IUnitOfWork unitofWork);

        /// <summary>
        /// �Ƴ�ָ����UnitOfWork
        /// </summary>
        void Remove(IUnitOfWork unitofWork);

        /// <summary>
        /// �ж�ĳ��������Ӧ��UnitOfWork�Ƿ����
        /// </summary>
        bool IsCurrentActivityEmptyFor(string alias);
    }
}
