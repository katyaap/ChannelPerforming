namespace ChannelPerforming.Data
{
    using System.Linq;

    public interface IRepository<TEntity>
    {
        TEntity Find(string partitionKey, string rowKey);
        TEntity Find(string rowKey);
        void Create(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entityToUpdate);
        void SubmitChange();
        IQueryable<TEntity> Get();
    }
}
