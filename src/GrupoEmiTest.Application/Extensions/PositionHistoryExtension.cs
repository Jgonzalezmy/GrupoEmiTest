using GrupoEmiTest.Application.DTOs.Response;
using GrupoEmiTest.Domain.Entities;

namespace GrupoEmiTest.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for mapping <see cref="PositionHistory"/> entities
    /// to their corresponding response DTOs.
    /// </summary>
    public static class PositionHistoryExtension
    {
        /// <summary>
        /// Maps a <see cref="PositionHistory"/> entity to a <see cref="PositionHistoryResponse"/> DTO.
        /// </summary>
        /// <param name="request">The <see cref="PositionHistory"/> entity to map. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="PositionHistoryResponse"/> populated with the entity's data.</returns>
        public static PositionHistoryResponse ToResponse(this PositionHistory request)
        {
            return new PositionHistoryResponse(
                Position: request.Position,
                StartDate: request.StartDate,
                EndDate: request.EndDate
                );
        }

    }
}
