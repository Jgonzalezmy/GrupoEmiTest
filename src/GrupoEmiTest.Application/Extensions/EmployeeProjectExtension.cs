using GrupoEmiTest.Application.DTOs.Response;
using GrupoEmiTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrupoEmiTest.Application.Extensions
{
    public static class EmployeeProjectExtension
    {
        public static EmployeeProjectResponse ToResponse(this EmployeeProject request)
        {
            return new EmployeeProjectResponse(
                Name: request.Project.Name,
                Description: request.Project.Description
            );
        }

    }
}
