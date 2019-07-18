using System.Collections.Generic;
using System.Linq;

namespace ContosoAssets.SolutionManagement.Data
{
    public class SolutionManagerOperationResult
    {
        private readonly List<SolutionManagementError> errors = new List<SolutionManagementError>();

        //
        // Summary:
        //     Returns a CustomerUserResult indicating a successful customer user operation.
        //
        // Returns:
        //     An CustomerUserResult indicating a successful customer user operation.
        public static SolutionManagerOperationResult Success => new SolutionManagerOperationResult {Succeeded = true};

        //
        // Summary:
        //     An System.Collections.Generic.IEnumerable`1 of CustomerUserResultError
        //     containing an errors that occurred during the customer user operation.
        public IEnumerable<SolutionManagementError> Errors => this.errors;

        public bool Succeeded { get; private set; }
        //
        // Summary:
        //     Creates an CustomerUserResult indicating a failed customer user
        //     operation, with a list of errors if applicable.
        //
        // Parameters:
        //   errors:
        //     An optional array of CustomerUserResultError which caused
        //     the operation to fail.
        //
        // Returns:
        //     An CustomerUserResult indicating a failed customer user
        //     operation, with a list of errors if applicable.

        public static SolutionManagerOperationResult Failed(params SolutionManagementError[] errors)
        {
            var result = new SolutionManagerOperationResult {Succeeded = false};
            if (errors != null)
            {
                result.errors.AddRange(errors);
            }

            return result;
        }

        //
        // Summary:
        //     Converts the value of the current CustomerUserResultError
        //     object to its equivalent string representation.
        //
        // Returns:
        //     A string representation of the current CustomerUserResultError
        //     object.
        //
        // Remarks:
        //     If the operation was successful the ToString() will return "Succeeded" otherwise
        //     it returned "Failed : " followed by a comma delimited list of error codes from
        //     its CustomerUserResultError collection, if any.
        public override string ToString()
        {
            return this.Succeeded
                ? "Succeeded"
                : string.Format("{0} : {1}", "Failed", string.Join(",", this.Errors.Select(x => x.Code).ToList()));
        }
    }
}
