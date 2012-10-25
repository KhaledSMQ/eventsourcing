//Copyright (c) CodeSharp.  All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Castle.Services.Transaction;

namespace CodeSharp.EventSourcing.Castles
{
    /// <summary>
    /// ͨ������ʵ����Castle�����ύ֮ǰ�ύUnitOfWork�������޸ģ������ύ֮���ͷ�UnitOfWork��
    /// </summary>
    public class UnitOfWorkSynchronization : ISynchronization
    {
        private ITransaction _transaction;
        private UnitOfWorkDelegate _unitOfWorkDelegate;
        private IAsyncMessageBus _asyncMessageBus;
        private ILogger _logger;
        private IEnumerable<object> _events;

        /// <summary>
        /// ���캯��
        /// </summary>
        public UnitOfWorkSynchronization(ITransaction transaction, UnitOfWorkDelegate unitOfWorkDelegate)
        {
            _transaction = transaction;
            _unitOfWorkDelegate = unitOfWorkDelegate;
            _asyncMessageBus = DependencyResolver.Resolve<IAsyncMessageBus>();
            _logger = DependencyResolver.Resolve<ILoggerFactory>().Create("EventSourcing.UnitOfWorkSynchronization");
        }

        public void BeforeCompletion()
        {
            _events = null;

            //ISynchronization��Castle������ύ��ع�ʱ���ᱻ���õ���
            //������Castle����ع�ʱ�����ǲ���Ҫִ��UnitOfWork��SubmitChanges������
            //������������Ҫ������жϣ�IsRollbackOnlySetΪtrue��ʾ��ǰCastle�������ڻع��Ĺ�����
            if (!_transaction.IsRollbackOnlySet)
            {
                var trackingAggregateRootCount = _unitOfWorkDelegate.GetAllTrackingAggregateRoots().Count();
                if (trackingAggregateRootCount > 0)
                {
                    if (_logger.IsDebugEnabled)
                    {
                        _logger.DebugFormat("{0} submiting changes. Total tracked aggregateRoot count��{1}", _unitOfWorkDelegate.InnerUnitOfWork.GetType().Name, trackingAggregateRootCount);
                    }
                    _events = _unitOfWorkDelegate.SubmitChanges();
                    if (_logger.IsDebugEnabled)
                    {
                        _logger.DebugFormat("{0} submitted changes. Total tracked aggregateRoot count��{1}", _unitOfWorkDelegate.InnerUnitOfWork.GetType().Name, trackingAggregateRootCount);
                    }
                }
            }
        }
        public void AfterCompletion()
        {
            _unitOfWorkDelegate.InternalDispose();

            //ֻ�е�Castle�����ύ�ɹ�ʱ����Ҫ�첽�ַ��¼�
            if (!_transaction.IsRollbackOnlySet && _events != null && _events.Count() > 0)
            {
                if (_logger.IsDebugEnabled)
                {
                    _logger.DebugFormat("{0} publishing events. Total events count��{1}", _asyncMessageBus.GetType().Name, _events.Count());
                }
                _asyncMessageBus.Publish(_events);
                if (_logger.IsDebugEnabled)
                {
                    _logger.DebugFormat("{0} published events. Total events count��{1}", _asyncMessageBus.GetType().Name, _events.Count());
                }
            }
        }
    }
}
