using kudos.backend.domain.exceptions;
using kudos.backend.domain.interfaces.repositories;
using FluentValidation;
using NHibernate;

namespace kudos.backend.infrastructure.nhibernate;

/// <summary>
/// Implementation of the repository pattern using NHibernate ORM.
/// </summary>
public abstract class NHRepository<T, TKey> : NHReadOnlyRepository<T, TKey>, IRepository<T, TKey> where T : class, new()
{
    private readonly AbstractValidator<T> validator;

    protected NHRepository(ISession session, IServiceProvider serviceProvider)
    : base(session)
    {
        Type genericType = typeof(AbstractValidator<>).MakeGenericType(typeof(T));
        this.validator = serviceProvider.GetService(genericType) as AbstractValidator<T>
            ?? throw new InvalidOperationException($"The validator for {typeof(T)} type could not be created");
    }

    public T Add(T item)
    {
        var validationResult = this.validator.Validate(item);
        if (!validationResult.IsValid)
            throw new InvalidDomainException(validationResult.Errors);

        this._session.Save(item);
        this.FlushWhenNotActiveTransaction();
        return item;
    }

    public async Task AddAsync(T item)
    {
        var validationResult = this.validator.Validate(item);
        if (!validationResult.IsValid)
            throw new InvalidDomainException(validationResult.Errors);

        await this._session.SaveAsync(item);
        await this.FlushWhenNotActiveTransactionAsync();
    }

    public T Save(T item)
    {
        var validationResult = this.validator.Validate(item);
        if (!validationResult.IsValid)
            throw new InvalidDomainException(validationResult.Errors);
        this._session.Update(item);
        this.FlushWhenNotActiveTransaction();
        return item;
    }

    public async Task SaveAsync(T item)
    {
        var validationResult = this.validator.Validate(item);
        if (!validationResult.IsValid)
            throw new InvalidDomainException(validationResult.Errors);

        await this._session.UpdateAsync(item);
        await this.FlushWhenNotActiveTransactionAsync();
    }

    public void Delete(T item)
    {
        this._session.Delete(item);
        this.FlushWhenNotActiveTransaction();
    }

    public async Task DeleteAsync(T item)
    {
        await this._session.DeleteAsync(item);
        await this.FlushWhenNotActiveTransactionAsync();
    }

    protected internal bool IsTransactionActive()
        => this._session.GetCurrentTransaction() != null && this._session.GetCurrentTransaction().IsActive;

    protected internal void FlushWhenNotActiveTransaction()
    {
        var currentTransaction = this._session.GetCurrentTransaction();
        if (currentTransaction == null || !currentTransaction.IsActive)
            this._session.Flush();
    }

    protected internal async Task FlushWhenNotActiveTransactionAsync()
    {
        var currentTransaction = this._session.GetCurrentTransaction();
        if (currentTransaction == null || !currentTransaction.IsActive)
            await this._session.FlushAsync();
    }
}
