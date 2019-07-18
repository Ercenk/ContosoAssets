namespace ContosoAssets.SolutionManagement.Provisioning
{
    public class OperationResult
    {
        public OperationResult()
        {
            this.Success = true;
            this.Message = "Blank";
        }

        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
