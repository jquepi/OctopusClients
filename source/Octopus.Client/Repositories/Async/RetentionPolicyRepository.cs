using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface IRetentionPolicyRepository
    {
        Task<TaskResource> ApplyNow(string spaceId = null);
    }

    class RetentionPolicyRepository : BasicRepository<RetentionPolicyResource>, IRetentionPolicyRepository
    {
        public RetentionPolicyRepository(IOctopusAsyncClient client)
            : base(client, "RetentionPolicies")
        {
        }

        public Task<TaskResource> ApplyNow(string spaceId = null)
        {
            var tasks = new TaskRepository(Client);
            var task = new TaskResource { Name = "Retention", Description = "Request to apply retention policies via the API", SpaceId = spaceId};
            return tasks.Create(task);
        }
    }
}
