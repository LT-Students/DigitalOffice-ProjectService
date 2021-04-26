using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    public class EditTaskValidatorTests
    {
        private IValidator<JsonPatchDocument<EditTaskRequest>> _validator;
        private JsonPatchDocument<EditTaskRequest> _editTaskRequest;
        
        private Mock<ITaskPropertyRepository> _taskPropertyRepository;
        private Mock<IUserRepository> _userRepository;
        
        Func<string, Operation> GetOperationByPath =>
            (path) => _editTaskRequest.Operations.Find(x => x.path == path);

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _taskPropertyRepository = new Mock<ITaskPropertyRepository>();
            
            _taskPropertyRepository
                .Setup(x => x.AreExist(It.IsAny<Guid[]>()))
                .Returns(true);

            _userRepository = new Mock<IUserRepository>();

            _userRepository
                .Setup(x => x.AreExist(It.IsAny<Guid[]>()))
                .Returns(true);
            
            _validator = new EditTaskValidator(_taskPropertyRepository.Object, _userRepository.Object);
        }
        
        [SetUp]
        public void SetUp()
        {
            _editTaskRequest = new JsonPatchDocument<EditTaskRequest>(new List<Operation<EditTaskRequest>>()
            {
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.Name)}",
                    "",
                    "NewName"),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.Description)}",
                    "",
                    "New Description"),
                
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.AssignedTo)}",
                    "",
                    Guid.NewGuid()),
                
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.PriorityId)}",
                    "",
                    Guid.NewGuid()),
                
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.StatusId)}",
                    "",
                    Guid.NewGuid()),
                
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.TypeId)}",
                    "",
                    Guid.NewGuid()),
                
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.PlannedMinutes)}",
                    "",
                    1)
            }, new CamelCasePropertyNamesContractResolver());
        }

        [Test]
        public void RightModelValidate()
        {
            _validator.TestValidate(_editTaskRequest).ShouldNotHaveAnyValidationErrors();
        }

        #region Base validate exceptions
        
        [Test]
        public void ExceptionWhenThereAreNotOperations()
        {
            _editTaskRequest.Operations.Clear();
            
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ExceptionWhenOperationsAreNotUnique()
        {
            _editTaskRequest.Operations.Add(_editTaskRequest.Operations.First());

            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ExceptionWhenNotSupportedOperation()
        {
            _editTaskRequest.Operations.Add(new Operation<EditTaskRequest>(
                "remove",
                $"/{nameof(EditTaskRequest.Name)}",
                "",
                "Name"));
            
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }
        
        #endregion

        [Test]
        public void ExceptionWhenNameTooLong()
        {
            GetOperationByPath(EditTaskValidator.Name).value = "".PadLeft(151);
            
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }

        #region Validate values types
        
        [Test]
        public void ExceptionWhenAssignedToIsNotId()
        { 
            GetOperationByPath(EditTaskValidator.AssignedTo).value = 123;
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }
        
        [Test]
        public void ExceptionWhenPriorityIdIsNotId()
        { 
            GetOperationByPath(EditTaskValidator.PriorityId).value = 123;
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ExceptionWhenStatusIdIsNotId()
        { 
            GetOperationByPath(EditTaskValidator.StatusId).value = 123;
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ExceptionWhenTypeIdIsNotId()
        { 
            GetOperationByPath(EditTaskValidator.TypeId).value = 123;
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }

        #endregion
        
        #region Validate null and empty
        
        [Test]
        public void ExceptionWhenNameIsNullOrEmpty()
        {
            GetOperationByPath(EditTaskValidator.Name).value = "";
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
            
            GetOperationByPath(EditTaskValidator.Name).value = null;
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ExceptionWhenDescriptionIsNull()
        {
            GetOperationByPath(EditTaskValidator.Description).value = null;
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }
        
        [Test]
        public void ExceptionWhenAssignedToIsEmpty()
        {
            GetOperationByPath(EditTaskValidator.AssignedTo).value = null;
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }
        
        [Test]
        public void ExceptionWhenPriorityIdIsNull()
        {
            GetOperationByPath(EditTaskValidator.PriorityId).value = null;
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ExceptionWhenStatusIdIsNull()
        {
            GetOperationByPath(EditTaskValidator.StatusId).value = null;
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ExceptionWhenTypeIdIsNull()
        {
            GetOperationByPath(EditTaskValidator.TypeId).value = null;
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }

        #endregion
        
        [Test]
        public void ExceptionWhenPlannedMinutesIsBelowZero()
        {
            GetOperationByPath(EditTaskValidator.PlannedMinutes).value = -1;
            _validator.TestValidate(_editTaskRequest).ShouldHaveAnyValidationError();
        }
    }
}