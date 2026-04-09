using GrupoEmiTest.Domain.Enums;

namespace GrupoEmiTest.Domain.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="PositionType"/>.
    /// </summary>
    public static class PositionTypeExtensions
    {
        /// <summary>
        /// Determines whether the given position type is a managerial role.
        /// Managerial roles start at <see cref="PositionType.TeamLead"/>.
        /// </summary>
        /// <param name="position">The position type to evaluate.</param>
        /// <returns><c>true</c> if the position is managerial; otherwise, <c>false</c>.</returns>
        public static bool IsManagerPosition(this PositionType position)
            => position > PositionType.RegularEmployee;
    }
}
