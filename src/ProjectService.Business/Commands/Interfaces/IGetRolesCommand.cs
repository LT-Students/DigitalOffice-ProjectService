using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting role models.
    /// </summary>
    public interface IGetRolesCommand
    {
        /// <summary>
        /// Returns role models.
        /// </summary>
        /// <param name="skip">First number of roles to skip.</param>
        /// <param name="take">Number of roles to take.</param>
        /// <returns>Role models.</returns>
        RolesResponse Execute(int skip, int take);
    }
}
