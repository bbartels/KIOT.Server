using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentValidation.Results;

namespace KIOT.Server.Core.Response
{
    public class CommandResponse
    {
        private readonly IList<Error> _errors = new List<Error>();

        public IReadOnlyCollection<Error> Errors => new ReadOnlyCollection<Error>(_errors);
        public bool Succeeded => !_errors.Any();
        public string Message { get; set; }
        public object ObjectResult { get; set; }

        public CommandResponse() { }
        public CommandResponse(string message) { Message = message; }

        public CommandResponse(DataResponse response)
        {
            if (!response.Succeeded) { _errors = response.Errors.ToList(); }
        }

        public CommandResponse(Error error) => _errors = new[] {error};
        public CommandResponse(IEnumerable<Error> errors) => _errors = errors.ToList();
        public CommandResponse(ValidationResult result)
        {
            _errors = result.Errors.Select(x => new Error(x.ErrorCode, x.ErrorMessage)).ToList();
        }

        public void AddError(string code, string description) => _errors.Add(new Error(code, description));
    }

    public class CommandResponse<TModel> : CommandResponse
    {
        public TModel Result { get; }
        public CommandResponse(IEnumerable<Error> errors) : base(errors) { }
        public CommandResponse(TModel model)
        {
            Result = model;
            ObjectResult = model;
        }
    }
}
