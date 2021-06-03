create table [Blocks](
	BlockNumber BIGINT NOT NULL PRIMARY KEY , 
	IndexingStatus NVARCHAR(20) NOT NULL DEFAULT 'INDEXED'
);