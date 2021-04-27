using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.RequestsMappers
{
    public class PatchDbTaskMapperTests
    {
        private JsonPatchDocument<EditTaskRequest> _request;
        private JsonPatchDocument<DbTask> _result;

        private IPatchDbTaskMapper _mapper;

        private string _name = "NewName";
        private string _description = "New Description";
        private Guid _assignedTo = Guid.NewGuid();
        private int _plannedMinutes = 60;
        private Guid _priorityId = Guid.NewGuid();
        private Guid _statusId = Guid.NewGuid();
        private Guid _typeId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _mapper = new PatchDbTaskMapper();

            _request = new JsonPatchDocument<EditTaskRequest>(new List<Operation<EditTaskRequest>>()
            {
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.Name)}",
                    "",
                    _name),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.Description)}",
                    "",
                    _description),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.AssignedTo)}",
                    "",
                    _assignedTo),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.PlannedMinutes)}",
                    "",
                    _plannedMinutes),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.PriorityId)}",
                    "",
                    _priorityId),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.StatusId)}",
                    "",
                    _statusId),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.TypeId)}",
                    "",
                    _typeId),
            }, new CamelCasePropertyNamesContractResolver());

            _result = new JsonPatchDocument<DbTask>(new List<Operation<DbTask>>()
            {
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.Name)}",
                    "",
                    _name),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.Description)}",
                    "",
                    _description),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.AssignedTo)}",
                    "",
                    _assignedTo),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.PlannedMinutes)}",
                    "",
                    _plannedMinutes),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.PriorityId)}",
                    "",
                    _priorityId),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.StatusId)}",
                    "",
                    _statusId),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.TypeId)}",
                    "",
                    _typeId)
            }, new CamelCasePropertyNamesContractResolver());
        }

        [Test]
        public void CorrectMapToDbTask()
        {
            SerializerAssert.AreEqual(_result, _mapper.Map(_request));
        }

        [Test]
        public void ThrowExceptionWhenNull()
        {
            _request = null;
            Assert.Throws<BadRequestException>(() => { _mapper.Map(_request); });
        }
    }
}
