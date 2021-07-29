namespace Badaboom.Client.Shared.DTOs
{
    public class SelectedFiltersDTO
    {
        public bool BlockNumber { get; set; }
        public bool From { get; set; }
        public bool To { get; set; }
        public bool ContractAddress { get; set; }
        public bool MethodId { get; set; }

        public bool IsAnyFilterSelected
        {
            get
            {
                if (BlockNumber || ContractAddress || MethodId || From || To)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
