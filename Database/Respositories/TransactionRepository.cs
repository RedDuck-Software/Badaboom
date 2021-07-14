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

        public async Task<IEnumerable<Call>> GetCallsByAddressAndMethodAsync(CallsPagination pagination)
        {
            if (pagination != null)
            {
                if (!(pagination.BlockId != null || pagination.To != null))
                    throw new System.ArgumentException("Invalid pagination parameters");
            }


            string method = pagination?.MethodId;
            string to = pagination?.To;
            string block = pagination?.BlockId?.ToString();

            string blockWherePaginationQuery = block == null ? "" : $"t.BlockId={block} {(to == null ? "" : " and ")}";
            string toWherePaginationQuery = to == null ? "" : $"c.To=convert(binary(20),'{to}',1) {(method == null ? "" : " and ")}";
            string methodWherePaginationQuery = method == null ? "" : $"c.MethodId=convert(binary(4),'{method}',1) ";

            var sql =
                "select " +
                    "c.CallId " +
                    "convert(varchar(6), c.[TransactionHash], 1) as TransactionHash, " +
                    "c.Error " +
                    "c.Type " +
                    "convert(varchar(44), c.[From], 1) as From, " +
                    "convert(varchar(44), c.[To], 1) as To, " +
                    "convert(varchar(10), c.[MethodId], 1) as MethodId, " +
                    "convert(varchar(MAX), c.[Input], 1) as Input, " +

                "from " +
                    "Calls c " +
                block == null ? "" :
                ("inner join Transactions t " +
                "on " +
                    "c.TransactionHash=t.TransactionHash") +
                "where " +
                    blockWherePaginationQuery +
                    toWherePaginationQuery +
                    methodWherePaginationQuery + ";";

            return await SqlConnection.QueryAsync<Call>(sql);
        }
    }
}
