namespace Database
{
    public static class ConnectionStrings
    {
        public static string GetDefaultConnectionToDatabase(string dbName) =>
            $@"server=(LocalDb)\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=True;MultipleActiveResultSets=True;";

        public static readonly string BscDbName = "bscBadaboomDb";
     
        public static readonly string EthDbName = "ethBadaboomDb";
    }
}
