using Expenses.Domain;
using Expenses.SharedKernel;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Transactions;

namespace Expenses.Data
{
    public class ExpensesDbContext : DbContext, IExpensesDbContext
    {
        #region Private Fields
        private bool _disposed;
        //private DbContextTransaction _currentTransaction;
        private TransactionScope _currentTransactionScope;
        private Guid _instanceId;
        #endregion Private Fields

        //protected readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DbSet<Expense> Expenses { get; set; }

        public ExpensesDbContext()
            : base("ExpensesDbContext")
        {
            // Do NOT enable proxied entities, else serialization fails
            Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            Configuration.LazyLoadingEnabled = false;

            // Because Web API will perform validation, we don't need/want EF to do so
            Configuration.ValidateOnSaveEnabled = false;

            //DbContext.Configuration.AutoDetectChangesEnabled = false;
            // We won't use this performance tweak because we don't need 
            // the extra performance and, when autodetect is false,
            // we'd have to be careful. We're not being that careful.

            _instanceId = Guid.NewGuid();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Use singular table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Remove Cascade delete conventions
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);

            // Ignore ObjectState property for all entities that implements IObjectTrackingState
            modelBuilder.Types<IObjectTrackingState>().Configure(c => c.Ignore(e => e.ObjectState));
        }

        public override int SaveChanges()
        {
            try
            {
                SyncObjectsStatePreCommit();
                ApplyAuditRules();
                var changes = base.SaveChanges();
                SyncObjectsStatePostCommit();
                return changes;
            }
            catch (DbEntityValidationException validationException)
            {
                //string errorMessage = LogErrorsOnSaveChanges(validationException);
                //// Throw a new DbEntityValidationException with the improved exception message.
                //throw new DbEntityValidationException(errorMessage, validationException.EntityValidationErrors);
                throw;
            }
            catch (DbUpdateException updateException)
            {
                //string errorMessage = LogErrorsOnSaveChanges(updateException);
                //throw new DbUpdateException(errorMessage, updateException.InnerException);
                throw;
            }
        }

        public void BeginTransaction()
        {
            try
            {
                if (_currentTransactionScope != null)
                {
                    return;
                }

                _currentTransactionScope = new TransactionScope();

                //_currentTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);
            }
            catch (Exception)
            {
                // todo: log transaction exception
                throw;
            }
        }

        public void CloseTransaction()
        {
            CloseTransaction(exception: null);
        }

        public void CloseTransaction(Exception exception)
        {
            try
            {
                if (_currentTransactionScope != null && exception != null)
                {
                    // todo: log exception
                    //_currentTransaction.Rollback();
                    return;
                }

                SaveChanges();

                if (_currentTransactionScope != null)
                {
                    _currentTransactionScope.Complete();
                }
            }
            catch (Exception)
            {
                // todo: log exception
                //if (_currentTransaction != null && _currentTransaction.UnderlyingTransaction.Connection != null)
                //{
                //    _currentTransaction.Rollback();
                //}

                throw;
            }
            finally
            {
                if (_currentTransactionScope != null)
                {
                    _currentTransactionScope.Dispose();
                    _currentTransactionScope = null;
                }
            }
        }

        /// <summary>
        /// Adds the Create and Modify, User and Date values to any entity that
        /// implements IAudit.
        /// </summary>
        private void ApplyAuditRules()
        {
            var currentDate = DateTime.Now;
            var currentPrincipal = Thread.CurrentPrincipal as System.Security.Claims.ClaimsPrincipal;

            var entries = ChangeTracker.Entries().Where(e =>
                e.Entity is IAudit && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var e = entry.Entity as IAudit;
                if (entry.State == EntityState.Added)
                {
                    e.DateCreated = currentDate;
                    e.UserCreated = currentPrincipal.Identity.Name;
                }
                e.DateModified = currentDate;
                e.UserModified = currentPrincipal.Identity.Name;
            }
        }

        #region Manage state

        private void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IObjectTrackingState
        {
            Entry(entity).State = StateHelper.ConvertState(entity.ObjectState);
        }

        private void SyncObjectsStatePreCommit()
        {
            foreach (var dbEntityEntry in ChangeTracker.Entries<IObjectTrackingState>())
            {
                dbEntityEntry.State = StateHelper.ConvertState(((IObjectTrackingState)dbEntityEntry.Entity).ObjectState);
            }
        }

        private void SyncObjectsStatePostCommit()
        {
            foreach (var dbEntityEntry in ChangeTracker.Entries<IObjectTrackingState>())
            {
                ((IObjectTrackingState)dbEntityEntry.Entity).ObjectState = StateHelper.ConvertState(dbEntityEntry.State);
            }
        }

        #endregion

        #region Log errors

        //private string LogErrorsOnSaveChanges(DbEntityValidationException ex)
        //{
        //    // Retrieve the error messages as a list of strings.
        //    var errorMessages = ex.EntityValidationErrors
        //            .SelectMany(x => x.ValidationErrors)
        //            .Select(x => x.ErrorMessage);

        //    // Join the list to a single string.
        //    var fullErrorMessage = string.Join("; ", errorMessages);

        //    // Combine the original exception message with the new one.
        //    var errorMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
        //    Log.Error(errorMessage);
        //    return errorMessage;
        //}

        //private string LogErrorsOnSaveChanges(DbUpdateException ex)
        //{
        //    var errorMessage = ex.GetBaseException().Message;

        //    // Combine the original exception message with the new one.
        //    var stringError = string.Concat(ex.Message, errorMessage);
        //    Log.Error(errorMessage, ex);
        //    return errorMessage;
        //}

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_currentTransactionScope != null)
                    {
                        _currentTransactionScope.Dispose();
                        _currentTransactionScope = null;
                    }
                    // free other managed objects that implement
                    // IDisposable only
                }

                // release any unmanaged objects
                // set object references to null

                _disposed = true;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
