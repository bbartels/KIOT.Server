using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace KIOT.Server.Core.Response
{
    public class DataResponse
    {
        private readonly IList<Error> _errors = new List<Error>();

        public IReadOnlyCollection<Error> Errors => new ReadOnlyCollection<Error>(_errors);
        public bool Succeeded => !_errors.Any();

        public DataResponse() { }
        public DataResponse(Error error) => _errors.Add(error);
        public DataResponse(IList<Error> errors) => _errors = errors;
        public DataResponse(IEnumerable<Error> errors) => _errors = errors.ToList();

        public void AddError(string code, string description)
        {
            _errors.Add(new Error(code, description));
        }
    }

    public class DataResponse<TEntity> : DataResponse
    {
        public TEntity Entity { get; }

        public DataResponse(TEntity entity) => Entity = entity;

        public DataResponse(TEntity entity, Error error) : base(new [] { error })
        {
            Entity = entity;
        }
    }
}
