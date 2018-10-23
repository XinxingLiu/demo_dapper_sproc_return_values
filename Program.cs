using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace sqlConnectionTest
{
    class StoredProcedureParameters : DynamicParameters
    {
        public StoredProcedureParameters(object parameters)
        {
            this.Add("@ErrorMessage", dbType:DbType.String, direction:ParameterDirection.Output, size:255);
            this.Add("@ErrorCode", dbType:DbType.Int32, direction:ParameterDirection.ReturnValue, size:1);
            this.AddDynamicParams(parameters);
        }

        public int getErrorCode()
        {
            return this.Get<int>("@ErrorCode");
        }
        public string getErrorMessage()
        {
            return this.Get<string>("@ErrorMessage");
        }
    }

    class StoredProcedureExecutionResult<T>
    {
        public T Result {set; get;}
        public bool IsSuccessful
        {
            get 
            {
                return ErrorCode == 0;
            }
        }
        public int ErrorCode {private set; get;}
        public string ErrorMessage {private set; get;}

        public StoredProcedureExecutionResult(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }

    class StoredProcedureExecutionResultBuilder
    {
        private int errorCode;
        private string errorMessage;

        public StoredProcedureExecutionResultBuilder retrieveOutputParameters(StoredProcedureParameters parameters)
        {
            errorCode = parameters.getErrorCode();
            errorMessage = parameters.getErrorMessage();
            return this;
        }

        public StoredProcedureExecutionResult<T> build<T>(Func<T> func)
        {
            StoredProcedureExecutionResult<T> result = new StoredProcedureExecutionResult<T>(errorCode, errorMessage);
            if(result.IsSuccessful)
            {
                result.Result = func();
            }
            return result;
        }
    }

    class Program
    {
        private static readonly string connectionString = "{your connection string}";
        
        static void Main(string[] args)
        {
            Console.WriteLine("Started...");
            runSqlProcedure().GetAwaiter().GetResult();
            Console.WriteLine("Done.");
        }

        static async Task<StoredProcedureExecutionResult<int>> runSqlProcedure()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    /*
                    var parameters = new 
                    {
                        Name = "state10",
                        Value = 80,
                        DataTypeId = 1,
                    };
                    */
                    var parameters = new StoredProcedureParameters(new
                    {
                        Name = "state10",
                        Value = 80,
                        DataTypeId = 1,
                    });

                    await connection.OpenAsync();
                    var output = await connection
                          .QueryAsync(
                            "[dbo].[InsertIntoEnumValue]",
                            param: parameters,
                            commandType: CommandType.StoredProcedure);
                    
                    /*
                    int result = (int)output.AsList().FirstOrDefault().NewEnumId;
                    */
                    var result = (new StoredProcedureExecutionResultBuilder())
                                 .retrieveOutputParameters(parameters)
                                 .build(() => (int)output.AsList().FirstOrDefault().NewEnumId);

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
