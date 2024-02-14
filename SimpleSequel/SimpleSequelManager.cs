using System.Data.Common;

namespace SimpleSequel
{
    public class SimpleSequelManager
    {
        public DbConnection Connection {  get; }

        private static SimpleSequelManager? _instance;

        public static SimpleSequelManager Instance => _instance
            ?? throw SimpleSequelException.NewNotInitException();

        private SimpleSequelManager(DbConnection _connection)
        {
            this.Connection = _connection;
        }

        public static void Initialize(DbConnection Connection) => _instance = new SimpleSequelManager(Connection);
        public DbCommand NewCommand() => Connection.CreateCommand();


        public delegate object? StatementExecutionDelegate(DbCommand command);
        public delegate Task<object?> StatemenExecutionDelegateAsync(DbCommand command);

        public object? ExecuteSequel(string statement, StatementExecutionDelegate execution, bool connectionHandling = true)
        {
            if(_instance == null)
                throw SimpleSequelException.NewNotInitException();

            bool IsConnectionOnInputOpen = _instance.Connection.State == System.Data.ConnectionState.Open;
            if(!IsConnectionOnInputOpen)
                _instance.Connection.Open();

            using DbCommand command = _instance.NewCommand();
            command.CommandText = statement;

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
                if(connectionHandling && !IsConnectionOnInputOpen)
                    _instance.Connection.Close();
            }
        }

        public async Task<object?> ExecuteSequelAsnyc(string statement, StatemenExecutionDelegateAsync execution, bool autocloseConnection = true)
        {
            if (_instance == null) 
                throw SimpleSequelException.NewNotInitException();

            bool IsConnectionOnInputOpen = _instance.Connection.State == System.Data.ConnectionState.Open;
            if (!IsConnectionOnInputOpen)
                await _instance.Connection.OpenAsync();

            using DbCommand command = _instance.NewCommand();
            command.CommandText = statement;

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
                if (autocloseConnection && !IsConnectionOnInputOpen)
                    await _instance.Connection.CloseAsync();
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
