using System;
using System.Collections.Generic;
using System.Threading;

namespace CodeSharp.EventSourcing.Castles
{
    /// <summary>
    /// һ�������࣬ʵ����IUnitOfWork�ӿڣ��ڲ���װ��һ��IUnitOfWorkʵ����
    /// ����IUnitOfWork�ӿڶ�������Ի򷽷���ת�����ڲ�ά����IUnitOfWorkʵ��ʵ�֡�
    /// ����ô����Ŀ����Ϊ�˵�UnitOfWork������Castle���������£�
    /// ��ǰUnitOfWorkDelegate���󲻻��Զ�����UnitOfWorkStore���Ƴ���
    /// ����������Castle������������ʱ���ͷŵ�ǰ��UnitOfWorkDelegate��
    /// </summary>
    public class UnitOfWorkDelegate : MarshalByRefObject, IUnitOfWork
    {
        #region Private Variables

        private IUnitOfWork _unitOfWork;
        private IUnitOfWorkStore _unitofWorkStore;
        private ILogger _logger;
        private bool _canAutoDispose;
        private bool _isDisposed;
        private readonly ReaderWriterLockSlim _readerWriterLockSlim;

        #endregion

        #region Constructors

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="unitOfWork">�ڲ���װ��UnitOfWork</param>
        /// <param name="unitofWorkStore">�洢UnitOfWork��Store</param>
        /// <param name="loggerFactory">Log Factory</param>
        /// <param name="canAutoDispose">��ʾ��ǰ��UnitOfWorkDelegate�����Ƿ�������Dispose����������ʱ�Զ���UnitOfWorkStore���Ƴ�</param>
        public UnitOfWorkDelegate(IUnitOfWork unitOfWork, IUnitOfWorkStore unitofWorkStore, ILoggerFactory loggerFactory, bool canAutoDispose)
        {
            _unitOfWork = unitOfWork;
            _unitofWorkStore = unitofWorkStore;
            _logger = loggerFactory.Create("EventSourcing.UnitOfWorkDelegate");
            _canAutoDispose = canAutoDispose;
            _isDisposed = false;
            _readerWriterLockSlim = new ReaderWriterLockSlim();
        }

        #endregion

        #region Public & Internal Properties

        /// <summary>
        /// �����ڲ���װ��UnitOfWork����
        /// </summary>
        public IUnitOfWork InnerUnitOfWork
        {
            get { return _unitOfWork; }
        }
        /// <summary>
        /// һ������ĿǰΪһ��Stackʵ�����ڽ���ǰUnitOfWorkDelegate�����UnitOfWorkStore���Ƴ�ʱ�����õ�������ԣ�
        /// �����AbstractUnitOfWorkStore��Remove������
        /// </summary>
        public object Cookie
        {
            get { return _unitOfWork.Cookie; }
            set { _unitOfWork.Cookie = value; }
        }

        #endregion

        #region IUnitOfWork �ӿ�ʵ��

        /// <summary>
        /// ����ĳ���ۺϸ�
        /// </summary>
        public void TrackingAggregateRoot(AggregateRoot aggregateRoot)
        {
            _unitOfWork.TrackingAggregateRoot(aggregateRoot);
        }

        /// <summary>
        /// �������е�ǰ���ٵľۺϸ�
        /// </summary>
        public IEnumerable<AggregateRoot> GetAllTrackingAggregateRoots()
        {
            return _unitOfWork.GetAllTrackingAggregateRoots();
        }

        /// <summary>
        /// �ύ��ǰ���ٵ����оۺϸ����������������¼�
        /// </summary>
        /// <returns>
        /// ���������ѱ�������¼�
        /// </returns>
        public IEnumerable<object> SubmitChanges()
        {
            return _unitOfWork.SubmitChanges();
        }

        /// <summary>
        /// �����ǰUnitOfWork����û�б���������װ�����ͷ��Լ������򣬲����κ����顣
        /// </summary>
        public void Dispose()
        {
            if (_canAutoDispose)
            {
                InternalDispose();
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// �����Ƿ���Ա��Զ�Dispose�ı��λ
        /// </summary>
        internal void UpdateCanAutoDispose(bool canAutoDispose)
        {
            _readerWriterLockSlim.AtomWrite(
                () =>
                {
                    _canAutoDispose = canAutoDispose;
                }
            );
        }
        /// <summary>
        /// �����ڲ�������ֻ�������ڲ����ã�
        /// ��Castle���������ύ��ɺ����ø÷�������ǰ��UnitOfWork��UnitOfWorkStore���Ƴ�
        /// </summary>
        internal void InternalDispose()
        {
            if (!_isDisposed)
            {
                 _unitofWorkStore.Remove(this);
                _unitOfWork.Dispose();
                _isDisposed = true;
                if (_logger.IsDebugEnabled)
                {
                    _logger.DebugFormat("Disposed {0}.", _unitOfWork.GetType().Name);
                }
            }
        }

        #endregion
    }
}