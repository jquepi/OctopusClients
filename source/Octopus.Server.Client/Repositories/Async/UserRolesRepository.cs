using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IUserRolesRepository : IFindByName<UserRoleResource>, IGet<UserRoleResource>, ICreate<UserRoleResource>, IModify<UserRoleResource>, IDelete<UserRoleResource>
    {
    }

    class UserRolesRepository : BasicRepository<UserRoleResource>, IUserRolesRepository
    {
        public UserRolesRepository(IOctopusAsyncRepository repository)
            : base(repository, "UserRoles")
        {
            MinimumCompatibleVersion("2019.1.0");
        }

        public override async Task<UserRoleResource> Create(UserRoleResource resource, object pathParameters = null)
        {
            await ThrowIfServerVersionIsNotCompatible();

            await RemoveInvalidPermissions(resource).ConfigureAwait(false);
            return await base.Create(resource, pathParameters).ConfigureAwait(false);
        }

        public override async Task<UserRoleResource> Modify(UserRoleResource resource)
        {
            await ThrowIfServerVersionIsNotCompatible();

            await RemoveInvalidPermissions(resource).ConfigureAwait(false);
            return await base.Modify(resource).ConfigureAwait(false);
        }

#pragma warning disable 618
        static readonly Permission TaskViewLogPermission = Permission.TaskViewLog;
#pragma warning restore 618

        private async Task RemoveInvalidPermissions(UserRoleResource resource)
        {
            var versionWhenTaskViewLogWasRemoved = SemanticVersion.Parse("2019.1.7");
            var rootDocument = await Repository.LoadRootDocument().ConfigureAwait(false);
            var serverSupportsTaskViewLog = SemanticVersion.Parse(rootDocument.Version) < versionWhenTaskViewLogWasRemoved;

            if (!serverSupportsTaskViewLog)
            {
                resource.GrantedSpacePermissions = RemoveDeprecatedPermission(TaskViewLogPermission, resource.GrantedSpacePermissions);
                resource.GrantedSystemPermissions = RemoveDeprecatedPermission(TaskViewLogPermission, resource.GrantedSystemPermissions);
            }

            List<Permission> RemoveDeprecatedPermission(Permission permissionToRemove, List<Permission> permissions)
            {
                return permissions?.Where(p => p != permissionToRemove).ToList();
            }
        }
    }
}
