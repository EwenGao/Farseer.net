namespace FS.Core.Infrastructure.Query
{
    public interface IQueryQueueDelete : IQueryQueueExecute
    {
        /// <summary>
        /// 生成SQL
        /// </summary>
        T Query<T>( ) where T : class;
    }
}
