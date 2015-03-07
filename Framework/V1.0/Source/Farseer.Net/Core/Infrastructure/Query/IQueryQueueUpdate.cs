namespace FS.Core.Infrastructure.Query
{
    public interface IQueryQueueUpdate : IQueryQueueExecute
    {
        /// <summary>
        /// 生成SQL
        /// </summary>
        T Query<T>() where T : class;
    }
}
