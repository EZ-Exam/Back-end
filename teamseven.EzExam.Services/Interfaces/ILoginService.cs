using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Object.Requests;

namespace teamseven.EzExam.Services.Interfaces
{
    public interface ILoginService
    {
     Task<(bool IsSuccess, string ResultOrError)> ValidateUserAsync(teamseven.EzExam.Services.Object.Requests.LoginRequest loginRequest);
    }
}
