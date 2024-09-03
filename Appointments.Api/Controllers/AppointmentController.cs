using Appointments.Application.Dtos.Requests;
using Appointments.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _slotBookingService;

        public AppointmentController(IAppointmentService slotBookingService)
        {
            _slotBookingService = slotBookingService ?? throw new ArgumentNullException(nameof(slotBookingService));
        }

        [Route("GetWeeklyFreeSlots")]
        [HttpGet]
        public async Task<IActionResult> GetWeeklyFreeSlots([FromQuery] GetWeeklyFreeSlotsRequest getWeeklyFreeSlotsRequest)
        {
            return Ok(await _slotBookingService.GetWeeklyFreeSlots(getWeeklyFreeSlotsRequest.DesiredDate));
        }

        [Route("TakeSlotByUser")]
        [HttpPost]
        public async Task<IActionResult> TakeAppointmentByUser([FromBody] TakeAppointmentByUserRequest appointmentRequest)
        {
            await _slotBookingService.TakeAppointmentByUser(appointmentRequest);
            return Ok();
        }
    }
}


