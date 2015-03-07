namespace FS.Core.Infrastructure.Query
{
    public interface IQueryQueueUpdate : IQueryQueueExecute
    {
        /// <summary>
        /// 生成SQL
        /// </summary>
        int Query<T>(T entity) where T : class;
    }
}
