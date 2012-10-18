using System;
using Castle.Services.Transaction;
using ICastleTransaction = Castle.Services.Transaction.ITransaction;

namespace CodeSharp.EventSourcing.Castles
{
    /// <summary>
    /// ����Castleʵ�ֵ�UnitOfWorkManager
    /// </summary>
    public class UnitOfWorkManager : MarshalByRefObject, IUnitOfWorkManager
    {
        private readonly string DefaultAlias = "codesharp.eventsourcing.unitofworkmanager.castle";
        private readonly IUnitOfWorkStore _unitofWorkStore;
        private ILoggerFactory _loggerFactory;
        private ILogger _logger;

        /// <summary>
        /// ���캯��
        /// </summary>
        public UnitOfWorkManager(IUnitOfWorkStore unitofWorkStore, ILoggerFactory loggerFactory)
        {
            _unitofWorkStore = unitofWorkStore;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.Create("EventSourcing.UnitOfWorkManager");
        }

        /// <summary>
        /// ����һ����ǰ���õ�UnitOfWorkʵ��
        /// </summary>
        /// <remarks>
        /// UnitOfWork���������ں�NHibernate Session����������һ�£�
        /// ���������������£�
        /// 1��������û�������Castle��Transaction���Ե��������UnitOfWork������������Castle���������������һ�£�
        /// 2��������û�������Castle��Transaction���Ե��������UnitOfWork���������ھ���ȫ��UnitOfWorkStore�������������һ�£�
        /// ��Ҫ˵�����ǣ���ȻNHibernate Session����UnitOfWork����ͨ��ISessionStore��IUnitOfWorkStore���洢�ģ����ԴӴ洢ʱ��ĽǶ���˵
        /// ���û��������Session��UnitOfWork��Store���Ƴ�����ô������������������ھ���CallContext��������HttpContext���������ڣ�
        /// ������Ϊ���������õ���Castle����������������������������ھͲ���������CallContext����HttpContext�ˣ�������Castle�Ķ�������TalkativeTransaction
        /// ����������һ�£���TalkativeTransaction Commit��ɺ󣬻������������ע������е�ISynchronization�����AfterCompletion������
        /// ĿǰNHibernate��SessionDisposeSynchronization���ע�ᵽCastle��TalkativeTransaction�����Իᱻ���ã�������AfterCompletion������
        /// ��Session����������ISessionStore���Ƴ���
        /// ͬ����Event Sourcing���Ҳ�����һ��UnitOfWorkSynchronization�࣬�����Ҳ������ʱ��UnitOfWork����������UnitOfWorkStore���Ƴ���
        /// </remarks>
        public IUnitOfWork GetUnitOfWork()
        {
            UnitOfWorkDelegate unitOfWorkDelegate = _unitofWorkStore.FindCompatibleUnitOfWork(DefaultAlias) as UnitOfWorkDelegate;
            ICastleTransaction currentTransaction = GetCurrentCastleTransaction();
            bool hasCastleTransaction = currentTransaction != null;

            if (unitOfWorkDelegate == null)
            {
                unitOfWorkDelegate = CreateUnitOfWorkDelegate(CreateUnitOfWork(), hasCastleTransaction);
                RegisterUnitOfWorkSynchronization(currentTransaction, unitOfWorkDelegate);
                _unitofWorkStore.Store(DefaultAlias, unitOfWorkDelegate);
            }
            else
            {
                unitOfWorkDelegate.UpdateCanAutoDispose(!hasCastleTransaction);
            }

            return unitOfWorkDelegate;
        }

        /// <summary>
        /// ��ITransactionManager��ȡ��ǰCastle����
        /// </summary>
        private ICastleTransaction GetCurrentCastleTransaction()
        {
            return DependencyResolver.Resolve<ITransactionManager>().CurrentTransaction;
        }
        /// <summary>
        /// ����һ��UnitOfWorkʵ��
        /// </summary>
        private IUnitOfWork CreateUnitOfWork()
        {
            return DependencyResolver.Resolve<IUnitOfWork>();
        }
        /// <summary>
        /// ��װUnitOfWork
        /// </summary>
        private UnitOfWorkDelegate CreateUnitOfWorkDelegate(IUnitOfWork unitofWork, bool hasCastleTransaction)
        {
            return new UnitOfWorkDelegate(unitofWork, _unitofWorkStore, _loggerFactory, !hasCastleTransaction);
        }
        /// <summary>
        /// ע��һ��UnitOfWorkSynchronization����ͬ�����󵽵�ǰCastle�Ķ���������
        /// </summary>
        private void RegisterUnitOfWorkSynchronization(ICastleTransaction transaction, UnitOfWorkDelegate unitofWorkDelegate)
        {
            if (transaction != null && !transaction.IsChildTransaction)
            {
                transaction.RegisterSynchronization(new UnitOfWorkSynchronization(transaction, unitofWorkDelegate));
            }
        }
    }
}