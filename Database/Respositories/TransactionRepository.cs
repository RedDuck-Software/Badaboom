using Dapper;
using Database.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Respositories
{
    public class TransactionRepository : RepositoryBase
    {
        public TransactionRepository(string connectionString) : base(connectionString) { }


        public async Task RemoveBlockTransftions(Block block)
        {
            var blockTransactions = await this.GetBlockTransactions(block);

            if (blockTransactions is null || !blockTransactions.Any()) return;

            string sql;

            foreach (var tx in blockTransactions)
            {
                sql =
                    "delete from [Calls] " +
                    "where [TransactionHash]=@TransactionHash";

                await SqlConnection.ExecuteAsync(sql, tx);



                sql =
                    "delete from [Transactions] " +
                    "where [TransactionHash]=@TransactionHash";

                await SqlConnection.ExecuteAsync(sql, tx);
            }
        }


        public async Task<IEnumerable<Transaction>> GetBlockTransactions(Block block)
        {
            var sql =
                "select * from [Transactions] " +
                "where BlockId=@BlockNumber";

            return await SqlConnection.QueryAsync<Transaction>(sql, block);
        }

        public async Task AddNewCallAsync(Call call)
        {
            var sql = "insert into Calls(TransactionId,[Error],[Type],[From],[To],MethodId) " +
                $"values(@TransactionHash,@Error,{(int)call.Type},@From,@To,@MethodId)";

            await SqlConnection.ExecuteAsync(sql, call);

        }

        public async Task<IEnumerable<Call>> GetCalls(CallsPagination pagination)
        {
            string method = pagination?.MethodId;
            string to = pagination?.To;
            string block = pagination?.BlockId?.ToString();
            int page = pagination?.Page ?? 1;
            int count = pagination?.Count ?? 1;
            int? callIdFrom = pagination?.CallIdFrom;

            string blockWherePaginationQuery = block == null ? "" : $" t.BlockId={block} ";
            string toWherePaginationQuery = to == null ? "" : $" c.[To]=convert(binary(20),'{to}',1) ";
            string methodWherePaginationQuery = method == null ? "" : $" c.MethodId=convert(binary(4),'{method}',1) ";
            string fromCallIDWherePaginationQuery = callIdFrom == null ? "" : $" c.CallId>={callIdFrom.Value} ";

            List<string> whereList = new List<string>();

            void pushToWhereListIfNotNull(string value) { if (!string.IsNullOrEmpty(value)) whereList.Add(value); };

            pushToWhereListIfNotNull(blockWherePaginationQuery);
            pushToWhereListIfNotNull(toWherePaginationQuery);
            pushToWhereListIfNotNull(methodWherePaginationQuery);
            pushToWhereListIfNotNull(fromCallIDWherePaginationQuery);

            string resWhereStatement = string.Join(" and ", whereList);

            var sql =
                "select " +
                    "c.CallId, " +
                    "convert(varchar(66), c.[TransactionHash], 1) as [TransactionHash], " +
                    "c.Error, " +
                    "c.Type, " +
                    "convert(varchar(44), c.[From], 1) as [From], " +
                    "convert(varchar(44), c.[To], 1) as [To], " +
                    "convert(varchar(10), c.[MethodId], 1) as [MethodId], " +
                    "lower(convert(varchar(MAX), c.[Input], 1)) as [Input], " +
                    "t.BlockId, " +
                    "t.TimeStamp " +

                "from " +
                    "Calls c, Transactions t " +
                "where " +
                    "t.TransactionHash=c.TransactionHash " +

                (string.IsNullOrEmpty(resWhereStatement) ? "" : " and " +  resWhereStatement) +
                "order by c.CallId " +
                $"offset {count * (page - 1)} rows " +
                $"FETCH NEXT {count} rows only;";

            return await SqlConnection.QueryAsync<Call>(sql);
        }
    }
}
