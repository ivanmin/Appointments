using Appointments.Api.Controllers;
using Appointments.Application.Dtos.Requests;
using Appointments.Application.Dtos.Responses;
using Appointments.Application.Services.Interfaces;
using Appointments.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Appointments.UnitTests
{
	public class AppointmentControllerTests
	{
        private readonly AppointmentController _controller;
        private readonly Mock<IAppointmentService> _mockSlotAppointmentService;
        private readonly TakeAppointmentByUserRequest _appointmentRequest;

        public AppointmentControllerTests()
        {
            _mockSlotAppointmentService = new Mock<IAppointmentService>();

            _controller = new AppointmentController(_mockSlotAppointmentService.Object);

            _appointmentRequest = new TakeAppointmentByUserRequest()
            {
                Comments = "",
                FacilityId = Guid.Empty,
                End = DateTime.Today,
                Start = DateTime.Today,
                Patient = new Patient
                {
                    Name = "",
                    SecondName = "",
                    Email = "",
                    Phone = ""
                }
            };
        }

        [Fact]
        public async Task GetWeeklyFreeSlots_DateEarlierThanNow_ReturnsBadRequest()
        {
            // Arrange
            GetWeeklyFreeSlotsRequest getWeeklyFreeSlotsRequestWithPastDate = new()
            {
                DesiredDate = DateTime.Now.AddDays(-1)
            };

            // Act
            var result = await _controller.GetWeeklyFreeSlots(getWeeklyFreeSlotsRequestWithPastDate);

            // Assert
            Assert.Equal(400, (result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task GetWeeklyFreeSlots_DateLaterThanAllowed_ReturnsBadRequest()
        {
            // Arrange
            GetWeeklyFreeSlotsRequest getWeeklyFreeSlotsRequestWithFutureDate = new()
            {
                DesiredDate = DateTime.Now.AddMonths(9)
            };

            // Act
            var result = await _controller.GetWeeklyFreeSlots(getWeeklyFreeSlotsRequestWithFutureDate);

            // Assert
            Assert.Equal(400, (result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task GetWeeklyFreeSlots_ServiceThrowsException_ReturnsProblem()
        {
            // Arrange
            GetWeeklyFreeSlotsRequest getWeeklyFreeSlotsRequestWithValidDate = new()
            {
                DesiredDate = DateTime.Now.AddMonths(2)
            };
            _mockSlotAppointmentService.Setup(service => service.GetWeeklyFreeSlots(getWeeklyFreeSlotsRequestWithValidDate.DesiredDate))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetWeeklyFreeSlots(getWeeklyFreeSlotsRequestWithValidDate);

            // Assert
            Assert.Equal(500, (result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task GetWeeklyFreeSlots_ValidDate_ReturnsOk()
        {
            // Arrange
            GetWeeklyFreeSlotsRequest getWeeklyFreeSlotsRequestWithValidDate = new()
            {
                DesiredDate = DateTime.Now.AddMonths(2)
            };
            _mockSlotAppointmentService.Setup(service => service.GetWeeklyFreeSlots(getWeeklyFreeSlotsRequestWithValidDate.DesiredDate))
                .ReturnsAsync(new ScheduleResponse());

            // Act
            var result = await _controller.GetWeeklyFreeSlots(getWeeklyFreeSlotsRequestWithValidDate);

            // Assert
            Assert.Equal(200, (result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task TakeAppointmentByUser_NullAppointmentRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.TakeAppointmentByUser(null!);

            // Assert
            Assert.Equal(400, (result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task TakeAppointmentByUser_ServiceThrowsException_ReturnsProblem()
        {
            // Arrange
            var appointmentRequest = _appointmentRequest;
            _mockSlotAppointmentService.Setup(service => service.TakeAppointmentByUser(appointmentRequest))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.TakeAppointmentByUser(appointmentRequest);

            // Assert
            Assert.Equal(500, (result as ObjectResult)?.StatusCode);
        }

        [Fact]
        public async Task TakeAppointmentByUser_ValidAppointmentRequest_ReturnsOk()
        {
            // Arrange
            var appointmentRequest = _appointmentRequest;

            _mockSlotAppointmentService.Setup(service => service.TakeAppointmentByUser(appointmentRequest))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.TakeAppointmentByUser(appointmentRequest);

            // Assert
            Assert.IsType<OkResult>(result);
            var okResult = (OkResult)result;
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}