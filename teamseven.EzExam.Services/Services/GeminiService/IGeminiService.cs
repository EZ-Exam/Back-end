using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Services.GeminiService
{
    internal interface IGeminiService
    {
        Task<string> AskGeminiAsync(string prompt);
    }
}
