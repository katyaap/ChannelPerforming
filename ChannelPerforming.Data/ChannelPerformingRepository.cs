namespace ChannelPerforming.Data
{
    using System;
    using System.Linq;

    using ChannelPerforming.Common;
    using ChannelPerforming.Entities;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public class ChannelPerformingRepository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : EntityBase
    {
        private static CloudStorageAccount _storageAccount;
        private ChannelPerformingContext _context;
        private string _entitySetName;

        static ChannelPerformingRepository()
        {
            _storageAccount = CloudStorageAccount.FromConfigurationSetting(Utils.ConfigurationString);
            CloudTableClient.CreateTablesFromModel(
                typeof(ChannelPerformingContext),
                _storageAccount.TableEndpoint.AbsoluteUri,
                _storageAccount.Credentials);
        }

        public ChannelPerformingRepository()
        {
            _entitySetName = typeof(TEntity).Name;

            this._context = new ChannelPerformingContext(_storageAccount.TableEndpoint.AbsoluteUri, _storageAccount.Credentials);
            this._context.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(1));
        }

        public TEntity Find(string partitionKey, string rowKey)
        {
            return (from g in _context.CreateQuery<TEntity>(_entitySetName)
                    where g.PartitionKey == partitionKey && g.RowKey == rowKey
                    select g).FirstOrDefault();
        }

        public TEntity Find(string rowKey)
        {
            return (from g in _context.CreateQuery<TEntity>(_entitySetName)
                    where g.RowKey == rowKey
                    select g).FirstOrDefault();
        }

        public void Create(TEntity entity)
        {
            this._context.AddObject(_entitySetName, entity);
        }

        public void Delete(TEntity entity)
        {
            this._context.DeleteObject(entity);
        }

        public void Update(TEntity entityToUpdate)
        {
            this._context.UpdateObject(entityToUpdate);
        }

        public void SubmitChange()
        {
            this._context.SaveChanges();
        }

        public IQueryable<TEntity> Get()
        {
            return this._context.CreateQuery<TEntity>(_entitySetName);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
