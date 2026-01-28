using kudos.backend.domain.daos;
using kudos.backend.domain.interfaces.repositories;
using kudos.backend.infrastructure.nhibernate.repositories;
using NHibernate;

namespace kudos.backend.infrastructure.nhibernate;

/// <summary>
/// NHUnitOfWork is a concrete implementation of the IUnitOfWork interface.
/// It is used to manage transactions and the lifecycle of database operations in an NHibernate context.
/// </summary>
public class NHUnitOfWork : IUnitOfWork
{
    private bool _disposed = false;
    protected internal readonly ISession _session;
    protected internal readonly IServiceProvider _serviceProvider;
    protected internal ITransaction? _transaction;

    #region crud Repositories

    private IAuthorRepository? _authors;
    public IAuthorRepository Authors => _authors ??= new NHAuthorRepository(_session, _serviceProvider);

    private IBookRepository? _books;
    public IBookRepository Books => _books ??= new NHBookRepository(_session, _serviceProvider);

    private IBookImageRepository? _bookImages;
    public IBookImageRepository BookImages => _bookImages ??= new NHBookImageRepository(_session, _serviceProvider);

    #endregion

    #region read-only Repositories

    private IBookDaoRepository? _bookDaos;
    public IBookDaoRepository BookDaos => _bookDaos ??= new NHBookDaoRepository(_session);

    #endregion

    /// <summary>
    /// Constructor for NHUnitOfWork
    /// </summary>
    public NHUnitOfWork(ISession session, IServiceProvider serviceProvider)
    {
        _session = session;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Begin transaction
    /// </summary>
    public void BeginTransaction()
    {
        this._transaction = this._session.BeginTransaction();
    }

    /// <summary>
    /// Execute commit
    /// </summary>
    public void Commit()
    {
        if (_transaction != null && _transaction.IsActive)
            _transaction.Commit();
        else
            throw new TransactionException("The actual transaction is not longer active");
    }

    /// <summary>
    /// Determine if there is an active transaction
    /// </summary>
    public bool IsActiveTransaction()
        => _transaction != null && _transaction.IsActive;

    /// <summary>
    /// Reset the current transaction
    /// </summary>
    public void ResetTransaction()
        => _transaction = _session.BeginTransaction();

    /// <summary>
    /// Execute rollback
    /// </summary>
    public void Rollback()
    {
        if (_transaction != null)
            _transaction.Rollback();
        else
            throw new ArgumentNullException($"No active transaction found");
    }

    /// <summary>
    /// Dispose the current session
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (this._transaction != null)
                    this._transaction.Dispose();
                this._session.Dispose();
            }
            _disposed = true;
        }
    }

    /// <summary>
    /// Dispose the current session
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~NHUnitOfWork()
    {
        Dispose(false);
    }
}
