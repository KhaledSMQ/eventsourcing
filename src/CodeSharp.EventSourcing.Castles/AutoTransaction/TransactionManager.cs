//Copyright (c) CodeSharp.  All rights reserved.

using Castle.Services.Transaction;

namespace CodeSharp.EventSourcing.Castles
{
    /// <summary>
    /// EventSourcing������õ���TransactionManager���Զ������Castle.Services.Transaction.DefaultTransactionManager
    /// ��TransactionCreated�¼��������ǰ�����������Ƕ��㸸�������Զ�����UnitOfWork
    /// </summary>
    public class TransactionManager : DefaultTransactionManager, ITransactionManager
    {
        private ILogger _logger = DependencyResolver.Resolve<ILoggerFactory>().Create("EventSourcing.TransactionManager");

        public TransactionManager() : base()
        {
            Initialize();
        }
        public TransactionManager(IActivityManager activityManager) : base(activityManager)
        {
            Initialize();
        }

        private void Initialize()
        {
            // ��Castle�Ķ������񴴽�ʱ����UnitOfWork��ȷ��UnitOfWork������Castle�Ķ��������ϣ�
            // UnitOfWork���븽���ڶ��������ϣ���Ϊֻ�ж�������Żᱻ����Commit��
            // ���UnitOfWork������Castle���������ϣ���ôUnitOfWork���޸Ľ��޷��Զ��ύ��
            TransactionCreated += (sender, e) =>
            {
                DependencyResolver.Resolve<IUnitOfWorkManager>().GetUnitOfWork();
                if (_logger.IsDebugEnabled)
                {
                    _logger.DebugFormat("{0} Created, Transaction Name:{1}.", e.Transaction.GetType().Name, e.Transaction.GetHashCode());
                }
            };

            //Castle�����񴴽�ʱ��¼��־
            ChildTransactionCreated += (sender, e) =>
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.DebugFormat("{0} Created, Transaction Name:{1}.", e.Transaction.GetType().Name, e.Transaction.GetHashCode());
                }
            };
        }
    }
}