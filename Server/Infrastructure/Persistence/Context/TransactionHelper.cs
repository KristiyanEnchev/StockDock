﻿namespace Persistence.Context
{
    using Microsoft.EntityFrameworkCore.Storage;

    using Shared.Interfaces;

    public class TransactionHelper : ITransactionHelper
    {
        private readonly ApplicationDbContext _context;

        public TransactionHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public async ValueTask<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}