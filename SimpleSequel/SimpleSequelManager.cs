using System.Data.Common;

namespace SimpleSequel
{
    public class SimpleSequelManager
    {
        private readonly DbConnection _connection;
        private static SimpleSequelManager? _instance;

        public static SimpleSequelManager Instance => _instance
            ?? throw SimpleSequelException.NewNotInitException();

        private SimpleSequelManager(DbConnection _connection)
        {
            this._connection = _connection;
        }

        public static void Initialize(DbConnection Connection) => _instance = new SimpleSequelManager(Connection);
        public DbCommand NewCommand() => _connection.CreateCommand();


        public delegate object? StatementExecutionDelegate(DbCommand command);
        public delegate Task<object?> StatemenExecutionDelegateAsync(DbCommand command);

        public object? ExecuteSequel(string statement, StatementExecutionDelegate execution)
        {
            if(_instance == null)
                throw SimpleSequelException.NewNotInitException();

            using DbCommand command = _instance.NewCommand();
            command.CommandText = statement;

            bool IsConnectionOnInputOpen = _instance._connection.State == System.Data.ConnectionState.Open;
            if(!IsConnectionOnInputOpen)
                _instance._connection.Open();

            try
            {
                object? result = execution(command);
                return result;
            }
            catch(Exception ex)
            {
                throw SimpleSequelException.NewOpException(ex, statement);
            }
            finally
            {
                if(!IsConnectionOnInputOpen)
                    _instance._connection.Close();
            }
        }

        public async Task<object?> ExecuteSequelAsnyc(string statement, StatemenExecutionDelegateAsync execution)
        {
            if (_instance == null) 
                throw SimpleSequelException.NewNotInitException();

            using DbCommand command = _instance.NewCommand();
            command.CommandText = statement;

            bool IsConnectionOnInputOpen = _instance._connection.State == System.Data.ConnectionState.Open;
            if (!IsConnectionOnInputOpen)
                _instance._connection.Open();

            try
            {
                object? result = await execution(command);
                return result;
            }
            catch (Exception ex)
            {
                throw SimpleSequelException.NewOpException(ex, statement);
            }
            finally
            {
                if (!IsConnectionOnInputOpen)
                    _instance._connection.Close();
            }
        }

        public async Task<T?> ExecuteSequelAsnyc<T>(string statement, StatemenExecutionDelegateAsync execution)
        {
            var result = await ExecuteSequelAsnyc(statement, execution);
            return SqlExtensions.ConvertToType<T>(result);
        }

        public T? ExecuteSequel<T>(string statement, StatementExecutionDelegate execution)
        {
            var result = ExecuteSequel(statement, execution);
            return SqlExtensions.ConvertToType<T>(result);
        }
    }
}
