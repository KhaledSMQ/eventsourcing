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
        /// <remarks>
        /// 1. UnitOfWork���������ڣ�ÿ��IUnitOfWorkManager��ʵ�������ʵ����ι���UnitOfWork����������
        /// 2. UnitOfWork����ά����������ģ���ھۺϸ������ĸı䣬ֻҪ���޸ĵİ��������ľۺϸ����ᱻUnitOfWork��ع���
        /// </remarks>
        /// </summary>
        IUnitOfWork GetUnitOfWork();
    }
}
