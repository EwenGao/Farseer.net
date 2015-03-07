namespace FS.Core.Infrastructure.Query
{
    public interface IQueryQueueInfo : IQueryQueueExecute
    {
        /// <summary>
        /// 生成SQL
        /// </summary>
        T Query<T>() where T : class;
    }
}
