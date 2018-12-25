using Pharmacy.Models.Scripts;
using System.Threading.Tasks;

namespace Pharmacy.Services.ScriptService
{
    public interface IScriptService
    {
        Task RequestNewScript(RequestScriptModel model);
        Task RespondToScript(RespondToScriptModel model);
    }
}