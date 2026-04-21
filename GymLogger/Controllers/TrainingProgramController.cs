using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymLogger.Controllers;

[Authorize]
public class TrainingProgramController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
