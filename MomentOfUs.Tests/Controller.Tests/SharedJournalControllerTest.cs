using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MomentOfUs.API.Controllers;
using MomentOfUs.Application.Dtos.SharedJournalDto;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.Domain.Exceptions;
using MomentOfUs.Domain.Models;
using Moq;
using NUnit.Framework;

namespace MomentOfUs.Tests.Controller.Tests
{
    public class SharedJournalControllerTest
    {
        private readonly Mock<IServiceManager> _serviceManagerMock ;
        private readonly SharedJournalController _controller;

        public SharedJournalControllerTest()
        {
            _serviceManagerMock=new Mock<IServiceManager>();
            _controller = new SharedJournalController(_serviceManagerMock.Object);
        }
        ///Helper methof to mock user authentication in the controller
        private void MockUserAuthentication(string userId)
        {
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(userClaims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
                
        }
                /// <summary>
        /// ✅ Test: Share a journal successfully
        /// </summary>
        /// 
        [Test]
        public async Task ShareJournal_Should_ReturnOk_When_ValidRequest()
        {
            //Arrange
            var userId = "testUserId";
            var journalId = Guid.NewGuid();
            var targetUserId = "targetUserId";
            var permission = PermissionLevel.Edit;

            MockUserAuthentication(userId);
            _serviceManagerMock.Setup(sm => sm.JournalService.ShareJournalAsync(journalId, userId, targetUserId, permission))
                .Returns(Task.CompletedTask);

            //Act
            var result = await _controller.ShareJournal(journalId, new SharedJournalCreateDto(journalId, permission, targetUserId));
            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            _serviceManagerMock.Verify(sm => sm.JournalService.ShareJournalAsync(journalId, userId, targetUserId, permission), Times.Once);
            
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        }
        /// <summary>
        /// ✅ Test: Share a journal with invalid request
        /// </summary>
        [Test]
        public async Task ShareJournal_Should_ReturnBadRequest_When_InvalidRequest()
        {   
            //Arrange
            var userId = "testUserId";
            var journalId = Guid.NewGuid();
            var targetUserId = "targetUserId";
            var permission = PermissionLevel.Edit;

            MockUserAuthentication(userId);
            _serviceManagerMock.Setup(sm => sm.JournalService.ShareJournalAsync(journalId, userId, targetUserId, permission))
                .ThrowsAsync(new BadRequestException("Invalid request"));

            //Act
            var result = await _controller.ShareJournal(journalId, new SharedJournalCreateDto(journalId, permission, targetUserId));
            
            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }
        /// <summary>
        /// ✅ Test: Update user permission successfully
        /// </summary>
        [Test]
        public async Task UpdatePermission_Should_ReturnOk_When_ValidRequest()
        {
            //Arrange
            var userId = "testUserId";
            var journalId = Guid.NewGuid();
            var targetUserId = "targetUserId";
            var permission = PermissionLevel.Edit;

            MockUserAuthentication(userId);
            _serviceManagerMock.Setup(sm => sm.JournalService.UpdateUserPermissionAsync(journalId, userId, targetUserId, permission))
                .Returns(Task.CompletedTask);

            //Act
            var result = await _controller.UpdatePermission(journalId, new SharedJournalUpdated(permission, targetUserId));
            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            _serviceManagerMock.Verify(sm => sm.JournalService.UpdateUserPermissionAsync(journalId, userId, targetUserId, permission), Times.Once);

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }
            /// <summary>
        /// ✅ Test: Update user permission with invalid request
        /// </summary>
        [Test]
        public async Task UpdatePermission_Should_ReturnBadRequest_When_InvalidRequest()
        {
            //Arrange
            var userId = "testUserId";
            var journalId = Guid.NewGuid();
            var targetUserId = "targetUserId";
            var permission = PermissionLevel.Edit;

            MockUserAuthentication(userId);
            _serviceManagerMock.Setup(sm => sm.JournalService.UpdateUserPermissionAsync(journalId, userId, targetUserId, permission))
                .ThrowsAsync(new BadRequestException("Invalid request"));

            //Act
            var result = await _controller.UpdatePermission(journalId, new SharedJournalUpdated(permission, targetUserId));
            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }
        /// <summary>
        /// ✅ Test: Revoke user access successfully
        /// </summary>
        [Test]
        public async Task RevokeAccess_Should_ReturnNoContent_When_ValidRequest()
        {
            //Arrange
            var userId = "testUserId";
            var journalId = Guid.NewGuid();
            var targetUserId = "targetUserId";  

            MockUserAuthentication(userId);
            _serviceManagerMock.Setup(sm => sm.JournalService.RevokeUserAccessAsync(journalId, userId, targetUserId))
                .Returns(Task.CompletedTask);

            //Act
            var result = await _controller.RevokeAccess(journalId, targetUserId);
            //Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _serviceManagerMock.Verify(sm => sm.JournalService.RevokeUserAccessAsync(journalId, userId, targetUserId), Times.Once); 

            var noContentResult = result as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
        }
        /// <summary>
        /// ✅ Test: Revoke user access with invalid request
        /// </summary>
        [Test]
        public async Task RevokeAccess_Should_ReturnBadRequest_When_InvalidRequest()
        {
            //Arrange
            var userId = "testUserId";
            var journalId = Guid.NewGuid();
            var targetUserId = "targetUserId";

            MockUserAuthentication(userId); 
            _serviceManagerMock.Setup(sm => sm.JournalService.RevokeUserAccessAsync(journalId, userId, targetUserId))
                .ThrowsAsync(new BadRequestException("Invalid request"));

            //Act
            var result = await _controller.RevokeAccess(journalId, targetUserId);   
            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }
        /// <summary>
        /// ✅ Test: Get all users with access to a shared journal successfully
        /// </summary>
        [Test]
        public async Task GetAllUsersWithAccess_Should_ReturnOk_When_ValidRequest()
        {
            //Arrange
            var userId = "testUserId";
            var journalId = Guid.NewGuid();
            var targetUserId = "targetUserId";

            MockUserAuthentication(userId);
            _serviceManagerMock.Setup(sm => sm.JournalService.GetSharedUsersAsync(userId))
                .ReturnsAsync(new List<UserSharedJournal> 
                { 
                    new UserSharedJournal 
                    { 
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        SharedJournalId = journalId,
                        PermissionLevel = PermissionLevel.Edit 
                    }
                });

            //Act
            var result = await _controller.GetAllUsersWithAccess();
            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }
        /// <summary>
        /// ✅ Test: Get all users with access to shared journals with invalid request
        /// </summary>
        [Test]
        public async Task GetAllUsersWithAccess_Should_ReturnBadRequest_When_InvalidRequest()
        {
            //Arrange
            var userId = "testUserId";

            MockUserAuthentication(userId);
            _serviceManagerMock.Setup(sm => sm.JournalService.GetSharedUsersAsync(userId))
                .ThrowsAsync(new BadRequestException("Invalid request"));

            //Act
            var result = await _controller.GetAllUsersWithAccess(); 

            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }
        /// <summary>
        /// ✅ Test: Get user access to a shared journal successfully
        /// </summary>
        [Test]
        public async Task GetUserAccess_Should_ReturnOk_When_ValidRequest()
        {
            //Arrange
            var userId = "testUserId";
            var journalId = Guid.NewGuid();
            var targetUserId = "targetUserId";

            MockUserAuthentication(userId);
            _serviceManagerMock.Setup(sm => sm.JournalService.GetSharedUsersAsync(userId))
                .ReturnsAsync(new List<UserSharedJournal> 
                { 
                    new UserSharedJournal 
                    { 
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        SharedJournalId = journalId,
                        PermissionLevel = PermissionLevel.Edit 
                    }
                });

            //Act
            var result = await _controller.GetAllUsersWithAccess();      
            //Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }
        /// <summary>
        /// ✅ Test: Get user access to a shared journal with invalid request
        /// </summary>
        [Test]
        public async Task GetUserAccess_Should_ReturnBadRequest_When_InvalidRequest()
        {
            //Arrange
            var userId = "testUserId";
            var journalId = Guid.NewGuid();
            var targetUserId = "targetUserId";

            MockUserAuthentication(userId);
            _serviceManagerMock.Setup(sm => sm.JournalService.GetSharedUsersAsync(userId))
                .ThrowsAsync(new BadRequestException("Invalid user request."));

            //Act
            var result = await _controller.GetAllUsersWithAccess();
            //Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }           
        
    }
}