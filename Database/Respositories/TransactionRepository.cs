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

        public async Task<bool> ContainsTransactionAsync(Transaction tx)
        => await SqlConnection.ExecuteScalarAsync<bool>($"select count(1) from Transactions where TransactionHash=convert(binary(32),'{tx.TransactionHash}',2)");



        public async Task RemoveBlockTransftions(Block block)
        {
            var blockTransactions = await this.GetBlockTransactions(block);

            if (blockTransactions is null || !blockTransactions.Any()) return;

            string sql;

            foreach (var tx in blockTransactions)
            {
                sql =
                    "delete from [Calls] " +
                    $"where [TransactionHash]=convert(binary(32),'{tx.TransactionHash}',2);";

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

        public async Task<(IEnumerable<Call>, int?)> GetCalls(CallsPagination pagination, bool isCountCalculatedNeded)
        {
            string method = pagination?.MethodId;
            string from = pagination?.From;
            string to = pagination?.To;
            string block = pagination?.BlockId?.ToString();
            int page = pagination?.Page ?? 1;
            int count = pagination?.Count ?? 1;
            long? callIdFrom = pagination?.CallIdFrom;

            string blockWherePaginationQuery = block == null ? "" : $" t.BlockId=@BlockId ";
            string fromWherePaginationQuery = from == null ? "" : $" c.[From]=convert(binary(20),@From,1) ";
            string toWherePaginationQuery = to == null ? "" : $" c.[To]=convert(binary(20),@To,1) ";
            string methodWherePaginationQuery = method == null ? "" : $" c.MethodId=convert(binary(4),@MethodId,1) ";
            string fromCallIDWherePaginationQuery = callIdFrom == null ? "" : $" c.CallId<=@CallId ";

            List<string> whereList = new List<string>();

            void pushToWhereListIfNotNull(string value) { if (!string.IsNullOrEmpty(value)) whereList.Add(value); };

            pushToWhereListIfNotNull(blockWherePaginationQuery);
            pushToWhereListIfNotNull(fromWherePaginationQuery);
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
                "order by c.CallId desc " +
                $"offset {count * (page - 1)} rows " +
                $"FETCH NEXT {count} rows only;";


            var result = await SqlConnection.QueryAsync<Call>(sql, 
                new {
                        BlockId = block,
                        From = from,
                        To = to, 
                        MethodId = method, 
                        CallId = callIdFrom ?? 0
                    });

            if (isCountCalculatedNeded)
            {
                int totalCount = 10_000;

                if ((whereList.Count > 0 && callIdFrom == null) || (whereList.Count > 1 && callIdFrom != null))
                {
                    var sql2 =
                    $"SELECT COUNT(*) " +
                       "FROM [dbo].[Calls] c, [dbo].[Transactions] t " +
                      "WHERE t.TransactionHash = c.TransactionHash" +
                    (string.IsNullOrEmpty(resWhereStatement) ? "" : " and " + resWhereStatement);

                    totalCount = await SqlConnection.QuerySingleAsync<int>(sql2,
                        new {
                                BlockId = block,
                                From = from,
                                To = to,
                                MethodId = method,
                                CallId = callIdFrom ?? 0
                            });
                }

                return (result, totalCount);
            }

            return (result, null);
        }

        public async Task<IEnumerable<Call>> GetInternalTransactions(string tx)
        {
            var sql = "SELECT c.[Type], convert(varchar(44), c.[From], 1) as [From],convert(varchar(44), c.[To], 1) as [To] " +
                        "FROM [dbo].[Calls] c " +
                       "WHERE TransactionHash = convert(binary(32), @TransactionHash, 1) " +
                    "ORDER BY [CallId] ASC";

            return await SqlConnection.QueryAsync<Call>(sql, new { TransactionHash = tx });
        }
    }
}
