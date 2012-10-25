//Copyright (c) CodeSharp.  All rights reserved.

using System;
using Castle.Services.Transaction;

namespace CodeSharp.EventSourcing.Castles
{
    /// <summary>
    /// ����Castle��������ʵ�ֵ�UnitOfWorkManager
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
        /// <remarks>
        /// 1. UnitOfWork����ά����������ģ���ھۺϸ������ĸı䣬ֻҪ���޸ĵİ��������ľۺϸ����ᱻUnitOfWork��ع���
        /// 2. UnitOfWork������������Castle���������������һ�£�����Castle�����ύ֮ǰUnitOfWork�Ὣ���оۺϸ����޸��ύ�����ݿ�;
        /// </remarks>
        /// </summary>
        public IUnitOfWork GetUnitOfWork()
        {
            var unitOfWorkDelegate = _unitofWorkStore.FindCompatibleUnitOfWork(DefaultAlias) as UnitOfWorkDelegate;
            var currentTransaction = GetCurrentTransaction();
            bool hasTransaction = currentTransaction != null;

            if (unitOfWorkDelegate == null)
            {
                unitOfWorkDelegate = CreateUnitOfWorkDelegate(CreateUnitOfWork(), hasTransaction);
                RegisterUnitOfWorkSynchronization(currentTransaction, unitOfWorkDelegate);
                _unitofWorkStore.Store(DefaultAlias, unitOfWorkDelegate);
            }
            else
            {
                unitOfWorkDelegate.UpdateCanAutoDispose(!hasTransaction);
            }

            return unitOfWorkDelegate;
        }

        /// <summary>
        /// ��ITransactionManager��ȡ��ǰCastle����
        /// </summary>
        private ITransaction GetCurrentTransaction()
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
        private void RegisterUnitOfWorkSynchronization(ITransaction transaction, UnitOfWorkDelegate unitofWorkDelegate)
        {
            if (transaction != null && !transaction.IsChildTransaction)
            {
                transaction.RegisterSynchronization(new UnitOfWorkSynchronization(transaction, unitofWorkDelegate));
            }
        }
    }
}